namespace PorthCs;

internal class ParserError : Exception
{
    private readonly Token _token;
    private readonly string _message;

    public ParserError(Token token, string message)
    {
        _token = token;
        _message = message;
    }

    public override string Message => $"{_token.FilePath}:{_token.LineNumber}:{_token.ColumnNumber}: {_message}";
}