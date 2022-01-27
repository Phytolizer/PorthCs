using SubCommand;

var tests = Path.Join(Environment.CurrentDirectory, "Tests");
foreach (var test in Directory.GetFiles(tests, "*.porth", SearchOption.AllDirectories))
{
    Console.WriteLine($"[INFO] Testing {test}");
    var writer = new StringWriter();
    PorthCs.Program.DoMain(new[] { "sim", test }, writer);
    var simOutput = string.Join('\n', writer.ToString().Split(Environment.NewLine));
    writer = new StringWriter();
    PorthCs.Program.DoMain(new[] { "-q", "com", "-r", test }, writer);
    var comOutput = writer.ToString();
    if (simOutput != comOutput)
    {
        Console.WriteLine("[ERROR] Output discrepancy between simulation and compilation");
        Console.WriteLine("  Simulation output:");
        Console.WriteLine(string.Join("\n", simOutput.Split("\n").Select(line => $"    {line}")));
        Console.WriteLine("  Compilation output:");
        Console.WriteLine(string.Join("\n", comOutput.Split("\n").Select(line => $"    {line}")));
        Environment.Exit(1);
    }
    Console.WriteLine($"[INFO] {test} OK");
}
