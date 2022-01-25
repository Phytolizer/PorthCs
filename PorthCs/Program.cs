using System.Diagnostics;

namespace PorthCs;

internal static class Program
{
    private static void Usage()
    {
        Console.WriteLine("Usage: porth <SUBCOMMAND> [ARGS]");
        Console.WriteLine("  SUBCOMMANDS:");
        Console.WriteLine("    sim     Simulate the program");
        Console.WriteLine("    com     Compile the program");
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

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Usage();
            Console.WriteLine("[ERROR] no subcommand is provided");
            Environment.Exit(1);
        }

        var subcommand = args[0];

        var program = new List<Op>
        {
            Ops.Push(34),
            Ops.Push(35),
            Ops.Plus(),
            Ops.Dump(),
            Ops.Push(500),
            Ops.Push(80),
            Ops.Minus(),
            Ops.Dump()
        };

        switch (subcommand)
        {
            case "sim":
                Simulator.Simulate(program);
                break;
            case "com":
                Compiler.Compile(program, "output.rs");
                CallCommand(new[] { "rustc", "output.rs" });
                break;
            default:
                Usage();
                Console.WriteLine($"[ERROR] unknown subcommand {subcommand}");
                Environment.Exit(1);
                break;
        }
    }
}