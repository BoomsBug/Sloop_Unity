using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public enum MerchantType
    {
        General,
        FoodSpecialist,
        WoodSpecialist
    }
    
    public MerchantType merchantType = MerchantType.General;
    
    // Different merchants might have different prices
    public float foodPriceMultiplier = 1.0f;
    public float woodPriceMultiplier = 1.0f;
}
