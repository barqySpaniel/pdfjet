/**
 *  Path.cs
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

/**
 *  Used to create path objects.
 *  The path objects may consist of lines, splines or both.
 *
 *  Please see Example_02.
 */
namespace PDFjet.NET {
public class Path : IDrawable {
    private float[] color = new float[] {0f, 0f, 0f};   // Black color


    // private bool fillShape = false;
    // private bool closePath = false;

    private List<Point> points = null;
    private float[] fillColor;
    private float strokeWidth;
    private float[] strokeColor;
    private String strokePattern = "[] 0";

    private float xBox;
    private float yBox;
    private CapStyle lineCapStyle = CapStyle.BUTT;
    private JoinStyle lineJoinStyle = JoinStyle.MITER;
    private int degrees;

    private String uri = null;
    private String key = null;
    private String language = null;
    private String actualText = Single.space;
    private String altDescription = Single.space;

    /**
     *  The default constructor.
     */
    public Path() {
        points = new List<Point>();
    }

    /**
     *  Adds a point to this path.
     *
     *  @param point the point to add.
     */
    public void Add(Point point) {
        points.Add(point);
    }

    /**
     *  Sets the line dash pattern for this path.
     *
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
     */
    public void SetStrokePattern(String pattern) {
        this.strokePattern = pattern;
    }

    /**
     *  Sets the pen width that will be used to draw the lines and splines that are part of this path.
     *
     *  @param width the pen width.
     */
    public void SetStrokeWidth(double width) {
        this.strokeWidth = (float) width;
    }

    /**
     *  Sets the pen color that will be used to draw this path.
     *
     *  @param color the color is specified as an integer.
     */
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

    public void SetStrokeWidth(float width) {
        this.strokeWidth = width;
    }

    public void SetStrokeColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        SetStrokeColor(r, g, b);
    }

    public void SetStrokeColor(float r, float g, float b) {
        this.strokeColor = new float[] {r, g, b};
    }

    public void SetStrokeColor(float[] rgbColor) {
        this.strokeColor = rgbColor;
    }



    /**
     *  Sets the line cap style.
     *
     *  @param style the cap style of this path.
     *  Supported values: CapStyle.BUTT, CapStyle.ROUND and CapStyle.PROJECTING_SQUARE
     */
    public void SetLineCapStyle(CapStyle style) {
        this.lineCapStyle = style;
    }

    /**
     *  Returns the line cap style for this path.
     *
     *  @return the line cap style for this path.
     */
    public CapStyle GetLineCapStyle() {
        return this.lineCapStyle;
    }

    /**
     *  Sets the line join style.
     *
     *  @param style the line join style code. Supported values: JoinStyle.MITER, JoinStyle.ROUND and JoinStyle.BEVEL
     */
    public void SetLineJoinStyle(JoinStyle style) {
        this.lineJoinStyle = style;
    }

    /**
     *  Returns the line join style.
     *
     *  @return the line join style.
     */
    public JoinStyle GetLineJoinStyle() {
        return this.lineJoinStyle;
    }

    public void SetRotateAngle(int degrees) {
        this.degrees = -degrees;
    }

    /**
     *  Places this path in the specified box at position (0.0, 0.0).
     *
     *  @param box the specified box.
     */
    public void PlaceIn(Box box) {
        PlaceIn(box, 0.0f, 0.0f);
    }

    /**
     *  Places the path inside the specified box at coordinates (xOffset, yOffset) of the top left corner.
     *
     *  @param box the specified box.
     *  @param xOffset the xOffset.
     *  @param yOffset the yOffset.
     */
    public void PlaceIn(
            Box box,
            double xOffset,
            double yOffset) {
        PlaceIn(box, (float) xOffset, (float) yOffset);
    }

    /**
     *  Places the path inside the specified box at coordinates (xOffset, yOffset) of the top left corner.
     *
     *  @param box the specified box.
     *  @param xOffset the xOffset.
     *  @param yOffset the yOffset.
     */
    public void PlaceIn(
            Box box,
            float xOffset,
            float yOffset) {
        xBox = box.x + xOffset;
        yBox = box.y + yOffset;
    }

    public void SetPosition(double x, double y) {
        SetLocation((float) x, (float) y);
    }

    public void SetPosition(float x, float y) {
        SetLocation(x, y);
    }

    public Path SetLocation(double x, double y) {
        return SetLocation((float) x, (float) y);
    }

    public Path SetLocation(float x, float y) {
        xBox += x;
        yBox += y;
        return this;
    }

    /**
     *  Sets the URI for the "click box" action.
     *
     *  @param uri the URI
     */
    public void SetURIAction(String uri) {
        this.uri = uri;
    }

    /**
     *  Sets the destination key for the action.
     *
     *  @param key the destination name.
     */
    public void SetGoToAction(String key) {
        this.key = key;
    }

    /**
     *  Scales the path using the specified factor.
     *
     *  @param factor the specified factor.
     */
    public void SetScaleFactor(double factor) {
        SetScaleFactor((float) factor);
    }

    /**
     *  Scales the path using the specified factor.
     *
     *  @param factor the specified factor.
     */
    public void SetScaleFactor(float factor) {
        for (int i = 0; i < points.Count; i++) {
            Point point = points[i];
            point.x *= factor;
            point.y *= factor;
        }
    }

    /**
     *  Draws this path on the page using the current selected color, pen width, line pattern and line join style.
     *
     *  @param page the page to draw on.
     *  @return x and y coordinates of the bottom right corner of this component.
     *  @throws Exception
     */
    public float[] DrawOn(Page page) {
        foreach (Point point in points) {
            point.x += xBox;
            point.y += yBox;
        }

        float x = float.MaxValue;
        float y = float.MaxValue;
        float xMax = 0f;
        float yMax = 0f;
        foreach (Point point in points) {
            if (point.x < x) { x = point.x; }
            if (point.y < y) { y = point.y; }

            if (point.x > xMax) { xMax = point.x; }
            if (point.y > yMax) { yMax = point.y; }

            // point.x -= xBox;
            // point.y -= yBox;
        }
        float w = xMax - x;
        float h = yMax - y;

        page.AddBMC(StructElem.P, this.language, this.actualText, this.altDescription);
        page.Append("q\n");

        float centerX = x + w/2;
        float centerY = (page.height - y) - h/2;
        page.RotateAroundCenter(centerX, centerY, degrees);
        page.DrawPath(points, fillColor, strokeWidth, strokeColor);


//        page.SetBrushColor(color);
//        page.SetPenWidth(width);
//        page.SetPenColor(color);
//        page.SetLinePattern(pattern);
//        page.SetLineCapStyle(lineCapStyle);
//        page.SetLineJoinStyle(lineJoinStyle);
//        if (closePath) {
//            page.DrawPath(points, Operation.CLOSE);
//        } else {
//            page.DrawPath(points, Operation.STROKE);
//        }

        page.Append("Q\n");
        page.AddEMC();

        if (uri != null || key != null) {
            page.AddAnnotation(new Annotation(
                    uri,
                    key,    // The destination name
                    x,
                    y,
                    x + w,
                    y + h,
                    language,
                    actualText,
                    altDescription));
        }

        return new float[] {xMax, yMax};
    }
}   // End of Path.cs
}   // End of namespace PDFjet.NET
