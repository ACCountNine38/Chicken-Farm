using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chicken : Creature
{
    // base chicken status
    public float hunger, eggCooldown;
    public int type;
    public bool isDead, canButcher, eating;

    public BoxCollider2D collider;
    public ParticleSystem bloodEffect;
    public ParticleSystem smokeEffect;
    public GameObject egg, raw;

    // variables for this chicken if it is named
    public bool isNamed, butcherProcess;
    public Text nametag;

    private float hungerTimer, butcherTimer, eggTimer, randomHunger;

    //chicken movement
    public float accelerationForce;
    List<Collision2D> currentCollisions = new List<Collision2D>();
    private float runOppositeTimer;
    private Vector2 runOppositeDirection;

    private GameObject currentFood;

    //chicken will go after object with this tag
    public string foodTag = "Egg";

    public void Awake()
    {
        status = "run";
        acceleration = 2000;
        eggCooldown = 100;
        original = sr.color;
        randomHunger = Random.Range(0, 100);
        accelerationForce = rb.mass * acceleration;
        photonView.RPC("SetRandomHunger", PhotonTargets.AllViaServer);
        photonView.RPC("UpdateType", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    private void SetRandomHunger()
    {
        hunger = randomHunger;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHovering();

        if (!isDead)
        {
            if (PhotonNetwork.isMasterClient)
            {
                UpdateMovingAnimation();
            }

            hungerTimer += Time.deltaTime;

            if (hungerTimer > 4)
            {
                hunger--;
                hungerTimer = 0;
            }

            if(type != CurrentType())
                photonView.RPC("UpdateType", PhotonTargets.MasterClient);

            if (butcherProcess)
            {
                butcherTimer += Time.deltaTime;
                if (butcherTimer >= 0.35f)
                {
                    photonView.RPC("Butcher", PhotonTargets.AllViaServer);
                    photonView.RPC("DropMeat", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
                    butcherTimer = 0;
                }
            }

        }
        else
        {
            if (!bloodEffect.isPlaying && !smokeEffect.isPlaying)
            {
                Die();
            }
        }

    }

    private int CurrentType()
    {
        if(hunger <= 33)
        {
            return 1;
        }
        else if(hunger <= 66)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    [PunRPC]
    private void UpdateType()
    {
        // thin chicken
        if (hunger <= 33)
        {
            type = 1;
            anim.SetInteger("type", 1);
            maxSpeed = 2;
            eggTimer = 100;
        }
        // normal chicken
        else if (hunger <= 66)
        {
            type = 0;
            anim.SetInteger("type", 0);
            maxSpeed = 2.5f;
            eggCooldown = 30;
        }
        // thicc chicken
        else
        {
            type = 2;
            anim.SetInteger("type", 2);
            maxSpeed = 1.5f;
            eggCooldown = 20;
        }
    }

    private void LayEgg()
    {
        if (type != 1)
        {
            eggTimer += Time.deltaTime;
            if (eggTimer > eggCooldown)
            {
                photonView.RPC("SpawnEgg", PhotonTargets.MasterClient, transform.position.x, transform.position.y);
                eggTimer = 0;
            }
        }
    }

    //moves the chicken
    private void UpdateMovingAnimation()
    {
        // animation updates
        if (Mathf.Abs(rb.velocity.magnitude) > 0.1)
        {
            anim.SetBool("isMoving", true);
            anim.SetBool("isEating", false);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    private void CheckStatus()
    {
        Vector3 forceDirection = moveDirection;

        if (status == "idle")
        {
            LayEgg();

        }
        else if (status == "move" || status == "run" || status == "eat" || status == "chaos")
        {

            if (direction == 1 && rb.velocity.x < -0.25)
            {
                direction = 0;
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }
            else if (direction == 0 && rb.velocity.x > 0.25)
            {
                direction = 1;
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }

            if (status == "move")
            {
                rb.AddForce(forceDirection * accelerationForce * Time.deltaTime);
                //rb.velocity = (moveDirection * maxSpeed);
            }
            else if(status == "eat")
            {
                rb.velocity = moveDirection * maxSpeed;
            }
            else if (status == "run")
            {
                if (rb.velocity.x == 0 || rb.velocity.y == 0)
                {
                    moveDirection = Random.onUnitSphere * maxSpeed;
                    statusTimer = 0;
                    maxTimer = 1f;
                    status = "chaos";
                }

                rb.AddForce(forceDirection * accelerationForce * Time.deltaTime * 1.2f);
                //rb.velocity = (moveDirection * maxSpeed);
            }
            else if (status == "chaos")
            {
                if (rb.velocity.x == 0 || rb.velocity.y == 0)
                {
                    moveDirection = Random.onUnitSphere * maxSpeed;
                    statusTimer = 0;
                    maxTimer = 1f;
                    status = "chaos";
                }
                rb.velocity = moveDirection * maxSpeed * 1.2f;
            }
        }

        statusTimer += Time.deltaTime;

        if (statusTimer >= maxTimer)
        {
            if(status == "eating")
            {
                if(currentFood != null)
                {
                    hunger += 5;
                    PhotonNetwork.Destroy(currentFood);
                    currentFood = null;
                }
            }
            photonView.RPC("RandomizeAction", PhotonTargets.MasterClient);
            //photonView.RPC("RandomizeAction", PhotonTargets.AllViaClients);
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            CheckStatus();
        }

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    [PunRPC]
    private void RandomizeAction()
    {
        maxTimer = Random.Range(1.0f, 7.5f);
        statusTimer = 0.0f;

        // randomizes what the chicken is going to do next
        int randomStatus = Random.Range(0, 2);

        if (randomStatus == 0)
        {
            status = "idle";
        }
        else if (randomStatus == 1)
        {
            moveDirection = Random.onUnitSphere;
            status = "move";
        }
    }

    public void DangerDetected(Vector2 dangerDir)
    {
        if(status != "chaos")
        {
            maxTimer = 1.0f;
            status = "run";
            statusTimer = 0.0f;
            moveDirection = -dangerDir;
        }
    }

    public void FoodDetected(GameObject food)
    {
        Vector3 foodPos = food.transform.position;

        if (status != "run" && status != "chaos")
        {
            if (Vector3.Distance(transform.position, foodPos) > 0.25f)
            {
                status = "eat";
                maxTimer = 2f;
                statusTimer = 0.0f;
                moveDirection = (foodPos - transform.position).normalized;
            }
            else
            {
                if(!anim.GetBool("isEating"))
                {
                    maxTimer = Random.Range(2f, 5f);
                    statusTimer = 0.0f;
                    status = "eating";
                    anim.SetBool("isMoving", false);
                    anim.SetBool("isEating", true);
                    currentFood = food;
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canButcher = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canButcher = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Add the GameObject collided with to the list.
        currentCollisions.Add(col);
    }

    void OnCollisionExit2D(Collision2D col)
    {

        // Remove the GameObject collided with from the list.
        currentCollisions.Remove(col);
    }

    [PunRPC]
    private void SpawnEgg(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(egg.name, new Vector2(x, y + 0.001f), Quaternion.identity, 0, null);
    }

    [PunRPC]
    private void DropMeat(float x, float y)
    {
        PhotonNetwork.InstantiateSceneObject(raw.name, new Vector2(x, y), Quaternion.identity, 0, null);
    }

    [PunRPC]
    public void PreButcher()
    {
        butcherProcess = true;
    }

    // fate
    [PunRPC]
    public void Butcher()
    {
        bloodEffect.gameObject.SetActive(true);
        smokeEffect.gameObject.SetActive(true);
        bloodEffect.Play();
        smokeEffect.Play();
        isDead = true;
        sr.enabled = false;
        collider.isTrigger = true;
    }
}
