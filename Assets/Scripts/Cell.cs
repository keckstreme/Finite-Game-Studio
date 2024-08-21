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
        if (value == 0) // Empty cell
        {
            text.gameObject.SetActive(false);
            bg.enabled = false;

            GameManager.Instance.cellZero = this;
        }
        else
        {
            text.gameObject.SetActive(true);
            bg.enabled = true;
            bg.color = ThemeManager.Instance.accentColor;
            text.text = value.ToString();
        }
        if (immediate)
        {
            Setcolor(ThemeManager.Instance.accentColor, ThemeManager.Instance.thirdColor);
        }
        else
        {
            Setcolor(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0)); // Hide for animation
        }
    }

    public void StartAnimation()
    {
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
}
