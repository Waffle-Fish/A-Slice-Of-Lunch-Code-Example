using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerRotate : MonoBehaviour
{
    bool rotating = false;
    bool onFood = false;
    PlayerSlice playerSlice;
    float degOffset;
    PlayerInputActions.PlayerActions playerActions;

    [Header("Food Collision Handler")]
    Collider2D foodCol;
    float foodPreviousZRot;
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
        if (rotating)
        {
            float newZRot = GetAngleToMouse() - degOffset + foodPreviousZRot;
            Vector3 rotatePoint = foodCol.bounds.center;
            // TODO: Have food rotate around center point of collider

            // foodCol.transform.parent.RotateAround(rotatePoint, Vector3.forward, newZRot * Time.deltaTime);

            // Vector3 vector = foodCol.transform.parent.position;
            // Quaternion quaternion = Quaternion.AngleAxis(newZRot, Vector3.forward);
            // Vector3 vector2 = vector - rotatePoint;
            // vector2 = quaternion * vector2;
            // vector = rotatePoint + vector2;
            // foodCol.transform.parent.position = vector;
            foodCol.transform.parent.rotation = Quaternion.Euler(0, 0, newZRot);
            // foodCol.transform.parent.Rotate(Vector3.forward, newZRot);


            // foodCol.transform.parent.transform.rotation = Quaternion.Euler(0, 0, newZRot);
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

        degOffset = GetAngleToMouse();
        foodCol.transform.parent.transform.rotation = Quaternion.Euler(0, 0, degOffset);



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

        foodPreviousZRot = foodCol.transform.eulerAngles.z;

        DisablePlacementObjects();
        foodCol.OverlapCollider(contactFilter2D.NoFilter(), results);
        foreach (var item in results)
        {
            if (!resultsTags.TryAdd(item.tag, 1)) resultsTags[item.tag]++;
        }
        if (resultsTags.ContainsKey("Border") || resultsTags.ContainsKey("Food"))
        {
            // Debug.Log("On Food");
            // StartCoroutine(HandleFoodCollision());
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
        foodPreviousZRot = new();
        initialSortingOrder = 0;
        foodSortingGroup = new();
    }

    // private IEnumerator HandleFoodCollision()
    // {
    //     float totalIterations = 50;
    //     float totalTime = 0.2f;
    //     float timePerIteration = totalTime / totalIterations;
    //     Vector3 startPos = foodCol.transform.parent.position;
    //     Vector3 endPos = foodPreviousZRot;
    //     for (float i = 0; i < totalIterations; ++i)
    //     {
    //         // Debug.Log("Handling Food Collision!");
    //         foodCol.transform.parent.position = Vector3.Lerp(startPos, endPos, i * timePerIteration / totalTime);
    //         yield return new WaitForSeconds(timePerIteration);
    //     }
    // }

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
