using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] GridController gridController;
    [SerializeField] PuzzleDealer puzzleDealer;

    int[,] puzzleData;

    public void PressedNewButton()
    {
        puzzleData = puzzleDealer.CreateRandomPuzzleData();
        gridController.GenerateGrid(puzzleData, true);
    }

    readonly WaitForEndOfFrame WFEOF;
    public Cell cellZero;
    public void Swap(Cell cellSwap)
    {
        int the_value = cellSwap.myValue;

        // Get coordinates of cells
        int cellSwap_row = 0;
        int cellSwap_col = 0;
        int cellZero_row = 0;
        int cellZero_col = 0;
        for (int r = 0; r < puzzleData.GetLength(0); r++) // For each row
        {
            for (int c = 0; c < puzzleData.GetLength(1); c++) // For each column
            {
                if (puzzleData[r, c] == 0)
                {
                    cellZero_row = r;
                    cellZero_col = c;
                }
                if (puzzleData[r, c] == cellSwap.myValue)
                {
                    cellSwap_row = r;
                    cellSwap_col = c;
                }
            }
        }

        // Skip if not neigbor to empty cell
        int neigborTop = -1;
        int neigborBottom = -1;
        int neigborLeft = -1;
        int neigborRight = -1;
        if (cellZero_row > 0) neigborTop = puzzleData[cellZero_row - 1, cellZero_col];
        if (cellZero_row < puzzleData.GetLength(0) - 1) neigborBottom = puzzleData[cellZero_row + 1, cellZero_col];
        if (cellZero_col > 0) neigborLeft = puzzleData[cellZero_row, cellZero_col - 1];
        if (cellZero_col < puzzleData.GetLength(1) - 1) neigborRight = puzzleData[cellZero_row, cellZero_col + 1];
        if (the_value == neigborTop || the_value == neigborBottom || the_value == neigborLeft || the_value == neigborRight) // If any of them
        {
            StartCoroutine(swapAnim());
        }

        IEnumerator swapAnim()
        {
            // Swap visually
            gridController.GLG.enabled = false;
            const float animTime = 0.1f;
            for (float t = 0; t < animTime; t += Time.deltaTime)
            {
                cellSwap.transform.position = Vector3.Lerp(cellSwap.transform.position, cellZero.transform.position, t / animTime);
                yield return WFEOF;
            }

            // Swap the data
            puzzleData[cellZero_row, cellZero_col] = the_value;
            puzzleData[cellSwap_row, cellSwap_col] = 0;

            // Update grid
            gridController.GenerateGrid(puzzleData);

            gridController.GLG.enabled = true;
        }
    }
}

public static class Extensions
{
    public static void Print(this int[,] array) // works!
    {
        try
        {
            int rowCount = array.GetLength(0);
            int columnCount = array.GetLength(1);

            for (int i = 0; i < rowCount; i++) // Iterate over rows
            {
                string row = "";
                for (int j = 0; j < columnCount; j++) // Iterate over columns
                {
                    row += array[i, j] + " ";
                }
                Debug.Log(row.Trim());
            }
        }
        catch (Exception)
        {
            Debug.LogError("error");
        }
    }

    public static int[] Shuffle(this int[] array, int col, int row) // Guaranteed solveable shuffle.
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (array[n], array[k]) = (array[k], array[n]); // Swap values
        }

        if (!array.IsSolvable(col, row))
        {
            Debug.LogWarning("reshuffle");
            Shuffle(array, col, row);
        }
        return array;
    }

    public static List<Cell> ShuffleCells(this List<Cell> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (array[n], array[k]) = (array[k], array[n]); // Swap values
        }

        return array;
    }

    // An inversion is when a higher-numbered tile precedes a lower-numbered tile in the array.
    private static int CountInversions(int[] puzzle)
    {
        int inversions = 0;
        for (int i = 0; i < puzzle.Length; i++)
        {
            for (int j = i + 1; j < puzzle.Length; j++)
            {
                if (puzzle[i] > puzzle[j] && puzzle[i] != 0 && puzzle[j] != 0)
                {
                    inversions++;
                }
            }
        }
        return inversions;
    }

    // Determine the row position of the blank (zero) tile from the bottom of the grid.
    private static int GetBlankRowFromBottom(int[] puzzle, int row)
    {
        int blankIndex = Array.IndexOf(puzzle, 0);
        int blankRow = blankIndex / row;
        int rowFromBottom = row - blankRow;
        return rowFromBottom;
    }

    public static bool IsSolvable(this int[] puzzle, int column, int row)
    {
        int inversions = CountInversions(puzzle);
        int blankRowFromBottom = GetBlankRowFromBottom(puzzle, row);

        // If the grid width is odd, the puzzle is solvable if the number of inversions is even.
        if (column % 2 != 0)
        {
            return inversions % 2 == 0;
        }
        else
        {
            // If the grid width is even, the puzzle is solvable if:
            // 1. The blank is on an even row from the bottom (1, 3, 5, ...) and the number of inversions is odd.
            // 2. The blank is on an odd row from the bottom (0, 2, 4, ...) and the number of inversions is even.
            if (blankRowFromBottom % 2 == 0)
            {
                return inversions % 2 != 0;
            }
            else
            {
                return inversions % 2 == 0;
            }
        }
    }
}
