using UnityEngine;

public class Clothing : Photon.MonoBehaviour
{
    public GameObject player;
    public Animator anim;
    public SpriteRenderer sr;
    public PhotonView photonView;
    public int bodyID;

    private bool butcher;
    private int direction;

    // Update is called once per frame
    void Update()
    {
        if(!photonView.isMine)
        {
            return;
        }

        // keyboard controls
        if (direction!= 0 && player.GetComponent<Player>().direction == 0)
        {
            direction = 0;
            photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }

        else if (direction != 1 && player.GetComponent<Player>().direction == 1)
        {
            direction = 1;
            photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }

        if(!butcher && player.GetComponent<Player>().butcher)
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
            if (butcher && !player.GetComponent<Player>().butcher)
            {
                butcher = false;
                anim.SetBool("butcher", false);
                if (bodyID == 0)
                {
                    transform.position -= new Vector3(0, -0.0001f, 0);
                }
            }

            // animation updates
            if (player.GetComponent<Player>().anim.GetBool("isMoving"))
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
