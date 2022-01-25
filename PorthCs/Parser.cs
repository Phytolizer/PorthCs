using System.Diagnostics;

namespace PorthCs;

internal static class Parser
{
    public static Op ParseTokenAsOp(Token tok)
    {
        Debug.Assert((int)OpCode.Count == 5, "OpCodes are not exhaustively handled in Parser.ParseTokenAsOp.");
        switch (tok.Word)
        {
            case "+":
                return Ops.Plus();
            case "-":
                return Ops.Minus();
            case ".":
                return Ops.Dump();
            case "=":
                return Ops.Equal();
            default:
                if (ulong.TryParse(tok.Word, out var value))
                {
                    return Ops.Push(value);
                }
                else
                {
                    throw new ParserError(tok, "Unknown token");
                }
        }
    }
}