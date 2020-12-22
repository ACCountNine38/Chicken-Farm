using UnityEngine;
using UnityEngine.UI;

public class PlayerHotbar : MonoBehaviour
{
    public Player player;
    public GameObject Hotbar, boarder;
    public GameObject[] slots = new GameObject[5];
    public Text[] amounts = new Text[5];
    public Image[] amountContainers = new Image[5];
    public GameObject[] hotbar = new GameObject[5];
    public Sprite empty;
    public bool visible;
    public int selected = 0;

    private float transformDestination = -31.5f;

    // items
    public GameObject eggItem;
    public GameObject cagedChicken;
    public GameObject axe;

    // Start is called before the first frame update
    private void Awake()
    {
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
                slots[i].GetComponent<Image>().sprite = empty;
                amounts[i].text = "";
                amountContainers[i].gameObject.SetActive(false);
            }
            else
            {
                slots[i].GetComponent<Image>().sprite = hotbar[i].GetComponent<Image>().sprite;
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

    public bool IsFull()
    {
        bool full = true;
        for (int i = 0; i < hotbar.Length; i++)
        {
            if(hotbar[i] == null ||
                (hotbar[i] != null && hotbar[i].GetComponent<Item>().currentStack != hotbar[i].GetComponent<Item>().maxStack))
            {
                full = false;
                break;
            }
        }

        return full;
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
}
