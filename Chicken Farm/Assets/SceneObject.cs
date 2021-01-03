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

    protected void CheckHovering()
    {
        if (IsHovering())
        {
            selected = true;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b - 0.2f);
        }
        else
        {
            selected = false;
            sr.color = original;
        }
    }

    protected bool IsHovering()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity))
        {
            if (hit && hit.collider.gameObject.GetComponent<SceneObject>() != null)
            {
                if (hit.collider.gameObject.GetComponent<SceneObject>() != null && hit.collider.gameObject.transform.position.y < transform.position.y)
                {
                    selected = false;
                    return false;
                }

                if (hit.collider == selectBound)
                {
                    selected = true;
                    return true;
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
