using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public float tweenTime = 0.1f;
    public int maxNumberOfActivePlaceholders;
    public GameObject[] itemPlaceholders;
    // public GameObject[] itemPrefabs;
    List<GameObject> activePlaceholders = new List<GameObject>(), inactivePlaceholders = new List<GameObject>();
    List<GameObject> spawnableItems = new List<GameObject>(), spawnableContainer = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemPlaceholders.Length; i++) {
            itemPlaceholders[i].SetActive(false);
            inactivePlaceholders.Add(itemPlaceholders[i]);
        }
    }
    public void AddSpawnableItem(GameObject itemPrefab) {
        spawnableItems.Add(itemPrefab);
    }
    void AddItemsToSpawnableContainer() {
        if (spawnableItems.Count == 0) {
            Debug.LogWarning("ItemSpawner: no spawnableItems, unable to proceed");
        }
        spawnableContainer.AddRange(spawnableItems);
    }
    public void AddRandomItemToWorld() {
        // adds random spawnable item
        if (spawnableContainer.Count == 0) {
            AddItemsToSpawnableContainer();
            // Debug.Log("ItemSpawner: No more items to spawn");
            // return;
        }

        int randInt = Random.Range(0, spawnableContainer.Count);
        GameObject placeholder = GetRandomUnusedPlaceholder();
        if (placeholder != null) {
            GameObject temp = spawnableContainer[randInt];
            spawnableContainer.RemoveAt(randInt);

            // set the properties of placeholder with that of the prefab
            placeholder.SetActive(true);
            placeholder.GetComponent<Item>().itemObject = temp.GetComponent<Item>().itemObject;
            placeholder.GetComponent<SpriteRenderer>().sprite = temp.GetComponent<SpriteRenderer>().sprite;
            LeanTween.scale(placeholder, Vector3.one, tweenTime).setOnComplete(() => {
                LeanTween.scale(placeholder, Vector3.one * 0.97f, tweenTime * 3).setLoopPingPong();
            });
        }
    }
    GameObject GetRandomUnusedPlaceholder() {
        if (inactivePlaceholders.Count == 0) {
            Debug.Log("ItemSpawner: No more placeholder positions to spawn");
            return null;
        }
        if (activePlaceholders.Count == maxNumberOfActivePlaceholders) {
            Debug.Log("ItemSpawner: max number of active placeholders reached");
            return null;
        }
        int randInt = Random.Range(0, inactivePlaceholders.Count);
        GameObject temp = inactivePlaceholders[randInt];
        activePlaceholders.Add(temp);
        inactivePlaceholders.RemoveAt(randInt);
        return temp;
    }
    public void RemoveItem(GameObject item) {
        // used when items are added to inventory
        // ie: remove item from world
        LeanTween.cancel(item);
        LeanTween.scale(item, Vector3.zero, tweenTime);
        item.GetComponent<Collider2D>().enabled = false;
        // item.SetActive(false);
        StartCoroutine(SetInactiveItem(item));
        PlayerStats.TotalItemsObtained++;
    }
    void RemovePlaceholder(GameObject placeholder) {
        activePlaceholders.Remove(placeholder);
        inactivePlaceholders.Add(placeholder);
    }
    IEnumerator SetInactiveItem(GameObject item) {
        yield return new WaitForSeconds(tweenTime);
        item.GetComponent<Collider2D>().enabled = true;
        item.SetActive(false);
        RemovePlaceholder(item);
    }
}
