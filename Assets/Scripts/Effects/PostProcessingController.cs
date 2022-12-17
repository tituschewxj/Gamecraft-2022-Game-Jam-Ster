using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
public class PostProcessingController : MonoBehaviour
{
    // public PostProcessVolume postProcessVolume;
    public Volume volume;
    public float tweenTime = 0.1f, minVignetteIntensity = 0.1f, maxVignetteIntensity = 0.3f, minDepthOfFieldFocus = 1, maxDepthOfFieldFocus = 300;
    DepthOfField depthOfField;
    Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        if (!volume.profile.TryGet<DepthOfField>(out depthOfField)) Debug.LogWarning("PostProcessingController: No DepthOfField");
        if (!volume.profile.TryGet<Vignette>(out vignette)) Debug.LogWarning("PostProcessingController: No Vignette");
    }
    public void UpdateDepthOfField(bool active) {
        depthOfField.active = active;
        if (active) {
            LeanTween.value(gameObject, minDepthOfFieldFocus, maxDepthOfFieldFocus, tweenTime).setOnUpdate((val) => {
                depthOfField.focalLength.value = val;
            });
        } else {
            LeanTween.value(gameObject, maxDepthOfFieldFocus, minDepthOfFieldFocus, tweenTime).setOnUpdate((val) => {
                depthOfField.focalLength.value = val;
            });
        }
    }
    public void UpdateVignette(bool higherIntensity) {
        if (higherIntensity) {
            LeanTween.value(gameObject, minVignetteIntensity, maxVignetteIntensity, tweenTime).setOnUpdate((val) => {
                vignette.intensity.value = val;
            });
        } else {
            LeanTween.value(gameObject, maxVignetteIntensity, minVignetteIntensity, tweenTime).setOnUpdate((val) => {
                vignette.intensity.value = val;
            });
        }
    }
    public void UpdateVignetteColor(Color color) {
        // LeanTween.value(gameObject, vignette.color.value, color, tweenTime).setOnUpdate((col) => {
        //     vignette.color.value = col;
        // });
        // doesn't work
    }
}
