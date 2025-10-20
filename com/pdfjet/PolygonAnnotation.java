/**
 * PolygonAnnotation.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet;

import java.util.*;

public class PolygonAnnotation extends BaseAnnotation {
    public PolygonAnnotation() {
        super.annotationType = Annotation.Polygon;
    }

    public void setVertices(float[] vertices) {
        super.vertices = vertices;
    }

    public void rotate(double degrees) {
        float[] center = container.getRotationCenter();
        super.point1 = Container.rotateAroundCenter(super.point1, center, degrees);
        super.point2 = Container.rotateAroundCenter(super.point2, center, degrees);

        for (int i = 0; i < vertices.length; i += 2) {
            float[] point = Container.rotateAroundCenter(
                new float[] {super.vertices[i], super.vertices[i + 1]}, new float[] {0f, 0f}, degrees);
            super.vertices[i] = point[0];
            super.vertices[i + 1] = point[1];
        }
    }
}
