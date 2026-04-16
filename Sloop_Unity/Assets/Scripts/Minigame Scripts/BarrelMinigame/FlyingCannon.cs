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
        // Remove all AnimationEvents from the clip to prevent the error
        Animator animator = GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "BirdFly" && clip.events.Length > 0)
                {
                    clip.events = new AnimationEvent[0];
                    Debug.Log("Cleared empty AnimationEvent from BirdFly clip");
                }
            }
        }

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
