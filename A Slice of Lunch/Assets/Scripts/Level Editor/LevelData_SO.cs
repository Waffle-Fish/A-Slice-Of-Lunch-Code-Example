using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData_SO : ScriptableObject
{
    [Header("Easy Data")]
    public BoxType BoxType;
    [Tooltip("0 = Level 1, 7 = Level 8")]
    [Range(0, 7)]
    public int LevelNum;
    [Min(0)] public int TotalSlices;

    [Header("Handle Food")]
    public List<FoodData<CuisineTypes.Japanese>> JapaneseFoodList;
    public List<FoodData<CuisineTypes.Italian>> ItalianFoodList;

    public static LevelData_SO Create(
        BoxType boxType, int levelNum, 
        List<FoodData<CuisineTypes.Japanese>> japaneseFoodList, List<FoodData<CuisineTypes.Italian>> italianFoodList
    )
    {
        LevelData_SO data = CreateInstance<LevelData_SO>();
        data.BoxType = boxType;
        data.LevelNum = levelNum;

        // Food List
        data.JapaneseFoodList = japaneseFoodList;
        data.ItalianFoodList = italianFoodList;
        return data;
    }
}

[Serializable]
public struct FoodData<T>
{
    public T foodType;
    public Vector3 position;
    public Vector3 rotation;
}


