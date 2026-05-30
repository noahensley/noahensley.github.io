/*
 * File: OpMul.cs
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
    /// Emits an x86-64 signed 64-bit integer multiplication instruction.
    /// </summary>
    /// <remarks>
    /// Emits <c>imulq right, left</c> in AT&amp;T syntax. Uses the two-operand form of
    /// <c>imulq</c>, which multiplies two signed 64-bit registers and stores the lower
    /// 64 bits of the result in the left register. Unlike <see cref="OpDiv"/>, no
    /// sign-extension setup via <see cref="OpCQO"/> is required.
    /// </remarks>
    public class OpMul : Op
    {
        IntRegister left, right;

        /// <summary>
        /// Creates a new signed integer multiplication operation.
        /// </summary>
        /// <param name="left">The destination register; also the left operand. Receives the lower 64 bits of the result.</param>
        /// <param name="right">The right operand register.</param>
        public OpMul(IntRegister left, IntRegister right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>imulq right, left</c>.</returns>
        public override string ToString()
        {
            return $"imulq {this.right}, {this.left}";
        }
    }
}