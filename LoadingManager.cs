using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    List<LoadingVolume> volumes = new List<LoadingVolume>();

    private void Awake()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        StartCoroutine(RecalculateLoadedScenes());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LoadingVolume"))
        {
            LoadingVolume volume = other.GetComponent<LoadingVolume>();
            if (volume != null && !volumes.Contains(volume))
            {
                AddToList(volume);
                StartCoroutine(RecalculateLoadedScenes());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LoadingVolume"))
        {
            LoadingVolume volume = other.GetComponent<LoadingVolume>();
            if (volume != null)
            {
                volumes.Remove(volume);
                StartCoroutine(RecalculateLoadedScenes());
            }
        }
    }

    public IEnumerator RecalculateLoadedScenes()
    {
        Dictionary<string, bool> sceneOverrides = new Dictionary<string, bool>();
        //SortList();

        //For each of the volumes, in order of the priority
        foreach (LoadingVolume volume in volumes)
        {
            //for each scene
            foreach (ScenePicker scene in volume.scenes)
            {
                if (sceneOverrides.ContainsKey(scene.sceneName))
                {
                    sceneOverrides[scene.sceneName] = scene.loaded;
                }
                else
                {
                    sceneOverrides.Add(scene.sceneName, scene.loaded);
                }
            }
        }

        string[] loadedScenes = GetLoadedScenes();

        //Load or unload any necessary scenes
        foreach (KeyValuePair<string, bool> scene in sceneOverrides)
        {
            //If the scene needs to be loaded
            if (scene.Value)
            {
                if (!loadedScenes.Contains(scene.Key))
                {
                    AsyncOperation async = SceneManager.LoadSceneAsync(scene.Key, LoadSceneMode.Additive);
                    async.allowSceneActivation = false;
                    while (!async.isDone)
                    {
                        if (async.progress >= 0.9f)
                        {
                            async.allowSceneActivation = true;
                            Debug.Log("Loaded Scene: " + scene.Key);
                        }
                        yield return null;
                    }
                    yield return null;
                }
            }
            //If the scene needs to be unloaded
            else
            {
                if (IsSceneLoaded(scene.Key))
                {
                    SceneManager.UnloadSceneAsync(scene.Key);
                }
            }
        }
    }

    public void AddToList(LoadingVolume volume)
    {
        for (int i = 0; i < volumes.Count; i++)
        {
            LoadingVolume currentVolume = volumes[i];
            if (currentVolume.priority > volume.priority)
            {
                volumes.Insert(i, volume);
                return;
            }
        }
        volumes.Add(volume);
    }

    bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals(sceneName))
            {
                return true;
            }
        }
        return false;
    }

    string[] GetLoadedScenes()
    {
        List<string> loadedScenes = new List<string>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i).name);
        }
        return loadedScenes.ToArray();
    }

    void SortList()
    {
        //continue looping untill the list is sorted.
        //In the case that we enter higher priority zones later (as we should) then this should take as little as 1-2 loops
        //In the case that we run in to a extremely low priority later, this will take count^2 amount of times.
        //Swap this for a better sorting algorithm later.
        bool isSorted = false;
        while (isSorted == false)
        {
            isSorted = true;
            for (int i = 0; i < volumes.Count - 1; i++)
            {
                LoadingVolume currentVolume = volumes[i];
                LoadingVolume nextVolume = volumes[i + 1];

                if (currentVolume.priority > nextVolume.priority)
                {
                    volumes.Remove(currentVolume);
                    volumes.Insert(i + 1, currentVolume);
                    isSorted = false;
                }
            }
        }
    }
}
