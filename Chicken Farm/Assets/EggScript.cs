using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    public SpriteRenderer sr;
    public PhotonView photonView;

    private Color original;
    public bool selected, isPickedUp;

    public void Start()
    {
        original = sr.color;
    }

    private void OnMouseEnter()
    {
        selected = true;
        sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b - 100);
    }

    private void OnMouseExit()
    {
        selected = false;
        sr.material.color = original;
    }

    [PunRPC]
    private void PickUp()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
