using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneMover : MonoBehaviour
{
    public float speed = 4f;
    public Vector2 direction = Vector2.right; // set by spawner
    public float destroyX = 25f;              // offscreen cleanup (spawner can set)

    void Update()
    {
        transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > destroyX)
            Destroy(gameObject);
    }
}
