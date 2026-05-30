/*
 * File: FunccallNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 11, 2026
 */

/// <summary>
/// Represents a function call expression in the expression tree.
/// </summary>
/// <remarks>
/// The left operand is the function expression being invoked (typically a <see cref="Variable"/>
/// node). The right operand represents the argument list, which is either a <see cref="NoArgs"/>
/// sentinel for zero-argument calls or a comma-separated expression tree for one or more arguments.
/// </remarks>
using System.Diagnostics;
using ASM;
public class FunccallNode : BinaryOperator
{
    /// <summary>
    /// Creates a new function call node.
    /// </summary>
    /// <param name="token">The synthetic func-call operator token injected by the parser.</param>
    /// <param name="func">The function expression being invoked.</param>
    /// <param name="right">The argument list expression or a <see cref="NoArgs"/> sentinel.</param>
    public FunccallNode(Token token, ExprNode func, ExprNode right): base(token, func, right, [])
    {
        if (func.token.sym == TokenSymbols.ID)
            return; // global function; already handled

        // if not, must be DotNode; e.g. bar.foo()
        DotNode? dotnode = func as DotNode;
        if (dotnode == null)
            throw new Exception($"Internal compiler error: expected func on line {token.line} LHS to be ID or DotNode; got {func}");
    }

    /// <summary>
    /// Resolves and assigns the type of this function call expression.
    /// </summary>
    /// <remarks>
    /// The type of a function call is the <see cref="FuncType"/> of the left operand.
    /// The left operand must resolve to a <see cref="FuncType"/>; if it does not, the
    /// variable is not callable and an error is reported.
    /// </remarks>
    /// <exception cref="InvalidExpression">
    /// Reported via <see cref="Utils.error"/> when the left operand does not resolve to a
    /// <see cref="FuncType"/>.
    /// </exception>
    public override void setType()
    {
        FuncType? funcType = left.type as FuncType;

        if (funcType == null)
        {
            Utils.error(new InvalidExpression(
                $"Variable {this.token.lexeme} on line {this.token.line} is not callable."
            ));
            throw new UnreachableException();
        }

        this.type = funcType.returnType;
    }

    /// <summary>
    /// Validates that the supplied arguments match the function's declared parameter signature.
    /// </summary>
    /// <remarks>
    /// Collects argument types from the right operand and verifies:
    /// <list type="bullet">
    /// <item>The number of supplied arguments matches the declared parameter count.</item>
    /// <item>Each argument type matches the corresponding declared parameter type.</item>
    /// </list>
    /// Zero-argument calls (right operand is <see cref="NoArgs"/>) pass validation immediately.
    /// </remarks>
    /// <exception cref="ParameterMismatch">
    /// Reported via <see cref="Utils.error"/> when the argument count differs from the parameter
    /// count, or when an argument type does not match the declared parameter type.
    /// </exception>
    public override void typeCheck()
    {
        if ((left.type as FuncType) == null)
            throw new Exception($"Internal compiler error: expected function call on line {token.line} to be on a DotNode or a function; got {left.type}");
            
        List<VarType> paramTypes = new List<VarType>();

        if (this.right is NoArgs)
        {
            if ((this.left as DotNode) != null)
                return;
            VarInfo fi = SymbolTable.lookup(this.left.token.lexeme, this.left.token.line);
            FuncType ftype = (fi.type as FuncType)!;
            if (ftype.parameters.Count() > 0)
                Utils.error(new InvalidExpression($"Unexpected function call {this.left.token.lexeme} with 0 arguments; expected {ftype.parameters.Count()}"));
            return; // zero arguments
        }
        else if (this.right.token.sym != TokenSymbols.COMMAOP)
        {
            if (this.right.type == null)
                throw new Exception(
                    $"Internal compiler error: unexpected null symbol in type checking phase " + 
                    $"while processing function '{this.token.lexeme}' at line {this.token.line}"
                );

            paramTypes.Add(this.right.type);
        }
        else
        {
            gatherParams(paramTypes, this.right);
        }

        FuncType? funcType = this.left.type as FuncType;
        if (funcType == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type checking phase " + 
                $"while processing function '{this.token.lexeme}' at line {this.token.line}"
            );

        if (funcType.parameters.Count != paramTypes.Count)
        {
            Utils.error(new ParameterMismatch(
                $"Function {this.token.lexeme} expected {funcType.parameters.Count}; " +
                $"got {paramTypes.Count} ({this.token.line},{this.token.column})"
            ));
        }

        for (int i = 0; i < funcType.parameters.Count; i++)
        {
            ParamInfo expected = funcType.parameters[i];
            VarType got = paramTypes[i];

            if (expected.type != got)
            {
                if (expected.token == null)
                {
                    Utils.error(new ParameterMismatch(
                        $"Function {this.token.lexeme} parameter {i + 1} type mismatch."
                    ));
                }
                else
                {
                    Utils.error(new ParameterMismatch(
                        $"Function {this.token.lexeme} parameter {i + 1} type mismatch {expected.token.line}"
                    ));
                }
            }
        }
    }

