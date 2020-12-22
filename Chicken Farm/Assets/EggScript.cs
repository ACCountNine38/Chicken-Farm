using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    public PhotonView photonView;

    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.Space))
        {
            PlayerHotbar hotbar = collision.gameObject.GetComponent<PlayerHotbar>();
            if(!hotbar.IsFull())
            {
                collision.gameObject.GetComponent<PlayerHotbar>().AddItem(Instantiate(hotbar.eggItem));
                photonView.RPC("PickUp", PhotonTargets.MasterClient);
            } 
        }
    }

    [PunRPC]
    private void PickUp()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
