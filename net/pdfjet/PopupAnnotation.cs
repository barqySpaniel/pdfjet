/**
 * PopupAnnotation.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
public class PopupAnnotation : BaseAnnotation {
    public PopupAnnotation() {
        base.annotationType = Annotation.Popup;
    }
}
}
