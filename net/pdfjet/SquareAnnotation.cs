/**
 * SquareAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class SquareAnnotation : BaseAnnotation {
    public SquareAnnotation() {
        base.annotationType = Annotation.Square;
    }

    public void Rotate(double degrees) {
        float[] center = container.GetRotationCenter();
        float[] point1 = Container.RotateAroundCenter(new float[] {x1, y1}, center, degrees);
        float[] point2 = Container.RotateAroundCenter(new float[] {x2, y2}, center, degrees);
        this.x1 = point1[0];
        this.y1 = point1[1];
        this.x2 = point2[0];
        this.y2 = point2[1];
    }
}
}
