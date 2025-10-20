/**
 * CircleAnnotation.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet;

public class CircleAnnotation extends BaseAnnotation {
    public CircleAnnotation() {
        super.annotationType = Annotation.Circle;
    }

    public void rotate(double degrees) {
        float[] center = container.getRotationCenter();
        super.point1 = Container.rotateAroundCenter(super.point1, center, degrees);
        super.point2 = Container.rotateAroundCenter(super.point2, center, degrees);
    }
}
