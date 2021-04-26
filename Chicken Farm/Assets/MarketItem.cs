using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public GameObject item;

    public int cost;

    public Image icon;
    public Text nameLabel, description, costLabel;
    public Button purchase;
}
