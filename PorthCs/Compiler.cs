using System.Diagnostics;
using System.Text;

namespace PorthCs;

internal static class Compiler
{
    public static void Compile(IEnumerable<Op> program, string outFilePath)
    {
        using var file = File.OpenWrite(outFilePath);
        using var writer = new StreamWriter(file, Encoding.UTF8);

        writer.Write(
            @"
                    fn main() {
                        let mut stack = Vec::<u64>::new();
                ");

        Debug.Assert((int)OpCode.Count == 7, "OpCodes are not exhaustively handled in Compiler.Compile");
        foreach (var op in program)
        {
            writer.WriteLine($"// -- {op.Code} --");
            switch (op.Code)
            {
                case OpCode.Push:
                {
                    var pushOp = (IntegerOp)op;
                    writer.WriteLine($"stack.push({pushOp.Operand});");
                }
                break;
                case OpCode.Plus:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a + b);");
                    break;
                case OpCode.Minus:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a - b);");
                    break;
                case OpCode.Equal:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a == b { 1 } else { 0 });");
                    break;
                case OpCode.Dump:
                    writer.WriteLine(@"println!(""{}"", stack.pop().unwrap());");
                    break;
                case OpCode.If:
                    writer.WriteLine("let condition = stack.pop().unwrap();");
                    writer.WriteLine("if cond == 1 {");
                    break;
                case OpCode.End:
                    writer.WriteLine("}");
                    break;
                case OpCode.Count:
                    Debug.Fail("This is unreachable.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }
        writer.WriteLine("}");

    }
}