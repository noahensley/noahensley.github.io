/*
 * File: BinaryOperator.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 4, 2026
 *      February 16, 2026
 */

/// <summary>
/// Abstract base class for binary operator nodes in the expression tree.
/// </summary>
/// <remarks>
/// Binary operators have two child expressions (left and right operands) and follow
/// standard infix notation. Specific operator types extend this class to represent
/// different operation categories.
/// </remarks>
public abstract class BinaryOperator : ExprNode
{
    /// <summary>
    /// The left operand expression subtree.
    /// </summary>
    public ExprNode left;

    /// <summary>
    /// The right operand expression subtree.
    /// </summary>
    public ExprNode right;

    /// <summary>
    /// Array of legal operand type combinations for this operator.
    /// </summary>
    /// <remarks>
    /// Each entry specifies a valid combination of operand types and the resulting
    /// type when that combination is used. This array is used during type checking
    /// to validate operand compatibility and determine the expression's result type.
    /// </remarks>
    LegalOperand[] legal;

    /// <summary>
    /// Creates a new binary operator node.
    /// </summary>
    /// <param name="tok">The operator token.</param>
    /// <param name="left">The left operand expression.</param>
    /// <param name="right">The right operand expression.</param>
    /// <param name="legal">Array of legal operand type combinations for this operator.</param>
    protected BinaryOperator(Token tok, ExprNode left, ExprNode right, LegalOperand[] legal) : base(tok)
    {
        this.left = left;
        this.right = right;
        this.legal = legal;
    }

    /// <summary>
    /// Gets the child nodes of this binary operator.
    /// </summary>
    /// <returns>A list containing the left and right operand expressions.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>() { left, right };
    }

    /// <summary>
    /// Infers and assigns the type of this binary operation based on its operands.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method performs type inference in a postorder manner (children have already been typed).
    /// It validates that both operands have compatible types and determines the result type by
    /// matching the operand types against the <see cref="legal"/> array.
    /// </para>
    /// <para>
    /// The method first checks for type equality between left and right operands, then searches
    /// the legal operand combinations for a matching entry. If a match is found, the corresponding
    /// result type is assigned to this expression node.
    /// </para>
    /// <para>
    /// Special case: If the <see cref="legal"/> array is empty (length 0), the method returns
    /// without performing type checking. This accommodates operators like ArrayNode, FunccallNode,
    /// and DotNode which have custom type inference logic.
    /// </para>
    /// </remarks>
    /// <exception cref="System.Exception">
    /// Throws an error via <see cref="Utils.error"/> if:
    /// <list type="bullet">
    /// <item>The left and right operand types do not match</item>
    /// <item>No legal operand combination matches the actual operand type</item>
    /// </list>
    /// </exception>
    public override void setType()
    {
        // this is for ArrayNode, FunccallNode, DotNode (not implemented; don't error)
        if (this.legal.Length == 0)
            return;
        // children already have types (postorder)
        if (this.left.type != this.right.type)
            Utils.error(new InvalidBinaryOperatorType(
                $"Type mismatch for {this.token.lexeme} ({this.token.line},{this.token.column}); {this.left.type} must equal {this.right.type}"
            ));
        foreach (var le in legal)
        {
            if (le.operandType == left.type)
            {
                this.type = le.resultType;
                return;
            }
        }
        Utils.error(new InvalidBinaryOperatorType($"Unsupported type for binary operator {this.token}"));
    }
}