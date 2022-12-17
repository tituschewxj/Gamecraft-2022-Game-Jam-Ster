using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Default
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    public Tier itemTier;
    [TextArea()]
    public string description;    
}
