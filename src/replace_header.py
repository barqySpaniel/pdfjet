#!/usr/bin/env python3
"""
Replace (or insert) a MIT header in every *.go file that lives in the
*current* directory (no sub‑folders).

Features
--------
* -d / --dry-run   – show what would be changed (no writes).
* -b / --backup    – create a <file>.bak backup before overwriting.
* Handles leading blank lines or a `package …` line before the comment.
* Ignores non‑UTF‑8 bytes gracefully.
"""

import argparse
import pathlib
import re
import shutil
import sys

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
# 3️⃣ Regex that finds the FIRST /** … */ block *near the top* of a file
# ----------------------------------------------------------------------
# • ^\s*               – optional leading whitespace / blank lines
# • (?:\/\/.*\n)*      – optional single‑line comments (e.g. //go:generate)
# • (/\*\*.*?\*/)      – the block comment we want
# • re.DOTALL + re.MULTILINE so '.' spans newlines and '^' works per line
HEADER_REGEX = re.compile(
    r'^\s*(?:\/\/.*\n)*(?P<header>/\*\*.*?\*/)',
    re.DOTALL | re.MULTILINE
)

def build_header(file_path: pathlib.Path) -> str:
    """Return the new header string for *file_path*, inserting its name."""
    filename = file_path.name          # e.g. mytool.go
    return HEADER_TEMPLATE.format(filename=filename)

def process_file(file_path: pathlib.Path) -> bool:
    """
    Replace (or insert) the header in *file_path*.

    Returns True if the file was (or would be) changed, False otherwise.
    """
    # Read the file, ignore undecodable bytes – they’re unlikely to be part of the header.
    original = file_path.read_text(encoding="utf-8", errors="ignore")

    new_header = build_header(file_path)

    # Try to locate an existing /** … */ block near the top.
    match = HEADER_REGEX.search(original)

    if match:
        # Replace the *found* block with the new header.
        start, end = match.span("header")
        updated = original[:start] + new_header + original[end:]
        action = "replaced"
    else:
        # No header found – prepend the new one (plus a newline for readability).
        updated = new_header + "\n\n" + original.lstrip("\ufeff")  # strip possible BOM
        action = "inserted"

    if args.dry_run:
        print(f"[DRY‑RUN] Would {action} header in: {file_path}")
        print("--- New header that would be written ---")
        print(new_header)
        print("--- End of preview --------------------\n")
        return True

    # If the file already contains the exact new header, skip rewriting.
    if original == updated:
        print(f"✅ Skipped (already up‑to‑date): {file_path}")
        return False

    if args.backup:
        backup_path = file_path.with_suffix(file_path.suffix + ".bak")
        shutil.copy2(file_path, backup_path)
        print(f"🔐 Backup created: {backup_path}")

    file_path.write_text(updated, encoding="utf-8")
    print(f"🛠️  {action.capitalize()} header in: {file_path}")
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
