package pdfjet

/**
 * page.go
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
	"fmt"
	"log"
	"math"
	"strconv"
	"strings"
	"unicode"

	"github.com/edragoev1/pdfjet/src/color"
	"github.com/edragoev1/pdfjet/src/compliance"
	"github.com/edragoev1/pdfjet/src/corefont"
	"github.com/edragoev1/pdfjet/src/direction"
	"github.com/edragoev1/pdfjet/src/fastfloat"
	"github.com/edragoev1/pdfjet/src/operation"
	"github.com/edragoev1/pdfjet/src/shape"
	"github.com/edragoev1/pdfjet/src/token"
)

// Page is used to create PDF page objects.
//
// Please note:
// <pre>
//
//	The coordinate (0.0, 0.0) is the top left corner of the page.
//	The size of the pages are represented in points.
//	1 point is 1/72 inches.
//
// </pre>
type Page struct {
	pdf           *PDF
	buf           []byte
	pageObj       *PDFobj
	objNumber     int
	tmx           [4]float32
	tm0           []byte
	tm1           []byte
	tm2           []byte
	tm3           []byte
	renderingMode int
	width         float32
	height        float32
	contents      []int
	annots        []*Annotation
	destinations  []*Destination
	cropBox       []float32
	bleedBox      []float32
	trimBox       []float32
	artBox        []float32
	structures    []*StructElem
	penColor      [3]float32
	brushColor    [3]float32
	penCMYK       [4]float32
	brushCMYK     [4]float32
	penWidth      float32
	lineCapStyle  int
	lineJoinStyle int
	linePattern   string
	font          *Font
	savedStates   []*State
	mcid          int
	savedHeight   float32
}

// Constants from Android's Matrix object:
const (
	mScaleX = iota
	mSkewX
	mTransX
	mScaleY
	mSkewY
	mTransY
)

/**
 *  Creates page object and add it to the PDF document.
 *
 *  Please note:
 *  <pre>
 *  The coordinate (0.0, 0.0) is the top left corner of the page.
 *  The size of the pages are represented in points.
 *  1 point is 1/72 inches.
 *  </pre>
 *
 *  @param pdf the pdf object.
 *  @param pageSize the page size of this page.
 */
func NewPage(pdf *PDF, pageSize [2]float32) *Page {
	return newPage(pdf, pageSize, true)
}

func NewPageDetached(pdf *PDF, pageSize [2]float32) *Page {
	return newPage(pdf, pageSize, false)
}

// NewPage constructs page object and adds it to the PDF document.
//
// Please note:
// <pre>
//
//	The coordinate (0.0, 0.0) is the top left corner of the page.
//	The size of the pages are represented in points.
//	1 point is 1/72 inches.
//
// </pre>
//
// @param pdf the pdf object.
// @param pageSize the page size of this page.
// @param addPageToPDF boolean flag.
func newPage(pdf *PDF, pageSize [2]float32, addToPDF bool) *Page {
	page := new(Page)
	page.pdf = pdf
	page.contents = []int{}
	page.width = pageSize[0]
	page.height = pageSize[1]
	page.linePattern = "[] 0"
	page.savedHeight = math.MaxFloat32
	page.penWidth = 0
	page.tmx = [4]float32{1.0, 0.0, 0.0, 1.0}
	page.tm0 = fastfloat.ToByteArray(page.tmx[0])
	page.tm1 = fastfloat.ToByteArray(page.tmx[1])
	page.tm2 = fastfloat.ToByteArray(page.tmx[2])
	page.tm3 = fastfloat.ToByteArray(page.tmx[3])
	if addToPDF {
		pdf.AddPage(page)
	}
	return page
}

// NewPageFromObject creates page object from PDFobj.
func NewPageFromObject(pdf *PDF, pageObj *PDFobj) *Page {
	page := new(Page)
	page.pdf = pdf
	page.pageObj = page.removeComments(pageObj)
	page.width = pageObj.GetPageSize()[0]
	page.height = pageObj.GetPageSize()[1]
	page.tmx = [4]float32{1.0, 0.0, 0.0, 1.0}
	page.tm0 = fastfloat.ToByteArray(page.tmx[0])
	page.tm1 = fastfloat.ToByteArray(page.tmx[1])
	page.tm2 = fastfloat.ToByteArray(page.tmx[2])
	page.tm3 = fastfloat.ToByteArray(page.tmx[3])
	page.appendString("q\n")
	if pageObj.gsNumber != 0 {
		page.appendString("/GS")
		page.appendInteger(pageObj.gsNumber + 1)
		page.appendString(" gs\n")
	}
	return page
}

// removeComments removes object dictionary comments.
func (page *Page) removeComments(obj *PDFobj) *PDFobj {
	list := make([]string, 0)
	comment := false
	for _, token := range obj.dict {
		if token == "%" {
			comment = true
		} else {
			if strings.HasPrefix(token, "/") {
				comment = false
				list = append(list, token)
			} else {
				if !comment {
					list = append(list, token)
				}
			}
		}
	}
	obj.dict = list
	return obj
}

// AddCoreFontResource adds core font to the PDF objects.
func (page *Page) AddCoreFontResource(coreFont *corefont.CoreFont, objects *[]*PDFobj) *Font {
	return page.pageObj.addCoreFontResource(coreFont, objects)
}

// AddImageResource adds an image to the PDF objects.
func (page *Page) AddImageResource(image *Image, objects *[]*PDFobj) {
	page.pageObj.AddImageResource(image, objects)
}

// AddFontResource adds font to the PDF objects.
func (page *Page) AddFontResource(font *Font, objects *[]*PDFobj) {
	page.pageObj.AddFontResource(font, objects)
}

// Complete completes adding content to the existing PDF.
func (page *Page) Complete(objects *[]*PDFobj) {
	page.appendString("Q\n")
	page.pageObj.addContent(page.getContent(), objects)
}

func (page *Page) getContent() []byte {
	return page.buf
}

// AddDestination adds destination to this page.
// @param name The destination name.
// @param xPosition The horizontal position of the destination on this page.
// @param yPosition The vertical position of the destination on this page.
func (page *Page) AddDestination(name *string, xPosition, yPosition float32) *Destination {
	dest := NewDestination(name, xPosition, page.height-yPosition)
	page.destinations = append(page.destinations, dest)
	return dest
}

// GetWidth returns the width of this page.
func (page *Page) GetWidth() float32 {
	return page.width
}

// GetHeight returns the height of this page.
func (page *Page) GetHeight() float32 {
	return page.height
}

// DrawLine draws a line on the page, using the current color, between the points (x1, y1) and (x2, y2).
func (page *Page) DrawLine(x1, y1, x2, y2 float32) {
	page.MoveTo(x1, y1)
	page.LineTo(x2, y2)
	page.StrokePath()
}

// DrawString draws a string using the specified font1 and font2 at the x, y location.
func (page *Page) DrawString(font1 *Font, font2 *Font, text string, x, y float32) {
	page.DrawStringUsingColorMap(font1, font2, text, x, y, color.Black, nil)
}

