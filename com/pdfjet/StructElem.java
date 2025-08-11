/**
 *  StructElem.java
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
package com.pdfjet;

import java.util.ArrayList;
import java.util.List;

/**
 *  Defines the StructElem types.
 */
public class StructElem {
    // Document structure
    public static final String DOCUMENT = "Document";
    public static final String PART = "Part";
    public static final String DIV = "Div";
    public static final String SECT = "Sect";

    // Headings
    public static final String H1 = "H1";
    public static final String H2 = "H2";
    public static final String H3 = "H3";
    public static final String H4 = "H4";
    public static final String H5 = "H5";
    public static final String H6 = "H6";

    // Paragraphs and text
    public static final String P = "P";
    public static final String TITLE = "Title";
    public static final String LBL = "Lbl";

    // Inline text
    public static final String SPAN = "Span";
    public static final String EM = "Em";
    public static final String STRONG = "Strong";

    // Links and annotations
    public static final String LINK = "Link";
    public static final String ANNOT = "Annot";

    // Lists
    public static final String L = "L";   // List container
    public static final String LI = "LI"; // List item

    // Tables
    public static final String TABLE = "Table";
    public static final String TR = "TR";
    public static final String TH = "TH";
    public static final String TD = "TD";
    public static final String THEAD = "THead";
    public static final String TBODY = "TBody";
    public static final String TFOOT = "TFoot";
    public static final String CAPTION = "Caption";

    // Figures
    public static final String FIGURE = "Figure";
    public static final String ARTIFACT = "Artifact";

    protected int objNumber;
    protected String structure = null;
    protected int pageObjNumber;
    protected int mcid = 0;
    protected String language = null;
    protected String actualText = null;
    protected String altDescription = null;
    protected Annotation annotation = null;
    protected List<StructElem> kids = null;

    /** The default constructor */
    public StructElem() {
        this.kids = new ArrayList<StructElem>();
    }

    public void addKidStructElem(StructElem structElem) {
        this.kids.add(structElem);
    }
}
