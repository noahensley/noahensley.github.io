/*
 * File: ArrayNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 11, 2026
 */

/// <summary>
/// Represents an array indexing expression node.
/// </summary>
/// <remarks>
/// Index nodes represent array or collection access operations using the
/// subscript operator (e.g., arr[index]). The left operand is the array
/// being indexed, and the right operand is the index expression.
/// </remarks>
public class ArrayNode : BinaryOperator
{
    
    /// <summary>
    /// Creates a new array indexing node.
    /// </summary>
    /// <param name="tok">The opening bracket token.</param>
    /// <param name="arr">The array or collection expression being indexed.</param>
    /// <param name="right">The index expression.</param>
    public ArrayNode(Token tok, ExprNode arr, ExprNode right) : base(tok, arr, right, [])
    {
    }

    public override void setType()
    {
        base.setType();
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}