#!/usr/bin/env python3

#====================================================#
# Developed to ensure the validity of new test files.
#
#   - Old test files do not include "$$" EOS token.
#
#   - New test files must be the same as old EXCEPT
#       for the introduction of "$$" EOS entries.
#
# Sources: Anthropic AI (Claude)
#====================================================#

import json
import sys
from pathlib import Path
from deepdiff import DeepDiff


def compare_json_files(file1, file2):
    """Compare two JSON files and return meaningful differences."""
    try:
        with open(file1, 'r') as f1, open(file2, 'r') as f2:
            data1 = json.load(f1)
            data2 = json.load(f2)
        
        diff = DeepDiff(data1, data2, ignore_order=True)
        return diff
    except json.JSONDecodeError as e:
        return f"JSON parse error: {e}"
    except Exception as e:
        return f"Error: {e}"


def format_value(value, max_len=80):
    """Format a value for display, truncating if too long."""
    if isinstance(value, dict):
        s = json.dumps(value, separators=(',', ':'))
        if len(s) > max_len:
            return s[:max_len] + "...}"
        return s
    elif isinstance(value, str):
        return f'"{value}"'
    else:
        s = str(value)
        if len(s) > max_len:
            return s[:max_len] + "..."
        return s


def summarize_diff(diff):
    """Create a concise summary of differences."""
    if isinstance(diff, str):
        return diff
    
    summary = []
    
    if 'values_changed' in diff:
        summary.append("Values changed:")
        for path, change in diff['values_changed'].items():
            old_val = format_value(change.get('old_value', 'N/A'))
            new_val = format_value(change.get('new_value', 'N/A'))
            summary.append(f"  {path}")
            summary.append(f"    OLD: {old_val}")
            summary.append(f"    NEW: {new_val}")
    
    if 'dictionary_item_added' in diff:
        summary.append("Items added:")
        for item in diff['dictionary_item_added']:
            summary.append(f"  {item}")
    
    if 'dictionary_item_removed' in diff:
        summary.append("Items removed:")
        for item in diff['dictionary_item_removed']:
            summary.append(f"  {item}")
    
    if 'iterable_item_added' in diff:
        summary.append("List items added:")
        for path, value in diff['iterable_item_added'].items():
            summary.append(f"  {path}: {format_value(value)}")
    
    if 'iterable_item_removed' in diff:
        summary.append("List items removed:")
        for path, value in diff['iterable_item_removed'].items():
            summary.append(f"  {path}: {format_value(value)}")
    
    return '\n'.join(summary) if summary else "Unknown difference type"
    

def compare_directories(dir1, dir2):
    """Compare all JSON files between two directories."""
    dir1_path = Path(dir1)
    dir2_path = Path(dir2)
    non_eos_diff = (False,None)
    
    # Get all files from dir1
    files1 = {f.stem: f for f in dir1_path.iterdir() if f.is_file()}
    files2 = {f.stem: f for f in dir2_path.iterdir() if f.is_file()}
    
    # Files in both directories
    common_files = set(files1.keys()) & set(files2.keys())
    
    # Files only in one directory
    only_in_dir1 = set(files1.keys()) - set(files2.keys())
    only_in_dir2 = set(files2.keys()) - set(files1.keys())
    
    # Report files only in one directory
    if only_in_dir1:
        print(f"\nFiles only in {dir1}:")
        for f in sorted(only_in_dir1):
            print(f"  - {f}")
    
    if only_in_dir2:
        print(f"\nFiles only in {dir2}:")
        for f in sorted(only_in_dir2):
            print(f"  - {f}")
    
    # Compare common files
    print(f"\nComparing {len(common_files)} common files:\n")
    
    different_files = 0
    for filename in sorted(common_files):
        diff = compare_json_files(files1[filename], files2[filename])
        
        if diff:
            has_non_eos = "$$" not in str(diff)
            if has_non_eos:
                non_eos_diff = (True, filename)
                different_files += 1
                print(f"{'='*60}")
                print(f"File: {filename}")
                print(f"{'='*60}")
                print(summarize_diff(diff))
                print()
                print("ERROR: Non-EOS diff found.\n")

    if different_files > 0:
        print(f"\nTotal files with non-EOS differences: {different_files}")
    else:
        print("\nAll differences are EOS-only (as expected)")
    
    return non_eos_diff

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python json_compare.py Dir1 Dir2")
        sys.exit(1)
    
    if (res := compare_directories(sys.argv[1], sys.argv[2]))[0]:
        print(f"RESULT: Fatal diff in {res[1]}")
        sys.exit(1)

    print(f"RESULT: ALL FILES GOOD")        