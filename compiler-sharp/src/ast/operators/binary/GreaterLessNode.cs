/*
 * File: GreaterLessNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

/// <summary>
/// Represents less-than [equal] or greater-than [equal] relational comparison operations in the expression tree.
/// </summary>
/// <remarks>
/// Handles GLOP tokens including: &lt; (less than), &gt; (greater than), &lt;= (less than or equal), and &gt;= (greater than or equal).
/// </remarks>
public class GreaterLessNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for relational comparison operators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Relational comparison operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Bool [&lt;|&gt;|&lt;=|&gt;=] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalGreaterLessOperands = 
    [
        new(VarType.Int, VarType.BoolConst),
        new(VarType.Float, VarType.BoolConst),
        new(VarType.StringConst, VarType.BoolConst)
    ];
    /// <summary>
    /// Creates a new relational comparison operation node.
    /// </summary>
    /// <param name="tok">The GLOP token (&lt;, &gt;, &lt;=, or &gt;=).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public GreaterLessNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalGreaterLessOperands)
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}