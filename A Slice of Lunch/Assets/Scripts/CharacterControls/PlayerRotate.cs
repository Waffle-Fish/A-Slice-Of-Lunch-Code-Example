using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField][Min(0.001f)] float sensitivity = 1f;

    bool rotating = false;
    bool onFood = false;
    PlayerSlice playerSlice;
    float degOffset;
    PlayerInputActions.PlayerActions playerActions;
    Vector3 pivotPoint = Vector3.zero;

    [Header("Food Collision Handler")]
    Collider2D foodCol;
    Quaternion foodPreviousRot;
    int initialSortingOrder;
    SortingGroup foodSortingGroup;
    SpriteRenderer foodSpriteRenderer;

    [Header("Validate Placement Settings")]
    bool placeable = true;
    GameObject validPlacementObj;
    GameObject invalidPlacementObj;

    private void Awake()
    {
        playerSlice = GameObject.FindWithTag("Player").GetComponent<PlayerSlice>();
        playerActions = PlayerInputManager.Instance.PlayerActions;

        Transform childTransform = transform.GetChild(0);
        if (childTransform.childCount == 2)
        {
            validPlacementObj = childTransform.GetChild(0).gameObject;
            invalidPlacementObj = childTransform.GetChild(1).gameObject;
        }

        DisablePlacementObjects();
    }

    private void OnEnable()
    {
        playerActions.LeftClick.performed += PickUpFood;
        playerActions.LeftClick.canceled += ReleaseFood;
    }

    void OnDisable()
    {
        playerActions.LeftClick.performed -= PickUpFood;
        playerActions.LeftClick.canceled -= ReleaseFood;
    }

    private void Update()
    {
        Vector2 mouseDelta = PlayerInputManager.Instance.PointerDelta;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos);
        if (mouseDelta == Vector2.zero) return;
        
        if (rotating)
        {
            Vector2 foodToMouse = mousePos - (Vector2)pivotPoint;
            float cross = foodToMouse.x * mouseDelta.y - foodToMouse.y * mouseDelta.x;
            cross *= sensitivity * Time.deltaTime;
            foodCol.transform.parent.RotateAround(pivotPoint, Vector3.forward, cross);

            if (DetectOverlap())
            {
                placeable = false;
                EnableInvalidObj();
            }
            else
            {
                placeable = true;
                EnableValidObj();
            }
        }
    }

    private void PickUpFood(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        foodCol = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos));
        if (!(foodCol && foodCol.CompareTag("Food"))) return;

        // Debug.Log(foodCol.transform.parent.name + " is being picked up");
        rotating = true;

        foodSortingGroup = foodCol.transform.parent.GetComponent<SortingGroup>();
        foodSpriteRenderer = foodCol.transform.parent.GetComponent<SpriteRenderer>();
        initialSortingOrder = foodSortingGroup.sortingOrder;
        foodSortingGroup.sortingOrder = 10000;

        pivotPoint = foodCol.bounds.center;
        pivotPoint.z = 0;

        foodPreviousRot = foodCol.transform.rotation;

        PlayGrabSFX(foodCol.GetComponent<ControlFood>().TextureSFX);

        if (foodCol.TryGetComponent<FoodShadowManager>(out FoodShadowManager fsm)) fsm.UpdateShadowToPickedUp();
    }

    private void ReleaseFood(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!rotating || !foodCol) return;
        foodSpriteRenderer.color = Color.white;
        rotating = false;
        Transform parentTransform = foodCol.transform.parent;
        foodSortingGroup.sortingOrder = initialSortingOrder;
        ContactFilter2D contactFilter2D = new();
        List<Collider2D> results = new();
        Dictionary<string, int> resultsTags = new();

        DisablePlacementObjects();
        foodCol.OverlapCollider(contactFilter2D.NoFilter(), results);
        foreach (var item in results)
        {
            if (!resultsTags.TryAdd(item.tag, 1)) resultsTags[item.tag]++;
        }
        if (resultsTags.ContainsKey("Border") || resultsTags.ContainsKey("Food"))
        {
            Debug.Log("On Food");
            StartCoroutine(HandleFoodCollision());
        }
        else
        {
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
        if (foodCol.TryGetComponent<FoodShadowManager>(out FoodShadowManager fsm)) fsm.UpdateShadowToPlacedDown();
        Debug.Log("Finish releasing food");
    }

    private void ResetHandleCollisionVariables()
    {
        foodCol = new();
        foodPreviousRot = new();
        initialSortingOrder = 0;
        foodSortingGroup = new();
    }

    private IEnumerator HandleFoodCollision()
    {
        const float TOTAL_ITERATIONS = 50;
        const float TOTAL_TIME = 0.2f;
        float timePerIteration = TOTAL_TIME / TOTAL_ITERATIONS;
        Quaternion startRot = foodCol.transform.parent.rotation;
        Quaternion endRot = foodPreviousRot;
        for (float i = 0; i < TOTAL_ITERATIONS; ++i)
        {
            foodCol.transform.parent.rotation = Quaternion.Lerp(startRot, endRot, i * timePerIteration / TOTAL_TIME);
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

    private void DisablePlacementObjects()
    {
        if (!validPlacementObj || !invalidPlacementObj) return;
        validPlacementObj.SetActive(false);
        invalidPlacementObj.SetActive(false);

    }

    private void EnableValidObj()
    {
        if (!validPlacementObj || !invalidPlacementObj) return;
        validPlacementObj.SetActive(true);
        invalidPlacementObj.SetActive(false);
        if (foodSpriteRenderer) foodSpriteRenderer.color = Color.white;
    }

    private void EnableInvalidObj()
    {
        if (!validPlacementObj || !invalidPlacementObj) return;
        validPlacementObj.SetActive(false);
        invalidPlacementObj.SetActive(true);
        if (foodSpriteRenderer) foodSpriteRenderer.color = Color.red;
    }

    private bool DetectOverlap()
    {
        ContactFilter2D cf = new();
        cf.NoFilter();
        List<Collider2D> results = new();
        foodCol.OverlapCollider(cf, results);
        foreach (var col in results)
        {
            if (col.CompareTag("Food") || col.CompareTag("Border"))
            {
                return true;
            }
        }
        return false;
    }

    private float GetAngleToMouse()
    {
        if (!foodCol) return 0;
        Vector2 foodPos = (Vector2)foodCol.bounds.center;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(PlayerInputManager.Instance.MousePos);
        Vector2 dir = mouseWorldPos - foodPos;
        float sensitivity = 1;
        float angle = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg * sensitivity;
        return angle;
    }
}
