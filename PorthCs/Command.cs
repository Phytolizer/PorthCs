using System.Diagnostics;

namespace PorthCs;

public static class Command
{
    public enum Fatality
    {
        NonFatal,
        Fatal
    }

    public static (int exitCode, string stdout) Call(string[] args,
        Fatality fatality = Fatality.NonFatal)
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
        if (fatality == Fatality.Fatal)
        {
            throw new SubcommandException(args);
        }

        return (process.ExitCode, process.StandardOutput.ReadToEnd());
    }
}
