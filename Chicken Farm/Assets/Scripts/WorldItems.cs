using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldItems : SceneObject
{
    public bool isPickedUp, overlap;

    private string[] tags = {"Egg", "Raw Chicken", "Caged Chicken", "Axe"};

    protected void CheckHovering()
    {
        if (IsHovering())
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

    //private bool IsHovering()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity))
    //    {
    //        if (hit)
    //        {
    //            if (hit.collider == selectBound)
    //            {
    //                selected = true;
    //                return true;
    //            }

    //            if (CheckTags(hit.collider) && hit.collider.gameObject.GetComponent<WorldItems>().selectBound && hit.collider.gameObject.GetComponent<WorldItems>().selected)
    //            {
    //                selected = false;
    //                return false;
    //            }
    //        }
    //    }

    //    selected = false;
    //    return false;
    //}

    //private bool CheckTags(Collider2D collider)
    //{
    //    for(int i = 0; i < tags.Length; i++)
    //    {
    //        if(collider.gameObject.CompareTag(tags[i]))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    [PunRPC]
    public void PickUp()
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
