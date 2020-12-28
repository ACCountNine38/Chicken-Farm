using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawChickenScript : WorldItems
{
    public float cookedMagnitude;
    public SpriteRenderer raw, cooked;

    private bool updated;

    public void Awake()
    {
        PhotonView photonView = transform.GetComponent<PhotonView>();

        object[] data = photonView.instantiationData;
        if (data != null && data[0] != null)
        {
            cookedMagnitude = (float)data[0];
        }
    }

    public void Update()
    {
        //CheckHovering();

        if (!updated)
        {
            updated = true;
            UpdateCookMagnitude();
        }

    }

    private void UpdateCookMagnitude()
    {
        if (cookedMagnitude <= 255)
        {
            Color temp = raw.color;
            temp.a = (255 - cookedMagnitude) / 255f;
            raw.color = temp;
        }
        else if (cookedMagnitude <= 510)
        {
            Color temp = cooked.color;
            Color temp2 = raw.color;
            temp.a = (510 - cookedMagnitude) / 255f;
            temp2.a = 0;
            cooked.color = temp;
            raw.color = temp2;
        }
        Debug.Log(raw.color.a);
    }
}