using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelData
{
    
    public BoxType BoxType;
    public int LevelNum;
    public GameObject FoodList;
    public GameObject LunchBox;
    // Sprite needs to be Tilable and 16/9
    public Sprite Background;
    List<GameObject> ExtraAsthetics;
    public int TotalSlices;


}
