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
    public int money, direction;
    public bool butcher = false;

    public PlayerMarket market;
    public PlayerHotbar hotbar;
    
    public Text PlayerNameText;
    public Text PlayerMoneyText;

    public GameObject chicken;

    private Vector2 moveDirection;

    private bool pressed, space;
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
            direction = 1;
            money = 1001;
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
        else if (!market.visible)
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                interactRange.radius = 0.2f;
                space = true;
            }

            if (pressed || space)
            {
                pressTimer += Time.deltaTime;
                if(pressTimer >= 0.1f)
                {
                    pressTimer = 0;
                    pressed = false;
                    space = false;
                    interactRange.radius = 0f;
                }
            }

            if (!butcher && Input.GetMouseButtonDown(0) && CanButcher())
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

    private bool CanButcher()
    {
        return hotbar.hotbar[hotbar.selected] != null && hotbar.hotbar[hotbar.selected].GetComponent<Item>().itemName == "Axe" &&
            !hotbar.drag;
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
        if(market.visible && !photonView.isMine)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Egg"))
        {
            if((space || (pressed && collision.gameObject.GetComponent<EggScript>().selected)) &&
                hotbar.CanAdd(hotbar.eggItem) && !collision.gameObject.GetComponent<EggScript>().isPickedUp)
            {
                collision.gameObject.GetComponent<EggScript>().isPickedUp = true;
                collision.gameObject.GetComponent<EggScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                hotbar.AddItem(Instantiate(hotbar.eggItem));
            }
        }

        else if (collision.gameObject.CompareTag("Raw Chicken"))
        {
            if ((space || (pressed && collision.gameObject.GetComponent<RawChickenScript>().selected)) &&
                hotbar.CanAdd(hotbar.rawChicken) && !collision.gameObject.GetComponent<RawChickenScript>().isPickedUp)
            {
                collision.gameObject.GetComponent<RawChickenScript>().isPickedUp = true;
                collision.gameObject.GetComponent<RawChickenScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                hotbar.AddItem(Instantiate(hotbar.rawChicken));
            }
        }

        else if (collision.gameObject.CompareTag("Caged Chicken"))
        {
            if ((space || (pressed && collision.gameObject.GetComponent<CagedChickenScript>().selected)) &&
                hotbar.CanAdd(hotbar.rawChicken) && !collision.gameObject.GetComponent<CagedChickenScript>().isPickedUp)
            {
                collision.gameObject.GetComponent<CagedChickenScript>().isPickedUp = true;
                collision.gameObject.GetComponent<CagedChickenScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                hotbar.AddItem(Instantiate(hotbar.cagedChicken));
            }
        }

        else if (collision.gameObject.CompareTag("Axe"))
        {
            if ((space || (pressed && collision.gameObject.GetComponent<AxeScript>().selected)) &&
                hotbar.CanAdd(hotbar.rawChicken) && !collision.gameObject.GetComponent<AxeScript>().isPickedUp)
            {
                collision.gameObject.GetComponent<AxeScript>().isPickedUp = true;
                collision.gameObject.GetComponent<AxeScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                hotbar.AddItem(Instantiate(hotbar.axe));
            }
        }

        else if (collision.gameObject.CompareTag("Chicken") && collision.gameObject.GetComponent<Chicken>().IsSelected() &&
            CanButcher())
        {
            if(pressed)
            {
                collision.gameObject.GetComponent<Chicken>().photonView.RPC("PreButcher", PhotonTargets.MasterClient);
            }
        }

        else if (collision.gameObject.CompareTag("Vendor") && collision.gameObject.GetComponent<Vendor>().IsSelected())
        {
            if (pressed)
            {
                if(transform.position.x < collision.gameObject.transform.position.x && collision.GetComponent<Vendor>().direction == 1)
                {
                    collision.GetComponent<Vendor>().direction = 0;
                    collision.GetComponent<Vendor>().photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
                }
                else if (transform.position.x > collision.gameObject.transform.position.x && collision.GetComponent<Vendor>().direction == 0)
                {
                    collision.GetComponent<Vendor>().direction = 1;
                    collision.GetComponent<Vendor>().photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
                }

                market.visible = true;
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