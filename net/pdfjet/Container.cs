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

    public void SetScaleFactor(float factor) {
        SetScaleFactorXY(factor, factor);
    }

    public void SetScaleFactorXY(float sx, float sy) {
        this.scaleX = sx;
        this.scaleY = sy;
    }

    public void Add(IDrawable element) {
        this.elements.Add(element);
    }

    public float[] DrawOn(Page page) {
        page.Append("q\n"); // Save the graphics state

        // 1) Translate container to its final position on the page
        //    This is logically the last transformation, but in PDF it’s applied first
        page.Append("1 0 0 1 ");
        page.Append(this.x);
        page.Append(' ');
        page.Append(-this.y);
        page.Append(" cm\n");

        float cx = width / 2f;
        float cy = height / 2f;

        // 2) Move origin to the center of the container
        //    Needed for rotation and scaling
        //    This transformation is applied on top of the previous translation
        page.Append("1 0 0 1 ");
        page.Append(cx);
        page.Append(' ');
        page.Append(page.height - cy);
        page.Append(" cm\n");

        // 3) Rotate around the container center
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

        // 4) Scale around the container center
        page.Append(scaleX);
        page.Append(' ');
        page.Append('0');
        page.Append(' ');
        page.Append('0');
        page.Append(' ');
        page.Append(scaleY);
        page.Append(' ');
        page.Append('0');
        page.Append(' ');
        page.Append('0');
        page.Append(" cm\n");

        // 5) Move origin back so children can draw using local coordinates (0,0)
        //    Executed last in the stream, but logically the first transformation for child drawing
        page.Append("1 0 0 1 ");
        page.Append(-cx);
        page.Append(' ');
        page.Append(-(page.height - cy));
        page.Append(" cm\n");

        // 6) Draw children elements
        foreach (IDrawable element in elements) {
            element.DrawOn(page);
        }

        page.Append("Q\n"); // Restore the graphics state

        // Return bottom-right position of container
        return new float[] { this.x + width, this.y + height };
    }
}
}
