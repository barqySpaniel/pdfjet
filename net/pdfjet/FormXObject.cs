using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class FormXObject : Canvas {
    private int objectNumber;
    private Dictionary<string, int> resourceRefs;

    public FormXObject(int objectNumber, float width, float height) {
        this.objectNumber = objectNumber;
        base.width = width;
        base.height = height;
        this.resourceRefs = new Dictionary<string, int>();
    }

    public int GetObjectNumber() {
        return objectNumber;
    }

    private void SetObjectNumber(int value) {
        objectNumber = value;
    }

//    public float GetWidth() {
//        return width;
//    }
//
//    public float GetHeight() {
//        return height;
//    }

    private void WriteString(String s) {
        var bytes = Encoding.ASCII.GetBytes(s);
        buf.Write(bytes, 0, bytes.Length);
    }

//    public void SetFillColorRGB(float r, float g, float b) {
//        buf.Write($"{r} {g} {b} rg\n");
//    }
//
//    public void SetStrokeColorRGB(float r, float g, float b) {
//        buf.Write($"{r} {g} {b} RG\n");
//    }
//
//    public void FillRectangle(float x, float y, float w, float h) {
//        buf.Write($"{x} {y} {w} {h} re\nf\n");
//    }

//    public void DrawRectangle(float x, float y, float w, float h) {
//        buf.Write($"{x} {y} {w} {h} re\nS\n");
//    }

    public void Stroke() {
        WriteString("S\n");
    }

    public void AddResource(string name, int objectNumber) {
        resourceRefs[name] = objectNumber;
    }

    public byte[] GetStreamData() {
        return buf.ToArray();
    }

    public string ToPdfObject() {
        var dict = new StringBuilder();

        dict.AppendLine("<<");
        dict.AppendLine("/Type /XObject");
        dict.AppendLine("/Subtype /Form");
        dict.AppendFormat("/BBox [0 0 {0} {1}]\n", width, height);

        if (resourceRefs.Count > 0) {
            dict.AppendLine("/Resources <<");
            foreach (var kv in resourceRefs)
                dict.AppendFormat("/{0} {1} 0 R\n", kv.Key, kv.Value);
            dict.AppendLine(">>");
        }

        dict.AppendFormat("/Length {0}\n", buf.Length);
        dict.AppendLine(">>");

        return $"{objectNumber} 0 obj\n{dict}stream\n" +
               Encoding.ASCII.GetString(GetStreamData()) +
               "endstream\nendobj\n";
    }
}   // End of FormXObject.cs
}   // End of PDFjet.NET
