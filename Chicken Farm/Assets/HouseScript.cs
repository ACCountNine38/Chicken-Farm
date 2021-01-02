using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : MonoBehaviour
{
    public GameObject roof, frontWall, backWall, midWall, sideWalls;
    public BoxCollider2D kitchen, livingRoom, backCoverage;
    public GameObject frontDoor, backDoor;

    private bool entered, inKitchen, inLivingRoom, inBackCoverage;

    private void Update()
    {
        if (entered)
        {
            if (roof.GetComponent<SpriteRenderer>().color.a > 0f)
            {
                Color temp = roof.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a - 500f / 255f * Time.deltaTime;
                if(tempAlpha < 0f)
                {
                    tempAlpha = 0f;
                }
                temp.a = tempAlpha;
                roof.GetComponent<SpriteRenderer>().color = temp;
                temp.a = tempAlpha * 2;
                sideWalls.GetComponent<SpriteRenderer>().color = temp;
            }
        }
        else
        {
            if (roof.GetComponent<SpriteRenderer>().color.a < 1)
            {
                Color temp = roof.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a + 500f / 255f * Time.deltaTime;
                if (tempAlpha > 1)
                {
                    tempAlpha = 1;
                }
                temp.a = tempAlpha;
                roof.GetComponent<SpriteRenderer>().color = temp;
                temp.a = tempAlpha * 2;
                sideWalls.GetComponent<SpriteRenderer>().color = temp;
            }
        }

        if (inBackCoverage)
        {
            if (backWall.GetComponent<SpriteRenderer>().color.a > 50f / 255f)
            {
                Color temp = backWall.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a - 500f / 255f * Time.deltaTime;
                if (tempAlpha < 50f / 255f)
                {
                    tempAlpha = 50f / 255f;
                }
                temp.a = tempAlpha;
                backWall.GetComponent<SpriteRenderer>().color = temp;
                temp.a = tempAlpha*2;
                //backDoor.GetComponent<SpriteRenderer>().color = temp;
            }
        }
        else
        {
            if (backWall.GetComponent<SpriteRenderer>().color.a < 1)
            {
                Color temp = backWall.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a + 500f / 255f * Time.deltaTime;
                if (tempAlpha > 1)
                {
                    tempAlpha = 1;
                }
                temp.a = tempAlpha;
                backWall.GetComponent<SpriteRenderer>().color = temp;
                temp.a = tempAlpha * 2;
                //backDoor.GetComponent<SpriteRenderer>().color = temp;
            }
        }

        if (inKitchen)
        {
            if (midWall.GetComponent<SpriteRenderer>().color.a > 50f / 255f)
            {
                Color temp = midWall.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a - 500f / 255f * Time.deltaTime;
                if (tempAlpha < 50f / 255f)
                {
                    tempAlpha = 50f / 255f;
                }
                temp.a = tempAlpha;
                midWall.GetComponent<SpriteRenderer>().color = temp;
            }
        }
        else
        {
            if (midWall.GetComponent<SpriteRenderer>().color.a < 1)
            {
                Color temp = midWall.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a + 500f / 255f * Time.deltaTime;
                if (tempAlpha > 1)
                {
                    tempAlpha = 1;
                }
                temp.a = tempAlpha;
                midWall.GetComponent<SpriteRenderer>().color = temp;
            }
        }

        if (inLivingRoom)
        {
            if (frontWall.GetComponent<SpriteRenderer>().color.a > 50f / 255f)
            {
                Color temp = frontWall.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a - 500f / 255f * Time.deltaTime;
                if (tempAlpha < 50f / 255f)
                {
                    tempAlpha = 50f / 255f;
                }
                temp.a = tempAlpha;
                frontWall.GetComponent<SpriteRenderer>().color = temp;
                //frontDoor.GetComponent<SpriteRenderer>().color = temp;
            }
        }
        else
        {
            if (frontWall.GetComponent<SpriteRenderer>().color.a < 1)
            {
                Color temp = frontWall.GetComponent<SpriteRenderer>().color;
                float tempAlpha = temp.a + 500f / 255f * Time.deltaTime;
                if (tempAlpha > 1)
                {
                    tempAlpha = 1;
                }
                temp.a = tempAlpha;
                frontWall.GetComponent<SpriteRenderer>().color = temp;
                //frontDoor.GetComponent<SpriteRenderer>().color = temp;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.isTrigger && collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Player>().photonView.isMine)
        {
            if (livingRoom.IsTouching(collision) || kitchen.IsTouching(collision) || backCoverage.IsTouching(collision))
            {
                entered = true;
            }
            
            if(kitchen.IsTouching(collision))
            {
                inKitchen = true;
            }
            else
            {
                inKitchen = false;
            }

            if (livingRoom.IsTouching(collision))
            {
                inLivingRoom = true;
            }
            else
            {
                inLivingRoom = false;
            }

            if (backCoverage.IsTouching(collision))
            {
                inBackCoverage = true;
            }
            else
            {
                inBackCoverage = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Player>().photonView.isMine)
        {
            if (!kitchen.IsTouching(collision))
            {
                inKitchen = false;
            }

            if (!livingRoom.IsTouching(collision))
            {
                inLivingRoom = false;
            }

            if (!backCoverage.IsTouching(collision))
            {
                inBackCoverage = false;
            }

        }

        if(!inKitchen && !inLivingRoom && !inBackCoverage)
        {
            entered = false;
        }
    }
}
