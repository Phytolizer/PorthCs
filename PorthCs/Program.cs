using System.Diagnostics;

namespace PorthCs;

internal static class Program
{
    private static void Usage()
    {
        Console.WriteLine($"Usage: {Environment.ProcessPath ?? "porth"} <SUBCOMMAND> [ARGS]");
        Console.WriteLine("  SUBCOMMANDS:");
        Console.WriteLine("    sim <file>       Simulate the program");
        Console.WriteLine("    com [-r] <file>  Compile the program");
        Console.WriteLine("    help             Print this message");
    }

    private enum CommandFatality
    {
        NonFatal,
        Fatal
    }

    private static (int exitCode, string stdout) CallCommand(string[] args, CommandFatality fatality = CommandFatality.NonFatal)
    {
        Console.WriteLine($"[CMD] {string.Join(' ', args)}");
        var startInfo = new ProcessStartInfo
        {
            FileName = args[0],
            Arguments = string.Join(' ', args[1..]),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new SubcommandException(args);
        }

        process.WaitForExit();
        if (process.ExitCode == 0)
        {
            return (process.ExitCode, process.StandardOutput.ReadToEnd());
        }

        Console.Error.Write(process.StandardError.ReadToEnd());
        if (fatality == CommandFatality.Fatal)
        {
            throw new SubcommandException(args);
        }
        return (process.ExitCode, process.StandardOutput.ReadToEnd());
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
                while (programPath == null && argsIter.MoveNext())
                {
                    var flag = (string)(argsIter.Current ?? throw new InvalidOperationException());
                    switch (flag)
                    {
                        case "-r":
                            run = true;
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

                var program = LoadProgramFromFile(programPath);
                var outputPath = Path.ChangeExtension(programPath, ".rs") ?? throw new InvalidOperationException();
                var outDir = Path.GetDirectoryName(programPath) ?? throw new InvalidOperationException();
                Console.WriteLine($"[INFO] Generating {outputPath}...");
                Compiler.Compile(program.ToList(), outputPath);
                var rustfmtArgs = new[] { "rustfmt", outputPath };
                // rustfmt is silent on success
                CallCommand(rustfmtArgs, CommandFatality.Fatal);

                var rustcArgs = new[] { "rustc", "-C", "opt-level=2", outputPath, "--out-dir", outDir, "--print", "file-names" };
                var (_, exeName) = CallCommand(rustcArgs, CommandFatality.Fatal);
                var exePath = Path.GetFullPath(Path.Join(outDir, exeName.TrimEnd()));
                if (run)
                {
                    var (_, stdout) = CallCommand(new[] { Path.GetFullPath(exePath) }, CommandFatality.Fatal);
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