using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PDFjet.NET;

/**
 * Example_29.cs
 */
public class Example_29 {
    public Example_29() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_29.pdf", FileMode.Create)));

        Page page = new Page(pdf, Letter.PORTRAIT);

        Font font = new Font(pdf, IBMPlexSans.Regular);
        font.SetSize(15f);

        Paragraph paragraph1 = new Paragraph();
        paragraph1.Add(new TextLine(font, Content.OfTextFile("data/languages/english.txt")));

        Paragraph paragraph2 = new Paragraph();
        paragraph2.Add(new TextLine(font, Content.OfTextFile("data/languages/greek.txt")));

        TextColumn column = new TextColumn();
        column.SetLocation(50f, 50f);
        column.SetSize(540f, 0f);
        column.AddParagraph(paragraph1);
        column.AddParagraph(paragraph2);
        // column.DrawOn(page);

        List<List<Cell>> tableData = new List<List<Cell>>();
        List<Cell> row = new List<Cell>();
        row.Add(new Cell(font, "Hello"));
        row.Add(new Cell(font, "World"));
        // cell.SetTextColumn(column);
        tableData.Add(row);

        Table table1 = new Table(font, font);
        table1.SetData(tableData);
        table1.SetLocation(50f, 50f);
        table1.DrawOn(page);

        pdf.Complete();
    }

    public static void Main(String[] args) {
        Stopwatch sw = Stopwatch.StartNew();
        long time0 = sw.ElapsedMilliseconds;
        new Example_29();
        long time1 = sw.ElapsedMilliseconds;
        sw.Stop();
        TextUtils.PrintDuration("Example_29", time0, time1);
    }
}   // End of Example_29.cs
