/*
 * File: OpShiftLeft.cs
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
    /// Emits an x86-64 logical left shift instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>shl %cl, reg</c> in AT&amp;T syntax. Shifts <paramref name="reg"/> left by
    /// the number of bits specified in <c>%cl</c> (the low byte of <c>rcx</c>), filling
    /// vacated bits with zero.
    /// </para>
    /// <para>
    /// The caller must load the shift count into <c>rcx</c> before emitting this instruction.
    /// Intel hardware masks the shift count to the low 6 bits for 64-bit operands, so shifts
    /// of 64 or more must be handled explicitly — typically by emitting an <see cref="OpCmovCC"/>
    /// that zeros the result when the count exceeds 63.
    /// </para>
    /// </remarks>
    public class OpShiftLeft : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new left shift operation.
        /// </summary>
        /// <param name="reg">The register to shift. The shift count is taken implicitly from <c>%cl</c>.</param>
        public OpShiftLeft(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>shl %cl, reg</c>.</returns>
        public override string ToString()
        {
            return $"shl %cl, {this.reg}";
        }
    }
}
