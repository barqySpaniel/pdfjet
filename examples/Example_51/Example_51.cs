using System;
using System.IO;
using System.Collections;
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
        f1.SetSize(8f);

        //Image image = new Image(pdf, "images/qrcode.png");
        //image.SetLocation(100f, 100f);

        Page page = new Page(pdf, Letter.PORTRAIT);

        TextBlock textBlock = new TextBlock(f1, "Hello, World");
        textBlock.SetLocation(50f, 50f);

        Table table1 = new Table(f1,f1);
        int tableWidth = 400;
        int numcols = 3;
        List<List<Cell>> tableData = new List<List<Cell>>();
        List<Cell> row = new List<Cell>();
        for (int i = 0; i < 3; i++) {
            TextBlock tb = new TextBlock(f1, "test");
            if (i == 0) {
                tb.SetText("View Panel this 123 this is more code to teste this hello world is  test to see what will happen Cell {i}");
            } else {
                tb.SetText("hello");
            }
            //tb.SetText();
            tb.SetBorderColor(new float[] {0,0,0});
            tb.SetBorderCornerRadius(1.0f);
            tb.SetHeight(91);

            Cell cell = new Cell(f1, "");
            //cell.SetColSpan(1);
            cell.SetTextBlock(tb);
            cell.SetWidth(tableWidth / numcols);
            row.Add(cell);
        }
        //row.Add(new Cell(font, ""));
        tableData.Add(row);
        table1.SetData(tableData);
        table1.SetLocation(100, 100);
        //table1.SetHeight(91);
        table1.DrawOn(page);
        pdf.Complete();
    }

    public static void Main(String[] args) {
        new Example_51();
    }
}   // End of Example_51.cs
