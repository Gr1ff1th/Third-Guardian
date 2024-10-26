using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
//
public class SelectorScript2 : MonoBehaviour
{
    //public static SelectorScript2 SS;
    //singleton
    public static SelectorScript2 ins;

    public PhotonView PV;

    public GameObject[] characterList;
    public int index;

    //for displaying the biography
    public TextMeshProUGUI biographyText;

    public Text characterNameText;

    //for handling duration display
    public Text durationText;
    public int gameDuration;
    public Button durationUpButton;
    public Button durationDownButton;

    public Button selectCharacterButton;
    public Button leaveGameButton;

    //for handling bots
    public Text aiText;
    public Button aiUpButton;
    public Button aiDownButton;
    public int aiNumber;
    public GameObject aiDisplay;

    //for handling starting location
    public Text locationText;
    public int locationNumber;
    public Button locationUpButton;
    public Button locationDownButton;

    //for handling difficulty level
    public Text difficultyText;
    public int difficultyNumber;
    public Button difficultyUpButton;
    public Button difficultyDownButton;

    //for map type
    public int mapTypeNumber;
    public Button mapTypeUpButton;
    public Button mapTypeDownButton;

    //for handling start coins
    public TextMeshProUGUI coinsText;
    public int coinsNumber;
    public Button coinsUpButton;
    public Button coinsDownButton;

    //score modifier
    public TextMeshProUGUI scoreText;
    public int scoreNumber;

    //for high scores
    public TextMeshProUGUI highScoreText;

    public CardSaveHandler cardSaveHandler;

    public GameObject perkCardArea;

    //maybe activate this later for mp games
    public GameObject durationDisplay;

    public int numberOfCharsChosen;

    //loading panel
    public GameObject loadingScreenPanel;

    //for stat texts
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI arcanePowerText;
    public TextMeshProUGUI resistanceText;

    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI mechanicsText;
    public TextMeshProUGUI diggingText;
    public TextMeshProUGUI loreText;
    public TextMeshProUGUI observeText;

    //these should have reference
    public GameObject toolTipBackground;
    public TextMeshProUGUI toolTipText;

    public LoadingHandler loadingHandler;

    public GameObject difficultyCanvas;
    public GameObject startLocationCanvas;
    public GameObject mapTypeCanvas;

    public SFXPlayer sfxPlayer;

    //making the class persistent, testing
    /* remove this for now
    private void OnEnable()
    {
        if (SelectorScript2.SS == null)
        {
            SelectorScript2.SS = this;
        }
        else
        {
            if (SelectorScript2.SS != this)
            {
                Destroy(SelectorScript2.SS.gameObject);
                SelectorScript2.SS = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        //very bad singleton
        ins = this;

        numberOfCharsChosen = 0;

        //deactivate loading panel
        loadingScreenPanel.SetActive(false);

        //makes start game button uninteractable at start
        //GameObject.Find("Start Game Button (new)").GetComponent<Button>().interactable = false;

        //checks if the player-prefs file exists
        if (PlayerPrefs.HasKey("MyCharacter"))
        {
            index = PlayerPrefs.GetInt("MyCharacter");
        }
        else
        {
            index = 0;
            PlayerPrefs.SetInt("MyCharacter", index);
        }

        /* not needed atm
        //index = PlayerPrefs.GetInt("SelectedCharacter");
        characterList = new GameObject[transform.childCount];
        
        //fill the array
        for (int i = 0; i < transform.childCount; i++)
            characterList[i] = transform.GetChild(i).gameObject;
        */
        //make them inactive
        foreach (GameObject go in characterList)
            go.SetActive(false);

        //toggle on first index
        if (characterList[index])
            characterList[index].SetActive(true);

        //strengthText.text = GameObject.Find("StrengthText").GetComponent<Text>;
        //set the duration display text
        gameDuration = 3;
        durationText.text = "3 days";

        //set bots
        aiNumber = 1;
        aiText.text = "1";

        //lets update high scores here too
        UpdateStatTexts();

        UpdateName();

        //get reference to card save handler
        cardSaveHandler = GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>();

        //next update starting perk displays according to each characters playerpref array
        //should be implemented for the final version, might be best to start working on it now
        CheckSaves();

        //enable the leave game button
        leaveGameButton.interactable = true;

        UpdateBiography();

        //for v92
        //scoreNumber = 100;

        //changed for v0.7.1.
        if (PlayerPrefs.HasKey("MyStartLocation"))
        {
            locationNumber = PlayerPrefs.GetInt("MyStartLocation");
            //locationNumber = 1;
            //PlayerPrefs.SetInt("MyStartLocation", locationNumber);
            if(locationNumber > 2)
            {
                locationNumber = 1;
            }
        }
        else
        {
            locationNumber = 1;
            PlayerPrefs.SetInt("MyStartLocation", locationNumber);
        }
        SetLocation();

        if (PlayerPrefs.HasKey("MyStartDifficulty"))
        {
            difficultyNumber = PlayerPrefs.GetInt("MyStartDifficulty");
        }
        else
        {
            difficultyNumber = 2;
            PlayerPrefs.SetInt("MyStartDifficulty", difficultyNumber);
        }
        SetDifficulty();

