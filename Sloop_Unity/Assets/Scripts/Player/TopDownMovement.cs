using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopDownPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    
    // Interaction colors
    public Color defaultColor = Color.blue;
    public Color npcInteractionColor = Color.green;
    public Color itemPickupColor = Color.yellow;
    public Color merchantInteractionColor = Color.green;
    
    // UI References
    public GameObject merchantPanel;
    public Text foodPriceText;
    public Text woodPriceText;
    public Text foodAmountText;
    public Text woodAmountText;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    
    // Track what we're near
    private bool isNearNPC = false;
    private bool isNearItem = false;
    private bool isNearMerchant = false;
    private bool isNearShip = false;

    private PlayerResources playerResources;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerResources = GetComponent<PlayerResources>();
        rb.gravityScale = 0;
        spriteRenderer.color = defaultColor;
        
        // Hide merchant UI initially
        if (merchantPanel != null)
            merchantPanel.SetActive(false);
    }
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        
        // Handle interactions
        if (Input.GetKeyDown(KeyCode.E) && isNearMerchant)
        {
            if (spriteRenderer.color == defaultColor)
            {
                // Open merchant UI
                spriteRenderer.color = merchantInteractionColor;
                OpenMerchantUI();
            }
            else if (spriteRenderer.color == merchantInteractionColor)
            {
                // Close merchant UI
                spriteRenderer.color = defaultColor;
                CloseMerchantUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isNearNPC && spriteRenderer.color == defaultColor)
        {
            spriteRenderer.color = npcInteractionColor;
            Debug.Log("Talked to NPC");
        } 
        else if (Input.GetKeyDown(KeyCode.E) && isNearNPC && spriteRenderer.color == npcInteractionColor)
        {
            spriteRenderer.color = defaultColor;
            Debug.Log("Stopped talking to NPC");
        }
        
        if (Input.GetKeyDown(KeyCode.F) && isNearItem)
        {
            spriteRenderer.color = itemPickupColor;
            Debug.Log("Picked up item");
        }
        
        // Reset to default color when moving away
        if (moveInput.magnitude > 0.1f && !isNearNPC && !isNearItem && !isNearMerchant)
        {
            spriteRenderer.color = defaultColor;
            CloseMerchantUI();
        }

        // Return to ship
        if (Input.GetKeyDown(KeyCode.E) && isNearShip)
        {
            ReturnToShip();
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 
            moveInput.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration);
    }
    
    void OpenMerchantUI()
    {
        if (merchantPanel != null)
        {
            merchantPanel.SetActive(true);
            UpdateMerchantPrices();
            Debug.Log("Merchant UI opened");
        }
    }
    
    void CloseMerchantUI()
    {
        if (merchantPanel != null)
        {
            merchantPanel.SetActive(false);
            Debug.Log("Merchant UI closed");
        }
    }
    
    void UpdateMerchantPrices()
    {
        if (playerResources != null && merchantPanel.activeSelf)
        {
            int foodCost = playerResources.crewMates * 2;
            int foodAmount = playerResources.crewMates * 3;
            int woodCost = playerResources.shipLevel * 5;
            int woodAmount = playerResources.shipLevel * 4;
            
            if (foodPriceText != null)
                foodPriceText.text = "Cost: " + foodCost + " Gold";
            if (foodAmountText != null)
                foodAmountText.text = "Get: " + foodAmount + " Food";
            if (woodPriceText != null)
                woodPriceText.text = "Cost: " + woodCost + " Gold";
            if (woodAmountText != null)
                woodAmountText.text = "Get: " + woodAmount + " Wood";
        }
    }
    
    // Public methods for UI buttons to call
    public void BuyFood()
    {
        if (playerResources != null && playerResources.BuyFoodFromMerchant())
        {
            UpdateMerchantPrices();
        }
    }
    
    public void BuyWood()
    {
        if (playerResources != null && playerResources.BuyWoodFromMerchant())
        {
            UpdateMerchantPrices();
        }
    }

    private void ReturnToShip()
    {
        Debug.Log("Returning to ship...");

        // optional clean-up
        spriteRenderer.color = defaultColor;
        CloseMerchantUI();

        // Load sailing scene via state machine (keeps flow consistent)
        GameManager.Instance.UpdateGameState(GameState.Sailing);
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Merchant"))
        {
            isNearMerchant = true;
        }
        if (other.CompareTag("NPC"))
        {
            isNearNPC = true;
        }
        if (other.CompareTag("Ship"))
        {
            isNearShip = true;
        }
        else if (other.CompareTag("Item"))
        {
            isNearItem = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Merchant"))
        {
            isNearMerchant = false;
            spriteRenderer.color = defaultColor;
            CloseMerchantUI();
        }
        if (other.CompareTag("NPC"))
        {
            isNearNPC = false;
            spriteRenderer.color = defaultColor;
        }
        if (other.CompareTag("Ship"))
        {
            isNearShip = false;
            spriteRenderer.color = defaultColor;
        }
        else if (other.CompareTag("Item"))
        {
            isNearItem = false;
            spriteRenderer.color = defaultColor;
        }
    }


}