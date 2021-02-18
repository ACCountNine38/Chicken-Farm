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

        if(photonView.isMine)
        {
            chatInput.enabled = false;
        }
    }

    private void Update()
    {
        if(photonView.isMine)
        {
            float anchorX = chatInput.GetComponent<RectTransform>().anchoredPosition.x;
            float anchorY = chatInput.GetComponent<RectTransform>().anchoredPosition.y;

            if (chatInput.enabled && chatInput.GetComponent<RectTransform>().anchoredPosition.y < 18)
            {
                if(chatInput.GetComponent<RectTransform>().anchoredPosition.y > 18)
                {
                    chatInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorX, -18);
                }
                else if(chatInput.GetComponent<RectTransform>().anchoredPosition.y < 18)
                {
                    chatInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorX, anchorY + Time.deltaTime * 500);
                }
            }
            else if(!chatInput.enabled && chatInput.GetComponent<RectTransform>().anchoredPosition.y > -18)
            {
                if (chatInput.GetComponent<RectTransform>().anchoredPosition.y < -18)
                {
                    chatInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorX, -18);
                }
                else if (chatInput.GetComponent<RectTransform>().anchoredPosition.y > -18)
                {
                    chatInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchorX, anchorY - Time.deltaTime * 500);
                }
            }

            if(chatInput.enabled)
            {
                chatInput.Select();
                chatInput.ActivateInputField();
            }

            if(Input.GetKeyDown(KeyCode.Return) && !disable)
            {
                if (chatInput.enabled)
                {
                    if (chatInput.text != "" && chatInput.text.Length > 0)
                    {
                        photonView.RPC("SendMessage", PhotonTargets.AllBuffered, chatInput.text);
                        bubbleSpeech.SetActive(true);

                        chatInput.text = "";
                        disable = true;
                        chatInput.enabled = false;
                    }
                    else
                    {
                        chatInput.enabled = false;
                    }
                }
                else
                {
                    chatInput.enabled = true;
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

    public InputField GetChatInput()
    {
        return chatInput;
    }
}
