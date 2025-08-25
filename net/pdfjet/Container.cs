using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PDFjet.NET {
public class Container : IDrawable {
    private float x;
    private float y;
    private float width;
    private float height;
    private List<IDrawable> elements = new List<IDrawable>();
    private float rotateDegrees = 0f;

    public Container(float width, float height) {
        this.width = width;
        this.height = height;
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

    public void Add(IDrawable element) {
        this.elements.Add(element);
    }

    public void Complete() {
    }

    public float[] DrawOn(Canvas page) {
        // page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n"); // Save the graphics state

//        float drawX = this.x;
//        float drawY = (page.height - this.height) - this.y;
//
//        // 5. POSITION: move to desired location on page
//        page.Append("1 0 0 1 ");
//        page.Append(drawX);
//        page.Append(' ');
//        page.Append(drawY);
//        page.Append(" cm\n");
/*
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
*/
        foreach (IDrawable element in elements) {
//            element.x += x;
//            element.y += y;
            element.DrawOn(page);
        }
        page.Append("Q\n"); // Restore the graphics state
        // page.AddEMC();

        return new float[] { this.x + width, this.y + height };
    }
}   // End of Container.cs
}   // End of PDFjet.NET
