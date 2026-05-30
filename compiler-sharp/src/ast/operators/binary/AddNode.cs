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
using ASM;
/// <summary>
/// Represents an addition operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles ADDOP token <c>+</c>.
/// </remarks>
public class AddNode : BinaryOperator 
{
    /// <summary>
    /// Defines the valid operand type combinations for the addition operator.
    /// </summary>
    /// <remarks>
    /// Addition is legal for the following type pairings:
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
    /// <param name="tok">The ADDOP token (<c>+</c>).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public AddNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalAddOperands)
    {
    }

    /// <summary>
    /// Infers and assigns the type of this addition expression by delegating to <see cref="BinaryOperator.setType"/>.
    /// </summary>
    public override void setType()
    {
        base.setType();
    }

    /// <summary>
    /// Type validation for addition nodes. Not yet implemented.
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
            Asm.emit(
                new Comment("*** OpAdd ***"),
                new OpAdd(left: Register.rax, right: Register.rbx)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
        else if (this.left.type == VarType.Float)
        {
            Asm.emit(
                new Comment("*** OpFAdd ***"),
                new OpFAdd(left: Register.xmm0, right: Register.xmm1)
            );
            this.getResultLocation()!.copyFromRegister(Register.xmm0, StorageClass.STATIC);
        }
        else if (this.left.type == VarType.StringConst)
        {
            Asm.emit(
                new OpMovRegReg(src: Register.rsp, dst: Register.r8),  // r8 = caller's rsp (pre-shadow)
                new OpMovRegReg(src: Register.rbp, dst: Register.r9),  // r9 = caller's rbp
                new OpSubRegConst(Register.rsp, 32),
                new OpCall(new Label("concatenateStrings")),
                new OpAddRegConst(Register.rsp, 32)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.HEAP);
        }
    }
}
