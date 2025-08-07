

class TextLineWithOffset {
    var textLine: String;   // A single line of text
    var xOffset: Float;     // The horizontal offset (from the X coordinate)

    // Constructor with clear parameter names
    init(_ textLine: String, _ xOffset: Float) {
        self.textLine = textLine;
        self.xOffset = xOffset;
    }
}
