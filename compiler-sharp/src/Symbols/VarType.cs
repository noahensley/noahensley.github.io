/*
 * File: VarType.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026
 */

using System.Diagnostics;

public class VarType
{
    /// <summary>
    /// Creates a <see cref="VarType"/> instance from a token containing a type lexeme.
    /// </summary>
    /// <param name="t">
    /// Token containing a type keyword ("int", "float", "string", "bool", or "func").
    /// </param>
    /// <returns>
    /// An instance of the appropriate <see cref="VarType"/> subclass:
    /// <list type="bullet">
    /// <item><see cref="IntType"/> for "int"</item>
    /// <item><see cref="FloatType"/> for "float"</item>
    /// <item><see cref="StringConstType"/> for "string"</item>
    /// <item><see cref="BoolConstType"/> for "bool"</item>
    /// <item>
    /// <see cref="FuncType"/> for "func", initialized with a <see cref="VoidType"/>
    /// return type and an empty parameter list
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// When the "func" keyword is encountered, a default function type is created
    /// with a <see cref="VoidType"/> return type and no parameters. This allows
    /// function-typed variables or declarations to be represented even before
    /// a full signature is known.
    /// </remarks>
    /// <exception cref="InvalidVariableType">
    /// Thrown when the token does not contain a recognized type keyword.
    /// </exception>
    public static VarType fromToken(Token t)
    {
        if (t.sym == TokenSymbols.ID || t.sym == TokenSymbols.THIS)
            return ProgramNode.getClassTypeFromClassName(t);
        switch (t.lexeme)
        {
            case "int": return new IntType();
            case "float": return new FloatType();
            case "string": return new StringConstType();
            case "bool": return new BoolConstType();
            case "func": return new FuncType(new VoidType(), [], FuncdefNode.currentFunction!);
            default:
                Utils.error(new InvalidVariableType($"Expected variable type, but got {t}"));
                throw new UnreachableException();  // Unreachable due to Utils.error throwing, but required for compiler
        }
    }

    /// <summary>
    /// Determines whether this <see cref="VarType"/> is equal to another object by comparing runtime types.
    /// </summary>
    /// <param name="other">The object to compare with this instance.</param>
    /// <returns><c>true</c> if <paramref name="other"/> is the same type as this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(Object? other)
    {
        if (other == null) // Just check other because == overload checks for null left object
            return false;
        return this.GetType() == other.GetType();
    }

    /// <summary>
    /// Determines whether two <see cref="VarType"/> instances are equal.
    /// </summary>
    /// <param name="v1">The left operand.</param>
    /// <param name="v2">The right operand.</param>
    /// <returns><c>true</c> if both instances are the same type or both are <c>null</c>; otherwise, <c>false</c>.</returns>
    public static bool operator==(VarType? v1, VarType? v2)
    {
        if (Object.ReferenceEquals(v1, null))
            return Object.ReferenceEquals(v2, null); // If left object is null, right object must be too (else neq)
        return v1.Equals(v2);
    }

    /// <summary>
    /// Determines whether two <see cref="VarType"/> instances are not equal.
    /// </summary>
    /// <param name="v1">The left operand.</param>
    /// <param name="v2">The right operand.</param>
    /// <returns><c>true</c> if the instances differ in type; otherwise, <c>false</c>.</returns>
    public static bool operator!=(VarType? v1, VarType? v2)
    {
        return !(v1 == v2);
    }

    /// <summary>
    /// Not implemented. Hash code generation based on memory location causes incorrect dictionary hashing behavior.
    /// </summary>
    /// <returns>Never returns; always throws.</returns>
    /// <exception cref="NotImplementedException">Always thrown.</exception>
    public override int GetHashCode()
    {
        // calling base.GetHashCode here causes dictionary hashing errors
        throw new NotImplementedException();
    }

    // Singletone instance = shared instance
    /// <summary>
    /// Shared singleton instance of <see cref="IntType"/> for use in type comparisons and annotations.
    /// </summary>
    public static readonly IntType Int = new IntType();

    /// <summary>
    /// Shared singleton instance of <see cref="FloatType"/> for use in type comparisons and annotations.
    /// </summary>
    public static readonly FloatType Float = new FloatType();

    /// <summary>
    /// Shared singleton instance of <see cref="StringConstType"/> for use in type comparisons and annotations.
    /// </summary>
    public static readonly StringConstType StringConst = new StringConstType();

    /// <summary>
    /// Shared singleton instance of <see cref="BoolConstType"/> for use in type comparisons and annotations.
    /// </summary>
    public static readonly BoolConstType BoolConst = new BoolConstType();

    /// <summary>
    /// Shared singleton instance of <see cref="VoidType"/> for use in type comparisons and annotations.
    /// </summary>
    public static readonly VoidType Void = new VoidType();
}

/// <summary>
/// Represents the integer type in the language type system.
/// </summary>
public class IntType: VarType
{
}

