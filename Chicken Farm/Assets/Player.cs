using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject PlayerCamera;
    public SpriteRenderer sr;

    public float MoveSpeed;
    public int money;
    private int previousMoney;
    private Vector2 moveDirection;

    public GameObject MarketMenu;
    private bool market = false;

    public Text PlayerNameText;
    public Text PlayerMoneyText;

    // hotbar stuff
    private Item[] hotbar = new Item[5];
    private int selected;
    //private GameObject slot1, slot2, slot3, slot4, slot5;

    // spawnable objects for testing
    public GameObject chicken;

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        // checks if the current client is this device
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.playerName;
            PlayerNameText.color = Color.yellow;
        }
        else
        {
            PlayerNameText.text = photonView.owner.name;
            PlayerNameText.color = Color.white;
        }

        previousMoney = 0;
    }

    // update is default function that is called every frame
    private void Update()
    {
        // if the current client is this device, then checks for user input
        if (photonView.isMine)
        {
            CheckInput();
            UpdateMarket();
            //UpdateHotbar();
        }

    }

    private void UpdateMarket()
    {
        if(money != previousMoney)
        {
            if(previousMoney < money)
            {
                if(previousMoney < money - 15)
                {
                    previousMoney += 10;
                }
                else
                {
                    previousMoney += 1;
                }
            }
            else if (previousMoney > money)
            {
                if (previousMoney > money + 15)
                {
                    previousMoney -= 10;
                }
                else
                {
                    previousMoney -= 1;
                }
            }

            PlayerMoneyText.text = "Bank: $" + previousMoney;
        }

        if (market && Input.GetKeyDown(KeyCode.P))
        {
            market = false;
        }
        else if (!market && Input.GetKeyDown(KeyCode.P))
        {
            market = true;
        }

        if (market)
        {
            if (MarketMenu.transform.localPosition.y > 0)
            {
                MarketMenu.transform.localPosition = new Vector3(MarketMenu.transform.localPosition.x, MarketMenu.transform.localPosition.y - 20);
            }
            if (!MarketMenu.activeSelf)
            {
                MarketMenu.SetActive(true);
            }
        }
        else
        {
            if (MarketMenu.transform.localPosition.y < 460)
            {
                MarketMenu.transform.localPosition = new Vector3(MarketMenu.transform.localPosition.x, MarketMenu.transform.localPosition.y + 20);
            }
            if (MarketMenu.transform.localPosition.y >= 460 && MarketMenu.activeSelf)
            {
                MarketMenu.SetActive(false);
            }
        }
    }

    //private void UpdateHotbar()
    //{
    //    if(hotbar[0] == null)
    //    {
    //        slot1.SetActive(false);
    //    }
    //}

    // called a set amount of times per tick, this is where all the physics happens for consistency
    private void FixedUpdate()
    {
        Move();
    }

    // method for user input
    private void CheckInput()
    {
        if (!MarketMenu.activeSelf)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector2(moveX, moveY).normalized;

            // keyboard controls
            if (rb.velocity.x < 0)
            {
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }

            else if (rb.velocity.x > 0)
            {
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }

            // animation updates
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // spawns a chicken for testing
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PhotonNetwork.Instantiate(chicken.name, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity, 0);
        }
    }

    // method that calcualtes and moves the player
    private void Move()
    {
        if (!MarketMenu.activeSelf)
        {
            rb.velocity = new Vector2(moveDirection.x * MoveSpeed, moveDirection.y * MoveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // photon methods that are used to sync on different devices
    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
    }
}