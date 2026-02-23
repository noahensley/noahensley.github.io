/*
 * File: ShiftNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Represents a bitwise shift operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles SHIFTOP tokens including: &lt;&lt; (left shift) and &gt;&gt; (right shift).
/// </remarks>
public class ShiftNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for bitwise shift operators &lt;&lt; and &gt;&gt;.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bitwise shift operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [&lt;&lt;|&gt;&gt;] Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalShiftOperands = 
    [
        new(VarType.Int, VarType.Int)
    ];

    /// <summary>
    /// Creates a new bitwise shift node.
    /// </summary>
    /// <param name="tok">The SHIFTOP token (&lt;&lt; or &gt;&gt;).</param>
    /// <param name="left">The left operand expression (value to shift).</param>
    /// <param name="right">The right operand expression (shift amount).</param>
    public ShiftNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalShiftOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}