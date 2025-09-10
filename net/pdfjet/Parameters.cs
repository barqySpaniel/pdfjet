using System;
using System.Text;

namespace PDFjet.NET {
public class Parameters {
    internal Font font;
    internal float fontSize;
    internal float x;
    internal float y;

    // Constructor to initialize with default values (optional)
    public Parameters() {
        this.fontSize = 12f;    // Default font size
        this.x = 0f;            // Default X
        this.y = 0f;            // Default Y
    }

    // Method to set the font
    public Parameters SetFont(Font font) {
        this.font = font;
        return this;
    }

    // Method to set the font size
    public Parameters SetFontSize(float fontSize) {
        this.fontSize = fontSize;
        return this;
    }

    // Method to set the location (X, Y)
    public Parameters SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
        return this;
    }
}   // End of TextParams.cs
}   // End of namespace PDFjet.NET
