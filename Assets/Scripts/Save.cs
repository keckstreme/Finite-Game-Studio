using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Save : MonoBehaviour
{
    public void SaveGame()
    {
        string saveFilePath = Application.persistentDataPath + "/data";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFilePath);
        bf.Serialize(file, GameManager.Instance.playerData);
        file.Close();
        Debug.Log("Game was successfully saved to file: " + saveFilePath);
    }

    public bool LoadGame()
    {
        string saveFilePath = Application.persistentDataPath + "/data";
        bool fileExists = File.Exists(saveFilePath);
        if (fileExists)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            GameManager.Instance.playerData = (PlayerData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game was successfully load from file: " + saveFilePath);
        }
        else
        {
            Debug.Log("Save file does not exist at: " + saveFilePath);
        }

        return fileExists;
    }
}

[System.Serializable]
public struct PlayerData
{
    public int[,] puzzleData;
    public float puzzleTime;
    public int puzzleMoves;
    public int rows;
    public int columns;
    public bool animationsOn;
    public int colorTheme;
}