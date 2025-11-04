#if UNITY_EDITOR
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SaveLevelData : MonoBehaviour
{
    [SerializeField] LevelData currentLevelData;
    string DIRECTORY_PATH;

    private void Awake()
    {
        DIRECTORY_PATH = Application.dataPath + Path.AltDirectorySeparatorChar + "Level Data";
        if (!Directory.Exists(DIRECTORY_PATH)) Debug.LogError(DIRECTORY_PATH + " does not exist!");
    }

    [ContextMenu("Save Button")]
    public void Save()
    {
        string json = JsonUtility.ToJson(currentLevelData, true);
        string fileName = currentLevelData.BoxType.ToString() + currentLevelData.LevelNum.ToString()+".json";
        string filePath = Path.Combine(DIRECTORY_PATH, fileName);
        File.WriteAllText(filePath, json);
    }
}

#endif