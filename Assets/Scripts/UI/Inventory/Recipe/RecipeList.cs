using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe List", menuName = "Recipe List")]
public class RecipeList : ScriptableObject
{
    public List<Recipe> Container = new List<Recipe>();
    public void Clear() {
        // clears items in container
        Container.Clear();
    }
    public void AddRecipe(Recipe _recipe) {
        // adds recipe to the recipe list
        Container.Add(_recipe);
    }
    public void RemoveRecipe(Recipe _recipe) {
        Container.Remove(_recipe);
    }
    public bool HasRecipe(Recipe _recipe) {
        for (int i = 0; i < Container.Count; i++) {
            if (_recipe.IsEqual(Container[i])) return true;
        }
        return false;
    }
    public bool HasRecipeIngredients(Recipe _recipe) {
        for (int i = 0; i < Container.Count; i++) {
            if (_recipe.IsEqualIngredients(Container[i])) return true;
        }
        return false;
    }
}
[System.Serializable]
public class Recipe
{
    public ItemObject item1, item2, result;
    TierLevel recipeTierLevel;
    public Recipe(ItemObject _item1, ItemObject _item2, ItemObject _result, TierLevel _recipeTierLevel) {
        item1 = _item1;
        item2 = _item2;
        result = _result;
        recipeTierLevel = _recipeTierLevel;
    }
    public bool IsEqual(Recipe _recipe) {
        return item1 == _recipe.item1 && 
            item2 == _recipe.item2 && 
            result == _recipe.result && 
            recipeTierLevel == _recipe.recipeTierLevel;
    }
    public bool IsEqualIngredients(Recipe _recipe) {
        return (item1 == _recipe.item1 && 
            item2 == _recipe.item2) || (item2 == _recipe.item1 && 
            item1 == _recipe.item2);
    }
}
/*
Contains the recipes for merging
*/