/**
 * TextBlock.cs
 *
 * ©2025 PDFjet Software
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFjet.NET {
    public class TextBlock {
        private float x;
        private float y;
        private float width;
        private float height;
        private Font font;
        private Font fallbackFont;
        private string textContent;
        private float lineSpacing = 1.0f;
        private int? textColor;
        private Dictionary<string, int> keywordHighlightColors;
        private float textPadding = 0.0f;
        private float[] fillColor;
        private float borderWidth = 0.0f;
        private float borderCornerRadius = 0.0f;
        private float[] borderColor;
        private string language = "en-US";
//        private string altDescription = "";
        private string uri;
//        private string key;
//        private string uriLanguage = "en-US";
//        private string uriActualText;
//        private string uriAltDescription;
        private Direction textDirection = Direction.LEFT_TO_RIGHT;
        private Alignment textAlignment = Alignment.LEFT;
//        private bool underline = false;
//        private bool strikeout = false;

        public TextBlock(Font font, string textContent) {
            this.x = 0.0f;
            this.y = 0.0f;
            this.width = 500.0f;
            this.height = 500.0f;
            this.font = font;
            this.textContent = textContent;
            this.textColor = Color.black;
            this.borderColor = new float[] {0f, 0f, 0f};    // Black color
        }

        public void SetFont(Font font) {
            this.font = font;
        }

        public void SetFallbackFont(Font font) {
            this.fallbackFont = font;
        }

        public void SetFontSize(float size) {
            this.font.SetSize(size);
        }

        public void SetFallbackFontSize(float size) {
            if (this.fallbackFont != null) {
                this.fallbackFont.SetSize(size);
            }
        }

        public void SetText(string text) {
            this.textContent = text;
        }

        public Font GetFont() {
            return this.font;
        }

        public string GetText() {
            return this.textContent;
        }

        public void SetLocation(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public void SetSize(float w, float h) {
            this.width = w;
            this.height = h;
        }

        public void SetWidth(float w) {
            this.width = w;
            this.height = 0.0f;
        }

        public void SetBorderCornerRadius(float borderCornerRadius) {
            this.borderCornerRadius = borderCornerRadius;
        }

        public void SetTextPadding(float padding) {
            this.textPadding = padding;
        }

        public void SetBorderWidth(float borderWidth) {
            this.borderWidth = borderWidth;
        }

        public void SetFillColor(int color) {
            float r = ((color >> 16) & 0xff)/255f;
            float g = ((color >>  8) & 0xff)/255f;
            float b = ((color)       & 0xff)/255f;
            SetFillColor(r, g, b);
        }

        public void SetFillColor(float r, float g, float b) {
            this.fillColor = new float[] {r, g, b};
        }

        public void SetBorderColor(int color) {
            float r = ((color >> 16) & 0xff)/255f;
            float g = ((color >>  8) & 0xff)/255f;
            float b = ((color)       & 0xff)/255f;
            SetBorderColor(r, g, b);
        }

        public void SetBorderColor(float r, float g, float b) {
            this.borderColor = new float[] {r, g, b};
        }

        public void SetLineSpacing(float textLineHeight) {
            this.lineSpacing = textLineHeight;
        }

        public void SetTextColor(int textColor) {
            this.textColor = textColor;
        }

        public void SetTextAlignment(Alignment textAlignment) {
            this.textAlignment = textAlignment;
        }

        public TextBlock SetURIAction(string uri) {
            this.uri = uri;
            return this;
        }

        public void SetTextDirection(Direction textDirection) {
            this.textDirection = textDirection;
        }

        public void SetKeywordHighlightColors(Dictionary<string, int> map) {
            this.keywordHighlightColors = new Dictionary<string, int>();
            foreach (var key in map.Keys) {
                this.keywordHighlightColors[key.ToLower()] = map[key];
            }
        }

        private bool TextIsCJK(string str) {
            int numOfCJK = 0;
            char[] chars = str.ToCharArray();
            foreach (char ch in chars) {
                if ((ch >= 0x4E00 && ch <= 0x9FD5) ||
                    (ch >= 0x3040 && ch <= 0x309F) ||
                    (ch >= 0x30A0 && ch <= 0x30FF) ||
                    (ch >= 0x1100 && ch <= 0x11FF)) {
                    numOfCJK++;
                }
            }
            return numOfCJK > (chars.Length / 2);
        }

        private TextLineWithOffset[] GetTextLinesWithOffsets() {
            List<TextLineWithOffset> textLines = new List<TextLineWithOffset>();

            float textAreaWidth = this.textDirection == Direction.LEFT_TO_RIGHT
                ? this.width - 2 * this.textPadding
                : this.height - 2 * this.textPadding;

            this.textContent = this.textContent.Replace("\r\n", "\n").Trim();
            string[] lines = this.textContent.Split('\n');

            foreach (string line in lines) {
                if (this.font.StringWidth(this.fallbackFont, line) <= textAreaWidth) {
                    textLines.Add(new TextLineWithOffset(line, 0f));
                } else {
                    if (TextIsCJK(line)) {
                        StringBuilder sb = new StringBuilder();
                        foreach (char ch in line.ToCharArray()) {
                            if (this.font.StringWidth(this.fallbackFont, sb.ToString() + ch) <= textAreaWidth) {
                                sb.Append(ch);
                            } else {
                                textLines.Add(new TextLineWithOffset(sb.ToString(), 0f));
                                sb.Clear();
                                sb.Append(ch);
                            }
                        }
                        if (sb.Length > 0) {
                            textLines.Add(new TextLineWithOffset(sb.ToString(), 0f));
                        }
                    } else {
                        StringBuilder sb = new StringBuilder();
                        string[] tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string token in tokens) {
                            if (this.font.StringWidth(this.fallbackFont, sb.ToString() + token) <= textAreaWidth) {
                                sb.Append(token).Append(" ");
                            } else {
                                textLines.Add(new TextLineWithOffset(sb.ToString().Trim(), 0f));
                                sb.Clear();
                                sb.Append(token).Append(" ");
                            }
                        }
                        if (sb.ToString().Trim().Length > 0) {
                            textLines.Add(new TextLineWithOffset(sb.ToString().Trim(), 0f));
                        }
                    }
                }
            }

            return textLines.ToArray();
        }

        private void RightAlignText(TextLineWithOffset[] textLines) {
            foreach (TextLineWithOffset textLineWithOffset in textLines) {
                textLineWithOffset.xOffset = this.width - font.StringWidth(textLineWithOffset.textLine);
            }
        }

        private void CenterText(TextLineWithOffset[] textLines) {
            foreach (TextLineWithOffset textLineWithOffset in textLines) {
                textLineWithOffset.xOffset = (this.width - font.StringWidth(textLineWithOffset.textLine)) / 2f;
            }
        }

        public float[] DrawOn(Page page) {
            if (page == null) {
                throw new ArgumentException("A valid Page object is required.");
            }

            page.SetPenWidth(this.borderWidth);

            float ascent = this.font.GetAscent();
            float descent = this.font.GetDescent();
            float leading = (ascent + descent) * this.lineSpacing;

            TextLineWithOffset[] textLines = GetTextLinesWithOffsets();
            if (textAlignment == Alignment.RIGHT) {
                RightAlignText(textLines);
            } else if (textAlignment == Alignment.CENTER) {
                CenterText(textLines);
            }

            page.AddBMC(StructElem.P, this.language, this.textContent, null);
            float textBlockHeight = page.DrawTextBlock(
                this.font,
                textLines,
                this.x + this.textPadding,
                this.y + this.textPadding,
                leading * this.lineSpacing,
                this.textDirection,
                this.textColor.Value,
                keywordHighlightColors);
            page.AddEMC();

            Rect rect = new Rect(this.x, this.y, this.width, textBlockHeight + 2 * this.textPadding);
            rect.SetFillColor(this.fillColor);
            rect.SetBorderColor(this.borderColor);
            rect.SetCornerRadius(this.borderCornerRadius);
            rect.DrawOn(page);

            return new float[] { this.x + this.width, this.y + textBlockHeight + 2 * this.textPadding };
        }
    }
}
