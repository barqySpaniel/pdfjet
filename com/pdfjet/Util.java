/**
 * Util.java
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
package com.pdfjet;

import java.nio.ByteBuffer;

public class Util {
    public static String toHexString(byte[] data) {
        StringBuilder sb = new StringBuilder(data.length * 2);
        for (byte b : data) {
            // & 0xFF makes the byte unsigned before formatting
            sb.append(String.format("%02x", b & 0xFF));
        }
        return sb.toString();
    }
}
