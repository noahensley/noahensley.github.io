/*
 * File: ShiftNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */
using ASM;
/// <summary>
/// Represents a bitwise shift operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles SHIFTOP tokens including: &lt;&lt; (left shift) and &gt;&gt; (right shift).
/// </remarks>
public class ShiftNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for bitwise shift operators &lt;&lt; and &gt;&gt;.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bitwise shift operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [&lt;&lt;|&gt;&gt;] Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalShiftOperands = 
    [
        new(VarType.Int, VarType.Int)
    ];

    /// <summary>
    /// Creates a new bitwise shift node.
    /// </summary>
    /// <param name="tok">The SHIFTOP token (&lt;&lt; or &gt;&gt;).</param>
    /// <param name="left">The left operand expression (value to shift).</param>
    /// <param name="right">The right operand expression (shift amount).</param>
    public ShiftNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalShiftOperands)
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
        if (this.left.type != this.right.type)
            throw new Exception("Internal compiler error: cannot generate asm code for type mismatched addition expr");

        base.genCode();

        if (this.left.type == VarType.Int)
        {
            switch (this.token.lexeme)
            {
                case "<<":
                    Asm.emit(
                        new Comment("*** OpShiftLeft ***"),
                        new OpMovRegReg(src: Register.rbx, dst: Register.rcx),
                        new OpShiftLeft(reg: Register.rax),
                        new OpXor(Register.rdx, Register.rdx),
                        new OpCmpRegImm(Register.rcx, 63),
                        new OpCmovCC("g", src: Register.rdx, dst: Register.rax)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                    break;
                case ">>":
                    Asm.emit(
                        new Comment("*** OpShiftRightArithmetic ***"),
                        new OpMovRegReg(src: Register.rbx, dst: Register.rcx),
                        new OpCmpRegImm(Register.rcx, 63),
                        new OpMovConstReg(value: 63, dst: Register.rdx),
                        new OpCmovCC("g", src: Register.rdx, dst: Register.rcx),
                        new OpShiftRightArithmetic(reg: Register.rax)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                    break;
                default:
                    throw new Exception($"Internal compiler error: invalid token for ShiftNode lexeme {this.token.lexeme}");
            }
        }
        else
            throw new Exception($"Internal compiler error: did not catch non-integer operands for ShiftNode {this.token}");
    }
}
