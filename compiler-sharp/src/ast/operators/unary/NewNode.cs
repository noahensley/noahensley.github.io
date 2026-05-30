/*
 * File: NewNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 */

using System.Diagnostics;

/// <summary>
/// Represents a new keyword in the expression tree.
/// </summary>
/// <remarks>
/// Handles the NEWOP token <c>new</c>.
/// </remarks>
public class NewNode : UnaryOperator
{
    /// <summary>
    /// Creates a new NEWOP node.
    /// </summary>
    /// <param name="tok">The NEWOP token (<c>new</c>).</param>
    /// <param name="term">The single operand expression.</param>
    public NewNode(Token tok, ExprNode term) : base(tok, term, [])
    {
        FunccallNode? fnode = term as FunccallNode;
        if (fnode == null)
        {
            Utils.error(new InvalidExpression($"Unsupported use of new keyword RHS.  Got {term}; expected function call."));
            throw new UnreachableException();
        }

        var className = fnode.left.token;
        if (className.sym != TokenSymbols.ID)
            Utils.error(new InvalidExpression($"Cannot create new instance of non-class.  Got {className.sym}; expected {TokenSymbols.ID}"));

        this.term = new NewType(className);
    }

    /// <summary>
    /// Type validation for NEWOP nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }
}

/// <summary>
/// Represents a type construction expression.
/// </summary>
/// <remarks>
/// NewType nodes correspond to expressions that create a new instance
/// of a class or type.
/// </remarks>
public class NewType : Term
{
    /// <summary>
    /// Creates a new type construction node.
    /// </summary>
    /// <param name="token">The token representing the constructed type.</param>
    public NewType(Token token) : base(token) {}
}
