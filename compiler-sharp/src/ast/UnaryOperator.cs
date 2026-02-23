/*
 * File: UnaryOperator.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 */

/// <summary>
/// Abstract base class for unary operator nodes in the expression tree.
/// </summary>
/// <remarks>
/// Unary operators have one child expression (a single term operand) and follow
/// standard prefix notation. Specific operator types extend this class to represent
/// different operation categories.
/// </remarks>
public abstract class UnaryOperator : ExprNode
{
    /// <summary>
    /// The single operand expression subtree.
    /// </summary>
    public ExprNode term;

    LegalOperand[] legal;

    /// <summary>
    /// Creates a new unary operator node.
    /// </summary>
    /// <param name="tok">The operator token.</param>
    /// <param name="term">The single operand expression.</param>
    protected UnaryOperator(Token tok, ExprNode term, LegalOperand[] legal) : base(tok)
    {
        this.term = term;
        this.legal = legal;
    }

    /// <summary>
    /// Gets the child nodes of this unary operator.
    /// </summary>
    /// <returns>A list containing the single operand expression.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>() {term};
    }

    public override void setType()
    {
        // this is for unimplemented unary operators
        if (this.legal.Length == 0)
            return;
        foreach (var le in legal)
        {
            if (le.operandType == term.type)
            {
                this.type = le.resultType;
                return;
            }
        }
        Utils.error(new InvalidUnaryOperatorType($"Unsupported type for binary operator {this.token}"));
    }
}