/*
 * File: IncNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a postfix increment "++" operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles INCOP token ++.
/// </remarks>
public class IncNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the postfix increment operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Postfix increment is legal for the following types:
    /// </para>
    /// <list type="bullet">
    /// <item>Int++ → Int</item>
    /// <item>Float++ → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalIncOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];
    /// <summary>
    /// Creates a new postfix increment operation node.
    /// </summary>
    /// <param name="tok">The INCOP token (++).</param>
    /// <param name="term">The single operand expression.</param>
    public IncNode(Token tok, ExprNode term) : base(tok, term, legalIncOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}