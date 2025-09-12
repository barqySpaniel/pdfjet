import Foundation

/**
 * Rect.swift
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

public class Rect : Drawable {
    private var x: Float = 0.0
    private var y: Float = 0.0
    private var w: Float = 0.0
    private var h: Float = 0.0
    private var r: Float = 0.0
    private var color: Int32
    private var width: Float
    private var pattern: String
    private var fillShape: Bool = false
    private var uri: String?
    private var key: String?
    private var language: String?
    private var altDescription: String
    private var actualText: String
    private var structureType: String

    /**
     * Creates new Rect object.
     */
    public init() {
        self.color = Color.black
        self.width = 0.0
        self.pattern = "[] 0"
        self.altDescription = Single.space
        self.actualText = Single.space
        self.structureType = "P" // StructureType.P; TODO
    }

    /**
     * Creates a rect object.
     * - Parameters:
     *   - x: the x coordinate of the top left corner of this rect when drawn on the page.
     *   - y: the y coordinate of the top left corner of this rect when drawn on the page.
     *   - w: the width of this rect.
     *   - h: the height of this rect.
     */
    public convenience init(_ x: Float, _ y: Float, _ w: Float, _ h: Float) {
        self.init()
        self.x = x
        self.y = y
        self.w = w
        self.h = h
    }

    /**
     * Sets the location of this rect on the page.
     * - Parameters:
     *   - x: the x coordinate of the top left corner of this rect when drawn on the page.
     *   - y: the y coordinate of the top left corner of this rect when drawn on the page.
     * - Returns: this Rect.
     */
    @discardableResult
    public func setLocation(_ x: Float, _ y: Float) -> Rect {
        self.x = x
        self.y = y
        return self
    }

    /**
     * Sets the position of this rect on the page. Required by the Drawable interface.
     * - Parameters:
     *   - x: the x coordinate of the top left corner of this rect when drawn on the page.
     *   - y: the y coordinate of the top left corner of this rect when drawn on the page.
     * - Returns: this Rect.
     */
    public func setPosition(_ x: Float, _ y: Float) {
        setLocation(x, y)
    }

    /**
     * Sets the size of this rect.
     * - Parameters:
     *   - w: the width of this rect.
     *   - h: the height of this rect.
     */
    public func setSize(_ w: Float, _ h: Float) {
        self.w = w
        self.h = h
    }

    /**
     * Sets the color for this rect.
     * - Parameter color: the color specified as an integer.
     */
    public func setBorderColor(_ color: Int32) {
        self.color = color
    }

    /**
     * Sets the width of this line.
     * - Parameter width: the width.
     */
    public func setLineWidth(_ width: Float) {
        self.width = width
    }

    /**
     * Sets the corner radius.
     * - Parameter r: the radius.
     */
    public func setCornerRadius(_ r: Float) {
        self.r = r
    }

    /**
     * Sets the URI for the "click rect" action.
     * - Parameter uri: the URI
     */
    public func setURIAction(_ uri: String) {
        self.uri = uri
    }

    /**
     * Sets the destination key for the action.
     * - Parameter key: the destination name.
     */
    public func setGoToAction(_ key: String) {
        self.key = key
    }

    /**
     * Sets the alternate description of this rect.
     * - Parameter altDescription: the alternate description of the rect.
     * - Returns: this Rect.
     */
    @discardableResult
    public func setAltDescription(_ altDescription: String) -> Rect {
        self.altDescription = altDescription
        return self
    }

    /**
     * Sets the actual text for this rect.
     * - Parameter actualText: the actual text for the rect.
     * - Returns: this Rect.
     */
    @discardableResult
    public func setActualText(_ actualText: String) -> Rect {
        self.actualText = actualText
        return self
    }

    /**
     * Sets the type of the structure.
     * - Parameter structureType: the structure type.
     * - Returns: this Rect.
     */
    @discardableResult
    public func setStructureType(_ structureType: String) -> Rect {
        self.structureType = structureType
        return self
    }

    /**
     * Sets the line dash pattern that controls the pattern of dashes and gaps used to stroke paths.
     * - Parameter pattern: the line dash pattern.
     */
    public func setPattern(_ pattern: String) {
        self.pattern = pattern
    }

    /**
     * Sets the private fillShape variable.
     * If the value of fillShape is true - the rect is filled with the current brush color.
     * - Parameter fillShape: the value used to set the private fillShape variable.
     */
    public func setFillShape(_ fillShape: Bool) {
        self.fillShape = fillShape
    }

    /**
     * Places this rect in the another rect.
     * - Parameters:
     *   - rect: the other rect.
     *   - xOffset: the x offset from the top left corner of the rect.
     *   - yOffset: the y offset from the top left corner of the rect.
     */
    public func placeIn(rect: Rect, xOffset: Float, yOffset: Float) {
        self.x = rect.x + xOffset
        self.y = rect.y + yOffset
    }

    /**
     * Scales this rect by the specified factor.
     * - Parameter factor: the factor used to scale the rect.
     */
    public func scaleBy(_ factor: Float) {
        self.x *= factor
        self.y *= factor
    }

    /**
     * Draws this rect on the specified page.
     * - Parameter page: the page to draw this rect on.
     * - Throws: an exception if drawing fails.
     * - Returns: x and y coordinates of the bottom right corner of this component.
     */
    @discardableResult
    public func drawOn(_ page: Page?) -> [Float] {
        let k: Float = 0.5517

        page!.addBMC(
                self.structureType,
                self.language,
                self.actualText,
                self.altDescription)

        if self.r == 0.0 {
            page!.moveTo(self.x, self.y)
            page!.lineTo(self.x + self.w, self.y)
            page!.lineTo(self.x + self.w, self.y + self.h)
            page!.lineTo(self.x, self.y + self.h)
            if self.fillShape {
                page!.setBrushColor(self.color)
                page!.fillPath()
            } else {
                page!.setPenWidth(self.width)
                page!.setPenColor(self.color)
                page!.setLinePattern(self.pattern)
                page!.closePath()
            }
        } else {
            page!.setPenWidth(self.width)
            page!.setPenColor(self.color)
            page!.setLinePattern(self.pattern)

            var points: [Point] = []
            points.append(Point(self.x + self.r, self.y, false))
            points.append(Point((self.x + self.w) - self.r, self.y, false))
            points.append(Point((self.x + self.w - self.r) + self.r * k, self.y, true))
            points.append(Point(self.x + self.w, (self.y + self.r) - self.r * k, true))
            points.append(Point(self.x + self.w, self.y + self.r, false))
            points.append(Point(self.x + self.w, (self.y + self.h) - self.r, false))
            points.append(Point(self.x + self.w, ((self.y + self.h) - self.r) + self.r * k, true))
            points.append(Point(((self.x + self.w) - self.r) + self.r * k, self.y + self.h, true))
            points.append(Point(((self.x + self.w) - self.r), self.y + self.h, false))
            points.append(Point(self.x + self.r, self.y + self.h, false))
            points.append(Point(((self.x + self.r) - self.r * k), self.y + self.h, true))
            points.append(Point(self.x, ((self.y + self.h) - self.r) + self.r * k, true))
            points.append(Point(self.x, (self.y + self.h) - self.r, false))
            points.append(Point(self.x, self.y + self.r, false))
            points.append(Point(self.x, (self.y + self.r) - self.r * k, true))
            points.append(Point((self.x + self.r) - self.r * k, self.y, true))
            points.append(Point(self.x + self.r, self.y, false))

            page!.drawPath(points, PathOperator.stroke)
        }
        page!.addEMC()

        if self.uri != nil || self.key != nil {
            page!.addAnnotation(Annotation(
                self.uri,
                self.key,
                self.x,
                self.y,
                self.x + self.w,
                self.y + self.h,
                self.language,
                self.actualText,
                self.altDescription))
        }

        return [self.x + self.w, self.y + self.h]
    }
}
