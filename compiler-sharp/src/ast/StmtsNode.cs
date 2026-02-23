/*
 * File: StmtsNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
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
    /// <item>Expects opening brace (LBRACE)</item>
    /// <item>Expects end-of-statement token (EOS) after opening brace</item>
    /// <item>Parses statements in a loop until closing brace (RBRACE) is encountered</item>
    /// <item>Expects closing brace (RBRACE)</item>
    /// <item>Conditionally consumes trailing EOS token if present (see below)</item>
    /// </list>
    /// <para>
    /// <b>Special handling for trailing EOS:</b> After the closing brace, if an EOS token
    /// is present, it is consumed UNLESS the next token after it might be ELSE. This allows
    /// if-else chains to work correctly without requiring the else keyword to appear on the
    /// same line as the closing brace of the if block. The parser peeks ahead to check for
    /// ELSE before consuming the EOS.
    /// </para>
    /// <para>
    /// Empty blocks are valid: <c>{ }</c> (just opening brace, EOS, closing brace).
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown when the opening brace (LBRACE) is not found.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the EOS token after the opening brace is missing.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the closing brace (RBRACE) is not found.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when statement parsing fails (delegated to StmtNode.parse()).
    /// </exception>
    public static StmtsNode parse(Tokenizer T)
    {
        T.expect(TokenSymbols.LBRACE);
        T.expect(TokenSymbols.EOS); // Newline after opening brace
        
        StmtsNode snode = new StmtsNode();
        
        // Parse statements until closing brace
        while (true)
        {
            if (T.peek() == TokenSymbols.RBRACE)
                break;
            if (T.peek() == TokenSymbols.EOS)
            {
                T.expect(TokenSymbols.EOS);
                break;
            }
            snode.stmts.Add(StmtNode.parse(T));
        }
        
        T.expect(TokenSymbols.RBRACE);

        // Conditionally consume trailing EOS (unless followed by ELSE)
        if (T.peek() == TokenSymbols.EOS)
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

    public override void setType()
    {
        return; // not implemented
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}
