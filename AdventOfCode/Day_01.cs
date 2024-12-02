using static AdventOfCode.Helpers.Test;

namespace AdventOfCode;

public class Day_01 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;

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
        var testResults = RunTestCases(Solve_1_Initial, _testCases, 1).ToList();
        var solution = Solve_1_Initial(_input);

        return new($"{string.Join("\n", testResults)}\nSolution to {ClassPrefix} {CalculateIndex()}, part 1: {solution}");
    }

    public override ValueTask<string> Solve_2()
    {
        var testResults = RunTestCases(Solve_2_Initial, _testCases, 2).ToList();
        var solution = Solve_2_Initial(_input);

        return new($"{string.Join("\n", testResults)}\nSolution to {ClassPrefix} {CalculateIndex()}, part 2: {solution}");
    }

    public string Solve_1_Initial(string input)
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

    public string Solve_2_Initial(string input)
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

    public string Solve_1_Optimized(string input)
    {
        var (first, second) = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return (X: int.Parse(parts[0]), Y: int.Parse(parts[1]));
            })
            .Aggregate(
                (first: new List<int>(), second: new List<int>()),
                (acc, pair) =>
                {
                    acc.first.Add(pair.X);
                    acc.second.Add(pair.Y);
                    return acc;
                });

        first.Sort();
        second.Sort();

        var solution = first.Select((x, i) => Math.Abs(x - second[i])).Sum();
        return $"{solution}";
    }

    public string Solve_2_Optimized(string input)
    {
        var (first, second) = input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return (X: int.Parse(parts[0]), Y: int.Parse(parts[1]));
            })
            .Aggregate(
                (first: new List<int>(), second: new List<int>()),
                (acc, pair) =>
                {
                    acc.first.Add(pair.X);
                    acc.second.Add(pair.Y);
                    return acc;
                });

        first.Sort();
        second.Sort();

        var secondGroups = second.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var solution = first
            .Select(item => secondGroups.TryGetValue(item, out var count) ? item * count : 0)
            .Sum();

        return $"{solution}";
    }
}
