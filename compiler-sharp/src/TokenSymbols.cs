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

    /// <summary>The <c>var</c> keyword, used to introduce a variable declaration.</summary>
    public const string VAR = "VAR";

    /// <summary>A primitive type keyword: <c>int</c>, <c>float</c>, <c>string</c>, or <c>bool</c>.</summary>
    public const string TYPE = "TYPE";

    public const string CLASS = "CLASS";
    public const string NEWOP = "NEWOP";
    public const string THIS = "THIS";

    /// <summary>The <c>as</c> cast operator keyword.</summary>
    public const string CASTOP = "CASTOP";

    /// <summary>The <c>func</c> keyword, used to introduce a function definition.</summary>
    public const string FUNCDEF = "FUNCDEF";

    /// <summary>The <c>return</c> keyword.</summary>
    public const string RETURN = "RETURN";

    /// <summary>The <c>while</c> keyword.</summary>
    public const string WHILE = "WHILE";

    public const string BREAK = "BREAK";

    public const string CONTINUE = "CONTINUE";

    public const string REPEAT = "REPEAT";

    public const string UNTIL = "UNTIL";

    /// <summary>The <c>if</c> keyword.</summary>
    public const string IF = "IF";

    /// <summary>The <c>else if</c> keyword sequence.</summary>
    public const string ELIF = "ELIF";

    /// <summary>The <c>else</c> keyword.</summary>
    public const string ELSE = "ELSE";

    // Literals

    /// <summary>A floating-point numeric literal (e.g., <c>3.14</c> or <c>1e10</c>).</summary>
    public const string FNUM = "FNUM";

    /// <summary>An integer numeric literal (e.g., <c>42</c>).</summary>
    public const string NUM = "NUM";

    /// <summary>A boolean literal: <c>true</c> or <c>false</c>.</summary>
    public const string BOOLCONST = "BOOLCONST";

    /// <summary>A quoted string literal (e.g., <c>"hello"</c>).</summary>
    public const string STRINGCONST = "STRINGCONST";

    /// <summary>An identifier token (variable or function name).</summary>
    public const string ID = "ID";

    // Delimiters

    /// <summary>Opening curly brace <c>{</c>.</summary>
    public const string LBRACE = "LBRACE";

    /// <summary>Closing curly brace <c>}</c>.</summary>
    public const string RBRACE = "RBRACE";

    /// <summary>Opening parenthesis <c>(</c>.</summary>
    public const string LPAREN = "LPAREN";

    /// <summary>Closing parenthesis <c>)</c>.</summary>
    public const string RPAREN = "RPAREN";

    /// <summary>Opening square bracket <c>[</c>.</summary>
    public const string LBRACK = "LBRACK";

    /// <summary>Closing square bracket <c>]</c>.</summary>
    public const string RBRACK = "RBRACK";

    /// <summary>Colon <c>:</c>, used in type annotations and function signatures.</summary>
    public const string COLON = "COLON";

    /// <summary>Whitespace or comment token, consumed internally by the tokenizer.</summary>
    public const string WHITESPACE = "WHITESPACE";

    // Operators (ordered by precedence, highest to lowest)

    /// <summary>The dot member-access operator <c>.</c> (precedence 500).</summary>
    public const string DOTOP = "DOTOP";

    /// <summary>Synthetic function-call operator injected between an identifier and <c>(</c> (precedence 500).</summary>
    public const string FUNCCALL = "func-call";

    /// <summary>Synthetic array-access operator injected between an expression and <c>[</c> (precedence 500).</summary>
    public const string ARRACCESS = "array-access";

    /// <summary>Postfix increment operator <c>++</c> (precedence 500).</summary>
    public const string INCOP = "INCOP";

    /// <summary>Postfix decrement operator <c>--</c> (precedence 500).</summary>
    public const string DECOP = "DECOP";

    /// <summary>Exponentiation operator <c>**</c> (precedence 220).</summary>
    public const string POWOP = "POWOP";

    /// <summary>Prefix increment operator <c>++</c>, re-tagged from INCOP when in prefix position (precedence 210).</summary>
    public const string PREINCOP = "PREINCOP";

    /// <summary>Prefix decrement operator <c>--</c>, re-tagged from DECOP when in prefix position (precedence 210).</summary>
    public const string PREDECOP = "PREDECOP";

    /// <summary>Bitwise NOT operator <c>~</c> (precedence 210).</summary>
    public const string BITNOTOP = "BITNOTOP";

    /// <summary>Boolean NOT operator <c>not</c> (precedence 210).</summary>
    public const string BOOLNOTOP = "BOOLNOTOP";

    /// <summary>Multiplication, division, or modulo operator <c>*</c>, <c>/</c>, or <c>%</c> (precedence 200).</summary>
    public const string MULOP = "MULOP";

    /// <summary>Unary negation operator <c>-</c>, re-tagged from SUBOP when in prefix position (precedence 190).</summary>
    public const string NEGATEOP = "NEGATEOP";

    /// <summary>Addition operator <c>+</c> (precedence 180).</summary>
    public const string ADDOP = "ADDOP";

    /// <summary>Subtraction operator <c>-</c> (precedence 180).</summary>
    public const string SUBOP = "SUBOP";

    /// <summary>Signed left or right shift operator <c>&lt;&lt;</c> or <c>&gt;&gt;</c> (precedence 160).</summary>
    public const string SHIFTOP = "SHIFTOP";

    /// <summary>Unsigned right shift operator <c>&gt;&gt;&gt;</c> (precedence 160).</summary>
    public const string UNRSHIFTOP = "UNRSHIFTOP";

    /// <summary>Bitwise AND, OR, or XOR operator <c>&amp;</c>, <c>|</c>, or <c>^</c> (precedence 140).</summary>
    public const string BITOP = "BITOP";

    /// <summary>Equality or inequality comparison operator <c>==</c> or <c>!=</c> (precedence 120).</summary>
    public const string EQNEQOP = "EQNEQOP";

    /// <summary>Relational comparison operator <c>&lt;</c>, <c>&gt;</c>, <c>&lt;=</c>, or <c>&gt;=</c> (precedence 120).</summary>
    public const string GLOP = "GLOP";

    /// <summary>Boolean logical operator <c>and</c> or <c>or</c> (precedence 100).</summary>
    public const string BOOLOP = "BOOLOP";

    /// <summary>Assignment operator <c>=</c> and compound variants (precedence 80).</summary>
    public const string ASSIGNOP = "ASSIGNOP";

    /// <summary>Comma operator <c>,</c>, used for argument lists and expression sequences (precedence 10).</summary>
    public const string COMMAOP = "COMMAOP";

    // Special

    /// <summary>End-of-statement sentinel <c>$$</c>, generated by the tokenizer on statement-terminating newlines.</summary>
    public const string EOS = "$$";

    /// <summary>Null sentinel used for uninitialized or default tokens.</summary>
    public const string NULL = "NULL";

    /// <summary>End-of-file sentinel <c>$</c>, generated when all input has been consumed.</summary>
    public const string EOF = "$";

    /// <summary>Sentinel symbol for a zero-argument function call, used as the right operand of <see cref="FUNCCALL"/>.</summary>
    public const string NOARGS = "no-args";
}
