/*
 * File: OpShiftRightLogical.cs
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
    /// Emits an x86-64 logical right shift instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>shr %cl, reg</c> in AT&amp;T syntax. Shifts <paramref name="reg"/> right by
    /// the number of bits specified in <c>%cl</c> (the low byte of <c>rcx</c>), always filling
    /// vacated high bits with zero regardless of the sign of the original value. This implements
    /// the unsigned <c>&gt;&gt;&gt;</c> operator.
    /// </para>
    /// <para>
    /// Contrast with <see cref="OpShiftRightArithmetic"/>, which fills vacated bits with the
    /// sign bit and implements the signed <c>&gt;&gt;</c> operator.
    /// </para>
    /// <para>
    /// The caller must load the shift count into <c>rcx</c> before emitting this instruction.
    /// Intel hardware masks the shift count to the low 6 bits for 64-bit operands, so shifts
    /// of 64 or more must be handled explicitly with a subsequent <see cref="OpCmovCC"/> guard.
    /// </para>
    /// </remarks>
    public class OpShiftRightLogical : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new logical right shift operation.
        /// </summary>
        /// <param name="reg">The register to shift. The shift count is taken implicitly from <c>%cl</c>.</param>
        public OpShiftRightLogical(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <remarks>
        /// Note: operand order is reversed relative to Intel syntax (AT&amp;T convention).
        /// </remarks>
        /// <returns>A string of the form <c>shr %cl, reg</c>.</returns>
        public override string ToString()
        {
            //operand order is backwards
            return $"shr %cl, {this.reg}";
        }
    }
}
