using System;
using System.IO;
using System.Diagnostics;
using PDFjet.NET;

/**
 *  Example_06.cs
 *  We will draw the American flag using Box, Line and Point objects.
 */
public class Example_06 {
    public Example_06() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_06.pdf", FileMode.Create)));
        pdf.SetTitle("Annotation Examples");
        pdf.SetAuthor("ED");
        pdf.SetSubject("Example");
        pdf.SetKeywords("Hello World This is a test");

        Font f1 = new Font(pdf, IBMPlexSans.Regular);

        EmbeddedFile file1 = new EmbeddedFile(pdf, "images/linux-logo.png", Compress.NO);
        EmbeddedFile file2 = new EmbeddedFile(pdf, "examples/Example_02/Example_02.cs", Compress.YES);

        Page page = new Page(pdf, Letter.PORTRAIT);

        Container container = new Container(400f, 400f);
        container.SetLocation(100f, 100f);
        PDFjet.NET.Rect rect = new PDFjet.NET.Rect();
        rect.SetSize(400f, 400f);
        container.Add(rect);
        container.Rotate(180);

        // File attachment functionality
        FileAttachment attachment = new FileAttachment(pdf, file1);
        attachment.SetLocation(100f, 300f);
        attachment.SetIconPushPin();
        attachment.SetIconSize(18f);
        attachment.SetTitle("Attached File: " + file1.GetFileName());
        attachment.SetDescription(
                "Right mouse click on the icon to save the attached file.");
        attachment.DrawOn(page);

        attachment = new FileAttachment(pdf, file2);
        attachment.SetLocation(200f, 300f);
        attachment.SetIconPaperclip();
        attachment.SetIconSize(18f);
        attachment.SetTitle("Attached File: " + file2.GetFileName());
        attachment.SetDescription(
                "Right mouse click on the icon to save the attached file.");
        attachment.DrawOn(page);

        TextLine textLine = new TextLine(f1, "pdfjet.com");
        textLine.SetLocation(50f, 400f);
        textLine.SetURIAction("https://pdfjet.com");
        textLine.DrawOn(page);

        SquareAnnotation squareAnnotation = new SquareAnnotation();
        squareAnnotation.SetLocation(0f, 0f);
        squareAnnotation.SetSize(50f, 50f);
        squareAnnotation.SetFillColor(new float[] {0f, 0f, 1f});
        squareAnnotation.SetTransparency(0.5f);
        squareAnnotation.SetTitle("Hello, World!");
        squareAnnotation.SetContents("The quick brown fox jumps over the lazy dog.");
        container.Add(squareAnnotation);
        // squareAnnotation.DrawOn(page);

        PolygonAnnotation polygonAnnotation = new PolygonAnnotation();
        polygonAnnotation.SetLocation(50f, 0f);
        polygonAnnotation.SetVertices(new float[] {0f, 0f, 50f, 0f, 50f, 50f, 0f, 0f});
        polygonAnnotation.SetFillColor(Color.red);
        polygonAnnotation.SetTransparency(0.5f);
        polygonAnnotation.SetTitle("This is a test ...");
        polygonAnnotation.SetContents("The quick brown cat caught the lazy mouse.");
        container.Add(polygonAnnotation);
        // polygonAnnotation.DrawOn(page);

        TextAnnotation textAnnotation = new TextAnnotation();
        textAnnotation.SetLocation(150f, 500f);
        textAnnotation.SetSize(20f, 20f);
        textAnnotation.SetTitle("Hello");
        textAnnotation.SetContents("World");
        textAnnotation.DrawOn(page);

        container.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_06();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_06", time0, time1);
    }
}   // End of Example_06.cs
