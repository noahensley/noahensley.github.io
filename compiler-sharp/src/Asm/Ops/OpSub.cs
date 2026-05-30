/*
 * File: OpSub.cs
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
    /// Emits an x86-64 64-bit integer subtraction instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>subq right, left</c> in AT&amp;T syntax. Subtracts the right register from
    /// the left register and stores the result in the left register.
    /// </remarks>
    public class OpSub : Op
    {
        IntRegister left, right;

        /// <summary>
        /// Creates a new integer subtraction operation.
        /// </summary>
        /// <param name="left">The destination register holding the minuend. Receives the result.</param>
        /// <param name="right">The subtrahend register.</param>
        public OpSub(IntRegister left, IntRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>subq right, left</c>.</returns>
        public override string ToString()
        {
            return $"subq {this.right}, {this.left}";
        }
    }
}
