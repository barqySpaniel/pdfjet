using System;
using System.IO;
using System.Diagnostics;
using PDFjet.NET;
using System.Collections.Generic;

/**
 *  Example_32.cs
 */
public class Example_32 {
    public Example_32() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_32.pdf", FileMode.Create)));

        Font font = new Font(pdf, "fonts/JetBrainsMono/JetBrainsMono-Regular.ttf.stream");
        font.SetSize(10f);

        Dictionary<String, Int32> colors = new Dictionary<String, Int32>();
        colors["new"] = Color.red;
        colors["ArrayList"] =  Color.blue;
        colors["List"] = Color.blue;
        colors["String"] = Color.blue;
        colors["Field"] = Color.blue;
        colors["Form"] = Color.blue;
        colors["Smart"] = Color.green;
        colors["Widget"] = Color.green;
        colors["Designs"] = Color.green;

        Page page = new Page(pdf, Letter.PORTRAIT);

        var point = new Point(350f, 75f);
        point.SetRadius(25f);
        point.SetFillColor(Color.limegreen);
        point.SetStrokeColor(Color.black);
        point.SetStrokeWidth(1f);
        point.SetShape(Point.CIRCLE);
        point.DrawOn(page);

        point = new Point(450f, 75f);
        point.SetRadius(25f);
        point.SetShape(Point.DIAMOND);
        point.SetStrokeColor(Color.blue);
        point.DrawOn(page);

        point = new Point(550f, 75f);
        point.SetRadius(25f);
        point.SetShape(Point.STAR);
        point.SetFillColor(Color.lightblue);
        point.DrawOn(page);

        float x = 50f;
        float y = 50f;
        float leading = font.GetBodyHeight(font.GetSize());
        List<String> lines = Text.ReadLines("examples/Example_02/Example_02.cs");
        foreach (String line in lines) {
            page.DrawString(font, font.GetSize(), line, x, y, new float[] {0f, 0f, 0f}, colors);
            y += leading;
            if (y > (page.GetHeight() - 20f)) {
                page = new Page(pdf, Letter.PORTRAIT);
                y = 50f;
            }
        }

        List<List<Cell>> tableData = new List<List<Cell>>();

        List<Cell> row = new List<Cell>();
        TextBox textBox = new TextBox(font, "View Panel:");
        row.Add(new Cell(font).SetColSpan(2).SetTextBox(textBox));
        row.Add(new Cell(font, ""));
        tableData.Add(row);

        row = new List<Cell>();
        textBox = new TextBox(font,
            "ROTATION:  Hello, World! This is a Rotation test.").SetTextAlignment(Align.RIGHT);
        row.Add(new Cell(font).SetTextBox(textBox));
        textBox = new TextBox(font, "0.0");
        row.Add(new Cell(font).SetTextBox(textBox));
        tableData.Add(row);

        row = new List<Cell>();
        textBox = new TextBox(font, "X:").SetTextAlignment(Align.RIGHT);
        row.Add(new Cell(font).SetTextBox(textBox));
        textBox = new TextBox(font, "0");
        row.Add(new Cell(font).SetTextBox(textBox));
        tableData.Add(row);

        row = new List<Cell>();
        row.Add(new Cell(font, "Y:").SetTextAlignment(Align.RIGHT));
        row.Add(new Cell(font, "0"));
        tableData.Add(row);

        row = new List<Cell>();
        row.Add(new Cell(font, "OBJECT:").SetTextAlignment(Align.RIGHT));
        row.Add(new Cell(font, "-"));
        tableData.Add(row);

        row = new List<Cell>();
        row.Add(new Cell(font, "LON:").SetTextAlignment(Align.RIGHT));
        row.Add(new Cell(font, "73°58′59″W"));
        tableData.Add(row);

        row = new List<Cell>();
        row.Add(new Cell(font, "LAT:").SetTextAlignment(Align.RIGHT));
        row.Add(new Cell(font, "40°45′11″N"));
        tableData.Add(row);

        Table table = new Table();
        table.SetData(tableData);
        table.SetLocation(450f, 450f);
        table.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_32();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_32", time0, time1);
    }
}   // End of Example_32.cs
