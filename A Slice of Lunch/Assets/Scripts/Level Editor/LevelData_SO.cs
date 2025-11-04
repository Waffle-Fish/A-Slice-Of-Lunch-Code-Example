using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData_SO : ScriptableObject
{
    public BoxType BoxType;
    [Tooltip("0 = Level 1, 7 = Level 8")]
    [Range(0, 7)]
    public int LevelNum;
    public GameObject FoodList;
    public GameObject LunchBox;
    // Sprite needs to be Tilable and 16/9
    public Sprite Background;
    List<GameObject> ExtraAsthetics;
    [Min(0)]
    public int TotalSlices;
}
