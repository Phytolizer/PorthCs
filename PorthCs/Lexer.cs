namespace PorthCs;

public static class Lexer
{
    private static IEnumerable<Token> LexLine(string filePath, int row, string line)
    {
        var col = FindCol(line, 0, x => !char.IsWhiteSpace(x));
        while (col < line.Length)
        {
            var colEnd = FindCol(line, col, char.IsWhiteSpace);
            yield return new Token(filePath, row, col, line[col..colEnd]);
            col = FindCol(line, colEnd, x => !char.IsWhiteSpace(x));
        }
    }

    private static int FindCol(string line, int col, Predicate<char> predicate)
    {
        for (var i = col; i < line.Length; ++i)
        {
            if (predicate(line[i]))
            {
                return i;
            }
        }

        return line.Length;
    }

    public static IEnumerable<Token> LexFile(string filePath)
    {
        return File.OpenText(filePath).ReadToEnd().Split('\n').SelectMany((line, row) => LexLine(filePath, row, line));
    }
}