// DrawStringUsingColorMap draws the text given by the specified string,
// using the specified main font and the current brushColor color.
// If the main font is missing some glyphs - the fallback font is used.
// The baseline of the leftmost character is at position (x, y) on the page.
func (page *Page) DrawStringUsingColorMap(
	font, fallbackFont *Font, text string, x, y float32, brush int32, colors map[string]int32) {
	if font.isCoreFont || font.isCJK || fallbackFont == nil || fallbackFont.isCoreFont || fallbackFont.isCJK {
		page.drawString(font, text, x, y, brush, colors)
	} else {
		activeFont := font
		var buf strings.Builder
		for _, ch := range text {
			if activeFont.unicodeToGID[ch] == 0 {
				page.drawString(activeFont, buf.String(), x, y, brush, colors)
				x += activeFont.stringWidth(buf.String())
				buf.Reset()
				// Switch the active font
				if activeFont == font {
					activeFont = fallbackFont
				} else {
					activeFont = font
				}
			}
			buf.WriteRune(ch)
		}
		page.drawString(activeFont, buf.String(), x, y, brush, colors)
	}
}

// drawString draws the text given by the specified string,
// using the specified font and the current brushColor color.
// The baseline of the leftmost character is at position (x, y) on the page.
//
// @param font the font to use.
// @param str the string to be drawn.
// @param x the x coordinate.
// @param y the y coordinate.
func (page *Page) drawString(font *Font, str string, x, y float32, brush int32, colors map[string]int32) {
	if str == "" {
		return
	}
	page.appendString("BT\n")
	page.SetTextFont(font)

	if page.renderingMode != 0 {
		page.appendInteger(page.renderingMode)
		page.appendString(" Tr\n")
	}

	if font.skew15 &&
		page.tmx[0] == 1.0 &&
		page.tmx[1] == 0.0 &&
		page.tmx[2] == 0.0 &&
		page.tmx[3] == 1.0 {
		var skew float32 = 0.26
		page.appendFloat32(page.tmx[0])
		page.appendString(" ")
		page.appendFloat32(page.tmx[1])
		page.appendString(" ")
		page.appendFloat32(page.tmx[2] + skew)
		page.appendString(" ")
		page.appendFloat32(page.tmx[3])
	} else {
		page.appendByteArray(page.tm0)
		page.appendString(" ")
		page.appendByteArray(page.tm1)
		page.appendString(" ")
		page.appendByteArray(page.tm2)
		page.appendString(" ")
		page.appendByteArray(page.tm3)
	}
	page.appendString(" ")
	page.appendFloat32(x)
	page.appendString(" ")
	page.appendFloat32(page.height - y)
	page.appendString(" Tm\n")

	if colors == nil {
		page.SetBrushColor(brush)
		if font.isCoreFont {
			page.appendString("[<")
			page.drawASCIIString(font, str)
			page.appendString(">] TJ\n")
		} else {
			page.appendString("<")
			page.drawUnicodeString(font, str)
			page.appendString("> Tj\n")
		}
	} else {
		page.drawColoredString(font, str, brush, colors)
	}
	page.appendString("ET\n")
}

func (page *Page) drawASCIIString(font *Font, text string) {
	runes := []rune(text)
	for i, c1 := range runes {
		if c1 < font.firstChar || c1 > font.lastChar {
			page.appendString(fmt.Sprintf("%02X", 0x20))
			continue
		}
		page.appendString(fmt.Sprintf("%02X", c1))
		if font.isCoreFont && font.kernPairs && i < (len(runes)-1) {
			c1 -= 32
			c2 := runes[i+1]
			if c2 < font.firstChar || c2 > font.lastChar {
				c2 = 32
			}
			for i := 2; i < len(font.metrics[c1]); i += 2 {
				if font.metrics[c1][i] == int(c2) {
					page.appendString(">")
					page.appendInteger(-page.font.metrics[c1][i+1])
					page.appendString("<")
					break
				}
			}
		}
	}
}

func (page *Page) drawTextBlock(
	font *Font,
	textLines []TextLineWithOffset,
	x float32,
	y float32,
	leading float32,
	direction direction.Direction,
	textColor int32,
	highlightColors map[string]int32) float32 {

	if len(textLines) == 0 {
		return float32(len(textLines)) * leading
	}

	page.appendString("BT\n")
	page.SetTextFont(font)

	xText := x
	yText := y
	for _, textLine := range textLines {
		if direction == LeftToRight {
			page.appendString("1 0 0 1 ")
			page.appendFloat32(xText + textLine.xOffset)
			page.appendString(" ")
			page.appendFloat32(page.height - (yText + font.ascent))
			page.appendString(" Tm\n")
		} else { // BOTTOM_TO_TOP
			page.appendString("0 1 -1 0 ")
			page.appendFloat32(xText + font.ascent)
			page.appendString(" ")
			page.appendFloat32(yText)
			page.appendString(" Tm\n")
		}

		if highlightColors == nil {
			page.SetBrushColor(textColor)
			page.appendString("<")
			page.drawUnicodeString(page.font, textLine.textLine)
			page.appendString("> Tj\n")
		} else {
			page.drawColoredString(page.font, textLine.textLine, textColor, highlightColors)
		}

		if direction == LeftToRight {
			yText += leading
		} else {
			xText += leading
		}
	}

	page.appendString("ET\n")

	return float32(len(textLines)) * leading
}

func (page *Page) drawUnicodeString(font *Font, text string) {
	runes := []rune(text)
	if font.isCJK {
		for _, c1 := range runes {
			if c1 != 0xFEFF { // BOM marker
				if c1 < font.firstChar || c1 > font.lastChar {
					// page.appendString(fmt.Sprintf("%04X", 0x0020))
					page.appendCodePointAsHex(0x0020)
				} else {
					// page.appendString(fmt.Sprintf("%04X", c1))
					page.appendCodePointAsHex(int(c1))
				}
			}
		}
	} else {
		for _, c1 := range runes {
			if c1 != 0xFEFF { // BOM marker
				if c1 < font.firstChar || c1 > font.lastChar {
					// page.appendString(fmt.Sprintf("%04X", font.unicodeToGID[0x0020]))
					page.appendCodePointAsHex(0x0020)
				} else {
					// page.appendString(fmt.Sprintf("%04X", font.unicodeToGID[c1]))
					page.appendCodePointAsHex(font.unicodeToGID[c1])
				}
			}
		}
	}
}

func (page *Page) appendCodePointAsHex(codePoint int) {
	if codePoint <= 0xFFFF {
		page.buf = append(page.buf,
			hexDigits[(codePoint>>12)&0xF],
			hexDigits[(codePoint>>8)&0xF],
			hexDigits[(codePoint>>4)&0xF],
			hexDigits[codePoint&0xF],
		)
	} else {
		page.buf = append(page.buf,
			hexDigits[(codePoint>>20)&0xF],
			hexDigits[(codePoint>>16)&0xF],
			hexDigits[(codePoint>>12)&0xF],
			hexDigits[(codePoint>>8)&0xF],
			hexDigits[(codePoint>>4)&0xF],
			hexDigits[codePoint&0xF],
		)
	}
}

