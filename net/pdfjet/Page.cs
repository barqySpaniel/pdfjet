/**
 *  Page.cs
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
using System.IO;
using System.Text;
using System.Collections.Generic;

/**
 *  Used to create PDF page objects.
 *
 *  Please note:
 *  <pre>
 *  The coordinate (0.0f, 0.0f) is the top left corner of the page.
 *  The size of the pages are represented in points.
 *  1 point is 1/72 inches.
 *  </pre>
 *
 */
namespace PDFjet.NET {
public class Page : Canvas {
    internal PDF pdf;
    internal PDFobj pageObj;
    internal int objNumber;

//    internal MemoryStream buf;
//    internal readonly float width;
//    internal float height;

//    internal float[] tm = {1f, 0f, 0f, 1f};
//    internal byte[] tm0;
//    internal byte[] tm1;
//    internal byte[] tm2;
//    internal byte[] tm3;

    internal int renderingMode = 0;
    internal readonly List<Int32> contents;
    internal readonly List<Annotation> annots;
    internal readonly List<Destination> destinations;
    internal float[] cropBox;
    internal float[] bleedBox;
    internal float[] trimBox;
    internal float[] artBox;
    internal readonly List<StructElem> structures = new List<StructElem>();
    private float[] brushColor = {0f, 0f, 0f};
    private float[] penColor = {0f, 0f, 0f};
    private float penWidth = 0.6f;
    private float[] penCMYK = {0f, 0f, 0f, 1f};
    private float[] brushCMYK = {0f, 0f, 0f, 1f};
    private CapStyle lineCapStyle = CapStyle.BUTT;
    private JoinStyle lineJoinStyle = JoinStyle.MITER;
    private String strokePattern = "[] 0";
    private Font font;
    private readonly List<State> savedStates = new List<State>();
    private int mcid;

    internal float savedHeight = -1;

    /*
     * From Android's Matrix object:
     */
    private static int MSCALE_X = 0;
    private static int MSKEW_X  = 1;
    private static int MTRANS_X = 2;
    private static int MSKEW_Y  = 3;
    private static int MSCALE_Y = 4;
    private static int MTRANS_Y = 5;

    public static bool DETACHED = false;

    /**
     *  Creates page object and add it to the PDF document.
     *
     *  Please note:
     *  <pre>
     *  The coordinate (0.0, 0.0) is the top left corner of the page.
     *  The size of the pages are represented in points.
     *  1 point is 1/72 inches.
     *  </pre>
     *
     *  @param pdf the pdf object.
     *  @param pageSize the page size of this page.
     */
    public Page(PDF pdf, float[] pageSize) : this(pdf, pageSize, true) {
    }

    /**
     *  Creates page object and add it to the PDF document.
     *
     *  Please note:
     *  <pre>
     *  The coordinate (0.0, 0.0) is the top left corner of the page.
     *  The size of the pages are represented in points.
     *  1 point is 1/72 inches.
     *  </pre>
     *
     *  @param pdf the pdf object.
     *  @param pageSize the page size of this page.
     *  @param addPageToPDF bool flag.
     */
    public Page(PDF pdf, float[] pageSize, bool addPageToPDF) {
        this.pdf = pdf;
        contents = new List<Int32>();
        annots = new List<Annotation>();
        destinations = new List<Destination>();
        width = pageSize[0];
        height = pageSize[1];
        // buf = new MemoryStream(8192);
        // tm0 = FastFloat.ToByteArray(tm[0]);
        // tm1 = FastFloat.ToByteArray(tm[1]);
        // tm2 = FastFloat.ToByteArray(tm[2]);
        // tm3 = FastFloat.ToByteArray(tm[3]);
        if (addPageToPDF) {
            pdf.AddPage(this);
        }
    }

    public Page(PDF pdf, PDFobj pageObj) {
        this.pdf = pdf;
        this.pageObj = RemoveComments(pageObj);
        width = pageObj.GetPageSize()[0];
        height = pageObj.GetPageSize()[1];
        // buf = new MemoryStream(8192);
        // tm0 = FastFloat.ToByteArray(tm[0]);
        // tm1 = FastFloat.ToByteArray(tm[1]);
        // tm2 = FastFloat.ToByteArray(tm[2]);
        // tm3 = FastFloat.ToByteArray(tm[3]);
        Append("q\n");
        if (pageObj.gsNumber != -1) {
            Append("/GS");
            Append(pageObj.gsNumber + 1);
            Append(" gs\n");
        }
    }

    private PDFobj RemoveComments(PDFobj obj) {
        List<String> list = new List<String>();
        bool comment = false;
        foreach (String token in obj.dict) {
            if (token.Equals("%")) {
                comment = true;
            } else {
                if (token.StartsWith("/")) {
                    comment = false;
                    list.Add(token);
                } else {
                    if (!comment) {
                        list.Add(token);
                    }
                }
            }
        }
        obj.dict = list;
        return obj;
    }

    public Font AddResource(CoreFont coreFont, List<PDFobj> objects) {
        return pageObj.AddResource(coreFont, objects);
    }

    public void AddResource(Image image, List<PDFobj> objects) {
        pageObj.AddResource(image, objects);
    }

    public void AddResource(Font font, List<PDFobj> objects) {
        pageObj.AddResource(font, objects);
    }

    public void Complete(List<PDFobj> objects) {
        Append("Q\n");
        pageObj.AddContent(GetContent(), objects);
    }

    public byte[] GetContent() {
        return buf.ToArray();
    }

    /**
     *  Adds destination to this page.
     *
     *  @param name The destination name.
     *  @param xPosition The horizontal position of the destination on this page.
     *  @param yPosition The vertical position of the destination on this page.
     *
     *  @return the destination.
     */
    public Destination AddDestination(String name, float xPosition, float yPosition) {
        Destination dest = new Destination(name, xPosition, height - yPosition);
        destinations.Add(dest);
        return dest;
    }

    /**
     *  Adds destination to this page.
     *
     *  @param name The destination name.
     *  @param yPosition The vertical position of the destination on this page.
     *
     *  @return the destination.
     */
    public Destination AddDestination(String name, float yPosition) {
        Destination dest = new Destination(name, 0f, height - yPosition);
        destinations.Add(dest);
        return dest;
    }

    /**
     *  Returns the width of this page.
     *
     *  @return the width of the page.
     */
//    public float GetWidth() {
//        return width;
//    }

    /**
     *  Returns the height of this page.
     *
     *  @return the height of the page.
     */
//    public float GetHeight() {
//        return height;
//    }

