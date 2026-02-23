/*
 * File: CompilerBridge.cs  (compiler-sharp-wasm)
 * Author: Noah Hensley
 *
 * Exposes the real compiler pipeline to JavaScript via [JSInvokable].
 * Each method takes source code as a string and returns output as a string,
 * mirroring what the CLI would print to stdout.
 *
 * JS usage after WASM loads:
 *   const tokJson  = await window.compiler.invokeMethodAsync("RunTokJson",  src);
 *   const treeBox  = await window.compiler.invokeMethodAsync("RunTreeBox",  src);
 *   const treeJson = await window.compiler.invokeMethodAsync("RunTreeJson", src);
 *   const dotfile  = await window.compiler.invokeMethodAsync("RunDotfile",  src);
 *   const typeChk  = await window.compiler.invokeMethodAsync("RunTypeCheck",src);
 */

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.JSInterop;

namespace CompilerSharpWasm;

public class CompilerBridge
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Tokenizer MakeTokenizer(string source)
    {
        var t = new Tokenizer();
        t.setInput(source);
        return t;
    }

    private static readonly System.Text.Json.JsonSerializerOptions JsonOpts = new()
    {
        IncludeFields  = true,
        WriteIndented  = true,
        MaxDepth       = 1_000_000,
        Encoder        = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private static string EscapeForJson(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "'");

    // ── JS-callable methods ───────────────────────────────────────────────────

    /// <summary>Tokenizes source and returns the token stream as JSON.</summary>
    [JSInvokable]
    public string RunTokJson(string source)
    {
        try
        {
            var tokenizer = MakeTokenizer(source);
            var tokens    = new List<Token>();
            while (true)
            {
                Token tok = tokenizer.next();
                if (tok.sym == TokenSymbols.EOF) break;
                tokens.Add(tok);
            }
            return System.Text.Json.JsonSerializer.Serialize(tokens, JsonOpts);
        }
        catch (Exception e)
        {
            return $"{{\"error\": \"{EscapeForJson(e.Message)}\"}}";
        }
    }

    /// <summary>Parses source and returns the box-drawn tree as a string.</summary>
    [JSInvokable]
    public string RunTreeBox(string source)
    {
        try
        {
            ExprNode.ResetCounter();
            var tokenizer = MakeTokenizer(source);
            var tree      = ProgramNode.parse(tokenizer);
            var sw        = new StringWriter();
            Treedump.textTree(tree, sw);
            return sw.ToString();
        }
        catch (Exception e)
        {
            return $"error: {e.Message}";
        }
    }

    /// <summary>Parses source and returns the first ExprNode as JSON.</summary>
    [JSInvokable]
    public string RunTreeJson(string source)
    {
        string result = "null";
        try
        {
            var tokenizer = MakeTokenizer(source);
            var tree      = ProgramNode.parse(tokenizer);
            WalkPreOrder(tree, node =>
            {
                if (node is ExprNode enode)
                {
                    result = enode.toJson();
                    throw new StopIteration();
                }
            });
        }
        catch (StopIteration) { /* normal early exit, result already set */ }
        catch (Exception e)
        {
            return $"{{\"error\": \"{EscapeForJson(e.Message)}\"}}";
        }
        return result;
    }

    /// <summary>Parses source and returns a Graphviz dotfile string.</summary>
    [JSInvokable]
    public string RunDotfile(string source)
    {
        var sb = new StringBuilder();
        try
        {
            ExprNode.ResetCounter();
            var tokenizer = MakeTokenizer(source);
            var tree      = ProgramNode.parse(tokenizer);

            if (tree.getChildren().Count == 0)
                return "null";

            WalkPreOrder(tree, node =>
            {
                if (node is not ExprNode enode) return;

                sb.AppendLine("graph program {");
                WalkPreOrder(enode, child =>
                {
                    var ec = (ExprNode)child;
                    sb.AppendLine($"  {ec.unique_id} [label=\"{ec.token.lexeme}\"]");
                });
                WalkPreOrder(enode, child =>
                {
                    var ec = (ExprNode)child;
                    foreach (var grandchild in ec.getChildren())
                        sb.AppendLine($"  {ec.unique_id} -- {((ExprNode)grandchild).unique_id}");
                });
                sb.AppendLine("}");
                throw new StopIteration();
            });
        }
        catch (StopIteration) { /* normal early exit */ }
        catch (Exception e)
        {
            return $"null /* error: {e.Message} */";
        }
        return sb.Length > 0 ? sb.ToString() : "null";
    }

    /// <summary>Runs the type checker and returns {"legal": true/false}.</summary>
    [JSInvokable]
    public string RunTypeCheck(string source)
    {
        try
        {
            var tokenizer = MakeTokenizer(source);
            var tree      = ProgramNode.parse(tokenizer);

            if (tree.getChildren().Count == 0)
                return "{\"legal\": false}";

            Utils.walkPostOrder(tree, n => n.setType());
            Utils.walkPostOrder(tree, n => n.typeCheck());
            return "{\"legal\": true}";
        }
        catch
        {
            return "{\"legal\": false}";
        }
    }

    // ── Tree walk ─────────────────────────────────────────────────────────────
    // Mirrors Utils.walk / Utils.walkHelper (which are private in the compiler).
    // StopIteration is re-thrown so callers can catch it for early exit.

    private static void WalkPreOrder(TreeNode node, Action<TreeNode> callback)
    {
        callback(node);
        foreach (var child in node.getChildren())
            WalkPreOrder(child, callback);
    }
}
