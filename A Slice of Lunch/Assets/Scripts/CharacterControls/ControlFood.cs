using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ControlFood : MonoBehaviour
{
    bool dragging = false;
    bool onFood = false;
    PlayerSlice playerSlice;
    SortingGroup sortingGroup;
    int initialSortingOrder;
    Transform parentTransform;
    Vector3 originalPosition;
    PolygonCollider2D polygonCollider2D;

    Vector3 mouseOffset = Vector3.zero;
    
    private void Awake() {
        playerSlice = GameObject.FindWithTag("Player").GetComponent<PlayerSlice>();
        sortingGroup = GetComponentInParent<SortingGroup>();
        initialSortingOrder = sortingGroup.sortingOrder;
        parentTransform = transform.parent;
        originalPosition = parentTransform.position;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Food")) {
            onFood = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Food")) {
            onFood = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Food")) {
            onFood = false;
        }
    }

    private void OnMouseDown() {
        dragging = true;
        sortingGroup.sortingOrder = 10000;
        mouseOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        mouseOffset.z = 0f;
    }

    private void OnMouseUp() {
        dragging = false;
        sortingGroup.sortingOrder = initialSortingOrder;
        ContactFilter2D contactFilter2D = new();
        List<Collider2D> results = new();
        Dictionary<string, int> resultsTags = new();
        polygonCollider2D.OverlapCollider(contactFilter2D, results);
        foreach (var item in results)
        {
            if(!resultsTags.TryAdd(item.tag, 1)) resultsTags[item.tag]++;
        }
        if (resultsTags.ContainsKey("Border") || onFood) {
            StartCoroutine(HandleFoodCollision());
        } else {
            originalPosition = parentTransform.position;
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
    }

    private void Update() {
        Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.parent.position = pos - mouseOffset;
        }
    }

    private IEnumerator HandleFoodCollision() {
        float numIterations = 50;
        float totalTime = 0.01f;
        float timePerIteration = totalTime / numIterations;
        Vector3 startPos = parentTransform.position;
        Vector3 endPos = originalPosition;
        for (float i = 0; i < totalTime; i+= timePerIteration)
        {
            parentTransform.position = Vector3.Lerp(startPos, endPos, i / totalTime);
            yield return new WaitForSeconds(timePerIteration);
        }
    }
}
