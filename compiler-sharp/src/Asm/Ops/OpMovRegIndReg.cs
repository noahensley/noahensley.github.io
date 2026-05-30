/*
 * File: OpMoveRegIndReg.cs
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
    /// Emits an x86-64 memory-to-register move using register-indirect addressing.
    /// </summary>
    /// <remarks>
    /// Emits <c>movq offset(src), dst</c> in AT&amp;T syntax. Reads a 64-bit value from
    /// memory at the address computed as <c>src + offset</c> and places it in <paramref name="dst"/>.
    /// Typically used to load temporaries from the stack frame using <c>rbp</c>-relative addressing.
    /// </remarks>
    public class OpMovRegIndReg : Op
    {
        Register src, dst;
        int offset;

        /// <summary>
        /// Creates a new register-indirect load operation.
        /// </summary>
        /// <param name="src">The base register providing the memory address.</param>
        /// <param name="offset">The signed byte offset added to <paramref name="src"/> to form the effective address.</param>
        /// <param name="dst">The destination register that receives the loaded value.</param>
        public OpMovRegIndReg(Register src, int offset, Register dst)
        {
            this.src = src;
            this.offset = offset;
            this.dst = dst;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>movq offset(src), dst</c>.</returns>
        public override string ToString()
        {
            return $"movq {this.offset}({this.src}), {this.dst}";
        }
    }
}
