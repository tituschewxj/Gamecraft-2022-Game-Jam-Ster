using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    private bool load = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Update is called once per frame
    // public IEnumerator PreloadScene(string sceneName) {
    //     var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    //     asyncOperation.allowSceneActivation = false;
    //     while (!asyncOperation.isDone) {
    //         yield return null;
            
    //         if (asyncOperation.progress >= 0.9f) {
    //             Debug.Log("Loaded scene");
    //             if (load) {
    //                 asyncOperation.allowSceneActivation = true;
    //                 load = false;
    //                 break;
    //             }
    //         } else {
    //             Debug.Log("Loading scene");
    //         }
            
    //     }
    // }
    // public void LoadScene() {
    //     load = true;
    // }
    public void LoadSceneNow(string sceneName) {
        StopAllCoroutines();
        StartCoroutine(LoadAlt(sceneName));
    }
    IEnumerator LoadAlt(string sceneName) {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone) {
            yield return null;
            if (asyncOperation.progress >= 0.9f) {
                Debug.Log("Loaded scene");
                asyncOperation.allowSceneActivation = true;
                load = false;
                break;
            } else {
                Debug.Log("Loading scene");
            }
            
        }
    }
    public void SelectDifficulty(int difficulty) {
        PlayerStats.Difficulty = difficulty;
    }
}
