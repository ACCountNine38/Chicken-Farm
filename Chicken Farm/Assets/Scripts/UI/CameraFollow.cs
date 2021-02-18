using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;

    public float smoothSpeed = 0.1f;
    public Vector2 offset;

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 desiredPos = new Vector2(target.transform.position.x + offset.x, target.transform.position.y + offset.y);
        Vector2 smoothedPos = Vector2.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }
}
