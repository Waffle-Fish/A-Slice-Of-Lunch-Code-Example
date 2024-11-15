using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class DelayLoadSceneButton : MonoBehaviour
{
    Button button;
    SceneController sc;
    [SerializeField]
    private int buildIndex = 0;
    [SerializeField]
    [Tooltip("This gets overriden if there is a playable director assigned")]
    private float delay = 0;
    [SerializeField]
    [Tooltip("This overrides delay")]
    private PlayableDirector timeline;

    private void Awake() {
        
        button = GetComponent<Button>();
        if (timeline) {
            delay = (float)timeline.duration;
        }
    }

    private void Start() {
        sc = SceneController.Instance;
    }

    private void OnEnable() {
        button.onClick.AddListener(() => LoadSceneCallback());
    }

    private void LoadSceneCallback() {
        if (timeline) timeline.gameObject.SetActive(true);
        sc.LoadScene(buildIndex, delay);
    }

    private void OnDisable() {
        button.onClick.RemoveAllListeners();
    }
}
