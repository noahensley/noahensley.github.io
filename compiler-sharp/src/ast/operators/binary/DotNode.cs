/*
 * File: DotNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

using System.Diagnostics;

/// <summary>
/// Represents a member access operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the DOTOP token <c>.</c> for accessing fields or methods of a structure.
/// Type inference is not yet implemented for this node; an empty <see cref="LegalOperand"/>
/// array is passed to the base class.
/// </remarks>
public class DotNode : BinaryOperator
{
    /// <summary>
    /// Creates a new member access node.
    /// </summary>
    /// <param name="tok">The DOTOP token (<c>.</c>).</param>
    /// <param name="left">The expression whose member is being accessed.</param>
    /// <param name="right">The member identifier expression.</param>
    public DotNode(Token token, ExprNode left, ExprNode right) : base(token, left, right, [])
    {
        if (right.token.sym != TokenSymbols.ID)
            Utils.error(new InvalidExpression($"Expected an ID for dot operator right on line {right.token.line}; Got {right.token.sym}"));
        
        this.left = left;
        Member member = new Member(right.token);
        this.right = member;
    }

    /// <summary>
    /// Infers and assigns the resulting type of this member access expression.
    /// </summary>
    /// <remarks>
    /// This method verifies that the left-hand expression evaluates to a
    /// <see cref="ClassType"/> and performs a field lookup using the identifier
    /// on the right-hand side. The resolved field type is assigned to both the
    /// <see cref="Member"/> node and this expression node.
    /// 
    /// Errors are reported if:
    /// <list type="bullet">
    /// <item>The left operand is not a class type.</item>
    /// <item>The referenced class has not been declared.</item>
    /// </list>
    /// </remarks>
    public override void setType()
    {
        ClassType? ctype = left.type as ClassType;
        if (ctype == null)
        {
            Utils.error(new InvalidExpression($"Using dot operator on non-class left on line {token.line}"));
            throw new UnreachableException();
        }

        if ((this.right as Member) == null)
            throw new Exception("Internal compiler error: DotNode constructor failed to set RHS as a Member.");
			
        if (ctype.declarer == null)
            Utils.error(new UndeclaredClass($"Class on line {ctype.name.line} is undeclared."));
			
        Member? m = this.right as Member;
        if (m == null)
            throw new Exception($"Internal compiler error: expected DotNode RHS to be member; got {this.right}");
            
        string fieldName = right.token.lexeme;
        m.type = ctype.lookup(fieldName);
        m.declaringClassType = ctype;
        this.type = m.type;
    }

    /// <summary>
    /// Type validation for member access nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return;
    }
}