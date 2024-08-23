using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RecordsWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI movesList;
    [SerializeField] TextMeshProUGUI timeList;
    [SerializeField] TextMeshProUGUI boardList;

    public void LoadRecordsData()
    {
        string moves = string.Empty;
        string time = string.Empty;
        string board = string.Empty;

        List<Record> recordsSorted = GameManager.Instance.playerData.records.OrderBy(x => x.puzzleMoves).ToList();

        foreach (Record record in recordsSorted)
        {
            moves += record.puzzleMoves.ToString() + "\n";
            time += record.puzzleTime.FormatTime() + "\n";
            board += record.columns.ToString() + "x" + record.rows.ToString() + "\n";
        }

        movesList.text = moves;
        timeList.text = time;
        boardList.text = board;
    }
}
