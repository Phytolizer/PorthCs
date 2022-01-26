﻿using System.Diagnostics;

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

    private static void CallCommand(string[] args)
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

        while (!process.HasExited)
        {
            var stdoutLine = process.StandardOutput.ReadLine();
            if (stdoutLine != null)
            {
                Console.WriteLine(stdoutLine);
            }
            var stderrLine = process.StandardError.ReadLine();
            if (stderrLine != null)
            {
                Console.WriteLine(stderrLine);
            }
        }
        // read remaining output
        Console.Write(process.StandardOutput.ReadToEnd());
        Console.Write(process.StandardError.ReadToEnd());
        if (process.ExitCode != 0)
        {
            Environment.Exit(process.ExitCode);
        }
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
                CallCommand(new[] { "rustfmt", outputPath });
                CallCommand(new[] { "rustc", "-C", "opt-level=2", outputPath, "--out-dir", outDir });
                if (run)
                {
                    CallCommand(new[] { Path.ChangeExtension(Path.GetFullPath(outputPath), ".exe") });
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