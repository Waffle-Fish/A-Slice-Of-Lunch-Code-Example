using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutsceneDeciderButton : DelayLoadSceneButton
{
    [SerializeField] private int CutsceneID_ToCheck = 0;
    [SerializeField]
    [Tooltip("This is the scene to go to if they've already scene the cutscene")]
    protected int altSceneIndex = 0;
    private SaveSystem saveSystem;

    protected override void Start()
    {
        base.Start();
        saveSystem = SaveSystem.Instance;
    }

    protected override void LoadSceneCallback()
    {
        saveSystem.LoadData();
        bool hasSeenCutscene = saveSystem.PlayerData.CutsceneData[CutsceneID_ToCheck];
        if (timeline) timeline.gameObject.SetActive(true);
        if (hasSeenCutscene) sc.LoadScene(altSceneIndex, delay);
        else sc.LoadScene(sceneIndex, delay);
    }
}
