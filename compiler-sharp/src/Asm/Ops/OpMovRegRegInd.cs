/*
 * File: OpMoveRegRegInd.cs
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
    /// Emits an x86-64 register-to-memory move using register-indirect addressing.
    /// </summary>
    /// <remarks>
    /// Emits <c>movq src, offset(dst)</c> in AT&amp;T syntax. Writes the 64-bit value of
    /// <paramref name="src"/> to memory at the address computed as <c>dst + offset</c>.
    /// Typically used to spill temporaries to the stack frame using <c>rbp</c>-relative addressing.
    /// </remarks>
    public class OpMovRegRegInd : Op
    {
        Register src, dst;
        int offset;

        /// <summary>
        /// Creates a new register-to-memory store operation.
        /// </summary>
        /// <param name="src">The source register whose value will be stored.</param>
        /// <param name="dst">The base register providing the memory address.</param>
        /// <param name="offset">The signed byte offset added to <paramref name="dst"/> to form the effective address.</param>
        public OpMovRegRegInd(Register src, Register dst, int offset)
        {
            this.src = src;
            this.dst = dst;
            this.offset = offset;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>movq src, offset(dst)</c>.</returns>
        public override string ToString()
        {
            return $"movq {this.src}, {this.offset}({this.dst})";
        }
    }
}
