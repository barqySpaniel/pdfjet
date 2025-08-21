/**
 *  Rect.cs
 *
©2025 PDFjet Software

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;

namespace PDFjet.NET {
public class Rect  : IDrawable {
    internal float x;
    internal float y;
    private float w;
    private float h;
    private float r;

    private float[] fillColor;
    private float borderWidth;
    private float[] borderColor;
    private string borderPattern = "[] 0";

    private string uri;
    private string key;
    private string language = "en-US";
    private string altDescription = "";
    private string actualText = "";
    private string structType = StructElem.P;

    /**
     * The default constructor.
     */
    public Rect() {
    }

    public Rect(float x, float y, float w, float h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    public Rect(double x, double y, double w, double h) {
        this.x = (float) x;
        this.y = (float) y;
        this.w = (float) w;
        this.h = (float) h;
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }

    public void SetPosition(double x, double y) {
        SetLocation((float) x, (float) y);
    }

    public void SetSize(float w, float h) {
        this.w = w;
        this.h = h;
    }

    public void SetFillColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        SetFillColor(r, g, b);
    }

    public void SetFillColor(float r, float g, float b) {
        this.fillColor = new float[] {r, g, b};
    }

    public void SetFillColor(float[] rgbColor) {
        this.fillColor = rgbColor;
    }

    public void SetBorderWidth(float width) {
        this.borderWidth = width;
    }

    public void SetBorderColor(int color) {
        if (color == Color.transparent) {
            this.borderColor = null;
            return;
        }
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        SetBorderColor(r, g, b);
    }

    public void SetBorderColor(float r, float g, float b) {
        this.borderColor = new float[] {r, g, b};
    }

    public void SetBorderColor(float[] rgbColor) {
        this.borderColor = rgbColor;
    }

    public void SetCornerRadius(float r) {
        this.r = r;
    }

    public void SetURIAction(string uri) {
        this.uri = uri;
    }

    public void SetGoToAction(string key) {
        this.key = key;
    }

    public Rect SetAltDescription(string altDescription) {
        this.altDescription = altDescription;
        return this;
    }

    public Rect SetActualText(String actualText) {
        this.actualText = actualText;
        return this;
    }

    public Rect SetStructType(String structType) {
        this.structType = structType;
        return this;
    }

    public void SetBorderPattern(String pattern) {
        this.borderPattern = pattern;
    }

    public void ScaleBy(float factor) {
        this.x *= factor;
        this.y *= factor;
    }

    public float[] DrawOn(Page page) {
        const float k = 0.55228f;
        page.AddBMC(this.structType, this.language, this.actualText, this.altDescription);
        page.Append("q\n");
        if (this.r == 0.0f) {
            page.MoveTo(this.x, this.y);
            page.LineTo(this.x + this.w, this.y);
            page.LineTo(this.x + this.w, this.y + this.h);
            page.LineTo(this.x, this.y + this.h);
            if (this.fillColor != null) {
                page.SetBrushColor(this.fillColor);
                page.FillPath();
            }
            if (borderColor != null) {
                page.SetPenWidth(this.borderWidth);
                page.SetPenColor(this.borderColor);
                page.SetStrokePattern(this.borderPattern);
                page.ClosePath();
            }
        } else {
            List<Point> points = new List<Point> {
                new Point((this.x + this.r), this.y, false),
                new Point((this.x + this.w) - this.r, this.y, false),
                new Point((this.x + this.w - this.r) + this.r * k, this.y, true),
                new Point((this.x + this.w), (this.y + this.r) - this.r * k, true),
                new Point((this.x + this.w), (this.y + this.r), false),
                new Point((this.x + this.w), (this.y + this.h) - this.r, false),
                new Point((this.x + this.w), ((this.y + this.h) - this.r) + this.r * k, true),
                new Point(((this.x + this.w) - this.r) + this.r * k, (this.y + this.h), true),
                new Point(((this.x + this.w) - this.r), (this.y + this.h), false),
                new Point((this.x + this.r), (this.y + this.h), false),
                new Point(((this.x + this.r) - this.r * k), (this.y + this.h), true),
                new Point(this.x, ((this.y + this.h) - this.r) + this.r * k, true),
                new Point(this.x, (this.y + this.h) - this.r, false),
                new Point(this.x, (this.y + this.r), false),
                new Point(this.x, (this.y + this.r) - this.r * k, true),
                new Point((this.x + this.r) - this.r * k, this.y, true),
                new Point((this.x + this.r), this.y, false)
            };
            page.DrawPath(points);
            if (borderColor != null && borderPattern != null) {
                page.SetStrokePattern(borderPattern);
            }
            if (fillColor != null && borderColor != null) {
                page.SetBrushColor(fillColor);
                page.SetPenWidth(borderWidth);
                page.SetPenColor(borderColor);
                page.Append("B\n");
            } else if (fillColor != null && borderColor == null) {
                page.SetBrushColor(fillColor);
                page.Append("f\n");
            } else if (fillColor == null && borderColor != null) {
                page.SetPenWidth(borderWidth);
                page.SetPenColor(borderColor);
                page.Append("S\n");
            }
        }
        page.Append("Q\n");
        page.AddEMC();

        if (this.uri != null || this.key != null) {
            page.AddAnnotation(new Annotation(
                this.uri,
                this.key,
                this.x,
                this.y,
                this.x + this.w,
                this.y + this.h,
                this.language,
                this.actualText,
                this.altDescription
            ));
        }

        return new float[] { this.x + this.w, this.y + this.h };
    }
}
}
