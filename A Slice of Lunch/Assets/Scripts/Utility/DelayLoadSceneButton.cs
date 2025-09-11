using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class DelayLoadSceneButton : MonoBehaviour
{
    protected Button button;
    protected SceneController sc;
    [SerializeField]
    protected int sceneIndex = 0;
    [SerializeField]
    [Tooltip("This gets overriden if there is a playable director assigned")]
    protected float delay = 0;
    [SerializeField]
    [Tooltip("This overrides delay")]
    protected PlayableDirector timeline;

    protected void Awake() {
        
        button = GetComponent<Button>();
        if (timeline) {
            delay = (float)timeline.duration;
        }
    }

    protected virtual void Start() {
        sc = SceneController.Instance;
    }

    protected void OnEnable() {
        button.onClick.AddListener(() => LoadSceneCallback());
    }

    protected virtual void LoadSceneCallback() {
        if (timeline) timeline.gameObject.SetActive(true);
        sc.LoadScene(sceneIndex, delay);
    }

    protected void OnDisable() {
        button.onClick.RemoveAllListeners();
    }
}
