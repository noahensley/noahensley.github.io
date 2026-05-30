namespace ASM
{ 
    public class OpJmpCC : Op
    {
        string cc;
        Label target;
        public OpJmpCC(string cc, Label target)
        {
            this.cc=cc;
            this.target=target;
        }
        public override string ToString()
        {
            return $"j{cc} {this.target.Ref()}";
        }
    }
}