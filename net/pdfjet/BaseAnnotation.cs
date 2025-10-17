/**
 * BaseAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class BaseAnnotation : IDrawable {
    internal float x1 = 0f;
    internal float y1 = 0f;
    internal float x2 = 0f;
    internal float y2 = 0f;
    internal String title = null;
    internal String contents = null;
    internal String uri = null;
    internal String key = null;
    internal String language = null;
    internal String actualText = null;
    internal String altDescription = null;
    internal float[] fillColor = new float[] {0.5f, 0.5f, 0.5f};
    internal float[] vertices = null;
    internal float transparency = 1f;
    internal Container container = null;
    internal String annotationType = null;

    public BaseAnnotation() {
    }

    public void SetLocation(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }

    public void SetPosition(float x, float y) {
        this.x1 = x;
        this.y1 = y;
    }

    public void SetSize(float w, float h) {
        this.x2 = x1 + w;
        this.y2 = y1 + h;
    }

    public void SetFillColor(float[] fillColor) {
        this.fillColor = fillColor;
    }

    public void SetFillColor(int color) {
        float r = ((color >> 16) & 0xff)/255f;
        float g = ((color >>  8) & 0xff)/255f;
        float b = ((color)       & 0xff)/255f;
        SetFillColor(new float[] {r, g, b});
    }

    public void SetTransparency(float transparency) {
        this.transparency = transparency;
    }

    public void SetTitle(String title) {
        this.title = title;
    }

    public void SetContents(String contents) {
        this.contents = contents;
    }

    public float[] DrawOn(Page page) {
        page.AddAnnotation(new Annotation(
                annotationType,
                x1,
                y1,
                x2,
                y2,
                vertices,       // Vertices
                fillColor,      // Fill Color
                transparency,   // Transparency
                title,          // Title
                contents,       // Contents
                uri,            //
                key,            // The destination name
                language,
                actualText,
                altDescription));
        return new float[] {x2, y2};
    }
}
}
