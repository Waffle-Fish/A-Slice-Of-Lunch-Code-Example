using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private Camera myCam;
    private Vector3 screenPosition;
    //private float angleOffset;
    private float angle;

    private void Start() {
        myCam = Camera.main;
    }
    
    private void Update()
    {
        Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D =  Physics2D.GetRayIntersection(clickRay);
        if (hit2D.collider != null && hit2D.collider.CompareTag("Food"))
        {
        RotateFood(hit2D.collider);
            

        }   
    }

    private void RotateFood(Collider2D col)
    {
        Vector3 mousePos = myCam.ScreenToWorldPoint(Input.mousePosition);
            if (col == Physics2D.OverlapPoint(mousePos))
            {
                screenPosition = myCam.WorldToScreenPoint(transform.position);
                Vector3 vec3 = Input.mousePosition - screenPosition;
                //angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) * Mathf.Rad2Deg;
                angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;
                col.transform.eulerAngles = new Vector3(0, 0, angle);
                Debug.Log("I am rotating!");

            }

    }
}