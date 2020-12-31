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
    public CircleCollider2D interactRange;
    public GameObject UIMenu;

    public float MoveSpeed;
    public int money, direction;
    public bool butcher = false;

    public PlayerMarket market;
    public PlayerHotbar hotbar;
    public PlayerOven oven;

    public Text PlayerNameText;
    public Text PlayerMoneyText;

    public GameObject chicken;

    private Vector2 moveDirection;

    private List<Collider2D> colliders = new List<Collider2D>();
    private bool pressed;

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
            UIMenu.SetActive(true);
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
        else if (!market.visible && !oven.visible)
        {
            if(Input.GetMouseButtonDown(0))
            {
                pressed = true;
            }
            else
            {
                pressed = false;
            }

            UpdateColliders();

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
        if (!market.MarketMenu.activeSelf && !oven.OvenMenu.activeSelf && !butcher)
        {
            rb.velocity = new Vector2(moveDirection.x * MoveSpeed, moveDirection.y * MoveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateColliders()
    {
        if (market.visible || oven.visible)
        {
            return;
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].gameObject.CompareTag("Egg"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (pressed && colliders[i].gameObject.GetComponent<EggScript>().selected)) &&
                    hotbar.CanAdd(hotbar.eggItem) && !colliders[i].gameObject.GetComponent<EggScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<EggScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<EggScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.eggItem));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Raw Chicken"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (pressed && colliders[i].gameObject.GetComponent<RawChickenScript>().selected)) &&
                    hotbar.CanAdd(hotbar.rawChicken) && !colliders[i].gameObject.GetComponent<RawChickenScript>().isPickedUp)
                {
                    hotbar.AddChicken(colliders[i].gameObject.GetComponent<RawChickenScript>().cookedMagnitude);
                    colliders[i].gameObject.GetComponent<RawChickenScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<RawChickenScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                }
            }

            else if (colliders[i].gameObject.CompareTag("Caged Chicken"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (pressed && colliders[i].gameObject.GetComponent<CagedChickenScript>().selected)) &&
                    hotbar.CanAdd(hotbar.rawChicken) && !colliders[i].gameObject.GetComponent<CagedChickenScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<CagedChickenScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<CagedChickenScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.cagedChicken));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Axe"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (pressed && colliders[i].gameObject.GetComponent<AxeScript>().selected)) &&
                    hotbar.CanAdd(hotbar.rawChicken) && !colliders[i].gameObject.GetComponent<AxeScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<AxeScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<AxeScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.axe));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Chicken") && colliders[i].gameObject.GetComponent<Chicken>().IsSelected() &&
                CanButcher())
            {
                if (pressed)
                {
                    colliders[i].gameObject.GetComponent<Chicken>().photonView.RPC("PreButcher", PhotonTargets.MasterClient);
                }
            }

            else if (colliders[i].gameObject.CompareTag("Vendor") && colliders[i].gameObject.GetComponent<Vendor>().IsSelected())
            {
                if (pressed)
                {
                    if (transform.position.x < colliders[i].gameObject.transform.position.x && colliders[i].GetComponent<Vendor>().direction == 1)
                    {
                        colliders[i].GetComponent<Vendor>().direction = 0;
                        colliders[i].GetComponent<Vendor>().photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
                    }
                    else if (transform.position.x > colliders[i].gameObject.transform.position.x && colliders[i].GetComponent<Vendor>().direction == 0)
                    {
                        colliders[i].GetComponent<Vendor>().direction = 1;
                        colliders[i].GetComponent<Vendor>().photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
                    }

                    market.visible = true;
                }
            }
            else if (colliders[i].gameObject.CompareTag("Oven") && colliders[i].gameObject.GetComponent<Oven>().IsSelected())
            {
                if (pressed)
                {
                    oven.CurrentOven = colliders[i].gameObject;
                    oven.visible = true;
                }
            }
            else if (colliders[i].gameObject.CompareTag("Door") && colliders[i].gameObject.GetComponent<Door>().IsSelected())
            {
                if (pressed)
                {
                    colliders[i].gameObject.GetComponent<Door>().photonView.RPC("UpdateState", PhotonTargets.AllViaServer);
                }
            }
            else if (colliders[i].gameObject.CompareTag("Switch") && colliders[i].gameObject.GetComponent<LightSwitch>().IsSelected())
            {
                if (pressed)
                {
                    colliders[i].gameObject.GetComponent<LightSwitch>().photonView.RPC("UpdateLight", PhotonTargets.AllViaServer);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SceneObject>() != null &&
            (!collision.isTrigger || collision.gameObject.CompareTag("Door")) && !colliders.Contains(collision))
        {
            colliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (colliders.Contains(collision))
        {
            colliders.Remove(collision);
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