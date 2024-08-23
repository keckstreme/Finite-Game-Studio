using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI movesText;
    [SerializeField] TextMeshProUGUI timeText;

    private void Update()
    {
        movesText.text = GameManager.Instance.playerData.puzzleMoves.ToString();
        timeText.text = GameManager.Instance.playerData.puzzleTime.FormatTime();
    }
}
