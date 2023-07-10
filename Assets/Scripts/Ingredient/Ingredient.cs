using System;
using UnityEngine;

[System.Serializable]
public class Ingredient
{
    [SerializeField] private IngredientType name;
    [SerializeField] private int maxQuantity;
    [SerializeField] private int quantity;
    [SerializeField] private GameObject gameObject;

    public event Action<int, int> QuantityChanged;

    public int Quantity
    {
        get { return quantity; }
        set
        {
            int previousQuantity = quantity;
            quantity = Mathf.Clamp(value, 0, maxQuantity);
            QuantityChanged?.Invoke(previousQuantity, quantity);
        }
    }

    public Ingredient(IngredientType name, int maxQuantity)
    {
        this.name = name;
        this.maxQuantity = maxQuantity;
        this.quantity = 0;
    }

    public void IncreaseQuantity(int amount)
    {
        Quantity += amount;
    }

    public void DecreaseQuantity(int amount)
    {
        Quantity -= amount;
    }

    public int GetQuantity()
    {
        return quantity;
    }

    public int GetMaxQuantity()
    {
        return maxQuantity;
    }

    public IngredientType GetName()
    {
        return name;
    }
    public void ResetQuantity()
    {
        Quantity = maxQuantity;
    }
}