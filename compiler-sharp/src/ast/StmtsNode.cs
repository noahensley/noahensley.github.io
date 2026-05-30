/*
 * File: StmtsNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 *      February 25, 2026
 */

/// <summary>
/// Represents a block of statements enclosed in braces in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// A statement block is a fundamental structural element in the language, providing
/// a way to group multiple statements into a single syntactic unit. Statement blocks
/// are delimited by braces { } and can contain zero or more statements.
/// </para>
/// <para>
/// Statement blocks are used in several contexts:
/// </para>
/// <list type="bullet">
/// <item>Function bodies (all statements in a function)</item>
/// <item>Loop bodies (statements to execute in each iteration)</item>
/// <item>Conditional bodies (statements to execute when condition is true/false)</item>
/// </list>
/// <para>
/// An empty statement block (just <c>{ }</c>) is valid and represents a no-op.
/// </para>
/// </remarks>
public class StmtsNode : TreeNode
{
    /// <summary>
    /// List of statements in this block, in order of appearance.
    /// </summary>
    /// <remarks>
    /// Contains all statements within the braces in the order they appear in the source code.
    /// An empty list represents an empty block. Statements are executed sequentially during
    /// program execution.
    /// </remarks>
    public List<StmtNode> stmts = new List<StmtNode>();
    
    /// <summary>
    /// Parses a statement block from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A StmtsNode containing all parsed statements within the block.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// {
    ///     statement1
    ///     statement2
    ///     ...
    /// }
    /// </code>
    /// <para>
    /// Parsing sequence:
    /// </para>
    /// <list type="number">
    /// <item>Expects opening brace (LBRACE).</item>
    /// <item>Expects end-of-statement token (EOS) after the opening brace.</item>
    /// <item>Parses statements in a loop until a closing brace (RBRACE) is encountered.</item>
    /// <item>Expects closing brace (RBRACE).</item>
    /// <item>Consumes a trailing EOS token if present.</item>
    /// </list>
    /// <para>
    /// <b>Scope management:</b> This method no longer creates or removes symbol table
    /// scopes. Scope handling is the responsibility of the enclosing construct
    /// (e.g., function definitions, loops, or conditionals). Those nodes explicitly
    /// push and pop scopes before and after invoking <see cref="StmtsNode.parse"/>.
    /// </para>
    /// <para>
    /// As a result, a statement block is now purely a syntactic grouping mechanism
    /// in the AST. Whether it introduces a new scope depends entirely on the
    /// surrounding node.
    /// </para>
    /// <para>
    /// Empty blocks are valid: <c>{ }</c>.
    /// </para>
    /// </remarks>
    public static StmtsNode parse(Tokenizer T, bool consumeEOS = true)
    {
        T.expect(TokenSymbols.LBRACE);
        T.expect(TokenSymbols.EOS);
        
        StmtsNode snode = new StmtsNode();
        
        while (true)
        {
            if (T.peek() == TokenSymbols.EOS)
                T.expect(TokenSymbols.EOS);
            if (T.peek() == TokenSymbols.RBRACE)
                break;
            snode.stmts.Add(StmtNode.parse(T));
        }
        
        T.expect(TokenSymbols.RBRACE);

        if (T.peek() == TokenSymbols.EOS && consumeEOS)
            T.expect(TokenSymbols.EOS);
            
        return snode;
    }

    /// <summary>
    /// Gets the child nodes of this statement block.
    /// </summary>
    /// <returns>
    /// A list containing all statement nodes in the block.
    /// </returns>
    /// <remarks>
    /// Converts the stmts list (which contains StmtNode objects) to a list of TreeNode
    /// objects for uniform tree traversal. This allows visitors and other tree-walking
    /// algorithms to process all statements in the block sequentially.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>(stmts);
    }

    /// <summary>
    /// Type inference for statement blocks. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return;
    }

    /// <summary>
    /// Type validation for statement blocks. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return;
    }
}