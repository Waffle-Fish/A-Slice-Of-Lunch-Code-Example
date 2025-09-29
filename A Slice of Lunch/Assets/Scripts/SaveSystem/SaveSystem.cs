using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum BoxType { Japan, Italy, French, Chinese }

public struct PlayerData
{
    public List<int> BoxLevelData;
    public List<bool> CutsceneData;

    public PlayerData(List<int> boxLevelData, List<bool> cutsceneData)
    {
        BoxLevelData = boxLevelData;
        CutsceneData = cutsceneData;
    }
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

    string FILE_PATH;

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
        FILE_PATH = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
        if (!File.Exists(FILE_PATH))
        {
            PlayerData = new PlayerData(new List<int> { 0, -1, -1 }, new List<bool> { false, false });
            string json = JsonUtility.ToJson(PlayerData);
            File.WriteAllText(FILE_PATH, json);
        }
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
        using (StreamWriter sw = new StreamWriter(FILE_PATH))
        {
            sw.Write(json);
        }
    }

    public void SaveCutsceneData()
    {
        UpdateCutsceneData(CutsceneID);
        string json = JsonUtility.ToJson(PlayerData);
        Debug.Log(json);
        using (StreamWriter sw = new StreamWriter(FILE_PATH))
        {
            sw.Write(json);
        }
    }

    public void LoadData()
    {
        string json = string.Empty;
        using (StreamReader reader = new StreamReader(FILE_PATH))
        {
            json = reader.ReadToEnd();
        }

        PlayerData = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log("Loaded Json File: " + PlayerData);
    }

    #region Utilities
    public void ResetLevels()
    {
        for (int i = 0; i < PlayerData.BoxLevelData.Count; i++)
        {
            PlayerData.BoxLevelData[i] = -1;

        }
        PlayerData.BoxLevelData[0] = 0;

        for (int i = 0; i < PlayerData.CutsceneData.Count; i++)
        {
            PlayerData.CutsceneData[i] = false;
        }
        
        string json = JsonUtility.ToJson(PlayerData);
        using (StreamWriter sw = new StreamWriter(FILE_PATH))
        {
            sw.Write(json);
        }
    }

    public void UpdateBoxLevelData(int boxId, int level)
    {
        if (boxId < 0 || boxId >= PlayerData.BoxLevelData.Count || level <= PlayerData.BoxLevelData[boxId]) return;
        PlayerData.BoxLevelData[boxId] = level;
    }

    public void UpdateCutsceneData(int cutsceneIndex)
    {
        if (cutsceneIndex < 0 || cutsceneIndex >= PlayerData.BoxLevelData.Count) return;
        PlayerData.CutsceneData[cutsceneIndex] = true;
    }
    #endregion
}
