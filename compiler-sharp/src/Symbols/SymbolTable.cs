/*
 * File: SymbolTable.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 23, 2026
 */

using System.Diagnostics;
using ASM;

/// <summary>
/// Manages scoped variable declarations for semantic analysis through a linked list of scopes.
/// </summary>
/// <remarks>
/// <para>
/// SymbolTable maintains a stack of scopes represented as a singly linked list. The current
/// scope is always at the head of the list, with each scope holding a reference to the
/// enclosing scope. The global scope is initialized at the base of the list and persists
/// for the lifetime of the analysis.
/// </para>
/// <para>
/// All public members are static, providing a single global interface for symbol table
/// operations. Scopes are pushed and popped as the analyzer enters and exits blocks such
/// as function bodies and control flow constructs.
/// </para>
/// </remarks>
public class SymbolTable
{
    /// <summary>
    /// The active (innermost) scope in the symbol table stack.
    /// </summary>
    /// <remarks>
    /// Initialized to a single global scope on program startup. As new scopes are entered
    /// via <see cref="addScope"/>, this reference is updated to point to the new innermost scope.
    /// <see cref="removeScope"/> restores it to the previous scope.
    /// </remarks>
    private static SymbolTable current = new SymbolTable(null);

    /// <summary>
    /// Reference to the enclosing scope, or <c>null</c> if this is the global scope.
    /// </summary>
    private SymbolTable? prev;

    /// <summary>
    /// The variable declarations contained within this scope, keyed by identifier name.
    /// </summary>
    private Dictionary<string, VarInfo> decls = new();

    /// <summary>
    /// Creates a new scope linked to the given enclosing scope.
    /// </summary>
    /// <param name="prev">The enclosing scope, or <c>null</c> if this is the global scope.</param>
    private SymbolTable(SymbolTable? prev)
    {
        this.prev = prev;
    }

    public static Dictionary<string, VarInfo> getCurrentDecls()
    {
        return current.decls;
    }

    /// <summary>
    /// Resets the symbol table to a fresh global scope, discarding all existing declarations.
    /// </summary>
    /// <remarks>
    /// Should be called between consecutive parse runs to prevent stale declarations from a
    /// previous file from polluting the next. Replaces the current scope chain with a single
    /// new global scope.
    /// </remarks>
    public static void resetTable()
    {
        current = new SymbolTable(null);
    }

    /// <summary>
    /// Returns <c>true</c> if the current active scope is the global (outermost) scope.
    /// </summary>
    /// <returns><c>true</c> when no enclosing scope exists; otherwise <c>false</c>.</returns>
    public static bool isGlobalScope()
    {
        return current.prev == null;
    }

    /// <summary>
    /// Pushes a new scope onto the symbol table stack.
    /// </summary>
    /// <remarks>
    /// Creates a new <see cref="SymbolTable"/> linked to the current scope and sets it as the
    /// active scope. Should be called when entering a new block, such as a function body or
    /// control flow construct. Each call to <see cref="addScope"/> must be matched by a
    /// corresponding call to <see cref="removeScope"/>.
    /// </remarks>
    public static void addScope()
    {
        current = new SymbolTable(current);
    }

    /// <summary>
    /// Pops the current scope from the symbol table stack.
    /// </summary>
    /// <remarks>
    /// Discards the active scope and restores the enclosing scope as current. All variable
    /// declarations in the popped scope become inaccessible. Should be called when exiting
    /// a block entered with <see cref="addScope"/>.
    /// </remarks>
    /// <exception cref="InvalidScopeRemoval">
    /// Reported via <see cref="Utils.error"/> when attempting to remove the global (outermost) scope.
    /// </exception>
    public static void removeScope()
    {
        if (current.prev == null)
        {
            Utils.error(new InvalidScopeRemoval("Cannot remove top-level (global) scope"));
            throw new UnreachableException();
        }
        current = current.prev;
    }

    /// <summary>
    /// Looks up a variable by name across all active scopes, reporting an error if not found.
    /// </summary>
    /// <param name="name">The identifier name to search for.</param>
    /// <param name="line">The source line number, used only for error reporting.</param>
    /// <returns>The <see cref="VarInfo"/> associated with the given name.</returns>
    /// <remarks>
    /// Searches from the innermost scope outward through all enclosing scopes. The first
    /// match found is returned, allowing inner declarations to shadow outer ones.
    /// </remarks>
    /// <exception cref="UndeclaredVariable">
    /// Reported via <see cref="Utils.error"/> when no declaration with the given name exists
    /// in any active scope.
    /// </exception>
    public static VarInfo lookup(string name, int line)
    {
        SymbolTable? cur_st = current;
        while (cur_st != null)
        {
            if (cur_st.decls.ContainsKey(name))
                return cur_st.decls[name];
            cur_st = cur_st.prev;
        }
        Utils.error(new UndeclaredVariable($"Variable on line {line} is undeclared; No scopes contain variable {name}"));
        throw new UnreachableException();
    }

