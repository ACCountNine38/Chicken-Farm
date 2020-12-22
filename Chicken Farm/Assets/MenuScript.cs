using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    // variables that are displayed on the Inspector
    [SerializeField] private string version = "0.1";
    [SerializeField] private GameObject Welcome_Menu;
    [SerializeField] private GameObject Welcome_Menu_items; // does not contain character preview items

    [SerializeField] private GameObject Character_Preview_Section;
    [SerializeField] private GameObject customize_Button;

    [SerializeField] private GameObject Option_Menu;
    [SerializeField] private GameObject option_Button;

    [SerializeField] private GameObject Create_Enter_Menu;
    [SerializeField] private GameObject Start_Button;

    [SerializeField] private InputField UsernameInput;
    [SerializeField] private InputField CreateGameInput;
    [SerializeField] private InputField JoinGameInput;
    [SerializeField] private GameObject username_Invalid_Waring;
    [SerializeField] private GameObject roomname_Invalid_Waring;

    [SerializeField] private GameObject facial_section;
    [SerializeField] private GameObject hats_section;
    [SerializeField] private GameObject suit_section;

    [SerializeField] private Image facial_image;
    [SerializeField] private Image hats_image;
    [SerializeField] private Image suit_image;

    private bool customize_on = false;
    private bool option_on = false;
    private bool start_on = false;

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(version);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Welcome_Menu.SetActive(true);
    }

    // OnConnectedToMaster() is a built in function in photon, this method logs the connection for each user
    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    // Update is called once per frame
    private void Update()
    {

        UpdateCustomizeCharacter();
        UpdateOption();
        UpdateStart();
        UpdateUserNameValidation();

    }

    // method that controls actions
    public void change_status_customize()
    {
        customize_on = !customize_on;

    }

    // method that controls actions
    public void change_status_option()
    {
        option_on = !option_on;

    }

    // method that controls actions
    public void change_status_start()
    {
        start_on = !start_on;

    }

    // turn to Create_Character_Menu with slide effect
    private void UpdateCustomizeCharacter()
    {

        //if (customize_on)
        //{
        //    customize_Button.GetComponentInChildren<Text>().text = "Finished";

        //    if (Character_Preview_Section.transform.localPosition.x > 0)
        //    {
        //        Character_Preview_Section.transform.localPosition = new Vector3(Character_Preview_Section.transform.localPosition.x - 20, Character_Preview_Section.transform.localPosition.y);
        //        Welcome_Menu_items.transform.localPosition = new Vector3(Welcome_Menu_items.transform.localPosition.x - 50, Welcome_Menu_items.transform.localPosition.y);
        //    }

        //}
        //else
        //{
        //    customize_Button.GetComponentInChildren<Text>().text = "Customize";

        //    if (Character_Preview_Section.transform.localPosition.x < 1920 * 0.43)
        //    {
        //        Character_Preview_Section.transform.localPosition = new Vector3(Character_Preview_Section.transform.localPosition.x + 20, Character_Preview_Section.transform.localPosition.y);
        //        Welcome_Menu_items.transform.localPosition = new Vector3(Welcome_Menu_items.transform.localPosition.x + 50, Welcome_Menu_items.transform.localPosition.y);
        //    }
        //}
    }

    // turn to Create_Character_Menu with  slide effect
    private void UpdateOption()
    {

        //if (option_on)
        //{
        //    if (Option_Menu.transform.localPosition.y < 0)
        //    {
        //        Option_Menu.transform.localPosition = new Vector3(Option_Menu.transform.localPosition.x, Option_Menu.transform.localPosition.y + 20);
        //        Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x, Welcome_Menu.transform.localPosition.y + 20);
        //    }

        //}
        //else
        //{

        //    if (Option_Menu.transform.localPosition.y > -1080)
        //    {
        //        Option_Menu.transform.localPosition = new Vector3(Option_Menu.transform.localPosition.x, Option_Menu.transform.localPosition.y - 20);
        //        Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x, Welcome_Menu.transform.localPosition.y - 20);
        //    }
        //}
    }


    // turn to Create_Enter_Menu with slide effect
    private void UpdateStart()
    {

        if (start_on)
        {
            if (UsernameInput.text.Length >= 1)
            {
                if (Welcome_Menu.transform.localPosition.x < 800)
                {
                    Create_Enter_Menu.transform.localPosition = new Vector3(Create_Enter_Menu.transform.localPosition.x + 20, Create_Enter_Menu.transform.localPosition.y);
                    Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x + 20, Welcome_Menu.transform.localPosition.y);
                }
            }
            else
            {
                username_Invalid_Waring.GetComponent<Text>().color = Color.yellow;
            }


        }
        else
        {
            if (Welcome_Menu.transform.localPosition.x > 0)
            {
                Create_Enter_Menu.transform.localPosition = new Vector3(Create_Enter_Menu.transform.localPosition.x - 20, Create_Enter_Menu.transform.localPosition.y);
                Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x - 20, Welcome_Menu.transform.localPosition.y);
            }
        }
    }

    // update if player entered a valid user name
    private void UpdateUserNameValidation()
    {
        if (UsernameInput.text.Length >= 1)
        {
            username_Invalid_Waring.SetActive(false);
        }
        else
        {
            username_Invalid_Waring.SetActive(true);
        }
    }

    // check the name input is vaild
    private bool checkCreateRoomNameValidation()
    {
        if (CreateGameInput.text.Length >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // check the name input is vaild
    private bool checkEnterRoomNameValidation()
    {
        if (JoinGameInput.text.Length >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // method that sets the username of the player
    public void SetUsername()
    {
        PhotonNetwork.playerName = UsernameInput.text;
    }

    // method that enables the user to host, given the server ip
    public void CreateGame()
    {
        if (checkCreateRoomNameValidation())
        {
            roomname_Invalid_Waring.SetActive(false);
            PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { maxPlayers = 5 }, null);
        }
        else
        {
            roomname_Invalid_Waring.SetActive(true);
        }

    }

    // method that enables the user to join a room
    public void JoinGame()
    {
        if (checkEnterRoomNameValidation())
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.maxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text, roomOptions, TypedLobby.Default);
        }
        else
        {
            roomname_Invalid_Waring.SetActive(true);
        }
    }

    // Photon built in function, loads the game scene after joining a host
    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void changeInventory(GameObject a)
    {
        facial_section.SetActive(false);
        hats_section.SetActive(false);
        suit_section.SetActive(false);

        a.SetActive(true);
    }

    public void setCharacterImage(int id)
    {
        Sprite[] sprite = Resources.LoadAll<Sprite>("facials");

        facial_image.sprite = sprite[id];
    }

}
