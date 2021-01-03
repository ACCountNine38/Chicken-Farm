using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : Structure
{
    public int direction = 1;

    public void Update()
    {
        CheckHovering();
    }
}