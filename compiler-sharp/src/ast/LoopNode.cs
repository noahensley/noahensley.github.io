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

using System.Diagnostics;
using ASM;

/// <summary>
/// Abstract base class for loop statements in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Provides shared structure and behavior for all loop statement types (e.g., while, repeat).
/// Concrete subclasses implement parsing and code generation specific to each loop form.
/// </para>
/// <para>
/// Loop statements can take two forms:
/// </para>
/// <list type="bullet">
/// <item>With condition: loop keyword followed by an expression and statement block.</item>
/// <item>Without condition: loop keyword followed directly by a statement block (infinite loop).</item>
/// </list>
/// <para>
/// When no condition expression is present, the loop executes indefinitely unless terminated
/// by a break statement. The condition field contains a placeholder Term node with an empty
/// token to maintain a consistent tree structure.
/// </para>
/// </remarks>
public abstract class LoopNode : StmtNode
{
    /// <summary>
    /// The loop keyword token (e.g., WHILE, REPEAT).
    /// </summary>
    /// <remarks>
    /// Storing this token allows subclasses and diagnostic messages to identify
    /// which loop keyword was used and on which line.
    /// </remarks>
    public Token clause;

    /// <summary>
    /// The loop condition expression, or a placeholder for infinite loops.
    /// </summary>
    /// <remarks>
    /// For conditional loops, contains the boolean expression evaluated before each iteration.
    /// The loop continues while this expression is true. For infinite loops (no condition),
    /// contains a placeholder <see cref="Term"/> node with an empty token.
    /// </remarks>
    public ExprNode? condition;

    /// <summary>
    /// The statement block to execute in each loop iteration.
    /// </summary>
    /// <remarks>
    /// Contains all statements within the loop body. These statements execute repeatedly
    /// until the condition becomes false or a break statement is encountered.
    /// </remarks>
    public StmtsNode stmts;

    /// <summary>
    /// The label marking the top of the loop, used as the jump target for condition re-evaluation.
    /// </summary>
    public Label test = new Label();

    /// <summary>
    /// The label marking the end of the loop, used as the jump target when the condition is false.
    /// </summary>
    public Label endLoop = new Label();

    /// <summary>
    /// Initializes a loop node with a condition expression (conditional loop).
    /// </summary>
    /// <param name="clause">The loop keyword token.</param>
    /// <param name="condition">The loop condition expression to evaluate before each iteration.</param>
    /// <param name="stmts">The loop body statement block.</param>
    protected LoopNode(Token clause, ExprNode? condition, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = condition;
        this.stmts = stmts;
    }

    /// <summary>
    /// Initializes a loop node without a condition (infinite loop).
    /// </summary>
    /// <param name="clause">The loop keyword token.</param>
    /// <param name="stmts">The loop body statement block.</param>
    /// <remarks>
    /// Creates a placeholder condition using an empty <see cref="Term"/> node to maintain
    /// consistent tree structure for traversal algorithms.
    /// </remarks>
    protected LoopNode(Token clause, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = new Term(new Token());  // Placeholder condition with empty token
        this.stmts = stmts;
    }

    /// <summary>
    /// Gets the child nodes of this loop statement.
    /// </summary>
    /// <returns>
    /// A list containing the condition expression followed by the statement block.
    /// </returns>
    /// <remarks>
    /// Returns both the condition (which may be a placeholder for infinite loops) and the
    /// statement block, allowing tree traversal algorithms to visit all parts uniformly.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        if (this.condition == null)
            return new List<TreeNode>() { stmts };
        return new List<TreeNode>() { condition, stmts };
    }

    /// <summary>
    /// Type inference for loop nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // not implemented
    }

    /// <summary>
    /// Validates that the loop condition evaluates to a boolean type.
    /// </summary>
    /// <exception cref="InvalidCondition">
    /// Reported via <see cref="Utils.error"/> when the condition type is not <see cref="BoolConstType"/>.
    /// </exception>
    public override void typeCheck()
    {
        if (this.condition == null)
        {
            Utils.error(new InvalidExpression($"Got a '{this.clause.lexeme}' clause with no condition."));
            throw new UnreachableException();
        }
        if (this.condition.type != VarType.BoolConst)
            Utils.error(new InvalidCondition($"Invalid condition on line {this.clause.line}; Expected BoolConst, got {this.condition.type}"));
    }

    /// <summary>
    /// Generates assembly code for this loop statement.
    /// </summary>
    public abstract override void genCode();
}