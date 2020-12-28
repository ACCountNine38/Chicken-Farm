using UnityEngine;

public class DangerRange : MonoBehaviour
{
    public CircleCollider2D dangerRange;

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.parent.GetComponent<Chicken>().DangerDetected(collision);
        }
    }
}
