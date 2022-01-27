using SubCommand;

namespace PorthCs;

public static class Program
{
    private static void Usage(TextWriter writer)
    {
        writer.WriteLine($"Usage: {Environment.ProcessPath ?? "porth"} <SUBCOMMAND> [ARGS]");
        writer.WriteLine("  SUBCOMMANDS:");
        writer.WriteLine("    sim <file>            Simulate the program");
        writer.WriteLine("    com [OPTIONS] <file>  Compile the program");
        writer.WriteLine("      OPTIONS:");
        writer.WriteLine("        -r                  Run the program after successful compilation");
        writer.WriteLine("        -o <file/dir>       Customize the output path");
        writer.WriteLine("    help                  Print this message");
    }

    private static IEnumerable<Op> LoadProgramFromFile(string filePath)
    {
        return SemanticAnalyzer.CrossReferenceBlocks(Lexer.LexFile(filePath).Select(Parser.ParseTokenAsOp).ToList());
    }

    public static void Main(string[] args)
    {
        DoMain(args, Console.Out);
    }

    public static void DoMain(IEnumerable<string> args, TextWriter writer)
    {
        using var argsIter = args.GetEnumerator();

        var quiet = false;
        string? subcommand = null;
        while (subcommand == null && argsIter.MoveNext())
        {
            var arg = argsIter.Current;
            switch (arg)
            {
                case "-q":
                    quiet = true;
                    break;
                default:
                    subcommand = arg;
                    break;
            }
        }

        if (subcommand == null)
        {
            Usage(writer);
            throw new UsageException("no subcommand is provided");
        }

        switch (subcommand)
        {
            case "sim":
            {
                if (!argsIter.MoveNext())
                {
                    Usage(writer);
                    throw new UsageException("no input file is provided for the simulation");
                }

                var programPath = argsIter.Current ?? throw new InvalidOperationException();
                var program = LoadProgramFromFile(programPath);
                Simulator.Simulate(program.ToList(), writer);
            }
            break;
            case "com":
            {
                var run = false;
                string? programPath = null;
                string? outputPath = null;
                while (programPath == null && argsIter.MoveNext())
                {
                    var flag = argsIter.Current ?? throw new InvalidOperationException();
                    switch (flag)
                    {
                        case "-r":
                            run = true;
                            break;
                        case "-o":
                            if (!argsIter.MoveNext())
                            {
                                Usage(writer);
                                throw new UsageException("no argument is provided for parameter '-o'");
                            }

                            outputPath = argsIter.Current ?? throw new InvalidOperationException();
                            break;
                        default:
                            programPath = flag;
                            break;
                    }
                }
                if (programPath == null)
                {
                    Usage(writer);
                    throw new UsageException("no input file is provided for the compilation");
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
                if (!quiet)
                {
                    writer.WriteLine($"[INFO] Generating {basePath}.rs ...");
                }

                Compiler.Compile(program.ToList(), $"{basePath}.rs");
                Command.Call(new[] { "rustfmt", $"{basePath}.rs" }, quiet);

                var exeName = Command.Call(new[] { "rustc", $"{basePath}.rs", "--print", "file-names" }, quiet);
                var exePath = Path.GetFullPath(Path.Join(outDir, exeName.TrimEnd()));
                Command.Call(new[] { "rustc", $"{basePath}.rs", "-C", "opt-level=2", "--out-dir", outDir }, quiet);
                if (run)
                {
                    var stdout = Command.Call(new[] { Path.GetFullPath(exePath) }, quiet);
                    writer.Write(stdout);
                }
                break;
            }
            case "help":
                Usage(writer);
                break;
            default:
                Usage(writer);
                throw new UsageException($"unknown subcommand {subcommand}");
        }
    }
}
