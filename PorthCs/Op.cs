namespace PorthCs;

internal class Op
{
    public OpCode Code { get; }
    public string FilePath { get; }
    public int LineNumber { get; }
    public int ColumnNumber { get; }

    public Op(OpCode code, Token t)
    {
        Code = code;
        FilePath = t.FilePath;
        LineNumber = t.LineNumber;
        ColumnNumber = t.ColumnNumber;
    }

    public override string ToString()
    {
        return Code.ToString();
    }
}