        if (PlayerPrefs.HasKey("MyMapType"))
        {
            mapTypeNumber = PlayerPrefs.GetInt("MyMapType");
        }
        else
        {
            mapTypeNumber = 1;
            PlayerPrefs.SetInt("MyMapType", mapTypeNumber);
        }
        SetMapType();

        if (PlayerPrefs.HasKey("MyStartCoins"))
        {
            coinsNumber = PlayerPrefs.GetInt("MyStartCoins");
        }
        else
        {
            coinsNumber = 0;
            PlayerPrefs.SetInt("MyStartCoins", coinsNumber);
        }
        SetCoins();

        gameObject.GetComponent<BackgroundHandler>().ResetBackGrounds();

        gameObject.GetComponent<BackgroundHandler>().ShowBackgroundOptions(false);
    }

    private void Update()
    {
        if (characterList[index].GetComponent<Character>().isSelected == true)
        {
            selectCharacterButton.interactable = false;
        }
    }

    public void NextCharacter()
    {
        //toggle off the current model
        characterList[index].SetActive(false);

        index++;
        if (index > characterList.Length - 1)
            index = 0;

        //toggle on new model
        characterList[index].SetActive(true);

        //test
        //PlayerPrefs.SetInt("MyCharacter", index);

        UpdateStatTexts();

        UpdateName();

        ClearPerkDisplay();

        CheckSaves();

        UpdateBiography();

        selectCharacterButton.interactable = true;

        gameObject.GetComponent<BackgroundHandler>().ResetBackGrounds();
        gameObject.GetComponent<BackgroundHandler>().ShowBackgroundOptions(false);
    }
    public void PreviousCharacter()
    {

        //toggle off the current model
        characterList[index].SetActive(false);

        index--;
        if (index < 0)
            index = characterList.Length - 1;

        //toggle on new model
        characterList[index].SetActive(true);

        //test
        //PlayerPrefs.SetInt("MyCharacter", index);

        //characterList[index].GetComponent<Animator>().SetBool("Cast 1", false);

        UpdateStatTexts();

        UpdateName();

        ClearPerkDisplay();

        CheckSaves();

        UpdateBiography();

        selectCharacterButton.interactable = true;

        gameObject.GetComponent<BackgroundHandler>().ResetBackGrounds();
        gameObject.GetComponent<BackgroundHandler>().ShowBackgroundOptions(false);
    }

    //for displaying selected characters biography
    public void UpdateBiography()
    {
        biographyText.text = characterList[index].GetComponent<Character>().biographyText;
    }

    public void UpdateName()
    {
        characterNameText.text = characterList[index].GetComponent<Character>().heroName;
    }

    // updates the skill display, might need to add to playerinfo or charcontroller also
    // next method displaces this
    private void UpdateSkills()
    {
        GameObject.Find("StrengthText").GetComponent <TextMeshProUGUI>().text = characterList[index].GetComponent<Character>().strength.ToString();
        GameObject.Find("InfluenceText").GetComponent<TextMeshProUGUI>().text = characterList[index].GetComponent<Character>().influence.ToString();
        GameObject.Find("MechanicsText").GetComponent<TextMeshProUGUI>().text = characterList[index].GetComponent<Character>().mechanics.ToString();
        GameObject.Find("DiggingText").GetComponent<TextMeshProUGUI>().text = characterList[index].GetComponent<Character>().digging.ToString();
        GameObject.Find("LoreText").GetComponent<TextMeshProUGUI>().text = characterList[index].GetComponent<Character>().lore.ToString();
        GameObject.Find("ObserveText").GetComponent<TextMeshProUGUI>().text = characterList[index].GetComponent<Character>().observe.ToString(); 
        
        //old format
        //observeText.text = characterList[index].GetComponent<Character>().observe.ToString();
    }

    /* old 
    public void UpdateStatTexts()
    {
        //show character skill values
        //strengthText.text = strength.ToString();
        strengthText.text = "";
        influenceText.text = "";
        mechanicsText.text = "";
        diggingText.text = "";
        loreText.text = "";
        observeText.text = "";

        if (characterList[index].GetComponent<Character>().strength > 6)
        {
            strengthText.text = "<voffset=4><size=24>" + characterList[index].GetComponent<Character>().strength.ToString() + "</size></voffset>" + "<sprite index=6>";
        }
        if (characterList[index].GetComponent<Character>().strength <= 6)
        {
            for (int i = 0; i < characterList[index].GetComponent<Character>().strength; i++)
            {
                strengthText.text += "<sprite index=6>";
            }
        }

        if (characterList[index].GetComponent<Character>().influence > 6)
        {
            influenceText.text = "<voffset=4><size=24>" + characterList[index].GetComponent<Character>().influence.ToString() + "</size></voffset>" + "<sprite index=4>";
        }
        if (characterList[index].GetComponent<Character>().influence <= 6)
        {
            for (int i = 0; i < characterList[index].GetComponent<Character>().influence; i++)
            {
                influenceText.text += "<sprite index=4>";
            }
        }

        if (characterList[index].GetComponent<Character>().mechanics > 6)
        {
            mechanicsText.text = "<voffset=4><size=24>" + characterList[index].GetComponent<Character>().mechanics.ToString() + "</size></voffset>" + "<sprite index=5>";
        }
        if (characterList[index].GetComponent<Character>().mechanics <= 6)
        {
            for (int i = 0; i < characterList[index].GetComponent<Character>().mechanics; i++)
            {
                mechanicsText.text += "<sprite index=5>";
            }
        }

        if (characterList[index].GetComponent<Character>().digging > 6)
        {
            diggingText.text = "<voffset=4><size=24>" + characterList[index].GetComponent<Character>().digging.ToString() + "</size></voffset>" + "<sprite index=8>";
        }
        if (characterList[index].GetComponent<Character>().digging <= 6)
        {
            for (int i = 0; i < characterList[index].GetComponent<Character>().digging; i++)
            {
                diggingText.text += "<sprite index=8>";
            }
        }

        if (characterList[index].GetComponent<Character>().lore > 6)
        {
            loreText.text = "<voffset=4><size=24>" + characterList[index].GetComponent<Character>().lore.ToString() + "</size></voffset>" + "<sprite index=7>";
        }
        if (characterList[index].GetComponent<Character>().lore <= 6)
        {
            for (int i = 0; i < characterList[index].GetComponent<Character>().lore; i++)
            {
                loreText.text += "<sprite index=7>";
            }
        }

        if (characterList[index].GetComponent<Character>().observe > 6)
        {
            observeText.text = "<voffset=4><size=24>" + characterList[index].GetComponent<Character>().observe.ToString() + "</size></voffset>" + "<sprite index=9>";
        }
        if (characterList[index].GetComponent<Character>().observe <= 6)
        {
            for (int i = 0; i < characterList[index].GetComponent<Character>().observe; i++)
            {
                observeText.text += "<sprite index=9>";
            }
        }
    }
    */

    public void UpdateStatTexts()
    {
        //show character skill values
        //strengthText.text = strength.ToString();
        strengthText.text = "";
        defenseText.text = "";
        arcanePowerText.text = "";
        resistanceText.text = "";

        influenceText.text = "";
        mechanicsText.text = "";
        diggingText.text = "";
        loreText.text = "";
        observeText.text = "";

        //strength
        //ill leave color tag here for later reference (recolors to green, last two hexes are for alpha)
        for (int i = 0; i < characterList[index].GetComponent<Character>().strength && i < 6; i++)
        {
            //GameManager.ins.references.strengthText.text += "<sprite=\"resources & skills\" index=20 color=#00FF19>";
            strengthText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //defense
        for (int i = 0; i < characterList[index].GetComponent<Character>().defense && i < 6; i++)
        {
            defenseText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //AP
        for (int i = 0; i < characterList[index].GetComponent<Character>().arcanePower && i < 6; i++)
        {
            arcanePowerText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //resistance
        for (int i = 0; i < characterList[index].GetComponent<Character>().resistance && i < 6; i++)
        {
            resistanceText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //influence
        for (int i = 0; i < characterList[index].GetComponent<Character>().influence && i < 6; i++)
        {
            influenceText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //mechanics
        for (int i = 0; i < characterList[index].GetComponent<Character>().mechanics && i < 6; i++)
        {
            mechanicsText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //digging
        for (int i = 0; i < characterList[index].GetComponent<Character>().digging && i < 6; i++)
        {
            diggingText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //lore
        for (int i = 0; i < characterList[index].GetComponent<Character>().lore && i < 6; i++)
        {
            loreText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //discovery
        for (int i = 0; i < characterList[index].GetComponent<Character>().observe && i < 6; i++)
        {
            observeText.text += "<sprite=\"resources & skills\" index=21>";
        }

        //try this here
        highScoreText.text = DataPersistenceManager.instance.gameData.highScores[index].ToString();

    }

    public void SelectCharacter()
    {
        //make the select character button disabled if some1 has alrdy chosen this character
        
        //new
        //done with duplicate playerprefs, dunno if good idea
        if (PlayerInfo.PI != null)
        {
            PlayerInfo.PI.mySelectedCharacter = index;
            PlayerPrefs.SetInt("MyCharacter", index);
        }

        //disables buttons
        GameObject.Find("Previous Character Button (new)").GetComponent<Button>().interactable = false;
        GameObject.Find("Next Character Button (new)").GetComponent<Button>().interactable = false;
        GameObject.Find("Select Character Button (new)").GetComponent<Button>().interactable = false;

        //show start button
        //GameObject.Find("Start Game Button (new)").GetComponent<Button>().interactable = true;

        //SceneManager.LoadScene("SeptimusScene");

        //PhotonRoom.room.StartGame();

        /*make all characters active, not needed atm
        foreach (GameObject go in characterList)
            go.SetActive(true);

        //another way?
        //DontDestroyOnLoad(activeCharacter);
        */

        PV.RPC("RPC_SelectCharacter", RpcTarget.AllBufferedViaServer, index);
        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().hasSelectedCharacter = true;

        //give message
        string msgs = "Player has selected a character.";
        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

        //could put this here?
        loadingHandler.ActivateLoadingScreen();
    }

    [PunRPC]
    void RPC_SelectCharacter(int transferIndex)
    {
        GameObject.Find("CharacterList").GetComponent<SelectorScript2>().characterList[transferIndex].GetComponent<Character>().isSelected = true;

        numberOfCharsChosen += 1;

        //save background cards
        SaveBackgroundCards();

        //host starts game if all have chosen char
        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().playersInRoom == numberOfCharsChosen)
        {
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

    public void SaveBackgroundCards()
    {
        PhotonRoom.room.background1 = gameObject.GetComponent<BackgroundHandler>().backgroundSlot1.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        PhotonRoom.room.background2 = gameObject.GetComponent<BackgroundHandler>().backgroundSlot2.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        PhotonRoom.room.background3 = gameObject.GetComponent<BackgroundHandler>().backgroundSlot3.transform.GetChild(0).GetComponent<Card>().numberInDeck;
    }

    public void ChangeScene()
    {
        //could add check here later to make sure that all players have selected character

        PhotonRoom.room.StartGame();
    }
    /*
     dont work
    public GameObject GiveCharacter()
    {
        return characterList[index];
    }
    */

    // checks the saved character info
    // same function is done at avatarsetup for the main game
    public void CheckSaves()
    {
        //checks if beren is selected
        if(characterList[index].GetComponent<Character>().heroNumber == 0)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("BerenSave"))
            {
                cardSaveHandler.berenCards = new int[5];
                cardSaveHandler.berenCards[0] = 0;
                cardSaveHandler.berenCards[1] = 101;
                cardSaveHandler.berenCards[2] = 3;
                cardSaveHandler.berenCards[3] = 98;
                cardSaveHandler.berenCards[4] = 24;
                PlayerPrefsX.SetIntArray("BerenSave", cardSaveHandler.berenCards);
                //cardSaveHandler.berenCards = PlayerPrefsX.GetIntArray("BerenSave");
            }
            else
            {
                cardSaveHandler.berenCards = new int[5];
                cardSaveHandler.berenCards[0] = 0;
                cardSaveHandler.berenCards[1] = 101;
                cardSaveHandler.berenCards[2] = 3;
                cardSaveHandler.berenCards[3] = 98;
                cardSaveHandler.berenCards[4] = 24;
                PlayerPrefsX.SetIntArray("BerenSave", cardSaveHandler.berenCards);
            }

            for(int i = 0; i < cardSaveHandler.berenCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.berenCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if suliman is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 1)
        {
            //checks if the player-prefs file exists
            //temporarily need to overwrite old file here
            if (PlayerPrefs.HasKey("SulimanSave"))
            {
                cardSaveHandler.sulimanCards = new int[5];
                cardSaveHandler.sulimanCards[0] = 2;
                cardSaveHandler.sulimanCards[1] = 5;
                cardSaveHandler.sulimanCards[2] = 27;
                cardSaveHandler.sulimanCards[3] = 153;
                cardSaveHandler.sulimanCards[4] = 102;
                PlayerPrefsX.SetIntArray("SulimanSave", cardSaveHandler.sulimanCards);
                //cardSaveHandler.sulimanCards = PlayerPrefsX.GetIntArray("SulimanSave");
            }
            else
            {
                cardSaveHandler.sulimanCards = new int[5];
                cardSaveHandler.sulimanCards[0] = 2;
                cardSaveHandler.sulimanCards[1] = 5;
                cardSaveHandler.sulimanCards[2] = 27;
                cardSaveHandler.sulimanCards[3] = 153;
                cardSaveHandler.sulimanCards[4] = 102;
                PlayerPrefsX.SetIntArray("SulimanSave", cardSaveHandler.sulimanCards);
            }

            for (int i = 0; i < cardSaveHandler.sulimanCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.sulimanCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if dazzle is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 2)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("DazzleSave"))
            {
                cardSaveHandler.dazzleCards = new int[5];
                cardSaveHandler.dazzleCards[0] = 1;
                cardSaveHandler.dazzleCards[1] = 4;
                cardSaveHandler.dazzleCards[2] = 106;
                cardSaveHandler.dazzleCards[3] = 108;
                cardSaveHandler.dazzleCards[4] = 170;
                PlayerPrefsX.SetIntArray("DazzleSave", cardSaveHandler.dazzleCards);
                //cardSaveHandler.dazzleCards = PlayerPrefsX.GetIntArray("DazzleSave");
            }
            else
            {
                cardSaveHandler.dazzleCards = new int[5];
                cardSaveHandler.dazzleCards[0] = 1;
                cardSaveHandler.dazzleCards[1] = 4;
                cardSaveHandler.dazzleCards[2] = 106;
                cardSaveHandler.dazzleCards[3] = 108;
                cardSaveHandler.dazzleCards[4] = 170;
                PlayerPrefsX.SetIntArray("DazzleSave", cardSaveHandler.dazzleCards);
            }

            for (int i = 0; i < cardSaveHandler.dazzleCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.dazzleCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if maximus is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 3)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("MaximusSave"))
            {
                cardSaveHandler.maximusCards = new int[6];
                cardSaveHandler.maximusCards[0] = 0;
                cardSaveHandler.maximusCards[1] = 178;
                cardSaveHandler.maximusCards[2] = 17;
                cardSaveHandler.maximusCards[3] = 26;
                cardSaveHandler.maximusCards[4] = 235;
                cardSaveHandler.maximusCards[5] = 180;
                PlayerPrefsX.SetIntArray("MaximusSave", cardSaveHandler.maximusCards);
                //cardSaveHandler.maximusCards = PlayerPrefsX.GetIntArray("MaximusSave");
            }
            else
            {
                cardSaveHandler.maximusCards = new int[6];
                cardSaveHandler.maximusCards[0] = 0;
                cardSaveHandler.maximusCards[1] = 178;
                cardSaveHandler.maximusCards[2] = 17;
                cardSaveHandler.maximusCards[3] = 26;
                cardSaveHandler.maximusCards[4] = 235;
                cardSaveHandler.maximusCards[5] = 180;
                PlayerPrefsX.SetIntArray("MaximusSave", cardSaveHandler.maximusCards);
            }

            for (int i = 0; i < cardSaveHandler.maximusCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.maximusCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if melissya is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 4)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("MelissyaSave"))
            {
                cardSaveHandler.melissyaCards = new int[4];
                cardSaveHandler.melissyaCards[0] = 101;
                cardSaveHandler.melissyaCards[1] = 24;
                cardSaveHandler.melissyaCards[2] = 104;
                cardSaveHandler.melissyaCards[3] = 107;
                PlayerPrefsX.SetIntArray("MelissyaSave", cardSaveHandler.melissyaCards);
                //cardSaveHandler.melissyaCards = PlayerPrefsX.GetIntArray("MelissyaSave");
            }
            else
            {
                cardSaveHandler.melissyaCards = new int[4];
                cardSaveHandler.melissyaCards[0] = 101;
                cardSaveHandler.melissyaCards[1] = 24;
                cardSaveHandler.melissyaCards[2] = 104;
                cardSaveHandler.melissyaCards[3] = 107;
                PlayerPrefsX.SetIntArray("MelissyaSave", cardSaveHandler.melissyaCards);
            }

            for (int i = 0; i < cardSaveHandler.melissyaCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.melissyaCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if targas is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 5)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("TargasSave"))
            {
                cardSaveHandler.targasCards = new int[5];
                cardSaveHandler.targasCards[0] = 0;
                cardSaveHandler.targasCards[1] = 1;
                cardSaveHandler.targasCards[2] = 9;
                cardSaveHandler.targasCards[3] = 106;
                cardSaveHandler.targasCards[4] = 109;
                //cardSaveHandler.targasCards[2] = 10;
                PlayerPrefsX.SetIntArray("TargasSave", cardSaveHandler.targasCards);
                //cardSaveHandler.targasCards = PlayerPrefsX.GetIntArray("TargasSave");
            }
            else
            {
                cardSaveHandler.targasCards = new int[5];
                cardSaveHandler.targasCards[0] = 0;
                cardSaveHandler.targasCards[1] = 1;
                cardSaveHandler.targasCards[2] = 9;
                cardSaveHandler.targasCards[3] = 106;
                cardSaveHandler.targasCards[4] = 109;
                //cardSaveHandler.targasCards[2] = 10;
                PlayerPrefsX.SetIntArray("TargasSave", cardSaveHandler.targasCards);
            }

            for (int i = 0; i < cardSaveHandler.targasCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.targasCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if naomi is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 6)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("NaomiSave"))
            {
                cardSaveHandler.naomiCards = new int[5];
                cardSaveHandler.naomiCards[0] = 0;
                cardSaveHandler.naomiCards[1] = 8;
                cardSaveHandler.naomiCards[2] = 25;
                cardSaveHandler.naomiCards[3] = 99;
                cardSaveHandler.naomiCards[4] = 108;
                PlayerPrefsX.SetIntArray("NaomiSave", cardSaveHandler.naomiCards);
                //cardSaveHandler.naomiCards = PlayerPrefsX.GetIntArray("NaomiSave");
            }
            else
            {
                cardSaveHandler.naomiCards = new int[5];
                cardSaveHandler.naomiCards[0] = 0;
                cardSaveHandler.naomiCards[1] = 8;
                cardSaveHandler.naomiCards[2] = 25;
                cardSaveHandler.naomiCards[3] = 99;
                cardSaveHandler.naomiCards[4] = 108;
                PlayerPrefsX.SetIntArray("NaomiSave", cardSaveHandler.naomiCards);
            }

            for (int i = 0; i < cardSaveHandler.naomiCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.naomiCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if ariel is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 7)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("ArielSave"))
            {
                cardSaveHandler.arielCards = new int[5];
                cardSaveHandler.arielCards[0] = 2;
                cardSaveHandler.arielCards[1] = 15;
                cardSaveHandler.arielCards[2] = 18;
                cardSaveHandler.arielCards[3] = 102;
                cardSaveHandler.arielCards[4] = 249;
                PlayerPrefsX.SetIntArray("ArielSave", cardSaveHandler.arielCards);
                //cardSaveHandler.arielCards = PlayerPrefsX.GetIntArray("ArielSave");
            }
            else
            {
                cardSaveHandler.arielCards = new int[5];
                cardSaveHandler.arielCards[0] = 2;
                cardSaveHandler.arielCards[1] = 15;
                cardSaveHandler.arielCards[2] = 18;
                cardSaveHandler.arielCards[3] = 102;
                cardSaveHandler.arielCards[4] = 249;
                PlayerPrefsX.SetIntArray("ArielSave", cardSaveHandler.arielCards);
            }

            for (int i = 0; i < cardSaveHandler.arielCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.arielCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if enigma is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 8)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("EnigmaSave"))
            {
                cardSaveHandler.enigmaCards = new int[5];
                cardSaveHandler.enigmaCards[0] = 1;
                cardSaveHandler.enigmaCards[1] = 11;
                cardSaveHandler.enigmaCards[2] = 12;
                cardSaveHandler.enigmaCards[3] = 100;
                cardSaveHandler.enigmaCards[4] = 106;
                //cardSaveHandler.enigmaCards = PlayerPrefsX.GetIntArray("EnigmaSave");
            }
            else
            {
                cardSaveHandler.enigmaCards = new int[5];
                cardSaveHandler.enigmaCards[0] = 1;
                cardSaveHandler.enigmaCards[1] = 11;
                cardSaveHandler.enigmaCards[2] = 12;
                cardSaveHandler.enigmaCards[3] = 100;
                cardSaveHandler.enigmaCards[4] = 106;

                PlayerPrefsX.SetIntArray("EnigmaSave", cardSaveHandler.enigmaCards);
            }

            for (int i = 0; i < cardSaveHandler.enigmaCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.enigmaCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }

        //checks if rimlic is selected
        if (characterList[index].GetComponent<Character>().heroNumber == 9)
        {
            //checks if the player-prefs file exists
            if (PlayerPrefs.HasKey("RimlicSave"))
            {
                cardSaveHandler.rimlicCards = new int[6];
                cardSaveHandler.rimlicCards[0] = 1;
                cardSaveHandler.rimlicCards[1] = 2;
                cardSaveHandler.rimlicCards[2] = 16;
                cardSaveHandler.rimlicCards[3] = 103;
                cardSaveHandler.rimlicCards[4] = 166;
                cardSaveHandler.rimlicCards[5] = 250;
                PlayerPrefsX.SetIntArray("RimlicSave", cardSaveHandler.rimlicCards);

                //cardSaveHandler.rimlicCards = PlayerPrefsX.GetIntArray("RimlicSave");
            }
            else
            {
                cardSaveHandler.rimlicCards = new int[6];
                cardSaveHandler.rimlicCards[0] = 1;
                cardSaveHandler.rimlicCards[1] = 2;
                cardSaveHandler.rimlicCards[2] = 16;
                cardSaveHandler.rimlicCards[3] = 103;
                cardSaveHandler.rimlicCards[4] = 166;
                cardSaveHandler.rimlicCards[5] = 250;
                PlayerPrefsX.SetIntArray("RimlicSave", cardSaveHandler.rimlicCards);
            }

            for (int i = 0; i < cardSaveHandler.rimlicCards.Length; i++)
            {
                GameObject playerCard = Instantiate(cardSaveHandler.generalDeck[cardSaveHandler.rimlicCards[i]], new Vector3(0, 0, 0), Quaternion.identity);

                playerCard.transform.SetParent(perkCardArea.transform, false);
            }
        }
    }

    public void ClearPerkDisplay()
    {
        int childCount = perkCardArea.transform.childCount;

        if (childCount > 0)
        {
            //lets do this in reverse
            for (int i = childCount; i > 0; i--)
            {
                Destroy(perkCardArea.transform.GetChild(i - 1).gameObject);
            }
        }
    }

    public void IncreaseDuration()
    {
        gameDuration += 1;

        durationText.text = gameDuration + " Days";

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().gameDuration = gameDuration;

        if (gameDuration > 1)
        {
            durationDownButton.interactable = true;
        }
    }

    public void DecreaseDuration()
    {
        gameDuration -= 1;

        durationText.text = gameDuration + " Days";

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().gameDuration = gameDuration;

        if (gameDuration == 1)
        {
            durationDownButton.interactable = false;
        }
    }

    public void IncreaseAis()
    {
        aiNumber += 1;

        aiText.text = aiNumber.ToString();

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().aiNumber = aiNumber;

        if (aiNumber > 0)
        {
            aiDownButton.interactable = true;
        }

        if (aiNumber == 3)
        {
            aiUpButton.interactable = false;
        }
    }

    public void DecreaseAis()
    {
        aiNumber -= 1;

        aiText.text = aiNumber.ToString();

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().aiNumber = aiNumber;

        if (aiNumber < 3)
        {
            aiUpButton.interactable = true;
        }

        if (aiNumber == 0)
        {
            aiDownButton.interactable = false;
        }
    }

    //for leave game button at scene 1
    public void ReturnToMainMenu()
    {
        GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().PlayTitleMusicWithDelay();

        PhotonRoom.room.CloseGame();
    }

    public void IncreaseStartLocation()
    {
        locationNumber += 1;

        //locationText.text = locationNumber.ToString();
        SetLocation();

        //lets set the roomcontroller variable on the set text method
        //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().startLocation = locationNumber;

    }

    public void DecreaseStartLocation()
    {
        locationNumber -= 1;

        SetLocation();
    }

    public void IncreaseMapType()
    {
        mapTypeNumber += 1;

        SetMapType();
    }

    public void DecreaseMapType()
    {
        mapTypeNumber -= 1;

        SetMapType();
    }

    public void SetLocation()
    {
        /*disable other locations for now for v93
        if(locationNumber > 2)
        {
            locationNumber = 2;
        }
        */

        //cornville inn start
        if(locationNumber == 1)
        {
            //locationText.text = "cornville";
            DeleteIcon(2);
            Invoke(nameof(SetLocationIcon), 0.1f);
            locationUpButton.interactable = true;
            locationDownButton.interactable = false;
        }

        //wilforge temple
        if (locationNumber == 2)
        {
            //locationText.text = "cornville";
            DeleteIcon(2);
            Invoke(nameof(SetLocationIcon), 0.1f);
            locationUpButton.interactable = false;
            locationDownButton.interactable = true;
        }

        /* unused in v0.7.1.
         * planning to set temple as second location soon
         * 
        if (locationNumber == 2)
        {
            //locationText.text = "coven";
            DeleteIcon(2);
            Invoke("SetLocationIcon", 0.1f);
            locationUpButton.interactable = true;
            locationDownButton.interactable = true;
        }

        if (locationNumber == 3)
        {
            //locationText.text = "inn";
            DeleteIcon(2);
            Invoke("SetLocationIcon", 0.1f);
            locationUpButton.interactable = true;
            locationDownButton.interactable = true;
        }
        if (locationNumber == 4)
        {
            //locationText.text = "temple";
            DeleteIcon(2);
            Invoke("SetLocationIcon", 0.1f);
            locationUpButton.interactable = false;
            locationDownButton.interactable = true;
        }
        
        if (locationNumber == 5)
        {
            //locationText.text = "temple";
            DeleteIcon(2);
            Invoke("SetLocationIcon", 0.1f);

            locationUpButton.interactable = false;
            locationDownButton.interactable = true;
        }
        */

        SetScore();

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().startLocation = locationNumber;

        PlayerPrefs.SetInt("MyStartLocation", locationNumber);
    }

    public void DeleteIcon(int canvasNumber)
    {
        //difficulty canvas
        if (canvasNumber == 1)
        {
            if (difficultyCanvas.transform.childCount > 1)
            {
                Destroy(difficultyCanvas.transform.GetChild(1).gameObject);
            }
            if (difficultyCanvas.transform.childCount > 0)
            {
                Destroy(difficultyCanvas.transform.GetChild(0).gameObject);
            }
        }

        //location canvas
        if (canvasNumber == 2)
        {
            if(startLocationCanvas.transform.childCount > 1)
            {
                Destroy(startLocationCanvas.transform.GetChild(1).gameObject);
            }
            if (startLocationCanvas.transform.childCount > 0)
            {
                Destroy(startLocationCanvas.transform.GetChild(0).gameObject);
            }
        }

        //maptype canvas
        if (canvasNumber == 3)
        {
            if (mapTypeCanvas.transform.childCount > 1)
            {
                Destroy(mapTypeCanvas.transform.GetChild(1).gameObject);
            }
            if (mapTypeCanvas.transform.childCount > 0)
            {
                Destroy(mapTypeCanvas.transform.GetChild(0).gameObject);
            }
        }
    }

    public void SetLocationIcon()
    {
        int deckNumberToDraw = 251;

        //v0.7.1.
        //cornville (inn)
        if (locationNumber == 1)
        {
            deckNumberToDraw = 251;
        }
        //wilforge (temple)
        if (locationNumber == 2)
        {
            deckNumberToDraw = 289;
        }

        /* old
         * cornville
        if (locationNumber == 1)
        {
            deckNumberToDraw = 251;
        }
        //coven
        if (locationNumber == 2)
        {
            deckNumberToDraw = 252;
        }
        //wilforge
        if (locationNumber == 3)
        {
            deckNumberToDraw = 289;
        }
        //citadel
        if (locationNumber == 4)
        {
            deckNumberToDraw = 255;
        }
        */
        //instantiates random quest card from the deck
        GameObject locationCard = Instantiate(cardSaveHandler.generalDeck[deckNumberToDraw], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in hand card area
        locationCard.transform.SetParent(startLocationCanvas.transform, false);

        //turns the card inactive
        locationCard.SetActive(true);
    }

    public void IncreaseDifficulty()
    {
        difficultyNumber += 1;

        //locationText.text = locationNumber.ToString();
        SetDifficulty();
    }

    public void DecreaseDifficulty()
    {
        difficultyNumber -= 1;

        SetDifficulty();
    }

    public void SetDifficulty()
    {
        //tutorial
        //unused at v94 (should add soon tho)
        if (difficultyNumber == 1)
        {
            //difficultyText.text = "Normal";
            DeleteIcon(1);
            Invoke("SetDifficultyIcon", 0.1f);
            
            difficultyUpButton.interactable = true;
            difficultyDownButton.interactable = false;
        }
        //normal
        if (difficultyNumber == 2)
        {
            //difficultyText.text = "Hard";
            DeleteIcon(1);
            Invoke("SetDifficultyIcon", 0.1f);
            difficultyUpButton.interactable = true;
            difficultyDownButton.interactable = true;
        }

        //advanced
        if (difficultyNumber == 3)
        {
            //difficultyText.text = "Hard";
            DeleteIcon(1);
            Invoke("SetDifficultyIcon", 0.1f);
            difficultyUpButton.interactable = true;
            difficultyDownButton.interactable = true;
        }
        //expert
        if (difficultyNumber == 4)
        {
            //difficultyText.text = "Hard";
            DeleteIcon(1);
            Invoke("SetDifficultyIcon", 0.1f);
            difficultyUpButton.interactable = false;
            difficultyDownButton.interactable = true;
        }

        SetScore();
        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().startDifficulty = difficultyNumber;

        PlayerPrefs.SetInt("MyStartDifficulty", difficultyNumber);
    }

    public void SetMapType()
    {
        if(mapTypeNumber == 1)
        {
            DeleteIcon(3);
            Invoke(nameof(SetMapTypeIcon), 0.1f);
            mapTypeUpButton.interactable = false;
            mapTypeDownButton.interactable = false;
        }
        if (mapTypeNumber == 2)
        {
            DeleteIcon(3);
            Invoke(nameof(SetMapTypeIcon), 0.1f);
            mapTypeUpButton.interactable = false;
            mapTypeDownButton.interactable = true;
        }

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().mapType = mapTypeNumber;

        PlayerPrefs.SetInt("MyMapType", mapTypeNumber);
    }

    public void SetDifficultyIcon()
    {
        int deckNumberToDraw = 257;

        //tutorial
        if (difficultyNumber == 1)
        {
            deckNumberToDraw = 256;
        }
        //normal
        if (difficultyNumber == 2)
        {
            deckNumberToDraw = 257;
        }
        //advanced
        if (difficultyNumber == 3)
        {
            deckNumberToDraw = 258;
        }
        //expert
        if (difficultyNumber == 4)
        {
            deckNumberToDraw = 259;
        }
        //instantiates random quest card from the deck
        GameObject locationCard = Instantiate(cardSaveHandler.generalDeck[deckNumberToDraw], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in hand card area
        locationCard.transform.SetParent(difficultyCanvas.transform, false);

        //turns the card inactive
        locationCard.SetActive(true);
    }

    public void SetMapTypeIcon()
    {
        int deckNumberToDraw = 299;

        if (mapTypeNumber == 1)
        {
            deckNumberToDraw = 299;
        }
        if (mapTypeNumber == 2)
        {
            deckNumberToDraw = 300;
        }

        //instantiates random quest card from the deck
        GameObject mapTypeCard = Instantiate(cardSaveHandler.generalDeck[deckNumberToDraw], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in hand card area
        mapTypeCard.transform.SetParent(mapTypeCanvas.transform, false);

        //turns the card inactive
        mapTypeCard.SetActive(true);
    }

    public void IncreaseCoins()
    {
        coinsNumber += 10;

        SetCoins();
    }

    public void DecreaseCoins()
    {
        coinsNumber -= 10;

        SetCoins();
    }

    public void SetCoins()
    {
        //disable coinscounts for v93
        if (coinsNumber > 0)
        {
            coinsNumber = 0;
        }

        coinsDownButton.interactable = false;
        coinsUpButton.interactable = false;

        coinsText.text = coinsNumber + " <sprite=13>";
        /*
        if (coinsNumber == 0)
        {
            coinsDownButton.interactable = false;
            coinsUpButton.interactable = true;
        }
        if (coinsNumber > 0 && coinsNumber < 100)
        {
            coinsDownButton.interactable = true;
            coinsUpButton.interactable = true;
        }
        if (coinsNumber == 100)
        {
            coinsDownButton.interactable = true;
            coinsUpButton.interactable = false;
        }
        */
        SetScore();
        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().startCoins = coinsNumber;

        PlayerPrefs.SetInt("MyStartCoins", coinsNumber);
    }

    public void SetScore()
    {
        scoreNumber = 100;// - (coinsNumber / 10);

        if (difficultyNumber == 1)
        {
            scoreNumber = 50;
        }
        if (difficultyNumber == 2)
        {
            scoreNumber = 100;
        }
        if (difficultyNumber == 3)
        {
            scoreNumber = 125;
        }
        if (difficultyNumber == 4)
        {
            scoreNumber = 160;
        }

        /* remove start location score modifiers for now
        if(locationNumber == 1)
        {
            scoreNumber -= 3;
        }
        if (locationNumber == 2)
        {
            scoreNumber -= 1;
        }
        if (locationNumber == 3)
        {
            scoreNumber += 1;
        }
        if (locationNumber == 4)
        {
            scoreNumber += 3;
        }
        */

        /* removed in v0.5.0.+
         * background card calculations
        if(gameObject.GetComponent<BackgroundHandler>().backgroundSlot1.transform.childCount > 0)
        {
            scoreNumber += gameObject.GetComponent<BackgroundHandler>().backgroundSlot1.transform.GetChild(0).GetComponent<Card>().value;
        }
        if (gameObject.GetComponent<BackgroundHandler>().backgroundSlot2.transform.childCount > 0)
        {
            scoreNumber += gameObject.GetComponent<BackgroundHandler>().backgroundSlot2.transform.GetChild(0).GetComponent<Card>().value;
        }
        if (gameObject.GetComponent<BackgroundHandler>().backgroundSlot3.transform.childCount > 0)
        {
            scoreNumber += gameObject.GetComponent<BackgroundHandler>().backgroundSlot3.transform.GetChild(0).GetComponent<Card>().value;
        }
        */


        scoreText.text = scoreNumber + "%";

        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().scoreModifier = scoreNumber;
    }
}

