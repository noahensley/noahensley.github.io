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
    /// Creates a VarType instance from a token containing a type lexeme.
    /// </summary>
    /// <param name="t">Token containing a type keyword ("int", "float", or "string").</param>
    /// <returns>An instance of the appropriate VarType subclass (IntType, FloatType, or StringConstType).</returns>
    /// <exception cref="Exception">Thrown when the token does not contain a recognized type keyword.</exception>
    public static VarType fromToken(Token t)
    {
        switch(t.lexeme)
        {
            case "int": return new IntType();
            case "float": return new FloatType();
            case "string": return new StringConstType();
            default:
                Utils.error(new InvalidVariableType($"Expected variable type, but got {t}"));
                throw new UnreachableException();  // Unreachable due to Utils.error throwing, but required for compiler
        }
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
/// Represents the void type (no return value) in the language type system.
/// </summary>
public class VoidType: VarType
{
}