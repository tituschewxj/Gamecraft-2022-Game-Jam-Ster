using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public float secondsBetweenItemSpawning, secondsBetweenMonsterSpawning, secondsBetweenNewGameplayState;
    public TierSystem tierSystem; // scriptable object: main reference for values
    EnemySpawner enemySpawner;
    RecipeManager recipeManager;
    ItemSpawner itemSpawner;
    UIController uIController;
    ScoreManager scoreManager;
    public GameObject[] itemPrefabs; // only used to add into unusedItems
    List<int> unusedItemsIndex = new List<int>();
    [SerializeField]
    List<ItemObject> usedItemObjects = new List<ItemObject>();
    Dictionary<ItemObject, int> usedItemObjectIndexes = new Dictionary<ItemObject, int>(); // stores the id of each item as an index, used in a matrix to search for valid combinations
    List<List<bool>> validRecipeMatrix = new List<List<bool>>(); // used to find valid combinations quickly, false = combination not used, true = combination used O(n^2) space
    int gameplayState = 0; // determines what action to do on enemy defeat
    Tier maxTier = new Tier(0);
    void Awake() {
        if (itemPrefabs.Length == 0) Debug.LogWarning(gameObject.name + ": GameplayManager: no itemPrefabs assigned");

        // reinitialize
        // unusedItemsIndex = new List<int>();
        // usedItemObjects = new List<ItemObject>();
        // usedItemObjectIndexes = new Dictionary<ItemObject, int>(); // still error: an item of the same key has already been defined.
        // validRecipeMatrix = new List<List<bool>>();
        for (int i = 0; i < itemPrefabs.Length; i++) {
            unusedItemsIndex.Add(i);
        }

        if (secondsBetweenItemSpawning < 1) Debug.LogError("gameplayManager: cannot have this value");
        if (secondsBetweenMonsterSpawning < 1) Debug.LogError("gameplayManager: cannot have this value");
        if (secondsBetweenNewGameplayState < 1) Debug.LogError("gameplayManager: cannot have this value");
    }
    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        recipeManager = FindObjectOfType<RecipeManager>(true);
        itemSpawner = FindObjectOfType<ItemSpawner>();
        uIController = FindObjectOfType<UIController>();
        uIController.UpdateProgressBar(enemySpawner.EnemyCount() / enemySpawner.TotalEnemies());
        scoreManager = uIController.GetComponent<ScoreManager>();

        // initialise game
        gameplayState = 0;
        AddNewItemToGameplay();
        TryAddRandomUniqueRequest(GetRandomTierLevelFromScore().tier.tier + 1);

        StartCoroutine(RepeatAndSpawnItems());
        StartCoroutine(RepeatAndSpawnMonsters());
        StartCoroutine(IncrementGameplayState());

        ResetPlayerStats();

        UpdateDifficulty(PlayerStats.Difficulty);
    }
    void UpdateDifficulty(int difficulty) {
        if (difficulty == 0) {
            secondsBetweenItemSpawning = 1;
            secondsBetweenMonsterSpawning = 10;
            secondsBetweenNewGameplayState = 15;
            maxTier = new Tier(1);
        } else if (difficulty == 1) {
            secondsBetweenItemSpawning = 1;
            secondsBetweenMonsterSpawning = 10;
            secondsBetweenNewGameplayState = 10;
            maxTier = new Tier(2);
        } else if (difficulty == 2) {
            secondsBetweenItemSpawning = 1;
            secondsBetweenMonsterSpawning = 7.5f;
            secondsBetweenNewGameplayState = 7.5f;
            maxTier = new Tier(3);
        } else if (difficulty == 3) {
            secondsBetweenItemSpawning = 1;
            secondsBetweenMonsterSpawning = 3;
            secondsBetweenNewGameplayState = 3;
            maxTier = new Tier(4);
        } else if (difficulty == -1) {
            // tutorial
            secondsBetweenItemSpawning = 1;
            secondsBetweenMonsterSpawning = 3;
            secondsBetweenNewGameplayState = 10;
            maxTier = new Tier(2);
        } else {
            Debug.LogError("Invalid difficulty");
        }
    }
    public void ResetPlayerStats() {
        PlayerStats.Score = 0;
        PlayerStats.GameLength = 0;
        PlayerStats.MonsterKills = 0;
        PlayerStats.SpellsUsed = 0;
        PlayerStats.ItemsDeleted = 0;
        PlayerStats.TotalItemsObtained = 0;
        PlayerStats.UniqueItemsObtained = 0;
    }
    public void EndGame() {
        if (FindObjectOfType<TutorialScene>() == null) {
            PlayerStats.Score = scoreManager.GetScore();
            PlayerStats.GameLength = (int)Time.timeSinceLevelLoad;
            PlayerStats.MonsterKills = scoreManager.GetEnemiesDefeated();
            PlayerStats.ItemsDeleted = scoreManager.GetItemsDeleted();
            PlayerStats.SpellsUsed = scoreManager.GetRecipesUsed();
            PlayerStats.UniqueItemsObtained = usedItemObjects.Count;
            FindObjectOfType<LevelLoader>().LoadSceneNow("SceneEnd");
        }
    }
    int GetUsedItemObjectIndex(ItemObject item) {
        return usedItemObjectIndexes[item];
    }
    Recipe GetUniqueRecipeFromRecipeMatrix(ItemObject result = null, bool bothUnique = false) {
        // this should be the default function for getting recipes
        // there shouldn't be softlock.
        // ensure all items have a recipe.
        int index;
        if (result == null) {
            // select a random usedItem.
            index = Random.Range(0, usedItemObjectIndexes.Count);
            result = usedItemObjects[index];
        } else {
            index = GetUsedItemObjectIndex(result);
        }
        List<int> randomNumbers = new List<int>();
        for (int i = 0; i < usedItemObjects.Count; i++) if (i != index) randomNumbers.Add(i);

        ItemObject item1, item2;
        int randInt = Random.Range(0, randomNumbers.Count);
        item1 = usedItemObjects[randomNumbers[randInt]];
        if (bothUnique) randomNumbers.RemoveAt(randomNumbers.Count - 1);
        randInt = Random.Range(0, randomNumbers.Count);
        item2 = usedItemObjects[randomNumbers[randInt]];

        // recipe tier level is determined by the highest tier of item in the recipe.
        int maxTier = Mathf.Max(item1.itemTier.tier, item2.itemTier.tier, result.itemTier.tier);
        TierLevel recipeTierLevel = tierSystem.GetTierLevel(new Tier(maxTier)); 

        return new Recipe(item1, item2, result, recipeTierLevel); // based on the itemObjects used
    }
    public int GetRequestXp(Request request) {
        // for increasing score
        int tempScore = 0;
        for (int i = 0; i < request.itemObjects.Length; i++) {
            tempScore += tierSystem.tierLevels[request.itemObjects[i].itemTier.tier].itemRequestXp;
        }
        return tempScore;
    }
    public void EnemyDefeated(Request request) {
        scoreManager.IncrementEnemiesDefeated();
        scoreManager.AddRequestXp(GetRequestXp(request));
        uIController.UpdateScore();
        uIController.UpdateProgressBar((float)enemySpawner.EnemyCount() / enemySpawner.TotalEnemies());

        UpdateGameplayState();
    }
    void UpdateGameplayState() {
        // based on the number of items, determine what to do
        if (gameplayState == 0) {
            AddNewItemToGameplay();
        } else if (gameplayState == 1) {
            TryAddRandomUniqueRequest(GetRandomTierLevelFromScore().tier.tier + 1);
        } else if (gameplayState == 2) {
            // adds the last item result as a recipe
            recipeManager.AddRecipe(GetUniqueRecipeFromRecipeMatrix(usedItemObjects[usedItemObjects.Count - 1])); 
        } else if (gameplayState >= usedItemObjects.Count) {
            gameplayState = -1;
        } else {
            if (gameplayState % 3 == 0) {
                // adds a random recipe
                TryAddRandomUniqueRequest(GetRandomTierLevelFromScore().tier.tier + 1);
            } else {
                recipeManager.AddRecipe(GetUniqueRecipeFromRecipeMatrix());
            }
            
        } 
        gameplayState++;
    }
    public void AddNewItemToGameplay() {
        // adds a new item to the game scene
        // will need to check if items of that tier even exist...
        // add item and assign tier
        TierLevel itemTierLevel = GetRandomTierLevelFromScore();
        int index = SelectRandomUnusedItemPrefabIndex();
        if (index == -1) {
            Debug.LogWarning("GameplayManager: can't add new item to gameplay");
            return;
        }
        ItemObject io = usedItemObjects[GetUsedItemObjectIndex(itemPrefabs[index].GetComponent<Item>().itemObject)];
        io.itemTier = itemTierLevel.tier;

        if (itemTierLevel.tier.tier == 0) {
            // based on item tier... item tier 1 and above shouldn't spawn...
            itemPrefabs[index].GetComponent<SpriteRenderer>().material = itemTierLevel.tierMaterial; // for the display of the item, which should be tier 0
            itemSpawner.AddSpawnableItem(itemPrefabs[index]);
        } else {
            // adds a recipe for it also 
            // prevents softlock
            recipeManager.AddRecipe(GetUniqueRecipeFromRecipeMatrix(io));
        }
        // helper functions
        int SelectRandomUnusedItemPrefabIndex() {
            // and adds it.
            if (unusedItemsIndex.Count == 0) {
                 Debug.LogWarning("UnusedItemsIndex not initialized");
                 for (int j = 0; j < itemPrefabs.Length; j++) {
                    unusedItemsIndex.Add(j);
                }
            }
            int randInt = Random.Range(0, unusedItemsIndex.Count);
            
            int index = unusedItemsIndex[randInt]; // the index of the prefab
            if (usedItemObjectIndexes.Count == itemPrefabs.Length) {
                Debug.LogWarning("GameplayManager: can't add any more unique items: UnusedItemsIndex == item prefabs size");
                return -1;
            }

            unusedItemsIndex.RemoveAt(randInt);
            usedItemObjects.Add(itemPrefabs[index].GetComponent<Item>().itemObject); // add item object into usedItems immediately
            int i = usedItemObjects.Count - 1; // the index of the usedItem
            usedItemObjectIndexes.Add(usedItemObjects[i], i); // index item with id
            return index;
        }
    }
    TierLevel GetRandomTierLevelFromScore() {
        TierLevel tierLevel = tierSystem.GetRandomTierLevelBasedOnScore(scoreManager.GetScore());
        // Debug.Log(tierLevel);
        Debug.Log("tier level = " + tierLevel.tier.ToString());
        // Debug.Log(tierLevel.tier.tier);
        // Debug.Log(tierSystem.GetTierLevel(maxTier));
        // Debug.Log(maxTier.tier);
        if (tierLevel.tier.tier > maxTier.tier) return tierSystem.GetTierLevel(maxTier);
        else return tierLevel;
    }
    bool TryAddRandomUniqueRequest(int numberOfItemsInRequest) {
        // there is no guarantee that the request is unique 
        // there is no need to link a request to a tier level...
        if (numberOfItemsInRequest == 0) {
            Debug.LogWarning("GameplayManager: invalid request number of items: TryAddRandomUniqueRequest");
            return false;
        }
        if (usedItemObjects.Count == 0) {
            Debug.LogWarning("GameplayManager: no item available in pool: TryAddRandomUniqueRequest");
            return false;
        }
        ItemObject[] itemObjects = new ItemObject[numberOfItemsInRequest];

        for (int i = 0; i < numberOfItemsInRequest; i++) {
            itemObjects[i] = GetRandomItemObject();
        }
        Request request = new Request(itemObjects);
        enemySpawner.AddRequest(request);

        // tierSystem.GetRequestTier(request); // gets the tier if needed
        return true;

        // helper functions
        ItemObject GetRandomItemObject() {
            if (usedItemObjects.Count == 0) {
                Debug.LogWarning("GameplayManager: no items available in pool: GetRandomItemObject");
                return null;
            }
            return usedItemObjects[Random.Range(0, usedItemObjects.Count)];
        }
    }
    public void EnemyAdded() {
        uIController.UpdateProgressBar((float)enemySpawner.EnemyCount() / enemySpawner.TotalEnemies());
    }
    IEnumerator RepeatAndSpawnItems() {
        while(true) {
            itemSpawner.AddRandomItemToWorld();
            yield return new WaitForSeconds(secondsBetweenItemSpawning);
        }
    }
    IEnumerator RepeatAndSpawnMonsters() {
        while(true) {
            yield return new WaitForSeconds(secondsBetweenMonsterSpawning);
            enemySpawner.TryAddRandomEnemy();
        }
    }
    IEnumerator IncrementGameplayState() {
        while(true) {
            yield return new WaitForSeconds(secondsBetweenNewGameplayState);
            UpdateGameplayState();
        }
    }
    // void AddCorrespondingRecipe(TierLevel recipeTier, ItemObject result) {
    //     // selects 2 other items in the usedItems, don't include the new item
    //     if (usedItems.Count == 1) return;

    //     // the recipes are not well defined 
    //     // the recipes need to have different inputs and outputs
    //     // there cannot be a recipe with two of the same inputs but different outputs
    //     int randInt1 = Random.Range(0, usedItems.Count - 2), randInt2 = Random.Range(0, usedItems.Count - 2); // the last item is not included
    //     recipeManager.AddRecipe(new Recipe(
    //         usedItems[randInt1].GetComponent<Item>().itemObject,
    //         usedItems[randInt2].GetComponent<Item>().itemObject,
    //         result,
    //         recipeTier
    //     ));
    // }
    // void AddRandomRecipe() {
    //     // adds a random useful recipe with all differing items, with no tier in mind
    //     // Debug.Log("Added random recipe");

    //     List<GameObject> temp = new List<GameObject>(usedItems);
    //     int randInt = Random.Range(0, temp.Count);
    //     ItemObject io1 = temp[randInt].GetComponent<Item>().itemObject;
    //     if (temp.Count > 1) temp.RemoveAt(randInt);
    //     randInt = Random.Range(0, temp.Count);
    //     ItemObject io2 = temp[randInt].GetComponent<Item>().itemObject;
    //     if (temp.Count > 1) temp.RemoveAt(randInt);
    //     randInt = Random.Range(0, temp.Count);
    //     ItemObject io3 = temp[randInt].GetComponent<Item>().itemObject;
    //     // temp.RemoveAt(randInt); // insert back if necessary

    //     TierLevel recipeTier = null; // #todo // based on item tiers

    //     recipeManager.AddRecipe(new Recipe(
    //         io1,
    //         io2,
    //         io3,
    //         recipeTier
    //     )); 
    // }
}
/*
the monobehaviour that manages and combines the classes
- recipe manager: add recipes
- item spawner: what items spawn
    - spawnable items
- enemyspawner: what requests are available


- this should manage the spawning of items and enemies also...


- should also manage ui?



- the gameplay flow
1. start with one item, one request
2. loop
2.1 add a new item
2.1.1 determine if craftable? based on it's tier
2.1.2 adds it's recipe if it is craftable only 
2.2 add a new request (carefully crafted to include that new item)
2.3 add a new recipe (between items of the same tier? or what?)

- recipes, requests are unique.
- recipes uniqueness can be determined by a matrix
- dict<ItemObject, int> // key, index pairs
- usedRecipes[i][j][k], k != i, k != j // grows O(n^3) space, but fast.


prefabs
-> once placed
    -> then itemObjects are only need


- gameplay should encourage merging of items.
    - craftable only items are a start... but how ... the limit on the number of items makes it hard to manage things... unlimited items??? i don't know merging stacks of items seem more valid... but i will need to work on the merging system
    - when one stack is dragged on top of the other, both stacks merge, leaving the result behind... // no too hard to implement
*/