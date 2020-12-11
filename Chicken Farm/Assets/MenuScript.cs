using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    // variables that are displayed on the Inspector
	[SerializeField] private string version = "0.1";
	[SerializeField] private GameObject UsernameMenu;
	[SerializeField] private GameObject ConnectPanel;

	[SerializeField] private InputField UsernameInput;
	[SerializeField] private InputField CreateGameInput;
	[SerializeField] private InputField JoinGameInput;

	[SerializeField] private GameObject StartButton;

    // Awake() is called when photon network is initiated
    private void Awake() {
		PhotonNetwork.ConnectUsingSettings(version);
	}

    // Start is called before the first frame update
    private void Start() {
        UsernameMenu.SetActive(true);
    }

    // OnConnectedToMaster() is a built in function in photon, this method logs the connection for each user
    private void OnConnectedToMaster() {
		PhotonNetwork.JoinLobby(TypedLobby.Default);
		Debug.Log("Connected");
	}

    // Update is called once per frame
    private void Update() {
        
    }

    // Username validation
    public void ChangeUsernameInput() {
    	if(UsernameInput.text.Length >= 1) {
    		StartButton.SetActive(true);
    	} else {
    		StartButton.SetActive(false);
    	}
    }

    // method that sets the username of the player
    public void SetUsername() {
    	UsernameMenu.SetActive(false);
    	PhotonNetwork.playerName = UsernameInput.text;
    }

    // method that enables the user to host, given the server ip
    public void CreateGame() {
        PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() {maxPlayers = 5}, null);
    }

    // method that enables the user to join a room
    public void JoinGame() {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text, roomOptions, TypedLobby.Default);
    }

    // Photon built in function, loads the game scene after joining a host
    private void OnJoinedRoom() {
        PhotonNetwork.LoadLevel("Game");
    }
}
