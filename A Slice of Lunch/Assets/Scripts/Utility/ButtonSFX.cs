using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonSFX : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] List<AudioClip> clickSFXS;
    [SerializeField] List<AudioClip> hoverSFXS;

    AudioSource audioSource;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSFXS.Count == 0) return;
        //audioSource.PlayOneShot(clickSFXS[Random.Range(0, clickSFXS.Count)]);
        AudioManager.Instance.PlaySFX(clickSFXS[Random.Range(0, clickSFXS.Count)]);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSFXS.Count == 0) return;
        //audioSource.PlayOneShot(hoverSFXS[Random.Range(0, hoverSFXS.Count)]);
        AudioManager.Instance.PlaySFX(hoverSFXS[Random.Range(0, hoverSFXS.Count)]);
    }
}
