/*
 * File: GreaterLessNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

using ASM;


/// <summary>
/// Represents less-than [equal] or greater-than [equal] relational comparison operations in the expression tree.
/// </summary>
/// <remarks>
/// Handles GLOP tokens including: &lt; (less than), &gt; (greater than), &lt;= (less than or equal), and &gt;= (greater than or equal).
/// </remarks>
public class GreaterLessNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for relational comparison operators.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Relational comparison operations are legal for the following type pairings:
    /// </para>
    /// <list type="bullet">
    /// <item>Bool [&lt;|&gt;|&lt;=|&gt;=] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalGreaterLessOperands = 
    [
        new(VarType.Int, VarType.BoolConst),
        new(VarType.Float, VarType.BoolConst),
        new(VarType.StringConst, VarType.BoolConst),
    ];
    /// <summary>
    /// Creates a new relational comparison operation node.
    /// </summary>
    /// <param name="tok">The GLOP token (&lt;, &gt;, &lt;=, or &gt;=).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public GreaterLessNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalGreaterLessOperands)
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

        if (this.left.type == VarType.Int)
        {
            string cc = this.token.lexeme switch
            {
                ">"  => "g",
                ">=" => "ge",
                "<"  => "l",
                "<=" => "le",
                _ => throw new Exception($"Internal compiler error: unexpected GreaterLessNode lexeme '{this.token.lexeme}'")
            };
            Asm.emit(
                new Comment($"*** int relational {this.token.lexeme} ***"),
                new OpCmpRegReg(left: Register.rax, right: Register.rbx),
                new OpSetCC(cc)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
        else if (this.left.type == VarType.Float)
        {
            string cc = this.token.lexeme switch
            {
                ">"  => "nle",
                ">=" => "nlt",
                "<"  => "lt",
                "<=" => "le",
                _ => throw new Exception($"Internal compiler error: unexpected GreaterLessNode lexeme '{this.token.lexeme}'")
            };
            Asm.emit(
                new Comment($"*** float relational {this.token.lexeme} ***"),
                new OpCmpSD(cc, left: Register.xmm0, right: Register.xmm1),
                new OpMovRegReg(src: Register.xmm0, dst: Register.rax),
                new OpAndRegConst(Register.rax, 1)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
        }
    }
}
