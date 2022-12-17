using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy: MonoBehaviour
{
    public Tier enemyTier; // determines the xp, request types
    public Request request;
    public EnemyRequestsList enemyRequestsList;
    public InventoryObject inventoryObject; // scriptable object
    public float tweenTime = 0.1f;
    RequestHandler requestHandler; // the ui handler for displaying requests, shared among all enemies
    InventoryManager inventoryManager;
    EnemySpawner enemySpawner;
    ItemObject[] requestItems;
    Vector2 position;
    ParticleSystem particleSystem;
    AudioSource audioSource;
    public AudioClip enemySpawnSFX, enemyDefeatSFX;
    void Awake() {
        Transform temp = GetComponent<Transform>();
        position = new Vector2(temp.position.x, temp.position.y + 
            GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2);
        audioSource = GetComponent<AudioSource>();
    }
    void Start() {
        particleSystem = GetComponentInChildren<ParticleSystem>();

        requestHandler = FindObjectOfType<RequestHandler>();
        inventoryManager = FindObjectOfType<InventoryManager>(true);
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (requestHandler == null) Debug.LogWarning("Enemy: Missing RequestHandler");
        if (inventoryManager == null) Debug.LogWarning("Enemy: Missing inventoryManager");
        if (enemySpawner == null) Debug.LogWarning("Enemy: Missing EnemySpawner");
    }
    void OnMouseDown() {
        // mouse down on this enemy
        enemySpawner.UpdateMouseDown();
    }
    public void RequestCompleted(Request request) {
        // call when requestCompleted / defeated
        enemyRequestsList.RestoreRequestToPool(request);
        inventoryObject.RemoveItems(requestItems);
        inventoryManager.UpdateInventory();
        particleSystem.Play();
        enemySpawner.RemoveEnemy(gameObject, request);
    }
    public bool IsRequestComplete() {
        return inventoryObject.HasItems(requestItems);
    }
    public void PlaySpawnAudio() {
        audioSource.PlayOneShot(enemySpawnSFX);
    }
     public void PlayDeathAudio() {
        audioSource.PlayOneShot(enemyDefeatSFX);
    }
    public void UpdateRequest() {
        request = enemyRequestsList.SelectRandomRequest();
        if (request == null) {
            Debug.LogWarning("Enemy: No request available");
            return;
        }

        requestItems = new ItemObject[request.itemObjects.Length];
        for (int i = 0; i < request.itemObjects.Length; i++) {
            requestItems[i] = request.itemObjects[i];
        }
    }
    void OnTriggerEnter2D(Collider2D other)  {
        requestHandler.ShowRequest(request, position);
        SetBounce(true);
    }
    void OnTriggerExit2D(Collider2D other)  {
        requestHandler.HideRequest();
        // SetBounce(false);
    }
    public void SetBounce(bool isBounceTrue) {
        if (isBounceTrue) {
            LeanTween.scale (gameObject, Vector3.one * 0.99f, tweenTime * 5).setLoopPingPong().setEase(LeanTweenType.easeInOutSine);
        } else {
            LeanTween.cancel(gameObject);
            LeanTween.scale (gameObject, Vector3.one, tweenTime);
        }
    }
}
/*
gets a request when required
- detects mouse hover and shows request
- 
*/