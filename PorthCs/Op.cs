namespace PorthCs;

internal class Op
{
    public OpCode Code { get; }
    public string FilePath { get; }
    public int LineNumber { get; }
    public int ColumnNumber { get; }

    public Op(OpCode code, string filePath, int lineNumber, int columnNumber)
    {
        Code = code;
        FilePath = filePath;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    public override string ToString() => $"{Code}";
}