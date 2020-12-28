using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeScript : WorldItems
{
    public void Update()
    {
        CheckHovering();
    }

    [PunRPC]
    public void FlipTrue()
    {
        sr.flipX = true;
    }
}