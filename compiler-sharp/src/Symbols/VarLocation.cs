/*
 * File: VarLocation.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      February 23, 2026
 */

using ASM;
/// <summary>
/// Abstract base class representing the storage location of a declared variable.
/// </summary>
/// <remarks>
/// VarLocation encodes where a variable lives at runtime. Subclasses distinguish
/// between the different storage categories the code generator must handle when
/// emitting load and store instructions.
/// </remarks>
public abstract class VarLocation
{
    public abstract void copyAddressToRegister(IntRegister reg);
}

/// <summary>
/// Indicates that a variable is stored in global (static) memory.
/// </summary>
/// <remarks>
/// Global variables are declared at program scope and accessible from any function.
/// The code generator uses this location to emit global load/store instructions.
/// </remarks>
public class GlobalLocation : VarLocation
{
    private static int _counter = 0;
    public int number;
    public Label lbl = new Label();
    public GlobalLocation()
    {
        this.number = _counter++;
    }

    public static void resetCounter()
    {
        _counter = 0;
    }

    public override void copyAddressToRegister(IntRegister reg)
    {
        //example output: mov $global0, %rax
        Asm.emit(
            new Comment("creating global label"),
            new OpMovLabelAddrReg(
                new Label($"global{this.number}"),
                reg
            )
        );
    }
}

/// <summary>
/// Indicates that a variable is stored on the call stack as a local variable.
/// </summary>
/// <remarks>
/// Local variables are declared within a function body and have a lifetime limited
/// to that function's activation. The code generator uses this location to emit
/// stack-relative load/store instructions.
/// </remarks>
public class LocalLocation : VarLocation
{
    private static int _counter = 0;

    int number;

    readonly FuncdefNode declarer;

    public LocalLocation(FuncdefNode fnode)
    {
        this.number = _counter++;
        this.declarer = fnode;
    }

    public static void resetCounter()
    {
        _counter = 0;
    }

    public int getNumber()
    {
        return number;
    }

    public override void copyAddressToRegister(IntRegister reg)
    {
        //i.e. local j lives at rbp - (maxTemporaries + j + 1) * 16
        int offset = -(this.declarer.maxTemporaries + this.number + 1) * 16;
        Asm.emit(new OpLea(offset: offset, src: Register.rbp, dst: reg));
    }
}

/// <summary>
/// Indicates that a variable is a function parameter.
/// </summary>
/// <remarks>
/// Parameters are passed by the caller and accessible within the function body.
/// The code generator uses this location to emit parameter-relative load instructions.
/// </remarks>
public class ParameterLocation : VarLocation
{
    private static int _counter = 0;

    int number;

    public ParameterLocation()
    {
        this.number = _counter++;
    }

    public static void resetCounter()
    {
        _counter = 0;
    }

    public override void copyAddressToRegister(IntRegister reg)
    {
        int offset = 16 * (this.number + 1);
        Asm.emit(new OpLea(offset: offset, src: Register.rbp, dst: reg));
    }
}

/// <summary>
/// Indicates that a variable is a member of a composite type or object.
/// </summary>
/// <remarks>
/// Member variables are accessed relative to a base address of the containing structure.
/// The code generator uses this location to emit member-relative load/store instructions.
/// </remarks>
public class MemberLocation : VarLocation
{
    private static int _counter = 0;

    int number;

    public MemberLocation()
    {
        this.number = _counter++;
    }

    public override void copyAddressToRegister(IntRegister reg)
    {
        throw new NotImplementedException();
    }
}