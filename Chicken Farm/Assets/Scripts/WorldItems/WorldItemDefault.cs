using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemDefault : WorldItems
{
    private void OnMouseEnter()
    {
        Hover();
    }

    private void OnMouseExit()
    {
        Unhover();
    }
}