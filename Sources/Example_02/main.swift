import Foundation
import PDFjet

/**
 * Example_02.swift
 */
public class Example_02 {
    public init() throws {
        let stream = OutputStream(toFileAtPath: "Example_02.pdf", append: false)
        let pdf = PDF(stream!)

        let font1 = try Font(pdf, "fonts/NotoSansJP/NotoSansJP-Regular.ttf.stream")
        font1.setSize(12.0)

        let font2 = try Font(pdf, "fonts/NotoSansKR/NotoSansKR-Regular.ttf.stream")
        font2.setSize(12.0)

        let page = Page(pdf, Letter.PORTRAIT)

        var text = try String(contentsOfFile: "data/languages/japanese.txt", encoding: .utf8)
        var textBlock = TextBlock(font1, text)
        textBlock.setLocation(50.0, 50.0)
        textBlock.setWidth(415.0)
        textBlock.drawOn(page)

        text = try String(contentsOfFile: "data/languages/korean.txt", encoding: .utf8)
        textBlock = TextBlock(font2, text)
        textBlock.setLocation(50.0, 450.0)
        textBlock.setWidth(415.0)
        textBlock.drawOn(page)

        pdf.complete()
    }
}   // End of Example_02.swift

let time0 = Int64(Date().timeIntervalSince1970 * 1000)
_ = try Example_02()
let time1 = Int64(Date().timeIntervalSince1970 * 1000)
TextUtils.printDuration("Example_02", time0, time1)
