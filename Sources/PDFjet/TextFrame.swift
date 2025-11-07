/**
 * TextFrame.swift
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
import Foundation

///
/// Please see Example_47
///
public class TextFrame : Drawable {
    private var paragraphs: Array<TextLine>?
    private var font: Font?
    private var fallbackFont: Font?
    private var fontSize: Float?
    private var x: Float = 0.0
    private var y: Float = 0.0
    private var w: Float = 0.0
    private var h: Float = 0.0
    private var leading: Float = 0.0
    private var paragraphLeading: Float = 0.0
    private var beginParagraphPoints: [[Float]]?
    private var border = false

    public init(_ paragraphs: Array<TextLine>) {
        self.paragraphs = Array(paragraphs)
        self.font = paragraphs[0].getFont()
        self.fallbackFont = paragraphs[0].getFallbackFont()
        self.leading = font!.getBodyHeight()
        self.paragraphLeading = 2*leading
        self.beginParagraphPoints = [[Float]]()
        if fallbackFont != nil && (fallbackFont!.getBodyHeight() > self.leading) {
            self.leading = fallbackFont!.getBodyHeight()
        }
        self.paragraphs!.reverse()
    }

    @discardableResult
    public func setLocation(_ x: Float, _ y: Float) -> TextFrame {
        self.x = x
        self.y = y
        return self
    }

    @discardableResult
    public func setWidth(_ w: Float) -> TextFrame {
        self.w = w
        return self
    }

    @discardableResult
    public func setHeight(_ h: Float) -> TextFrame {
        self.h = h
        return self
    }

    @discardableResult
    public func setLeading(_ leading: Float) -> TextFrame {
        self.leading = leading
        return self
    }

    @discardableResult
    public func setParagraphLeading(_ paragraphLeading: Float) -> TextFrame {
        self.paragraphLeading = paragraphLeading
        return self
    }

    public func setParagraphs(_ paragraphs: Array<TextLine>) {
        self.paragraphs = paragraphs
    }

    public func getParagraphs() -> Array<TextLine>? {
        return self.paragraphs
    }

    @discardableResult
    public func getBeginParagraphPoints() -> [[Float]]? {
        return self.beginParagraphPoints
    }

    public func setPosition(_ x: Float, _ y: Float) {
        setLocation(x, y)
    }

    public func setBorder(_ border: Bool) {
        self.border = border
    }

    public func setDrawBorder(_ border: Bool) {
        self.border = border
    }

    public func setFontSize(_ fontSize: Float) {
        self.fontSize = fontSize
    }

    @discardableResult
    public func drawOn(_ page: Page?) -> [Float] {
        var xText = self.x
        var yText = self.y + self.font!.ascent
        while paragraphs!.count > 0 {
            // The paragraphs are reversed so we can efficiently remove the first one:
            var textLine = paragraphs!.removeLast()
            textLine.setLocation(xText, yText)
            beginParagraphPoints!.append([xText, yText])
            while true {
                textLine = drawLineOnPage(textLine, page)
                if textLine.getText() == "" {
                    break
                }
                yText = textLine.advance(leading)
                if yText + font!.descent >= (self.y + self.h) {
                    // The paragraphs are reversed so we can efficiently add new first paragraph:
                    paragraphs!.append(textLine)
                    drawBorder(page!)
                    return [x + w, y + h]
                }
            }
            xText = x
            yText += paragraphLeading
        }
        drawBorder(page!)
        return [x + w, y + h]
    }

    private func drawBorder(_ page: Page?) {
        if page != nil && border {
            let box = Rect(x, y, w, h)
            box.drawOn(page)
        }
    }

    private func drawLineOnPage(_ textLine: TextLine, _ page: Page?) -> TextLine {
        var sb1 = String()
        var sb2 = String()
        let tokens = textLine.text!.components(separatedBy: .whitespaces)
        var testForFit = true
        for token in tokens {
            if testForFit && textLine.getStringWidth(sb1 + token) < self.w {
                sb1.append(token + Single.space)
            } else {
                testForFit = false
                sb2.append(token + Single.space)
            }
        }
        textLine.setText(sb1.trim())
        if page != nil {
            textLine.drawOn(page!)
        }
        textLine.setText(sb2.trim())
        return textLine
    }

    public func isNotEmpty() -> Bool {
        return paragraphs!.count > 0
    }
}   // End of TextFrame.swift
