/**
 *  EmbeddedFile.cs
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
using System;
using System.IO;

namespace PDFjet.NET {
/**
 *  Used to embed file objects.
 *  The file objects must added to the PDF before drawing on the first page.
 */
public class EmbeddedFile {
    internal int objNumber = -1;
    internal String fileName = null;

    public EmbeddedFile(PDF pdf, String fileName, Compress compress) :
        this(pdf, fileName.Substring(fileName.LastIndexOf("/") + 1),
                new BufferedStream(new FileStream(fileName, FileMode.Open, FileAccess.Read)), compress) {
    }

    public EmbeddedFile(PDF pdf, String fileName, Stream stream, Compress compress) {
        this.fileName = fileName;
        byte[] buf = Content.GetFromStream(stream);

        if (compress == Compress.YES) {
            MemoryStream baos = new MemoryStream();
            DeflaterOutputStream dos = new DeflaterOutputStream(baos);
            dos.Write(buf, 0, buf.Length);
            buf = baos.ToArray();
        }

        pdf.NewObj();
        pdf.Append(Token.BeginDictionary);
        pdf.Append("/Type /EmbeddedFile\n");
        if (compress == Compress.YES) {
            pdf.Append("/Filter /FlateDecode\n");
        }
        pdf.Append(Token.Length);
        pdf.Append(buf.Length);
        pdf.Append(Token.Newline);
        pdf.Append(Token.EndDictionary);
        pdf.Append(Token.Stream);
        pdf.Append(buf);
        pdf.Append(Token.EndStream);
        pdf.EndObj();

        pdf.NewObj();
        pdf.Append(Token.BeginDictionary);
        pdf.Append("/Type /Filespec\n");
        pdf.Append("/F (");
        pdf.Append(fileName);
        pdf.Append(")\n");
        pdf.Append("/EF <</F ");
        pdf.Append(pdf.GetObjNumber() - 1);
        pdf.Append(" 0 R>>\n");
        pdf.Append(Token.EndDictionary);
        pdf.EndObj();

        this.objNumber = pdf.GetObjNumber();
    }

    public String GetFileName() {
        return fileName;
    }
}   // End of EmbeddedFile.cs
}   // End of namespace PDFjet.NET
