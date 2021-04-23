
using UnityEngine;

public class FridgeManager : MonoBehaviour
{
    [HideInInspector]
    public Fridge CurrentFridge;

    public ItemSlot[] slots = new ItemSlot[15];

    // Update is called once per frame
    void Update()
    {
        
    }
}
