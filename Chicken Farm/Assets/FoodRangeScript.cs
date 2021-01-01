using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodRangeScript : MonoBehaviour
{
    public CircleCollider2D foodRange;
    List<GameObject> currentFoodInRange = new List<GameObject>();
    Chicken chicken;

    //chicken will go after object with this tag
    public string foodTag;

    private void Start()
    {
        chicken = transform.parent.GetComponent<Chicken>();
        foodTag = chicken.foodTag;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == foodTag)
        {
            currentFoodInRange.Add(collider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == foodTag)
        {
            currentFoodInRange.Remove(collider.gameObject);
        }
    }

    private void Update()
    {

        if (currentFoodInRange.Count > 0)
        {
            GameObject closestFood = null;
            float distance = Mathf.Infinity;

            foreach (GameObject food in currentFoodInRange)
            {
                float dDistance = Vector3.Distance(food.transform.position, transform.position);
                if (dDistance < distance)
                {
                    distance = dDistance;
                    closestFood = food;
                }

            }

            VisionToFood(closestFood.transform.position);
        }

    }

    private void VisionToFood(Vector3 foodPos)
    {
        Ray ray = new Ray(chicken.transform.position, foodPos - chicken.transform.position);
        Debug.DrawRay(ray.origin, foodPos - chicken.transform.position, Color.white, 0.0f, false);

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction, foodRange.radius * 3))
        {
            if (hit)
            {
                if (hit.collider.gameObject.tag == "Chicken Sensory Range" || hit.collider.gameObject.tag == "Chicken")
                {
                    continue;
                }
                else if (hit.collider.gameObject.tag == foodTag)
                {
                    chicken.FoodDetected(foodPos);
                }
                else
                {
                    break;
                }

            }
        }
    }
}
