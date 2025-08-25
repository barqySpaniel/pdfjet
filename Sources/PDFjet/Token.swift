/**
 *  Token.swift
 *
©2025 PDFjet Software

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

///
/// Please see PDF.swift
///
public class Token {
    // Fundamental structural tokens
    public static let space = " "  //: UInt8 = 32      // ASCII space
    public static let newline: UInt8 = 10    // ASCII LF

    public static let beginDictionary = [UInt8]("<<\n".utf8)
    public static let endDictionary = [UInt8](">>\n".utf8)
    public static let stream = [UInt8]("stream\n".utf8)
    public static let endStream = [UInt8]("\nendstream\n".utf8)

    // Object management tokens
    public static let newObj = [UInt8](" 0 obj\n".utf8)
    public static let endObj = [UInt8]("endobj\n".utf8)
    public static let objRef = " 0 R\n" // [UInt8](" 0 R\n".utf8)

    // Text and content tokens
    public static let beginText = [UInt8]("BT\n".utf8)
    public static let endText = [UInt8]("ET\n".utf8)

    // Essential property tokens (used everywhere)
    public static let length = [UInt8]("/Length ".utf8)
    public static let type = [UInt8]("/Type ".utf8)
    public static let resources = [UInt8]("/Resources ".utf8)
}
