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
    // Modes
    public const string GenTests  = "--gen-tests";
    public const string Run       = "--run";

    // Output formats
    public const string TokJson   = "-tok-json";

    public const string TreeJson  = "-tree-json";
    public const string TreeBox   = "-tree-box";
    public const string Dotfile   = "-dotfile";
    public const string TypeCheck = "-type-check";

    // Optional
    public const string OutDir    = "-dir";
}
