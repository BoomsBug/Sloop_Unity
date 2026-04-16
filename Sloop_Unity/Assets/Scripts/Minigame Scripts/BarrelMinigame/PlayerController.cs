using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float parryRange = 2f;
    public int parryScore = 20;
    public float parryCooldown = 0.5f;

    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Jump Audio")]
    public AudioClip[] jumpClips;
    [Range(0f, 1f)] public float jumpVolume = 1f;
    [Range(0.8f, 1.2f)] public float jumpPitchMin = 0.9f;
    [Range(0.8f, 1.2f)] public float jumpPitchMax = 1.1f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private BarrelDashTestManager manager;
    private float lastParryTime = -10f;
    private float lastDashTime = -10f;
    private bool isDashing = false;
    private bool isInvincible = false;
    private int dashCharges = 0;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<BarrelDashTestManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlayJumpSound()
    {
        if (jumpClips.Length == 0 || audioSource == null) return;
        AudioClip clip = jumpClips[Random.Range(0, jumpClips.Length)];
        audioSource.pitch = Random.Range(jumpPitchMin, jumpPitchMax);
        audioSource.PlayOneShot(clip, jumpVolume);
    }

    void Update()
    {
        // Jump input (Space)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            PlayJumpSound();
            animator.SetTrigger("Jump");
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

        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", horizontalSpeed);
        animator.SetBool("IsGrounded", isGrounded);

        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0 && !isDashing)
        {
            spriteRenderer.flipX = moveInput < 0;
        }
        else if (rb.velocity.x != 0 && !isDashing)
        {
            spriteRenderer.flipX = rb.velocity.x < 0;
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
        animator.SetTrigger("Slash");

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
            dashCharges++;

            if (manager != null)
                manager.AddParryScore(parryScore);
        }
    }

    public int GetDashCharges()
    {
        return dashCharges;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;          // Invincible only for cannons (see OnCollisionEnter2D)
        lastDashTime = Time.time;
        dashCharges--;

        Vector2 originalVelocity = rb.velocity;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float direction = Input.GetAxisRaw("Horizontal");
        if (direction == 0)
            direction = spriteRenderer.flipX ? -1 : 1;

        float dashSpeed = dashDistance / dashDuration;
        rb.velocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.velocity = originalVelocity;
        isDashing = false;
        isInvincible = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Barrel"))
        {
            // Barrels always kill the player, even during dash
            if (manager != null)
                manager.GameOver();
        }
        else if (collision.gameObject.CompareTag("FlyingCannon"))
        {
            // Only die from cannon if NOT invincible (dashing)
            if (!isInvincible && manager != null)
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
        isInvincible = false;
        dashCharges = 0;
        StopAllCoroutines();
    }
}