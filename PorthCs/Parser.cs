using System.Diagnostics;

namespace PorthCs;

internal static class Parser
{
    public static Op ParseWordAsOp(string word)
    {
        Debug.Assert((int)OpCode.Count == 4, "OpCodes are not exhaustively handled in Parser.ParseWordAsOp.");
        return word switch
        {
            "+" => Ops.Plus(),
            "-" => Ops.Minus(),
            "." => Ops.Dump(),
            _ => Ops.Push(ulong.Parse(word))
        };
    }
}