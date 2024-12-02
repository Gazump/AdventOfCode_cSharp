/*namespace AdventOfCode;

public class DayXX : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;

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
        var testResults = Test.Run(Solve_1_Initial, _testCases, 1).ToList();
        var solution = Solve_1_Initial(_input);

        return new($"{string.Join("\n", testResults)}\nSolution to {ClassPrefix} {CalculateIndex()}, part 1: {solution}");
    }

    public override ValueTask<string> Solve_2()
    {
        var testResults = Test.Run(Solve_2_Initial, _testCases, 2).ToList();
        var solution = Solve_2_Initial(_input);

        return new($"{string.Join("\n", testResults)}\nSolution to {ClassPrefix} {CalculateIndex()}, part 2: {solution}");
    }

    public string Solve_1_Initial(string input)
    {
        var solution = 1;

        return $"{solution}";
    }

    public string Solve_2_Initial(string input)
    {        
        var solution = 2;
        return $"{solution}";
    }

    public string Solve_1_Optimized(string input)
    {
        var solution = 0;
        return $"{solution}";
    }

    public string Solve_2_Optimized(string input)
    {
        var solution = 0;
        return $"{solution}";
    }
}
*/