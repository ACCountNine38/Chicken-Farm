using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]
    public Player player;

    public bool marketVisible, speechVisible, auctionVisible;

    // Oven UI
    public GameObject OvenMenu;
    public OvenManager oven;

    // Market UI
    private GameObject MarketMenu, VendorSpeech;
    public GameObject MarketLayout, BackupLayout;
    public GameObject eggOption, cagedChickenOption, feedOption;
    public Text vendorSpeechText;
    private string currentVendorText = "";
    private float vendorSpeechTimer = 0f, vendorSoundTimer = 0f;
    private bool canSpeek;

    // Auction UI
    private GameObject AuctionMenu;
    private ItemSlot AuctionSlot;
    private Text AuctionSlotNameText, AuctionSellPrice, AuctionInstruction;
    private Image AuctionCoinIcon;

    private int previousMoney;

    private void Awake()
    {
        VendorSpeech = GameObject.Find("Vendor Speech").gameObject;
        MarketMenu = GameObject.Find("Market Menu").gameObject;
        AuctionMenu = GameObject.Find("Auction Menu").gameObject;
        AuctionSlot = GameObject.Find("Auction Slot").GetComponent<ItemSlot>();
        AuctionSlotNameText = GameObject.Find("Auction Item Name").GetComponent<Text>();
        AuctionSellPrice = GameObject.Find("Auction Sell Price").GetComponent<Text>();
        AuctionInstruction = GameObject.Find("Auction Instruction").GetComponent<Text>();
        AuctionCoinIcon = GameObject.Find("Auction Coin Icon").GetComponent<Image>();
        OvenMenu = GameObject.Find("Oven Menu").gameObject;
        oven = OvenMenu.GetComponent<OvenManager>();

        AddToMarket(eggOption);
        AddToMarket(cagedChickenOption);
        AddToMarket(feedOption);
        previousMoney = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (player == null)
            return;

        if (player.photonView.isMine)
        {
            UpdateMarket();
            UpdateAuction();
            UpdateOven();
        }
    }

    private void UpdateMarket()
    {
        if (player.money != previousMoney)
        {
            if (previousMoney < player.money)
            {
                if (previousMoney < player.money - 15)
                {
                    previousMoney += 10;
                }
                else
                {
                    previousMoney += 1;
                }
            }
            else if (previousMoney > player.money)
            {
                if (previousMoney > player.money + 15)
                {
                    previousMoney -= 10;
                }
                else
                {
                    previousMoney -= 1;
                }
            }

            player.PlayerMoneyText.text = "Bank: $" + previousMoney;
        }

        UpdatePurchase();
        UpdateAuction();
        UpdateSpeech();
    }

    private void UpdatePurchase()
    {
        float anchorX = MarketMenu.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = MarketMenu.GetComponent<RectTransform>().anchoredPosition.y;

        if (marketVisible)
        {
            if (MarketMenu.GetComponent<RectTransform>().anchoredPosition.y > -176)
            {
                MarketMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
                if (MarketMenu.GetComponent<RectTransform>().anchoredPosition.y < -176)
                {
                    VendorSpeech.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, -176);
                }
            }
            if (!MarketMenu.activeSelf)
            {
                MarketMenu.SetActive(true);
            }
        }
        else
        {
            if (MarketMenu.GetComponent<RectTransform>().anchoredPosition.y < 220)
            {
                MarketMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
            }

            if (MarketMenu.activeSelf && MarketMenu.GetComponent<RectTransform>().anchoredPosition.y >= 220)
            {
                MarketMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 220);
                MarketMenu.SetActive(false);
            }
        }
    }

    private void UpdateAuction()
    {
        if (AuctionSlot.item != null)
        {
            if (!AuctionSlotNameText.enabled)
            {
                AuctionInstruction.enabled = false;
                AuctionSlotNameText.enabled = true;
                AuctionSellPrice.enabled = true;
                AuctionCoinIcon.enabled = true;
            }
            string nameText, priceText;
            if (AuctionSlot.item.stackable)
            {
                nameText = AuctionSlot.item.itemName + " x" + AuctionSlot.item.currentStack;
                priceText = "Price x     : " + AuctionSlot.item.sellPrice * AuctionSlot.item.currentStack;
            }
            else
            {
                int sellPrice;
                if (AuctionSlot.item.itemName == "Raw Chicken")
                {
                    if (AuctionSlot.item.cookedMagnitude <= 255)
                    {
                        sellPrice = AuctionSlot.item.sellPrice + (int)(AuctionSlot.item.cookedMagnitude / 10);
                    }
                    else
                    {
                        sellPrice = 35 - (int)((AuctionSlot.item.cookedMagnitude - 255) / 7.5f);
                    }
                }
                else
                {
                    sellPrice = AuctionSlot.item.sellPrice;
                }
                nameText = AuctionSlot.item.itemName;
                priceText = "Price x     : " + sellPrice;
            }
            
            if (AuctionSlotNameText.text != nameText)
            {
                AuctionSlotNameText.text = nameText;
            }
            if (AuctionSellPrice.text != priceText)
            {
                AuctionSellPrice.text = priceText;
            }
        }
        else
        {
            if (AuctionSlotNameText.enabled)
            {
                AuctionInstruction.enabled = true;
                AuctionSlotNameText.enabled = false;
                AuctionSellPrice.enabled = false;
                AuctionCoinIcon.enabled = false;
            }
        }

        float anchorX = AuctionMenu.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = AuctionMenu.GetComponent<RectTransform>().anchoredPosition.y;

        if (auctionVisible)
        {
            if (AuctionMenu.GetComponent<RectTransform>().anchoredPosition.y > -176)
            {
                AuctionMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
                if (AuctionMenu.GetComponent<RectTransform>().anchoredPosition.y < -176)
                {
                    AuctionMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, -176);
                }
            }
            if (!AuctionMenu.activeSelf)
            {
                AuctionMenu.SetActive(true);
            }
        }
        else
        {
            if (AuctionMenu.GetComponent<RectTransform>().anchoredPosition.y < 220)
            {
                AuctionMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
            }

            if (AuctionMenu.activeSelf && AuctionMenu.GetComponent<RectTransform>().anchoredPosition.y >= 220)
            {
                AuctionMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 220);
                AuctionMenu.SetActive(false);
            }
        }
    }

    private void UpdateSpeech()
    {
        float anchorX = VendorSpeech.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = VendorSpeech.GetComponent<RectTransform>().anchoredPosition.y;

        if (speechVisible)
        {
            if (VendorSpeech.GetComponent<RectTransform>().anchoredPosition.y < 94)
            {
                VendorSpeech.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
                if (VendorSpeech.GetComponent<RectTransform>().anchoredPosition.y >= 94)
                {
                    VendorSpeech.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 94);
                    canSpeek = true;
                }
            }
            if (!VendorSpeech.activeSelf)
            {
                VendorSpeech.SetActive(true);
            }
        }
        else
        {
            canSpeek = false;
            if (VendorSpeech.GetComponent<RectTransform>().anchoredPosition.y > -94)
            {
                VendorSpeech.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
            }
            if (VendorSpeech.activeSelf && VendorSpeech.GetComponent<RectTransform>().anchoredPosition.y <= -94)
            {
                VendorSpeech.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, -94);
                VendorSpeech.SetActive(false);
            }
        }

        if (speechVisible && vendorSpeechText.text != currentVendorText && canSpeek)
        {
            vendorSpeechTimer += Time.deltaTime;
            vendorSoundTimer += Time.deltaTime;
            if (vendorSpeechTimer > 0.04f)
            {
                if (vendorSpeechText.text.Length < currentVendorText.Length)
                {
                    vendorSpeechText.text += currentVendorText[vendorSpeechText.text.Length];
                    vendorSpeechTimer = 0f;
                }
            }
            if (vendorSoundTimer > 0.15f)
            {
                FindObjectOfType<AudioManager>().Play("talk" + Random.Range(1, 4));
                vendorSoundTimer = 0f;
            }
        }
    }

    private void UpdateOven()
    {
        float anchorX = OvenMenu.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = OvenMenu.GetComponent<RectTransform>().anchoredPosition.y;

        if (oven.visible)
        {
            if (OvenMenu.GetComponent<RectTransform>().anchoredPosition.y > 0)
            {
                OvenMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 1200 * Time.deltaTime);
            }
            else
            {
                OvenMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 0);
            }

            if (!OvenMenu.activeSelf)
            {
                OvenMenu.SetActive(true);
            }
        }
        else
        {
            if (OvenMenu.GetComponent<RectTransform>().anchoredPosition.y < 400)
            {
                OvenMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 1200 * Time.deltaTime);
            }
            else
            {
                OvenMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, 400);
            }

            if (OvenMenu.GetComponent<RectTransform>().anchoredPosition.y >= 400 && OvenMenu.activeSelf)
            {
                OvenMenu.SetActive(false);
            }
        }

        if (oven.CurrentOven != null)
        {
            if (oven.CurrentOven.stored != null && player.hotbar.slots[player.hotbar.OVEN_INDEX] != null && player.hotbar.slots[player.hotbar.OVEN_INDEX].item != null)
            {
                player.hotbar.slots[player.hotbar.OVEN_INDEX].item.cookedMagnitude = oven.CurrentOven.stored.GetComponent<Item>().cookedMagnitude;
            }
        }
    }

    public void AddToMarket(GameObject item)
    {
        item.transform.SetParent(MarketLayout.transform);
    }

    public void RemoveFromMarket(GameObject item)
    {
        item.transform.SetParent(BackupLayout.transform);
    }

    public void BuyItem(MarketItem item)
    {
        if (player.hotbar.CanAdd(Instantiate(item.item)))
        {
            player.money -= item.cost;
            player.hotbar.AddItem(Instantiate(item.item));
            FindObjectOfType<AudioManager>().Play("buy" + Random.Range(1, 4));
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("deny");
        }
    }

    public void SellAuction()
    {
        if (AuctionSlot.item != null)
        {
            int sellPrice;
            if (AuctionSlot.item.stackable)
            {
                sellPrice = AuctionSlot.item.sellPrice * AuctionSlot.item.currentStack;
            }
            else
            {
                if (AuctionSlot.item.itemName == "Raw Chicken")
                {
                    if (AuctionSlot.item.cookedMagnitude <= 255)
                    {
                        sellPrice = AuctionSlot.item.sellPrice + (int)(AuctionSlot.item.cookedMagnitude / 10);
                    }
                    else
                    {
                        sellPrice = 35 - (int)((AuctionSlot.item.cookedMagnitude - 255) / 7.5f);
                    }
                }
                else
                {
                    sellPrice = AuctionSlot.item.sellPrice;
                }
            }
            player.money += sellPrice;
            AuctionSlot.item = null;
            FindObjectOfType<AudioManager>().Play("buy1");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("deny");
        }
    }

    public void EnterAuction()
    {
        auctionVisible = true;
        FindObjectOfType<AudioManager>().Play("market");
    }

    public void ExitAuction()
    {
        auctionVisible = false;
        vendorSpeechText.text = "";
        speechVisible = true;
        int randText = Random.Range(1, 9);
        if (randText == 1)
        {
            currentVendorText = "Come on...thats it???";
        }
        else if (randText == 2)
        {
            currentVendorText = "Anything else you like to sell?";
        }
        else if (randText == 3)
        {
            currentVendorText = "Your farm won't last long if you dont keep trading.";
        }
        else if (randText == 4)
        {
            currentVendorText = "I'm sure you can buy or sell more stuff...";
        }
        else if (randText == 5)
        {
            currentVendorText = "Anything else I can help you with?";
        }
        else if (randText == 6)
        {
            currentVendorText = "I'll be here if you need to sell anything else.";
        }
        else if (randText == 7)
        {
            currentVendorText = "The market is open if you want to buy...";
        }
        else
        {
            currentVendorText = "These are the lowest offers I can go.";
        }
        vendorSoundTimer = 0f;
        vendorSpeechTimer = 0f;
        FindObjectOfType<AudioManager>().Play("market");
    }

    public void ExitMarket()
    {
        marketVisible = false;
        speechVisible = true;
        vendorSpeechText.text = "";
        int randText = Random.Range(1, 9);
        if (randText == 1)
        {
            currentVendorText = "Come on...thats it???";
        }
        else if (randText == 2)
        {
            currentVendorText = "Anything else you like to buy?";
        }
        else if (randText == 3)
        {
            currentVendorText = "Your farm won't last long if you dont keep trading.";
        }
        else if (randText == 4)
        {
            currentVendorText = "I'm sure you can buy or sell more stuff...";
        }
        else if (randText == 5)
        {
            currentVendorText = "Anything else I can help you with?";
        }
        else if (randText == 6)
        {
            currentVendorText = "I'll be here if you need to buy anything else.";
        }
        else if (randText == 7)
        {
            currentVendorText = "The auction is open if you want to sell...";
        }
        else
        {
            currentVendorText = "Make sure to get more money next time!!!";
        }
        vendorSoundTimer = 0f;
        vendorSpeechTimer = 0f;
        FindObjectOfType<AudioManager>().Play("market");
    }

    public void EnterMarket()
    {
        marketVisible = true;
        FindObjectOfType<AudioManager>().Play("market");
    }

    public void ExitVendor()
    {
        speechVisible = false;
    }

    public void TalkToVendor()
    {
        speechVisible = true;
        vendorSpeechText.text = "";
        int randText = Random.Range(1, 8);
        if (randText == 1)
        {
            currentVendorText = "Good to see you. Find anything you like?";
        }
        else if (randText == 2)
        {
            currentVendorText = "Finally someone with money is around...";
        }
        else if (randText == 3)
        {
            currentVendorText = "See anything you like?";
        }
        else if (randText == 4)
        {
            currentVendorText = "How can I help you?";
        }
        else if (randText == 5)
        {
            currentVendorText = "Wanna buy some chickens friend?";
        }
        else if (randText == 6)
        {
            currentVendorText = "Hi...hi...you wanna buy some...funnel cake?";
        }
        else
        {
            currentVendorText = "Get the finest products for your farm!";
        }
        vendorSoundTimer = 0f;
        vendorSpeechTimer = 0f;
    }

    public void ExitOven()
    {
        oven.visible = false;
        oven.CurrentOven = null;
        FindObjectOfType<AudioManager>().Play("oven");
    }
}
