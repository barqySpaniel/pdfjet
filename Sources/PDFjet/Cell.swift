/**
 * Cell.swift
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
import Foundation

/**
 * Used to create table cell objects.
 * See the Table class for more information.
 */
public class Cell {
    var font: Font?
    var fallbackFont: Font?
    var fontSize: Float = 12.0
    var text: String?
    var image: Image?
    var barcode: Barcode?
    var textBox: TextBox?
    var textBlock: TextBlock?
    var textColumn: TextColumn?
    var point: Point?
    var compositeTextLine: CompositeTextLine?
    var width: Float = 50.0
    var topPadding: Float = 2.0
    var bottomPadding: Float = 2.0
    var leftPadding: Float = 2.0
    var rightPadding: Float = 2.0
    var lineWidth: Float = 0.0

    var backgroundColor: [Float] = [1.0, 1.0, 1.0]
    var textColor: [Float] = [0.0, 0.0, 0.0]
    var strokeWidth: Float = 0.0
    var strokeColor: [Float] = [0.0, 0.0, 0.0]
    var strokePattern: String = "[] 0"  // Solid

    // Cell properties
    // Colspan:
    // bits 0 to 15
    // Border:
    // bit 16 - top
    // bit 17 - bottom
    // bit 18 - left
    // bit 19 - right
    // Text Alignment:
    // bit 20
    // bit 21
    // Text Decoration:
    // bit 22 - underline
    // bit 23 - strikeout
    // Future use:
    // bits 24 to 31
    private var properties: UInt32 = 0x00050001 // Set only left and top borders!
    private var uri: String?
    private var valign = Align.TOP

    /**
     * Creates a cell object and sets the font.
     *
     * @param font the font.
     */
    public init(_ font: Font?) {
        self.font = font
    }

    /**
     * Creates a cell object and sets the font and the cell text.
     *
     * @param font the font.
     * @param text the text.
     */
    public init(_ font: Font?, _ text: String?) {
        self.font = font
        self.text = text
    }

    /**
     * Creates a cell object and sets the font, fallback font and the cell text.
     *
     * @param font the font.
     * @param fallbackFont the fallback font.
     * @param text the text.
     */
    public init(_ font: Font?, _ fallbackFont: Font?, _ text: String?) {
        self.font = font
        self.fallbackFont = fallbackFont
        self.text = text
    }

    /**
     * Sets the font for this cell.
     *
     * @param font the font.
     */
    public func setFont(_ font: Font?) {
        self.font = font
    }

    /**
     * Sets the fallback font for this cell.
     *
     * @param fallbackFont the fallback font.
     */
    public func setFallbackFont(_ fallbackFont: Font?) {
        self.fallbackFont = fallbackFont
    }

    /**
     * Returns the font used by this cell.
     *
     * @return the font.
     */
    public func getFont() -> Font? {
        return self.font
    }

    /**
     * Returns the fallback font used by this cell.
     *
     * @return the fallback font.
     */
    public func getFallbackFont() -> Font? {
        return self.fallbackFont
    }

    /**
     * Sets the cell text.
     *
     * @param text the cell text.
     */
    @discardableResult
    public func setText(_ text: String?) -> Cell {
        self.text = text
        return self
    }

    /**
     * Returns the cell text.
     *
     * @return the cell text.
     */
    public func getText() -> String? {
        return self.text
    }

    /**
     * Sets the image inside this cell.
     *
     * @param image the image.
     */
    public func setImage(_ image: Image?) {
        self.image = image
        self.text = nil
    }

    /**
     * Returns the cell image.
     *
     * @return the image.
     */
    public func getImage() -> Image? {
        return self.image
    }

    public func setBarcode(_ barcode: Barcode) {
        self.barcode = barcode
        self.text = nil
    }

    public func setTextBox(_ textBox: TextBox) {
        self.textBox = textBox
    }

    /**
     * Sets the point inside this cell.
     * See the Point class and Example_09 for more information.
     *
     * @param point the point.
     */
    @discardableResult
    public func setPoint(_ point: Point?) -> Cell {
        self.point = point
        return self
    }

    /**
     * Returns the cell point.
     *
     * @return the point.
     */
    public func getPoint() -> Point? {
        return self.point
    }

