/*
* File: Program.cs
* Author: Noah Hensley
* Course: ETEC4401
*
* AI Usage: Claude (Anthropic) was used to generate XML documentation comments
* for this file on the following dates:
*       January 14, 2026,
*       January 25, 2026
*/


/// <summary>
/// Entry point for the compiler that processes source files and generates
/// token streams, parse trees, type-check results, or variable declaration info
/// based on command-line options.
/// </summary>
/// <remarks>
/// <para>Supported command-line options (mode + format):</para>
/// <list type="bullet">
/// <item><description>--gen-tests -tok-json: Tokenizes input and writes a JSON token array to the outputs directory.</description></item>
/// <item><description>--gen-tests -tree-json: Parses input and writes a JSON expression tree to the outputs directory.</description></item>
/// <item><description>--gen-tests -tree-box: Parses input and writes a box-drawing text tree to the outputs directory.</description></item>
/// <item><description>--gen-tests -dotfile: Parses input and writes a Graphviz dotfile to the outputs directory.</description></item>
/// <item><description>--gen-tests -type-check: Parses and type-checks input, writing a JSON legal/illegal result to the outputs directory.</description></item>
/// <item><description>--run [format]: Same as --gen-tests but writes output to the console instead.</description></item>
/// </list>
/// <para>If no options are provided, defaults to --run -tree-box behavior (parse tree to console).</para>
/// </remarks>
public class Program 
{
    /// <summary>
    /// Main entry point that orchestrates the compiler workflow based on command-line arguments.
    /// </summary>
    /// <param name="args">
    /// Command-line arguments. When options are provided, the first argument is the mode
    /// (--gen-tests or --run), the second is the output format (-tok-json, -tree-json,
    /// -tree-box, -dotfile, or -type-check), followed by optional flags and input file paths.
    /// If no options are provided, all arguments are treated as input file paths with default
    /// --run -tree-box behavior.
    /// </param>
    /// <remarks>
    /// <para>Uses <see cref="Utils.parseOptions"/> to validate and extract options, then
    /// <see cref="Utils.handleOptions"/> to process files according to those options.</para>
    /// <para>When no options are provided, the program parses each input file and outputs a
    /// box-drawing parse tree to the console.</para>
    /// <para>The program exits with code 0 upon successful completion, or code 1 if a
    /// per-file error occurs during default execution.</para>
    /// </remarks>
    /// <exception cref="InvalidOption">
    /// Reported via <see cref="Utils.error"/> when a mode option is provided without a
    /// format option, or when an unrecognized option is supplied.
    /// </exception>
    public static void Main(string[] args)
    {
        try
        {
            ParsedOptions opts = new ParsedOptions();
            List<string> files = new List<string>();
            if (args.Count() > 1 && args[^1] != "out.asm")
            {
                opts = Utils.parseOptions(args);
                files = Utils.parseInputFilesOrDirectories(args, opts);
            }
            else
            {
                opts.mode = Options.Client;
                opts.fmt = Options.CompileAsm;
                files.AddRange(args);
                opts.optarg = files[^1];
                files.Remove(opts.optarg);
                Utils.handleOptions(files, opts);
            }     
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Environment.Exit(1);
        }

        Environment.Exit(0);
    }
}