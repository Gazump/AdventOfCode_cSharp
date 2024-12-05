using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;
using Spectre.Console;

namespace AdventOfCode;

public class Day_04 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    public Day_04()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                MMMSXXMASM
                MSAMXMSMSA
                AMXSXMAAMM
                MSAMASMSMX
                XMASAMXAMM
                XXAMMXXAMA
                SMSMSASXSS
                SAXAMASAAA
                MAMMMXMMMM
                MXMXAXMASX
                """,
                "18", // Expected result for part 1
                "9"  // Expected result for part 2
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
        var word = "XMAS";
        
        var grid = ParseGrid(input);

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        var directions = new (int, int)[]
        {
                (-1, 0), (1, 0), (0, -1), (0, 1),
                (-1, -1), (-1, 1), (1, -1), (1, 1)
        };

        var count = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                foreach (var (directionRow, directionCol) in directions)
                {   
                    count += CountFrom(row, col, directionRow, directionCol, rows, cols, grid, word);
                }
            }
        }

        return $"{count}";
    }

    public static string Solve_2_Initial(string input)
    {
        var grid = ParseGrid(input);

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        var totalCount = 0;

        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < cols - 1; col++)
            {
                totalCount += CountMASCrossShapes(row, col, rows, cols, grid);
            }
        }

        return $"{totalCount}";
    }

    // Not a huge performance gain here, and I can't seem to get it under 2ms.
    // Parallel processing overhead is too much for such a small input size.
    public static string Solve_1_Optimized(string input)
    {
        var word = "XMAS";
        var grid = ParseGrid(input);

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        int wordLength = word.Length;

        var directions = new int[,]
        {
        { -1,  0 }, { 1,  0 }, { 0, -1 }, { 0,  1 },
        { -1, -1 }, { -1,  1 }, { 1, -1 }, { 1,  1 }
        };

        int count = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] != word[0])
                    continue;

                for (int d = 0; d < directions.GetLength(0); d++)
                {
                    int dirRow = directions[d, 0];
                    int dirCol = directions[d, 1];
             
                    int endRow = row + (wordLength - 1) * dirRow;
                    int endCol = col + (wordLength - 1) * dirCol;

                    if (endRow < 0 || endRow >= rows || endCol < 0 || endCol >= cols)
                        continue;

                    bool match = true;
                    for (int i = 1; i < wordLength; i++)
                    {
                        int currentRow = row + i * dirRow;
                        int currentCol = col + i * dirCol;

                        if (grid[currentRow, currentCol] != word[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        count++;
                    }
                }
            }
        }

        return $"{count}";
    }

    // Initial solution runs <1ms so I'm happy with it's optimization already
    public static string Solve_2_Optimized(string input)
    {
        return Solve_2_Initial(input);
    }

    private static int CountFrom(int row, int col, int directionRow, int directionCol, int rows, int cols, char[,] grid, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int currentRow = row + i * directionRow;
            int currentCol = col + i * directionCol;

            if (currentRow < 0 || currentRow >= rows || currentCol < 0 || currentCol >= cols || grid[currentRow, currentCol] != word[i])
                return 0;
        }
        return 1;
    }

    private static int CountMASCrossShapes(int centerRow, int centerCol, int rows, int cols, char[,] grid)
    {
        if (centerRow - 1 < 0 || centerRow + 1 >= rows || centerCol - 1 < 0 || centerCol + 1 >= cols || grid[centerRow, centerCol] != 'A')
            return 0;

        int count = 0;

        if (grid[centerRow - 1, centerCol - 1] == 'M' &&
            grid[centerRow - 1, centerCol + 1] == 'M' &&
            grid[centerRow + 1, centerCol + 1] == 'S' &&
            grid[centerRow + 1, centerCol - 1] == 'S')
        {
            count++;
        }

        if (grid[centerRow - 1, centerCol - 1] == 'S' &&
            grid[centerRow - 1, centerCol + 1] == 'M' &&
            grid[centerRow + 1, centerCol + 1] == 'M' &&
            grid[centerRow + 1, centerCol - 1] == 'S')
        {
            count++;
        }

        if (grid[centerRow - 1, centerCol - 1] == 'S' &&
            grid[centerRow - 1, centerCol + 1] == 'S' &&
            grid[centerRow + 1, centerCol + 1] == 'M' &&
            grid[centerRow + 1, centerCol - 1] == 'M')
        {
            count++;
        }

        if (grid[centerRow - 1, centerCol - 1] == 'M' &&
            grid[centerRow - 1, centerCol + 1] == 'S' &&
            grid[centerRow + 1, centerCol + 1] == 'S' &&
            grid[centerRow + 1, centerCol - 1] == 'M')
        {
            count++;
        }

        return count;
    }
}
