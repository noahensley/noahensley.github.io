/*
 * File: OpFSub.cs
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
    /// Emits an x86-64 double-precision floating-point subtraction instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>subsd right, left</c> in AT&amp;T syntax. Operates on XMM registers using
    /// the SSE2 <c>subsd</c> instruction, which subtracts the right operand from the left.
    /// The result is stored in the left register.
    /// </remarks>
    public class OpFSub : Op
    {
        FloatRegister left, right;

        /// <summary>
        /// Creates a new double-precision floating-point subtraction operation.
        /// </summary>
        /// <param name="left">The destination XMM register holding the minuend. Receives the result.</param>
        /// <param name="right">The subtrahend XMM register.</param>
        public OpFSub(FloatRegister left, FloatRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>subsd right, left</c>.</returns>
        public override string ToString()
        {
            return $"subsd {this.right}, {this.left}";
        }
    }
}
