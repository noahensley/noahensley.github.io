/*
 * File: BitNotNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a bitwise logical "not" operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles BITNOTOP token ~.
/// </remarks>
public class BitNotNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the bitwise not operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bitwise not is legal for the following type:
    /// </para>
    /// <list type="bullet">
    /// <item>~Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBitNotOperands = 
    [
        new(VarType.Int, VarType.Int)
    ];

    /// <summary>
    /// Creates a new bitwise logical "not" operation node.
    /// </summary>
    /// <param name="tok">The BITNOTOP token (~).</param>
    /// <param name="term">The single operand expression.</param>
    public BitNotNode(Token tok, ExprNode term) : base(tok, term, legalBitNotOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}