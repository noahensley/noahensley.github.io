/*
 * File: OpAndRegConst.cs
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
    /// Emits an x86-64 bitwise AND instruction between a register and an immediate constant.
    /// </summary>
    /// <remarks>
    /// Emits <c>andq $value, reg</c> in AT&amp;T syntax. Used after <see cref="OpCmpSD"/>
    /// to mask the all-ones/all-zeros float comparison result down to a single bit,
    /// producing a clean 0 or 1 boolean integer.
    /// </remarks>
    public class OpAndRegConst : Op
    {
        IntRegister reg;
        long value;

        /// <summary>
        /// Creates a new register-and-immediate operation.
        /// </summary>
        /// <param name="reg">The register to AND in-place. Receives the result.</param>
        /// <param name="value">The immediate constant to AND against.</param>
        public OpAndRegConst(IntRegister reg, long value)
        {
            this.reg = reg;
            this.value = value;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>andq $value, reg</c>.</returns>
        public override string ToString()
        {
            return $"andq ${this.value}, {this.reg}";
        }
    }
}