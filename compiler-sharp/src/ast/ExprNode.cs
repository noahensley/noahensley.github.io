/*
 * File: ExprNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026
 *      January 28, 2026
 *      February 1, 2026
 *      February 2, 2026
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Defines a valid operand and result type pairing for an operator.
/// </summary>
/// <remarks>
/// Instances of this class describe which operand types an operator
/// accepts and what result type is produced when the operator is
/// applied to that operand.
/// </remarks>
public class LegalOperand
{
    /// <summary>
    /// The type that the operand must be for this pairing to apply.
    /// </summary>
    public VarType operandType;

    /// <summary>
    /// The type that the operation produces when given an operand of <see cref="operandType"/>.
    /// </summary>
    public VarType resultType;

    /// <summary>
    /// Creates a new legal operand pairing with the specified operand and result types.
    /// </summary>
    /// <param name="operand">The required type of the operand.</param>
    /// <param name="result">The type produced by the operation for the given operand type.</param>
    public LegalOperand(VarType operand, VarType result)
    {
        this.operandType = operand;
        this.resultType = result;
    }
}

/// <summary>
/// Represents an expression node in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// This abstract base class provides the foundation for all expression types in the parser.
/// It uses the shunting-yard algorithm to convert infix expressions into an abstract syntax tree,
/// handling operator precedence and associativity automatically.
/// </para>
/// <para>
/// Currently supported expression types:
/// </para>
/// <list type="bullet">
/// <item>Numeric literals (NUM tokens)</item>
/// <item>Identifier references (ID tokens)</item>
/// <item>Binary arithmetic operations (addition, subtraction, multiplication, division, modulo, exponentiation)</item>
/// <item>Binary bitwise operations (shifts, bitwise AND/OR/XOR)</item>
/// <item>Binary boolean operations (logical AND/OR)</item>
/// <item>Equality and inequality comparisons (==, !=)</item>
/// <item>Relational comparisons (less than, greater than, less than or equal, greater than or equal)</item>
/// <item>Assignment operations (simple and compound assignments)</item>
/// <item>Member access operations (dot operator)</item>
/// <item>Unary prefix increment and decrement (++x, --x)</item>
/// <item>Unary postfix increment and decrement (x++, x--)</item>
/// </list>
/// <para>
/// Each expression node is assigned a unique identifier for graph visualization purposes.
/// </para>
/// </remarks>
public abstract class ExprNode : StmtNode
{
    public Temporary? temporary = null;
    /// <summary>
    /// The token representing this expression node's operator or value.
    /// </summary>
    /// <remarks>
    /// For terminal nodes (Term), this is the numeric literal or identifier token.
    /// For binary operator nodes, this is the operator token (e.g., +, *, &lt;&lt;).
    /// </remarks>
    public Token token;
    
    /// <summary>
    /// The resolved type of this expression node, assigned during semantic analysis.
    /// </summary>
    /// <remarks>
    /// Null until <see cref="setType"/> has been called. Once set, used for both
    /// type checking and code generation.
    /// </remarks>
    public VarType? type;

    /// <summary>
    /// Unique identifier for this node used in graph visualization.
    /// </summary>
    /// <remarks>
    /// Generated sequentially as "n0", "n1", "n2", etc. Used when generating
    /// Graphviz dotfiles to uniquely identify nodes in the graph.
    /// </remarks>
    public readonly string unique_id;

    /// <summary>
    /// Global counter for generating unique node identifiers.
    /// </summary>
    private static int _ctr;

    /// <summary>
    /// Creates a new expression node with the given token.
    /// </summary>
    /// <param name="tok">The token associated with this expression node.</param>
    /// <remarks>
    /// Automatically assigns a unique identifier to the node by incrementing the global counter.
    /// </remarks>
    protected ExprNode(Token tok)
    {
        this.token = tok;
        this.type = null;
        this.unique_id = $"n{_ctr++}";
    }

    /// <summary>
    /// Infers and assigns the type of this expression node from its child nodes and operands.
    /// </summary>
    /// <remarks>
    /// Implemented by each concrete subclass to perform type inference appropriate to that
    /// node's operator or value. Must be called during semantic analysis before <see cref="type"/>
    /// is accessed.
    /// </remarks>
    public override void setType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Resets the unique identifier counter to zero.
    /// </summary>
    /// <remarks>
    /// Should be called before parsing each new file to ensure consistent node identifiers
    /// across multiple parse operations.
    /// </remarks>
    public static void resetCounter()
    {
        _ctr = 0;
    }

    /// <summary>
    /// Specifies the associativity of an operator.
    /// </summary>
    /// <remarks>
    /// Used in OpInfo to determine how operators of the same precedence are evaluated.
    /// </remarks>
    enum Associativity
    {
        /// <summary>
        /// Left-associative (LA). Operations are evaluated left-to-right.
        /// </summary>
        LEFT,
        
        /// <summary>
        /// Right-associative (RA). Operations are evaluated right-to-left.
        /// </summary>
        RIGHT,
        
        /// <summary>
        /// Non-associative (NA). Consecutive operations are not allowed.
        /// </summary>
        NONE
    }

    /// <summary>
    /// Contains metadata about a binary operator including precedence, associativity, and node construction.
    /// </summary>
    /// <remarks>
    /// Used in the parser to determine operator precedence and create appropriate AST nodes.
    /// Supports implementation of the shunting-yard algorithm for expression parsing.
    /// See: https://www.reedbeta.com/blog/the-shunting-yard-algorithm/
    /// </remarks>
    class OpInfo
    {
        /// <summary>
        /// Operator precedence value (higher values bind more tightly).
        /// </summary>
        public readonly int precedence;

        /// <summary>
        /// The number of operands taken by an operation or relation.
        /// </summary>
        public readonly int arity;

        /// <summary>
        /// The associativity of the operator (left, right, or none).
        /// </summary>
        /// <remarks>
        /// Determines how operators of the same precedence are evaluated.
        /// </remarks>
        public readonly Associativity associativity;

        /// <summary>
        /// Factory function to create the appropriate AST node for this operator.
        /// </summary>
        /// <remarks>
        /// Takes the operator token and left/right expression subtrees, returns
        /// the constructed binary operator node.
        /// </remarks>
        public readonly Func<Token, ExprNode[], ExprNode> maker; // Anthropic

        /// <summary>
        /// Creates operator information with the specified precedence, associativity, and node factory.
        /// </summary>
        /// <param name="precedence">The operator's precedence level.</param>
        /// <param name="associativity">The operator's associativity (LEFT, RIGHT, or NONE).</param>
        /// <param name="arity">The operator's required number of operands.</param>
        /// <param name="maker">Factory function to construct AST nodes for this operator.</param>
        public OpInfo(int precedence, Associativity associativity, int arity, Func<Token, ExprNode, ExprNode, ExprNode> binaryMaker)
        {
            this.precedence = precedence;
            this.associativity = associativity;
            this.arity = arity;
            this.maker = (token, args) => binaryMaker(token, args[0], args[1]);
        }

        /// <summary>
        /// Creates operator information with the specified precedence, associativity, and node factory.
        /// </summary>
        /// <param name="precedence">The operator's precedence level.</param>
        /// <param name="associativity">The operator's associativity (LEFT, RIGHT, or NONE).</param>
        /// <param name="arity">The operator's required number of operands.</param>
        /// <param name="maker">Factory function to construct AST nodes for this operator.</param>
        public OpInfo(int precedence, Associativity associativity, int arity, Func<Token, ExprNode, ExprNode> unaryMaker)
        {
            this.precedence = precedence;
            this.associativity = associativity;
            this.arity = arity;
            this.maker = (token, args) => unaryMaker(token, args[0]);
        }
    }

    /// <summary>
    /// Dictionary mapping operator token symbols to their precedence and construction logic.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Current precedence hierarchy (highest to lowest):
    /// </para>
    /// <list type="bullet">
    /// <item>DOTOP (500): Member access (dot operator)</item>
    /// <item>func-call (500): Function invocation</item>
    /// <item>array-access (500): Array/indexer access</item>
    /// <item>INCOP (500): Postfix increment (x++)</item>
    /// <item>DECOP (500): Postfix decrement (x--)</item>
    /// <item>POWOP (220): Exponentiation</item>
    /// <item>PREINCOP (210): Prefix increment (++x)</item>
    /// <item>PREDECOP (210): Prefix decrement (--x)</item>
    /// <item>BITNOTOP (210): Bitwise NOT</item>
    /// <item>BOOLNOTOP (210): Boolean NOT</item>
    /// <item>MULOP (200): Multiplication, division, modulo</item>
    /// <item>NEGATEOP (190): Unary negation</item>
    /// <item>ADDOP (180): Addition</item>
    /// <item>SUBOP (180): Subtraction</item>
    /// <item>SHIFTOP (160): Bitwise left shift, right shift</item>
    /// <item>BITOP (140): Bitwise AND, OR, XOR</item>
    /// <item>EQNEQOP (120): Equality and inequality comparisons (==, !=)</item>
    /// <item>GLOP (120): Relational comparisons (&lt;, &gt;, &lt;=, &gt;=)</item>
    /// <item>BOOLOP (100): Boolean logical operations (AND, OR)</item>
    /// <item>ASSIGNOP (80): Assignment and compound assignment</item>
    /// <item>COMMAOP (10): Comma-separated expression sequences</item>
    /// </list>
    /// <para>
    /// This table will be expanded as additional operators are added to the language.
    /// </para>
    /// </remarks>
    static Dictionary<string, OpInfo> operators = new Dictionary<string, OpInfo>()
    {
        { "DOTOP"        ,  new (500, Associativity.LEFT,  2, (tok, structure, member) => {return new DotNode           (tok, structure, member);   }) },
        { "func-call"    ,  new (500, Associativity.LEFT,  2, (tok, func, right)       => {return new FunccallNode      (tok, func, right);         }) },
        { "array-access" ,  new (500, Associativity.LEFT,  2, (tok, structure, right)  => {return new ArrayNode         (tok, structure, right);    }) },
        { "INCOP"        ,  new (500, Associativity.LEFT,  1, (tok, term)              => {return new IncNode           (tok, term);                }) },
        { "DECOP"        ,  new (500, Associativity.LEFT,  1, (tok, term)              => {return new DecNode           (tok, term);                }) },
        { "NEWOP"        ,  new (400, Associativity.RIGHT, 1, (tok, term)              => {return new NewNode           (tok, term);                }) },
        { "CASTOP"       ,  new (300, Associativity.LEFT,  2, (tok, left, right)       => {return new CastNode          (tok, left, right);         }) },
        { "POWOP"        ,  new (220, Associativity.RIGHT, 2, (tok, left, right)       => {return new PowNode           (tok, left, right);         }) },
        { "PREINCOP"     ,  new (210, Associativity.RIGHT, 1, (tok, term)              => {return new PreIncNode        (tok, term);                }) },
        { "PREDECOP"     ,  new (210, Associativity.RIGHT, 1, (tok, term)              => {return new PreDecNode        (tok, term);                }) },
        { "BITNOTOP"     ,  new (210, Associativity.RIGHT, 1, (tok, term)              => {return new BitNotNode        (tok, term);                }) },
        { "BOOLNOTOP"    ,  new (210, Associativity.RIGHT, 1, (tok, term)              => {return new BoolNotNode       (tok, term);                }) },
        { "MULOP"        ,  new (200, Associativity.LEFT,  2, (tok, left, right)       => {return new MulNode           (tok, left, right);         }) },
        { "NEGATEOP"     ,  new (190, Associativity.RIGHT, 1, (tok, term)              => {return new NegateNode        (tok, term);                }) },
        { "ADDOP"        ,  new (180, Associativity.LEFT,  2, (tok, left, right)       => {return new AddNode           (tok, left, right);         }) },
        { "SUBOP"        ,  new (180, Associativity.LEFT,  2, (tok, left, right)       => {return new SubNode           (tok, left, right);         }) },
        { "SHIFTOP"      ,  new (160, Associativity.LEFT,  2, (tok, left, right)       => {return new ShiftNode         (tok, left, right);         }) },
        { "UNRSHIFTOP"   ,  new (160, Associativity.LEFT,  2, (tok, left, right)       => {return new UnRShiftNode      (tok, left, right);         }) },
        { "BITOP"        ,  new (140, Associativity.LEFT,  2, (tok, left, right)       => {return new BitNode           (tok, left, right);         }) },
        { "EQNEQOP"      ,  new (120, Associativity.NONE,  2, (tok, left, right)       => {return new EqNeqNode         (tok, left, right);         }) },
        { "GLOP"         ,  new (120, Associativity.NONE,  2, (tok, left, right)       => {return new GreaterLessNode   (tok, left, right);         }) },
        { "BOOLOP"       ,  new (100, Associativity.LEFT,  2, (tok, left, right)       => {return new BoolNode          (tok, left, right);         }) },     
        { "ASSIGNOP"     ,  new (80,  Associativity.RIGHT, 2, (tok, left, right)       => {return new AssignNode        (tok, left, right);         }) },
        { "COMMAOP"      ,  new (10,  Associativity.LEFT,  2, (tok, left, right)       => {return new CommaNode         (tok, left, right);         }) },
    };

    public virtual ResultLocation getResultLocation()
    {
        if (this.temporary == null)
            throw new Exception("Internal copmmiler error: expression with unintialized temporary count.");
        return this.temporary;
    }

    /// <summary>
    /// Checks if a token symbol represents a binary operator.
    /// </summary>
    /// <param name="sym">The token symbol to check.</param>
    /// <returns>True if the symbol is a recognized binary operator; false otherwise.</returns>
    static bool isOperator(string sym)
    {
        return operators.ContainsKey(sym);
    }

    /// <summary>
    /// Reduces an operator by popping operands and the operator, then pushing the resulting subtree.
    /// </summary>
    /// <param name="operatorStack">Stack of pending operators.</param>
    /// <param name="termStack">Stack of expression subtrees (operands).</param>
    /// <remarks>
    /// <para>
    /// Performs a single reduction step in the shunting-yard algorithm:
    /// </para>
    /// <list type="number">
    /// <item>Pops one operator from the operator stack</item>
    /// <item>Pops two operands from the term stack (right, then left)</item>
    /// <item>Creates a binary operator node using the operator's factory function</item>
    /// <item>Pushes the new subtree back onto the term stack</item>
    /// </list>
    /// </remarks>
    static void applyOperator(Stack<Token> operatorStack, Stack<ExprNode> termStack)
    {
        Token op = operatorStack.Pop();
        if (Tokenizer.isNestingOpener(op.sym) || Tokenizer.isNestingCloser(op.sym))
            Utils.error(new InvalidOperator($"Invalid operator on stack: {op.sym}"));

        ExprNode[] operands = new ExprNode[2]; // unary maker doesn't care about operands[1]
        switch (operators[op.sym].arity)
        {
            case 1:
                if (termStack.Count() < 1)
                    Utils.error(new MissingOperands($"Not enough operands on stack for: {op.sym}"));
                operands[0] = termStack.Pop(); // unary
                break;
            case 2:
                if (termStack.Count() < 2)
                    Utils.error(new MissingOperands($"Not enough operands on stack for: {op.sym}"));
                operands[1] = termStack.Pop(); // right
                operands[0] = termStack.Pop(); // left      
                break;
        }
        ExprNode expression = operators[op.sym].maker(op, operands);
        termStack.Push(expression);
    } 

    /// <summary>
    /// Repeatedly applies operators from the operator stack to the term stack while a condition is met.
    /// </summary>
    /// <param name="tok">The current token being processed, used to check associativity rules.</param>
    /// <param name="operatorStack">The stack of pending operators.</param>
    /// <param name="termStack">The stack of operand expression trees.</param>
    /// <param name="f">Predicate function that receives the top operator's precedence and returns true if the operator should be applied.</param>
    /// <remarks>
    /// Used during expression parsing to reduce operators based on precedence and associativity rules.
    /// Stops when the operator stack is empty, when the top of the operator stack is a nesting opener,
    /// when a right-associative unary operator would incorrectly consume a binary operand, or when
    /// the predicate returns false.
    /// </remarks>
    public static void applyOperatorWhileTrue(Token tok, Stack<Token> operatorStack, Stack<ExprNode> termStack, Func<int,bool> f)
    {
        while(true)
        {
            if (operatorStack.Count == 0)
                return;
            if (Tokenizer.isNestingOpener(operatorStack.Peek().sym))
                return;
            if (isOperator(tok.sym) && 
                    operators[tok.sym].associativity == Associativity.RIGHT && operators[tok.sym].arity == 1 &&
                    operators[operatorStack.Peek().sym].arity == 2)
                return;
            bool shouldApply = f(operators[operatorStack.Peek().sym].precedence);
            if (!shouldApply)
                return;
            applyOperator(operatorStack, termStack);
        }
    }

    /// <summary>
    /// Returns <c>true</c> when the token pair indicates a function call or array access should be
    /// injected as a synthetic operator (e.g., <c>id(</c>, <c>)(</c>, <c>][</c>).
    /// </summary>
    private static bool isCallOrIndex(Token tok, Token next)
    {
        return (tok.sym == TokenSymbols.ID     && next.sym == TokenSymbols.LPAREN) || // e.g. - func()
               (tok.sym == TokenSymbols.ID     && next.sym == TokenSymbols.LBRACK) || // e.g. - A[0] - (Source: Anthropic)
               (tok.sym == TokenSymbols.RPAREN && next.sym == TokenSymbols.LPAREN) || // e.g. - func(1)(2)
               (tok.sym == TokenSymbols.RPAREN && next.sym == TokenSymbols.LBRACK) || // e.g. - f()[0]
               (tok.sym == TokenSymbols.RBRACK && next.sym == TokenSymbols.LPAREN) || // e.g. - A[0](1,2,3)
               (tok.sym == TokenSymbols.RBRACK && next.sym == TokenSymbols.LBRACK);   // e.g. - A[1][2]
    }

    enum AutomatonState
    {
        EXPECTING_TERM_OR_PREFIX_OP,
        EXPECTING_INFIX_OR_POSTFIX_OP
    }
    
    /// <summary>
    /// Processes a single token during expression parsing, updating the operator and term stacks.
    /// </summary>
    /// <param name="T">The tokenizer, used for error reporting (line and column info).</param>
    /// <param name="state">
    /// The current automaton state, indicating whether a term/prefix operator or an
    /// infix/postfix operator is expected next. Updated in place as tokens are consumed.
    /// </param>
    /// <param name="tok">The token to process.</param>
    /// <param name="operatorStack">Stack of pending operators.</param>
    /// <param name="termStack">Stack of expression subtrees (operands).</param>
    /// <remarks>
    /// <para>
    /// Handles the core per-token logic of the shunting-yard algorithm:
    /// </para>
    /// <list type="number">
    /// <item>Operands (NUM, ID) are pushed directly onto the term stack as <see cref="Term"/> nodes.</item>
    /// <item>Opening groupers (e.g., LPAREN) are pushed onto the operator stack.</item>
    /// <item>Closing groupers (e.g., RPAREN) trigger reductions until the matching opener is found, then discard it.</item>
    /// <item>
    ///     Binary operators are reduced against the stack based on their associativity:
    ///     left-associative operators reduce same-or-higher-precedence operators first;
    ///     right-associative operators reduce only strictly-higher-precedence operators;
    ///     non-associative operators reduce strictly-higher-precedence operators and error on consecutive same-operator use.
    /// </item>
    /// <item>
    ///     Unary operators are disambiguated by the current automaton state: prefix operators
    ///     (e.g., unary minus, ++x, --x) are recognized when a term is expected, and postfix
    ///     operators (e.g., x++, x--) are recognized when an infix or postfix operator is expected.
    ///     SUBOP, INCOP, and DECOP are re-tagged to their prefix variants (NEGATEOP, PREINCOP, PREDECOP)
    ///     when encountered in prefix position.
    /// </item>
    /// </list>
    /// </remarks>
    /// <exception cref="MissingExpression">
    /// Reported via <see cref="Utils.error"/> when an unrecognized token is encountered that is neither an operand, grouper, nor known operator.
    /// </exception>
    static void handleToken(Tokenizer T, ref AutomatonState state, Token tok, Stack<Token> operatorStack, Stack<ExprNode> termStack) // ref => C# global changes
    {
        if (tok.sym == TokenSymbols.SUBOP && state == AutomatonState.EXPECTING_TERM_OR_PREFIX_OP)
            tok.sym = TokenSymbols.NEGATEOP;
        if (tok.sym == TokenSymbols.INCOP && state == AutomatonState.EXPECTING_TERM_OR_PREFIX_OP)
            tok.sym = TokenSymbols.PREINCOP;
        if (tok.sym == TokenSymbols.DECOP && state == AutomatonState.EXPECTING_TERM_OR_PREFIX_OP)
            tok.sym = TokenSymbols.PREDECOP;

        if (tok.sym == TokenSymbols.NUM || tok.sym == TokenSymbols.ID || tok.sym == TokenSymbols.FNUM ||
            tok.sym == TokenSymbols.BOOLCONST || tok.sym == TokenSymbols.STRINGCONST || tok.sym == TokenSymbols.TYPE ||
            tok.sym == TokenSymbols.THIS)
        {
            if (state == AutomatonState.EXPECTING_TERM_OR_PREFIX_OP)
            {
                // Operands go directly to term stack
                if (tok.sym == TokenSymbols.ID || tok.sym == TokenSymbols.THIS)
                    termStack.Push(new Variable(tok));
                else if (tok.sym == TokenSymbols.TYPE)
                    termStack.Push(new CastType(tok));
                else   
                    termStack.Push(new Term(tok));
                state = AutomatonState.EXPECTING_INFIX_OR_POSTFIX_OP;
            }               
            else
            {
                Utils.error(new InvalidState($"Expecting term or prefix operator on line {T.getLine()}, got: {tok.lexeme}"));
            }
        }
        else if (Tokenizer.isNestingOpener(tok.sym))
        {
            if (state == AutomatonState.EXPECTING_TERM_OR_PREFIX_OP)
            {
                // Add opening grouper to the operatorStack; don't change states
                operatorStack.Push(tok);
            }
            else
            {
                Utils.error(new InvalidState($"Expecting nesting opener (prefix operator) on line {T.getLine()}, got: {tok.sym}"));
            }
        }
        else if (Tokenizer.isNestingCloser(tok.sym))
        {
            if (state == AutomatonState.EXPECTING_INFIX_OR_POSTFIX_OP)
            {
                applyOperatorWhileTrue(tok, operatorStack, termStack, (stackPrec) =>
                {
                    return operatorStack.Peek().lexeme != Tokenizer.getMatchingNestableLex(tok.sym);
                });
                // Discard opener; don't change states
                operatorStack.Pop();
            }
            else
            {
                Utils.error(new InvalidState($"Expecting nesting closer (postfix operator) on line {T.getLine()}, got: {tok.sym}"));
            }
        }
        else if (isOperator(tok.sym))
        {
            OpInfo opInfo = operators[tok.sym];
            switch (opInfo.arity)
            {
                case 2:
                    // binary operators
                    if (state == AutomatonState.EXPECTING_INFIX_OR_POSTFIX_OP)
                    {
                        // binary operators can be: LA, NONE, or RA
                        switch (opInfo.associativity)
                        {
                            case Associativity.LEFT:
                                applyOperatorWhileTrue(tok, operatorStack, termStack, (stackPrec) => 
                                {
                                    return operators[operatorStack.Peek().sym].precedence >= opInfo.precedence;
                                });
                                break;
                            case Associativity.NONE:
                                applyOperatorWhileTrue(tok, operatorStack, termStack, (stackPrec) =>
                                {
                                    return operators[operatorStack.Peek().sym].precedence > opInfo.precedence;
                                });
                                if (operatorStack.Count > 0 && operatorStack.Peek().sym == tok.sym)
                                    Utils.error(new InvalidOperator($"Consecutive use of right-associative operators on line {T.getLine()}"));
                                break;
                            case Associativity.RIGHT:
                                applyOperatorWhileTrue(tok, operatorStack, termStack, (stackPrec) =>
                                {
                                    return operators[operatorStack.Peek().sym].precedence > opInfo.precedence;
                                });
                                break;
                            default:
                                Utils.error(new InvalidOperator($"Binary operator with uninitialized associativity: {tok.sym}"));
                                break;
                        }

                        state = AutomatonState.EXPECTING_TERM_OR_PREFIX_OP;
                    }
                    else
                    {
                        Utils.error(new InvalidState($"Expecting infix binary operator on line {T.getLine()}, got: {tok.sym}"));
                    }
                    break;

                case 1:
                    // unary operators
                    if (state == AutomatonState.EXPECTING_TERM_OR_PREFIX_OP)
                    {
                        // prefix unary operators are RA
                        switch (opInfo.associativity)
                        {
                            case Associativity.RIGHT:
                                applyOperatorWhileTrue(tok, operatorStack, termStack, (stackPrec) =>
                                {
                                    return operators[operatorStack.Peek().sym].precedence > opInfo.precedence;
                                });
                                break;
                            default:
                                Utils.error(new InvalidState($"Expecting prefix unary operator on line {T.getLine()}, got: {tok.sym}"));
                                break;
                            // don't change state; operator can be repeated
                        }
                    }
                    else if (state == AutomatonState.EXPECTING_INFIX_OR_POSTFIX_OP)
                    {
                        // postfix unary operators are LA
                        switch (opInfo.associativity)
                        {
                            case Associativity.LEFT:
                                applyOperatorWhileTrue(tok, operatorStack, termStack, (stackPrec) =>
                                {
                                    return operators[operatorStack.Peek().sym].precedence > opInfo.precedence;
                                });
                                break;
                            default:
                                Utils.error(new InvalidState($"Expecting infix or postfix unary operator on line {T.getLine()}, got: {tok.sym}"));
                                break;
                            // don't change state; operator can be repeated
                        }
                    }
                    else
                    {
                        Utils.error(new InvalidState($"Expecting prefix or postfix unary operator on line {T.getLine()}, got: {tok.sym}"));
                    }
                    break;
            }
            
            // Push current operator
            operatorStack.Push(tok);
        }
        else
            Utils.error(new MissingExpression($"Expected term or operator at line {tok.line}, got: {tok.sym}")); // does tok.line fix bad lines (using T.getLine())?
    }

    /// <summary>
    /// Determines whether the token stream is positioned at the start of a parseable expression.
    /// </summary>
    /// <param name="T">The tokenizer to peek at.</param>
    /// <returns>
    /// <c>true</c> if the next token is a valid expression-starting symbol (numeric literal,
    /// identifier, left parenthesis, or a prefix unary operator); otherwise <c>false</c>.
    /// </returns>
    public static bool canParse(Tokenizer T)
    {
        string sym = T.peek();
        switch (sym)
        {
            case TokenSymbols.NUM:
            case TokenSymbols.FNUM:
            case TokenSymbols.STRINGCONST:
            case TokenSymbols.BOOLCONST:
            case TokenSymbols.ID:
            case TokenSymbols.LPAREN:
            case TokenSymbols.SUBOP:
            case TokenSymbols.BITNOTOP:
            case TokenSymbols.BOOLNOTOP:
            case TokenSymbols.THIS:
            case TokenSymbols.NEWOP:
                return true;
        }
        return false;
    }

    /// <summary>
    /// Parses an expression from the token stream using the shunting-yard algorithm.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>An <see cref="ExprNode"/> representing the root of the parsed expression tree.</returns>
    /// <remarks>
    /// <para>
    /// Reads tokens until a terminating symbol (LBRACE, EOS, or EOF), then delegates
    /// per-token processing to <see cref="handleToken"/>. After all tokens are consumed,
    /// reduces any remaining operators on the stack and returns the final expression tree.
    /// </para>
    /// <para>
    /// This method also handles the special case of zero-argument function calls: when an ID
    /// token is immediately followed by LPAREN and RPAREN, a <see cref="NoArgs"/> sentinel is
    /// pushed onto the term stack and the call is reduced immediately, consuming both parentheses.
    /// </para>
    /// <para>
    /// Note: EOF is included as a terminating symbol to handle files that end without
    /// a trailing newline after an expression.
    /// </para>
    /// </remarks>
    /// <exception cref="MissingExpression">Reported via <see cref="Utils.error"/> when no tokens are found (empty expression).</exception>
    /// <exception cref="InvalidExpression">Reported via <see cref="Utils.error"/> when the final reduction does not result in exactly one expression tree, indicating malformed input.</exception>
    public static new ExprNode parse(Tokenizer T)
    {
        // Read tokens until we hit a statement-ending or block-starting symbol
        // EOF is needed for files that end without a newline after an expression
        List<Token> tokens = T.readUntil(consume:true, TokenSymbols.LBRACE, TokenSymbols.EOS, TokenSymbols.EOF);
        Stack<Token> operatorStack = new Stack<Token>();
        Stack<ExprNode> termStack = new Stack<ExprNode>();

        AutomatonState state = AutomatonState.EXPECTING_TERM_OR_PREFIX_OP;

        if (tokens.Count == 0)
            Utils.error(new MissingExpression($"Expected expression at line {T.getLine()}"));

        // Process each token using shunting-yard algorithm
        for (int i = 0; i < tokens.Count; i++)
        {
            Token tok = tokens[i];
            handleToken(T, ref state, tok, operatorStack, termStack);
            
            Token next1 = new Token();
            if (i < tokens.Count - 1)
                next1 = tokens[i+1];     
            if (isCallOrIndex(tok, next1))
            {
                string opName = next1.sym == TokenSymbols.LPAREN ? TokenSymbols.FUNCCALL : TokenSymbols.ARRACCESS;
                handleToken(T, ref state, new Token(opName, opName, T.getLine(), T.getColumn()),
                    operatorStack, termStack);

                Token next2 = new Token();
                if (i < tokens.Count - 2)
                    next2 = tokens[i+2];
                if (next2.sym == TokenSymbols.RPAREN)
                {
                    termStack.Push(new NoArgs(new Token(TokenSymbols.NOARGS, TokenSymbols.NOARGS, T.getLine(), T.getColumn())));
                    applyOperator(operatorStack, termStack);
                    i += 2; // consume both '(' and ')' from tokens List
                    state = AutomatonState.EXPECTING_INFIX_OR_POSTFIX_OP; // adjust state based on consumed input
                }
            } 
        }

        // Reduce all remaining operators
        while (operatorStack.Count > 0)
            applyOperator(operatorStack, termStack);

        // Should have exactly one expression tree remaining
        if (termStack.Count != 1)
            Utils.error(new InvalidExpression($"Unable to reduce expression tree to 1; Tree: {tokens[^1]}"));

        return termStack.Pop();
    }

    /// <summary>
    /// Serializes this expression node and its subtree to JSON format.
    /// </summary>
    /// <returns>A JSON string representation of the expression tree.</returns>
    /// <remarks>
    /// <para>
    /// Recursively writes the expression tree as a JSON object with the structure:
    /// </para>
    /// <code>
    /// {
    ///   "token": "lexeme",
    ///   "children": [child1, child2, ...]
    /// }
    /// </code>
    /// <para>
    /// Each child node is serialized recursively, creating a complete JSON representation
    /// of the entire expression tree.
    /// </para>
    /// </remarks>
    public string toJson()
    {
        string _data = "{";
        _data = _data + $"\"token\":\"{this.token.lexeme}\",";
        _data = _data + "\"children\":[";
        //recursive magic
        List<TreeNode> children = this.getChildren();
        for (int i = 0; i < children.Count; i++)
        {
            if (i != 0)
                _data = _data + ",";
            _data = _data + (children[i] as ExprNode)!.toJson(); // ! => "I know this will not be null"
        }
        _data = _data + "]";
        _data = _data + "}";
        return _data;
    }
}