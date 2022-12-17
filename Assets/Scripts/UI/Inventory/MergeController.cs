using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeController : MonoBehaviour
{
    public RecipeList recipeList;
    public InventoryObject inventoryObject;
    InventoryManager inventoryManager;
    void Awake()
    {
        if (recipeList == null) Debug.LogError("Merge Controller: Recipe List: null");
        if (inventoryObject == null) Debug.LogError("Merge Controller: inventoryObject: null");
    }
    void Start() {
        inventoryManager = FindObjectOfType<InventoryManager>(true);
    }

    public void MergeInventoryItems(InventoryItem inventoryItem1, InventoryItem inventoryItem2) {
        // inventory slots are the things that are merged, the items are just a property here
        // ensures that if one is null, it is just swapped
        if (inventoryItem1.itemObject == null && inventoryItem2.itemObject == null) {
            // do nothing
            return;
        }
        if (inventoryItem1.itemObject == null || inventoryItem2.itemObject == null) {
            // swap items
            inventoryManager.SwapInventoryItems(inventoryItem1, inventoryItem2);
            return;
        }

        // refers to the recipe list to determine if items can be merged
        Recipe recipe = null;
        for (int i = 0; i < recipeList.Container.Count; i++) {
            if (recipeList.Container[i].item1 == inventoryItem1.itemObject && 
                recipeList.Container[i].item2 == inventoryItem2.itemObject) {
                recipe = recipeList.Container[i];
                break;
            } 
            if (recipeList.Container[i].item2 == inventoryItem1.itemObject && 
                recipeList.Container[i].item1 == inventoryItem2.itemObject) {
                recipe = recipeList.Container[i];
                break;
            }
        }
        inventoryManager.Merge(recipe, inventoryItem1, inventoryItem2);
    }
}