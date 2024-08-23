using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Animator animator;
    [SerializeField] RectTransform RT;
    public int myValue;
    public int myRow;
    public int myColumn;

    private void Start()
    {
        GameManager.Instance.pauseEvent += PauseEvent;
        GameManager.Instance.unpauseEvent += UnpauseEvent;
    }

    private void PauseEvent()
    {
        if (myValue == 0) return;
        Setcolor(ThemeManager.Instance.colors.accentColor, ThemeManager.Instance.colors.accentColor);
    }

    private void UnpauseEvent()
    {
        if (myValue == 0) return;
        Setcolor(ThemeManager.Instance.colors.accentColor, ThemeManager.Instance.colors.thirdColor);
    }

    public void LoadCell(int value, bool immediate = false)
    {
        myValue = value;

        if (immediate && myValue != 0)
        {
            Setcolor(ThemeManager.Instance.colors.accentColor, ThemeManager.Instance.colors.thirdColor);
        }
        else
        {
            HideMe(); // Hide for animation
        }

        if (value == 0) // Empty cell
        {
            GameManager.Instance.cellZero = this;
        }
        else
        {
            text.text = value.ToString();
        }

        // Set font size
        text.fontSize = GameManager.Instance.gridController.GLG.cellSize.x / 2f;
    }

    public void StartAnimation()
    {
        if (myValue == 0) return;
        Setcolor(ThemeManager.Instance.colors.accentColor, ThemeManager.Instance.colors.thirdColor);
        animator.SetTrigger("Start");
    }

    public void Setcolor(Color bg_, Color text_)
    {
        bg.color = bg_;
        text.color = text_;
    }

    public void PressedOnMe()
    {
        GameManager.Instance.Swap(this);
    }

    public void HideMe()
    {
        Setcolor(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0));
    }

    public void PointerDown(bool isTrue)
    {
        GameManager.Instance.fingerDown = isTrue;
        GameManager.Instance.fingerDownCell = this;
    }

    public void PointerEnter() // Activate fingerDownCell if pointer entered to empty tile (basically user dragged finger)
    {
        if (GameManager.Instance.fingerDown)
        {
            if (myValue == 0)
            {
                GameManager.Instance.fingerDown = false;
                GameManager.Instance.fingerDownCell.PressedOnMe();
            }
        }
    }
}
