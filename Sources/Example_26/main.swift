import Foundation
import PDFjet

/**
 * Example_26.swift
 */
public class Example_26 {
    public init() {
        let pdf = PDF(OutputStream(toFileAtPath: "Example_26.pdf", append: false)!)

        let f1 = Font(pdf, CoreFont.HELVETICA_BOLD)

        let page = Page(pdf, Letter.PORTRAIT)

        let x: Float = 50.0
        var y: Float = 50.0

        CheckBox(f1, "Hello")
                .setFontSize(12.0)
                .setLocation(x, y)
                .setCheckmark(Color.blue)
                .check(Mark.CHECK)
                .drawOn(page)

        y += 30.0
        CheckBox(f1, "World!")
                .setFontSize(12.0)
                .setLocation(x, y)
                .setCheckmark(Color.blue)
                .setURIAction("http://pdfjet.com")
                .check(Mark.CHECK)
                .drawOn(page)

        y += 30.0
        CheckBox(f1, "This is a test.")
                .setFontSize(12.0)
                .setLocation(x, y)
                .setURIAction("http://pdfjet.com")
                .drawOn(page)

        y += 30.0
        RadioButton(f1, "Hello, World!")
                .setFontSize(12.0)
                .setLocation(x, y)
                .select(true)
                .drawOn(page)

        var xy = (RadioButton(f1, "Yes"))
                .setFontSize(12.0)
                .setLocation(x + 100.0, 50.0)
                .setURIAction("http://pdfjet.com")
                .select(true)
                .drawOn(page)

        xy = (RadioButton(f1, "No"))
                .setFontSize(12.0)
                .setLocation(xy[0], 50.0)
                .drawOn(page)

        xy = (CheckBox(f1, "Hello"))
                .setFontSize(12.0)
                .setLocation(xy[0], 50.0)
                .setCheckmark(Color.blue)
                .check(Mark.X)
                .drawOn(page)

        xy = (CheckBox(f1, "Yahoo")
                .setFontSize(12.0)
                .setLocation(xy[0], 50.0)
                .setCheckmark(Color.blue)
                .check(Mark.CHECK)
                .drawOn(page))

        let box = Box()
        box.setLocation(xy[0], xy[1])
        box.setSize(20.0, 20.0)
        box.drawOn(page)

        pdf.complete()
    }
}   // End of Example_26.swift

let time0 = Int64(Date().timeIntervalSince1970 * 1000)
_ = Example_26()
let time1 = Int64(Date().timeIntervalSince1970 * 1000)
TextUtils.printDuration("Example_26", time0, time1)