    /**
     *  Draws a line on the page, using the current color, between the points (x1, y1) and (x2, y2).
     *
     *  @param x1 the first point's x coordinate.
     *  @param y1 the first point's y coordinate.
     *  @param x2 the second point's x coordinate.
     *  @param y2 the second point's y coordinate.
     */
/*
    public void DrawLine(
            double x1,
            double y1,
            double x2,
            double y2) {
        MoveTo(x1, y1);
        LineTo(x2, y2);
        StrokePath();
    }

    public void DrawString(
            Font font,
            Font fallbackFont,
            String str,
            float x,
            float y) {
        DrawString(font, fallbackFont, str, x, y, new float[] {0f, 0f, 0f}, null);
    }

    public void DrawString(
            Font font,
            Font fallbackFont,
            String str,
            float x,
            float y,
            Int32 color,
            Dictionary<String, Int32> colors) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        DrawString(font, fallbackFont, str, x, y, new float[] {r, g, b}, colors);
    }
*/
    /**
     *  Draws the text given by the specified string,
     *  using the specified Thai or Hebrew font and the current brush color.
     *  If the font is missing some glyphs - the fallback font is used.
     *  The baseline of the leftmost character is at position (x, y) on the page.
     *
     *  @param font1 the Thai or Hebrew font.
     *  @param font2 the fallback font.
     *  @param str the string to be drawn.
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
/*
    public void DrawString(
            Font font,
            Font fallbackFont,
            String str,
            float x,
            float y,
            float[] textColor,
            Dictionary<String, Int32> colors) {
        if (font.isCoreFont || font.isCJK || fallbackFont == null || fallbackFont.isCoreFont || fallbackFont.isCJK) {
            DrawString(font, str, x, y, textColor, colors);
        } else {
            Font activeFont = font;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++) {
                int ch = str[i];
                if (activeFont.unicodeToGID[ch] == 0) {
                    DrawString(activeFont, sb.ToString(), x, y, textColor, colors);
                    x += activeFont.StringWidth(sb.ToString());
                    sb.Length = 0;
                    // Switch the font
                    if (activeFont == font) {
                        activeFont = fallbackFont;
                    } else {
                        activeFont = font;
                    }
                }
                sb.Append((char) ch);
            }
            DrawString(activeFont, sb.ToString(), x, y, textColor, colors);
        }
    }
*/
    /**
     *  Draws the text given by the specified string,
     *  using the specified font and the current brush color.
     *  The baseline of the leftmost character is at position (x, y) on the page.
     *
     *  @param font the font to use.
     *  @param str the string to be drawn.
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
//    public void DrawString(
//            Font font,
//            String str,
//            double x,
//            double y) {
//        DrawString(font, str, (float) x, (float) y);
//    }
//
//    public void DrawString(
//            Font font,
//            String str,
//            float x,
//            float y) {
//        DrawString(font, str, x, y, new float[] {0f, 0f, 0f}, null);
//    }

    /**
     *  Draws the text given by the specified string,
     *  using the specified font and the current brush color.
     *  The baseline of the leftmost character is at position (x, y) on the page.
     *
     *  @param font the font to use.
     *  @param str the string to be drawn.
     *  @param x the x coordinate.
     *  @param y the y coordinate.
     */
/*
    public void DrawString(
            Font font,
            String str,
            float x,
            float y,
            float[] color,
            Dictionary<String, Int32> highlightColors) {
        if (str == null || str.Equals("")) {
            return;
        }
        Append("BT\n");
        SetTextFont(font);

        if (renderingMode != 0) {
            Append(renderingMode);
            Append(" Tr\n");
        }

        if (font.skew15 &&
                tm[0] == 1f &&
                tm[1] == 0f &&
                tm[2] == 0f &&
                tm[3] == 1f) {
            float skew = 0.26f;
            Append(tm[0]);
            Append(' ');
            Append(tm[1]);
            Append(' ');
            Append(tm[2] + skew);
            Append(' ');
            Append(tm[3]);
        } else {
            Append(tm0);
            Append(' ');
            Append(tm1);
            Append(' ');
            Append(tm2);
            Append(' ');
            Append(tm3);
        }
        Append(' ');
        Append(x);
        Append(' ');
        Append(height - y);
        Append(" Tm\n");

        if (highlightColors == null) {
            SetBrushColor(color);
            if (font.isCoreFont) {
                Append("[<");
                DrawASCIIString(font, str);
                Append(">] TJ\n");
            } else {
                Append("<");
                DrawUnicodeString(font, str);
                Append("> Tj\n");
            }
        } else {
            DrawColoredString(font, str, color, highlightColors);
        }
        Append("ET\n");
    }
*/
    private void DrawASCIIString(Font font, String str) {
        int len = str.Length;
        for (int i = 0; i < len; i++) {
            int c1 = str[i];
            if (c1 < font.firstChar || c1 > font.lastChar) {
                // Append(0x20.ToString("X2"));
                AppendCodePointAsHex(0x20);
                continue;
            }
            // Append(c1.ToString("X2"));
            AppendCodePointAsHex(c1);
            if (font.isCoreFont && font.kernPairs && i < (str.Length - 1)) {
                c1 -= 32;
                int c2 = str[i + 1];
                if (c2 < font.firstChar || c2 > font.lastChar) {
                    c2 = 32;
                }
                for (int j = 2; j < font.metrics[c1].Length; j += 2) {
                    if (font.metrics[c1][j] == c2) {
                        Append(">");
                        Append(-font.metrics[c1][j + 1]);
                        Append("<");
                        break;
                    }
                }
            }
        }
    }
/*
    internal float DrawTextBlock(
            Font font,
            TextLineWithOffset[] textLines,
            float x,
            float y,
            float leading,
            Direction direction,
            float[] color,
            Dictionary<String, Int32> highlightColors) {
        if (textLines == null || textLines.Length == 0) {
            return textLines.Length * leading;
        }

        Append("BT\n");
        SetTextFont(font);

        float xText = x;
        float yText = y;
        foreach (TextLineWithOffset textLine in textLines) {
            if (direction == Direction.LEFT_TO_RIGHT) {
                Append("1 0 0 1 ");
                Append(xText + textLine.xOffset);
                Append(' ');
                Append(height - (yText + font.ascent));
                Append(" Tm\n");
            } else {                // BOTTOM_TO_TOP
                Append("0 1 -1 0 ");
                Append(xText + font.ascent);
                Append(' ');
                Append(yText);
                Append(" Tm\n");
            }

            if (highlightColors == null) {
                SetBrushColor(color);
                Append("<");
                DrawUnicodeString(font, textLine.textLine);
                Append("> Tj\n");
            } else {
                DrawColoredString(font, textLine.textLine, color, highlightColors);
            }

            if (direction == Direction.LEFT_TO_RIGHT) {
                yText += leading;
            } else {
                xText += leading;
            }
        }

        Append("ET\n");

        return textLines.Length * leading;
    }

    private void DrawUnicodeString(Font font, String str) {
        if (str == null || str.Length == 0) {
            return;
        }
        if (font.isCJK) {
            int i = 0;
            while (i < str.Length) {
                int codePoint = char.ConvertToUtf32(str, i);
                if (codePoint != 0xFEFF) {                  // BOM
                    if (codePoint < font.firstChar || codePoint > font.lastChar) {
                        AppendCodePointAsHex(0x0020);       // Space fallback
                    } else {
                        AppendCodePointAsHex(codePoint);
                    }
                }
                i += char.IsHighSurrogate(str[i]) ? 2 : 1;  // Proper surrogate handling
            }
        } else {
            int i = 0;
            while (i < str.Length) {
                int codePoint = char.ConvertToUtf32(str, i);
                if (codePoint != 0xFEFF) {                  // BOM
                    if (codePoint < font.firstChar || codePoint > font.lastChar) {
                        AppendCodePointAsHex(font.unicodeToGID[0x0020]); // Space fallback
                    } else {
                        AppendCodePointAsHex(font.unicodeToGID[codePoint]);
                    }
                }
                i += char.IsHighSurrogate(str[i]) ? 2 : 1;  // Proper surrogate handling
            }
        }
    }
*/
    /**
     *  Sets the graphics state. Please see Example_31.
     *
     *  @param gs the graphics state to use.
     */
    public void SetGraphicsState(GraphicsState gs) {
        StringBuilder sb = new StringBuilder();
        sb.Append("/CA ");
        sb.Append(gs.GetAlphaStroking());
        sb.Append(" ");
        sb.Append("/ca ");
        sb.Append(gs.GetAlphaNonStroking());
        String state = sb.ToString();
        Int32 n;
        if (pdf.states.ContainsKey(state)) {
            n = pdf.states[state];
        } else {
            n = pdf.states.Count + 1;
            pdf.states[state] = n;
        }
        Append("/GS");
        Append(n);
        Append(" gs\n");
    }

