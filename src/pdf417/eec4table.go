package pdf417

/**
 * ecc4table.go
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */

// ecc4Table holds the Reed‑Solomon coefficients for error‑correction level 4.
// It is an internal implementation detail, so the name starts with a lower‑case
// letter (unexported) and follows the “camelCase” convention for package‑private
// identifiers.
var ecc4Table = []int{
	361, 575, 922, 525, 176, 586, 640, 321, 536, 742, 677, 742, 687, 284, 193, 517,
	273, 494, 263, 147, 593, 800, 571, 320, 803, 133, 231, 390, 685, 330, 63, 410,
}
