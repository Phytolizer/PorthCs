using System.Diagnostics;

namespace PorthCs;

internal static class SemanticAnalyzer
{
    public static IEnumerable<Op> CrossReferenceBlocks(IList<Op> program)
    {
        var stack = new Stack<int>();
        Debug.Assert((int)OpCode.Count == 7, "OpCodes are not exhaustively handled in Parser.CrossReferenceBlocks.");
        for (var ip = 0; ip < program.Count; ++ip)
        {
            var op = program[ip];
            switch (op.Code)
            {
                case OpCode.If:
                    stack.Push(ip);
                    yield return op;
                    break;
                case OpCode.End:
                {
                    var ifIp = stack.Pop();
                    if (program[ifIp].Code != OpCode.If)
                    {
                        throw new SemanticError(op, "Mismatched 'end'.");
                    }

                    yield return new IntegerOp(op, (ulong)ip);
                    break;
                }
                case OpCode.Push:
                case OpCode.Plus:
                case OpCode.Minus:
                case OpCode.Equal:
                case OpCode.Dump:
                    yield return op;
                    break;
                case OpCode.Count:
                    Debug.Fail("This is unreachable.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }
    }
}

internal class SemanticError : Exception
{
    private readonly Op _op;
    private readonly string _message;

    public SemanticError(Op op, string message)
    {
        _op = op;
        _message = message;
    }

    public override string Message => $"{_op.FilePath}:{_op.LineNumber}:{_op.ColumnNumber}: {_message}";
}