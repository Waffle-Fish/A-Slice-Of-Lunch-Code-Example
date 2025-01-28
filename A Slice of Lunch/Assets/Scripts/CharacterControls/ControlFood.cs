using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ControlFood : MonoBehaviour
{
    bool dragging = false;
    PlayerControls playerControls;
    SortingGroup sortingGroup;
    int initialSortingOrder;
    Transform parentTransform;
    Vector3 originalPosition;
    PolygonCollider2D polygonCollider2D;
    
    private void Awake() {
        playerControls = GameObject.FindWithTag("Player").GetComponent<PlayerControls>();
        sortingGroup = GetComponentInParent<SortingGroup>();
        initialSortingOrder = sortingGroup.sortingOrder;
        parentTransform = transform.parent;
        originalPosition = parentTransform.position;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void OnMouseDown() {
        if (playerControls.IsHoldingKnife) return;
        // TODO:
        // Center food slice to center of mouse, or to where it clicked
        dragging = true;
        sortingGroup.sortingOrder = 10000;
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
        if (resultsTags.ContainsKey("Food") || resultsTags.ContainsKey("Border")) {
            HandleFoodCollision();
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
        if (dragging) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            transform.parent.position = pos;
            
        }
    }

    private void HandleFoodCollision() {
        parentTransform.position = originalPosition;
    }
}
