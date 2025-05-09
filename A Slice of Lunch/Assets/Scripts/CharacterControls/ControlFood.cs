using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ControlFood : MonoBehaviour /*, IPointerDownHandler, IPointerUpHandler*/
{
    public string TextureSFX = "";
    public string TableTextureSFX = "";

    bool onFood = false;
    Transform parentTransform;
    Vector3 originalPosition;
    
    private void Awake() {
        parentTransform = transform.parent;
        originalPosition = parentTransform.position;
    }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if (other.CompareTag("Food")) {
    //         onFood = true;
    //     }
    // }

    // private void OnTriggerStay2D(Collider2D other) {
    //     if (other.CompareTag("Food")) {
    //         onFood = true;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other) {
    //     if (other.CompareTag("Food")) {
    //         onFood = false;
    //     }
    // }

    // private IEnumerator HandleFoodCollision() {
    //     float numIterations = 50;
    //     float totalTime = 0.01f;
    //     float timePerIteration = totalTime / numIterations;
    //     Vector3 startPos = parentTransform.position;
    //     Vector3 endPos = originalPosition;
    //     for (float i = 0; i < totalTime; i+= timePerIteration)
    //     {
    //         parentTransform.position = Vector3.Lerp(startPos, endPos, i / totalTime);
    //         yield return new WaitForSeconds(timePerIteration);
    //     }
    // }

    // public void PlayGrabSFX()
    // {
    //     if (textureSFX == "FoodCrunch")
    //     {
    //         AudioManager.Instance.PlaySFX("FoodCrunch");
    //     }
    //     else if (textureSFX == "FoodSquish")
    //     {
    //         AudioManager.Instance.PlaySFX("FoodSquish");
    //     }
    // }

    // public void PlayDropSFX()
    // {
    //     if (tableTextureSFX == "Wood")
    //     {
    //         AudioManager.Instance.PlaySFX("PlaceOnWood");
    //     }
    //     else
    //     {
    //         AudioManager.Instance.PlaySFX("PlaceOnWood");
    //     }
    // }
}
