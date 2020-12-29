using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : SceneObject
{
    public Rigidbody2D rb;

    // base attributes
    public float speed;
    public float runSpeed;
    public string status;
    protected float statusTimer;
    protected float maxTimer;
    protected Vector3 randomDirection;
    protected int direction = 1;

    [PunRPC]
    protected void Die()
    {
        if(photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
