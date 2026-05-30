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
/// Represents an array indexing expression in the expression tree.
/// </summary>
/// <remarks>
/// Handles the array subscript operator (e.g., <c>arr[index]</c>). The left operand is the
/// array or collection being indexed; the right operand is the index expression. An empty
/// <see cref="LegalOperand"/> array is passed to the base class because type inference for
/// array access requires custom logic not yet implemented.
/// </remarks>
public class ArrayNode : BinaryOperator
{
    /// <summary>
    /// Creates a new array indexing node.
    /// </summary>
    /// <param name="tok">The LBRACK token (<c>[</c>) that triggered the array-access operator.</param>
    /// <param name="arr">The array or collection expression being indexed.</param>
    /// <param name="right">The index expression.</param>
    public ArrayNode(Token tok, ExprNode arr, ExprNode right) : base(tok, arr, right, [])
    {
    }

    /// <summary>
    /// Type inference for array indexing nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        base.setType();
    }

    /// <summary>
    /// Type validation for array indexing nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return; // not implemented
    }
}
