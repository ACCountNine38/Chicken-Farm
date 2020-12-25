using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldItems : MonoBehaviour
{
    public SpriteRenderer sr;
    public PhotonView photonView;

    protected Color original;
    public bool selected, isPickedUp;

    void Awake()
    {
        original = sr.color;
        photonView = GetComponent<PhotonView>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected void Hover()
    {
        selected = true;
        sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b - 100);
    }

    protected void Unhover()
    {
        selected = false;
        sr.material.color = original;
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
