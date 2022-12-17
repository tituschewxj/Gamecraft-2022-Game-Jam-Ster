using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSFX, monsterDefeatedSFX, monsterBlockSFX, pickupSFX, movingItemsSFX, 
        craftableSFX, mergeSFX, attackSFX, itemTouchSFX, inventorySFX, deleteSFX, snapBackSFX, 
        newRecipeSFX, enemySpawnSFX;
    public static SFX sFX;
    private void Awake() {
        if (sFX == null) {
            sFX = this;
            DontDestroyOnLoad(gameObject);
            // Destroy(this.gameObject);
            // return;
        }
        // sFX = this;
        // DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();
    }
    public void PlayClickSFX() {
        audioSource.PlayOneShot(clickSFX);
    }
    public void PlayMonsterDefeatedSFX() {
        audioSource.PlayOneShot(monsterDefeatedSFX);
    }
    public void PlayMonsterBlockSFX() {
        audioSource.PlayOneShot(monsterBlockSFX);
    }
    public void PlayPickUpSFX() {
        audioSource.PlayOneShot(pickupSFX);
    }
    public void PlayMovingItemsSFX() {
        audioSource.PlayOneShot(movingItemsSFX);
    }
    public void PlayCraftableSFX() {
        audioSource.PlayOneShot(craftableSFX);
    }
    public void PlayMergeSFX() {
        audioSource.PlayOneShot(mergeSFX);
    }
    public void PlayAttackSFX() {
        audioSource.PlayOneShot(attackSFX);
    }
    public void PlayItemTouchSFX() {
        audioSource.PlayOneShot(itemTouchSFX);
    }
    public void PlayInventorySFX() {
        audioSource.PlayOneShot(inventorySFX);
    }
    public void PlayDeleteSFX() {
        audioSource.PlayOneShot(deleteSFX);
    }
    public void PlaySnapBackSFX() {
        audioSource.PlayOneShot(snapBackSFX);
    }
    public void PlayNewRecipeSFX() {
        audioSource.PlayOneShot(newRecipeSFX);
    }
    public void PlayEnemySpawnSFX() {
        audioSource.PlayOneShot(enemySpawnSFX);
    }
}
