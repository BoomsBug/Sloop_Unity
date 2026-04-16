using System.Collections;
using UnityEngine;

public class RodDip : MonoBehaviour
{
    [Header("Rod Positions")]
    public float topY = 1.0f;          // normal resting position
    public float chargeY = 2.0f;       // how high it lifts while charging
    public float minDipY = -1.5f;      // shallow dip on quick tap
    public float maxDipY = -4.3f;      // deepest dip on full charge

    [Header("Charge")]
    public float maxChargeTime = 1.2f; // time to reach full charge

    [Header("Movement")]
    public float releaseDownTime = 0.12f;
    public float returnUpTime = 0.18f;
    public float cooldown = 0.25f;

    public bool IsDipping { get; private set; }

    [Header("Fishing Audio")]
    public AudioClip dipSound;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.9f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.1f;

    private AudioSource audioSource;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        SetY(topY);
    }

    void Update()
    {
        if (IsDipping)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);

            float chargePercent = chargeTimer / maxChargeTime;

            // Move upward while charging
            float currentY = Mathf.Lerp(topY, chargeY, chargePercent);
            SetY(currentY);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            float chargePercent = chargeTimer / maxChargeTime;
            float dipTargetY = Mathf.Lerp(minDipY, maxDipY, chargePercent);
            float releaseStartY = transform.position.y;

            PlayDipSound();
            StartCoroutine(Dip(releaseStartY, dipTargetY));
        }
    }

    void PlayDipSound()
    {
        if (dipSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.PlayOneShot(dipSound, volume);
        }
    }

    IEnumerator Dip(float fromY, float targetDipY)
    {
        IsDipping = true;

        // Shoot downward from current charged height
        yield return MoveY(fromY, targetDipY, releaseDownTime);

        // Return to resting position
        yield return MoveY(targetDipY, topY, returnUpTime);

        yield return new WaitForSeconds(cooldown);

        IsDipping = false;
    }

    IEnumerator MoveY(float from, float to, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / time);
            float y = Mathf.Lerp(from, to, a);
            SetY(y);
            yield return null;
        }

        SetY(to);
    }

    void SetY(float y)
    {
        Vector3 p = transform.position;
        p.y = y;
        transform.position = p;
    }
}