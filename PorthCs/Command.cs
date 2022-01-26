using System.Diagnostics;

namespace PorthCs;

public static class Command
{
    public static string Call(string[] args)
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
            return process.StandardOutput.ReadToEnd();
        }

        Console.Error.Write(process.StandardError.ReadToEnd());
        throw new SubcommandException(args);
    }
}
