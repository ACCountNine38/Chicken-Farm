using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    // base attributes
    public float speed;
    public float runSpeed;
    public string status;
    protected float statusTimer;
    protected float maxTimer;
    protected Vector3 randomDirection;
    protected Color original;
    protected int direction = 1;

    // photon methods that are used to sync on different devices
    [PunRPC]
    protected void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    protected void FlipFalse()
    {
        sr.flipX = false;
    }

    [PunRPC]
    protected void Die()
    {
        if(photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
