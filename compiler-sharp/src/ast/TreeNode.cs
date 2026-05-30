/*
 * File: TreeNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026
 */

/// <summary>
/// Base class for all abstract syntax tree (AST) nodes.
/// </summary>
/// <remarks>
/// Serves as the root of the AST node hierarchy. All specific node types
/// (ProgramNode, FuncdefNode, StmtNode, ExprNode, etc.) inherit from this class.
/// This allows for polymorphic handling of tree nodes during tree traversal and analysis.
/// </remarks>
public abstract class TreeNode
{
    /// <summary>
    /// The parent node of this node in the AST, or <c>null</c> if this is the root.
    /// </summary>
    /// <remarks>
    /// Set during the parent-assignment walk that precedes semantic analysis.
    /// Used by nodes such as <see cref="ReturnNode"/> to traverse upward and locate
    /// the enclosing function definition.
    /// </remarks>
    public TreeNode? parent = null;

    /// <summary>
    /// Returns the direct children of this node for use during tree traversal.
    /// </summary>
    /// <returns>An ordered list of child <see cref="TreeNode"/> instances.</returns>
    public abstract List<TreeNode> getChildren();

    /// <summary>
    /// Infers and assigns the type of this node from its children and associated token.
    /// </summary>
    /// <remarks>
    /// Called during a post-order walk of the AST so that child types are already
    /// resolved before the parent node computes its own type. Nodes that do not
    /// participate in type inference should override this method with a no-op.
    /// </remarks>
    public abstract void setType();

    /// <summary>
    /// Validates the type constraints of this node after all types have been assigned.
    /// </summary>
    /// <remarks>
    /// Called during a second post-order walk that follows the <see cref="setType"/> pass.
    /// Implementations should report errors via <see cref="Utils.error"/> rather than
    /// throwing directly. Nodes with no type constraints should override with a no-op.
    /// </remarks>
    public abstract void typeCheck();

    public virtual void genCode()
    {
        foreach(var n in this.getChildren())
        {
            n.genCode();   
        }
    }
}
