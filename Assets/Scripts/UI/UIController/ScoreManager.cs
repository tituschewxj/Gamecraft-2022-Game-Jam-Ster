using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float recipeXp, enemyXp, deleteXp, multiplierIncrement = 0.1f, multiplierDecayTime = 1f;
    ProgressBar progressBar;
    UIController uIController;
    private int enemiesDefeated = 0, recipesUsed = 0, itemsDeleted = 0, requestXp = 0;
    private float score = 0, previousScore = 0, multiplier = 1f;
    // Start is called before the first frame update
    IEnumerator coroutine;
    void Awake() {
        uIController = GetComponent<UIController>();
    }
    void Start()
    {
        progressBar = GetComponentInChildren<ProgressBar>();
    }
    public void CalculateScore() {
        // calculates the score based on certain rules
        // score = (int)(
        //     recipeXp * recipesUsed + 
        //     enemyXp * enemiesDefeated + 
        //     deleteXp * itemsDeleted +
        //     requestXp);
        
        // Updates score
        // uIController.scoreText.text = score.ToString();

        LeanTween.value(gameObject, previousScore, score, 0.2f).setOnUpdate((float val) => {
            uIController.scoreText.text = val.ToString("F0");
            });
        previousScore = score;
    }
    public void IncrementEnemiesDefeated() {
        IncrementMultiplier();
        enemiesDefeated++;
        score += enemyXp * multiplier;
    }
    public void IncrementRecipesUsed() {
        recipesUsed++;
        score += recipeXp;
    }
    public void IncrementItemsDeleted() {
        itemsDeleted++;
        score += deleteXp;
    }
    public void AddRequestXp(int amount) {
        // requestXp += amount;
        score += amount;
    }
    void IncrementMultiplier() {
        multiplier += multiplierIncrement;
        if (coroutine != null) {
            Debug.Log("Started coroutine: score multiplier");
            coroutine = DecayMultiplier();
            StartCoroutine(coroutine);
        }
    }
    public int GetScore() {
        return (int)score;
    }
    public int GetRecipesUsed() {
        return recipesUsed;
    }
    public int GetEnemiesDefeated() {
        return enemiesDefeated;
    }
    IEnumerator DecayMultiplier() {
        while (multiplier != 1) {
            yield return new WaitForSeconds(multiplierDecayTime);
            multiplier -= multiplierIncrement;
            uIController.UpdateKillColor(multiplier);
        }
    }
    public int GetItemsDeleted() {
        return itemsDeleted * 2;
    }
}
/*
manages the score
- 

scoring rules
- current time
- combining items using the recipes: recipe xp?
- defeating enemy: enemy xp
- multiplier: a multiplier to control?
- deleting -xp (is even because two items are deleted at once)


score is auto updated, no need to recalculate each time
*/
