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

        // Create a 100x100 point Form XObject (reusable graphics container)
        FormXObject form = new FormXObject(pdf, 100f, 100f);

        // Draw a blue square
        form.SetPenColor(Color.blue);
        form.SetPenWidth(2f);
        form.MoveTo(0f, 0f);
        form.LineTo(100f, 0f);
        form.LineTo(100f, 100f);
        form.LineTo(0f, 100f);
        form.ClosePath();

        // Draw a black diagonal line inside the square
        form.SetPenColor(Color.black);
        form.SetPenWidth(2f);
        form.MoveTo(0f, 0f);
        form.LineTo(50f, 50f);
        form.StrokePath();

        TextLine text = new TextLine(font, "Hello");
        text.SetLocation(15f, 15f);
        text.SetTextColor(Color.black);
        form.Add(text);

        text = new TextLine(font, "World");
        text.SetLocation(35f, 35f);
        text.SetTextColor(Color.blue);
        form.Add(text);

        // Finalize the Form XObject definition in the PDF
        form.Complete();

        // Create a new page
        Page page = new Page(pdf, Letter.PORTRAIT);

        // Draw the Form XObject at multiple positions to demonstrate reusability
        float currentX = 100f;
        float currentY = 100f;

        // First placement
        form.SetLocation(currentX, currentY);
        float[] pointXY = form.DrawOn(page);

        // Second placement - using returned position from previous draw
        form.SetLocation(pointXY[0], pointXY[1]);
        pointXY = form.DrawOn(page);

        // Third placement
        form.SetLocation(pointXY[0], pointXY[1]);
        form.SetRotateDegreesCW(45f);
        form.DrawOn(page);

        // Fourth placement
        form.SetLocation(100f, 300f);
        form.SetRotateDegreesCCW(45f);
        form.DrawOn(page);

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
