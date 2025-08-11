// Package element defines standard PDF structure element tags
// as defined in ISO 32000-1 (PDF 1.7) and PDF/UA specifications
package element

// Structure Elements for Tagged PDF
const (
	// Document-level structure elements
	Document = "Document"
	Part     = "Part"
	Div      = "Div"
	Sect     = "Sect"

	// Heading elements
	H1 = "H1"
	H2 = "H2"
	H3 = "H3"
	H4 = "H4"
	H5 = "H5"
	H6 = "H6"

	// Paragraph and text elements
	P     = "P"
	Title = "Title"
	Lbl   = "Lbl"

	// Inline text
	Span   = "Span"
	Em     = "Em"
	Strong = "Strong"

	// Links and annotations
	Link  = "Link"
	Annot = "Annot"

	// List elements
	L  = "L"  // List
	LI = "LI" // List Item

	// Table elements
	Table   = "Table"
	TR      = "TR"    // Table Row
	TH      = "TH"    // Table Header
	TD      = "TD"    // Table Data
	THead   = "THead" // Table Header group
	TBody   = "TBody" // Table Body group
	TFoot   = "TFoot" // Table Footer group
	Caption = "Caption"

	// Figure and special elements
	Figure   = "Figure"
	Artifact = "Artifact"
)
