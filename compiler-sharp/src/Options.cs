/*
 * File: Options.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: ChatGPT (OpenAI) was used to refactor and populate
 * option constants for this file on the following dates:
 *      January 26, 2026
 */

/// <summary>
/// Central repository for all supported Main program options.
/// </summary>
public static class Options
{
    // MODES

    /// <summary>
    /// Generates test output files to disk.
    /// </summary>
    public const string GenTests = "--gen-tests";

    /// <summary>
    /// Runs the compiler and writes output to the console.
    /// </summary>
    public const string Run = "--run";

    public const string Client = "--client";

    // OUTPUTS

    /// <summary>
    /// Output format: serializes the token stream as a JSON array.
    /// </summary>
    public const string TokJson = "-tok-json";

    /// <summary>
    /// Output format: serializes the token stream as a JSON tree.
    /// </summary>
    public const string TreeJson  = "-tree-json";

    /// <summary>
    /// Output format: serializes the token stream as a tree box drawing.
    /// </summary>
    public const string TreeBox   = "-tree-box";

    /// <summary>
    /// Output format: serializes the token stream as a Graphviz dotfile
    /// </summary>
    public const string Dotfile   = "-dotfile";

    /// <summary>
    /// Runs the compiler and performs type setting and checking.
    /// Output format: JSON array with a legal: true field or legal: false.
    /// </summary>
    public const string TypeCheck = "-type-check";

    /// <summary>
    /// Output format: line by line file variable declaration information or null.
    /// </summary>
    public const string VarDecl   = "-var-decl";

    /// <summary>
    /// Output format: line by line file member declaration information or null.
    /// </summary>
    public const string ClassDecl   = "-class-decl";


    public const string CompileAsm  = "-comp-asm";

    // OPTIONAL
    
    /// <summary>
    /// Allows the user to specify an output directory for generated test files 
    /// relative to the parent subdirectory (inside /tests).
    /// </summary>
    public const string OutDir    = "-dir";
}
