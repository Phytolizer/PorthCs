using System.Diagnostics;

namespace PorthCs;

internal static class Simulator
{
    public static void Simulate(IEnumerable<Op> program)
    {
        Debug.Assert((int)OpCode.Count == 5, "OpCodes are not exhaustively handled in Simulator.Simulate.");
        var stack = new Stack<object>();
        foreach (var op in program)
        {
            switch (op.Code)
            {
                case OpCode.Push:
                {
                    var pushOp = (IntegerOp)op;
                    stack.Push(pushOp.Operand);
                    break;
                }
                case OpCode.Plus:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a + b);
                    break;
                }
                case OpCode.Minus:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a - b);
                    break;
                }
                case OpCode.Equal:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a == b ? 1 : 0);
                    break;
                }
                case OpCode.Dump:
                {
                    var a = stack.Pop();
                    Console.WriteLine(a);
                    break;
                }
                case OpCode.Count:
                    Debug.Fail("This is unreachable.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }
    }
}