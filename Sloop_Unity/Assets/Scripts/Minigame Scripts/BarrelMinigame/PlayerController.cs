using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float parryRange = 2f;          // How close a cannon must be to parry
    public int parryScore = 20;             // Points awarded for a successful parry
    public float parryCooldown = 0.5f;      // Seconds between parries

    private Rigidbody2D rb;
    private bool isGrounded;
    private BarrelDashTestManager manager;
    private float lastParryTime = -10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        manager = FindObjectOfType<BarrelDashTestManager>();
    }

    void Update()
    {
        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        // Parry input (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastParryTime + parryCooldown)
        {
            TryParry();
        }
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void TryParry()
    {
        // Find all flying cannons within parry range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, parryRange, LayerMask.GetMask("Default")); // Adjust layer if needed
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
            if (manager != null)
                manager.AddParryScore(parryScore);
            Debug.Log("Parried a cannon!");
        }
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

        if (manager != null && manager.playerStartPos != null)
            transform.position = manager.playerStartPos.position;

        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}