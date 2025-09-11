using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum BoxType { Japan, Italy, French, Chinese }

public struct PlayerData
{
    public List<int> BoxLevelData;
    public List<bool> CutsceneData;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    [Header("Next Level To Unlock")]
    [SerializeField] BoxType BoxID = 0;
    [Tooltip("0 = Level 1 \n7 = Level 8")]
    [SerializeField][Range(0, 7)] int Level = 0;
    [Header("Cutscenes")]
    [SerializeField] int CutsceneID = 0;

    [Header("Test Variables")]
    [SerializeField] bool TestSave = false;
    [SerializeField] bool ResetPlayerdata = false;

    public PlayerData PlayerData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple SaveSystems in this scene!");
            Destroy(this);
        }
        else Instance = this; 
    }

    private void Start()
    {
        PlayerData = new PlayerData();
        LoadData();
    }

    private void Update()
    {
        // if (TestSave)
        // {
        //     SaveBoxLevelData();
        //     TestSave = false;
        // }
        // if (ResetPlayerdata)
        // {
        //     ResetLevels();
        //     ResetPlayerdata = false;
        // }
    }

    public void SaveBoxLevelData()
    {
        UpdateBoxLevelData((int)BoxID, Level);
        string json = JsonUtility.ToJson(PlayerData);
        Debug.Log("Saved to box: " + json);
        using (StreamWriter sw = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json"))
        {
            sw.Write(json);
        }
    }

    public void SaveCutsceneData()
    {
        UpdateCutsceneData(CutsceneID);
        string json = JsonUtility.ToJson(PlayerData);
        Debug.Log(json);
        using (StreamWriter sw = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json"))
        {
            sw.Write(json);
        }
    }

    public void LoadData()
    {
        string json = string.Empty;
        using (StreamReader reader = new StreamReader(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json"))
        {
            json = reader.ReadToEnd();
        }

        PlayerData = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log("Loaded Json File: " + PlayerData);
    }

    #region Utilities
    private void ResetLevels()
    {
        for (int i = 0; i < PlayerData.BoxLevelData.Count; i++)
        {
            PlayerData.BoxLevelData[i] = -1;
        }
        PlayerData.BoxLevelData[0] = 1;

        for (int i = 0; i < PlayerData.CutsceneData.Count; i++)
        {
            PlayerData.CutsceneData[i] = false;
        }
    }

    public void UpdateBoxLevelData(int boxId, int level)
    {
        if (boxId < 0 || boxId >= PlayerData.BoxLevelData.Count) return;
        PlayerData.BoxLevelData[boxId] = level;
    }

    public void UpdateCutsceneData(int cutsceneIndex)
    {
        if (cutsceneIndex < 0 || cutsceneIndex >= PlayerData.BoxLevelData.Count) return;
        PlayerData.CutsceneData[cutsceneIndex] = true;
    }
    #endregion
}
