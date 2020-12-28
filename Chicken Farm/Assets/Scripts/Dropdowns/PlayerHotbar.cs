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
    public Sprite empty;
    public bool visible, drag;
    public int selected = 0;

    private float transformDestination = -31.5f, dropOffsetX, dropOffsetY;
    private int draggedIndex;
    private Vector2[] originalHotbarPositions = new Vector2[5];

    // items
    public GameObject eggItem, cagedChicken, axe, rawChicken;
    // world items
    public GameObject egg, raw, cageChickenWorld, burriedAxe;

    // Start is called before the first frame update
    private void Awake()
    {
        for(int i = 0; i < 5; i++)
        {
            originalHotbarPositions[i] = slotIcons[i].transform.localPosition;
        }
        AddItem(Instantiate(axe));
        AddItem(Instantiate(eggItem), 2);
        AddItem(Instantiate(cagedChicken));
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
            slotIcons[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            slotIcons[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!drag)
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if(drag)
        {
            bool found = false;
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

            if(!found)
            {
                if(player.direction == 0)
                {
                    dropOffsetX = -0.25f;
                }
                else
                {
                    dropOffsetX = 0.25f;
                }

                if (hotbar[draggedIndex].GetComponent<Item>().stackable)
                {
                    for(int i = 0; i < hotbar[draggedIndex].GetComponent<Item>().currentStack; i++)
                    {
                        dropOffsetY = Random.Range(0, 0.2f);
                        DropItem(hotbar[draggedIndex].GetComponent<Item>().itemName);
                    }
                }
                else
                {
                    dropOffsetY = Random.Range(0, 0.2f);
                    DropItem(hotbar[draggedIndex].GetComponent<Item>().itemName);
                }
                hotbar[draggedIndex] = null;
            }

            drag = false;
            slotIcons[draggedIndex].transform.localPosition = originalHotbarPositions[draggedIndex];
            slotIcons[draggedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private void DropItem(string name)
    {
        if(name == "Egg")
        {
            player.photonView.RPC("DropEgg", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
        else if(name == "Raw Chicken")
        {
            player.photonView.RPC("DropMeat", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
        else if (name == "Caged Chicken")
        {
            player.photonView.RPC("DropCagedChicken", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
        }
        else if (name == "Axe")
        {
            player.photonView.RPC("DropAxe", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
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
    private void DropMeat(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(raw.name, new Vector2(x + dropOffsetX, y), Quaternion.identity, 0, null);
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
}
