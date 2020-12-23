using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject PlayerCamera;
    public SpriteRenderer sr;
    public CircleCollider2D interactRange;

    public float MoveSpeed;
    public int money, direction = 1;
    public bool butcher = false;

    public PlayerMarket market;
    public PlayerHotbar hotbar;
    
    public Text PlayerNameText;
    public Text PlayerMoneyText;

    public GameObject chicken;

    private Vector2 moveDirection;

    private bool pressed;
    private float pressTimer;

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        // checks if the current client is this device
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.playerName;
            PlayerNameText.color = Color.yellow;
            hotbar.visible = true;
        }
        else
        {
            PlayerNameText.text = photonView.owner.name;
            PlayerNameText.color = Color.white;
        }
    }

    // update is default function that is called every frame
    private void Update()
    {
        // if the current client is this device, then checks for user input
        if (photonView.isMine)
        {
            CheckInput();
        }

    }
    
    // called a set amount of times per tick, this is where all the physics happens for consistency
    private void FixedUpdate()
    {
        Move();
    }

    // method for user input
    private void CheckInput()
    {
        if (butcher)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
            {
                butcher = false;
                anim.SetBool("butcher", false);
            }
        }
        else if (!market.MarketMenu.activeSelf)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector2(moveX, moveY).normalized;

            // keyboard controls
            if (direction != 0 && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                direction = 0;
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }

            else if (direction != 1 && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                direction = 1;
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }

            // animation updates
            if (Mathf.Abs(rb.velocity.x) > 0.25 || Mathf.Abs(rb.velocity.y) > 0.25)
            {
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }

            // spawns a chicken for testing
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                photonView.RPC("SpawnChicken", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
            }

            if(Input.GetMouseButton(0))
            {
                interactRange.radius = 0.2f;
                pressed = true;
            }

            if(pressed)
            {
                pressTimer += Time.deltaTime;
                if(pressTimer > 0.2f)
                {
                    pressTimer = 0;
                    pressed = false;
                    interactRange.radius = 0f;
                }
            }

            if (!butcher && Input.GetMouseButtonDown(0)
                && hotbar.hotbar[hotbar.selected] != null && hotbar.hotbar[hotbar.selected].GetComponent<Item>().itemName == "Axe")
            {
                butcher = true;
                anim.SetBool("butcher", true);
                anim.SetBool("isMoving", false);
            }
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    // method that calcualtes and moves the player
    private void Move()
    {
        if (!market.MarketMenu.activeSelf && !butcher)
        {
            rb.velocity = new Vector2(moveDirection.x * MoveSpeed, moveDirection.y * MoveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Egg"))
        {
            if(Input.GetKeyDown(KeyCode.Space) || 
                (pressed && collision.gameObject.GetComponent<EggScript>().selected &&
                !collision.gameObject.GetComponent<EggScript>().isPickedUp)) {
                if(!hotbar.IsFull())
                {
                    collision.gameObject.GetComponent<EggScript>().isPickedUp = true;
                    hotbar.AddItem(Instantiate(hotbar.eggItem));
                    collision.gameObject.GetComponent<EggScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                }
            }
        }

        if (collision.gameObject.CompareTag("Chicken") && collision.gameObject.GetComponent<Chicken>().selected)
        {
            if(pressed)
            {
                collision.gameObject.GetComponent<Chicken>().butcherProcess = true;
            }
        }
    }

    // photon methods that are used to sync on different devices
    [PunRPC]
    private void SpawnChicken(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(chicken.name, new Vector2(x, y), Quaternion.identity, 0, null);
    }

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