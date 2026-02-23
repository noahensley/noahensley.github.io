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
/// Entry point for the parser program that processes source files and generates 
/// either token streams (JSON) or parse trees based on command-line options.
/// </summary>
/// <remarks>
/// <para>Supported command-line options:</para>
/// <list type="bullet">
/// <item><description>--gen-tests -json: Generates JSON token output to staging/outputs directory</description></item>
/// <item><description>--gen-tests -tree: Generates parse tree output to staging/outputs directory</description></item>
/// <item><description>--run -json: Outputs JSON tokens to console</description></item>
/// <item><description>--run -tree: Outputs parse tree to console</description></item>
/// </list>
/// <para>If no options are provided, defaults to --run -tree behavior.</para>
/// </remarks>
public class Program 
{
    /// <summary>
    /// Main entry point that orchestrates the parsing workflow based on command-line arguments.
    /// </summary>
    /// <param name="args">
    /// Command line arguments. First argument should be an option (--gen-tests or --run),
    /// second argument should be output format (-json or -tree), followed by input file path(s).
    /// If no options provided, remaining arguments are treated as input file paths with default --run -tree behavior.
    /// </param>
    /// <remarks>
    /// <para>The method uses Utils.parseOptions() to validate and extract options, then Utils.handleOptions() 
    /// to process files according to those options. Options are parsed and validated to ensure proper format.</para>
    /// <para>When no options are provided, the program defaults to parsing input files and outputting 
    /// parse trees to the console (equivalent to --run -tree).</para>
    /// <para>The program exits with code 0 upon successful completion.</para>
    /// </remarks>
    /// <exception cref="Exception">
    /// Thrown by Utils helper functions when invalid options are provided or when option 2 
    /// (-json or -tree) is missing after option 1 (--gen-tests or --run).
    /// </exception>
    public static void Main(string[] args)
    {
        List<string> opts = Utils.parseOptions(args);

        if (Utils.handleOptions(args, opts) == 0)
        {
            // no options, execute normally (tree)
            Tokenizer T = new Tokenizer();
            foreach (string input in args)
            {
                try
                {
                    using (StreamReader r = new StreamReader(input))
                        T.setInput(r.ReadToEnd());               
                    ProgramNode tree_data = ProgramNode.parse(T);
                    Treedump.textTree(tree_data, Console.Out);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    Environment.Exit(1);
                }
            }
        }  
        Environment.Exit(0);
    }
}