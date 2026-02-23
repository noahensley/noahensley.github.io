/*
 * File: EqNeqNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents an equal or not equal relational comparison operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles EQNEQOP token ==, !=
/// </remarks>
public class EqNeqNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for equality comparison operators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Equality comparison operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Bool [==|!=] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalEqNeqOperands = 
    [
        new(VarType.Int, VarType.BoolConst),
        new(VarType.Float, VarType.BoolConst),
        new(VarType.StringConst, VarType.BoolConst),
        new(VarType.BoolConst, VarType.BoolConst),
    ];

    /// <summary>
    /// Creates a new relational comparison operation node.
    /// </summary>
    /// <param name="tok">The EQNEQOP token (==, !=).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public EqNeqNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalEqNeqOperands)
    {
    }

    public override void setType()
    {
        base.setType();
    }

    public override void typeCheck()
    {
        if (left.type != right.type)
            Environment.Exit(1);
    }
}