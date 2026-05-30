# compiler-sharp

A compiler for a statically typed, C-like language that targets x86-64 AT&T assembly. Written in C# as a senior capstone project at Shawnee State University (ETEC4401). This repository also hosts the portfolio site at [noahensley.github.io](https://noahensley.github.io), which runs the compiler live in the browser via Blazor WebAssembly.

---

## What it does

The compiler takes source code written in a custom typed language and produces x86-64 assembly output through a classical multi-pass pipeline:

1. **Lexer** — tokenizes source text using a hand-written regular expression matcher defined in `Terminals.cs`
2. **Parser** — builds a typed abstract syntax tree (AST) using recursive descent and a shunting-yard algorithm for expression parsing
3. **Type checker** — performs post-order tree walks to assign and verify types for all expressions and statements
4. **Code generator** — walks the AST to emit x86-64 AT&T assembly with a Windows x64 calling convention, including stack frame management, shadow space, temporaries, and function call sequences

Supported language features include `int`, `float`, and `bool` types; global and local variable declarations; function definitions with parameters and return values; `if`/`else if`/`else` conditionals; `while` loops; full arithmetic, bitwise, boolean, shift, and comparison operators; and string constants.

---

## Language overview

```
func add(x: int, y: int): int {
    return x + y
}

func main(): int {
    return add(3, 4)
}
```

The language uses `func` for function definitions, `:` for type annotations, and `var` for variable declarations. The `main` function serves as the program entry point and its return value becomes the process exit code.

---

## Project structure

```
compiler-sharp/              Source library — lexer, parser, type checker, codegen
  src/
    Terminals.cs             Token definitions and regex patterns
    Tokenizer.cs             Lexer
    Ast/                     AST node hierarchy (ExprNode, FuncdefNode, StmtsNode, etc.)
    Symbols/                 Symbol table, variable info, parameter info
    Asm/                     x86-64 opcode representations and assembly emitter
    Utils.cs                 Tree walk utilities and top-level pipeline driver
    Run.cs                   CLI entry point
  tests/
    unit/                    Lexer, parser, type checker, and treebox unit tests
    staging/                 Integration tests with expected ASM outputs

compiler-sharp-wasm/         Blazor WebAssembly portfolio site
  wwwroot/
    index.html               Portfolio page
    portfolio.js             JS interop — calls real C# compiler via window.compiler
  CompilerBridge.cs          JSInvokable methods exposed to JavaScript
  Program.cs                 Blazor host setup, registers CompilerBridge as window.compiler

compiler-sharp.sln           Solution file
global.json                  SDK version pin
```

---

## Running locally

**Dev server (portfolio site):**
```
cd compiler-sharp-wasm
dotnet run
```
Open `https://localhost:7188` in your browser. The compiler runs client-side in WebAssembly — no server processes requests.

**CLI compiler:**
```
cd compiler-sharp
dotnet run -- <input.txt> [options]
```

Available options:

| Flag | Output |
|---|---|
| `-tok-json` | Token stream as JSON |
| `-tree-box` | Parse tree (ASCII box-draw) |
| `-tree-json` | AST as JSON |
| `-dotfile` | Graphviz dot file of expression AST |
| `-type-check` | Type check result |
| `-comp-asm` | x86-64 AT&T assembly |

---

## Testing

Tests are driven by Python test harnesses in `compiler-sharp/tests/`. Unit tests cover the lexer (`tokjson`), parser (`treebox`, `treejson`), and type checker (`typecheck`) with over 1,275 type-check cases and 80+ staging integration tests for assembly output.

```
cd compiler-sharp
python testwrapper.py
```

---

## How the browser demo works

The portfolio site runs the real C# compiler entirely client-side:

1. Blazor WebAssembly boots a .NET 10 runtime in the browser
2. `Program.cs` registers a `CompilerBridge` instance as `window.compiler`
3. `portfolio.js` calls `window.compiler.invokeMethodAsync("RunAsmText", source)` on Run
4. `RunAsmText` invokes `Utils.CompileAsmToString`, which runs the full pipeline and returns the assembly as a string
5. The result is syntax-highlighted and displayed in the output panel

No requests leave the browser after the initial page load.

---

## Built with

- C# / .NET 10
- Blazor WebAssembly (`Microsoft.AspNetCore.Components.WebAssembly`)
- x86-64 AT&T assembly (Windows x64 calling convention)
- clang + lld-link (for native execution outside the browser)
