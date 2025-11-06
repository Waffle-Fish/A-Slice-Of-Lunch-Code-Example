#if UNITY_EDITOR
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[Serializable]
public struct LevelData
{
    public BoxType BoxType;
    public int LevelNum;
    public List<GameObject> FoodList;
    public int TotalSlices;
}

[ExecuteInEditMode]
public class SaveLevelData : MonoBehaviour
{
    [SerializeField] LevelData currentLevelData;
    string DIRECTORY_PATH;

    private void Awake()
    {
        DIRECTORY_PATH = Application.dataPath + Path.AltDirectorySeparatorChar + "Level Data" + Path.AltDirectorySeparatorChar + "ScriptableObjects";
        // LevelData_SO
        if (!Directory.Exists(DIRECTORY_PATH)) Debug.LogError(DIRECTORY_PATH + " does not exist!");
    }

    [ContextMenu("Save Button")]
    public void Save()
    {
        LevelData_SO levelData = ConvertSerializeDataToScriptableData();
        string json = JsonUtility.ToJson(currentLevelData, true);
        string fileName = currentLevelData.BoxType.ToString() + currentLevelData.LevelNum.ToString() + ".json";
        string filePath = Path.Combine(DIRECTORY_PATH, fileName);
        File.WriteAllText(filePath, json);
    }
    
    private LevelData_SO ConvertSerializeDataToScriptableData()
    {
        LevelData_SO data = ScriptableObject.CreateInstance<LevelData_SO>();
        data.BoxType = currentLevelData.BoxType;
        data.LevelNum = currentLevelData.LevelNum;
        data.TotalSlices = currentLevelData.TotalSlices;

        

        return data;
    }
}

#endif