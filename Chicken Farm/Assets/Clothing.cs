using UnityEngine;

public class Clothing : Photon.MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer sr;
    public PhotonView photonView;
    public int bodyID;

    public Player player;
    private bool butcher;

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

        if(!butcher && player.butcher)
        {
            butcher = true;
            anim.SetBool("isMoving", false);
            anim.SetBool("butcher", true);
            if(bodyID == 0)
            {
                transform.position += new Vector3(0, -0.0001f, 0);
            }
        }
        else
        {
            if (butcher && !player.butcher)
            {
                butcher = false;
                anim.SetBool("butcher", false);
                if (bodyID == 0)
                {
                    transform.position -= new Vector3(0, -0.0001f, 0);
                }
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
