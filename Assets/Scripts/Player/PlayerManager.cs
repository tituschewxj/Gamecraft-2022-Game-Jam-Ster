using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerManager : MonoBehaviour
{
    public bool noUI = false;
    [SerializeField]
    public InventoryObject inventory;
    InventoryManager inventoryManager;
    UIController uIController;
    ItemSpawner itemSpawner;
    MouseCursor mouseCursor;
    IEnumerator coroutine;
    SFX sFX;
    bool mouseDown = false;

    void Start() {
        sFX = FindObjectOfType<SFX>();
        mouseCursor = GetComponentInChildren<MouseCursor>();

        if (!noUI) {
            itemSpawner = FindObjectOfType<ItemSpawner>();
            uIController = FindObjectOfType<UIController>();
            inventoryManager = FindObjectOfType<InventoryManager>(true);
            itemSpawner = FindObjectOfType<ItemSpawner>();

            if (inventoryManager == null) Debug.LogError("Player: inventoryManager: Null"); 
            if (uIController == null) Debug.LogError("Player: uIController: Null"); 
        }
    }
    void Update() {
        // this is the only reliable way for mousedown input
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            SetMouseDown(true);
        }
    }
    void OnMouseDown() {
        // Debug.Log("mouse down");
        SetMouseDown(true);
    }
    public void SetMouseDown(bool isDown) {
        // OnMouseDown doesn't work all of the time
        // because of leantween adjusting the object's scale constantly
        // it could work if something else is detecting mousedown
        mouseDown = isDown;
        if (isDown) sFX.PlayClickSFX();
    }
    public void OnTriggerEnter2D(Collider2D other) {
        if (noUI) return;
        SetMouseDown(false); // reset mousedown 
        var item = other.GetComponent<Item>();
        if (item != null) {
            mouseCursor.SetClickableCursor();

            coroutine = UntilClick(() => AddItem(item, other));
            StartCoroutine(coroutine);
            sFX.PlayItemTouchSFX();
            return;
        } 
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            mouseCursor.SetClickableCursor();
            if (enemy.IsRequestComplete()) {
                mouseCursor.SetAttackCursor();

                coroutine = UntilClick(() => {
                    enemy.SetBounce(false);
                    enemy.RequestCompleted(enemy.request);
                });
                StartCoroutine(coroutine);
                sFX.PlayAttackSFX();
                return;
            } 

            if (inventoryManager.IsRequestCraftableImmediate(enemy.request)) {
                mouseCursor.SetCraftableCursor();
                sFX.PlayCraftableSFX();
                return;
            }
            mouseCursor.SetUnableToAttackCursor();
            sFX.PlayMonsterBlockSFX();

            // if (inventoryManager.IsRequestCraftableDFS(enemy.request)) {
            //     Debug.Log("PlayerManager: possible request craftable via dfs");
            //     return;
            // }
        }
    }
    public void OnTriggerExit2D(Collider2D other) {
        if (noUI) return;
        mouseCursor.SetDefaultCursor(); // reset mousecursor

        if (coroutine != null) StopCoroutine(coroutine);
    }
    void AddItem(Item item, Collider2D other) {
        if (inventory.AddItem(item.itemObject)) {
            sFX.PlayPickUpSFX();
            // Destroy(other.gameObject); // use object pooling #todo
            itemSpawner.RemoveItem(other.gameObject);
            // add to inventory
            inventoryManager.AddToInventory(item);
        } else {
            // Debug.Log("Unable to add item");
            uIController.ChangeBottomText("Inventory full");
        }
    }
    IEnumerator UntilClick(Action callback) {
        while (true) {
            if (mouseDown) {
                callback?.Invoke();
                // AddItem(item, other);
                break;
            }
            yield return null;
        }
    }
    
}
/*
is responsible for
- ui of mouse cursor (not mouse controller)
- on click, add items to inventory / defeat monsters
*/