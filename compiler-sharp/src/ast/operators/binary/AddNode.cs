/*
 * File: AddNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Represents an addition operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles ADDOP token +.
/// </remarks>
public class AddNode : BinaryOperator 
{
    /// <summary>
    /// Defines the valid operand type combinations for the addition operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Addition is legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int + Int → Int</item>
    /// <item>Float + Float → Float</item>
    /// <item>StringConst + StringConst → StringConst (string concatenation)</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalAddOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float),
        new(VarType.StringConst, VarType.StringConst)
    ];
    
    /// <summary>
    /// Creates a new addition node.
    /// </summary>
    /// <param name="tok">The ADDOP token (+).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public AddNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalAddOperands)
    {
    }

    public override void setType()
    {
        base.setType();
    }

    /// <summary>
    /// Performs semantic type checking for this addition operation.
    /// </summary>
    /// <remarks>
    /// Validates that both operands have compatible types according to <see cref="legalAddOperands"/>
    /// and reports type errors if the operands are incompatible.
    /// </remarks>
    /// <exception cref="NotImplementedException">Currently not implemented.</exception>
    public override void typeCheck()
    {
        return; // not implemented
    }
}