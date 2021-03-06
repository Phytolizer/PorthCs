using System.Text;

namespace PorthCs;

internal static class Compiler
{
    public static void Compile(IList<Op> program, string outFilePath)
    {
        using var file = File.Create(outFilePath);
        using var writer = new StreamWriter(file, Encoding.UTF8);

        writer.Write(
            $@"
                use std::io::Write;

                fn do_syscall(mem: &mut [u8], number: u64, args: Vec<u64>) -> u64 {{
                    match number {{
                        1 => match args[0] {{
                            1 => {{
                                return std::io::stdout().write(&mem[args[1] as usize..args[2] as usize]).unwrap() as u64;
                            }}
                            2 => {{
                                return std::io::stderr().write(&mem[args[1] as usize..args[2] as usize]).unwrap() as u64;
                            }}
                            _ => panic!(""unknown file descriptor {{}}"", args[0]),
                        }}
                        39 => return std::process::id() as u64,
                        _ => panic!(""unknown syscall number {{}}"", number),
                    }}
                }}
                fn main() {{
                    let mut stack = Vec::<u64>::new();
                    let mut memory = vec![0u8; {Memory.Capacity}];
            ");

        for (var ip = 0; ip < program.Count; ip++)
        {
            var op = program[ip];
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
                case OpCode.Mod:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a % b);");
                    break;
                case OpCode.Eq:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a == b { 1 } else { 0 });");
                    break;
                case OpCode.Ne:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a != b { 1 } else { 0 });");
                    break;
                case OpCode.Lt:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a < b { 1 } else { 0 });");
                    break;
                case OpCode.Gt:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a > b { 1 } else { 0 });");
                    break;
                case OpCode.Le:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a <= b { 1 } else { 0 });");
                    break;
                case OpCode.Ge:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(if a >= b { 1 } else { 0 });");
                    break;
                case OpCode.Shr:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a >> b);");
                    break;
                case OpCode.Shl:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a << b);");
                    break;
                case OpCode.Bor:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a | b);");
                    break;
                case OpCode.Band:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a & b);");
                    break;
                case OpCode.Print:
                    writer.WriteLine(@"println!(""{}"", stack.pop().unwrap());");
                    break;
                case OpCode.If:
                    writer.WriteLine("let condition = stack.pop().unwrap();");
                    writer.WriteLine("if condition == 1 {");
                    break;
                case OpCode.End:
                    writer.WriteLine("}");
                    break;
                case OpCode.Else:
                    writer.WriteLine("} else {");
                    break;
                case OpCode.While:
                    writer.WriteLine($"'addr_{ip}: loop {{");
                    break;
                case OpCode.Do:
                {
                    var doOp = (IntegerOp)op;
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine($"if a == 0 {{ break 'addr_{doOp.Operand}; }}");
                    break;
                }
                case OpCode.Dup:
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a);");
                    writer.WriteLine("stack.push(a);");
                    break;
                case OpCode.Dup2:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a);");
                    writer.WriteLine("stack.push(b);");
                    writer.WriteLine("stack.push(a);");
                    writer.WriteLine("stack.push(b);");
                    break;
                case OpCode.Swap:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(b);");
                    writer.WriteLine("stack.push(a);");
                    break;
                case OpCode.Drop:
                    writer.WriteLine("stack.pop();");
                    break;
                case OpCode.Over:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(a);");
                    writer.WriteLine("stack.push(b);");
                    writer.WriteLine("stack.push(a);");
                    break;
                case OpCode.Mem:
                    writer.WriteLine("stack.push(0);");
                    break;
                case OpCode.Load:
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("stack.push(memory[a as usize] as u64);");
                    break;
                case OpCode.Store:
                    writer.WriteLine("let b = stack.pop().unwrap();");
                    writer.WriteLine("let a = stack.pop().unwrap();");
                    writer.WriteLine("memory[a as usize] = b as u8;");
                    break;
                case OpCode.Syscall0:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Syscall1:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let arg1 = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![arg1]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Syscall2:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let arg1 = stack.pop().unwrap();");
                    writer.WriteLine("let arg2 = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![arg1, arg2]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Syscall3:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let arg1 = stack.pop().unwrap();");
                    writer.WriteLine("let arg2 = stack.pop().unwrap();");
                    writer.WriteLine("let arg3 = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![arg1, arg2, arg3]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Syscall4:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let arg1 = stack.pop().unwrap();");
                    writer.WriteLine("let arg2 = stack.pop().unwrap();");
                    writer.WriteLine("let arg3 = stack.pop().unwrap();");
                    writer.WriteLine("let arg4 = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![arg1, arg2, arg3, arg4]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Syscall5:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let arg1 = stack.pop().unwrap();");
                    writer.WriteLine("let arg2 = stack.pop().unwrap();");
                    writer.WriteLine("let arg3 = stack.pop().unwrap();");
                    writer.WriteLine("let arg4 = stack.pop().unwrap();");
                    writer.WriteLine("let arg5 = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![arg1, arg2, arg3, arg4, arg5]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Syscall6:
                    writer.WriteLine("let syscall_number = stack.pop().unwrap();");
                    writer.WriteLine("let arg1 = stack.pop().unwrap();");
                    writer.WriteLine("let arg2 = stack.pop().unwrap();");
                    writer.WriteLine("let arg3 = stack.pop().unwrap();");
                    writer.WriteLine("let arg4 = stack.pop().unwrap();");
                    writer.WriteLine("let arg5 = stack.pop().unwrap();");
                    writer.WriteLine("let arg6 = stack.pop().unwrap();");
                    writer.WriteLine("let result = do_syscall(&mut memory, syscall_number, vec![arg1, arg2, arg3, arg4, arg5, arg6]);");
                    writer.WriteLine("stack.push(result);");
                    break;
                case OpCode.Count:
                    throw new InvalidOperationException("unreachable");
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }
        writer.WriteLine("}");

    }
}
