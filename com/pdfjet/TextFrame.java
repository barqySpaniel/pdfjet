/**
 * TextFrame.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet;

import java.util.*;

/**
 *  Please see Example_47
 */
public class TextFrame implements Drawable {
    private List<TextLine> paragraphs;
    private final Font font;
    private float x;
    private float y;
    private float w;
    private float h;
    private float leading;
    private float paragraphLeading;
    private final List<float[]> beginParagraphPoints;
    private boolean border;

    public TextFrame(List<TextLine> paragraphs) {
        this.paragraphs = new ArrayList<TextLine>(paragraphs);
        this.font = paragraphs.get(0).getFont();
        this.leading = font.getBodyHeight();
        this.paragraphLeading = 2*leading;
        this.beginParagraphPoints = new ArrayList<float[]>();
        Font fallbackFont = paragraphs.get(0).getFallbackFont();
        if (fallbackFont != null && (fallbackFont.getBodyHeight() > this.leading)) {
            this.leading = fallbackFont.getBodyHeight();
        }
        Collections.reverse(this.paragraphs);
    }

    public TextFrame setLocation(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }

    public TextFrame setLocation(double x, double y) {
        return setLocation((float) x, (float) y);
    }

    public TextFrame setWidth(float w) {
        this.w = w;
        return this;
    }

    public TextFrame setWidth(double w) {
        return setWidth((float) w);
    }

    public TextFrame setHeight(float h) {
        this.h = h;
        return this;
    }

    public TextFrame setHeight(double h) {
        return setHeight((float) h);
    }

    public float getHeight() {
        return this.h;
    }

    public TextFrame setLeading(float leading) {
        this.leading = leading;
        return this;
    }

    public TextFrame setLeading(double leading) {
        return setLeading((float) leading);
    }

    public TextFrame setParagraphLeading(float paragraphLeading) {
        this.paragraphLeading = paragraphLeading;
        return this;
    }

    public TextFrame setParagraphLeading(double paragraphLeading) {
        return setParagraphLeading((float) paragraphLeading);
    }

    public void setParagraphs(List<TextLine> paragraphs) {
        this.paragraphs = paragraphs;
    }

    public List<TextLine> getParagraphs() {
        return this.paragraphs;
    }

    public List<float[]> getBeginParagraphPoints() {
        return this.beginParagraphPoints;
    }

    public void setBorder(boolean drawBorder) {
        this.border = drawBorder;
    }

    public void setPosition(float x, float y) {
        setLocation(x, y);
    }

    public float[] drawOn(Page page) throws Exception {
        float xText = x;
        float yText = y + font.ascent;
        while (paragraphs.size() > 0) {
            // The paragraphs are reversed so we can efficiently remove the first one:
            TextLine textLine = paragraphs.remove(paragraphs.size() - 1);
            textLine.setLocation(xText, yText);
            beginParagraphPoints.add(new float[] {xText, yText});
            while (true) {
                textLine = drawLineOnPage(textLine, page);
                if (textLine.getText().equals("")) {
                    break;
                }
                yText = textLine.advance(leading);
                if (yText + font.descent >= (y + h)) {
                    // The paragraphs are reversed so we can efficiently add new first paragraph:
                    paragraphs.add(textLine);
                    drawBorder(page);
                    return new float[] {this.x + this.w, this.y + this.h};
                }
            }
            xText = x;
            yText += paragraphLeading;
        }
        drawBorder(page);
        return new float[] {this.x + this.w, this.y + this.h};
    }

    private void drawBorder(Page page) throws Exception {
        if (page != null && border) {
            Box box = new Box();
            box.setLocation(x, y);
            box.setSize(w, h);
            box.drawOn(page);
        }
    }

    private TextLine drawLineOnPage(TextLine textLine, Page page) throws Exception {
        StringBuilder sb1 = new StringBuilder();
        StringBuilder sb2 = new StringBuilder();
        String[] tokens = textLine.getText().split("\\s+");
        boolean testForFit = true;
        for (String token : tokens) {
            if (testForFit && textLine.getStringWidth(sb1.toString() + token) < this.w) {
                sb1.append(token + Single.space);
            } else {
                testForFit = false;
                sb2.append(token + Single.space);
            }
        }
        textLine.setText(sb1.toString().trim());
        if (page != null) {
            textLine.drawOn(page);
        }
        textLine.setText(sb2.toString().trim());
        return textLine;
    }

    public boolean isNotEmpty() {
        return paragraphs.size() > 0;
    }
}   // End of TextFrame.java
