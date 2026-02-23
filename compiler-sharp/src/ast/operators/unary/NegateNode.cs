/*
 * File: NegateNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents a negation operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles SUBOP token -.
/// </remarks>
public class NegateNode : UnaryOperator 
{
    /// <summary>
    /// Defines the valid operand type combinations for the negation operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Negation is legal for the following types:
    /// </para>
    /// <list type="bullet">
    /// <item>-Int → Int</item>
    /// <item>-Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalNegateOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float),
    ];
    
    /// <summary>
    /// Creates a new negation node.
    /// </summary>
    /// <param name="tok">The SUBOP token (-).</param>
    /// <param name="left">The single operand expression.</param>
    public NegateNode(Token tok, ExprNode term) : base(tok, term, legalNegateOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}