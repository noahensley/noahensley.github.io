/*
 * File: LValue.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      March 28, 2026
 */
using ASM;

/// <summary>
/// Represents an expression that can appear on the left-hand side of an assignment.
/// </summary>
/// <remarks>
/// An lvalue has a location in memory whose address can be computed. This interface
/// is implemented by <c>Variable</c> and may later be implemented by array index
/// expressions and member access expressions.
/// </remarks>
public interface LValue
{
    /// <summary>
    /// Emits code to load the memory address of this lvalue into the specified register.
    /// </summary>
    /// <param name="reg">The register that will receive the address.</param>
    public void copyAddressToRegister(IntRegister reg);
}