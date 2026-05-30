/*
 * File: Comment.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 *
 * AI Usage: Claude (Anthropic) was used to suggest the following changes
 * on April 12, 2026:
 *      Replaced comment delimiter with spaced comment delimiter in comment text to prevent embedded comment delimiter
 *      sequences from prematurely terminating the block comment in the
 *      assembler output
 *      Replaced newline characters with their escaped literal representations
 *      to keep each emitted comment on a single line
 */

namespace ASM
{
    public class Comment: Op
    {
        public readonly string comment;
        public Comment(string s)
        {
            this.comment=s;
        }
        public override string ToString()
        {
            // Sanitize the comment text before wrapping it in /* ... */
            // to prevent any embedded "*/" sequence from prematurely
            // terminating the block comment in the assembler output.
            // Without this, a STRINGCONST like "the /* text */\nint3 /*"
            // would emit a live `int3` instruction between two comments,
            // crashing the program with STATUS_BREAKPOINT (0x80000003).
            //
            // We also replace newlines so that the entire comment stays
            // on one logical line, which keeps the output readable and
            // avoids any assembler that treats /* */ as single-line only.
            string safe = this.comment
                .Replace("*/", "* /")
                .Replace("\r\n", "\\n")
                .Replace("\n",   "\\n")
                .Replace("\r",   "\\r");
            return $"/* {safe} */";
        }
    }
}