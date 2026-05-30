/*
 * File: UnRShiftNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 17, 2026
 */
using ASM;
/// <summary>
/// Represents an unsigned right shift operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the UNRSHIFTOP token: &gt;&gt;&gt; (unsigned right shift).
/// </remarks>
public class UnRShiftNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the unsigned right shift operator &gt;&gt;&gt;.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unsigned right shift operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [&gt;&gt;&gt;] Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalUnRShiftOperands = 
    [
        new(VarType.Int, VarType.Int)
    ];

    /// <summary>
    /// Creates a new unsigned right shift node.
    /// </summary>
    /// <param name="tok">The UNRSHIFTOP token (&gt;&gt;&gt;).</param>
    /// <param name="left">The left operand expression (value to shift).</param>
    /// <param name="right">The right operand expression (shift amount).</param>
    public UnRShiftNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalUnRShiftOperands)
    {
    }

    /// <summary>
    /// Type validation for this node. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        base.genCode();

        Asm.emit(
            new Comment("*** OpShiftRightLogical ***"),
            new OpMovRegReg(src: Register.rbx, dst: Register.rcx),
            new OpShiftRightLogical(reg: Register.rax),
            new OpXor(Register.rdx, Register.rdx),
            new OpCmpRegImm(Register.rcx, 63),
            new OpCmovCC("g", src: Register.rdx, dst: Register.rax)
        );
        this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
    }

}
