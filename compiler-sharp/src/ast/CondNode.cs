/*
 * File: CondNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 */

using ASM;
using System.Diagnostics;

/// <summary>
/// Represents a conditional statement (if, else if, or else) in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Conditional statements control program flow based on boolean expressions. They can be
/// chained together to create if-elif-else sequences, with each clause represented by a
/// separate CondNode in the AST.
/// </para>
/// <para>
/// Conditional statements take two forms:
/// </para>
/// <list type="bullet">
/// <item>With condition: if/elif followed by an expression and statement block</item>
/// <item>Without condition: else followed by a statement block only</item>
/// </list>
/// <para>
/// For else clauses (which have no condition), the condition field contains a placeholder
/// Term node with an empty token to maintain a consistent tree structure.
/// </para>
/// </remarks>
public class CondNode : StmtNode
{
    /// <summary>
    /// The clause keyword token (IF, ELIF, or ELSE).
    /// </summary>
    /// <remarks>
    /// Identifies which type of conditional clause this node represents, which affects
    /// both code generation and semantic analysis.
    /// </remarks>
    public Token clause;

    public List<CondNode> alternatives = [];
    
    /// <summary>
    /// The condition expression to evaluate, or a placeholder for else clauses.
    /// </summary>
    /// <remarks>
    /// For if and elif clauses, contains the boolean expression that determines whether
    /// the statement block executes. For else clauses, contains a placeholder
    /// <see cref="Term"/> node with an empty token.
    /// </remarks>
    public ExprNode? condition;
    
    /// <summary>
    /// The statement block to execute when the condition is true (or unconditionally for else).
    /// </summary>
    public StmtsNode stmts;
    
    /// <summary>
    /// Creates a conditional node with a condition expression (if or elif clause).
    /// </summary>
    /// <param name="clause">The clause keyword token (IF or ELIF).</param>
    /// <param name="condition">The condition expression to evaluate.</param>
    /// <param name="stmts">The statement block to execute when the condition is true.</param>
    private CondNode(Token clause, ExprNode? condition, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = condition;
        this.stmts = stmts;
    }
    
    /// <summary>
    /// Creates a conditional node without a condition (else clause).
    /// </summary>
    /// <param name="clause">The ELSE keyword token.</param>
    /// <param name="stmts">The statement block to execute unconditionally.</param>
    /// <remarks>
    /// Creates a placeholder condition using an empty <see cref="Term"/> node to maintain
    /// consistent tree structure for traversal algorithms.
    /// </remarks>
    private CondNode(Token clause, StmtsNode stmts)
    {
        this.clause = clause;
        this.condition = new Term(new Token());  // Placeholder condition with empty token
        this.stmts = stmts;
    }
    
    /// <summary>
    /// Parses a conditional statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A <see cref="CondNode"/> representing the parsed conditional statement.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// if expression { statements }
    /// else if expression { statements }
    /// else { statements }
    /// </code>
    /// <para>
    /// The parser determines whether a condition is present by examining the token following
    /// the clause keyword:
    /// </para>
    /// <list type="bullet">
    /// <item>Literal tokens (NUM, FNUM, BOOLCONST, STRINGCONST): indicates the start of a condition expression (if/elif clause).</item>
    /// <item>LBRACE token: indicates the start of a statement block with no condition (else clause).</item>
    /// </list>
    /// <para>
    /// <b>Scope management:</b> A new scope is pushed immediately before parsing the statement
    /// block and removed immediately after. Variables declared inside a conditional clause are
    /// therefore local to that block and not visible outside it.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidCondition">
    /// Reported via <see cref="Utils.error"/> when the next token is not IF, ELIF, or ELSE,
    /// or when neither a condition expression nor a statement block follows the clause keyword.
    /// </exception>
    public new static CondNode parse(Tokenizer T)
    {
        Token clause = T.expect(TokenSymbols.IF);
        CondNode? condNode = null;

        // Check if a condition expression is present
        if (ExprNode.canParse(T))
        {
            // if clause with condition
            ExprNode cond = ExprNode.parse(T);
            SymbolTable.addScope();
            StmtsNode stmts = StmtsNode.parse(T,false);
            SymbolTable.removeScope();
            condNode = new CondNode(clause, cond, stmts);
        }

        if (condNode == null)
        {
            Utils.error(new InvalidCondition($"Expected expression and statement block after conditional clause {clause.lexeme} on line {clause.line}"));
            throw new UnreachableException("");
        }

        while (T.peek() == TokenSymbols.ELIF)
        {
            // Check if a condition expression is present
            Token altclause = T.expect(TokenSymbols.ELIF);
            if (ExprNode.canParse(T))
            {
                // elif clause with condition
                ExprNode cond = ExprNode.parse(T);
                SymbolTable.addScope();
                StmtsNode stmts = StmtsNode.parse(T,false);
                SymbolTable.removeScope();
                condNode.alternatives.Add(new CondNode(altclause, cond, stmts));
            }
        }

        if (T.peek() == TokenSymbols.ELSE)
        {
            Token altclause = T.expect(TokenSymbols.ELSE);
            SymbolTable.addScope();
            StmtsNode stmts = StmtsNode.parse(T);
            SymbolTable.removeScope();
            condNode.alternatives.Add(new CondNode(altclause, null, stmts));
        }

        return condNode;
    }
    
