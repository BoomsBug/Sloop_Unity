using Sloop.Economy;
using UnityEngine;

public class MerchantShopExample : MonoBehaviour
{
    private ResourceAmount[] cost =
    {
        new ResourceAmount { type = Resource.Gold, amount = 10 },
        new ResourceAmount { type = Resource.Wood, amount = 3 }
    };

    public void Buy()
    {
        if (ResourceManager.Instance.TrySpend(cost))
            Debug.Log("Bought item!");
        else
            Debug.Log("Not enough resources!");
    }
}
