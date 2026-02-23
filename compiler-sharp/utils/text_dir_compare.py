#!/usr/bin/env python3

#====================================================#
# Compares plain text files across two directories.
#
#   - Matches files by stem (name without extension)
#       so cross-extension comparisons are supported.
#
#   - UTF-8 encoding is used to support tree box
#       characters and other unicode content.
#
#   - Only differing sections are shown, labeled
#       clearly by which file they belong to.
#
# Sources: Anthropic AI (Claude)
#====================================================#

import sys
import difflib
from pathlib import Path


def read_text_file(filepath):
    """Read a text file with UTF-8 encoding."""
    try:
        with open(filepath, 'r', encoding='utf-8-sig') as f:
            return f.readlines()
    except UnicodeDecodeError as e:
        return f"UTF-8 decode error: {e}"
    except Exception as e:
        return f"Error reading file: {e}"


def compare_text_files(file1, file2):
    """Compare two text files and return grouped difference blocks."""
    lines1 = read_text_file(file1)
    lines2 = read_text_file(file2)

    if isinstance(lines1, str):
        return lines1  # error string
    if isinstance(lines2, str):
        return lines2  # error string

    if lines1 == lines2:
        return None  # identical

    matcher = difflib.SequenceMatcher(None, lines1, lines2)
    blocks = []

    for tag, i1, i2, j1, j2 in matcher.get_opcodes():
        if tag == 'equal':
            continue  # skip identical lines

        block = {'line_num_1': i1 + 1, 'line_num_2': j1 + 1}

        if tag in ('replace', 'delete'):
            block['removed'] = [l.rstrip('\n') for l in lines1[i1:i2]]
        else:
            block['removed'] = []

        if tag in ('replace', 'insert'):
            block['added'] = [l.rstrip('\n') for l in lines2[j1:j2]]
        else:
            block['added'] = []

        blocks.append(block)

    return blocks if blocks else None


def format_diff(diff, file1, file2):
    """Format grouped difference blocks with clear file attribution."""
    if isinstance(diff, str):
        return diff  # error string

    name1 = Path(file1).name
    name2 = Path(file2).name
    output = []

    for i, block in enumerate(diff, 1):
        output.append(f"  Difference {i}:")

        if block['removed']:
            output.append(f"    [ {name1} ]  line {block['line_num_1']}:")
            for line in block['removed']:
                output.append(f"      {line}")

        if block['added']:
            output.append(f"    [ {name2} ]  line {block['line_num_2']}:")
            for line in block['added']:
                output.append(f"      {line}")

    return '\n'.join(output)


def compare_directories(dir1, dir2):
    """Compare all text files between two directories by stem."""
    dir1_path = Path(dir1)
    dir2_path = Path(dir2)

    if not dir1_path.is_dir():
        print(f"ERROR: '{dir1}' is not a valid directory.")
        sys.exit(1)
    if not dir2_path.is_dir():
        print(f"ERROR: '{dir2}' is not a valid directory.")
        sys.exit(1)

    # Key files by stem so different extensions can be matched
    files1 = {f.stem: f for f in dir1_path.iterdir() if f.is_file()}
    files2 = {f.stem: f for f in dir2_path.iterdir() if f.is_file()}

    common_files = set(files1.keys()) & set(files2.keys())
    only_in_dir1 = set(files1.keys()) - set(files2.keys())
    only_in_dir2 = set(files2.keys()) - set(files1.keys())

    if only_in_dir1:
        print(f"\nFiles only in {dir1}:")
        for stem in sorted(only_in_dir1):
            print(f"  - {files1[stem].name}")

    if only_in_dir2:
        print(f"\nFiles only in {dir2}:")
        for stem in sorted(only_in_dir2):
            print(f"  - {files2[stem].name}")

    print(f"\nComparing {len(common_files)} common files:\n")

    different_files = []

    for stem in sorted(common_files):
        f1, f2 = files1[stem], files2[stem]
        diff = compare_text_files(f1, f2)

        if diff is not None:
            different_files.append(stem)
            print(f"{'='*60}")
            print(f"File: {f1.name}  <->  {f2.name}")
            print(f"{'='*60}")
            print(format_diff(diff, f1, f2))
            print()

    if different_files:
        print(f"\nTotal files with differences: {len(different_files)}")
        for stem in different_files:
            print(f"  - {stem}")
        return True, different_files[0]
    else:
        print("\nAll files are identical.")
        return False, None


if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python text_dir_compare.py Dir1 Dir2")
        sys.exit(1)

    found_diff, first_diff = compare_directories(sys.argv[1], sys.argv[2])

    if found_diff:
        print(f"\nRESULT: Differences found, first in '{first_diff}'")
        sys.exit(1)

    print("RESULT: ALL FILES IDENTICAL")