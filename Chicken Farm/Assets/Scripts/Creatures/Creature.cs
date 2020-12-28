using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;
    public BoxCollider2D selectBound;

    // base attributes
    public float speed;
    public float runSpeed;
    public string status;
    protected float statusTimer;
    protected float maxTimer;
    protected Vector3 randomDirection;
    protected Color original;
    protected int direction = 1;
    protected bool selected;

    protected void CheckHovering()
    {
        if(IsHovering())
        {
            selected = true;
            sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b - 100);
        }
        else
        {
            selected = false;
            sr.material.color = original;
        }
    }

    private bool IsHovering()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity))
        {
            if(hit)
            {
                if (hit.collider == selectBound)
                {
                    selected = true;
                    return true;
                }

                if (hit.collider.gameObject.CompareTag("Chicken") && hit.collider.gameObject.GetComponent<Creature>().IsSelected())
                {
                    selected = false;
                    return false;
                }
            }
        }

        selected = false;
        return false;
    }

    public bool IsSelected()
    {
        return selected;
    }

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
