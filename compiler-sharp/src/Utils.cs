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
    private const string INPUT_DIR = "inputs/";
    private const string OUTPUT_DIR = "outputs/";

    // Output constants
    private const string NULL_JSON = "null";
    
    // File extension constants
    private const string TEXT_EXT = ".txt";
    private const string JSON_EXT = ".json";

    /// <summary>
    /// Writes an exception's message to stderr and rethrows it.
    /// </summary>
    /// <param name="e">The exception to report and rethrow.</param>
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
    /// </list>
    /// <para>Optional flags (must follow the format option):</para>
    /// <list type="bullet">
    /// <item>--outdir [dir]: Write output files to the specified directory instead of the default outputs/ directory</item>
    /// </list>
    /// </remarks>
    public static List<string> parseOptions(string[] args)
    {
        List<string> opts = new List<string>();
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case Options.GenTests or Options.Run:
                {
                    opts.Add(args[0]);
                    if (args.Length < 2)
                        error(new InvalidOption("Provided mode option without format option (-tree-[json][box]|-tok-json|-dotfile|-type-check)"));
                    opts.Add(args[1]);
                    if (args.Length > 2)
                    {
                        switch (args[2])
                        {
                            case Options.OutDir:
                                opts.Add(args[2]);
                                if (args.Length < 4)
                                    error(new InvalidOption("Provided outdir option with no out directory specified"));
                                opts.Add(args[3]);
                                break;
                        }
                    }
                    break;
                }
                default:
                {
                    if (args[0].Contains("--"))
                        error(new InvalidOption("Provided unrecognized option"));
                    break;
                }
            }
        }
        return opts;
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
    public static int handleOptions(string[] args, List<string> opts)
    {
        if (opts.Count == 0)
            return 0;
        if (opts.Count > 4) // optional -dir [dir] option
            return -1;

        string[] files = args[opts.Count..];

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
                processFile(input, opts);
            }
            else
            {
                Console.Error.WriteLine("Could not find input file/directory:", input);
                break;
            }
        }
        return 1;
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
    /// Each format handles parse/tokenization failures internally and writes a
    /// format-appropriate failure value (e.g., <c>NULL_JSON</c> or <c>{"legal": false}</c>).
    /// </remarks>
    private static void processFile(string inputPath, List<string> opts)
    {
        Tokenizer tokenizer = new Tokenizer();
        using (StreamReader reader = new StreamReader(inputPath))
            tokenizer.setInput(reader.ReadToEnd());

        string format = opts[1];

        try
        {
            switch (format)
            {
                case Options.TokJson or Options.TreeJson:
                    outputJson(inputPath, opts, tokenizer);
                    break;
                case Options.TreeBox:
                    ExprNode.ResetCounter();
                    ProgramNode tree = parseToTree(tokenizer);
                    outputBoxDrawing(tree, inputPath, opts);
                    break;
                case Options.Dotfile:
                    ExprNode.ResetCounter();
                    tree = parseToTree(tokenizer);     
                    walkTree(tree, inputPath, opts);
                    break;
                case Options.TypeCheck:
                    tree = parseToTree(tokenizer);
                    walkTree(tree, inputPath, opts);
                    break;
                default:
                    Utils.error(new InvalidOption($"Unrecognized option provided: {format}"));
                    break;
            }
        }
        catch (Exception)
        {
            // each format handles failures internally:
            //      e.g. Json => NULL_JSON
            //      e.g. TypeCheck => {"legal": false}
            //      ...
        }
    }

    /// <summary>
    /// Tokenizes input and serializes the resulting token stream to a JSON string.
    /// </summary>
    /// <param name="tokenizer">The tokenizer instance containing the input to process.</param>
    /// <returns>
    /// A JSON string representing the list of tokens, or <c>NULL_JSON</c> if a tokenization error occurred.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Consumes tokens until EOF is reached. If an error occurs during tokenization,
    /// returns <c>NULL_JSON</c> and writes the error to stderr.
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

        return hasError ? NULL_JSON : System.Text.Json.JsonSerializer.Serialize(tokens, jsonOpts);
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
    private static ProgramNode parseToTree(Tokenizer tokenizer)
    {
        try
        {
            return ProgramNode.parse(tokenizer);
        }
        catch (Exception)
        {
            return new ProgramNode();
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
    static void walk(TreeNode n, Action<TreeNode> callback)
    {
        try
        {
            walkHelper(n, callback);
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
    static void walkHelper(TreeNode node, Action<TreeNode> callback)
    {
        callback(node);
        foreach(TreeNode child in node.getChildren())
        {
            walk(child, callback);
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
            walkPostOrder(child, callback);
        }
        callback(node);
    }

    /// <summary>
    /// Walks the program tree and generates output in the specified format (dotfile or type-check).
    /// </summary>
    /// <param name="tree">The program tree to process.</param>
    /// <param name="inputPath">The original input file path (used for output path generation in --gen-tests mode).</param>
    /// <param name="opts">List containing processing options where:
    /// <list type="bullet">
    /// <item>opts[0]: Processing mode (--gen-tests or --run)</item>
    /// <item>opts[1]: Output format (-dotfile or -type-check)</item>
    /// <item>opts[2]: (Optional) --outdir flag</item>
    /// <item>opts[3]: (Optional) Output directory path</item>
    /// </list>
    /// </param>
    /// <remarks>
    /// <para>
    /// For <c>-dotfile</c>: walks the tree to find the first expression node, then generates
    /// a Graphviz <c>graph</c> block containing node label declarations and undirected edge
    /// declarations for that expression subtree. Outputs <c>NULL_JSON</c> if the tree is empty.
    /// </para>
    /// <para>
    /// For <c>-type-check</c>: performs two full post-order traversals — one to assign types
    /// to each node and one to validate them — then outputs <c>{"legal": true}</c> on success
    /// or <c>{"legal": false}</c> if the tree is empty or a type error is encountered.
    /// </para>
    /// </remarks>
    private static void walkTree(ProgramNode tree, string inputPath, List<string> opts)
    {
        string mode = opts[0];
        string format = "";
        if (opts.Count > 1)
            format = opts[1];
        TextWriter outw = Console.Out;
        switch (mode)
        {
            case Options.GenTests:
                outw = new StreamWriter(getOutputPath(inputPath, opts));
                break;
            case Options.Run:
                outw = Console.Out;
                break;
            default:
                error(new InvalidOption($"Unrecognized mode: {mode}"));
                break;
        }

        if (tree.getChildren().Count == 0)
        {
            switch (format)
            {
                case Options.Dotfile:
                    outw.Write(NULL_JSON);
                    break;
                case Options.TypeCheck:
                    outw.Write("{\"legal\": false}");
                    break;
                default:
                    break;
            }
            outw.Close();
            return;
        }

        try
        {
            switch (format)
            {
                case Options.Dotfile:
                    walk(tree, (TreeNode node) =>
                    {
                        var enode = node as ExprNode; 
                        if (enode == null) // TreeNode was not ExprNode; don't care about it
                            return;

                        outw.WriteLine("graph program {");

                        // Write all node declarations
                        walk(enode, (child) =>
                        {
                            ExprNode echild = (ExprNode)child;
                            outw.WriteLine($"{echild.unique_id} [label=\"{echild.token.lexeme}\"]");
                        });

                        // Write all edge declarations
                        walk(enode, (child) =>
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
                        
                        // Terminate tree walk after first expression
                        throw new StopIteration();
                    });
                break;

            case Options.TypeCheck:
                // Traverse entire tree post order; set types
                walkPostOrder(tree, (n) =>
                {
                    n.setType();
                });

                // Traverse entire tree post order; check types
                walkPostOrder(tree, (n) =>
                {
                    n.typeCheck(); 
                });
                outw.Write("{\"legal\": true}");
                break;

            default:
                // Unsupported formats are filtered out in processFile; no action needed here.
                break;
            }        
        }
        catch (Exception)
        {
            switch (format)
            {
                case Options.TypeCheck:
                    outw.Write("{\"legal\": false}");
                    break;
            }
        }
        finally
        {
            outw.Close();
        }
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
    /// In both cases, any exception during processing causes <c>NULL_JSON</c> to be written instead.
    /// </remarks>
    private static void outputJson(string inputPath, List<string> opts, Tokenizer tokenizer)
    {
        string mode = opts[0];
        string format = opts[1];

        TextWriter outw = Console.Out;
        switch (mode)
        {
            case Options.GenTests:
                outw = new StreamWriter(getOutputPath(inputPath, opts));
                break;
            case Options.Run:
                outw = Console.Out;
                break;
            default:
                error(new InvalidOption($"Unrecognized mode: {mode}"));
                break;
        }

        string jsonData = NULL_JSON;
        try
        {
            switch (format)
            {
                case Options.TokJson:
                    jsonData = tokenizeToJson(tokenizer);
                    break;
                case Options.TreeJson:
                    ProgramNode tree = ProgramNode.parse(tokenizer);
                    walk(tree, (TreeNode node) =>
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
        }
        finally
        {
            outw.Write(jsonData);
            outw.Close();
        }  
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
    /// Uses UTF-8 encoding to correctly render box-drawing characters. Writes <c>NULL_JSON</c>
    /// if the tree contains no function nodes. Any rendering errors are reported to stderr.
    /// </remarks>
    private static void outputBoxDrawing(ProgramNode tree, string inputPath, List<string> opts)
    {
        string mode = opts[0];
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
                return;
        }

        using (outw)
        {
            if (tree.funcs.Count == 0)
            {
                outw.Write(NULL_JSON);
                return;
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
    private static string getOutputPath(string inputPath, List<string> opts)
    {
        string outdir = OUTPUT_DIR;
        string ext = TEXT_EXT;
        string format = "";
        if (opts.Count > 1)
            format = opts[1];
        if (opts.Count == 4 && opts[2] == Options.OutDir)
        {
            outdir = $"{opts[3]}/";
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
}