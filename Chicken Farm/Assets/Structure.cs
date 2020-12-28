using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public SpriteRenderer sr;
    public PhotonView photonView;
    public Animator anim;

    protected Color original;
    protected bool selected;

    public void Awake()
    {
        original = sr.color;
    }

    public bool IsSelected()
    {
        return selected;
    }

    [PunRPC]
    public void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    public void FlipFalse()
    {
        sr.flipX = false;
    }
}
