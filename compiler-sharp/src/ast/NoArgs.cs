/*
 * File: NoArgs.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 11, 2026
 */

/// <summary>
/// Represents an empty argument list for function calls.
/// </summary>
/// <remarks>
/// This sentinel class is used to distinguish between function calls with
/// no arguments and those with arguments. It allows the parser to uniformly
/// handle the right operand of FunccallNode while still identifying zero-argument
/// calls during evaluation or code generation.
/// </remarks>
public class NoArgs : ExprNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoArgs"/> class with a default token.
    /// </summary>
    /// <remarks>
    /// Passes a default <see cref="Token"/> to the base <see cref="ExprNode"/> constructor,
    /// as this sentinel node does not correspond to any token in the source text.
    /// </remarks>
    public NoArgs(Token tok) : base(tok)
    {
        
    }

    /// <summary>
    /// Gets the child nodes of this void return statement.
    /// </summary>
    /// <returns>An empty list, as void returns have no expression to evaluate.</returns>
    /// <remarks>
    /// Returns an empty list because there is no expression subtree to traverse.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
    }

    public override void setType()
    {
        throw new NotImplementedException();
    }

    public override void typeCheck()
    {
        throw new NotImplementedException();
    }
}