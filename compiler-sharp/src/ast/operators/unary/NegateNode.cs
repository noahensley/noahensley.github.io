/*
 * File: NegateNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

using ASM;


/// <summary>
/// Represents a unary negation operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the NEGATEOP token <c>-</c> when used as a prefix operator.
/// The token symbol is re-tagged from SUBOP to NEGATEOP by the expression parser
/// when a <c>-</c> appears in a prefix (term-expected) position.
/// </remarks>
public class NegateNode : UnaryOperator 
{
    /// <summary>
    /// Defines the valid operand type combinations for the negation operator.
    /// </summary>
    /// <remarks>
    /// Negation is legal for the following types:
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
    /// <param name="tok">The NEGATEOP token (<c>-</c>).</param>
    /// <param name="term">The single operand expression to negate.</param>
    public NegateNode(Token tok, ExprNode term) : base(tok, term, legalNegateOperands)
    {
    }

    public override void setType()
    {
        base.setType();
    }

    /// <summary>
    /// Type validation for negation nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        base.genCode();

        if (this.type == VarType.Int)
        {
            Asm.emit(
                new Comment("*** OpNegate ***"),
                new OpMovRegReg(src: Register.rbx, dst: Register.rax),
                new OpNegate(Register.rax)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
        else if (this.type == VarType.Float)
        {
            Asm.emit(
                new Comment("*** OpFNegate ***"),
                new OpMovRegReg(src: Register.xmm1, dst: Register.rax),  // extract float bits from xmm1
                new OpMovConstReg(value: long.MinValue, dst: Register.rbx),
                new OpXor(Register.rax, Register.rbx),
                new OpMovRegReg(src: Register.rax, dst: Register.xmm0)
            );
            this.getResultLocation()!.copyFromRegister(Register.xmm0, StorageClass.STATIC);
        }
    }

}
