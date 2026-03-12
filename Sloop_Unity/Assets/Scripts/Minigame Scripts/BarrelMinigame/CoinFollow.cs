using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFollow : MonoBehaviour
{
    public Transform target;      // The barrel this coin follows
    public Vector3 offset;        // World-space offset from the barrel

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity; // keep upright
        }
        else
        {
            Destroy(gameObject);
        }
    }
}