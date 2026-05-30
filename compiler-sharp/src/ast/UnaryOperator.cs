/*
 * File: UnaryOperator.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 */
using ASM;
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

    /// <summary>
    /// Array of legal operand type combinations for this operator.
    /// </summary>
    /// <remarks>
    /// Each entry specifies a valid operand type and the resulting type when that
    /// operand type is used. This array is used during type inference to determine
    /// the expression's result type. An empty array indicates custom type inference
    /// logic is required in the subclass.
    /// </remarks>
    LegalOperand[] legal;

    /// <summary>
    /// Creates a new unary operator node.
    /// </summary>
    /// <param name="tok">The operator token.</param>
    /// <param name="term">The single operand expression.</param>
    /// <param name="legal">Array of legal operand type combinations for this operator.</param>
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

    /// <summary>
    /// Infers and assigns the type of this unary operation based on its operand.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method performs type inference in a post-order manner (children have already been typed).
    /// It searches the <see cref="legal"/> array for an entry matching the operand's type and assigns
    /// the corresponding result type to this expression node.
    /// </para>
    /// <para>
    /// If the <see cref="legal"/> array is empty, the method returns without performing type inference.
    /// This accommodates unary operators that have not yet been implemented or that require custom logic.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidUnaryOperatorType">
    /// Reported via <see cref="Utils.error"/> when no entry in <see cref="legal"/> matches the
    /// operand's type.
    /// </exception>
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
        Utils.error(new InvalidUnaryOperatorType($"Unsupported type for unary operator {this.token}; got {this.term.type}, expected {this.legal}"));
    }

    public override void genCode()
    {
        Asm.emit(new Comment($"*** generated unary {this.type} expr ***"));
        if(this.type == VarType.Int || this.type == VarType.BoolConst)
        {
            this.term.genCode();
            this.term.getResultLocation()!.copyToRegister(Register.rbx, null);
        } 
        else if (this.type == VarType.Float)
        {
            this.term.genCode();
            this.term.getResultLocation()!.copyToRegister(Register.xmm1, StorageClass.STATIC);
        }
    }
}
