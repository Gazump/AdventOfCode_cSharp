using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;

namespace AdventOfCode;

public class Day_02 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    public Day_02()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                7 6 4 2 1
                1 2 7 8 9
                9 7 6 2 1
                1 3 2 4 5
                8 6 4 4 1
                1 3 6 7 9
                """,
                "2", // Expected result for part 1
                "4"  // Expected result for part 2
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

    public string Solve_1_Initial(string input)
    {
        var lines = input.Split(Environment.NewLine);

        var safeCount = 0;

        foreach (var line in lines) {
            var levels = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            if (IsOrdered(levels) && AreDifferencesWithinRange(levels, 1, 3))
            {
                safeCount++;
            }
        }

        var solution = safeCount;
        return $"{solution}";
    }

    public string Solve_2_Initial(string input)
    {
        var lines = input.Split(Environment.NewLine);

        var safeCount = 0;

        foreach (var line in lines)
        {
            var levels = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            if (IsOrdered(levels) && AreDifferencesWithinRange(levels, 1, 3))
            {
                safeCount++;
                
            } 
            else
            {
                for (int excluded = 0; excluded < levels.Count; excluded++)
                {
                    var newLevels = levels.Where((_, i) => i != excluded).ToList();

                    if (IsOrdered(newLevels) && AreDifferencesWithinRange(newLevels, 1, 3))
                    {
                        safeCount++;
                        break;
                    }
                }
            }
        }

        var solution = safeCount;
        return $"{solution}";
    }

    public string Solve_1_Optimized(string input)
    {
        var safeCount = 0;
        var inputSpan = input.AsSpan();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var levels = ParseNumbers(line);
            if (IsOrderedAndWithinRange(levels))
            {
                safeCount++;
            }
        }

        return $"{safeCount}";
    }

    public string Solve_2_Optimized(string input)
    {
        var safeCount = 0;
        var inputSpan = input.AsSpan();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var levels = ParseNumbers(line);

            if (IsOrderedAndWithinRange(levels))
            {
                safeCount++;
            }
            else
            {
                for (int excluded = 0; excluded < levels.Length; excluded++)
                {
                    if (IsOrderedAndWithinRangeExcludingIndex(levels, excluded))
                    {
                        safeCount++;
                        break;
                    }
                }
            }
        }

        return $"{safeCount}";
    }



    private static bool IsOrdered(List<int> numbers)
    {
        var isAscending = true;
        var isDescending = true;

        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] <= numbers[i - 1])
                isAscending = false;

            if (numbers[i] >= numbers[i - 1])
                isDescending = false;

            if (!isAscending && !isDescending)
                return false;
        }

        return isAscending || isDescending;
    }

    private static bool AreDifferencesWithinRange(List<int> numbers, int minDiff = 1, int maxDiff = 3)
    {
        for (int i = 1; i < numbers.Count; i++)
        {
            int diff = Math.Abs(numbers[i] - numbers[i - 1]);
            if (diff < minDiff || diff > maxDiff)
            {
                return false;
            }
        }

        return true;
    }

    // Efficient combined check
    private static bool IsOrderedAndWithinRange(Span<int> numbers, int minDiff = 1, int maxDiff = 3)
    {
        bool isAscending = true;
        bool isDescending = true;

        for (int i = 1; i < numbers.Length; i++)
        {
            int diff = Math.Abs(numbers[i] - numbers[i - 1]);
            if (diff < minDiff || diff > maxDiff)
            {
                return false;
            }

            if (numbers[i] >= numbers[i - 1]) isDescending = false;
            if (numbers[i] <= numbers[i - 1]) isAscending = false;

            if (!isAscending && !isDescending)
            {
                return false;
            }
        }

        return true;
    }

    // Efficient exclusion-based check
    private static bool IsOrderedAndWithinRangeExcludingIndex(Span<int> numbers, int excludedIndex, int minDiff = 1, int maxDiff = 3)
    {
        bool isAscending = true;
        bool isDescending = true;

        int? prev = null;

        for (int i = 0; i < numbers.Length; i++)
        {
            if (i == excludedIndex) continue;

            if (prev.HasValue)
            {
                int diff = Math.Abs(numbers[i] - prev.Value);
                if (diff < minDiff || diff > maxDiff)
                {
                    return false;
                }

                if (numbers[i] >= prev.Value) isDescending = false;
                if (numbers[i] <= prev.Value) isAscending = false;

                if (!isAscending && !isDescending)
                {
                    return false;
                }
            }

            prev = numbers[i];
        }

        return true;
    }
}
