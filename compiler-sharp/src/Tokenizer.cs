/*
 * File: Tokenizer.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate or review XML documentation comments
 * and refactor the code for this file on the following dates:
 *      January 14, 2026
 *      January 20, 2026
 *      January 25, 2026
 *      January 28, 2026
 *      February 17, 2026
 */

using System.Diagnostics;
using System.Text.RegularExpressions;

/// <summary>
/// Performs lexical analysis on input text, converting it into a stream of tokens.
/// </summary>
/// <remarks>
/// <para>
/// The tokenizer handles automatic whitespace skipping, string unescaping, nesting validation
/// for braces/parentheses/brackets, and end-of-statement detection based on newlines.
/// It maintains a bidirectional token history allowing for lookahead (peek) and rewind operations.
/// </para>
/// <para>
/// <b>Key features:</b>
/// </para>
/// <list type="bullet">
/// <item>Automatic whitespace handling with newline-based statement detection</item>
/// <item>String literal unescaping (removes quotes and processes escape sequences)</item>
/// <item>Balanced delimiter tracking (braces, parentheses, brackets)</item>
/// <item>Lookahead support via peek() and bidirectional navigation via rewind()</item>
/// <item>Context-aware EOS (end-of-statement) generation suppressed inside parentheses/brackets</item>
/// </list>
/// </remarks>
public class Tokenizer
{
    private string _input;
    private int _index;
    private int _line;
    private int _column;
    private Stack<Token> _nesting;
    private int _paren_bracket_depth;
    private Stack<Token> _past;
    private Stack<Token> _future;

    /// <summary>
    /// Enumeration of possible nesting error states.
    /// </summary>
    private enum nestingError
    {
        /// <summary>No nesting error detected.</summary>
        OK,
        /// <summary>An opening delimiter was never closed.</summary>
        unmatchedOpener,
        /// <summary>A closing delimiter doesn't match its corresponding opener.</summary>
        mismatchedCloser
    }

    private nestingError _nestErr;

    /// <summary>
    /// Creates a new tokenizer with empty input starting at line 1, column 0.
    /// </summary>
    public Tokenizer()
    {
        _input = "";
        _index = 0;
        _line = 1;
        _column = 0;
        _nesting = new Stack<Token>();
        _paren_bracket_depth = 0;
        _past = new Stack<Token>();
        _future = new Stack<Token>();
        _nestErr = nestingError.OK;
    }
    
    /// <summary>
    /// Sets the input string to be tokenized and resets all tokenizer state.
    /// </summary>
    /// <param name="input">The source text to tokenize.</param>
    /// <remarks>
    /// Resets position to start, clears nesting stack, and discards token history.
    /// </remarks>
    public void setInput(string input)
    {
        _input = input;
        _index = 0;
        _line = 1;
        _column = 0;
        _nesting.Clear();
        _paren_bracket_depth = 0;
        _past.Clear();
        _future.Clear();
        _nestErr = nestingError.OK;
    }

    /// <summary>
    /// Checks whether the tokenizer has reached the end of input.
    /// </summary>
    /// <returns>True if at end of input, false otherwise.</returns>
    public bool isEndOfInput()
    {
        return _index >= _input.Length;
    }

    /// <summary>
    /// Determines whether a token symbol represents a nestable opening delimiter.
    /// </summary>
    /// <param name="sym">The token symbol to classify.</param>
    /// <returns>True if the symbol is LBRACE, LPAREN, or LBRACK; false otherwise.</returns>
    public static bool isNestingOpener(string sym)
    {
        return sym == TokenSymbols.LBRACE || sym == TokenSymbols.LPAREN || sym == TokenSymbols.LBRACK;
    }

    /// <summary>
    /// Determines whether a token symbol represents a nestable closing delimiter.
    /// </summary>
    /// <param name="sym">The token symbol to classify.</param>
    /// <returns>True if the symbol is RBRACE, RPAREN, or RBRACK; false otherwise.</returns>
    public static bool isNestingCloser(string sym)
    {
        return sym == TokenSymbols.RBRACE || sym == TokenSymbols.RPAREN || sym == TokenSymbols.RBRACK;
    }

