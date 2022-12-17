using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
manages the display of recipes in the inventory.
adds/remove recipes whenever.

some recipes may not be in the recipe screen, but exists, just allow the merging.
- this will store the known list of recipes
*/
public class RecipeManager : MonoBehaviour
{
    public GameObject noSpells;
    public List<Recipe> knownRecipesList;
    public RecipeList recipeList;
    [SerializeField]
    bool knowAllRecipes = true;
    public GameObject recipePrefab;
    public GameObject scrollRect;
    public TierSystem tierSystem;
    RectTransform rectTransform;
    float rectGLGHeight, rectGLGSpacing;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = scrollRect.GetComponent<RectTransform>();
        rectGLGHeight = scrollRect.GetComponent<GridLayoutGroup>().cellSize.y;
        rectGLGSpacing = scrollRect.GetComponent<GridLayoutGroup>().spacing.y;
        recipeList.Clear();
        // InitializeRecipes();
    }
    public void AddRecipe(Recipe recipe) {
        // used to add new recipes that are created on runtime
        if (recipeList.HasRecipeIngredients(recipe)) return; // no duplicate recipes with same ingredients

        AddRecipeToKnown(recipe);
        recipeList.AddRecipe(recipe);
        if (knownRecipesList.Count == 1) noSpells.SetActive(false);
    }
    void InitializeRecipes() {
        // doesn't do anything if it is cleared at the start
        if (knowAllRecipes) {
            for (int i = 0; i < recipeList.Container.Count; i++) {
                knownRecipesList.Add(recipeList.Container[i]);
            }
        }
        for (int i = 0; i < knownRecipesList.Count; i++) {
            AddRecipeImages(knownRecipesList[i]);
        }
        UpdateScrollRect();
    }
    void AddRecipeToKnown(Recipe recipe) {
        knownRecipesList.Add(recipe);
        AddRecipeImages(recipe);
        if (knownRecipesList.Count >= 1) noSpells.SetActive(false);
    }
    void AddRecipeImages(Recipe recipe) {
        // add recipe images in scroll container
        Sprite[] sprites = {
            recipe.item1.prefab.GetComponent<SpriteRenderer>().sprite,
            recipe.item2.prefab.GetComponent<SpriteRenderer>().sprite,
            recipe.result.prefab.GetComponent<SpriteRenderer>().sprite,
        };
        Material[] materials = {
            tierSystem.GetTierLevel(recipe.item1.itemTier).tierMaterial,
            tierSystem.GetTierLevel(recipe.item2.itemTier).tierMaterial,
            tierSystem.GetTierLevel(recipe.result.itemTier).tierMaterial,
        };

        // add recipe prefab
        GameObject temp = Instantiate(recipePrefab, scrollRect.transform);
        temp.GetComponent<RecipeFormula>().ChangeImages(sprites, materials);
        
        UpdateScrollRect();
    }
    void UpdateScrollRect() {
        // updates the height of the scroll rect 
        rectTransform.sizeDelta = new Vector2 (
            rectTransform.sizeDelta.x, 
            (rectGLGHeight + rectGLGSpacing) * knownRecipesList.Count);
    }
}
/*
deals with updating the ui for
 - the scroll rect and recipes
 - and adding recipes
*/