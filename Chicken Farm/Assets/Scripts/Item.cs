using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public bool stackable;
    public int maxStack, currentStack;

    public abstract void OnClick();
}
