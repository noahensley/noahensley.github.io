using ASM;

public static class StringPool
{
    public static Dictionary<string, Label> pool = new Dictionary<string, Label>();
    public static Label add(string s)
    {
        if (!pool.ContainsKey(s))
            pool[s] = new Label();
        return pool[s];
    }
}