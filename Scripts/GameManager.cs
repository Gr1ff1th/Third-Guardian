using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using TMPro;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviour, IDataPersistence
{
    //test
    public PhotonView GM;

    //singleton
    public static GameManager ins;
    //public ABCanvas abCanvas;

    //the object to move on the node
    public List<GameObject> avatars;

    public Node startingNode;

    //[HideInInspector]
    public Node currentNode;

    public Node nextNode;

    //flag variable for first artifact offer & first quest offer
    //not used anymore?
    public bool firstArtifactOfferGiven;
    public bool firstQuestOfferGiven;

    //for lerping
    public bool startMoving = false;
    public bool startMoving2 = false;

    //the speed of moving
    public float moveSpeed;

    //used for current characters blinking
    public float blinkSpeed;
    public float blinkTimer;

    //default time
    public float timer;
    public float timer2;

    //total turns in game
    public int endTime;

    //boolean to track if game is past midway
    public bool lateGame;

    //actually lets rather use int value from 1-3 to see if its early game, midgame or endgame)
    public int gameStage;

    //this use to change the difficultyvalues of progressive locations, not game setups difficulty level
    //starting difficulty function was changed in v0.7.1.
    //now adjusts combat encoutner difficulty only
    public int startingDifficulty;
    public int scoreModifier;

    //agent
    public GameObject agent;

    //for movementBonus
    public Vector3 lerpPos;
    public Vector3 lerpPos2;

    //turn info
    public int numberOfPlayers = 0;
    public int turnNumber = 0;

    //variable to keep track of which player is moving (in case of turn ending before movementBonus is complete)
    public int movementNumber = 0;

    //for keeping track of event cards on the display
    public int eventCards;

    //these have misleading names, perhaps should rename?
    public GameObject handCardArea;
    public GameObject artifactCardArea;
    public GameObject equipmentCardArea;
    public GameObject effectCardArea;
    public GameObject levelupCardArea;
    public GameObject levelupCardArea2;
    public GameObject levelupCardArea3;
    public GameObject combatCardArea;
    public GameObject difficultyCardArea;

    public GameObject eventCardArea;
    public GameObject cardOptionsArea;
    public GameObject cardOptionsCanvas;
    public GameObject objectiveCardArea;
    public GameObject artifactOptionsArea;
    public GameObject artifactOptionsCanvas;
    public GameObject dialogCanvas;
    public GameObject questCanvas;
    public GameObject eventCanvas;
    public GameObject finalWindows;
    public GameObject characterDisplays;
    public GameObject skillCheckHandler;
    public GameObject questOfferDisplay;
    public GameObject artifactOfferDisplay;
    public GameObject startingCardSelect;
    public GameObject locations;
    public GameObject scoreCanvasController;
    public GameObject exploreHandler;
    public GameObject encounterHandler;
    public GameObject sfxPlayer;

    //texts for card option canvases
    public Text artifactOptionsText;
    public Text cardOptionsText;

    //for the turn indicator
    public GameObject turnIndicator;
    public TextMeshProUGUI turnIndicatorText;
    public TextMeshProUGUI turnTimerCounterText;
    public TextMeshProUGUI turnsLeftText;

    //decks
    public List<GameObject> questDeck = new List<GameObject>();
    public List<GameObject> intelligenceDeck = new List<GameObject>();
    public List<GameObject> artifactDeck = new List<GameObject>();
    public List<GameObject> eventDeck = new List<GameObject>();
    public List<GameObject> pyramidOffer = new List<GameObject>();
    public List<GameObject> sonyasOffer = new List<GameObject>();
    public List<GameObject> perkDeck = new List<GameObject>();
    public List<GameObject> questOffer = new List<GameObject>();

    //list of hero icons
    public List<GameObject> characterIcons = new List<GameObject>();

    //list of incoming events, always has 3 cards
    public List<GameObject> eventsIncoming = new List<GameObject>();

    //used for quest plates
    public GameObject questPlateHandler;

    //these should have reference
    public GameObject toolTipBackground;
    public TextMeshProUGUI toolTipText;

    //references for invisible assets
    public GameObject consoleBorders;
    public Text hccText;

    //need reference for this
    public UiButtonHandler uiButtonHandler;

    //use this for extra references
    public References references;
    public SpecialVariables specialVariables;
    public MapRandomizeHandler mapRandomizeHandler;

    public new Camera camera;

    public Node testNode;

    //used by sub area transits (so that hero stays invisible during transits)
    public bool keepHeroInvisible;

    //public float colorChangeValue;
    //public bool colorChangeUp;

    void Awake()
    {
        currentNode = startingNode;
        nextNode = startingNode;
        //firstArtifactOfferGiven = false;
        lateGame = false;

        //set this off for now
        //gameStage = 2;

        //test
        GM = GetComponent<PhotonView>();

        // very bad singleton
        ins = this;
        //abCanvas.gameObject.SetActive(false);

        //resets the decks (this is removed)
        //CardReset();

        //this should be RPC called actually
        //ArtifactReset();

        //QuestOfferReset();
    }

    // Start is called before the first frame update
    void Start()
    {
        //for MP, is this used?
        Client c = FindObjectOfType<Client>();

        blinkTimer = 0;

        references.soundManager.PlayDayMusic2();

        DisplayMapInfo();

        //set this here for now?
        GameManager.ins.specialVariables.currentSceneIndex = 3;
    }

    private void Update()
    {
        /*used by upgrade option blinking
        if (colorChangeUp == true)
        {
            colorChangeValue += 0.0025f;
        }
        else if (colorChangeUp == false)
        {
            //used by upgrade option blinking
            colorChangeValue -= 0.0025f;
        }
        if (colorChangeValue < 0.7f && colorChangeUp == false)
        {
            colorChangeUp = true;
        }
        else if (colorChangeValue > 1f && colorChangeUp == true)
        {
            colorChangeUp = false;
        }
        */
        //for turn timer
        //lets deactivate this for now, remove last condition later
        if (ins.avatars.Count > 0 && PhotonRoom.room.isPractice == false && PhotonRoom.room.isPractice == true)
        {
            if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() && ins.avatars[ins.turnNumber].GetComponent<CharController>().isAi == false)
            {
                if (ins.avatars[ins.turnNumber].GetComponent<CharController>().countTurnTimer == true)
                {
                    ins.avatars[ins.turnNumber].GetComponent<CharController>().turnTimer -= Time.deltaTime;

                    int minutes = Mathf.FloorToInt(ins.avatars[ins.turnNumber].GetComponent<CharController>().turnTimer / 60F);
                    int seconds = Mathf.FloorToInt(ins.avatars[ins.turnNumber].GetComponent<CharController>().turnTimer - minutes * 60);
                    string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

                    turnTimerCounterText.text = niceTime;

                    //give alert at 15s remaining, and every 3 seconds after
                    if (ins.avatars[ins.turnNumber].GetComponent<CharController>().turnTimer < ins.avatars[ins.turnNumber].GetComponent<CharController>().giveAlertAt)
                    {
                        if (ins.avatars[ins.turnNumber].GetComponent<CharController>().giveAlertAt == 15)
                        {
                            //give message
                            string msgs = "You are running low on time!";
                            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                        }

                        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayAlert();
                        ins.avatars[ins.turnNumber].GetComponent<CharController>().giveAlertAt -= 3;
                    }

                    if (ins.avatars[ins.turnNumber].GetComponent<CharController>().turnTimer <= 0)
                    {
                        //give message
                        string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " ran out of time and lost action!";
                        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

                        Clock.clock.TurnTimeEnded();
                        ins.avatars[ins.turnNumber].GetComponent<CharController>().EndTurn();
                        //ins.avatars[ins.turnNumber].GetComponent<CharController>().countTurnTimer = false;
                    }
                }
            }
        }

        blinkTimer += Time.deltaTime * blinkSpeed;

        //lets try this for blinking 
        if(blinkTimer > 100)
        {
            //lets try checking this first
            if (ins.avatars.Count > 0)
            {
                if (ins.avatars[ins.turnNumber] != null)
                {
                    ins.avatars[ins.turnNumber].GetComponentInChildren<Canvas>().enabled = false;
                    blinkTimer = 0;
                    Invoke(nameof(ReEnableCharacter), 0.15f);
                }
            }
        }

        //opens menu with esc clicked
        if (Input.GetKeyDown("escape"))
        {
            if (dialogCanvas.GetComponent<CanvasController>().optionsPanel.gameObject.activeSelf)
            {
                dialogCanvas.GetComponent<CanvasController>().OnBackButtonClicked();
            }
            else
            {
                dialogCanvas.GetComponent<CanvasController>().OnOptionsButtonClicked();
            }
        }

        //opens inventory with I clicked
        //lets put exceptions for when console is open for now
        if (Input.GetKeyDown("i") && !ins.uiButtonHandler.consoleFrame.activeSelf)
        {
            //if (GameManager.ins.uiButtonHandler.equipmentDisplay.gameObject.activeSelf)
            GameManager.ins.uiButtonHandler.EquipmentButtonPressed();
        }

        /* not in v94+
         * opens stats sheet with S clicked
        if (Input.GetKeyDown("s"))
        {
            //if (GameManager.ins.uiButtonHandler.equipmentDisplay.gameObject.activeSelf)
            GameManager.ins.uiButtonHandler.StatsButtonPressed();
        }
        */

        //opens console sheet with C clicked
        if (Input.GetKeyDown("c") && !ins.uiButtonHandler.consoleFrame.activeSelf)
        {
            //if (GameManager.ins.uiButtonHandler.equipmentDisplay.gameObject.activeSelf)
            GameManager.ins.uiButtonHandler.ConsoleButtonPressed();
        }

        //opens upgrade optionst with U clicked
        if (Input.GetKeyDown("u") && !ins.uiButtonHandler.consoleFrame.activeSelf)
        {
            //if (GameManager.ins.uiButtonHandler.equipmentDisplay.gameObject.activeSelf)
            GameManager.ins.uiButtonHandler.UpgradeButtonPressed();
        }

        // for v0.5.7.
        //pauses battle when space is clicked (combat only)
        if (Input.GetKeyDown("space") && !ins.uiButtonHandler.consoleFrame.activeSelf && ins.encounterHandler.GetComponent<EncounterHandler>().battlefieldDisplay.activeSelf)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().BattlefieldCombatPauseButton();
        }

        /* changes quest view when tab clicked
         * lets not use this now
        if (Input.GetKeyDown("tab"))
        {
            questPlateHandler.GetComponent<QuestPlates>().QuestPlateButton();
        }
        */

        /* irrelevant since v88
         * lets try use this for interactions
        if ((Input.GetMouseButtonDown(1)) && currentNode.inter != null && currentNode.inter.enabled == true)
        {
            //check if the options canvas is activated
            if (dialogCanvas.GetComponent<CanvasController>().optionsPanelActivated == false)
            {
                //currentNode.RightClick();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().OpenActionWindow();
            }
        }
        */

        //lets try use this for lerping
        //this is used by the movebetweennodes method in node class
        if (startMoving == true)
        {
            //could use fixedDeltaTime or timeScale also, but effect seems to be the same, still too long animation
            timer += Time.deltaTime * moveSpeed;

            //checks if the avatars current position differs from its destination
            //if (ins.avatars[ins.turnNumber].gameObject.transform.position != lerpPos) 
            if (ins.avatars[ins.movementNumber].gameObject.transform.position != lerpPos)
            {
                //do the lerp
                ins.avatars[ins.movementNumber].gameObject.transform.position = Vector3.Lerp(ins.avatars[ins.movementNumber].gameObject.transform.position, lerpPos, timer);
            }

            else if (ins.avatars[ins.movementNumber].gameObject.transform.position == lerpPos)
            {
                //stops running animation
                //ins.avatars[ins.turnNumber].GetComponentInChildren<Animator>().SetBool("Run", false);

                timer = 0;
                startMoving = false;
                //ins.avatars[ins.movementNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().MoveBetweenNodesContinued();
            }
        }

        //lets try use this as an alternate, to prevent movementBonus from "stucking" for previous player
        //only called for current players turn, called from Arrive-method from the Node-class
        //bit shame to use so many duplicate variables, but dont see other way of doing this
        //this is used by the arrive method in node class
        if (startMoving2 == true)
        {
            //could use fixedDeltaTime or timeScale also, but effect seems to be the same, still too long animation
            timer2 += Time.deltaTime * moveSpeed;

            //checks if the avatars current position differs from its destination
            if (ins.avatars[ins.turnNumber].gameObject.transform.position != lerpPos2)
            {
                //do the lerp
                ins.avatars[ins.turnNumber].gameObject.transform.position = Vector3.Lerp(ins.avatars[ins.turnNumber].gameObject.transform.position, lerpPos2, timer2);
            }

            else if (ins.avatars[ins.turnNumber].gameObject.transform.position == lerpPos2)
            {
                //stops running animation
                //ins.avatars[ins.turnNumber].GetComponentInChildren<Animator>().SetBool("Run", false);

                timer2 = 0;
                startMoving2 = false;
                ins.avatars[ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().ArriveContinued();

            }
        }
    }

    void ReEnableCharacter()
    {
        if (ins.avatars[ins.turnNumber] != null && keepHeroInvisible == false)
        {
            ins.avatars[ins.turnNumber].GetComponentInChildren<Canvas>().enabled = true;
        }
    }

    /* old card methods
     * takes available quest card number
    public int TakeQuestCardNumber()
    {
        int i = 0;

        do
        {
            int cardNumber = Random.Range(0, questDeck.Count);

            //int cardNumber = Random.Range(40, 50);

            if (questDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                int rarityCheck = Random.Range(1, 11);

                //raritychecks for both early and lategame
                if (lateGame == false && rarityCheck <= questDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity)
                {
                    ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
                if (lateGame == true && rarityCheck <= questDeck[cardNumber].GetComponent<CardDisplay>().lateRarity)
                {
                    ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //takes available quest card number, and makes sure theres not more than 2 active cards at the location per player
    //was used for board quest card draws
    //since v82 this is used for seek quest actions
    // rerolls 9 times, if quest is not destined to current players location
    public int TakeQuestCardNumber2()
    {
        int i = 0;

        int rerolls = 9;

        do
        {
            int cardNumber = Random.Range(0, questDeck.Count);

            //int cardNumber = Random.Range(45, 60);

            if (questDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                if (questDeck[cardNumber].GetComponent<QuestCard>().qDestination != ins.avatars[ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber &&
                    rerolls > 0)
                {
                    rerolls -= 1;
                }

                else
                {
                    //take location number -1
                    int y = questDeck[cardNumber].GetComponent<QuestCard>().qDestination - 1;

                    int numberOfQuests = numberOfOwnedQuests(y);

                    if (numberOfQuests < 2)
                    {
                        int rarityCheck = Random.Range(1, 11);

                        //raritychecks for both early and lategame
                        if (lateGame == false && rarityCheck <= questDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity)
                        {
                            return cardNumber;
                        }
                        if (lateGame == true && rarityCheck <= questDeck[cardNumber].GetComponent<CardDisplay>().lateRarity)
                        {
                            return cardNumber;
                        }
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    public int numberOfOwnedQuests(int location)
    {
        int numberOfQuests = 0;

        for(int i = 0; i < questPlateHandler.GetComponent<QuestPlates>().questPlates[location].GetComponent<Transform>().childCount; i++)
        {
            if (questPlateHandler.GetComponent<QuestPlates>().questPlates[location].GetComponent<Transform>().GetChild(i).gameObject.activeSelf)
            {
                numberOfQuests += 1;
            }
        }
        return numberOfQuests;
    }

    //another way of doing this (checks the belongs to variable)
    public int numberOfOwnedQuests2(int location)
    {
        int numberOfQuests = 0;

        for (int i = 0; i < questPlateHandler.GetComponent<QuestPlates>().questPlates[location].GetComponent<Transform>().childCount; i++)
        {
            if (questPlateHandler.GetComponent<QuestPlates>().questPlates[location].GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay>().belongsTo == ins.turnNumber)
            {
                numberOfQuests += 1;
            }
        }
        return numberOfQuests;
    }


    //takes available quest card number
    //for current player only
    //rerolls 9 times, to find specific int cards depending on location
    public int TakeIntelligenceCardNumber()
    {
        int i = 0;

        int rerolls = 9;

        do
        {
            int cardNumber = Random.Range(0, intelligenceDeck.Count);

            //int cardNumber = Random.Range(24, intelligenceDeck.Count);

            if (intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                if (rerolls > 0 && TestIntCard(cardNumber) == false)
                {
                    rerolls -= 1;
                }
                else
                {
                    //check if the player has the class required
                    for (int y = 0; y < ins.avatars[ins.turnNumber].GetComponentInChildren<Character>().heroClasses.Count; y++)
                    {
                        int rarityCheck = Random.Range(1, 11);

                        //raritychecks for both early and lategame
                        if (lateGame == false && rarityCheck <= intelligenceDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity &&
                            ins.avatars[ins.turnNumber].GetComponentInChildren<Character>().heroClasses[y] == intelligenceDeck[cardNumber].GetComponent<IntelligenceCard>().classReq)
                        {
                            return cardNumber;
                        }
                        if (lateGame == true && rarityCheck <= intelligenceDeck[cardNumber].GetComponent<CardDisplay>().lateRarity &&
                            ins.avatars[ins.turnNumber].GetComponentInChildren<Character>().heroClasses[y] == intelligenceDeck[cardNumber].GetComponent<IntelligenceCard>().classReq)
                        {
                            return cardNumber;
                        }
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //sees if the location is likely to have that int card
    public bool TestIntCard(int cardNumber)
    {
        int effectNumber = ins.intelligenceDeck[cardNumber].GetComponent<IntelligenceCard>().effect;

        int nodeNumber = ins.avatars[ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber;

        //smithy cards
        if (nodeNumber == 1 && (effectNumber == 31 || effectNumber == 19))
        {
            return true;
        }
        //inn cards
        if (nodeNumber == 2 && (effectNumber == 32 || effectNumber == 13))
        {
            return true;
        }
        //wilforge cards
        if (nodeNumber == 3 && (effectNumber == 17 || effectNumber == 32))
        {
            return true;
        }
        //oldmines cards
        if (nodeNumber == 4 && (effectNumber == 9 || effectNumber == 16))
        {
            return true;
        }
        //factory cards
        if (nodeNumber == 5 && (effectNumber == 34 || effectNumber == 10 || effectNumber == 11))
        {
            return true;
        }
        //temple cards
        if (nodeNumber == 6 && (effectNumber == 61 || effectNumber == 20))
        {
            return true;
        }
        //grimhold cards
        if (nodeNumber == 7 && (effectNumber == 45 || effectNumber == 14))
        {
            return true;
        }
        //citadel cards
        if (nodeNumber == 8 && (effectNumber == 62 || effectNumber == 12))
        {
            return true;
        }
        //vault cards
        if (nodeNumber == 9 && (effectNumber == 44 || effectNumber == 21))
        {
            return true;
        }
        //coven cards
        if (nodeNumber == 10 && (effectNumber == 43 || effectNumber == 18))
        {
            return true;
        }
        //valley cards
        if (nodeNumber == 11 && (effectNumber == 22 || effectNumber == 6))
        {
            return true;
        }
        //guildhouse cards
        if (nodeNumber == 12 && (effectNumber == 45 || effectNumber == 41))
        {
            return true;
        }
        //cornville cards
        if (nodeNumber == 13 && (effectNumber == 13 || effectNumber == 9))
        {
            return true;
        }

        return false;
    }

    //takes intelligence card from deck, for specific hero
    /*remove this soon
    public GameObject TakeIntelligenceCard(int turnNumber2)
    {

        int i = 0;

        do
        {
            GameObject playerCard = intelligenceDeck[Random.Range(0, intelligenceDeck.Count)];

            if (playerCard.GetComponent<CardDisplay>().isTaken == false)
            {
                //check if the player has the class required
                for (int y = 0; y < ins.avatars[turnNumber2].GetComponentInChildren<Character>().heroClasses.Count; y++)
                {
                    if (ins.avatars[turnNumber2].GetComponentInChildren<Character>().heroClasses[y] == playerCard.GetComponent<IntelligenceCard>().classReq)
                    {
                        playerCard.GetComponent<CardDisplay>().isTaken = true;
                        return playerCard;
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return null;
    }
    */

    /* more old card methods
     * takes intelligence card number from deck, for specific hero
    //replaces the earlier method
    public int TakeIntelligenceCardNumber2(int turnNumber2)
    {
        int i = 0;

        do
        {
            int cardNumber = Random.Range(0, intelligenceDeck.Count);

            if (intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                //check if the player has the class required
                for (int y = 0; y < ins.avatars[turnNumber2].GetComponentInChildren<Character>().heroClasses.Count; y++)
                {
                    int rarityCheck = Random.Range(1, 11);

                    //raritychecks for both early and lategame
                    if (lateGame == false && rarityCheck <= intelligenceDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity &&
                        ins.avatars[turnNumber2].GetComponentInChildren<Character>().heroClasses[y] == intelligenceDeck[cardNumber].GetComponent<IntelligenceCard>().classReq)
                    {
                        return cardNumber;
                    }
                    if (lateGame == true && rarityCheck <= intelligenceDeck[cardNumber].GetComponent<CardDisplay>().lateRarity &&
                        ins.avatars[turnNumber2].GetComponentInChildren<Character>().heroClasses[y] == intelligenceDeck[cardNumber].GetComponent<IntelligenceCard>().classReq)
                    {
                        return cardNumber;
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //takes artifact card from deck
    //not actually used atm
    public GameObject TakeArtifactCard()
    {
        int i = 0;

        do
        {
            GameObject playerCard = artifactDeck[Random.Range(0, artifactDeck.Count)];

            if (playerCard.GetComponent<CardDisplay>().isTaken == false)
            {
                playerCard.GetComponent<CardDisplay>().isTaken = true;
                return playerCard;
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return null;
    }

    //takes available quest card number
    //has rarity check also now
    public int TakeArtifactCardNumber()
    {
        int i = 0;

        do
        {
            int cardNumber = Random.Range(0, artifactDeck.Count);

            //int cardNumber = Random.Range(12, 23);

            if (artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                //return cardNumber;

                int rarityCheck = Random.Range(1, 11);

                //raritychecks for both early and lategame
                if (lateGame == false && rarityCheck <= artifactDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity)
                {
                    artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
                if (lateGame == true && rarityCheck <= artifactDeck[cardNumber].GetComponent<CardDisplay>().lateRarity)
                {
                    artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //takes event card number from deck
    public int TakeEventCardNumber()
    {
        int i = 0;

        do
        {
            int cardNumber = Random.Range(2, eventDeck.Count);

            //int cardNumber = Random.Range(20, 28);

            if (eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                //"removes" the card from deck
                //eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                //return cardNumber;

                int rarityCheck = Random.Range(1, 11);

                //raritychecks for both early and lategame
                if (lateGame == false && rarityCheck <= eventDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity)
                {
                    eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
                if (lateGame == true && rarityCheck <= eventDeck[cardNumber].GetComponent<CardDisplay>().lateRarity)
                {
                    eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 2;
    }

    //used by the wish intelligence card, has fixed set of possible cards
    //note that we might need to change this later, if more rare cards are added
    public int TakeWishCardNumber()
    {
        int i = 0;

        do
        {
            //doesnt matter that harsh sentencing is on the list, since it has 0 rarity
            int cardNumber = Random.Range(18, eventDeck.Count);

            //int cardNumber = Random.Range(22, 29);

            if (eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken == false)
            {
                //"removes" the card from deck
                //eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                //return cardNumber;

                int rarityCheck = Random.Range(1, 11);

                //raritychecks for both early and lategame
                if (lateGame == false && rarityCheck <= eventDeck[cardNumber].GetComponent<CardDisplay>().earlyRarity)
                {
                    eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
                if (lateGame == true && rarityCheck <= eventDeck[cardNumber].GetComponent<CardDisplay>().lateRarity)
                {
                    eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    return cardNumber;
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 2;
    }
    */

    /* old return quest method
    //returns quest card to deck
    public void ReturnQuestCard(int card, int numberInDeck, int deckType)
    {
        //GameObject playerCard = handCardArea.transform.GetChild(card).gameObject;
        //questDeck.Add(Instantiate(handCardArea.transform.GetChild(card).gameObject));
        //questDeck.Add(playerCard);

        //decktype 1 returns card from handcard area back to deck
        if (deckType == 1)
        {
            Destroy(handCardArea.transform.GetChild(card).gameObject);
            questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = false;

            //get the location number, and remove the quest card from that quest plate too
            int locationNb = ins.questDeck[numberInDeck].GetComponent<QuestCard>().qDestination - 1;

            //check that the reference equals to the original reference
            for (int i = 0; i < questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.childCount; i++)
            {
                //if (ReferenceEquals(temporaryCard, GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject))
                if (numberInDeck == questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck)
                {
                    //remove card from board
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ReturnCard(i, numberInDeck, 7, 5);
                    Destroy(questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject);
                }
            }
        }

        //decktype 2 is card options area, works in reverse as in decktype 1
        //could make the whole operation here actually, for simplicity
        //this only works for 1 out of 4 options choice now
        if (deckType == 2)
        {
            //instantiates the card from options area
            //GameObject playerCard = Instantiate(cardOptionsArea.transform.GetChild(card).gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject playerCard = Instantiate(questDeck[numberInDeck].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(handCardArea.transform, false);

            int card1 = cardOptionsArea.transform.GetChild(0).GetComponent<CardDisplay>().numberInDeck;
            int card2 = cardOptionsArea.transform.GetChild(1).GetComponent<CardDisplay>().numberInDeck;
            int card3 = cardOptionsArea.transform.GetChild(2).GetComponent<CardDisplay>().numberInDeck;
            int card4 = cardOptionsArea.transform.GetChild(3).GetComponent<CardDisplay>().numberInDeck;

            //destroys all cards options on area
            Destroy(cardOptionsArea.transform.GetChild(3).gameObject);
            Destroy(cardOptionsArea.transform.GetChild(2).gameObject);
            Destroy(cardOptionsArea.transform.GetChild(1).gameObject);
            Destroy(cardOptionsArea.transform.GetChild(0).gameObject);

            
            //turns the card inactive
            //probably need to do this here, since the quest offer gets generated from different route than usual
            playerCard.SetActive(false);

            //unless its your turn
            //places the quest card on board here also
            if (avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }

            //except the chosen one
            questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = true;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = 0;

            //set the "owner" variable to the card
            playerCard.GetComponent<CardDisplay>().belongsTo = ins.turnNumber;

            if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //place the card on quest plates also
                questPlateHandler.GetComponent<QuestPlates>().PV.RPC("RPC_DrawQuestCardOnBoard", RpcTarget.AllBufferedViaServer, numberInDeck);
            }

            //remove the card from questoffer list & draw new card there
            questOffer.RemoveAt(card);

            //destroys the selected card from quest offer display
            Destroy(questOfferDisplay.transform.GetChild(card).gameObject);

            //add new card to the offer
            if (ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //int cardNumber = ins.TakeQuestCardNumber();

                //ins.GM.RPC("RPC_QuestOffer", RpcTarget.AllBufferedViaServer, cardNumber);
            }

            //in case you doing the mystic mushrooms quest
            // not actually used anymore? might leave it in just in case it will get re-used
            if (ins.questDeck[questCanvas.GetComponent<QuestingDialog>().numberInDeck].GetComponent<QuestCard>().specialEffect == 3)
            {
                questCanvas.GetComponent<QuestingDialog>().counter -= 1;

                if (questCanvas.GetComponent<QuestingDialog>().counter > 0 && ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    questCanvas.GetComponent<QuestingDialog>().OpenQuestOptions();
                    return;
                }
                else if (questCanvas.GetComponent<QuestingDialog>().counter == 0 && ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //ends the players turn
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                    questCanvas.GetComponent<QuestingDialog>().QuestEnd();

                    //this shuts down the dialog 
                    cardOptionsCanvas.gameObject.SetActive(false);
                }

                //reset the variable, just in case
                questCanvas.GetComponent<QuestingDialog>().numberInDeck = 0;
            }

            else
            {

                //ends the players turn
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();

                //this shuts down the dialog 
                //GameObject.Find("Card Options Canvas").gameObject.SetActive(false);
                cardOptionsCanvas.gameObject.SetActive(false);
            }
        }

        //decktype 3 removes card from handcard area permanently, and it doesnt return to deck
        //change this later to take into account replenishing cards, actually done now but not tested
        if (deckType == 3)
        {
            Destroy(handCardArea.transform.GetChild(card).gameObject);
            questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = true;

            if (questDeck[numberInDeck].GetComponent<QuestCard>().respawns == true)
            {
                questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = false;
            }
        }

        //decktype 4 removes card from handcard area permanently, and returns to deck
        //used for valley interaction, regret int card and offering quest
        //also used for memory steal, in case you stole card which you shouldnt (over 2 cards on that destination alrdy)
        if (deckType == 4)
        {
            Debug.Log("decktype 4");

            Destroy(handCardArea.transform.GetChild(card).gameObject);
            questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = false;
            
            //in case youre choosing starting cards
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().startingCardsSelected == false)
            {
                startingCardSelect.GetComponent<StartingCardSelection>().DiscardCard();
            }

            //in case you doing the offering quest
            else if (ins.questDeck[questCanvas.GetComponent<QuestingDialog>().numberInDeck].GetComponent<QuestCard>().specialEffect == 8)
            {
                questCanvas.GetComponent<QuestingDialog>().TheOffering();
            }

            //in case you using regret card
            else if (dialogCanvas.GetComponent<CanvasController>().temporaryNumber == 14)
            {
                dialogCanvas.GetComponent<CanvasController>().RegretReward();
            }

            //otherwise do the valley interaction
            else
            {
                dialogCanvas.GetComponent<CanvasController>().ValleyReward();
            }

            //get the location number, and remove the quest card from that quest plate too
            int locationNb = ins.questDeck[numberInDeck].GetComponent<QuestCard>().qDestination - 1;

            //check that the reference equals to the original reference
            for (int i = 0; i < questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.childCount; i++)
            {
                //if (ReferenceEquals(temporaryCard, GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject))
                if (numberInDeck == questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck)
                {
                    //remove card from board
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ReturnCard(i, numberInDeck, 7, 5);
                    Destroy(questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject);
                }
            }
        }

        //decktype 5 removes card from board quest plate permanently, and it doesnt return to deck
        //change this later to take into account replenishing cards, actually done now but not tested
        if (deckType == 5)
        {
            int locationNb = ins.avatars[ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber - 1;

            Destroy(questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(card).gameObject);
            questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = true;

            if (questDeck[numberInDeck].GetComponent<QuestCard>().respawns == true)
            {
                questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = false;
            }
        }

        //decktype 10 switches the ownership of a card
        //for memory steal int card
        if (deckType == 10)
        {
            //get the location number, and remove the quest card from that quest plate too
            int locationNb = ins.questDeck[numberInDeck].GetComponent<QuestCard>().qDestination - 1;

            bool tooManyQuests = false;

            //check that the reference equals to the original reference
            for (int i = 0; i < questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.childCount; i++)
            {
                //if (ReferenceEquals(temporaryCard, GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject))
                if (numberInDeck == questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck)
                {
                    //turns the card inactive to all except current player
                    questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject.SetActive(false);

                    questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).GetComponent<CardDisplay>().belongsTo = ins.turnNumber;

                    if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                    {
                        questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject.SetActive(true);
                    }
                    //check that you dont alrdy have 2 quests directed at quests destination
                    //bit weird way of doing this but..
                    //if (numberOfOwnedQuests2(locationNb) > 2)
                    {
                        tooManyQuests = true;

                        Destroy(questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject);
                    }
                }
            }

            for (int i = 0; i < handCardArea.GetComponent<Transform>().childCount; i++)
            {
                // switches the ownership of the card in question
                if (handCardArea.transform.GetChild(i).gameObject.GetComponent<QuestCard>() != null &&
                    handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck == numberInDeck)
                {
                    handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo = ins.turnNumber;

                    //turns the card inactive to all except current player
                    handCardArea.transform.GetChild(i).gameObject.SetActive(false);

                    if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                    {
                        handCardArea.transform.GetChild(i).gameObject.SetActive(true);

                        //give message
                        string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " picked the brain of " +
                            GameManager.ins.avatars[dialogCanvas.GetComponent<AttackResolve>().targetTurnNumber].GetComponentInChildren<Character>().heroName;
                        GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                    }

                    //check that you dont alrdy have 2 quests directed at quests destination
                    //bit weird way of doing this but..
                    if (tooManyQuests == true)
                    {
                        if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                        {
                            //give message
                            string msgs2 = "You already have 2 quests in that location, gain Favor instead.";
                            //GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_PrivateMessage", RpcTarget.AllBufferedViaServer, msgs2, ins.turnNumber);

                            ins.avatars[ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
                        }
                        Destroy(handCardArea.transform.GetChild(i).gameObject);
                        //Debug.Log("card is: " + card);
                    }

                    if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                    {
                        //destroys displayed cards
                        GameObject.Find("Spying").GetComponent<SpyCanvas>().DestroyDisplayedCards();

                        //closes spy dialog and ends turn
                        //activate the canvas
                        GameObject.Find("Spying").GetComponent<SpyCanvas>().spyCanvas.SetActive(false);

                        //ins.avatars[ins.turnNumber].GetComponent<CharController>().EndTurn();
                        ins.avatars[ins.turnNumber].GetComponent<CharController>().Cancel();
                        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
                    }
                }
            }
            //need to reset this just in case (although cancel method alrdy does this?)
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;
        }

        //decktype 18 is for choosing quest from offer as straight action
        if (deckType == 18)
        {
            //instantiates the card from options area
            //GameObject playerCard = Instantiate(cardOptionsArea.transform.GetChild(card).gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject playerCard = Instantiate(questDeck[numberInDeck].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(handCardArea.transform, false);

            //turns the card inactive
            //probably need to do this here, since the quest offer gets generated from different route than usual
            playerCard.SetActive(false);

            //unless its your turn
            //places the quest card on board here also
            if (avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true && avatars[ins.turnNumber].GetComponent<CharController>().isAi == false)
            {
                playerCard.SetActive(true);
            }

            //except the chosen one
            questDeck[numberInDeck].GetComponent<CardDisplay>().isTaken = true;

            ins.avatars[ins.turnNumber].GetComponent<CharController>().afterEffect = 0;

            //set the "owner" variable to the card
            playerCard.GetComponent<CardDisplay>().belongsTo = ins.turnNumber;

            //lets allow ai to use this too, for now
            if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)// && ins.avatars[ins.turnNumber].GetComponent<CharController>().isAi == false)
            {
                //place the card on quest plates also
                questPlateHandler.GetComponent<QuestPlates>().PV.RPC("RPC_DrawQuestCardOnBoard", RpcTarget.AllBufferedViaServer, numberInDeck);
            }
            
            //questPlateHandler.GetComponent<QuestPlates>().PV.RPC("RPC_DrawQuestCardOnBoard", RpcTarget.AllBufferedViaServer, numberInDeck);

            //remove the card from questoffer list & draw new card there
            questOffer.RemoveAt(card);

            //destroys the selected card from quest offer display
            Destroy(questOfferDisplay.transform.GetChild(card).gameObject);

            //add new card to the offer
            if (ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //int cardNumber = ins.TakeQuestCardNumber();

                //ins.GM.RPC("RPC_QuestOffer", RpcTarget.AllBufferedViaServer, cardNumber);
            }

            //ends the players turn with delay?
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
            Invoke("EndCurrentTurnWithDelay", 0.5f);
        }
    
        //makes the check after any method used here
        //just make sure theres no return statements before this
        //perhaps try with invoke if it fixes the wrong hand count after hand quest
        Invoke("CheckHandLimit", 0.1f);
    }
    */

    void EndCurrentTurnWithDelay()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
    }

    //method for checking number of hand cards of each player
    //calculates all the hand cards for each player each time this method is launched
    //might be heavy operation.. but not used too often?
    public void CheckHandLimit()
    {
        //resets the counter
        for (int i = 0; i < ins.avatars.Count; i++)
        {
            ins.avatars[i].GetComponentInChildren<Character>().handCards = 0;
        }

        //counts the cards each player has
        for (int i = 0; i < handCardArea.GetComponent<Transform>().childCount; i++)
        {
            ins.avatars[handCardArea.transform.GetChild(i).GetComponent<CardDisplay>().belongsTo].GetComponentInChildren<Character>().handCards += 1;
        }

        //checks if hand cards are over 8
        //also updates the text counter display
        for (int i = 0; i < ins.avatars.Count; i++)
        {
            //checks if handcards go above 8
            if (ins.avatars[i].GetComponentInChildren<Character>().handCards > 8)
            {
                RemoveHandCardFromPlayer(i);
            }

            //updates the display for that player only, and only human players
            if (ins.avatars[i].GetComponent<CharController>().ItsYourTurn() == true && ins.avatars[i].GetComponent<CharController>().isAi == false)
            {
                ins.avatars[i].GetComponentInChildren<Character>().hccText.text = ins.avatars[i].GetComponentInChildren<Character>().handCards.ToString() + " / 8";
            }
        }
    }

    public void RemoveHandCardFromPlayer(int player)
    {
        //take random number between 1 and 8
        int cardToRemove = Random.Range(1, 9);

        int cardCounter = 0;

        //selects the card to remove
        for (int i = 0; i < handCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (handCardArea.transform.GetChild(i).GetComponent<CardDisplay>().belongsTo == player)
            {
                cardCounter += 1;

                if (cardCounter == cardToRemove)
                {
                    CardDisplay card = handCardArea.transform.GetChild(i).GetComponent<CardDisplay>();

                    if (card.cardType == 7)
                    {
                        //ReturnQuestCard(i, card.numberInDeck, 1);
                        Destroy(handCardArea.transform.GetChild(i).gameObject);
                        questDeck[card.numberInDeck].GetComponent<CardDisplay>().isTaken = false;

                        ins.avatars[player].GetComponentInChildren<Character>().handCards -= 1;

                        if (ins.avatars[player].GetComponent<CharController>().ItsYourTurn() == true)
                        {
                            //give message
                            string msgs = "You went over hand-limit and lost a card.";
                            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                        }

                        //get the location number, and remove the quest card from that quest plate too
                        int locationNb = ins.questDeck[card.numberInDeck].GetComponent<QuestCard>().qDestination - 1;

                        //check that the reference equals to the original reference
                        for (int y = 0; y < GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().questPlates[locationNb].transform.childCount; y++)
                        {
                            //if (ReferenceEquals(temporaryCard, GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(i).gameObject))
                            if (card.numberInDeck == GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(y).gameObject.GetComponent<CardDisplay>().numberInDeck)
                            {
                                //remove card from board
                                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ReturnCard(i, numberInDeck, 7, 5);
                                Destroy(questPlateHandler.GetComponent<QuestPlates>().questPlates[locationNb].transform.GetChild(y).gameObject);
                            }
                        }

                        //end the method immediately, so theres no stack overflow
                        return;
                    }

                    if (card.cardType == 8)
                    {
                        //ReturnIntelligenceCard(i, card.numberInDeck, 1);
                        Destroy(handCardArea.transform.GetChild(i).gameObject);
                        intelligenceDeck[card.numberInDeck].GetComponent<CardDisplay>().isTaken = false;

                        ins.avatars[player].GetComponentInChildren<Character>().handCards -= 1;

                        if (ins.avatars[player].GetComponent<CharController>().ItsYourTurn() == true)
                        {
                            //give message
                            string msgs = "You went over hand-limit and lost a card.";
                            GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                        }

                        //end the method immediately, so theres no stack overflow
                        return;
                    }

                    //ins.avatars[i].GetComponent<CharController>().ReturnCard(i, card.numberInDeck, card.cardType, 1);

                }
            }
        }
    }

    //reclaims intelligence card after using it, for current player
    public void ReclaimIntelligence(int numberID)
    {
        //GameObject playerCard = intelligenceDeck[numberID];
        //Instantiate(playerCard, new Vector3(0, 0, 0), Quaternion.identity);

        intelligenceDeck[numberID].GetComponent<CardDisplay>().isTaken = true;

        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(intelligenceDeck[numberID], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in hand card area
        playerCard.transform.SetParent(handCardArea.transform, false);

        //turns the card inactive
        playerCard.SetActive(false);

        //set the "owner" variable to the card
        playerCard.GetComponent<CardDisplay>().belongsTo = ins.turnNumber;

        //unless its your turn
        if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            playerCard.SetActive(true);
        }
    }
    /* reclaim intelligence & card options
    //reclaims intelligence card after using it, takes turnnumber
    public void ReclaimIntelligence2(int numberID, int targetTurnNumber)
    {
        //GameObject playerCard = intelligenceDeck[numberID];
        //Instantiate(playerCard, new Vector3(0, 0, 0), Quaternion.identity);

        intelligenceDeck[numberID].GetComponent<CardDisplay>().isTaken = true;

        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(intelligenceDeck[numberID], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in hand card area
        playerCard.transform.SetParent(handCardArea.transform, false);

        //turns the card inactive
        playerCard.SetActive(false);

        //set the "owner" variable to the card
        playerCard.GetComponent<CardDisplay>().belongsTo = targetTurnNumber;

        //unless its your turn
        if (avatars[targetTurnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            playerCard.SetActive(true);
        }
    }


    //cards instantiated by this method wont get "taken" from the original deck
    //for current player only
    public void DrawCardOptions(int cardNumber, int type)
    {
        if (type == 7)
        {
            //instantiates random quest card from the deck
            GameObject playerCard = Instantiate(questDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(cardOptionsArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //unless its your turn
            //places the quest card on board here also
            if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }
        }

        else if (type == 8)
        {
            GameObject playerCard = Instantiate(intelligenceDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            playerCard.transform.SetParent(cardOptionsArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //unless its your turn
            if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }
        }

        //this option draws the 4 cards from pyramid deck to the card options area
        else if (type == 9)
        {
            GameObject playerCard = Instantiate(pyramidOffer[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            playerCard.transform.SetParent(artifactOptionsArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //unless its your turn
            if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }
        }

        //this option draws the 4 cards from sonyas offer list to the artifact options area
        else if (type == 10)
        {
            GameObject playerCard = Instantiate(sonyasOffer[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            playerCard.transform.SetParent(artifactOptionsArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //unless its your turn
            if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }
        }

        //this option draws the 3 cards from incoming events list to the artifact options area
        else if (type == 11)
        {
            GameObject playerCard = Instantiate(eventsIncoming[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            playerCard.transform.SetParent(artifactOptionsArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //unless its your turn
            if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }
        }

        //this option draws cards from event deck to the card options area
        else if (type == 12)
        {
            GameObject playerCard = Instantiate(eventDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            playerCard.transform.SetParent(cardOptionsArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //unless its your turn
            if (avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                playerCard.SetActive(true);
            }
        }
    }
    */

    /* draw events
    //cards instantiated by this method wont get "taken" from the original deck
    //for current player only
    //drawtype 1 shows event popup window, drawtype 2 doesnt
    public void DrawEvent(int cardNumber, int drawType)
    {
        //only effects under 100 are displayed on the event card area
        if (eventDeck[cardNumber].GetComponent<EventCard>().effect < 100)
        {
            //instantiates random quest card from the deck
            GameObject playerCard = Instantiate(eventDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(eventCardArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(true);
        }

        if (drawType == 1)
        {
            eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

            //shows the drawn event card for all (only used for wish atm?)
            eventCanvas.GetComponent<EventHandler>().DisplayEvent(cardNumber);
        }

        if (drawType == 2)
        {
            //"removes" the card from deck
            // needs to be done separately, and inside rpc call
            eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

            //doesnt show the drawn event card
            eventCanvas.GetComponent<EventHandler>().DrawEvent2(cardNumber);
        }

        Invoke("CheckEventCount", 0.1f);
    }

    //cards instantiated by this method wont get "taken" from the original deck
    //for current player only
    public void DrawEventFromList()
    {
        //only effects under 100 are displayed on the event card area
        if (eventsIncoming[0].GetComponent<EventCard>().effect < 100)
        {
            //instantiates random quest card from the deck
            GameObject playerCard = Instantiate(eventsIncoming[0], new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(eventCardArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(true);
        }

        //shows the drawn event card for all
        eventCanvas.GetComponent<EventHandler>().DisplayEvent(eventsIncoming[0].GetComponent<CardDisplay>().numberInDeck);

        //removes the card from list
        ins.eventsIncoming.RemoveAt(0);

        if (ins.avatars[ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            //int cardNumber = TakeEventCardNumber();

            //GM.RPC("RPC_AddEventIncoming", RpcTarget.AllBufferedViaServer, cardNumber);
        }

        Invoke("CheckEventCount", 0.1f);
    }
    */

    //[PunRPC]
    public void AddPlayer(GameObject player)
    {
        avatars.Add(player);
    }

    /* remove mp cards & artifact reset
    void RemoveMpCards()
    {
        //remove quest attack cards
        for (int i = 0; i < questDeck.Count; i++)
        {
            if (questDeck[i].GetComponent<CardDisplay>().numberInDeck == 18 ||
                questDeck[i].GetComponent<CardDisplay>().numberInDeck == 19 ||
                questDeck[i].GetComponent<CardDisplay>().numberInDeck == 40 ||
                questDeck[i].GetComponent<CardDisplay>().numberInDeck == 46 ||
                questDeck[i].GetComponent<CardDisplay>().numberInDeck == 51 ||
                questDeck[i].GetComponent<CardDisplay>().numberInDeck == 52)
            {
                questDeck[i].GetComponent<CardDisplay>().isTaken = true;
            }
        }

        //remove shield, ward & minefield (acquisition allowed now)
        for (int i = 0; i < intelligenceDeck.Count; i++)
        {
            if (intelligenceDeck[i].GetComponent<IntelligenceCard>().effect == 61 ||
                intelligenceDeck[i].GetComponent<IntelligenceCard>().effect == 62 ||
                intelligenceDeck[i].GetComponent<IntelligenceCard>().effect == 34)// || intelligenceDeck[i].GetComponent<IntelligenceCard>().effect == 44)
            {
                intelligenceDeck[i].GetComponent<CardDisplay>().isTaken = true;
            }
        }
    }

    //removes all cards from pyramid offer, and sets new cards
    public void ArtifactReset()
    {
        if (pyramidOffer.Count != 0)
        {
            if (pyramidOffer[2] != null)
            {
                int cardNumber = pyramidOffer[2].GetComponent<CardDisplay>().numberInDeck;
                artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = false;
                pyramidOffer.RemoveAt(2);
            }
            if (pyramidOffer[1] != null)
            {
                int cardNumber = pyramidOffer[1].GetComponent<CardDisplay>().numberInDeck;
                artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = false;
                pyramidOffer.RemoveAt(1);
            }
            if (pyramidOffer[0] != null)
            {
                int cardNumber = pyramidOffer[0].GetComponent<CardDisplay>().numberInDeck;
                artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = false;
                pyramidOffer.RemoveAt(0);
            }
        }
        //draws 3 cards
        for (int i = 0; i < 3; i++)
        {
            pyramidOffer.Add(TakeArtifactCard());
        }
    }
    */

    /*trash
    public void ArtifactOffer()
    {
        //draws 3 cards
        for (int i = 0; i < 3; i++)
        {
            pyramidOffer.Add(TakeArtifactCard());
        }
    }
    

    //this method destroys the artifact offer cards from artifact canvas
    [PunRPC]
    void RPC_DestroyArtifactOffer()
    {
        //int card1 = artifactOptionsArea.transform.GetChild(0).GetComponent<CardDisplay>().numberInDeck;
        //int card2 = artifactOptionsArea.transform.GetChild(1).GetComponent<CardDisplay>().numberInDeck;
        //int card3 = artifactOptionsArea.transform.GetChild(2).GetComponent<CardDisplay>().numberInDeck;
        //int card4 = artifactOptionsArea.transform.GetChild(3).GetComponent<CardDisplay>().numberInDeck;

        //destroys all cards options on area
        Destroy(artifactOptionsArea.transform.GetChild(3).gameObject);
        Destroy(artifactOptionsArea.transform.GetChild(2).gameObject);
        Destroy(artifactOptionsArea.transform.GetChild(1).gameObject);
        Destroy(artifactOptionsArea.transform.GetChild(0).gameObject);

    }

    //this method destroys the event cards displayed on the artifact options canvas (name bit misleading)
    [PunRPC]
    void RPC_DestroyArtifactOffer2()
    {
        //destroys all cards options on area
        Destroy(artifactOptionsArea.transform.GetChild(2).gameObject);
        Destroy(artifactOptionsArea.transform.GetChild(1).gameObject);
        Destroy(artifactOptionsArea.transform.GetChild(0).gameObject);

        ins.artifactOptionsText.text = "Choose one card to buy:";

        ins.artifactOptionsCanvas.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<Text>().text = "Cancel";

        //this need to be reset somewhere, this might be ok place
        ins.artifactOptionsCanvas.GetComponent<Action>().cardOptionType = 0;
    }
    */

    //for updating character classes, called from artifact effects
    [PunRPC]
    void RPC_UpdateClasses(int effect)
    {
        //way of the sword
        if (effect == 15)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroClasses.Add(1);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence += 1;
        }

        //engineers guidebook
        if (effect == 16)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroClasses.Add(2);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging += 1;
        }

        //arcana unobscured
        if (effect == 17)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroClasses.Add(3);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe += 1;
        }
    }

    //for removing character classes, called from artifact effects
    //requires turnnumber also, incase removing class from non-current player
    [PunRPC]
    void RPC_RemoveClasses(int tNumber, int effect)
    {
        //flag variable to make sure only 1 class is removed
        bool removed = false;

        for (int i = 0; i < GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses.Count; i++)
        {
            //way of the sword
            if (effect == 15 && GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses[i] == 1 && removed == false)
            {
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses.RemoveAt(i);
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().strength -= 1;
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().influence -= 1;
                removed = true;
            }

            //engineers guidebook
            if (effect == 16 && GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses[i] == 2 && removed == false)
            {
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses.RemoveAt(i);
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().mechanics -= 1;
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().digging -= 1;
                removed = true;
            }

            //arcana unobscured
            if (effect == 17 && GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses[i] == 3 && removed == false)
            {
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroClasses.RemoveAt(i);
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().lore -= 1;
                GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().observe -= 1;
                removed = true;
            }
        }
    }

    public void DisplayHero(int number)
    {
        for (int i = 0; i < characterIcons.Count; i++)
        {
            if (i == number)
            {
                characterIcons[i].gameObject.SetActive(true);
            }
            else
            {
                characterIcons[i].gameObject.SetActive(false);
            }
        }

        //lets do the stat display change here too
        //also equipment icon change in v95
        for (int i = 0; i < GameManager.ins.references.statDisplayCharacterIcons.Count; i++)
        {
            if (i == number)
            {
                GameManager.ins.references.statDisplayCharacterIcons[i].gameObject.SetActive(true);
                GameManager.ins.references.equipmentDisplayCharacterIcons[i].gameObject.SetActive(true);
            }
            else
            {
                GameManager.ins.references.statDisplayCharacterIcons[i].gameObject.SetActive(false);
                GameManager.ins.references.equipmentDisplayCharacterIcons[i].gameObject.SetActive(false);
            }
        }

        //lets do the equipment display change here too
        for (int i = 0; i < uiButtonHandler.equipmentDisplayCharacterIcons.Count; i++)
        {
            if (i == number)
            {
                uiButtonHandler.equipmentDisplayCharacterIcons[i].gameObject.SetActive(true);
            }
            else
            {
                uiButtonHandler.equipmentDisplayCharacterIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetGameLength(int days)
    {
        ins.GM.RPC("RPC_SetGameLength", RpcTarget.AllBufferedViaServer, days);
    }

    //sets number of turns in game
    [PunRPC]
    void RPC_SetGameLength(int days)
    {
        endTime = days * 12;

        //update turn indicator
        ins.turnIndicatorText.text = "<sprite index=0>";

        //update the turns left counter
        ins.turnsLeftText.text = Clock.clock.totalTurnsPlayed + "/" + ins.endTime + " <sprite index=25>";
    }

    //draws the 4 quest options
    [PunRPC]
    void RPC_DrawQuestOptions()
    {
        //draws cards to the card choice canvas
        for (int i = 0; i < ins.questOffer.Count; i++)
        {
            //ins.DrawCardOptions(ins.questOffer[i].GetComponent<CardDisplay>().numberInDeck, 7);
        }
    }

    //when selecting starting cards
    public void InitialCardSelection()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 4;
        startingCardSelect.GetComponent<StartingCardSelection>().startingCardDiscardPrompt.SetActive(true);
        startingCardSelect.GetComponent<StartingCardSelection>().startingCardDiscardText.text = "You must discard 2 cards:";
        startingCardSelect.GetComponent<StartingCardSelection>().counter = 2;
    }

    //method for sending player to sleep (from previous attack resolve)
    [PunRPC]
    void RPC_Sleep(int player, int turns)
    {
        Debug.Log("sleep should activate");

        CardHandler.ins.DrawCards(player, 21, 7, turns);

        //add sleep overlay
        ins.avatars[player].GetComponentInChildren<Character>().sleepOverlay.SetActive(true);
    }

    public void LoadData(GameData data)
    {
        /*only load other avatars, if the count is bigger than 0
        if (data.avatars != null)
        {
            ins.avatars = data.avatars;
        }
        ins.numberOfPlayers = data.numberOfPlayers;
        */
        //ins.testNode = data.startingNode;
        if (PhotonRoom.room.spContinue == true && PhotonRoom.room.mainSceneLoaded == false)
        {
            Debug.Log("load data called, SP continue true");

            ins.startingNode = ins.references.nodes[data.startingNodeNumber];

            ins.gameStage = data.gameStage;

            PhotonRoom.room.mapType = data.mapType;
            PhotonRoom.room.startLocation = data.startingLocation;

            ins.startingDifficulty = data.startingDifficulty;

            //draw the difficulty card
            SetDifficultyCardDelayed();

            ins.references.combatModifierMultiplyer = data.combatModifierMultiplyer;

            //for v1.0.0.
            ins.references.nabamaxLessonCost = data.nabamaxLessonCost;

            //for v99
            ins.mapRandomizeHandler.randomTransits = data.randomTransits;

            //load waypoints
            ins.references.foundWaypoints = data.foundWaypoints;

            //need this here too, for continue games
            ins.scoreModifier = data.scoreModifier;

            //probably not best way of doing this
            //added in v0.7.1.
            ins.references.chosenCharacter = data.chosenCharacter;

            //clock variables
            Clock.clock.turnNumber = data.turnNumber;
            Clock.clock.totalTurnsPlayed = data.totalTurnsPlayed;
            Clock.clock.day2Started = data.day2Started;
            Clock.clock.day3Started = data.day3Started;
            Clock.clock.citadelEntered = data.citadelEntered;
            Clock.clock.isNight = data.isNight;

            //do necessary graphics changes
            Clock.clock.MoveHand2(Clock.clock.turnNumber);

            if (Clock.clock.isNight == true)
            {
                Clock.clock.dayToken.SetActive(false);
                Clock.clock.nightToken.SetActive(true);

                Clock.clock.NightStart();

                //need these later, when we add minimap starting spawns
                if (GameManager.ins.references.currentMinimap != null)
                {
                    if (GameManager.ins.references.currentMinimap.gameObject.activeSelf)
                    {
                        GameManager.ins.references.currentMinimap.NightStart();
                    }
                }
            }

            //need to remake load system for overmap nodes
            int encounterToCheck = 0;

            for (int i = 0; i < ins.references.nodes.Count; i++)
            {
                if (ins.references.nodes[i] != null)
                {
                    if (ins.references.nodes[i].GetComponent<NodeEncounterHandler>() != null)
                    {
                        ins.references.nodes[i].GetComponent<NodeEncounterHandler>().strategicEncounterCount = data.overmapEncounters[i];

                        if (ins.references.nodes[i].GetComponent<NodeEncounterHandler>().strategicEncounterCount > 0)
                        {
                            for (int j = 0; j < ins.references.nodes[i].GetComponent<NodeEncounterHandler>().strategicEncounterCount; j++)
                            {
                                ins.references.nodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.overmapEncounterNumbers[encounterToCheck]);
                                encounterToCheck += 1;
                            }
                        }
                    }
                }
            }

            //made special method for this
            //LoadCards(DataPersistenceManager.instance.gameData);

            Clock.clock.Zaarin3MessageGiven = data.zaarin3MessageGiven;

            //need to make sure the avatars are spawned before calling this
            Invoke(nameof(LoadContinuedWithDelay), 4f);

            PhotonRoom.room.mainSceneLoaded = true;
        }

        else if (PhotonRoom.room.spContinue == false && PhotonRoom.room.mainSceneLoaded == false)
        {
            /* unused in v95
             * starting locations
            if (PhotonRoom.room.startLocation == 1)
            {
                ins.startingNode = ins.references.nodes[1];
                ins.references.foundWaypoints[0] = 1;
            }
            else if (PhotonRoom.room.startLocation == 2)
            {
                ins.startingNode = ins.references.nodes[16];
                ins.references.foundWaypoints[1] = 16;
            }
            else if (PhotonRoom.room.startLocation == 3)
            {
                ins.startingNode = ins.references.nodes[37];
                ins.references.foundWaypoints[2] = 37;
            }
            else if (PhotonRoom.room.startLocation == 4)
            {
                ins.startingNode = ins.references.nodes[42];
                ins.references.foundWaypoints[3] = 42;
            }
            */

            //for v1.0.0.
            //seems not to work, lets do it on references start method instead
            ins.references.nabamaxLessonCost = data.nabamaxLessonCost;

            //put these these need only be done once?
            ins.mapRandomizeHandler.RandomizeTransits();
            data.randomTransits = ins.mapRandomizeHandler.randomTransits;

            Invoke(nameof(TeleportToStartNode), 1.0f);

            data.mapType = PhotonRoom.room.mapType;
            data.startingLocation = PhotonRoom.room.startLocation;

            //game difficulty setting
            //tutorial (unused at v94), unless
            if (PhotonRoom.room.startDifficulty == 1)
            {
                ins.gameStage = 2;
                ins.startingDifficulty = 1;

                //set combat modifiers
                ins.references.combatModifierMultiplyer = 1.5f;

            }
            //normal
            if (PhotonRoom.room.startDifficulty == 2)
            {
                ins.gameStage = 2;
                ins.startingDifficulty = 2;

                //set combat modifiers
                ins.references.combatModifierMultiplyer = 1f;
            }
            //advanced
            if (PhotonRoom.room.startDifficulty == 3)
            {
                ins.gameStage = 2;
                ins.startingDifficulty = 3;

                //set combat modifiers
                //ins.references.combatModifierMultiplyer = 0.8f;
                ins.references.combatModifierMultiplyer = 1f;
            }
            //expert
            if (PhotonRoom.room.startDifficulty == 4)
            {
                ins.gameStage = 2;
                ins.startingDifficulty = 4;

                //set combat modifiers
                //ins.references.combatModifierMultiplyer = 0.8f;
                ins.references.combatModifierMultiplyer = 1f;

                //additional stat penalty is given when prologue ok is pressed
            }

            //draw the difficulty card
            SetDifficultyCardDelayed();

            /* save this for late arrival perk
            if (PhotonRoom.room.startDifficulty == 2)
            {
                ins.gameStage = 3;
                ins.startingDifficulty = 2;
            }
            */
            //start coins
            //data.coins = PhotonRoom.room.startCoins;

            //use the photon room scoremodifier for new games
            ins.scoreModifier = PhotonRoom.room.scoreModifier;

            //special variables
            Clock.clock.Zaarin3MessageGiven = false;

            PhotonRoom.room.mainSceneLoaded = true;
        }
    }

    //for v95
    public void TeleportToStartNode()
    {
        //only use this once
        ins.references.useSpecificNodeForSpawn = 0;

        ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(4, 0, true);
    }

    public void LoadContinuedWithDelay()
    {
        LoadContinued(DataPersistenceManager.instance.gameData);
    }

    public void SaveData(ref GameData data)
    {
        //data.avatars = ins.avatars;
        //data.numberOfPlayers = ins.numberOfPlayers;

        //for v99+
        data.randomTransits = ins.mapRandomizeHandler.randomTransits;
        data.mapType = PhotonRoom.room.mapType;
        data.startingLocation = PhotonRoom.room.startLocation;

        //for v0.7.1.
        data.chosenCharacter = ins.references.chosenCharacter;

        //for v1.0.0.
        data.nabamaxLessonCost = ins.references.nabamaxLessonCost;

        //need to save unique encounter data too
        for (int i = 0; i < ins.exploreHandler.GetComponent<ExploreHandler>().exhaustableEncounters.Count; i++)
        {
            if (ins.exploreHandler.GetComponent<ExploreHandler>().exhaustableEncounters[i].GetComponent<Encounter2>() != null)
            {
                data.uniqueTaken[i] = ins.exploreHandler.GetComponent<ExploreHandler>().exhaustableEncounters[i].GetComponent<Encounter2>().isTaken;
            }
        }

        //this only works for SP tho
        if (ins.avatars[ins.turnNumber].GetComponent<CharController>().locationNode != null && ins.avatars[ins.turnNumber].GetComponent<CharController>().standingOn.isSubAreaTransit == false)
        {
            data.startingNodeNumber = ins.avatars[ins.turnNumber].GetComponent<CharController>().locationNode.nodeNumber;
        }

        data.sceneToGoTo = 999;
        data.nodeToGoTo = 999;

        //lets see if we need to save minimap location too
        if (ins.avatars[ins.turnNumber].GetComponent<CharController>().isExploring == true)
        {
            data.sceneToGoTo = ins.references.currentMinimap.sceneBuildIndex;
            //data.sceneToGoTo = SceneManager.GetActiveScene().buildIndex;
            data.nodeToGoTo = ins.references.currentMinimap.FindCurrentNodeNumber();
        }

        //for saving difficulty settings
        data.gameStage = ins.gameStage;

        //starting difficulty function was changed in v0.7.1.
        data.startingDifficulty = ins.startingDifficulty;

        //clock variables
        data.turnNumber = Clock.clock.turnNumber;
        data.totalTurnsPlayed = Clock.clock.totalTurnsPlayed;
        data.day2Started = Clock.clock.day2Started;
        data.day3Started = Clock.clock.day3Started;
        data.citadelEntered = Clock.clock.citadelEntered;
        data.isNight = Clock.clock.isNight;

        //this is handled in character class now
        //data.coins = ins.avatars[ins.turnNumber].GetComponentInChildren<Character>().coins;

        data.scoreModifier = ins.scoreModifier;

        //could set this here (gets changed if theres a final scoring event?)
        data.continueAvailable = true;

        //disable continue if game has been scored, or hero defeated
        //might want more foolproof check tho?
        if (dialogCanvas.GetComponent<CanvasController>().finalScore > 0 || ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == true)
        {
            data.continueAvailable = false;
        }

        //save possible highscore
        if (dialogCanvas.GetComponent<CanvasController>().finalScore > 0)
        {
            int heroNumber = ins.avatars[ins.turnNumber].GetComponentInChildren<Character>().heroNumber;
            int firstScore = data.highScores[heroNumber];
            int secondScore = data.highScores[heroNumber +10];
            int thirdScore = data.highScores[heroNumber +20];

            int firstScoreVersion = data.highScoreForVersion[heroNumber];
            int secondScoreVersion = data.highScoreForVersion[heroNumber + 10];

            //this should be updated for each new version
            int versionNumber = 1;

            //when replacing first score
            if (dialogCanvas.GetComponent<CanvasController>().finalScore > firstScore)
            {
                data.highScores[heroNumber] = dialogCanvas.GetComponent<CanvasController>().finalScore;
                data.highScoreForVersion[heroNumber] = versionNumber;

                //move previous first score to second place
                if (firstScore != 0)
                {
                    data.highScores[heroNumber + 10] = firstScore;
                    data.highScoreForVersion[heroNumber + 10] = firstScoreVersion;
                }
                //second score to third place, previous third score will be deleted
                if (secondScore != 0)
                {
                    data.highScores[heroNumber + 20] = secondScore;
                    data.highScoreForVersion[heroNumber + 20] = secondScoreVersion;
                }
            }

            //when replacing second best score
            else if (dialogCanvas.GetComponent<CanvasController>().finalScore > secondScore)
            {
                data.highScores[heroNumber +10] = dialogCanvas.GetComponent<CanvasController>().finalScore;
                data.highScoreForVersion[heroNumber +10] = versionNumber;

                //second score to third place, previous third score will be deleted
                if (secondScore != 0)
                {
                    data.highScores[heroNumber + 20] = secondScore;
                    data.highScoreForVersion[heroNumber + 20] = secondScoreVersion;
                }
            }

            //when replacing third best score
            else if (dialogCanvas.GetComponent<CanvasController>().finalScore > thirdScore)
            {
                data.highScores[heroNumber + 20] = dialogCanvas.GetComponent<CanvasController>().finalScore;
                data.highScoreForVersion[heroNumber + 20] = versionNumber;

                //previous third score will be deleted
            }
        }

        //save waypoints
        data.foundWaypoints = ins.references.foundWaypoints;

        //save the multiplier here
        data.combatModifierMultiplyer = ins.references.combatModifierMultiplyer;

        //lets try save this here, since saving in character class didnt seem to work
        data.actionPoints = ins.avatars[ins.turnNumber].GetComponent<CharController>().actionPoints;

        //lets just save cards like this?
        int usablesCount = handCardArea.transform.childCount;
        int skillCount = artifactCardArea.transform.childCount;
        int equipmentCount = equipmentCardArea.transform.childCount;
        int effectCount = effectCardArea.transform.childCount;
        int objectiveCount = objectiveCardArea.transform.childCount;

        data.usableCards = new int[usablesCount];
        data.skillCards = new int[skillCount];
        data.equipmentCards = new int[equipmentCount];
        data.effectCards = new int[effectCount];

        data.usableQty = new int[usablesCount];
        data.usableLvl = new int[usablesCount];
        data.skillLvl = new int[skillCount];
        data.equipmentQty = new int[equipmentCount];
        data.effectQty = new int[effectCount];

        data.objectiveCards = new int[objectiveCount];


        //save usables
        for (int i = 0; i < usablesCount; i++)
        {
            data.usableCards[i] = handCardArea.transform.GetChild(i).GetComponent<Card>().numberInDeck;
            data.usableQty[i] = handCardArea.transform.GetChild(i).GetComponent<CardDisplay2>().quantity;
            data.usableLvl[i] = handCardArea.transform.GetChild(i).GetComponent<Card>().cardLevel;
        }

        //save skillcards
        for (int i = 0; i < skillCount; i++)
        {
            data.skillCards[i] = artifactCardArea.transform.GetChild(i).GetComponent<Card>().numberInDeck;
            data.skillLvl[i] = artifactCardArea.transform.GetChild(i).GetComponent<Card>().cardLevel;
        }

        //save equipmentcards
        for (int i = 0; i < equipmentCount; i++)
        {
            data.equipmentCards[i] = equipmentCardArea.transform.GetChild(i).GetComponent<Card>().numberInDeck;
            data.equipmentQty[i] = equipmentCardArea.transform.GetChild(i).GetComponent<CardDisplay2>().quantity;
        }

        //save effectcards
        for (int i = 0; i < effectCount; i++)
        {
            data.effectCards[i] = effectCardArea.transform.GetChild(i).GetComponent<Card>().numberInDeck;
            data.effectQty[i] = effectCardArea.transform.GetChild(i).GetComponent<CardDisplay2>().quantity;
        }

        //save objectivecards
        for (int i = 0; i < objectiveCount; i++)
        {
            data.objectiveCards[i] = objectiveCardArea.transform.GetChild(i).GetComponent<Card>().numberInDeck;
        }

        //lets reset this before saving
        data.equippedItems = new int[12];

        //equipped items
        //helm 0, armor 1, ring 2, weapon 3, mount 4, misc 5, goggles 6, mask 7, amulet 8, tome 9, toolbox 10, shovel 11
        if (CardHandler.ins.helmSlot.transform.childCount > 0)
        {
            data.equippedItems[0] = CardHandler.ins.helmSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.armorSlot.transform.childCount > 0)
        {
            data.equippedItems[1] = CardHandler.ins.armorSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.ringSlot.transform.childCount > 0)
        {
            data.equippedItems[2] = CardHandler.ins.ringSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.weaponSlot.transform.childCount > 0)
        {
            data.equippedItems[3] = CardHandler.ins.weaponSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.mountSlot.transform.childCount > 0)
        {
            data.equippedItems[4] = CardHandler.ins.mountSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.miscSlot1.transform.childCount > 0)
        {
            data.equippedItems[5] = CardHandler.ins.miscSlot1.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.gogglesSlot.transform.childCount > 0)
        {
            data.equippedItems[6] = CardHandler.ins.gogglesSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.maskSlot.transform.childCount > 0)
        {
            data.equippedItems[7] = CardHandler.ins.maskSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.amuletSlot.transform.childCount > 0)
        {
            data.equippedItems[8] = CardHandler.ins.amuletSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.tomeSlot.transform.childCount > 0)
        {
            data.equippedItems[9] = CardHandler.ins.tomeSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.toolboxSlot.transform.childCount > 0)
        {
            data.equippedItems[10] = CardHandler.ins.toolboxSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }
        if (CardHandler.ins.shovelSlot.transform.childCount > 0)
        {
            data.equippedItems[11] = CardHandler.ins.shovelSlot.transform.GetChild(0).GetComponent<Card>().numberInDeck;
        }

        /*
         * STORE SAVING
         * 
        */

        int wilforgeShopCount = StoreHandler.ins.wilforgeShop.Count;
        int smithyShopCount = StoreHandler.ins.smithyShop.Count;
        int innShopCount = StoreHandler.ins.innShop.Count;
        int factoryShopCount = StoreHandler.ins.factoryShop.Count;
        int templeShopCount = StoreHandler.ins.templeShop.Count;
        int covenShopCount = StoreHandler.ins.covenShop.Count;
        int guildhouseShopCount = StoreHandler.ins.guildhouseShop.Count;
        int cornvilleShopCount = StoreHandler.ins.cornvilleShop.Count;

        data.wilforgeShop = new int[wilforgeShopCount];
        data.smithyShop = new int[smithyShopCount];
        data.innShop = new int[innShopCount];
        data.factoryShop = new int[factoryShopCount];
        data.templeShop = new int[templeShopCount];
        data.covenShop = new int[covenShopCount];
        data.guildhouseShop = new int[guildhouseShopCount];
        data.cornvilleShop = new int[cornvilleShopCount];

        data.wilforgeShopQty = new int[wilforgeShopCount];
        data.smithyShopQty = new int[smithyShopCount];
        data.innShopQty = new int[innShopCount];
        data.factoryShopQty = new int[factoryShopCount];
        data.templeShopQty = new int[templeShopCount];
        data.covenShopQty = new int[covenShopCount];
        data.guildhouseShopQty = new int[guildhouseShopCount];
        data.cornvilleShopQty = new int[cornvilleShopCount];

        //wilforge cards
        for (int i = 0; i < wilforgeShopCount; i++)
        {
            data.wilforgeShop[i] = StoreHandler.ins.wilforgeShop[i].GetComponent<Card>().numberInDeck;
            data.wilforgeShopQty[i] = StoreHandler.ins.wilforgeShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //smithy cards
        for (int i = 0; i < smithyShopCount; i++)
        {
            data.smithyShop[i] = StoreHandler.ins.smithyShop[i].GetComponent<Card>().numberInDeck;
            data.smithyShopQty[i] = StoreHandler.ins.smithyShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //inn cards
        for (int i = 0; i < innShopCount; i++)
        {
            data.innShop[i] = StoreHandler.ins.innShop[i].GetComponent<Card>().numberInDeck;
            data.innShopQty[i] = StoreHandler.ins.innShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //factory cards
        for (int i = 0; i < factoryShopCount; i++)
        {
            data.factoryShop[i] = StoreHandler.ins.factoryShop[i].GetComponent<Card>().numberInDeck;
            data.factoryShopQty[i] = StoreHandler.ins.factoryShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //temple cards
        for (int i = 0; i < templeShopCount; i++)
        {
            data.templeShop[i] = StoreHandler.ins.templeShop[i].GetComponent<Card>().numberInDeck;
            data.templeShopQty[i] = StoreHandler.ins.templeShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //coven cards
        for (int i = 0; i < covenShopCount; i++)
        {
            data.covenShop[i] = StoreHandler.ins.covenShop[i].GetComponent<Card>().numberInDeck;
            data.covenShopQty[i] = StoreHandler.ins.covenShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //guildhouse cards
        for (int i = 0; i < guildhouseShopCount; i++)
        {
            data.guildhouseShop[i] = StoreHandler.ins.guildhouseShop[i].GetComponent<Card>().numberInDeck;
            data.guildhouseShopQty[i] = StoreHandler.ins.guildhouseShop[i].GetComponent<CardDisplay2>().quantity;
        }
        //cornville cards
        for (int i = 0; i < cornvilleShopCount; i++)
        {
            data.cornvilleShop[i] = StoreHandler.ins.cornvilleShop[i].GetComponent<Card>().numberInDeck;
            data.cornvilleShopQty[i] = StoreHandler.ins.cornvilleShop[i].GetComponent<CardDisplay2>().quantity;
        }

        /*
         * Quest variables
        */
        int haveTakenQuestCount = ins.specialVariables.haveTakenQuest.Count;
        int haveCompletedQuestObjectiveCount = ins.specialVariables.haveCompletedQuestObjective.Count;
        int haveCompletedQuestCount = ins.specialVariables.haveCompletedQuest.Count;

        data.haveTakenQuest = new int[haveTakenQuestCount];
        data.haveCompletedQuestObjective = new int[haveCompletedQuestObjectiveCount];
        data.haveCompletedQuest = new int[haveCompletedQuestCount];

        for (int i = 0; i < haveTakenQuestCount; i++)
        {
            data.haveTakenQuest[i] = ins.specialVariables.haveTakenQuest[i];
        }
        for (int i = 0; i < haveCompletedQuestObjectiveCount; i++)
        {
            data.haveCompletedQuestObjective[i] = ins.specialVariables.haveCompletedQuestObjective[i];
        }
        for (int i = 0; i < haveCompletedQuestCount; i++)
        {
            data.haveCompletedQuest[i] = ins.specialVariables.haveCompletedQuest[i];
        }

        //for some reason character data wont be saved when game exits (maybe the object gets destroyed too soon?)
        ins.avatars[ins.turnNumber].GetComponentInChildren<Character>().SaveData(ref DataPersistenceManager.instance.gameData);

        data.zaarin3MessageGiven = Clock.clock.Zaarin3MessageGiven;


        //need to save overmap encounters too
        int encounterToCheck = 0;

        for (int i = 0; i < ins.references.nodes.Count; i++)
        {
            if (ins.references.nodes[i] != null)
            {
                if (ins.references.nodes[i].GetComponent<NodeEncounterHandler>() != null)
                {
                    data.overmapEncounters[i] = ins.references.nodes[i].GetComponent<NodeEncounterHandler>().strategicEncounterCount;

                    if (ins.references.nodes[i].GetComponent<NodeEncounterHandler>().strategicEncounterCount > 0)
                    {
                        for (int j = 0; j < ins.references.nodes[i].GetComponent<NodeEncounterHandler>().strategicEncounterCount; j++)
                        {
                            data.overmapEncounterNumbers[encounterToCheck] = ins.references.nodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
            }
        }
    }

    //show basic info of map
    public void DisplayMapInfo()
    {
        int day = ins.gameStage;

        day -= 1;
        /* lets change the starting difficulty function in v0.7.1.
        if(ins.startingDifficulty == 1)
        {
            day -= 1;
        }
        if (ins.startingDifficulty == 2)
        {
            day -= 2;
        }
        */

        ins.references.locationText.text = "Wandering Isle, Day " + day;
    }

    public void LoadContinued(GameData data)
    {
        //load usables
        for (int i = 0; i < data.usableCards.Length; i++)
        {
            int holderType = CardHandler.ins.generalDeck[data.usableCards[i]].GetComponent<Card>().cardType;

            CardHandler.ins.DrawCards(ins.turnNumber, data.usableCards[i], holderType, data.usableQty[i]);

            //need special operation for upgrading cardlevels (for few instances in the usables holder too)
            //lvl 2
            if (data.usableLvl[i] > 1)
            {
                int lvl2Card = CardHandler.ins.generalDeck[data.usableCards[i]].GetComponent<Card>().levelUpgradeCardNumbers[0];
                CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(lvl2Card);
                //CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(data.skillCards[i]);
            }
            //lvl 3
            if (data.usableLvl[i] > 2)
            {
                int lvl3Card = CardHandler.ins.generalDeck[data.usableCards[i]].GetComponent<Card>().levelUpgradeCardNumbers[1];
                CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(lvl3Card);
            }
        }

        //load skillcards
        for (int i = 0; i < data.skillCards.Length; i++)
        {
            int holderType = CardHandler.ins.generalDeck[data.skillCards[i]].GetComponent<Card>().cardType;

            CardHandler.ins.DrawCards(ins.turnNumber, data.skillCards[i], holderType, 1);

            //need special operation for upgrading cardlevels
            //lvl 2
            if(data.skillLvl[i] > 1)
            {
                int lvl2Card = CardHandler.ins.generalDeck[data.skillCards[i]].GetComponent<Card>().levelUpgradeCardNumbers[0];
                CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(lvl2Card);
                //CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(data.skillCards[i]);
            }
            //lvl 3
            if (data.skillLvl[i] > 2)
            {
                int lvl3Card = CardHandler.ins.generalDeck[data.skillCards[i]].GetComponent<Card>().levelUpgradeCardNumbers[1];
                CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(lvl3Card);
            }
        }

        //load equipmentcards
        for (int i = 0; i < data.equipmentCards.Length; i++)
        {
            int holderType = CardHandler.ins.generalDeck[data.equipmentCards[i]].GetComponent<Card>().cardType;

            CardHandler.ins.DrawCards(ins.turnNumber, data.equipmentCards[i], holderType, data.equipmentQty[i]);
        }

        //load effectcards
        for (int i = 0; i < data.effectCards.Length; i++)
        {
            int holderType = CardHandler.ins.generalDeck[data.effectCards[i]].GetComponent<Card>().cardType;

            CardHandler.ins.DrawCards(ins.turnNumber, data.effectCards[i], holderType, data.effectQty[i]);
        }

        //load objectivecards
        for (int i = 0; i < data.objectiveCards.Length; i++)
        {
            int holderType = CardHandler.ins.generalDeck[data.objectiveCards[i]].GetComponent<Card>().cardType;

            CardHandler.ins.DrawCards(ins.turnNumber, data.objectiveCards[i], holderType, 1);
        }

        //load equipped items
        //helm 0, armor 1, ring 2, weapon 3, mount 4, misc 5, goggles 6, mask 7, amulet 8, tome 9, toolbox 10, shovel 11
        for (int i = 0; i < data.equippedItems.Length; i++)
        {
            if (data.equippedItems[i] != 0)
            {
                CardHandler.ins.AddItemToSlot(turnNumber, data.equippedItems[i], false);
            }
        }

        // load starting location, if theres minimap to go to
        if (data.sceneToGoTo != 999)
        {
            //only use this once
            ins.references.useSpecificNodeForSpawn = data.nodeToGoTo;

            ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(data.sceneToGoTo, data.nodeToGoTo, true);
        }

        //load store items
        //wilforge
        for (int i = 0; i < data.wilforgeShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.wilforgeShop[i], 37, data.wilforgeShopQty[i]);
        }
        //smithy
        for (int i = 0; i < data.smithyShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.smithyShop[i], 28, data.smithyShopQty[i]);
        }
        //inn
        for (int i = 0; i < data.innShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.innShop[i], 31, data.innShopQty[i]);
        }
        //factory
        for (int i = 0; i < data.factoryShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.factoryShop[i], 44, data.factoryShopQty[i]);
        }
        //temple
        for (int i = 0; i < data.templeShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.templeShop[i], 47, data.templeShopQty[i]);
        }
        //coven
        for (int i = 0; i < data.covenShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.covenShop[i], 16, data.covenShopQty[i]);
        }
        //guildhouse
        for (int i = 0; i < data.guildhouseShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.guildhouseShop[i], 11, data.guildhouseShopQty[i]);
        }
        //cornville
        for (int i = 0; i < data.cornvilleShop.Length; i++)
        {
            StoreHandler.ins.DrawShopCards(data.cornvilleShop[i], 1, data.cornvilleShopQty[i]);
        }

        //load quest variables
        //might need to clear this, since the drawn quests alrdy add to this list?
        ins.specialVariables.haveTakenQuest.Clear();

        for (int i = 0; i < data.haveTakenQuest.Length; i++)
        {
            ins.specialVariables.haveTakenQuest.Add(data.haveTakenQuest[i]);
        }
        for (int i = 0; i < data.haveCompletedQuestObjective.Length; i++)
        {
            ins.specialVariables.haveCompletedQuestObjective.Add(data.haveCompletedQuestObjective[i]);
        }
        for (int i = 0; i < data.haveCompletedQuest.Length; i++)
        {
            ins.specialVariables.haveCompletedQuest.Add(data.haveCompletedQuest[i]);
        }
    }

    public void SetDifficultyCardDelayed()
    {
        Invoke(nameof(SetDifficultyCard), 0.5f);
    }

    //for v1.0.0.
    public void SetDifficultyCard()
    {
        GameObject card = null;

        if (ins.startingDifficulty == 1)
        {
            card = Instantiate(CardHandler.ins.generalDeck[256], new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (ins.startingDifficulty == 2)
        {
            card = Instantiate(CardHandler.ins.generalDeck[257], new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (ins.startingDifficulty == 3)
        {
            card = Instantiate(CardHandler.ins.generalDeck[258], new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (ins.startingDifficulty == 4)
        {
            card = Instantiate(CardHandler.ins.generalDeck[259], new Vector3(0, 0, 0), Quaternion.identity);
        }

        card.transform.SetParent(ins.difficultyCardArea.transform, false);

        //turns the card inactive
        card.SetActive(true);
    }

}
