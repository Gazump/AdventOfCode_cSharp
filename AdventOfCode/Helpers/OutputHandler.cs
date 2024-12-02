namespace AdventOfCode.Helpers
{
    public class OutputHandler
    {
        public static string FormatSolutionOutput(string solution, IEnumerable<string> testResults, bool runTestCases, bool runOptimized)
        {
            return $"{(runTestCases ? string.Join("\n", testResults) + '\n' : "")}{(runOptimized ? "Optimized " : "")}Solution: {solution}";
        }
    }
}
