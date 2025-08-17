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
using System;
using System.Collections.Generic;

namespace PDFjet.NET {
public class Ellipse  : Arc {
//    private float x;
//    private float y;
//    private float r1;
//    private float r2;
//    private float[] fillColor;
//    private float[] strokeColor = new float[] {0f, 0f, 0f}; // Black color
//    private float strokeWidth = 0f;
//    private string strokePattern = "[] 0";
//    private string uri;
//    private string key;
//    private string language = "en-US";
//    private string altDescription = "";
//    private string actualText = "";
//    private string structureType = StructElem.P;

    private int degrees = 0;

    /**
     * The default constructor.
     */
    public Ellipse() : base() {
        SetStartAngle(0f);
        SetEndAngle(359.9f);
    }

//    public Ellipse(float x, float y, float r1, float r2) {
//        this.x = x;
//        this.y = y;
//        this.r1 = r1;
//        this.r2 = r2;
//    }
//
//    public Ellipse(double x, double y, double r1, double r2) {
//        this.x = (float) x;
//        this.y = (float) y;
//        this.r1 = (float) r1;
//        this.r2 = (float) r2;
//    }
//
//    public void SetLocation(float x, float y) {
//        this.x = x;
//        this.y = y;
//    }
//
//    public void SetPosition(float x, float y) {
//        SetLocation(x, y);
//    }
//
//    public void SetPosition(double x, double y) {
//        SetLocation((float) x, (float) y);
//    }
//
//    public void SetSize(float r1, float r2) {
//        this.r1 = r1;
//        this.r2 = r2;
//    }
//
//
//    public void SetFillColor(int color) {
//        float r = ((color >> 16) & 0xff)/255f;
//        float g = ((color >>  8) & 0xff)/255f;
//        float b = ((color)       & 0xff)/255f;
//        SetFillColor(r, g, b);
//    }
//
//    public void SetFillColor(float r, float g, float b) {
//        this.fillColor = new float[] {r, g, b};
//    }
//
//    public void SetFillColor(float[] rgbColor) {
//        this.fillColor = rgbColor;
//    }
//
//    public void SetStrokeWidth(float strokeWidth) {
//        this.strokeWidth = strokeWidth;
//    }
//
//    public void SetStrokeColor(int color) {
//        float r = ((color >> 16) & 0xff)/255f;
//        float g = ((color >>  8) & 0xff)/255f;
//        float b = ((color)       & 0xff)/255f;
//        SetStrokeColor(r, g, b);
//    }
//
//    public void SetStrokeColor(float r, float g, float b) {
//        this.strokeColor = new float[] {r, g, b};
//    }
//
//    public void SetStrokeColor(float[] rgbColor) {
//        this.strokeColor = rgbColor;
//    }
//
//    public void SetR1(float r1) {
//        this.r1 = r1;
//    }
//
//    public void SetR2(float r2) {
//        this.r2 = r2;
//    }
//
//    public void SetURIAction(string uri) {
//        this.uri = uri;
//    }
//
//    public void SetGoToAction(string key) {
//        this.key = key;
//    }
//
//    public Ellipse SetAltDescription(string altDescription) {
//        this.altDescription = altDescription;
//        return this;
//    }
//
//    public Ellipse SetActualText(string actualText) {
//        this.actualText = actualText;
//        return this;
//    }
//
//    public Ellipse SetStructureType(string structureType) {
//        this.structureType = structureType;
//        return this;
//    }
//
//    public void SetStrokePattern(string pattern) {
//        this.strokePattern = pattern;
//    }
//
//    public void SetScaleFactor(float factor) {
//        this.x *= factor;
//        this.y *= factor;
//    }
//
//    public void SetRotationAngle(int degrees) {
//        this.degrees = -degrees;
//    }
//
//    public float[] DrawOn(Page page) {
//        page.AddBMC(this.structureType, this.language, this.actualText, this.altDescription);
//        page.Append("q\n");
//
//        float centerX = x + r1/2;
//        float centerY = (page.height - y) - r2/2;
//        page.RotateAroundCenter(centerX, centerY, degrees);
//Console.WriteLine("Are we hjere??");
//        page.DrawEllipse(x, y, r1, r2, fillColor, strokeWidth, strokeColor, strokePattern);
//
//        page.Append("Q\n");
//        page.AddEMC();
//
//        if (this.uri != null || this.key != null) {
//            page.AddAnnotation(new Annotation(
//                this.uri,
//                this.key,
//                this.x,
//                this.y,
//                this.x + this.r1,
//                this.y + this.r2,
//                this.language,
//                this.actualText,
//                this.altDescription
//            ));
//        }
//
//        return new float[] { this.x + this.r1, this.y + this.r2 };
//    }
}
}
