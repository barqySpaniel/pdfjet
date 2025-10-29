/**
 * TextSegment.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet;

/**
 * Used to create text segment objects.
 */
public class TextSegment implements Drawable {
    protected float x;
    protected float y;
    protected Font f1;
    protected Font f2;
    protected float f1Size;
    protected float f2Size;
    // protected float[] textColor = new float[] {0f, 0f, 0f};
    protected int textColor;
    protected String text;

    public TextSegment(Font f1, Font f2) {
        this.f1 = f1;
        this.f2 = f2;
    }

    public TextSegment setText(String text) {
        this.text = text;
        return this;
    }

    public String getText() {
        return text;
    }

    /**
     *  Sets the position where this text line will be drawn on the page.
     *
     *  @param x the x coordinate of the text line.
     *  @param y the y coordinate of the text line.
     */
    public void setPosition(float x, float y) {
        setLocation(x, y);
    }

    /**
     *  Sets the position where this text line will be drawn on the page.
     *
     *  @param x the x coordinate of the text line.
     *  @param y the y coordinate of the text line.
     */
    public void setPosition(double x, double y) {
        setLocation(x, y);
    }

    /**
     *  Sets the location where this text line will be drawn on the page.
     *
     *  @param x the x coordinate of the text line.
     *  @param y the y coordinate of the text line.
     *  @return this TextSegment.
     */
    public TextSegment setLocation(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }

    public TextSegment setLocation(double x, double y) {
        return setLocation((float) x, (float) y);
    }

    /**
     *  Sets the font size to use for this text line.
     *
     *  @param fontSize the fontSize to use.
     *  @return this TextLine.
     */
    public TextSegment setFontSize(float fontSize) {
        this.f1.setSize(fontSize);
        this.f2.setSize(fontSize);
        return this;
    }

    public void setTextColor(int color) {
        this.textColor = color;
    }

//     public void setTextColor(int color) {
//         float r = ((color >> 16) & 0xff)/255f;
//         float g = ((color >>  8) & 0xff)/255f;
//         float b = ((color)       & 0xff)/255f;
//         this.textColor = new float[] {r, g, b};
//     }
//
//     public void setTextColor(float r, float g, float b) {
//         this.textColor = new float[] {r, g, b};
//     }
//
//     public void setTextColor(float[] rgbColor) {
//         this.textColor = rgbColor;
//     }
//
//     public float[] getTextColor() {
//         return textColor;
//     }

    public float getStringWidth(String text) {
        return f1.stringWidth(f2, text);
    }

    /**
     *  Returns the height of this TextLine.
     *
     *  @return the height.
     */
//     public float getHeight() {
//         float ascent = Math.max(font.ascent, fallbackFont.ascent);
//         float descent = Math.max(font.descent, fallbackFont.descent);
//         return ascent + descent;
//     }

    /**
     *  Draws this text line on the specified page.
     *
     *  @param page the page to draw this text line on.
     *  @return float[] with the coordinates of the bottom right corner.
     *  @throws Exception  If an input or output exception occurred
     */
    public float[] drawOn(Page page) throws Exception {
        if (page == null || text == null || text.equals("")) {
            return new float[] {x, y};
        }

        page.drawString(f1, f2, text, x, y, textColor, null);

        return new float[] {x, y};  // TODO:
    }
}   // End of TextSegment.java
