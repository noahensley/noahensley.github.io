/*
 * File: ReturnNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026
 *
 * Additional AI Usage: ChatGPT (OpenAI) was used on March 3, 2026 to assist
 * in refactoring the typeCheck implementation to eliminate duplicated logic
 * between return statement subclasses by applying a template method design
 * pattern and centralizing semantic validation in the base class.
 */

using System.Diagnostics;
using ASM;

/// <summary>
/// Abstract base class for return statement nodes in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// A return statement terminates execution of the enclosing function and optionally
/// supplies a value to the caller.
/// </para>
/// <para>
/// Two concrete subclasses represent the possible forms:
/// </para>
/// <list type="bullet">
/// <item><b>ReturnExprNode</b>: Returns the value of an expression.</item>
/// <item><b>ReturnVoidNode</b>: Returns no value.</item>
/// </list>
/// <para>
/// During semantic analysis, the returned type is validated against the declared
/// return type of the enclosing function. The base class implements this validation
/// using a template method pattern: subclasses provide the returned type while
/// the base class performs the shared comparison logic.
/// </para>
/// </remarks>
public abstract class ReturnNode : StmtNode
{
    /// <summary>
    /// The RETURN keyword token.
    /// </summary>
    /// <remarks>
    /// Stored for diagnostic reporting and source position tracking during
    /// later compilation phases.
    /// </remarks>
    public Token retToken;

    /// <summary>
    /// Initializes a return statement node.
    /// </summary>
    /// <param name="retToken">The RETURN keyword token from the source program.</param>
    protected ReturnNode(Token retToken)
    {
        this.retToken = retToken;
    }

    /// <summary>
    /// Parses a return statement from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer providing input tokens.</param>
    /// <returns>
    /// A <see cref="ReturnVoidNode"/> if no expression follows the RETURN keyword, or a
    /// <see cref="ReturnExprNode"/> if an expression is present.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Supported syntax:
    /// </para>
    /// <code>
    /// return;
    /// return expression;
    /// </code>
    /// <para>
    /// If the token immediately following <c>RETURN</c> is an end-of-statement
    /// marker, a void return node is created. Otherwise, an expression is parsed
    /// and wrapped in a return expression node.
    /// </para>
    /// </remarks>
    /// <exception cref="UnexpectedToken">
    /// Reported via <see cref="Utils.error"/> if the expected RETURN token or EOS is absent.
    /// </exception>
    public new static ReturnNode parse(Tokenizer T)
    {
        Token ret = T.expect(TokenSymbols.RETURN);

        if (T.peek() == TokenSymbols.EOS)
        {
            T.expect(TokenSymbols.EOS);
            return new ReturnVoidNode(ret);
        }
        else
        {
            ExprNode expr = ExprNode.parse(T);
            T.expect(TokenSymbols.EOS);
            return new ReturnExprNode(ret, expr, expr.type);
        }
    }

    /// <summary>
    /// Determines whether the upcoming token sequence represents a return statement.
    /// </summary>
    /// <param name="T">The tokenizer to inspect.</param>
    /// <returns><c>true</c> if the next token is RETURN; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Used by higher-level statement parsing logic to select the correct
    /// parsing routine without consuming input.
    /// </remarks>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.RETURN;
    }

    /// <summary>
    /// Retrieves the declared return type of the enclosing function definition.
    /// </summary>
    /// <returns>The return type specified in the containing function declaration.</returns>
    /// <remarks>
    /// Traverses the parent chain until a <see cref="FuncdefNode"/> is located.
    /// If no enclosing function is found, a semantic error is reported, as return
    /// statements are only valid within function bodies.
    /// </remarks>
    /// <exception cref="InvalidExpression">
    /// Reported via <see cref="Utils.error"/> if no enclosing <see cref="FuncdefNode"/> exists.
    /// </exception>
    protected VarType getEnclosingReturnType()
    {
        TreeNode? cur = this.parent;

        while (cur is not FuncdefNode)
        {
            if (cur == null)
            {
                Utils.error(new InvalidExpression(
                    $"Unexpected return with no enclosing function on line {this.retToken.line}"
                ));
                throw new UnreachableException();
            }

            cur = cur.parent;
        }

        FuncdefNode? fdef = cur as FuncdefNode;
        if (fdef == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type checking phase " +
                $"while processing function '{this.retToken.lexeme}' at line {this.retToken.line}"
            );

        if (fdef.info == null)
            throw new Exception("Internal compiler error: function info was null during return type check -- function parsing failed to assign non-null info to function.");
        FuncType? target = fdef.info.type as FuncType;
        if (target == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type checking phase " +
                $"while processing function '{this.retToken.lexeme}' at line {this.retToken.line}"
            );

        return target.returnType;
    }

    /// <summary>
    /// Gets the type produced by this return statement.
    /// </summary>
    /// <returns>The type being returned by the statement.</returns>
    /// <remarks>
    /// Implemented by subclasses to supply the specific returned type.
    /// Expression returns provide the expression's type; void returns provide <see cref="VarType.Void"/>.
    /// </remarks>
    protected abstract VarType getReturnedType();

    /// <summary>
    /// Validates that the type returned by this statement matches the enclosing function's declared return type.
    /// </summary>
    /// <remarks>
    /// Compares the type produced by this return statement with the declared return type of the
    /// enclosing function. If the types differ, a <see cref="ReturnMismatch"/> error is reported.
    /// </remarks>
    /// <exception cref="ReturnMismatch">
    /// Reported via <see cref="Utils.error"/> when the returned type does not match the function's
    /// declared return type.
    /// </exception>
    public override void typeCheck()
    {
        VarType expected = getEnclosingReturnType();
        VarType actual = getReturnedType();

        FuncType? factual = actual as FuncType;
        if (factual != null)
            actual = factual.returnType;

        if (actual != expected)
        {
            Utils.error(new ReturnMismatch(
                $"Return type on line {this.retToken.line} does not match parent function declaration. " +
                $"Got {actual}, expected {expected}"
            ));
        }
    }
}

