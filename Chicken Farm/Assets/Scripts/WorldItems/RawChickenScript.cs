using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawChickenScript : WorldItems
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