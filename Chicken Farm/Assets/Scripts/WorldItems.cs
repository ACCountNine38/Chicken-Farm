using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldItems : SceneObject
{
    public bool isPickedUp, overlap;

    protected void CheckHovering()
    {
        if (IsHovering())
        {
            selected = true;
            sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b - 100);
        }
        else
        {
            selected = false;
            sr.material.color = original;
        }
    }

    [PunRPC]
    public void PickUp()
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
