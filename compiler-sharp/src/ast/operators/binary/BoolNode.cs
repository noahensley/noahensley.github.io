/*
 * File: BoolNode.cs
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
/// Represents a boolean logical operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles BOOLOP tokens: <c>and</c> (AND) and <c>or</c> (OR).
/// </remarks>
public class BoolNode : BinaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for boolean logical operators.
    /// </summary>
    /// <remarks>
    /// Boolean logical operations are legal for the following type pairings:
    /// <list type="bullet">
    /// <item>Bool [and | or] Bool → Bool</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBoolOperands = 
    [
        new(VarType.BoolConst, VarType.BoolConst),
    ];

    /// <summary>
    /// Creates a new boolean logical operation node.
    /// </summary>
    /// <param name="tok">The BOOLOP token (<c>and</c> or <c>or</c>).</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    public BoolNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalBoolOperands)
    {
    }

    /// <summary>
    /// Type validation for boolean logical operation nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        //cannot use base binaryoperator gencode, because left and right
        // expr must be evaluated one at a time (for short-circuiting)
        this.left.genCode();
        this.left.getResultLocation()!.copyToRegister(Register.rax, null);
        Label lbl = new Label();
        string cc = this.token.lexeme == "or" ? "ne" : "e"; //changes short-circuit
        Asm.emit(
            new Comment("checking for short-circuit"),
            new OpCmpRegImm(Register.rax, 0),
            new OpJmpCC(cc, lbl)
        );

        this.right.genCode();
        this.right.getResultLocation()!.copyToRegister(Register.rax, null);

        Asm.emit(lbl);
        this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
    }
}
