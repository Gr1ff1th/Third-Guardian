using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]

    [SerializeField] 
    private string fileName;

    [SerializeField]
    private bool useEncryption;

    public GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    public static DataPersistenceManager instance { get; private set; }

    private FileDataHandler dataHandler;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying newest one.");
            //maybe we need to destroy this in some cases
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        //could do this in this class, since the class should be persistent between all scenes?
        //ShowIntro();
        //Invoke(nameof(ShowIntro), 0.5f);
        //Invoke(nameof(PlayMusic), 0.5f);
    }

    void ShowIntro()
    {
        SceneManager.LoadScene(70, LoadSceneMode.Single);
    }

    public void SetMenuScene()
    {
        PhotonRoom.room.CloseGame();
        //Destroy(GameObject.Find("AMMRoomController").gameObject);
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
        //Invoke(nameof(GoToMainMenu), 0.5f);
        Invoke(nameof(PlayMusic), 0.5f);
    }

    void GoToMainMenu()
    {
        //GameObject.Find("MultiplayerSettings").GetComponent<MultiplayerSettings>().multiplayerScene = 1;
        //PhotonRoom.room.currentScene = 0;
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
        //PhotonRoom.room.CloseGame();
        Invoke(nameof(PlayMusic), 0.5f);
    }

    void PlayMusic()
    {
        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().titleMusic.Play();
    }

    //dont call this now
    public void PlayTitleMusicWithDelay()
    {
        //Invoke(nameof(PlayMusic), 0.5f);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded Called");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded Called");
        //something wrong with this (it calls it twice might be the issue)
        /*lets try not saving if were in main scene (since it saves when quitting from the options menu there anyway)
         * seems this gives error on unloading minimap scenes as well
        if (scene.buildIndex != 2)
            SaveGame();
        */
        //might need this to unload minimap idataobjects?
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    private void Start()
    {
        //some code for testing (old)
        //this.gameData = null;
        //LoadGame();

        //lets try this for v92
        if(this.gameData == null)
        {
            this.gameData = new GameData();
            Debug.Log("New Data instantiated.");
        }
        else
        {
            LoadGame();
            Debug.Log("Loads old Data.");
        }
    }

    //used when starting new sp game
    public void NewGame()
    {
        //pre v92
        //this.gameData = new GameData();

        //maybe put this here, since were not using sceneunloaded
        //SaveGame();
        Debug.Log("NewGame initiated.");

        //for v92, instead of resetting whole savefile, lets just reset parts of it when starting new game
        //that way certain things can persist between games (high scores etc), and theyre recorded in single file (which should remove certain abuses)
        this.gameData.ResetInGameData();
        SaveGame();
    }

    public void LoadGame()
    {
        //load any saved data from file using the data handler
        this.gameData = dataHandler.Load();

        //if no data can be loaded, dont continue
        if(this.gameData == null)
        {
            Debug.Log("No data was found. A new game needs to be started before data can be loaded.");
            return;
            //NewGame();
        }
        //push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        //Debug.Log("Number of Players is: " + gameData.numberOfPlayers);
        
        /*
        bool isNull = true;
        
        if (gameData.avatars == null)
        {
            isNull = false;
        }
        */
    }

    public void SaveGame()
    {
        if(this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        //pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        //save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    /* when closing application.. dunno if we should keep this tho since quitting main scene isnt actually quitting the game
     * 
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    */

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
