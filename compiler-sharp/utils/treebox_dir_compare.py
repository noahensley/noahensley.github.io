#!/usr/bin/env python3

import os
import sys
import re
from pathlib import Path

# ------------------------------------------------------------
# Tree normalization
# ------------------------------------------------------------

def normalize_treebox(text: str) -> list[str]:
    """
    Normalize treebox output for semantic comparison.

    - Removes tree drawing prefixes entirely
    - Removes `parent :` lines
    - Strips whitespace
    """

    normalized = []

    for line in text.splitlines():
        line = line.rstrip()

        # Ignore parent fields
        if re.search(r'\bparent\s*:', line):
            continue

        # Remove ALL leading tree drawing characters and spacing
        # This removes any combination of:
        # | ├ └ ─ │ spaces etc.
        line = re.sub(r'^[\s\|\├\└\─\│]+', '', line)

        normalized.append(line)

    return normalized


# ------------------------------------------------------------
# File comparison
# ------------------------------------------------------------

def compare_files(file1: Path, file2: Path) -> list[str]:
    """
    Returns list of differences between two treebox files.
    Empty list means structurally equal.
    """

    with open(file1, "r", encoding="utf-8") as f:
        t1 = normalize_treebox(f.read())

    with open(file2, "r", encoding="utf-8") as f:
        t2 = normalize_treebox(f.read())

    diffs = []

    max_len = max(len(t1), len(t2))

    for i in range(max_len):
        line1 = t1[i] if i < len(t1) else "<EOF>"
        line2 = t2[i] if i < len(t2) else "<EOF>"

        if line1 != line2:
            diffs.append(
                f"  Line {i+1}:\n"
                f"    [A] {line1}\n"
                f"    [B] {line2}"
            )

    return diffs


# ------------------------------------------------------------
# Directory comparison
# ------------------------------------------------------------

def compare_directories(dir1: Path, dir2: Path):
    files1 = {f.name for f in dir1.iterdir() if f.is_file()}
    files2 = {f.name for f in dir2.iterdir() if f.is_file()}

    common = sorted(files1 & files2)
    only1 = sorted(files1 - files2)
    only2 = sorted(files2 - files1)

    print(f"\nComparing {len(common)} common files:\n")

    for filename in common:
        file1 = dir1 / filename
        file2 = dir2 / filename

        diffs = compare_files(file1, file2)

        if diffs:
            print("=" * 60)
            print(f"File: {filename}")
            print("=" * 60)
            for d in diffs:
                print(d)
            print()
        else:
            print(f"{filename}: OK")

    if only1:
        print("\nFiles only in first directory:")
        for f in only1:
            print(f"  {f}")

    if only2:
        print("\nFiles only in second directory:")
        for f in only2:
            print(f"  {f}")


# ------------------------------------------------------------
# Main
# ------------------------------------------------------------

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: treebox_dir_compare <dir1> <dir2>")
        sys.exit(1)

    dir1 = Path(sys.argv[1])
    dir2 = Path(sys.argv[2])

    if not dir1.is_dir() or not dir2.is_dir():
        print("Both arguments must be directories.")
        sys.exit(1)

    compare_directories(dir1, dir2)