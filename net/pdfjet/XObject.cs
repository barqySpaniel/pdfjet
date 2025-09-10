/**
 * XObject.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class XObject : IDrawable {
    internal int objNumber;

    private PDF pdf;
    private float x;
    private float y;
    private float width;
    private float height;
    private float[] fillColor;
    private float[] strokeColor;
    private float strokeWidth = 1f;
    private float rotateDegrees = 0f;
    private MemoryStream buf = new MemoryStream();
    private List<Font> fonts = new List<Font>();

    public XObject(PDF pdf, float width, float height) {
        this.pdf = pdf;
        this.width = width;
        this.height = height;
    }

    public void AddFontResource(Font font) {
        fonts.Add(font);
    }

    public void SetPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetLocation(double x, double y) {
        this.x = (float)x;
        this.y = (float)y;
    }

    private void Append(float value) {
        byte[] bytes = FastFloat.ToByteArray(value);
        buf.Write(bytes, 0, bytes.Length);
    }

    private void Append(String str) {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        buf.Write(bytes, 0, bytes.Length);
    }

    public void SetFillColor(float[] rgbColor) {
        Append(rgbColor[0]);
        Append(" ");
        Append(rgbColor[1]);
        Append(" ");
        Append(rgbColor[2]);
        Append(" rg\n");
        this.fillColor = rgbColor;
    }

    public void SetFillColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        Append(r);
        Append(" ");
        Append(g);
        Append(" ");
        Append(b);
        Append(" rg\n");
        this.fillColor = new float[] {r, g, b};
    }

    public void SetStrokeColor(float[] rgbColor) {
        Append(rgbColor[0]);
        Append(" ");
        Append(rgbColor[1]);
        Append(" ");
        Append(rgbColor[2]);
        Append(" RG\n");
        this.strokeColor = rgbColor;
    }

    public void SetStrokeColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        Append(r);
        Append(" ");
        Append(g);
        Append(" ");
        Append(b);
        Append(" RG\n");
        this.fillColor = new float[] {r, g, b};
    }

    public void SetStrokeWidth(float width) {
        Append(width);
        Append(" w\n");
        this.strokeWidth = width;
    }

    public void MoveTo(float x, float y) {
        Append(x);
        Append(" ");
        Append(height - y);
        Append(" m\n");
    }

    public void LineTo(float x, float y) {
        Append(x);
        Append(" ");
        Append(height - y);
        Append(" l\n");
    }

    public void CurveTo(
            float x1,
            float y1,
            float x2,
            float y2,
            float x3,
            float y3) {
        Append(x1);
        Append(" ");
        Append(height - y1);
        Append(" ");
        Append(x2);
        Append(" ");
        Append(height - y2);
        Append(" ");
        Append(x3);
        Append(" ");
        Append(height - y3);
        Append(" c\n");
    }

    public void StrokePath() {
        Append("S\n");
    }

    public void ClosePath() {
        Append("s\n");
    }

    public void FillPath() {
        Append("f\n");
    }

    public void DrawText(TextParameters parameters) {
        Append("BT\n");
        Append("/F");
        Append(parameters.font.objNumber);
        Append(" ");
        Append(parameters.fontSize);
        Append(" Tf\n");
        Append(parameters.x);
        Append(" ");
        Append(height - parameters.y);
        Append(" Td\n");
        Append("<");
        DrawText(parameters.font, parameters.text);
        Append("> Tj\n");
        Append("ET\n");
    }

    /// <summary>
    /// Sets the rotation angle.
    /// </summary>
    /// <param name="degrees">The rotation angle in degrees.</param>
    public void SetRotation(double degrees) {
        this.rotateDegrees = (float)degrees;
    }

    /// <summary>
    /// Sets clockwise rotation.
    /// </summary>
    /// <param name="degrees">The rotation angle in degrees (clockwise).</param>
    public void SetRotationClockwise(double degrees) {
        this.rotateDegrees = (float)-degrees;
    }

    /// <summary>
    /// Sets counter-clockwise rotation.
    /// </summary>
    /// <param name="degrees">The rotation angle in degrees (counter-clockwise).</param>
    public void SetRotationCounterClockwise(double degrees) {
        this.rotateDegrees = (float)degrees;
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

        pdf.Append("/Resources <<\n");
        if (fonts.Count > 0) {
            pdf.Append("/Font <<\n");
            foreach (Font font in fonts) {
                pdf.Append("/F");
                pdf.Append(font.objNumber);
                pdf.Append(" ");
                pdf.Append(font.objNumber);
                pdf.Append(" 0 R\n");
            }
            pdf.Append(">>\n");
        }
        pdf.Append(">>\n");
        pdf.Append("/Length ");
        pdf.Append(buf.Length);
        pdf.Append(Token.Newline);
        pdf.Append(Token.EndDictionary);    // End of XObject dictionary
        pdf.Append(Token.Stream);
        pdf.Append(buf.ToArray());
        pdf.Append(Token.EndStream);
        pdf.EndObj();
        pdf.xObjects.Add(this);
        objNumber = pdf.GetObjNumber();
    }

    private void DrawText(Font font, string str) {
        foreach (char c in str) {
            int codePoint = c;
            if (codePoint != 0xFEFF) {          // Skip BOM
                int gid = (codePoint < font.firstChar || codePoint > font.lastChar)
                    ? font.unicodeToGID[0x0020] // Use space fallback if outside the font's supported range
                    : font.unicodeToGID[codePoint];
                AppendCodePointAsHex(gid);
            }
        }
    }

    private void AppendCodePointAsHex(int codePoint) {
        buf.WriteByte(Page.HEX[(codePoint >> 12) & 0xF]);
        buf.WriteByte(Page.HEX[(codePoint >> 8)  & 0xF]);
        buf.WriteByte(Page.HEX[(codePoint >> 4)  & 0xF]);
        buf.WriteByte(Page.HEX[codePoint & 0xF]);
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

        page.Append("Q\n"); // Restore the graphics state
        // page.AddEMC();

        return new float[] { this.x + width, this.y + height };
    }
}   // End of XObject.cs
}   // End of PDFjet.NET
