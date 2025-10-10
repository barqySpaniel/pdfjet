/**
 * PopupAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class PopupAnnotation : IDrawable {
    private float x = 0f;
    private float y = 0f;

    public PopupAnnotation() {
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public float[] DrawOn(Page page) {
        return new float[] {0f, 0f};
    }
}
}
