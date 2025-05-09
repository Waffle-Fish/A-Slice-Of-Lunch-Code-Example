using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMoveFood : MonoBehaviour
{
    bool dragging = false;
    bool onFood = false;
    PlayerSlice playerSlice;
    Vector3 mouseOffset = Vector3.zero;
    PlayerInputActions.PlayerActions playerActions;

    [Header("Food Collision Handler")]
    Collider2D foodCol;
    Vector3 foodPreviousPosition;
    int initialSortingOrder;
    SortingGroup foodSortingGroup;

    
    private void Awake() {
        playerSlice = GameObject.FindWithTag("Player").GetComponent<PlayerSlice>();
        playerActions = PlayerInputManager.Instance.PlayerActions;
    }

    private void OnEnable() {
        playerActions.LeftClick.performed += PickUpFood;
        playerActions.LeftClick.canceled += ReleaseFood;
    }

    void OnDisable()
    {
        playerActions.LeftClick.performed -= PickUpFood;
        playerActions.LeftClick.canceled -= ReleaseFood;
    }

    private void Update() {
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos);
            pos.z = 0f;
            foodCol.transform.parent.position = pos - mouseOffset;
        }
    }

    private void PickUpFood(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        foodCol = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos));
        if (!(foodCol && foodCol.CompareTag("Food"))) return;

        // Debug.Log(foodCol.transform.parent.name + " is being picked up");
        StopCoroutine(nameof(HandleFoodCollision));
        dragging = true;

        foodSortingGroup = foodCol.transform.parent.GetComponent<SortingGroup>();
        initialSortingOrder = foodSortingGroup.sortingOrder;
        foodSortingGroup.sortingOrder = 10000;
        foodPreviousPosition = foodCol.transform.parent.transform.position;
        mouseOffset = Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos) - foodCol.transform.position;
        mouseOffset.z = 0f;

        PlayGrabSFX(foodCol.GetComponent<ControlFood>().TextureSFX);
    }

    private void ReleaseFood(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!dragging || !foodCol) return;
        dragging = false;
        Transform parentTransform = foodCol.transform.parent;
        foodSortingGroup.sortingOrder = initialSortingOrder;
        ContactFilter2D contactFilter2D = new();
        List<Collider2D> results = new();
        Dictionary<string, int> resultsTags = new();
        

        foodCol.OverlapCollider(contactFilter2D.NoFilter(), results);
        foreach (var item in results)
        {
            if(!resultsTags.TryAdd(item.tag, 1)) resultsTags[item.tag]++;
        }
        if (resultsTags.ContainsKey("Border") || resultsTags.ContainsKey("Food")) {
            // Debug.Log("On Food");
            StartCoroutine(HandleFoodCollision());
        } else {
            // originalPosition = parentTransform.position;
            if (!resultsTags.ContainsKey("Food")) return;

            // Put the food on top of the other food
            // Doesnt work
            int maxLayer = parentTransform.GetComponent<SortingGroup>().sortingOrder;
            foreach (var item in results)
            {
                int itemLayer = item.transform.parent.GetComponent<SortingGroup>().sortingOrder;
                if (itemLayer > maxLayer) maxLayer = itemLayer + 1;
            }
            parentTransform.GetComponent<SortingGroup>().sortingOrder = maxLayer;
        }

        PlayDropSFX(foodCol.GetComponent<ControlFood>().TableTextureSFX);
    }

    private void ResetHandleCollisionVariables() {
        foodCol = new();
        foodPreviousPosition = new();
        initialSortingOrder = 0;
        foodSortingGroup = new();
    }

    private IEnumerator HandleFoodCollision() {
        float totalIterations = 50;
        float totalTime = 0.2f;
        float timePerIteration = totalTime / totalIterations;
        Vector3 startPos = foodCol.transform.parent.position;
        Vector3 endPos = foodPreviousPosition;
        for (float i = 0; i < totalIterations; ++i)
        {
            // Debug.Log("Handling Food Collision!");
            foodCol.transform.parent.position = Vector3.Lerp(startPos, endPos, i * timePerIteration / totalTime);
            yield return new WaitForSeconds(timePerIteration);
        }
    }

    private void PlayGrabSFX(string textureSFX)
    {
        switch (textureSFX)
        {
            case "FoodCrunch":
                AudioManager.Instance.PlaySFX("FoodCrunch");
                break;
            case "FoodSquish":
                AudioManager.Instance.PlaySFX("FoodSquish");
                break;
            case "FoodRice":
                AudioManager.Instance.PlaySFX("FoodRice");
                break;
            default:
                AudioManager.Instance.PlaySFX("FoodRice");
                break;
        }
    }

    private void PlayDropSFX(string tableTextureSFX)
    {
        switch (tableTextureSFX)
        {
            case "Wood":
                AudioManager.Instance.PlaySFX("Wood");
                break;
            default:
                AudioManager.Instance.PlaySFX("Wood");
                break;
        }
    }
}