    /**
     * Sets the color for stroking operations.
     * The pen color is used when drawing lines and splines.
     *
     * @param r the red component is float value from 0.0 to 1.0.
     * @param g the green component is float value from 0.0 to 1.0.
     * @param b the blue component is float value from 0.0 to 1.0.
     */
//    public void SetPenColor(
//            double r, double g, double b) {
//        SetPenColor((float) r, (float) g, (float) b);
//    }

    /**
     * Sets the color for stroking operations using CMYK.
     * The pen color is used when drawing lines and splines.
     *
     * @param c the cyan component is float value from 0.0f to 1.0f.
     * @param m the magenta component is float value from 0.0f to 1.0f.
     * @param y the yellow component is float value from 0.0f to 1.0f.
     * @param k the black component is float value from 0.0f to 1.0f.
     */
//    public void SetPenColorCMYK(float c, float m, float y, float k) {
//        Append(c);
//        Append(' ');
//        Append(m);
//        Append(' ');
//        Append(y);
//        Append(' ');
//        Append(k);
//        Append(" K\n");
//    }

    /**
     * Sets the color for brush operations.
     * This is the color used when drawing regular text and filling shapes.
     *
     * @param r the red component is float value from 0.0 to 1.0.
     * @param g the green component is float value from 0.0 to 1.0.
     * @param b the blue component is float value from 0.0 to 1.0.
     */
//    public void SetBrushColor(double r, double g, double b) {
//        SetBrushColor((float) r, (float) g, (float) b);
//    }

    /**
     * Sets the color for brush operations.
     * This is the color used when drawing regular text and filling shapes.
     *
     * @param r the red component is float value from 0.0f to 1.0f.
     * @param g the green component is float value from 0.0f to 1.0f.
     * @param b the blue component is float value from 0.0f to 1.0f.
     */
//    public void SetBrushColor(float r, float g, float b) {
//        Append(r);
//        Append(' ');
//        Append(g);
//        Append(' ');
//        Append(b);
//        Append(" rg\n");
//    }

    /**
     * Sets the color for brush operations using CMYK.
     * This is the color used when drawing regular text and filling shapes.
     *
     * @param c the cyan component is float value from 0.0f to 1.0f.
     * @param m the magenta component is float value from 0.0f to 1.0f.
     * @param y the yellow component is float value from 0.0f to 1.0f.
     * @param k the black component is float value from 0.0f to 1.0f.
     */
//    public void SetBrushColorCMYK(float c, float m, float y, float k) {
//        Append(c);
//        Append(' ');
//        Append(m);
//        Append(' ');
//        Append(y);
//        Append(' ');
//        Append(k);
//        Append(" k\n");
//    }

    /**
     * Sets the color for brush operations.
     *
     * @param color the color.
     * @throws IOException
     */
//    public void SetBrushColor(float[] rgbColor) {
//        if (rgbColor != null) {
//            SetBrushColor(rgbColor[0], rgbColor[1], rgbColor[2]);
//        }
//    }

    /**
     * Returns the brush color.
     *
     * @return the brush color.
     */
//    public float[] GetBrushColor() {
//        return brushColor;
//    }

    /**
     * Sets the pen color.
     *
     * @param color the color. See the Color class for predefined values or define your own using 0x00RRGGBB packed integers.
     * @throws IOException
     */
//    public void SetPenColor(int color) {
//        float r = ((color >> 16) & 0xff)/255f;
//        float g = ((color >>  8) & 0xff)/255f;
//        float b = ((color)       & 0xff)/255f;
//        SetPenColor(r, g, b);
//    }

//    public void SetPenColor(float[] rgbColor) {
//        if (rgbColor != null) {
//            SetPenColor(rgbColor[0], rgbColor[1], rgbColor[2]);
//        }
//    }

    /**
     * Sets the color for stroking operations.
     * The pen color is used when drawing lines and splines.
     *
     * @param r the red component is float value from 0.0f to 1.0f.
     * @param g the green component is float value from 0.0f to 1.0f.
     * @param b the blue component is float value from 0.0f to 1.0f.
     */
/*
    public void SetPenColor(float r, float g, float b) {
        Append(r);
        Append(' ');
        Append(g);
        Append(' ');
        Append(b);
        Append(" RG\n");
    }

    public float[] GetPenColor() {
        return penColor;
    }
*/
    /**
     * Sets the brush color.
     *
     * @param color the color. See the Color class for predefined values or define your own using 0x00RRGGBB packed integers.
     * @throws IOException
     */
//    public void SetBrushColor(int color) {
//        float r = ((color >> 16) & 0xff)/255f;
//        float g = ((color >>  8) & 0xff)/255f;
//        float b = ((color)       & 0xff)/255f;
//        SetBrushColor(r, g, b);
//    }

