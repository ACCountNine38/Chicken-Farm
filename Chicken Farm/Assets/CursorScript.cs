using UnityEngine;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    public Image icon;
    public Sprite normal, hover, butcher, pickup;

    // Start is called before the first frame update
    public void Awake()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
}
