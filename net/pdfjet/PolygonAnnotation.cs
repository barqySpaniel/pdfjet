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
        base.point1 = Container.RotateAroundCenter(base.point1, center, degrees);
        base.point2 = Container.RotateAroundCenter(base.point2, center, degrees);

        List<float> list = new List<float>();
        for (int i = 0; i < vertices.Length; i += 2) {
            float[] point = Container.RotateAroundCenter(
                new float[] {base.vertices[i], base.vertices[i + 1]}, new float[] {0f, 0f}, degrees);
            list.Add(point[0]);
            list.Add(point[1]);
        }
        base.vertices = list.ToArray();
    }
}
}
