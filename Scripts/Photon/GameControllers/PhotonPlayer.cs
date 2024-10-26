using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    //public int characterValue;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();

        //Invoke("CreateAvatarWithDelay", 2.0f);

        //dont spawn new avatar on reloads
        if (PV.IsMine && PhotonRoom.room.currentScene == 3 && PhotonRoom.room.spContinue == false)
        {
            //first command specifies the method, all buffered makes clients joining later still get the info, overload comes last
            //PV.RPC("RPC_PlaceAvatar", RpcTarget.AllBuffered, SelectorScript2.SS.index);

            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                transform.position, transform.rotation, 0);

            //move the chat display to correct position
            Vector3 displayPos = GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.transform.position;
            //displayPos.y = GameObject.Find("Console Borders").transform.position.y;
            displayPos.y = GameManager.ins.consoleBorders.transform.position.y;
            displayPos.x = GameManager.ins.consoleBorders.transform.position.x;
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.transform.position = displayPos;

            //hide chatpanel, but activate the "woodback"
            GameManager.ins.uiButtonHandler.consoleFrame.SetActive(false);
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().woodback.SetActive(true);
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.gameObject.SetActive(false);

            //GameManager.ins.uiButtonHandler.HandCardsButtonPressed();

            /*
            //spawn bots
            if (PhotonRoom.room.aiNumber > 0)
            {
                Invoke("SpawnBot", 2.0f);
            }
            if (PhotonRoom.room.aiNumber > 1)
            {
                Invoke("SpawnBot", 3.0f);
            }
            if (PhotonRoom.room.aiNumber > 2)
            {
                Invoke("SpawnBot", 4.0f);
            }

            //PV.RPC("RPC_PlaceAvatar", RpcTarget.AllBufferedViaServer, myAvatar);

            //GameManager.ins.AddPlayer(myAvatar);
            
            //old test
            //GameObject test = GameObject.FindWithTag("Respawn").GetComponent<Node>().piece;
            GameObject test = GameManager.ins.piece;
            myAvatar = test;
            

            GameManager.ins.startingNode.Arrive(myAvatar);
            Debug.Log("Arrived at: " + GameManager.ins.startingNode);
            GameManager.ins.AddPlayer(myAvatar);
            */
        }

        //spawn the old avatar, previously saved?
        if (PV.IsMine && PhotonRoom.room.currentScene == 3 && PhotonRoom.room.spContinue == true)
        {
            //first command specifies the method, all buffered makes clients joining later still get the info, overload comes last
            //PV.RPC("RPC_PlaceAvatar", RpcTarget.AllBuffered, SelectorScript2.SS.index);

            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                transform.position, transform.rotation, 0);

            //move the chat display to correct position
            Vector3 displayPos = GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.transform.position;
            //displayPos.y = GameObject.Find("Console Borders").transform.position.y;
            displayPos.y = GameManager.ins.consoleBorders.transform.position.y;
            displayPos.x = GameManager.ins.consoleBorders.transform.position.x;
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.transform.position = displayPos;

            //hide chatpanel, but activate the "woodback"
            GameManager.ins.uiButtonHandler.consoleFrame.SetActive(false);
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().woodback.SetActive(true);
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.gameObject.SetActive(false);
        }


    }

    void CreateAvatarWithDelay()
    {
        
    }

    //for spawning bots
    void SpawnBot()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), transform.position, transform.rotation, 0);
    }

    /*pointless test
    [PunRPC]
    void RPC_PlaceAvatar(GameObject avatar)
    {
        GameManager.ins.AddPlayer(avatar);
    }
    */
}
