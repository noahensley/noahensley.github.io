/*
 * File: OpMoveRegReg.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      March 21, 2026
 */
namespace ASM
{
    /// <summary>
    /// Emits an x86-64 register-to-register move instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>movq src, dst</c> in AT&amp;T syntax. Copies the 64-bit value of
    /// <paramref name="src"/> into <paramref name="dst"/>. Supports both integer and
    /// XMM register arguments via the base <see cref="Register"/> type, enabling use
    /// for general-purpose moves as well as transferring float bit patterns between
    /// integer and XMM registers (e.g., loading a double constant via <c>rax</c> into <c>xmm0</c>).
    /// </remarks>
    public class OpMovRegReg : Op
    {
        Register src, dst;

        /// <summary>
        /// Creates a new register-to-register move operation.
        /// </summary>
        /// <param name="src">The source register.</param>
        /// <param name="dst">The destination register.</param>
        public OpMovRegReg(Register src, Register dst)
        {
            this.src = src;
            this.dst = dst;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>movq src, dst</c>.</returns>
        public override string ToString()
        {
            return $"movq {this.src}, {this.dst}";
        }
    }
}
