namespace PorthCs;

internal class Op
{
    public OpCode Code { get; }
    public Token Token { get; }

    public Op(OpCode code, Token token)
    {
        Code = code;
        Token = token;
    }

    public override string ToString()
    {
        return Code.ToString();
    }
}
