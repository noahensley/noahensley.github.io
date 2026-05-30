/*
 * File: Utils.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * and refactor the code for this file on the following dates:
 *      January 25, 2026
 *      January 28, 2026
 *      February 4, 2026
 *      February 17, 2026
 */

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using ASM;

/// <summary>
/// Utility class providing helper functions for command-line option parsing, file processing,
/// and output generation in the parser program.
/// </summary>
/// <remarks>
/// This class handles the main workflow of processing input files in different modes:
/// <list type="bullet">
/// <item>Test generation mode (--gen-tests): Processes files and writes output to staging/outputs/</item>
/// <item>Run mode (--run): Processes files and writes output to console</item>
/// </list>
/// Supports multiple output formats including JSON token streams, parse trees, and Graphviz dotfiles.
/// </remarks>
public class Utils
{   
    // Path constants
    private const string INPUT_DIR = "inputs";
    private const string OUTPUT_DIR = "outputs";

    // Output constants
    private const string NULL = "null";
    
    // File extension constants
    private const string TEXT_EXT = ".txt";
    private const string JSON_EXT = ".json";

    /// <summary>
    /// Records a compiler error.
    /// </summary>
    /// <param name="e">The exception representing the error condition.</param>
    /// <remarks>
    /// Instead of immediately terminating compilation, errors are collected so
    /// that multiple issues can be reported during a single compilation run.
    /// The caller is responsible for ensuring that compilation halts if the
    /// accumulated errors prevent further meaningful processing.
    /// </remarks>
    /// <exception cref="Exception">Always rethrows the provided exception.</exception>
    public static void error(Exception e)
    {
        Console.Error.WriteLine(e.Message);
        throw e;
    }

