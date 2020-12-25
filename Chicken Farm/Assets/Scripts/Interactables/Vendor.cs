using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : MonoBehaviour
{
    public SpriteRenderer sr;
    public PhotonView photonView;
    public Animator anim;
    public int direction = 1;

    private Color original;
    private bool selected;

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

    public bool IsSelected()
    {
        return selected;
    }

    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
    }
}
