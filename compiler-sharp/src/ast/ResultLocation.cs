/*
 * File: ResultLocation.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      March 28, 2026
 */
using ASM;

/// <summary>
/// Represents a location that can store or provide the result of an expression.
/// </summary>
/// <remarks>
/// Implemented by <c>Temporary</c> and <c>Variable</c>. Temporaries store intermediate
/// expression results on the stack; variables return themselves as their result location
/// and load their value directly from memory.
/// </remarks>
public interface ResultLocation
{
    /// <summary>
    /// Emits code to load the integer value from this result location into the specified register.
    /// </summary>
    /// <param name="reg">The destination integer register.</param>
    public void copyToRegister(IntRegister reg, IntRegister? klass);

    public void copyFromRegister(IntRegister reg, IntRegister klass);

    /// <summary>
    /// Emits code to store an integer register's value into this result location.
    /// </summary>
    /// <param name="reg">The source integer register.</param>
    /// <param name="klass">The storage class to associate with the stored value.</param>
    public void copyFromRegister(IntRegister reg, StorageClass klass);

    /// <summary>
    /// Emits code to load the float value from this result location into the specified register.
    /// </summary>
    /// <param name="reg">The destination float register.</param>
    public void copyToRegister(FloatRegister reg, StorageClass klass);

    /// <summary>
    /// Emits code to store a float register's value into this result location.
    /// </summary>
    /// <param name="reg">The source float register.</param>
    /// <param name="klass">The storage class to associate with the stored value.</param>
    public void copyFromRegister(FloatRegister reg, StorageClass klass);
}