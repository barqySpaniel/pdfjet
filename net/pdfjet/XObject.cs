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
    private PDF pdf;
    private float x;
    private float y;
    private float width;
    private float height;
    private MemoryStream buf;
    private float rotateDegrees = 0f;
    private int objNumber;

    public XObject(PDF pdf, float width, float height) {
        this.pdf = pdf;
        this.width = width;
        this.height = height;
        this.buf = new MemoryStream();
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
        pdf.xObjects.Add(this);
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

        page.Append("Q\n"); // Restore the graphics state
        // page.AddEMC();

        return new float[] { this.x + width, this.y + height };
    }
}   // End of XObject.cs
}   // End of PDFjet.NET
