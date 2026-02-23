/*
 * File: Term.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 */

/// <summary>
/// Represents a terminal expression node (numeric literal or identifier).
/// </summary>
/// <remarks>
/// Terminal nodes are leaf nodes in the expression tree with no child expressions.
/// They represent the basic building blocks of expressions: numbers (NUM tokens),
/// variable/function references (ID tokens), decimal numbers (FNUM tokens), string 
/// constants (STRINGCONST tokens), and boolean cosntants (BOOLCONST tokens)
/// </remarks>
public class Term : ExprNode
{
    /// <summary>
    /// Creates a new terminal expression node.
    /// </summary>
    /// <param name="term">The token representing this terminal [NUM, ID, FNUM, STRINGCONST, or BOOLCONST].</param>
    public Term(Token term) : base(term)
    {
    }

    /// <summary>
    /// Gets the child nodes of this terminal.
    /// </summary>
    /// <returns>An empty list, as terminals have no children.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>();
    }

    public override void setType()
    {
        switch (this.token.sym)
        {
            case TokenSymbols.NUM:
                this.type = VarType.Int;
                return;
            case TokenSymbols.FNUM:
                this.type = VarType.Float;
                return;
            case TokenSymbols.STRINGCONST:
                this.type = VarType.StringConst;
                return;
            case TokenSymbols.BOOLCONST:
                this.type = VarType.BoolConst;
                return;
            default:
                Utils.error(new InvalidTermType($"Term with unsupported type on line {this.token.line}, got: {this.token.sym}"));
                return;
        }
    }

    public override void typeCheck()
    {
        return; // not implemented
    }
}