/*
 * File: OpAdd.cs
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
    /// Emits an x86-64 64-bit integer addition instruction operating on two integer registers.
    /// </summary>
    /// <remarks>
    /// Emits <c>addq right, left</c> in AT&amp;T syntax. The result is stored in the
    /// left register.
    /// </remarks>
    public class OpAdd : Op
    {
        IntRegister left, right;

        /// <summary>
        /// Creates a new integer addition operation.
        /// </summary>
        /// <param name="left">The destination register; also the left operand. Receives the result.</param>
        /// <param name="right">The right operand register.</param>
        public OpAdd(IntRegister left, IntRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>addq right, left</c>.</returns>
        public override string ToString()
        {
            return $"addq {this.right}, {this.left}";
        }
    }
}
