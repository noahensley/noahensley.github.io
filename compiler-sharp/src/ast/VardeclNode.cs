/*
 * File: VardeclNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 23, 2026
 *      February 25, 2026
 */

using ASM;
/// <summary>
/// Represents a variable declaration statement in the abstract syntax tree.
/// </summary>
/// <remarks>
/// A VardeclNode is produced when the parser encounters a variable declaration of the form
/// <c>var &lt;identifier&gt; : &lt;type&gt;</c>. It appears both as a statement within function
/// bodies and as a top-level global declaration in <see cref="ProgramNode"/>.
/// </remarks>
public class VardeclNode : StmtNode 
{
    /// <summary>
    /// The identifier token representing the name of the variable being declared.
    /// </summary>
    public Token varname;

    public VarType type;

    public VarLocation location;

    /// <summary>
    /// Initializes a VardeclNode with the given name and type tokens.
    /// </summary>
    /// <param name="varname">The identifier token for the variable name.</param>
    /// <param name="vartype">The type keyword token for the variable type.</param>
    public VardeclNode(Token varname, VarType type, VarLocation location)
    {
        this.varname = varname;
        this.type = type;
        this.location = location;
    }

    /// <summary>
    /// Determines whether the token stream is positioned at a variable declaration.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns><c>true</c> if the next token is the VAR keyword; otherwise <c>false</c>.</returns>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.VAR;
    }

    /// <summary>
    /// Parses a variable declaration from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer containing the input token stream.</param>
    /// <returns>A <see cref="VardeclNode"/> containing the parsed name and type tokens.</returns>
    /// <remarks>
    /// <para>
    /// Expects and consumes tokens in the following order:
    /// </para>
    /// <list type="number">
    /// <item>VAR keyword</item>
    /// <item>Identifier (variable name)</item>
    /// <item>COLON</item>
    /// <item>TYPE keyword</item>
    /// <item>EOS (end-of-statement), or EOF if the declaration appears at the very end of the file</item>
    /// </list>
    /// <para>
    /// <b>Location assignment:</b> The variable is declared in the symbol table with either
    /// <see cref="GlobalLocation"/> or <see cref="LocalLocation"/> depending on whether the
    /// current scope is the global scope. <see cref="ParameterLocation"/> is never assigned here —
    /// that is handled exclusively by <see cref="FuncdefNode.parse"/> when processing function
    /// parameter lists. <c>MemberLocation</c> is not yet implemented.
    /// </para>
    /// </remarks>
    /// <exception cref="UnexpectedToken">
    /// Reported via <see cref="Utils.error"/> when the token stream does not match the expected
    /// declaration syntax (e.g., missing ID, COLON, or TYPE).
    /// </exception>
    public static VardeclNode parse(Tokenizer T, bool doDeclare=true, VarLocation? location=null)
    {
        T.expect(TokenSymbols.VAR);
        Token varname = T.expect(TokenSymbols.ID);
        T.expect(TokenSymbols.COLON);

        Token vartype = T.next();
        if (vartype.sym != TokenSymbols.TYPE && vartype.sym != TokenSymbols.ID)
            Utils.error(new InvalidExpression($"Expected variable TYPE; got {vartype.lexeme} ({vartype.line},{vartype.column})"));

        if (T.peek() == TokenSymbols.EOS)
            T.expect(TokenSymbols.EOS);
        else if (T.peek() == TokenSymbols.EOF)
            T.expect(TokenSymbols.EOF);

        VarLocation loc;
        VardeclNode? vdecl;
        if (SymbolTable.isGlobalScope())
        {
            loc = new GlobalLocation();
            vdecl = new VardeclNode(varname, VarType.fromToken(vartype), loc);
        }
        else
        {
            if (FuncdefNode.currentFunction == null)
                throw new Exception("Internal compiler error: not in global scope but current function was null");

            loc = new LocalLocation(FuncdefNode.currentFunction);
            vdecl = new VardeclNode(varname, VarType.fromToken(vartype), loc);
            FuncdefNode.currentFunction.locals.Add(vdecl);
            FuncdefNode.currentFunction.numLocals++;
        }

        if (location != null) // location is a MemberLocation here
            loc = location;
            
        if (doDeclare)
            SymbolTable.declare(varname, VarType.fromToken(vartype), loc);
        return vdecl;
    }

    /// <summary>
    /// Gets the child nodes of this declaration.
    /// </summary>
    /// <returns>An empty list, as variable declarations have no child nodes.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>(){};
    }

    /// <summary>
    /// Type inference for variable declaration nodes. Not yet implemented.
    /// </summary>
    public override void setType()
    {
        return;
    }

    /// <summary>
    /// Type validation for variable declaration nodes. Not yet implemented.
    /// </summary>
    public override void typeCheck()
    {
        return;
    }
}
