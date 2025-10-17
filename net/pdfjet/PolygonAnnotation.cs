/**
 * PolygonAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;
using System.Collections.Generic;

namespace PDFjet.NET {
public class PolygonAnnotation : IDrawable {
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
    internal Container container = null;

    private float[] vertices = null;
    private float[] fillColor = new float[] {0.5f, 0.5f, 0.5f};
    private float transparency = 1f;

    public PolygonAnnotation() {
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

    public void SetTransparency(float transparency) {
        this.transparency = transparency;
    }

    public void SetTitle(String title) {
        this.title = title;
    }

    public void SetContents(String contents) {
        this.contents = contents;
    }

    public void Rotate(double rotateDegrees) {
        float[] rotateCenter = container.GetRotationCenter();
        float[] xy1 = Container.RotateAroundCenter(x1, y1, rotateCenter[0], rotateCenter[1], rotateDegrees);
        float[] xy2 = Container.RotateAroundCenter(x2, y2, rotateCenter[0], rotateCenter[1], rotateDegrees);
        this.x1 = xy1[0];
        this.y1 = xy1[1];
        this.x2 = xy2[0];
        this.y2 = xy2[1];

        List<float> list = new List<float>();
        for (int i = 0; i < vertices.Length; i += 2) {
            float[] xy = Container.RotateAroundCenter(
                vertices[i], vertices[i + 1], 0f, 0f, rotateDegrees);
            list.Add(xy[0]);
            list.Add(xy[1]);
        }
        vertices = list.ToArray();
    }

    public float[] DrawOn(Page page) {
        page.AddAnnotation(new Annotation(
                Annotation.Polygon,
                x1,
                y1,
                x2,
                y2,
                vertices,       // Vertices
                fillColor,      // Fill Color
                transparency,   // Transparency
                title,          // Title
                contents,       // Contents
                uri,
                key,            // The destination name
                language,
                actualText,
                altDescription));
        return new float[] {0f, 0f};    // TODO:
    }
}
}
