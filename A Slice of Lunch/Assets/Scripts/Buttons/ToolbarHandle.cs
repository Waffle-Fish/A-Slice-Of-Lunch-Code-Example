using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolbarHandle : MonoBehaviour, IPointerClickHandler
{
    Animator animator;
    bool isOpen = true;

    private void Awake()
    {
        animator = transform.parent.GetComponent<Animator>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isOpen = !isOpen;
        animator.SetBool("Open", isOpen);
    }

    public void CloseToolbar()
    {
        isOpen = false;
        animator.SetBool("Open", isOpen);
    }
}
