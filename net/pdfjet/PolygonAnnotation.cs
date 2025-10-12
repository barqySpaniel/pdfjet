/**
 * PolygonAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class PolygonAnnotation : IDrawable {
    internal String title = null;
    internal String contents = null;
    internal String uri = null;
    internal String key = null;
    internal String language = null;
    internal String actualText = null;
    internal String altDescription = null;

    private float x = 0f;
    private float y = 0f;
    private float w = 0f;
    private float h = 0f;
    private float[] vertices = null;
    private float[] fillColor = new float[] {0.5f, 0.5f, 0.5f};

    public PolygonAnnotation() {
    }

    public void SetLocation(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetPosition(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public void SetSize(float w, float h) {
        this.w = w;
        this.h = h;
    }

    public void SetVertices(float[] vertices) {
        this.vertices = vertices;
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

    public void SetTitle(String title) {
        this.title = title;
    }

    public void SetContents(String contents) {
        this.contents = contents;
    }

    public float[] DrawOn(Page page) {
        page.AddAnnotation(new Annotation(
                Annotation.Polygon,
                x,
                y,
                x + w,
                y + h,
                vertices,   // Vertices
                fillColor,  // Fill Color
                title,      // Title
                contents,   // Contents
                uri,
                key,        // The destination name
                language,
                actualText,
                altDescription));
        return new float[] {0f, 0f};    // TODO:
    }
}
}
