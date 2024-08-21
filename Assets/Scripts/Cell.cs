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
    public int myValue;
    public int myRow;
    public int myColumn;

    public void LoadCell(int value, bool immediate = false)
    {
        myValue = value;

        if (immediate && myValue != 0)
        {
            Setcolor(ThemeManager.Instance.accentColor, ThemeManager.Instance.thirdColor);
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
    }

    public void StartAnimation()
    {
        if (myValue == 0) return;
        Setcolor(ThemeManager.Instance.accentColor, ThemeManager.Instance.thirdColor);
        animator.SetTrigger("Start");
    }

    public void Setcolor(Color accent, Color third)
    {
        bg.color = accent;
        text.color = third;
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
