using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using PDFjet.NET;

/**
 * Example_51.cs
 */
public class Example_51 {
    public Example_51() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_51.pdf", FileMode.Create)));

        Font f1 = new Font(pdf, CoreFont.HELVETICA);
        f1.SetSize(16f);

        Image image = new Image(pdf, "images/qrcode.png");
        image.SetLocation(100f, 100f);

        Page page = new Page(pdf, Letter.PORTRAIT);

        TextBlock textBlock = new TextBlock(f1, "Hello, World");
        textBlock.SetLocation(50f, 50f);

        Container container = new Container(400f, 400f);
        container.Add(image);
        container.Add(textBlock);
        container.SetLocation(100f, 100f);

        TextLine textLine = new TextLine(f1, "This is a test!!");
        textLine.SetLocation(400f, 400f);

        OptionalContentGroup group = new OptionalContentGroup(pdf, "Open Street Map");
        group.Add(container);
        group.Add(textLine);
        group.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        new Example_51();
    }
}   // End of Example_51.cs
