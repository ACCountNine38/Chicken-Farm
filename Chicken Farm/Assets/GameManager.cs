using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player PlayerPrefab;
    public GameObject GameCanvas;
    //public GameObject SceneCamera;
    public Text PingText;

    public GameObject OptionsMenu;
    public GameObject PlayerFeed;
    public GameObject FeedGrid;

    private bool options = false;

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        GameCanvas.SetActive(true);
    }

    // update is default function that is called every frame
    private void Update()
    {
        CheckInput();
        PingText.text = "Ping: " + PhotonNetwork.GetPing();
    }

    private void CheckInput()
    {
        ActivateOptions();
    }

    public void ActivateOptions()
    {
        if (options && Input.GetKeyDown(KeyCode.Escape))
        {
            OptionsMenu.SetActive(false);
            options = false;
        }
        else if (!options && Input.GetKeyDown(KeyCode.Escape))
        {
            OptionsMenu.SetActive(true);
            options = true;
        }

    }

    // method that creates a new Player Prefab
    public void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity, 0);

        GameCanvas.SetActive(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Main Menu");
    }

    private void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        GameObject feed = Instantiate(PlayerFeed, new Vector2(0, 0), Quaternion.identity);
        feed.transform.SetParent(FeedGrid.transform, false);
        feed.GetComponent<Text>().text = player.name + " has joined the game";
        feed.GetComponent<Text>().color = Color.yellow;
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        GameObject feed = Instantiate(PlayerFeed, new Vector2(0, 0), Quaternion.identity);
        feed.transform.SetParent(FeedGrid.transform, false);
        feed.GetComponent<Text>().text = player.name + " has left the game";
        feed.GetComponent<Text>().color = Color.red;
    }
}
