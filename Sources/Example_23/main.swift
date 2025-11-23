import Foundation
import PDFjet

/**
 * Example_23.swift
 */
public class Example_23 {
    public init() throws {
        let pdf = PDF(OutputStream(toFileAtPath: "Example_23.pdf", append: false)!)

        let f1 = try Font(pdf, IBMPlexSans.Regular)
        f1.setSize(72.0)

        let f2 = Font(pdf, CoreFont.HELVETICA)
        f2.setSize(24.0)

        let page = Page(pdf, Letter.PORTRAIT)

        let x1: Float = 90.0
        let y1: Float = 50.0

        let textLine = TextLine(f2, "(x1, y1)")
        textLine.setLocation(x1, y1 - 15.0)
        textLine.drawOn(page)

        let textBlock = TextBlock(f1,
            "Heya, World! This is a test to show the functionality of a TextBlock.")
        textBlock.setLocation(x1, y1)
        textBlock.setWidth(500.0)
        textBlock.setBorderColor(Color.lightgreen)
        textBlock.setFillColor(Color.lightgreen)
        textBlock.setTextColor(Color.black)
        let xy = textBlock.drawOn(page)

        let x2 = xy[0]

        f2.setSize(18.0)

        // Text on the left
        let ascentText = TextLine(f2, "Ascent")
        ascentText.setLocation(x1 - 85.0, y1 + 40.0)
        ascentText.drawOn(page)

        let descentText = TextLine(f2, "Descent")
        descentText.setLocation(x1 - 85.0, y1 + f1.getAscent() + 15.0)
        descentText.drawOn(page)

        // Line beside the text ascent
        let blueLine = Line(
            x1 - 10.0,
            y1,
            x1 - 10.0,
            y1 + f1.getAscent())
        blueLine.setColor(Color.blue)
        blueLine.setWidth(3.0)
        blueLine.drawOn(page)

        // Line beside the text descent
        let redLine = Line(
            x1 - 10.0,
            y1 + f1.getAscent(),
            x1 - 10.0,
            y1 + f1.getAscent() + f1.getDescent())
        redLine.setColor(Color.red)
        redLine.setWidth(3.0)
        redLine.drawOn(page)

        // Lines for first line of text
        let text_line1 = Line(
                x1,
                y1 + f1.getAscent(),
                x2,
                y1 + f1.getAscent())
        text_line1.drawOn(page)

        let descent_line1 = Line(
                x1,
                y1 + (f1.getAscent() + f1.getDescent()),
                x2,
                y1 + (f1.getAscent() + f1.getDescent()))
        descent_line1.drawOn(page)

        // Lines for second line of text
        let curr_y = y1 + f1.getBodyHeight()

        let text_line2 = Line(
                x1,
                curr_y + f1.getAscent(),
                x2,
                curr_y + f1.getAscent())
        text_line2.drawOn(page)

        let descent_line2 = Line(
                x1,
                curr_y + f1.getAscent() + f1.getDescent(),
                x2,
                curr_y + f1.getAscent() + f1.getDescent())
        descent_line2.drawOn(page)

        let p1 = Point(x1, y1)
        p1.setRadius(5.0)
        p1.drawOn(page)

        let p2 = Point(xy[0], xy[1])
        p2.setRadius(5.0)
        p2.drawOn(page)

        f2.setSize(24.0)
        let textLine2 = TextLine(f2, "(x2, y2)")
        textLine2.setLocation(xy[0] - 80.0, xy[1] + 30.0)
        textLine2.drawOn(page)

        let box = Box()
        box.setLocation(xy[0], xy[1])
        box.setSize(20.0, 20.0)
        box.drawOn(page)

        pdf.complete()
    }
}   // End of Example_23.swift

let time0 = Int64(Date().timeIntervalSince1970 * 1000)
_ = try Example_23()
let time1 = Int64(Date().timeIntervalSince1970 * 1000)
TextUtils.printDuration("Example_23", time0, time1)