    /**
     * Sets the composite text object.
     *
     * @param compositeTextLine the composite text object.
     */
    public func setCompositeTextLine(_ compositeTextLine: CompositeTextLine?) {
        self.compositeTextLine = compositeTextLine
    }

    /**
     * Returns the composite text object.
     *
     * @return the composite text object.
     */
    public func getCompositeTextLine() -> CompositeTextLine? {
        return self.compositeTextLine
    }

    /**
     * Sets the width of this cell.
     *
     * @param width the specified width.
     */
    public func setWidth(_ width: Float) {
        self.width = width
        if self.textBox != nil {
            self.textBox!.setWidth(self.width - (self.leftPadding + self.rightPadding))
        }
    }

    /**
     * Returns the cell width.
     *
     * @return the cell width.
     */
    public func getWidth() -> Float {
        return self.width
    }

    /**
     * Sets the top padding of this cell.
     *
     * @param padding the top padding.
     */
    public func setTopPadding(_ padding: Float) {
        self.topPadding = padding
    }

    /**
     * Sets the bottom padding of this cell.
     *
     * @param padding the bottom padding.
     */
    public func setBottomPadding(_ padding: Float) {
        self.bottomPadding = padding
    }

    /**
     * Sets the left padding of this cell.
     *
     * @param padding the left padding.
     */
    public func setLeftPadding(_ padding: Float) {
        self.leftPadding = padding
    }

    public func getLeftPadding() -> Float {
        return self.leftPadding
    }

    /**
     * Sets the right padding of this cell.
     *
     * @param padding the right padding.
     */
    public func setRightPadding(_ padding: Float) {
        self.rightPadding = padding
    }

    public func getRightPadding() -> Float {
        return self.rightPadding
    }

    /**
     * Sets the top, bottom, left and right paddings of this cell.
     *
     * @param padding the right padding.
     */
    public func setPadding(_ padding: Float) {
        self.topPadding = padding
        self.bottomPadding = padding
        self.leftPadding = padding
        self.rightPadding = padding
    }

    /**
     * Returns the cell height.
     *
     * @return the cell height.
     */
    public func getHeight(_ width: Float) -> Float {
        var cellHeight = Float(0.0)
        if textBox != nil {
            textBox!.setWidth(width)
            cellHeight = (textBox!.drawOn(nil)[1] - textBox!.y) + topPadding + bottomPadding
        } else if image != nil {
            cellHeight = image!.getHeight() + topPadding + bottomPadding
        } else if barcode != nil {
            cellHeight = barcode!.getHeight() + topPadding + bottomPadding
        } else if text != nil {
            var fontHeight = font!.getBodyHeight()
            if fallbackFont != nil && fallbackFont!.getBodyHeight() > fontHeight {
                fontHeight = fallbackFont!.getBodyHeight()
            }
            cellHeight = fontHeight + topPadding + bottomPadding
        }
        return cellHeight
    }

    public func setTextColor(_ color: Int32) {
        let r = Float(((color >> 16) & 0xff))/255.0
        let g = Float(((color >>  8) & 0xff))/255.0
        let b = Float(((color)       & 0xff))/255.0
        self.textColor = [r, g, b]
    }

    public func setTextColor(_ r: Float, _ g: Float, _ b: Float) {
        self.textColor = [r, g, b]
    }

    public func setTextColor(_ textColor: [Float]) {
        self.textColor = textColor
    }

    public func getTextColor() -> [Float] {
        return self.textColor
    }

    public func setBackgroundColor(_ color: Int32) {
        let r = Float(((color >> 16) & 0xff))/255.0
        let g = Float(((color >>  8) & 0xff))/255.0
        let b = Float(((color)       & 0xff))/255.0
        self.backgroundColor = [r, g, b]
    }

    public func setBackgroundColor(_ r: Float, _ g: Float, _ b: Float) {
        self.backgroundColor = [r, g, b]
    }

    public func setBackgroundColor(_ backgroundColor: [Float]) {
        self.backgroundColor = backgroundColor
    }

    public func getBackgroundColor() -> [Float] {
        return self.backgroundColor
    }

    public func setStrokeColor(_ color: Int32) {
        let r = Float(((color >> 16) & 0xff))/255.0
        let g = Float(((color >>  8) & 0xff))/255.0
        let b = Float(((color)       & 0xff))/255.0
        self.strokeColor = [r, g, b]
    }

