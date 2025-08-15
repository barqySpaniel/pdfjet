/**
 * Ellipse.cs
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
using System.Collections.Generic;

namespace PDFjet.NET {
public class Ellipse  : IDrawable {
    private float x;
    private float y;
    private float w;
    private float h;
    private float r;
    private float[] fillColor;
    private float[] borderColor = new float[] {0f, 0f, 0f}; // Black color
    private float width = 0f;
    private string pattern = "[] 0";
    private string uri;
    private string key;
    private string language = "en-US";
    private string altDescription = "";
    private string actualText = "";
    private string structureType = StructElem.P;

    private int degrees = 0;

    /**
     * The default constructor.
     */
    public Ellipse() {
    }

    public Ellipse(float x, float y, float w, float h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    public Ellipse(double x, double y, double w, double h) {
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

    public void SetBorderColor(int color) {
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

    public void SetLineWidth(float width) {
        this.width = width;
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

    public Ellipse SetAltDescription(string altDescription) {
        this.altDescription = altDescription;
        return this;
    }

    public Ellipse SetActualText(string actualText) {
        this.actualText = actualText;
        return this;
    }

    public Ellipse SetStructureType(string structureType) {
        this.structureType = structureType;
        return this;
    }

    public void SetPattern(string pattern) {
        this.pattern = pattern;
    }
/*
    public void PlaceIn(Rect rect, float xOffset, float yOffset) {
        this.x = rect.x + xOffset;
        this.y = rect.y + yOffset;
    }
*/
    public void ScaleBy(float factor) {
        this.x *= factor;
        this.y *= factor;
    }

    public void RotateBy(int degrees) {
        this.degrees = -degrees;
    }

    public float[] DrawOn(Page page) {
        page.AddBMC(this.structureType, this.language, this.actualText, this.altDescription);
        page.Append("q\n");

        float centerX = x + w/2;
        float centerY = (page.height - y) - h/2;
        page.RotateAroundCenter(centerX, centerY, degrees);
        page.DrawEllipse(x, y, w, h);

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
