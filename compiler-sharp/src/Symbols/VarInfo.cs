/*
 * File: VarInfo.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 23, 2026
 */

/// <summary>
/// Holds the semantic information associated with a declared variable.
/// </summary>
/// <remarks>
/// VarInfo is the value type stored in the symbol table for each declared identifier.
/// It bundles together everything downstream phases — such as type checking and code
/// generation — need to know about a variable after its declaration has been processed.
/// All fields are readonly since a declaration's properties do not change after it is created.
/// </remarks>
public class VarInfo
{
    /// <summary>
    /// The token from the original declaration, used for error reporting and debugging.
    /// </summary>
    /// <remarks>
    /// Retaining the token allows error messages to reference the exact source location
    /// where the variable was declared, rather than only the site where the error was detected.
    /// </remarks>
    public readonly Token? token;

    /// <summary>
    /// The declared type of the variable.
    /// </summary>
    public readonly VarType? type;

    /// <summary>
    /// The storage location of the variable, indicating how it should be accessed at runtime.
    /// </summary>
    /// <remarks>
    /// Used by the code generator to emit the correct load and store instructions.
    /// See <see cref="VarLocation"/> and its subclasses for the available location categories.
    /// </remarks>
    public readonly VarLocation? location;

    /// <summary>
    /// Creates a new <see cref="VarInfo"/> record for a declared variable.
    /// </summary>
    /// <param name="token">The identifier token from the declaration site.</param>
    /// <param name="type">The declared type of the variable.</param>
    /// <param name="location">The storage location category for the variable.</param>
    public VarInfo(Token token, VarType type, VarLocation? location)
    {
        this.token = token;
        this.type = type;
        this.location = location;
    }
}