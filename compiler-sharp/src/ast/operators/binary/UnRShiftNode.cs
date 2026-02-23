/*
 * File: ShiftNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 17, 2026
 */

/// <summary>
/// Represents an unsigned right shift operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the UNRSHIFTOP token: &gt;&gt;&gt; (unsigned right shift).
/// </remarks>
public class UnRShiftNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the unsigned right shift operator &gt;&gt;&gt;.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unsigned right shift operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [&gt;&gt;&gt;] Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalUnRShiftOperands = 
    [
        new(VarType.Int, VarType.Int)
    ];

    /// <summary>
    /// Creates a new unsigned right shift node.
    /// </summary>
    /// <param name="tok">The UNRSHIFTOP token (&gt;&gt;&gt;).</param>
    /// <param name="left">The left operand expression (value to shift).</param>
    /// <param name="right">The right operand expression (shift amount).</param>
    public UnRShiftNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalUnRShiftOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}