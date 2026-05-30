/*
 * File: OpCQO.cs
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
    /// Emits an x86-64 <c>cqo</c> (convert quad to oct) instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sign-extends the 64-bit value in <c>%rax</c> into the 128-bit register pair
    /// <c>%rdx:%rax</c> by filling <c>%rdx</c> with the sign bit of <c>%rax</c>.
    /// </para>
    /// <para>
    /// This instruction must be emitted immediately before <see cref="OpDiv"/> to correctly
    /// set up the dividend for signed 64-bit division (<c>idivq</c>). Omitting it leaves
    /// <c>%rdx</c> undefined, which will produce incorrect quotients or a divide fault.
    /// </para>
    /// </remarks>
    public class OpCQO : Op
    {
        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>The string <c>cqo</c>.</returns>
        public override string ToString()
        {
            return $"cqo";
        }
    }
}
