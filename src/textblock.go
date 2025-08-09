package pdfjet

/**
 * textblock.go
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

import (
	"errors"
	"strings"

	"github.com/edragoev1/pdfjet/src/alignment"
	"github.com/edragoev1/pdfjet/src/color"
	"github.com/edragoev1/pdfjet/src/direction"
)

// TextBlock creates block of line-wrapped text.
type TextBlock struct {
	x                      float32
	y                      float32
	width                  float32
	height                 float32
	font                   *Font
	fallbackFont           *Font
	textContent            string
	lineSpacing            float32
	textColor              int32
	textPadding            float32
	borderWidth            float32
	borderCornerRadius     float32
	borderColor            int32
	language               string
	altDescription         string
	uri                    *string
	key                    *string
	uriLanguage            string
	uriActualText          string
	uriAltDescription      string
	textDirection          direction.Direction
	textAlignment          int
	underline              bool
	strikeout              bool
	keywordHighlightColors map[string]int32
}

// NewTextBlock creates a text block and sets the font.
//
//	@param font the font.
func NewTextBlock(font *Font, textContent string) *TextBlock {
	textBlock := new(TextBlock)
	textBlock.x = 0.0
	textBlock.y = 0.0
	textBlock.width = 500.0
	textBlock.height = 500.0
	textBlock.font = font

	textBlock.textContent = textContent
	textBlock.lineSpacing = 1.0
	textBlock.textColor = color.Black
	textBlock.textPadding = 0.0
	textBlock.textDirection = direction.LeftToRight
	textBlock.textAlignment = alignment.Left
	textBlock.keywordHighlightColors = make(map[string]int32)

	textBlock.borderWidth = 0.5
	textBlock.borderCornerRadius = 0.0
	textBlock.borderColor = color.Black

	textBlock.language = "en-US"
	textBlock.altDescription = ""
	textBlock.underline = false
	textBlock.strikeout = false

	return textBlock
}

// SetFont sets the font for textBlock text block.
//
// @param font the font.
func (textBlock *TextBlock) SetFont(font *Font) {
	textBlock.font = font
}

// SetFallbackFont sets the fallback font.
func (textBlock *TextBlock) SetFallbackFont(font *Font) {
	textBlock.fallbackFont = font
}

// SetFontSize sets the font size for the text block.
//
// @param size the font size.
func (textBlock *TextBlock) SetFontSize(size float32) {
	textBlock.font.SetSize(size)
}

// SetFontSize sets the font size for the text block.
//
// @param size the font size.
func (textBlock *TextBlock) SetFallbackFontSize(size float32) {
	textBlock.fallbackFont.SetSize(size)
}

// SetText sets the text block text.
// @param text the text block text.
func (textBlock *TextBlock) SetText(text string) {
	textBlock.textContent = text
}

// GetFont returns the font used by textBlock text block.
// @return the font.
func (textBlock *TextBlock) GetFont() *Font {
	return textBlock.font
}

// GetText returns the text block text.
// @return the text block text.
func (textBlock *TextBlock) GetText() string {
	return textBlock.textContent
}

// SetLocation sets the location where textBlock text block will be drawn on the page.
// @param x the x coordinate of the top left corner of the text block.
// @param y the y coordinate of the top left corner of the text block.
func (textBlock *TextBlock) SetLocation(x, y float32) {
	textBlock.x = x
	textBlock.y = y
}

// SetSize sets the size of the textBlock.
// @param w the width of the text block.
// @param h the height of the text block.
func (textBlock *TextBlock) SetSize(w, h float32) {
	textBlock.width = w
	textBlock.height = h
}

// SetWidth sets the width of the text block.
// The height is adjusted automatically to fit the text.
// @param w the width of the text block.
// @param h the height of the text block.
func (textBlock *TextBlock) SetWidth(w float32) {
	textBlock.width = w
	textBlock.height = 0.0
}

// SetBorderCornerRadius sets the border corner radius.
// @param borderRadius float the border corners radius.
func (textBlock *TextBlock) SetBorderCornerRadius(borderCornerRadius float32) {
	textBlock.borderCornerRadius = borderCornerRadius
}

// SetPadding sets the padding around the block of text.
// @param padding the padding between the text and the border.
func (textBlock *TextBlock) SetTextPadding(padding float32) {
	textBlock.textPadding = padding
}

// SetBorderWidth sets the border line width.
// @param lineWidth float
func (textBlock *TextBlock) SetBorderWidth(borderWidth float32) {
	textBlock.borderWidth = borderWidth
}

// Sets the pen color.
// @param color the color specified as 0xRRGGBB integer.
func (textBlock *TextBlock) SetBorderColor(borderColor int32) {
	textBlock.borderColor = borderColor
}

// SetLineHeight sets the extra leading between lines of text.
// @param lineHeight
func (textBlock *TextBlock) SetTextLineHeight(lineSpacing float32) {
	textBlock.lineSpacing = lineSpacing
}

// Sets the brush color.
// @param color the color specified as 0xRRGGBB integer.
func (textBlock *TextBlock) SetTextColor(textColor int32) {
	textBlock.textColor = textColor
}

// SetTextColors sets the text colors map.
func (textBlock *TextBlock) SetHighlightColors(keywordHighlightColors map[string]int32) {
	textBlock.keywordHighlightColors = keywordHighlightColors
}

// Sets the brush color.
// @param color the color specified as 0xRRGGBB integer.
func (textBlock *TextBlock) SetTextAlignment(textAlignment int) {
	textBlock.textAlignment = textAlignment
}

func (text *TextBlock) textIsCJK(str string) bool {
	// CJK Unified Ideographs Range: 4E00–9FD5
	// Hiragana Range: 3040–309F
	// Katakana Range: 30A0–30FF
	// Hangul Jamo Range: 1100–11FF
	numOfCJK := 0
	runes := []rune(str)
	for _, ch := range runes {
		if (ch >= 0x4E00 && ch <= 0x9FD5) ||
			(ch >= 0x3040 && ch <= 0x309F) ||
			(ch >= 0x30A0 && ch <= 0x30FF) ||
			(ch >= 0x1100 && ch <= 0x11FF) {
			numOfCJK++
		}
	}
	return numOfCJK > (len(runes) / 2)
}

func (textBlock *TextBlock) getTextLines() []string {
	list := make([]string, 0)

	var textAreaWidth float32
	if textBlock.textDirection == direction.LeftToRight {
		textAreaWidth = textBlock.width - 2*textBlock.textPadding
	} else {
		// When writting text vertically!
		textAreaWidth = textBlock.height - 2*textBlock.textPadding
	}
	textBlock.textContent = strings.ReplaceAll(textBlock.textContent, "\r\n", "\n")
	textBlock.textContent = strings.TrimRight(textBlock.textContent, "\n")
	lines := strings.Split(textBlock.textContent, "\n")
	for _, line := range lines {
		if textBlock.font.StringWidth(textBlock.fallbackFont, line) <= textAreaWidth {
			list = append(list, line)
		} else {
			if textBlock.textIsCJK(line) {
				var sb strings.Builder
				for _, ch := range line {
					if textBlock.font.StringWidth(
						textBlock.fallbackFont, sb.String()+string(ch)) <= textAreaWidth {
						sb.WriteRune(ch)
					} else {
						list = append(list, sb.String())
						sb.Reset()
						sb.WriteRune(ch)
					}
				}
				if sb.Len() > 0 {
					list = append(list, sb.String())
				}
			} else {
				var sb strings.Builder
				var tokens = strings.Fields(line)
				for _, token := range tokens {
					if textBlock.font.StringWidth(
						textBlock.fallbackFont, sb.String()+token) <= textAreaWidth {
						sb.WriteString(token + " ")
					} else {
						list = append(list, strings.TrimSpace(sb.String()))
						sb.Reset()
						sb.WriteString(token + " ")
					}
				}
				if len(strings.TrimSpace(sb.String())) > 0 {
					list = append(list, strings.TrimSpace(sb.String()))
				}
			}
		}
	}

	return list
}

// DrawOn draws text block on the specified page at specified location.
// @param page the Page where the TextBlock is to be drawn.
// @param draw flag specifying if text block component should actually be drawn on the page.
// @return x and y coordinates of the bottom right corner of text block component.
func (textBlock *TextBlock) DrawOnOld(page *Page) [2]float32 {
	if page != nil {
		// TODO: Deal with this now!!
	}

	page.SetBrushColor(textBlock.textColor)
	page.SetPenWidth(textBlock.font.underlineThickness)
	page.SetPenColor(textBlock.borderColor)

	ascent := textBlock.font.ascent
	descent := textBlock.font.descent
	leading := (ascent - descent) * textBlock.lineSpacing
	lines := textBlock.getTextLines()
	var xText float32 = 0.0
	var yText float32 = 0.0
	switch textBlock.textDirection {
	case direction.LeftToRight:
		yText = textBlock.y + ascent + textBlock.textPadding
		for _, line := range lines {
			switch textBlock.textAlignment {
			case alignment.Left:
				xText = textBlock.x + textBlock.textPadding
			case alignment.Right:
				xText = (textBlock.x + textBlock.width) -
					(textBlock.font.StringWidth(textBlock.fallbackFont, line) + textBlock.textPadding)
			case alignment.Center:
				xText = textBlock.x + (textBlock.width-textBlock.font.StringWidth(textBlock.fallbackFont, line))/2
			}
			textBlock.drawTextLine(
				page,
				textBlock.font,
				textBlock.fallbackFont,
				line,
				xText,
				yText,
				textBlock.textColor,
				textBlock.keywordHighlightColors)
			yText += leading
		}
	case direction.BottomToTop:
		xText = textBlock.x + textBlock.textPadding + ascent
		yText = textBlock.y + textBlock.height - textBlock.textPadding
		for _, line := range lines {
			textBlock.drawTextLine(
				page,
				textBlock.font,
				textBlock.fallbackFont,
				line,
				xText,
				yText,
				textBlock.textColor,
				textBlock.keywordHighlightColors)
			xText += leading
		}
	}

	xText -= leading
	if (xText-descent+textBlock.textPadding)-textBlock.x > textBlock.width {
		textBlock.width = (xText - descent + textBlock.textPadding) - textBlock.x
	}
	yText -= leading
	if (yText-descent+textBlock.textPadding)-textBlock.y > textBlock.height {
		textBlock.height = (yText - descent + textBlock.textPadding) - textBlock.y
	}
	rect := NewRectAt(textBlock.x, textBlock.y, textBlock.width, textBlock.height)
	rect.SetBorderColor(textBlock.borderColor)
	rect.SetCornerRadius(textBlock.borderCornerRadius)
	rect.DrawOn(page)

	if textBlock.textDirection == direction.LeftToRight &&
		(textBlock.uri != nil || textBlock.key != nil) {
		page.AddAnnotation(NewAnnotation(
			textBlock.uri,
			textBlock.key, // The destination name
			textBlock.x,
			textBlock.y,
			textBlock.x+textBlock.width,
			textBlock.y+textBlock.height,
			textBlock.uriLanguage,
			textBlock.uriActualText,
			textBlock.uriAltDescription))
	}
	page.SetTextDirection(0)

	return [2]float32{textBlock.x + textBlock.width, textBlock.y + textBlock.height}
}

// DrawTextLine draws text line on the page.
func (textBlock *TextBlock) drawTextLine(
	page *Page,
	font *Font,
	fallbackFont *Font,
	text string,
	xText float32,
	yText float32,
	brush int32,
	colors map[string]int32) {
	page.AddBMC("P", textBlock.language, text, textBlock.altDescription)
	if textBlock.textDirection == direction.BottomToTop {
		page.SetTextDirection(90)
	}
	page.DrawStringUsingColorMap(font, fallbackFont, text, xText, yText, brush, colors)
	page.AddEMC()
	if textBlock.textDirection == direction.LeftToRight {
		lineLength := textBlock.font.StringWidth(fallbackFont, text)
		if textBlock.underline {
			page.AddArtifactBMC()
			page.MoveTo(xText, yText+font.underlinePosition)
			page.LineTo(xText+lineLength, yText+font.underlinePosition)
			page.StrokePath()
			page.AddEMC()
		}
		if textBlock.strikeout {
			page.AddArtifactBMC()
			page.MoveTo(xText, yText-(font.bodyHeight/4))
			page.LineTo(xText+lineLength, yText-(font.bodyHeight/4))
			page.StrokePath()
			page.AddEMC()
		}
	}
}

func (textBlock *TextBlock) SetURIAction(uri string) *TextBlock {
	textBlock.uri = &uri
	return textBlock
}

func (textBlock *TextBlock) SetTextDirection(textDirection direction.Direction) {
	textBlock.textDirection = textDirection
}

func (textBlock *TextBlock) SetKeywordHighlightColors(keywordHighlightColors map[string]int32) {
	textBlock.keywordHighlightColors = keywordHighlightColors
}

func (t *TextBlock) getTextLinesWithOffsets() []TextLineWithOffset {
	var textLines []TextLineWithOffset

	var textAreaWidth float32
	if t.textDirection == direction.LeftToRight {
		textAreaWidth = t.width - 2*t.textPadding
	} else {
		// When writing text vertically!
		textAreaWidth = t.height - 2*t.textPadding
	}

	t.textContent = strings.ReplaceAll(t.textContent, "\r\n", "\n")
	t.textContent = strings.TrimSpace(t.textContent)
	lines := strings.Split(t.textContent, "\n")

	for _, line := range lines {
		if t.font.StringWidth(t.fallbackFont, line) <= textAreaWidth {
			textLines = append(
				textLines,
				TextLineWithOffset{textLine: line, xOffset: 0})
		} else {
			if t.textIsCJK(line) {
				var sb strings.Builder
				for _, ch := range line {
					if t.font.StringWidth(t.fallbackFont, sb.String()+string(ch)) <= textAreaWidth {
						sb.WriteRune(ch)
					} else {
						textLines = append(
							textLines,
							TextLineWithOffset{textLine: sb.String(), xOffset: 0})
						sb.Reset()
						sb.WriteRune(ch)
					}
				}
				if sb.Len() > 0 {
					textLines = append(
						textLines,
						TextLineWithOffset{textLine: sb.String(), xOffset: 0})
				}
			} else {
				var sb strings.Builder
				tokens := strings.Fields(line) // Split by whitespace
				for _, token := range tokens {
					if t.font.StringWidth(t.fallbackFont, sb.String()+token) <= textAreaWidth {
						sb.WriteString(token + " ")
					} else {
						textLines = append(
							textLines,
							TextLineWithOffset{textLine: strings.TrimSpace(sb.String()), xOffset: 0})
						sb.Reset()
						sb.WriteString(token + " ")
					}
				}
				if sb.Len() > 0 {
					textLines = append(
						textLines,
						TextLineWithOffset{textLine: strings.TrimSpace(sb.String()), xOffset: 0})
				}
			}
		}
	}

	return textLines
}

func (t *TextBlock) rightAlignText(textLines []TextLineWithOffset) {
	for _, textLineWithOffset := range textLines {
		textLineWithOffset.xOffset = t.width - t.font.stringWidth(textLineWithOffset.textLine)
	}
}

func (t *TextBlock) centerText(textLines []TextLineWithOffset) {
	for _, textLineWithOffset := range textLines {
		textLineWithOffset.xOffset = (t.width - t.font.stringWidth(textLineWithOffset.textLine)) / 2.0
	}
}

func (t *TextBlock) DrawOn(page *Page) ([]float32, error) {
	if page == nil {
		return nil, errors.New("a valid Page object is required")
	}

	page.SetPenWidth(t.borderWidth)

	ascent := t.font.ascent
	descent := t.font.descent
	leading := (ascent + descent) * t.lineSpacing

	textLines := t.getTextLinesWithOffsets()

	switch t.textAlignment {
	case alignment.Right:
		t.rightAlignText(textLines)
	case alignment.Center:
		t.centerText(textLines)
	}

	page.AddBMC("P", t.uriLanguage, t.textContent, "")
	textBlockHeight := page.drawTextBlock(
		t.font,
		textLines,
		t.x+t.textPadding,
		t.y+t.textPadding,
		leading*t.lineSpacing,
		t.textDirection,
		t.textColor,
		t.keywordHighlightColors)
	page.AddEMC()

	page.AddArtifactBMC()
	rect := NewRectAt(t.x, t.y, t.width, textBlockHeight+2*t.textPadding)
	rect.SetBorderColor(t.borderColor)
	rect.SetCornerRadius(t.borderCornerRadius)
	rect.DrawOn(page)
	page.AddEMC()

	// You can uncomment and adapt if required.
	/*
		if t.TextDirection == LEFT_TO_RIGHT &&
			(t.Uri != "" || t.Key != "") {
			page.AddAnnotation(Annotation{
				Uri:                 t.Uri,
				Key:                 t.Key,
				X:                   t.X,
				Y:                   t.Y,
				Width:               t.X + t.Width,
				Height:              t.Y + t.Height,
				UriLanguage:         t.UriLanguage,
				UriActualText:       t.UriActualText,
				UriAltDescription:   t.UriAltDescription,
			})
		}
	*/

	return []float32{t.x + t.width, t.y + textBlockHeight + 2*t.textPadding}, nil
}
