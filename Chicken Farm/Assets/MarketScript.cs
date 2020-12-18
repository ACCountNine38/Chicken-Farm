using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketScript : MonoBehaviour
{

    public ParticleSystem SmallPurchase, MediumPurchase, HeavyPurchase;
    public GameObject CoinReleaser;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseEggs()
    {
        player.money -= 5;
    }

    public void PurchaseChicken()
    {
        player.money -= 25;
    }

    public void PurchaseSmall()
    {
        Instantiate(SmallPurchase, CoinReleaser.transform.localPosition, Quaternion.identity);
    }

    public void PurchaseMedium()
    {
        Instantiate(SmallPurchase, CoinReleaser.transform.localPosition, Quaternion.identity);
    }

    public void PurchaseHeavy()
    {
        Instantiate(SmallPurchase, CoinReleaser.transform.localPosition, Quaternion.identity);
    }
}
