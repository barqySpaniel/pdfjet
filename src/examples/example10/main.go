package main

import (
	"time"

	pdfjet "github.com/edragoev1/pdfjet/src"
	"github.com/edragoev1/pdfjet/src/letter"
)

func Example10() {
	pdf := pdfjet.NewPDFFile("Example_10.pdf")

	image := pdfjet.NewImageFromFile(pdf, "images/sz-map.png")

	f1 := pdfjet.NewFontFromFile(pdf, "fonts/IBMPlexSans/IBMPlexSans-Regular.ttf.stream")
	f1.SetSize(10.0)

	f2 := pdfjet.NewFontFromFile(pdf, "fonts/IBMPlexSans/IBMPlexSans-SemiBold.ttf.stream")
	f2.SetSize(14.0)

	f3 := pdfjet.NewFontFromFile(pdf, "fonts/IBMPlexSans/IBMPlexSans-SemiBold.ttf.stream")
	f3.SetSize(12.0)

	f4 := pdfjet.NewFontFromFile(pdf, "fonts/IBMPlexSans/IBMPlexSans-Italic.ttf.stream")
	f4.SetSize(10.0)

	page := pdfjet.NewPage(pdf, letter.Portrait)

	image.SetLocation(90.0, 35.0)
	image.ScaleBy(0.75)
	image.DrawOn(page)
	/*
		rotate := 0
		// int rotate := 90
		// int rotate := 270
		column := pdfjet.NewTextColumn(rotate)
		column.SetLineSpacing(1.3)		// 1.3 x font height
		column.SetParagraphSpacing(1.0) // 1.0 x line spacing

		p1 := pdfjet.NewParagraph()
		p1.SetAlignment(align.CENTER)
		p1.Add(pdfjet.NewTextLine(f2, "Switzerland"))

		p2 := pdfjet.NewParagraph()
		p2.Add(pdfjet.NewTextLine(f2, "Introduction"))

		buf := StringBuilder()
		buf.Append("The Swiss Confederation was founded in 1291 as a defensive ")
		buf.Append("alliance among three cantons. In succeeding years, other ")
		buf.Append("localities joined the original three. ")
		buf.Append("The Swiss Confederation secured its independence from the ")
		buf.Append("Holy Roman Empire in 1499. Switzerland's sovereignty and ")
		buf.Append("neutrality have long been honored by the major European ")
		buf.Append("powers, and the country was not involved in either of the ")
		buf.Append("two World Wars. The political and economic integration of ")
		buf.Append("Europe over the past half century, as well as Switzerland's ")
		buf.Append("role in many UN and international organizations, has ")
		buf.Append("strengthened Switzerland's ties with its neighbors. ")
		buf.Append("However, the country did not officially become a UN member ")
		buf.Append("until 2002.")

		p3 := pdfjet.NewParagraph()
		// p3.SetAlignment(Align.LEFT)
		// p3.SetAlignment(Align.RIGHT)
		p3.SetAlignment(Align.JUSTIFY)
		text := NewTextLine(f1, buf.ToString())
		p3.Add(text)

		buf := new StringBuilder()
		buf.Append("Switzerland remains active in many UN and international ")
		buf.Append("organizations but retains a strong commitment to neutrality.")

		text = NewTextLine(f1, buf.ToString())
		text.SetTextColor(Color.red)
		p3.Add(text)

		p4 = NewParagraph()
		p4.Add(NewTextLine(f3, "Economy"))

		buf = new StringBuilder()
		buf.Append("Switzerland is a peaceful, prosperous, and stable modern ")
		buf.Append("market economy with low unemployment, a highly skilled ")
		buf.Append("labor force, and a per capita GDP larger than that of the ")
		buf.Append("big Western European economies. The Swiss in recent years ")
		buf.Append("have brought their economic practices largely into ")
		buf.Append("conformity with the EU's to enhance their international ")
		buf.Append("competitiveness. Switzerland remains a safe haven for ")
		buf.Append("investors, because it has maintained a degree of bank secrecy ")
		buf.Append("and has kept up the franc's long-term external value. ")
		buf.Append("Reflecting the anemic economic conditions of Europe, GDP ")
		buf.Append("growth stagnated during the 2001-03 period, improved during ")
		buf.Append("2004-05 to 1.8% annually and to 2.9% in 2006.")

		p5 := NewParagraph()
		p5.SetAlignment(Align.JUSTIFY)
		text = NewTextLine(f1, buf.ToString())
		p5.Add(text)

		text = NewTextLine(f4,
			"Even so, unemployment has remained at less than half the EU average.")
		text.SetTextColor(Color.blue)
		p5.Add(text)

		column.AddParagraph(p1)
		column.AddParagraph(p2)
		column.AddParagraph(p3)
		column.AddParagraph(p4)
		column.AddParagraph(p5)

		if rotate == 0 {
			column.SetLocation(90.0, 300.0)
		} else if rotate == 90 {
			column.SetLocation(90.0, 780.0)
		} else if rotate == 270 {
			column.SetLocation(550.0, 310.0)
		}

		columnWidth := 470.0
		column.SetSize(columnWidth, 100.0)
		xy := column.DrawOn(page)

		line := NewLine(
			xy[0],
			xy[1],
			xy[0] + columnWidth,
			xy[1])
		line.DrawOn(page)
	*/
	pdf.Complete()
}

func main() {
	start := time.Now()
	Example10()
	pdfjet.PrintDuration("Example_10", time.Since(start))
}
