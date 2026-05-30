namespace ASM
{
    public class OpAddRegConst : Op
    {
        IntRegister reg;
        int value;
        public OpAddRegConst( IntRegister reg, int value)
        {
            this.reg=reg;
            this.value=value;
        }
        public override string ToString()
        {
            return $"addq ${this.value}, {this.reg}";
        }
    }
}