/// <summary>
/// Represents a return statement that evaluates and returns an expression.
/// </summary>
/// <remarks>
/// <para>
/// Used in functions that declare a non-void return type.
/// The enclosed expression is evaluated and its resulting type must match
/// the function's declared return type.
/// </para>
/// <para>
/// Semantic validation is inherited from <see cref="ReturnNode"/>.
/// </para>
/// </remarks>
public class ReturnExprNode : ReturnNode
{
    /// <summary>
    /// The expression whose value is returned to the caller.
    /// </summary>
    /// <remarks>
    /// This expression subtree is evaluated at runtime. Its type is used during
    /// semantic analysis to validate return-type correctness.
    /// </remarks>
    public ExprNode expr;

    /// <summary>
    /// Initializes a return expression node.
    /// </summary>
    /// <param name="retToken">The RETURN keyword token.</param>
    /// <param name="expr">The expression to evaluate and return.</param>
    /// <param name="type">The statically determined type of the expression (may be null before type inference).</param>
    public ReturnExprNode(Token retToken, ExprNode expr, VarType? type) : base(retToken)
    {
        this.expr = expr;
    }

    /// <summary>
    /// Gets the child nodes of this return expression statement.
    /// </summary>
    /// <returns>A list containing the expression subtree.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>() { expr };
    }

    /// <summary>
    /// Type inference for expression return nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // no type setting needed
    }

    /// <summary>
    /// Provides the type returned by this statement.
    /// </summary>
    /// <returns>The resolved type of the enclosed expression.</returns>
    protected override VarType getReturnedType()
    {
        if ((this.expr as NewNode) != null)
        {
            NewNode nnode = (this.expr as NewNode)!;
            if (nnode.term == null)
                throw new Exception("Internal compiler error: NewNode was initialized with null term.");
            if (nnode.term.type == null)
                throw new Exception($"Internal compiler error: term {nnode.term.token.lexeme} type was not set during type setting phase.");
            return nnode.term.type;
        }
        else if (this.expr.type == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type checking phase " +
                $"while processing function '{this.retToken.lexeme}' at line {this.retToken.line}"
            );
            
        return this.expr.type;
    }

    public override void genCode()
    {
        this.expr.genCode();
        expr.getResultLocation().copyToRegister(Register.rax, Register.rbx);
        Asm.emit(
            new Comment("returning expression"),
            new OpMovRegReg(src: Register.rbp, dst: Register.rsp),
            new OpPopReg(Register.rbp),
            new OpRet()
        );
    }
}

/// <summary>
/// Represents a return statement that produces no value.
/// </summary>
/// <remarks>
/// <para>
/// Used in functions declared with a void return type, or to exit such functions early.
/// </para>
/// <para>
/// Semantic validation is inherited from <see cref="ReturnNode"/>.
/// </para>
/// </remarks>
public class ReturnVoidNode : ReturnNode
{
    /// <summary>
    /// Initializes a void return node.
    /// </summary>
    /// <param name="retToken">The RETURN keyword token.</param>
    public ReturnVoidNode(Token retToken) : base(retToken)
    {
    }

    /// <summary>
    /// Gets the child nodes of this void return statement.
    /// </summary>
    /// <returns>An empty list, as void returns have no expression subtree.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
    }

    /// <summary>
    /// Type inference for void return nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return; // no type setting needed
    }

    /// <summary>
    /// Provides the type returned by this statement.
    /// </summary>
    /// <returns><see cref="VarType.Void"/>.</returns>
    protected override VarType getReturnedType()
    {
        return VarType.Void;
    }

    public override void genCode()
    {
        Asm.emit(
            new Comment($"return from {this.retToken}"),
            new OpMovRegReg( src: Register.rbp, dst: Register.rsp),
            new OpPopReg(Register.rbp),
            new OpRet()
        );
    }
}