// SetGraphicsState sets the graphics state. Please see Example_31.
// @param gs the graphics state to use.
func (page *Page) SetGraphicsState(gs *GraphicsState) {
	var buf strings.Builder
	buf.WriteString("/CA ")
	buf.WriteString(fmt.Sprintf("%.2f", gs.GetAlphaStroking()))
	buf.WriteString(" ")
	buf.WriteString("/ca ")
	buf.WriteString(fmt.Sprintf("%.2f", gs.GetAlphaNonStroking()))
	state := buf.String()
	n, ok := page.pdf.states[state]
	if !ok {
		n = len(page.pdf.states) + 1
		page.pdf.states[state] = n
	}
	page.appendString("/GS")
	page.appendInteger(n)
	page.appendString(" gs\n")
}

// SetPenColorRGB sets the color for stroking operations.
// The penColor color is used when drawing lines and splines.
//
// @param r the red component is float value from 0.0 to 1.0.
// @param g the green component is float value from 0.0 to 1.0.
// @param b the blue component is float value from 0.0 to 1.0.
func (page *Page) SetPenColorRGB(r, g, b float32) {
	page.appendFloat32(r)
	page.appendString(" ")
	page.appendFloat32(g)
	page.appendString(" ")
	page.appendFloat32(b)
	page.appendString(" RG\n")
}

func (page *Page) SetPenColorWithFloat32Array(rgbColor [3]float32) {
	// Your exact code for range check
	if rgbColor[0] < 0.0 || rgbColor[0] > 1.0 ||
		rgbColor[1] < 0.0 || rgbColor[1] > 1.0 ||
		rgbColor[2] < 0.0 || rgbColor[2] > 1.0 {
		fmt.Println("Warning: RGB color values must be between 0f and 1f. Ignoring request.")
		return // Early exit if out of range
	}

	// Now set the penColor color
	page.penColor = rgbColor

	// Proceed with setting the color (example)
	page.appendFloat32(rgbColor[0])
	page.appendString(" ")
	page.appendFloat32(rgbColor[1])
	page.appendString(" ")
	page.appendFloat32(rgbColor[2])
	page.appendString(" RG\n")
}

// SetPenColorCMYK sets the color for stroking operations using CMYK.
// The penColor color is used when drawing lines and splines.
//
// @param c the cyan component is float value from 0.0 to 1.0.
// @param m the magenta component is float value from 0.0 to 1.0.
// @param y the yellow component is float value from 0.0 to 1.0.
// @param k the black component is float value from 0.0 to 1.0.
func (page *Page) SetPenColorCMYK(c, m, y, k float32) {
	page.appendFloat32(c)
	page.appendString(" ")
	page.appendFloat32(m)
	page.appendString(" ")
	page.appendFloat32(y)
	page.appendString(" ")
	page.appendFloat32(k)
	page.appendString(" K\n")
}

// SetBrushColorCMYK sets the color for brushColor operations using CMYK.
// This is the color used when drawing regular text and filling shapes.
// @param c the cyan component is float value from 0.0 to 1.0.
// @param m the magenta component is float value from 0.0 to 1.0.
// @param y the yellow component is float value from 0.0 to 1.0.
// @param k the black component is float value from 0.0 to 1.0.
func (page *Page) SetBrushColorCMYK(c, m, y, k float32) {
	page.appendFloat32(c)
	page.appendString(" ")
	page.appendFloat32(m)
	page.appendString(" ")
	page.appendFloat32(y)
	page.appendString(" ")
	page.appendFloat32(k)
	page.appendString(" k\n")
}

func (page *Page) SetBrushColorWithFloat32Array(rgbColor [3]float32) {
	// Your exact code for range check
	if rgbColor[0] < 0.0 || rgbColor[0] > 1.0 ||
		rgbColor[1] < 0.0 || rgbColor[1] > 1.0 ||
		rgbColor[2] < 0.0 || rgbColor[2] > 1.0 {
		fmt.Println("Warning: RGB color values must be between 0f and 1f. Ignoring request.")
		return // Early exit if out of range
	}

	// Now set the brush color
	page.brushColor = rgbColor

	// Proceed with setting the color (example)
	page.appendFloat32(rgbColor[0])
	page.appendString(" ")
	page.appendFloat32(rgbColor[1])
	page.appendString(" ")
	page.appendFloat32(rgbColor[2])
	page.appendString(" rg\n")
}

// SetBrushColorRGB sets the color for brushColor operations.
// This is the color used when drawing regular text and filling shapes.
// @param r the red component is float value from 0.0 to 1.0.
// @param g the green component is float value from 0.0 to 1.0.
// @param b the blue component is float value from 0.0 to 1.0.
func (page *Page) SetBrushColorRGB(r, g, b float32) {
	page.appendFloat32(r)
	page.appendString(" ")
	page.appendFloat32(g)
	page.appendString(" ")
	page.appendFloat32(b)
	page.appendString(" rg\n")
}

// SetPenColor sets the penColor color.
// See the Color class for predefined values or define your own using 0x00RRGGBB packed integers.
func (page *Page) SetPenColor(color int32) {
	r := float32(((color >> 16) & 0xff)) / 255.0
	g := float32(((color >> 8) & 0xff)) / 255.0
	b := float32(((color) & 0xff)) / 255.0
	page.SetPenColorRGB(r, g, b)
}

// SetBrushColor sets the brushColor color.
// See the Color class for predefined values or define your own using 0x00RRGGBB packed integers.
func (page *Page) SetBrushColor(color int32) {
	r := float32(((color >> 16) & 0xff)) / 255.0
	g := float32(((color >> 8) & 0xff)) / 255.0
	b := float32(((color) & 0xff)) / 255.0
	page.SetBrushColorRGB(r, g, b)
}

func (page *Page) GetPenColor() [3]float32 {
	return page.penColor
}

// GetBrushColor returns the brushColor color.
// @return the brushColor color.
func (page *Page) GetBrushColor() [3]float32 {
	return page.brushColor
}

// SetDefaultLineWidth sets the line width to the default.
// The default is the finest line width.
func (page *Page) SetDefaultLineWidth() {
	page.appendFloat32(0.0)
	page.appendString(" w\n")
}

