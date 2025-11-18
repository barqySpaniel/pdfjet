package alignment

/**
 * alignment.go
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */

type Alignment int

// Used to specify the text alignment in textblock.go
const (
	Left = iota
	Right
	Center
)
