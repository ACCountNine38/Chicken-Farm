using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseHoverPanel : MonoBehaviour
{
    public Image leftMouse, rightMouse;
    public Text hoverInfo, leftClick, rightClick;

    public void Update()
    {
        if (rightClick.text == "")
        {
            rightMouse.enabled = false;
        }
        else
        {
            rightMouse.enabled = true;
        }

        if (leftClick.text == "")
        {
            leftMouse.enabled = false;
        }
        else
        {
            leftMouse.enabled = true;
        }
    }
}
