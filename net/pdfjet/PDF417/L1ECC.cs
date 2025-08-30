/**
 * L1ECC.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */

namespace PDFjet.NET.PDF417 {
class L1ECC {
// L1ECC holds the Reed‑Solomon coefficients for the Level‑1 (ECC‑L1)
// block of a PDF‑417 symbol.
//
// Values taken from the PDF‑417 spec: 522, 568, 723, 809.
// Treated as read‑only – callers should not modify the slice.
public static readonly int[] table = {
522, 568, 723, 809,
};
}
}