    public func setStrokeColor(_ r: Float, _ g: Float, _ b: Float) {
        self.strokeColor = [r, g, b]
    }

    public func setStrokeColor(_ rgbColor: [Float]) {
        self.strokeColor = rgbColor
    }

    public func getStrokeColor() -> [Float] {
        return self.strokeColor
    }

    public func setLineWidth(_ width: Float) {
        self.strokeWidth = width
    }

    func setStrokeWidth(_ strokeWidth: Float) {
        self.strokeWidth = strokeWidth
    }

    func getStrokeWidth() -> Float {
        return self.strokeWidth
    }

    func setProperties(_ properties: UInt32) {
        self.properties = properties
    }

    func getProperties() -> UInt32 {
        return self.properties
    }

    /**
     * Sets the column span private variable.
     *
     * @param colspan the specified column span value.
     */
    public func setColSpan(_ colspan: UInt32) {
        self.properties &= 0x00FF0000
        self.properties |= (colspan & 0x0000FFFF)
    }

    /**
     * Returns the column span private variable value.
     *
     * @return the column span value.
     */
    public func getColSpan() -> UInt32 {
        return (self.properties & 0x0000FFFF)
    }

    /**
     * Sets the cell border object.
     *
     * @param border the border object.
     */
    public func setBorder(_ border: UInt32, _ visible: Bool) {
        if visible {
            self.properties |= border
        } else {
            self.properties &= (~border & 0x00FFFFFF)
        }
    }

    /**
     * Returns the cell border object.
     *
     * @return the cell border object.
     */
    public func getBorder(_ border: UInt32) -> Bool {
        return (self.properties & border) != 0
    }

    /**
     * Sets all cell borders.
     */
    public func setBorders(_ borders: Bool) {
        if borders {
            self.properties &= 0x00FFFFFF
        } else {
            self.properties &= 0x00F0FFFF
        }
    }

    /**
     * Sets the cell text alignment.
     *
     * @param alignment the alignment code.
     * Supported values: Align.LEFT, Align.RIGHT and Align.CENTER.
     */
    public func setTextAlignment(_ alignment: UInt32) {
        self.properties &= 0x00CFFFFF
        self.properties |= (alignment & 0x00300000)
    }

    /**
     * Returns the text alignment.
     *
     * @return the text horizontal alignment code.
     */
    public func getTextAlignment() -> UInt32{
        return (self.properties & 0x00300000)
    }

    /**
     * Sets the cell text vertical alignment.
     *
     * @param alignment the alignment code.
     * Supported values: Align.TOP, Align.CENTER and Align.BOTTOM.
     */
    public func setVerTextAlignment(_ alignment: UInt32) {
        self.valign = alignment
    }

    /**
     * Returns the cell text vertical alignment.
     *
     * @return the vertical alignment code.
     */
    public func getVerTextAlignment() -> UInt32 {
        return self.valign
    }

    /**
     * Sets the underline text parameter.
     * If the value of the underline variable is 'true' - the text is underlined.
     *
     * @param underline the underline text parameter.
     */
    public func setUnderline(_ underline: Bool) {
        if underline {
            self.properties |= 0x00400000
        } else {
            self.properties &= 0x00BFFFFF
        }
    }

    /**
     * Returns the underline text parameter.
     *
     * @return the underline text parameter.
     */
    public func getUnderline() -> Bool {
        return (properties & 0x00400000) != 0
    }

    /**
     * Sets the strikeout text parameter.
     *
     * @param strikeout the strikeout text parameter.
     */
    public func setStrikeout(_ strikeout: Bool) {
        if strikeout {
            self.properties |= 0x00800000
        } else {
            self.properties &= 0x007FFFFF
        }
    }

    /**
     * Returns the strikeout text parameter.
     *
     * @return the strikeout text parameter.
     */
    public func getStrikeout() -> Bool{
        return (properties & 0x00800000) != 0
    }

    public func setURIAction(_ uri: String) {
        self.uri = uri
    }

