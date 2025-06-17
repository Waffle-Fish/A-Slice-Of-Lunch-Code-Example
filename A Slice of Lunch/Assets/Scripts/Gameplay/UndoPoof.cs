using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoPoof : MonoBehaviour
{
    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
    }

    void PlayPoofAnimation()
    {
        anim.Play("Poof");
    }
}
