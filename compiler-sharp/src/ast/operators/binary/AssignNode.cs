/*
 * File: AssignNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

using System.Diagnostics;
using ASM;


/// <summary>
/// Represents an assignment operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles ASSIGNOP token <c>=</c> (simple assignment). Compound assignment operators
/// (e.g., <c>+=</c>, <c>-=</c>) are not yet supported by the language and are not
/// emitted by the tokenizer.
/// </remarks>
public class AssignNode : BinaryOperator
{
    /// <summary>
    /// Creates a new assignment node.
    /// </summary>
    /// <param name="tok">The ASSIGNOP token (<c>=</c>).</param>
    /// <param name="left">The target variable or lvalue expression.</param>
    /// <param name="right">The value expression to assign.</param>
    public AssignNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, [])
    {   
    }

    /// <summary>
    /// Type inference for assignment nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // not implemented
    }

    /// <summary>
    /// Validates that the left and right operands have matching types.
    /// </summary>
    /// <exception cref="InvalidExpression">
    /// Reported via <see cref="Utils.error"/> when the left and right operand types differ.
    /// </exception>
    public override void typeCheck()
    {
        if ((right.type as FuncType) != null)
        {
            // compare against return type instead
            return; // for now
        }
        ClassType? lctype = left.type as ClassType;
        ClassType? rctype = right.type as ClassType;
        if (lctype == null && rctype == null)
        {
            if (left.type == right.type)
                return;
        }
        else if (lctype == null && rctype != null)
        {
            // compare right (class) to left (not); not yet
        }
        else if (lctype != null && rctype == null)
        {
            // compare left (class) to right (not); not yet
        }
        else if (lctype != null && rctype != null)
        {
            // compare both class types
            if (lctype.name.lexeme != rctype.name.lexeme)
                Utils.error(new InvalidExpression($"Expected matching types for assignment; got {lctype.name.lexeme} and {rctype.name.lexeme}"));
            return;
        }
           
        Utils.error(new InvalidExpression($"Expected matching types for assignment; got {left.type} and {right.type}"));
    }

    public override void genCode()
    {
        //NOTE: This does not handle array index assignment
        //  and ignores StorageClass... for now
        this.right.genCode();
        Variable? variable = this.left as Variable;
        if (variable == null)
        {
            Utils.error(new InvalidExpression($"Trying to assign a value to non-variable ({this.left.token.line},{this.left.token.column})"));
            throw new UnreachableException();
        }

        //get the address of the destination variable into rax
        variable.copyAddressToRegister(Register.rax);

        //move the value of its remporary into rbx
        this.right.getResultLocation()!.copyToRegister(Register.rbx, Register.rcx);

        //copy the value of the temporary into the destination variable
        Asm.emit(
            new Comment("copying temporary value into destination variable"),
            new OpMovRegRegInd(src: Register.rbx, dst: Register.rax, offset: 0),
            new OpMovRegRegInd(src: Register.rcx, dst: Register.rax, offset: 8)
        );
    }
}
