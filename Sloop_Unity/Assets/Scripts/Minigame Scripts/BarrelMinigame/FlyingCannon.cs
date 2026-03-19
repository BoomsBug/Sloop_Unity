using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCannon : MonoBehaviour
{
    public static float GlobalSpeed = 5f;  // Set by manager

    private bool moving = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (moving)
        {
            rb.velocity = new Vector2(-GlobalSpeed, 0f);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void StopMoving()
    {
        moving = false;
        rb.velocity = Vector2.zero;
    }

    public void Parry()
    {
        Destroy(gameObject);
    }
}
