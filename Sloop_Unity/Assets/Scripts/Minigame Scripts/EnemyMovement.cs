using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyMovement : MonoBehaviour
{

    public Transform player;          // Player ship
    public Rigidbody2D rb;

    public float moveSpeed = 4f;

    // Distance enemy wants to keep from player
    public float preferredDistance = 16f;

    private Animator animator;

    int currentDirection = 0; // dir boat currently facing
    int targetDirection = 0; // dir boat should be facing based on input

    float turnDelay = 0.1f; // delay between turning for smoother feel
    float turnTimer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Direction from enemy to player
        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        Vector2 moveDir = Vector2.zero;

        // If too far → move closer
        if (distance > preferredDistance + 1f)
        {
            moveDir = toPlayer.normalized;
        }
        // If too close → move away
        else if (distance < preferredDistance - 1f)
        {
            moveDir = -toPlayer.normalized;
        }
        // If at good distance → strafe (circle player)
        else
        {
            moveDir = new Vector2(-toPlayer.y, toPlayer.x).normalized;
        }

        if (moveDir.magnitude > 0.1f)
        {
            // convert dir to angle
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            angle = (angle + 360) % 360;

            // convert angle to dir
            targetDirection = Mathf.RoundToInt(angle / 45f) % 8;
        }

        // Boat turning over time
        turnTimer += Time.fixedDeltaTime;

        if (turnTimer >= turnDelay && currentDirection != targetDirection)
        {
            turnTimer = 0f;

            int diff = (targetDirection - currentDirection + 8) % 8;

            if (diff > 4)
                currentDirection = (currentDirection - 1 + 8) % 8;
            else
                currentDirection = (currentDirection + 1) % 8;
        }

        // Apply movement
        rb.velocity = moveDir * moveSpeed;

        // update animator to Animate the boat with cur dir
        animator.SetInteger("direction", currentDirection);
    }
}
