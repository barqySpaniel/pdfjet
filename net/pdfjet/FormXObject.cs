using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class FormXObject : Canvas {
    public int objNumber = -1;
    private Dictionary<string, int> resourceRefs;

    public FormXObject(PDF pdf, float width, float height) : base(pdf) {
        base.width = width;
        base.height = height;
        this.resourceRefs = new Dictionary<string, int>();
    }

    public void AddToPDF(PDF pdf) {
        pdf.Newobj();
        Append("<<\n");
        Append("/Type /XObject\n");
        Append("/Subtype /Form\n");
        Append("/BBox [0 0 ");
        Append(width);
        Append(' ');
        Append(height);
        Append("]\n");
        Append("/Resources <<\n");      // Must be here even if empty!!
        if (resourceRefs.Count > 0) {
            foreach (var kv in resourceRefs) {
                Append('/');
                Append(kv.Key);
                Append(' ');
                Append(kv.Value);
                Append(" 0 R\n");
            }
        }
        Append(">>\n");                 // End of Resources
        Append("/Length ");
        Append(buf.Length);
        Append('\n');
        Append(">>\n");                 // End of XObject dictionary
        Append("stream\n");
        Append(buf.ToArray());
        Append("\nendstream\n");
        pdf.Endobj();
        pdf.formXObjects.Add(this);
        objNumber = pdf.GetObjNumber();
    }

    public float[] DrawOn(Page page) {
        // 1. CHECK IF THE FORM HAS BEEN ADDED TO THE PDF YET
        //    (objNumber is invalid until AddToPDF is called)
        if (this.objNumber == -1) {
            // 2. IF NOT, ADD IT NOW.
            this.AddToPDF(page.pdf);
        }

        // page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n");

        // page.ScaleAndRotate(x, y, w, h, degrees);
        page.Append("300 300 cm\n");

        page.Append("/Fm");
        page.Append(objNumber);
        page.Append(" Do\n");

        page.Append("Q\n");
        // page.AddEMC();
        return new float[] { 0f, 0f };
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
