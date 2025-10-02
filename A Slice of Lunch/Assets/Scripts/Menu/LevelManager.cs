using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SaveSystem))]
public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject JapaneseBox;
    List<Button> JapaneseButtons = new();
    [SerializeField] GameObject ItalianBox;
    List<Button> ItalianButtons = new();
    SaveSystem saveSystem;

    void Start()
    {
        saveSystem = GetComponent<SaveSystem>();

        JapaneseBox.GetComponentsInChildren<Button>(JapaneseButtons);
        ItalianBox.GetComponentsInChildren<Button>(ItalianButtons);

        DisableAllButtons(JapaneseButtons);
        DisableAllButtons(ItalianButtons);

        saveSystem.LoadData();
        if (saveSystem.PlayerData.BoxLevelData[0] > -1) EnableAllButtonsUpToALevel(JapaneseButtons, saveSystem.PlayerData.BoxLevelData[0]);
        if (saveSystem.PlayerData.BoxLevelData[1] > -1) EnableAllButtonsUpToALevel(ItalianButtons, saveSystem.PlayerData.BoxLevelData[1]);
    }

    private void DisableAllButtons(List<Button> buttons)
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

    private void EnableAllButtonsUpToALevel(List<Button> buttons, int level)
    {
        for (int i = 0; i <= level; i++)
        {
            buttons[i].interactable = true;
        }
    }
}
