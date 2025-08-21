using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class FormXObject : Canvas {
    private PDF pdf;
    private int objectNumber;
    private Dictionary<string, int> resourceRefs;

    public FormXObject(PDF pdf, int objectNumber, float width, float height) {
        this.pdf = pdf;
        base.width = width;
        base.height = height;
        this.objectNumber = objectNumber;
        this.resourceRefs = new Dictionary<string, int>();
    }

    public int GetObjectNumber() {
        return objectNumber;
    }

    private void SetObjectNumber(int objectNumber) {
        this.objectNumber = objectNumber;
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

    public void AddResource(string name, int objectNumber) {
        resourceRefs[name] = objectNumber;
    }

    public byte[] GetStreamData() {
        return buf.ToArray();
    }

    public void ToPdfObject() {
        pdf.Newobj();
        Append("<<\n");
        Append("/Type /XObject\n");
        Append("/Subtype /Form\n");
        Append("/BBox [0 0 ");
        Append(width);
        Append(' ');
        Append(height);
        Append("]\n")
        Append("/Resources <<\n");
        if (resourceRefs.Count > 0) {
            foreach (var kv in resourceRefs) {
                Append('/');
                Append(kv.Key);
                Append(' ');
                Append(kv.Value);
                Append(" 0 R\n");
            }
        }
        Append(">>\n");         // End of Resources
        Append("/Length ");
        Append(buf.Length);
        Append('\n');
        Append(">>\n");         // End of XObject dictionary
        Append("stream\n");
        Append(buf, 0, buf.Length);
        Append("\nendstream\n");
        pdf.Endobj();        // TODO:
        // page.images.Add(this);
        // objNumber = pdf.GetObjNumber();
    }
}   // End of FormXObject.cs
}   // End of PDFjet.NET
