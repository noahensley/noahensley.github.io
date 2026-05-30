/*
 * File: BitNotNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 16, 2026
 */

using ASM;


/// <summary>
/// Represents a bitwise NOT operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the BITNOTOP token <c>~</c>.
/// </remarks>
public class BitNotNode : UnaryOperator
{
    /// <summary>
    /// Defines the valid operand type combinations for the bitwise NOT operator.
    /// </summary>
    /// <remarks>
    /// Bitwise NOT is legal for the following type:
    /// <list type="bullet">
    /// <item>~Int → Int</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalBitNotOperands = 
    [
        new(VarType.Int, VarType.Int)
    ];

    /// <summary>
    /// Creates a new bitwise NOT node.
    /// </summary>
    /// <param name="tok">The BITNOTOP token (<c>~</c>).</param>
    /// <param name="term">The single operand expression.</param>
    public BitNotNode(Token tok, ExprNode term) : base(tok, term, legalBitNotOperands)
    {
    }

    /// <summary>
    /// Type validation for bitwise NOT nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        base.genCode();
        Asm.emit(
            new Comment("*** OpBitNot ***"),
            new OpBitNot(Register.rax)
        );
        this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
    }

}
