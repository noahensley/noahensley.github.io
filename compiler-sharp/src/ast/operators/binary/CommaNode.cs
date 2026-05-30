/*
 * File: CommaNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 11, 2026
 */

/// <summary>
/// Represents a comma-separated expression sequence in the expression tree.
/// </summary>
/// <remarks>
/// The comma operator is used to pass multiple arguments to a function call. Argument lists
/// containing two or more expressions are represented as a left-associative tree of CommaNodes,
/// traversed by <see cref="FunccallNode"/> during type checking to collect argument types in
/// source order.
/// </remarks>
public class CommaNode : BinaryOperator
{
    /// <summary>
    /// Creates a new comma operator node.
    /// </summary>
    /// <param name="tok">The COMMAOP token (<c>,</c>).</param>
    /// <param name="left">The left-hand expression.</param>
    /// <param name="right">The right-hand expression.</param>
    public CommaNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, [])
    {
    }

    /// <summary>
    /// Type validation for comma expression nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }
}
