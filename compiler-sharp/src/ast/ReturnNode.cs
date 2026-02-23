/*
 * File: ReturnNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 */

/// <summary>
/// Abstract base class for return statement nodes in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Return statements control the exit from functions and optionally specify a value
/// to return to the caller. The two concrete implementations handle different cases:
/// </para>
/// <list type="bullet">
/// <item><b>ReturnExprNode</b>: Returns an expression value (for non-void functions)</item>
/// <item><b>ReturnVoidNode</b>: Returns nothing (for void functions or early exits)</item>
/// </list>
/// <para>
/// During semantic analysis, the return statement's type is checked against the
/// containing function's declared return type to ensure type safety.
/// </para>
/// </remarks>
public abstract class ReturnNode : StmtNode 
{
    /// <summary>
    /// The RETURN keyword token.
    /// </summary>
    /// <remarks>
    /// Stored for error reporting (showing the location of the return statement) and
    /// for tracking the source position during code generation.
    /// </remarks>
    public Token retToken;

    /// <summary>
    /// Creates a return node with the specified RETURN token.
    /// </summary>
    /// <param name="retToken">The RETURN keyword token from the source.</param>
    protected ReturnNode(Token retToken)
    {
        this.retToken = retToken;
    }

    /// <summary>
    /// Parses a return statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>
    /// Either a ReturnVoidNode (if no expression follows) or ReturnExprNode (if an expression follows).
    /// </returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// return
    /// return expression
    /// </code>
    /// <para>
    /// The parser determines which type of return statement by checking the token following
    /// the RETURN keyword:
    /// </para>
    /// <list type="bullet">
    /// <item>EOS (end-of-statement): Creates ReturnVoidNode (no value returned)</item>
    /// <item>Any other token: Parses an expression and creates ReturnExprNode</item>
    /// </list>
    /// <para>
    /// Both forms require an EOS token to terminate the statement. For void returns, the
    /// EOS immediately follows the RETURN keyword. For expression returns, the EOS follows
    /// the expression.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the RETURN keyword token is not found.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the end-of-statement token is missing after the return statement.
    /// </exception>
    public new static ReturnNode parse(Tokenizer T)
    {
        Token ret = T.expect(TokenSymbols.RETURN);
        
        if (T.peek() == TokenSymbols.EOS)
        {
            // Void return: return;
            T.expect(TokenSymbols.EOS);
            return new ReturnVoidNode(ret);
        }
        else
        {
            // Expression return: return expr;
            ExprNode expr = ExprNode.parse(T);
            T.expect(TokenSymbols.EOS);
            return new ReturnExprNode(ret, expr);
        }
    }
    
    /// <summary>
    /// Checks if the next token can be parsed as a return statement.
    /// </summary>
    /// <param name="T">The tokenizer to check.</param>
    /// <returns>True if the next token is RETURN; false otherwise.</returns>
    /// <remarks>
    /// Used by the statement parser to determine which type of statement to parse.
    /// This lookahead allows the parser to dispatch to the appropriate parsing method
    /// without consuming tokens.
    /// </remarks>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.RETURN;
    }
}

/// <summary>
/// Represents a return statement that returns an expression value.
/// </summary>
/// <remarks>
/// <para>
/// Used for functions that have a non-void return type (int, float, string, etc.).
/// The expression's type must match the function's declared return type, which is
/// verified during semantic analysis.
/// </para>
/// <para>
/// Example: In a function declared as <c>func getValue() : int</c>, the statement
/// <c>return 42</c> would be represented as a ReturnExprNode containing a numeric
/// literal expression.
/// </para>
/// </remarks>
public class ReturnExprNode : ReturnNode 
{
    /// <summary>
    /// The expression whose value is returned to the caller.
    /// </summary>
    /// <remarks>
    /// This expression is evaluated at runtime and its value becomes the return value
    /// of the function. The expression's type must be compatible with the function's
    /// declared return type.
    /// </remarks>
    public ExprNode expr;
    
    /// <summary>
    /// Creates a return expression node.
    /// </summary>
    /// <param name="retToken">The RETURN keyword token.</param>
    /// <param name="expr">The expression to evaluate and return.</param>
    public ReturnExprNode(Token retToken, ExprNode expr) : base(retToken)
    {
        this.expr = expr;
    }

    /// <summary>
    /// Gets the child nodes of this return expression statement.
    /// </summary>
    /// <returns>A list containing only the expression being returned.</returns>
    /// <remarks>
    /// The expression subtree is included so tree traversal algorithms can visit
    /// and analyze the returned expression.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>(){expr};
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

/// <summary>
/// Represents a return statement that returns no value (void).
/// </summary>
/// <remarks>
/// <para>
/// Used in two scenarios:
/// </para>
/// <list type="number">
/// <item>Functions with void return type that need to exit early</item>
/// <item>The final return at the end of a void function (optional in some languages)</item>
/// </list>
/// <para>
/// Example: In a function declared as <c>func printMessage()</c> (no return type = void),
/// the statement <c>return</c> would be represented as a ReturnVoidNode.
/// </para>
/// </remarks>
public class ReturnVoidNode : ReturnNode 
{
    /// <summary>
    /// Creates a void return node.
    /// </summary>
    /// <param name="retToken">The RETURN keyword token.</param>
    public ReturnVoidNode(Token retToken) : base(retToken)
    {
    }

    /// <summary>
    /// Gets the child nodes of this void return statement.
    /// </summary>
    /// <returns>An empty list, as void returns have no expression to evaluate.</returns>
    /// <remarks>
    /// Returns an empty list because there is no expression subtree to traverse.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
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
