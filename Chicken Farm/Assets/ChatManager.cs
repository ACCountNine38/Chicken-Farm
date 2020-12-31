using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public PhotonView photonView;
    public GameObject bubbleSpeech;
    public Text newText;

    private InputField chatInput;
    private bool disable;

    private void Awake()
    {
        chatInput = GameObject.Find("ChatInput").GetComponent<InputField>();
    }

    private void Update()
    {
        if(photonView.isMine)
        {
            if(!disable && chatInput.isFocused)
            {
                if(chatInput.text != "" && chatInput.text.Length > 0 && Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    photonView.RPC("SendMessage", PhotonTargets.AllBuffered, chatInput.text);
                    bubbleSpeech.SetActive(true);

                    chatInput.text = "";
                    disable = true;
                }
            }
        }
    }

    [PunRPC]
    private void SendMessage(string message)
    {
        newText.text = message;

        // waits until Remove() can be executed
        StartCoroutine("Remove");
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        bubbleSpeech.SetActive(false);
        disable = false;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(bubbleSpeech.active);
        }
        else if(stream.isReading)
        {
            bubbleSpeech.SetActive((bool)stream.ReceiveNext());
        }
    }
}
