/*
 * File: OpShiftRightArithmetic.cs
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
    /// Emits an x86-64 arithmetic right shift instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>sar %cl, reg</c> in AT&amp;T syntax. Shifts <paramref name="reg"/> right by
    /// the number of bits specified in <c>%cl</c> (the low byte of <c>rcx</c>), filling
    /// vacated high bits with the sign bit of the original value. This preserves the sign
    /// of signed integers, making it appropriate for the <c>&gt;&gt;</c> operator.
    /// </para>
    /// <para>
    /// Contrast with <see cref="OpShiftRightLogical"/>, which always fills with zero and
    /// implements the unsigned <c>&gt;&gt;&gt;</c> operator.
    /// </para>
    /// <para>
    /// The caller must load the shift count into <c>rcx</c> before emitting this instruction.
    /// </para>
    /// </remarks>
    public class OpShiftRightArithmetic : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new arithmetic right shift operation.
        /// </summary>
        /// <param name="reg">The register to shift. The shift count is taken implicitly from <c>%cl</c>.</param>
        public OpShiftRightArithmetic(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>sar %cl, reg</c>.</returns>
        public override string ToString()
        {
            return $"sar %cl, {this.reg}";
        }
    }
}
