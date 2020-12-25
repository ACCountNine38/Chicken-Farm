using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour
{
    private RectTransform bounds;

    public void Awake()
    {
        bounds = GetComponent<RectTransform>();
    }

    public bool MouseHover()
    {
        Vector2 localMousePosition = bounds.InverseTransformPoint(Input.mousePosition);
        if (bounds.rect.Contains(localMousePosition))
        {
            return true;
        }

        return false;
    }
}
