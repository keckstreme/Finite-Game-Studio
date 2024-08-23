using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public ColorTheme colors;

    public List<ColorTheme> themes;

    public void ActivateNextTheme()
    {

    }
}

[System.Serializable]
public class ColorTheme
{
    public Color bgColor;
    public Color accentColor;
    public Color thirdColor;
}