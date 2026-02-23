import sys
import os
from pathlib import Path
from natsort import natsorted

# TODO: Could be made to reset back to startnum when file ext changes
#   This would allow you to rename a series of .txt files AND .json in the
#   same execution.

STAGING_IN_OUT_DIR = Path(__file__).parent
# 1. file prefix (-n for none)
# 2. startnum
# 3. padding (-n for none)
if len(sys.argv) == 4:
    prefix = str(sys.argv[1])
    if prefix == "-n":
        prefix = ""
    startnum = sys.argv[2]
    pad = sys.argv[3]
else:
    print("Not enough arguments (prefix,startnum,pad).  Exiting...")

changed = 0
i = int(startnum)
target_files = os.listdir(STAGING_IN_OUT_DIR)
target_files = natsorted(target_files)
for file in target_files:
    str_file = str(file)
    if not str_file.endswith(".txt") and not str_file.endswith(".json"):
        continue
    cur_file = str_file.split(".")
    if pad == "-n":
        num = str(i)
    else:
        if not pad.isdigit():
            print("Invalid padding value.  Exiting...")
        num = str(i).zfill(int(pad))
    new_file = prefix + num + "." + cur_file[1]
    os.rename(str_file, new_file)
    changed += 1
    i += 1
print("Renamed:", changed, "files")