    /**
     *  Sets the line width to the default.
     *  The default is the finest line width.
     */
//    public void SetDefaultStrokeWidth() {
//        Append("0 w\n");
//    }

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
     */
//    public void SetStrokePattern(String pattern) {
//        Append(pattern);
//        Append(" d\n");
//    }

    /**
     * Sets the default stroke pattern to be solid line or curve.
     */
//    public void SetDefaultStrokePattern() {
//        Append("[] 0");
//        Append(" d\n");
//    }

    /**
     *  Sets the pen width that will be used to draw lines and splines on this page.
     *
     *  @param width the pen width.
     */
//    public void SetPenWidth(double width) {
//        SetPenWidth((float) width);
//    }

    /**
     *  Sets the pen width that will be used to draw lines and splines on this page.
     *
     *  @param width the pen width.
     */
//    public void SetPenWidth(float width) {
//        Append(width);
//        Append(" w\n");
//    }

//    public float GetPenWidth() {
//        return this.penWidth;
//    }

    /**
     *  Sets the current line cap style.
     *
     *  @param style the cap style of the current line.
     *  Supported values: CapStyle.BUTT, CapStyle.ROUND and CapStyle.PROJECTING_SQUARE
     */
//    public void SetLineCapStyle(CapStyle style) {
//        Append((int) style);
//        Append(" J\n");
//    }

    /**
     *  Sets the line join style.
     *
     *  @param style the line join style code.
     *  Supported values: JoinStyle.MITER, JoinStyle.ROUND and JoinStyle.BEVEL
     */
//    public void SetLineJoinStyle(JoinStyle style) {
//        Append((int) style);
//        Append(" j\n");
//    }

    /**
     *  Moves the pen to the point with coordinates (x, y) on the page.
     *
     *  @param x the x coordinate of new pen position.
     *  @param y the y coordinate of new pen position.
     */
//    public void MoveTo(double x, double y) {
//        MoveTo((float) x, (float) y);
//    }

    /**
     *  Moves the pen to the point with coordinates (x, y) on the page.
     *
     *  @param x the x coordinate of new pen position.
     *  @param y the y coordinate of new pen position.
     */
//    public void MoveTo(float x, float y) {
//        Append(x);
//        Append(' ');
//        Append(height - y);
//        Append(" m\n");
//    }

    /**
     *  Draws a line from the current pen position to the point with coordinates (x, y),
     *  using the current pen width and stroke color.
     */
//    public void LineTo(double x, double y) {
//        LineTo((float) x, (float) y);
//    }

    /**
     *  Draws a line from the current pen position to the point with coordinates (x, y),
     *  using the current pen width and stroke color.
     */
//    public void LineTo(float x, float y) {
//        Append(x);
//        Append(' ');
//        Append(height - y);
//        Append(" l\n");
//    }

    /**
     *  Draws the path using the current pen color.
     */
//    public void StrokePath() {
//        Append("S\n");
//    }

    /**
     *  Closes the path and draws it using the current pen color.
     */
//    public void ClosePath() {
//        Append("s\n");
//    }

    /**
     *  Closes and fills the path with the current brush color.
     */
//    public void FillPath() {
//        Append("f\n");
//    }

    /**
     *  Draws the outline of the specified rectangle on the page.
     *  The left and right edges of the rectangle are at x and x + w.
     *  The top and bottom edges are at y and y + h.
     *  The rectangle is drawn using the current pen color.
     *
     *  @param x the x coordinate of the rectangle to be drawn.
     *  @param y the y coordinate of the rectangle to be drawn.
     *  @param w the width of the rectangle to be drawn.
     *  @param h the height of the rectangle to be drawn.
     */
    public void DrawRect(double x, double y, double w, double h) {
        MoveTo(x, y);
        LineTo(x+w, y);
        LineTo(x+w, y+h);
        LineTo(x, y+h);
        ClosePath();
    }

    /**
     *  Fills the specified rectangle on the page.
     *  The left and right edges of the rectangle are at x and x + w.
     *  The top and bottom edges are at y and y + h.
     *  The rectangle is drawn using the current pen color.
     *
     *  @param x the x coordinate of the rectangle to be drawn.
     *  @param y the y coordinate of the rectangle to be drawn.
     *  @param w the width of the rectangle to be drawn.
     *  @param h the height of the rectangle to be drawn.
     */
//    public void FillRect(double x, double y, double w, double h) {
//        MoveTo(x, y);
//        LineTo(x+w, y);
//        LineTo(x+w, y+h);
//        LineTo(x, y+h);
//        FillPath();
//    }

    /**
     * Draws or fills the specified path using the current pen or brush.
     *
     * @param path list of the path points.
     */
/*
    public void DrawPath(List<Point> path) {
        if (path.Count < 2) {
            throw new Exception("The Path object must contain at least 2 points");
        }
        Point point = path[0];
        MoveTo(point.x, point.y);
        bool curve = false;
        for (int i = 1; i < path.Count; i++) {
            point = path[i];
            if (point.isControlPoint) {
                curve = true;
                Append(point);
            } else {
                if (curve) {
                    curve = false;
                    Append(point);
                    Append("c\n");
                } else {
                    LineTo(point.x, point.y);
                }
            }
        }
    }
*/
    /**
     * Strokes a bezier curve and draws it using the current pen.
     * @deprecated  As of v4.00 replaced by {@link #drawPath(List, char)}
     *
     * @param list the list of points that define the bezier curve.
     */
//    public void DrawBezierCurve(List<Point> list) {
//        DrawBezierCurve(list, Operation.STROKE);
//    }

