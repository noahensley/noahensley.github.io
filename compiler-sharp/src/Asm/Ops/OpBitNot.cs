/*
 * File: OpBitNot.cs
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
    /// Emits an x86-64 bitwise NOT instruction operating on a single integer register.
    /// </summary>
    /// <remarks>
    /// Emits <c>notq operand</c> in AT&amp;T syntax. The instruction flips all bits of the
    /// operand register in-place. Unlike the binary operators, this instruction takes only
    /// one register and writes the result back to the same register.
    /// </remarks>
    public class OpBitNot : Op
    {
        IntRegister operand;

        /// <summary>
        /// Creates a new bitwise NOT operation.
        /// </summary>
        /// <param name="operand">The register whose bits will be flipped. Receives the result.</param>
        public OpBitNot(IntRegister operand)
        {
            this.operand = operand;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>notq operand</c>.</returns>
        public override string ToString()
        {
            return $"notq {this.operand}";
        }
    }
}
