using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;
using AdventOfCode.Helpers;

namespace AdventOfCode;

public class Day_05 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    public Day_05()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                47|53
                97|13
                97|61
                97|47
                75|29
                61|13
                75|53
                29|13
                97|29
                53|29
                61|53
                97|53
                61|29
                47|13
                75|47
                97|75
                47|61
                75|61
                47|29
                75|13
                53|13

                75,47,61,53,29
                97,61,53,29,13
                75,29,13
                75,97,47,61,53
                61,13,29
                97,13,75,29,47
                """,
                "143", // Expected result for part 1
                "123"  // Expected result for part 2
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
        var isParsingRules = true;
        var rulesDict = new Dictionary<int, List<int>>();
        var updateList = new List<List<int>>();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);

            if (line.IsEmpty)
            {
                isParsingRules = false;
                continue;
            }
            else if (!isParsingRules)
            {
                var nums = ParseNumbers(line, ",".AsSpan());

                updateList.Add(new List<int>(nums.ToArray()));
            }
            else if (isParsingRules)
            {
                var nums = ParseNumbers(line, "|".AsSpan());

                if (rulesDict.TryGetValue(nums[0], out var list))
                {
                    list.AddRange(nums[1..]);
                }
                else
                {
                    rulesDict[nums[0]] = new List<int>(nums[1..].ToArray());
                }
            }
        }

        var total = 0;

        // process updates based on rules
        foreach (var update in updateList)
        {
            if (IsUpdateOrdered(update, rulesDict))
            {
                total += update[update.Count / 2];
            }
        }

        return $"{total}";
    }

    public static string Solve_2_Initial(string input)
    {
        var inputSpan = input.AsSpan();
        var isParsingRules = true;
        var rulesDict = new Dictionary<int, List<int>>();
        var updateList = new List<List<int>>();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);

            if (line.IsEmpty)
            {
                isParsingRules = false;
                continue;
            }
            else if (!isParsingRules)
            {
                var nums = ParseNumbers(line, ",".AsSpan());

                updateList.Add(new List<int>(nums.ToArray()));
            }
            else if (isParsingRules)
            {
                var nums = ParseNumbers(line, "|".AsSpan());

                if (rulesDict.TryGetValue(nums[0], out var list))
                {
                    list.AddRange(nums[1..]);
                }
                else
                {
                    rulesDict[nums[0]] = new List<int>(nums[1..].ToArray());
                }
            }
        }

        var total = 0;

        // process updates based on rules
        foreach (var update in updateList)
        {
            if (!IsUpdateOrdered(update, rulesDict))
            {
                var fixedUpdate = OrderUpdate(update, rulesDict);

                total += fixedUpdate[fixedUpdate.Count / 2];
            }
        }

        return $"{total}";
    }

    public static string Solve_1_Optimized(string input)
    {
        var (rules, updateList) = ParseInputOptimized(input);

        int total = 0;

        foreach (var update in updateList)
        {
            if (IsUpdateOrderedOptimized(update, rules))
            {
                total += update[update.Length / 2];
            }
        }

        return $"{total}";
    }

    public static string Solve_2_Optimized(string input)
    {
        var (rules, updateList) = ParseInputOptimized(input);

        int total = 0;

        foreach (var update in updateList)
        {
            if (!IsUpdateOrderedOptimized(update, rules))
            {
                var fixedUpdate = OrderUpdateOptimized(update, rules);

                total += fixedUpdate[fixedUpdate.Length / 2];
            }
        }

        return $"{total}";
    }

    private static bool IsUpdateOrdered(List<int> nums, Dictionary<int, List<int>> rules)
    {
        var indices = new Dictionary<int, int>();
        for (int i = 0; i < nums.Count; i++)
        {
            indices[nums[i]] = i;
        }

        foreach (var rule in rules)
        {
            int key = rule.Key;
            List<int> values = rule.Value;

            if (!indices.ContainsKey(key))
                continue;

            int keyIndex = indices[key];

            foreach (int value in values)
            {
                if (indices.TryGetValue(value, out int v) && v < keyIndex)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static List<int> OrderUpdate(List<int> updates, Dictionary<int, List<int>> rules)
    {   
        var dependencies = new Dictionary<int, HashSet<int>>();

        foreach (var rule in rules)
        {
            int key = rule.Key;
            foreach (int value in rule.Value)
            {
                if (!dependencies.TryGetValue(value, out HashSet<int> v))
                {
                    v = [];
                    dependencies[value] = v;
                }

                v.Add(key);
            }
        }

        updates.Sort((x, y) =>
        {
            if (dependencies.TryGetValue(x, out var xDeps) && xDeps.Contains(y))
                return 1;
            if (dependencies.TryGetValue(y, out var yDeps) && yDeps.Contains(x))
                return -1;
            return 0;
        });

        return updates;
    }

    private static (List<int>[] rules, List<int[]> updates) ParseInputOptimized(string input)
    {
        var inputSpan = input.AsSpan();
        var isParsingRules = true;
        var rulesDict = new Dictionary<int, HashSet<int>>();
        var updateList = new List<int[]>();

        while (!inputSpan.IsEmpty)
        {
            var line = ReadNextLine(ref inputSpan);

            if (line.IsEmpty)
            {
                isParsingRules = false;
                continue;
            }

            if (isParsingRules)
            {
                var nums = ParseNumbers(line, "|".AsSpan(),2);
                if (!rulesDict.TryGetValue(nums[0], out var set))
                {
                    set = new HashSet<int>();
                    rulesDict[nums[0]] = set;
                }

                foreach (var num in nums[1..])
                {
                    set.Add(num);
                }
            }
            else
            {
                var nums = ParseNumbers(line, ",".AsSpan(),24);
                updateList.Add(nums.ToArray());
            }
        }

        var rules = PrecomputeDependencies(99, rulesDict);
        return (rules, updateList);
    }

    private static bool IsUpdateOrderedOptimized(Span<int> nums, List<int>[] dependencies)
    {
        var indices = new int[100];
        Array.Fill(indices, -1);

        for (int i = 0; i < nums.Length; i++)
        {
            indices[nums[i]] = i;
        }

        for (int i = 0; i < dependencies.Length; i++)
        {
            var deps = dependencies[i];
            if (deps == null) continue;

            foreach (var dependent in deps)
            {
                if (indices[dependent] != -1 && indices[i] != -1 && indices[dependent] > indices[i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static Span<int> OrderUpdateOptimized(Span<int> updates, List<int>[] dependencies)
    {
        var sorted = updates.ToArray();
        Array.Sort(sorted, (x, y) =>
        {
            var xDeps = dependencies[x];
            var yDeps = dependencies[y];

            if (xDeps != null && xDeps.Contains(y)) return 1;
            if (yDeps != null && yDeps.Contains(x)) return -1;
            return 0;
        });

        return sorted;
    }


    private static List<int>[] PrecomputeDependencies(int maxNumber, Dictionary<int, HashSet<int>> rules)
    {
        var dependencies = new List<int>[maxNumber + 1];

        foreach (var rule in rules)
        {
            foreach (var dependent in rule.Value)
            {
                if (dependencies[dependent] == null)
                {
                    dependencies[dependent] = new List<int>();
                }
                dependencies[dependent].Add(rule.Key);
            }
        }

        return dependencies;
    }
}
