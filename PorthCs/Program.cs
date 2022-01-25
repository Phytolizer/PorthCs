﻿using System.Diagnostics;

namespace PorthCs;

internal static class Program
{
    private static void Usage()
    {
        Console.WriteLine($"Usage: {Environment.ProcessPath ?? "porth"} <SUBCOMMAND> [ARGS]");
        Console.WriteLine("  SUBCOMMANDS:");
        Console.WriteLine("    sim <file>     Simulate the program");
        Console.WriteLine("    com <file>     Compile the program");
        Console.WriteLine("    help           Print this message");
    }

    private static void CallCommand(string[] args)
    {
        Console.WriteLine(string.Join(' ', args));
        var startInfo = new ProcessStartInfo
        {
            FileName = args[0],
            Arguments = string.Join(' ', args[1..]),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    private static IEnumerable<Op> LoadProgramFromFile(string filePath)
    {
        return File.OpenText(filePath).ReadToEnd().Split(' ', '\r', '\n', '\t').Where(word => word.Length > 0).Select(Parser.ParseWordAsOp);
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
                Simulator.Simulate(program);
            }
            break;
            case "com":
            {
                if (!argsIter.MoveNext())
                {
                    Usage();
                    Console.WriteLine("[ERROR] no input file is provided for the compilation");
                    Environment.Exit(1);
                }

                var programPath = (string)(argsIter.Current ?? throw new InvalidOperationException());
                var program = LoadProgramFromFile(programPath);
                var outputPath = Path.ChangeExtension(programPath, ".rs") ?? throw new InvalidOperationException();
                var outDir = Path.GetDirectoryName(programPath) ?? throw new InvalidOperationException();
                Compiler.Compile(program, outputPath);
                CallCommand(new[] { "rustc", "-C", "opt-level=2", outputPath, "--out-dir", outDir });
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