using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventoryManager : MonoBehaviour
{
    public InventoryObject inventoryObject;
    public GameObject[] inventoryItems;
    UIController uIController;
    MergeController mergeController;
    ScoreManager scoreManager;
    RecipeManager recipeManager;
    MouseCursor mouseCursor;
    SFX sFX;
    void Awake() {
        mergeController = GetComponent<MergeController>();
        if (inventoryItems.Length != 9) Debug.LogWarning("DisplayInventory: Wrong Inventory Size");

        // inventoryObject.InitialiseInventory(inventoryItems);
    }
    void Start() {
        uIController = FindObjectOfType<UIController>();
        scoreManager = FindObjectOfType<ScoreManager>();
        recipeManager = FindObjectOfType<RecipeManager>(true);
        mouseCursor = FindObjectOfType<MouseCursor>();
        sFX = FindObjectOfType<SFX>();
        ClearInventory();
    }
    public void AddToInventory(Item item) {
        // add to inventory from world
        for (int i = 0; i < inventoryItems.Length; i++) {
            InventoryItem temp = inventoryItems[i].GetComponent<InventoryItem>();
            if (temp != null && temp.itemObject == null) {
                temp.itemObject = item.itemObject;
                temp.GetComponent<Image>().sprite = item.itemObject.prefab.GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }
    }
    void ClearInventory() {
        inventoryObject.ClearInventory();
        UpdateInventory();
    }
    public void UpdateInventory() {
        // update inventory items on fulfillment of request
        // fulfillment of request only updates the inventoryObject
        for (int i = 0; i < inventoryItems.Length; i++) {
            inventoryItems[i].GetComponent<InventoryItem>().itemObject = inventoryObject.inventorySlots[i].itemObject;
            inventoryItems[i].GetComponent<MergeItems>().UpdateImage();
        }
    }
    public void Merge(Recipe recipe, InventoryItem ii1, InventoryItem ii2) {
        inventoryObject.RemoveItem(ii1.itemObject);
        ii1.itemObject = null;
        inventoryObject.RemoveItem(ii2.itemObject);
        ii2.itemObject = null;

        if (recipe != null) {
            inventoryObject.AddItem(recipe.result);
            ii2.itemObject = recipe.result;
            scoreManager.IncrementRecipesUsed();
            mouseCursor.SetClickableCursor();
            sFX.PlayMergeSFX();
        } else {
            // if recipe == null: means that no recipe is found: merge failure
            mouseCursor.SetDefaultCursor();
            scoreManager.IncrementItemsDeleted();
            sFX.PlayDeleteSFX();
        }

        ii1.GetComponent<MergeItems>().UpdateImage();
        ii2.GetComponent<MergeItems>().UpdateImage();
        
        uIController.UpdateScore();
        
    }
    public void SwapInventoryItems(InventoryItem ii1, InventoryItem ii2) {
        ItemObject temp = ii1.itemObject;
        ii1.itemObject = ii2.itemObject;
        ii2.itemObject = temp;

        ii1.GetComponent<MergeItems>().UpdateImage();
        ii2.GetComponent<MergeItems>().UpdateImage();

        mouseCursor.SetClickableCursor();
        sFX.PlayMovingItemsSFX();
    }
    public bool IsCraftableImmediate(ItemObject itemObject) {
        // determines if an item, not in the inventory, can be obtained with a single merging

        // with other items in the inventory
        for (int i = 0; i < recipeManager.knownRecipesList.Count; i++) {
            if (itemObject == recipeManager.knownRecipesList[i].result) {
                if (inventoryObject.HasItems(new ItemObject[2]{recipeManager.knownRecipesList[i].item1, 
                    recipeManager.knownRecipesList[i].item2})) {
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsRequestCraftableImmediate(Request req) {
        // this is not the same as is craftable immediate
        // there can be more than one of the same item in the request
        List<ItemObject> request = new List<ItemObject>();
        List<ItemObject> temp = new List<ItemObject>();
        List<ItemObject> tempInventory = new List<ItemObject>();
        for (int i = 0; i < inventoryObject.inventorySlots.Length; i++) {
            tempInventory.Add(inventoryObject.inventorySlots[i].itemObject);
        }
        for (int i = 0; i < req.itemObjects.Length; i++) {
            request.Add(req.itemObjects[i]);
        }

        // first pass, remove items that are in the inventory
        foreach (ItemObject ioRequest in request) {
            int foundIndex = -1;
            for (int j = 0; j < tempInventory.Count; j++) {
                // if the result can be found in the inventory
                if (ioRequest == tempInventory[j]) {
                    foundIndex = j;
                    break;
                }
            }
            if (foundIndex != -1) {
                tempInventory.RemoveAt(foundIndex);
            } else {
                // add to second pass
                temp.Add(ioRequest);
            }
        }
        request.Clear();
        request.AddRange(temp);
        if (request.Count == 0) {
            Debug.Log("Request should have cleared");
            return true;
        }

        Queue<List<ItemObject>> dfsQueue = new Queue<List<ItemObject>>(); //dfs
        dfsQueue.Enqueue(request);
        // second pass, remove items that can be crafted with one merge
        int dfsPasses = 0; // ensures a limit to the number of dfs passes
        while (dfsQueue.Count != 0 && dfsPasses++ < 50) {
            request = dfsQueue.Dequeue();
            if (request.Count >= 9) continue;
            int completedRequests = 0;
            foreach (ItemObject ioRequest in request) {
                bool found = false;
                for (int j = 0; j < recipeManager.knownRecipesList.Count; j++) {
                    // if the result can be found in recipes
                    if (ioRequest == recipeManager.knownRecipesList[j].result) {
                        // check if the items needed are there
                        ItemObject item1 = recipeManager.knownRecipesList[j].item1, 
                            item2 = recipeManager.knownRecipesList[j].item2;

                        // dfs is extremely laggy
                        // dfs item even if has item 
                        temp = new List<ItemObject>(request);
                        temp.Remove(ioRequest);
                        temp.Add(item1);
                        temp.Add(item2);
                        dfsQueue.Enqueue(temp);

                        if (inventoryObject.HasItems(new ItemObject[2]{item1, item2}, tempInventory)) {
                            tempInventory.Remove(item1);
                            tempInventory.Remove(item2);
                            found = true;
                            completedRequests++;
                            break; // proceed to next entry in request
                        } 
                        
                    }
                }
                if (!found) {
                    // proceed with dfs
                    // skip all other items in this attempt?
                    break;
                }
            }
            if (completedRequests == request.Count) {
                //
                Debug.Log("DFS found request craftable");
                return true;
            }
        }
        // dfs completed, still not found
        return false;
    }
    // public bool IsRequestCraftableDFS(Request request) {
    //     // doesn't work
    //     List<ItemObject> requestItems = new List<ItemObject>();
    //     for (int i = 0; i < request.itemObjects.Length; i++) {
    //         requestItems.Add(request.itemObjects[i]);
    //     }
    //     return IsRequestCraftableDFS(requestItems);
    // }
    // bool IsRequestCraftableDFS(List<ItemObject> request, List<ItemObject> tempInventoryReference = null) {
    //     // this is not the same as is craftable immediate
    //     // there can be more than one of the same item in the request
    //     // does dfs to determine if it is possible to craft something based on the items in the inventory
    //     List<ItemObject> tempInventory = new List<ItemObject>();
    //     if (tempInventoryReference == null) {
    //         for (int i = 0; i < inventoryObject.inventorySlots.Length; i++) {
    //             tempInventory.Add(inventoryObject.inventorySlots[i].itemObject);
    //         }
    //     } else {
    //         tempInventory = tempInventoryReference;
    //     }
    //     if (tempInventory.Count == 0) return false; // base case for dfs
    //     if (request.Count == inventoryObject.inventorySlots.Length) return false; // limit to the number of possible items

        
    //     for (int i = 0; i < request.Count; i++) {
    //         bool found = false;
    //         List<ItemObject[]> dfsStore = new List<ItemObject[]>();
    //         for (int j = 0; j < recipeManager.knownRecipesList.Count; j++) {
    //             // if the result can be found in recipes
    //             if (request[i] == recipeManager.knownRecipesList[j].result) {
    //                 // check if the items needed are there
    //                 ItemObject item1 = recipeManager.knownRecipesList[j].item1, 
    //                     item2 = recipeManager.knownRecipesList[j].item2;
    //                 // add these item pairs to a list in case dfs is required
    //                 dfsStore.Add(new ItemObject[2]{item1, item2});

    //                 if (inventoryObject.HasItems(new ItemObject[2]{item1, item2}, tempInventory)) {
    //                     tempInventory.Remove(item1);
    //                     tempInventory.Remove(item2);
    //                     // request.Remove(request[i]); // cannot remove 
    //                     // actually, the loop takes care of the removing
    //                     found = true;
    //                     break; // proceed to next entry in request
    //                 }
    //             }
    //         }
    //         if (!found) {
    //             // do dfs for that item
    //             // request.Remove(request[i]); // cannot remove
    //             for (int j = 0; j < dfsStore.Count; j++) {
    //                 List<ItemObject> newRequest = new List<ItemObject>(request);
    //                 newRequest.Add(dfsStore[j][0]);
    //                 newRequest.Add(dfsStore[j][1]);
    //                 newRequest.Remove(request[i]);
    //                 if (IsRequestCraftableDFS(newRequest, tempInventory)) {
    //                     found = true;
    //                     break;
    //                 }
    //             }
    //         }
    //         if (!found) return false;
    //     }
    //     return true;
    // }
}
/*
Collects the items into the inventory slots
- displays the added items in the inventory
- this should manage everything that deals with the inventory.
    - inventory object: scriptable object
    - merge controller

*/