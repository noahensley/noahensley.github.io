/*
 * File: OpFMul.cs
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
    /// Emits an x86-64 double-precision floating-point multiplication instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>mulsd right, left</c> in AT&amp;T syntax. Operates on XMM registers using
    /// the SSE2 <c>mulsd</c> instruction, which multiplies two 64-bit IEEE 754 double values.
    /// The result is stored in the left register.
    /// </remarks>
    public class OpFMul : Op
    {
        FloatRegister left, right;

        /// <summary>
        /// Creates a new double-precision floating-point multiplication operation.
        /// </summary>
        /// <param name="left">The destination XMM register; also the left operand. Receives the result.</param>
        /// <param name="right">The right operand XMM register.</param>
        public OpFMul(FloatRegister left, FloatRegister right)
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
        /// <returns>A string of the form <c>mulsd right, left</c>.</returns>
        public override string ToString()
        {
            //operand order is backwards
            return $"mulsd {this.right}, {this.left}";
        }
    }
}