    /// <summary>
    /// Looks up a variable by name across all active scopes, returning <c>null</c> if not found.
    /// </summary>
    /// <param name="name">The identifier name to search for.</param>
    /// <returns>The <see cref="VarInfo"/> associated with the given name, or <c>null</c> if absent.</returns>
    /// <remarks>
    /// Behaves identically to <see cref="lookup(string, int)"/> but returns <c>null</c> instead of
    /// reporting an error when the name is not found. Use this method when the absence of a
    /// declaration is a valid, expected outcome — for example, when checking whether a variable
    /// exists before deciding how to handle it, without treating a missing declaration as a program error.
    /// </remarks>
    public static VarInfo? lookupIfExists(string name)
    {
        SymbolTable? cur_st = current;
        while (cur_st != null)
        {
            if (cur_st.decls.ContainsKey(name))
                return cur_st.decls[name];
            cur_st = cur_st.prev;
        }
        return null;
    }

    /// <summary>
    /// Declares a new variable in the current (innermost) scope.
    /// </summary>
    /// <param name="id">The identifier token representing the variable name.</param>
    /// <param name="type">The type of the variable being declared.</param>
    /// <param name="location">The storage location of the variable (e.g., local, global, parameter).</param>
    /// <remarks>
    /// Adds the variable to the active scope's declaration table. The token is used both
    /// as the key and as a source of line information for error reporting.
    /// </remarks>
    /// <exception cref="DuplicateDeclaration">
    /// Reported via <see cref="Utils.error"/> when a variable with the same name has already
    /// been declared in the current scope.
    /// </exception>
    public static void declare(Token id, VarType type, VarLocation? location)
    {
        if (SymbolTable.current.decls.Keys.Contains(id.lexeme))
        {
            VarInfo vi = current.decls[id.lexeme];
            if (vi.token != null)
                Utils.error(new DuplicateDeclaration($"Variable declaration '{id.lexeme}' on line {id.line} already exists; Declared on ({vi.token.line},{vi.token.column})"));
            Utils.error(new DuplicateDeclaration($"Variable declaration '{id.lexeme}' on line {id.line} already exists."));
        }
            
        current.decls[id.lexeme] = new VarInfo(id, type, location);
    }
    public static Dictionary<string, VarInfo> getGlobals()
    {
        if (!isGlobalScope())
            return new Dictionary<string, VarInfo>();
        return current.decls;
    }

    private static void declareBuiltIn(string name, VarType rtype, List<ParamInfo> argTypes)
    {
        Token t = new Token("ID", name, -1, -1);
        FuncType ftype = new FuncType(rtype, argTypes, true);
        FuncdefNode fdef = new FuncdefNode(t, ftype, new StmtsNode(), new Label(name)); // function label is just the name for builtins
        ftype.declarer = fdef;
        declare(t, ftype, new GlobalLocation());
    }

    public static void populateBuiltins()
    {
        declareBuiltIn("putc", VarType.BoolConst, [new ParamInfo(new Token("ID", "c", -1, -1), VarType.Int)]);
        declareBuiltIn("newline", VarType.Void, []);
        declareBuiltIn("putv", VarType.BoolConst, [new ParamInfo(new Token("ID", "x", -1, -1), VarType.Int),
                                                    new ParamInfo(new Token("ID", "y", -1, -1), VarType.Int)]);
        declareBuiltIn("getc", VarType.Int, []);
        declareBuiltIn("print", VarType.Void, [new ParamInfo(new Token("ID", "s", -1, -1), VarType.StringConst)]);
        declareBuiltIn("length", VarType.Int, [new ParamInfo(new Token("ID", "s", -1, -1), VarType.StringConst)]);
        declareBuiltIn("concatenateStrings", VarType.StringConst, [new ParamInfo(new Token("ID", "s1", -1, -1), VarType.StringConst),
                                                    new ParamInfo(new Token("ID", "s2", -1, -1), VarType.StringConst)]);
        declareBuiltIn("intToString", VarType.StringConst, [new ParamInfo(new Token("ID", "number", -1, -1), VarType.Int),
                                                    new ParamInfo(new Token("ID", "rsp", -1, -1), VarType.Int),
                                                new ParamInfo(new Token("ID", "rbp", -1, -1), VarType.Int)]);
    }


}