/// <summary>
/// Represents the floating-point type in the language type system.
/// </summary>
public class FloatType: VarType
{
}

/// <summary>
/// Represents the string constant type in the language type system.
/// </summary>
public class StringConstType: VarType
{
}

/// <summary>
/// Represents the boolean constant type in the language type system.
/// </summary>
public class BoolConstType: VarType
{
}

/// <summary>
/// Represents a function type in the language type system.
/// </summary>
/// <remarks>
/// A function type encapsulates the complete signature of a function,
/// including:
/// <list type="bullet">
/// <item>The return type</item>
/// <item>The ordered list of parameter descriptors</item>
/// </list>
/// 
/// Function types are used by function definitions and by any variables
/// declared with type <c>func</c>. The parameter list preserves declaration
/// order to support argument matching during semantic analysis.
/// 
/// When constructed via <see cref="VarType.fromToken"/> using the
/// "func" keyword, the function type defaults to a <see cref="VoidType"/>
/// return type and an empty parameter list.
/// </remarks>
public class FuncType : VarType
{
    /// <summary>
    /// The return type of the function.
    /// </summary>
    /// <remarks>
    /// Specifies the type of value produced when the function completes.
    /// For procedures (functions with no return value), this is an instance
    /// of <see cref="VoidType"/>.
    /// </remarks>
    public VarType returnType;

    /// <summary>
    /// The ordered list of parameters accepted by the function.
    /// </summary>
    /// <remarks>
    /// Each entry describes a single parameter, including its identifier
    /// and type. The order of this list corresponds to the order in which
    /// parameters are declared and is significant for argument matching
    /// during function calls.
    /// 
    /// An empty list indicates that the function accepts no arguments.
    /// </remarks>
    public List<ParamInfo> parameters;

    public FuncdefNode? declarer;

    public bool builtin = false;

    /// <summary>
    /// Creates a new function type with the specified return type and parameters.
    /// </summary>
    /// <param name="ret">The return type of the function.</param>
    /// <param name="par">The ordered list of parameter descriptors.</param>
    /// <remarks>
    /// This constructor is used when constructing full function signatures,
    /// such as during parsing of a function definition.
    /// </remarks>
    public FuncType(VarType ret, List<ParamInfo> par, FuncdefNode declarer)
    {
        if (declarer == null)
            throw new Exception("Internal compiler error: got FuncType with no declaring function.");
        this.returnType = ret;
        this.parameters = par;
        this.declarer = declarer;
    }

    public FuncType(VarType ret, List<ParamInfo> par, bool builtin)
    {
        this.returnType = ret;
        this.parameters = par;
        this.builtin = builtin;
        this.declarer = null;
    }
}

/// <summary>
/// Represents a user-defined class type in the language type system.
/// </summary>
/// <remarks>
/// ClassType instances correspond to class declarations parsed from the source program.
/// Each instance stores the identifier token for the class and a reference to the
/// <see cref="ClassdefNode"/> that declares it once semantic analysis resolves the class.
/// </remarks>
public class ClassType : VarType
{
    /// <summary>
    /// The identifier token naming the class.
    /// </summary>
    public Token name;

    /// <summary>
    /// The AST node that declares this class.
    /// </summary>
    /// <remarks>
    /// This reference is assigned once the corresponding <see cref="ClassdefNode"/>
    /// has been parsed and registered during semantic analysis.
    /// </remarks>
    public ClassdefNode? declarer = null;

    /// <summary>
    /// Creates a new <see cref="ClassType"/> instance.
    /// </summary>
    /// <param name="name">The identifier token representing the class name.</param>
    /// <param name="declarer">The class declaration node, if already known.</param>
    public ClassType(Token name, ClassdefNode? declarer)
    {
        this.name = name;
        this.declarer = declarer;
    }

    /// <summary>
    /// Looks up a member declared within the class.
    /// </summary>
    /// <param name="name">The member name to resolve.</param>
    /// <returns>The <see cref="VarType"/> of the member.</returns>
    /// <remarks>
    /// Searches the class declaration for a matching member function or variable.
    /// If no matching member exists, an <see cref="UndeclaredVariable"/> error is reported.
    /// </remarks>
    public VarType lookup(string name)
    {
        if (this.declarer == null)
        {
            Utils.error(new UndeclaredClass($"Class on line {this.name.line} is undeclared."));
            throw new UnreachableException();
        }

        if (this.declarer.memberFunctions.ContainsKey(name))
            return this.declarer.memberFunctions[name];

        if (this.declarer.memberVariables.TryGetValue(name, out VarType? vtype))
        {
            if (vtype == null)
                throw new Exception("Internal compiler error: tried to lookup class member variable but got null VarType.");
            return vtype;
        }

        Utils.error(new UndeclaredVariable($"Class {this.name} does not contain declaration of variable {name}"));
        throw new UnreachableException();
    }
}

/// <summary>
/// Represents the void type (no return value) in the language type system.
/// </summary>
public class VoidType: VarType
{
}