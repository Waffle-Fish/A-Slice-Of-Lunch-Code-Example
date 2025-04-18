using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ProcessWin : MonoBehaviour
{
    PlayableDirector pd;
    Button button;
    ParticleSystem stars;
    private void Awake() {
        button = GetComponent<Button>();
        pd = GetComponent<PlayableDirector>();
        stars = GetComponent<ParticleSystem>();
    }

    public void Win() {
        pd.Play();
        StartCoroutine(ActivateParticles());
    }

    IEnumerator ActivateParticles() {
        yield return new WaitForSeconds((float)pd.playableAsset.duration);
        if(stars) stars.Play();
    }
}
