#!/usr/bin/env python3
"""
Replace the first /** … */ comment block at the top of every *.go file
in the *current* directory (no sub‑folders) with a new MIT header that
contains the file’s own name.

Options
-------
-d / --dry-run   Show what would be changed without writing files.
-b / --backup    Create a <file>.bak backup before overwriting.
"""

import argparse
import pathlib
import re
import shutil

# ----------------------------------------------------------------------
# 1️⃣ CLI arguments
# ----------------------------------------------------------------------
parser = argparse.ArgumentParser(
    description="Replace top‑of‑file headers in *.go files (non‑recursive)."
)
parser.add_argument("-d", "--dry-run", action="store_true",
                    help="Show what would be changed without actually writing files.")
parser.add_argument("-b", "--backup", action="store_true",
                    help="Create a .bak backup before overwriting each file.")
args = parser.parse_args()

# ----------------------------------------------------------------------
# 2️⃣ Header template (placeholder for the file name)
# ----------------------------------------------------------------------
HEADER_TEMPLATE = """/**
 * {filename}
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */"""

# ----------------------------------------------------------------------
# 3️⃣ Regex that matches the *first* /** … */ block at the very start of a file
# ----------------------------------------------------------------------
HEADER_REGEX = re.compile(r"\A/\*\*.*?\*/", re.DOTALL)


def build_header(file_path: pathlib.Path) -> str:
    """Return the new header string for *file_path*, inserting its name."""
    filename = file_path.name          # e.g. mytool.go
    return HEADER_TEMPLATE.format(filename=filename)


def process_file(file_path: pathlib.Path) -> bool:
    """Replace the old top‑of‑file header with the new one."""
    try:
        original = file_path.read_text(encoding="utf-8")
    except UnicodeDecodeError:
        # Skip binary or non‑UTF‑8 files silently
        return False

    new_header = build_header(file_path)

    # Substitute only the first match at the very beginning.
    updated, count = HEADER_REGEX.subn(new_header, original, count=1)

    if count == 0:
        # No /** … */ block at the top – nothing to do.
        return False

    if args.dry_run:
        print(f"[DRY‑RUN] Would update: {file_path}")
        return True

    if args.backup:
        shutil.copy2(file_path, file_path.with_suffix(file_path.suffix + ".bak"))

    file_path.write_text(updated, encoding="utf-8")
    print(f"Updated: {file_path}")
    return True


def main() -> None:
    cwd = pathlib.Path(".")
    # Non‑recursive: only files directly inside the current directory
    for p in cwd.iterdir():
        if not p.is_file():
            continue
        if p.suffix != ".go":
            continue
        process_file(p)


if __name__ == "__main__":
    main()
