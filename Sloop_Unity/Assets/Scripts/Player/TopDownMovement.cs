using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal"); // Notice: GetAxis, not GetAxisRaw
        moveInput.y = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // Smooth acceleration/deceleration
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 
            moveInput.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration);
    }
}