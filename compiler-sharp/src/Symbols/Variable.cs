/*
 * File: Variable.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 23, 2026
 *      February 25, 2026
 *      March 28, 2026
 */


using ASM;

/// <summary>
/// Represents a variable reference as a terminal node in the abstract syntax tree.
/// </summary>
/// <remarks>
/// A Variable node is created when an identifier is used in an expression context.
/// Its type is resolved by looking up the variable's name in the symbol table to
/// retrieve the associated <see cref="VarInfo"/>, which carries type and location information.
/// </remarks>
class Variable : Term, ResultLocation, LValue
{
    /// <summary>
    /// The symbol table entry for this variable, populated during semantic analysis.
    /// </summary>
    /// <remarks>
    /// Null until <see cref="assignVarInfo"/> is called. Once set, this field provides the type
    /// and storage location of the variable as declared in the enclosing scope.
    /// </remarks>
    public VarInfo? info;

    /// <summary>
    /// Initializes a Variable node with the identifier token from the source.
    /// </summary>
    /// <param name="tok">The identifier token representing this variable reference.</param>
    /// <remarks>
    /// The symbol table lookup is performed with the non-erroring <see cref="SymbolTable.lookupIfExists"/>
    /// overload. A null result is intentional and indicates the variable may be a forward reference to a
    /// global declared later in the file; <see cref="assignVarInfo"/> must be called during the hoisting
    /// phase to resolve it before use.
    /// </remarks>
    public Variable(Token tok) : base(tok)
    {
        string varname = tok.lexeme;
        if (tok.sym == TokenSymbols.ID)
        {
            VarInfo? vi = SymbolTable.lookupIfExists(varname);
            info = vi;
        }
        else if (tok.sym == TokenSymbols.THIS)
        {
            if (ClassdefNode.curClassType == null)
                throw new Exception("Internal compiler error: tried to add VarInfo for 'this' variable but got null class type.");
            info = new VarInfo(tok, ClassdefNode.curClassType, null);
        }
        else
            throw new Exception("Internal compiler error: cannot construct variable that is not ID or THIS token.");
    }

    /// <summary>
    /// Sets the type of this node from its resolved <see cref="VarInfo"/>.
    /// </summary>
    /// <remarks>
    /// Has no effect if <see cref="info"/> has not yet been assigned via <see cref="assignVarInfo"/>.
    /// Once info is available, this node's type is set to match the declared type of the variable,
    /// writing through to the inherited <c>type</c> field defined on <see cref="ExprNode"/>.
    /// </remarks>
    public override void setType()
    {
        if (info != null)
            type = info.type; // setting inherited ExprNode's type attribute using Variable VarInfo
    }

    /// <summary>
    /// Emits code to load this variable's integer value from memory into the specified register.
    /// </summary>
    /// <param name="reg">The destination integer register.</param>
    /// <param name="klass">The storage class of the value. Currently unused.</param>
    public void copyToRegister(IntRegister reg, IntRegister? klass)
    {
        this.info!.location!.copyAddressToRegister(reg);
        
        // read storage class BEFORE stomping reg with value
        if (klass != null)
            Asm.emit(new OpMovRegIndReg(offset: 8, src: reg, dst: klass));

        Asm.emit(
            new Comment("moving expr LHS result (stomps LHS)"),
            new OpMovRegIndReg(offset: 0, src: reg, dst: reg)
        );
    }

    /// <summary>
    /// Not supported on variables. Throws an internal compiler error if called.
    /// </summary>
    /// <remarks>
    /// Only expression nodes that are parents of variables should store results via
    /// this interface. Calling it on the variable itself indicates a compiler bug.
    /// </remarks>
    public void copyFromRegister(IntRegister reg, StorageClass klass)
    {
        //we should never try to do this!
        // this is because only expression nodes (parents of variables/terms)
        // should use this function--never the variable/term itself
        throw new Exception("Internal compiler error: tried to move expression variable/term address instead of an expression parent.");
    }
    public void copyFromRegister(IntRegister reg, IntRegister? klass)
    {
        //we should never try to do this (bug!)
        throw new Exception("Internal compiler error: tried to copy a temporary value out of a Variable.");
    }

    /// <summary>
    /// Not supported on variables for float registers. Bonus functionality.
    /// </summary>
    public void copyToRegister(FloatRegister reg, StorageClass klass)
    {
        // bonus
        throw new NotImplementedException();
    }

    /// <summary>
    /// Not supported on variables for float registers. Bonus functionality.
    /// </summary>
    public void copyFromRegister(FloatRegister reg, StorageClass klass)
    {
        // bonus
        throw new NotImplementedException();
    }

    public override void genCode()
    {
        return; //nothing to do for l-values
    }

    public void copyAddressToRegister(IntRegister reg)
    {
        this.info!.location!.copyAddressToRegister(reg);
    }

    public override ResultLocation getResultLocation()
    {
        return this;
    }

