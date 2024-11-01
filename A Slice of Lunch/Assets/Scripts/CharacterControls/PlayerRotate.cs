using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    //vectors meant to instantiate position of mouse/cursor
    Vector3 prevPosition = Vector3.zero;
    Vector3 positionDelta = Vector3.zero;

    public RigidBody2d rb;
    // Update is called once per frame
    void Update()
    {
        if(input.GetMouseButton(0))
        {
            positionDelta = Input.mousePosition - prevPosition;
        }
    }
}
