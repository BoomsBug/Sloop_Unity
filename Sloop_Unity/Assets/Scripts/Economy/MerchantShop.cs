using UnityEngine;
using Sloop.Economy;

namespace Sloop.NPC.Dialogue
{
    /// <summary>
    /// Very simple merchant shop: one trade offer.
    /// Spend cost -> receive reward.
    /// </summary>
    public class MerchantShop : MonoBehaviour
    {
        [Header("Single Offer (for now)")]
        [SerializeField] private ResourceAmount[] cost =
        {
            new ResourceAmount { type = Resource.Gold, amount = 100 }
        };

        [SerializeField] private ResourceAmount[] reward =
        {
            new ResourceAmount { type = Resource.Wood, amount = 400 }
        };

        public void OpenShop(NPCDialogueUI ui)
        {
            if (ui == null) return;

            ui.SetLine($"Offer: Pay 100 Gold for 400 Wood.");
            ui.ShowChoices(
                "Buy (100 Gold → 400 Wood)",
                () =>
                {
                    TryBuy(ui);
                },
                "Leave",
                () =>
                {
                    ui.SetLine("Come back anytime.");
                    ui.HideChoices();
                }
            );
        }

        private void TryBuy(NPCDialogueUI ui)
        {
            var rm = ResourceManager.Instance;
            if (rm == null)
            {
                ui.SetLine("(ResourceManager missing in scene.)");
                ui.HideChoices();
                return;
            }

            if (!rm.TrySpend(cost))
            {
                int gold = rm.GetAmount(Resource.Gold);
                ui.SetLine($"Not enough Gold. You have {gold}.");
                ui.HideChoices();
                return;
            }

            rm.Add(reward);

            int newGold = rm.GetAmount(Resource.Gold);
            int newWood = rm.GetAmount(Resource.Wood);

            ui.SetLine($"Deal! You bought 400 Wood.\nGold: {newGold} | Wood: {newWood}");
            ui.HideChoices();
        }
    }
}
