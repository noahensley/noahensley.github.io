#!/usr/bin/env python

ORIGINAL_HARNESS = ""
TIMEOUT_SECONDS = 30

import sys, os, getopt, subprocess, shutil, re, threading, queue

# ── terminal helpers ─────────────────────────────────────────

def supports_color():
    return hasattr(sys.stdout, "isatty") and sys.stdout.isatty()

GREEN      = "\033[32m" if supports_color() else ""
RED        = "\033[31m" if supports_color() else ""
YELLOW     = "\033[33m" if supports_color() else ""
CYAN       = "\033[36m" if supports_color() else ""
BOLD       = "\033[1m"  if supports_color() else ""
RESET      = "\033[0m"  if supports_color() else ""
ERASE_LINE = "\033[2K"  if supports_color() else ""
CURSOR_UP  = "\033[1A"  if supports_color() else ""

_display_initialized = False

def progress_bar(current, passed, failed, total=None, width=28):
    if total:
        filled = int(width * current / total)
        pct    = f"{int(100 * current / total):3d}%"
        right  = f"{current}/{total}"
        bar    = "█" * filled + "░" * (width - filled)
    else:
        filled = min(current, width)
        bar    = "█" * filled + "░" * (width - filled)
        pct    = "   "
        right  = f"{current}/?"
    counts = f"{GREEN}✓{passed}{RESET} {RED}✗{failed}{RESET}"
    return f"{CYAN}[{bar}]{RESET} {pct}  {right}  {counts}"

def redraw(test_line, bar_line):
    global _display_initialized
    if _display_initialized:
        sys.stdout.write(
            f"{CURSOR_UP}\r{ERASE_LINE}{test_line}\n"
            f"\r{ERASE_LINE}{bar_line}"
        )
    else:
        sys.stdout.write(f"{test_line}\n{bar_line}")
        _display_initialized = True
    sys.stdout.flush()

def finalize_display():
    global _display_initialized
    if _display_initialized:
        sys.stdout.write("\n")
        sys.stdout.flush()

def _python_exe():
    for candidate in ("python", "python3"):
        found = shutil.which(candidate)
        if found:
            return found
    return sys.executable

# ── argument parsing ─────────────────────────────────────────

opts, args = getopt.getopt(sys.argv[1:], "o:c:n:d:t:")
passthrough = []

for o, a in opts:
    if o in ("-o", "-n"):
        pass
    elif o == "-t":
        TIMEOUT_SECONDS = int(a)
    elif a:
        passthrough += [o, a]

_opt_dict = dict(opts)
if "-o" in _opt_dict:
    ORIGINAL_HARNESS = _opt_dict["-o"]

if not ORIGINAL_HARNESS:
    print("Usage: testharness.py -o <path/to/original_harness.py> [original args...]")
    sys.exit(1)

if not os.path.exists(ORIGINAL_HARNESS):
    print(f"Error: original harness not found at {ORIGINAL_HARNESS!r}")
    sys.exit(1)

# ── test counting ────────────────────────────────────────────

_inputdir = os.path.join(
    os.path.dirname(os.path.abspath(ORIGINAL_HARNESS)),
    "tests", "unit", "inputs"
)

total = sum(
    len([f for f in files if f.endswith(".txt")])
    for _, _, files in os.walk(_inputdir)
)

if total == 0:
    total = None

RE_TEST = re.compile(r"^Test\s+(\d+)\s*\(\s*(\S+)\s*/\s*(\S+)\s*\)")

# ── launch subprocess ────────────────────────────────────────

P = subprocess.Popen(
    [_python_exe(), "-u", ORIGINAL_HARNESS, "-n", "false"] + passthrough,
    stdout=subprocess.PIPE,
    stderr=subprocess.PIPE,
    encoding="utf-8",
    errors="ignore"
)

line_queue = queue.Queue()

def _reader():
    for line in P.stdout:
        line_queue.put(line)
    line_queue.put(None)

def _stderr_drain():
    P.stderr.read()

threading.Thread(target=_reader, daemon=True).start()
threading.Thread(target=_stderr_drain, daemon=True).start()

# ── state tracking ───────────────────────────────────────────

current = 0
passed = 0
failed = 0
label = ""
failures = []
timed_out = False

in_failure_block = False
failure_buffer = []

print(f"{BOLD}Running tests via {os.path.basename(ORIGINAL_HARNESS)}...{RESET}\n")

while True:
    try:
        raw_line = line_queue.get(timeout=TIMEOUT_SECONDS)
    except queue.Empty:
        timed_out = True
        P.kill()
        break

    if raw_line is None:
        break

    line = raw_line.rstrip()
    m = RE_TEST.match(line)

    # New test begins
    if m:
        # If previous test did not fail, it passed
        if current > 0 and not in_failure_block:
            passed += 1

        current = int(m.group(1))
        label = f"{m.group(2)}/{m.group(3)}"
        in_failure_block = False
        failure_buffer = []

        redraw(
            f"  {label[:50].ljust(50)}",
            progress_bar(current, passed, failed, total)
        )

    # Failure starts
    elif "Expected:" in line:
        if not in_failure_block:
            failed += 1
            failures.append(label)
            in_failure_block = True
            failure_buffer = []

        failure_buffer.append(line)

    # Continue capturing multiline failure
    elif in_failure_block:
        failure_buffer.append(line)

    # Harness success message at end
    elif "All required functionality OK" in line:
        pass

P.wait()
finalize_display()

# Final test may have passed
if current > 0 and not in_failure_block:
    passed += 1

# ── summary ─────────────────────────────────────────────────

print()

if timed_out:
    print(f"{BOLD}{RED}HUNG{RESET} — no output for {TIMEOUT_SECONDS}s on test {current} ({label})")
    print(f"  {GREEN}{passed} passed{RESET} before hang.")
elif failures:
    print(f"{BOLD}{RED}Failures:{RESET}")
    for lbl in failures:
        print(f"  {RED}✗ {lbl}{RESET}")
    print()
    print(f"{BOLD}Results: {GREEN}{passed} passed{RESET}, {RED}{failed} failed{RESET} / {current} total")
elif passed == current and current > 0:
    print(f"{BOLD}{GREEN}All {passed} test(s) passed! ✓{RESET}")
else:
    print(f"{YELLOW}Original harness exited without a recognizable result.{RESET}")

sys.exit(P.returncode)