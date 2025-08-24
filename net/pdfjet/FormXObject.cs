using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class FormXObject : Canvas {
    public int objNumber = -1;
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

    public float[] DrawOn(Page page, float x, float y) {
//        // 1. CHECK IF THE FORM HAS BEEN ADDED TO THE PDF YET
//        //    (objNumber is invalid until AddToPDF is called)
//        if (this.objNumber == -1) {
//            // 2. IF NOT, ADD IT NOW.
//            this.AddToPDF(page.pdf);
//        }

        // page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n");

        // page.ScaleAndRotate(x, y, width, height - y, 0f);
        // page.Append("300 300 cm\n");

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
