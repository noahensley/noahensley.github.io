/*
 * File: FuncdefNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026,
 *      January 28, 2026,
 *      February 25, 2026
 */

/// <summary>
/// Represents a function definition in the abstract syntax tree.
/// </summary>
/// <remarks>
/// <para>
/// Function definitions declare callable code blocks with a name, optional return type,
/// parameters, and a body containing statements.
/// </para>
/// <para>
/// Current syntax:
/// </para>
/// <code>
/// func name(param: Type, ...) : returnType { statements }
/// func name(param: Type, ...) { statements }
/// </code>
/// <para>
/// The function's full type information (return type and parameter list) is stored in
/// a <see cref="FuncType"/> object inside the <see cref="VarInfo"/> referenced by
/// <see cref="info"/>. The AST node itself does not store parameters separately;
/// they are captured as part of the function's type.
/// </para>
/// <para>
/// When no return type is specified (no colon after the parameter list), the function
/// defaults to <see cref="VoidType"/>.
/// </para>
/// </remarks>
using ASM;
public class FuncdefNode : TreeNode
{
    public int maxTemporaries = 0;
    /// <summary>
    /// Metadata describing the function: name token, <see cref="FuncType"/>, and storage location.
    /// </summary>
    /// <remarks>
    /// Contains the function identifier token and its associated <see cref="FuncType"/>,
    /// which includes both the return type and ordered parameter list. The location
    /// associated with this <see cref="VarInfo"/> represents where the function itself
    /// is stored (currently always <see cref="GlobalLocation"/>).
    /// </remarks>
    public VarInfo? info;

    public List<VardeclNode> locals = new List<VardeclNode>();

    public static FuncdefNode? currentFunction = null;

    public int numLocals = 0;
    
    /// <summary>
    /// The statement block comprising the function body.
    /// </summary>
    /// <remarks>
    /// Contains all executable statements within the function. This is where the function's
    /// logic resides, including variable declarations, control flow, and return statements.
    /// </remarks>
    public StmtsNode? stmts;

    public Label lbl = new Label();

    public FuncdefNode(Token token, FuncType ftype, StmtsNode stmts, Label label)
    {
        this.info = new VarInfo(token, ftype, new GlobalLocation());
        this.stmts = stmts;
        this.lbl = label;
    }
    
    /// <summary>
    /// Creates a function definition node from its pre-built components.
    /// </summary>
    /// <param name="info">The <see cref="VarInfo"/> record describing the function's name, type, and location.</param>
    /// <param name="stmts">The function body statement block.</param>
    private FuncdefNode(VarInfo info, StmtsNode stmts)
    {
        this.info = info;
        this.stmts = stmts;
    }

    private FuncdefNode(Token token)
    {
        this.info = null;
        this.stmts = null;
    }

