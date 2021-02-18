using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvenManager : MonoBehaviour
{
    public Button dial, exit;
    public Text instructions, panel;
    public Image status;
    [HideInInspector]
    public Oven CurrentOven;

    [HideInInspector]
    public Vector2 originalPosition;
    [HideInInspector]
    public bool visible, cooking;
    [HideInInspector]
    public int mode = 0;

    public Sprite off, low, medium, high, heatOff, heatOn;

    public void Update()
    {
        // this is a repeat from functions below incase another user updates the same oven
        if (mode == 0)
        {
            if (panel.text != "OFF")
            {
                panel.text = "OFF";
                status.sprite = heatOff;
                dial.image.sprite = off;
            }
        }
        else if (mode == 1)
        {
            if (panel.text != "LOW")
            {
                panel.text = "LOW";
                status.sprite = heatOn;
                dial.image.sprite = low;
            }
        }
        else if (mode == 2)
        {
            if (panel.text != "MEDIUM")
            {
                panel.text = "MEDIUM";
                status.sprite = heatOn;
                dial.image.sprite = medium;
            }
        }
        else if (mode == 3)
        {
            if (panel.text != "HIGH")
            {
                panel.text = "HIGH";
                status.sprite = heatOn;
                dial.image.sprite = high;
            }
        }

        if (CurrentOven != null && CurrentOven.stored != null)
        {
            if (instructions.enabled)
            {
                instructions.enabled = false;
            }
        }
        else
        {
            if (!instructions.enabled)
            {
                instructions.enabled = true;
            }
        }
    }

    public void NextLevel()
    {
        if (mode == 0)
        {
            mode++;
            panel.text = "LOW";
            status.sprite = heatOn;
            dial.image.sprite = low;
        }
        else if (mode == 1)
        {
            mode++;
            panel.text = "MEDIUM";
            status.sprite = heatOn;
            dial.image.sprite = medium;
        }
        else if (mode == 2)
        {
            mode++;
            panel.text = "HIGH";
            status.sprite = heatOn;
            dial.image.sprite = high;
        }
        else if (mode == 3)
        {
            mode = 0;
            panel.text = "OFF";
            status.sprite = heatOff;
            dial.image.sprite = off;
        }

        // updates all other user's oven
        CurrentOven.GetComponent<Oven>().photonView.RPC("ChangeMode", PhotonTargets.AllViaServer, mode);
    }

    public void ExitOven()
    {
        visible = false;
    }
}
