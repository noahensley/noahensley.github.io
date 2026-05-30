/*
 * File: Terminals.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 14, 2026
 */

using System.Text.RegularExpressions;

/// <summary>
/// Defines terminal symbols and their regular expression patterns for lexical analysis.
/// </summary>
public static class Terminals
{
    /// <summary>
    /// Specification string defining terminal symbols and their regex patterns.
    /// Format: SYMBOL :: regex_pattern (one per line).
    /// </summary>
    public static string TerminalSpec = @"

        VAR :: var\b
        TYPE :: \b(int|float|string|bool)\b
        CLASS :: class\b
        NEWOP :: \bnew\b
        THIS :: this\b
        CASTOP :: as
        FUNCDEF :: func
        RETURN :: return
        WHILE :: while
        BREAK :: break
        CONTINUE :: continue
        REPEAT :: repeat
        UNTIL :: until
        FNUM :: ((\d*\.\d+)|(\d+([eE][-+]?\d+))|(\d*\.\d*\d+([eE][-+]?\d+)))\b
        NUM :: \d+\b
        IF :: if
        ELIF :: else if
        ELSE :: else
        LBRACE :: \{
        RBRACE :: \}
        LPAREN :: \(
        RPAREN :: \)
        LBRACK :: \[
        RBRACK :: \]
        WHITESPACE :: [\s\r\n]*(\/\/.*)[\s\r\n]*|[\s\r\n]+
        POWOP :: \*\*
        MULOP :: \*|\/|%
        INCOP :: \+{2}
        DECOP :: -{2}
        ADDOP :: \+
        SUBOP :: -
        UNRSHIFTOP :: \>{3}
        SHIFTOP :: \<{2}|\>{2}
        BITOP :: &|\||\^
        BITNOTOP :: ~
        BOOLOP :: and|or
        BOOLNOTOP :: not
        BOOLCONST :: true|false
        EQNEQOP :: ==|!=
        GLOP :: \<=|\<|\>=|\>
        ASSIGNOP :: \=
        DOTOP :: \.
        COLON :: \:
        COMMAOP :: \,
        EOF :: \\z
        STRINGCONST :: ""(\\[nt\\""]|[^""\\])*""
        ID :: \w+

    ";

    /// <summary>
    /// Static constructor that initializes the terminal list.
    /// </summary>
    static Terminals()
    {
        Init();
    }

    /// <summary>
    /// Represents a terminal symbol with its name and compiled regex pattern.
    /// </summary>
    public class Terminal
    {
        /// <summary>
        /// The symbol name of the terminal.
        /// </summary>
        public string Sym;
        
        /// <summary>
        /// The compiled regular expression for matching the terminal.
        /// </summary>
        public Regex Rex;
        
        /// <summary>
        /// Creates a new terminal with the specified symbol and regex.
        /// </summary>
        /// <param name="sym">The terminal symbol name.</param>
        /// <param name="rex">The compiled regex pattern.</param>
        public Terminal(string sym, Regex rex)
        {
            this.Sym = sym;
            this.Rex = rex;
        }
    }

    /// <summary>
    /// List of all terminal symbols parsed from the TerminalSpec.
    /// </summary>
    public static List<Terminal> Items = new();

    /// <summary>
    /// Parses the TerminalSpec string and populates the Items list with Terminal objects.
    /// </summary>
    public static void Init()
    {
        foreach(var line in TerminalSpec.Split('\n'))
        {
            var fLine = line.Trim();
            if(fLine.Length == 0)
                continue;
            var tmp = fLine.Split("::");
            string sym = tmp[0].Trim();
            string regex = tmp[1].Trim();
            Items.Add(new Terminal(sym, new Regex("\\G(" + regex + ")")));
        }
    }
}