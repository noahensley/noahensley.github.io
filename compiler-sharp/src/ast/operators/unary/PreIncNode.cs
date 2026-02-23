/*
 * File: PreIncNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a prefix increment "++" operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles INCOP token ++.
/// </remarks>
public class PreIncNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the prefix increment operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Prefix increment is legal for the following types:
    /// </para>
    /// <list type="bullet">
    /// <item>++Int → Int</item>
    /// <item>++Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalPreIncOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];

    /// <summary>
    /// Creates a new prefix increment operation node.
    /// </summary>
    /// <param name="tok">The INCOP token (++).</param>
    /// <param name="term">The single operand expression.</param>
    public PreIncNode(Token tok, ExprNode term) : base(tok, term, legalPreIncOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}