// SetLinePattern the line dash pattern controls the pattern of dashes and gaps used to stroke paths.
// It is specified by a dash array and a dash phase.
// The elements of the dash array are positive numbers that specify the lengths of
// alternating dashes and gaps.
// The dash phase specifies the distance into the dash pattern at which to start the dash.
// The elements of both the dash array and the dash phase are expressed in user space units.
// <pre>
// Examples of line dash patterns:
//
//	"[Array] Phase"     Appearance          Description
//	 _______________     _________________   ____________________________________
//
//	 "[] 0"              -----------------   Solid line
//	 "[3] 0"             ---   ---   ---     3 units on, 3 units off, ...
//	 "[2] 1"             -  --  --  --  --   1 on, 2 off, 2 on, 2 off, ...
//	 "[2 1] 0"           -- -- -- -- -- --   2 on, 1 off, 2 on, 1 off, ...
//	 "[3 5] 6"             ---     ---       2 off, 3 on, 5 off, 3 on, 5 off, ...
//	 "[2 3] 11"          -   --   --   --    1 on, 3 off, 2 on, 3 off, 2 on, ...
//
// </pre>
//
// @param pattern the line dash pattern.
func (page *Page) SetLinePattern(pattern string) {
	if page.linePattern != pattern {
		page.linePattern = pattern
		page.appendString(page.linePattern)
		page.appendString(" d\n")
	}
}

// SetDefaultLinePattern sets the default line dash pattern - solid line.
func (page *Page) SetDefaultLinePattern() {
	page.linePattern = "[] 0"
	page.appendString(page.linePattern)
	page.appendString(" d\n")
}

// SetPenWidth sets the penColor width that will be used to draw lines and splines on this page.
func (page *Page) SetPenWidth(width float32) {
	if page.penWidth != width {
		page.penWidth = width
		page.appendFloat32(page.penWidth)
		page.appendString(" w\n")
	}
}

// SetLineCapStyle sets the current line cap style.
// Supported values: Cap.BUTT, Cap.ROUND and Cap.PROJECTING_SQUARE
func (page *Page) SetLineCapStyle(style int) {
	if page.lineCapStyle != style {
		page.lineCapStyle = style
		page.appendInteger(page.lineCapStyle)
		page.appendString(" J\n")
	}
}

// SetLineJoinStyle sets the line join style.
// Supported values: Join.MITER, Join.ROUND and Join.BEVEL
func (page *Page) SetLineJoinStyle(style int) {
	if page.lineJoinStyle != style {
		page.lineJoinStyle = style
		page.appendInteger(page.lineJoinStyle)
		page.appendString(" j\n")
	}
}

// MoveTo moves the penColor to the point with coordinates (x, y) on the page.
//
// @param x the x coordinate of new penColor position.
// @param y the y coordinate of new penColor position.
func (page *Page) MoveTo(x, y float32) {
	page.appendFloat32(x)
	page.appendString(" ")
	page.appendFloat32(page.height - y)
	page.appendString(" m\n")
}

// LineTo draws a line from the current penColor position to the point with coordinates (x, y),
// using the current penColor width and stroke color.
// Make sure you call strokePath(), closePath() or fillPath() after the last call to this method.
func (page *Page) LineTo(x, y float32) {
	page.appendFloat32(x)
	page.appendString(" ")
	page.appendFloat32(page.height - y)
	page.appendString(" l\n")
}

// StrokePath draws the path using the current penColor color.
func (page *Page) StrokePath() {
	page.appendString("S\n")
}

// ClosePath closes the path and draws it using the current penColor color.
func (page *Page) ClosePath() {
	page.appendString("s\n")
}

// FillPath closes and fills the path with the current brushColor color.
func (page *Page) FillPath() {
	page.appendString("f\n")
}

// DrawRect draws the outline of the specified rectangle on the page.
// The left and right edges of the rectangle are at x and x + w.
// The top and bottom edges are at y and y + h.
// The rectangle is drawn using the current penColor color.
// @param x the x coordinate of the rectangle to be drawn.
// @param y the y coordinate of the rectangle to be drawn.
// @param w the width of the rectangle to be drawn.
// @param h the height of the rectangle to be drawn.
func (page *Page) DrawRect(x, y, w, h float32) {
	page.MoveTo(x, y)
	page.LineTo(x+w, y)
	page.LineTo(x+w, y+h)
	page.LineTo(x, y+h)
	page.ClosePath()
}

// FillRect fills the specified rectangle on the page.
// The left and right edges of the rectangle are at x and x + w.
// The top and bottom edges are at y and y + h.
// The rectangle is drawn using the current penColor color.
// @param x the x coordinate of the rectangle to be drawn.
// @param y the y coordinate of the rectangle to be drawn.
// @param w the width of the rectangle to be drawn.
// @param h the height of the rectangle to be drawn.
func (page *Page) FillRect(x, y, w, h float32) {
	page.MoveTo(x, y)
	page.LineTo(x+w, y)
	page.LineTo(x+w, y+h)
	page.LineTo(x, y+h)
	page.FillPath()
}

// DrawPath draws or fills the specified path using the current penColor or brushColor.
// @param path the path.
// @param operation specifies 'stroke' or 'fill' operation.
func (page *Page) DrawPath(path []*Point, operation string) {
	if len(path) < 2 {
		log.Fatal("The Path object must contain at least 2 points.")
	}
	point := path[0]
	page.MoveTo(point.x, point.y)
	var curve bool = false
	for i := 1; i < len(path); i++ {
		point = path[i]
		if point.isControlPoint {
			curve = true
			page.appendPoint(point)
		} else {
			if curve {
				curve = false
				page.appendPoint(point)
				page.appendString("c\n")
			} else {
				page.LineTo(point.x, point.y)
			}
		}
	}
	page.appendString(operation)
	page.appendString("\n")
}

// DrawCircle sdraws a circle on the page.
//
// The outline of the circle is drawn using the current penColor color.
//
// @param x the x coordinate of the center of the circle to be drawn.
// @param y the y coordinate of the center of the circle to be drawn.
// @param r the radius of the circle to be drawn.
func (page *Page) DrawCircle(x, y, r float32) {
	page.drawEllipse(x, y, r, r, operation.Stroke)
}

// FillCircle draws the specified circle on the page and fills it with the current brushColor color.
//
// @param x the x coordinate of the center of the circle to be drawn.
// @param y the y coordinate of the center of the circle to be drawn.
// @param r the radius of the circle to be drawn.
// @param operation must be Operation.STROKE, Operation.CLOSE or Operation.FILL.
func (page *Page) FillCircle(x, y, r float32) {
	page.drawEllipse(x, y, r, r, operation.Fill)
}

// DrawEllipse draws an ellipse on the page using the current penColor color.
// @param x the x coordinate of the center of the ellipse to be drawn.
// @param y the y coordinate of the center of the ellipse to be drawn.
// @param r1 the horizontal radius of the ellipse to be drawn.
// @param r2 the vertical radius of the ellipse to be drawn.
func (page *Page) DrawEllipse(x, y, r1, r2 float32) {
	page.drawEllipse(x, y, r1, r2, operation.Stroke)
}

// FillEllipse fills an ellipse on the page using the current penColor color.
// @param x the x coordinate of the center of the ellipse to be drawn.
// @param y the y coordinate of the center of the ellipse to be drawn.
// @param r1 the horizontal radius of the ellipse to be drawn.
// @param r2 the vertical radius of the ellipse to be drawn.
func (page *Page) FillEllipse(x, y, r1, r2 float32) {
	page.drawEllipse(x, y, r1, r2, operation.Fill)
}

