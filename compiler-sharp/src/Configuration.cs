/*
 * File: Configuration.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to suggest the following changes
 * on April 6, 2026:
 *      Added a prerequisite step to compile runtime.c into runtime.o
 *      Added runtime.o as a fixed input to the linker command
 */

namespace Configuration{

    /// <summary>
    /// Provides build tool paths and command configurations for compiling and linking
    /// generated assembly output into an executable.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All commands are expressed as string arrays where the first element is the
    /// executable path and subsequent elements are arguments. The placeholder <c>{}</c>
    /// is substituted at build time with the relevant input file(s).
    /// </para>
    /// <para>
    /// Windows paths are active by default. Linux equivalents are provided in the
    /// commented block below and can be swapped in as needed.
    /// </para>
    /// </remarks>
    public static class Configuration {

        /// <summary>
        /// Command used to compile a single source file to an object file via clang.
        /// </summary>
        /// <remarks>
        /// The <c>{}</c> placeholder is replaced with the source file path at build time.
        /// The <c>-g</c> flag enables debug information.
        /// </remarks>
        public static readonly string[] clang = new string[] {
            @"c:\program files\llvm\bin\clang.exe","-g","-c","{}"
        };

        /// <summary>
        /// Commands executed before compiling generated assembly, in order.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The first step generates <c>kernel32.lib</c> from <c>kernel32.def</c> using
        /// llvm-dlltool, which is required to resolve Win32 API symbols at link time.
        /// </para>
        /// <para>
        /// The second step compiles <c>runtime.c</c> into <c>runtime.o</c>, which provides
        /// the runtime support functions (rtinit, rtcleanup, putc, putv, newline, getc)
        /// linked into every compiled program.
        /// </para>
        /// </remarks>
        public static readonly string[][] prerequisites = new string[][]{
            new string[]{@"c:\program files\llvm\bin\llvm-dlltool.exe",
                          "-m", "i386:x86-64",
                          "-d", "kernel32.def",
                          "-l", "kernel32.lib"
            },
            new string[]{@"c:\program files\llvm\bin\clang.exe",
                          "-g", "-c", "runtime.c", "-o", "runtime.o"
            }
        };

        /// <summary>
        /// Command used to link compiled object files into the final executable.
        /// </summary>
        /// <remarks>
        /// The <c>{}</c> placeholder is replaced with the compiled object file(s) from the
        /// current build. <c>runtime.o</c> and <c>kernel32.lib</c> are always appended as
        /// fixed inputs. The entry point is set to <c>_start</c>, which is emitted by the
        /// code generator and calls <c>rtinit</c>, <c>main</c>, <c>rtcleanup</c>, and
        /// <c>ExitProcess</c> in sequence.
        /// </remarks>
        public static readonly string[] linker = new string[]{
            @"c:\program files\llvm\bin\lld-link.exe", "/debug",
            "/entry:_start", "/subsystem:console", "/out:out.exe", "{}", "runtime.o", "kernel32.lib"
        };


        // Linux configuration (uncomment and replace the Windows block above to use)

        /*
        //Command to execute clang
        public static readonly string[] clang = new string[] {
            "clang","-g","-c","{}"
        };

        //Commands that need to be executed before compiling asm code
        public static readonly string[][] prerequisites = new string[][]{
            new string[]{"clang", "-g", "-c", "ExitProcess.c"}
        };

        //Command to link everything together
        public static readonly string[] linker = new string[]{
            "ld.lld", "-o", "out.exe", "{}", "ExitProcess.o"
        };
        */
    }

}