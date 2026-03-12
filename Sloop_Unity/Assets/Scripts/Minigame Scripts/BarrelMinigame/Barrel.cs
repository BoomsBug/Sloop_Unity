using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float speed = 5f;
    private bool moving = true;
    private bool hasLanded = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Rigidbody should be Dynamic in the prefab (set in Inspector)
    }

    void Update()
    {
        if (moving && hasLanded)
        {
            // Apply constant leftward velocity while preserving vertical motion
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Detect first contact with ground to start rolling
        if (collision.gameObject.CompareTag("Ground") && !hasLanded)
        {
            hasLanded = true;
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void StopMoving()
    {
        moving = false;
        if (rb != null)
            rb.velocity = Vector2.zero;
    }
}