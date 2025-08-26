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
        font.SetSize(14f);

        // Create a new page
        Page page = new Page(pdf, Letter.PORTRAIT);

        Rect rect = new Rect(0f, 0f, 300f, 300f);
        rect.SetLocation(50f, 450f);
        rect.SetBorderColor(Color.blue);
        rect.DrawOn(page);

        // Finalize the PDF document
        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch performanceTimer = Stopwatch.StartNew();
        long startTime = performanceTimer.ElapsedMilliseconds;

        new Example_36();

        long endTime = performanceTimer.ElapsedMilliseconds;
        performanceTimer.Stop();

        TextUtils.PrintDuration("Example_36", startTime, endTime);
    }
}   // End of Example_36.cs
