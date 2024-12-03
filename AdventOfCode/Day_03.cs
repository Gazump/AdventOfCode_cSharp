using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day_03 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    public Day_03()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))
                """,
                "161", // Expected result for part 1
                "48"  // Expected result for part 2
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
        var matches = new List<string>();
        int sum = 0;

        foreach (Match match in Regex1().Matches(input))
        {
            matches.Add(match.Value);

            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);

            sum += x * y;
        }

        return $"{sum}";
    }

    public static string Solve_2_Initial(string input)
    {
        var matches = new List<string>();
        int sum = 0;
        bool isActive = true;

        foreach (Match match in Regex2().Matches(input))
        {
            matches.Add(match.Value);

            if (match.Value == "do()")
            {
                isActive = true;
            }
            else if (match.Value == "don't()")
            {
                isActive = false;
            }
            else if (isActive)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);

                sum += x * y;
            }
        }

        return $"{sum}";
    }

    public static string Solve_1_Optimized(string input)
    {
        int sum = 0;
        ReadOnlySpan<char> span = input.AsSpan();
        int start = 0;

        while (start < span.Length)
        {
            int mulIndex = span[start..].IndexOf("mul(");
            if (mulIndex == -1) break;
            start += mulIndex + 4;

            int commaIndex = span[start..].IndexOf(',');
            int closeParenIndex = span[start..].IndexOf(')');

            if (commaIndex == -1 || closeParenIndex == -1 || commaIndex > closeParenIndex)
            {
                start += Math.Max(1, closeParenIndex + 1);
                continue;
            }

            if (int.TryParse(span[start..(start + commaIndex)], out int x) &&
                int.TryParse(span[(start + commaIndex + 1)..(start + closeParenIndex)], out int y))
            {
                sum += x * y;
                start += closeParenIndex + 1;
            } else
            {
                start++;
            }
        }
        return sum.ToString();
    }

    public static string Solve_2_Optimized(string input)
    {
        int sum = 0;
        bool isActive = true;
        ReadOnlySpan<char> span = input.AsSpan();
        int start = 0;

        while (start < span.Length)
        {   
            if (span[start..].StartsWith("do()"))
            {
                isActive = true;
                start += 4;
            }
            else if (span[start..].StartsWith("don't()"))
            {
                isActive = false;
                start += 7;
            }
            else if (span[start..].StartsWith("mul("))
            {
                start += 4;
                
                int commaIndex = span[start..].IndexOf(',');
                int closeParenIndex = span[start..].IndexOf(')');

                if (commaIndex == -1 || closeParenIndex == -1 || commaIndex > closeParenIndex)
                {
                    start++;
                    continue;
                }

                if (isActive &&
                    int.TryParse(span[start..(start + commaIndex)], out int x) &&
                    int.TryParse(span[(start + commaIndex + 1)..(start + closeParenIndex)], out int y))
                {
                    sum += x * y;
                    start += closeParenIndex + 1;
                } else
                {
                    start++;
                }
            }
            else
            {   
                start++;
            }
        }

        return sum.ToString();
    }

    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)")]
    private static partial Regex Regex1();

    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)")]
    private static partial Regex Regex2();
}