    /**
     * Draws a bezier curve and fills it using the current brush.
     * @deprecated  As of v4.00 replaced by {@link #drawPath(List, char)}
     *
     * @param list the list of points that define the bezier curve.
     * @param operation must be Operation.STROKE or Operation.FILL.
     */
/*
    public void DrawBezierCurve(List<Point> list, char operation) {
        Point point = list[0];
        MoveTo(point.x, point.y);
        for (int i = 1; i < list.Count; i++) {
            point = list[i];
            Append(point);
            if (i % 3 == 0) {
                Append("c\n");
            }
        }
        Append(operation);
        Append('\n');
    }
*/
    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param rx the horizontal radius of the ellipse to be drawn.
     *  @param ry the vertical radius of the ellipse to be drawn.
     *  @param operation must be: Operation.FILL
     */
/*
    public void DrawEllipse(
            float x,
            float y,
            float rx,
            float ry) {
        DrawArc(x, y, rx, ry, 0f, 360f);
    }

    internal void DrawCircle(float x, float y, float r) {
        DrawEllipse(x, y, r, r);
    }
*/
    /**
     *  Draws an ellipse on the page and fills it using the current brush color.
     *
     *  @param x the x coordinate of the center of the ellipse to be drawn.
     *  @param y the y coordinate of the center of the ellipse to be drawn.
     *  @param r1 the horizontal radius of the ellipse to be drawn.
     *  @param r2 the vertical radius of the ellipse to be drawn.
     *  @param operation the operation.
     */
/*
    internal void DrawEllipse(
            float x,
            float y,
            float r1,
            float r2,
            float[] brushColor,
            float penWidth,
            float[] penColor,
            String pattern) {
        // The best 4-spline magic number
        float m4 = 0.55228f;
        // Starting point
        MoveTo(x, y - r2);

        AppendPointXY(x + m4*r1, y - r2);
        AppendPointXY(x + r1, y - m4*r2);
        AppendPointXY(x + r1, y);
        Append("c\n");

        AppendPointXY(x + r1, y + m4*r2);
        AppendPointXY(x + m4*r1, y + r2);
        AppendPointXY(x, y + r2);
        Append("c\n");

        AppendPointXY(x - m4*r1, y + r2);
        AppendPointXY(x - r1, y + m4*r2);
        AppendPointXY(x - r1, y);
        Append("c\n");

        AppendPointXY(x - r1, y - m4*r2);
        AppendPointXY(x - m4*r1, y - r2);
        AppendPointXY(x, y - r2);
        Append("c\n");

        if (brushColor != null) {
            SetBrushColor(brushColor);
        }
        if (penColor != null) {
            SetPenWidth(penWidth);
            SetPenColor(penColor);
        }
        if (pattern != null) {
            SetStrokePattern(pattern);
        }
        if (brushColor != null && penColor != null) {
            Append("B\n");
        } else if (brushColor != null && penColor == null) {
            Append("f\n");
        } else if (brushColor == null && penColor != null) {
            Append("S\n");
        } else {
            // Both brushColor == null and penColor == null
            SetPenWidth(0f);
            SetPenColor(0f, 0f, 0f);
            Append("S\n");
        }
    }
*/
    /**
     *  Draws a point on the page using the current pen color.
     *
     *  @param p the point.
     */
/*
    public void DrawPoint(Point p) {
        if (p.shape != Point.INVISIBLE) {
            List<Point> list;
            if (p.shape == Point.CIRCLE) {
                DrawCircle(p.x, p.y, p.r);
            } else if (p.shape == Point.DIAMOND) {
                list = new List<Point>();
                list.Add(new Point(p.x, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y));
                list.Add(new Point(p.x, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y));
                DrawPath(list);
                FillPath();
            } else if (p.shape == Point.BOX) {
                list = new List<Point>();
                list.Add(new Point(p.x - p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y + p.r));
                DrawPath(list);
                FillPath();
            } else if (p.shape == Point.PLUS) {
                DrawLine(p.x - p.r, p.y, p.x + p.r, p.y);
                DrawLine(p.x, p.y - p.r, p.x, p.y + p.r);
            } else if (p.shape == Point.UP_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y + p.r));
                list.Add(new Point(p.x, p.y - p.r));
                DrawPath(list);
                FillPath();
            } else if (p.shape == Point.DOWN_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x - p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y - p.r));
                list.Add(new Point(p.x, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y - p.r));
                DrawPath(list);
                FillPath();
            } else if (p.shape == Point.LEFT_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x + p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y));
                list.Add(new Point(p.x + p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y + p.r));
                DrawPath(list);
                FillPath();
            } else if (p.shape == Point.RIGHT_ARROW) {
                list = new List<Point>();
                list.Add(new Point(p.x - p.r, p.y - p.r));
                list.Add(new Point(p.x + p.r, p.y));
                list.Add(new Point(p.x - p.r, p.y + p.r));
                list.Add(new Point(p.x - p.r, p.y - p.r));
                DrawPath(list);
                FillPath();
            } else if (p.shape == Point.H_DASH) {
                DrawLine(p.x - p.r, p.y, p.x + p.r, p.y);
            } else if (p.shape == Point.V_DASH) {
                DrawLine(p.x, p.y - p.r, p.x, p.y + p.r);
            } else if (p.shape == Point.X_MARK) {
                DrawLine(p.x - p.r, p.y - p.r, p.x + p.r, p.y + p.r);
                DrawLine(p.x - p.r, p.y + p.r, p.x + p.r, p.y - p.r);
            } else if (p.shape == Point.MULTIPLY) {
                DrawLine(p.x - p.r, p.y - p.r, p.x + p.r, p.y + p.r);
                DrawLine(p.x - p.r, p.y + p.r, p.x + p.r, p.y - p.r);
                DrawLine(p.x - p.r, p.y, p.x + p.r, p.y);
                DrawLine(p.x, p.y - p.r, p.x, p.y + p.r);
            } else if (p.shape == Point.STAR) {
                float angle = (float) Math.PI / 10;
                float sin18 = (float) Math.Sin(angle);
                float cos18 = (float) Math.Cos(angle);
                float a = p.r * cos18;
                float b = p.r * sin18;
                float c = 2 * a * sin18;
                float d = 2 * a * cos18 - p.r;
                list = new List<Point>();
                list.Add(new Point(p.x, p.y - p.r));
                list.Add(new Point(p.x + c, p.y + d));
                list.Add(new Point(p.x - a, p.y - b));
                list.Add(new Point(p.x + a, p.y - b));
                list.Add(new Point(p.x - c, p.y + d));
                list.Add(new Point(p.x, p.y - p.r));
                DrawPath(list);
                FillPath();
            }
        }
    }
*/
    /**
     *  Sets the text rendering mode.
     *
     *  @param mode the rendering mode.
     */
/*
    public void SetTextRenderingMode(int mode) {
        if (mode >= 0 && mode <= 7) {
            this.renderingMode = mode;
        } else {
            throw new Exception("Invalid text rendering mode: " + mode);
        }
    }
*/
    /**
     *  Sets the text direction.
     *
     *  @param degrees the angle.
     */
//    public void SetTextDirection(int degrees) {
//        if (degrees > 360) degrees %= 360;
//        if (degrees == 0) {
//            tm = new float[] {1f,  0f,  0f,  1f};
//        } else if (degrees == 90) {
//            tm = new float[] {0f,  1f, -1f,  0f};
//        } else if (degrees == 180) {
//            tm = new float[] {-1f,  0f,  0f, -1f};
//        } else if (degrees == 270) {
//            tm = new float[] {0f, -1f,  1f,  0f};
//        } else if (degrees == 360) {
//            tm = new float[] {1f,  0f,  0f,  1f};
//        } else {
//            float sinOfAngle = (float) Math.Sin(degrees * (Math.PI / 180));
//            float cosOfAngle = (float) Math.Cos(degrees * (Math.PI / 180));
//            tm = new float[] {cosOfAngle, sinOfAngle, -sinOfAngle, cosOfAngle};
//        }
//        tm0 = FastFloat.ToByteArray(tm[0]);
//        tm1 = FastFloat.ToByteArray(tm[1]);
//        tm2 = FastFloat.ToByteArray(tm[2]);
//        tm3 = FastFloat.ToByteArray(tm[3]);
//    }