// drawEllipse draws an ellipse on the page and fills it using the current brushColor color.
// @param x the x coordinate of the center of the ellipse to be drawn.
// @param y the y coordinate of the center of the ellipse to be drawn.
// @param r1 the horizontal radius of the ellipse to be drawn.
// @param r2 the vertical radius of the ellipse to be drawn.
// @param operation the operation.
func (page *Page) drawEllipse(x, y, r1, r2 float32, operation string) {
	// The best 4-spline magic number
	var m4 float32 = 0.55228

	// Starting point
	page.MoveTo(x, y-r2)

	page.appendPointXY(x+m4*r1, y-r2)
	page.appendPointXY(x+r1, y-m4*r2)
	page.appendPointXY(x+r1, y)
	page.appendString("c\n")

	page.appendPointXY(x+r1, y+m4*r2)
	page.appendPointXY(x+m4*r1, y+r2)
	page.appendPointXY(x, y+r2)
	page.appendString("c\n")

	page.appendPointXY(x-m4*r1, y+r2)
	page.appendPointXY(x-r1, y+m4*r2)
	page.appendPointXY(x-r1, y)
	page.appendString("c\n")

	page.appendPointXY(x-r1, y-m4*r2)
	page.appendPointXY(x-m4*r1, y-r2)
	page.appendPointXY(x, y-r2)
	page.appendString("c\n")

	page.appendString(operation)
	page.appendString("\n")
}

// DrawPoint draws a point on the page using the current penColor color.
// @param p the point.
func (page *Page) DrawPoint(p *Point) {
	if p.shape != shape.Invisible {
		list := []*Point{}
		switch p.shape {
		case shape.Circle:
			if p.fillShape {
				page.FillCircle(p.x, p.y, p.r)
			} else {
				page.DrawCircle(p.x, p.y, p.r)
			}
		case shape.Diamond:
			list = append(list, NewPoint(p.x, p.y-p.r))
			list = append(list, NewPoint(p.x+p.r, p.y))
			list = append(list, NewPoint(p.x, p.y+p.r))
			list = append(list, NewPoint(p.x-p.r, p.y))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		case shape.Box:
			list = append(list, NewPoint(p.x-p.r, p.y-p.r))
			list = append(list, NewPoint(p.x+p.r, p.y-p.r))
			list = append(list, NewPoint(p.x+p.r, p.y+p.r))
			list = append(list, NewPoint(p.x-p.r, p.y+p.r))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		case shape.Plus:
			page.DrawLine(p.x-p.r, p.y, p.x+p.r, p.y)
			page.DrawLine(p.x, p.y-p.r, p.x, p.y+p.r)
		case shape.UpArrow:
			list = append(list, NewPoint(p.x, p.y-p.r))
			list = append(list, NewPoint(p.x+p.r, p.y+p.r))
			list = append(list, NewPoint(p.x-p.r, p.y+p.r))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		case shape.DownArrow:
			list = append(list, NewPoint(p.x-p.r, p.y-p.r))
			list = append(list, NewPoint(p.x+p.r, p.y-p.r))
			list = append(list, NewPoint(p.x, p.y+p.r))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		case shape.LeftArrow:
			list = append(list, NewPoint(p.x+p.r, p.y+p.r))
			list = append(list, NewPoint(p.x-p.r, p.y))
			list = append(list, NewPoint(p.x+p.r, p.y-p.r))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		case shape.RightArrow:
			list = append(list, NewPoint(p.x-p.r, p.y-p.r))
			list = append(list, NewPoint(p.x+p.r, p.y))
			list = append(list, NewPoint(p.x-p.r, p.y+p.r))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		case shape.HDash:
			page.DrawLine(p.x-p.r, p.y, p.x+p.r, p.y)
		case shape.VDash:
			page.DrawLine(p.x, p.y-p.r, p.x, p.y+p.r)
		case shape.XMark:
			page.DrawLine(p.x-p.r, p.y-p.r, p.x+p.r, p.y+p.r)
			page.DrawLine(p.x-p.r, p.y+p.r, p.x+p.r, p.y-p.r)
		case shape.Multiply:
			page.DrawLine(p.x-p.r, p.y-p.r, p.x+p.r, p.y+p.r)
			page.DrawLine(p.x-p.r, p.y+p.r, p.x+p.r, p.y-p.r)
			page.DrawLine(p.x-p.r, p.y, p.x+p.r, p.y)
			page.DrawLine(p.x, p.y-p.r, p.x, p.y+p.r)
		case shape.Star:
			angle := math.Pi / 10
			sin18 := float32(math.Sin(angle))
			cos18 := float32(math.Cos(angle))
			a := p.r * cos18
			b := p.r * sin18
			c := 2 * a * sin18
			d := 2*a*cos18 - p.r
			list = append(list, NewPoint(p.x, p.y-p.r))
			list = append(list, NewPoint(p.x+c, p.y+d))
			list = append(list, NewPoint(p.x-a, p.y-b))
			list = append(list, NewPoint(p.x+a, p.y-b))
			list = append(list, NewPoint(p.x-c, p.y+d))
			if p.fillShape {
				page.DrawPath(list, operation.Fill)
			} else {
				page.DrawPath(list, operation.Close)
			}
		default:
			panic("unhandled default case")
		}
	}
}

/**
 *  Sets the text rendering mode.
 *
 *  @param mode the rendering mode.
 */
func (page *Page) SetTextRenderingMode(mode int) {
	if mode >= 0 && mode <= 7 {
		page.renderingMode = mode
	} else {
		log.Fatal("Invalid text rendering mode: " + fmt.Sprint(mode))
	}
}

// SetTextDirection sets the text direction.
// @param degrees the angle.
func (page *Page) SetTextDirection(degrees int) {
	if degrees > 360 {
		degrees %= 360
	}
	switch degrees {
	case 0:
		page.tmx = [4]float32{1.0, 0.0, 0.0, 1.0}
	case 90:
		page.tmx = [4]float32{0.0, 1.0, -1.0, 0.0}
	case 180:
		page.tmx = [4]float32{-1.0, 0.0, 0.0, -1.0}
	case 270:
		page.tmx = [4]float32{0.0, -1.0, 1.0, 0.0}
	case 360:
		page.tmx = [4]float32{1.0, 0.0, 0.0, 1.0}
	default:
		sinOfAngle := float32(math.Sin(float64(degrees) * (math.Pi / 180)))
		cosOfAngle := float32(math.Cos(float64(degrees) * (math.Pi / 180)))
		page.tmx = [4]float32{cosOfAngle, sinOfAngle, -sinOfAngle, cosOfAngle}
	}
	page.tm0 = fastfloat.ToByteArray(page.tmx[0])
	page.tm1 = fastfloat.ToByteArray(page.tmx[1])
	page.tm2 = fastfloat.ToByteArray(page.tmx[2])
	page.tm3 = fastfloat.ToByteArray(page.tmx[3])
}

