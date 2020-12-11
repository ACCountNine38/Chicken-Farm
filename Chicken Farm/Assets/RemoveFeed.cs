using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveFeed : MonoBehaviour
{
    public float displayTime = 5f;

    // built in function that is called when object becomes active
    private void OnEnable()
    {
        // removes this object after the display time
        Destroy(gameObject, displayTime);
    }
}
