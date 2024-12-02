
namespace AdventOfCode.Helpers
{
    class Test
    {
        public delegate string SolutionFunction(string input);

        public static IEnumerable<string> RunTestCases(SolutionFunction SolutionFunction, List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases, int part)
        {
            return _testCases.Select((test, index) =>
            {
                string actual = SolutionFunction(test.TestInput);
                return ValidateResult(actual, part == 1 ? test.ExpectedSolve1 : test.ExpectedSolve2, $"Part {part} Test {index + 1}");
            });
        }

        private static string ValidateResult(string actual, string expected, string testName)
        {
            if (actual == expected)
            {
                return $"[PASS] {testName}";
            }
            else
            {
                return $"[FAIL] {testName}. Expected: {expected}, Actual: {actual}.";
            }
        }
    }
}
