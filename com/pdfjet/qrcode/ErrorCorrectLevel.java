/**
 * ErrorCorrectLevel.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet.qrcode;

/**
 * Used to specify the error correction level for QR Codes.
 *
 * @author Kazuhiko Arase
 */
public class ErrorCorrectLevel {
    /** Default Constructor */
    public ErrorCorrectLevel() {
    }

    /** Low */
    public static final int L = 1;

    /** Medium */
    public static final int M = 0;

    /** Quartile */
    public static final int Q = 3;

    /** High */
    public static final int H = 2;
}
