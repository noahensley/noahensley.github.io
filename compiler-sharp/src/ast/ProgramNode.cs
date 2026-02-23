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
/// A program consists of zero or more function definitions, and this node holds the
/// complete collection of all functions defined in the source file.
/// </para>
/// <para>
/// As the root of the AST, this node is the entry point for tree traversal operations
/// such as code generation, semantic analysis, and visualization. An empty program
/// (no functions) is valid and will have an empty funcs list.
/// </para>
/// </remarks>
public class ProgramNode : TreeNode 
{
    /// <summary>
    /// List of all function definitions in the program.
    /// </summary>
    /// <remarks>
    /// Contains the parsed function definitions in the order they appear in the source code.
    /// An empty list indicates a valid program with no function definitions. This list is
    /// used during code generation to emit all function definitions and during semantic
    /// analysis to build the symbol table.
    /// </remarks>
    public List<FuncdefNode> funcs = new List<FuncdefNode>();

    /// <summary>
    /// Parses a complete program from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A ProgramNode containing all parsed function definitions.</returns>
    /// <remarks>
    /// <para>
    /// Parsing continues until the end of input (EOF token) is reached. The parser
    /// repeatedly attempts to parse function definitions, each starting with the FUNCDEF keyword.
    /// </para>
    /// <para>
    /// Parsing sequence:
    /// </para>
    /// <list type="number">
    /// <item>Peek at the next token</item>
    /// <item>If FUNCDEF: parse a function definition and add it to the funcs list</item>
    /// <item>If EOF: parsing is complete, return the ProgramNode</item>
    /// <item>Otherwise: error (unexpected token at program level)</item>
    /// </list>
    /// <para>
    /// An empty program (containing only whitespace or comments) is valid and will
    /// result in a ProgramNode with an empty funcs list.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when encountering a token at program level that is neither FUNCDEF nor EOF.
    /// This indicates a syntax error such as statements outside of functions.
    /// </exception>
    public static ProgramNode parse(Tokenizer T)
    {
        ProgramNode pnode = new ProgramNode();
        
        while (true)
        {
            if (T.peek() == TokenSymbols.FUNCDEF)
                pnode.funcs.Add(FuncdefNode.parse(T));
            else if (T.peek() == TokenSymbols.EOF)
                break;
            else
                Utils.error(new MissingMainFunction($"Expected function declaration or EOF at line {T.getLine()}, got: {T.peek()}"));
        }
        
        return pnode;
    }

    /// <summary>
    /// Gets the child nodes of this program.
    /// </summary>
    /// <returns>
    /// A list containing all function definition nodes in the program.
    /// </returns>
    /// <remarks>
    /// Converts the funcs list (which contains FuncdefNode objects) to a list of TreeNode
    /// objects for uniform tree traversal. This allows visitors and other tree-walking
    /// algorithms to process all children polymorphically.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>(funcs);
    }

    public override void setType()
    {
        return; // not implemented
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}
