/**
 * CircleAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class CircleAnnotation : BaseAnnotation {
    public CircleAnnotation() {
        base.annotationType = Annotation.Circle;
    }

    public void Rotate(double degrees) {
        float[] center = container.GetRotationCenter();
        float[] xy1 = Container.RotateAroundCenter(x1, y1, center, degrees);
        float[] xy2 = Container.RotateAroundCenter(x2, y2, center, degrees);
        this.x1 = xy1[0];
        this.y1 = xy1[1];
        this.x2 = xy2[0];
        this.y2 = xy2[1];
    }
}
}
