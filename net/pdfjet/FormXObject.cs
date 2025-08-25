using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class FormXObject : Canvas {
    public int objNumber;
    private Dictionary<string, int> resourceRefs;
    private float x;
    private float y;
    private float rotateDegrees = 0f;

    public FormXObject(PDF pdf, float width, float height) : base(pdf) {
        base.width = width;
        base.height = height;
        this.resourceRefs = new Dictionary<string, int>();
        float scalingFactor = 1f / (float)Math.Max(width, height);
Console.WriteLine(scalingFactor);
        Append(FastFloat.ToByteArray(scalingFactor));
        Append(" 0 0 ");
        Append(FastFloat.ToByteArray(scalingFactor));
        Append(" 0 0 cm\n");
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

    public void Complete() {
        pdf.NewObj();
        pdf.Append("<<\n");
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Form\n");

        pdf.Append("/BBox [0 0 1 1]\n");
//        pdf.Append("/BBox [0 0 ");
//        pdf.Append(width);
//        pdf.Append(' ');
//        pdf.Append(height);
//        pdf.Append("]\n");

        pdf.Append("/Resources <<\n");  // Must be here even if empty!!
        if (resourceRefs.Count > 0) {
            foreach (var kv in resourceRefs) {
                pdf.Append('/');
                pdf.Append(kv.Key);
                pdf.Append(' ');
                pdf.Append(kv.Value);
                pdf.Append(" 0 R\n");
            }
        }
        pdf.Append(">>\n");             // End of Resources
        pdf.Append("/Length ");
        pdf.Append(buf.Length);
        pdf.Append('\n');
        pdf.Append(">>\n");             // End of XObject dictionary
        pdf.Append("stream\n");
        pdf.Append(buf.ToArray());
        pdf.Append("\nendstream\n");
        pdf.EndObj();
        pdf.formXObjects.Add(this);
        objNumber = pdf.GetObjNumber();
    }

    public float[] DrawOn(Page page) {
        // page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n"); // Save the graphics state

        // Calculate the correct Y position for a top-down coordinate system.
        // We are given a desired (x, y) where (0,0) is the top-left of the page.
        // We need to convert this 'y' to the PDF's coordinate system where (0,0) is the bottom-left.
        float drawX = this.x;
        float drawY = (page.height - this.height) - this.y;  // The key calculation

        // Apply a simple translation matrix to move the FormXObject.
        // [1 0 0 1 drawX drawY] cm
        page.Append("1 0 0 1 ");
        page.Append(drawX);
        page.Append(' ');
        page.Append(drawY);
        page.Append(" cm\n");

        page.Append("1 0 0 1 0.5 0.5 cm\n");        // Move back

        // Rotates around current origin (0,0) by 'rotateDegrees'
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

        page.Append("1 0 0 1 -0.5 -0.5 cm\n");      // Move the center to (0,0)

        // Scale the Form XObject
        page.Append(FastFloat.ToByteArray(width));
        page.Append(" 0 0 ");
        page.Append(FastFloat.ToByteArray(height));
        page.Append(" 0 0 cm\n");

        // Draw the normalized 1×1 object
        page.Append("/Fm");
        page.Append(objNumber);
        page.Append(" Do\n");

        page.Append("Q\n"); // Restore the graphics state
        // page.AddEMC();

        return new float[] { this.x + width, this.y + height };
    }
}   // End of FormXObject.cs
}   // End of PDFjet.NET
