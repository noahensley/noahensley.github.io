/*
 * File: SubNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a subtraction operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles SUBOP token -.
/// </remarks>
public class SubNode : BinaryOperator 
{
    /// <summary>
    /// Defines the valid operand type combinations for the subtraction operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subtraction is legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int - Int → Int</item>
    /// <item>Float - Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalSubOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];
    /// <summary>
    /// Creates a new subtraction node.
    /// </summary>
    /// <param name="tok">The SUBOP token (-).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public SubNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalSubOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}