/*
 * File: OpPushReg.cs
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
    /// Emits an x86-64 stack push instruction from an integer register.
    /// </summary>
    /// <remarks>
    /// Emits <c>pushq reg</c> in AT&amp;T syntax. Decrements <c>%rsp</c> by 8 and writes
    /// the 64-bit value of <paramref name="reg"/> to the top of the stack. Used in the
    /// function prologue to save the caller's frame pointer.
    /// </remarks>
    public class OpPushReg : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new stack push operation.
        /// </summary>
        /// <param name="reg">The source register whose value will be pushed onto the stack.</param>
        public OpPushReg(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>pushq reg</c>.</returns>
        public override string ToString()
        {
            return $"pushq {this.reg}";
        }
    }

}
