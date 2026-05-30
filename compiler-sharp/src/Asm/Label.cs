namespace ASM
{
    public class Label: Op
    {
        private static int _counter = 0;
        public readonly string lbl;
        public Label()
        {
            this.lbl = $"lbl{_counter++}";
        }

        public Label(string str)
        {
            this.lbl = str;
        }

        public static void resetLabelCounter()
        {
            _counter = 0;
        }
        public override string ToString()
        {
            return $"{lbl}:";
        }

        // OpenAI
        public string Ref()
        {
            return lbl;
        }
    }
}