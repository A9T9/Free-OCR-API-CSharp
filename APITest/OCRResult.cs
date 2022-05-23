namespace OCRAPITest
{

    public class RootObject
    {
        public ParsedResult[] ParsedResults { get; set; }
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string[] ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class ParsedResult
    {
        public object FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

}
