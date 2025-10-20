/**
 * TextAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class TextAnnotation : BaseAnnotation {
    public TextAnnotation() {
        base.annotationType = Annotation.Text;
    }

//    public void Rotate(double degrees) {
//        float[] center = container.GetRotationCenter();
//        base.point1 = Container.RotateAroundCenter(base.point1, center, degrees);
//        base.point2 = Container.RotateAroundCenter(base.point2, center, degrees);
//    }
}
}
