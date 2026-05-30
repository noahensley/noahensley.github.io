/*
 * File: RepeatNode.cs
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
/// Represents a repeat-until loop statement in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// A repeat-until loop executes its body at least once, then evaluates the condition.
/// The loop continues repeating until the condition evaluates to true.
/// </para>
/// <para>
/// Expected syntax:
/// </para>
/// <code>
/// repeat { statements } until expression
/// </code>
/// </remarks>
public class RepeatNode : LoopNode
{
    /// <summary>
    /// The UNTIL keyword token, marking the end of the loop body and the start of the condition.
    /// </summary>
    public Token endClause;

    public Label startLoop = new Label();

    /// <summary>
    /// Creates a repeat-until loop node.
    /// </summary>
    /// <param name="openClause">The REPEAT keyword token.</param>
    /// <param name="endClause">The UNTIL keyword token.</param>
    /// <param name="condition">The condition expression evaluated after each iteration.</param>
    /// <param name="stmts">The loop body statement block.</param>
    private RepeatNode(Token openClause, Token endClause, ExprNode condition, StmtsNode stmts) : base(openClause, condition, stmts)
    {
        this.endClause = endClause;
    }

    /// <summary>
    /// Parses a repeat-until loop statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A <see cref="RepeatNode"/> representing the parsed loop statement.</returns>
    /// <remarks>
    /// <b>Scope management:</b> A new scope is pushed immediately before parsing the loop body
    /// and removed immediately after. Variables declared inside the loop body are local to that
    /// iteration and not visible outside the loop.
    /// </remarks>
    /// <exception cref="MissingExpression">
    /// Reported via <see cref="Utils.error"/> when no condition expression follows the UNTIL keyword.
    /// </exception>
    public new static RepeatNode parse(Tokenizer T)
    {
        Token openClause = T.expect(TokenSymbols.REPEAT);
        SymbolTable.addScope();
        StmtsNode stmts = StmtsNode.parse(T);
        SymbolTable.removeScope();
        Token endClause = T.expect(TokenSymbols.UNTIL);
        ExprNode cond = ExprNode.parse(T);
        return new RepeatNode(openClause, endClause, cond, stmts);
    }

    /// <summary>
    /// Determines whether the next token can begin a repeat-until loop statement.
    /// </summary>
    /// <param name="T">The tokenizer to inspect.</param>
    /// <returns><c>true</c> if the next token is REPEAT; otherwise <c>false</c>.</returns>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.REPEAT;
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

    /// <summary>
    /// Generates assembly code for the repeat-until loop.
    /// </summary>
    /// <remarks>
    /// Emits the loop body first, then evaluates the condition. Jumps back to
    /// <see cref="LoopNode.test"/> if the condition is false, exits to
    /// <see cref="LoopNode.endLoop"/> if true.
    /// </remarks>
    public override void genCode()
    {
        if (condition == null)
            throw new Exception($"Internal compiler error: type checking did not catch '{this.clause}' clause with missing condition.");
        Asm.emit(
            new Comment("repeat-until body"),
            startLoop
        );
        stmts.genCode();
        Asm.emit(test);
        condition.genCode();
        condition.getResultLocation()!.copyToRegister(Register.rax, null);
        Asm.emit(
            new Comment("testing until condition"),
            new OpTest(Register.rax, Register.rax),
            new OpJmpCC("z", startLoop)
        );
        Asm.emit(
            new Comment("condition true, exiting loop"),
            endLoop
        );
    }
}