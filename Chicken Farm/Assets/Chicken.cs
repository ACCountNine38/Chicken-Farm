using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chicken : Creature
{
    // base chicken status
    public float hunger;
    public int type;

    // variables for this chicken if it is named
    private bool isNamed;
    private string name;
    public Text nametag;

    public void Awake()
    {
        hunger = Random.Range(0, 100);
        UpdateType();
        Debug.Log(hunger);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateType();
    }

    private void UpdateType()
    {
        if(hunger <= 33)
        {
            type = 1;
            anim.SetInteger("type", 1);
            speed = 2;
            runSpeed = 4;
        }
        else if(hunger <= 66)
        {
            type = 0;
            anim.SetInteger("type", 0);
            speed = 3;
            runSpeed = 5;
        }
        else
        {
            type = 2;
            anim.SetInteger("type", 2);
            speed = 1;
            runSpeed = 3;
        }
    }

    // method that calcualtes and moves the player
    protected void Move()
    {
        if (status == "idle")
        {
            statusTimer += Time.deltaTime;
            if (statusTimer >= maxTimer)
            {
                RandomizeAction();
            }
        }
        else if(status == "move")
        {
            statusTimer += Time.deltaTime;

            if (rb.velocity.x < 0)
            {
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }
            else if (rb.velocity.x > 0)
            {
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }

            rb.velocity = new Vector2(randomDirection.x * speed, randomDirection.y * speed);
            if (statusTimer >= maxTimer)
            {
                RandomizeAction();
            }
        }

        // animation updates
        if(Mathf.Abs(rb.velocity.x) > 0.1 || Mathf.Abs(rb.velocity.y) > 0.1)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    protected void RandomizeAction()
    {
        maxTimer = Random.Range(1.0f, 5.0f);
        statusTimer = 0.0f;

        // randomizes what the chicken is going to do next
        int randomStatus = Random.Range(0, 2);

        if (randomStatus == 0)
        {
            status = "idle";
        }
        else if (randomStatus == 1)
        {
            randomDirection = Random.onUnitSphere;
            status = "move";
        }
    }
}
