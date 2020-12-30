using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOven : MonoBehaviour
{
    public GameObject OvenMenu;
    public Button off, small, medium, high, exit;
    public Text instructions, panel;
    public Image ovenOn, ovenOff;
    public PlayerHotbar hotbar;
    public Image layer1, layer2, layer3;
    public GameObject cookSlot;
    public GameObject CurrentOven;

    public Vector2 originalPosition;
    public bool visible, cooking;
    public int mode = 0;

    private void Awake()
    {
        originalPosition = layer1.transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if(CurrentOven != null && CurrentOven.GetComponent<Oven>().mode != mode)
        {
            CurrentOven.GetComponent<Oven>().photonView.RPC("ChangeSettings", PhotonTargets.AllViaServer, mode); 
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitOven();
        }

        if(mode == 0)
        {
            ovenOn.enabled = false;
            ovenOff.enabled = true;
        }
        else
        {
            ovenOn.enabled = true;
            ovenOff.enabled = false;
        }

        if (mode == 0)
        {
            panel.text = "OFF";
        }
        else if(mode == 1)
        {
            panel.text = "LOW";
        }
        else if (mode == 2)
        {
            panel.text = "MEDIUM";
        }
        else if (mode == 3)
        {
            panel.text = "HIGH";
        }

        if (CurrentOven != null && CurrentOven.GetComponent<Oven>().stored != null)
        {
            layer1.enabled = true;
            layer2.enabled = true;
            layer3.enabled = true;

            float cookedMagnitude = CurrentOven.GetComponent<Oven>().stored.GetComponent<RawChicken>().cookedMagnitude;
            if (cookedMagnitude <= 255)
            {
                Color temp = layer1.color;
                temp.a = (255 - cookedMagnitude) / 255f;
                layer1.color = temp;
                layer2.color = new Color(1f, 1f, 1f, 1f);
            }
            else if (cookedMagnitude <= 510)
            {
                Color temp = layer2.color;
                Color temp2 = layer1.color;
                temp.a = (510 - cookedMagnitude) / 255f;
                temp2.a = 0;
                layer2.color = temp;
                layer1.color = temp2;
            }

            instructions.enabled = false;
        }
        else
        {
            layer1.enabled = false;
            layer2.enabled = false;
            layer3.enabled = false;
            instructions.enabled = true;
        }

        float anchorX = OvenMenu.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = OvenMenu.GetComponent<RectTransform>().anchoredPosition.y;

        if (visible)
        {
            if (OvenMenu.GetComponent<RectTransform>().anchoredPosition.y > 0)
            {
                OvenMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
                cookSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
            }
            if (!OvenMenu.activeSelf)
            {
                OvenMenu.SetActive(true);
                cookSlot.SetActive(true);
            }
        }
        else
        {
            if (OvenMenu.GetComponent<RectTransform>().anchoredPosition.y < 400)
            {
                OvenMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
                cookSlot.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
            }
            if (OvenMenu.GetComponent<RectTransform>().anchoredPosition.y >= 400 && OvenMenu.activeSelf)
            {
                OvenMenu.SetActive(false);
                cookSlot.SetActive(false);
            }
        }
    }

    public void Off()
    {
        mode = 0;
    }

    public void Low()
    {
        mode = 1;
    }

    public void Mid()
    {
        mode = 2;
    }

    public void High()
    {
        mode = 3;
    }

    public void ExitOven()
    {
        visible = false;
    }
}
