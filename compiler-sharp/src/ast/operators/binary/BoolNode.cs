/*
 * File: BoolNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Represents a boolean logical operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles BOOLOP tokens including: and (AND) and or (OR).
/// </remarks>
public class BoolNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for boolean operators and (AND) and or (OR)
    /// </summary>
    /// <remarks>
    /// <para>
    /// Boolean operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Bool [andor] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBoolOperands = 
    [
        new(VarType.BoolConst, VarType.BoolConst),
    ];

    /// <summary>
    /// Creates a new boolean logical operation node.
    /// </summary>
    /// <param name="tok">The BOOLOP token (&amp;&amp; or ||).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public BoolNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalBoolOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}