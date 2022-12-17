using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MergeItems : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TierSystem tierSystem;
    public Sprite blankImage;
    MergeController mergeController;
    InventoryItem inventoryItem;
    Image image;
    MouseCursor mouseController;
    void Awake() {
        inventoryItem = GetComponent<InventoryItem>();
        image = GetComponent<Image>();
    }
    void Start() {
        mergeController = GetComponentInParent<MergeController>();
        mouseController = FindObjectOfType<MouseCursor>();
    }
    public void UpdateImage() {
        // updates the image of the gameobject with this script attached.
        // Debug.Log("image updated");
        if (inventoryItem.itemObject == null) {
            image.sprite = blankImage;
            image.material = tierSystem.GetTierLevel(new Tier(0)).tierMaterial;
        } else {
            image.sprite = inventoryItem.itemObject.prefab.GetComponent<SpriteRenderer>().sprite;
            image.material = tierSystem.GetTierLevel(inventoryItem.itemObject.itemTier).tierMaterial;
        }
    }
    public void OnDrop(PointerEventData eventData) {
        // merges itemObjects, modifies itemObjects
        InventoryItem other = eventData.pointerDrag.GetComponent<InventoryItem>();
        mouseController.SetClickableCursor();
        // call merge controller to deal with merging
        mergeController.MergeInventoryItems(other, inventoryItem);
    }
    // mouse cursor
    public void OnPointerEnter(PointerEventData eventData) {
        if (image.sprite != blankImage) mouseController.SetClickableCursor();
    }
    public void OnPointerExit(PointerEventData eventData) {
        if (image.sprite != blankImage) mouseController.SetDefaultCursor();
    }
}
/*
placed inside inventory item object.
- detects if items touch - for merging
- changes image

- problems with using leantween on the inventory items

turns out that graphic raycaster on the canvas can block rays
*/
