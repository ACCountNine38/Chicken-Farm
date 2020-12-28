using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : Structure
{
    public int direction = 1;

    private void OnMouseEnter()
    {
        selected = true;
        sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b - 100);
    }

    private void OnMouseExit()
    {
        selected = false;
        sr.material.color = original;
    }
}