using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Sprite emptySprite, cookedChicken, burntChicken;
    public Image layer1, layer2, layer3;
    public GameObject amountHolder;
    public Text amount;

    public Item item;
    private RectTransform bounds;
    private bool inBound;

    private Color originalColor = new Color(1f, 1f, 1f, 1f);

    public void Awake()
    {
        bounds = GetComponent<RectTransform>();
    }

    public void Update()
    {
        MouseHover();
        if (item != null)
        {
            layer1.sprite = item.GetComponent<Image>().sprite;

            // the item icon is special (composed of 3 layers) for chickens
            if (item.itemName == "Raw Chicken")
            {
                layer2.sprite = cookedChicken;
                layer3.sprite = burntChicken;

                float cookedMagnitude = item.cookedMagnitude;
                if (cookedMagnitude <= 255)
                {
                    Color temp = layer1.color;
                    temp.a = (255 - cookedMagnitude) / 255f;
                    layer1.color = temp;
                    layer2.color = new Color(1f, 1f, 1f, 1f);
                }
                else if (cookedMagnitude <= 510)
                {
                    Color temp1 = layer1.color;
                    temp1.a = 0;
                    layer1.color = temp1;

                    Color temp2 = layer2.color;
                    temp2.a = (510 - cookedMagnitude) / 255f;
                    layer2.color = temp2;
                }
            }
            else
            {
                layer2.sprite = emptySprite;
                layer3.sprite = emptySprite;
            }

            if (item.stackable)
            {
                amountHolder.SetActive(true);
                amount.text = item.currentStack + "";
            }
            else
            {
                amount.text = "";
                amountHolder.SetActive(false);
            }
        }
        else
        {
            layer1.sprite = emptySprite;
            layer2.sprite = emptySprite;
            layer3.sprite = emptySprite;

            layer1.color = originalColor;
            layer2.color = originalColor;
            layer3.color = originalColor;

            amountHolder.SetActive(false);
        }
    }

    public void ChangePosition(Vector2 newPos)
    {
        layer1.transform.position = newPos;
        layer2.transform.position = newPos;
        layer3.transform.position = newPos;
    }

    public void ResetPosition()
    {
        layer1.transform.localPosition = Vector2.zero;
        layer2.transform.localPosition = Vector2.zero;
        layer3.transform.localPosition = Vector2.zero;
    }

    public void ChangeColor(Color color)
    {
        layer1.color = color;
        layer2.color = color;
        //layer3.color = color;
    }

    public bool MouseHover()
    {
        if (bounds == null)
            return false;

        Vector2 localMousePosition = bounds.InverseTransformPoint(Input.mousePosition);
        if (bounds.rect.Contains(localMousePosition))
        {
            if (!inBound)
            {
                FindObjectOfType<AudioManager>().Play("slot hover");
                inBound = true;
            }
            return true;
        }

        inBound = false;
        return false;
    }
}
