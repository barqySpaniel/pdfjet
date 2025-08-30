/**
 * State.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet;

class State {
    private float[] pen;
    private float[] brush;
    private float penWidth;
    private CapStyle lineCapStyle;
    private JoinStyle lineJoinStyle;
    private String linePattern;

    public State(
            float[] pen,
            float[] brush,
            float penWidth,
            CapStyle lineCapStyle,
            JoinStyle lineJoinStyle,
            String linePattern) {
        this.pen = new float[] { pen[0], pen[1], pen[2] };
        this.brush = new float[] { brush[0], brush[1], brush[2] };
        this.penWidth = penWidth;
        this.lineCapStyle = lineCapStyle;
        this.lineJoinStyle = lineJoinStyle;
        this.linePattern = linePattern;
    }

    public float[] getPen() {
        return pen;
    }

    public float[] getBrush() {
        return brush;
    }

    public float getPenWidth() {
        return penWidth;
    }

    public CapStyle getLineCapStyle() {
        return lineCapStyle;
    }

    public JoinStyle getLineJoinStyle() {
        return lineJoinStyle;
    }

    public String getLinePattern() {
        return linePattern;
    }
}   // End of State.java
