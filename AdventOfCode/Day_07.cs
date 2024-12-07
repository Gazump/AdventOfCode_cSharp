using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;

namespace AdventOfCode;

public class Day_07 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    public Day_07()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                190: 10 19
                3267: 81 40 27
                83: 17 5
                156: 15 6
                7290: 6 8 6 15
                161011: 16 10 13
                192: 17 8 14
                21037: 9 7 18 13
                292: 11 6 16 20
                """,
                "3749", // Expected result for part 1
                "11387"  // Expected result for part 2
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
        var inputSpan = input.AsSpan();

        long total = 0;

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var nums = ParseLongNumbers(line, ": ".AsSpan(), 64);

            var result = nums[0];
            var operands = nums[1..];
            var n = operands.Length;
            var combos = 1 << (n - 1);

            for (int c = 0; c < combos; c++)
            {
                long answer = operands[0];

                for (int o = 0; o < n - 1; o++)
                {
                    if ((c & (1 << o)) == 0)
                    {
                        answer += operands[o + 1];
                    } 
                    else
                    {
                        answer *= operands[o + 1];
                    }
                }

                if (answer == result)
                {
                    total += result;
                    break;
                }
            }
        }

        return $"{total}";
    }

    public static string Solve_2_Initial(string input)
    {
        var inputSpan = input.AsSpan();

        long total = 0;

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var nums = ParseLongNumbers(line, ": ".AsSpan(), 64);

            var result = nums[0];
            var operands = nums[1..];
            var n = operands.Length;
            var combos = (int)Math.Pow(3, n - 1);
            var done = false;

            for (int c = 0; c < combos && !done; c++)
            {
                long answer = operands[0];
                int currentCombo = c;

                for (int o = 0; o < n - 1; o++)
                {   
                    int operatorCode = currentCombo % 3;
                    currentCombo /= 3;

                    switch (operatorCode)
                    {
                        case 0: // +
                            answer += operands[o + 1];
                            break;
                        case 1: // *
                            answer *= operands[o + 1];
                            break;
                        case 2: // ||
                            answer = Concatenate(answer, operands[o + 1]);
                            break;
                    }
                }

                if (answer == result)
                {
                    total += result;
                    done = true;
                }
            }
        }

        return $"{total}";
    }

    // tried optimizing but overhead for setup was >13ms
    public static string Solve_1_Optimized(string input)
    {
        return Solve_1_Initial(input);
    }

    // optimized and running <300ms
    public static string Solve_2_Optimized(string input)
    {
        var inputSpan = input.AsSpan();
        long total = 0;

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

            var nums = ParseLongNumbers(line, ": ".AsSpan(), 64);

            var result = nums[0];
            var operands = nums[1..];
            var n = operands.Length;

            if (n == 0) continue;

            var memoDict = new Dictionary<(int, long), bool>();
            if (FindCombinationWithConcat(operands, result, 0, operands[0], memoDict))
            {
                total += result;
            }
        }

        return $"{total}";
    }

    private static bool FindCombinationWithConcat(ReadOnlySpan<long> operands, long target, int index, long current, Dictionary<(int, long), bool> memoDict)
    {
        if (current > target) return false;

        if (index == operands.Length - 1)
        {
            return current == target;
        }

        var key = (index, current);
        if (memoDict.TryGetValue(key, out var cached))
        {
            return cached;
        }

        long nextOperand = operands[index + 1];

        bool found =
            FindCombinationWithConcat(operands, target, index + 1, current + nextOperand, memoDict) ||
            FindCombinationWithConcat(operands, target, index + 1, current * nextOperand, memoDict) ||
            FindCombinationWithConcat(operands, target, index + 1, Concatenate(current, nextOperand), memoDict);

        memoDict[key] = found;
        return found;
    }

    private static long Concatenate(long left, long right)
    {
        long multiplier = 1;
        while (right >= multiplier) multiplier *= 10;
        return left * multiplier + right;
    }
}
