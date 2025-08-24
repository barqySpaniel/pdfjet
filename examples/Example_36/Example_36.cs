using System;
using System.IO;
using System.Diagnostics;
using PDFjet.NET;

/**
 *  Example_36.cs
 */
public class Example_36 {
    public Example_36() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_36.pdf", FileMode.Create)));

        Image image = new Image(pdf, "images/ee-map.png");

        FormXObject form = new FormXObject(pdf, 100, 100);
        form.MoveTo(0f, 0f);
        form.LineTo(50f, 50f);
        form.StrokePath();
        form.AddToPDF(pdf);

        Page page = new Page(pdf, Letter.PORTRAIT);

        image.SetLocation(100f, 100f);
        image.DrawOn(page);

        // form.SetLocation(200f, 200f);
        form.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_36();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_36", time0, time1);
    }
}   // End of Example_36.cs
