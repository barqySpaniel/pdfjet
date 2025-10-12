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

        Container flag = new Container(190f, 100f);
        flag.SetLocation(50f, 50f);

        Rect border = new Rect();
        border.SetLocation(0f, 0f);
        border.SetSize(190.0f, 100.0f);
        border.SetBorderColor(Color.lightgray);
        flag.Add(border);

        float sw = 7.69f;       // stripe width
        for (int row = 0; row < 7; row++) {
            Line stripe = new Line(0f, sw/2 + 2*row*sw, 190f, sw/2 + 2*row*sw);
            stripe.SetWidth(sw);
            stripe.SetColor(Color.oldgloryred);
            flag.Add(stripe);
        }

        Container union = new Container(76.0f, 53.85f);
        union.SetLocation(0f, 0f);
        Rect rect = new Rect();
        rect.SetLocation(0f, 0f);
        rect.SetSize(76.0f, 53.85f);
        rect.SetFillColor(Color.oldgloryblue);
        union.Add(rect);
        flag.Add(union);

        float h_si = 12.6f;    // horizontal star interval
        float v_si = 10.8f;    // vertical star interval
        for (int row = 0; row < 5; row++) {
            for (int col = 0; col < 6; col++) {
                Point star = new Point(h_si/2 + col * h_si, v_si/2 + row * v_si);
                star.SetShape(Point.STAR);
                star.SetRadius(3.0f);
                star.SetFillColor(Color.white);
                union.Add(star);
            }
        }
        for (int row = 0; row < 4; row++) {
            for (int col = 0; col < 5; col++) {
                Point star = new Point(h_si + col * h_si, v_si + row * v_si);
                star.SetShape(Point.STAR);
                star.SetRadius(3.0f);
                star.SetFillColor(Color.white);
                union.Add(star);
            }
        }
        flag.DrawOn(page);

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
        squareAnnotation.SetLocation(50f, 500f);
        squareAnnotation.SetSize(50f, 50f);
        squareAnnotation.SetFillColor(new float[] {0f, 0f, 1f});
        squareAnnotation.SetTitle("Hello, World!");
        squareAnnotation.SetContents("The quick brown fox jumps over the lazy dog.");
        squareAnnotation.DrawOn(page);

        PolygonAnnotation polygonAnnotation = new PolygonAnnotation();
        polygonAnnotation.SetLocation(100f, 500f);
        polygonAnnotation.SetVertices(new float[] {0f, 0f, 50f, 0f, 50f, 50f, 0f, 0f});
        polygonAnnotation.SetFillColor(Color.orange);
        polygonAnnotation.SetTitle("This is a test ...");
        polygonAnnotation.SetContents("The quick brown cat caught the lazy mouse.");
        polygonAnnotation.DrawOn(page);

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
