/**
 * Util.cs
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
using System;
using System.Text;

namespace PDFjet.NET {
public class Util {
    internal static string ToHexString(byte[] data) {
        var sb = new StringBuilder(data.Length * 2);
        foreach (byte b in data) {
            sb.AppendFormat("{0:x2}", b);
        }
        return sb.ToString();
    }
}
}   // End of namespace PDFjet.NET
