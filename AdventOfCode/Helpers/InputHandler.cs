namespace AdventOfCode.Helpers
{
    public class InputHandler
    {
        public static Span<int> ParseNumbers(ReadOnlySpan<char> line)
        {
            var buffer = new int[8];
            int count = 0;
            int start = 0;

            for (int i = 0; i <= line.Length; i++)
            {
                if (i == line.Length || line[i] == ' ')
                {
                    if (i > start)
                    {
                        var segment = line[start..i];
                        if (int.TryParse(segment, out var num))
                        {
                            buffer[count++] = num;
                        }
                    }
                    start = i + 1;
                }
            }

            return buffer.AsSpan(0, count);
        }

        public static ReadOnlySpan<char> ReadNextLine(ref ReadOnlySpan<char> input)
        {
            int index = input.IndexOf('\n');
            if (index == -1)
            {
                var line = input;
                input = ReadOnlySpan<char>.Empty;
                return line.TrimEnd('\r');
            }

            var nextLine = input[..index].TrimEnd('\r');
            input = input[(index + 1)..];
            return nextLine;
        }
    }
}