    /**
     *  Draws a cubic bezier curve starting from the current point to the end point p3
     *
     *  @param x1 first control point x
     *  @param y1 first control point y
     *  @param x2 second control point x
     *  @param y2 second control point y
     *  @param x3 end point x
     *  @param y3 end point y
     */
/*
    public void CurveTo(
            float x1, float y1, float x2, float y2, float x3, float y3) {
        Append(x1);
        Append(' ');
        Append(height - y1);
        Append(' ');
        Append(x2);
        Append(' ');
        Append(height - y2);
        Append(' ');
        Append(x3);
        Append(' ');
        Append(height - y3);
        Append(" c\n");
    }

    public float[] DrawCircularArc(
            float x, float y, float r, float startAngle, float sweepDegrees) {
        return DrawArc(x, y, r, r, startAngle, sweepDegrees);
    }

    public float[] DrawArc(
            float x,
            float y,
            float rx,
            float ry,
            float startAngle,
            float sweepDegrees) {
        float x1 = 0f;
        float y1 = 0f;
        float x2 = 0f;
        float y2 = 0f;
        float x3 = 0f;
        float y3 = 0f;

        int numSegments = (int)Math.Ceiling(Math.Abs(sweepDegrees) / 90.0);
        double angleRad = startAngle * Math.PI / 180.0;
        double deltaPerSeg = (sweepDegrees / numSegments) * Math.PI / 180.0;
        for (int i = 0; i < numSegments; i++) {
            double segStart = angleRad;
            double segEnd   = angleRad + deltaPerSeg;
            double deltaRad = segEnd - segStart; // guaranteed ≤ ±π/2

            // Calculate safe κ
            float k = (float)(4.0 / 3.0 * Math.Tan(deltaRad / 4.0));

            float cosStart = (float)Math.Cos(segStart);
            float sinStart = (float)Math.Sin(segStart);
            float cosEnd   = (float)Math.Cos(segEnd);
            float sinEnd   = (float)Math.Sin(segEnd);

            // End points
            float x0 = x + rx * cosStart;
            float y0 = y + ry * sinStart;
            x3 = x + rx * cosEnd;
            y3 = y + ry * sinEnd;

            // Control points
            x1 = x0 - (k * rx * sinStart);
            y1 = y0 + (k * ry * cosStart);
            x2 = x3 + (k * rx * sinEnd);
            y2 = y3 - (k * ry * cosEnd);

            if (i == 0) {
                MoveTo(x0, y0);
            }
            CurveTo(x1, y1, x2, y2, x3, y3);

            angleRad = segEnd;
        }

        return new float[6] { x1, y1, x2, y2, x3, y3 };
    }
*/
    /**
     *  Draws a bezier curve starting from the current point.
     *  <strong>Please note:</strong> You must call the StrokePath,
     *  ClosePath or FillPath methods after the last BezierCurveTo call.
     *  <p><i>Author:</i> <strong>Pieter Libin</strong>, pieter@emweb.be</p>
     *
     *  @param p1 this first control point.
     *  @param p2 this second control point.
     *  @param p3 this end point.
     */
/*
    public void BezierCurveTo(Point p1, Point p2, Point p3) {
        Append(p1);
        Append(p2);
        Append(p3);
        Append("c\n");
    }

    public void SetTextFont(Font font) {
        this.font = font;
        if (font.fontID != null) {
            Append('/');
            Append(font.fontID);
        } else {
            Append("/F");
            Append(font.objNumber);
        }
        Append(Token.space);
        Append(font.size);
        Append(" Tf\n");
    }
*/
    // Code provided by:
    // Dominique Andre Gunia <contact@dgunia.de>
    // <<
/*
    public void DrawRectRoundCorners(
            float x,
            float y,
            float w,
            float h,
            float r1,
            float r2,
            float[] fillColor,
            float strokeWidth,
            float[] strokeColor) {
        // The best 4-spline magic number
        float m4 = 0.55228f;
        List<Point> list = new List<Point>();

        // Starting point
        list.Add(new Point(x + w - r1, y));
        list.Add(new Point(x + w - r1 + m4*r1, y, Point.CONTROL_POINT));
        list.Add(new Point(x + w, y + r2 - m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x + w, y + r2));

        list.Add(new Point(x + w, y + h - r2));
        list.Add(new Point(x + w, y + h - r2 + m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x + w - m4*r1, y + h, Point.CONTROL_POINT));
        list.Add(new Point(x + w - r1, y + h));

        list.Add(new Point(x + r1, y + h));
        list.Add(new Point(x + r1 - m4*r1, y + h, Point.CONTROL_POINT));
        list.Add(new Point(x, y + h - m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x, y + h - r2));

        list.Add(new Point(x, y + r2));
        list.Add(new Point(x, y + r2 - m4*r2, Point.CONTROL_POINT));
        list.Add(new Point(x + m4*r1, y, Point.CONTROL_POINT));
        list.Add(new Point(x + r1, y));
        list.Add(new Point(x + w - r1, y));

        DrawPath(list);
    }
*/
    /**
     *  Clips the path.
     */
    public void ClipPath() {
        Append("W\n");
        Append("n\n");  // Close the path without painting it.
    }

    public void ClipRect(float x, float y, float w, float h) {
        MoveTo(x, y);
        LineTo(x + w, y);
        LineTo(x + w, y + h);
        LineTo(x, y + h);
        ClipPath();
    }

    public void Save() {
        Append("q\n");
        savedStates.Add(new State(
                brushColor, penColor, penWidth, lineCapStyle, lineJoinStyle, strokePattern));
    }

    public void Restore() {
        Append("Q\n");
        if (savedStates.Count > 0) {
            int lastIndex = savedStates.Count - 1;
            State savedState = savedStates[lastIndex];
            brushColor = savedState.GetBrushColor();
            penColor = savedState.GetPenColor();
            penWidth = savedState.GetPenWidth();
            lineCapStyle = savedState.GetLineCapStyle();
            lineJoinStyle = savedState.GetLineJoinStyle();
            strokePattern = savedState.GetStrokePattern();
            savedStates.RemoveAt(lastIndex);
        }
    }
    // <<

