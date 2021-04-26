using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Oven : Structure
{
    public GameObject stored;
    public GameObject chickenPrefab;
    public Light2D cookLight;

    public float timer, cooldown;
    public int mode;
    public bool isOn;

    public void Update()
    {
        if(mode != 0)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                if (stored != null)
                {
                    if (stored.GetComponent<Item>().cookedMagnitude < 510)
                    {
                        stored.GetComponent<Item>().cookedMagnitude += 1;
                    }
                }
                    
                timer = 0;
            }
        }

        if (anim.GetBool("isOn") && mode == 0)
        {
            anim.SetBool("isOn", false);
        }
        else if(!anim.GetBool("isOn") && mode != 0)
        {
            anim.SetBool("isOn", true);
        }

        CheckHovering();
    }

    [PunRPC]
    private void RemoveChicken()
    {
        stored = null;
    }

    [PunRPC]
    private void SwapChicken(float cookedMagnitude)
    {
        stored = Instantiate(chickenPrefab);
        stored.GetComponent<Item>().cookedMagnitude = cookedMagnitude;
    }

    [PunRPC]
    public void ChangeMode(int mode)
    {
        this.mode = mode;

        if (mode == 1)
        {
            cooldown = 1f;
            cookLight.intensity = 0.25f;
        }
        else if (mode == 2)
        {
            cooldown = 0.66f;
            cookLight.intensity = 0.5f;
        }
        else if (mode == 3)
        {
            cooldown = 0.33f;
            cookLight.intensity = 0.75f;
        }
        else
        {
            cookLight.intensity = 0;
        }
    }
}
