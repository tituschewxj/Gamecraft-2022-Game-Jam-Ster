using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBar : MonoBehaviour
{
    public float tweenTime = 0.1f;
    float rectTransformWidth;
    public RectTransform progressBarRectTransform;
    void Start()
    {
        rectTransformWidth = GetComponent<RectTransform>().sizeDelta.x;
        // progressBarRectTransform = GetComponentInChildren<RectTransform>();
        progressBarRectTransform.sizeDelta = new Vector2(0, progressBarRectTransform.sizeDelta.y);
    }
    public void UpdateProgressBar(float percentage)
    {
        // Debug.Log("progress bar updated");
        Vector2 newSizeDelta = new Vector2(rectTransformWidth * percentage, progressBarRectTransform.sizeDelta.y);
        LeanTween.size (progressBarRectTransform, newSizeDelta, tweenTime);

    }
}
