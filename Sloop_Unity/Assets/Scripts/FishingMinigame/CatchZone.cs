using UnityEngine;

public class CatchZone : MonoBehaviour
{
    public FishingGameManager game;
    public RodDip rodDip;

    void Awake()
    {
        if (!rodDip) rodDip = GetComponent<RodDip>();
        if (!game) game = FindObjectOfType<FishingGameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only catch during the dip motion
        if (rodDip != null && !rodDip.IsDipping) return;

        if (other.CompareTag("Fish"))
        {
            game.CatchFish(other.gameObject);
        }
        else if (other.CompareTag("Loot"))
        {
            game.CatchGold(other.gameObject);
        }
    }
}