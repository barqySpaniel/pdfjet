package compression

type CompressionType int

const (
	DEFLATE = iota
	LZW
	NONE
)
