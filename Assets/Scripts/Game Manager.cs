using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public delegate void Events();
    public event Events pauseEvent;
    public event Events unpauseEvent;
    public event Events colorThemeChanged;

    public void InvokeColorThemeChanged()
    {
        colorThemeChanged.Invoke();
    }

    private void Start()
    {
        gridController.pausegridRT.gameObject.SetActive(false);
        gridController.wingridRT.gameObject.SetActive(false);
        paused = true;
        cheat = false;
        settingsWindow.SetActive(false);
        recordsWindow.gameObject.SetActive(false);
        

        if (!save.LoadGame()) // If savefile does not exist, generate new board
        {
            PressedNewButton();
        }
        else // Load grid
        {
            gridController.GenerateGrid(playerData.puzzleData);
        }

        IEnumerator wait_ActivateCurrentTheme()
        {
            yield return null;
            ThemeManager.Instance.ActivateCurrentTheme();
        }
        StartCoroutine(wait_ActivateCurrentTheme());
    }

    private void Update()
    {
        if (!paused)
        {
            playerData.puzzleTime += Time.deltaTime;
        }
    }

    public GridController gridController;
    [SerializeField] PuzzleDealer puzzleDealer;
    public Save save;

    public bool fingerDown;
    public Cell fingerDownCell;

    public PlayerData playerData;
    public bool paused;

    public void PressedNewButton()
    {
        paused = true;
        playerData.puzzleData = puzzleDealer.CreateRandomPuzzleData();
        playerData.puzzleMoves = 0;
        playerData.puzzleTime = 0;
        gridController.GenerateGrid(playerData.puzzleData, true);
        save.SaveGame();
        gridController.pausegridRT.gameObject.SetActive(false);
        gridController.wingridRT.gameObject.SetActive(false);
    }

    readonly WaitForEndOfFrame WFEOF;
    public Cell cellZero;
    [SerializeField] bool cheat; // skips neighbor checking for easy testing
    public void Swap(Cell cellSwap)
    {
        int the_value = cellSwap.myValue;

        // Get coordinates of cells
        int cellSwap_row = 0;
        int cellSwap_col = 0;
        int cellZero_row = 0;
        int cellZero_col = 0;
        for (int r = 0; r < playerData.puzzleData.GetLength(0); r++) // For each row
        {
            for (int c = 0; c < playerData.puzzleData.GetLength(1); c++) // For each column
            {
                if (playerData.puzzleData[r, c] == 0)
                {
                    cellZero_row = r;
                    cellZero_col = c;
                }
                if (playerData.puzzleData[r, c] == cellSwap.myValue)
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
        if (cellZero_row > 0) neigborTop = playerData.puzzleData[cellZero_row - 1, cellZero_col];
        if (cellZero_row < playerData.puzzleData.GetLength(0) - 1) neigborBottom = playerData.puzzleData[cellZero_row + 1, cellZero_col];
        if (cellZero_col > 0) neigborLeft = playerData.puzzleData[cellZero_row, cellZero_col - 1];
        if (cellZero_col < playerData.puzzleData.GetLength(1) - 1) neigborRight = playerData.puzzleData[cellZero_row, cellZero_col + 1];
        if (cheat || the_value == neigborTop || the_value == neigborBottom || the_value == neigborLeft || the_value == neigborRight) // If any of them
        {
            StartCoroutine(performSwap());
        }

        IEnumerator performSwap()
        {
            // Swap visually
            gridController.GLG.enabled = false;
            if (playerData.animationsOn)
            {
                float animTime = 0.1f;
                for (float t = 0; t < animTime; t += Time.deltaTime)
                {
                    cellSwap.transform.position = Vector3.Lerp(cellSwap.transform.position, cellZero.transform.position, t / animTime);
                    yield return WFEOF;
                }
            }
            else
            {
                cellSwap.transform.position = cellZero.transform.position;
            }

            // Swap the data
            playerData.puzzleData[cellZero_row, cellZero_col] = the_value;
            playerData.puzzleData[cellSwap_row, cellSwap_col] = 0;

            // Update grid
            gridController.GenerateGrid(playerData.puzzleData);
            gridController.GLG.enabled = true;

            playerData.puzzleMoves++;
            paused = false;

            CheckWin();
        }
    }

    public void CheckWin()
    {
        // Crush 2D into 1D
        int[] linear_puzzleData = new int[playerData.puzzleData.Length];
        int counter = 0;
        foreach (var item in playerData.puzzleData)
        {
            linear_puzzleData[counter++] = item;
        }

        bool win = true;
        for (int i = 0; i < linear_puzzleData.Length - 1; i++) // Skip last cell because in solved puzzle it will be 0
        {
            if (linear_puzzleData[i] != i + 1) win = false; // If any of one element is in wrong place, didn't win
        }

        if (win)
        {
            gridController.wingridRT.gameObject.SetActive(true);
            paused = true;

            Record record = new()
            {
                puzzleTime = playerData.puzzleTime,
                puzzleMoves = playerData.puzzleMoves,
                rows = playerData.rows,
                columns = playerData.columns,
            };
            playerData.records.Add(record);
        }

        save.SaveGame();
    }

    [Header("Settings Scene References")]
    [SerializeField] TextMeshProUGUI widthText;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] TextMeshProUGUI animationSettingText;
    [SerializeField] GameObject settingsWindow;
    [SerializeField] RecordsWindow recordsWindow;
    private void UpdateData()
    {
        widthText.text = playerData.columns.ToString();
        heightText.text = playerData.rows.ToString();
        animationSettingText.text = playerData.animationsOn ? "ON" : "OFF";
    }
    public void IncreaseWidth()
    {
        if (playerData.columns < 10)
        {
            playerData.columns++;
        }
        else
        {
            playerData.columns = 3;
        }
        UpdateData();
        PressedNewButton();
    }
    public void IncreaseHeight()
    {
        if (playerData.rows < 10)
        {
            playerData.rows++;
        }
        else
        {
            playerData.rows = 3;
        }
        UpdateData();
        PressedNewButton();
    }
    public void SetActiveSettingsWindow(bool active)
    {
        UpdateData();
        settingsWindow.SetActive(active);
        save.SaveGame();
    }
    public void ToggleAnimations()
    {
        playerData.animationsOn = !playerData.animationsOn;
        UpdateData();
        save.SaveGame();
    }

    public void SetPause(bool pause)
    {
        paused = pause;
        save.SaveGame();

        if (paused) pauseEvent.Invoke();
        else unpauseEvent.Invoke();

        gridController.pausegridRT.gameObject.SetActive(pause);
    }

    public void SetActiveRecordsWindow(bool active)
    {
        recordsWindow.gameObject.SetActive(active);
        if (active)
        {
            recordsWindow.LoadRecordsData();
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

    public static string FormatTime(this float totalSeconds)
    {
        int minutes = (int)totalSeconds / 60;
        int seconds = (int)totalSeconds % 60;
        int milliseconds = (int)((totalSeconds - Math.Floor(totalSeconds)) * 1000);

        return minutes.ToString("0") + ":" + seconds.ToString("00") + "." + milliseconds.ToString("000");
    }
}
