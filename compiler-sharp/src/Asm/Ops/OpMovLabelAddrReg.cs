/*
 * File: OpMovLabelAddrReg.cs
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
    /// Emits an x86-64 instruction that loads the absolute address of a label into an integer register.
    /// </summary>
    /// <remarks>
    /// Emits <c>movabsq $label, dst</c> in AT&amp;T syntax. Used to obtain the address
    /// of a global variable by its label.
    /// </remarks>
    public class OpMovLabelAddrReg : Op
    {
        Label lbl;
        IntRegister dst;

        /// <summary>
        /// Creates a new move-label-address-to-register operation.
        /// </summary>
        /// <param name="lbl">The label whose address will be loaded.</param>
        /// <param name="dst">The destination register that receives the address.</param>
        public OpMovLabelAddrReg(Label lbl, IntRegister dst)
        {
            this.lbl = lbl;
            this.dst = dst;
        }

        /// <summary>
        /// Returns the AT&amp;T syntax assembly string for this instruction.
        /// </summary>
        /// <returns>A string of the form <c>movabsq $label, dst</c>.</returns>
        public override string ToString()
        {
            return $"movabsq ${this.lbl.Ref()}, {this.dst}";
        }
    }
}