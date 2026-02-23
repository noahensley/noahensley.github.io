/*
 * File: BitNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

/// <summary>
/// Represents a bitwise logical operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles BITOP tokens including: &amp; (AND), | (OR), and ^ (XOR).
/// </remarks>
public class BitNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for bitwise operators &amp;, |, and ^.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bitwise operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [&|^] Int → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBitOperands = 
    [
        new(VarType.Int, VarType.Int),
    ];

    /// <summary>
    /// Creates a new bitwise logical operation node.
    /// </summary>
    /// <param name="tok">The BITOP token (&amp;, |, or ^).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public BitNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalBitOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}