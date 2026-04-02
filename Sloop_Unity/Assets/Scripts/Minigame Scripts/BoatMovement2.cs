using System;
using UnityEngine;

public class BoatMovementSimple : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public float speed = 6f;

    int currentDirection = 0; // dir boat currently facing
    int targetDirection = 0; // dir boat should be facing based on input

    float turnDelay = 0.1f; // delay between turning for smoother feel
    float turnTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // converts a dir into a movement vector
    Vector2 DirectionToVector(int dir)
    {
        switch (dir)
        {
            case 0: return new Vector2(1, 0);     // East
            case 1: return new Vector2(1, 1).normalized;
            case 2: return new Vector2(0, 1);     // North
            case 3: return new Vector2(-1, 1).normalized;
            case 4: return new Vector2(-1, 0);    // West
            case 5: return new Vector2(-1, -1).normalized;
            case 6: return new Vector2(0, -1);    // South
            case 7: return new Vector2(1, -1).normalized;
        }

        return Vector2.right;
    }

    void FixedUpdate()
    {
        // get player input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 input = new Vector2(h, v);

        // if player pressing movement key
        if (input.magnitude > 0.1f)
        {
            // convert dir to angle
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
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

        // get Movement dir based on current facing dir
        Vector2 moveDir = DirectionToVector(currentDirection);

        if (input.magnitude > 0.1f)
            rb.velocity = moveDir * speed;
        else
            rb.velocity = Vector2.zero;

        // update animator to Animate the boat with cur dir
        animator.SetInteger("direction", currentDirection);
    }
}