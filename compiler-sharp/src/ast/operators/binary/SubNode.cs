/*
 * File: SubNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */
using ASM;
/// <summary>
/// Represents a subtraction operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles SUBOP token -.
/// </remarks>
public class SubNode : BinaryOperator 
{
    /// <summary>
    /// Defines the valid operand type combinations for the subtraction operator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subtraction is legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Int - Int → Int</item>
    /// <item>Float - Float → Float</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalSubOperands = 
    [
        new(VarType.Int, VarType.Int),
        new(VarType.Float, VarType.Float)
    ];
    /// <summary>
    /// Creates a new subtraction node.
    /// </summary>
    /// <param name="tok">The SUBOP token (-).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public SubNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalSubOperands)
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
            Asm.emit(
                new Comment("*** OpSub ***"),
                new OpSub(left: Register.rax, right: Register.rbx)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
        else if (this.left.type == VarType.Float)
        {
            Asm.emit(
                new Comment("*** OpFSub ***"),
                new OpFSub(left: Register.xmm0, right: Register.xmm1)
            );
            this.getResultLocation()!.copyFromRegister(Register.xmm0, StorageClass.STATIC);
        }
    }

}
