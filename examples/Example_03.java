package examples;

import java.io.*;
import com.pdfjet.*;
import com.pdfjet.fonts.*;

/**
 *  Example_03.java
 */
public class Example_03 {
    public Example_03() throws Exception {
        PDF pdf = new PDF(
                new BufferedOutputStream(new FileOutputStream("Example_03.pdf")));
        pdf.setCompliance(Compliance.PDF_UA_1);

        pdf.setKeywords("React Vue Java Python");
        // Font f1 = new Font(pdf, CoreFont.HELVETICA);
        Font f1 = new Font(pdf, IBMPlexSans.Regular);

        Image image1 = new Image(pdf, "images/ee-map.png");
        Image image2 = new Image(pdf, "images/fruit.jpg");
        Image image3 = new Image(pdf, "images/mt-map.bmp");

        Page page = new Page(pdf, A4.PORTRAIT);

        TextLine text = new TextLine(f1,
                "React The map below is an embedded PNG image");
        text.setLocation(20f, 20f);
        text.setStrikeout(true);
        text.setUnderline(true);
        text.setURIAction("https://en.wikipedia.org/wiki/European_Union");
        float[] point = text.drawOn(page);

        image1.setLocation(90f, point[1] + f1.getDescent() + 5f);
        image1.scaleBy(2f/3f);
        image1.drawOn(page);

        text.setText("JPG image file embedded once and drawn 3 times");
        text.setLocation(90f, 550f);
        point = text.drawOn(page);

        image2.setLocation(90f, point[1] + f1.getDescent());
        image2.scaleBy(0.5f);
        image2.drawOn(page);

        image2.setLocation(260f, point[1] + f1.getDescent());
        image2.scaleBy(0.5f);
        image2.rotateClockwise(90);
        image2.drawOn(page);

        image2.setLocation(350f, point[1] + f1.getDescent());
        image2.rotateClockwise(0);
        image2.scaleBy(0.5f);
        image2.drawOn(page);

        text.setText("The map on the right is an embedded BMP image");
        text.setUnderline(true);
        text.setVerticalOffset(3f);
        text.setStrikeout(true);
        text.setTextDirection(15);
        text.setLocation(90f, 800f);
        text.drawOn(page);

        image3.setLocation(390f, 630f);
        image3.scaleBy(0.5f);
        image3.drawOn(page);

        Page page2 = new Page(pdf, A4.PORTRAIT);
        float[] xy = image1.drawOn(page2);

        Box box = new Box();
        box.setLocation(xy[0], xy[1]);
        box.setSize(20f, 20f);
        box.drawOn(page2);

        pdf.complete();
    }

    public static void main(String[] args) throws Exception {
        long time0 = System.currentTimeMillis();
        new Example_03();
        long time1 = System.currentTimeMillis();
        TextUtils.printDuration("Example_03", time0, time1);
    }
}   // End of Example_03.java
