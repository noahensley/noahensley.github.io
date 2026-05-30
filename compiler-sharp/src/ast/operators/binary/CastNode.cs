/*
 * File: CastNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 */

using System.Net.Http.Headers;
using ASM;


/// <summary>
/// Represents a type cast expression in the expression tree.
/// </summary>
/// <remarks>
/// Handles the CASTOP keyword <c>as</c>. The left operand is the value being cast;
/// the right operand is a <see cref="CastType"/> node carrying the target type keyword.
/// </remarks>
public class CastNode : BinaryOperator 
{
    /// <summary>
    /// Defines the valid source-to-target type combinations for the cast operator.
    /// </summary>
    /// <remarks>
    /// Cast operations are legal for the following combinations:
    /// <list type="bullet">
    /// <item>Int → Int (no-op), Float, StringConst</item>
    /// <item>Float → Int, Float (no-op), StringConst</item>
    /// <item>StringConst → Int, Float, StringConst (no-op)</item>
    /// <item>BoolConst → BoolConst (no-op)</item>
    /// </list>
    /// </remarks>
    private static readonly LegalOperand[] legalCastOperands = 
    [
        // Integers => Integers (no-op), Floats, Strings
        new(VarType.Int, VarType.Int),
        new(VarType.Int, VarType.Float),
        new(VarType.Int, VarType.StringConst),

        // Floats => Integers, Floats (no-op), Strings
        new(VarType.Float, VarType.Int),
        new(VarType.Float, VarType.Float),
        new(VarType.Float, VarType.StringConst),

        // Strings => Integers, Strings (no-op)
        new(VarType.StringConst, VarType.Int),
        new(VarType.StringConst, VarType.Float),
        new(VarType.StringConst, VarType.StringConst),

        // Booleans => Booleans (no-op)
        new(VarType.BoolConst, VarType.BoolConst)
    ];
    
    /// <summary>
    /// Creates a new cast node.
    /// </summary>
    /// <param name="tok">The CASTOP token (<c>as</c>).</param>
    /// <param name="left">The expression being cast (the source value).</param>
    /// <param name="right">A <see cref="CastType"/> node representing the target type.</param>
    public CastNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, legalCastOperands)
    {
    }

    /// <summary>
    /// Assigns this node's type directly from the target type specified by the right operand.
    /// </summary>
    /// <remarks>
    /// Reads the type keyword from the right operand's token (e.g., <c>int</c>, <c>float</c>)
    /// via <see cref="VarType.fromToken"/>. This overrides the standard binary operator type
    /// inference because a cast always produces the declared target type, not a type derived
    /// from the operands' common type.
    /// </remarks>
    public override void setType()
    {
        if ((this.right as CastType) == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type setting phase " +
                $"while processing function '{this.token.lexeme}' at line {this.token.line}"
            );
        this.type = VarType.fromToken(this.right.token);
    }

    /// <summary>
    /// Validates that the source-to-target type combination is a permitted cast.
    /// </summary>
    /// <remarks>
    /// Checks the source type (<see cref="BinaryOperator.left"/>.type) against the resolved
    /// target type (<see cref="ExprNode.type"/>) using <see cref="legalCastOperands"/>.
    /// </remarks>
    /// <exception cref="InvalidExpression">
    /// Reported via <see cref="Utils.error"/> when the source or target type is null, or when
    /// the source-to-target combination is not listed in <see cref="legalCastOperands"/>.
    /// </exception>
    public override void typeCheck()
    {
        // Look at left.type (casting FROM) and this.type (casting TO)
        //  and verify they are compatible
        if (this.left.type == null || this.type == null)
            Utils.error(new InvalidExpression($"Uninitialized CastNode type on line {this.token.line}"));
        else
        {
            foreach (var le in legalCastOperands)
            {
                if (this.left.type == le.operandType && this.type == le.resultType)
                    return;
            }
        }
        Utils.error(new InvalidExpression($"Cannot cast from {this.left.type} to {this.type} on line {this.token.line}"));
    }

    public override void genCode()
    {
        if (this.left.type == VarType.Int && this.right.token.lexeme == "string") // fix later
        {
            this.left.genCode();
            this.left.getResultLocation()!.copyToRegister(Register.rcx, null);
            Asm.emit(
                new OpMovRegReg(src: Register.rsp, dst:Register.rdx),
                new OpMovRegReg(src: Register.rbp, dst: Register.r8),
                new OpSubRegConst(Register.rsp, 32),
                new OpCall(new Label("intToString")),
                new OpAddRegConst(Register.rsp, 32)
            );
            this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.HEAP); // we know it's dynamic
        }
    }
}

/// <summary>
/// Represents the target type operand of a cast expression (the right-hand side of <c>as</c>).
/// </summary>
/// <remarks>
/// This node appears as the right child of a <see cref="CastNode"/>. Its type is always
/// <see cref="NeverMatchesAnyType"/>, which prevents the standard binary operator type-matching
/// logic from firing and allows <see cref="CastNode.setType"/> to derive the result type directly
/// from the token lexeme.
/// </remarks>
public class CastType : Term
{
    /// <summary>
    /// Creates a new cast-target type node.
    /// </summary>
    /// <param name="token">The TYPE keyword token representing the target type (e.g., <c>int</c>, <c>float</c>).</param>
    public CastType(Token token) : base(token) {}

    /// <summary>
    /// Assigns <see cref="NeverMatchesAnyType"/> as this node's type.
    /// </summary>
    /// <remarks>
    /// This sentinel type prevents the standard <see cref="BinaryOperator.setType"/> logic
    /// from treating the cast target as a normal operand during type inference.
    /// </remarks>
    public override void setType()
    {
        this.type = new NeverMatchesAnyType();
    }
}