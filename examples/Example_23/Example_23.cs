using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using PDFjet.NET;

/**
 * Example_23.cs
 */
public class Example_23 {
    public Example_23() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_23.pdf", FileMode.Create)));

        Font f1 = new Font(pdf, IBMPlexSans.Regular);
        f1.SetSize(72f);

        Font f2 = new Font(pdf, CoreFont.HELVETICA);
        f2.SetSize(24f);

        Page page = new Page(pdf, Letter.PORTRAIT);

        StringBuilder buf = new StringBuilder();
        buf.Append("Heya, World! This is a test to show the functionality of a TextBox.");

        float x1 = 90f;
        float y1 = 50f;

        TextLine textLine = new TextLine(f2, "(x1, y1)");
        textLine.SetLocation(x1, y1 - 15f);
        textLine.DrawOn(page);

        TextBox textBox = new TextBox(f1, buf.ToString());
        textBox.SetLocation(x1, y1);
        textBox.SetWidth(500f);
        textBox.SetFillColor(Color.lightgreen);
        textBox.SetTextColor(Color.black);
        float[] xy = textBox.DrawOn(page);

        float x2 = x1 + textBox.GetWidth();
        float y2 = y1 + textBox.GetHeight();

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
        // p1.SetFillShape(true);
        p1.DrawOn(page);

        Point p2 = new Point(x2, y2);
        p2.SetRadius(5f);
        // p2.SetFillShape(true);
        p2.DrawOn(page);

        f2.SetSize(24f);
        TextLine textLine2 = new TextLine(f2, "(x2, y2)");
        textLine2.SetLocation(x2 - 80f, y2 + 30f);
        textLine2.DrawOn(page);

        Box box = new Box();
        box.SetLocation(xy[0], xy[1]);
        box.SetSize(20f, 20f);
        box.DrawOn(page);

        pdf.Complete();
    }

    public void DrawTextAndLines(
            String text, Page page, Font font, float x, float y) {
        TextLine textLine = new TextLine(font, text);
        textLine.SetLocation(x, y);
        textLine.DrawOn(page);

        Line ascenderLine = new Line(x, y - font.GetAscent(), x + 100f, y - font.GetAscent(font.GetSize()));
        ascenderLine.SetWidth(2f);
        ascenderLine.DrawOn(page);

        Line line = new Line(x, y, x + 100f, y);
        line.SetWidth(2f);
        line.DrawOn(page);

        Line descenderLine = new Line(x, y + font.GetDescent(), x + 100f, y + font.GetDescent(font.GetSize()));
        descenderLine.SetWidth(2f);
        descenderLine.DrawOn(page);
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_23();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_23", time0, time1);
    }
}   // End of Example_23.cs
