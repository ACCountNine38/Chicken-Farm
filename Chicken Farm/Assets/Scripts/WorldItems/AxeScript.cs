using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeScript : WorldItems
{
    private void OnMouseEnter()
    {
        Hover();
    }

    private void OnMouseExit()
    {
        Unhover();
    }

    [PunRPC]
    public void FlipTrue()
    {
        sr.flipX = true;
    }
}