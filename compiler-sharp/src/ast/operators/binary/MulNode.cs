/*
 * File: MulNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Represents a multiplication, division, or modulo operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles MULOP tokens including: * (multiplication), / (division), and % (modulo).
/// </remarks>
using ASM;
public class MulNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for multiplication, division, and modulo operators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Multiplication, division, and modulo operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int [*/%] Int → Int</item>
    /// <item>Float [*/%] Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalMulOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];

    /// <summary>
    /// Creates a new multiplication/division/modulo node.
    /// </summary>
    /// <param name="tok">The MULOP token (*, /, or %).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public MulNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalMulOperands)
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
        if (this.type == null)
            throw new Exception($"Internal compiler error: got MulNode operands of uninitialized type.");

        base.genCode(); // moves both float and int L/R

        if (this.type == VarType.Int)
        {
            switch (this.token.lexeme)
            {
                case "*":
                    Asm.emit(
                        new Comment($"*** int multiplication {this.token} ***"),
                        new OpMul(Register.rax, Register.rbx)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                        break;
                case "/":
                    Asm.emit(
                        new Comment($"*** int division {this.token} ***"),
                        new OpCQO(),
                        new OpDiv(Register.rbx)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
                    break;
                case "%":
                    Asm.emit(
                        new Comment($"*** int modulo {this.token} ***"),
                        new OpCQO(),
                        new OpDiv(Register.rbx)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.rdx, StorageClass.STATIC);
                    break;
                default:
                    throw new Exception($"Internal compiler error: got unexpected MulNode lexeme '{this.token.lexeme}'");
            }
        }
        else // VarType.Float
        {       
            switch (this.token.lexeme)
            {
                case "*":
                    Asm.emit(
                        new Comment("*** float multiplication ***"),
                        new OpFMul(left: Register.xmm0, right: Register.xmm1)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.xmm0, StorageClass.STATIC);
                    break;
                case "/":
                    Asm.emit(
                        new Comment("*** float division ***"),
                        new OpFDiv(left: Register.xmm0, right: Register.xmm1)
                    );
                    this.getResultLocation()!.copyFromRegister(Register.xmm0, StorageClass.STATIC);
                    break;
                case "%":
                    throw new NotImplementedException();
                    
                default:
                    throw new Exception($"Internal compiler error: got unexpected MulNode lexeme '{this.token.lexeme}'");
            }
        }
    }
}