    /// <summary>
    /// Determines whether a closing delimiter mismatches the current nesting context.
    /// </summary>
    /// <param name="sym">The closing delimiter symbol to check.</param>
    /// <returns>
    /// True if the nesting stack is empty or if the closer doesn't match the top opener; 
    /// false otherwise.
    /// </returns>
    /// <remarks>
    /// Checks if the closer matches its corresponding opener by converting the closer
    /// to an opener form (e.g., "RBRACE" → "LBRACE") and comparing with the stack top.
    /// </remarks>
    public bool isNestingMismatch(string sym)
    {
        return _nesting.Count() == 0 || ("L" + sym[1..]) != _nesting.Peek().sym;
    }

    /// <summary>
    /// Determines whether a token suppresses end-of-statement detection across newlines.
    /// </summary>
    /// <param name="sym">The token symbol to classify.</param>
    /// <returns>
    /// True if the symbol is a parenthesis or bracket (which suppress EOS across newlines); 
    /// false otherwise.
    /// </returns>
    /// <remarks>
    /// Parentheses and brackets allow multi-line expressions without generating EOS tokens
    /// at each newline, enabling natural line breaks in function calls and array literals.
    /// </remarks>
    public static bool isEOSSuppressing(string nest_sym)
    {
        string trim = nest_sym[1..];
        return trim == TokenSymbols.LPAREN[1..] || trim == TokenSymbols.LBRACK[1..];
    }

    /// <summary>
    /// Finds the matching closing delimiter for a given opening delimiter symbol.
    /// </summary>
    /// <param name="nest_sym">The nestable opening symbol (LBRACE, LPAREN, or LBRACK).</param>
    /// <returns>
    /// The closing delimiter character (}, ), or ]) if a match exists; 
    /// empty string otherwise.
    /// </returns>
    public static string getMatchingNestableLex(string nest_sym)
    {
        switch (nest_sym)
        {
            case TokenSymbols.LBRACE:
                return "}";
            case TokenSymbols.LPAREN:
                return ")";
            case TokenSymbols.LBRACK:
                return "}";
            case TokenSymbols.RBRACE:
                return "{";
            case TokenSymbols.RPAREN:
                return "(";
            case TokenSymbols.RBRACK:
                return "{";
            default:
                Utils.error(new UnexpectedToken($"Expected nestable token symbol, got: {nest_sym}"));
                throw new UnreachableException();
        }
    }

    /// <summary>
    /// Rewinds the input stream by moving tokens from past history to future queue.
    /// </summary>
    /// <param name="n">The number of tokens to rewind (default: 1).</param>
    /// <exception cref="Exception">Thrown when attempting to rewind beyond available history.</exception>
    /// <remarks>
    /// Enables backtracking by restoring previously consumed tokens. Tokens moved to the
    /// future queue will be returned by subsequent next() calls before new tokens are read.
    /// </remarks>
    public void rewind(int n = 1)
    {
        while (n > 0)
        {
            if (_past.Count() == 0)
                Utils.error(new UnexpectedEndOfInput("Cannot rewind past beginning of input."));

            Token t = _past.Pop();
            _future.Push(t);
            n--;
        }
    }
    
    /// <summary>
    /// Handles detected nesting errors by throwing detailed exceptions.
    /// </summary>
    /// <param name="tok">The current token being processed when the error is detected.</param>
    /// <exception cref="Exception">Thrown for unmatched opening delimiters.</exception>
    /// <exception cref="Exception">Thrown for mismatched closing delimiters.</exception>
    /// <exception cref="Exception">Thrown for invalid nesting error states.</exception>
    private void handleNestingErrors(Token tok)
    {
        if (_nestErr == nestingError.OK)
            return;

        switch (_nestErr)
        {
            case nestingError.unmatchedOpener:
                handleUnmatchedOpener(tok);
                break;
            case nestingError.mismatchedCloser:
                handleMismatchedCloser(tok);
                break;
            default:
                Utils.error(new InvalidState($"Expected unmatched, mismatched, or OK; got: {_nestErr}"));
                throw new UnreachableException();
        }
    }

