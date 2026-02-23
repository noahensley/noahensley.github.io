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
/// Represents a comma operator expression node.
/// </summary>
/// <remarks>
/// The comma operator evaluates its left operand, discards the result,
/// then evaluates and returns its right operand. This allows multiple
/// expressions to be evaluated in sequence where a single expression
/// is expected.
/// </remarks>
public class CommaNode : BinaryOperator
{
    /// <summary>
    /// Creates a new comma operator node.
    /// </summary>
    /// <param name="tok">The comma token.</param>
    /// <param name="left">The left-hand expression to evaluate first.</param>
    /// <param name="right">The right-hand expression to evaluate and return.</param>
    public CommaNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, [])
    {
    }
    public override void typeCheck()
    {
        return; // not implemented;
    }
}