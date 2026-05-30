/*
 * File: OpLea.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      March 28, 2026
 */
namespace ASM
{
    /// <summary>
    /// Emits an x86-64 load effective address instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>lea offset(src), dst</c> in AT&amp;T syntax. Used to compute the
    /// address of a stack-relative memory location, such as a local variable or parameter,
    /// without performing a memory access.
    /// </remarks>
    public class OpLea : Op
    {
        Register src;
        IntRegister dst;
        int offset;

        /// <summary>
        /// Creates a new load effective address operation.
        /// </summary>
        /// <param name="src">The base register used in the address calculation.</param>
        /// <param name="offset">The signed byte offset added to the base register.</param>
        /// <param name="dst">The destination register that receives the computed address.</param>
        public OpLea(Register src, int offset, IntRegister dst)
        {
            this.src = src;
            this.offset = offset;
            this.dst = dst;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>lea offset(src), dst</c>.</returns>
        public override string ToString()
        {
            return $"lea {this.offset}({this.src}), {this.dst}";
        }
    }
}