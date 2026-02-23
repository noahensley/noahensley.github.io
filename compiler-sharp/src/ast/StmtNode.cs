/*
 * File: StmtNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026
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
/// </list>
/// Additional statement types can be added by extending this class.
/// </remarks>
public abstract class StmtNode : TreeNode 
{
    /// <summary>
    /// Parses a statement from the token stream by delegating to specific statement parsers.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A specific StmtNode subclass instance (ReturnNode, CondNode, or LoopNode).</returns>
    /// <remarks>
    /// Uses the canParse() methods of each statement type to determine which parser to invoke.
    /// Checks statement types in order: return, conditional, loop.
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the next token(s) do not match any recognized statement pattern.
    /// </exception>
    public static StmtNode parse(Tokenizer T)
    {
        if (ReturnNode.canParse(T))
            return ReturnNode.parse(T);
        if (CondNode.canParse(T))
            return CondNode.parse(T);
        if (LoopNode.canParse(T))
            return LoopNode.parse(T);
        if (ExprNode.canParse(T))
            return ExprNode.parse(T);
        
        Utils.error(new MissingStatement($"Expecting statement, didn't get one (Ln {T.getLine()}, Col {T.getColumn()})"));
        throw new UnreachableException();
    }
}