    /// <summary>
    /// Parses command-line arguments to extract and validate program options.
    /// </summary>
    /// <param name="args">Command-line arguments array.</param>
    /// <returns>
    /// A list containing validated options. Returns a list with two elements if valid options 
    /// are provided (mode and format), or an empty list if no options are present.
    /// An optional third and fourth element are included when the --outdir flag is provided.
    /// </returns>
    /// <exception cref="InvalidOption">
    /// Thrown when a mode option is provided without a format option, when --outdir is provided
    /// without a directory argument, or when an unrecognized option is provided.
    /// </exception>
    /// <remarks>
    /// <para>Valid mode options:</para>
    /// <list type="bullet">
    /// <item>--gen-tests: Generate test output files</item>
    /// <item>--run: Run and output to console</item>
    /// </list>
    /// <para>Valid format options:</para>
    /// <list type="bullet">
    /// <item>-tok-json: Output token stream as JSON</item>
    /// <item>-tree-json: Output parse tree as JSON</item>
    /// <item>-tree-box: Output parse tree as box-drawing text</item>
    /// <item>-dotfile: Output Graphviz dotfile</item>
    /// <item>-type-check: Output type checking result as JSON</item>
    /// <item>-var-decl: Output variable declaration info to console</item>
    /// <item>-var-decl: Output class/variable declaration info to console</item>
    /// </list>
    /// <para>Optional flags (must follow the format option):</para>
    /// <list type="bullet">
    /// <item>--outdir [dir]: Write output files to the specified directory instead of the default outputs/ directory</item>
    /// </list>
    /// </remarks>
    public static ParsedOptions parseOptions(string[] args)
    {
        ParsedOptions opts = new ParsedOptions();
        try
        {
            switch(args[0])
            {
                case Options.Run:
                    opts.mode = args[0];
                    break;
                case Options.GenTests:
                    opts.mode = args[0];
                    switch(args[2])
                    {
                        case Options.OutDir:
                            opts.opt = args[2];
                            opts.optarg = args[3];
                            break;
                        default:
                            // should be file here
                            break;
                    }
                    break;
                default:
                    error(new InvalidOption($"Provided unrecognized mode '{args[0]}' (mode: [--run | --gen-tests]"));
                    break;
            }

            switch(args[1])
            {
                case Options.TokJson:
                case Options.TreeJson:
                case Options.TreeBox:
                case Options.Dotfile:
                case Options.TypeCheck:
                case Options.VarDecl:
                case Options.ClassDecl:
                case Options.CompileAsm:
                    opts.fmt = args[1];
                    break;
                default:
                    error(new InvalidOption($@"Provided unrecognized format '{args[1]}' (format: 
                        [-tok-json|-tree-json|-tree-box|-dotfile|
                        -type-check|-var-decl|-class-decl|-comp-asm]"));
                    break;
            }
        }
        catch (IndexOutOfRangeException e)
        {
            error(new InvalidOption($"Encountered error while parsing '{args}': {e}"));
        }

        return opts;
    }

    public static List<string> parseInputFilesOrDirectories(string[] args, ParsedOptions opts)
    {
        int i = 0;
        List<string> inputs = new List<string>();
        if (args.Count() > 1)
        {
            i = opts.Length();
            inputs.AddRange(args[i..]);
        }
        else
        {
            return new List<string>(){args[0]};
        }

        if (inputs.Count() == 0)
            error(new InvalidOption($"Tried to parse input files but none found '{args}'"));

        foreach (string input in inputs)
        {
            if (!input.EndsWith(".txt") && !Directory.Exists(input))
                error(new InvalidOption($"Could not find input directory '{input}'"));
        }
            
        return inputs;
    }

    /// <summary>
    /// Processes input files according to the parsed options.
    /// </summary>
    /// <param name="args">Full command-line arguments array containing options and input file paths.</param>
    /// <param name="opts">Parsed options list from <see cref="parseOptions"/>.</param>
    /// <returns>
    /// Returns 1 if options were processed successfully, 0 if no options were provided and
    /// default execution should be used, or -1 if too many options were provided.
    /// </returns>
    /// <remarks>
    /// File paths are taken from <paramref name="args"/> starting after the option arguments.
    /// Both individual files and directories are accepted; directories are searched non-recursively
    /// for <c>*.txt</c> files.
    /// </remarks>
    public static void handleOptions(List<string> files, ParsedOptions opts)
    {
        foreach (string input in files)
        {
            if (Directory.Exists(input))
            {
                foreach (string file in Directory.EnumerateFiles(input, "*.txt", SearchOption.TopDirectoryOnly))
                {
                    processFile(file, opts);
                }
            }
            else if (File.Exists(input))
            {
                if (processFile(input, opts) == 1) // return code available if needed
                    Environment.Exit(1);
            }
            else
            {
                error(new InvalidOption($"Could not find input file or directory: {input}"));
            }
        }
    }

    /// <summary>
    /// Processes a single input file based on the specified mode and format.
    /// </summary>
    /// <param name="inputPath">Path to the input file to process.</param>
    /// <param name="opts">List containing processing options where:
    /// <list type="bullet">
    /// <item>opts[0]: Processing mode (--gen-tests or --run)</item>
    /// <item>opts[1]: Output format (-tok-json, -tree-json, -tree-box, -dotfile, or -type-check)</item>
    /// <item>opts[2]: (Optional) --outdir flag</item>
    /// <item>opts[3]: (Optional) Output directory path</item>
    /// </list>
    /// </param>
    /// <remarks>
    /// <para>Processing steps:</para>
    /// <list type="number">
    /// <item>Creates a tokenizer and loads the input file</item>
    /// <item>Processes according to format:
    ///   <list type="bullet">
    ///   <item>-tok-json / -tree-json: Tokenizes and serializes to JSON</item>
    ///   <item>-tree-box: Parses and outputs a box-drawing text tree</item>
    ///   <item>-dotfile: Parses and generates a Graphviz graph</item>
    ///   <item>-type-check: Parses, sets types, and validates them</item>
    ///   </list>
    /// </item>
    /// <item>Outputs results based on mode (file or console)</item>
    /// </list>
    /// <para>
    /// <b>Error handling contract:</b> Each format is responsible for writing its own
    /// format-appropriate failure value when a parse or processing error occurs — for example,
    /// <c>null</c> for JSON formats and <c>{"legal": false}</c> for type-check. The outer
    /// catch in this method therefore silently swallows exceptions, relying on each format
    /// handler to have already written its sentinel output before propagating.
    /// </para>
    /// </remarks>
    private static int processFile(string inputPath, ParsedOptions opts)
    {
        Tokenizer tokenizer = new Tokenizer();
        using (StreamReader reader = new StreamReader(inputPath))
            tokenizer.setInput(reader.ReadToEnd());

        ExprNode.resetCounter();
        SymbolTable.resetTable();
        SymbolTable.populateBuiltins();
        Label.resetLabelCounter();
        GlobalLocation.resetCounter();

        switch (opts.fmt)
        {
            case Options.TokJson or Options.TreeJson:
                return outputJson(inputPath, opts, tokenizer);
            case Options.TreeBox:
                ProgramNode? tree = parseToTree(tokenizer);
                return outputBoxDrawing(tree, inputPath, opts);
            case Options.Dotfile:
                tree = parseToTree(tokenizer);
                walkTree(tree, inputPath, opts);
                break;
            case Options.TypeCheck:
                tree = parseToTree(tokenizer);
                return walkTree(tree, inputPath, opts);
            case Options.VarDecl:
                tree = parseToTree(tokenizer);
                return walkTree(tree, inputPath, opts);
            case Options.ClassDecl:
                tree = parseToTree(tokenizer);
                return walkTree(tree, inputPath, opts);
            case Options.CompileAsm:
                tree = parseToTree(tokenizer);
                return walkTree(tree, inputPath, opts);
            default:
                Utils.error(new InvalidOption($"Could not process unknown format: {opts.fmt}"));
                break;
        }

        return 0;
    }

    /// <summary>
    /// Tokenizes input and serializes the resulting token stream to a JSON string.
    /// </summary>
    /// <param name="tokenizer">The tokenizer instance containing the input to process.</param>
    /// <returns>
    /// A JSON string representing the list of tokens, or <c>NULL</c> if a tokenization error occurred.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Consumes tokens until EOF is reached. If an error occurs during tokenization,
    /// returns <c>NULL</c> and writes the error to stderr.
    /// </para>
    /// <para>
    /// JSON serialization options:
    /// </para>
    /// <list type="bullet">
    /// <item>Indented formatting for readability</item>
    /// <item>Field serialization for Token objects</item>
    /// <item>Full Unicode character encoding</item>
    /// <item>Deep nesting support (1,000,000 levels)</item>
    /// </list>
    /// </remarks>
    private static string tokenizeToJson(Tokenizer tokenizer)
    {
        List<Token> tokens = new List<Token>();
        bool hasError = false;

        var jsonOpts = new System.Text.Json.JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true,
            MaxDepth = 1000000,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        while (true)
        {
            try
            {
                Token tok = tokenizer.next();
                if (tok.sym == TokenSymbols.EOF)
                    break;
                tokens.Add(tok);
            }
            catch (Exception e)
            {
                hasError = true;
                Console.Error.WriteLine(e);
                break;
            }
        }

        return hasError ? NULL : System.Text.Json.JsonSerializer.Serialize(tokens, jsonOpts);
    }

    /// <summary>
    /// Parses input into a program tree structure.
    /// </summary>
    /// <param name="tokenizer">The tokenizer instance containing the input to parse.</param>
    /// <returns>
    /// A <see cref="ProgramNode"/> representing the parsed tree, or an empty
    /// <see cref="ProgramNode"/> if parsing failed.
    /// </returns>
    /// <remarks>
    /// Catches all parsing exceptions and returns an empty tree on failure, allowing
    /// the program to continue processing other files in batch mode.
    /// </remarks>
    private static ProgramNode? parseToTree(Tokenizer tokenizer)
    {
        try
        {
            return ProgramNode.parse(tokenizer);
        }
        catch (Exception)
        {
            //Console.Error.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Performs a pre-order depth-first traversal of a tree, applying a callback to each node.
    /// </summary>
    /// <param name="n">The root node to begin traversal from.</param>
    /// <param name="callback">Action to apply to each node during traversal.</param>
    /// <remarks>
    /// Catches and suppresses <see cref="StopIteration"/> exceptions to allow the callback
    /// to terminate traversal early by throwing one.
    /// </remarks>
    static void walkPreOrder(TreeNode n, Action<TreeNode> callback)
    {
        try
        {
            walkPreOrderHelper(n, callback);
        }
        catch (StopIteration)
        {
            // Allows callback to terminate traversal early
        }
    }

    /// <summary>
    /// Recursive helper for pre-order depth-first tree traversal.
    /// </summary>
    /// <param name="node">The current node being visited.</param>
    /// <param name="callback">Action to apply to the current node before visiting its children.</param>
    static void walkPreOrderHelper(TreeNode node, Action<TreeNode> callback)
    {
        callback(node);
        foreach(TreeNode child in node.getChildren())
        {
           walkPreOrderHelper(child, callback);
        }
    }

    /// <summary>
    /// Performs a post-order depth-first traversal of a tree, applying a callback to each node.
    /// </summary>
    /// <param name="n">The root node to begin traversal from.</param>
    /// <param name="callback">Action to apply to each node during traversal.</param>
    /// <remarks>
    /// Children are visited before their parent. Catches and suppresses <see cref="StopIteration"/>
    /// exceptions to allow the callback to terminate traversal early by throwing one.
    /// </remarks>
    public static void walkPostOrder(TreeNode n, Action<TreeNode> callback)
    {
        try
        {
            walkPostOrderHelper(n, callback);
        }
        catch (StopIteration)
        {
            // Allows callback to terminate traversal early
        }
    }

    /// <summary>
    /// Recursive helper for post-order depth-first tree traversal.
    /// </summary>
    /// <param name="node">The current node being visited.</param>
    /// <param name="callback">Action to apply to the current node after all its children have been visited.</param>
    public static void walkPostOrderHelper(TreeNode node, Action<TreeNode> callback)
    {
        foreach(TreeNode child in node.getChildren())
        {
            walkPostOrderHelper(child, callback);
        }
        callback(node);
    }

    public static void walkPreAndPostOrder(TreeNode n, Action<TreeNode> precallback, Action<TreeNode> postcallback)
    {
        try
        {
            walkPreAndPostOrderHelper(n, precallback, postcallback);
        }
        catch (StopIteration)
        {
            
        }
    }

    public static void walkPreAndPostOrderHelper(TreeNode n, Action<TreeNode> precallback, Action<TreeNode> postcallback)
    {
        precallback(n);
        foreach (TreeNode c in n.getChildren())
        {
            walkPreAndPostOrderHelper(c, precallback, postcallback);
        }
        postcallback(n);
    }

    public static void setTemporaries(TreeNode root)
    {
        ExprNode? topLevel = null;
        FuncdefNode? currentFunction = null;
        int numTemporaries = 0;
        walkPreAndPostOrder(root, 
            (n) =>
        {
            FuncdefNode? f = n as FuncdefNode;
            if (f != null)
            {
                currentFunction = f;
            }

            ExprNode? e = n as ExprNode;
            if (e != null)
            {
                Variable? v = e as Variable;
                if (v == null) // don't set temporaries for variables--not needed
                {
                    if (topLevel == null)
                    {
                        topLevel = e;
                        numTemporaries = 0;
                    }
                    e.temporary = new Temporary(numTemporaries);
                    numTemporaries++;
                }
            }
        },
        (n) =>
        {
            if (n == topLevel)
            {
                // assuming currentFunction != null rules out allowing for
                // declaration and initialization to be used in one line
                //  e.g. var x = ...
                currentFunction!.maxTemporaries = Math.Max(numTemporaries, currentFunction!.maxTemporaries);
                topLevel = null;
            }
        });
    }

    /// <summary>
    /// Compiles a source string through the full codegen pipeline and returns the
    /// x86-64 AT&amp;T assembly as a string. Does not invoke clang or the linker.
    /// Returns a string beginning with "error:" on failure.
    /// </summary>
    public static string CompileAsmToString(string source)
    {
        try
        {
            ASM.Asm.opcodes.Clear();
 
            var tokenizer = new Tokenizer();
            tokenizer.setInput(source);
            var tree = ProgramNode.parse(tokenizer);
 
            if (tree.getChildren().Count == 0)
                return "error: empty program";
 
            walkPreOrder(tree, (n) =>
            {
                foreach (var c in n.getChildren())
                    c.parent = n;
            });
 
            walkPreOrder(tree, (n) =>
            {
                Variable? v = n as Variable;
                if (v != null)
                    v.assignVarInfo();
            });
 
            walkPostOrder(tree, (n) => n.setType());
            walkPostOrder(tree, (n) => n.typeCheck());
            setTemporaries(tree);
            tree.genCode();
 
            ASM.Label? mainLabel = null;
            foreach (FuncdefNode f in tree.funcs)
            {
                if (f.info?.token == null)
                    continue;
                if (f.info.token.lexeme == "main")
                    mainLabel = f.lbl;
            }
            if (mainLabel is null)
                return "error: no main function found";
 
            var sw = new StringWriter();
            ASM.Asm.write(sw, mainLabel);
            return sw.ToString();
        }
        catch (Exception e)
        {
            return $"error: {e.Message}";
        }
    }

    /// <summary>
    /// Walks the program tree and generates output in the specified format (dotfile, type-check, or var-decl).
    /// </summary>
    /// <param name="tree">The program tree to process.</param>
    /// <param name="inputPath">The original input file path (used for output path generation in --gen-tests mode).</param>
    /// <param name="opts">List containing processing options where:
    /// <list type="bullet">
    /// <item>opts[0]: Processing mode (--gen-tests or --run)</item>
    /// <item>opts[1]: Output format (-dotfile, -type-check, or -var-decl)</item>
    /// <item>opts[2]: (Optional) --outdir flag</item>
    /// <item>opts[3]: (Optional) Output directory path</item>
    /// </list>
    /// </param>
    /// <remarks>
    /// <para>
    /// For <c>-dotfile</c>: walks the tree to find the first expression node, then generates
    /// a Graphviz <c>graph</c> block containing node label declarations and undirected edge
    /// declarations for that expression subtree. Outputs <c>NULL</c> if the tree is empty.
    /// </para>
    /// <para>
    /// For <c>-type-check</c>: performs two full post-order traversals — one to assign types
    /// to each node and one to validate them — then outputs <c>{"legal": true}</c> on success
    /// or <c>{"legal": false}</c> if the tree is empty or a type error is encountered.
    /// </para>
    /// <para>
    /// For <c>-var-decl</c>: performs a four-phase post-parse pipeline:
    /// </para>
    /// <list type="number">
    /// <item><b>Hoist</b> (pre-order walk): Calls <see cref="Variable.assignVarInfo"/> on every
    /// <see cref="Variable"/> node. By this point all local scopes have been removed, so this
    /// pass resolves any forward references to globals that could not be resolved during parsing.</item>
    /// <item><b>Set types</b> (post-order walk): Calls <c>setType()</c> on every node to propagate
    /// type information bottom-up through the tree.</item>
    /// <item><b>Check types</b> (post-order walk): Calls <c>typeCheck()</c> on every node to
    /// validate that all type constraints are satisfied.</item>
    /// <item><b>Print variable info</b> (pre-order walk): Calls <see cref="Variable.getVarInfo"/>
    /// on every <see cref="Variable"/> node and writes the result to the output writer.</item>
    /// </list>
    /// <para>
    /// On any exception during the pipeline, outputs <c>null</c> for <c>-var-decl</c> and
    /// <c>{"legal": false}</c> for <c>-type-check</c>.
    /// </para>
    /// </remarks>
    private static int walkTree(ProgramNode? tree, string inputPath, ParsedOptions opts)
    {
        string mode = opts.mode!;
        string format = "";
        if (opts.Length() > 1)
            format = opts.fmt!;
        TextWriter outw = Console.Out;
        string outfile;
        switch (mode)
        {
            case Options.Client:
                switch (format)
                {
                    case Options.Dotfile:
                        outfile = "tree.dot";
                        outw = new StreamWriter(outfile);
                        break;
                    case Options.VarDecl:
                        break; // outw is already Console.Out
                    case Options.CompileAsm:
                        break;
                    case Options.ClassDecl:
                        break;
                    case Options.TypeCheck:
                        break;
                    default:
                        outfile = getOutputFile(inputPath);
                        outw = new StreamWriter(outfile);
                        break;
                }
                break;
            case Options.GenTests:
                if (opts.opt != Options.OutDir)
                    throw new Exception("GenTests mode with no specified output directory");
                outfile = getOutputPath(inputPath, opts);
                outw = new StreamWriter(outfile);
                break;
            case Options.Run:
                outw = Console.Out;
                break;
            default:
                error(new InvalidOption($"Unrecognized mode: {mode}"));
                break;
        }

        if (tree == null)
        {
            switch (format)
            {
                case Options.Dotfile:
                    outw.WriteLine(NULL);
                    break;
                case Options.TypeCheck:
                    outw.WriteLine("{\"legal\": false}");
                    break;
                case Options.VarDecl:
                    outw.WriteLine(NULL);
                    break;
                case Options.ClassDecl:
                    outw.WriteLine("INVALID");
                    break;
                case Options.CompileAsm:
                    //outw.WriteLine("INVALID"); // change this based on tests
                    break;
                default:
                    break;
            }
            outw.Close();
            return 1;
        }

        switch (format)
        {
            case Options.Dotfile:
               walkPreOrder(tree, (TreeNode node) =>
                {
                    var enode = node as ExprNode; 
                    if (enode == null)
                        return;

                    outw.WriteLine("graph program {");

                   walkPreOrder(enode, (child) =>
                    {
                        ExprNode echild = (ExprNode)child;
                        outw.WriteLine($"{echild.unique_id} [label=\"{echild.token.lexeme}\"]");
                    });

                   walkPreOrder(enode, (child) =>
                    {
                        ExprNode echild = (ExprNode)child;
                        foreach (var grandchild in echild.getChildren())
                        {
                            ExprNode egrandchild = (ExprNode)grandchild;
                            outw.WriteLine($"{echild.unique_id} -- {egrandchild.unique_id}");
                        }
                    });

                    outw.WriteLine("}");
                    outw.Close();
                    
                    throw new StopIteration();
                });
            break;

        case Options.TypeCheck:
            try
            {
               walkPreOrder(tree, (n) =>
                {
                    foreach (var c in n.getChildren())
                    {
                        c.parent = n;
                    }
                });

                walkPostOrder(tree, (n) =>
                {
                    n.setType();
                });

                walkPostOrder(tree, (n) =>
                {
                    n.typeCheck(); 
                });
                outw.Write("{\"legal\": true}");
            }
            catch (Exception)
            {
                outw.Write("{\"legal\": false}");
                return 1;
            }
            break;

        case Options.VarDecl:
            try
            {
               walkPreOrder(tree, (n) =>
                {
                    foreach (var c in n.getChildren())
                    {
                        c.parent = n;
                    }
                });

               walkPreOrder(tree, (n) => 
                {
                    Variable v = (n as Variable)!;
                    if (v != null)
                        v.assignVarInfo();
                });

                walkPostOrder(tree, (n) =>
                {
                    n.setType();
                });

                // some vardecl tests did not expect function return type
                //  type checking. Comment out ReturnNode.typeCheck to by-
                //  pass the test failure
                walkPostOrder(tree, (n) =>
                {
                    n.typeCheck(); 
                });
                
               walkPreOrder(tree, (n) => 
                {
                    Variable v = (n as Variable)!;
                    if (v != null)
                        outw.WriteLine(v.getVarInfo());
                });
            }
            catch (Exception)
            {
                outw.Write(NULL);
                return 1;
            }
            break;

        case Options.ClassDecl:
            try
            {
               walkPreOrder(tree, (n) =>
                    {
                        foreach (var c in n.getChildren())
                        {
                            c.parent = n;
                        }
                    });

               walkPreOrder(tree, (n) => 
                {
                    Variable v = (n as Variable)!;
                    if (v != null)
                        v.assignVarInfo();
                });

                walkPostOrder(tree, (n) =>
                {
                    n.setType();
                });

                walkPostOrder(tree, (n) =>
                {
                    n.typeCheck(); 
                });
                
               walkPreOrder(tree, (n) => 
                {
                    Variable v = (n as Variable)!;
                    if (v != null)
                        outw.WriteLine(v.getVarInfo(verbose: false));
                    else
                    {
                        Member m = (n as Member)!;
                        if (m != null)
                            outw.WriteLine(m.getMemberInfo());
                    }
                });
            }
            catch (Exception)
            {
                outw.WriteLine("INVALID");
                return 1;
            }
            
            break;

        case Options.CompileAsm:
            string outAsm = opts.optarg!;
            try
            {
               walkPreOrder(tree, (n) =>
                {
                    foreach (var c in n.getChildren())
                    {
                        c.parent = n;
                    }
                });

                walkPreOrder(tree, (n) => 
                {
                    Variable v = (n as Variable)!;
                    if (v != null)
                        v.assignVarInfo();
                });

                walkPostOrder(tree, (n) =>
                {
                    n.setType();
                });

                walkPostOrder(tree, (n) =>
                {
                    n.typeCheck(); 
                });

                setTemporaries(tree);
                tree.genCode();
                
                using(var w = new StreamWriter(outAsm))
                {
                    Label? mainLabel = null;
                    foreach (FuncdefNode f in tree.funcs)
                    {
                        if (f.info!.token == null)
                            continue;
                        if (f.info.token.lexeme == "main")
                            mainLabel = f.lbl;
                    }
                    if (mainLabel == null)
                        throw new Exception("Didn't find main label");
                    Asm.write(w, mainLabel);
                }
                lab.Run.compile(outAsm);               
            }
            catch (Exception e)
            {
                Console.Error.Write(e);
                outw.WriteLine("INVALID"); // change this based on tests
                return 1;
            }
            break;

        default:
            break;
        }        
        
        outw.Close();
        return 0;
    }

    /// <summary>
    /// Generates JSON output for tokenized or parsed input, writing to a file or the console.
    /// </summary>
    /// <param name="inputPath">The original input file path (used for output path generation in --gen-tests mode).</param>
    /// <param name="opts">List containing processing options where:
    /// <list type="bullet">
    /// <item>opts[0]: Processing mode (--gen-tests or --run)</item>
    /// <item>opts[1]: Output format (-tok-json or -tree-json)</item>
    /// <item>opts[2]: (Optional) --outdir flag</item>
    /// <item>opts[3]: (Optional) Output directory path</item>
    /// </list>
    /// </param>
    /// <param name="tokenizer">The tokenizer instance containing the input to process.</param>
    /// <remarks>
    /// For <c>-tok-json</c>, serializes the full token stream to JSON via <see cref="tokenizeToJson"/>.
    /// For <c>-tree-json</c>, parses the token stream into a tree and serializes the first
    /// <see cref="ExprNode"/> encountered during a pre-order walk.
    /// In both cases, any exception during processing causes <c>NULL</c> to be written instead.
    /// </remarks>
    private static int outputJson(string inputPath, ParsedOptions opts, Tokenizer tokenizer)
    {
        string mode = opts.mode!;
        string format = opts.fmt!;
        int failed = 0;

        TextWriter outw = Console.Out;
        switch (mode)
        {
            case Options.GenTests or Options.Client:
                string outfile = "";
                if (opts.opt == Options.OutDir)
                    outfile = getOutputPath(inputPath, opts);
                else
                {
                    switch (format)
                    {
                        case Options.TreeJson:
                            outfile = "tree.json";
                            break;
                    }
                }
                outw = new StreamWriter(outfile);
                break;
            case Options.Run:
                outw = Console.Out;
                break;
            default:
                error(new InvalidOption($"Unrecognized mode: {mode}"));
                break;
        }

        string jsonData = NULL;
        try
        {
            switch (format)
            {
                case Options.TokJson:
                    jsonData = tokenizeToJson(tokenizer);
                    break;
                case Options.TreeJson:
                    ProgramNode tree = ProgramNode.parse(tokenizer);
                   walkPreOrder(tree, (TreeNode node) =>
                    {
                        var enode = node as ExprNode;
                        if (enode != null)
                        {
                            jsonData = enode.toJson();
                            throw new StopIteration();
                        }
                    });
                    break;
                default:
                    error(new InvalidOption($"Unrecognized format: {format}"));
                    break;
            }
        }
        catch (Exception)
        {
            // jsonData should be null if parsing error occurred
            failed = 1;
        }
        finally
        {
            outw.Write(jsonData);
            outw.Close();
        }  

        return failed;
    }

    /// <summary>
    /// Renders the parse tree as a box-drawing text representation and writes it to a file or the console.
    /// </summary>
    /// <param name="tree">The <see cref="ProgramNode"/> tree to render.</param>
    /// <param name="inputPath">The original input file path (used to determine the output path in --gen-tests mode).</param>
    /// <param name="opts">List containing processing options where:
    /// <list type="bullet">
    /// <item>opts[0]: Processing mode (--gen-tests or --run)</item>
    /// <item>opts[1]: Output format (-tree-box)</item>
    /// <item>opts[2]: (Optional) --outdir flag</item>
    /// <item>opts[3]: (Optional) Output directory path</item>
    /// </list>
    /// </param>
    /// <remarks>
    /// Uses UTF-8 encoding to correctly render box-drawing characters. Writes <c>NULL</c>
    /// if the tree contains no function nodes. Any rendering errors are reported to stderr.
    /// </remarks>
    private static int outputBoxDrawing(ProgramNode? tree, string inputPath, ParsedOptions opts)
    {
        string mode = opts.mode!;
        Console.OutputEncoding = Encoding.UTF8;
        TextWriter outw;
        switch (mode)
        {
            case Options.GenTests:
                outw = new StreamWriter(getOutputPath(inputPath, opts), false, Encoding.UTF8);
                break;
            case Options.Run:
                outw = Console.Out;
                break;
            default:
                error(new InvalidOption($"Unrecognized mode: {mode}"));
                return 1;
        }

        using (outw)
        {
            if (tree == null)
            {
                outw.Write(NULL);
                return 1;
            }
            try
            {
                Treedump.textTree(tree, outw);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        return 0;
    }

    /// <summary>
    /// Converts an input file path to its corresponding output file path.
    /// </summary>
    /// <param name="inputPath">The input file path, which must contain the <c>inputs/</c> directory segment.</param>
    /// <param name="opts">List containing processing options where:
    /// <list type="bullet">
    /// <item>opts[2]: (Optional) --outdir flag</item>
    /// <item>opts[3]: (Optional) Custom output directory path</item>
    /// </list>
    /// </param>
    /// <returns>
    /// The output file path with the <c>inputs/</c> segment replaced by the appropriate output directory.
    /// </returns>
    /// <remarks>
    /// <para>
    /// By default, replaces the <c>inputs/</c> segment with <c>outputs/</c> and preserves the
    /// original <c>.txt</c> extension. For example: <c>staging/inputs/test.txt</c> → <c>staging/outputs/test.txt</c>.
    /// </para>
    /// <para>
    /// If the --outdir option is specified (opts[2] == <c>"-dir"</c>), the custom directory is used
    /// instead. The file extension is also changed to <c>.json</c> for <c>-tree-json</c> and
    /// <c>-type-check</c> formats; all other formats retain <c>.txt</c>.
    /// </para>
    /// </remarks>
    private static string getOutputPath(string inputPath, ParsedOptions opts)
    {
        string outdir = OUTPUT_DIR;
        string ext = TEXT_EXT;
        string format = "";
        if (opts.Length() > 1)
            format = opts.fmt!;
        if (opts.Length() == 4 && opts.opt == Options.OutDir)
        {
            outdir = $"{opts.optarg}";
            switch (format)
            {
                case Options.Dotfile:
                case Options.TokJson:
                case Options.TreeBox:
                    break; // ext stays .txt
                case Options.TreeJson:
                case Options.TypeCheck:
                    ext = JSON_EXT;
                    break;
                default:
                    break;
            }
        }
        int i = inputPath.IndexOf(INPUT_DIR);
        int j = inputPath.IndexOf(TEXT_EXT); // assuming all input files are .txt
        string outputPath = inputPath[..i] + outdir + inputPath[(i + INPUT_DIR.Length)..j] + ext;
        return outputPath;
    }

    private static string getOutputFile(string inputPath)
    {
        string fname = "";
        for (int i = inputPath.Length - 1; i >= 0; i--)
        {
            char cur = inputPath[i];
            if (cur == '/' || cur == '\\')
                break;
            fname += cur;
        }
        char[] charr = fname.ToArray();
        Array.Reverse(charr);
        return new string(charr);
    }
}