    /**
     * Sets the page CropBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the CropBox.
     * @param upperLeftY the top left Y coordinate of the CropBox.
     * @param lowerRightX the bottom right X coordinate of the CropBox.
     * @param lowerRightY the bottom right Y coordinate of the CropBox.
     */
    public void SetCropBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.cropBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }

    /**
     * Sets the page BleedBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the BleedBox.
     * @param upperLeftY the top left Y coordinate of the BleedBox.
     * @param lowerRightX the bottom right X coordinate of the BleedBox.
     * @param lowerRightY the bottom right Y coordinate of the BleedBox.
     */
    public void SetBleedBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.bleedBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }

    /**
     * Sets the page TrimBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the TrimBox.
     * @param upperLeftY the top left Y coordinate of the TrimBox.
     * @param lowerRightX the bottom right X coordinate of the TrimBox.
     * @param lowerRightY the bottom right Y coordinate of the TrimBox.
     */
    public void SetTrimBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.trimBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }

    /**
     * Sets the page ArtBox.
     * See page 77 of the PDF32000_2008.pdf specification.
     *
     * @param upperLeftX the top left X coordinate of the ArtBox.
     * @param upperLeftY the top left Y coordinate of the ArtBox.
     * @param lowerRightX the bottom right X coordinate of the ArtBox.
     * @param lowerRightY the bottom right Y coordinate of the ArtBox.
     */
    public void SetArtBox(
            float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY) {
        this.artBox = new float[] {upperLeftX, upperLeftY, lowerRightX, lowerRightY};
    }

    private void AppendPointXY(float x, float y) {
        Append(x);
        Append(' ');
        Append(height - y);
        Append(' ');
    }

    private void Append(Point point) {
        Append(point.x);
        Append(' ');
        Append(height - point.y);
        Append(' ');
    }

//    internal void Append(String str) {
//        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
//        buf.Write(bytes, 0, bytes.Length);
//    }
//
//    internal void Append(int num) {
//        Append(num.ToString());
//    }
//
//    internal void Append(float f) {
//        Append(FastFloat.ToByteArray(f));
//    }
//
//    internal void Append(char ch) {
//        buf.WriteByte((byte) ch);
//    }
//
//    internal void Append(byte b) {
//        buf.WriteByte(b);
//    }
//
//    /**
//     * Appends the specified array of bytes to the page.
//     */
//    public void Append(byte[] buffer) {
//        buf.Write(buffer, 0, buffer.Length);
//    }
/*
    private static readonly byte[] HEX = {
        (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9',
        (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F'
    };
    private void AppendCodePointAsHex(int codePoint) {
        if (codePoint <= 0xFFFF) {
            // Basic Multilingual Plane (BMP) character
            base.buf.WriteByte(HEX[(codePoint >> 12) & 0xF]);
            base.buf.WriteByte(HEX[(codePoint >> 8)  & 0xF]);
            base.buf.WriteByte(HEX[(codePoint >> 4)  & 0xF]);
            base.buf.WriteByte(HEX[(codePoint)       & 0xF]);
        } else {
            // Supplementary character (needs surrogate pair in UTF-16)
            // Write as 6 hex digits (max Unicode code point is 0x10FFFF)
            base.buf.WriteByte(HEX[(codePoint >> 20) & 0xF]);
            base.buf.WriteByte(HEX[(codePoint >> 16) & 0xF]);
            base.buf.WriteByte(HEX[(codePoint >> 12) & 0xF]);
            base.buf.WriteByte(HEX[(codePoint >> 8)  & 0xF]);
            base.buf.WriteByte(HEX[(codePoint >> 4)  & 0xF]);
            base.buf.WriteByte(HEX[(codePoint)       & 0xF]);
        }
    }

    private void DrawWord(
            Font font,
            StringBuilder sb,
            float[] color,
            Dictionary<String, Int32> highlightColors) {
        if (sb.Length > 0) {
            String str = sb.ToString();
            if (highlightColors.ContainsKey(str.ToLower())) {
                SetBrushColor(highlightColors[str.ToLower()]);
            } else {
                SetBrushColor(color);
            }
            if (font.isCoreFont) {
                Append("[<");
                DrawASCIIString(font, str);
                Append(">] TJ\n");
            } else {
                Append("<");
                DrawUnicodeString(font, str);
                Append("> Tj\n");
            }
            sb.Length = 0;
        }
    }

    internal void DrawColoredString(
            Font font,
            String str,
            float[] color,
            Dictionary<String, Int32> highlightColors) {
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        for (int i = 0; i < str.Length; i++) {
            char ch = str[i];
            if (Char.IsLetterOrDigit(ch)) {
                DrawWord(font, sb2, color, highlightColors);
                sb1.Append(ch);
            } else {
                DrawWord(font, sb1, color, highlightColors);
                sb2.Append(ch);
            }
        }
        DrawWord(font, sb1, color, highlightColors);
        DrawWord(font, sb2, color, highlightColors);
    }
*/
    internal void SetStructElementsPageObjNumber(int pageObjNumber) {
        foreach (StructElem element in structures) {
            element.pageObjNumber = pageObjNumber;
        }
    }

    public void AddBMC(
            String structure,
            String actualText,
            String altDescription) {
        AddBMC(structure, null, actualText, altDescription);
    }

    public void AddBMC(
            String structure,
            String language,
            String actualText,
            String altDescription) {
        if (pdf.compliance == Compliance.PDF_UA_1) {
            StructElem element = new StructElem();
            element.structure = structure;
            element.mcid = mcid;
            element.language = language;
            element.actualText = actualText;
            element.altDescription = altDescription;
            structures.Add(element);

            Append("/");
            Append(structure);
            Append(" <</MCID ");
            Append(mcid++);
            Append(">>\n");
            Append("BDC\n");
        }
    }

    public void AddArtifactBMC() {
        if (pdf.compliance == Compliance.PDF_UA_1) {
            Append("/Artifact BMC\n");
        }
    }

    public void AddEMC() {
        if (pdf.compliance == Compliance.PDF_UA_1) {
            Append("EMC\n");
        }
    }

    internal void AddAnnotation(Annotation annotation) {
        annotation.y1 = this.height - annotation.y1;
        annotation.y2 = this.height - annotation.y2;
        annots.Add(annotation);
        if (pdf.compliance == Compliance.PDF_UA_1) {
            StructElem element = new StructElem();
            element.structure = StructElem.LINK;
            element.language = annotation.language;
            element.actualText = annotation.actualText;
            element.altDescription = annotation.altDescription;
            element.annotation = annotation;
            structures.Add(element);
        }
    }
