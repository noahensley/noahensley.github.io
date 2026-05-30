/*
 * File: ClassdefNode.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 */

/// <summary>
/// Represents a class definition in the abstract syntax tree.
/// </summary>
/// <remarks>
/// Class definition nodes store the name of the class and its declared
/// member variables and functions. During parsing they collect member
/// declarations, and during semantic analysis they associate those
/// members with the corresponding <see cref="ClassType"/>.
/// </remarks>
public class ClassdefNode : TreeNode
{
    /// <summary>
    /// The identifier token representing the class name.
    /// </summary>
    public Token name;
    
    /// <summary>
    /// The semantic type representation associated with this class.
    /// </summary>
    public ClassType classType;

    /// <summary>
    /// Tracks the class type currently being parsed.
    /// </summary>
    public static ClassType? curClassType;

    /// <summary>
    /// The list of method definitions declared in the class.
    /// </summary>
    public List<FuncdefNode> methods;

    /// <summary>
    /// A mapping of member function names to their corresponding function types.
    /// </summary>
    public Dictionary<string, FuncType> memberFunctions = new Dictionary<string, FuncType>();
    
	/// <summary>
    /// A mapping of member variable names to their corresponding variable types.
    /// </summary>
	public Dictionary<string, VarType?> memberVariables = new Dictionary<string, VarType?>();

    private ClassdefNode(Token name, List<FuncdefNode> methods)
    {
        this.name = name;
        this.classType = ProgramNode.getClassTypeFromClassName(this.name);
        if (this.classType.declarer != null)
        {
            ClassdefNode exists = this.classType.declarer;
            Utils.error(new DuplicateDeclaration($"Class declaration '{this.name.lexeme}' on line {this.name.line} already exists; Declared on ({exists.name.line},{exists.name.column})"));
        }
        this.classType.declarer = this;
        this.methods = methods;
        curClassType = this.classType;
    }

    /// <summary>
    /// Determines whether the tokenizer is positioned at the start of a class definition.
    /// </summary>
    /// <param name="T">The tokenizer providing the token stream.</param>
    /// <returns>
    /// <c>true</c> if the next token represents a class declaration; otherwise <c>false</c>.
    /// </returns>
    public static bool canParse(Tokenizer T)
    {
        return T.peek() == TokenSymbols.CLASS;
    }

    /// <summary>
    /// Parses a class definition from the token stream.
    /// </summary>
    /// <param name="T">The tokenizer providing the token stream.</param>
    /// <returns>The parsed <see cref="ClassdefNode"/>.</returns>
    /// <remarks>
    /// This method reads a class declaration and collects its member functions
    /// and variables, recording them in the class member tables.
    /// </remarks>
    public static ClassdefNode parse(Tokenizer T)
    {
        T.expect(TokenSymbols.CLASS);
        Token name = T.expect(TokenSymbols.ID);
        T.expect(TokenSymbols.LBRACE);
        T.expect(TokenSymbols.EOS);

        ClassdefNode clnode = new ClassdefNode(name, []);

        while (T.peek() != TokenSymbols.RBRACE)
        {
            if (FuncdefNode.canParse(T))
            {
                FuncdefNode funcDef = FuncdefNode.parse(T, doDeclare: false); // pass member location option
                if (funcDef.info == null)
                    throw new Exception("Internal compiler error: function info is null during class parsing -- function parsing failed to assign non-null info to function.");
                if (funcDef.info.token == null)
                    throw new Exception($"Internal compiler error: failed to assign a token to FuncdefNode inside member {name}");

                string funcName = funcDef.info.token.lexeme;
                FuncType? funcType = funcDef.info.type as FuncType;

                if (funcType == null)
                    throw new Exception($"Internal compiler error: failed to assign a FuncType to FuncdefNode inside member {name}");

                if (clnode.memberFunctions.ContainsKey(funcName) || clnode.memberVariables.ContainsKey(funcName))
                    Utils.error(new DuplicateDeclaration($"Duplicate member function {funcName} on line {funcDef.info.token.line}."));

                clnode.memberFunctions[funcName] = funcType;
                clnode.methods.Add(funcDef);
            }
            else if (VardeclNode.canParse(T))
            {
                VardeclNode varDecl = VardeclNode.parse(T, doDeclare: false, location: new MemberLocation()); // pass member location option
                VarInfo varInfo = new VarInfo(varDecl.varname, varDecl.type, new MemberLocation());
                string varName = varDecl.varname.lexeme;

                if (varInfo.token == null)
                    throw new Exception("Internal compiler error: failed to initialize new member VarInfo with non-null token.");

                if (clnode.memberFunctions.ContainsKey(varName) || clnode.memberVariables.ContainsKey(varName))
                    Utils.error(new DuplicateDeclaration($"Duplicate member variable {varName} on line {varInfo.token.line}."));

                clnode.memberVariables[varDecl.varname.lexeme] = varInfo.type;
            }
            else
            {
                Token mismatch = T.next();
                Utils.error(new InvalidMember($"Expected function or variable member in class {name}; Got {mismatch.lexeme} ({mismatch.line},{mismatch.column})"));
            }
        }
        T.next(); // consume the RBRACE

        return clnode;
    }

    /// <summary>
    /// Gets the child nodes belonging to this class definition.
    /// </summary>
    /// <returns>A list of method nodes declared in the class.</returns>
    public override List<TreeNode> getChildren()
    {
        return new List<TreeNode>(methods);
    }

    /// <summary>
    /// Performs type inference for this node.
    /// </summary>
    public override void setType()
    {
        return;
    }

    /// <summary>
    /// Performs semantic validation for this node.
    /// </summary>
    public override void typeCheck()
    {
        return;
    }
}