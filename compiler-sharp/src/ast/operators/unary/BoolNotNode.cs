/*
 * File: BoolNotNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a boolean logical "not" operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles BOOLNOTOP token "not".
/// </remarks>
public class BoolNotNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for boolean not operator not (NOT)
    /// </summary>
    /// <remarks>
    /// <para>
    /// Boolean not (NOT) operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>[not] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBoolNotOperands = 
    [
        new(VarType.BoolConst, VarType.BoolConst),
    ];

    /// <summary>
    /// Creates a new boolean logical "not" operation node.
    /// </summary>
    /// <param name="tok">The BOOLNOTOP token (not).</param>
    /// <param name="term">The single operand expression.</param>
    public BoolNotNode(Token tok, ExprNode term) : base(tok, term, legalBoolNotOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}