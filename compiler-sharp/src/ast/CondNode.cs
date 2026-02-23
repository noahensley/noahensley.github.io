/*
 * File: CondNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 */

/// <summary>
/// Represents a conditional statement (if, else if, or else) in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Conditional statements control program flow based on boolean expressions. They can be
/// chained together to create if-elif-else sequences, with each clause represented by a
/// separate CondNode in the AST.
/// </para>
/// <para>
/// Conditional statements take two forms:
/// </para>
/// <list type="bullet">
/// <item>With condition: if/elif followed by an expression and statement block</item>
/// <item>Without condition: else followed by a statement block only</item>
/// </list>
/// <para>
/// For else clauses (which have no condition), the condition field contains a placeholder
/// Term node with an empty token to maintain a consistent tree structure.
/// </para>
/// </remarks>
public class CondNode : StmtNode
{
    /// <summary>
    /// The clause keyword token (IF, ELIF, or ELSE).
    /// </summary>
    /// <remarks>
    /// Identifies which type of conditional clause this node represents, which affects
    /// both code generation and semantic analysis.
    /// </remarks>
    public Token clause;
    
    /// <summary>
    /// The condition expression to evaluate (placeholder for else clauses).
    /// </summary>
    /// <remarks>
    /// For if and elif clauses, contains the boolean expression that determines whether
    /// the statement block executes. For else clauses, contains a placeholder Term node
    /// with an empty token.
    /// </remarks>
    public ExprNode condition;
    
    /// <summary>
    /// The statement block to execute when the condition is true (or for else clauses).
    /// </summary>
    public StmtsNode stmts;
    
    /// <summary>
    /// Creates a conditional node with a condition expression.
    /// </summary>
    /// <param name="clause">The clause keyword token (IF or ELIF).</param>
    /// <param name="condition">The condition expression to evaluate.</param>
    /// <param name="stmts">The statement block to execute when the condition is true.</param>
    /// <remarks>
    /// This constructor is used for if and elif clauses that have a condition expression.
    /// </remarks>
    private CondNode(Token clause, ExprNode condition, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = condition;
        this.stmts = stmts;
    }
    
    /// <summary>
    /// Creates a conditional node without a condition (else clause).
    /// </summary>
    /// <param name="clause">The ELSE keyword token.</param>
    /// <param name="stmts">The statement block to execute unconditionally.</param>
    /// <remarks>
    /// This constructor is used for else clauses. Creates a placeholder condition using
    /// an empty Term node to maintain consistent tree structure for traversal algorithms.
    /// </remarks>
    private CondNode(Token clause, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = new Term(new Token());  // Placeholder condition with empty token
        this.stmts = stmts;
    }
    
    /// <summary>
    /// Parses a conditional statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A CondNode representing the parsed conditional statement.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// if expression { statements }
    /// else if expression { statements }
    /// else { statements }
    /// </code>
    /// <para>
    /// The parser determines whether a condition is present by examining the token following
    /// the clause keyword:
    /// </para>
    /// <list type="bullet">
    /// <item>NUM token: Indicates the start of an expression (if/elif clause)</item>
    /// <item>LBRACE token: Indicates the start of a statement block with no condition (else clause)</item>
    /// </list>
    /// <para>
    /// Note: This assumes expressions always begin with NUM tokens. This will need to be
    /// updated when the language supports expressions starting with other tokens (e.g., 
    /// identifiers, parentheses, unary operators).
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the next token is not IF, ELIF, or ELSE.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when expected tokens (expression or statement block) are missing.
    /// </exception>
    public new static CondNode parse(Tokenizer T)
    {
        Token clause = new Token();
        
        // Determine which clause type we're parsing
        switch (T.peek())
        {
            case TokenSymbols.IF:
                clause = T.expect(TokenSymbols.IF);
                break;
            case TokenSymbols.ELIF:
                clause = T.expect(TokenSymbols.ELIF);
                break;
            case TokenSymbols.ELSE:
                clause = T.expect(TokenSymbols.ELSE);
                break;
            default:
                Utils.error(new InvalidCondition($"Expected condition clause at line {T.getLine()}"));
                break;
        }

        // Check if a condition expression is present
        if (T.peek() == TokenSymbols.NUM || T.peek() == TokenSymbols.FNUM || T.peek() == TokenSymbols.BOOLCONST || 
            T.peek() == TokenSymbols.STRINGCONST)
        {
            // if/elif clause with condition
            ExprNode cond = ExprNode.parse(T);
            StmtsNode stmts = StmtsNode.parse(T);
            return new CondNode(clause, cond, stmts);
        }
        else if (T.peek() == TokenSymbols.LBRACE)
        {
            // else clause without condition
            StmtsNode stmts = StmtsNode.parse(T);
            return new CondNode(clause, stmts);
        }

        Utils.error(new InvalidCondition("Expected expression or statement block after conditional clause"));
        throw new Exception(""); // Unreachable, but required for compiler
    }
    
    /// <summary>
    /// Checks if the next token can be parsed as a conditional statement.
    /// </summary>
    /// <param name="T">The tokenizer to check.</param>
    /// <returns>True if the next token is IF, ELIF, or ELSE; false otherwise.</returns>
    /// <remarks>
    /// Used by the statement parser to determine which type of statement to parse.
    /// This lookahead allows the parser to dispatch to the appropriate parsing method
    /// without consuming tokens.
    /// </remarks>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.IF || T.peek() == TokenSymbols.ELIF || T.peek() == TokenSymbols.ELSE;
    }

    /// <summary>
    /// Gets the child nodes of this conditional statement.
    /// </summary>
    /// <returns>
    /// A list containing the condition expression and statement block.
    /// </returns>
    /// <remarks>
    /// Returns both the condition (which may be a placeholder for else clauses) and the
    /// statement block. This allows tree traversal algorithms to visit all parts of the
    /// conditional structure uniformly.
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
