using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float tweenTime = 0.1f;
    public GameObject[] enemies; // enemy gameobjects in the scene
    public EnemyRequestsList enemyRequestsList;
    List<GameObject> activeEnemies = new List<GameObject>(), inactiveEnemies = new List<GameObject>();
    GameplayManager gameplayManager;
    PlayerManager playerManager;
    SFX sFX;
    void Awake() {
        enemyRequestsList.Clear();
        // transform.localScale = Vector3.zero;
    }
    void Start() {
        for (int i = 0; i < enemies.Length; i++) {
            enemies[i].SetActive(false);
            inactiveEnemies.Add(enemies[i]);
        }
        
        gameplayManager = FindObjectOfType<GameplayManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        sFX = FindObjectOfType<SFX>();
    }
    public void UpdateMouseDown() {
        playerManager.SetMouseDown(true);
    }
    public bool TryAddRandomEnemy() {
        // try: if there is no unique inactive requests, it will fail
        // or if there is no more enemies available
        if (inactiveEnemies.Count == 0) {
            Debug.Log("EnemySpawner: No more enemies to spawn");
            return false;
        }
        if (!enemyRequestsList.IsInactiveRequestAvailable()) {
            Debug.Log("EnemySpawner: No requests available");
            return false;
        }

        int randInt = Random.Range(0, inactiveEnemies.Count);
        GameObject temp = inactiveEnemies[randInt];
        activeEnemies.Add(temp);

        temp.SetActive(true);
        LeanTween.scale(temp, Vector3.one, tweenTime).setOnComplete(() => {
            temp.GetComponent<Collider2D>().enabled = true;
        });
        temp.GetComponent<Enemy>().UpdateRequest();
        temp.GetComponent<Enemy>().PlaySpawnAudio();
        inactiveEnemies.RemoveAt(randInt);

        gameplayManager.EnemyAdded();
        return true;
    }
    public void AddRequest(Request _request) {
        // enemyRequest has to match the enemyTier
        // all requests should be unique.
        // this can be done with a matrix much more effieciently
        if (enemyRequestsList.HasRequest(_request)) return;
        // else add request
        enemyRequestsList.AddRequest(_request);
    }
    public void RemoveEnemy(GameObject enemy, Request request) {
        // no checking done
        // enemy.GetComponent<Enemy>().PlayDeathAudio();
        sFX.PlayMonsterDefeatedSFX();
        LeanTween.scale(enemy, Vector3.zero, tweenTime).setOnComplete(() => {
            // Debug.Log("removed enemy"); // unable to put a delay on complete
        });
        enemy.GetComponent<Collider2D>().enabled = false;
        gameplayManager.EnemyDefeated(request);
        StartCoroutine(TweenThenRemoveEnemy(enemy));
    }
    public int EnemyCount() {
        return activeEnemies.Count;
    }
    public int TotalEnemies() {
        return enemies.Length;
    }
    IEnumerator TweenThenRemoveEnemy(GameObject enemy) {
        yield return new WaitForSeconds(tweenTime + 1f); // adds the time needed for the particle system
        inactiveEnemies.Add(enemy);
        activeEnemies.Remove(enemy);
        enemy.SetActive(false);
    }
}
/*
adds enemies to the scene every x seconds.
- this should be managing the enemy requests also
*/