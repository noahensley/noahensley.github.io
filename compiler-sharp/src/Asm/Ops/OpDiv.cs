/*
 * File: OpDiv.cs
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
    /// Emits an x86-64 signed 64-bit integer division instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>idivq reg</c> in AT&amp;T syntax. Divides the 128-bit value in
    /// <c>%rdx:%rax</c> by <paramref name="reg"/>. After the instruction:
    /// </para>
    /// <list type="bullet">
    /// <item><c>%rax</c> holds the quotient.</item>
    /// <item><c>%rdx</c> holds the remainder.</item>
    /// </list>
    /// <para>
    /// <see cref="OpCQO"/> must be emitted immediately before this instruction to
    /// correctly sign-extend <c>%rax</c> into <c>%rdx:%rax</c>.
    /// </para>
    /// </remarks>
    public class OpDiv : Op
    {
        IntRegister reg;

        /// <summary>
        /// Creates a new signed integer division operation.
        /// </summary>
        /// <param name="reg">The divisor register. The dividend is taken implicitly from <c>%rdx:%rax</c>.</param>
        public OpDiv(IntRegister reg)
        {
            this.reg = reg;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>idivq reg</c>.</returns>
        public override string ToString()
        {
            return $"idivq {this.reg}";
        }
    }
}
