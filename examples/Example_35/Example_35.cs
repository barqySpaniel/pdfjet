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

        // Font f1 = new Font(pdf, CoreFont.HELVETICA_BOLD);
        // f1.SetSize(8f);

        // Font f2 = new Font(pdf, CoreFont.HELVETICA);
        // f2.SetSize(8f);

        Font f1 = new Font(pdf, IBMPlexSans.Bold);
        f1.SetSize(8f);

        Font f2 = new Font(pdf, IBMPlexSans.Regular);
        f2.SetSize(8f);

        List<List<Point>> chartData = new List<List<Point>>();

        List<Point> path1 = new List<Point>();
        path1.Add(new Point(50f, 50f).SetStartOfPath()); // .SetStrokeColor(Color.blue));
        path1.Add(new Point(55f, 55f));
        path1.Add(new Point(60f, 60f));
        path1.Add(new Point(65f, 58f));
        path1.Add(new Point(70f, 59f));
        path1.Add(new Point(75f, 63f));
        path1.Add(new Point(80f, 65f));
        chartData.Add(path1);

        List<Point> path2 = new List<Point>();
        path2.Add(new Point(50f, 30f).SetStartOfPath().SetStrokeColor(Color.red));
        path2.Add(new Point(55f, 35f));
        path2.Add(new Point(60f, 40f));
        path2.Add(new Point(65f, 48f));
        path2.Add(new Point(70f, 49f));
        path2.Add(new Point(75f, 53f));
        path2.Add(new Point(80f, 55f));
        chartData.Add(path2);

        List<Point> path3 = new List<Point>();
        path3.Add(new Point(50f, 80f).SetStartOfPath().SetStrokeColor(Color.green));
        path3.Add(new Point(55f, 70f));
        path3.Add(new Point(60f, 60f));
        path3.Add(new Point(65f, 55f));
        path3.Add(new Point(70f, 59f));
        path3.Add(new Point(75f, 63f));
        path3.Add(new Point(80f, 61f));
        chartData.Add(path3);

        Chart chart = new Chart(f1, f2);
        chart.SetData(chartData);
        chart.SetLocation(70f, 50f);
        chart.SetSize(500f, 300f);
        chart.SetTitle("Chart Title");
        chart.SetXAxisTitle("X Axis Title");
        chart.SetYAxisTitle("Y Axis Title");

        // You can adjust the X and Y min and max manually:
        // chart.SetXAxisMinMax(45f, 80f, 7);
        // chart.SetYAxisMinMax(20f, 80f, 6);
        chart.DrawOn(page);


        Rect rect = new Rect(0f, 0f, 300f, 300f);
        rect.SetLocation(0f, 0f);
        rect.SetBorderColor(Color.blue);

        TextLine text = new TextLine(font, "Yahoo!");
        text.SetLocation(25f, 25f);
        text.SetFontSize(16f);
        text.SetTextColor(Color.blue);

        Container c = new Container(300f, 300f);
        c.SetLocation(150f, 250f);
        c.Add(rect);
        c.Add(text);
        c.SetRotateDegreesCW(50);
        c.DrawOn(page);

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
