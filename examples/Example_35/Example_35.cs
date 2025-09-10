using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using PDFjet.NET;
using PDFjet.NET.Fonts;

/**
 *  Example_35.cs
 */
public class Example_35 {
    public Example_35() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_35.pdf", FileMode.Create)));

        Page page = new Page(pdf, Letter.PORTRAIT);

        Font font = new Font(pdf, IBMPlexSans.Regular);
        font.SetSize(14f);

/*
        Rect rect = new Rect(0f, 0f, 300f, 300f);
        rect.SetLocation(0f, 0f);
        rect.SetBorderColor(Color.blue);
        rect.SetBorderWidth(2f);

        TextLine text = new TextLine(font, "Yahoo!");
        text.SetLocation(25f, 25f);
        text.SetFontSize(16f);
        text.SetTextColor(Color.blue);

        Container c = new Container(300f, 300f);
        c.SetLocation(75f, 75f);
        c.Add(rect);
        c.Add(text);
        // c.SetScaleFactor(0.5f);
        c.SetRotateDegreesCCW(45);
        c.DrawOn(page);
*/

        // Base container
        Container container = new Container(400f, 400f);
        container.SetLocation(100f, 100f);

        // Add a rectangle to container
        Rect rect = new Rect(0f, 0f, 400f, 400f);
        rect.SetFillColor(Color.gray);
        container.Add(rect);

        XObject xObject = new XObject(pdf, 400f, 400f);
        xObject.SetStrokeColor(Color.darkgreen);
        xObject.SetStrokeWidth(5f);
        xObject.MoveTo(0f, 0f);
        xObject.LineTo(0f, 400f);
        xObject.LineTo(400f, 400f);
        xObject.LineTo(400f, 0f);
        xObject.ClosePath();
        xObject.Complete();
        xObject.DrawOn(page);
        // container.Add(xObject);

        // Add a text line to container
        TextLine title = new TextLine(font, "Container");
        title.SetLocation(150f, 20f);
        container.Add(title);

        // Nested container #1
        Container nested1 = new Container(200f, 200f);
        nested1.SetLocation(0f, 0f);
        nested1.SetRotationCounterClockwise(30);
        nested1.SetScaleFactor(0.8f);

        Rect innerRect = new Rect(0f, 0f, 200f, 200f);
        innerRect.SetFillColor(Color.blue);
        nested1.Add(innerRect);

        TextLine innerText = new TextLine(font, "Nested 1");
        innerText.SetLocation(50f, 100f);
        nested1.Add(innerText);

        container.Add(nested1);

        // Nested container #2
        Container nested2 = new Container(100f, 100f);
        nested2.SetLocation(250f, 250f);
        nested2.SetRotationCounterClockwise(45);

        Rect smallRect = new Rect(0f, 0f, 100f, 100f);
        smallRect.SetFillColor(Color.red);
        nested2.Add(smallRect);

        TextLine smallText = new TextLine(font, "Nested 2");
        smallText.SetLocation(10f, 50f);
        nested2.Add(smallText);

        container.Add(nested2);

        container.SetRotationClockwise(45);
        // Draw the entire hierarchy on the page
        container.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_35();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_35", time0, time1);
    }
}   // End of Example_35.cs
