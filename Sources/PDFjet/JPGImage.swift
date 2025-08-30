/**
 * JPGImage.swift
 *
 * Copyright (c) 2025 PDFjet Software
 * Licensed under the MIT License. See LICENSE file in the project root.
 */
import Foundation

///
/// Used to embed JPG images in the PDF document.
///
class JPGImage {

    let M_SOF0: UInt8  = 0xC0       // Start Of Frame N
    let M_SOF1: UInt8  = 0xC1       // N indicates which compression process
    let M_SOF2: UInt8  = 0xC2       // Only SOF0-SOF2 are now in common use
    let M_SOF3: UInt8  = 0xC3
    let M_SOF5: UInt8  = 0xC5       // NB: codes C4 and CC are NOT SOF markers
    let M_SOF6: UInt8  = 0xC6
    let M_SOF7: UInt8  = 0xC7
    let M_SOF9: UInt8  = 0xC9
    let M_SOF10: UInt8 = 0xCA
    let M_SOF11: UInt8 = 0xCB
    let M_SOF13: UInt8 = 0xCD
    let M_SOF14: UInt8 = 0xCE
    let M_SOF15: UInt8 = 0xCF

    var width: UInt16 = 0
    var height: UInt16 = 0
    var colorComponents: UInt16 = 0
    var data: [UInt8]
    var index = 0

    public init(_ stream: InputStream) throws {
        self.data = try Content.getFromStream(stream)
        processImage(&data)
    }

    func getWidth() -> UInt16 {
        return self.width
    }

    func getHeight() -> UInt16 {
        return self.height
    }

    func getFileSize() -> Int64 {
        return Int64(self.data.count)
    }

    func getColorComponents() -> UInt16 {
        return self.colorComponents
    }

    func getData() -> [UInt8] {
        return self.data
    }

    private func processImage(_ buffer: inout [UInt8]) {
        if buffer[0] != UInt8(0xFF) || buffer[1] != UInt8(0xD8) {
            Swift.print("Error: Invalid JPEG header.")
        }

        index += 2
        while true {
            let ch = nextMarker(&buffer)
            // Note that marker codes 0xC4, 0xC8, 0xCC are not,
            // and must not be treated as SOFn. C4 in particular
            // is actually DHT.
            if ch == M_SOF0 ||          // Baseline
                    ch == M_SOF1 ||     // Extended sequential, Huffman
                    ch == M_SOF2 ||     // Progressive, Huffman
                    ch == M_SOF3 ||     // Lossless, Huffman
                    ch == M_SOF5 ||     // Differential sequential, Huffman
                    ch == M_SOF6 ||     // Differential progressive, Huffman
                    ch == M_SOF7 ||     // Differential lossless, Huffman
                    ch == M_SOF9 ||     // Extended sequential, arithmetic
                    ch == M_SOF10 ||    // Progressive, arithmetic
                    ch == M_SOF11 ||    // Lossless, arithmetic
                    ch == M_SOF13 ||    // Differential sequential, arithmetic
                    ch == M_SOF14 ||    // Differential progressive, arithmetic
                    ch == M_SOF15 {     // Differential lossless, arithmetic
                // Skip 3 bytes to get to the image height and width
                index += 3
                height = getUInt16(&buffer)
                index += 2
                width = getUInt16(&buffer)
                index += 2
                colorComponents = UInt16(buffer[index])
                break
            } else {
                skipVariable(&buffer)
            }
        }
    }

    private func getUInt16(_ buffer: inout [UInt8]) -> UInt16 {
        return UInt16(buffer[index]) << 8 | UInt16(buffer[index + 1])
    }

    // Find the next JPEG marker and return its marker code.
    // We expect at least one FF byte, possibly more if the compressor
    // used FFs to pad the file.
    // There could also be non-FF garbage between markers. The treatment
    // of such garbage is unspecified; we choose to skip over it but
    // emit a warning msg.
    // NB: this routine must not be used after seeing SOS marker, since
    // it will not deal correctly with FF/00 sequences in the compressed
    // image data...
    private func nextMarker(_ buffer: inout [UInt8]) -> UInt8 {
        // Find 0xFF byte; count and skip any non-FFs.
        var ch = buffer[index]
        index += 1
        if ch != UInt8(0xFF) {
            Swift.print("0xFF byte expected.")
        }

        // Get marker code byte, swallowing any duplicate FF bytes.
        // Extra FFs are legal as pad bytes, so don't count them in discarded_bytes.
        repeat {
            ch = buffer[index]
            index += 1
        } while ch == UInt8(0xFF)

        return ch
    }

    // Most types of marker are followed by a variable-length parameter
    // segment. This routine skips over the parameters for any marker we
    // don't otherwise want to process.
    // Note that we MUST skip the parameter segment explicitly in order
    // not to be fooled by 0xFF bytes that might appear within the
    // parameter segment such bytes do NOT introduce new markers.
    private func skipVariable(_ buffer: inout [UInt8]) {
        // Get the marker parameter length count
        var length = getUInt16(&buffer)
        index += 2
        if length < 2 {
            // Length includes itself, so must be at least 2
            Swift.print("Error: length must be at least 2.")
        }
        length -= 2

        // Skip over the remaining bytes
        while length > 0 {
            index += 1
            length -= 1
        }
    }
}   // End of JPGImage.swift
