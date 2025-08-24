using System;
using System.Text;

namespace PDFjet.NET {
public class Token {
    // Fundamental structural tokens
    public static readonly byte Space = (byte) ' ';
    public static readonly byte Newline = (byte) '\n';
    public static readonly byte[] BeginDictionary = Encoding.ASCII.GetBytes("<<\n");
    public static readonly byte[] EndDictionary = Encoding.ASCII.GetBytes(">>\n");
    public static readonly byte[] Stream = Encoding.ASCII.GetBytes("stream\n");
    public static readonly byte[] EndStream = Encoding.ASCII.GetBytes("\nendstream\n");

    // Object management tokens
    public static readonly byte[] NewObj = Encoding.ASCII.GetBytes(" 0 obj\n");
    public static readonly byte[] EndObj = Encoding.ASCII.GetBytes("endobj\n");
    public static readonly byte[] ObjRef = Encoding.ASCII.GetBytes(" 0 R\n");

    // Text and content tokens
    public static readonly byte[] BeginText = Encoding.ASCII.GetBytes("BT\n");
    public static readonly byte[] EndText = Encoding.ASCII.GetBytes("ET\n");

    // Essential property tokens (used everywhere)
    public static readonly byte[] Length = Encoding.ASCII.GetBytes("/Length ");
    public static readonly byte[] Type = Encoding.ASCII.GetBytes("/Type ");
    public static readonly byte[] Resources = Encoding.ASCII.GetBytes("/Resources ");
}
}