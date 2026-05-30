/*
 * File: PreDecNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a prefix decrement "--" operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles DECOP token --.
/// </remarks>
public class PreDecNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the prefix decrement operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Prefix decrement is legal for the following types:
    /// </para>
    /// <list type="bullet">
    /// <item>--Int → Int</item>
    /// <item>--Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalPreDecOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];

    /// <summary>
    /// Creates a new prefix decrement operation node.
    /// </summary>
    /// <param name="tok">The DECOP token (--).</param>
    /// <param name="term">The single operand expression.</param>
    public PreDecNode(Token tok, ExprNode term) : base(tok, term, legalPreDecOperands)
    {
    }

    public override void setType()
    {
        base.setType();
    }

    /// <summary>
    /// Type validation for this node. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; //not implemented
    }
}
