using System;

namespace PDFjet.NET {
    public class TextLineWithOffset {
        internal String textLine;   // A single line of text
        internal float xOffset;     // The horizontal offset (from the X coordinate)

        // Constructor with clear parameter names
        protected TextLineWithOffset(String textLine, float xOffset) {
            this.textLine = textLine;
            this.xOffset = xOffset;
        }
    }
}
