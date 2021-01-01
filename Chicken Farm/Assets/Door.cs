using UnityEngine;
using UnityEngine.UI;

public class Door : Structure
{
    public Sprite open, close;
    public BoxCollider2D collider;
    public bool isOpen, canChange;
    public float doorTimer;

    public void Update()
    {
        if (IsHovering())
        {
            selected = true;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b - 0.25f);
        }
        else
        {
            selected = false;
            sr.color = original;
        }

        if(!canChange)
        {
            doorTimer += Time.deltaTime;
            if(doorTimer >= 0.5f)
            {
                canChange = true;
            }
        }
    }

    [PunRPC]
    public void UpdateState()
    {
        if(canChange)
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                sr.sprite = open;
                collider.isTrigger = true;
            }
            else
            {
                sr.sprite = close;
                collider.isTrigger = false;
            }

            canChange = false;
        }
    }
}
