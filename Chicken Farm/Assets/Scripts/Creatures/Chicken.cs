using UnityEngine;
using UnityEngine.UI;

public class Chicken : Creature
{
    // base chicken status
    public float hunger, eggCooldown;
    public int type;
    public bool isDead, canButcher;

    public BoxCollider2D collider;
    public ParticleSystem bloodEffect;
    public ParticleSystem smokeEffect;
    public GameObject egg, raw;

    // variables for this chicken if it is named
    public bool isNamed, butcherProcess;
    public Text nametag;

    private float butcherTimer, eggTimer, randomHunger;

    public Vector3 dangerPos;

    public void Awake()
    {
        eggCooldown = 100;
        original = sr.color;
        randomHunger = Random.Range(0, 100);
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
            if(PhotonNetwork.isMasterClient)
            {
                Move();
            }
            
            //photonView.RPC("UpdateType", PhotonTargets.MasterClient);

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
            if(!bloodEffect.isPlaying && !smokeEffect.isPlaying)
            {
                Die();
            }
        }
    }

    [PunRPC]
    private void UpdateType()
    {
        // thin chicken
        if(hunger <= 33)
        {
            type = 1;
            anim.SetInteger("type", 1);
            speed = 2;
            eggCooldown = 100;
        }
        // normal chicken
        else if(hunger <= 66)
        {
            type = 0;
            anim.SetInteger("type", 0);
            speed = 2.5f;
            eggCooldown = 30;
        }
        // thicc chicken
        else
        {
            type = 2;
            anim.SetInteger("type", 2);
            speed = 1.5f;
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

    // method that calcualtes and moves the player
    private void Move()
    {
        CheckStatus();

        // animation updates
        if (Mathf.Abs(rb.velocity.x) > 0.1 || Mathf.Abs(rb.velocity.y) > 0.1)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    private void CheckStatus()
    {
        if (status == "idle")
        {
            LayEgg();
            statusTimer += Time.deltaTime;
            if (statusTimer >= maxTimer)
            {
                photonView.RPC("RandomizeAction", PhotonTargets.MasterClient);
            }
        }
        else if (status == "move" || status == "run")
        {
            statusTimer += Time.deltaTime;

            if (direction == 1 && rb.velocity.x < -0.5)
            {
                direction = 0;
                photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
            }
            else if (direction == 0 && rb.velocity.x > 0.5)
            {
                direction = 1;
                photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
            }

            if (status == "move")
            {
                rb.velocity = new Vector2(randomDirection.x * speed, randomDirection.y * speed);
            }
            else if (status == "run")
            {
                Vector3 runDirection = (rb.transform.position - dangerPos).normalized;
                rb.velocity = new Vector2(runDirection.x * (speed + 1), runDirection.y * (speed + 1));
            }

            if (statusTimer >= maxTimer)
            {
                photonView.RPC("RandomizeAction", PhotonTargets.MasterClient);
            }
        }
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
            randomDirection = Random.onUnitSphere;
            status = "move";
        }
    }

    public void DangerDetected(Collider2D collision)
    {
        maxTimer = 1.0f;
        status = "run";
        statusTimer = 0.0f;
        dangerPos = collision.gameObject.GetComponent<Player>().transform.position;
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
