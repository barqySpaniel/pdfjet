import Foundation
import PDFjet

public class Example_01 {
    public init() throws {
        let stream = OutputStream(toFileAtPath: "Example_01.pdf", append: false)
        let pdf = PDF(stream!)

        // Use embedded font reference or load from stream
        let font = try Font(pdf, IBMPlexSans.Regular)

        let page = Page(pdf, Letter.PORTRAIT)

        let englishText = try String(
                contentsOfFile: "data/languages/english.txt", encoding: .utf8)
        let textBlock = TextBlock(font, englishText)
        textBlock.setLocation(50, 50)
        textBlock.setWidth(430)
        textBlock.setTextPadding(10)
        var xy = textBlock.drawOn(page)

        let rect = Rect(xy[0], xy[1], 30, 30)
        rect.setBorderColor(Color.blue)
        rect.drawOn(page)

        let greekText = try String(
                contentsOfFile: "data/languages/greek.txt", encoding: .utf8)
        let textBlock2 = TextBlock(font, greekText)
        textBlock2.setLocation(50, xy[1] + 30)
        textBlock2.setWidth(430)
        textBlock2.setBorderColor(Color.none)
        xy = textBlock2.drawOn(page)

        let bulgarianText = try String(
                contentsOfFile: "data/languages/bulgarian.txt", encoding: .utf8)
        let textBlock3 = TextBlock(font, bulgarianText)
        textBlock3.setLocation(50, xy[1] + 30)
        textBlock3.setWidth(430)
        textBlock3.setTextPadding(10)
        textBlock3.setBorderColor(Color.blue)
        textBlock3.setBorderCornerRadius(10)
        textBlock3.drawOn(page)

        pdf.complete()
    }
}

// Entry point
let time0 = Int64(Date().timeIntervalSince1970 * 1000)
_ = try Example_01()
let time1 = Int64(Date().timeIntervalSince1970 * 1000)
TextUtils.printDuration("Example_01", time0, time1)
