import os
from pathlib import Path

REMOVE_EXTS = ["o", "asm", "exe", "pdb"]
TARGET_DIR = Path(__file__).parent.parent
count = 0

for file in os.listdir(TARGET_DIR):
    if os.path.isfile(file) and "." in file:
        if file.split(".")[-1] in REMOVE_EXTS:
            os.remove(Path.joinpath(TARGET_DIR, file))
            count += 1
print(f"Removed {count} files from {TARGET_DIR}")