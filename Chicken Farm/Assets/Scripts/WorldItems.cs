using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldItems : SceneObject
{
    public bool isPickedUp, overlap;

    [PunRPC]
    public void PickUp()
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
