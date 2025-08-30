/**
 * Compressor.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System.IO;

namespace PDFjet.NET {
class Compressor {
    internal static byte[] deflate(byte[] data) {
        MemoryStream buf = new MemoryStream();
        DeflaterOutputStream dos = new DeflaterOutputStream(buf);
        dos.Write(data, 0, data.Length);
        return buf.ToArray();
    }
}   // End of Compressor.cs
}   // End of package PDFjet.NET
