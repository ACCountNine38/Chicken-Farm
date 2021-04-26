using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerRange : MonoBehaviour
{
    public CircleCollider2D dangerRange;
    List<GameObject> currentDangerInRange = new List<GameObject>();

    //chicken will run away from objects with these tags
    List<string> dangerTags = new List<string>();

    private void Start()
    {
        dangerTags.AddRange(new string[] {"Player"});
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (dangerTags.Contains(collider.gameObject.tag))
        {
            currentDangerInRange.Add(collider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (dangerTags.Contains(collider.gameObject.tag))
        {
            currentDangerInRange.Remove(collider.gameObject);
        }
    }

    private void Update()
    {

        if (currentDangerInRange.Count > 0)
        {
            Vector2 dangerDir = new Vector2(0, 0);

            foreach (GameObject danger in currentDangerInRange)
            {
                dangerDir = dangerDir + (Vector2) (danger.transform.position - transform.position).normalized;
            }

            transform.parent.GetComponent<Chicken>().DangerDetected(dangerDir.normalized);

        }

    }

}
