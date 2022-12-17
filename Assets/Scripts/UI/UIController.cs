using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIController : MonoBehaviour
{
    bool tutorial;
    public float tweenTime = 0.1f;
    public TextMeshProUGUI bottomText;
    GameObject bottomTextGameObject;
    public TextMeshProUGUI scoreText;
    public GameObject inventory;
    RectTransform inventoryRectTransform;
    public GameObject cmvcam, player;
    public ProgressBar progressBar;
    public float bottomTextFadeTime = 1f;
    public PostProcessingController postProcessingController;
    ScoreManager scoreManager;
    SFX sFX;
    public DragDrop[] dragDrop;
    GameplayManager gameplayManager;
    void Awake() {
        scoreManager = GetComponent<ScoreManager>();
    }
    void Start() {
        player.GetComponent<BoxCollider2D>().enabled = !inventory.activeSelf;
        // progressBar = GetComponentInChildren<ProgressBar>();
        inventoryRectTransform = inventory.GetComponent<RectTransform>();
        bottomTextGameObject = bottomText.gameObject;
        gameplayManager = FindObjectOfType<GameplayManager>();

        sFX = FindObjectOfType<SFX>();
        StartCoroutine(LateStart());
        if (FindObjectOfType<TutorialScene>()) tutorial = true;
    }
    IEnumerator LateStart() {
        yield return null;
        inventory.SetActive(false);
        cmvcam.SetActive(true);
        postProcessingController.UpdateDepthOfField(false);
        postProcessingController.UpdateVignette(false);
        player.GetComponent<BoxCollider2D>().enabled = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ChangeInventoryState();
        }
    }
    void ChangeInventoryState() {
        if (!inventory.activeSelf) {
            // activate inventory
            inventory.SetActive(true);
            cmvcam.SetActive(false); // problematic
            postProcessingController.UpdateDepthOfField(true);
            postProcessingController.UpdateVignette(true);
            player.GetComponent<BoxCollider2D>().enabled = false;
            LeanTween.scale(inventoryRectTransform, Vector3.one, tweenTime);
        } else {
            // disable inventory
            postProcessingController.UpdateDepthOfField(false);
            postProcessingController.UpdateVignette(false);
            cmvcam.SetActive(true);
            player.GetComponent<BoxCollider2D>().enabled = true;
            LeanTween.scale(inventoryRectTransform, Vector3.zero, tweenTime)
                .setOnComplete(() => inventory.SetActive(false));

            // ensures that no items floats in the inventory
            foreach (DragDrop dd in dragDrop) {
                dd.ReturnToOriginalPosition();
            }
        } 
        sFX.PlayInventorySFX();
    }
    public void ChangeBottomText(string _text) {
        bottomText.text = _text;
        LeanTween.value(bottomTextGameObject, 0f, 1f, tweenTime).setOnUpdate((val) => bottomText.alpha = val);
        // LeanTween.textAlpha(bottomTextRectTransform, 1f, bottomTextFadeTime).setEase(LeanTweenType.easeInCirc);
        StartCoroutine(StartTimerRemoveBottomText());
    }
    IEnumerator StartTimerRemoveBottomText() {
        yield return new WaitForSeconds(bottomTextFadeTime);
        // LeanTween.textAlpha(bottomTextRectTransform, 0f, bottomTextFadeTime).setEase(LeanTweenType.easeInCirc);
        LeanTween.value(bottomTextGameObject, 1f, 0f, tweenTime).setOnUpdate((val) => bottomText.alpha = val);
        // yield return new WaitForSeconds(TweenTime);
        // ChangeBottomText("");
    }
    public void UpdateScore() {
        scoreManager.CalculateScore();
    }
    public void UpdateProgressBar(float percentage) {
        // relate to enemy manager/spawner
        if (!tutorial && percentage >= 0.95f) {
            gameplayManager.EndGame();
        }
        progressBar.UpdateProgressBar(percentage);
    }
    public void UpdateKillColor(float intensity) {
        postProcessingController.UpdateVignetteColor(Color.Lerp(Color.black, Color.red, (intensity - 1) * 2));
    }
}