    /// <summary>
    /// Resolves and assigns the symbol table entry for this variable.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Attempts to resolve the variable identifier by searching the symbol table
    /// using the non-erroring <see cref="SymbolTable.lookupIfExists"/> method.
    /// If the variable exists in the global scope, the returned
    /// <see cref="VarInfo"/> is stored in <see cref="info"/>.
    /// </para>
    /// <para>
    /// If the lookup fails, the method checks whether the variable corresponds
    /// to a class reference (such as the <c>this</c> keyword), which will already
    /// have its type assigned during construction.
    /// </para>
    /// <para>
    /// If the variable is still unresolved, the method searches the enclosing
    /// function definition for a parameter whose name matches the identifier
    /// using <see cref="getEnclosingParamInfo"/>. When found, a new
    /// <see cref="VarInfo"/> instance is created using a
    /// <see cref="ParameterLocation"/>.
    /// </para>
    /// <para>
    /// This method has no effect if <see cref="info"/> has already been assigned.
    /// </para>
    /// </remarks>
    public void assignVarInfo()
    {
        if (this.info == null)
        {
            string varname = this.token.lexeme;
            this.info = SymbolTable.lookupIfExists(varname);
            if (this.info != null)
                return;

            if ((this.type as ClassType) != null)
                return; // 'this' keyword, already assigned

            // member function parameter, NEEDS type set...
            ParamInfo? pi = getEnclosingParamInfo(this.token.lexeme);
            if (pi == null)
                throw new Exception("Internal compiler error: could not find parameter info in enclosing function.");
            if (pi.type == null)
                throw new Exception("Internal compiler error: got ParamInfo for member function parameter with missing type.");
            this.info = new VarInfo(this.token, pi.type, new ParameterLocation());
        }
    }

    /// <summary>
    /// Searches enclosing function definitions to locate a parameter with the given name.
    /// </summary>
    /// <param name="pname">The parameter name to search for.</param>
    /// <returns>
    /// The <see cref="ParamInfo"/> describing the parameter if found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Traverses parent AST nodes until an enclosing <see cref="FuncdefNode"/> is found,
    /// then inspects that function's parameter list for a parameter whose identifier
    /// matches <paramref name="pname"/>. This is used when resolving member function
    /// parameters that are not present in the symbol table.
    /// </remarks>
    public ParamInfo? getEnclosingParamInfo(string pname)
    {
        TreeNode? cur = this.parent;
        while (cur != null)
        {
            FuncdefNode? enclosing = cur as FuncdefNode;
            if (enclosing != null)
            {
                 // found enclosing function
                if (enclosing.info == null)
                    throw new Exception("Internal compiler error: function info was null during variable information assignment -- function parsing failed to assign non-null info to function.");
                VarInfo vi = enclosing.info;
                FuncType? ftype = vi.type as FuncType;
                if (ftype == null)
                    throw new Exception("Internal compiler error: got FuncdefNode with no FuncType during member parameter assignment.");

                foreach (ParamInfo pi in ftype.parameters)
                {
                    if (pi.token == null)
                        throw new Exception("Internal compiler error: got uninitailized ParamInfo token during member parameter assignment.");

                    if (pi.token.lexeme == pname)
                        return pi;
                }
            }
            cur = cur.parent;
        }

        return null;
    }

    /// <summary>
    /// Formats a human-readable description of this variable's declaration for diagnostic output.
    /// </summary>
    /// <returns>
    /// A string of the form <c>"&lt;name&gt; on line &lt;N&gt; is a &lt;location&gt; declared on line &lt;M&gt;"</c>,
    /// or <c>"ERROR"</c> if no <see cref="VarInfo"/> has been resolved yet.
    /// </returns>
    /// <remarks>
    /// Location categories reported are <c>global</c>, <c>parameter</c>, and <c>local</c>.
    /// <see cref="MemberLocation"/> is not yet handled and will produce <c>"ERROR"</c> as the location string.
    /// Used by the <c>-var-decl</c> output pass in <see cref="Utils"/>.
    /// </remarks>
    public string getVarInfo(bool verbose=true)
    {
        string loc = "ERROR";

        if (info == null)
            throw new Exception($"Internal compiler error: tried to get info for variable {this.token.lexeme} but it was never assigned.");

        switch (info.location)
        {
            case GlobalLocation:
                loc = "global";
                break;
            case ParameterLocation:
                loc = "parameter";
                break;
            case LocalLocation:
                loc = "local";
                break;
            case MemberLocation:
                loc = "member";
                break;
            // 'this' variables will have null info.location
        }

        string dtype = "ERROR";
        switch (info.type)
        {
            case IntType:
                dtype = "int";
                break;
            case FloatType:
                dtype = "float";
                break;
            case StringConstType:
                dtype = "string";
                break;
            case BoolConstType:
                dtype = "bool";
                break;
            case FuncType:
                dtype = "function";
                break;
            case ClassType:
                ClassType cltype = (info.type as ClassType)!;
                if (cltype.declarer == null)
                    throw new Exception("Internal compiler error: tried to get member info for class type with no declarer.");
                if (cltype.declarer.memberVariables.ContainsKey(this.token.lexeme))
                {
                    ClassType? clMemberType = cltype.declarer.memberVariables[token.lexeme] as ClassType;
                    if (clMemberType == null)
                        throw new Exception($"Internal compiler error: mismatch between declared variable type and actual: got {cltype}; expected ClassType");
                    dtype = clMemberType.name.lexeme;
                }
                else
                    dtype = cltype.name.lexeme;
                break;
            default:
                Utils.error(new InvalidMember($"Expected member of type int,float,string,bool,function; got {info.type}"));
                break;
        }

        if (info.token == null)
            throw new Exception("Internal compiler error: variable VarInfo token was never assigned.");

        int decl_line = info.token.line;
        //string vtype = (info.type as FuncType) == null ? "Variable" : "Function";
        //string line = $"{vtype} {token.lexeme} on line {token.line} ";
        string line = $"Variable {token.lexeme} on line {token.line} ";

        if (verbose)
            line = line + $"is a {loc} declared on line {decl_line} and ";

        line = line + $"is of type {dtype}";
        return line;
    }
}