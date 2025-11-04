using UnityEngine;
using UnityEngine.EventSystems;

public class SetUpNextLevel : MonoBehaviour, IPointerClickHandler 
{
    [SerializeField] BoxType boxType;
    [SerializeField] int nextLevel;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneDataToLoad.boxType = boxType;
        SceneDataToLoad.level = nextLevel;
    }
}
