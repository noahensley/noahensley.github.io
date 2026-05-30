/*
 * File: PowNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Represents an exponentiation operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the POW operator (**) for raising a value to a power.
/// </remarks>
public class PowNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the exponentiation operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Exponentiation is legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int ** Int → Int</item>
    /// <item>Float ** Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalPowOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];

    /// <summary>
    /// Creates a new exponentiation operation node.
    /// </summary>
    /// <param name="tok">The POW token (**).</param>
    /// <param name="left">The base operand expression.</param>
    /// <param name="right">The exponent operand expression.</param>
    public PowNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalPowOperands)
    {
    }

    /// <summary>
    /// Type validation for this node. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }
}
