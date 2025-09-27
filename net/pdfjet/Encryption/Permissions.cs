/**
 * Permissions.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
    /// <summary>
    /// Represents the user access permissions for an encrypted PDF document as defined in
    /// ISO 32000-2 (PDF 2.0) Table 22. Permissions are stored as flags in a 32-bit integer.
    /// </summary>
    [Flags]
    public enum UserAccess {
        /// <summary>
        /// No permissions are granted. This is the default state.
        /// Reserved bits (0-2, 13-31) must be zero.
        /// </summary>
        None = 0,

        /// <summary>
        /// Permission to print the document (possibly not at the highest quality level,
        /// depending on whether <see cref="PrintHighQuality"/> is also set).
        /// (Bit position: 3)
        /// </summary>
        Print = 1 << 3, // Decimal: 8

        /// <summary>
        /// Permission to modify the contents of the document by operations other than
        /// those controlled by <see cref="ModifyAnnotations"/>, <see cref="FillFormFields"/>,
        /// and <see cref="AssembleDocument"/>.
        /// (Bit position: 4)
        /// </summary>
        ModifyContents = 1 << 4, // Decimal: 16

        /// <summary>
        /// Permission to copy or otherwise extract text and graphics from the document,
        /// including for accessibility purposes.
        /// (Bit position: 5)
        /// </summary>
        CopyContents = 1 << 5, // Decimal: 32

        /// <summary>
        /// Permission to add, modify, or delete text annotations and interactive form fields.
        /// Note: This permission is not used in PDF 2.0 but is retained for legacy support.
        /// (Bit position: 6)
        /// </summary>
        ModifyAnnotations = 1 << 6, // Decimal: 64

        /// <summary>
        /// Permission to fill existing interactive form fields (including signature fields),
        /// even if <see cref="ModifyContents"/> is not set.
        /// (Bit position: 9)
        /// </summary>
        FillFormFields = 1 << 9, // Decimal: 512

        /// <summary>
        /// Permission to extract text and graphics (in support of accessibility to
        /// users with disabilities or for other purposes).
        /// (Bit position: 10)
        /// </summary>
        ExtractContentsForAccessibility = 1 << 10, // Decimal: 1024

        /// <summary>
        /// Permission to assemble the document: insert, rotate, or delete pages and
        /// create bookmarks or thumbnail images.
        /// (Bit position: 11)
        /// </summary>
        AssembleDocument = 1 << 11, // Decimal: 2048

        /// <summary>
        /// Permission to print the document to a representation from which a faithful
        /// digital copy of the PDF content could be generated. When this bit is clear
        /// (and <see cref="Print"/> is set), printing is limited to a low-level
        /// representation of the appearance, possibly of degraded quality.
        /// (Bit position: 12)
        /// </summary>
        PrintHighQuality = 1 << 12 // Decimal: 4096
    }

    /// <summary>
    /// Encapsulates the user access permissions for a PDF document as specified in
    /// ISO 32000-2, Table 22. Provides a type-safe interface to manipulate and query
    /// the permissions flags.
    /// </summary>
    public class Permissions {
        private uint _permissionsFlags;

        /// <summary>
        /// A mask that defines the valid bits (3-12) that can be set in the permissions flag.
        /// Bits outside this range are reserved and must be zero.
        /// </summary>
        private const uint ValidBitsMask = 0b1_1111_1111_1000; // Hex: 0xFFF8

        /// <summary>
        /// Initializes a new instance of the <see cref="Permissions"/> class
        /// with no permissions granted.
        /// </summary>
        public Permissions() {
            _permissionsFlags = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permissions"/> class
        /// from the raw 32-bit integer value found in the PDF encryption dictionary's /P key.
        /// Invalid bits (outside positions 3-12) are masked out to ensure compliance.
        /// </summary>
        /// <param name="rawFlags">The raw integer value.</param>
        public Permissions(int rawFlags) : this((uint)rawFlags) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permissions"/> class
        /// from the raw 32-bit integer value found in the PDF encryption dictionary's /P key.
        /// Invalid bits (outside positions 3-12) are masked out to ensure compliance.
        /// </summary>
        /// <param name="rawFlags">The raw integer value.</param>
        public Permissions(uint rawFlags) {
            _permissionsFlags = rawFlags & ValidBitsMask;
        }

        /// <summary>
        /// Gets or sets the permissions using the type-safe <see cref="UserAccess"/> enum.
        /// The getter returns the enum representation of the current flags.
        /// The setter applies the enum value, automatically masking any invalid bits.
        /// </summary>
        public UserAccess Access {
            get => (UserAccess)_permissionsFlags;
            set => _permissionsFlags = (uint)value & ValidBitsMask;
        }

        /// <summary>
        /// Gets the raw 32-bit integer value of the permissions flags.
        /// This value is suitable for writing to the /P key in a PDF encryption dictionary.
        /// All reserved bits are guaranteed to be zero.
        /// </summary>
        public uint RawValue => _permissionsFlags;

        /// <summary>
        /// Gets a value indicating whether the user can print the document
        /// (possibly at low quality, unless <see cref="CanPrintHighQuality"/> is true).
        /// </summary>
        public bool CanPrint => Access.HasFlag(UserAccess.Print);

        /// <summary>
        /// Gets a value indicating whether the user can modify the document's contents.
        /// </summary>
        public bool CanModifyContents => Access.HasFlag(UserAccess.ModifyContents);

        /// <summary>
        /// Gets a value indicating whether the user can copy or extract content.
        /// </summary>
        public bool CanCopyContents => Access.HasFlag(UserAccess.CopyContents);

        /// <summary>
        /// Gets a value indicating whether the user can add or modify annotations and form fields.
        /// This is primarily for legacy PDF support.
        /// </summary>
        public bool CanModifyAnnotations => Access.HasFlag(UserAccess.ModifyAnnotations);

        /// <summary>
        /// Gets a value indicating whether the user can fill interactive form fields.
        /// </summary>
        public bool CanFillFormFields => Access.HasFlag(UserAccess.FillFormFields);

        /// <summary>
        /// Gets a value indicating whether the user can extract content for accessibility.
        /// </summary>
        public bool CanExtractForAccessibility => Access.HasFlag(UserAccess.ExtractContentsForAccessibility);

        /// <summary>
        /// Gets a value indicating whether the user can assemble the document (manipulate pages).
        /// </summary>
        public bool CanAssembleDocument => Access.HasFlag(UserAccess.AssembleDocument);

        /// <summary>
        /// Gets a value indicating whether the user can print the document at high quality.
        /// </summary>
        public bool CanPrintHighQuality => Access.HasFlag(UserAccess.PrintHighQuality);

        /// <summary>
        /// Sets or clears the specified permissions.
        /// </summary>
        /// <param name="permissions">The permissions to modify (from the <see cref="UserAccess"/> enum).</param>
        /// <param name="grant">True to grant the permissions; false to revoke them.</param>
        public void SetPermissions(UserAccess permissions, bool grant = true) {
            if (grant) {
                _permissionsFlags |= (uint)permissions;
            } else {
                _permissionsFlags &= ~(uint)permissions;
            }
            // Re-apply mask to ensure no invalid bits were set by the enum value itself
            _permissionsFlags &= ValidBitsMask;
        }

        /// <summary>
        /// Returns a string that represents the current permissions for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the current permissions.</returns>
        public override string ToString() {
            return $"Permissions: {Access} (Raw Value: {RawValue})";
        }
    }
}
