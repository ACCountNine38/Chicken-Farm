using UnityEngine;

public class Fridge : Structure
{
    [HideInInspector]
    public GameObject[] stored = new GameObject[15];

    public GameObject[] prefabs;

    private void Update()
    {
        CheckHovering();
    }

    [PunRPC]
    private void RemoveItem(int index)
    {
        stored[index] = null;
    }

    [PunRPC]
    private void SwapItem(int itemIndex, int index, int currentStack, float cookedMagnitude)
    {
        stored[index] = Instantiate(prefabs[itemIndex]);
        stored[index].GetComponent<Item>().cookedMagnitude = cookedMagnitude;
        stored[index].GetComponent<Item>().currentStack = currentStack;
    }
}
