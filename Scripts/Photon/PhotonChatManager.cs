using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Security.Cryptography;
using UnityEngine.UI;
using TMPro;
using System;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{

    public ChatClient chatClient;
    bool isConnected;

    //activations removed from onbackbuttonclicked, and onbackbuttonclicked2 in photonlobby
    public GameObject usernameCanvas;
    public string username;
    //[SerializeField] string username;

    public GameObject chatPanel;
    string privateReceiver = "";
    string currentChat;
    public TMP_InputField chatField;
    public TextMeshProUGUI chatDisplay;

    public bool firstJoin;

    //flag variable for system messages
    public bool isSystemMessage;

    //should be the same as in photonlobby class
    public string roomName;

    //activate woodback at septimus scene
    public GameObject woodback;

    // Start is called before the first frame update
    void Start()
    {
        //chatClient = new ChatClient(this);
        firstJoin = false;
        isSystemMessage = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected == true)
        {
            chatClient.Service();
        }

        if (chatField.text != "" && Input.GetKey(KeyCode.Return))
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
        }
    }

    public void UserIDOnValueChange(string valueIn)
    {
        Debug.Log("valueIn is: " + valueIn);
        username = valueIn;
    }

    public void ChatConnect()
    {
        if (PhotonNetwork.OfflineMode == false)
        {
            isConnected = true;
        }
        chatClient = new ChatClient(this);

        //need to set specific room name here probably
        //roomName = PhotonRoom.room.roomName;
        //chatClient.Subscribe(new string[] { roomName });

        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
        //ChatConnect();
    }

    public void OnConnected()
    {
        //throw new System.NotImplementedException();
        Debug.Log("Connected to chat");
        if (PhotonNetwork.OfflineMode == false)
        {
            isConnected = true;
        }

        //should remove this canvas someplace else probably
        usernameCanvas.SetActive(false);

        //need to set specific room name here probably
        //roomName = GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().roomName;
        chatClient.Subscribe(new string[] { roomName });

        //chatClient.Subscribe(new string[] { "RegionChannel" });
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
        if (PhotonRoom.room.isGameLoaded == false)
        {
            chatClient.Unsubscribe(new string[] { roomName });
        }

        else
        {
            //consider later if this is good solution for the disconnect (of clients) when third scene is loaded
            Debug.Log("disconnected, trying reconnect");
            ChatConnect();
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        string msgs = "";

        if (isSystemMessage == false)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                msgs = string.Format("{0}: {1}", senders[i], messages[i]);

                chatDisplay.text += "\n " + msgs;

                Debug.Log(msgs);

                //play sfx
                GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayMessage();
            }
        }

        if (isSystemMessage == true)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                msgs = string.Format("{0}: {1}", "System", messages[i]);

                chatDisplay.text += "\n <color=green>" + msgs + "</color>";

                //reset the flag variable
                isSystemMessage = false;
            }
        }

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
        string msgs = "";

        if (isSystemMessage == false)
        {
            msgs = string.Format("(Private) {0}: {1}", sender, message);

            chatDisplay.text += "\n <color=#ff00ffff>" + msgs + "</color>";

            //play sfx
            GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayPrivateMessage();
        }
        if (isSystemMessage == true)
        {
            msgs = string.Format("(Hidden) {0}: {1}", "System", message);

            chatDisplay.text += "\n <color=#00fcffff>" + msgs + "</color>";

            //reset the flag variable
            isSystemMessage = false;
        }

        Debug.Log(msgs);
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();

        //Console.WriteLine("Status change for: {0} to: {1}", user, status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();

        //lets open chatpanel when scene 1 has finished loading instead
        //chatPanel.SetActive(true);
        //chatDisplay.text += "\n " + username + " has joined the room.";

        //show this message only once (since theres another rejoin on scene change)
        if (firstJoin == false)
        {
            //string msgs = username + " connected to chat.";
            //isSystemMessage = true;


            Invoke("SendSubscriptionMessage", 0.5f);
            /*
            string msgs = "<color=green>Connected to chat.</color>";
            chatClient.PublishMessage("RegionChannel", msgs);

            firstJoin = true;
            */
        }
        //Console.WriteLine("{0}: {1}", "RegionChannel", username, currentChat);
    }

    void SendSubscriptionMessage()
    {
        GetComponentInParent<PhotonRoom>().PV.RPC("RPC_FirstMessage", RpcTarget.AllBufferedViaServer, username);
    }

    void SendLeaveMessage()
    {
        GetComponentInParent<PhotonRoom>().PV.RPC("RPC_LeaveMessage", RpcTarget.AllBufferedViaServer, username);
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();

        SendLeaveMessage();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();

    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    //for changing text input field
    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    //for public messages
    public void SubmitPublicChatOnClick()
    {
        /*
        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene > 2)
        {
            //add "cheatcodes" here for now
            if (chatField.text == "portals")
            {
                GameManager.ins.references.foundWaypoints[0] = 1;
                GameManager.ins.references.foundWaypoints[1] = 2;
                GameManager.ins.references.foundWaypoints[2] = 3;
                GameManager.ins.references.foundWaypoints[3] = 4;
                GameManager.ins.references.foundWaypoints[4] = 5;

                //draw portal skill too
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 286, 1, 1);

                //give message
                string msgs = "All waypoints opened!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }

            if (chatField.text == "keys")
            {
                //draw 3 keystones
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 186, 5, 3);

                //give message
                string msgs = "Three Keystones gained!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }

            if (chatField.text == "wealth")
            {
                //gain 500 coins
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 500);

                //give message
                string msgs = "500 coins gained!";
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }

            if (chatField.text == "levels")
            {
                //get 10 levels
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();

                //give message
                string msgs = "Ten levels gained!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }
        }
        */

        if (privateReceiver == "")
        {
            //could add second chat channel later for public messages?
            //chatClient.PublishMessage("RegionChannel", currentChat);


            chatClient.PublishMessage(roomName, currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }

    //for private messages
    public void SubmitPrivateChatOnClick()
    {
        if (privateReceiver != "")
        {
            chatClient.SendPrivateMessage(privateReceiver, currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }

    public void ReceiverOnValueChange(string valueIn)
    {
        privateReceiver = valueIn;
    }

    /*sets isSystemMessage variable for all players
    [PunRPC]
    void RPC_FirstMessage()
    {
        GameObject.Find("ChatManager").GetComponent<PhotonChatManager>().isSystemMessage = true;

        if (PV.IsMine == true)
        {
            string msgs = "<color=green>Connected to chat.</color>";
            chatClient.PublishMessage("RegionChannel", msgs);

            firstJoin = true;
        }
    }
    */

}
