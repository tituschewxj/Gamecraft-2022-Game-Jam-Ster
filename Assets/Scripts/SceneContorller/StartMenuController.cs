using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartMenuController : MonoBehaviour
{
    public string playSceneName;
    AsyncOperation loadingOperation;
    public GameObject opening, selection, bg;
    LevelLoader levelLoader;
    SFX sFX;
    // Start is called before the first frame update
    int state = 0;
    void Start()
    {
        // bg.GetComponent<Image>().color = Color.black;
        LeanTween.color(bg.GetComponent<Image>().rectTransform, Color.white, 1f);
        levelLoader = FindObjectOfType<LevelLoader>();
        sFX = FindObjectOfType<SFX>();
        // StartCoroutine(levelLoader.PreloadScene("Scene2"));
        
        
        // StartCoroutine(LoadAsyncScene());
    }
    // public void playPressed() {
    //     SceneManager.LoadSceneAsync(playSceneName);
    //     // SceneManager.SetActiveScene(SceneManager.GetSceneByName(playSceneName));
    // }
    // IEnumerator LoadAsyncScene() {
    //     loadingOperation = SceneManager.LoadSceneAsync(playSceneName, LoadSceneMode.Additive);
    //     loadingOperation.allowSceneActivation = false;
    //     while (!loadingOperation.isDone) {
    //         if (loadingOperation.progress >= 0.9f) {
    //             loadingOperation.allowSceneActivation = true;
    //         }
    //         yield return null;
    //     }
    // }
    // LevelLoader.Instance.LoadScene("Scene2");
    public void SwitchToSelection() {

        sFX.PlayClickSFX();
        StartCoroutine(WaitForTween(0.1f));
        LeanTween.color(bg.GetComponent<Image>().rectTransform, Color.black, 0.1f);
        LeanTween.color(bg.GetComponent<Image>().rectTransform, Color.white, 0.1f).setDelay(0.1f);

    }
    IEnumerator WaitForTween(float seconds) {
        yield return new WaitForSeconds(seconds);
        opening.SetActive(false);
        selection.SetActive(true);
    }
    public void SwitchToTutorial() {
        sFX.PlayClickSFX();
        LeanTween.color(bg.GetComponent<Image>().rectTransform, Color.black, 0.1f);
    }
    public void SwitchToPlay() {
        // load that scene
        sFX.PlayClickSFX();
        LeanTween.color(bg.GetComponent<Image>().rectTransform, Color.black, 0.1f);
    }
    // IEnumerator LoadAsyncScene() {
    //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene2");
    //     while (!asyncLoad.isDone) {
    //         yield return null;
    //     }
    // }
}
/*
load scene async not working
*/