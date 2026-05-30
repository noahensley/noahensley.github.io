/*
 * File: OpSubRegConst.cs
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
    /// Emits an x86-64 register-minus-immediate subtraction instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>subq $value, reg</c> in AT&amp;T syntax. Subtracts the immediate constant
    /// from the register and stores the result back in the register. Used in the function
    /// prologue to reserve stack space for temporaries by decrementing <c>%rsp</c>.
    /// </remarks>
    public class OpSubRegConst : Op
    {
        IntRegister reg;
        int value;

        /// <summary>
        /// Creates a new register-minus-immediate subtraction operation.
        /// </summary>
        /// <param name="reg">The register to subtract from. Receives the result.</param>
        /// <param name="value">The immediate integer value to subtract.</param>
        public OpSubRegConst(IntRegister reg, int value)
        {
            this.reg = reg;
            this.value = value;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>subq $value, reg</c>.</returns>
        public override string ToString()
        {
            return $"subq ${this.value}, {this.reg}";
        }
    }
}
