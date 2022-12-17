using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    LevelLoader levelLoader;
    void Start() {
        levelLoader = FindObjectOfType<LevelLoader>();
        // StartCoroutine(levelLoader.PreloadScene("StartMenu"));
    }
}
