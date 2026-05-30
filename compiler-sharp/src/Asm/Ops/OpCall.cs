namespace ASM
{
    public class OpCall : Op
    {
        Label target;
        public OpCall(Label target)
        {
            this.target=target;
        }
        public override string ToString()
        {
            return $"call {this.target.Ref()}";
        }
    }
}