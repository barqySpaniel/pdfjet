/**
 *  State.cs
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

namespace PDFjet.NET {
class State {
    private float[] brushColor;
    private float[] penColor;
    private float penWidth;
    private CapStyle lineCapStyle;
    private JoinStyle lineJoinStyle;
    private String strokePattern;

    public State(
            float[] brushColor,
            float[] penColor,
            float penWidth,
            CapStyle lineCapStyle,
            JoinStyle lineJoinStyle,
            String strokePattern) {
        this.brushColor = new float[] { brushColor[0], brushColor[1], brushColor[2] }; // TODO: Is this needed?
        this.penColor = new float[] { penColor[0], penColor[1], penColor[2] };         // Creating new objects?
        this.penWidth = penWidth;
        this.lineCapStyle = lineCapStyle;
        this.lineJoinStyle = lineJoinStyle;
        this.strokePattern = strokePattern;
    }

    public float[] GetBrushColor() {
        return brushColor;
    }

    public float[] GetPenColor() {
        return penColor;
    }

    public float GetPenWidth() {
        return penWidth;
    }

    public CapStyle GetLineCapStyle() {
        return lineCapStyle;
    }

    public JoinStyle GetLineJoinStyle() {
        return lineJoinStyle;
    }

    public String GetStrokePattern() {
        return strokePattern;
    }
}   // End of State.cs
}   // End of namespace PDFjet.NET
