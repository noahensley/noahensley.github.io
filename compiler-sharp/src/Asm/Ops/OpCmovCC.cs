/*
 * File: OpCmovCC.cs
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
    /// Emits an x86-64 conditional move instruction with a parameterized condition code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>cmovCC src, dst</c> in AT&amp;T syntax, where <c>CC</c> is a condition code suffix
    /// such as <c>g</c> (greater than), <c>e</c> (equal), or <c>ne</c> (not equal).
    /// </para>
    /// <para>
    /// The destination register is only written if the condition is true at the time of execution;
    /// otherwise the destination is left unchanged. This is used instead of conditional jumps to
    /// avoid branch misprediction penalties — for example, zeroing a shift result when the shift
    /// count exceeds 63.
    /// </para>
    /// <para>
    /// Note: valid Intel condition code suffixes include <c>g</c>, <c>l</c>, <c>e</c>, <c>ne</c>,
    /// <c>a</c>, and <c>b</c>. The suffix <c>gt</c> is not valid and will cause an assembler error.
    /// </para>
    /// </remarks>
    public class OpCmovCC : Op
    {
        string cc;
        IntRegister src;
        IntRegister dst;

        /// <summary>
        /// Creates a new conditional move instruction.
        /// </summary>
        /// <param name="cc">The condition code suffix (e.g., <c>"g"</c> for greater-than).</param>
        /// <param name="src">The source register moved into <paramref name="dst"/> if the condition is true.</param>
        /// <param name="dst">The destination register, written only if the condition holds.</param>
        public OpCmovCC(string cc, IntRegister src, IntRegister dst)
        {
            this.cc = cc;
            this.src = src;
            this.dst = dst;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>cmovCC src, dst</c>.</returns>
        public override string ToString()
        {
            return $"cmov{cc} {this.src}, {this.dst}";
        }
    }
}
