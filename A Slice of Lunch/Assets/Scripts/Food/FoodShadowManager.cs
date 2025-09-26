using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodShadowManager : MonoBehaviour
{
    [SerializeField] Transform shadowTransform;
    [SerializeField] Vector3 placedDownOffset = Vector3.zero;
    [SerializeField] float placedDownScale = 1;
    [SerializeField] Vector3 pickedUpOffset = Vector3.zero;
    [SerializeField] float pickedUpScale = 1;

    float prevZRot = 0;

    // Start is called before the first frame update
    void Start()
    {
        // TEMP DISABLE SHADOWS
        shadowTransform.gameObject.SetActive(false);
        // TEMP DISABLE SHADOWS

        UpdateShadowToPlacedDown();
        prevZRot = transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        if (CheckZRotChange())
        {
            UpdateShadowPos();
        }
    }

    public void UpdateShadowToPlacedDown()
    {
        shadowTransform.localPosition = placedDownOffset;
        shadowTransform.localScale = Vector3.one * placedDownScale;
    }

    public void UpdateShadowToPickedUp()
    {
        shadowTransform.localPosition = pickedUpOffset;
        shadowTransform.localScale = Vector3.one * pickedUpScale;
    }

    private bool CheckZRotChange()
    {
        bool returnVal = prevZRot != transform.rotation.eulerAngles.z;
        prevZRot = transform.rotation.eulerAngles.z;
        return returnVal;
    }

    private void UpdateShadowPos()
    {
        float zRot = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float x = placedDownOffset.x - Mathf.Cos(zRot);
        float y = placedDownOffset.y - Mathf.Sin(zRot);
        shadowTransform.localPosition = new Vector3(x, y, 0);
    }
}
