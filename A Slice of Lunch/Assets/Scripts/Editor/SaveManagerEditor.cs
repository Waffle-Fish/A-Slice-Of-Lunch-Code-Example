#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveLevelData))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveLevelData SaveLevelDataScript = (SaveLevelData)target;

        if (GUILayout.Button("Save"))
        {
            SaveLevelDataScript.Save();
        }
    }
}

#endif