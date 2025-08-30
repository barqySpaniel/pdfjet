/**
 * BitBuffer.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;

namespace PDFjet.NET {
/**
 * BitBuffer
 * @author Kazuhiko Arase
 */
public class BitBuffer {
    private byte[] buffer;
    private int length;
    private int increments = 32;

    public BitBuffer() {
        buffer = new byte[increments];
        length = 0;
    }

    public byte[] GetBuffer() {
        return buffer;
    }

    public int GetLengthInBits() {
        return length;
    }

    public void Put(int num, int length) {
        for (int i = 0; i < length; i++) {
            Put(((int) ((uint) num >> (length - i - 1)) & 1) == 1);
        }
    }

    public void Put(bool bit) {
        if (length == buffer.Length * 8) {
            byte[] newBuffer = new byte[buffer.Length + increments];
            Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
            buffer = newBuffer;
        }
        if (bit) {
            buffer[length / 8] |= (byte) ((uint) 0x80 >> (length % 8));
        }
        length++;
    }
}
}   // End of namespace PDFjet.NET
