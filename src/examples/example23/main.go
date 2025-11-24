package main

import (
	"time"

	pdfjet "github.com/edragoev1/pdfjet/src"
	"github.com/edragoev1/pdfjet/src/IBMPlexSans"
	"github.com/edragoev1/pdfjet/src/corefont"
	"github.com/edragoev1/pdfjet/src/letter"
)

// Example23 draws the Canadian flag using a Path object that contains both lines
// and curve segments. Every curve segment must have exactly 2 control points.
func Example23() {
	pdf := pdfjet.NewPDFFile("Example_23.pdf")

	f1 := pdfjet.NewFontFromFile(pdf, IBMPlexSans.Regular)
	f1.SetSize(72.0)

	f2 := pdfjet.NewCoreFont(pdf, corefont.Helvetica())
	f2.SetSize(24.0)

	page := pdfjet.NewPage(pdf, letter.Portrait)

	x1 := float32(90.0)
	y1 := float32(50.0)

	textLine := pdfjet.NewTextLine(f2, "(x1, y1)")
	textLine.SetLocation(x1, y1-15.0)
	textLine.DrawOn(page)

	textBox := pdfjet.NewTextBox(f1)
	textBox.SetText("Heya, World! This is a test to show the functionality of a TextBox.")
	textBox.SetLocation(x1, y1)
	textBox.SetWidth(500.0)
	// textBox.SetFillColor(color.LightGreen)
	// textBox.SetTextColor(color.Black)
	xy := textBox.DrawOn(page)

	/*
		float x2 = x1 + textBox.GetWidth();
		// float y2 = y1 + textBox.GetHeight();

		f2.SetSize(18f);

		// Text on the left
		TextLine ascent_text = new TextLine(f2, "Ascent");
		ascent_text.SetLocation(x1 - 85f, y1 + 40f);
		ascent_text.DrawOn(page);

		TextLine descentText = new TextLine(f2, "Descent");
		descentText.SetLocation(x1 - 85f, y1 + f1.GetAscent(f1.GetSize()) + 15f);
		descentText.DrawOn(page);

		// Line beside the text ascent
		Line ascentLine = new Line(
			x1 - 10f,
			y1,
			x1 - 10f,
			y1 + f1.GetAscent());
		ascentLine.SetColor(Color.blue);
		ascentLine.SetWidth(3f);
		ascentLine.DrawOn(page);

		// Line beside the text descent
		Line descentLine = new Line(
			x1 - 10f,
			y1 + f1.GetAscent(f1.GetSize()),
			x1 - 10f,
			y1 + f1.GetAscent(f1.GetSize()) + f1.GetDescent(f1.GetSize()));
		descentLine.SetColor(Color.red);
		descentLine.SetWidth(3f);
		descentLine.DrawOn(page);

		// Lines for first line of text
		Line text_line1 = new Line(
			x1,
			y1 + f1.GetAscent(f1.GetSize()),
			x2,
			y1 + f1.GetAscent(f1.GetSize()));
		text_line1.DrawOn(page);

		Line descent_line1 = new Line(
			x1,
			y1 + (f1.GetAscent(f1.GetSize()) + f1.GetDescent(f1.GetSize())),
			x2,
			y1 + (f1.GetAscent(f1.GetSize()) + f1.GetDescent(f1.GetSize())));
		descent_line1.DrawOn(page);

		// Lines for second line of text
		float curr_y = y1 + f1.GetBodyHeight(f1.GetSize());

		Line text_line2 = new Line(
			x1,
			curr_y + f1.GetAscent(f1.GetSize()),
			x2,
			curr_y + f1.GetAscent(f1.GetSize()));
		text_line2.DrawOn(page);

		Line descent_line2 = new Line(
			x1,
			curr_y + f1.GetAscent(f1.GetSize()) + f1.GetDescent(f1.GetSize()),
			x2,
			curr_y + f1.GetAscent(f1.GetSize()) + f1.GetDescent(f1.GetSize()));
		descent_line2.DrawOn(page);

		Point p1 = new Point(x1, y1);
		p1.SetRadius(5f);
		p1.DrawOn(page);

		Point p2 = new Point(xy[0], xy[1]);
		p2.SetRadius(5f);
		p2.DrawOn(page);

		f2.SetSize(24f);
		TextLine textLine2 = new TextLine(f2, "(x2, y2)");
		textLine2.SetLocation(xy[0] - 80f, xy[1] + 30f);
		textLine2.DrawOn(page);
	*/
	box := pdfjet.NewBox()
	box.SetLocation(xy[0], xy[1])
	box.SetSize(20.0, 20.0)
	box.DrawOn(page)

	pdf.Complete()
}

func main() {
	start := time.Now()
	Example23()
	pdfjet.PrintDuration("Example_23", time.Since(start))
}
