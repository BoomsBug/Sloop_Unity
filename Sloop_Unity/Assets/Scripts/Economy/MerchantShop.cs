using UnityEngine;
using Sloop.Economy;

namespace Sloop.NPC.Dialogue
{
    /// <summary>
    /// Simple merchant shop with willingness-based pricing.
    /// Low willingness => markup
    /// Neutral => base price
    /// High willingness => discount
    /// </summary>
    public class MerchantShop : MonoBehaviour
    {
        [Header("Base Prices")]
        [SerializeField] private int woodGoldCost = 100;
        [SerializeField] private int woodAmount = 400;

        [SerializeField] private int foodGoldCost = 50;
        [SerializeField] private int foodAmount = 200;

        public void OpenShop(NPCDialogueUI ui, int willingness)
        {
            if (ui == null) return;

            int pricedWoodCost = GetAdjustedCost(woodGoldCost, willingness);
            int pricedFoodCost = GetAdjustedCost(foodGoldCost, willingness);

            ui.SetLine(
                "What would you like to buy?\n" +
                $"Wood: {pricedWoodCost} Gold → {woodAmount} Wood\n" +
                $"Food: {pricedFoodCost} Gold → {foodAmount} Food"
            );

            ui.ShowChoices(
                $"Buy Wood ({pricedWoodCost} Gold → {woodAmount} Wood)",
                () =>
                {
                    TryBuy(
                        ui,
                        pricedWoodCost,
                        Resource.Wood,
                        woodAmount,
                        "Wood"
                    );
                },
                $"Buy Food ({pricedFoodCost} Gold → {foodAmount} Food)",
                () =>
                {
                    TryBuy(
                        ui,
                        pricedFoodCost,
                        Resource.Food,
                        foodAmount,
                        "Food"
                    );
                }
            );
        }

        private void TryBuy(
            NPCDialogueUI ui,
            int goldCost,
            Resource rewardType,
            int rewardAmount,
            string rewardName)
        {
            var rm = ResourceManager.Instance;
            if (rm == null)
            {
                ui.SetLine("(ResourceManager missing in scene.)");
                ui.HideChoices();
                return;
            }

            if (!rm.TrySpend(Resource.Gold, goldCost))
            {
                int gold = rm.GetAmount(Resource.Gold);
                ui.SetLine($"Not enough Gold. Need {goldCost}, you have {gold}.");
                ui.HideChoices();
                return;
            }

            rm.Add(rewardType, rewardAmount);

            int newGold = rm.GetAmount(Resource.Gold);
            int newWood = rm.GetAmount(Resource.Wood);
            int newFood = rm.GetAmount(Resource.Food);

            ui.SetLine(
                $"Deal! You bought {rewardAmount} {rewardName}.\n" +
                $"Gold: {newGold} | Wood: {newWood} | Food: {newFood}"
            );
            ui.HideChoices();
        }

        private int GetAdjustedCost(int baseCost, int willingness)
        {
            // Match the same band logic used elsewhere:
            // Hostile (<= -25): more expensive
            // Friendly (>= 25): discounted
            // Neutral: unchanged

            if (willingness <= -25)
                return Mathf.RoundToInt(baseCost * 1.5f);

            if (willingness >= 25)
                return Mathf.RoundToInt(baseCost * 0.7f);

            return baseCost;
        }
    }
}