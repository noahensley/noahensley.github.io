/*
 * File: FunccallNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 11, 2026
 */

/// <summary>
/// Represents a function call expression node.
/// </summary>
/// <remarks>
/// Call nodes represent function invocation operations. The left operand
/// is the function expression being called, and the right operand contains
/// the argument list (which may be a NoArgs instance for zero-argument calls
/// or a comma-separated expression tree for multiple arguments).
/// </remarks>
public class FunccallNode : BinaryOperator
{
    /// <summary>
    /// Creates a new function call node.
    /// </summary>
    /// <param name="tok">The opening parenthesis token.</param>
    /// <param name="func">The function expression being invoked.</param>
    /// <param name="right">The argument list expression or NoArgs instance.</param>
    public FunccallNode(Token tok, ExprNode func, ExprNode right): base(tok, func, right, [])
    {   
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}