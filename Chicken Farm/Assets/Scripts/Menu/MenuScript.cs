using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    // variables that are displayed on the Inspector
    [SerializeField] private string version = "0.1";
    [SerializeField] private int golbalCurrencyNumber= 0;
    [SerializeField] private GameObject Welcome_Menu;
    [SerializeField] private GameObject Welcome_Menu_items; // does not contain character preview items

    [SerializeField] private GameObject CurrencyControl_items;

    [SerializeField] private GameObject Character_Preview_Section;
    [SerializeField] private GameObject customize_Button;

    [SerializeField] private GameObject Option_Menu;
    [SerializeField] private GameObject option_Button;

    [SerializeField] private GameObject Create_Enter_Menu;
    [SerializeField] private GameObject Start_Button;

    [SerializeField] private InputField UsernameInput;
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

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private GameObject masterSlider;
    [SerializeField] private GameObject musicSlider;
    [SerializeField] private GameObject sfxSlider;

    public static float masterVolume = 0.5f;
    public static float musicVolume = 0.5f;
    public static float sfxVolume = 0.2f;

    [SerializeField] private int facial_id = 0;
    [SerializeField] private int hat_id = 0;
    [SerializeField] private int suit_id = 0;

    private int screen_size_height = 1080;
    private int screen_size_width = 1920;
    private int MAX_LENGTH_ID = 9; // This is ONLY for randonization of ID

    // Awake() is called when photon network is initiated
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(version);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Welcome_Menu.SetActive(true);

        //initialize variables
        facial_id = 0;
        hat_id = 0;
        suit_id = 0;
        masterVolume = 0.5f;
        musicVolume = 0.5f;
        sfxVolume = 0.5f;
        customize_on = false;
        option_on = false;
        start_on = false;
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
        UpdateCoin();
        UpdateCustomizeCharacter();
        UpdateOption();
        UpdateStart();
        UpdateUserNameValidation();
        UpdateVolume();

    }

    private void UpdateCoin()
    {
        CurrencyControl_items.GetComponentInChildren<Text>().text = golbalCurrencyNumber.ToString();
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

        if (customize_on)
        {
            customize_Button.GetComponentInChildren<Text>().text = "Finished";

            if (Character_Preview_Section.transform.localPosition.y > 0)
            {
                Character_Preview_Section.transform.localPosition = new Vector3(Character_Preview_Section.transform.localPosition.x, Character_Preview_Section.transform.localPosition.y-20);
                Welcome_Menu_items.transform.localPosition = new Vector3(Welcome_Menu_items.transform.localPosition.x, Welcome_Menu_items.transform.localPosition.y + 20);
            }
            else
            {
                Character_Preview_Section.transform.localPosition = new Vector3(Character_Preview_Section.transform.localPosition.x, 0);
                Welcome_Menu_items.transform.localPosition = new Vector3(Welcome_Menu_items.transform.localPosition.x, 2 * screen_size_height);
            }


        }
        else
        {
            customize_Button.GetComponentInChildren<Text>().text = "Customize";

            if (Character_Preview_Section.transform.localPosition.y < screen_size_height)
            {
                Character_Preview_Section.transform.localPosition = new Vector3(Character_Preview_Section.transform.localPosition.x, Character_Preview_Section.transform.localPosition.y + 20);
                Welcome_Menu_items.transform.localPosition = new Vector3(Welcome_Menu_items.transform.localPosition.x, Welcome_Menu_items.transform.localPosition.y - 20);
            }
            else
            {
                Character_Preview_Section.transform.localPosition = new Vector3(Character_Preview_Section.transform.localPosition.x, screen_size_height);
                Welcome_Menu_items.transform.localPosition = new Vector3(Welcome_Menu_items.transform.localPosition.x, 0);
            }
        }
    }

    // turn to Create_Character_Menu with  slide effect
    private void UpdateOption()
    {

        if (option_on)
        {
            if (Option_Menu.transform.localPosition.y < 0)
            {
                Option_Menu.transform.localPosition = new Vector3(Option_Menu.transform.localPosition.x, Option_Menu.transform.localPosition.y + 20);
                Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x, Welcome_Menu.transform.localPosition.y + 20);
            }
            else
            {
                Option_Menu.transform.localPosition = new Vector3(Option_Menu.transform.localPosition.x, 0);
                Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x, 2 * screen_size_height);
            }

        }
        else
        {

            if (Option_Menu.transform.localPosition.y > 2 * -screen_size_height)
            {
                Option_Menu.transform.localPosition = new Vector3(Option_Menu.transform.localPosition.x, Option_Menu.transform.localPosition.y - 20);
                Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x, Welcome_Menu.transform.localPosition.y - 20);
            }
            else
            {
                Option_Menu.transform.localPosition = new Vector3(Option_Menu.transform.localPosition.x, 2 * -screen_size_height);
                Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x, 0);
            }
        }
    }

    // turn to Create_Enter_Menu with slide effect
    private void UpdateStart()
    {

        if (start_on)
        {
            if (UsernameInput.text.Length >= 1)
            {
                if (Welcome_Menu.transform.localPosition.x < screen_size_width)
                {
                    Create_Enter_Menu.transform.localPosition = new Vector3(Create_Enter_Menu.transform.localPosition.x + 20, Create_Enter_Menu.transform.localPosition.y);
                    Welcome_Menu.transform.localPosition = new Vector3(Welcome_Menu.transform.localPosition.x + 20, Welcome_Menu.transform.localPosition.y);
                }
            }
            else
            {
                start_on = false;
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

    //method that create a string of MAX_LENGTH_ID digits with random characters from 0-9 and a-Z
    private string randomID()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var nums = "0123456789";
        var stringChars = new char[MAX_LENGTH_ID];

        for (int i = 0; i < MAX_LENGTH_ID; i++)
        {
            stringChars[i] = Random.Range(0, 2) == 1 ? chars[Random.Range(0, chars.Length)] : nums[Random.Range(0, nums.Length)];
            
        }

        var finalString = new string(stringChars);

        return finalString;
    }

    public void CreateGame()
    {
        string roomId = randomID();
        Debug.Log(roomId);
        roomname_Invalid_Waring.SetActive(false);
        PhotonNetwork.CreateRoom(roomId, new RoomOptions() { maxPlayers = 5 }, null);
    }

    // method that enables create a single-player game
    public void SinglePlayerGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 1;
        PhotonNetwork.CreateRoom(randomID(), roomOptions, TypedLobby.Default);
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

    public void setFacialImage(int id)
    {
        facial_id = id;

        Sprite[] sprite = Resources.LoadAll<Sprite>("Icons/facials");

        facial_image.sprite = sprite[facial_id];

    }

    public void setHatImage(int id)
    {
        hat_id = id;

        Sprite[] sprite = Resources.LoadAll<Sprite>("Icons/hats");

        hats_image.sprite = sprite[hat_id];

    }

    public void setSuitImage(int id)
    {
        suit_id = id;

        Sprite[] sprite = Resources.LoadAll<Sprite>("Icons/suits");

        suit_image.sprite = sprite[suit_id];
    }

    public void setRandomCharacterImages()
    {
        Sprite[] sprite1 = Resources.LoadAll<Sprite>("Icons/facials");
        Sprite[] sprite2 = Resources.LoadAll<Sprite>("Icons/hats");
        Sprite[] sprite3 = Resources.LoadAll<Sprite>("Icons/suits");

        int facial_new_id = Random.Range(0, sprite1.Length - 1);
        int hats_new_id = Random.Range(0, sprite2.Length - 1);
        int suits_new_id = Random.Range(0, sprite3.Length - 1);

        setFacialImage(facial_new_id);
        setHatImage(hats_new_id);
        setSuitImage(suits_new_id);

    }

    private void UpdateVolume()
    {
        musicAudioSource.volume = musicVolume * masterVolume;
        sfxAudioSource.volume = sfxVolume * masterVolume;
    }

    public void updateALLVolume()
    {
        masterVolume = masterSlider.GetComponent<Slider>().value;
        musicVolume = musicSlider.GetComponent<Slider>().value;
        sfxVolume = sfxSlider.GetComponent<Slider>().value;
    }

    public void playMenusfx()
    {
        sfxAudioSource.Play();
    }

}
