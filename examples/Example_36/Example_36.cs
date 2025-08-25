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

        // Create a 100x100 point Form XObject (reusable graphics container)
        FormXObject form = new FormXObject(pdf, 100f, 100f);

        // Draw a blue square
        form.SetPenColor(Color.blue);
        form.MoveTo(0f, 0f);
        form.LineTo(100f, 0f);
        form.LineTo(100f, 100f);
        form.LineTo(0f, 100f);
        form.ClosePath();

        // Draw a black diagonal line inside the square
        form.SetPenColor(Color.black);
        form.MoveTo(0f, 0f);
        form.LineTo(50f, 50f);
        form.StrokePath();

        TextLine text = new TextLine(font, "Hello,");
        text.SetLocation(15f, 15f);
        text.DrawOn(form);

        // Finalize the Form XObject definition in the PDF
        form.AddToPDF(pdf);

        // Create a new page
        Page page = new Page(pdf, Letter.PORTRAIT);

        // Draw the Form XObject at multiple positions to demonstrate reusability
        float currentX = 100f;
        float currentY = 100f;

        // First placement
        form.SetLocation(currentX, currentY);
        float[] newPosition = form.DrawOn(page);

        // Second placement - using returned position from previous draw
        form.SetLocation(newPosition[0], newPosition[1]);
        newPosition = form.DrawOn(page);

        // Third placement
        form.SetLocation(newPosition[0], newPosition[1]);
        form.DrawOn(page);

        text = new TextLine(font, "World!");
        text.SetLocation(315f, 115f);
        text.DrawOn(page);

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
