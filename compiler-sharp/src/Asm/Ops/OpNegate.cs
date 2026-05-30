/*
 * File: OpNegate.cs
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
    /// Emits an x86-64 64-bit integer two's complement negation instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>negq reg</c> in AT&amp;T syntax. Computes the two's complement negation
    /// of the register in-place (equivalent to <c>0 - reg</c>). This is the correct
    /// integer negation instruction; for floating-point negation, XOR bit 63 of the
    /// value's integer representation instead, as no dedicated <c>negsd</c> instruction exists.
    /// </remarks>
    public class OpNegate : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new integer negation operation.
        /// </summary>
        /// <param name="reg">The register to negate in-place.</param>
        public OpNegate(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>negq reg</c>.</returns>
        public override string ToString()
        {
            return $"negq {this.reg}";
        }
    }
}