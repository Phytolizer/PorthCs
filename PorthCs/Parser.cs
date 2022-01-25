using System.Diagnostics;

namespace PorthCs;

internal static class Parser
{
    public static Op ParseTokenAsOp(Token tok)
    {
        Debug.Assert((int)OpCode.Count == 7, "OpCodes are not exhaustively handled in Parser.ParseTokenAsOp.");
        switch (tok.Word)
        {
            case "+":
                return Ops.Plus(tok);
            case "-":
                return Ops.Minus(tok);
            case ".":
                return Ops.Dump(tok);
            case "=":
                return Ops.Equal(tok);
            case "if":
                return Ops.If(tok);
            case "end":
                return Ops.End(tok);
            default:
                if (ulong.TryParse(tok.Word, out var value))
                {
                    return Ops.Push(tok, value);
                }
                else
                {
                    throw new ParserError(tok, "Unknown token");
                }
        }
    }
}