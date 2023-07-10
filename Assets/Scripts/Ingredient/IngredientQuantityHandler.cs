using TMPro;
using UnityEngine;

public class IngredientQuantityHandler : MonoBehaviour
{
    [SerializeField] private IngredientManager ingredientManager;
    [SerializeField] private TextMeshProUGUI textMeshProText;
    [SerializeField] private IngredientType ingredientName;

    private void OnEnable()
    {
        UpdateText();
        ingredientManager.GetIngredientByName(ingredientName).QuantityChanged += HandleQuantityChanged;
    }

    private void OnDisable()
    {
        ingredientManager.GetIngredientByName(ingredientName).QuantityChanged -= HandleQuantityChanged;
    }

    private void HandleQuantityChanged(int previousQuantity, int currentQuantity)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        Ingredient ingredient = ingredientManager.GetIngredientByName(ingredientName);
        if (ingredient != null)
        {
            textMeshProText.text = $"{ingredient.Quantity}/{ingredient.GetMaxQuantity()}";
        }
    }
}
