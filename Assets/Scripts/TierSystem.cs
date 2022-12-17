using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tier System", menuName = "Tier System")]
public class TierSystem: ScriptableObject
{
    public List<TierLevel> tierLevels; // the list of tier levels
    public TierLevel GetTierLevel(Tier _tier) {
        for (int i = 0; i < tierLevels.Count; i++) {
            if (_tier == tierLevels[i].tier) return tierLevels[i];
        }
        Debug.LogWarning("TierSystem: No such tier level exists: Consider adding");
        return null;
    }
    public TierLevel GetRandomTierLevelBasedOnScore(float score) {
        List<TierLevel> _tierLevels = new List<TierLevel>();
        for (int i = 0; i < tierLevels.Count; i++) {
            if (tierLevels[i].scoreRequired <= score) {
                _tierLevels.Add(tierLevels[i]);
            }
        }
        if (_tierLevels.Count == 0) Debug.LogWarning("TierSystem: No tier levels exists: Consider adding");
        return tierLevels[Random.Range(0, _tierLevels.Count)];
    }
    // public Tier GetRequestTier(Request request) {
    //     // gets the request tier 
    //     // this is an unused function, request tier doesn't do anything really.
    //     List<Tier> requestTierLevels = new List<Tier>();
    //     for (int i = 0; i < request.itemObjects.Length; i++) {
    //         requestTierLevels.Add(request.itemObjects[i].itemTier);    
    //     }

    //     // iterate the container
    //     for (int i = 0; i < tierLevels.Count; i++) {
    //         // for each tier level, check the request types list for the tier
    //         foreach (RequestType rt in tierLevels[i].requestTypesList) {
    //             if (rt.requestItemTiers.Count == requestTierLevels.Count) {
    //                 // if same request length
    //                 bool isTier = true;
    //                 List<Tier> tempTierLevels = new List<Tier>(requestTierLevels);
    //                 foreach (Tier t in rt.requestItemTiers) {
    //                     if (!tempTierLevels.Remove(t)) {
    //                         // cannot remove
    //                         isTier = false;
    //                         break;
    //                     }
    //                 }
    //                 if (isTier) return new Tier(i);
    //             }
    //         }
    //     }
    //     return null;
    // }
}
[System.Serializable]
public class TierLevel {
    public Tier tier;
    // tier = item craftable level = monster requests = recipe tier
    public int monsterBaseXp;
    public int itemRequestXp;
    public int recipeXp;
    public int scoreRequired;
    public Material tierMaterial; // the tier of the material will determine the color around the item
    // public List<RequestType> requestTypesList;
}
[System.Serializable]
public class RequestType {
    public List<Tier> requestItemTiers;
}
[System.Serializable]
public class Tier {
    public int tier;
    public override bool Equals(object obj) => this.Equals(obj as Tier);
    public override int GetHashCode() => (tier).GetHashCode();
    public Tier(int _tier) {
        tier = _tier;
    }
    public static bool operator == (Tier t1, Tier t2) {
        if (t1.tier == t2.tier) return true;
        return false;
    }
    public static bool operator != (Tier t1, Tier t2) {
        if (t1.tier != t2.tier) return true;
        return false;
    }
}
/*
determines things about the tier
*/