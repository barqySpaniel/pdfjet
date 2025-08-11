/**
 *  StructElem.swift
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

/**
 *  Defines the StructElem types.
 */
public class StructElem {
    // Defines standard PDF structure element tags
    // as defined in ISO 32000-1 (PDF 1.7) and PDF/UA specifications

    // Document-level structure elements
    public static let Document = "Document"
    public static let Part = "Part"
    public static let Div = "Div"
    public static let Sect = "Sect"
    
    // Heading elements
    public static let H1 = "H1"
    public static let H2 = "H2"
    public static let H3 = "H3"
    public static let H4 = "H4"
    public static let H5 = "H5"
    public static let H6 = "H6"
    
    // Paragraph and text elements
    public static let P = "P"
    public static let Title = "Title"
    public static let Lbl = "Lbl"
    
    // Inline text
    public static let Span = "Span"
    public static let Em = "Em"
    public static let Strong = "Strong"
    
    // Links and annotations
    public static let Link = "Link"
    public static let Annot = "Annot"
    
    // List elements
    public static let L = "L"   // List
    public static let LI = "LI" // List Item
    
    // Table elements
    public static let Table = "Table"
    public static let TR = "TR"     // Table Row
    public static let TH = "TH"     // Table Header
    public static let TD = "TD"     // Table Data
    public static let THead = "THead" // Table Header group
    public static let TBody = "TBody" // Table Body group
    public static let TFoot = "TFoot" // Table Footer group
    public static let Caption = "Caption"
    
    // Figure and special elements
    public static let Figure = "Figure"
    public static let Artifact = "Artifact"
    
    // Figure and special elements
    public static let figure = "Figure"
    public static let artifact = "Artifact"
    var objNumber: Int?
    var structure: String?
    var pageObjNumber: Int?
    var mcid = 0
    var language: String?
    var altDescription: String?
    var actualText: String?
    var annotation: Annotation?
}
