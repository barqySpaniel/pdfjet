/**
 * ErrorCorrectLevel.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

/**
 * Used to specify the error correction level for QR Codes.
 */
namespace PDFjet.NET {
public class ErrorCorrectLevel {
    public const int L = 1;
    public const int M = 0;
    public const int Q = 3;
    public const int H = 2;
}
}   // End of namespace PDFjet.NET
