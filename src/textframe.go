package pdfjet

/**
 * textframe.go
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */

import (
	"strings"

	"github.com/edragoev1/pdfjet/src/single"
)

// TextFrame please see Example_47
type TextFrame struct {
	paragraphs               []*TextLine
	font                     *Font
	fallbackFont             *Font
	x, y, w, h, xText, yText float32
	leading                  float32
	paragraphLeading         float32
	beginParagraphPoints     [][]float32
	spaceBetweenTextLines    float32
	border                   bool
}

// NewTextFrame constructs new text frame.
func NewTextFrame(paragraphs []*TextLine) *TextFrame {
	textFrame := new(TextFrame)
	if paragraphs != nil {
		textFrame.paragraphs = paragraphs
		textFrame.font = textFrame.paragraphs[0].GetFont()
		textFrame.fallbackFont = textFrame.paragraphs[0].GetFallbackFont()
		textFrame.leading = textFrame.font.GetBodyHeight(textFrame.font.GetSize())
		textFrame.paragraphLeading = 2 * textFrame.leading
		textFrame.beginParagraphPoints = make([][]float32, 0)
		textFrame.spaceBetweenTextLines = textFrame.font.StringWidth(
			textFrame.fallbackFont, textFrame.font.size, single.Space)
		// Reverse the paragraphs
		for i, j := 0, len(paragraphs)-1; i < j; i, j = i+1, j-1 {
			paragraphs[i], paragraphs[j] = paragraphs[j], paragraphs[i]
		}
	}
	return textFrame
}

// SetLocation sets the location of the frame on the page.
func (frame *TextFrame) SetLocation(x, y float32) *TextFrame {
	frame.x = x
	frame.y = y
	return frame
}

// SetWidth sets the width of the frame.
func (frame *TextFrame) SetWidth(w float32) *TextFrame {
	frame.w = w
	return frame
}

// SetHeight sets the height of the frame.
func (frame *TextFrame) SetHeight(h float32) *TextFrame {
	frame.h = h
	return frame
}

// SetLeading sets the text lines leading.
func (frame *TextFrame) SetLeading(leading float32) *TextFrame {
	frame.leading = leading
	return frame
}

// SetParagraphLeading sets the paragraph leading.
func (frame *TextFrame) SetParagraphLeading(paragraphLeading float32) *TextFrame {
	frame.paragraphLeading = paragraphLeading
	return frame
}

// GetStartParagraphPoints returns the start paragraph points.
func (frame *TextFrame) GetStartParagraphPoints() [][]float32 {
	return frame.beginParagraphPoints
}

// SetSpaceBetweenTextLines sets the space between the text lines.
func (frame *TextFrame) SetSpaceBetweenTextLines(spaceBetweenTextLines float32) *TextFrame {
	frame.spaceBetweenTextLines = spaceBetweenTextLines
	return frame
}

// GetParagraphs returns the paragraphs.
func (frame *TextFrame) GetParagraphs() []*TextLine {
	return frame.paragraphs
}

// SetPosition sets the position of the text frame on the page.
func (frame *TextFrame) SetPosition(x, y float32) {
	frame.SetLocation(x, y)
}

// SetDrawBorder sets the 'set border' variable.
func (frame *TextFrame) SetDrawBorder(border bool) {
	frame.border = border
}

// DrawOn draws the text frame on the page.
func (frame *TextFrame) DrawOn(page *Page) []float32 {
	frame.xText = frame.x
	frame.yText = frame.y + frame.font.ascent
	for len(frame.paragraphs) > 0 {
		// The paragraphs are reversed so we can efficiently remove the first one:
		textLine := frame.paragraphs[len(frame.paragraphs)-1]
		textLine.SetLocation(frame.xText, frame.yText)
		frame.paragraphs = frame.paragraphs[:len(frame.paragraphs)-1]
		frame.beginParagraphPoints = append(frame.beginParagraphPoints, []float32{frame.xText, frame.yText})
		for {
			textLine = frame.drawLineOnPage(page, textLine)
			if textLine.text == "" {
				break
			}
			frame.yText = textLine.advance(frame.leading)
			if frame.yText+frame.font.descent >= (frame.y + frame.h) {
				// The paragraphs are reversed so we can efficiently add new first paragraph:
				frame.paragraphs = append(frame.paragraphs, textLine)
				frame.DrawBorder(page)
				return []float32{frame.x + frame.w, frame.y + frame.h}
			}
		}
		frame.xText = frame.x
		frame.yText += frame.paragraphLeading
	}
	frame.DrawBorder(page)
	return []float32{frame.x + frame.w, frame.y + frame.h}
}

func (frame *TextFrame) DrawBorder(page *Page) {
	if page != nil && frame.border {
		box := NewBox()
		box.SetLocation(frame.x, frame.y)
		box.SetSize(frame.w, frame.h)
		box.DrawOn(page)
	}
}

func (frame *TextFrame) drawLineOnPage(page *Page, textLine *TextLine) *TextLine {
	var sb1 strings.Builder
	var sb2 strings.Builder
	tokens := strings.Fields(textLine.text)
	testForFit := true
	for _, token := range tokens {
		if testForFit && textLine.font.stringWidth(textLine.font.size, sb1.String()+token) < frame.w {
			sb1.WriteString(token + single.Space)
		} else {
			testForFit = false
			sb2.WriteString(token + single.Space)
		}
	}
	textLine.SetText(strings.TrimSpace(sb1.String()))
	if page != nil {
		textLine.DrawOn(page)
	}
	textLine.SetText(strings.TrimSpace(sb2.String()))
	return textLine
}

// IsNotEmpty returns true if there is more text to draw, otherwise it returns false.
func (frame *TextFrame) IsNotEmpty() bool {
	return len(frame.paragraphs) > 0
}
