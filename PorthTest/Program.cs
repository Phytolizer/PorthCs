using PorthTest;

using var argsIter = ((IEnumerable<string>)args).GetEnumerator();

if (!argsIter.MoveNext())
{
    Test();
}
else
{
    var subcommand = argsIter.Current;
    switch (subcommand)
    {
        case "record":
            Record();
            break;
        case "test":
            Test();
            break;
        case "help":
            Usage();
            break;
        default:
            Usage();
            Console.WriteLine($"[ERROR] Unknown subcommand: {subcommand}.");
            Environment.ExitCode = 1;
            break;
    }
}

void Usage()
{
    Console.WriteLine($"Usage: {Environment.ProcessPath ?? "PorthTest"} [SUBCOMMAND]");
    Console.WriteLine("  SUBCOMMANDS:");
    Console.WriteLine("    test    Run the tests. (Default operation when no subcommand is provided.)");
    Console.WriteLine("    record  Record the expected output of the tests.");
    Console.WriteLine("    help    Print this message.");
}

void Record()
{
    var tests = Path.Join(Environment.CurrentDirectory, "Tests");
    foreach (var test in Directory.GetFiles(tests, "*.porth", SearchOption.AllDirectories))
    {
        var simOutput = RecordSimOutput(test);
        var txtPath = Path.ChangeExtension(test, ".txt");
        Console.WriteLine($"[INFO] Saving output to {txtPath}");
        using var txtFile = File.CreateText(txtPath);
        txtFile.Write(simOutput);
    }
}

void Test()
{
    var tests = Path.Join(Environment.CurrentDirectory, "Tests");
    foreach (var test in Directory.GetFiles(tests, "*.porth", SearchOption.AllDirectories))
    {
        Console.WriteLine($"[INFO] Testing {test}");
        var txtPath = Path.ChangeExtension(test, ".txt");
        var txtFile = File.OpenText(txtPath);
        var expectedOutput = txtFile.ReadToEnd().ReplaceLineEndings("\n");
        txtFile.Close();
        var simOutput = RecordSimOutput(test);
        if (simOutput != expectedOutput)
        {
            Console.WriteLine("[ERROR] Unexpected simulation output");
            Console.WriteLine("  Expected:");
            Console.WriteLine(IndentMultiLineString(expectedOutput, "    "));
            Console.WriteLine("  Actual:");
            Console.WriteLine(IndentMultiLineString(simOutput, "    "));
            throw new TestFailureException();
        }
        var comOutput = RecordComOutput(test);
        if (comOutput != expectedOutput)
        {
            Console.WriteLine("[ERROR] Unexpected simulation output");
            Console.WriteLine("  Expected:");
            Console.WriteLine(IndentMultiLineString(expectedOutput, "    "));
            Console.WriteLine("  Actual:");
            Console.WriteLine(IndentMultiLineString(comOutput, "    "));
            throw new TestFailureException();
        }
        Console.WriteLine($"[INFO] {test} OK");
    }
}

string RecordSimOutput(string s)
{
    var writer = new StringWriter();
    PorthCs.Program.DoMain(new[] { "sim", s }, writer);
    return writer.ToString().ReplaceLineEndings("\n");
}

string RecordComOutput(string s)
{
    var writer = new StringWriter();
    PorthCs.Program.DoMain(new[] { "-q", "com", "-r", s }, writer);
    return writer.ToString();
}

string IndentMultiLineString(string str, string indent)
{
    return string.Join("\n", str.Split("\n").Select(line => indent + line));
}
