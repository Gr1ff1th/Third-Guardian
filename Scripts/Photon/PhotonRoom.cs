using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.Audio;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //room info
    public static PhotonRoom room;
    public PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    //need separate flag variable for main scene loaded, for load handling at least
    public bool mainSceneLoaded;

    //public int multiplayerScene;

    //player info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    //for charselection purposes
    public int playersInGame;
    public bool hasSelectedCharacter;

    //delayed start
    //note that "maxplayers" variables are for timer purposes only
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart;

    // flag variable for chat started
    public bool chatStarted = false;

    //game duration in days, only host can set this
    public int gameDuration;

    //for single player games & tutorial
    public int aiNumber;
    public bool isPractice;
    public bool isTutorial;

    //new setting variables for v92
    public int startLocation;
    public int startDifficulty;
    public int startCoins;
    public int scoreModifier;
    public int mapType;

    //for menu music
    public AudioSource titleMusic;

    //keep track of roomname (needed for photonchatmanager at least)
    public string roomName;

    //flag to tell whether were taking from reload (on single player)
    public bool spContinue;

    //save these here, only used for new games
    public int background1;
    public int background2;
    public int background3;


    //reference of character selection screen camera
    //public GameObject SecondScreenCamera;

    private void Awake()
    {
        //dunno if this is needed here, since its alrdy done at photonlobby
        //PhotonNetwork.AutomaticallySyncScene = true;

        //set up singleton
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
            //roomName = PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            if(PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
                //roomName = PhotonNetwork.CurrentRoom.Name;
            }
        }
        DontDestroyOnLoad(this.gameObject);

        gameDuration = 3;
        //for DEMO
        //gameDuration = 1;

        aiNumber = 0;
        isPractice = false;
        isTutorial = false;
        hasSelectedCharacter = false;

        spContinue = false;

        //titleMusic.Play();

        mainSceneLoaded = false;
    }

    public override void OnEnable()
    {
        //subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        
        //event listener?
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        //subscribe to functions
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }
   
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;

        //these are timers (not really functioning atm?)
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 5;
        timeToStart = startingTime;

        chatStarted = false;
    }
   
    //dunno if theres any purpose for this atm
    void Update()
    {
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if(playersInRoom == 1)
            {
                RestartTimer();
            }
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart = atMaxPlayers;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                Debug.Log("Display time to start to the players " + timeToStart);

                if(timeToStart <= 0)
                {
                    //this doesnt work anyway? 
                    StartGame();
                }
            }
        }
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();
        roomName = PhotonNetwork.CurrentRoom.Name;

        //this is never true atm
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            //need to replace this later to show the player numbers in game
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");

            if(playersInRoom > 1)
            {
                readyToCount = true;
            }
            if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                //closes the room so no new players can join it
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            //game shouldnt start from here
            StartGame();
        }
        /*
        if (!PhotonNetwork.IsMasterClient)
            return;

        StartGame();
        */
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has entered room");
        //update playerlist
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

        //this will synchronize loaded level between all clients (dunno if this should be used?)
        //PhotonNetwork.AutomaticallySyncScene = true;

        //for delayed start
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                //closes the room so no new players can join it
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

        if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
        {
            /*
            readyToStart = true;
            if (!PhotonNetwork.IsMasterClient)
                return;
                */
            //closes the room so no new players can join it
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //test
            Debug.Log("Room becomes closed");
        }
    }

    //lets try this
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        base.OnPlayerLeftRoom(newPlayer);

        Debug.Log("A player has left room");
        //update playerlist
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom--;

        //give message
        string msgs = "Player has left chat.";
        GameObject.Find("ChatManager").GetComponent<PhotonChatManager>().chatDisplay.text += "\n <color=green>" + msgs + "</color>";

        //only do these on char selection scene
        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene == 2)
        {
            //reset this
            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().numberOfCharsChosen = 0;

            //check again how many players have selected char, and start game if it equals number of players
            //PV.RPC("RPC_CountSelectedPlayers", RpcTarget.AllBufferedViaServer, hasSelectedCharacter);
            Invoke("CountSelectedPlayers", 0.5f);

            //string msgs = username + " connected to chat.";
            //GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=green> System: " + msgs + "</color>";

            if (playersInRoom < MultiplayerSettings.multiplayerSettings.maxPlayers && isGameLoaded != false)
            {
                /*
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                    */
                //closes the room so no new players can join it
                PhotonNetwork.CurrentRoom.IsOpen = true;
                //test
                Debug.Log("Room becomes open");
            }

            Invoke("StartIfAllChosen", 1.5f);
        }
    }

    void CountSelectedPlayers()
    {
        for (int i = 0; i < GameObject.Find("CharacterList").GetComponent<SelectorScript2>().characterList.Length; i++)
        {
            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().characterList[i].GetComponent<Character>().isSelected = false;
        }

        PV.RPC("RPC_CountSelectedPlayers", RpcTarget.AllBufferedViaServer, hasSelectedCharacter, PlayerInfo.PI.mySelectedCharacter);
    }

    //for counting how many players have selected
    //lso deactivates all the select buttons of selected characters again
    [PunRPC]
    void RPC_CountSelectedPlayers(bool hasSC, int mySelectedChar)
    {
        if(hasSC == true)
        {
            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().numberOfCharsChosen += 1;
            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().characterList[mySelectedChar].GetComponent<Character>().isSelected = true;
        }

    }

    void StartIfAllChosen()
    {
        //host starts game if all have chosen char
        if (playersInRoom == GameObject.Find("CharacterList").GetComponent<SelectorScript2>().numberOfCharsChosen)
        {
            //close room
            PhotonNetwork.CurrentRoom.IsOpen = false;

            //activate loading panel
            //loadingScreenPanel.SetActive(true);

            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().leaveGameButton.interactable = false;

            if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().myNumberInRoom == 1)
            {
                //give message
                string msgs2 = "All players have selected. Loading main scene..";
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs2);

                Invoke("ChangeScene", 0.6f);
                //ChangeScene();

            }
        }
    }

    public void ChangeScene()
    {
        //could add check here later to make sure that all players have selected character

        room.StartGame();
    }

    public void StartGame()
    {
        isGameLoaded = true;
        Debug.Log("Loading level");

        if (chatStarted == false)
        {
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().roomName = roomName;

            //new system: start chatroom
            GetComponentInChildren<PhotonChatManager>().ChatConnect();

            chatStarted = true;
        }

        /* removed in v96
         * if not masterclient
        if (!PhotonNetwork.IsMasterClient)
            return;
        */
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        //dunno what that delaystart is for anymore
        //PhotonNetwork.CurrentRoom.IsOpen = false;

        //this seems like bs
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);

        //lets try this now
        //PhotonNetwork.LoadLevel(2);



        //MultiplayerSettings.multiplayerSettings.multiplayerScene++;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //called when multiplayer scene is loaded
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;
            if (MultiplayerSettings.multiplayerSettings.delayStart || PhotonNetwork.OfflineMode == false)
            {
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                //weird rpc call? (this isnt called atm though?)
                RPC_CreatePlayer();
                //CreatePlayer();
            }
            MultiplayerSettings.multiplayerSettings.multiplayerScene++;
        }

        if(currentScene == 2)
        {
            //open this here instead
            //actually lets not on v92
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.SetActive(false);
            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(false);

            //deactivate these for now
            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().aiDisplay.SetActive(false);
            GameObject.Find("CharacterList").GetComponent<SelectorScript2>().durationDisplay.SetActive(false);

            //lets put this on onjoinedroom method
            //roomName = PhotonNetwork.CurrentRoom.Name;

            /* maybe activate duration display later for mp
             * 
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("CharacterList").GetComponent<SelectorScript2>().durationDisplay.SetActive(true);
                
                if (MultiplayerSettings.multiplayerSettings.maxPlayers == 1)//(PhotonNetwork.OfflineMode == true)
                {
                    GameObject.Find("CharacterList").GetComponent<SelectorScript2>().aiDisplay.SetActive(true);

                    if(GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isTutorial == true)
                    {
                        GameObject.Find("CharacterList").GetComponent<SelectorScript2>().durationDisplay.SetActive(false);
                        GameObject.Find("CharacterList").GetComponent<SelectorScript2>().aiDisplay.SetActive(false);
                    }
                }
            }
            */

            //lets try this
            //SecondScreenCamera = GameObject.Find("Main Camera").gameObject;
            //DontDestroyOnLoad(SecondScreenCamera.gameObject);
        }

        //sets the game lengths for all players
        if (currentScene == 3)
        {
            titleMusic.Stop();

            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.ins.SetGameLength(gameDuration);
            }

            //Destroy(SecondScreenCamera.gameObject);
        }
    }

    //not needed anymore
    void SetGameLength()
    {
        Debug.Log("Setting game length");
        //GameManager.ins.SetGameLength(gameDuration);
        //GameObject.Find("Game Manager").GetComponent<GameManager>().SetGameLength(gameDuration);
    }

    //not used?
    [PunRPC]
    private void CreatePlayer()
    {
        //creates player network controller but not player character
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    void RestartTimer()
    {
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        //to make sure not to make duplicate player objects
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    //sets isSystemMessage variable for all players
    [PunRPC]
    void RPC_FirstMessage(string username)
    {
        /*
        GameObject.Find("ChatManager").GetComponent<PhotonChatManager>().isSystemMessage = true;

        if (PV.IsMine == true)
        {
            string msgs = username + " connected to chat.";
            GetComponentInChildren<PhotonChatManager>().chatClient.PublishMessage("RegionChannel", msgs);

            GetComponentInChildren<PhotonChatManager>().firstJoin = true;
        }
        */
        string msgs = username + " connected to chat.";
        GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=green> System: " + msgs + "</color>";

        if (PV.IsMine == true)
        {
            GetComponentInChildren<PhotonChatManager>().firstJoin = true;
        }
    }

    //sets isSystemMessage variable for all players
    [PunRPC]
    void RPC_LeaveMessage(string username)
    {
        string msgs = username + " left chat.";
        GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=green> System: " + msgs + "</color>";
    }

    public void SystemMessage(string msgs)
    {
        PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

        if (currentScene < 3)
        {
            return;
        }

        GameManager.ins.references.systemText.text = "<color=white>" + msgs;
        GameManager.ins.references.systemText.gameObject.SetActive(true);

        Invoke("HideSystemText", 1.5f);
        Invoke("ShowSystemText", 1.7f);
        Invoke("HideSystemText", 3f);
        Invoke("ShowSystemText", 3.2f);
        Invoke("HideSystemText", 4.5f);

    }

    //sets isSystemMessage variable for all players
    [PunRPC]
    void RPC_SystemMessage(string msgs)
    {
        /*
        GameObject.Find("ChatManager").GetComponent<PhotonChatManager>().isSystemMessage = true;

        if (PV.IsMine == true)
        {
            //string msgs = username + " connected to chat.";
            GetComponentInChildren<PhotonChatManager>().chatClient.PublishMessage("RegionChannel", msgs);

        }
        */
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlaySystemMessage();

        GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=green> System: " + msgs + "</color>";
    }

    public void HideSystemText()
    {
        GameManager.ins.references.systemText.gameObject.SetActive(false);
    }

    public void ShowSystemText()
    {
        GameManager.ins.references.systemText.gameObject.SetActive(true);
    }

    //for relayed private system messages
    [PunRPC]
    void RPC_PrivateMessage(string msgs, int turnNumber)
    {
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            //play sfx
            GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayPrivateSystemMessage();

            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
        }
    }

    //called from FinishHandler class, when closing the positionings window
    public void CloseGame()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    //this closes the game, and returns to main menu
    IEnumerator DisconnectAndLoad()
    {

        Debug.Log("calls disconnectAndLoad");

        isGameLoaded = false;
        Debug.Log("returning to main menu");

        //MultiplayerSettings.multiplayerSettings.menuScene = 0;

        if (chatStarted == true)
        {
            //new system: start chatroom
            //GetComponentInChildren<PhotonChatManager>().ChatConnect();

            chatStarted = false;
        }

        //reset these back to start values
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.SetActive(false);
        GameObject.Find("MultiplayerSettings").GetComponent<MultiplayerSettings>().multiplayerScene = 2;

        //test
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();

        //use coroutines yield statement to see if player is still connected
        while (PhotonNetwork.IsConnected)
        //while (PhotonNetwork.InRoom)
            yield return null;

        Debug.Log("should load menu scene from photonroom class");

        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.menuScene);

        //dunno if we should use this now
        //PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);

        //destroyes this old roomcontoller, so the one in menuscene can take effect
        Destroy(this.gameObject);
    }
}
