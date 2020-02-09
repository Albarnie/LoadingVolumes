using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingVolume : MonoBehaviour
{
    [SerializeField]
    public int priority;

    public ScenePicker[] scenes;

    [SerializeField]
    bool isGlobal = false;

    private void OnEnable()
    {
        if(isGlobal)
        {
            LoadingManager manager = FindObjectOfType<LoadingManager>();
            manager.AddToList(this);
            StartCoroutine(manager.RecalculateLoadedScenes());
        }
    }
}
