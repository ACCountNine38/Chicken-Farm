using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothing : Photon.MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer sr;
    public PhotonView photonView;

    public Player player;

    // Update is called once per frame
    void Update()
    {
        // keyboard controls
        if (player.rb.velocity.x < 0)
        {
            photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }

        else if (player.rb.velocity.x > 0)
        {
            photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }

        // animation updates
        if (player.anim.GetBool("isMoving"))
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    // photon methods that are used to sync on different devices
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
