/*
 * File: ProgramNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments 
 * for this file on the following dates:
 *      January 23, 2026,
        January 25, 2026,
        January 28, 2026
 */

/// <summary>
/// Represents the root node of the abstract syntax tree for an entire program.
/// </summary>
/// <remarks>
/// <para>
/// ProgramNode serves as the top-level container for the entire parsed source code.
/// A program consists of zero or more global variable declarations followed by zero or more
/// function definitions. This node holds the complete collection of both in source order.
/// </para>
/// <para>
/// As the root of the AST, this node is the entry point for tree traversal operations
/// such as code generation, semantic analysis, and visualization. An empty program
/// (no functions or global declarations) is valid and will have empty lists.
/// </para>
/// </remarks>
public class ProgramNode : TreeNode 
{
    /// <summary>
    /// List of all function definitions in the program, in source order.
    /// </summary>
    /// <remarks>
    /// An empty list indicates a valid program with no function definitions. This list is
    /// used during code generation to emit all function definitions and during semantic
    /// analysis to build the symbol table.
    /// </remarks>
    public List<FuncdefNode> funcs = new List<FuncdefNode>();

    /// <summary>
    /// List of all global variable declarations in the program, in source order.
    /// </summary>
    /// <remarks>
    /// Contains global variable declarations that appear before any function definitions.
    /// An empty list indicates no global variables were declared. This list is used during
    /// semantic analysis to populate the global scope of the symbol table, making these
    /// variables accessible throughout all function definitions in the program.
    /// </remarks>
    public List<VardeclNode> globals = new List<VardeclNode>();
    private static Dictionary<string, ClassType> classes = new Dictionary<string, ClassType>();
    public static ClassType getClassTypeFromClassName(Token className)
    {
        if (className.sym != TokenSymbols.ID)
            Utils.error(new InvalidExpression($"Got non-identifier class name on line {className.line}.  Expected {TokenSymbols.ID}; Got {className.sym}"));
        if (!classes.ContainsKey(className.lexeme))
            classes[className.lexeme] = new ClassType(className, null);
        return classes[className.lexeme];
    }   

    /// <summary>
    /// Parses a complete program from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A <see cref="ProgramNode"/> containing all parsed global declarations and function definitions.</returns>
    /// <remarks>
    /// <para>
    /// Parsing continues until the end of input (EOF token) is reached. The parser
    /// repeatedly dispatches on the next token to parse either a global variable declaration
    /// (VAR) or a function definition (FUNCDEF).
    /// </para>
    /// <para>
    /// Parsing sequence:
    /// </para>
    /// <list type="number">
    /// <item>Skip any leading EOS tokens.</item>
    /// <item>If FUNCDEF: parse a function definition and append it to <see cref="funcs"/>.</item>
    /// <item>If VAR: parse a global variable declaration and append it to <see cref="globals"/>.</item>
    /// <item>If EOF: parsing is complete, return the ProgramNode.</item>
    /// <item>Otherwise: report a syntax error.</item>
    /// </list>
    /// <para>
    /// An empty program (containing only whitespace or comments) is valid and will
    /// result in a ProgramNode with empty lists.
    /// </para>
    /// </remarks>
    /// <exception cref="MissingMainFunction">
    /// Reported via <see cref="Utils.error"/> when a token at program level is neither
    /// FUNCDEF, VAR, nor EOF, indicating a syntax error such as a bare statement outside a function.
    /// </exception>
    public static ProgramNode parse(Tokenizer T)
    {
        ProgramNode pnode = new ProgramNode();
        
        while (true)
        {
            if (T.peek() == TokenSymbols.EOS)
                T.next();
            if (FuncdefNode.canParse(T))
                pnode.funcs.Add(FuncdefNode.parse(T));
            else if (VardeclNode.canParse(T))
                pnode.globals.Add(VardeclNode.parse(T));
            else if (ClassdefNode.canParse(T))
            {
                ClassdefNode cldef = ClassdefNode.parse(T);
                ProgramNode.classes[cldef.name.lexeme] = cldef.classType;
            }
            else if (T.peek() == TokenSymbols.EOF)
                break;
            else
                Utils.error(new MissingMainFunction($"Expected function declaration or EOF at line {T.getLine()}, got: {T.peek()}"));
        }
        
        return pnode;
    }

    /// <summary>
    /// Gets the child nodes of this program node.
    /// </summary>
    /// <returns>
    /// A list containing all function definition nodes in the program.
    /// </returns>
    /// <remarks>
    /// Converts the <see cref="funcs"/> list to a list of <see cref="TreeNode"/> objects
    /// for uniform tree traversal. Global variable declarations in <see cref="globals"/> are
    /// not included here; they are visited separately during semantic analysis passes.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        List<TreeNode> children = [.. funcs];
        foreach (var classname in classes.Keys)
        {
            ClassdefNode? classdef = ProgramNode.classes[classname].declarer;
            if (classdef == null)
                throw new Exception($"Internal compiler error: failed to add null class definition to ProgramNode children");
            children.Add(classdef);
        }
        return children;
    }

    /// <summary>
    /// Type inference for program nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // not implemented
    }

    /// <summary>
    /// Type validation for program nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }
}
