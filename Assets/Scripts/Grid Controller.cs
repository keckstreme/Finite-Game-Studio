using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    [SerializeField] RectTransform gridRT;
    [SerializeField] RectTransform canvasRT;
    [SerializeField] List<Cell> cells = new();
    public GridLayoutGroup GLG;

    public void GenerateGrid(int[,] gridData, bool fresh = false)
    {
        int rowCount = gridData.GetLength(0);
        int columnCount = gridData.GetLength(1);

        // Crush 2D into 1D
        int[] linear_gridData = new int[gridData.Length];
        int counter = 0;
        foreach (var item in gridData)
        {
            linear_gridData[counter++] = item;
        }

        // Place cells
        if (rowCount <= columnCount) // Wide or square
        {
            // Limit grid column count
            GLG.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GLG.constraintCount = columnCount;

            // Resize cells - consider columns
            float cellSize = (canvasRT.rect.width - GLG.padding.horizontal + GLG.spacing.x) / columnCount - GLG.spacing.x;
            GLG.cellSize = new(cellSize, cellSize);

            // Set grid bg width to be as wide as possible
            gridRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasRT.rect.width);

            // Set grid bg height
            float height = GLG.padding.vertical + rowCount * cellSize + (rowCount - 1) * GLG.spacing.y;
            gridRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        else // Tall
        {
            // Limit grid row count
            GLG.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            GLG.constraintCount = rowCount;

            // Resize cells - consider rows
            float cellSize = (canvasRT.rect.width - GLG.padding.horizontal + GLG.spacing.x) / rowCount - GLG.spacing.x;
            GLG.cellSize = new(cellSize, cellSize);

            // Set grid bg height to be as wide as possible
            gridRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasRT.rect.width);

            // Set grid bg width
            float height = GLG.padding.vertical + columnCount * cellSize + (columnCount - 1) * GLG.spacing.y;
            gridRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, height);
        }

        if (fresh && GameManager.Instance.playerData.animationsOn)
        {
            ActivateCellsStylized(gridData, linear_gridData);
        }
        else
        {
            ActivateCellsStandard(gridData, linear_gridData);
        }
    }

    private void ActivateCellsStandard(int[,] gridData, int[] linear_gridData)
    {
        // Activate correct amount of cells
        for (int i = 0; i < cells.Count; i++)
        {
            bool activate = i < gridData.Length;
            cells[i].gameObject.SetActive(activate);
            if (activate)
            {
                cells[i].LoadCell(linear_gridData[i], true);
            }
        }
    }

    public int shuffleAnimationStyleCounter = 0;
    private void ActivateCellsStylized(int[,] gridData, int[] linear_gridData)
    {
        int columnCount = gridData.GetLength(1);
        int rowCount = gridData.GetLength(0);

        List<Cell> requiredCells = new(); // This will be used for animations

        // Activate all required cells and fill requiredCells
        for (int i = 0; i < cells.Count; i++)
        {
            bool activate = i < gridData.Length;
            cells[i].gameObject.SetActive(activate);
            if (activate)
            {
                cells[i].LoadCell(linear_gridData[i]);
                requiredCells.Add(cells[i]);
            }
        }

        if (shuffleAnimationStyleCounter == 0) StartCoroutine(sudden());
        if (shuffleAnimationStyleCounter == 1) StartCoroutine(random());
        if (shuffleAnimationStyleCounter == 2) StartCoroutine(rowDesc());
        if (shuffleAnimationStyleCounter == 3) StartCoroutine(columnAsc());
        if (shuffleAnimationStyleCounter == 4) StartCoroutine(rowAsc());
        if (shuffleAnimationStyleCounter == 5) StartCoroutine(columnDesc());

        shuffleAnimationStyleCounter++;
        if (shuffleAnimationStyleCounter > 5)
        {
            shuffleAnimationStyleCounter = 0;
        }

        IEnumerator sudden()
        {
            foreach (Cell cell in requiredCells)
            {
                cell.StartAnimation();
            }
            yield return null;
        }

        IEnumerator random()
        {
            foreach (Cell cell in requiredCells)
            {
                cell.HideMe();
            }
            // Activate shuffled cells
            requiredCells.ShuffleCells();

            foreach (Cell cell in requiredCells)
            {
                cell.StartAnimation();
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        IEnumerator rowDesc()
        {
            // Activate rows
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    requiredCells[i * columnCount + j].StartAnimation();
                }
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }

        IEnumerator rowAsc()
        {
            // Activate rows
            for (int i = rowCount; i > 0; i--)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    requiredCells[(i - 1) * columnCount + j].StartAnimation();
                }
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }

        IEnumerator columnDesc()
        {
            // Activate columns
            for (int i = 0; i < columnCount; i++) // For every column i
            {
                for (int j = 0; j < rowCount; j++)
                {
                    requiredCells[columnCount * j + i].StartAnimation();
                }
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }

        IEnumerator columnAsc()
        {
            // Activate columns
            for (int i = columnCount; i > 0; i--) // For every column i
            {
                for (int j = 0; j < rowCount; j++)
                {
                    requiredCells[columnCount * j + i - 1].StartAnimation();
                }
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }

        IEnumerator topleft()
        {
            for (int i = 0; i < requiredCells.Count; i++)
            {
                print(gridData[i, i]);
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }
    }
}
