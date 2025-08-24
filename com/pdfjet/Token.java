package com.pdfjet;

import java.nio.charset.StandardCharsets;

public class Token {
    // Fundamental structural tokens
    public static final byte SPACE = (byte) ' ';
    public static final byte NEWLINE = (byte) '\n';
    public static final byte[] BEGIN_DICTIONARY = "<<\n".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] END_DICTIONARY = ">>\n".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] STREAM = "stream\n".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] END_STREAM = "\nendstream\n".getBytes(StandardCharsets.US_ASCII);

    // Object management tokens
    public static final byte[] NEW_OBJ = " 0 obj\n".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] END_OBJ = "endobj\n".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] OBJ_REF = " 0 R\n".getBytes(StandardCharsets.US_ASCII);

    // Text and content tokens
    public static final byte[] BEGIN_TEXT = "BT\n".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] END_TEXT = "ET\n".getBytes(StandardCharsets.US_ASCII);

    // Essential property tokens (used everywhere)
    public static final byte[] LENGTH = "/Length ".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] TYPE = "/Type ".getBytes(StandardCharsets.US_ASCII);
    public static final byte[] RESOURCES = "/Resources ".getBytes(StandardCharsets.US_ASCII);
}
