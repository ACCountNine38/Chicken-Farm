using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    public PhotonView photonView;
    public Light2D ambientLight;

    public float currentTime;

    private float updateCooldown = 0.1f, nextActionTime = 0.0f;

    private void Awake()
    {
        nextActionTime = Time.time;
        currentTime = 60;
        ambientLight.intensity = 1f;
    }

    private void Update()
    {
        Debug.Log(currentTime);
        if(Input.GetKeyDown(KeyCode.M))
        {
            updateCooldown /= 2;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            updateCooldown *= 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ambientLight.intensity = 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ambientLight.intensity = 1;
        }

        StartCoroutine("UpdateTime");
    }

    private void UpdateTime()
    {
        if (Time.time >= nextActionTime)
        {
            nextActionTime += updateCooldown;

            if (currentTime == 3000)
            {
                currentTime = 0;
            }
            else
            {
                currentTime += 1;
            }

            if (currentTime <= 600)
            {
                ambientLight.intensity += 1 / 600f;
                if (ambientLight.intensity > 1f)
                {
                    ambientLight.intensity = 1f;
                }
            }

            else if (currentTime >= 1500 && currentTime <= 2100)
            {
                ambientLight.intensity -= 1 / 600f;
                if (ambientLight.intensity < 0.05f)
                {
                    ambientLight.intensity = 0.05f;
                }
            }
        }
    }
}