    /// <summary>
    /// Extracts argument types from a comma-separated expression tree into a flat list.
    /// </summary>
    /// <param name="ptypes">List populated with argument types in left-to-right source order.</param>
    /// <param name="e">The root of a comma-separated argument expression subtree.</param>
    /// <remarks>
    /// Argument lists containing two or more expressions are represented as a left-associative
    /// <see cref="CommaNode"/> tree. This method recursively traverses that structure and records
    /// argument types in left-to-right order.
    /// </remarks>
    private void gatherParams(List<VarType> ptypes, ExprNode e)
    {
        CommaNode? cnode = e as CommaNode;
        if (cnode == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type checking phase " + 
                $"while processing function '{this.token.lexeme}' at line {this.token.line}"
            );

        if (cnode.right.type == null)
            throw new Exception(
                $"Internal compiler error: unexpected null symbol in type checking phase " + 
                $"while processing function '{this.token.lexeme}' at line {this.token.line}"
            );

        if (cnode.left is CommaNode)
        {
            gatherParams(ptypes, cnode.left);
        }
        else
        {
            if (cnode.left.type == null)
                throw new Exception(
                    $"Internal compiler error: unexpected null symbol in type checking phase " + 
                    $"while processing function '{this.token.lexeme}' at line {this.token.line}"
                );

            ptypes.Add(cnode.left.type);
        }

        ptypes.Add(cnode.right.type);
    }

    public override void genCode()
    {
        //do not call base
        if ((this.right as NoArgs) != null)
        {
            //nothing to do
        }
        else
        {
            ExprNode n = this.right;
            while ((n as CommaNode) != null)
            {
                //...push right child's value to stack
                CommaNode c = (n as CommaNode)!;
                c.right.genCode();
                c.right.getResultLocation()!.copyToRegister(Register.rax, Register.rbx);
                Asm.emit(
                    new OpPushReg(Register.rbx),
                    new OpPushReg(Register.rax)
                );
                n = c.left;
            }
            //...push n's value to stack...
            n.genCode();
            n.getResultLocation()!.copyToRegister(Register.rax, Register.rbx);
            Asm.emit(
                new OpPushReg(Register.rbx),
                new OpPushReg(Register.rax)
            );
        }
        
        Variable? fname = this.left as Variable;
        
        if (fname == null)
            //never null now; might be null with member functions (DotNode not Variables)
            throw new Exception("Internal compiler error: got function invocation with unsupported (non-variable) LHS.");
           
        FuncType? ftype = fname.info!.type as FuncType;
        if (ftype == null)
            throw new Exception("Internal compiler error: got function invocation with a non-function type LHS.");       

        if (ftype.builtin)
        {
            Asm.emit(
                new OpMovRegReg(src: Register.rsp, dst: Register.rcx),
                new OpSubRegConst(reg: Register.rsp, 32) // shadow space
            );
        }

        if (ftype.declarer == null)
            throw new Exception("Internal compiler error: got FuncType with no declaring FuncdefNode.");
        
        Asm.emit(new OpCall(ftype.declarer.lbl));
        
        if (ftype.returnType == VarType.Void)
        {
            //nothing to do
        }
        else if (ftype.returnType == VarType.Int || ftype.returnType == VarType.BoolConst || 
            ftype.returnType == VarType.StringConst)
        {
            if (ftype.builtin)
            {
                this.getResultLocation()!.copyFromRegister(Register.rax, StorageClass.STATIC);
            }
            else
            {
                this.getResultLocation()!.copyFromRegister(Register.rbx, Register.rax);
            }
        }
        else
        {
            throw new NotImplementedException();
            //bonus from before; not implemented
            //use Float copyFromRegister version
        }

        if (ftype.builtin)
        {
            Asm.emit(
                new OpAddRegConst(Register.rsp, 32)
                );
        }
        
        if (ftype.parameters.Count > 0)
        {
            Asm.emit(
                new OpAddRegConst(Register.rsp, 16*ftype.parameters.Count())
            );
        }
    }
}
