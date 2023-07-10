using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    public List<Ingredient> ingredients ;

    public Ingredient GetIngredientByName(IngredientType name)
    {
        return ingredients.Find(ingredient => ingredient.GetName() == name);
    }

}