    /// <summary>
    /// Handles unmatched opening delimiter errors.
    /// </summary>
    /// <param name="tok">The current token when the error was detected.</param>
    /// <exception cref="Exception">Thrown with details about the unclosed delimiter.</exception>
    /// <remarks>
    /// Provides context about the unmatched opener including its location and the expected
    /// closing delimiter, along with what was actually found.
    /// </remarks>
    private void handleUnmatchedOpener(Token tok)
    {
        Token unmatched = _nesting.Peek();
        string lex = unmatched.lexeme;
        int line = unmatched.line;
        int col = unmatched.column;
        string actual = getLexFromSym(tok.sym);
        string expected = getMatchingNestableLex(unmatched.sym);

        Utils.error(new UnexpectedToken($"Unclosed {lex} at ({line},{col}); expected '{expected}', got {actual}"));
    }

    /// <summary>
    /// Handles mismatched closing delimiter errors.
    /// </summary>
    /// <param name="tok">The current token causing the mismatch.</param>
    /// <exception cref="Exception">Thrown with details about the unexpected closer.</exception>
    /// <remarks>
    /// If there's an unclosed opener on the stack, suggests the correct closing delimiter
    /// to help guide the user toward the fix.
    /// </remarks>
    private void handleMismatchedCloser(Token tok)
    {
        string lex = tok.lexeme;
        int line = tok.line;
        int col = tok.column;
        string suggestion = "";
        
        if (_nesting.Count() > 0)
        {
            string suggested = getMatchingNestableLex(_nesting.Peek().sym);
            suggestion = $"; did you mean '{suggested}'?";
        }
        
        Utils.error(new UnexpectedToken($"Unexpected {lex} at ({line},{col}) {suggestion}"));
    }

    /// <summary>
    /// Gets a display-friendly lexeme for error messages.
    /// </summary>
    /// <param name="sym">The token symbol for which to get a display representation.</param>
    /// <returns>A readable string representation of the token.</returns>
    /// <exception cref="Exception">Thrown for unrecognized whitespace token symbols.</exception>
    private string getLexFromSym(string sym)
    {
        if (sym != TokenSymbols.WHITESPACE && sym != TokenSymbols.EOF && sym != TokenSymbols.EOS)
            return sym;

        switch (sym)
        {
            case TokenSymbols.EOF:
                return "EOF";
            case TokenSymbols.EOS:
                return "EOS";
            default:
                Utils.error(new InvalidToken($"Cannot get lexeme for token: {sym}"));
                throw new UnreachableException();
        }
    }

    /// <summary>
    /// Removes surrounding quotes and unescapes a string constant token.
    /// </summary>
    /// <param name="tok">The string token to unescape (modified in place).</param>
    /// <remarks>
    /// Strips the leading and trailing quote characters, then processes escape sequences
    /// (e.g., \n, \t, \\) to their actual character representations.
    /// </remarks>
    private void unescapeStringToken(Token tok)
    {
        tok.lexeme = tok.lexeme[1..(tok.lexeme.Length - 1)];
        tok.lexeme = Regex.Unescape(tok.lexeme);
    }

    /// <summary>
    /// Updates line and column tracking after encountering a newline.
    /// </summary>
    /// <param name="tok">The whitespace token containing the newline.</param>
    /// <param name="newlineIndex">The index of the newline character in the token.</param>
    /// <remarks>
    /// Increments the line counter and adjusts the column to account for characters
    /// appearing after the newline in the same token.
    /// </remarks>
    private void updateLineAndColumn(Token tok, int newlineIndex)
    {
        _line++;
        _column = tok.lexeme.Length - 1 - newlineIndex;
    }

    /// <summary>
    /// Handles encountering an opening nesting delimiter.
    /// </summary>
    /// <param name="tok">The opening delimiter token.</param>
    /// <remarks>
    /// Pushes the token onto the nesting stack and increments the EOS suppression depth
    /// if the delimiter is a parenthesis or bracket.
    /// </remarks>
    private void handleNestingOpener(Token tok)
    {
        _nesting.Push(tok);
        if (isEOSSuppressing(tok.sym))
            _paren_bracket_depth++;
    }

