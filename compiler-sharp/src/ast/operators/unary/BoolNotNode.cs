/*
 * File: BoolNotNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

using ASM;

/// <summary>
/// Represents a boolean NOT operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the BOOLNOTOP token <c>not</c>.
/// </remarks>
public class BoolNotNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the boolean NOT operator.
    /// </summary>
    /// <remarks>
    /// Boolean NOT is legal for the following type:
    /// <list type="bullet">
    /// <item>not Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBoolNotOperands = 
    [
        new(VarType.BoolConst, VarType.BoolConst),
    ];

    /// <summary>
    /// Creates a new boolean NOT node.
    /// </summary>
    /// <param name="tok">The BOOLNOTOP token (<c>not</c>).</param>
    /// <param name="term">The single operand expression.</param>
    public BoolNotNode(Token tok, ExprNode term) : base(tok, term, legalBoolNotOperands)
    {
    }

    /// <summary>
    /// Type validation for boolean NOT nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        base.genCode();
        Asm.emit(
            new Comment("*** generated BoolNotNode expr ***"),
            new OpCmpRegImm(Register.rax, 0),
            new OpSetCC("e")
        );
        this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
    }
}
