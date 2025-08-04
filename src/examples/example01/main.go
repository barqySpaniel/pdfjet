package main

import (
	"time"

	pdfjet "github.com/edragoev1/pdfjet/src"
	"github.com/edragoev1/pdfjet/src/color"
	"github.com/edragoev1/pdfjet/src/content"
	"github.com/edragoev1/pdfjet/src/font"
	"github.com/edragoev1/pdfjet/src/letter"
)

// Example01 demonstrates creating a PDF with text blocks containing multilingual content
func Example01() {
	pdf := pdfjet.NewPDFFile("Example_01.pdf")

	font1 := pdfjet.NewFontFromFile(pdf, font.IBMPlexSans.Regular)
	font1.SetSize(12.0)

	page := pdfjet.NewPage(pdf, letter.Portrait)

	// English text block
	textBlock := pdfjet.NewTextBlock(font1, content.OfTextFile("data/languages/english.txt"))
	textBlock.SetLocation(50.0, 50.0)
	textBlock.SetWidth(430.0) // The height is adjusted automatically to fit the text.
	textBlock.SetTextPadding(10.0)
	xy := textBlock.DrawOn(page)

	// Draw a blue rectangle at the bottom right corner of the English text block.
	rect := pdfjet.NewRectAt(xy[0], xy[1], 30.0, 30.0)
	rect.SetBorderColor(color.Blue)
	rect.DrawOn(page)

	// Greek text block
	textBlock = pdfjet.NewTextBlock(font1, content.OfTextFile("data/languages/greek.txt"))
	textBlock.SetLocation(50.0, xy[1]+30.0)
	textBlock.SetWidth(430.0) // The height is adjusted automatically to fit the text.
	textBlock.SetTextPadding(10.0)
	textBlock.SetBorderColor(color.None)
	xy = textBlock.DrawOn(page)

	// Bulgarian text block
	textBlock = pdfjet.NewTextBlock(font1, content.OfTextFile("data/languages/bulgarian.txt"))
	textBlock.SetLocation(50.0, xy[1]+30.0)
	textBlock.SetWidth(430.0) // The height is adjusted automatically to fit the text.
	textBlock.SetTextPadding(10.0)
	textBlock.SetBorderColor(color.Blue)
	textBlock.SetBorderCornerRadius(10.0)
	textBlock.DrawOn(page)

	// Finalize the PDF
	pdf.Complete()
}

func main() {
	start := time.Now()
	Example01()
	pdfjet.PrintDuration("Example_01", time.Since(start))
}
