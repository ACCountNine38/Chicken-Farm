using UnityEngine;

public class PlayerMarket : MonoBehaviour
{
    public Player player;
    public GameObject MarketMenu;

    public bool visible = false;
    private int previousMoney;

    private void Awake()
    {
        previousMoney = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if(player.photonView.isMine) {
            UpdateMarket();
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

        if (visible && Input.GetKeyDown(KeyCode.P))
        {
            ExitMarket();
        }
        else if (!visible && Input.GetKeyDown(KeyCode.P))
        {
            visible = true;
        }

        float anchorX = MarketMenu.GetComponent<RectTransform>().anchoredPosition.x;
        float anchorY = MarketMenu.GetComponent<RectTransform>().anchoredPosition.y;

        if (visible)
        {
            if (MarketMenu.GetComponent<RectTransform>().anchoredPosition.y > -240)
            {
                MarketMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY - 20);
            }
            if (!MarketMenu.activeSelf)
            {
                MarketMenu.SetActive(true);
            }
        }
        else
        {
            if (MarketMenu.GetComponent<RectTransform>().anchoredPosition.y < 240)
            {
                MarketMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(anchorX, anchorY + 20);
            }
            if (MarketMenu.GetComponent<RectTransform>().anchoredPosition.y >= 240 && MarketMenu.activeSelf)
            {
                MarketMenu.SetActive(false);
            }
        }
    }

    public void ExitMarket()
    {
        visible = false;
    }

    // methods for purchasing a specific item
    public void PurchaseEggs()
    {
        player.money -= 5;
    }

    public void PurchaseChicken()
    {
        player.money -= 25;
    }

}
