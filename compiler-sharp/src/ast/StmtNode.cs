/*
 * File: StmtNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      February 23, 2026
 */

using System.Diagnostics;

/// <summary>
/// Abstract base class for all statement nodes in the abstract syntax tree.
/// </summary>
/// <remarks>
/// Statement nodes represent executable code constructs including:
/// <list type="bullet">
/// <item><description>Return statements (ReturnNode)</description></item>
/// <item><description>Conditional statements (CondNode)</description></item>
/// <item><description>Loop statements (LoopNode)</description></item>
/// <item><description>Expression statements (ExprNode)</description></item>
/// <item><description>Variable declarations (VardeclNode)</description></item>
/// </list>
/// Additional statement types can be added by extending this class.
/// </remarks>
public abstract class StmtNode : TreeNode 
{
    /// <summary>
    /// Parses a statement from the token stream by delegating to specific statement parsers.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A specific StmtNode subclass instance (ReturnNode, CondNode, LoopNode, ExprNode,
    /// or VardeclNode).</returns>
    /// <remarks>
    /// Uses the canParse() methods of each statement type to determine which parser to invoke.
    /// Checks statement types in the following order:
    /// <list type="number">
    /// <item>If ReturnNode.canParse(): parse and return a ReturnNode</item>
    /// <item>If CondNode.canParse(): parse and return a CondNode</item>
    /// <item>If LoopNode.canParse(): parse and return a LoopNode</item>
    /// <item>If ExprNode.canParse(): parse and return an ExprNode</item>
    /// <item>If VardeclNode.canParse(): parse and return a VardeclNode</item>
    /// <item>Otherwise: error (no recognized statement pattern found)</item>
    /// </list>
    /// </remarks>
    /// <exception cref="MissingStatement">
    /// Thrown when the next token(s) do not match any recognized statement pattern.
    /// </exception>
    public static StmtNode parse(Tokenizer T)
    {
        if (ReturnNode.canParse(T))
            return ReturnNode.parse(T);
        if (CondNode.canParse(T))
            return CondNode.parse(T);
        if (WhileNode.canParse(T))
            return WhileNode.parse(T);
        if (RepeatNode.canParse(T))
            return RepeatNode.parse(T);
        if (ExprNode.canParse(T))
            return ExprNode.parse(T);
        if (VardeclNode.canParse(T))
            return VardeclNode.parse(T);
        if (BreakNode.canParse(T))
            return BreakNode.parse(T);
        if (ContinueNode.canParse(T))
            return ContinueNode.parse(T);
        
        Utils.error(new MissingStatement($"Expecting statement, didn't get one (Ln {T.getLine()}, Col {T.getColumn()})"));
        throw new UnreachableException();
    }
}