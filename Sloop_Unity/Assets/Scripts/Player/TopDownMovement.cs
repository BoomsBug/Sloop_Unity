using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    
    // Interaction colors
    public Color defaultColor = Color.blue;
    public Color npcInteractionColor = Color.green;
    public Color itemPickupColor = Color.yellow;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    
    // Track what we're near
    private bool isNearNPC = false;
    private bool isNearItem = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        spriteRenderer.color = defaultColor;
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        
        // Handle interactions
        if (Input.GetKeyDown(KeyCode.E) && isNearNPC)
        {
            spriteRenderer.color = npcInteractionColor;
            Debug.Log("Talked to NPC");
        }
        
        if (Input.GetKeyDown(KeyCode.F) && isNearItem)
        {
            spriteRenderer.color = itemPickupColor;
            Debug.Log("Picked up item");
        }
        
        // Reset to default color when moving away
        if (moveInput.magnitude > 0.1f && !isNearNPC && !isNearItem)
        {
            spriteRenderer.color = defaultColor;
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 
            moveInput.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = true;
        }
        else if (other.CompareTag("Item"))
        {
            isNearItem = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = false;
            spriteRenderer.color = defaultColor;
        }
        else if (other.CompareTag("Item"))
        {
            isNearItem = false;
            spriteRenderer.color = defaultColor;
        }
    }
}