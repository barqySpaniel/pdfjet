using System;
using System.IO;
using System.Diagnostics;
using PDFjet.NET;
using PDFjet.NET.Fonts;

/**
 *  Example_36.cs - Demonstrates creation and placement of Form XObjects.
 *  A Form XObject is a reusable graphics object that can be drawn multiple times
 *  on different pages or locations with a single definition.
 */
public class Example_36 {
    public Example_36() {
        // Initialize PDF document with buffered streaming for performance
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_36.pdf", FileMode.Create)));

        Font font = new Font(pdf, IBMPlexSans.Regular);

        // Create a new page
        Page page = new Page(pdf, Letter.PORTRAIT);

        // Base container
        Container container = new Container(150f, 150f);
        container.SetLocation(50f, 50f);

        // Add a rectangle to container
        Rect rect = new Rect(0f, 0f, 150f, 150f);
        rect.SetBorderColor(Color.blue);
        rect.SetBorderWidth(2f);
        container.Add(rect);

        Line line = new Line(0f, 0f, 75f, 75f);
        line.SetWidth(2f);
        container.Add(line);

        // Add a text line to container
        TextLine title = new TextLine(font, "Hello");
        title.SetFontSize(16f);
        title.SetLocation(20f, 20f);
        container.Add(title);

        title = new TextLine(font, "World");
        title.SetFontSize(16f);
        title.SetLocation(40f, 40f);
        title.SetTextColor(Color.blue);
        container.Add(title);

        float[] pointXY = container.DrawOn(page);

        container.SetLocation(pointXY[0], pointXY[1]);
        pointXY = container.DrawOn(page);

        container.SetLocation(pointXY[0], pointXY[1]);
        container.SetRotateDegreesCW(45f);
        pointXY = container.DrawOn(page);

        container.SetLocation(pointXY[0] - 300f, pointXY[1]);
        container.SetRotateDegreesCCW(45f);
        pointXY = container.DrawOn(page);

        // Finalize the PDF document
        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_36();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_36", time0, time1);
    }
}   // End of Example_36.cs
