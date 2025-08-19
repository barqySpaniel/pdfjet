/**
 *  Arc.cs
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

/**
 * Used to create arc objects.
 */
namespace PDFjet.NET {
public class Arc : IDrawable {
    private float x;
    private float y;
    private float rx;
    private float ry;
    private float startAngle;
    private float endAngle;
    private Sweep sweep = Sweep.CLOCKWISE;
    private float degrees;

    private float[] fillColor;
    private float[] strokeColor = new float[] {0f, 0f, 0f};   // Black color
    private float strokeWidth = 0f;
    private String strokePattern = "[] 0";

    private String language = null;
    private String actualText = Single.space;
    private String altDescription = Single.space;

    /**
     *  The default constructor.
     */
    public Arc() {
    }

    public void SetPosition(float x, float y) {
        SetCenterXY(x, y);
    }

    public Arc SetCenterXY(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }

    public Arc SetRadiusX(float rx) {
        this.rx = rx;
        return this;
    }

    public Arc SetRadiusY(float ry) {
        this.ry = ry;
        return this;
    }

    public Arc SetRadius(float r) {
        this.rx = r;
        this.ry = r;
        return this;
    }

    public Arc SetStartAngle(float angle) {
        this.startAngle = angle;
        return this;
    }

    public Arc SetEndAngle(float angle) {
        this.endAngle = angle;
        return this;
    }

    public Arc SetSweep(Sweep sweep) {
        this.sweep = sweep;
        return this;
    }

    /**
     *  The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
     *  It is specified by a dash array and a dash phase.
     *  The elements of the dash array are positive numbers that specify the lengths of
     *  alternating dashes and gaps.
     *  The dash phase specifies the distance into the dash pattern at which to start the dash.
     *  The elements of both the dash array and the dash phase are expressed in user space units.
     *  <pre>
     *  Examples of line dash patterns:
     *
     *      "[Array] Phase"     Appearance          Description
     *      _______________     _________________   ____________________________________
     *
     *      "[] 0"              -----------------   Solid line
     *      "[3] 0"             ---   ---   ---     3 units on, 3 units off, ...
     *      "[2] 1"             -  --  --  --  --   1 on, 2 off, 2 on, 2 off, ...
     *      "[2 1] 0"           -- -- -- -- -- --   2 on, 1 off, 2 on, 1 off, ...
     *      "[3 5] 6"             ---     ---       2 off, 3 on, 5 off, 3 on, 5 off, ...
     *      "[2 3] 11"          -   --   --   --    1 on, 3 off, 2 on, 3 off, 2 on, ...
     *  </pre>
     *
     *  @param pattern the line dash pattern.
     *  @return this Arc object.
     */
    public Arc SetStrokePattern(String pattern) {
        this.strokePattern = pattern;
        return this;
    }

    /**
     *  Sets the width of this line.
     *
     *  @param width the width.
     *  @return this Arc object.
     */
    public Arc SetStrokeWidth(double width) {
        this.strokeWidth = (float) width;
        return this;
    }

    /**
     *  Sets the width of this line.
     *
     *  @param strokeWidth the width.
     *  @return this Arc object.
     */
    public Arc SetStrokeWidth(float width) {
        this.strokeWidth = width;
        return this;
    }

    /**
     *  Sets the color for this line.
     *
     *  @param color the color specified as an integer.
     *  @return this Arc object.
     */
    public Arc SetStrokeColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        this.SetStrokeColor(r, g, b);
        return this;
    }

    public Arc SetStrokeColor(float r, float g, float b) {
        this.strokeColor = new float[] {r, g, b};
        return this;
    }

    public Arc SetStrokeColor(float[] rgbColor) {
        this.strokeColor = rgbColor;
        return this;
    }

    public Arc SetFillColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        this.SetFillColor(r, g, b);
        return this;
    }

    public Arc SetFillColor(float r, float g, float b) {
        this.fillColor = new float[] {r, g, b};
        return this;
    }

    public Arc SetFillColor(float[] rgbColor) {
        this.fillColor = rgbColor;
        return this;
    }

    public Arc SetRotateAngle(float degrees) {
        this.degrees = -degrees;
        return this;
    }

    public Arc SetRotateAngle(double degrees) {
        this.degrees = (float) -degrees;
        return this;
    }

    /**
     *  Sets the alternate description of this line.
     *
     *  @param altDescription the alternate description of the line.
     *  @return this Arc.
     */
    public Arc SetAltDescription(String altDescription) {
        this.altDescription = altDescription;
        return this;
    }

    /**
     *  Sets the actual text for this line.
     *
     *  @param actualText the actual text for the line.
     *  @return this Arc.
     */
    public Arc SetActualText(String actualText) {
        this.actualText = actualText;
        return this;
    }

    /**
     *  Scales this line by the specified factor.
     *
     *  @param factor the factor used to scale the line.
     *  @return this Arc object.
     */
    public Arc SetScaleFactor(double factor) {
        return SetScaleFactor((float) factor);
    }

    /**
     *  Scales this line by the specified factor.
     *
     *  @param factor the factor used to scale the line.
     *  @return this Arc object.
     */
    public Arc SetScaleFactor(float factor) {
        this.rx *= factor;
        this.ry *= factor;
        return this;
    }

    /**
     *  Draws this line on the specified page.
     *
     *  @param page the page to draw on.
     *  @return x and y coordinates of the bottom right corner of this component.
     *  @throws Exception
     */
    public float[] DrawOn(Page page) {
        page.AddBMC(StructElem.P, language, actualText, altDescription);
        page.Append("q\n");
        float centerX = x + rx/2;
        float centerY = (page.height - y) - ry/2;
        page.RotateAroundCenter(centerX, centerY, degrees);
        Page.DrawArc(
                page,
                x,
                y,
                rx,
                ry,
                startAngle,
                endAngle,
                sweep);
        if (strokeColor != null && strokePattern != null) {
            page.SetStrokePattern(strokePattern);
        }
        if (fillColor != null && strokeColor != null) {
            page.SetBrushColor(fillColor);
            page.SetPenWidth(strokeWidth);
            page.SetPenColor(strokeColor);
            page.Append("B\n");
        } else if (fillColor != null && strokeColor == null) {
            page.SetBrushColor(fillColor);
            page.Append("f\n");
        } else if (fillColor == null && strokeColor != null) {
            page.SetPenWidth(strokeWidth);
            page.SetPenColor(strokeColor);
            page.Append("S\n");
        } else {    // Both brushColor == null and penColor == null
            page.SetPenWidth(0f);
            page.SetPenColor(0f, 0f, 0f);
            page.Append("S\n");
        }
        page.Append("Q\n");
        page.AddEMC();
        return new float[] {0f, 0f};    // TODO
    }
}   // End of Arc.cs
}   // End of namespace PDFjet.NET
