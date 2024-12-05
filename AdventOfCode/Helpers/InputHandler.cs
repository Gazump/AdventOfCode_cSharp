namespace AdventOfCode.Helpers
{
    public class InputHandler
    {
        public static Span<int> ParseNumbers(ReadOnlySpan<char> line, ReadOnlySpan<char> delimiters = default, int bufferSize = 8)
        {
            if (delimiters.IsEmpty) 
            {
                delimiters = " ".AsSpan();
            }

            var buffer = new int[bufferSize];
            int count = 0, start = 0;

            for (int i = 0; i <= line.Length; i++)
            {
                if (i == line.Length || delimiters.Contains(line[i]))
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


        public static bool Contains(ReadOnlySpan<char> span, char c)
        {
            foreach (var item in span)
            {
                if (item == c)
                {
                    return true;
                }
            }
            return false;
        }

        public static int[] ParseNumbersThreadSafe(ReadOnlySpan<char> line, int bufferSize = 8)
        {
            var buffer = new int[bufferSize];
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

            var result = new int[count];
            Array.Copy(buffer, result, count);
            return result;
        }


        public static ReadOnlySpan<char> ReadNextLine(ref ReadOnlySpan<char> input)
        {
            int index = input.IndexOf('\n');
            if (index == -1)
            {
                var line = input;
                input = [];
                return line.Trim();
            }

            var nextLine = input[..index].Trim();
            input = input[(index + 1)..];
            return nextLine;
        }

        public static char[,] ParseGrid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty.");

            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int width = lines[0].Length;

            foreach (var line in lines)
            {
                if (line.Length != width)
                    throw new ArgumentException("All lines in the input must have the same length to form a rectangle.");
            }

            int height = lines.Length;
            var grid = new char[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[row, col] = lines[row][col];
                }
            }

            return grid;
        }
    }
}