/**
 *  Draws a cubic bezier curve starting from the current point to the end point p3
 *
 *  @param x1 first control point x
 *  @param y1 first control point y
 *  @param x2 second control point x
 *  @param y2 second control point y
 *  @param x3 end point x
 *  @param y3 end point y
 */
func (page *Page) CurveTo(x1, y1, x2, y2, x3, y3 float32) {
	page.appendFloat32(x1)
	page.appendString(" ")
	page.appendFloat32(page.height - y1)
	page.appendString(" ")
	page.appendFloat32(x2)
	page.appendString(" ")
	page.appendFloat32(page.height - y2)
	page.appendString(" ")
	page.appendFloat32(x3)
	page.appendString(" ")
	page.appendFloat32(page.height - y3)
	page.appendString(" c\n")
}

func (page *Page) DrawCircularArc(
	x, y, r, startAngle, sweepDegrees float32) []float32 {
	return page.DrawArc(x, y, r, r, startAngle, sweepDegrees)
}

func (page *Page) DrawArc(
	x, y, rx, ry, startAngle, sweepDegrees float32) []float32 {
	var x1, y1, x2, y2, x3, y3 float32

	numSegments := int(math.Ceil(math.Abs(float64(sweepDegrees)) / 90.0))
	angleRad := float64(startAngle) * math.Pi / 180.0
	deltaPerSeg := float64(sweepDegrees/float32(numSegments)) * math.Pi / 180.0

	for i := 0; i < numSegments; i++ {
		segStart := angleRad
		segEnd := angleRad + deltaPerSeg
		deltaRad := segEnd - segStart // guaranteed ≤ ±π/2

		// Calculate safe κ
		k := float32(4.0 / 3.0 * math.Tan(deltaRad/4.0))

		cosStart := float32(math.Cos(segStart))
		sinStart := float32(math.Sin(segStart))
		cosEnd := float32(math.Cos(segEnd))
		sinEnd := float32(math.Sin(segEnd))

		// End points
		x0 := x + rx*cosStart
		y0 := y + ry*sinStart
		x3 = x + rx*cosEnd
		y3 = y + ry*sinEnd

		// Control points
		x1 = x0 - (k * rx * sinStart)
		y1 = y0 + (k * ry * cosStart)
		x2 = x3 + (k * rx * sinEnd)
		y2 = y3 - (k * ry * cosEnd)

		if i == 0 {
			page.MoveTo(x0, y0)
		}
		page.CurveTo(x1, y1, x2, y2, x3, y3)

		angleRad = segEnd
	}

	return []float32{x1, y1, x2, y2, x3, y3}
}

/**
 *  Draws a bezier curve starting from the current point.
 *  <strong>Please note:</strong> You must call the fillPath, closePath or strokePath method after the last bezierCurveTo call.
 *  <p><i>Author:</i> <strong>Pieter Libin</strong>, pieter@emweb.be</p>
 *
 *  @param p1 first control point
 *  @param p2 second control point
 *  @param p3 end point
 */
func (page *Page) BezierCurveTo(p1, p2, p3 *Point) {
	page.appendPoint(p1)
	page.appendPoint(p2)
	page.appendPoint(p3)
	page.appendString("c\n")
}

// SetTextFont sets the text fonts.
func (page *Page) SetTextFont(font *Font) {
	page.font = font
	if font.fontID != "" {
		page.appendByte('/')
		page.appendString(font.fontID)
	} else {
		page.appendString("/F")
		page.appendInteger(font.objNumber)
	}
	page.appendByte(token.Space)
	page.appendFloat32(font.size)
	page.appendString(" Tf\n")
}

// DrawRectRoundCorners draws rectangle with rounded corners.
// Code provided by:
// Dominique Andre Gunia <contact@dgunia.de>
func (page *Page) DrawRectRoundCorners(x, y, w, h, r1, r2 float32, operation string) {
	// The best 4-spline magic number
	var m4 float32 = 0.551784

	list := []*Point{}

	// Starting point
	list = append(list, NewPoint(x+w-r1, y))
	list = append(list, NewControlPoint(x+w-r1+m4*r1, y))
	list = append(list, NewControlPoint(x+w, y+r2-m4*r2))
	list = append(list, NewPoint(x+w, y+r2))

	list = append(list, NewPoint(x+w, y+h-r2))
	list = append(list, NewControlPoint(x+w, y+h-r2+m4*r2))
	list = append(list, NewControlPoint(x+w-m4*r1, y+h))
	list = append(list, NewPoint(x+w-r1, y+h))

	list = append(list, NewPoint(x+r1, y+h))
	list = append(list, NewControlPoint(x+r1-m4*r1, y+h))
	list = append(list, NewControlPoint(x, y+h-m4*r2))
	list = append(list, NewPoint(x, y+h-r2))

	list = append(list, NewPoint(x, y+r2))
	list = append(list, NewControlPoint(x, y+r2-m4*r2))
	list = append(list, NewControlPoint(x+m4*r1, y))
	list = append(list, NewPoint(x+r1, y))
	list = append(list, NewPoint(x+w-r1, y))

	page.DrawPath(list, operation)
}

// clipPath clips the path.
func (page *Page) ClipPath() {
	page.appendString("W\n")
	page.appendString("n\n") // Close the path without painting it.
}

func (page *Page) ClipRect(x, y, w, h float32) {
	page.MoveTo(x, y)
	page.LineTo(x+w, y)
	page.LineTo(x+w, y+h)
	page.LineTo(x, y+h)
	page.ClipPath()
}

func (page *Page) Save() {
	page.appendString("q\n")
	page.savedStates = append(page.savedStates, NewState(
		page.penColor,
		page.brushColor,
		page.penWidth,
		page.lineCapStyle,
		page.lineJoinStyle,
		page.linePattern))
	page.savedHeight = page.height
}

func (page *Page) Restore() {
	page.appendString("Q\n")
	if len(page.savedStates) > 0 {
		lastIndex := len(page.savedStates) - 1
		savedState := page.savedStates[lastIndex]
		page.penColor = savedState.GetPen()
		page.brushColor = savedState.GetBrush()
		page.penWidth = savedState.GetPenWidth()
		page.lineCapStyle = savedState.GetLineCapStyle()
		page.lineJoinStyle = savedState.GetLineJoinStyle()
		page.linePattern = savedState.GetLinePattern()
		page.savedStates = page.savedStates[:lastIndex] // Remove the last state
	}
	if page.savedHeight != math.MaxFloat32 {
		page.height = page.savedHeight
		page.savedHeight = math.MaxFloat32
	}
}

// SetCropBox sets the page CropBox.
// See page 77 of the PDF32000_2008.pdf specification.
// @param upperLeftX the top left X coordinate of the CropBox.
// @param upperLeftY the top left Y coordinate of the CropBox.
// @param lowerRightX the bottom right X coordinate of the CropBox.
// @param lowerRightY the bottom right Y coordinate of the CropBox.
func (page *Page) SetCropBox(upperLeftX, upperLeftY, lowerRightX, lowerRightY float32) {
	page.cropBox = []float32{upperLeftX, upperLeftY, lowerRightX, lowerRightY}
}

