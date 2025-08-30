package pdf417

/**
 * ecc5table.go
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */

// ecc5Table holds the Reed‑Solomon coefficients for error‑correction level 5.
// It is an internal implementation detail of the PDF417 encoder, so the name
// starts with a lower‑case letter (unexported) and follows camelCase.
var ecc5Table = []int{
	539, 422, 6, 93, 862, 771, 453, 106, 610, 287, 107, 505, 733, 877, 381, 612,
	723, 476, 462, 172, 430, 609, 858, 822, 543, 376, 511, 400, 672, 762, 283, 184,
	440, 35, 519, 31, 460, 594, 225, 535, 517, 352, 605, 158, 651, 201, 488, 502,
	648, 733, 717, 83, 404, 97, 280, 771, 840, 629, 4, 381, 843, 623, 264, 543,
}
