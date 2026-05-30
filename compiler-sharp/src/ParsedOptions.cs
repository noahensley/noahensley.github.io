public class ParsedOptions
{
    public string? mode;
    public string? fmt;
    public string? opt;
    public string? optarg;

    public ParsedOptions()
    {
        this.mode = null;
        this.fmt = null;
        this.opt = null;
        this.optarg = null;
    }
    public ParsedOptions(string mode, string fmt, string? opt, string? optarg)
    {
        this.mode = mode;
        this.fmt = fmt;
        this.opt = opt;
        this.optarg = optarg;
    }

    public string?[] Members()
    {
        return [mode, fmt, opt, optarg];
    }
    public int Length()
    {
        int l = 0;
        foreach (string? att in this.Members())
        {
            if (att != null)
                l++;
        }
        return l;
    }
}