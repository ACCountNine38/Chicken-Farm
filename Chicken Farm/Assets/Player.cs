using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{
    public PhotonView photonView;
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject PlayerCamera;
    public SpriteRenderer sr;
    public Text PlayerNameText;

    public float MoveSpeed;
    private Vector2 moveDirection;

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        // checks if the current client is this device
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.playerName;
        }
        else
        {
            PlayerNameText.text = photonView.owner.name;
            PlayerNameText.color = Color.yellow;
        }
    }

    // update is default function that is called every frame
    private void Update()
    {
        // if the current client is this device, then checks for user input
        if (photonView.isMine)
        {
            CheckInput();
        }
    }

    // called a set amount of times per tick, this is where all the physics happens for consistency
    private void FixedUpdate()
    {
        Move();
    }

    // method for user input
    private void CheckInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        // keyboard controls
        if (Input.GetKeyDown(KeyCode.A))
        {
            photonView.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            photonView.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }

        // animation updates
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isRunning", true);
        }

        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    // method that calcualtes and moves the player
    private void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * MoveSpeed, moveDirection.y * MoveSpeed);
    }

    // photon methods that are used to sync on different devices
    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
    }

    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
    }
}