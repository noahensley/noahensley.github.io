/*
 * File: WhileNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 */

using ASM;
/// <summary>
/// Represents a loop statement (while) in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Loop statements enable repeated execution of code blocks based on a condition.
/// The while loop continues executing its body as long as the condition evaluates to true.
/// </para>
/// <para>
/// Loop statements can take two forms:
/// </para>
/// <list type="bullet">
/// <item>With condition: while followed by an expression and statement block (conditional loop).</item>
/// <item>Without condition: while followed directly by a statement block (infinite loop).</item>
/// </list>
/// <para>
/// When no condition expression is present, the loop executes indefinitely unless terminated
/// by a break statement. The condition field contains a placeholder Term node with an empty
/// token to maintain a consistent tree structure.
/// </para>
/// </remarks>
public class WhileNode : LoopNode
{
    /// <summary>
    /// Creates a loop node with a condition expression (conditional while loop).
    /// </summary>
    /// <param name="clause">The WHILE keyword token.</param>
    /// <param name="condition">The loop condition expression to evaluate before each iteration.</param>
    /// <param name="stmts">The loop body statement block.</param>
    private WhileNode(Token clause, ExprNode condition, StmtsNode stmts) : base(clause, condition, stmts)
    {
        
    }
    
    /// <summary>
    /// Creates a loop node without a condition (infinite loop).
    /// </summary>
    /// <param name="clause">The WHILE keyword token.</param>
    /// <param name="stmts">The loop body statement block.</param>
    /// <remarks>
    /// Creates a placeholder condition using an empty <see cref="Term"/> node to maintain
    /// consistent tree structure for traversal algorithms.
    /// </remarks>
    private WhileNode(Token clause, StmtsNode stmts) : base(clause, new Term(new Token()), stmts)
    {
        
    }
    
    /// <summary>
    /// Parses a loop statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A <see cref="WhileNode"/> representing the parsed loop statement.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// while expression { statements }
    /// while { statements }
    /// </code>
    /// <para>
    /// The parser determines whether a condition is present by examining the token following
    /// the WHILE keyword:
    /// </para>
    /// <list type="bullet">
    /// <item>Literal tokens (NUM, FNUM, BOOLCONST, STRINGCONST) or BOOLNOTOP:
    /// indicates the start of a condition expression (conditional loop).</item>
    /// <item>LBRACE token: indicates the start of a statement block with no condition (infinite loop).</item>
    /// </list>
    /// <para>
    /// <b>Scope management:</b> A new scope is pushed immediately before parsing the loop body
    /// and removed immediately after. Variables declared inside the loop body are local to that
    /// iteration and not visible outside the loop.
    /// </para>
    /// </remarks>
    /// <exception cref="MissingExpression">
    /// Reported via <see cref="Utils.error"/> when the token following the WHILE keyword is
    /// neither a valid expression start nor LBRACE.
    /// </exception>
    public new static WhileNode parse(Tokenizer T)
    {
        Token clause = T.expect(TokenSymbols.WHILE);
        
        // Check if a condition expression is present
        if (ExprNode.canParse(T))
        {
            // Conditional loop
            ExprNode cond = ExprNode.parse(T);
            SymbolTable.addScope();
            StmtsNode stmts = StmtsNode.parse(T);
            SymbolTable.removeScope();
            return new WhileNode(clause, cond, stmts);
        }
        else if (T.peek() == TokenSymbols.LBRACE)
        {
            // Infinite loop (no condition)
            SymbolTable.addScope();
            StmtsNode stmts = StmtsNode.parse(T);
            SymbolTable.removeScope();
            return new WhileNode(clause, stmts);
        }

        Utils.error(new MissingExpression($"Expected expression or statement block after while keyword, got: {T.peek()}"));
        throw new Exception(""); // Unreachable, but required for compiler
    }
    
    /// <summary>
    /// Determines whether the next token can begin a loop statement.
    /// </summary>
    /// <param name="T">The tokenizer to inspect.</param>
    /// <returns><c>true</c> if the next token is WHILE; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Used by the statement parser to dispatch to this parser without consuming tokens.
    /// </remarks>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.WHILE;
    }

    /// <summary>
    /// Gets the child nodes of this loop statement.
    /// </summary>
    /// <returns>
    /// A list containing the condition expression followed by the statement block.
    /// </returns>
    /// <remarks>
    /// Returns both the condition (which may be a placeholder for infinite loops) and the
    /// statement block, allowing tree traversal algorithms to visit all parts uniformly.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        return base.getChildren();
    }

    /// <summary>
    /// Type inference for loop nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // not implemented
    }

    /// <summary>
    /// Validates that the loop condition evaluates to a boolean type.
    /// </summary>
    /// <exception cref="InvalidCondition">
    /// Reported via <see cref="Utils.error"/> when the condition type is not <see cref="BoolConstType"/>.
    /// </exception>
    public override void typeCheck()
    {
        base.typeCheck();
    }

    public override void genCode()
    {
        if (condition == null)
            throw new Exception($"Internal compiler error: type checking did not catch '{this.clause}' clause with missing condition.");
        Asm.emit(
            new Comment("first test of loop condition"),
            test
        );
        condition.genCode();     //loop expression
        condition.getResultLocation()!.copyToRegister(Register.rax, null);
        Asm.emit(
            new Comment("testing loop condition"),
            new OpTest(Register.rax, Register.rax),
            new OpJmpCC("z", endLoop)
        );
        stmts.genCode();    //loop body
        Asm.emit(
            new Comment("testing loop condition"),
            new OpJmp(test)
        );
        Asm.emit(
            new Comment("test was false, exiting loop"),
            endLoop
        );
    }
}
