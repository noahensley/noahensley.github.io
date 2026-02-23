/*
 * File: AssignNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

/// <summary>
/// Represents an assignment operation in the expression tree.
/// </summary>
/// <remarks>
/// Handles assignment operators including: = (simple assignment) and compound assignments (+=, -=, *=, /=, etc.).
/// </remarks>
public class AssignNode : BinaryOperator
{
    /// <summary>
    /// Creates a new assignment operation node.
    /// </summary>
    /// <param name="tok">The assignment token (=, +=, -=, etc.).</param>
    /// <param name="left">The target variable or lvalue expression.</param>
    /// <param name="right">The value expression to assign.</param>
    public AssignNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, right, [])
    {   
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}