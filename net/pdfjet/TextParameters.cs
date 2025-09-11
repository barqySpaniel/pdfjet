using System;
using System.Text;

namespace PDFjet.NET {
public class TextParameters {
    internal Font font;
    internal float fontSize;
    internal float x;
    internal float y;
    internal String text;

    // Constructor to initialize with default values (optional)
    public TextParameters() {
        this.fontSize = 12f;    // Default font size
        this.x = 0f;            // Default X
        this.y = 0f;            // Default Y
    }

    // Method to set the font
    public TextParameters SetFont(Font font) {
        this.font = font;
        return this;
    }

    // Method to set the font size
    public TextParameters SetFontSize(float fontSize) {
        this.fontSize = fontSize;
        return this;
    }

    // Method to set the location (X, Y)
    public TextParameters SetTextLocation(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }

    public TextParameters SetText(String text) {
        this.text = text;
        return this;
    }
}   // End of TextParameters.cs
}   // End of namespace PDFjet.NET