    /**
     * Draws the point, text and borders of this cell.
     */
    func drawOn(
            _ page: Page,
            _ x: Float,
            _ y: Float,
            _ w: Float,
            _ h: Float) {
        // if backgroundColor != nil {
            drawBackground(page, x, y, w, h)
        // }

        if textBox != nil {
            textBox!.setLocation(x + leftPadding, y + topPadding)
            textBox!.setWidth(w - (leftPadding + rightPadding))
            textBox!.drawOn(page)
        } else if image != nil {
            if (getTextAlignment() == Align.LEFT) {
                image!.setLocation(x + leftPadding, y + topPadding)
                image!.drawOn(page)
            } else if (getTextAlignment() == Align.CENTER) {
                image!.setLocation((x + w/2.0) - image!.getWidth()/2.0, y + topPadding)
                image!.drawOn(page)
            } else if (getTextAlignment() == Align.RIGHT) {
                image!.setLocation((x + w) - (image!.getWidth() + leftPadding), y + topPadding)
                image!.drawOn(page)
            }
        } else if barcode != nil {
            if (getTextAlignment() == Align.LEFT) {
                barcode!.drawOnPageAtLocation(page, x + leftPadding, y + topPadding)
            } else if (getTextAlignment() == Align.CENTER) {
                let barcodeWidth = barcode!.drawOn(nil)[0]
                barcode!.drawOnPageAtLocation(page, (x + w/2.0) - barcodeWidth/2.0, y + topPadding)
            } else if (getTextAlignment() == Align.RIGHT) {
                let barcodeWidth = barcode!.drawOn(nil)[0]
                barcode!.drawOnPageAtLocation(page, (x + w) - (barcodeWidth + leftPadding), y + topPadding)
            }
        } else if text != nil && text != "" {
            drawText(page, x, y, w, h)
        }

        drawBorders(page, x, y, w, h)
        if point != nil {
            if point!.align == Align.LEFT {
                point!.x = x + 2*point!.r
            } else if point!.align == Align.RIGHT {
                point!.x = (x + w) - self.rightPadding/2
            }
            point!.y = y + h/2
            page.setBrushColor(point!.getColor())
            if point!.getURIAction() != nil {
                page.addAnnotation(Annotation(
                        point!.getURIAction(),
                        nil,
                        point!.x - point!.r,
                        point!.y - point!.r,
                        point!.x + point!.r,
                        point!.y + point!.r,
                        "",
                        nil,
                        nil))
            }
            page.drawPoint(point!)
        }
    }

    private func drawBackground(
            _ page: Page,
            _ x: Float,
            _ y: Float,
            _ cellW: Float,
            _ cellH: Float) {
        page.setBrushColor(backgroundColor)
        page.fillRect(x, y + strokeWidth / 2, cellW, cellH + strokeWidth)
    }

    private func drawBorders(
            _ page: Page,
            _ x: Float,
            _ y: Float,
            _ cellW: Float,
            _ cellH: Float) {
        page.setPenColor(strokeColor)
        page.setPenWidth(strokeWidth)
        let qWidth: Float = strokeWidth / 4.0
        if getBorder(Border.TOP) {
            // page.addBMC(StructElem.P, Single.space, Single.space)
            page.moveTo(x - qWidth, y)
            page.lineTo(x + cellW, y)
            page.strokePath()
            // page.addEMC()
        }
        if getBorder(Border.BOTTOM) {
            // page.addBMC(StructElem.P, Single.space, Single.space)
            page.moveTo(x - qWidth, y + cellH)
            page.lineTo(x + cellW, y + cellH)
            page.strokePath()
            // page.addEMC()
        }
        if getBorder(Border.LEFT) {
            // page.addBMC(StructElem.P, Single.space, Single.space)
            page.moveTo(x, y - qWidth)
            page.lineTo(x, y + cellH + qWidth)
            page.strokePath()
            // page.addEMC()
        }
        if getBorder(Border.RIGHT) {
            // page.addBMC(StructElem.P, Single.space, Single.space)
            page.moveTo(x + cellW, y - qWidth)
            page.lineTo(x + cellW, y + cellH + qWidth)
            page.strokePath()
            // page.addEMC()
        }
    }

