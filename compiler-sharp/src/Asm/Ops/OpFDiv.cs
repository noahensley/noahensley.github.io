/*
 * File: OpFDiv.cs
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
    /// Emits an x86-64 double-precision floating-point division instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>divsd right, left</c> in AT&amp;T syntax. Operates on XMM registers using
    /// the SSE2 <c>divsd</c> instruction, which divides two 64-bit IEEE 754 double values.
    /// The result is stored in the left register. Unlike integer division (<c>idivq</c>),
    /// this instruction requires no sign-extension setup and has no implicit register dependencies.
    /// </remarks>
    public class OpFDiv : Op
    {
        FloatRegister left, right;

        /// <summary>
        /// Creates a new double-precision floating-point division operation.
        /// </summary>
        /// <param name="left">The destination XMM register holding the dividend. Receives the result.</param>
        /// <param name="right">The divisor XMM register.</param>
        public OpFDiv(FloatRegister left, FloatRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>divsd right, left</c>.</returns>
        public override string ToString()
        {
            return $"divsd {this.right}, {this.left}";
        }
    }
}
