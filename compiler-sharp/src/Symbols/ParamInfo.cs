/*
 * File: ParamInfo.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 */

/// <summary>
/// Holds the semantic information associated with a function parameter.
/// </summary>
/// <remarks>
/// ParamInfo is the value type stored within a <see cref="FuncType"/>'s parameter list for each
/// declared parameter. It bundles together everything downstream phases — such as type checking and
/// code generation — need to know about a parameter after its declaration has been processed.
/// All fields are readonly since a declaration's properties do not change after it is created.
/// </remarks>
public class ParamInfo
{
    /// <summary>
    /// The identifier token from the parameter declaration, used for error reporting.
    /// </summary>
    /// <remarks>
    /// Retaining the token allows error messages to reference the exact source location
    /// where the parameter was declared, rather than only the site where the error was detected.
    /// </remarks>
    public readonly Token? token;

    /// <summary>
    /// The declared type of the parameter.
    /// </summary>
    public readonly VarType? type;

    /// <summary>
    /// Creates a new <see cref="ParamInfo"/> record for a declared parameter.
    /// </summary>
    /// <param name="token">The identifier token from the declaration site.</param>
    /// <param name="type">The declared type of the parameter.</param>
    public ParamInfo(Token token, VarType type)
    {
        this.token = token;
        this.type = type;
    }
}
