using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animator anim;
    private GameObject PlayerCamera;
    public SpriteRenderer sr;
    public CircleCollider2D interactRange;
    public GameObject UIMenu;
    public ChatManager chat;
    public GameObject hoverPanel;
    public InfoPanelScript infoPanel;
    // image used for when player drags an item like food bag
    public Image interactImage;

    public float MoveSpeed;
    public int money, direction;
    public bool butcher, feeding;

    public Text PlayerNameText;
    public Text PlayerMoneyText;

    public GameObject chicken, feed;

    private Vector2 moveDirection;

    private List<Collider2D> colliders = new List<Collider2D>();
    private string hoveringObject;
    // object used for temporary structure placement
    private GameObject PlaceChicken, PlaceFeed;

    public Hotbar hotbar;
    public UIManager uiManager;

    private Shake shake;

    [HideInInspector]
    public GameObject gameCursor;

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        // checks if the current client is this device
        if (photonView.isMine)
        {
            shake = GameObject.Find("Screen Shake").GetComponent<Shake>();
            PlayerCamera = GameObject.Find("Camera Holder");
            PlayerCamera.GetComponent<CameraFollow>().target = gameObject;
            uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
            uiManager.player = gameObject.GetComponent<Player>();
            hotbar = GameObject.Find("Hotbar").gameObject.GetComponent<Hotbar>();
            hotbar.player = gameObject.GetComponent<Player>();
            gameCursor = GameObject.Find("Game Cursor");

            PlayerNameText.text = PhotonNetwork.playerName;
            PlayerNameText.color = Color.yellow;
            hotbar.visible = true;
            direction = 1;
            money = 1001;
            UIMenu.SetActive(true);

            PlaceChicken = GameObject.Find("PlaceChicken").gameObject;
            PlaceChicken.SetActive(false);
            PlaceFeed = GameObject.Find("PlaceFeed").gameObject;
            PlaceFeed.SetActive(false);
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
        if (photonView.isMine && !chat.GetChatInput().enabled)
        {
            CheckInput();

            if(infoPanel.currentObject != null
                && (hotbar.slots[hotbar.selected].item != null && !hotbar.slots[hotbar.selected].item.canObserve || InterfaceOpen()))
            {
                infoPanel.currentObject = null;
            }

            StructurePlacement();
        }

    }

    private void StructurePlacement()
    {
        if (hotbar.drag || hotbar.slots[hotbar.selected].item == null || InterfaceOpen())
        {
            PlaceChicken.SetActive(false);
            PlaceFeed.SetActive(false);
        }
        else if (hotbar.slots[hotbar.selected].item.itemName == "Caged Chicken")
        {
            PlaceChicken.SetActive(true);
            PlaceChicken.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            PlaceFeed.SetActive(false);

            if (PlaceChicken.GetComponent<ObjectPlacement>().colliders.Count == 0 && Vector2.Distance(transform.position, PlaceChicken.transform.position) <= 2f)
            {
                Color temp = PlaceChicken.GetComponent<SpriteRenderer>().color;
                temp.r = 1f; temp.g = 1f; temp.b = 1f;
                PlaceChicken.GetComponent<SpriteRenderer>().color = temp;

                if (Input.GetMouseButtonDown(0))
                {
                    for (int i = 0; i < hotbar.slots.Length; i++)
                    {
                        if (hotbar.slots[i].GetComponent<ItemSlot>().MouseHover())
                        {
                            return;
                        }
                    }
                    hotbar.slots[hotbar.selected].item = null;
                    photonView.RPC("PlayWorldAudio", PhotonTargets.All, "free chicken");
                    photonView.RPC("SpawnCagedChicken", PhotonTargets.MasterClient, PlaceChicken.transform.position.x, PlaceChicken.transform.position.y);
                }
            }
            else
            {
                Color temp = PlaceChicken.GetComponent<SpriteRenderer>().color;
                temp.r = 1f; temp.g = 0.5f; temp.b = 0.5f;
                PlaceChicken.GetComponent<SpriteRenderer>().color = temp;
            }
        }
        else if (hotbar.slots[hotbar.selected].item.itemName == "Chicken Feed" && !feeding)
        {
            PlaceFeed.SetActive(true);
            PlaceFeed.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

            PlaceChicken.SetActive(false);

            if (PlaceFeed.GetComponent<ObjectPlacement>().colliders.Count == 0 && Vector2.Distance(transform.position, PlaceFeed.transform.position) <= 2f)
            {
                Color temp = PlaceFeed.GetComponent<SpriteRenderer>().color;
                temp.r = 1f; temp.g = 1f; temp.b = 1f;
                PlaceFeed.GetComponent<SpriteRenderer>().color = temp;

                if (Input.GetMouseButtonDown(0) && !feeding)
                {
                    feeding = true;
                    anim.SetBool("feeding", true);
                    anim.SetBool("isMoving", false);

                    for (int i = 0; i < 5; i++)
                    {
                        if (hotbar.slots[i].MouseHover())
                        {
                            return;
                        }
                    }

                    hotbar.slots[hotbar.selected].item.currentStack -= 1;
                    if (hotbar.slots[hotbar.selected].item.currentStack <= 0)
                    {
                        hotbar.slots[hotbar.selected].item = null;
                    }
                }
            }
            else
            {
                Color temp = PlaceFeed.GetComponent<SpriteRenderer>().color;
                temp.r = 0.5f; temp.g = 0f; temp.b = 0f;
                PlaceFeed.GetComponent<SpriteRenderer>().color = temp;
            }
        }
        else
        {
            PlaceChicken.SetActive(false);
            PlaceFeed.SetActive(false);
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
        else if(feeding)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
            {
                feeding = false;
                photonView.RPC("SpawnFeed", PhotonTargets.MasterClient, PlaceFeed.transform.position.x, PlaceFeed.transform.position.y);
                anim.SetBool("feeding", false);
            }
        }
        else if (!InterfaceOpen())
        {
            if(!hotbar.visible)
            {
                hotbar.visible = true;
            }

            CheckHover();

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

            if(Input.GetMouseButtonDown(0))
            {
                if(!butcher && CanButcher() && !feeding)
                {
                    shake.SwingShake(direction);
                    int randSwing = Random.Range(0, 3);
                    if (randSwing == 0)
                    {
                        photonView.RPC("PlayWorldAudio", PhotonTargets.All, "swing1");
                    } else if(randSwing == 1)
                    {
                        photonView.RPC("PlayWorldAudio", PhotonTargets.All, "swing2");
                    } else
                    {
                        photonView.RPC("PlayWorldAudio", PhotonTargets.All, "swing3");
                    }
                    butcher = true;
                    anim.SetBool("butcher", true);
                    anim.SetBool("isMoving", false);
                }
            }
        }
        else
        {
            anim.SetBool("isMoving", false);

            if(uiManager.speechVisible && hotbar.visible)
            {
                hotbar.visible = false;
            }
            else if (uiManager.marketVisible || uiManager.auctionVisible)
            {
                hotbar.visible = true;
            }
        }
    }

    private void CheckHover()
    {
        if(IsHoveringObject())
        {
            if(!hoverPanel.activeSelf)
            {
                hoverPanel.SetActive(true);
            }

            MouseHoverPanel panel = hoverPanel.GetComponent<MouseHoverPanel>();
            if (panel.hoverInfo.text != hoveringObject)
            {
                if (hoveringObject == "Door")
                {
                    panel.hoverInfo.text = "Door";
                    panel.leftClick.text = "Observe";
                    panel.rightClick.text = "Open/Close";
                }
                else if (hoveringObject == "Switch")
                {
                    panel.hoverInfo.text = "Light Switch";
                    panel.leftClick.text = "Observe";
                    panel.rightClick.text = "Open/Close";
                }
                else if (hoveringObject == "Oven")
                {
                    panel.hoverInfo.text = "Oven";
                    panel.leftClick.text = "Observe";
                    panel.rightClick.text = "Open";
                }
                else if (hoveringObject == "Egg" || hoveringObject == "Axe" || hoveringObject == "Caged Chicken" || hoveringObject == "Feed Bag")
                {
                    panel.hoverInfo.text = hoveringObject;
                    panel.leftClick.text = "Observe";
                    panel.rightClick.text = "Pick Up";
                }
                else if (hoveringObject == "Raw Chicken")
                {
                    panel.hoverInfo.text = "Chicken";
                    panel.leftClick.text = "Observe";
                    panel.rightClick.text = "Pick Up";
                }
                else if (hoveringObject == "Vendor")
                {
                    panel.hoverInfo.text = "Vendor";
                    panel.leftClick.text = "Observe";
                    panel.rightClick.text = "Buy/Sell";
                }
                else if (hoveringObject == "Chicken")
                {
                    panel.hoverInfo.text = "Chicken";
                    panel.leftClick.text = "Butcher (with axe) or observe";
                    panel.rightClick.text = "";
                }
            }

            if(hoveringObject == "Chicken" && hotbar.slots[hotbar.selected].item != null && hotbar.slots[hotbar.selected].item.itemName == "Axe")
            {
                gameCursor.GetComponent<CursorScript>().icon.sprite = gameCursor.GetComponent<CursorScript>().butcher;
            }
            else if(hoveringObject == "Egg" || hoveringObject == "Axe" || hoveringObject == "Caged Chicken" || hoveringObject == "Raw Chicken")
            {
                gameCursor.GetComponent<CursorScript>().icon.sprite = gameCursor.GetComponent<CursorScript>().pickup;
            }
            else
            {
                gameCursor.GetComponent<CursorScript>().icon.sprite = gameCursor.GetComponent<CursorScript>().hover;
            }
        }
        else
        {
            gameCursor.GetComponent<CursorScript>().icon.sprite = gameCursor.GetComponent<CursorScript>().normal;

            if (hoverPanel.GetComponent<MouseHoverPanel>().enabled)
            {
                hoverPanel.SetActive(false);
            }
        }
    }

    private bool CanButcher()
    {
        return hotbar.slots[hotbar.selected].item != null && hotbar.slots[hotbar.selected].item.itemName == "Axe" &&
            !hotbar.drag;
    }

    private bool CanFeed()
    {
        return hotbar.slots[hotbar.selected].item != null && hotbar.slots[hotbar.selected].item.itemName == "Chicken Feed" &&
            !hotbar.drag;
    }

    // method that calcualtes and moves the player
    private void Move()
    {
        if (!InterfaceOpen() && !butcher && !feeding && !chat.GetChatInput().enabled)
        {
            rb.velocity = new Vector2(moveDirection.x * MoveSpeed, moveDirection.y * MoveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public bool InterfaceOpen()
    {
        if(photonView.isMine &&
            (uiManager.marketVisible || uiManager.auctionVisible || uiManager.speechVisible || uiManager.ovenVisible))
        {
            return true;
        }

        return false;
    }

    private void UpdateColliders()
    {
        if (InterfaceOpen())
        {
            return;
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].gameObject.CompareTag("Egg"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(1) && colliders[i].gameObject.GetComponent<EggScript>().selected)) &&
                    hotbar.CanAdd(hotbar.eggItem) && !colliders[i].gameObject.GetComponent<EggScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<EggScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<EggScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.eggItem));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Raw Chicken"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(1) && colliders[i].gameObject.GetComponent<RawChickenScript>().selected)) &&
                    hotbar.CanAdd(hotbar.rawChicken) && !colliders[i].gameObject.GetComponent<RawChickenScript>().isPickedUp)
                {
                    hotbar.AddChicken(colliders[i].gameObject.GetComponent<RawChickenScript>().cookedMagnitude);
                    colliders[i].gameObject.GetComponent<RawChickenScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<RawChickenScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                }
            }

            else if (colliders[i].gameObject.CompareTag("Caged Chicken"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(1) && colliders[i].gameObject.GetComponent<CagedChickenScript>().selected)) &&
                    hotbar.CanAdd(hotbar.rawChicken) && !colliders[i].gameObject.GetComponent<CagedChickenScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<CagedChickenScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<CagedChickenScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.cagedChicken));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Axe"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(1) && colliders[i].gameObject.GetComponent<AxeScript>().selected)) &&
                    hotbar.CanAdd(hotbar.rawChicken) && !colliders[i].gameObject.GetComponent<AxeScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<AxeScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<AxeScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.axe));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Feed Bag"))
            {
                if ((Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(1) && colliders[i].gameObject.GetComponent<FeedItemScript>().selected)) &&
                    hotbar.CanAdd(hotbar.feedBag) && !colliders[i].gameObject.GetComponent<FeedItemScript>().isPickedUp)
                {
                    colliders[i].gameObject.GetComponent<FeedItemScript>().isPickedUp = true;
                    colliders[i].gameObject.GetComponent<FeedItemScript>().photonView.RPC("PickUp", PhotonTargets.MasterClient);
                    hotbar.AddItem(Instantiate(hotbar.feedBag));
                }
            }

            else if (colliders[i].gameObject.CompareTag("Chicken") && colliders[i].gameObject.GetComponent<Chicken>().IsSelected() &&
                CanButcher())
            {
                if (Input.GetMouseButtonDown(0) && !butcher)
                {
                    colliders[i].gameObject.GetComponent<Chicken>().photonView.RPC("PreButcher", PhotonTargets.MasterClient);
                }
            }

            else if (colliders[i].gameObject.CompareTag("Vendor") && colliders[i].gameObject.GetComponent<Vendor>().IsSelected())
            {
                if (Input.GetMouseButtonDown(1))
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

                    uiManager.TalkToVendor();
                }
            }
            else if (colliders[i].gameObject.CompareTag("Oven") && colliders[i].gameObject.GetComponent<Oven>().IsSelected())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    uiManager.OvenMenu.GetComponent<OvenManager>().CurrentOven = colliders[i].gameObject.GetComponent<Oven>();
                    uiManager.ovenVisible = true;
                    FindObjectOfType<AudioManager>().Play("oven");
                }
            }
            else if (colliders[i].gameObject.CompareTag("Door") && colliders[i].gameObject.GetComponent<Door>().IsSelected())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    colliders[i].gameObject.GetComponent<Door>().photonView.RPC("UpdateState", PhotonTargets.AllBufferedViaServer);
                    if (colliders[i].gameObject.GetComponent<Door>().isOpen)
                        photonView.RPC("PlayWorldAudio", PhotonTargets.All, "door close");
                    else
                        photonView.RPC("PlayWorldAudio", PhotonTargets.All, "door open");
                }
            }
            else if (colliders[i].gameObject.CompareTag("Switch") && colliders[i].gameObject.GetComponent<LightSwitch>().IsSelected())
            {
                if (Input.GetMouseButtonDown(1))
                {
                    colliders[i].gameObject.GetComponent<LightSwitch>().photonView.RPC("UpdateLight", PhotonTargets.AllBufferedViaServer);
                }
            }
        }
    }

    protected bool IsHoveringObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity))
        {
            if (hit)
            {
                if (hit.collider.gameObject.GetComponent<SceneObject>() != null && hit.collider.gameObject.GetComponent<SceneObject>().IsSelected())
                {
                    hoveringObject = hit.collider.gameObject.tag;

                    if(Input.GetMouseButtonDown(0))
                    {
                        infoPanel.currentObject = hit.collider.gameObject;
                    }

                    return true;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            infoPanel.currentObject = null;
        }

        hoveringObject = "";
        return false;
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
    private void PlayWorldAudio(string name)
    {
        FindObjectOfType<AudioManager>().Play(name);
    }

    [PunRPC]
    private void SpawnChicken(float x, float y)
    {
        object[] obj = { 0 };
        PhotonNetwork.InstantiateSceneObject(chicken.name, new Vector2(x, y), Quaternion.identity, 0, obj);
    }

    [PunRPC]
    private void SpawnCagedChicken(float x, float y)
    {
        object[] obj = { 1 };
        PhotonNetwork.InstantiateSceneObject(chicken.name, new Vector2(x, y), Quaternion.identity, 0, obj);
    }

    [PunRPC]
    private void SpawnFeed(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(feed.name, new Vector2(x, y), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropEgg(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(hotbar.eggItem.GetComponent<Item>().WorldItem.name, new Vector2(x + Random.Range(0, 0.2f) - 0.1f, y - 0.1f), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropMeat(float x, float y, float cookedMagnitude)
    {
        object[] magnitude = { cookedMagnitude };
        PhotonNetwork.InstantiateSceneObject(hotbar.rawChicken.GetComponent<Item>().WorldItem.name, new Vector2(x, y - 0.1f), Quaternion.identity, 0, magnitude);
    }

    [PunRPC]
    private void DropCagedChicken(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(hotbar.cagedChicken.GetComponent<Item>().WorldItem.name, new Vector2(x, y - 0.1f), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropAxe(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(hotbar.axe.GetComponent<Item>().WorldItem.name, new Vector2(x, y - 0.1f), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropFeedBag(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(hotbar.feedBag.GetComponent<Item>().WorldItem.name, new Vector2(x + Random.Range(0, 0.4f) - 0.2f, y - 0.1f), Quaternion.identity, 0, null);
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