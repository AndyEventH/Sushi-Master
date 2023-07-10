using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Recipe
{
    public SushiType foodName;
    public GameObject gameObject;
    public IngredientAmount[] ingredients;
}

[System.Serializable]
public struct IngredientAmount
{
    public IngredientType ingredientName;
    public int amount;
}

public enum FoodMakerReturns
{
    RecipeNotFound,
    IngredientNotFound,
    InsufficientQuantity,
    Success,
    Unknown
}
public class FoodManager : MonoBehaviour
{
    [SerializeField] private IngredientManager ingredientManager;

    // Example recipe dictionary
    [SerializeField] private Recipe[] recipes;

    private void Start()
    {
        //FoodMakerReturns ret = MakeFood(SushiType.SalmonNigiri);
        //Debug.Log(ret.ToString());
    }

    // Function to make food based on a specified recipe


    public FoodMakerReturns MakeFood(SushiType foodName)
    {
        Recipe? recipe = FindRecipe(foodName);
        if (!recipe.HasValue)
        {
            Debug.Log("Recipe not found!");
            return FoodMakerReturns.RecipeNotFound;
        }

        Recipe actualRecipe = recipe.Value; // Access the underlying struct value

        foreach (IngredientAmount ingredientAmount in actualRecipe.ingredients)
        {
            Ingredient ingredient = FindIngredient(ingredientAmount.ingredientName);
            if (ingredient == null)
            {
                Debug.Log("Ingredient not found: " + ingredientAmount.ingredientName);
                return FoodMakerReturns.IngredientNotFound;
            }

            if (ingredient.GetQuantity() < ingredientAmount.amount)
            {
                Debug.Log("Insufficient quantity of ingredient: " + ingredientAmount.ingredientName);
                return FoodMakerReturns.InsufficientQuantity;
            }

            ingredient.DecreaseQuantity(ingredientAmount.amount);
            //Debug.Log(ingredient.GetName());
        }

        Debug.Log("Food created: " + foodName);
        return FoodMakerReturns.Success;
    }









    // Function to find a recipe by food name
    private Recipe? FindRecipe(SushiType foodName)
    {
        foreach (Recipe recipe in recipes)
        {
            if (recipe.foodName == foodName)
            {
                return recipe;
            }
        }

        return null;
    }


    // Function to find an ingredient by name
    private Ingredient FindIngredient(IngredientType ingredientName)
    {
        foreach (Ingredient ingredient in ingredientManager.ingredients)
        {
            if (ingredient.GetName() == ingredientName)
            {
                return ingredient;
            }
        }

        return null;
    }

    public Recipe GetRandomRecipe()
    {
        if (recipes.Length == 0)
        {
            Debug.Log("No recipes available!");
            return default;
        }

        int randomIndex = UnityEngine.Random.Range(0, recipes.Length);
        Recipe randomRecipe = recipes[randomIndex];
        return randomRecipe;
    }

    public void RefillIngredients()
    {
        foreach (Ingredient ingredient in ingredientManager.ingredients)
        {
            ingredient.ResetQuantity();
        }
    }

}
