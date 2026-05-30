/*
 * File: OpCmpRegReg.cs
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
    /// Emits an x86-64 64-bit register-to-register compare instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Emits <c>cmpq right, left</c> in AT&amp;T syntax. Computes <c>left - right</c>
    /// and sets the CPU flags (Z, S, C, V) based on the result without storing it.
    /// The flags are then consumed by a subsequent conditional instruction such as
    /// <see cref="OpCmovCC"/> or <see cref="OpSetCC"/>.
    /// </para>
    /// <para>
    /// Note: operand order in AT&amp;T syntax is reversed from Intel syntax —
    /// the right operand is written first and the left operand second, but the
    /// comparison semantics are <c>left - right</c>.
    /// </para>
    /// </remarks>
    public class OpCmpRegReg : Op
    {
        IntRegister left;
        IntRegister right;

        /// <summary>
        /// Creates a new register-to-register compare operation.
        /// </summary>
        /// <param name="left">The left-hand side of the comparison (minuend).</param>
        /// <param name="right">The right-hand side of the comparison (subtrahend).</param>
        public OpCmpRegReg(IntRegister left, IntRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <remarks>
        /// Operand order is reversed relative to Intel syntax; the right operand is written first.
        /// </remarks>
        /// <returns>A string of the form <c>cmpq right, left</c>.</returns>
        public override string ToString()
        {
            //operand order is backwards
            return $"cmpq {this.right}, {this.left}";
        }
    }
}
