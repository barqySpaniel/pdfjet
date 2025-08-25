using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class FormXObject : Canvas {
    public int objNumber;
    private float x;
    private float y;
    private float rotateDegrees = 0f;
    private List<TextLine> textLines = new List<TextLine>();

    public FormXObject(PDF pdf, float width, float height) : base(pdf) {
        base.width = width;
        base.height = height;
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetLocation(double x, double y) {
        this.x = (float)x;
        this.y = (float)y;
    }

    public void SetRotateDegreesCW(float degrees) {
        this.rotateDegrees = -degrees;
    }

    public void SetRotateDegreesCW(double degrees) {
        this.rotateDegrees = (float) -degrees;
    }

    public void SetRotateDegreesCCW(float degrees) {
        this.rotateDegrees = degrees;
    }

    public void SetRotateDegreesCCW(double degrees) {
        this.rotateDegrees = (float) degrees;
    }

    public void Add(TextLine text) {
        this.textLines.Add(text);
    }

    public void Complete() {
        pdf.NewObj();
        pdf.Append(Token.BeginDictionary);
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Form\n");

        pdf.Append("/BBox [0 0 ");
        pdf.Append(FastFloat.ToByteArray(width));
        pdf.Append(' ');
        pdf.Append(FastFloat.ToByteArray(height));
        pdf.Append("]\n");

        pdf.Append("/Resources <<>>\n");    // Must be here even if empty!!
        pdf.Append("/Length ");
        pdf.Append(buf.Length);
        pdf.Append(Token.Newline);
        pdf.Append(Token.EndDictionary);    // End of XObject dictionary
        pdf.Append(Token.Stream);
        pdf.Append(buf.ToArray());
        pdf.Append(Token.EndStream);
        pdf.EndObj();
        pdf.formXObjects.Add(this);
        objNumber = pdf.GetObjNumber();
    }

    public float[] DrawOn(Page page) {
        // page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n"); // Save the graphics state

        float drawX = this.x;
        float drawY = (page.height - this.height) - this.y;

        // 5. POSITION: move to desired location on page
        page.Append("1 0 0 1 ");
        page.Append(drawX);
        page.Append(' ');
        page.Append(drawY);
        page.Append(" cm\n");

        // 4. MOVE BACK: after rotation
        page.Append("1 0 0 1 ");
        page.Append(width/2);
        page.Append(' ');
        page.Append(height/2);
        page.Append(" cm\n");

        // 3. ROTATE: rotate around origin
        double radians = rotateDegrees * (Math.PI / 180);
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        page.Append(FastFloat.ToByteArray(cos));
        page.Append(' ');
        page.Append(FastFloat.ToByteArray(sin));
        page.Append(' ');
        page.Append(FastFloat.ToByteArray(-sin));
        page.Append(' ');
        page.Append(FastFloat.ToByteArray(cos));
        page.Append(" 0 0 cm\n");

        // 2. MOVE: move the center of the object to origin
        page.Append("1 0 0 1 ");
        page.Append(-width/2);
        page.Append(' ');
        page.Append(-height/2);
        page.Append(" cm\n");

        // 1. DRAW: draw the object
        page.Append("/Fm");
        page.Append(objNumber);
        page.Append(" Do\n");

        page.Append("BT\n");
        foreach (TextLine textLine in textLines) {
            Font font = textLine.GetFont();
            if (font.fontID != null) {
                page.Append('/');
                page.Append(font.fontID);
            } else {
                page.Append("/F");
                page.Append(font.objNumber);
            }
            page.Append(' ');
            page.Append(textLine.GetFont().size);
            page.Append(" Tf\n");

            page.Append("1 0 0 1 ");
            page.Append(textLine.x);
            page.Append(' ');
            page.Append(height - textLine.y);
            page.Append(" Tm\n");

            float[] textColor = textLine.GetTextColor();
            page.Append(textColor[0]);
            page.Append(' ');
            page.Append(textColor[1]);
            page.Append(' ');
            page.Append(textColor[2]);
            page.Append(" rg\n");

            page.Append("<");
            page.DrawUnicodeString(textLine.GetFont(), textLine.GetText());
            page.Append("> Tj\n");
        }
        page.Append("ET\n");

        page.Append("Q\n"); // Restore the graphics state
        // page.AddEMC();

        return new float[] { this.x + width, this.y + height };
    }
}   // End of FormXObject.cs
}   // End of PDFjet.NET
