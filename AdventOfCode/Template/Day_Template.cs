/*using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;

namespace AdventOfCode;

public class DayXX : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = true;

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
        if (_runTestCases)
        {
            var testResults = RunTestCases(Solve_1_Initial, _testCases, 1).ToList();
            var solution = Solve_1_Initial(_input);

            return new($"{string.Join("\n", testResults)}\nSolution to {ClassPrefix} {CalculateIndex()}, part 1: {solution}");
        }
        else
        {
            var solution = Solve_1_Optimized(_input);

            return new($"Optimized Solution: {solution}");
        }
    }

    public override ValueTask<string> Solve_2()
    {
        if (_runTestCases)
        {
            var testResults = RunTestCases(Solve_2_Initial, _testCases, 2).ToList();
            var solution = Solve_2_Initial(_input);

            return new($"{string.Join("\n", testResults)}\nSolution to {ClassPrefix} {CalculateIndex()}, part 2: {solution}");
        }
        else
        {
            var solution = Solve_2_Optimized(_input);

            return new($"Optimized Solution: {solution}");
        }
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