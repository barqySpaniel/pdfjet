package pdf417

/**
 * ecc1table.go
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */

// ecc1Table holds the Reed‑Solomon coefficients for error‑correction level 1.
// It is an internal implementation detail of the PDF417 encoder, so the name
// starts with a lower‑case letter (unexported) and follows camelCase.
var ecc1Table = []int{
	522,
	568,
	723,
	809,
}
