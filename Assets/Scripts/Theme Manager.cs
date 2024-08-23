using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        GameManager.Instance.colorThemeChanged += UpdateColorsInList;
    }

    public ColorTheme colors;

    public List<ColorTheme> themes;

    public List<Image> accentColorableObjects;
    public List<Image> accentVibrantColorableObjects;
    public List<TextMeshProUGUI> textColorableObjects;
    public List<Image> thirdColorableObjects;

    public void ActivateNextTheme()
    {
        // Loop next theme
        GameManager.Instance.playerData.colorTheme++;
        if (GameManager.Instance.playerData.colorTheme > themes.Count - 1) GameManager.Instance.playerData.colorTheme = 0;
        ActivateCurrentTheme();
        GameManager.Instance.save.SaveGame();
    }

    public void ActivateCurrentTheme()
    {
        colors = themes[GameManager.Instance.playerData.colorTheme];

        GameManager.Instance.InvokeColorThemeChanged();
    }

    private void UpdateColorsInList() // This is for objects that has no script
    {
        Camera.main.backgroundColor = colors.gameBgColor;
        foreach (var item in accentColorableObjects)
        {
            item.color = colors.accentColor;
        }
        foreach (var item in accentVibrantColorableObjects)
        {
            item.color = colors.accentColorVibrant;
        }
        foreach (var item in textColorableObjects)
        {
            item.color = colors.textColor;
        }
        foreach (var item in thirdColorableObjects)
        {
            item.color = colors.thirdColor;
        }
    }
}

[System.Serializable]
public class ColorTheme
{
    public Color gameBgColor;
    public Color accentColor;
    public Color accentColorVibrant;
    public Color textColor;
    public Color thirdColor;
}