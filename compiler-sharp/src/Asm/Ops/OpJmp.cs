namespace ASM
{  
    public class OpJmp : Op
    {
        Label target;
        public OpJmp(Label target)
        {
            this.target=target;
        }
        public override string ToString()
        {
            return $"jmp {this.target.Ref()}";
        }
    }
}