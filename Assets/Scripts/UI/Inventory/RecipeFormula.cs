using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeFormula : MonoBehaviour
{
    public Image[] images;
    // Start is called before the first frame update
    void Start()
    {
        if (images.Length != 3) Debug.LogWarning("RecipeFormula: Wrong images length");
    }
    public void ChangeImages(Sprite[] _sprite, Material[] materials) {
        if (_sprite.Length != images.Length) Debug.LogWarning("RecipeFormula: images length not equal");
        if (materials.Length != images.Length) Debug.LogWarning("RecipeFormula: materials length not equal");
        for (int i = 0; i < images.Length; i++) {
            images[i].sprite = _sprite[i];
            // and their material
            images[i].material = materials[i];
        }
    }
}
