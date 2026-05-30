/*
 * File: OpPopReg.cs
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
    /// Emits an x86-64 stack pop instruction into an integer register.
    /// </summary>
    /// <remarks>
    /// Emits <c>popq reg</c> in AT&amp;T syntax. Reads the 64-bit value at the top of the
    /// stack into <paramref name="reg"/> and increments <c>%rsp</c> by 8. Used in the
    /// function epilogue to restore the saved frame pointer.
    /// </remarks>
    public class OpPopReg : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new stack pop operation.
        /// </summary>
        /// <param name="reg">The destination register that receives the popped value.</param>
        public OpPopReg(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>popq reg</c>.</returns>
        public override string ToString()
        {
            return $"popq {this.reg}";
        }
    }
}
