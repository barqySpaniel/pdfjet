using System;
using System.Collections.Generic;

namespace PDFjet.NET {
public class Container : IDrawable {
    public float x;
    public float y;
    public float width;
    public float height;
    public float rotateDegrees;
    public float scaleX;
    public float scaleY;
    private List<IDrawable> elements;

    public Container(float width, float height) {
        this.width = width;
        this.height = height;
        this.rotateDegrees = 0f;
        this.scaleX = 1f;
        this.scaleY = 1f;
        this.elements = new List<IDrawable>();
    }

    public void SetPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetRotateDegreesCW(float degrees) {
        this.rotateDegrees = -degrees;
    }

    public void SetRotateDegreesCCW(float degrees) {
        this.rotateDegrees = degrees;
    }

    public void SetScale(float sx, float sy) {
        this.scaleX = sx;
        this.scaleY = sy;
    }

    public void Add(IDrawable element) {
        this.elements.Add(element);
    }

    public void Complete() {
    }

    public float[] DrawOn(Canvas page) {
        page.Append("q\n"); // Save the graphics state

        page.Append("1 0 0 1 ");
        page.Append(this.x);
        page.Append(' ');
        page.Append(-this.y);
        page.Append(" cm\n");

        float cx = width * 0.5f;
        float cy = height * 0.5f;

        page.Append("1 0 0 1 ");
        page.Append(cx);
        page.Append(' ');
        page.Append(page.height - cy);
        page.Append(" cm\n");

        double rad = rotateDegrees * (Math.PI / 180.0);
        float cos = (float)Math.Cos(rad);
        float sin = (float)Math.Sin(rad);
        page.Append(FastFloat.ToByteArray(cos));
        page.Append(' ');
        page.Append(FastFloat.ToByteArray(sin));
        page.Append(' ');
        page.Append(FastFloat.ToByteArray(-sin));
        page.Append(' ');
        page.Append(FastFloat.ToByteArray(cos));
        page.Append(" 0 0 cm\n");

//        page.Append(scaleX);
//        page.Append(' ');
//        page.Append('0');
//        page.Append(' ');
//        page.Append('0');
//        page.Append(' ');
//        page.Append(scaleY);
//        page.Append(' ');
//        page.Append('0');
//        page.Append(' ');
//        page.Append('0');
//        page.Append(" cm\n");

        page.Append("1 0 0 1 ");
        page.Append(-cx);
        page.Append(' ');
        page.Append(-(page.height - cy));
        page.Append(" cm\n");

        foreach (IDrawable element in elements) {
            element.DrawOn(page);
        }

        page.Append("Q\n"); // Restore graphics state

        return new float[] { this.x + width, this.y + height };
    }
}
}
