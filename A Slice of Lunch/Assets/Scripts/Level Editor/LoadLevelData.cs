using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SceneDataToLoad
{
    public static BoxType boxType;
    public static int level;
}

public class LoadLevelData : MonoBehaviour
{
    [SerializeField]
    GameObject FoodList;
    [SerializeField]
    List<CuisineAesthetics> CuisineAesthetics;

    [Header("Food Pools")]
    public List<FoodPool<CuisineTypes.Japanese>> JapanesePool;
    public List<FoodPool<CuisineTypes.Italian>> ItalianPool;

    private void Awake()
    {
        string DIRECTORY_PATH = Application.dataPath + Path.AltDirectorySeparatorChar + "Level Data";
        string fileName = SceneDataToLoad.boxType.ToString() + SceneDataToLoad.level.ToString() + ".json";
        string filePath = Path.Combine(DIRECTORY_PATH, fileName);

        // File.ReadAllText(filePath);
        Debug.Log(File.ReadAllText(filePath));
    }
    
    private void Start() {
        HashSet<BoxType> cuisinesInList = new();
        for(int i = 0; i < CuisineAesthetics.Count; i++)
        {
            BoxType type = CuisineAesthetics[i].BoxType;
            if (cuisinesInList.Contains(type))
            {
                Debug.LogError("Duplicate " + type + " at " + i + " in Cuisine Aesthetics");
            }
        }
    }

    // Need to load
    // - Background
    // Lunchb
}

[Serializable]
public struct FoodPool<T>
{
    public T foodType;
    public ObjectPooler foodPool;
}

[Serializable]
public struct CuisineAesthetics
{
    public BoxType BoxType;
    public Aesthetics SceneObjects;
}

[Serializable]
public struct Aesthetics
{
    public GameObject LunchBox;
    // Sprite needs to be Tilable and 16/9
    public Sprite Background;
    List<GameObject> ExtraAsthetics;
}