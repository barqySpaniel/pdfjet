import Foundation

class BufferedOutputStream {
    private let outputStream: OutputStream
    private var buffer = [UInt8]()
    private let bufferSize: Int
    private(set) var byteCount = 0

    /// Initializes with an existing OutputStream (file, socket, etc.)
    init(_ outputStream: OutputStream, bufferSize: Int = 4096) {
        self.outputStream = outputStream
        self.bufferSize = bufferSize
        self.outputStream.open()
    }

    /// Appends full buffer of data
    func write(_ data: [UInt8]) {
        write(data, 0, data.count)
    }

    /// Appends partial buffer of data
    func write(_ data: [UInt8], _ offset: Int, _ length: Int) {
        guard offset >= 0, length >= 0, offset + length <= data.count else {
            fatalError("Invalid offset or length in write()")
        }

        buffer.append(contentsOf: data[offset ..< offset + length])
        byteCount += length

        if buffer.count >= bufferSize {
            flush()
        }
    }

    /// Flush buffer contents to the underlying stream
    func flush() {
        var totalWritten = 0
        while totalWritten < buffer.count {
            let bytesLeft = buffer.count - totalWritten
            let written = buffer.withUnsafeBytes {
                outputStream.write(
                    $0.baseAddress!.advanced(by: totalWritten).assumingMemoryBound(to: UInt8.self),
                    maxLength: bytesLeft)
            }
            if written <= 0 {
                print("Error writing to stream")
                break
            }
            totalWritten += written
        }
        buffer.removeAll(keepingCapacity: true)
    }

    /// Closes the stream
    func close() {
        flush()
        outputStream.close()
    }

    deinit {
        close()
    }
}
