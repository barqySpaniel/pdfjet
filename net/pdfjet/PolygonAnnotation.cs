/**
 * PolygonAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;
using System.Collections.Generic;

namespace PDFjet.NET {
public class PolygonAnnotation : BaseAnnotation {
    public PolygonAnnotation() {
        base.annotationType = Annotation.Polygon;
    }

    public void SetVertices(float[] vertices) {
        base.vertices = vertices;
    }

    public void Rotate(double degrees) {
        float[] center = container.GetRotationCenter();
        float[] point1 = Container.RotateAroundCenter(new float[] {x1, y1}, center, degrees);
        float[] point2 = Container.RotateAroundCenter(new float[] {x2, y2}, center, degrees);
        this.x1 = point1[0];
        this.y1 = point1[1];
        this.x2 = point2[0];
        this.y2 = point2[1];

        List<float> list = new List<float>();
        for (int i = 0; i < vertices.Length; i += 2) {
            float[] xy = Container.RotateAroundCenter(
                new float[] {base.vertices[i], base.vertices[i + 1]}, new float[] {0f, 0f}, degrees);
            list.Add(xy[0]);
            list.Add(xy[1]);
        }
        base.vertices = list.ToArray();
    }
}
}
