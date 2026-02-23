/*
 * File: DotNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

/// <summary>
/// Represents a member access operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles the DOT operator (.) for accessing members of a structure or union.
/// See: https://www.tutorialspoint.com/cprogramming/c_dot_operator.htm
/// </remarks>
public class DotNode : BinaryOperator
{
    /// <summary>
    /// Creates a new member access operation node.
    /// </summary>
    /// <param name="tok">The DOT token (.).</param>
    /// <param name="structure">The structure or union expression.</param>
    /// <param name="member">The member being accessed.</param>
    public DotNode(Token tok, ExprNode structure, ExprNode member) : base(tok, structure, member, [])
    {
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}