// SetBleedBox sets the page BleedBox.
// See page 77 of the PDF32000_2008.pdf specification.
// @param upperLeftX the top left X coordinate of the BleedBox.
// @param upperLeftY the top left Y coordinate of the BleedBox.
// @param lowerRightX the bottom right X coordinate of the BleedBox.
// @param lowerRightY the bottom right Y coordinate of the BleedBox.
func (page *Page) SetBleedBox(upperLeftX, upperLeftY, lowerRightX, lowerRightY float32) {
	page.bleedBox = []float32{upperLeftX, upperLeftY, lowerRightX, lowerRightY}
}

// SetTrimBox sets the page TrimBox.
// See page 77 of the PDF32000_2008.pdf specification.
// @param upperLeftX the top left X coordinate of the TrimBox.
// @param upperLeftY the top left Y coordinate of the TrimBox.
// @param lowerRightX the bottom right X coordinate of the TrimBox.
// @param lowerRightY the bottom right Y coordinate of the TrimBox.
func (page *Page) SetTrimBox(upperLeftX, upperLeftY, lowerRightX, lowerRightY float32) {
	page.trimBox = []float32{upperLeftX, upperLeftY, lowerRightX, lowerRightY}
}

// SetArtBox sets the page ArtBox.
// See page 77 of the PDF32000_2008.pdf specification.
// @param upperLeftX the top left X coordinate of the ArtBox.
// @param upperLeftY the top left Y coordinate of the ArtBox.
// @param lowerRightX the bottom right X coordinate of the ArtBox.
// @param lowerRightY the bottom right Y coordinate of the ArtBox.
func (page *Page) SetArtBox(upperLeftX, upperLeftY, lowerRightX, lowerRightY float32) {
	page.artBox = []float32{upperLeftX, upperLeftY, lowerRightX, lowerRightY}
}

func (page *Page) appendPointXY(x, y float32) {
	page.appendFloat32(x)
	page.appendString(" ")
	page.appendFloat32(page.height - y)
	page.appendString(" ")
}

func (page *Page) appendPoint(point *Point) {
	page.appendFloat32(point.x)
	page.appendString(" ")
	page.appendFloat32(page.height - point.y)
	page.appendString(" ")
}

func (page *Page) drawWord(font *Font, buf *strings.Builder, brush int32, colors map[string]int32) {
	if buf.Len() > 0 {
		if brushColor, ok := colors[strings.ToLower(buf.String())]; ok {
			page.SetBrushColor(brushColor)
		} else {
			page.SetBrushColor(brush)
		}

		if font.isCoreFont {
			page.appendString("[<")
			page.drawASCIIString(font, buf.String())
			page.appendString(">] TJ\n")
		} else {
			page.appendString("<")
			page.drawUnicodeString(font, buf.String())
			page.appendString("> Tj\n")
		}

		buf.Reset()
	}
}

func (page *Page) drawColoredString(font *Font, str string, brush int32, colors map[string]int32) {
	var buf1 strings.Builder
	var buf2 strings.Builder
	for _, ch := range str {
		if unicode.IsLetter(ch) || unicode.IsDigit(ch) {
			page.drawWord(font, &buf2, brush, colors)
			buf1.WriteRune(ch)
		} else {
			page.drawWord(font, &buf1, brush, colors)
			buf2.WriteRune(ch)
		}
	}
	page.drawWord(font, &buf1, brush, colors)
	page.drawWord(font, &buf2, brush, colors)
}

func (page *Page) setStructElementsPageObjNumber(pageObjNumber int) {
	for _, element := range page.pdf.structElements {
		element.pageObjNumber = pageObjNumber
	}
}

// AddBMC adds BMC to the page.
func (page *Page) AddBMC(structure, language, actualText, altDescription string) {
	if page.pdf.compliance == compliance.PDF_UA_1 {
		element := NewStructElem()
		element.structure = structure
		element.mcid = page.mcid
		element.language = language
		element.actualText = actualText
		element.altDescription = altDescription
		page.pdf.structElements = append(page.pdf.structElements, element)

		page.appendString("/")
		page.appendString(structure)
		page.appendString(" <</MCID ")
		page.appendInteger(page.mcid)
		page.mcid++
		page.appendString(">>\n")
		page.appendString("BDC\n")
	}
}

func (page *Page) AddArtifactBMC() {
	if page.pdf.compliance == compliance.PDF_UA_1 {
		page.appendString("/Artifact BMC\n")
	}
}

// AddEMC adds EMC to the page.
func (page *Page) AddEMC() {
	if page.pdf.compliance == compliance.PDF_UA_1 {
		page.appendString("EMC\n")
	}
}

// AddAnnotation adds annotation to the page.
func (page *Page) AddAnnotation(annotation *Annotation) {
	annotation.y1 = page.height - annotation.y1
	annotation.y2 = page.height - annotation.y2
	page.annots = append(page.annots, annotation)
	if page.pdf.compliance == compliance.PDF_UA_1 {
		element := NewStructElem()
		element.structure = "Link"
		element.language = annotation.language
		element.actualText = *annotation.actualText
		element.altDescription = *annotation.altDescription
		element.annotation = annotation
		page.pdf.structElements = append(page.pdf.structElements, element)
	}
}

func (page *Page) beginTransform(x, y, xScale, yScale float32) {
	page.appendString("q\n")

	page.appendFloat32(xScale)
	page.appendString(" 0 0 ")
	page.appendFloat32(yScale)
	page.appendString(" ")
	page.appendFloat32(x)
	page.appendString(" ")
	page.appendFloat32(y)
	page.appendString(" cm\n")

	page.appendFloat32(xScale)
	page.appendString(" 0 0 ")
	page.appendFloat32(yScale)
	page.appendString(" ")
	page.appendFloat32(x)
	page.appendString(" ")
	page.appendFloat32(y)
	page.appendString(" Tm\n")
}

func (page *Page) endTransform() {
	page.appendString("Q\n")
}

// DrawContents draws the contents on the page.
func (page *Page) DrawContents(
	content []byte,
	h float32, // The height of the graphics object in points.
	x float32,
	y float32,
	xScale float32,
	yScale float32) {
	page.beginTransform(x, (page.height-yScale*h)-y, xScale, yScale)
	page.appendByteArray(content)
	page.endTransform()
}

// DrawArrayOfCharacters draws array of equally spaced characters.
func (page *Page) DrawArrayOfCharacters(font *Font, text string, x, y, dx float32) {
	x1 := x
	for i := 0; i < len(text); i++ {
		page.DrawStringUsingColorMap(font, nil, text[i:i+1], x1, y, color.Black, nil)
		x1 += dx
	}
}

