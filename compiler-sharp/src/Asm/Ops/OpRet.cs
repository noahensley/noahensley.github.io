/*
 * File: OpRet.cs
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
    /// Emits an x86-64 <c>ret</c> instruction to return from the current function.
    /// </summary>
    /// <remarks>
    /// Pops the return address from the stack and transfers control to the caller.
    /// Must be preceded by the function epilogue (restoring <c>%rsp</c> and <c>%rbp</c>)
    /// to ensure the stack is in the correct state.
    /// </remarks>
    public class OpRet : Op
    {
        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>The string <c>ret</c>.</returns>
        public override string ToString()
        {
            return "ret";
        }
    }
}
