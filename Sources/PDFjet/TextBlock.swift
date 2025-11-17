/**
 * TextBlock.swift
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
import Foundation

public class TextBlock : Drawable {
    private var x: Float = 0.0
    private var y: Float = 0.0
    private var width: Float = 500.0
    private var height: Float = 500.0
    private var font: Font
    private var fallbackFont: Font?
    private var fontSize: Float = 12.0
    private var textContent: String
    private var textLineHeight: Float = 1.0
    private var textPadding: Float = 0.0

    private var fillColor: [Float]?
    private var textColor: [Float] = [0.0, 0.0, 0.0]
    private var borderColor: [Float]?
    private var borderWidth: Float = 0.5
    private var borderCornerRadius: Float = 0.0

    private var language: String = "en-US"
    private var altDescription: String = ""
    private var uri: String?
    private var key: String?
    private var uriLanguage: String?
    private var uriActualText: String?
    private var uriAltDescription: String?
    private var textDirection: Direction = Direction.LEFT_TO_RIGHT
    private var textAlignment: Alignment = Alignment.LEFT
    private var underline: Bool = false
    private var strikeout: Bool = false

    private var lineSpacing: Float = 1.0

    private var highlightColors: [String: Int32]?

    public init(_ font: Font, _ textContent: String) {
        self.font = font
        self.textContent = textContent
    }

    public func setFont(_ font: Font) {
        self.font = font
    }

    public func setFallbackFont(_ font: Font) {
        self.fallbackFont = font
    }

    public func setFontSize(_ size: Float) {
        self.font.setSize(size)
    }

    public func setFallbackFontSize(_ size: Float) {
        fallbackFont?.setSize(size)
    }

    public func setText(_ text: String) {
        self.textContent = text
    }

    public func getFont() -> Font {
        return font
    }

    public func getText() -> String {
        return textContent
    }

    public func setLocation(_ x: Float, _ y: Float) {
        self.x = x
        self.y = y
    }

    public func setPosition(_ x: Float, _ y: Float) {
        self.x = x
        self.y = y
    }

    public func setSize(_ w: Float, _ h: Float) {
        self.width = w
        self.height = h
    }

    public func setWidth(_ w: Float) {
        self.width = w
        self.height = 0.0
    }

    public func setBorderCornerRadius(_ radius: Float) {
        self.borderCornerRadius = radius
    }

    public func setTextPadding(_ padding: Float) {
        self.textPadding = padding
    }

    public func setBorderWidth(_ width: Float) {
        self.borderWidth = width
    }

    public func setTextLineHeight(_ lineHeight: Float) {
        self.textLineHeight = lineHeight
    }

    public func setTextColor(_ color: Int32) {
        let r = Float(((color >> 16) & 0xff))/255.0
        let g = Float(((color >>  8) & 0xff))/255.0
        let b = Float(((color)       & 0xff))/255.0
        self.textColor = [r, g, b]
    }

    public func setTextColor(_ textColor: [Float]) -> TextBlock {
        self.textColor = textColor
        return self
    }

    public func setTextColor(_ r: Float, _ g: Float, _ b: Float) -> TextBlock {
        self.textColor = [r, g, b]
        return self
    }

    public func setFillColor(_ color: Int32) {
        let r = Float(((color >> 16) & 0xff))/255.0
        let g = Float(((color >>  8) & 0xff))/255.0
        let b = Float(((color)       & 0xff))/255.0
        self.fillColor = [r, g, b]
    }

    public func setFillColor(_ fillColor: [Float]?) -> TextBlock {
        self.fillColor = fillColor
        return self
    }

    public func setBorderColor(_ color: Int32) {
        let r = Float(((color >> 16) & 0xff))/255.0
        let g = Float(((color >>  8) & 0xff))/255.0
        let b = Float(((color)       & 0xff))/255.0
        self.borderColor = [r, g, b]
    }

    public func setBorderColor(_ borderColor: [Float]?) -> TextBlock {
        self.borderColor = borderColor
        return self
    }

    public func setHighlightColors(_ highlightColors: [String: Int32]) {
        self.highlightColors = highlightColors
    }

    public func setLineSpacing(_ lineSpacing: Float) {
        self.lineSpacing = lineSpacing
    }

    public func setTextAlignment(_ alignment: Alignment) {
        self.textAlignment = alignment
    }

    private func textIsCJK(_ str: String) -> Bool {
        let chars = Array(str)
        var numOfCJK = 0
        for ch in chars {
            if (ch.unicodeScalars.first!.value >= 0x4E00 && ch.unicodeScalars.first!.value <= 0x9FD5) ||
                (ch.unicodeScalars.first!.value >= 0x3040 && ch.unicodeScalars.first!.value <= 0x309F) ||
                (ch.unicodeScalars.first!.value >= 0x30A0 && ch.unicodeScalars.first!.value <= 0x30FF) ||
                (ch.unicodeScalars.first!.value >= 0x1100 && ch.unicodeScalars.first!.value <= 0x11FF) {
                numOfCJK += 1
            }
        }
        return numOfCJK > chars.count / 2
    }

    private func getTextLines() -> [TextLine] {
        var list = [TextLine]()

        let textAreaWidth: Float
        if textDirection == Direction.LEFT_TO_RIGHT {
            textAreaWidth = width - 2 * textPadding
        } else {
            textAreaWidth = height - 2 * textPadding
        }

        let lines = textContent.components(separatedBy: .newlines)
        for line in lines {
            if font.stringWidth(fallbackFont, line) <= textAreaWidth {
                list.append(TextLine(font, line))
            } else {
                if textIsCJK(line) {
                    var sb = ""
                    for ch in line {
                        if font.stringWidth(fallbackFont, sb + String(ch)) <= textAreaWidth {
                            sb.append(ch)
                        } else {
                            list.append(TextLine(font, sb))
                            sb = String(ch)
                        }
                    }
                    if !sb.isEmpty {
                        list.append(TextLine(font, sb))
                    }
                } else {
                    var sb = ""
                    let tokens = line.split(separator: " ").map { String($0) }
                    for token in tokens {
                        if font.stringWidth(fallbackFont, sb + token) <= textAreaWidth {
                            sb.append(token)
                            sb.append(" ")
                        } else {
                            list.append(TextLine(font, sb.trimmingCharacters(in: .whitespaces)))
                            sb = token + " "
                        }
                    }
                    if !sb.trimmingCharacters(in: .whitespaces).isEmpty {
                        list.append(TextLine(font, sb.trimmingCharacters(in: .whitespaces)))
                    }
                }
            }
        }

        return list
    }
/*
    @discardableResult
    public func drawOn(_ page: Page?) -> [Float] {
        page!.setBrushColor(textColor)
        page!.setPenWidth(Float(font.getUnderlineThickness()))
        page!.setPenColor(borderColor)

        let ascent = font.getAscent()
        let descent = font.getDescent()
        let leading = (ascent + descent) * textLineHeight
        let lines = getTextLines()
        var xText: Float = 0.0
        var yText: Float = 0.0

        switch textDirection {
        case Direction.LEFT_TO_RIGHT:
            yText = y + ascent + textPadding
            for line in lines {
                switch textAlignment {
                case Alignment.LEFT:
                    xText = x + textPadding
                case Alignment.RIGHT:
                    xText = (x + width) - (font.stringWidth(fallbackFont, line) + textPadding)
                case Alignment.CENTER:
                    xText = x + (width - font.stringWidth(fallbackFont, line)) / 2
                }
                if page != nil {
//                     drawTextLine(
//                         page!,
//                         font,
//                         fallbackFont,
//                         line,
//                         xText,
//                         yText,
//                         textColor,
//                         highlightColors)
                }
                yText += leading
            }
        case Direction.BOTTOM_TO_TOP:
            xText = x + textPadding + ascent
            yText = y + height - textPadding
            for line in lines {
//                 drawTextLine(
//                     page,
//                     font,
//                     fallbackFont,
//                     line,
//                     xText,
//                     yText,
//                     textColor,
//                     highlightColors)
                xText += leading
            }
        case Direction.TOP_TO_BOTTOM:
            break
        }

        xText -= leading
        if (xText + descent + textPadding) - x > width {
            width = (xText + descent + textPadding) - x
        }
        yText -= leading
        if (yText + descent + textPadding) - y > height {
            height = (yText + descent + textPadding) - y
        }

        let rect = Rect(x, y, width, height)
        rect.setBorderColor(borderColor)
        rect.setCornerRadius(borderCornerRadius)
        rect.drawOn(page!)

        if textDirection == Direction.LEFT_TO_RIGHT && (uri != nil || key != nil) {
            page!.addAnnotation(Annotation(
                uri,
                key,
                x,
                y,
                x + width,
                y + height,
                uriLanguage,
                uriActualText,
                uriAltDescription))
        }
        page!.setTextDirection(0)

        return [x + width, y + height]
    }

    @discardableResult
    public func drawTextLine(
        _ page: Page?,
        _ font: Font,
        _ fallbackFont: Font?,
        _ fontSize: Float,
        _ text: String,
        _ xText: Float,
        _ yText: Float,
        _ textColor: [Float],
        _ highlightColors: [String: Int32]?) -> [Float] {

        page!.addBMC("P", language, text, altDescription)
        if textDirection == Direction.BOTTOM_TO_TOP {
            page!.setTextDirection(90)
        }
        page!.drawString(font, fallbackFont, fontSize, text, xText, yText, textColor, highlightColors)
        page!.addEMC()
        if textDirection == Direction.LEFT_TO_RIGHT {
            let lineLength = font.stringWidth(fallbackFont, text)
            if underline {
                page!.addArtifactBMC()
                page!.moveTo(xText, yText + Float(font.getUnderlinePosition()))
                page!.lineTo(xText + lineLength, yText + Float(font.getUnderlinePosition()))
                page!.strokePath()
                page!.addEMC()
            }
            if strikeout {
                page!.addArtifactBMC()
                page!.moveTo(xText, yText - (font.getBodyHeight() / 4))
                page!.lineTo(xText + lineLength, yText - (font.getBodyHeight() / 4))
                page!.strokePath()
                page!.addEMC()
            }
        }
    }
*/
    @discardableResult
    public func setURIAction(_ uri: String) -> TextBlock {
        self.uri = uri
        return self
    }

    public func setTextDirection(_ direction: Direction) {
        self.textDirection = direction
    }

    @discardableResult
    public func drawOn(_ page: Page?) -> [Float] {
        let ascent = font.getAscent(fontSize)
        let descent = font.getDescent(fontSize)
        let leading = (ascent + descent) * lineSpacing
        let textLines = getTextLines()
        if page == nil {
            return [width, max(height, Float(textLines.count) * leading + 2 * textPadding)]
        }

        page!.append("q\n")
        page!.setPenWidth(borderWidth)
        if textAlignment == Alignment.RIGHT {
            // rightAlignText(textLines)    // TODO:
        } else if textAlignment == Alignment.CENTER {
            // centerText(textLines)
        }
        if underline {
            // underlineText(textLines)
        }

        if self.borderColor != nil {
            let rect = Rect(
                x,
                y,
                width,
                max(height, Float(textLines.count) * leading + 2 * textPadding))
            // rect.setFillColor(fillColor) // TODO
            // rect.setBorderWidth(borderWidth)
            rect.setBorderColor(self.borderColor)
            rect.setCornerRadius(borderCornerRadius)
            rect.drawOn(page)
        }

        // page!.addBMC(StructElem.P, language, textContent, null)
        page!.drawTextBlock(
            font,
            fontSize,
            textLines,
            x + textPadding,
            y + textPadding,
            leading,
            textColor,
            highlightColors)
        // page!.addEMC()

        page!.append("Q\n")

        return [x + width, max(y + height, y + Float(textLines.count) * leading + 2 * textPadding)]
    }
}
