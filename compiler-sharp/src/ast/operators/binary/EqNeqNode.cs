/*
 * File: EqNeqNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */
using ASM;
/// <summary>
/// Represents an equality or inequality comparison in the expression tree.
/// </summary>
/// <remarks>
/// Handles EQNEQOP tokens: <c>==</c> (equal) and <c>!=</c> (not equal).
/// Both operators always produce a <see cref="BoolConstType"/> result.
/// </remarks>
public class EqNeqNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for equality comparison operators.
    /// </summary>
    /// <remarks>
    /// Equality comparison is legal for the following type pairings (both operands must match):
    /// <list type="bullet">
    /// <item>Int [== | !=] Int → Bool</item>
    /// <item>Float [== | !=] Float → Bool</item>
    /// <item>StringConst [== | !=] StringConst → Bool</item>
    /// <item>Bool [== | !=] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalEqNeqOperands = 
    [
        new(VarType.Int, VarType.BoolConst),
        new(VarType.Float, VarType.BoolConst),
        new(VarType.StringConst, VarType.BoolConst),
        new(VarType.BoolConst, VarType.BoolConst),
    ];

    /// <summary>
    /// Creates a new equality or inequality comparison node.
    /// </summary>
    /// <param name="tok">The EQNEQOP token (<c>==</c> or <c>!=</c>).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public EqNeqNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalEqNeqOperands)
    {
    }

    /// <summary>
    /// Infers and assigns the result type of this comparison by delegating to <see cref="BinaryOperator.setType"/>.
    /// </summary>
    public override void setType()
    {
        base.setType();
    }

    /// <summary>
    /// Validates that both operands have the same type.
    /// </summary>
    /// <exception cref="InvalidExpression">
    /// Reported via <see cref="Utils.error"/> when the left and right operand types differ.
    /// </exception>
    public override void typeCheck()
    {
        if (left.type != right.type)
            Utils.error(new InvalidExpression($"Expected same types for tokens {left.token.lexeme} and {right.token.lexeme}; got {left.type} and {right.type}"));
    }

    public override void genCode()
    {
        base.genCode();

        if (this.left.type == VarType.Int || this.left.type == VarType.BoolConst)
        {
            string cc = this.token.lexeme switch
            {
                "==" => "e",
                "!=" => "ne",
                _ => throw new Exception($"Internal compiler error: unexpected EqNeqNode lexeme '{this.token.lexeme}'")
            };
            Asm.emit(
                new Comment($"*** int/bool equality {this.token.lexeme} ***"),
                new OpCmpRegReg(left: Register.rax, right: Register.rbx),
                new OpSetCC(cc)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
        else if (this.left.type == VarType.Float)
        {
            string cc = this.token.lexeme switch
            {
                "==" => "eq",
                "!=" => "neq",
                _ => throw new Exception($"Internal compiler error: unexpected EqNeqNode lexeme '{this.token.lexeme}'")
            };
            Asm.emit(
                new Comment($"*** float equality {this.token.lexeme} ***"),
                new OpCmpSD(cc, left: Register.xmm0, right: Register.xmm1),
                new OpMovRegReg(src: Register.xmm0, dst: Register.rax),
                new OpAndRegConst(Register.rax, 1)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
    }
}
