/*
 * File: Term.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

/// <summary>
/// Represents a terminal expression node (a literal value or identifier).
/// </summary>
/// <remarks>
/// Terminal nodes are leaf nodes in the expression tree with no child expressions.
/// They represent the basic building blocks of expressions: integer literals (NUM),
/// floating-point literals (FNUM), string literals (STRINGCONST), boolean literals
/// (BOOLCONST), and variable or function references (ID).
/// </remarks>
using ASM;
public class Term : ExprNode
{
    /// <summary>
    /// Creates a new terminal expression node.
    /// </summary>
    /// <param name="term">The token representing this terminal (NUM, ID, FNUM, STRINGCONST, or BOOLCONST).</param>
    public Term(Token term) : base(term)
    {
    }

    /// <summary>
    /// Gets the child nodes of this terminal.
    /// </summary>
    /// <returns>An empty list, as terminal nodes have no children.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
    }

    /// <summary>
    /// Assigns the type of this terminal node based on its token symbol.
    /// </summary>
    /// <remarks>
    /// Maps token symbols to their corresponding <see cref="VarType"/> singletons:
    /// NUM → Int, FNUM → Float, STRINGCONST → StringConst, BOOLCONST → BoolConst.
    /// </remarks>
    /// <exception cref="InvalidTermType">
    /// Reported via <see cref="Utils.error"/> when the token symbol is not one of the
    /// recognized literal types.
    /// </exception>
    public override void setType()
    {
        switch (this.token.sym)
        {
            case TokenSymbols.NUM:
                this.type = VarType.Int;
                return;
            case TokenSymbols.FNUM:
                this.type = VarType.Float;
                return;
            case TokenSymbols.STRINGCONST:
                this.type = VarType.StringConst;
                return;
            case TokenSymbols.BOOLCONST:
                this.type = VarType.BoolConst;
                return;
            case TokenSymbols.ID:
                this.type = new ClassType(this.token, null);
                return;
            default:
                Utils.error(new InvalidTermType($"Term with unsupported type on line {this.token.line}, got: {this.token.sym}"));
                return;
        }
    }

    /// <summary>
    /// Type validation for terminal nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }

    public override void genCode()
    {
        if(this.temporary == null)
            throw new Exception("Internal compiler error: got uninitialized expression temporary during ASM code generation.");

        switch (this.token.sym)
        {
            case TokenSymbols.NUM:
                long v = Int64.Parse(this.token.lexeme);
                Asm.emit(new Comment($"*** constant {this.token} ***"));
                Asm.emit(new OpMovConstReg(value: v, dst: Register.rax));
                this.temporary.copyFromRegister(Register.rax, StorageClass.STATIC);
                return;
            case TokenSymbols.FNUM:
                double f = Double.Parse(this.token.lexeme);
                long bits = BitConverter.DoubleToInt64Bits(f);
                Asm.emit(new Comment($"*** constant {this.token} ***"));
                Asm.emit(new OpMovConstReg(value: bits, dst: Register.rax));
                Asm.emit(new OpMovRegReg(src: Register.rax, dst: Register.xmm0));
                this.temporary.copyFromRegister(Register.xmm0, StorageClass.STATIC);
                return;
            case TokenSymbols.BOOLCONST:
                string val = this.token.lexeme == "true" ? "1" : "0";
                v = Int64.Parse(val);
                Asm.emit(new Comment($"*** constant {this.token} ***"));
                Asm.emit(new OpMovConstReg(value: v, dst: Register.rax));
                this.temporary.copyFromRegister(Register.rax, StorageClass.STATIC);
                return;
            case TokenSymbols.STRINGCONST:
                Asm.emit(new Comment($"*** constant {this.token} ***"));
                Asm.emit(
                    new OpMovLabelAddrReg(StringPool.add(this.token.lexeme), dst: Register.rax)
                );
                this.temporary!.copyFromRegister(Register.rax, StorageClass.STATIC);
                return;
            default:
                throw new NotImplementedException();
        }
    }
}

/// <summary>
/// A sentinel <see cref="VarType"/> that never compares equal to any other type.
/// </summary>
/// <remarks>
/// Used exclusively by <see cref="CastType"/> to prevent the standard binary operator
/// type-matching logic from inadvertently treating the cast's right operand as a value type.
/// </remarks>
public class NeverMatchesAnyType : VarType
{
    /// <summary>
    /// Always returns <c>false</c>, regardless of the comparison target.
    /// </summary>
    /// <param name="other">The object to compare against.</param>
    /// <returns><c>false</c> unconditionally.</returns>
    public override bool Equals(object? other)
    {
        return false;
    }

    /// <summary>
    /// Returns a hash code by delegating to the base implementation.
    /// </summary>
    /// <remarks>
    /// This class should not be used as a dictionary key; doing so may cause unexpected
    /// collisions with the base <see cref="VarType"/> hash code.
    /// </remarks>
    /// <returns>A hash code from the base implementation.</returns>
    public override int GetHashCode()
    {
        // this class cannot be hashed; will cause hashing collisions with base type
        return base.GetHashCode();
    }
}