    /// <summary>
    /// Determines whether the tokenizer is positioned at the start of a function definition.
    /// </summary>
    /// <param name="T">The tokenizer providing the token stream.</param>
    /// <returns>
    /// <c>true</c> if the next tokens represent a function definition; otherwise <c>false</c>.
    /// </returns>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.FUNCDEF;
    }

    /// <summary>
    /// Parses a function definition from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A <see cref="FuncdefNode"/> representing the parsed function definition.</returns>
    /// <remarks>
    /// <para>
    /// Expected syntax:
    /// </para>
    /// <code>
    /// func identifier(param: Type, ...) : type { statements }
    /// func identifier(param: Type, ...) { statements }
    /// </code>
    /// <para>
    /// Parsing sequence:
    /// </para>
    /// <list type="number">
    /// <item>Consumes the FUNCDEF keyword.</item>
    /// <item>Reads the function name identifier.</item>
    /// <item>Pushes a new scope dedicated to parameter declarations.</item>
    /// <item>
    /// Parses the parameter list between LPAREN and RPAREN:
    ///   <list type="bullet">
    ///   <item>Each parameter has the form <c>identifier : TYPE</c>.</item>
    ///   <item>Each parameter is declared in the current (function) scope using
    ///   <see cref="ParameterLocation"/>.</item>
    ///   <item>Parameter metadata is collected into a list for construction of the
    ///   function's <see cref="FuncType"/>.</item>
    ///   </list>
    /// </item>
    /// <item>
    /// Parses the optional return type:
    ///   <list type="bullet">
    ///   <item>If COLON is present after the parameter list, the following TYPE token
    ///   is converted to a <see cref="VarType"/>.</item>
    ///   <item>If no COLON is present, the return type defaults to <see cref="VoidType"/>.</item>
    ///   </list>
    /// </item>
    /// <item>Parses the function body as a statement block.</item>
    /// <item>
    /// Constructs a <see cref="FuncType"/> from the collected return type and parameter list,
    /// then creates a <see cref="VarInfo"/> describing the function.
    /// </item>
    /// <item>Pops the parameter scope.</item>
    /// <item>Declares the function in the enclosing (global) scope.</item>
    /// </list>
    /// <para>
    /// <b>Scope management:</b> Parameters are declared in a dedicated inner scope that
    /// exists only during parsing of the function definition. After the body is parsed,
    /// this scope is removed and the function itself is declared in the enclosing scope.
    /// This ensures parameters are not visible outside the function while the function
    /// symbol remains globally accessible.
    /// </para>
    /// </remarks>
    /// <exception cref="UnexpectedToken">
    /// Reported via <see cref="Utils.error"/> when expected tokens (FUNCDEF, ID, LPAREN,
    /// RPAREN, LBRACE, RBRACE) are not found in the correct sequence.
    /// </exception>
    /// <exception cref="InvalidVariableType">
    /// Reported via <see cref="Utils.error"/> when an unrecognized type keyword follows the COLON.
    /// </exception>
    public static FuncdefNode parse(Tokenizer T, bool doDeclare=true)
    {
        T.expect(TokenSymbols.FUNCDEF);
        Token name = T.expect(TokenSymbols.ID);
        FuncdefNode func = new FuncdefNode(name);
        currentFunction = func;
        LocalLocation.resetCounter();
        ParameterLocation.resetCounter();
        SymbolTable.addScope();
        List<ParamInfo> parameters = new List<ParamInfo>();

        T.expect(TokenSymbols.LPAREN);
        if (T.peek() != TokenSymbols.RPAREN)
        {
            while (T.peek() != TokenSymbols.LBRACE && T.peek() != TokenSymbols.COLON)
            {
                Token id = T.expect(TokenSymbols.ID);
                T.expect(TokenSymbols.COLON);
                Token ptype = new Token(); // what is this...
                if (T.peek() != TokenSymbols.TYPE && T.peek() != TokenSymbols.ID)
                    Utils.error(new InvalidParameter($"Expected paramter type to be ID or TYPE on line {T.getLine()}; got {T.peek()}"));
                ptype = T.next();
                if (doDeclare)
                    SymbolTable.declare(id, VarType.fromToken(ptype), new ParameterLocation());
                parameters.Add(new ParamInfo(id, VarType.fromToken(ptype)));
                T.next(); // consumes COMMA or RPAREN
            }
        }
        else
            T.expect(TokenSymbols.RPAREN);
        
        VarType returnType;
        // Check for optional return type specification
        if (T.peek() == TokenSymbols.COLON)
        {
            T.expect(TokenSymbols.COLON);
            returnType = VarType.fromToken(T.next());
        }
        else
            returnType = new VoidType();

        StmtsNode stmts = StmtsNode.parse(T);
        FuncType funcType = new FuncType(returnType, parameters, currentFunction);
        SymbolTable.removeScope();
        VarLocation loc = SymbolTable.isGlobalScope() ? new GlobalLocation() : new LocalLocation(func);

        if (doDeclare)
            SymbolTable.declare(name, funcType, loc); // declare function in the scope above parameters
        else
            loc = new MemberLocation(); // only members should not be declared atm

        VarInfo funcInfo = new VarInfo(name, funcType, loc);
        currentFunction = null;
        func.info = funcInfo;
        func.stmts = stmts;
        return func;
    }
    
    /// <summary>
    /// Gets the child nodes of this function definition.
    /// </summary>
    /// <returns>A list containing only the statement block node.</returns>
    /// <remarks>
    /// Returns only the statement block. Parameter information is not represented
    /// as a separate AST subtree; it is stored within the function's <see cref="FuncType"/>
    /// inside <see cref="info"/>.
    /// </remarks>
    public override List<TreeNode> getChildren()
    {
        if (stmts == null)
            throw new Exception("Internal compiler error: tried to return a null function StmtsNode");
        return new List<TreeNode>() { stmts };
    }

    /// <summary>
    /// Type inference for function definition nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return;
    }

    /// <summary>
    /// Type validation for function definition nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        if (this.info == null)
            throw new Exception("Internal compiler error: function info was null during typecheck -- function parsing failed to assign non-null info to function.");
        if (this.info.token == null)
            throw new Exception("Internal compiler error: unexpected function with uninitialized token during type check.");
        if (this.info.token.lexeme == "main")
        {
            VarInfo vtype = SymbolTable.lookup(this.info.token.lexeme, this.info.token.line);
            FuncType? ftype = vtype.type as FuncType;
            if (ftype == null)
                throw new Exception("Internal compiler error: unexpected function with non-function type.");
            if (ftype.returnType != VarType.Void)
            {
                if (ftype.returnType != VarType.Int && ftype.returnType != VarType.Float && ftype.returnType != VarType.BoolConst)
                    Utils.error(new MissingMainFunction($"Invalid main function. Expected return Int; got {ftype.returnType}"));
            }
        }
    }

    public override void genCode()
    {
        if (this.info == null)
            throw new Exception("Internal compiler error: function info was null during code generation -- function parsing failed to assign non-null info to function.");
        if (this.info.token == null)
            throw new Exception("Internal compiler error: cannot generate ASM for function with missing VarInfo token.");
        Asm.emit(
            new Comment($"*********** {this.info.token.lexeme} ************"),
            this.lbl,
            new Comment("*** function prologue ***"),
            new OpPushReg(Register.rbp),
            new OpMovRegReg(src: Register.rsp, dst: Register.rbp),
            new Comment("*** allocate space for temporaries ***"),
            new OpSubRegConst(Register.rsp, (this.maxTemporaries + this.numLocals) * 16)
        );
        if (this.stmts == null)
            throw new Exception("Internal compiler error: function statements was null during code generation -- function parsing failed to assign non-null statements to function.");
       
        foreach (VardeclNode local in locals)
        {
            LocalLocation? location = local.location as LocalLocation; //must be local, was added to locals
            if (location == null)
                throw new Exception($"Internal compiler error: expected variable {local.varname.lexeme} to have a local location; declared in function {this.info.token.lexeme}");
            int offset = -(this.maxTemporaries + location.getNumber() + 1) * 16;
            VarType type = local.type;
    
            if ((type as StringConstType) != null)
            {
                // pointer field: emptyString
                Asm.emit(new OpMovLabelAddrReg(new Label("emptyString"), Register.rax));
                Asm.emit(new OpMovRegRegInd(src: Register.rax, offset: offset, dst: Register.rbp));
                //STATIC
                Asm.emit(new OpMovConstReg((long)StorageClass.STATIC, Register.rax));
                Asm.emit(new OpMovRegRegInd(src: Register.rax, offset: offset + 8, dst: Register.rbp));
            }
            else
            {
                // value field: 0
                Asm.emit(new OpMovConstReg(0, Register.rax));
                Asm.emit(new OpMovRegRegInd(src: Register.rax, offset: offset, dst: Register.rbp));
                //STATIC
                Asm.emit(new OpMovConstReg((long)StorageClass.STATIC, Register.rax));
                Asm.emit(new OpMovRegRegInd(src: Register.rax, offset: offset + 8, dst: Register.rbp));
            }
        }
        
        this.stmts.genCode();

        // Safety net: if execution falls off the end of the function
        // (no return statement), this ensures a clean epilogue
        Asm.emit(
            new Comment("return fall off end"),
            new OpMovRegReg(src: Register.rbp, dst: Register.rsp),
            new OpPopReg(Register.rbp),
            new OpRet()
        );
        
        Asm.emit(new Comment($"*********** End of {this.info.token.lexeme} ***********"));
    }
}
