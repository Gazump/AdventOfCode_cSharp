/*using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;

namespace AdventOfCode;

public class DayXX : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = true;
    private readonly bool _runOptimizedSolutions = false;

    public DayXX()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                
                """,
                "1", // Expected result for part 1
                "2"  // Expected result for part 2
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
        var solution = 1;
        var inputSpan = input.AsSpan();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);
            if (line.IsEmpty) continue;

        }

        return $"{solution}";
    }

    public string Solve_2_Initial(string input)
    {
        return $"No Solution";
    }

    public string Solve_1_Optimized(string input)
    {
        return $"No Solution";
    }

    public string Solve_2_Optimized(string input)
    {
        return $"No Solution";
    }
}
*/