/*
 * File: OpFAdd.cs
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
    /// Emits an x86-64 double-precision floating-point addition instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>addsd right, left</c> in AT&amp;T syntax. Operates on XMM registers using
    /// the SSE2 <c>addsd</c> instruction, which adds two 64-bit IEEE 754 double values.
    /// The result is stored in the left register. Use <see cref="OpFSub"/>, <see cref="OpFMul"/>,
    /// or <see cref="OpFDiv"/> for other floating-point arithmetic operations.
    /// </remarks>
    public class OpFAdd : Op
    {
        FloatRegister left, right;

        /// <summary>
        /// Creates a new double-precision floating-point addition operation.
        /// </summary>
        /// <param name="left">The destination XMM register; also the left operand. Receives the result.</param>
        /// <param name="right">The right operand XMM register.</param>
        public OpFAdd(FloatRegister left, FloatRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>addsd right, left</c>.</returns>
        public override string ToString()
        {
            return $"addsd {this.right}, {this.left}";
        }
    }
}
