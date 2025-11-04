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
    
    private void Awake()
    {
        string DIRECTORY_PATH = Application.dataPath + Path.AltDirectorySeparatorChar + "Level Data";
        string fileName = SceneDataToLoad.boxType.ToString() + SceneDataToLoad.level.ToString() + ".json";
        string filePath = Path.Combine(DIRECTORY_PATH, fileName);

        // File.ReadAllText(filePath);
        Debug.Log(File.ReadAllText(filePath));
    }
}
