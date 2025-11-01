using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using PDFjet.NET;

/**
 *  Example_29.cs
 */
public class Example_29 {
    public Example_29() {
        PDF pdf = new PDF(new BufferedStream(
                new FileStream("Example_29.pdf", FileMode.Create)));

        Font font = new Font(pdf, CoreFont.HELVETICA);
        font.SetSize(16f);

        Page page = new Page(pdf, Letter.LANDSCAPE);

        Paragraph paragraph = new Paragraph();
        Paragraph paragraph2 = new Paragraph();
        // a Paragraph has no size, just the list of
        // internal List<TextLine> lines = null;
        // each TextLine has its own settings
        paragraph.Add(new TextLine(font, "Lorem ipsum dolor sit amet, consectetur adipiscing elit."));
        paragraph.Add(new TextLine(font, " Nulla elementum interdum elit, quis vehicula urna interdum quis. "));
        paragraph.Add(new TextLine(font, "Phasellus gravida ligula quam, nec blandit nulla. Sed posuere, lorem eget feugiat placerat, ipsum nulla euismod nisi, in semper mi nibh sed elit. "));
        paragraph.Add(new TextLine(font, "Mauris libero est, sodales dignissim congue sed, pulvinar non ipsum. "));
        paragraph.Add(new TextLine(font, "Sed risus nisi, ultrices nec eleifend at, viverra sed neque. "));
        paragraph2.Add(new TextLine(font, "Integer vehicula massa non arcu viverra ullamcorper. "));
        paragraph2.Add(new TextLine(font, "Ut id tellus id ante mattis commodo. "));
        paragraph2.Add(new TextLine(font, "Donec dignissim aliquam tortor, eu pharetra ipsum ullamcorper in. "));
        paragraph2.Add(new TextLine(font, "Vivamus ultrices imperdiet iaculis."));
        List<TextLine> lines = paragraph.GetTextLines();

        float r = 0.1f, g = 0.2f, b = 0f;
        float fontSize = 8f;
        int i = 0;
        foreach (var line in lines) {
            line.SetTextColor(r, g, b);
            r = (r + 0.1f) - MathF.Truncate(r + 0.1f);
            g = (g + 0.3f) - MathF.Truncate(g + 0.3f);
            b = (b + 0.2f) - MathF.Truncate(b + 0.2f);
            line.SetFontSize(fontSize);
            //line.SetFontSize(i % 3 == 0 ? fontSize + i * 2f : fontSize);
            //font.SetSize(i % 3 == 0 ? fontSize + i * 2f : fontSize);
            //font.SetItalic(i % 4 == 0);
            //line.SetFont(font);
            line.SetUnderline(i % 2 == 0);
            line.SetStrikeout(i % 2 == 0);
            line.SetTextEffect(i % 2 != 0 ? Effect.SUBSCRIPT : Effect.NORMAL);
            i += 1;
        }

        TextColumn column = new TextColumn();
        column.SetLocation(50f, 50f);
        column.SetSize(420f, 0f);

        column.SetLineBetweenParagraphs(false);
        column.AddParagraph(paragraph);
        column.AddParagraph(paragraph2);

        Table table1 = new Table(font,font);
        List<List<Cell>> tableData = new List<List<Cell>>();
        List<Cell> row = new List<Cell>();
        Cell cell = new Cell(font, "");
        cell.SetTextColumn(column);
        // cell.SetWidth(400);
        cell.SetStrokeColor(Color.red);
        cell.SetLineWidth(2f);
        row.Add(cell);
        tableData.Add(row);

        //table1.DrawOn(page);//column.DrawOn(page);

        float[] point2 = column.DrawOn(null);


        TextBlock tb = new TextBlock(font,
                "Peter Blood, bachelor of medicine and several other things besides, smoked a pipe and tended the geraniums boxed on the sill of his window above Water Lane in the town of Bridgewater.");
                //List<Cell> row2 = new List<Cell>();
        tb.SetFontSize(16f);
        Cell cell2 = new Cell(font, "");
        cell2.SetTextBlock(tb);
        cell2.SetWidth(200);
        row.Add(cell2);
        //tableData.Add(row2);
        table1.SetData(tableData);
        table1.SetLocation(50f, 50f);
        float[] xy = table1.DrawOn(page);//column.DrawOn(page);

        Box box = new Box();
        box.SetLocation(xy[0], xy[1] + 5);
        box.SetSize(540f, 25f);
        box.SetLineWidth(2f);
        box.SetColor(Color.darkblue);
        box.DrawOn(page);

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
