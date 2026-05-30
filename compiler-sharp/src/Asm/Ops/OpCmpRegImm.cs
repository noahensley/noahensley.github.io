/*
 * File: OpCmpRegImm.cs
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
    /// Emits an x86-64 compare instruction between a register and an immediate value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>cmpq $value, reg</c> in AT&amp;T syntax. Sets CPU flags based on the result
    /// of <c>reg - value</c> without storing the result. The flags are then consumed by a
    /// subsequent conditional instruction such as <see cref="OpCmovCC"/>.
    /// </para>
    /// <para>
    /// Note: operand order in the emitted AT&amp;T syntax is reversed from Intel syntax —
    /// the immediate appears first and the register second.
    /// </para>
    /// </remarks>
    public class OpCmpRegImm : Op
    {
        IntRegister reg;
        int value;

        /// <summary>
        /// Creates a new register-vs-immediate comparison operation.
        /// </summary>
        /// <param name="reg">The register to compare against <paramref name="value"/>.</param>
        /// <param name="value">The immediate integer value to compare against.</param>
        public OpCmpRegImm(IntRegister reg, int value)
        {
            this.reg = reg;
            this.value = value;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <remarks>
        /// Operand order is reversed relative to Intel syntax; the immediate is written first.
        /// </remarks>
        /// <returns>A string of the form <c>cmpq $value, reg</c>.</returns>
        public override string ToString()
        {
            //operand order is backwards
            return $"cmpq ${this.value}, {this.reg}";
        }
    }
}
