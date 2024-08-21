using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDealer : MonoBehaviour
{
    public int rows;
    public int columns;

    public int[,] CreateRandomPuzzleData()
    {
        int[,] puzzle = new int[rows, columns]; // Create empty puzzle array

        int[] randomizedElements = CreateRandomizedSequence(rows * columns); // Create randomized values

        // Put randomized values to array
        int counter = 0;
        for (int r = 0; r < rows; r++) // For each row
        {
            for (int c = 0; c < columns; c++) // For each column
            {
                puzzle[r, c] = randomizedElements[counter++];
            }
        }

        //puzzle.Print();

        return puzzle;
    }

    private int[] CreateRandomizedSequence(int length)
    {
        int[] randomizedElements = new int[length];
        for (int i = 0; i < randomizedElements.Length; i++) // Organized values
        {
            randomizedElements[i] = i;
        }

        return randomizedElements.Shuffle(columns, rows);
    }
}
