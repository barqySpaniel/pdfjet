using System;
using System.IO;
using System.Diagnostics;
using PDFjet.NET;

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

        // Create a 100x100 point Form XObject (reusable graphics container)
        FormXObject squareWithDiagonal = new FormXObject(pdf, 100f, 100f);

        // Draw a blue square
        squareWithDiagonal.SetPenColor(Color.blue);
        squareWithDiagonal.MoveTo(0f, 0f);
        squareWithDiagonal.LineTo(100f, 0f);
        squareWithDiagonal.LineTo(100f, 100f);
        squareWithDiagonal.LineTo(0f, 100f);
        squareWithDiagonal.ClosePath();

        // Draw a black diagonal line inside the square
        squareWithDiagonal.SetPenColor(Color.black);
        squareWithDiagonal.MoveTo(0f, 0f);
        squareWithDiagonal.LineTo(50f, 50f);
        squareWithDiagonal.StrokePath();

        // Finalize the Form XObject definition in the PDF
        squareWithDiagonal.AddToPDF(pdf);

        // Create a new page
        Page page = new Page(pdf, Letter.PORTRAIT);

        // Draw the Form XObject at multiple positions to demonstrate reusability
        float currentX = 100f;
        float currentY = 100f;

        // First placement
        squareWithDiagonal.SetLocation(currentX, currentY);
        float[] newPosition = squareWithDiagonal.DrawOn(page);

        // Second placement - using returned position from previous draw
        squareWithDiagonal.SetLocation(newPosition[0], newPosition[1]);
        newPosition = squareWithDiagonal.DrawOn(page);

        // Third placement
        squareWithDiagonal.SetLocation(newPosition[0], newPosition[1]);
        squareWithDiagonal.DrawOn(page);

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
