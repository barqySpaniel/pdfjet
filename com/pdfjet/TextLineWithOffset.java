package com.pdfjet;

class TextLineWithOffset {
    String textLine;    // A single line of text
    float xOffset;      // The horizontal offset (from the X coordinate)
    boolean underline = false;
    boolean strikeout = false;

    TextLineWithOffset(String textLine, float xOffset) {
        this.textLine = textLine;
        this.xOffset = xOffset;
    }
}
