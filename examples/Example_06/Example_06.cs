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
        pdf.SetTitle("Hello");
        pdf.SetAuthor("Eugene");
        pdf.SetSubject("Example");
        pdf.SetKeywords("Hello World This is a test");
        pdf.SetCreator("Application Name");

        Font f1 = new Font(pdf, IBMPlexSans.Regular);

        EmbeddedFile file1 = new EmbeddedFile(pdf, "images/linux-logo.png", Compress.NO);
        EmbeddedFile file2 = new EmbeddedFile(pdf, "examples/Example_02/Example_02.cs", Compress.YES);

        Page page = new Page(pdf, Letter.PORTRAIT);

        page.RotateBy(90);

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

        OptionalContentGroup group = new OptionalContentGroup(pdf, "Blue Layer");
        TextLine textLine = new TextLine(f1, "Blue Layer Text");
        textLine.SetLocation(50f, 360f);
        textLine.SetTextColor(new float[] { 0f, 0f, 0.8f });
        textLine.SetURIAction("https://www.planetassociates.com");
        group.Add(textLine);
        group.SetVisible(true);
        group.DrawOn(page);

        textLine = new TextLine(f1, "pdfjet.com");
        textLine.SetLocation(50f, 400f);
        textLine.SetURIAction("https://pdfjet.com");
        textLine.DrawOn(page);

        Container container = new Container(50f, 200f);
        container.SetLocation(100f, 500f);
        PDFjet.NET.Rect rect1 = new PDFjet.NET.Rect(0,0, 50,200);
        container.Add(rect1);
        textLine = new TextLine(f1, "The Container");
        textLine.SetLocation(0f, 0f);
        container.Add(textLine);

        SquareAnnotation squareAnnotation = new SquareAnnotation();
        // squareAnnotation.SetLocation(0f, 0f);//50f, 500f);
        squareAnnotation.SetSize(75f, 100f);
        squareAnnotation.SetFillColor(new float[] {0f, 0f, 1f});
        squareAnnotation.SetTransparency(0.5f);
        squareAnnotation.SetTitle("Hello, World!");
        squareAnnotation.SetContents("The quick brown fox jumps over the lazy dog.");
        container.Add(squareAnnotation);
        //squareAnnotation.DrawOn(page);

        PolygonAnnotation polygonAnnotation = new PolygonAnnotation();
        polygonAnnotation.SetLocation(25f, 0f);//50f, 500f);
        polygonAnnotation.SetVertices(new float[] {0f, 0f, 50f, 0f, 50f, 50f, 0f, 0f});
        polygonAnnotation.SetFillColor(Color.red);
        polygonAnnotation.SetTransparency(0.5f);
        polygonAnnotation.SetTitle("This is a test ...");
        polygonAnnotation.SetContents("The quick brown cat caught the lazy mouse.");
        //polygonAnnotation.DrawOn(page);
        container.Add(polygonAnnotation);
        // container.SetRotationClockwise(90);
        container.DrawOn(page);

        TextAnnotation textAnnotation = new TextAnnotation();
        textAnnotation.SetLocation(150f, 500f);
        textAnnotation.SetSize(20f, 20f);
        textAnnotation.SetTitle("Hello");
        textAnnotation.SetContents("World");
        textAnnotation.DrawOn(page);

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
