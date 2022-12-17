using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler 
{
    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalRectPosition; // stores the rect position before drag
    SFX sFX;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    void Start() {
        originalRectPosition = rectTransform.position;
        sFX = FindObjectOfType<SFX>();
    }
    public void OnBeginDrag(PointerEventData eventData) {
        // canvasGroup.interactable = false;
        originalRectPosition = rectTransform.position;

        canvasGroup.blocksRaycasts = false;
        rectTransform.SetAsLastSibling(); // ui renders last
    }
    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData) {
        // canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        // Debug.Log("EndDrag");

        // check if there is an item slot below, otherwise return to original position
        if (!hasItemSlot(eventData.hovered)) {
            // return to original position
            ReturnToOriginalPosition();
            sFX.PlaySnapBackSFX();
        }
    }
    public void ReturnToOriginalPosition() {
        // used when inventory is exited, or fails item slot check
        canvasGroup.blocksRaycasts = true;
        rectTransform.position = originalRectPosition;
    }
    bool hasItemSlot(List<GameObject> list) {
        // check if there is an item slot below
        for (int i = 0; i < list.Count; i++) {
            if (list[i].GetComponent<ItemSlot>() != null) {
                // Debug.Log(list[i]);
                return true;
            }
        }
        return false;
    }
}
/*
implemented on inventory items
- ensures that the inventory item stays in a slot.
*/