/**
 * DeflaterOutputStream.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System.IO;
using System.IO.Compression;

namespace PDFjet.NET {
public class DeflaterOutputStream {
    private readonly MemoryStream buf1 = null;
    private MemoryStream buf2 = null;
    private DeflateStream ds1 = null;
    private const uint prime = 65521;

    public DeflaterOutputStream(MemoryStream buf1) {
        this.buf1 = buf1;
        this.buf2 = new MemoryStream();
        this.buf2.WriteByte(0x58);   // These are the correct values for
        this.buf2.WriteByte(0x85);   // CMF and FLG according to Microsoft
        this.ds1 = new DeflateStream(buf2, CompressionMode.Compress, true);
    }

    public void Write(byte[] buffer, int off, int len) {
        // Compress the data in the buffer
        ds1.Write(buffer, off, len);
        buf2.WriteTo(buf1);

        // Calculate the Adler-32 checksum
        ulong s1 = 1L;
        ulong s2 = 0L;
        for (int i = 0; i < len; i++) {
            s1 = (s1 + buffer[off + i]) % prime;
            s2 = (s2 + s1) % prime;
        }
        appendAdler((s2 << 16) + s1);

        ds1.Dispose();
    }

    private void appendAdler(ulong adler) {
        buf1.WriteByte((byte) (adler >> 24));
        buf1.WriteByte((byte) (adler >> 16));
        buf1.WriteByte((byte) (adler >>  8));
        buf1.WriteByte((byte) (adler));
        buf1.Flush();
    }
}   // End of DeflaterOutputStream.cs
}   // End of package PDFjet.NET
