using System;
using System.Collections.Generic;
using System.IO;
using PDFjet.NET;
using PDFjet.NET.Fonts;

/**
 *  Example_46.cs
 */
public class Example_46 {
    public Example_46() {
        PDF pdf = new PDF(new BufferedStream(
            new FileStream("Example_46.pdf", FileMode.Create)));
        pdf.SetCompliance(Compliance.PDF_UA_1);

        // pdf.SetEncryption(new PDFEncryption(pdf, "hello", "world"));

        Font f1 = new Font(pdf, CoreFont.HELVETICA);
        f1.SetSize(36f);

        Page page = new Page(pdf, Letter.PORTRAIT);

        TextLine textLine = new TextLine(f1, "Hello, World!");
        textLine.SetLocation(100f, 100f);
        textLine.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        System.Diagnostics.Stopwatch sw =
                System.Diagnostics.Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_46();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_46", time0, time1);
    }
}   // End of Example_46.cs
