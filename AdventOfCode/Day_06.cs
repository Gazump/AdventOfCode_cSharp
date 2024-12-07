using static AdventOfCode.Helpers.Test;
using static AdventOfCode.Helpers.InputHandler;
using static AdventOfCode.Helpers.OutputHandler;
using Spectre.Console;

namespace AdventOfCode;

public class Day_06 : BaseDay
{
    private readonly string _input;
    private readonly List<(string TestInput, string ExpectedSolve1, string ExpectedSolve2)> _testCases;
    private readonly bool _runTestCases = false;
    private readonly bool _runOptimizedSolutions = true;

    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private static readonly (int rowDelta, int colDelta)[] Deltas =
    [
        (-1, 0), // Up
        (0, 1),  // Right
        (1, 0),  // Down
        (0, -1)  // Left
    ];

    public Day_06()
    {
        _input = File.ReadAllText(InputFilePath);

        _testCases =
        [
            (
                """
                ....#.....
                .........#
                ..........
                ..#.......
                .......#..
                ..........
                .#..^.....
                ........#.
                #.........
                ......#...
                """,
                "41", // Expected result for part 1
                "6"  // Expected result for part 2
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
        var grid = ParseGrid(input);
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var guard = FindCharacterInGrid(grid, '^');
        grid[guard.Value.row, guard.Value.col] = 'X';

        var direction = Direction.Up;
        var done = false;
        var uniqueCount = 1;

        while (!done)
        {
            var (rowDelta, colDelta) = Deltas[(int)direction];
            var newRow = guard.Value.row + rowDelta;
            var newCol = guard.Value.col + colDelta;
            
            if (newRow < 0 || newCol < 0 || newRow >= rows || newCol >= cols) // exit grid
            {
                done = true;
            } 
            else if (grid[newRow, newCol] == '.') // move and increment
            {
                guard = (newRow, newCol);
                grid[newRow, newCol] = 'X';
                uniqueCount++;
            } 
            else if (grid[newRow, newCol] == 'X') // move no increment
            {
                guard = (newRow, newCol);
            }
            else // turn right at obstacle
            {
                direction = (Direction)(((int)direction + 1) % 4);
            }
        } 

        return $"{uniqueCount}";
    }

    public static string Solve_2_Initial(string input)
    {
        var initialGrid = ParseGrid(input);        
        var rows = initialGrid.GetLength(0);
        var cols = initialGrid.GetLength(1);
        var initialGuard = FindCharacterInGrid(initialGrid, '^');

        var successfulObstacle = 0;

        // try every spot (not efficient)
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var grid = DeepCopy(initialGrid);
                if (grid[r, c] == '.')
                {
                    // set obstacle
                    grid[r, c] = '#';

                    var guard = initialGuard;
                    grid[guard.Value.row, guard.Value.col] = 'X';

                    var tracker = new Dictionary<(int, int), Direction>
                    {
                        { (guard.Value.row, guard.Value.col), Direction.Up }
                    };

                    var direction = Direction.Up;
                    var done = false;

                    while (!done)
                    {
                        var (rowDelta, colDelta) = Deltas[(int)direction];
                        var newRow = guard.Value.row + rowDelta;
                        var newCol = guard.Value.col + colDelta;

                        if (newRow < 0 || newCol < 0 || newRow >= rows || newCol >= cols) // exit grid
                        {
                            done = true;
                        }
                        else if (grid[newRow, newCol] == '.') // move and increment
                        {
                            guard = (newRow, newCol);
                            grid[newRow, newCol] = 'X';
                            tracker.Add((newRow, newCol), direction);
                        }
                        else if (grid[newRow, newCol] == 'X') // move no increment
                        {
                            guard = (newRow, newCol);

                            //check if in loop
                            if (tracker[(newRow, newCol)] == direction)
                            {
                                successfulObstacle++;
                                done = true;
                            }
                        }
                        else // turn right at obstacle
                        {
                            direction = (Direction)(((int)direction + 1) % 4);
                        }
                    }
                }
            }
        }

        return $"{successfulObstacle}";
    }

    // pretty quick already < 1ms
    public static string Solve_1_Optimized(string input)
    {
        return Solve_1_Initial(input);
    }

    // some parallet processing to get from 2.7s down to <650ms.
    public static string Solve_2_Optimized(string input)
    {
        var initialGrid = ParseGrid(input);
        var rows = initialGrid.GetLength(0);
        var cols = initialGrid.GetLength(1);
        var initialGuard = FindCharacterInGrid(initialGrid, '^');

        int successfulObstacle = 0;
        
        var obstaclePositions = new List<(int row, int col)>();
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (initialGrid[r, c] == '.')
                {
                    obstaclePositions.Add((r, c));
                }
            }
        }

        object lockObj = new();
        Parallel.ForEach(obstaclePositions, (position) =>
        {
            var (r, c) = position;

            var grid = (char[,])initialGrid.Clone();
            grid[r, c] = '#';

            var guardPos = (initialGuard.Value.row, initialGuard.Value.col);
            var direction = Direction.Up;

            var visitedStates = new HashSet<(int row, int col, Direction dir)>();
            bool isLoop = false;

            while (true)
            {
                var state = (guardPos.row, guardPos.col, direction);
                if (visitedStates.Contains(state))
                {
                    isLoop = true;
                    break;
                }
                visitedStates.Add(state);

                var (rowDelta, colDelta) = Deltas[(int)direction];
                var newRow = guardPos.row + rowDelta;
                var newCol = guardPos.col + colDelta;

                if (newRow < 0 || newCol < 0 || newRow >= rows || newCol >= cols)
                {
                    break;
                }

                var cell = grid[newRow, newCol];
                if (cell == '.' || cell == '^')
                {
                    guardPos = (newRow, newCol);
                }
                else if (cell == '#' || cell == 'X')
                {
                    direction = (Direction)(((int)direction + 1) % 4);
                }
            }

            if (isLoop)
            {
                lock (lockObj)
                {
                    successfulObstacle++;
                }
            }
        });

        return $"{successfulObstacle}";
    }


}
