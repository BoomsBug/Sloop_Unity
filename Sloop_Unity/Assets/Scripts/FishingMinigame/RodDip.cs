using System.Collections;
using UnityEngine;

public class RodDip : MonoBehaviour
{
    public float topY = 1.0f;        // resting position
    public float bottomY = -3.0f;    // how deep it dips
    public float downTime = 0.12f;   // dip speed down
    public float upTime = 0.18f;     // return speed up
    public float cooldown = 0.25f;   // delay before next dip

    public bool IsDipping { get; private set; }

    void Start()
    {
        // snap to top at start
        Vector3 p = transform.position;
        p.y = topY;
        transform.position = p;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsDipping)
            StartCoroutine(Dip());
    }

    IEnumerator Dip()
    {
        IsDipping = true;

        // move down
        yield return MoveY(topY, bottomY, downTime);

        // move up
        yield return MoveY(bottomY, topY, upTime);

        // small cooldown
        yield return new WaitForSeconds(cooldown);

        IsDipping = false;
    }

    IEnumerator MoveY(float from, float to, float time)
    {
        float t = 0f;
        Vector3 p = transform.position;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / time);

            p.y = Mathf.Lerp(from, to, a);
            transform.position = p;

            yield return null;
        }

        p.y = to;
        transform.position = p;
    }
}