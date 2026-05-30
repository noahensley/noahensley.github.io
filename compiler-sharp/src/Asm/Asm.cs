/*
 * File: Asm.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: ChatGPT (OpenAI) was used to fix stack alignment and calling
 * convention issues in the assembly output on April 6, 2026:
 *      Ensured 16-byte stack alignment before all function calls
 *      Standardized call pattern to use 40 bytes (32 shadow + 8 alignment)
 *      Removed reliance on prior stack state for alignment correctness
 *      Fixed misalignment caused by push instructions before calls
 */

namespace ASM
{
    public static class Asm
    {
        public static List<Op> opcodes = new();

        public static void emit(List<Op> ops)
        {
            opcodes.AddRange(ops);
        }

        public static void emit(params Op[] ops)
        {
            opcodes.AddRange(ops);
        }

        public static void write(TextWriter outputfile, Label mainLabel)
        {
            outputfile.Write(@$"

                .section .text
                .global _start

                _start:
                andq $~0xf, %rsp       // align stack

                subq $32, %rsp         // shadow space
                call rtinit
                addq $32, %rsp

                subq $32, %rsp         // shadow space for main
                call {mainLabel.lbl}   // main returns in rax
                addq $32, %rsp

                pushq %rax             // save return value
                subq $40, %rsp         // shadow space for rtcleanup + align
                call rtcleanup
                addq $40, %rsp
                popq %rcx              // restore main return code

                subq $32, %rsp         // shadow space for ExitProcess
                call ExitProcess

                "
            );

            foreach (var op in opcodes)
            {
                outputfile.WriteLine(op);
            }

            // string pool and emptyString live in .text
            outputfile.WriteLine(".section .text");

            outputfile.WriteLine("emptyString:");
            outputfile.WriteLine("    .quad 0");

            foreach (var pair in StringPool.pool)
            {
                string s = pair.Key;
                Label lbl = pair.Value;

                // write out the label
                outputfile.WriteLine(lbl);

                // write out the string length
                outputfile.WriteLine($"    .quad {s.Length}");

                // write out the string data byte by byte
                foreach (char c in s)
                    outputfile.WriteLine($"    .byte {(int)c}");

                // pad data up to the next 8-byte boundary
                int pad = (8 - (s.Length % 8)) % 8;
                if (pad > 0)
                    outputfile.WriteLine($"    .zero {pad}");
            }

            outputfile.WriteLine(".section .data");
            // At start of data section:
            outputfile.WriteLine(".global dataStart");
            outputfile.WriteLine("dataStart:");
            foreach (var tmp in SymbolTable.getCurrentDecls())
            {
                var name = tmp.Key;
                var info = tmp.Value;
                if (info.type == VarType.StringConst)
                {
					GlobalLocation global = (info.location as GlobalLocation)!;
                    outputfile.WriteLine($"global{global.number}:");
                    outputfile.WriteLine(".quad emptyString");
                    outputfile.WriteLine($".quad {(int)StorageClass.STATIC}");
                }
            }
            // At end of data section:
            outputfile.WriteLine(".global dataEnd");
            outputfile.WriteLine("dataEnd:");

            outputfile.WriteLine(".section .bss");
            outputfile.WriteLine(".global bssStart");
            outputfile.WriteLine("bssStart:");
			foreach (var vi in SymbolTable.getGlobals())
			{
				if ((vi.Value.type as FuncType) != null)
					continue;
				if ((vi.Value.type as StringConstType) != null)
					continue;
				GlobalLocation? global = vi.Value.location as GlobalLocation;
				if (global != null)
					outputfile.WriteLine($"\nglobal{global.number}:\n\t.skip 16");
			}
            outputfile.WriteLine(".global bssEnd");
            outputfile.WriteLine("bssEnd:");
    	}
	}
}