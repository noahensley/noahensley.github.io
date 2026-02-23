/*
 * File: MulNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Represents a multiplication, division, or modulo operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles MULOP tokens including: * (multiplication), / (division), and % (modulo).
/// </remarks>
public class MulNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for multiplication, division, and modulo operators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Multiplication, division, and modulo operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [*/%] Int → Int</item>
    /// <item>Float [*/%] Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalMulOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];

    /// <summary>
    /// Creates a new multiplication/division/modulo node.
    /// </summary>
    /// <param name="tok">The MULOP token (*, /, or %).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public MulNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalMulOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}