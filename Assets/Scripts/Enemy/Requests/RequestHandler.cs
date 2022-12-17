using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RequestHandler : MonoBehaviour
{
    int numItems;
    Image[] images = new Image[9];
    Image image;
    public GameObject[] imagesGameObject;
    public Transform playerTransform;
    RectTransform rectTransform;
    public TierSystem tierSystem;
    
    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        // imagesGameObject = GetComponentsInChildren<GameObject>(); // doesn't work
        for (int i = 0; i < imagesGameObject.Length; i++) {
            images[i] = imagesGameObject[i].GetComponent<Image>();
        }
        image = GetComponent<Image>();
        // rectGLGHeight = scrollRect.GetComponent<GridLayoutGroup>().cellSize.y;
        // rectGLGSpacing = scrollRect.GetComponent<GridLayoutGroup>().spacing.y;
        HideRequest();
    }
    void UpdateRequestUI() {
        // updates size of rect
        rectTransform.sizeDelta = new Vector2(numItems * 2, rectTransform.sizeDelta.y);
    }
    
    public void ShowRequest(Request request, Vector2 position) {
        // ui
        image.enabled = true;
        numItems = request.itemObjects.Length; // object reference not set to instance
        for (int i = 0; i < numItems; i++) {
            imagesGameObject[i].SetActive(true);
            // update images
            images[i].sprite = request.itemObjects[i].prefab.GetComponent<SpriteRenderer>().sprite;
            images[i].material = tierSystem.GetTierLevel(request.itemObjects[i].itemTier).tierMaterial;
        }
        UpdateRequestPosition(position);
        UpdateRequestUI();
    }
    public void HideRequest() {
        image.enabled = false;
        for (int i = 0; i < imagesGameObject.Length; i++) {
            imagesGameObject[i].SetActive(false);
        }
    }
    void UpdateRequestPosition(Vector2 position) {
        rectTransform.position = position;
    }
}
/*
manages the requests ui
*/