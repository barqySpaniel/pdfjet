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

    public FormXObject(PDF pdf, float width, float height) : base(pdf) {
        base.width = width;
        base.height = height;
        this.resourceRefs = new Dictionary<string, int>();
    }

    public void AddToPDF(PDF pdf) {
        pdf.Newobj();
        pdf.Append("<<\n");
        pdf.Append("/Type /XObject\n");
        pdf.Append("/Subtype /Form\n");
        pdf.Append("/BBox [0 0 ");
        pdf.Append(width);
        pdf.Append(' ');
        pdf.Append(height);
        pdf.Append("]\n");
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
        pdf.Endobj();
        pdf.formXObjects.Add(this);
        objNumber = pdf.GetObjNumber();
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public float[] DrawOn(Page page) {
        // page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n"); // Save the graphics state

        // Calculate the correct Y position for a top-down coordinate system.
        // We are given a desired (x, y) where (0,0) is the top-left of the page.
        // We need to convert this 'y' to the PDF's coordinate system where (0,0) is the bottom-left.
        float pageHeight = page.GetHeight();
        float drawX = this.x;
        float drawY = (pageHeight - this.height) - this.y;  // The key calculation

        // Apply a simple translation matrix to move the FormXObject.
        // [1 0 0 1 drawX drawY] cm
        page.Append("1 0 0 1 ");
        page.Append(drawX);
        page.Append(' ');
        page.Append(drawY);
        page.Append(" cm\n");

        // Draw the Form XObject. Its internal (0,0) will now be at (drawX, drawY).
        page.Append("/Fm");
        page.Append(objNumber);
        page.Append(" Do\n");

        page.Append("Q\n"); // Restore the graphics state
        // page.AddEMC();

        return new float[] { this.x + width, this.y + height };
    }

    public int GetObjectNumber() {
        return objNumber;
    }

    private void WriteString(String s) {
        var bytes = Encoding.ASCII.GetBytes(s);
        buf.Write(bytes, 0, bytes.Length);
    }

    public void SetFillColorRGB(float r, float g, float b) {
        WriteString($"{r} {g} {b} rg\n");
    }

    public void SetStrokeColorRGB(float r, float g, float b) {
        WriteString($"{r} {g} {b} RG\n");
    }

    public void FillRectangle(float x, float y, float w, float h) {
        WriteString($"{x} {y} {w} {h} re\nf\n");
    }

    public void DrawRectangle(float x, float y, float w, float h) {
        WriteString($"{x} {y} {w} {h} re\nS\n");
    }

    public void Stroke() {
        WriteString("S\n");
    }

    public void AddResource(string name, int objNumber) {
        resourceRefs[name] = objNumber;
    }

    public byte[] GetFormXObjectData() {
        return buf.ToArray();
    }
}   // End of FormXObject.cs
}   // End of PDFjet.NET
