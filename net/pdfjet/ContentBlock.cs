using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class ContentBlock {
    private int objectNumber;
    private float width;
    private float height;
    private MemoryStream stream;
    private Dictionary<string, int> resourceRefs;

    public ContentBlock(int objectNumber, float width, float height) {
        this.objectNumber = objectNumber;
        this.width = width;
        this.height = height;
        this.stream = new MemoryStream();
        this.resourceRefs = new Dictionary<string, int>();
    }

    public int GetObjectNumber() {
        return objectNumber;
    }

    private void SetObjectNumber(int value) {
        objectNumber = value;
    }

    public float GetWidth() {
        return width;
    }

    public float GetHeight() {
        return height;
    }

    private void Write(string s) {
        var bytes = Encoding.ASCII.GetBytes(s);
        stream.Write(bytes, 0, bytes.Length);
    }

    public void SetFillColorRGB(float r, float g, float b) {
        Write($"{r} {g} {b} rg\n");
    }

    public void SetStrokeColorRGB(float r, float g, float b) {
        Write($"{r} {g} {b} RG\n");
    }

    public void FillRectangle(float x, float y, float w, float h) {
        Write($"{x} {y} {w} {h} re\nf\n");
    }

    public void DrawRectangle(float x, float y, float w, float h) {
        Write($"{x} {y} {w} {h} re\nS\n");
    }

    public void MoveTo(float x, float y) {
        Write($"{x} {y} m\n");
    }

    public void LineTo(float x, float y) {
        Write($"{x} {y} l\n");
    }

    public void Stroke() {
        Write("S\n");
    }

    public void AddResource(string name, int objectNumber) {
        resourceRefs[name] = objectNumber;
    }

    public void WriteRaw(string s) {
        Write(s);
    }

    public byte[] GetStreamData() {
        return stream.ToArray();
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

        dict.AppendFormat("/Length {0}\n", stream.Length);
        dict.AppendLine(">>");

        return $"{objectNumber} 0 obj\n{dict}stream\n" +
               Encoding.ASCII.GetString(GetStreamData()) +
               "endstream\nendobj\n";
    }
}
}