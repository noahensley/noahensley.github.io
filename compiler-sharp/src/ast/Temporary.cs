/*
 * File: Temporary.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      March 28, 2026
 */
using ASM;

/// <summary>
/// Represents a numbered temporary storage slot on the stack used to hold intermediate
/// expression results.
/// </summary>
public class Temporary : ResultLocation 
{
    /// <summary>The zero-based index of this temporary.</summary>
    public readonly int number;

    /// <summary>
    /// Creates a new temporary with the given index.
    /// </summary>
    /// <param name="number">The zero-based index of this temporary slot.</param>
    public Temporary(int number)
    {
        this.number = number;
    }

    /// <summary>
    /// Emits code to load this temporary's integer value from its stack slot into a register.
    /// </summary>
    /// <param name="reg">The destination integer register.</param>
    /// <param name="klass">The storage class of the value. Currently unused.</param>
    public void copyToRegister(IntRegister reg, IntRegister? klass)
    {
        int offset = -(this.number + 1) * 16;  // base offset

        // value at offset
        Asm.emit(new OpMovRegIndReg(
            offset: offset,
            src: Register.rbp,
            dst: reg
        ));

        // storage class at offset+8
        if (klass != null)
        {
            Asm.emit(new OpMovRegIndReg(
                src: Register.rbp,
                offset: offset + 8,
                dst: klass
            ));
        }
    }

    public void copyFromRegister(IntRegister reg, IntRegister klass)
    {
        Asm.emit(new OpMovRegRegInd(
            src: klass,
            offset: -(this.number + 1) * 16,      // storage class at lower address
            dst: Register.rbp
        ));
        Asm.emit(new OpMovRegRegInd(
            src: reg,
            offset: -(this.number + 1) * 16 + 8,  // value at higher address
            dst: Register.rbp
        ));
    }

    /// <summary>
    /// Emits code to store an integer register's value into this temporary's stack slot.
    /// </summary>
    /// <param name="reg">The source integer register.</param>
    /// <param name="klass">The storage class of the value. Currently unused.</param>
    public void copyFromRegister(IntRegister reg, StorageClass klass)
    {
        int offset = -(this.number + 1) * 16;

        // value at offset
        Asm.emit(new OpMovRegRegInd(src: reg, dst: Register.rbp, offset: offset));
        
        // storage class at offset+8
        Asm.emit(new OpMovConstReg((long)klass, Register.rax));
        Asm.emit(new OpMovRegRegInd(src: Register.rax, dst: Register.rbp, offset: offset + 8));
    }

    /// <summary>
    /// Emits code to load this temporary's float value from its stack slot into a register.
    /// </summary>
    /// <param name="reg">The destination float register.</param>
    /// <param name="klass">The storage class of the value. Currently unused.</param>
    public void copyToRegister(FloatRegister reg, StorageClass klass)
    {
        // bonus
        throw new NotImplementedException();
    }

    /// <summary>
    /// Emits code to store a float register's value into this temporary's stack slot.
    /// </summary>
    /// <param name="reg">The source float register.</param>
    /// <param name="klass">The storage class of the value. Currently unused.</param>
    public void copyFromRegister(FloatRegister reg, StorageClass klass)
    {
        // bonus
        throw new NotImplementedException();
    }
}