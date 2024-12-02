using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;
using System.Collections.Concurrent;

namespace AdventOfCode;

public class Day_01 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    public Day_01()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                3   4
                4   3
                2   5
                1   3
                3   9
                3   3
                """,
                "11", // Expected result for part 1
                "31"  // Expected result for part 2
            )
        ];
    }

    public override ValueTask<string> Solve_1()
    {
        var testResults = _runTestCases ? RunTestCases(_runOptimizedSolutions ? Solve_1_Optimized : Solve_1_Initial, _testCases, 1).ToList() : null;
        var solution = _runOptimizedSolutions ? Solve_1_Optimized(_input) : Solve_1_Initial(_input);

        return new(FormatSolutionOutput(solution, testResults, _runTestCases, _runOptimizedSolutions));
    }

    public override ValueTask<string> Solve_2()
    {
        var testResults = _runTestCases ? RunTestCases(_runOptimizedSolutions ? Solve_2_Optimized : Solve_2_Initial, _testCases, 2).ToList() : null;
        var solution = _runOptimizedSolutions ? Solve_2_Optimized(_input) : Solve_2_Initial(_input);

        return new(FormatSolutionOutput(solution, testResults, _runTestCases, _runOptimizedSolutions));
    }

    public static string Solve_1_Initial(string input)
    {
        List<int> first = [];
        List<int> second = [];

        var lines = input.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);

            first.Add(x);
            second.Add(y);
        }

        first.Sort();
        second.Sort();

        var diffs = first.Select((x, i) => Math.Abs(x - second[i])).ToList();
        var solution = diffs.Sum();

        return $"{solution}";
    }

    public static string Solve_2_Initial(string input)
    {
        List<int> first = [];
        List<int> second = [];
        List<int> scores = [];

        var lines = input.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);

            first.Add(x);
            second.Add(y);
        }

        first.Sort();
        second.Sort();

        foreach (var item in first)
        {
            var count = second.Count(x => x == item);

            scores.Add(item * count);
        }

        var solution = scores.Sum();
        return $"{solution}";
    }

    public static string Solve_1_Optimized(string input)
    {
        var inputSpan = input.AsSpan();
        var bufferX = new List<int>();
        var bufferY = new List<int>();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var parts = ParseNumbers(line);
            bufferX.Add(parts[0]);
            bufferY.Add(parts[1]);
        }

        bufferX.Sort();
        bufferY.Sort();

        int solution = 0;

        for (int i = 0; i < bufferX.Count; i++)
        {
            solution += Math.Abs(bufferX[i] - bufferY[i]);
        }

        return $"{solution}";
    }

    public static string Solve_2_Optimized(string input)
    {
        var inputSpan = input.AsSpan();
        var bufferX = new List<int>();
        var bufferY = new List<int>();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var parts = ParseNumbers(line);
            bufferX.Add(parts[0]);
            bufferY.Add(parts[1]);
        }

        bufferX.Sort();
        bufferY.Sort();

        var secondGroups = new Dictionary<int, int>();
        foreach (var y in bufferY)
        {
            if (secondGroups.TryGetValue(y, out int value))
            {
                secondGroups[y] = ++value;
            }
            else
            {
                secondGroups[y] = 1;
            }
        }

        int solution = 0;
        foreach (var x in bufferX)
        {
            if (secondGroups.TryGetValue(x, out int count))
            {
                solution += x * count;
            }
        }

        return $"{solution}";
    }

    public string Solve_1_Optimized_Parallel(string input)
    {
        var inputSpan = input.AsSpan();
        var bufferX = new List<int>();
        var bufferY = new List<int>();
        var lineSegments = new List<string>();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (!line.IsEmpty)
            {
                lineSegments.Add(line.ToString());
            }
        }

        var localX = new ConcurrentBag<int>();
        var localY = new ConcurrentBag<int>();

        Parallel.ForEach(lineSegments, line =>
        {
            var parts = ParseNumbersThreadSafe(line.AsSpan());
            localX.Add(parts[0]);
            localY.Add(parts[1]);
        });

        bufferX = [.. localX];
        bufferY = [.. localY];

        bufferX.Sort();
        bufferY.Sort();

        int solution = 0;
        for (int i = 0; i < bufferX.Count; i++)
        {
            solution += Math.Abs(bufferX[i] - bufferY[i]);
        }

        return $"{solution}";
    }

}
