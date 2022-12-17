using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemObject itemObject;
    PlayerManager playerManager;
    public void Start() {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    void OnMouseDown() {
        Debug.Log("Item: mousedown");
        playerManager.SetMouseDown(true);
    }
}
/*
World Items:
- are in the world
- not in inventory
- once in inventory, it would be converted to an inventory item
*/
