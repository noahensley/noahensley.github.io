/// <summary>
/// Represents a class member reference within the abstract syntax tree.
/// </summary>
/// <remarks>
/// Member nodes represent identifiers that correspond to fields or functions
/// belonging to a class. They are typically produced when resolving the
/// right-hand side of a dot operator and later store the class that declares
/// the referenced member.
/// </remarks>
public class Member: Term
{
    /// <summary>
    /// The class type that declares this member.
    /// </summary>
    public ClassType? declaringClassType;

    /// <summary>
    /// Creates a new member node.
    /// </summary>
    /// <param name="token">The identifier token representing the member name.</param>
    public Member(Token token): base(token)
    {
    }

    /// <summary>
    /// Produces descriptive information about the member and its resolved type.
    /// </summary>
    /// <returns>
    /// A string describing the member name, the declaring class, and the member type.
    /// </returns>
    /// <remarks>
    /// This method is primarily used for diagnostics and debugging. It determines
    /// the member type and constructs a human-readable description of the resolved
    /// class member.
    /// </remarks>
    public string getMemberInfo()
    {
        if (this.declaringClassType == null)
            throw new Exception($"Internal compiler error: no declaring class found for {token.lexeme}");
        string mtype = "ERROR";
        switch (type)
        {
            case IntType:
                mtype = "int";
                break;
            case FloatType:
                mtype = "float";
                break;
            case StringConstType:
                mtype = "string";
                break;
            case BoolConstType:
                mtype = "bool";
                break;
            case FuncType:
                mtype = "function";
                break;
            case ClassType:
                if (declaringClassType.declarer == null)
                    throw new Exception("Internal compiler error: tried to get member info for class type with no declarer.");
                if (declaringClassType.declarer.memberVariables.ContainsKey(this.token.lexeme))
                {
                    ClassType? cltype = declaringClassType.declarer.memberVariables[token.lexeme] as ClassType;
                    if (cltype == null)
                        throw new Exception($"Internal compiler error: mismatch between declared variable type and actual: got {cltype}; expected ClassType");
                    mtype = cltype.name.lexeme;
                }
                break;
            default:
                Utils.error(new InvalidMember($"Expected member of type int,float,string,bool,function; got {type}"));
                break;
        }
        string info = $"Variable {token.lexeme} on line {token.line} is a member of class {declaringClassType.name.lexeme} and is of type {mtype}";
        return info;
    }

}