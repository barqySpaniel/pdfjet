namespace PDFjet.NET {
public static class PathOperator {
    public static readonly string Stroke = "S";                         // Stroke the path
    public static readonly string CloseAndStroke = "s";                 // Close and then stroke the path
    public static readonly string Fill = "f";                           // Close ant fill the path
    public static readonly string FillAndStroke = "b";                  // Close, fill and then stroke the path
    public static readonly string FillUsingEvenOddRule = "f*";          // Like 'f' but using even odd rule
    public static readonly string FillUsingEvenOddRuleAndStroke = "b*"; // Like 'b' but using even odd rule
}
}
