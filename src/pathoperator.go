package pdfjet

// PathOperator contains constants for path operations
var PathOperator = struct {
	Stroke                        string // Stroke the path
	CloseAndStroke                string // Close and then stroke the path
	Fill                          string // Close and fill the path
	FillAndStroke                 string // Close, fill and then stroke the path
	FillUsingEvenOddRule          string // Like 'f' but using even odd rule
	FillUsingEvenOddRuleAndStroke string // Like 'b' but using even odd rule
}{
	Stroke:                        "S",
	CloseAndStroke:                "s",
	Fill:                          "f",
	FillAndStroke:                 "b",
	FillUsingEvenOddRule:          "f*",
	FillUsingEvenOddRuleAndStroke: "b*",
}
