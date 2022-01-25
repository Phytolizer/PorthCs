namespace PorthCs;

public class Token
{
    public string FilePath { get; init; }
    public int LineNumber { get; init; }
    public int ColumnNumber { get; init; }
    public string Word { get; init; }

    public Token(string filePath, int lineNumber, int columnNumber, string word)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
        Word = word;
    }
}