    private func drawText(
            _ page: Page,
            _ x: Float,
            _ y: Float,
            _ cellW: Float,
            _ cellH: Float) {

        var xText: Float?
        var yText: Float?
        if valign == Align.TOP {
            yText = y + font!.ascent + self.topPadding
        } else if valign == Align.CENTER {
            yText = y + cellH/2 + font!.ascent/2
        } else if valign == Align.BOTTOM {
            yText = (y + cellH) - self.bottomPadding
        } else {
            Swift.print("Invalid vertical text alignment option.")
        }

        page.setPenColor(strokeColor)
        if getTextAlignment() == Align.RIGHT {
            if compositeTextLine == nil {
                xText = (x + cellW) - (font!.stringWidth(text) + self.rightPadding)
                page.addBMC(StructElem.P, text!, text!)
                page.drawString(font!, fallbackFont, font!.size, text!, xText!, yText!, textColor, nil) // TODO
                page.addEMC()
                if getUnderline() {
                    underlineText(page, font!, text!, xText!, yText!)
                }
                if getStrikeout() {
                    strikeoutText(page, font!, text!, xText!, yText!)
                }
            } else {
                xText = (x + cellW) - (compositeTextLine!.getWidth() + self.rightPadding)
                compositeTextLine!.setLocation(xText!, yText!)
                page.addBMC(StructElem.P, text!, text!)
                compositeTextLine!.drawOn(page)
                page.addEMC()
            }
        } else if getTextAlignment() == Align.CENTER {
            if compositeTextLine == nil {
                xText = x + self.leftPadding +
                        (((cellW - (leftPadding + rightPadding)) - font!.stringWidth(text)) / 2)
                page.addBMC(StructElem.P, text!, text!)
                page.drawString(font!, fallbackFont, font!.size, text!, xText!, yText!, textColor, nil) // TODO
                page.addEMC()
                if getUnderline() {
                    underlineText(page, font!, text!, xText!, yText!)
                }
                if getStrikeout() {
                    strikeoutText(page, font!, text!, xText!, yText!)
                }
            } else {
                xText = x + self.leftPadding +
                        (((cellW - (leftPadding + rightPadding)) - compositeTextLine!.getWidth()) / 2)
                compositeTextLine!.setLocation(xText!, yText!)
                page.addBMC(StructElem.P, text!, text!)
                compositeTextLine!.drawOn(page)
                page.addEMC()
            }
        } else if getTextAlignment() == Align.LEFT {
            xText = x + self.leftPadding
            if compositeTextLine == nil {
                page.addBMC(StructElem.P, text!, text!)
                page.drawString(font!, fallbackFont, font!.size, text!, xText!, yText!, textColor, nil) // TODO
                page.addEMC()
                if getUnderline() {
                    underlineText(page, font!, text!, xText!, yText!)
                }
                if getStrikeout() {
                    strikeoutText(page, font!, text!, xText!, yText!)
                }
            } else {
                compositeTextLine!.setLocation(xText!, yText!)
                page.addBMC(StructElem.P, text!, text!)
                compositeTextLine!.drawOn(page)
                page.addEMC()
            }
        } else {
            print("Invalid Text Alignment!")
        }

        if uri != nil {
            let w = (compositeTextLine != nil) ?
                    compositeTextLine!.getWidth() : font!.stringWidth(text)
            page.addAnnotation(Annotation(
                    uri!,
                    "",
                    xText!,
                    (page.height - yText!) - font!.ascent,
                    xText! + w,
                    (page.height - yText!) + font!.descent,
                    "",
                    nil,
                    nil))
        }
    }

    private func underlineText(
            _ page: Page, _ font: Font, _ text: String, _ x: Float, _ y: Float) {
        page.addBMC(StructElem.P, "underline", "underline")
        page.setPenWidth(font.underlineThickness)
        page.moveTo(x, y + font.descent)
        page.lineTo(x + font.stringWidth(text), y + font.descent)
        page.strokePath()
        page.addEMC()
    }

    private func strikeoutText(
            _ page: Page, _ font: Font, _ text: String, _ x: Float, _ y: Float) {
        page.addBMC(StructElem.P, "strike out", "strike out")
        page.setPenWidth(font.underlineThickness)
        page.moveTo(x, y - font.getAscent()/3.0)
        page.lineTo(x + font.stringWidth(text), y - font.getAscent()/3.0)
        page.strokePath()
        page.addEMC()
    }

    public func getTextBox() -> TextBox? {
        return textBox
    }
}   // End of Cell.swift
