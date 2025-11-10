package com.pdfjet;

class TextLineWithOffset {      // TODO: We don't really need this class!!!
    String textLine;    // A single line of text
    float xOffset;      // The horizontal offset (from the X coordinate)
    boolean underline = false;
    boolean strikeout = false;

    TextLineWithOffset(String textLine, float xOffset) {
        this.textLine = textLine;
        this.xOffset = xOffset;
    }
}
