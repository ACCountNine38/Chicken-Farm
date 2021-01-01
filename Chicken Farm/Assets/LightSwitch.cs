using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightSwitch : Structure
{
    public Sprite lightsOn, lightsOff;
    public Light2D houseLight;

    public bool isOn;

    public void Update()
    {
        if(!isOn && houseLight.enabled)
        {
            houseLight.enabled = false;
        }
        else if(isOn && !houseLight.enabled)
        {
            houseLight.enabled = true;
        }

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

    [PunRPC]
    public void UpdateLight()
    {
        isOn = !isOn;

        if (isOn)
        {
            houseLight.enabled = true;
            sr.sprite = lightsOn;
        }
        else
        {
            houseLight.enabled = false;
            sr.sprite = lightsOff;
        }
    }
}
