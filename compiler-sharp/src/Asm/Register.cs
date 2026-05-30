namespace ASM
{
    public abstract class Register
    {
        public static readonly IntRegister rax = new IntRegister("rax");
        public static readonly IntRegister rbx = new IntRegister("rbx");
        public static readonly IntRegister rcx = new IntRegister("rcx");
        public static readonly IntRegister rdx = new IntRegister("rdx");
        public static readonly IntRegister rsi = new IntRegister("rsi");
        public static readonly IntRegister rdi = new IntRegister("rdi");
        public static readonly IntRegister rsp = new IntRegister("rsp");
        public static readonly IntRegister rbp = new IntRegister("rbp");
        public static readonly IntRegister r8  = new IntRegister("r8" );
        public static readonly IntRegister r9  = new IntRegister("r9" );
        public static readonly IntRegister r10 = new IntRegister("r10");
        public static readonly IntRegister r11 = new IntRegister("r11");
        public static readonly IntRegister r12 = new IntRegister("r12");
        public static readonly IntRegister r13 = new IntRegister("r13");
        public static readonly IntRegister r14 = new IntRegister("r14");
        public static readonly IntRegister r15 = new IntRegister("r15");

        public static readonly FloatRegister xmm0 =  new FloatRegister(0);
        public static readonly FloatRegister xmm1 =  new FloatRegister(1);
        public static readonly FloatRegister xmm2 =  new FloatRegister(2);
        public static readonly FloatRegister xmm3 =  new FloatRegister(3);
        public static readonly FloatRegister xmm4 =  new FloatRegister(4);
        public static readonly FloatRegister xmm5 =  new FloatRegister(5);
        public static readonly FloatRegister xmm6 =  new FloatRegister(6);
        public static readonly FloatRegister xmm7 =  new FloatRegister(7);
        public static readonly FloatRegister xmm8 =  new FloatRegister(8);
        public static readonly FloatRegister xmm9 =  new FloatRegister(9);
        public static readonly FloatRegister xmm10 = new FloatRegister(10);
        public static readonly FloatRegister xmm11 = new FloatRegister(11);
        public static readonly FloatRegister xmm12 = new FloatRegister(12);
        public static readonly FloatRegister xmm13 = new FloatRegister(13);
        public static readonly FloatRegister xmm14 = new FloatRegister(14);
        public static readonly FloatRegister xmm15 = new FloatRegister(15);
        
        public readonly string name;
        protected Register(string name){ this.name=name; }
        public override string ToString(){
            return "%"+name;
        }
    }

    public class IntRegister : Register
    {
        public IntRegister(string name): base(name)
        {
        }
    }

    public class FloatRegister : Register
    {
        public FloatRegister(int num): base($"xmm{num}"){}
    }

}