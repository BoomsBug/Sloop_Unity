using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float parryRange = 2f;          // How close a cannon must be to parry
    public int parryScore = 20;             // Points awarded for a successful parry
    public float parryCooldown = 0.5f;      // Seconds between parries

    [Header("Dash Settings")]
    public float dashDistance = 3f;          // Distance to dash
    public float dashDuration = 0.2f;        // How long the dash takes
    public float dashCooldown = 0.5f;        // Cooldown before next dash

    private Rigidbody2D rb;
    private bool isGrounded;
    private BarrelDashTestManager manager;
    private float lastParryTime = -10f;
    private float lastDashTime = -10f;
    private bool isDashing = false;
    private int dashCharges = 0;              // Number of dash charges available

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        manager = FindObjectOfType<BarrelDashTestManager>();
    }

    void Update()
    {
        // Jump input (Space)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        // Parry input (Left Mouse Button)
        if (Input.GetMouseButtonDown(0) && Time.time >= lastParryTime + parryCooldown && !isDashing)
        {
            TryParry();
        }

        // Dash input (Right Mouse Button)
        if (Input.GetMouseButtonDown(1) && !isDashing && Time.time >= lastDashTime + dashCooldown && dashCharges > 0)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    void TryParry()
    {
        // Find all flying cannons within parry range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, parryRange);
        FlyingCannon nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("FlyingCannon"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.GetComponent<FlyingCannon>();
                }
            }
        }

        if (nearest != null)
        {
            nearest.Parry();
            lastParryTime = Time.time;

            // Add dash charge on successful parry
            dashCharges++;
            Debug.Log($"Parried a cannon! Dash charges: {dashCharges}");

            if (manager != null)
                manager.AddParryScore(parryScore);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        // Consume one dash charge
        dashCharges--;
        Debug.Log($"Dash used! Remaining charges: {dashCharges}");

        // Store original velocity and disable gravity temporarily
        Vector2 originalVelocity = rb.velocity;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Determine dash direction (facing direction based on movement or last input)
        float direction = Input.GetAxisRaw("Horizontal");
        if (direction == 0)
            direction = transform.localScale.x > 0 ? 1 : -1; // use facing direction

        // Calculate dash velocity
        float dashSpeed = dashDistance / dashDuration;
        rb.velocity = new Vector2(direction * dashSpeed, 0f);

        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);

        // Restore gravity and velocity
        rb.gravityScale = originalGravity;
        rb.velocity = originalVelocity;
        isDashing = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Barrel") || collision.gameObject.CompareTag("FlyingCannon"))
        {
            if (manager != null)
                manager.GameOver();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void ResetPlayer()
    {
        if (manager == null)
            manager = FindObjectOfType<BarrelDashTestManager>();

        if (manager != null)
        {
            transform.position = manager.GetSafeSpawnPosition();
            transform.rotation = Quaternion.identity;
        }
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        isDashing = false;
        dashCharges = 0;          // Reset dash charges on reset
        StopAllCoroutines();
    }
}