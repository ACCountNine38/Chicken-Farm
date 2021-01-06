using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public AudioSource source;
    public AudioClip hover, press;

    public void Hover()
    {
        source.PlayOneShot(hover);
    }

    public void Press()
    {
        source.PlayOneShot(press);
    }
}
