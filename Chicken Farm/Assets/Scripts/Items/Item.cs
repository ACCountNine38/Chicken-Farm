using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public bool stackable, canObserve;
    public int maxStack, currentStack;
}