    /// <summary>
    /// Determines whether the next token can begin a conditional statement.
    /// </summary>
    /// <param name="T">The tokenizer to inspect.</param>
    /// <returns><c>true</c> if the next token is IF, ELIF, or ELSE; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Used by the statement parser to dispatch to this parser without consuming tokens.
    /// </remarks>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.IF || T.peek() == TokenSymbols.ELIF || T.peek() == TokenSymbols.ELSE;
    }

    /// <summary>
    /// Gets the child nodes of this conditional statement.
    /// </summary>
    /// <returns>
    /// A list containing the condition expression followed by the statement block.
    /// </returns>
    /// <remarks>
    /// Returns both the condition (which may be a placeholder for else clauses) and the
    /// statement block, allowing tree traversal algorithms to visit all parts uniformly.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        if (this.clause.lexeme.ToUpper() == TokenSymbols.ELSE)
        {
            if (this.alternatives.Count() > 0)
                throw new Exception("Internal compiler error: got else condition with illegal alternatives.");
            return new List<TreeNode>([stmts, .. this.alternatives]);
        }
        if (this.condition == null)
            throw new Exception($"Condition node {this.clause.lexeme} has a null condition on line {this.clause.line}");
        return new List<TreeNode>([condition!, stmts, .. this.alternatives]);
    }

    /// <summary>
    /// Type inference for conditional nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // not implemented
    }

    /// <summary>
    /// Validates that the condition expression evaluates to a boolean type.
    /// </summary>
    /// <exception cref="InvalidCondition">
    /// Reported via <see cref="Utils.error"/> when the condition type is not <see cref="BoolConstType"/>.
    /// </exception>
    public override void typeCheck()
    {
        if (this.condition == null)
        {
            if (this.clause.lexeme.ToUpper() == TokenSymbols.ELSE)
                return;
            Utils.error(new InvalidExpression($"Got a '{this.clause.lexeme}' clause with no condition on line {this.clause.line}"));
            throw new UnreachableException();
        }
            
        if (this.condition.type != VarType.BoolConst)
            Utils.error(new InvalidCondition($"Invalid condition on line {this.clause.line}; Expected BoolConst, got {this.condition.type}"));
    }

    public override void genCode()
    {
        Label endLabel = new Label();      // one label for the whole if/elif/else

        if (this.condition == null)
            throw new Exception("Internal compiler error: got a condition with a null expression.");

        Label nextAlternative = new Label();  // local, passed consistently

        this.condition.genCode();
        this.condition.getResultLocation()!.copyToRegister(Register.rax, null);
        Asm.emit(
            new Comment("testing main condition"),
            new OpTest(Register.rax, Register.rax),
            new OpJmpCC("z", nextAlternative)
        );

        // main body
        this.stmts.genCode();
        Asm.emit(new OpJmp(endLabel));
        Asm.emit(nextAlternative);

        foreach(CondNode alt in this.alternatives)
        {
            nextAlternative = new Label(); // new local label for this alt's skip target
            ExprNode? expr = alt.condition;

            if (expr != null) // elif
            {
                expr.genCode();
                expr.getResultLocation()!.copyToRegister(Register.rax, null);
                Asm.emit(
                    new Comment("testing alternative condition"),
                    new OpTest(Register.rax, Register.rax),
                    new OpJmpCC("z", nextAlternative)
                );
            }

            alt.stmts.genCode();
            Asm.emit(new OpJmp(endLabel));
            Asm.emit(nextAlternative);
        }

        Asm.emit(endLabel);
    }
}
