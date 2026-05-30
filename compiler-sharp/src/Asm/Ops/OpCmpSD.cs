/*
 * File: OpCmpSD.cs
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
    /// Emits an x86-64 SSE2 double-precision floating-point compare instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>cmpCCsd right, left</c> in AT&amp;T syntax. Compares two 64-bit IEEE 754
    /// doubles and writes either all-ones (<c>0xFFFFFFFFFFFFFFFF</c>, true) or all-zeros
    /// (<c>0x0000000000000000</c>, false) into the left XMM register.
    /// </para>
    /// <para>
    /// Supported condition codes: <c>eq</c>, <c>lt</c>, <c>le</c>, <c>neq</c>,
    /// <c>nlt</c> (≥), <c>nle</c> (&gt;).
    /// </para>
    /// <para>
    /// The all-ones/all-zeros result must be transferred to an integer register via
    /// <see cref="OpMovRegReg"/> and masked to the lowest bit via <see cref="OpBitAnd"/>
    /// to produce a usable 0/1 boolean integer.
    /// </para>
    /// </remarks>
    public class OpCmpSD : Op
    {
        string cc;
        FloatRegister left;
        FloatRegister right;

        /// <summary>
        /// Creates a new double-precision floating-point compare operation.
        /// </summary>
        /// <param name="cc">The SSE2 comparison predicate suffix (e.g., <c>"lt"</c> for less-than).</param>
        /// <param name="left">The left XMM register; receives the all-ones or all-zeros result.</param>
        /// <param name="right">The right XMM register (comparison source).</param>
        public OpCmpSD(string cc, FloatRegister left, FloatRegister right)
        {
            this.cc = cc;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>cmpCCsd right, left</c>.</returns>
        public override string ToString()
        {
            return $"cmp{cc}sd {this.right}, {this.left}";
        }
    }
}
