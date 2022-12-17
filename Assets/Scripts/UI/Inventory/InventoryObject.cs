using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class InventoryObject : ScriptableObject
{
    public InventorySlot[] inventorySlots = new InventorySlot[9]; // fixed inventory size
    public bool AddItem(ItemObject _item) {
        // Adds item to inventory
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (inventorySlots[i].itemObject == null) {
                inventorySlots[i].itemObject = _item;
                return true;
            }
        }
        return false;
    }
    public bool RemoveItem(ItemObject _item) {
        // Removes item from inventory
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (inventorySlots[i].itemObject == _item) {
                inventorySlots[i].itemObject = null;
                return true;
            }
        }
        return false;
    }
    public void RemoveItems(ItemObject[] _item) {
        // Removes item from inventory
        // note the items must be in the inventory, no checking is done
        // if the items are not in the inventory, nothing is done
        for (int i = 0; i < _item.Length; i++) {
            for (int j = 0; j < inventorySlots.Length; j++) {
                if (inventorySlots[j].itemObject == _item[i]) {
                    inventorySlots[j].itemObject = null;
                    break;
                }
            }
        }
        // and should update the inventory when items are removed (done by another script)
    }
    public void ClearInventory() {
        for (int i = 0; i < inventorySlots.Length; i++) {
            inventorySlots[i].itemObject = null;
        }
    }
    public void InitialiseInventory(GameObject[] inventoryItems) {
        // initialises inventory with the existing items in scene inventory // not what we want
        if (inventoryItems.Length != inventorySlots.Length) Debug.LogWarning("InventoryObject: Inventory Size not equal");
        for (int i = 0; i < inventorySlots.Length; i++) {
            inventorySlots[i].itemObject = inventoryItems[i].GetComponent<InventoryItem>().itemObject;
        }
    }
    // public bool HasItems(ItemObject[] items) {
    //     // checks if inventory has the items
    //     List<ItemObject> tempInventory = new List<ItemObject>();
    //     for (int i = 0; i < inventorySlots.Length; i++) {
    //         tempInventory.Add(inventorySlots[i].itemObject);
    //     }
        
    //     for (int i = 0; i < items.Length; i++) {
    //         if (items == null) continue;
    //         if (!tempInventory.Remove(items[i])) return false;
    //     }
    //     return true;
    // }
     public bool HasItems(ItemObject[] items, List<ItemObject> tempInventoryReference = null) {
        // checks if inventory has the items. this acts on a tempInventory if it is provided
        // list is a reference type, so this will alter the tempInventory
        // makes a copy
        List<ItemObject> tempInventory;
        // no tempInventoryReference provided, create a tempInventory
        if (tempInventoryReference == null) {
            tempInventory = new List<ItemObject>();
            for (int i = 0; i < inventorySlots.Length; i++) {
                tempInventory.Add(inventorySlots[i].itemObject);
            }
        } else {
            tempInventory = new List<ItemObject>(tempInventoryReference);
        }
        for (int i = 0; i < items.Length; i++) {
            if (items == null) continue;
            if (!tempInventory.Remove(items[i])) return false;
        }
        return true;
    }
    public bool HasItem(ItemObject item) {
        // checks if inventory has the item
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (inventorySlots[i].itemObject == item) return true;
        }
        return false;
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject itemObject;
    public InventorySlot(ItemObject _itemObject) {
        itemObject = _itemObject;
    }

}

/*
inventory object as a scriptable object
- has a method to add item to inventory
    - used by display inventory
- the order in the inventory doesn't matter
*/