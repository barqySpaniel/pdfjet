package com.pdfjet;

class TextLineWithOffset {
    protected String textLine;  // A single line of text
    protected float xOffset;    // The horizontal offset (from the X coordinate)

    // Constructor with clear parameter names
    protected TextLineWithOffset(String textLine, float xOffset) {
        this.textLine = textLine;
        this.xOffset = xOffset;
    }
}
