using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemSlot : MonoBehaviour, IDropHandler
{
    RectTransform rectTransform;
    // DisplayInventory displayInventory;
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        // displayInventory = GetComponentInParent<DisplayInventory>();
    }
    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = rectTransform.anchoredPosition;
            eventData.pointerDrag.GetComponent<InventoryItem>();
        }
    }
}
/*
related to DragDrop
- holds the position for the moving of the inventory items
*/