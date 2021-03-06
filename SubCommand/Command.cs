using System.Diagnostics;

namespace SubCommand;

public static class Command
{
    public static string Call(IEnumerable<string> argsEnumerable, bool quiet = false)
    {
        var args = argsEnumerable.ToArray();

        if (!quiet)
        {
            Console.WriteLine($"[CMD] {string.Join(' ', args)}");
        }

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

        using var process = Process.Start(startInfo) ?? throw new StartException(args);

        process.WaitForExit();
        if (process.ExitCode == 0)
        {
            return process.StandardOutput.ReadToEnd();
        }

        Console.Error.Write(process.StandardError.ReadToEnd());
        throw new ExitCodeException(args);
    }
}