/*
    internal void BeginTransform(
            float x, float y, float xScale, float yScale) {
        Append("q\n");

        Append(xScale);
        Append(" 0 0 ");
        Append(yScale);
        Append(' ');
        Append(x);
        Append(' ');
        Append(y);
        Append(" cm\n");

        Append(xScale);
        Append(" 0 0 ");
        Append(yScale);
        Append(' ');
        Append(x);
        Append(' ');
        Append(y);
        Append(" Tm\n");
    }

    internal void EndTransform() {
        Append("Q\n");
    }

    public void DrawContents(
            byte[] content,
            float h,    // The height of the graphics object in points.
            float x,
            float y,
            float xScale,
            float yScale) {
        BeginTransform(x, (this.height - yScale * h) - y, xScale, yScale);
        Append(content);
        EndTransform();
    }

    public void DrawString(Font font, String str, float x, float y, float dx) {
        float x1 = x;
        for (int i = 0; i < str.Length; i++) {
            DrawString(font, str.Substring(i, 1), x1, y);
            x1 += dx;
        }
    }
*/
    public void AddWatermark(Font font, String text) {
        float hypotenuse = (float)
                Math.Sqrt(this.height * this.height + this.width * this.width);
        float stringWidth = font.StringWidth(text);
        float offset = (hypotenuse - stringWidth) / 2f;
        double angle = Math.Atan(this.height / this.width);
        TextLine watermark = new TextLine(font);
        watermark.SetText(text);
        watermark.SetTextColor(Color.lightgrey);
        watermark.SetLocation(
                (float) (offset * Math.Cos(angle)),
                (this.height - (float) (offset * Math.Sin(angle))));
        watermark.SetTextDirection((int) (angle * (180.0 / Math.PI)));
        watermark.DrawOn(this);
    }

    public void InvertYAxis() {
        Append("1 0 0 -1 0 ");
        Append(this.height);
        Append(" cm\n");
    }

    /**
     * Transformation matrix.
     */
    public void Transform(float[] values) {
        float scalex = values[MSCALE_X];
        float scaley = values[MSCALE_Y];
        float transx = values[MTRANS_X];
        float transy = values[MTRANS_Y];

        Append(scalex);
        Append(Token.space);
        Append(values[MSKEW_X]);
        Append(Token.space);
        Append(values[MSKEW_Y]);
        Append(Token.space);
        Append(scaley);
        Append(Token.space);

        if (Math.Asin(values[MSKEW_Y]) != 0f) {
            transx -= values[MSKEW_Y] * height / scaley;
        }
        Append(transx);
        Append(Token.space);
        Append(-transy);
        Append(" cm\n");

        // Weil mit der Hoehe immer die Y-Koordinate im PDF-Koordinatensystem berechnet wird:
        height = height / scaley;
    }

    public float[] AddHeader(TextLine textLine) {
        return AddHeader(textLine, 1.5f*textLine.font.ascent);
    }

    public float[] AddHeader(TextLine textLine, float offset) {
        textLine.SetLocation((GetWidth() - textLine.GetWidth())/2, offset);
        float[] xy = textLine.DrawOn(this);
        xy[1] += font.descent;
        return xy;
    }

    public float[] AddFooter(TextLine textLine) {
        return AddFooter(textLine, textLine.font.ascent);
    }

    public float[] AddFooter(TextLine textLine, float offset) {
        textLine.SetLocation((GetWidth() - textLine.GetWidth())/2, GetHeight() - offset);
        return textLine.DrawOn(this);
    }

    /**
     *  Sets the text location.
     *
     *  @param x the x coordinate of new text location.
     *  @param y the y coordinate of new text location.
     */
/*
    internal void SetTextLocation(float x, float y) {
        Append(x);
        Append(Token.space);
        Append(height - y);
        Append(" Td\n");
    }
*/
    /**
     *  Sets the text leading.
     *  @param leading the leading.
     */
//    internal void SetTextLeading(float leading) {
//        Append(leading);
//        Append(" TL\n");
//    }

    /**
     *  Advance to the next line.
     */
//    internal void NextLine() {
//        Append("T*\n");
//    }

//    internal void SetTextScaling(float scaling) {
//        Append(scaling);
//        Append(" Tz\n");
//    }

//    internal void SetTextRise(float rise) {
//        Append(rise);
//        Append(" Ts\n");
//    }

    /**
     *  Draws a string at the specified location.
     *  @param str the string.
     */
/*
    internal void DrawText(String str) {
        if (font.isCoreFont) {
            Append("[<");
            DrawASCIIString(font, str);
            Append(">] TJ\n");
        } else {
            Append("<");
            DrawUnicodeString(font, str);
            Append("> Tj\n");
        }
    }

    internal void ScaleAndRotate(float x, float y, float w, float h, float degrees) {
        // PDF transformations apply LAST-TO-FIRST (like a stack: last command = first applied)

        // [FINAL POSITIONING - Applied First]
        // Moves rotated/scaled image to target (x,y) on page
        Append("1 0 0 1 ");
        Append(x + w/2);
        Append(" ");
        Append((height - y) - h/2);
        Append(" cm\n");

        // [ROTATION - Applied Second]
        // Rotates around current origin (0,0) by 'degrees'
        double radians = degrees * (Math.PI / 180);
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        Append(FastFloat.ToByteArray(cos));
        Append(" ");
        Append(FastFloat.ToByteArray(sin));
        Append(" ");
        Append(FastFloat.ToByteArray(-sin));
        Append(" ");
        Append(FastFloat.ToByteArray(cos));
        Append(" 0 0 cm\n");

        // [ORIGIN SETUP - Applied Last]
        // Centers image at (0,0) and sets scale
        Append(w);
        Append(" 0 0 ");
        Append(h);
        Append(" ");
        Append(-w/2);
        Append(" ");
        Append(-h/2);
        Append(" cm\n");
    }

    internal void RotateAroundCenter(float centerX, float centerY, float degrees) {
        Append("1 0 0 1 ");
        Append(centerX);
        Append(" ");
        Append(centerY);
        Append(" cm\n");

        double radians = degrees * Math.PI / 180;
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        Append(FastFloat.ToByteArray(cos));
        Append(" ");
        Append(FastFloat.ToByteArray(sin));
        Append(" ");
        Append(FastFloat.ToByteArray(-sin));
        Append(" ");
        Append(FastFloat.ToByteArray(cos));
        Append(" 0 0 cm\n");

        Append("1 0 0 1 ");
        Append(-centerX);
        Append(" ");
        Append(-centerY);
        Append(" cm\n");
    }
*/
}   // End of Page.cs
}   // End of namespace PDFjet.NET