    /// <summary>
    /// Handles encountering a closing nesting delimiter.
    /// </summary>
    /// <param name="tok">The closing delimiter token.</param>
    /// <remarks>
    /// Decrements the EOS suppression depth if applicable, then either pops the matching
    /// opener from the stack or records a mismatch error.
    /// </remarks>
    private void handleNestingCloser(Token tok)
    {
        if (isEOSSuppressing(tok.sym))
            _paren_bracket_depth--;

        if (isNestingMismatch(tok.sym))
            _nestErr = nestingError.mismatchedCloser;
        else
            _nesting.Pop();
    }

    /// <summary>
    /// Retrieves the next token from the input stream.
    /// </summary>
    /// <returns>
    /// The next token, an EOF token ("$") when input is exhausted, or an EOS token ("$$") 
    /// at the end of a statement.
    /// </returns>
    /// <remarks>
    /// <para>This method automatically:</para>
    /// <list type="bullet">
    /// <item>Skips whitespace tokens (unless they contain newlines that trigger EOS)</item>
    /// <item>Unescapes string constants (removes quotes and processes escape sequences)</item>
    /// <item>Tracks nesting of braces, parentheses, and brackets</item>
    /// <item>Generates end-of-statement ($$) tokens on newlines when not inside parentheses/brackets</item>
    /// <item>Uses the future stack for lookahead operations (peek/rewind)</item>
    /// </list>
    /// <para>
    /// EOS tokens are generated when a newline is encountered outside of parentheses and brackets,
    /// enabling statement-oriented parsing while allowing multi-line expressions.
    /// </para>
    /// </remarks>
    /// <exception cref="Exception">Thrown when no terminal pattern matches the current input position.</exception>
    /// <exception cref="Exception">Thrown for nesting errors (unmatched or mismatched delimiters).</exception>
    public Token next()
    {
        bool foundMatch = false;
        Token tok = new Token();

        // Return token from future queue if available (from peek/rewind)
        if (_future.Count() > 0)
        {
            tok = _future.Pop();
            _past.Push(tok);
            return tok;
        }
        
        // Match input against terminal patterns
        foreach(var terminal in Terminals.Items)
        {
            var match = terminal.Rex.Match(_input, _index);
            if (match.Success)
            {
                foundMatch = true;  
                tok = new Token(terminal.Sym, match.Value, _line, _column);
                _index += tok.lexeme.Length;
                _column += tok.lexeme.Length;

                // Handle string constants
                if (tok.sym == TokenSymbols.STRINGCONST)
                {  
                    unescapeStringToken(tok);
                } 
                // Handle whitespace and newline-based EOS generation
                else if (tok.sym == TokenSymbols.WHITESPACE)
                {
                    int iNewLine = tok.lexeme.LastIndexOf('\n');
                    if (iNewLine != -1)
                    {                  
                        updateLineAndColumn(tok, iNewLine);
                        
                        // Check for unclosed delimiters at EOF
                        if (isEndOfInput())
                        {
                            if (_nesting.Count() != 0)
                            {
                                _nestErr = nestingError.unmatchedOpener;
                                break;
                            }
                        }
                        else
                        {
                            // Generate EOS token if not inside parentheses/brackets
                            if (_paren_bracket_depth == 0)
                            {
                                if (peek() != TokenSymbols.EOS)
                                    tok = new Token(TokenSymbols.EOS, "", tok.line, tok.column);
                                else
                                    return next();
                                break;
                            }                   
                        }              
                    }

                    // Skip whitespace tokens (recursively get next token)
                    _past.Push(tok);
                    return next();
                }
                // Handle opening delimiters
                else if (isNestingOpener(tok.sym))
                {
                    handleNestingOpener(tok);
                }
                // Handle closing delimiters
                else if (isNestingCloser(tok.sym))
                {
                    handleNestingCloser(tok);
                }

                break;
            }
        }

        // Handle end of input or no match found
        if (!foundMatch)
        {
            if (isEndOfInput())
            {
                if (_nesting.Count() != 0)
                    _nestErr = nestingError.unmatchedOpener;

                tok = new Token(TokenSymbols.EOF, "", _line, _column);
            }
            else
                Utils.error(new UnexpectedToken($"Match not found for: '{_input[_index]}' at Ln: {_line} Col: {_column}"));
        }

        handleNestingErrors(tok);
        _past.Push(tok);
        return tok;
    }

