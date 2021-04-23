using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public Player player;
    public GameObject boarder;
    public Sprite empty, cooked, burnt;

    public ItemSlot[] slots = new ItemSlot[22];

    public bool visible, drag;
    public int selected, draggedIndex;

    private float transformDestination = -31.5f;

    [HideInInspector]
    public bool dragColorUpdated;
    [HideInInspector]
    public int HOTBAR_LENGTH = 5, AUCTION_INDEX = 5, OVEN_INDEX = 6, FRIDGE_START_INDEX = 7;

    // items
    public GameObject eggItem, cagedChicken, axe, rawChicken, feedBag;

    // Start is called before the first frame update
    private void Awake()
    {
        AddItem(Instantiate(axe));
        AddItem(Instantiate(cagedChicken));
        AddItem(Instantiate(feedBag), 5);
    }

    // Update is called once per frame
    private void Update()
    {
        if(player == null)
        {
            return;
        }

        if (player.photonView.isMine)
        {
            UpdatePosition();
            UpdateHotbar();
            CheckPress();
            CheckDrag();
        }
    }

    // updates the item icon if an item is still being dragged
    private void CheckDrag()
    {
        if (drag && !slots[draggedIndex].MouseHover())
        {
            slots[draggedIndex].ChangePosition(Input.mousePosition);

            if (!dragColorUpdated)
            {
                slots[draggedIndex].ScaleAlpha(0.5f);
                dragColorUpdated = true;
            }
        }
    }

    // checks if an item is going to be dragged or dropped
    public void CheckPress()
    {
        if (Input.GetMouseButtonDown(0) && !drag)
        {
            for(int i = 0; i < slots.Length; i++)
            {
                if(slots[i].MouseHover() && slots[i].item != null)
                {
                    drag = true;
                    draggedIndex = i;
                    break;
                }
            }
        }

        if(Input.GetMouseButtonUp(0) && drag)
        {
            bool found = false;

            // check if item is droped into hotbar
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].MouseHover())
                {
                    found = true;
                    if (i == OVEN_INDEX && slots[draggedIndex].item.itemName != "Raw Chicken")
                    {
                        FindObjectOfType<AudioManager>().Play("deny");
                    }
                    else if (i == OVEN_INDEX && slots[draggedIndex].item.itemName == "Raw Chicken")
                    {
                        player.uiManager.OvenMenu.GetComponent<OvenManager>().CurrentOven.photonView.RPC("SwapChicken", PhotonTargets.AllBuffered, slots[draggedIndex].item.cookedMagnitude);
                        Item temp = slots[i].item;
                        slots[i].item = slots[draggedIndex].item;
                        slots[draggedIndex].item = temp;
                        slots[i].ChangeColor(Color.white);
                    }
                    else 
                    {
                        Item temp = slots[i].item;
                        slots[i].item = slots[draggedIndex].item;
                        slots[draggedIndex].item = temp;
                        slots[i].ChangeColor(Color.white);
                        if (draggedIndex == OVEN_INDEX)
                        {
                            player.uiManager.OvenMenu.GetComponent<OvenManager>().CurrentOven.photonView.RPC("RemoveChicken", PhotonTargets.AllBuffered);
                        }
                    }
                }
            }

            // checks if item is being dropped onto the world
            if (!found && slots[draggedIndex].item && !player.InterfaceOpen())
            {
                FindObjectOfType<AudioManager>().Play("slot select");
                if (slots[draggedIndex].item.stackable)
                {
                    for (int i = 0; i < slots[draggedIndex].item.currentStack; i++)
                    {
                        DropItem(slots[draggedIndex].item);
                    }
                }
                else
                {
                    DropItem(slots[draggedIndex].item);
                }

                slots[draggedIndex].item = null;
            }

            drag = false;
            dragColorUpdated = false;

            slots[draggedIndex].ChangeColor(Color.white);
            slots[draggedIndex].ResetPosition();
        }
    }

    // drops items to world view as scene objects
    private void DropItem(Item item)
    {
        if (item.itemName == "Egg")
        {
            player.photonView.RPC("DropEgg", PhotonTargets.MasterClient, player.transform.position.x, player.transform.position.y);
        }
        else if (item.itemName == "Raw Chicken")
        {
            player.photonView.RPC("DropMeat", PhotonTargets.MasterClient, player.transform.position.x, player.transform.position.y, item.cookedMagnitude);
        }
        else if (item.itemName == "Caged Chicken")
        {
            player.photonView.RPC("DropCagedChicken", PhotonTargets.MasterClient, player.transform.position.x, player.transform.position.y);
        }
        else if (item.itemName == "Axe")
        {
            player.photonView.RPC("DropAxe", PhotonTargets.MasterClient, player.transform.position.x, player.transform.position.y);
        }
        else if (item.itemName == "Chicken Feed")
        {
            player.photonView.RPC("DropFeedBag", PhotonTargets.MasterClient, player.transform.position.x, player.transform.position.y);
        }
    }

    // updates the location of the entire hotbar
    private void UpdatePosition()
    {
        float anchorX = GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = GetComponent<RectTransform>().anchoredPosition.y;

        if (visible)
        {
            if (GetComponent<RectTransform>().anchoredPosition.y < 32)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + Time.deltaTime * 300);
                if(GetComponent<RectTransform>().anchoredPosition.y > 32)
                {
                    GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 32);
                }
            }
        }
        else
        {
            if (GetComponent<RectTransform>().anchoredPosition.y > -36)
            {
                GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - Time.deltaTime * 300);
                if (GetComponent<RectTransform>().anchoredPosition.y < -36)
                {
                    GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, -36);
                }
            }
        }
    }

    private void UpdateHotbar()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActivateSlot(4);
        }

        for (int i = 0; i < HOTBAR_LENGTH; i++)
        {
            if (Input.GetMouseButtonDown(0) && slots[i].MouseHover())
            {
                ActivateSlot(i);
            }
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
    }

    // method that adds 1 item to the hotbar
    public void AddItem(GameObject item)
    {
        if (item.GetComponent<Item>().stackable)
        {
            for (int i = 0; i < HOTBAR_LENGTH; i++)
            {
                if (slots[i].item != null && slots[i].item.itemName == item.GetComponent<Item>().itemName &&
                    slots[i].item.currentStack < slots[i].item.maxStack)
                {
                    slots[i].item.currentStack++;
                    return;
                }
            }

            for (int i = 0; i < HOTBAR_LENGTH; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].item = item.GetComponent<Item>();
                    slots[i].item.currentStack = 1;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < HOTBAR_LENGTH; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].item = item.GetComponent<Item>();
                    return;
                }
            }
        }
    }

    // adds a specific amount of an item to the hotbar
    public void AddItem(GameObject item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddItem(item);
        }
    }

    // a spot must be avaliable in the inventory
    public void AddChicken(float magnitude)
    {
        for (int i = 0; i < HOTBAR_LENGTH; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = Instantiate(rawChicken).GetComponent<Item>();
                slots[i].item.cookedMagnitude = magnitude;
                break;
            }
        }
    }

    public bool CanAdd(GameObject item)
    {
        bool canAdd = false; ;
        if (item.GetComponent<Item>().stackable)
        {
            for (int i = 0; i < HOTBAR_LENGTH; i++)
            {
                if (slots[i].item == null ||
                    (slots[i].item != null && slots[i].item.itemName == item.GetComponent<Item>().itemName
                    && slots[i].item.currentStack != slots[i].item.maxStack))
                {
                    canAdd = true;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < HOTBAR_LENGTH; i++)
            {
                if (slots[i].item == null)
                {
                    canAdd = true;
                    break;
                }
            }
        }

        return canAdd;
    }

    public void ActivateSlot(int i)
    {
        if (i >= 5)
            return;
        FindObjectOfType<AudioManager>().Play("slot select");
        selected = i;
        transformDestination = i*15.75f - 31.5f;
    }
}
