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
    public abstract List<TreeNode> getChildren();

    public abstract void setType();
    public abstract void typeCheck();
}