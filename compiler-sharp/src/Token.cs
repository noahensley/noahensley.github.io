/*
 * File: Token.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 14, 2026,
 *      January 25, 2026
 */

/// <summary>
/// Represents a lexical token with its symbol type, lexeme, and position information.
/// </summary>
public class Token 
{
    /// <summary>
    /// The symbol type of the token (e.g., "ID", "NUM", "WHILE").
    /// </summary>
    public string sym;
    
    /// <summary>
    /// The actual text content of the token from the source.
    /// </summary>
    public string lexeme;
    
    /// <summary>
    /// The line number where the token appears in the source.
    /// </summary>
    public int line;
    
    /// <summary>
    /// The column number where the token starts in the source.
    /// </summary>
    public int column;

    /// <summary>
    /// Creates a default token with TokenSymbols.NULL values and position -1.
    /// </summary>
    public Token()
    {
        this.sym = TokenSymbols.NULL;
        this.lexeme = TokenSymbols.NULL;
        this.line = -1;
        this.column = -1;
    }
    
    /// <summary>
    /// Creates a token with the specified properties.
    /// </summary>
    /// <param name="sym">The symbol type.</param>
    /// <param name="lexeme">The lexeme text.</param>
    /// <param name="line">The line number.</param>
    /// <param name="column">The column number.</param>
    public Token(string sym, string lexeme, int line, int column)
    {
        this.sym = sym;
        this.lexeme = lexeme;
        this.line = line;
        this.column = column;
    }

    /// <summary>
    /// Checks if this token is the default null token.
    /// </summary>
    /// <returns>True if the token has NULL symbol, NULL lexeme, and position -1; false otherwise.</returns>
    public bool IsNull()
    {
        return this.sym == TokenSymbols.NULL && this.lexeme == TokenSymbols.NULL && this.line == -1 && this.column == -1;
    }

    /// <summary>
    /// Returns a string representation of the token in the format [sym lexeme line column].
    /// </summary>
    /// <returns>A formatted string showing the token's properties.</returns>
    public override string ToString()
    {
        return $"[{this.sym} {this.lexeme} {this.line} {this.column}]";
    }
}