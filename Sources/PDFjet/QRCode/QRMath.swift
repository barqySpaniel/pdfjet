/**
 *  QRMath.swift
 *
 *  Copyright (c) 2025 PDFjet Software
 *  Licensed under the MIT License. See LICENSE file in the project root.
 *
 *  Original author: Kazuhiko Arase, 2009
 *  URL: http://www.d-project.com/
 *  Licensed under MIT: http://www.opensource.org/licenses/mit-license.php
 *
 *  The word "QR Code" is a registered trademark of
 *  DENSO WAVE INCORPORATED
 *  http://www.denso-wave.com/qrcode/faqpatent-e.html
 *
 *  Modified and adapted for use in PDFjet by PDFjet Software
 */
import Foundation

class QRMath {
    static let singleton = QRMath()

    private static var EXP_TABLE = [Int](repeating: 0, count: 256)
    private static var LOG_TABLE = [Int](repeating: 0, count: 256)

    private init() {
        for i in 0..<8 {
            QRMath.EXP_TABLE[i] = (1 << i)
        }
        for i in 8..<256 {
            QRMath.EXP_TABLE[i] =
                    QRMath.EXP_TABLE[i - 4] ^
                    QRMath.EXP_TABLE[i - 5] ^
                    QRMath.EXP_TABLE[i - 6] ^
                    QRMath.EXP_TABLE[i - 8]
        }
        for i in 0..<255 {
            QRMath.LOG_TABLE[QRMath.EXP_TABLE[i]] = i
        }
    }

    public func glog(_ n: Int) -> Int {
        if n < 1 {
            Swift.print("log(" + String(describing: n) + ")")
        }
        return QRMath.LOG_TABLE[n]
    }

    public func gexp(_ i: Int) -> Int {
        var n = i
        while n < 0 {
            n += 255
        }
        while n >= 256 {
            n -= 255
        }
        return QRMath.EXP_TABLE[n]
    }
}
