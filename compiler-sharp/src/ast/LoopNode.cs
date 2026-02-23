/*
 * File: LoopNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 */

/// <summary>
/// Represents a loop statement (while) in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Loop statements enable repeated execution of code blocks based on a condition.
/// The while loop continues executing its body as long as the condition evaluates to true.
/// </para>
/// <para>
/// Loop statements can take two forms:
/// </para>
/// <list type="bullet">
/// <item>With condition: while followed by an expression and statement block</item>
/// <item>Without condition: while followed directly by a statement block (infinite loop)</item>
/// </list>
/// <para>
/// When no condition expression is present, the loop executes indefinitely (infinite loop)
/// unless terminated by a break statement. The condition field contains a placeholder
/// Term node with an empty token to maintain consistent tree structure.
/// </para>
/// </remarks>
public class LoopNode : StmtNode
{
    /// <summary>
    /// The WHILE keyword token.
    /// </summary>
    /// <remarks>
    /// Currently only while loops are supported. This field allows for future expansion
    /// to support other loop types (for, do-while, etc.) by storing which loop keyword
    /// was used.
    /// </remarks>
    public Token clause;
    
    /// <summary>
    /// The loop condition expression (placeholder for infinite loops).
    /// </summary>
    /// <remarks>
    /// For conditional loops, contains the boolean expression evaluated before each iteration.
    /// The loop continues while this expression is true. For infinite loops (no condition),
    /// contains a placeholder Term node with an empty token.
    /// </remarks>
    public ExprNode condition;
    
    /// <summary>
    /// The statement block to execute in each loop iteration.
    /// </summary>
    /// <remarks>
    /// Contains all statements within the loop body. These statements execute repeatedly
    /// until the condition becomes false or a break statement is encountered.
    /// </remarks>
    public StmtsNode stmts;
    
    /// <summary>
    /// Creates a loop node with a condition expression.
    /// </summary>
    /// <param name="clause">The WHILE keyword token.</param>
    /// <param name="condition">The loop condition expression to evaluate.</param>
    /// <param name="stmts">The loop body statement block.</param>
    /// <remarks>
    /// This constructor is used for standard while loops with a condition expression.
    /// The condition is evaluated before each iteration.
    /// </remarks>
    private LoopNode(Token clause, ExprNode condition, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = condition;
        this.stmts = stmts;
    }
    
    /// <summary>
    /// Creates a loop node without a condition (infinite loop).
    /// </summary>
    /// <param name="clause">The WHILE keyword token.</param>
    /// <param name="stmts">The loop body statement block.</param>
    /// <remarks>
    /// This constructor is used for infinite loops with no condition. Creates a placeholder
    /// condition using an empty Term node to maintain consistent tree structure for traversal
    /// algorithms. Such loops continue indefinitely unless terminated by a break statement.
    /// </remarks>
    private LoopNode(Token clause, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = new Term(new Token());  // Placeholder condition with empty token
        this.stmts = stmts;
    }
    
    /// <summary>
    /// Parses a loop statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A LoopNode representing the parsed loop statement.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// while expression { statements }
    /// while { statements }
    /// </code>
    /// <para>
    /// The parser determines whether a condition is present by examining the token following
    /// the WHILE keyword:
    /// </para>
    /// <list type="bullet">
    /// <item>NUM token: Indicates the start of an expression (conditional loop)</item>
    /// <item>LBRACE token: Indicates the start of a statement block with no condition (infinite loop)</item>
    /// </list>
    /// <para>
    /// Note: This assumes expressions always begin with NUM tokens. This will need to be
    /// updated when the language supports expressions starting with other tokens (e.g.,
    /// identifiers, parentheses, unary operators, boolean literals).
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the WHILE keyword token is not found.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when expected tokens (expression or statement block) are missing.
    /// </exception>
    public new static LoopNode parse(Tokenizer T)
    {
        Token clause = T.expect(TokenSymbols.WHILE);
        
        // Check if a condition expression is present
        if (T.peek() == TokenSymbols.NUM || T.peek() == TokenSymbols.FNUM || T.peek() == TokenSymbols.BOOLCONST ||
            T.peek() == TokenSymbols.STRINGCONST || T.peek() == TokenSymbols.BOOLNOTOP)
        {
            // Conditional loop
            ExprNode cond = ExprNode.parse(T);
            StmtsNode stmts = StmtsNode.parse(T);
            return new LoopNode(clause, cond, stmts);
        }
        else if (T.peek() == TokenSymbols.LBRACE)
        {
            // Infinite loop (no condition)
            StmtsNode stmts = StmtsNode.parse(T);
            return new LoopNode(clause, stmts);
        }

        Utils.error(new MissingExpression($"Expected expression or statement block after while keyword, got: {T.peek()}"));
        throw new Exception(""); // Unreachable, but required for compiler
    }
    
    /// <summary>
    /// Checks if the next token can be parsed as a loop statement.
    /// </summary>
    /// <param name="T">The tokenizer to check.</param>
    /// <returns>True if the next token is WHILE; false otherwise.</returns>
    /// <remarks>
    /// Used by the statement parser to determine which type of statement to parse.
    /// This lookahead allows the parser to dispatch to the appropriate parsing method
    /// without consuming tokens.
    /// </remarks>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.WHILE;
    }

    /// <summary>
    /// Gets the child nodes of this loop statement.
    /// </summary>
    /// <returns>
    /// A list containing the condition expression and statement block.
    /// </returns>
    /// <remarks>
    /// Returns both the condition (which may be a placeholder for infinite loops) and the
    /// statement block. This allows tree traversal algorithms to visit all parts of the
    /// loop structure uniformly.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>() { condition, stmts };
    }

    public override void setType()
    {
        return; // not implemented
    }

    public override void typeCheck()
    {
        if (this.condition.type != VarType.BoolConst)
            Utils.error(new InvalidCondition($"Invalid condition on line {this.clause.line}; Expected BoolConst, got {this.condition.type}"));
    }
}