// AddWatermark add watermark to the page.
func (page *Page) AddWatermark(font *Font, text string) {
	hypotenuse := float32(math.Sqrt(
		float64(page.height*page.height + page.width*page.width)))
	stringWidth := font.stringWidth(text)
	offset := (hypotenuse - stringWidth) / 2.0
	angle := math.Atan(float64(page.height / page.width))
	watermark := NewTextLine(font, "")
	watermark.SetColor(color.LightGray)
	watermark.SetText(text)
	watermark.SetLocation(
		offset*float32(math.Cos(angle)),
		page.height-offset*float32(math.Sin(angle)))
	watermark.SetTextDirection((int)(angle * (180.0 / math.Pi)))
	watermark.DrawOn(page)
}

// InvertYAxis inverts the Y axis.
func (page *Page) InvertYAxis() {
	page.appendString("1 0 0 -1 0 ")
	page.appendFloat32(page.height)
	page.appendString(" cm\n")
}

// Transformation matrix.
// Use save before, restore afterwards!
// 9 value array like generated by androids Matrix.getValues()
func (page *Page) Transform(values []float32) {
	scalex := values[mScaleX]
	scaley := values[mScaleY]
	transx := values[mTransX]
	transy := values[mTransY]

	page.appendFloat32(scalex)
	page.appendString(" ")
	page.appendFloat32(values[mSkewX])
	page.appendString(" ")
	page.appendFloat32(values[mSkewY])
	page.appendString(" ")
	page.appendFloat32(scaley)
	page.appendString(" ")

	if math.Asin(float64(values[mSkewY])) != 0.0 {
		transx -= values[mSkewY] * page.height / scaley
	}

	page.appendFloat32(transx)
	page.appendString(" ")
	page.appendFloat32(-transy)
	page.appendString(" cm\n")

	page.height = page.height / scaley
}

// AddHeader adds header to this page.
func (page *Page) AddHeader(textLine *TextLine) []float32 {
	return page.AddHeaderOffsetBy(textLine, 1.5*textLine.font.ascent)
}

// AddHeaderOffsetBy adds header to this page offset by the specified value.
func (page *Page) AddHeaderOffsetBy(textLine *TextLine, offset float32) []float32 {
	textLine.SetLocation((page.GetWidth()-textLine.GetWidth())/2, offset)
	xy := textLine.DrawOn(page)
	xy[1] += page.font.descent
	return xy
}

// AddFooter adds footer to this page.
func (page *Page) AddFooter(textLine *TextLine) []float32 {
	return page.AddFooterOffsetBy(textLine, textLine.font.ascent)
}

// AddFooterOffsetBy adds footer to this page offset by the specified value.
func (page *Page) AddFooterOffsetBy(textLine *TextLine, offset float32) []float32 {
	textLine.SetLocation((page.GetWidth()-textLine.GetWidth())/2, page.GetHeight()-offset)
	return textLine.DrawOn(page)
}

// BeginText begins text block.
func (page *Page) BeginText() {
	page.appendString("BT\n")
}

// EndText ends text block.
func (page *Page) EndText() {
	page.appendString("ET\n")
}

func (page *Page) SetTextLocation(x, y float32) {
	page.appendFloat32(x)
	page.appendByte(token.Space)
	page.appendFloat32(page.height - y)
	page.appendString(" Td\n")
}

func (page *Page) SetTextLeading(leading float32) {
	page.appendFloat32(leading)
	page.appendString(" TL\n")
}

func (page *Page) NextLine() {
	page.appendString("T*\n")
}

func (page *Page) SetTextScaling(scaling float32) {
	page.appendFloat32(scaling)
	page.appendString(" Tz\n")
}

func (page *Page) SetTextRise(rise float32) {
	page.appendFloat32(rise)
	page.appendString(" Ts\n")
}

func (page *Page) DrawText(str string) {
	if page.font.isCoreFont {
		page.appendString("[<")
		page.drawASCIIString(page.font, str)
		page.appendString(">] TJ\n")
	} else {
		page.appendString("<")
		page.drawUnicodeString(page.font, str)
		page.appendString("> Tj\n")
	}
}

func (page *Page) appendInteger(value int) {
	page.buf = append(page.buf, []byte(strconv.Itoa(value))...)
}

func (page *Page) appendFloat32(value float32) {
	page.buf = append(page.buf, fastfloat.ToByteArray(value)...)
}

func (page *Page) appendString(s1 string) {
	page.buf = append(page.buf, []byte(s1)...)
}

func (page *Page) appendByte(b byte) {
	page.buf = append(page.buf, b)
}

func (page *Page) appendByteArray(a []byte) {
	page.buf = append(page.buf, a...)
}

func (page *Page) ScaleAndRotate(x, y, w, h, degrees float32) {
	// PDF transformations apply LAST-TO-FIRST (like a stack: last command = first applied)

	// [FINAL POSITIONING - Applied First]
	// Moves rotated/scaled image to target (x,y) on page
	page.appendString("1 0 0 1 ")
	page.appendFloat32(x + w/2)
	page.appendString(" ")
	page.appendFloat32((page.height - y) - h/2)
	page.appendString(" cm\n")

	// [ROTATION - Applied Second]
	// Rotates around current origin (0,0) by 'degrees'
	radians := degrees * (math.Pi / 180)
	cos := float32(math.Cos(float64(radians)))
	sin := float32(math.Sin(float64(radians)))
	page.appendByteArray(fastfloat.ToByteArray(cos))
	page.appendString(" ")
	page.appendByteArray(fastfloat.ToByteArray(sin))
	page.appendString(" ")
	page.appendByteArray(fastfloat.ToByteArray(-sin))
	page.appendString(" ")
	page.appendByteArray(fastfloat.ToByteArray(cos))
	page.appendString(" 0 0 cm\n")

	// [ORIGIN SETUP - Applied Last]
	// Centers image at (0,0) and sets scale
	page.appendFloat32(w)
	page.appendString(" 0 0 ")
	page.appendFloat32(h)
	page.appendString(" ")
	page.appendFloat32(-w / 2)
	page.appendString(" ")
	page.appendFloat32(-h / 2)
	page.appendString(" cm\n")
}

func (page *Page) RotateAroundCenter(centerX, centerY, degrees float32) {
	page.appendString("1 0 0 1 ")
	page.appendFloat32(centerX)
	page.appendString(" ")
	page.appendFloat32(centerY)
	page.appendString(" cm\n")

	radians := degrees * (math.Pi / 180)
	cos := float32(math.Cos(float64(radians)))
	sin := float32(math.Sin(float64(radians)))
	page.appendByteArray(fastfloat.ToByteArray(cos))
	page.appendString(" ")
	page.appendByteArray(fastfloat.ToByteArray(sin))
	page.appendString(" ")
	page.appendByteArray(fastfloat.ToByteArray(-sin))
	page.appendString(" ")
	page.appendByteArray(fastfloat.ToByteArray(cos))
	page.appendString(" 0 0 cm\n")

	page.appendString("1 0 0 1 ")
	page.appendFloat32(-centerX)
	page.appendString(" ")
	page.appendFloat32(-centerY)
	page.appendString(" cm\n")
}
