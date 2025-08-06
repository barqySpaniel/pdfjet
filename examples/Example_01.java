package examples;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.HashMap;
import java.util.Map;

import com.pdfjet.*;
import com.pdfjet.font.*;

/**
 *  Example_01.java
 *  
 *  A simple example demonstrating how to create a PDF with multilingual text
 *  (English, Greek, and Bulgarian) using PDFjet.
 */
public class Example_01 {

    // Constructor to generate the PDF document
    public Example_01() throws Exception {
        // Create a new PDF document and set output stream to a file
        PDF pdf = new PDF(new BufferedOutputStream(new FileOutputStream("Example_01.pdf")));
        
        // Set PDF/UA compliance (required for accessibility)
        pdf.setCompliance(Compliance.PDF_UA);

        // Set the title of the document (required for PDF/UA compliance)
        pdf.setTitle("Document containing English, Greek and Bulgarian text blocks.");

        // Load the font (using IBMPlexSans Regular)
        Font font = new Font(pdf, IBMPlexSans.Regular);
        font.setSize(12f);

        // Create a new page with Portrait orientation
        Page page = new Page(pdf, Letter.PORTRAIT);

        Map<String, Integer> map = new HashMap<String, Integer>();
        map.put("Everyone", Color.red);
        map.put("pay", Color.green);
        map.put("freedom", Color.blue);

        // Add English text from a file
        TextBlock textBlock = new TextBlock(
            font, Content.ofTextFile("data/languages/english.txt"));
        textBlock.setLocation(50f, 50f);
        textBlock.setWidth(473f);   // Why 473f? To match the Google Fonts samples.
        textBlock.setTextPadding(10f);
        textBlock.setKeywordHighlightColors(map);
        float[] xy = textBlock.drawOn(page);

        // Draw a small blue rectangle for testing ...
        Rect rect = new Rect(xy[0], xy[1], 30f, 30f);
        rect.setBorderColor(Color.blue);
        rect.drawOn(page);

        // Add Greek text from a file
        textBlock = new TextBlock(font, Content.ofTextFile("data/languages/greek.txt"));
        textBlock.setLocation(50f, xy[1] + 30f);
        textBlock.setWidth(473f);
        textBlock.setTextPadding(10f);
        textBlock.setBorderColor(Color.none); // No border for Greek text
        xy = textBlock.drawOn(page);

        // Add Bulgarian text from a file with a blue border and rounded corners
        textBlock = new TextBlock(font, Content.ofTextFile("data/languages/bulgarian.txt"));
        textBlock.setLocation(50f, xy[1] + 30f);
        textBlock.setWidth(473f);
        textBlock.setTextPadding(10f);
        textBlock.setBorderColor(Color.blue);
        textBlock.setBorderCornerRadius(10f);
        textBlock.drawOn(page);

        // Complete the PDF document creation
        pdf.complete();
    }

    // Main method to run the example and measure execution time
    public static void main(String[] args) throws Exception {
        long time0 = System.currentTimeMillis();
        new Example_01();  // Create the PDF
        long time1 = System.currentTimeMillis();
        TextUtils.printDuration("Example_01", time0, time1);  // Print the duration of the process
    }
}   // End of Example_01.java
