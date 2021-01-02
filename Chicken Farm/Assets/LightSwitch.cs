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

        CheckHovering();
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
