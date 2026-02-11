using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour
{
    // Resource amounts
    public int gold = 100;
    public int wood = 0;
    public int food = 0;
    public int power = 0;
    
    // Crew and Ship stats
    public int crewMates = 5;
    public int shipLevel = 1;
    
    // UI References
    public Text goldText;
    public Text woodText;
    public Text foodText;
    public Text powerText;
    public Text crewText;
    public Text shipText;

    void Start()
    {
        UpdateUI();
    }
    
    // Simple functions to modify resources
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }
    
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void AddWood(int amount)
    {
        wood += amount;
        UpdateUI();
    }
    
    public bool UseWood(int amount)
    {
        if (wood >= amount)
        {
            wood -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void AddFood(int amount)
    {
        food += amount;
        UpdateUI();
    }
    
    public bool UseFood(int amount)
    {
        if (food >= amount)
        {
            food -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void AddCrew(int amount)
    {
        crewMates += amount;
        if (crewMates < 0) crewMates = 0;
        UpdateUI();
    }
    
    public void UpgradeShip()
    {
        shipLevel++;
        UpdateUI();
    }
    
    // Upkeep calculation functions
    public int CalculateCrewUpkeep()
    {
        // Each crewmate costs 2 gold per day to maintain
        return crewMates * 2;
    }
    
    public int CalculateShipUpkeep()
    {
        // Higher ship level costs more wood (5 wood per level)
        return shipLevel * 5;
    }
    
    // Merchant purchase functions
    public bool BuyFoodFromMerchant()
    {
        // Food cost scales with crew size (2 gold per crewmate)
        int foodCost = crewMates * 2;
        int foodAmount = crewMates * 3; // 3 food per crewmate
        
        if (SpendGold(foodCost))
        {
            AddFood(foodAmount);
            Debug.Log($"Bought {foodAmount} food for {foodCost} gold");
            return true;
        }
        return false;
    }
    
    public bool BuyWoodFromMerchant()
    {
        // Wood cost scales with ship level (5 gold per ship level)
        int woodCost = shipLevel * 5;
        int woodAmount = shipLevel * 4; // 4 wood per ship level
        
        if (SpendGold(woodCost))
        {
            AddWood(woodAmount);
            Debug.Log($"Bought {woodAmount} wood for {woodCost} gold");
            return true;
        }
        return false;
    }
    
    // Calculate total daily costs
    public void ApplyDailyCosts()
    {
        int crewCost = CalculateCrewUpkeep();
        int shipCost = CalculateShipUpkeep();
        
        if (SpendGold(crewCost) && UseWood(shipCost))
        {
            Debug.Log($"Daily costs: {crewCost} gold for crew, {shipCost} wood for ship");
        }
        else
        {
            Debug.LogWarning("Not enough resources for daily upkeep!");
        }
    }
    
    // Update UI display
    void UpdateUI()
    {
        if (goldText != null)
            goldText.text = "Gold: " + gold;
        if (woodText != null)
            woodText.text = "Wood: " + wood;
        if (foodText != null)
            foodText.text = "Food: " + food;
        if (powerText != null)
            powerText.text = "Power: " + power;
        if (crewText != null)
            crewText.text = "Crew: " + crewMates;
        if (shipText != null)
            shipText.text = "Ship: Lvl " + shipLevel;
    }
}