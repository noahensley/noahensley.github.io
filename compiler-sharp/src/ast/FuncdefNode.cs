/*
 * File: FuncdefNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 */

/// <summary>
/// Represents a function definition in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Function definitions declare callable code blocks with a name, optional return type,
/// parameters (future implementation), and a body containing statements.
/// </para>
/// <para>
/// Current syntax:
/// </para>
/// <code>
/// func name() : returnType { statements }
/// func name() { statements }
/// </code>
/// <para>
/// When no return type is specified (no colon after parentheses), the function defaults
/// to void (no return value). Parameters are not yet implemented; all functions currently
/// accept zero arguments.
/// </para>
/// </remarks>
public class FuncdefNode : TreeNode 
{
    /// <summary>
    /// The return type of the function.
    /// </summary>
    /// <remarks>
    /// Can be any VarType subclass (IntType, FloatType, StringType, or VoidType).
    /// VoidType indicates the function doesn't return a value. This field is used
    /// during semantic analysis to verify return statements match the declared type.
    /// </remarks>
    public VarType returnType;
    
    /// <summary>
    /// The function name identifier token.
    /// </summary>
    /// <remarks>
    /// Contains the function's name as it appears in source code. Used for symbol table
    /// lookups, function calls, and error reporting. The lexeme field contains the actual
    /// name string.
    /// </remarks>
    public Token name;
    
    /// <summary>
    /// The statement block comprising the function body.
    /// </summary>
    /// <remarks>
    /// Contains all executable statements within the function. This is where the function's
    /// logic resides, including variable declarations, control flow, and return statements.
    /// </remarks>
    public StmtsNode stmts;
    
    // TODO: Add parameter list when parameter parsing is implemented

    /// <summary>
    /// Creates a function definition node with the specified components.
    /// </summary>
    /// <param name="returnType">The function's return type (VoidType if unspecified).</param>
    /// <param name="name">The function name identifier token.</param>
    /// <param name="stmts">The function body statement block.</param>
    private FuncdefNode(VarType returnType, Token name, StmtsNode stmts)
    {
        this.returnType = returnType;
        this.name = name;
        this.stmts = stmts;
    }

    /// <summary>
    /// Parses a function definition from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A FuncdefNode representing the parsed function definition.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// func identifier() : type { statements }
    /// func identifier() { statements }
    /// </code>
    /// <para>
    /// Parsing sequence:
    /// </para>
    /// <list type="number">
    /// <item>Consumes the FUNCDEF keyword</item>
    /// <item>Captures the function name (ID token)</item>
    /// <item>Expects empty parameter list: LPAREN followed by RPAREN</item>
    /// <item>Checks for optional return type:
    ///   <list type="bullet">
    ///   <item>If COLON present: parses type token and converts to VarType</item>
    ///   <item>If no COLON: defaults to VoidType</item>
    ///   </list>
    /// </item>
    /// <item>Parses the statement block (expects LBRACE ... RBRACE)</item>
    /// </list>
    /// <para>
    /// Parameters are not yet implemented. The parentheses must be empty, but the grammar
    /// is structured to easily add parameter parsing in the future by inserting parameter
    /// list parsing between LPAREN and RPAREN.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when expected tokens (FUNCDEF, ID, LPAREN, RPAREN, LBRACE, RBRACE) are not found
    /// in the correct sequence.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when an invalid type token follows the COLON.
    /// </exception>
    public static FuncdefNode parse(Tokenizer T)
    {
        T.expect(TokenSymbols.FUNCDEF);
        Token name = T.expect(TokenSymbols.ID);
        T.expect(TokenSymbols.LPAREN);
        T.expect(TokenSymbols.RPAREN);
        VarType returnType;

        // Check for optional return type specification
        if (T.peek() == TokenSymbols.COLON)
        {
            T.expect(TokenSymbols.COLON);
            returnType = VarType.fromToken(T.next());
        }
        else
        {
            // No return type specified, default to void
            returnType = new VoidType();
        }

        StmtsNode stmts = StmtsNode.parse(T);
        return new FuncdefNode(returnType, name, stmts);
    }
    
    /// <summary>
    /// Gets the child nodes of this function definition.
    /// </summary>
    /// <returns>A list containing only the statement block.</returns>
    /// <remarks>
    /// Currently returns only the statements node. When parameters are implemented,
    /// this will need to include the parameter list node as well for complete tree
    /// traversal.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>() { stmts };
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
