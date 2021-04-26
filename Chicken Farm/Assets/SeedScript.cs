using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedScript : MonoBehaviour
{
    public PhotonView photonView;
    public Sprite oneThirdEaten, twoThirdEaten;
    public SpriteRenderer sr;

    private float amountLeft = 12;
    private int amountState = 3;

    private void Update()
    {
        if (amountLeft <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else if (amountLeft <= 4)
        {
            if(amountState != 1)
            {
                amountState = 1;
                sr.sprite = twoThirdEaten;
            }
        }
        else if (amountLeft <= 8)
        {
            if(amountState != 2)
            {
                amountState = 2;
                sr.sprite = oneThirdEaten;
            }
        }
    }

    [PunRPC]
    public void Eat()
    {
        amountLeft -= 1;
    }
}
