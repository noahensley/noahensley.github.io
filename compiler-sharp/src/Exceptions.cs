/*
 * File: Exceptions.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 17, 2026
 *      February 18, 2026
 */

/// <summary>
/// Custom exception used to terminate tree traversal early.
/// </summary>
/// <remarks>
/// Used as a control flow mechanism in tree walking to stop processing after
/// the first expression node is found and processed.
/// See <see cref="Utils"/> for usage.
/// </remarks>
public class StopIteration : Exception
{
}

/// <summary>
/// Exception thrown when an unrecognized or malformed command-line option is provided.
/// </summary>
/// <remarks>
/// See <see cref="Utils"/> for usage.
/// </remarks>
public class InvalidOption : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidOption"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid option.</param>
    public InvalidOption(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a variable is assigned or used with an unrecognized type.
/// </summary>
/// <remarks>
/// See <see cref="VarType"/> for usage.
/// </remarks>
public class InvalidVariableType : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidVariableType"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid variable type.</param>
    public InvalidVariableType(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a binary operator is applied to operands of incompatible types.
/// </summary>
/// <remarks>
/// See <see cref="BinaryOperator"/> for usage.
/// </remarks>
public class InvalidBinaryOperatorType : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidBinaryOperatorType"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid binary operator type combination.</param>
    public InvalidBinaryOperatorType(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a unary operator is applied to an operand of an incompatible type.
/// </summary>
/// <remarks>
/// See <see cref="UnaryOperator"/> for usage.
/// </remarks>
public class InvalidUnaryOperatorType : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidUnaryOperatorType"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid unary operator type combination.</param>
    public InvalidUnaryOperatorType(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a condition expression is invalid or malformed.
/// </summary>
/// <remarks>
/// See <see cref="CondNode"/> and <see cref="LoopNode"/> for usage.
/// </remarks>
public class InvalidCondition : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidCondition"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid condition.</param>
    public InvalidCondition(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when an operator token is unrecognized or not valid in the current expression context.
/// </summary>
/// <remarks>
/// See <see cref="ExprNode"/> for usage.
/// </remarks>
public class InvalidOperator : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidOperator"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid operator.</param>
    public InvalidOperator(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when an operator is missing one or more required operands.
/// </summary>
/// <remarks>
/// See <see cref="ExprNode"/> for usage.
/// </remarks>
public class MissingOperands : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="MissingOperands"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the missing operands.</param>
    public MissingOperands(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a node or component is encountered in an unexpected or inconsistent state.
/// </summary>
/// <remarks>
/// See <see cref="ExprNode"/> for usage.
/// </remarks>
public class InvalidState : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidState"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid state.</param>
    public InvalidState(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when an expression is structurally invalid or cannot be parsed.
/// </summary>
/// <remarks>
/// See <see cref="ExprNode"/> for usage.
/// </remarks>
public class InvalidExpression : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidExpression"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid expression.</param>
    public InvalidExpression(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when an expression is required but absent.
/// </summary>
/// <remarks>
/// See <see cref="ExprNode"/> and <see cref="LoopNode"/> for usage.
/// </remarks>
public class MissingExpression : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="MissingExpression"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the missing expression.</param>
    public MissingExpression(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when no main function is found in the program.
/// </summary>
/// <remarks>
/// See <see cref="ProgramNode"/> for usage.
/// </remarks>
public class MissingMainFunction : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="MissingMainFunction"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the missing main function.</param>
    public MissingMainFunction(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a statement is required but absent or unrecognized.
/// </summary>
/// <remarks>
/// See <see cref="StmtNode"/> for usage.
/// </remarks>
public class MissingStatement : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="MissingStatement"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the missing statement.</param>
    public MissingStatement(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a term is encountered with an unrecognized or invalid type.
/// </summary>
/// <remarks>
/// See <see cref="Term"/> for usage.
/// </remarks>
public class InvalidTermType : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidTermType"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid term type.</param>
    public InvalidTermType(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when the end of input is reached before a required token or symbol is found.
/// </summary>
/// <remarks>
/// See <see cref="Tokenizer"/> for usage.
/// </remarks>
public class UnexpectedEndOfInput : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="UnexpectedEndOfInput"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing where end of input was unexpectedly encountered.</param>
    public UnexpectedEndOfInput(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a token that is not a valid EOS-suppressing delimiter is encountered where one is expected.
/// </summary>
/// <remarks>
/// See <see cref="Tokenizer"/> for usage.
/// </remarks>
public class InvalidEOSSuppressor : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidEOSSuppressor"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid EOS-suppressing token.</param>
    public InvalidEOSSuppressor(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when nesting delimiters are imbalanced, such as more closers than openers.
/// </summary>
/// <remarks>
/// See <see cref="Tokenizer"/> for usage.
/// </remarks>
public class NestingImbalance : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="NestingImbalance"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the nesting imbalance.</param>
    public NestingImbalance(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a token is encountered that is not valid in the current parsing context.
/// </summary>
/// <remarks>
/// See <see cref="Tokenizer"/> for usage.
/// </remarks>
public class UnexpectedToken : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="UnexpectedToken"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the unexpected token.</param>
    public UnexpectedToken(string msg) : base(msg) {}
}

/// <summary>
/// Exception thrown when a token cannot be matched to any recognized terminal pattern.
/// </summary>
/// <remarks>
/// See <see cref="Tokenizer"/> for usage.
/// </remarks>
public class InvalidToken : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidToken"/> with the specified error message.
    /// </summary>
    /// <param name="msg">A message describing the invalid token.</param>
    public InvalidToken(string msg) : base(msg) {}
}