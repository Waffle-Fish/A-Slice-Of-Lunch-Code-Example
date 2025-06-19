using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ControlFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string TextureSFX = "";
    public string TableTextureSFX = "";

    bool placeable = true;
    Vector2 offset;
    Transform parentTransform;
    Vector3 prevPos;
    SortingGroup sortingGroup;
    int originalSortingOrder = 0;

    [Header("Validate Placement Sprite")]
    SpriteRenderer validPlaceSpriteRenderer;

    private void Awake()
    {
        parentTransform = transform.parent;
        prevPos = parentTransform.position;
        sortingGroup = GetComponentInParent<SortingGroup>();
        validPlaceSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        prevPos = parentTransform.position;
        originalSortingOrder = sortingGroup.sortingOrder;
        validPlaceSpriteRenderer.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food") || collision.CompareTag("Border"))
        {
            placeable = false;
            validPlaceSpriteRenderer.color = Color.red;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Food") || other.CompareTag("Border"))
        {
            placeable = false;
            validPlaceSpriteRenderer.color = Color.red;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        placeable = true;
        validPlaceSpriteRenderer.color = Color.green;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        offset = newPosition - parentTransform.position;
        sortingGroup.sortingOrder = 1000;
        validPlaceSpriteRenderer.gameObject.SetActive(true);
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
        validPlaceSpriteRenderer.gameObject.SetActive(false);
        if (!placeable)
        {
            placeable = true;
            StartCoroutine(ProcessReturn());
        }
        else
        {
            sortingGroup.sortingOrder = originalSortingOrder;
            prevPos = parentTransform.position;
        }
    }

    private IEnumerator HandleFoodCollision()
    {
        float numIterations = 50;
        float totalTime = 0.01f;
        float timePerIteration = totalTime / numIterations;
        Vector3 startPos = parentTransform.position;
        Vector3 endPos = prevPos;
        for (float i = 0; i < totalTime; i += timePerIteration)
        {
            parentTransform.position = Vector3.Lerp(startPos, endPos, i / totalTime);
            yield return new WaitForSeconds(timePerIteration);
        }
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

    private IEnumerator ProcessReturn()
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
}