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
/// Represents an empty argument list for a zero-argument function call.
/// </summary>
/// <remarks>
/// This sentinel class is used to distinguish between function calls with no arguments
/// and those with arguments. It allows the parser to uniformly handle the right operand
/// of <see cref="FunccallNode"/> while still identifying zero-argument calls during
/// semantic analysis or code generation.
/// </remarks>
public class NoArgs : ExprNode
{
    /// <summary>
    /// Initializes a new <see cref="NoArgs"/> node with a synthetic no-args token.
    /// </summary>
    /// <param name="tok">
    /// A synthetic <see cref="Token"/> with symbol <see cref="TokenSymbols.NOARGS"/>,
    /// passed to the base <see cref="ExprNode"/> constructor. This node does not
    /// correspond to any token in the source text.
    /// </param>
    public NoArgs(Token tok) : base(tok)
    {
        
    }

    /// <summary>
    /// Gets the child nodes of this node.
    /// </summary>
    /// <returns>An empty list, as a zero-argument sentinel has no children.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
    }

    /// <summary>
    /// Not implemented. <see cref="NoArgs"/> nodes do not participate in type inference.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public override void setType()
    {
        return;
    }

    /// <summary>
    /// Not implemented. <see cref="NoArgs"/> nodes do not participate in type checking.
    /// </summary>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public override void typeCheck()
    {
        return;
    }
}
