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

    [Header("Lives")]
    public int maxLives = 2;
    public float invincibleAfterHitDuration = 1f;

    [Header("Audio")]
    public AudioClip[] jumpClips;
    [Range(0f, 1f)] public float jumpVolume = 1f;
    [Range(0.8f, 1.2f)] public float jumpPitchMin = 0.9f;
    [Range(0.8f, 1.2f)] public float jumpPitchMax = 1.1f;

    

    [Header("Boundaries")]
    public float leftBoundaryOffset = 0.2f;
    public float rightBoundaryOffset = 0.2f;

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
    private int currentLives;
    private int parryCount = 0;
    private AudioSource audioSource;
    private Camera mainCamera;
    private Vector2 lastHitPosition;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<BarrelDashTestManager>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        currentLives = maxLives;
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
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            PlayJumpSound();
            animator.SetTrigger("Jump");
        }

        // Parry (Left Click)
        if (Input.GetMouseButtonDown(0) && Time.time >= lastParryTime + parryCooldown && !isDashing)
        {
            TryParry();
        }

        // Dash (Right Click)
        if (Input.GetMouseButtonDown(1) && !isDashing && Time.time >= lastDashTime + dashCooldown && dashCharges > 0)
        {
            StartCoroutine(Dash());
        }

        // Animations
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", horizontalSpeed);
        animator.SetBool("IsGrounded", isGrounded);

        // Flipping sprite
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

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Vector3 viewMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 viewMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float leftBoundary = viewMin.x + leftBoundaryOffset;
        float rightBoundary = viewMax.x - rightBoundaryOffset;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBoundary, rightBoundary);
        transform.position = pos;
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
            parryCount++;

            if (manager != null)
                manager.UpdateParryCounter(parryCount);

            if (parryCount % 10 == 0)
            {
                currentLives++;
                if (currentLives > 99) currentLives = 99;
                if (manager != null)
                    manager.UpdateLivesUI(currentLives);
                Debug.Log("Bonus life from parry! Lives: " + currentLives);
            }

            if (manager != null)
                manager.AddParryScore(parryScore);
        }
    }

    // Called when dashing into a flying cannon
    void OnDashHitFlyingCannon(FlyingCannon cannon)
    {
        // Destroy the cannon
        Destroy(cannon.gameObject);

        // Reward: dash charge refunded + parry counter increment
        dashCharges++;
        parryCount++;

        // Update UI
        if (manager != null)
        {
            manager.UpdateParryCounter(parryCount);
            manager.AddParryScore(parryScore);   // Also add score for dash-parry
        }

        // Bonus life every 10 parries (combined from left-click and dash)
        if (parryCount % 10 == 0)
        {
            currentLives++;
            if (currentLives > 99) currentLives = 99;
            if (manager != null)
                manager.UpdateLivesUI(currentLives);
            Debug.Log("Bonus life from dash parry! Lives: " + currentLives);
        }
    }

    public int GetDashCharges()
    {
        return dashCharges;
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
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

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsed = 0f;
        while (elapsed < invincibleAfterHitDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    void TakeDamage()
    {
        if (isInvincible || isDashing) return;

        currentLives--;
        if (manager != null)
            manager.UpdateLivesUI(currentLives);

        if (currentLives <= 0)
        {
            if (manager != null)
                manager.GameOver();
        }
        else
        {
            transform.position = lastHitPosition;
            rb.velocity = Vector2.zero;
            StartCoroutine(InvincibilityFrames());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Barrel"))
        {
            if (isDashing)
            {
                Destroy(collision.gameObject);
                return;
            }
            lastHitPosition = collision.contacts[0].point;
            TakeDamage();
        }
        else if (collision.gameObject.CompareTag("FlyingCannon"))
        {
            if (isDashing)
            {
                FlyingCannon cannon = collision.gameObject.GetComponent<FlyingCannon>();
                if (cannon != null)
                    OnDashHitFlyingCannon(cannon);
                return;
            }
            lastHitPosition = collision.contacts[0].point;
            TakeDamage();
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
        currentLives = maxLives;
        parryCount = 0;
        if (manager != null)
        {
            manager.UpdateLivesUI(currentLives);
            manager.UpdateParryCounter(parryCount);
        }
        StopAllCoroutines();
        spriteRenderer.enabled = true;
    }
}