    /// <summary>
    /// Looks at the next token without consuming it from the input stream.
    /// </summary>
    /// <returns>The symbol of the next token.</returns>
    /// <remarks>
    /// This method internally calls next() and then rewind() to restore state,
    /// enabling lookahead without affecting the token stream position.
    /// </remarks>
    public string peek()
    {
        Token t = next();
        rewind();
        return t.sym;
    }

    /// <summary>
    /// Expects a specific token symbol and consumes it if present.
    /// </summary>
    /// <param name="sym">The expected token symbol.</param>
    /// <returns>The consumed token if it matches the expected symbol.</returns>
    /// <exception cref="Exception">
    /// Thrown if the next token doesn't match the expected symbol, with details about
    /// what was expected versus what was found.
    /// </exception>
    public Token expect(string sym)
    {
        if (peek() == sym)
        {
            Token t = next();
            return t;
        }

        Utils.error(new UnexpectedToken($"Expected {sym}, got {peek()} ({_line},{_column})"));
        throw new UnreachableException();
    }

    /// <summary>
    /// Consumes and collects tokens until one of the specified terminating symbols is encountered.
    /// </summary>
    /// <param name="terminatingSymbol">
    /// One or more token symbols that terminate tokenization. The terminating token is not included
    /// in the returned list.
    /// </param>
    /// <returns>A list of consumed tokens up to (but not including) the terminating symbol.</returns>
    /// <exception cref="Exception">Thrown if the end of input is reached before finding a terminating symbol.</exception>
    /// <exception cref="Exception">Thrown if nesting becomes imbalanced during collection.</exception>
    /// <remarks>
    /// This method respects nesting depth; it only terminates on matching symbols that appear
    /// at the same nesting level where collection began. This prevents premature termination
    /// when the terminating symbol appears inside nested structures.
    /// </remarks>
    public List<Token> readUntil(params string[] terminatingSymbol)
    {
        if (_index >= _input.Length)
            Utils.error(new UnexpectedEndOfInput("Cannot read past end of input."));
        
        List<Token> tokens = new List<Token>();
        int nesting = 0;
        
        while (true)
        {
            string next_sym = peek();

            // Track nesting depth for EOS-suppressing delimiters
            if (isEOSSuppressing(next_sym))
            {
                switch (next_sym)
                {
                    case TokenSymbols.LPAREN:
                    case TokenSymbols.LBRACK:
                    case TokenSymbols.LBRACE:
                        nesting++;
                        break;
                    case TokenSymbols.RPAREN:
                    case TokenSymbols.RBRACK:
                    case TokenSymbols.RBRACE:
                        nesting--;
                        break;
                    default:
                        Utils.error(new InvalidEOSSuppressor($"Expected opener or closer, got: {next_sym}"));
                        throw new Exception();
                }
                    
                if (nesting < 0)
                    Utils.error(new NestingImbalance("Got more closers than openers."));
            }

            // Check for terminating symbol at current nesting level
            if (nesting == 0 && terminatingSymbol.Contains(next_sym))
                break;

            tokens.Add(next());
        }

        return tokens;
    }

    /// <summary>
    /// Gets the current line number in the input.
    /// </summary>
    /// <returns>The current line number (1-indexed).</returns>
    public int getLine()
    {
        return _line;
    }

    /// <summary>
    /// Gets the current column number in the input.
    /// </summary>
    /// <returns>The current column number (0-indexed).</returns>
    public int getColumn()
    {
        return _column;
    }
}