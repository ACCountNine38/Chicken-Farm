using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerHotbar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player player;
    public GameObject Hotbar, boarder;
    public GameObject[] slots = new GameObject[5];
    public Image[] slotIcons = new Image[5];
    public Text[] amounts = new Text[5];
    public Image[] amountContainers = new Image[5];
    public GameObject[] hotbar = new GameObject[5];
    public Sprite empty, cooked, burnt;
    public PlayerOven oven;
    public GameObject ovenSlot;
    public bool visible, drag, ovenDrag;
    public int selected = 0;

    public Image[] iconLayer2 = new Image[5];
    public Image[] iconLayer3 = new Image[5];

    private float transformDestination = -31.5f, dropOffsetX, dropOffsetY;
    private int draggedIndex;
    private Vector2[] originalHotbarPositions = new Vector2[5];
    private bool dragColorUpdated;

    // items
    public GameObject eggItem, cagedChicken, axe, rawChicken, feedBag;
    // world items
    public GameObject egg, raw, cageChickenWorld, burriedAxe, chickenFeed;

    // Start is called before the first frame update
    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            originalHotbarPositions[i] = slotIcons[i].transform.localPosition;
        }

        AddItem(Instantiate(axe));
        AddItem(Instantiate(cagedChicken));
        AddItem(Instantiate(feedBag));
    }

    // Update is called once per frame
    private void Update()
    {
        if (player.photonView.isMine)
        {
            UpdatePosition();
            UpdateHotbar();
            CheckDrag();
        }
    }

    private void CheckDrag()
    {
        if (drag && !slots[draggedIndex].GetComponent<ItemSlot>().MouseHover())
        {
            slotIcons[draggedIndex].transform.position = Input.mousePosition;
            iconLayer2[draggedIndex].transform.position = Input.mousePosition;
            iconLayer3[draggedIndex].transform.position = Input.mousePosition;
            if(!dragColorUpdated)
            {
                slotIcons[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f * slotIcons[draggedIndex].GetComponent<Image>().color.a);
                iconLayer2[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f * iconLayer2[draggedIndex].GetComponent<Image>().color.a);
                iconLayer3[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f * iconLayer3[draggedIndex].GetComponent<Image>().color.a);
                dragColorUpdated = true;
            }
        }
        else if(ovenDrag)
        {
            oven.layer1.transform.position = Input.mousePosition;
            oven.layer2.transform.position = Input.mousePosition;
            oven.layer3.transform.position = Input.mousePosition;
        }
        else
        {
            slotIcons[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
            iconLayer2[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
            iconLayer3[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];

            if(oven.OvenMenu.activeSelf)
            {
                oven.layer1.transform.localPosition = oven.originalPosition;
                oven.layer2.transform.localPosition = oven.originalPosition;
                oven.layer3.transform.localPosition = oven.originalPosition;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!drag && !ovenDrag)
        {
            if(ovenSlot.GetComponent<ItemSlot>().MouseHover())
            {
                ovenDrag = true;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (hotbar[i] != null && slots[i].GetComponent<ItemSlot>().MouseHover())
                    {
                        drag = true;
                        draggedIndex = i;
                        break;
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(drag)
        {
            bool found = false;

            if(oven.OvenMenu.activeSelf)
            {
                if (ovenSlot.GetComponent<ItemSlot>().MouseHover() && hotbar[draggedIndex].GetComponent<Item>().itemName == "Raw Chicken")
                {
                    GameObject temp;
                    if (oven.CurrentOven.GetComponent<Oven>().stored != null)
                    {
                        temp = oven.CurrentOven.GetComponent<Oven>().stored;
                    }
                    else
                    {
                        temp = null;
                    }

                    oven.CurrentOven.GetComponent<PhotonView>().RPC("SwapChicken", PhotonTargets.AllBufferedViaServer, hotbar[draggedIndex].GetComponent<RawChicken>().cookedMagnitude);
                    hotbar[draggedIndex] = temp;
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (slots[i].GetComponent<ItemSlot>().MouseHover())
                    {
                        found = true;
                        GameObject temp = hotbar[i];
                        hotbar[i] = hotbar[draggedIndex];
                        hotbar[draggedIndex] = temp;
                        break;
                    }
                }

                if (!found)
                {
                    if (player.direction == 0)
                    {
                        dropOffsetX = -0.25f;
                    }
                    else
                    {
                        dropOffsetX = 0.25f;
                    }

                    if (hotbar[draggedIndex].GetComponent<Item>().stackable)
                    {
                        for (int i = 0; i < hotbar[draggedIndex].GetComponent<Item>().currentStack; i++)
                        {
                            dropOffsetY = Random.Range(0, 0.2f);
                            DropItem(hotbar[draggedIndex].GetComponent<Item>());
                        }
                    }
                    else
                    {
                        dropOffsetY = Random.Range(0, 0.2f);
                        DropItem(hotbar[draggedIndex].GetComponent<Item>());
                    }
                    hotbar[draggedIndex] = null;
                }
            }

            drag = false;
            dragColorUpdated = false;
            slotIcons[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
            iconLayer2[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
            iconLayer3[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
            slotIcons[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 2f * slotIcons[draggedIndex].GetComponent<Image>().color.a);
            iconLayer2[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 2f * iconLayer2[draggedIndex].GetComponent<Image>().color.a);
            iconLayer3[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 2f * iconLayer3[draggedIndex].GetComponent<Image>().color.a);
        }

        else if(ovenDrag)
        {
            for (int i = 0; i < 5; i++)
            {
                if (slots[i].GetComponent<ItemSlot>().MouseHover())
                {
                    if(hotbar[i] == null)
                    {
                        GameObject temp = oven.CurrentOven.GetComponent<Oven>().stored;
                        oven.CurrentOven.GetComponent<PhotonView>().RPC("RemoveChicken", PhotonTargets.AllBufferedViaServer);
                        hotbar[i] = temp;
                        break;
                    }
                    else if(hotbar[i].GetComponent<Item>().itemName == "Raw Chicken")
                    {
                        GameObject temp = oven.CurrentOven.GetComponent<Oven>().stored;
                        oven.CurrentOven.GetComponent<PhotonView>().RPC("SwapChicken", PhotonTargets.AllBufferedViaServer, hotbar[i].GetComponent<RawChicken>().cookedMagnitude);
                        hotbar[i] = temp;
                        break;
                    }
                }
            }

            ovenDrag = false;
        }
    }

    private void DropItem(Item item)
    {
        if(item.itemName == "Egg")
        {
            player.photonView.RPC("DropEgg", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
        else if(item.itemName == "Raw Chicken")
        {
            player.photonView.RPC("DropMeat", PhotonTargets.MasterClient, transform.position.x, transform.position.y, item.GetComponent<RawChicken>().cookedMagnitude);
        }
        else if (item.itemName == "Caged Chicken")
        {
            player.photonView.RPC("DropCagedChicken", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
        else if (item.itemName == "Axe")
        {
            player.photonView.RPC("DropAxe", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
        else if (item.itemName == "Chicken Feed")
        {
            player.photonView.RPC("DropFeedBag", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
    }

    private void UpdatePosition()
    {
        float anchorX = Hotbar.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = Hotbar.GetComponent<RectTransform>().anchoredPosition.y;

        if (visible)
        {
            if (Hotbar.GetComponent<RectTransform>().anchoredPosition.y < 32)
            {
                Hotbar.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 2);
            }
        }
        else
        {
            if (Hotbar.GetComponent<RectTransform>().anchoredPosition.y > -36)
            {
                Hotbar.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 2);
            }
        }
    }

    private void UpdateHotbar()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateSlot1();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateSlot2();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateSlot3();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateSlot4();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActivateSlot5();
        }
        
        float xPos = boarder.GetComponent<RectTransform>().anchoredPosition.x;
        float yPos = boarder.GetComponent<RectTransform>().anchoredPosition.y;
        if (xPos < transformDestination - 2.5f)
        {
            boarder.GetComponent<RectTransform>().anchoredPosition = new Vector3(boarder.transform.localPosition.x + 2.5f, yPos);
        }
        else if (xPos > transformDestination + 2.5f)
        {
            boarder.GetComponent<RectTransform>().anchoredPosition = new Vector3(boarder.transform.localPosition.x - 2.5f, yPos);
        }
        else
        {
            boarder.GetComponent<RectTransform>().anchoredPosition = new Vector3(transformDestination, yPos);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (hotbar[i] == null)
            {
                slotIcons[i].sprite = empty;
                amounts[i].text = "";
                amountContainers[i].gameObject.SetActive(false);
            }
            else
            {
                slotIcons[i].sprite = hotbar[i].GetComponent<Image>().sprite;
                if(hotbar[i].GetComponent<Item>().stackable)
                {
                    amountContainers[i].gameObject.SetActive(true);
                    amounts[i].text = hotbar[i].GetComponent<Item>().currentStack + "";
                }
                else
                {
                    amounts[i].text = "";
                    amountContainers[i].gameObject.SetActive(false);
                }
            }

            if (!drag)
            {
                // updates the special image layers for chicken
                if (hotbar[i] != null && hotbar[i].GetComponent<Item>().itemName == "Raw Chicken")
                {
                    iconLayer2[i].sprite = cooked;
                    iconLayer3[i].sprite = burnt;
                    float cookedMagnitude = hotbar[i].GetComponent<RawChicken>().cookedMagnitude;
                    if (cookedMagnitude <= 255)
                    {
                        Color temp = slotIcons[i].color;
                        temp.a = (255 - cookedMagnitude) / 255f;
                        slotIcons[i].color = temp;
                        iconLayer2[i].color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (cookedMagnitude <= 510)
                    {
                        Color temp = iconLayer2[i].color;
                        Color temp2 = slotIcons[i].color;
                        temp.a = (510 - cookedMagnitude) / 255f;
                        temp2.a = 0;
                        iconLayer2[i].color = temp;
                        slotIcons[i].color = temp2;
                    }
                }
                else
                {
                    slotIcons[i].color = new Color(1f, 1f, 1f, 1f);
                    iconLayer2[i].color = new Color(1f, 1f, 1f, 1f);
                    iconLayer3[i].color = new Color(1f, 1f, 1f, 1f);
                    iconLayer2[i].sprite = empty;
                    iconLayer3[i].sprite = empty;
                }
            }
            
        }
    }

    // method that adds 1 item to the hotbar
    public void AddItem(GameObject item)
    {
        if (item.GetComponent<Item>().stackable) {
            bool contain = false;
            for (int i = 0; i < hotbar.Length; i++)
            {
                if (hotbar[i] != null && hotbar[i].GetComponent<Item>().itemName == item.GetComponent<Item>().itemName &&
                    hotbar[i].GetComponent<Item>().currentStack != hotbar[i].GetComponent<Item>().maxStack)
                {
                    contain = true;
                    hotbar[i].GetComponent<Item>().currentStack++;
                    break;
                }
            }

            if (!contain)
            {
                for (int i = 0; i < hotbar.Length; i++)
                {
                    if (hotbar[i] == null)
                    {
                        hotbar[i] = item;
                        hotbar[i].GetComponent<Item>().currentStack = 1;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < hotbar.Length; i++)
            {
                if (hotbar[i] == null)
                {
                    hotbar[i] = item;
                    break;
                }
            }
        }
    }

    // adds a specific amount of an item to the hotbar
    public void AddItem(GameObject item, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            AddItem(item);
        }
    }

    // a spot must be avaliable in the inventory
    public void AddChicken(float magnitude)
    {
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i] == null)
            {
                hotbar[i] = Instantiate(rawChicken);
                hotbar[i].GetComponent<RawChicken>().cookedMagnitude = magnitude;
                break;
            }
        }
    }

    public bool CanAdd(GameObject item)
    {
        bool canAdd = false; ;
        if(item.GetComponent<Item>().stackable)
        {
            for (int i = 0; i < hotbar.Length; i++)
            {
                if (hotbar[i] == null ||
                    (hotbar[i] != null && hotbar[i].GetComponent<Item>().itemName == item.GetComponent<Item>().itemName
                    && hotbar[i].GetComponent<Item>().currentStack != hotbar[i].GetComponent<Item>().maxStack))
                {
                    canAdd = true;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < hotbar.Length; i++)
            {
                if (hotbar[i] == null)
                {
                    canAdd = true;
                    break;
                }
            }
        }

        return canAdd;
    }

    public void ActivateSlot1()
    {
        selected = 0;
        transformDestination = -31.5f;
    }

    public void ActivateSlot2()
    {
        selected = 1;
        transformDestination = -15.75f;
    }

    public void ActivateSlot3()
    {
        selected = 2;
        transformDestination = 0f;
    }

    public void ActivateSlot4()
    {
        selected = 3;
        transformDestination = 15.75f;
    }

    public void ActivateSlot5()
    {
        selected = 4;
        transformDestination = 31.5f;
    }

    [PunRPC]
    private void DropEgg(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(egg.name, new Vector2(x + dropOffsetX, y + dropOffsetY - 0.1f), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropMeat(float x, float y, float cookedMagnitude)
    {
        object[] magnitude = { cookedMagnitude };
        PhotonNetwork.InstantiateSceneObject(rawChicken.name, new Vector2(x + dropOffsetX, y), Quaternion.identity, 0, magnitude);
    }

    [PunRPC]
    private void DropCagedChicken(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(cageChickenWorld.name, new Vector2(x + dropOffsetX, y), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropAxe(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(burriedAxe.name, new Vector2(x + dropOffsetX, y), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropFeedBag(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(chickenFeed.name, new Vector2(x + dropOffsetX, y), Quaternion.identity, 0, null);
    }
}
