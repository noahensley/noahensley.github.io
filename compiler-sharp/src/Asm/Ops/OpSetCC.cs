/*
 * File: OpSetCC.cs
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
    /// Emits an x86-64 set-condition-code instruction that writes 0 or 1 into a register.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>setCC %al</c> followed by <c>movzxq %al, %rax</c> in AT&amp;T syntax.
    /// Sets the low byte of <c>%rax</c> to 1 if the condition is true, or 0 if false,
    /// based on the CPU flags set by a preceding <see cref="OpCmpRegReg"/> or
    /// <see cref="OpCmpSD"/> instruction. The <c>movzx</c> zero-extends the byte result
    /// to the full 64-bit register, clearing any garbage in the upper bytes.
    /// </para>
    /// <para>
    /// Common condition code suffixes: <c>e</c> (equal), <c>ne</c> (not equal),
    /// <c>g</c> (greater), <c>ge</c> (greater or equal), <c>l</c> (less),
    /// <c>le</c> (less or equal).
    /// </para>
    /// </remarks>
    public class OpSetCC : Op
    {
        string cc;

        /// <summary>
        /// Creates a new set-condition-code operation targeting <c>%al</c>/<c>%rax</c>.
        /// </summary>
        /// <param name="cc">The condition code suffix (e.g., <c>"g"</c> for greater-than).</param>
        public OpSetCC(string cc)
        {
            this.cc = cc;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction pair.
        /// </summary>
        /// <returns>
        /// Two instructions: <c>setCC %al</c> to write the condition result into the low byte,
        /// followed by <c>movzxq %al, %rax</c> to zero-extend it to 64 bits.
        /// </returns>
        public override string ToString()
        {
            return $"set{cc} %al\n\tmovzbq %al, %rax";
        }
    }
}
