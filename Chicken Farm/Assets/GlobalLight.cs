using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    public PhotonView photonView;
    public Light2D ambientLight;

    public float currentTime, updateTimer;

    private float updateCooldown = 0.1f;

    public void Awake()
    {
        currentTime = 60;
        ambientLight.intensity = 1f;
    }

    void Update()
    {
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
            ambientLight.intensity = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ambientLight.intensity = 1;
        }

        updateTimer += Time.deltaTime;
        if (updateTimer >= updateCooldown)
        {
            if(currentTime == 3000)
            {
                currentTime = 0;
            }
            else
            {
                currentTime += 1;
            }

            if (currentTime <= 600)
            {
                ambientLight.intensity += 1/600f;
                if (ambientLight.intensity > 1f)
                {
                    ambientLight.intensity = 1f;
                }
            }

            else if (currentTime >= 1500 && currentTime <= 2100)
            {
                ambientLight.intensity -= 1/600f;
                if (ambientLight.intensity < 0f)
                {
                    ambientLight.intensity = 0f;
                }
            }

            updateTimer = 0;
        }
    }
}
