using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : Structure
{
    public int direction = 1;

    public void Update()
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
}