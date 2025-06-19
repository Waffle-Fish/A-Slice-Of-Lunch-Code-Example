using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ControlFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Audio Settings")]
    public string TextureSFX = "";
    public string TableTextureSFX = "";   

    [Header("Validate Placement Settings")]
    GameObject validPlacementObj;
    GameObject invalidPlacementObj;

    [Header("Default Variables")]
    bool placeable = true;
    Vector2 offset;
    Transform parentTransform;
    Vector3 prevPos;
    SortingGroup sortingGroup;
    int originalSortingOrder = 0;

    private void Awake()
    {
        parentTransform = transform.parent;
        prevPos = parentTransform.position;
        sortingGroup = GetComponentInParent<SortingGroup>();
        if (transform.childCount == 2)
        {
            validPlacementObj = transform.GetChild(0).gameObject;
            invalidPlacementObj = transform.GetChild(1).gameObject;
        }
    }

    private void Start()
    {
        prevPos = parentTransform.position;
        originalSortingOrder = sortingGroup.sortingOrder;
        DisablePlacementObjects();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food") || collision.CompareTag("Border"))
        {
            placeable = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Food") || other.CompareTag("Border"))
        {
            placeable = false;
            EnableInvalidObj();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        placeable = true;
        EnableValidObj();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        offset = newPosition - parentTransform.position;
        sortingGroup.sortingOrder = 1000;
        EnableValidObj();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        newPosition -= (Vector3)offset;
        newPosition.z = parentTransform.position.z;
        parentTransform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        offset = Vector2.zero;
        DisablePlacementObjects();
        if (!placeable)
        {
            placeable = true;
            StartCoroutine(HandleFoodCollision());
        }
        else
        {
            sortingGroup.sortingOrder = originalSortingOrder;
            prevPos = parentTransform.position;
        }
    }

    private IEnumerator HandleFoodCollision()
    {
        float totalIterations = 50;
        float totalTime = 0.2f;
        float timePerIteration = totalTime / totalIterations;
        Vector3 startPos = parentTransform.position;
        Vector3 endPos = prevPos;
        for (float i = 0; i < totalIterations; ++i)
        {
            // Debug.Log("Handling Food Collision!");
            parentTransform.position = Vector3.Lerp(startPos, endPos, i * timePerIteration / totalTime);
            yield return new WaitForSeconds(timePerIteration);
        }
        sortingGroup.sortingOrder = originalSortingOrder;
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
    }

    private void EnableInvalidObj()
    {
        if (!validPlacementObj || !invalidPlacementObj) return;
        validPlacementObj.SetActive(false);
        invalidPlacementObj.SetActive(true);
    }
    
    public void PlayGrabSFX()
    {
        if (TextureSFX == "FoodCrunch")
        {
            AudioManager.Instance.PlaySFX("FoodCrunch");
        }
        else if (TextureSFX == "FoodSquish")
        {
            AudioManager.Instance.PlaySFX("FoodSquish");
        }
    }

    public void PlayDropSFX()
    {
        if (TableTextureSFX == "Wood")
        {
            AudioManager.Instance.PlaySFX("PlaceOnWood");
        }
        else
        {
            AudioManager.Instance.PlaySFX("PlaceOnWood");
        }
    }
}