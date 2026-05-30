/*
 * File: OpMovConstReg.cs
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
    /// Emits an x86-64 64-bit immediate-to-register move instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>movabsq $value, dst</c> in AT&amp;T syntax. Loads a full 64-bit immediate
    /// constant into the destination register. Unlike <c>movq</c> with an immediate, <c>movabsq</c>
    /// can encode any 64-bit value including large positive integers and reinterpreted float bit patterns.
    /// </para>
    /// <para>
    /// Used to load integer literals and the raw bit representations of floating-point literals
    /// before transferring them into XMM registers via <see cref="OpMovRegReg"/>.
    /// </para>
    /// </remarks>
    public class OpMovConstReg : Op
    {
        long value;
        IntRegister dst;

        /// <summary>
        /// Creates a new immediate-to-register move operation.
        /// </summary>
        /// <param name="value">The 64-bit immediate value to load.</param>
        /// <param name="dst">The destination integer register.</param>
        public OpMovConstReg(long value, IntRegister dst)
        {
            this.value = value;
            this.dst = dst;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>movabsq $value, dst</c>.</returns>
        public override string ToString()
        {
            return $"movabsq ${this.value}, {this.dst}";
        }
    }
}
