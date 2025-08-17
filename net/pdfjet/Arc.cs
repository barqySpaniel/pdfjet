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
    private float r1;
    private float r2;
    private float startAngle;
    private float endAngle;
    private Sweep sweep = Sweep.CLOCKWISE;

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

    /**
     *  Create an arc object.
     *
     *  @param x1 the x coordinate of the start point.
     *  @param y1 the y coordinate of the start point.
     *  @param r1 the x coordinate of the end point.
     *  @param r2 the y coordinate of the end point.
     */
    public Arc(double x, double y, double r1, double r2) : this((float) x, (float) y, (float) r1, (float) r2) {
    }

    /**
     *  Create an arc object.
     *
     *  @param x1 the x coordinate of the start point.
     *  @param y1 the y coordinate of the start point.
     *  @param r1 the x coordinate of the end point.
     *  @param r2 the y coordinate of the end point.
     */
    public Arc(float x, float y, float r1, float r2) {
        this.x = x;
        this.y = y;
        this.r1 = r1;
        this.r2 = r2;
    }

    public Arc(
            float x, float y, float r1, float r2, float startAngle, float endAngle, Sweep sweep) {
        this.x = x;
        this.y = y;
        this.r1 = r1;
        this.r2 = r2;
        this.startAngle = startAngle;
        this.endAngle = endAngle;
        this.sweep = sweep;
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
     *  Sets the x and y coordinates of the start point.
     *
     *  @param x the x coordinate of the start point.
     *  @param y the t coordinate of the start point.
     *  @return this Arc object.
     */
    public Arc SetStartPoint(double x, double y) {
        this.x = (float) x;
        this.y = (float) y;
        return this;
    }

    public void SetPosition(float x, float y) {
        SetStartPoint(x, y);
    }

    /**
     *  Sets the x and y coordinates of the start point.
     *
     *  @param x the x coordinate of the start point.
     *  @param y the y coordinate of the start point.
     *  @return this Arc object.
     */
    public Arc SetStartPoint(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }

    /**
     *  Sets the x and y coordinates of the start point.
     *
     *  @param x the x coordinate of the start point.
     *  @param y the y coordinate of the start point.
     *  @return this Arc object.
     */
//    public Arc SetPointA(float x, float y) {
//        this.x1 = x;
//        this.y1 = y;
//        return this;
//    }

    /**
     *  Returns the start point of this line.
     *
     *  @return Point the point.
     */
//    public Point GetStartPoint() {
//        return new Point(x1, y1);
//    }

    /**
     *  Sets the x and y coordinates of the end point.
     *
     *  @param x the x coordinate of the end point.
     *  @param y the y coordinate of the end point.
     *  @return this Arc object.
     */
//    public Arc SetEndPoint(double x, double y) {
//        this.x2 = (float) x;
//        this.y2 = (float) y;
//        return this;
//    }

    /**
     *  Sets the x and y coordinates of the end point.
     *
     *  @param x the x coordinate of the end point.
     *  @param y the y coordinate of the end point.
     *  @return this Arc object.
     */
//    public Arc SetEndPoint(float x, float y) {
//        this.x2 = x;
//        this.y2 = y;
//        return this;
//    }

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
    public Arc SetColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        SetStrokeColor(r, g, b);
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
     *  Scales this line by the spacified factor.
     *
     *  @param factor the factor used to scale the line.
     *  @return this Arc object.
     */
    public Arc ScaleBy(double factor) {
        return ScaleBy((float) factor);
    }

    /**
     *  Scales this line by the spacified factor.
     *
     *  @param factor the factor used to scale the line.
     *  @return this Arc object.
     */
    public Arc ScaleBy(float factor) {
        this.r1 *= factor;
        this.r2 *= factor;
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
        page.SetPenColor(strokeColor);
        page.SetPenWidth(strokeWidth);
        page.SetStrokePattern(strokePattern);
        page.DrawEllipticalArc(
                x,
                y,
                r1,
                r2,
                startAngle,
                endAngle,
                sweep);
        page.Append("Q\n");
        page.AddEMC();

//        float xMax = Math.Max(x + xBox, x2 + xBox);
//        float yMax = Math.Max(y1 + yBox, y2 + yBox);
//        return new float[] {xMax, yMax};
        return new float[] {0f, 0f};
    }
}   // End of Arc.cs
}   // End of namespace PDFjet.NET
