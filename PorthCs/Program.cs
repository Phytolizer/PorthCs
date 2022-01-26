namespace PorthCs;

internal static class Program
{
    private static void Usage()
    {
        Console.WriteLine($"Usage: {Environment.ProcessPath ?? "porth"} <SUBCOMMAND> [ARGS]");
        Console.WriteLine("  SUBCOMMANDS:");
        Console.WriteLine("    sim <file>            Simulate the program");
        Console.WriteLine("    com [OPTIONS] <file>  Compile the program");
        Console.WriteLine("      OPTIONS:");
        Console.WriteLine("        -r                  Run the program after successful compilation");
        Console.WriteLine("        -o <file/dir>       Customize the output path");
        Console.WriteLine("    help                  Print this message");
    }

    private static IEnumerable<Op> LoadProgramFromFile(string filePath)
    {
        return SemanticAnalyzer.CrossReferenceBlocks(Lexer.LexFile(filePath).Select(Parser.ParseTokenAsOp).ToList());
    }

    public static void Main(string[] args)
    {
        var argsIter = args.GetEnumerator();
        if (!argsIter.MoveNext())
        {
            Usage();
            Console.WriteLine("[ERROR] no subcommand is provided");
            Environment.Exit(1);
        }

        var subcommand = argsIter.Current;

        switch (subcommand)
        {
            case "sim":
            {
                if (!argsIter.MoveNext())
                {
                    Usage();
                    Console.WriteLine("[ERROR] no input file is provided for the simulation");
                    Environment.Exit(1);
                }

                var programPath = (string)(argsIter.Current ?? throw new InvalidOperationException());
                var program = LoadProgramFromFile(programPath);
                Simulator.Simulate(program.ToList());
            }
            break;
            case "com":
            {
                var run = false;
                string? programPath = null;
                string? outputPath = null;
                while (programPath == null && argsIter.MoveNext())
                {
                    var flag = (string)(argsIter.Current ?? throw new InvalidOperationException());
                    switch (flag)
                    {
                        case "-r":
                            run = true;
                            break;
                        case "-o":
                            if (!argsIter.MoveNext())
                            {
                                Usage();
                                Console.WriteLine("[ERROR] no argument is provided for parameter '-o'");
                                Environment.Exit(1);
                            }

                            outputPath = (string)(argsIter.Current ?? throw new InvalidOperationException());
                            break;
                        default:
                            programPath = flag;
                            break;
                    }
                }
                if (programPath == null)
                {
                    Usage();
                    Console.WriteLine("[ERROR] no input file is provided for the compilation");
                    Environment.Exit(1);
                }

                string? baseName;
                string? baseDir;
                if (outputPath != null)
                {
                    baseName = Path.GetFileNameWithoutExtension(Directory.Exists(outputPath) ? programPath : outputPath);
                    baseDir = Path.GetDirectoryName(outputPath);
                }
                else
                {
                    baseName = Path.GetFileNameWithoutExtension(programPath);
                    baseDir = Path.GetDirectoryName(programPath);
                }

                var basePath = Path.Join(baseDir, baseName);

                var program = LoadProgramFromFile(programPath);
                var outDir = Path.GetDirectoryName(programPath) ?? throw new InvalidOperationException();
                Console.WriteLine($"[INFO] Generating {basePath}.rs ...");
                Compiler.Compile(program.ToList(), $"{basePath}.rs");
                Command.Call(new[] { "rustfmt", $"{basePath}.rs" });

                var exeName = Command.Call(new[] { "rustc", $"{basePath}.rs", "--print", "file-names" });
                var exePath = Path.GetFullPath(Path.Join(outDir, exeName.TrimEnd()));
                Command.Call(new[] { "rustc", $"{basePath}.rs", "-C", "opt-level=2", "--out-dir", outDir });
                if (run)
                {
                    var stdout = Command.Call(new[] { Path.GetFullPath(exePath) });
                    Console.Write(stdout);
                }
                break;
            }
            case "help":
                Usage();
                break;
            default:
                Usage();
                Console.WriteLine($"[ERROR] unknown subcommand {subcommand}");
                Environment.Exit(1);
                break;
        }
    }
}
