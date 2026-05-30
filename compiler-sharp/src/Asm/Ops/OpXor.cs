/*
 * File: OpXor.cs
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
    /// Emits an x86-64 bitwise XOR instruction operating on two integer registers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>xor right, left</c> in AT&amp;T syntax. The result is stored in the left register.
    /// </para>
    /// <para>
    /// A common use is zeroing a register by XOR-ing it with itself (e.g., <c>xor %rdx, %rdx</c>),
    /// which is the idiom used before shift-overflow guards to prepare a zero value for
    /// <see cref="OpCmovCC"/>.
    /// </para>
    /// </remarks>
    public class OpXor : Op
    {
        IntRegister left, right;

        /// <summary>
        /// Creates a new bitwise XOR operation.
        /// </summary>
        /// <param name="left">The destination register; also the left operand. Receives the result.</param>
        /// <param name="right">The right operand register.</param>
        public OpXor(IntRegister left, IntRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <remarks>
        /// Note: operand order is reversed relative to Intel syntax (AT&amp;T convention).
        /// </remarks>
        /// <returns>A string of the form <c>xor right, left</c>.</returns>
        public override string ToString()
        {
            return $"xor {this.right}, {this.left}";
        }
    }
}
