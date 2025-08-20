package main

import (
	"strconv"
	"time"

	pdfjet "github.com/edragoev1/pdfjet/src"
	"github.com/edragoev1/pdfjet/src/color"
	"github.com/edragoev1/pdfjet/src/corefont"
	"github.com/edragoev1/pdfjet/src/letter"
	"github.com/edragoev1/pdfjet/src/shape"
)

// Example05 draws the Canadian flag using a Path object that contains both lines
// and curve segments. Every curve segment must have exactly 2 control points.
func Example05() {
	pdf := pdfjet.NewPDFFile("Example_05.pdf")

	f1 := pdfjet.NewCoreFont(pdf, corefont.HelveticaBold())
	f1.SetItalic(true)

	page := pdfjet.NewPage(pdf, letter.Portrait)

	// page.SetPenColor(color.Aqua)
	page.SetPenWidth(0.0)
	_ = page.DrawArc(500, 100, 50, 50, 0, 90)
	// (new Line(xy[0], xy[1], xy[0], xy[1] + 50)).DrawOn(page);

	text := pdfjet.NewTextLine(f1, "")
	text.SetLocation(300.0, 300.0)
	for i := 0; i < 360; i += 15 {
		text.SetTextDirection(i)
		text.SetUnderline(true)
		// text.setStrikeLine(true);
		text.SetText("             Hello, World -- " + strconv.Itoa(i) + " degrees.")
		text.DrawOn(page)
	}

	text = pdfjet.NewTextLine(f1, "WAVE AWAY")
	text.SetLocation(70.0, 50.0)
	text.DrawOn(page)

	f1.SetKernPairs(true)
	text = pdfjet.NewTextLine(f1, "WAVE AWAY")
	text.SetLocation(70.0, 70.0)
	text.DrawOn(page)

	f1.SetKernPairs(false)
	text = pdfjet.NewTextLine(f1, "WAVE AWAY")
	text.SetLocation(70.0, 90.0)
	text.DrawOn(page)

	f1.SetSize(8.0)
	text = pdfjet.NewTextLine(f1, "-- font.setKernPairs(false);")
	text.SetLocation(150.0, 50.0)
	text.DrawOn(page)
	text.SetLocation(150.0, 90.0)
	text.DrawOn(page)
	text = pdfjet.NewTextLine(f1, "-- font.setKernPairs(true);")
	text.SetLocation(150.0, 70.0)
	text.DrawOn(page)

	point := pdfjet.NewPoint(300.0, 300.0)
	point.SetShape(shape.Circle)
	point.SetFillShape(true)
	point.SetColor(color.Blue)
	point.SetRadius(37.0)
	point.DrawOn(page)
	point.SetRadius(25.0)
	point.SetColor(color.White)
	point.DrawOn(page)

	page.SetPenWidth(1.0)
	page.DrawEllipse(300.0, 600.0, 100.0, 50.0)

	pdf.Complete()
}

func main() {
	start := time.Now()
	Example05()
	pdfjet.PrintDuration("Example_05", time.Since(start))
}
