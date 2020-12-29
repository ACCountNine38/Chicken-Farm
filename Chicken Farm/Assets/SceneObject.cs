using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    public SpriteRenderer sr;
    public PhotonView photonView;
    public Animator anim;
    public BoxCollider2D selectBound;

    protected Color original;
    public bool selected;

    public void Awake()
    {
        original = sr.color;
    }

    protected bool IsHovering()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity))
        {
            if (hit)
            {
                if (hit.collider == selectBound)
                {
                    selected = true;
                    return true;
                }

                if (hit.collider.gameObject.GetComponent<SceneObject>() != null && hit.collider.gameObject.GetComponent<SceneObject>().IsSelected())
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
