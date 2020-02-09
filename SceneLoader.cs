using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Jobs;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    Transform sceneRoot;

    [SerializeField]
    int maxObjectsPerFrame = 10;

    private void OnEnable()
    {
        StartActivateScene(maxObjectsPerFrame);
        sceneRoot.gameObject.SetActive(false);
    }

    public void StartActivateScene (int objectsPerFrame)
    {
        StartCoroutine(ActivateScene(objectsPerFrame));
    }

    IEnumerator ActivateScene(int objectsPerFrame)
    {
        int childCount = sceneRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {

            //If a prefab is forcing us to enable too many objects
            if (sceneRoot.GetChild(0).childCount > objectsPerFrame)
                yield return null;

            sceneRoot.GetChild(0).SetParent(transform);
            if (i % objectsPerFrame == 0)
            {
                yield return null;
            }
        }
    }
}
