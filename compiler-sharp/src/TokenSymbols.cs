/*
 * File: TokenSymbols.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate this file
 * on January 25, 2026.
 */

/// <summary>
/// Central repository for all terminal symbol constants used throughout the tokenizer and parser.
/// </summary>
public static class TokenSymbols
{
    // Keywords
    public const string VAR = "VAR";
    public const string TYPE = "TYPE";
    public const string FUNCDEF = "FUNCDEF";
    public const string RETURN = "RETURN";
    public const string WHILE = "WHILE";
    public const string IF = "IF";
    public const string ELIF = "ELIF";
    public const string ELSE = "ELSE";

    // Literals
    public const string FNUM = "FNUM";
    public const string NUM = "NUM";
    public const string BOOLCONST = "BOOLCONST";
    public const string STRINGCONST = "STRINGCONST";
    public const string ID = "ID";

    // Delimiters
    public const string LBRACE = "LBRACE";
    public const string RBRACE = "RBRACE";
    public const string LPAREN = "LPAREN";
    public const string RPAREN = "RPAREN";
    public const string LBRACK = "LBRACK";
    public const string RBRACK = "RBRACK";
    public const string COLON = "COLON";
    public const string WHITESPACE = "WHITESPACE";

    // Operators (ordered by precedence, highest to lowest)
    public const string DOTOP = "DOTOP";
    public const string FUNCCALL = "func-call";
    public const string ARRACCESS = "array-access";
    public const string INCOP = "INCOP";
    public const string DECOP = "DECOP";
    public const string POWOP = "POWOP";
    public const string PREINCOP = "PREINCOP";
    public const string PREDECOP = "PREDECOP";
    public const string BITNOTOP = "BITNOTOP";
    public const string BOOLNOTOP = "BOOLNOTOP";
    public const string MULOP = "MULOP";
    public const string NEGATEOP = "NEGATEOP";
    public const string ADDOP = "ADDOP";
    public const string SUBOP = "SUBOP";
    public const string SHIFTOP = "SHIFTOP";
    public const string UNRSHIFTOP = "UNRSHIFTOP";
    public const string BITOP = "BITOP";
    public const string EQNEQOP = "EQNEQOP";
    public const string GLOP = "GLOP";
    public const string BOOLOP = "BOOLOP";
    public const string ASSIGNOP = "ASSIGNOP";
    public const string COMMAOP = "COMMAOP";

    // Special
    public const string EOS = "$$";
    public const string NULL = "NULL";
    public const string EOF = "$";
    public const string NOARGS = "no-args";
}