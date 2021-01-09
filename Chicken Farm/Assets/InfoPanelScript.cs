using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelScript : MonoBehaviour
{
    public GameObject infoPanel;
    public Image icon;
    public Text title, info;

    public GameObject currentObject;

    // icons
    public Sprite normalChicken, thinChicken, thiccChicken, doorClose, lightSwitch, ovenOff;

    // Update is called once per frame
    private void Update()
    {
        if(currentObject == null)
        {
            icon.enabled = false;
            title.enabled = false;
            info.enabled = false;
        }
        else
        {
            if(!icon.enabled)
            {
                icon.enabled = true;
                title.enabled = true;
                info.enabled = true;
            }

            
            if (currentObject.CompareTag("Chicken"))
            {
                Chicken chicken = currentObject.GetComponent<Chicken>();
                
                if(chicken.young)
                {
                    title.text = "Young Chicken";
                    info.text = "This chicken is a big smol...";
                    icon.sprite = normalChicken;
                }
                else if (chicken.type == 0)
                {
                    title.text = "Normal Chicken";
                    info.text = "This chicken looks fine";
                    icon.sprite = normalChicken;
                }
                else if(chicken.type == 1)
                {
                    title.text = "Thin Chicken";
                    info.text = "This chicken is looking a bit thin";
                    icon.sprite = thinChicken;
                }
                else if (chicken.type == 2)
                {
                    title.text = "Thicc Chicken";
                    info.text = "This chicken is looking too thicc";
                    icon.sprite = thiccChicken;
                }
            }

            // structures
            else if (currentObject.CompareTag("Door"))
            {
                Door door = currentObject.GetComponent<Door>();
                title.text = "Door";
                icon.sprite = doorClose;
                if (door.isOpen)
                {
                    info.text = "Who left the door open?";
                }
                else
                {
                    info.text = "This door is currently closed";
                }
            }
            else if (currentObject.CompareTag("Switch"))
            {
                LightSwitch light = currentObject.GetComponent<LightSwitch>();
                title.text = "Light Switch";
                icon.sprite = lightSwitch;
                if (light.isOn)
                {
                    info.text = "Lights are on, beware of electricity bills";
                }
                else
                {
                    info.text = "Can turn the house lights on and off";
                }
            }
            else if (currentObject.CompareTag("Oven"))
            {
                Oven oven = currentObject.GetComponent<Oven>();
                title.text = "Oven";
                icon.sprite = ovenOff;
                if (oven.isOn)
                {
                    info.text = "Make sure you turn the oven off when nothing is inside";
                }
                else
                {
                    info.text = "Can be used to cook normal chickens";
                }
            }
            else if (currentObject.CompareTag("Vendor"))
            {
                icon.sprite = currentObject.GetComponent<Vendor>().sr.sprite;
                title.text = "Vendor";
                info.text = "This guy sells and buys";
            }

            // items
            else if (currentObject.CompareTag("Egg"))
            {
                icon.sprite = currentObject.GetComponent<WorldItems>().sr.sprite;
                title.text = "Egg";
                info.text = "You can sell it or leave it to hatch";
            }
            else if(currentObject.CompareTag("Raw Chicken"))
            {
                icon.sprite = currentObject.GetComponent<WorldItems>().sr.sprite;
                title.text = "Chicken";
                info.text = "Can be cooked in the oven";
            }
            else if (currentObject.CompareTag("Axe"))
            {
                icon.sprite = currentObject.GetComponent<WorldItems>().sr.sprite;
                title.text = "Chicken";
                info.text = "Its no toy... (can be used to butcher chickens)";
            }
            else if (currentObject.CompareTag("Caged Chicken"))
            {
                icon.sprite = currentObject.GetComponent<WorldItems>().sr.sprite;
                title.text = "Caged Chicken";
                info.text = "Should free it before it dies...";
            }
            else if (currentObject.CompareTag("Feed Bag"))
            {
                icon.sprite = currentObject.GetComponent<WorldItems>().sr.sprite;
                title.text = "Chicken Feed";
                info.text = "Use this to feed your chickens";
            }
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float anchorX = infoPanel.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = infoPanel.GetComponent<RectTransform>().anchoredPosition.y;

        if (currentObject == null)
        {
            if (infoPanel.GetComponent<RectTransform>().anchoredPosition.y < 125)
            {
                infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
            }
            else
            {
                infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 125);
            }

            if (infoPanel.GetComponent<RectTransform>().anchoredPosition.y >= 125 && infoPanel.activeSelf)
            {
                infoPanel.SetActive(false);
            }
        }
        else
        {
            if (infoPanel.GetComponent<RectTransform>().anchoredPosition.y > 25)
            {
                infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
            }
            else
            {
                infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 25);
            }

            if (!infoPanel.activeSelf)
            {
                infoPanel.SetActive(true);
            }
        }
    }
}
