using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;

    public float smoothSpeed = 0.1f;
    public Vector2 offset;

    public int mapWidth, mapHeight;

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector2 desiredPos = new Vector2(
            Mathf.Clamp(target.transform.position.x + offset.x, -(mapWidth / 2 - 2) * 5, (mapWidth / 2 - 2) * 5),
            Mathf.Clamp(target.transform.position.y + offset.y, -(mapHeight / 2 - 1) * 5, (mapWidth / 2 - 1) * 5));
        Vector2 smoothedPos = Vector2.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }
}
