/*
 * File: BitNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */
using ASM;
/// <summary>
/// Represents a bitwise logical operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles BITOP tokens: <c>&amp;</c> (AND), <c>|</c> (OR), and <c>^</c> (XOR).
/// </remarks>
public class BitNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for bitwise operators.
    /// </summary>
    /// <remarks>
    /// Bitwise operations are legal for the following type pairings:
    /// <list type="bullet">
    /// <item>Int [&amp; | ^] Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBitOperands = 
    [
        new(VarType.Int, VarType.Int),
    ];

    /// <summary>
    /// Creates a new bitwise logical operation node.
    /// </summary>
    /// <param name="tok">The BITOP token (<c>&amp;</c>, <c>|</c>, or <c>^</c>).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public BitNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalBitOperands)
    {
    }

    /// <summary>
    /// Type validation for bitwise operation nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        base.genCode();
        switch (this.token.lexeme)
        {
            case "&":
                Asm.emit(
                    new Comment("*** OpBitAnd ***"),
                    new OpBitAnd(left: Register.rax, right: Register.rbx)
                );
                this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                break;
            case "|":
                Asm.emit(
                    new Comment("*** OpBitOr ***"),
                    new OpBitOr(left: Register.rax, right: Register.rbx)
                );
                this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                break;
            case "^":
                Asm.emit(
                    new Comment("*** OpBitXor ***"),
                    new OpXor(left: Register.rax, right: Register.rbx)
                );
                this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                break;
        }
        
    }
}
