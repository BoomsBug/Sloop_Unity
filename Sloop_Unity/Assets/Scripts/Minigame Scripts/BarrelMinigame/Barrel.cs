using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public static float GlobalSpeed = 5f;  // Set by manager

    private bool moving = true;
    private bool hasLanded = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (moving && hasLanded)
        {
            rb.velocity = new Vector2(-GlobalSpeed, rb.velocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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