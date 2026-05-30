namespace ASM
{
    public class OpTest : Op
    {
        IntRegister left; 
        IntRegister right;
        public OpTest( IntRegister left, IntRegister right)
        {
            this.left=left;
            this.right=right;
        }
        public override string ToString()
        {
            //operand order is backwards
            return $"test {this.right}, {this.left}";
        }
    }
}