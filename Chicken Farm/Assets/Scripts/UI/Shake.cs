using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public Animator anim;

    public void SwingShake(int direction)
    {
        if(direction == 1)
        {
            anim.SetTrigger("swing");
        }
        else
        {
            anim.SetTrigger("swing2");
        }
    }
}
