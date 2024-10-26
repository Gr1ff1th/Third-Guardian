using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    //Singleton. Because its static other scripts can access this
    public static PhotonLobby lobby;

    //main menu buttons
    public GameObject battleButton;
    public GameObject cancelButton;
    public GameObject offlineButton;

    //menu canvases
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject singlePlayerMenu;
    public GameObject confirmationMenu;
    public GameObject creditsDisplay;
    public GameObject highScoresDisplay;

    //for resolution items
    public List<ResItem> resolutions = new List<ResItem>();
    public int selectedResolution;
    public Text resolutionLabelText;

    //can be disabled
    public Button continueSpGameButton;

    //for v95
    public TextMeshProUGUI creditsText;
    public Vector3 creditsTextStartPosition;
    public float screenHeight;

    //public string roomName;

    private void Awake()
    {
        //Creates the singleton, lives within the Main menu scene.
        lobby = this;

        //GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel = PlayerPrefs.GetFloat("MusicLevel");
        //GameObject.Find("Music Volume Slider").GetComponent<Slider>().value = GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel;

        //Debug.Log("sfxVol is: " + GameObject.Find("SoundManager").GetComponent<SoundManager>().masterMixer.GetFloat("sfxVol", Mathf.Log10(sfxLvl) * 20));
        //selectedResolution = 1;        
    }
    // Start is called before the first frame update
    void Start()
    {
        //Connects to Master photon server.
        //set this back, if we want mp at some point (should be still kinda working, in parts..)
        //PhotonNetwork.ConnectUsingSettings();

        //lets try this here
        PhotonNetwork.OfflineMode = true;

        Debug.Log("Is supposed to connect..");

        screenHeight = Screen.height;
        creditsTextStartPosition = creditsText.gameObject.transform.position;

        //checks the sfx level player prefs
        if (PlayerPrefs.HasKey("SfxVol"))
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().masterMixer.SetFloat("sfxVol", PlayerPrefs.GetFloat("SfxVol"));
        }
        //checks the music level player prefs
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().masterMixer.SetFloat("musicVol", PlayerPrefs.GetFloat("MusicVol"));
        }

        //checks theres setting for this set alrdy
        if (!PlayerPrefs.HasKey("PauseOn"))
        {
            //1 is on, 0 is off
            PlayerPrefs.SetInt("PauseOn", 1);
        }

        //UpdateResLabel();
        //bool foundRes = false;

        //dunno what the boolean is for
        //here we find the correct resolution
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                selectedResolution = i;
                UpdateResLabel();
            }
        }

        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueSpGameButton.interactable = false;
        }

        //force resolution, if it hasnt been set previously
        if (PlayerPrefs.HasKey("MyResolution"))
        {
            selectedResolution = PlayerPrefs.GetInt("MyResolution");

            Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, true);
        }
        else
        {
            Screen.SetResolution(1600, 900, true);
        }
    }

    private void Update()
    {
        float finalScrollSpeed = screenHeight * 0.05f * Time.deltaTime;

        if (creditsDisplay.activeSelf)
        {
            creditsText.gameObject.transform.position += new Vector3(0, finalScrollSpeed, 0);
        }
    }

    //override the callback function
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon master server");
        
        //this will synchronize loaded level between all clients
        PhotonNetwork.AutomaticallySyncScene = true;

        //Player is now connected to servers, enables battlebutton to allow join a game.
        if (PhotonNetwork.OfflineMode == false)
        {
            //lets put everything false for now
            battleButton.SetActive(false); //was true
            offlineButton.SetActive(false);
        }
    }

    public void OnBattleButtonClicked()
    {
        Debug.Log("Battle button was clicked");
        battleButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();

        mainMenu.SetActive(false);

        //need to disable the username display also
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(false);
    }

    public void OnOptionsButtonClicked()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        //need to disable the username display also
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(false);

        //checks the sfx level player prefs
        if (PlayerPrefs.HasKey("SfxLevel"))
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxLevel = PlayerPrefs.GetFloat("SfxLevel");
            GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxVol = PlayerPrefs.GetFloat("SfxVol");
            GameObject.Find("SFX Volume Slider").GetComponent<Slider>().value = GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxLevel;
        }
        else
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxLevel = 0.5f;
            GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxVol = -6.0f;
            PlayerPrefs.SetFloat("SfxLevel", GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxLevel);
            PlayerPrefs.SetFloat("SfxVol", GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxVol);
            GameObject.Find("SFX Volume Slider").GetComponent<Slider>().value = GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxLevel;
        }
        //checks the music level player prefs
        if (PlayerPrefs.HasKey("MusicLevel"))
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel = PlayerPrefs.GetFloat("MusicLevel");
            GameObject.Find("SoundManager").GetComponent<SoundManager>().musicVol = PlayerPrefs.GetFloat("MusicVol");
            GameObject.Find("Music Volume Slider").GetComponent<Slider>().value = GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel;
        }
        else
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel = 0.5f;
            GameObject.Find("SoundManager").GetComponent<SoundManager>().musicVol = -6.0f;
            PlayerPrefs.SetFloat("MusicLevel", GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel);
            PlayerPrefs.SetFloat("MusicVol", GameObject.Find("SoundManager").GetComponent<SoundManager>().musicVol);
            GameObject.Find("Music Volume Slider").GetComponent<Slider>().value = GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel;
        }

        //here we find the correct resolution
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                selectedResolution = i;
                UpdateResLabel();
            }
        }
    }

    public void OnCreditButtonClicked()
    {
        mainMenu.SetActive(false);

        creditsText.gameObject.transform.position = creditsTextStartPosition;

        creditsDisplay.SetActive(true);

        //need to disable the username display also
        //this shouldnt be activated anyway
        //GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(false);
    }

    public void OnSinglePlayerMenuButtonClicked()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(true);

        if(DataPersistenceManager.instance.gameData.continueAvailable == true)
        {
            continueSpGameButton.interactable = true;
            continueSpGameButton.GetComponentInChildren<CanvasGroup>().alpha = 1f;
        }
        else
        {
            continueSpGameButton.interactable = false;
            continueSpGameButton.GetComponentInChildren<CanvasGroup>().alpha = 0.5f;
        }

        //need to disable the username display also
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(false);
    }

    //for v1.0.0.
    public void OnHighScoresButtonClicked()
    {
        mainMenu.SetActive(false);
        highScoresDisplay.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsDisplay.SetActive(false);
        //GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(true);
    }

    //for single player menu
    //also for high score display in v1.0.0.
    public void OnBackButtonClicked2()
    {
        mainMenu.SetActive(true);
        singlePlayerMenu.SetActive(false);
        confirmationMenu.SetActive(false);
        highScoresDisplay.SetActive(false);
        //GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().usernameCanvas.SetActive(true);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join random game but failed. There must be no open games available");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Trying to create new room");
        int randomRoomName = Random.Range(0, 10000);

        //sets certain options. whether the room is public, whether its open, playersize. 
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };

        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);

        //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().roomName = "Room" + randomRoomName;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be room with the same name");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button was clicked");
        cancelButton.SetActive(false);
        battleButton.SetActive(false); //was true
        PhotonNetwork.LeaveRoom();
    }

    public void OnSinglePlayerButtonClicked()
    {
        if(singlePlayerMenu.activeSelf && DataPersistenceManager.instance.gameData.continueAvailable == true)
        {
            confirmationMenu.SetActive(true);
            singlePlayerMenu.SetActive(false);
            return;
        }

        //PhotonNetwork.OfflineMode = true;

        Debug.Log("Single player button was clicked");
        //battleButton.SetActive(false);
        //cancelButton.SetActive(true);
        //PhotonNetwork.Disconnect();

        //Invoke("StartSinglePlayer", 1.0f);
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isPractice = false;
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().aiNumber = 0;

        MultiplayerSettings.multiplayerSettings.maxPlayers = 1;
        RoomOptions roomOps = new RoomOptions() { IsVisible = false, IsOpen = false, MaxPlayers = 1};
        PhotonNetwork.CreateRoom(null, roomOps, null, null);

        //SceneManager.LoadScene(1, LoadSceneMode.Single);

        singlePlayerMenu.SetActive(false);

        //this shouldnt change anything, but lets put just in case
        PhotonRoom.room.spContinue = false;

        //new code for v90
        DataPersistenceManager.instance.NewGame();
    }

    public void OnContinueGameButtonClicked()
    {
        Debug.Log("Single player button was clicked");
        //battleButton.SetActive(false);
        //cancelButton.SetActive(true);
        //PhotonNetwork.Disconnect();

        //Invoke("StartSinglePlayer", 1.0f);
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isPractice = false;
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().aiNumber = 0;

        //for v1.0.0
        GameObject.Find("Loading Canvas").GetComponentInChildren<LoadingHandler>().ActivateLoadingScreen();

        //lets test this here for v92
        MultiplayerSettings.multiplayerSettings.multiplayerScene++;

        PhotonRoom.room.spContinue = true;

        MultiplayerSettings.multiplayerSettings.maxPlayers = 1;
        RoomOptions roomOps = new RoomOptions() { IsVisible = false, IsOpen = false, MaxPlayers = 1 };
        PhotonNetwork.CreateRoom(null, roomOps, null, null);

        singlePlayerMenu.SetActive(false);

        //MultiplayerSettings.multiplayerSettings.multiplayerScene++;
        //MultiplayerSettings.multiplayerSettings.multiplayerScene = 2;

        //try remove this for v92
        //PhotonRoom.room.StartGame();
    }


    public void OnTutorialButtonClicked()
    {
        Debug.Log("Single player button was clicked");
        //battleButton.SetActive(false);
        //cancelButton.SetActive(true);
        //PhotonNetwork.Disconnect();

        //Invoke("StartSinglePlayer", 1.0f);
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isPractice = true;
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isTutorial = true;
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().aiNumber = 1;

        MultiplayerSettings.multiplayerSettings.maxPlayers = 1;
        RoomOptions roomOps = new RoomOptions() { IsVisible = false, IsOpen = false, MaxPlayers = 1 };
        PhotonNetwork.CreateRoom(null, roomOps, null, null);

        singlePlayerMenu.SetActive(false);
    }
    /*
    void StartSinglePlayer()
    {
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.JoinRandomRoom();
    }
    */

    // for decreasing resolution
    public void ResDown()
    {
        selectedResolution--;
        if(selectedResolution < 0)
        {
            selectedResolution = 0;
        }

        UpdateResLabel();
    }

    // for increasing resolution
    public void ResUp()
    {
        selectedResolution++;
        if (selectedResolution > resolutions.Count -1)
        {
            selectedResolution = resolutions.Count -1;
        }

        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        resolutionLabelText.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    public void ApplyGraphics()
    {
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, true);

        PlayerPrefs.SetInt("MyResolution", selectedResolution);
    }
}


//resolution items
[System.Serializable]
public class ResItem
{
    public int horizontal, vertical;
}
