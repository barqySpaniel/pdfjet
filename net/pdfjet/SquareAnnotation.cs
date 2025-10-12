/**
 * SquareAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class SquareAnnotation : IDrawable {
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

    public SquareAnnotation() {
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

    public void SetTitle(String title) {
        this.title = title;
    }

    public void SetContents(String contents) {
        this.contents = contents;
    }

    public float[] DrawOn(Page page) {
        page.AddAnnotation(new Annotation(
                Annotation.Square,
                x,
                y,
                x + w,
                y + h,
                null,       // Vertices
                title,      // Title
                contents,   // Contents
                uri,        //
                key,        // The destination name
                language,
                actualText,
                altDescription));
        return new float[] {x + w, y + h};
    }
}
}
