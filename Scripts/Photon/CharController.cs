using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.LowLevel.PlayerLoop;
using UnityEngine.UI;
using TMPro;

//not used atm
public class CharController : MonoBehaviour//, IDataPersistence
{
    public PhotonView PV;

    //private Node targetNode;
    //optional?
    //private AvatarSetup avatarSetup;

    //node youre standing on
    public Node standingOn;

    //the node you came from
    public Node previousNode;

    //records the location node (when entering location)
    public Node locationNode;

    //used in v94 to remember which location you came from (for possible re-enter location purposes)
    public Node previousLocationNode;

    //need separate variable for this for certain purposes
    //public Node previousLocationNode;

    //for keeping track of internal node where the character is located
    public InternalNode internalNode;

    public int movement = 1;
    public int extraMovement = 1;
    public int actionPoints = 16;
    public bool canMove = true;
    public bool turnEnded;

    //players cards
    public List<GameObject> playerCards = new List<GameObject>();

    //variable, to tell if you're going to return card from hand back to deck, and which type of card
    public int wantToReturn;

    //variable, to tell which function you waiting from Card Display (could replace some of the below variables with this later)
    //2 = when choosing card from card option, 3 = when choosing quest to do, 4 = when choosing any hand card to return, 
    //for intelligence: 5= skillcheck cards, 6 = map cards, 7= skillcheck/defense cards, 8= defense cards only
    //9 = gift artifact card , 10 = steal card
    public int cardFunction;

    //new fag variable for allowing actions straight from board (no prequisite action required)
    public bool straightActionAllowed;

    //variable to tell what happens after certain methods are ran
    public int afterEffect;

    //variable to keep track of various special effects (1 = time warp, 2 = motivate)
    public int specialEffect;

    //for storing turn number
    public int turnNumber;

    //list of player owned agents
    public List<GameObject> agents;

    //flag to tell if hero has agent bonus or not
    public bool agentBonus;

    //counter to tell how many turns hero is still marching
    public int isMarching;

    //counter to tell how many turns hero is still marching (is counted down after each players turn)
    public int isWarded;

    //counter to tell how many turns hero is still bookkeeping (is counted down after each players turn)
    public int isBookkeeping;

    //counter to tell how many turns hero is still sleeping (is counted down at turn start?)
    //public int isSleeping;

    //counter to tell how many turns hero is still imprisoned (is counted down when player makes an action at grimhold?)
    public int isImprisoned;

    //flag variable to tell if player is blessed or not (for shield of isolore)
    public bool isBlessed;

    //total score you gained at final scoring
    public int totalScore;

    //for selecting starting cards
    public bool startingCardsSelected;

    //when pathways int card is activated
    public bool hasPathways;

    //flag variable to keep track if this character is bot or not
    public bool isAi;

    //for turn timer purposes
    public float turnTimer;
    public bool countTurnTimer;
    public int giveAlertAt;

    //flag to tell whether character is inside location (minimap since v90)
    public bool isExploring;

    //tells whether character is dead or not
    public bool isDead;

    //need these to check certain status effects?
    public bool isPetrified;
    public bool isFrozen;

    //lets make new flag variable for interacting with non-forced encounters
    public bool interactWithAnyEncounter;

    //flag variable to tell if character force moved with 0 energy
    //public bool sleptWhileMoving;

    /*
    //variable to tell if youre dealing with card options, not used anymore?
    public bool itsCardOption;

    //variable to tell if youre going to attempt quest, not needed anymore?
    public bool doingQuest;

    //not needed anymore?
    public bool takeHandCard;
    */

    private void Awake()
    {
        //lets put this here, because seems in sp mode the start method isnt soon enough
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //PV = GetComponent<PhotonView>();
        wantToReturn = 0;
        cardFunction = 0;
        afterEffect = 0;
        isMarching = 0;
        isWarded = 0;
        isBookkeeping = 0;
        //isSleeping = 0;
        isImprisoned = 0;
        isBlessed = false;
        totalScore = 0;
        straightActionAllowed = true;
        startingCardsSelected = false;
        hasPathways = false;
        turnTimer = 150;
        giveAlertAt = 15;
        interactWithAnyEncounter = false;
        //sleptWhileMoving = false;
        //itsCardOption = false;
        //doingQuest = false;
        //takeHandCard = false;
    }

    //previous move method, bit clumsy, but still used for certain purposes
    public void Mover(GameObject sendNode)
    {
        // to make sure you can only move your own character
        if (!PV.IsMine)
        {
            return;
        }
        //PV.RPC("RPC_AvatarMover", RpcTarget.AllBufferedViaServer); 
        //test
        //PV.RPC("RPC_AvatarMover", RpcTarget.AllBufferedViaServer, sendNode);

        //standingOn = PhotonView.Find(sendNode).GetComponent<Node>();

        //MovePointReduction();
        RPC_AvatarMover(sendNode);
    }

    //removed the rpc call in v99
    //[PunRPC]
    void RPC_AvatarMover(GameObject sendNode)
    {
        //makes sure all players have same nextNode        
        GameManager.ins.nextNode = sendNode.GetComponent<Node>();

        //updates the players standingOn -node across all the players
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn = sendNode.GetComponent<Node>();

        //technically this shouldnt be needed anymore
        //but theres many places which add +movementBonus to compensate for this, so..
        MovePointReduction(1);

        //then moves avatar to it
        //GameManager.ins.nextNode.Arrive(gameObject);
        standingOn.Arrive(gameObject);

    }


    //for commanded moves only (when clicking on new node)
    /*moves through route
    public void MoveCommanded(int sendNode, int movecost, bool isInstantMove, bool playSound)
    {
        // to make sure you can only move your own character
        if (!PV.IsMine)
        {
            return;
        }
        //PV.RPC("RPC_AvatarMover", RpcTarget.AllBufferedViaServer); 
        //test
        PV.RPC(nameof(RPC_MoveCommanded), RpcTarget.AllBufferedViaServer, sendNode, movecost, isInstantMove, playSound);

        //standingOn = PhotonView.Find(sendNode).GetComponent<Node>();

        //MovePointReduction();
    }
    */

    //[PunRPC]
    //lets try removing at least some of the pv calls in v99 (dont rly need anymore)
    //pv idnumber changed with gameobject
    public void MoveCommanded(GameObject sendNode, int movecost, bool isInstantMove, bool playSound)
    {
        //Debug.Log(gameObject + " moving to nodeviewID: " + sendNode + ", turnNumber is: " + turnNumber);

        //makes sure all players have same nextNode        
        //GameManager.ins.nextNode = PhotonView.Find(sendNode).GetComponent<Node>();
        GameManager.ins.nextNode = sendNode.GetComponent<Node>();

        //need to keep track of previous node now too
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;

        //updates the players standingOn -node across all the players
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn = sendNode.GetComponent<Node>();

        //save location node on each movementBonus to overmap node
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isLocationNode == true)
        {
            locationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;
        }

        //lets try this for v94
        //kinda complicated..
        if (standingOn.isLocationNode && previousNode.isLocationNode) //&& !ReferenceEquals(previousLocationNode, locationNode)
        {
            previousLocationNode = previousNode;
        }


        turnEnded = MovePointReduction(movecost);

        if (isInstantMove == false)
        {
            //then moves avatar to it
            standingOn.MoveBetweenNodes(gameObject);
        }

        if (isInstantMove == true)
        {
            standingOn.InstantArrive(gameObject);
        }

        //bit annoying to have delay here, but otherwise sleeping method might bug out
        //Invoke("DelayedMovement", 0.1f);

        //play movementBonus sound for all to hear
        //might want to change this later for various mount sounds
        if (hasPathways == false && playSound == true)// && isInstantMove == false)
        {
            GameManager.ins.references.sfxPlayer.MovementSound();
        }
    }

    //not used atm
    void DelayedMovement()
    {
        standingOn.MoveBetweenNodes(gameObject);
    }

    //put this to charcontroller?
    public void UpdateTimer()
    {
        // to make sure only you call this
        if (!PV.IsMine)
        {
            return;
        }


        PV.RPC("RPC_UpdateTimer", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_UpdateTimer()
    {
        //lets do its own method for this
        if (CheckIfHeroDead() == true)
        {
            return;
        }

        actionPoints -= 1;

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            CountActionPoints();
        }

        if(actionPoints > 0)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true) 
            { 
                PV.RPC("RPC_StartTurn", RpcTarget.AllBufferedViaServer);
            }
            return;
        }

        if (actionPoints <= 0)
        {
            //this is when player has no more AP, and turn actually ends
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                /* lets remove this for now
                 * special case for ring of rejuvenation
                if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 3, 61) == true)
                {
                    UpdateResources(0, 3);
                    UpdateResources(3, 3);
                }
                */

                //CountActionPoints();
                GameManager.ins.turnIndicatorText.text = "<sprite index=0>";

                //dont count timer for this player anymore
                countTurnTimer = false;
            }

            //maybe also reset pathways variable here
            hasPathways = false;

            if (GameManager.ins.turnNumber < GameManager.ins.numberOfPlayers)
            {
                GameManager.ins.turnNumber += 1;
            }

            if (GameManager.ins.turnNumber == GameManager.ins.numberOfPlayers)
            {
                GameManager.ins.turnNumber = 0;
            }
            //could add clock call here
            //lets do this after turn change
            //hmm.. need some testing
            Clock.clock.MoveHand();

            //maybe also reset pathways variable here
            hasPathways = false;

            //could enable all character canvases here, in case something was left invisible
            for (int i = 0; i < GameManager.ins.avatars.Count; i++)
            {
                GameManager.ins.avatars[i].GetComponentInChildren<Canvas>().enabled = true;
            }

        }
    }

    void AdvanceClock()
    {
        Clock.clock.MoveHand();
    }
    

    public void EndTurn()
    {
        // to make sure only you call this
        if (!PV.IsMine)
        {
            return;
        }

        //stop timer here also
        countTurnTimer = false;

        //this should be called most the time now?
        //actually lets put this at refrest encounter
        //interactWithAnyEncounter = false;

        //disallow straight action (maybe do this before end turn rpc, becaue the rpc call seems to go too fast in sp mode)
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;

        //normal end turn
        if (specialEffect == 0)
        {
            PV.RPC("RPC_EndTurn", RpcTarget.AllBufferedViaServer);
        }
        //in case of time warp
        if (specialEffect == 1)
        {
            //put in delay for this, just in case sleep was triggered during your turn (rpc call wont have time to finish otherwise)
            Invoke("DelayedStartTurn", 0.2f);

            if (ItsYourTurn() == true)
            {
                //give message
                //string msgs = "You gained extra action.";
                //GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                //give public message
                string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " gained extra action.";
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            }
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
            return;
        }
        //in case of motivate
        if (specialEffect == 2)
        {
            //reset the flag variables, just in case
            afterEffect = 0;
            wantToReturn = 0;
            cardFunction = 0;
            specialEffect = 0;
            PV.RPC("RPC_StartTurn", RpcTarget.AllBufferedViaServer);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
            return;
        }
    }

    void DelayedStartTurn()
    {
        //StartTurn();
        PV.RPC("RPC_StartTurn", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_EndTurn()
    {
        //perhaps best put the forced encounter refresh here
        //although technically the encounters will be forced only once per turn then, but it should be fine
        //need to remake this too for v90
        RefreshForcedEncounterChecks();

        //reset these variable for all here, seems necessary atm
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().eventCardNumber = 0;
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().temporaryNumber2 = 0;

        //maybe also reset pathways variable here
        //hasPathways = false;

        //test removing the next two lines
        //standingOn = GameManager.ins.currentNode;
        //GameManager.ins.currentNode.Leave();
        if (ItsYourTurn() == true)
        {
            standingOn.Leave();
        }

        UpdateTimer();
        //test this on updatetimer method?
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();
    }

    //dunno if this needs to be done via RPC call, but lets make sure
    public void StartTurn()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayTurnStart();

        // to make sure only you call this
        if (!PV.IsMine)
        {
            return;
        }

        //allow straight action
        straightActionAllowed = true;

        //resets movementBonus points
        //CountMovementPoints();
        canMove = true;

        //reset action points
        actionPoints = gameObject.GetComponentInChildren<Character>().maxActionPoints;
        CountActionPoints();

        //check passive bonuses (not poison or sleep)
        CardHandler.ins.CheckCardTimers(turnNumber);

        //reset the flag variables, just in case
        afterEffect = 0;
        wantToReturn = 0;
        cardFunction = 0;
        specialEffect = 0;
        turnEnded = false;
        //lets put this at refresh encounter
        //interactWithAnyEncounter = false;

        //isExploring = false;
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().inspirationUsed = false;

        //for turn timer purposes
        turnTimer += 30;
        giveAlertAt = 15;
        countTurnTimer = true;

        //sleptWhileMoving = false;

        if (isAi == false)
        {
            //changes the turn indicator text
            //GameManager.ins.turnIndicatorText.gameObject.SetActive(true);
            GameManager.ins.turnIndicatorText.text = "<sprite index=1>";

            gameObject.GetComponentInChildren<Character>().UpdateResourceTexts();
        }

        //AI wont select starting cards
        //also if its tutorial mode, starting cards wont need to be selected
        if (isAi == true || PhotonRoom.room.isTutorial == true)
        {
            startingCardsSelected = true;
        }


        //enable all UI buttons, but disable the displays
        GameManager.ins.uiButtonHandler.EnableAllButtons();
        GameManager.ins.uiButtonHandler.CloseAllDisplays();

        PV.RPC("RPC_StartTurn", RpcTarget.AllBufferedViaServer);
    }

    //this is called whenever new turn starts
    //also when first action is used, unlike the regular start turn method
    [PunRPC]
    void RPC_StartTurn()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            /* lets do new system
            //special cases for ring of regrowth etc..
            if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 3, 62) == true)
            {
                UpdateResources(3, 1);
            }
            //ring of energy
            if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 3, 177) == true)
            {
                UpdateResources(0, 1);
            }
            //regrowth ability
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(105) == true)
            {
                UpdateResources(3, 1);
            }
            //inner energy ability
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(106) == true)
            {
                UpdateResources(0, 1);
            }
            */

            //lets add special check for test of strength quest trigger here (was lightstone)
            //check if player has both test of strength quest & lvl 5
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 302, 11) > 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel >= 5)
            {
                GameManager.ins.dialogCanvas.GetComponent<CanvasController>().Zaarin2();

                return;
            }

            //give message also when 3 keystones have been collected
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 186, 5) > 2 &&
                Clock.clock.Zaarin3MessageGiven == false)
            {
                GameManager.ins.dialogCanvas.GetComponent<CanvasController>().Zaarin3();

                return;
            }

            //dunno why the regen roll is both on start method and movement point reduction method?
            int hpRegenRoll = Random.Range(1, 101);
            if(hpRegenRoll <= gameObject.GetComponentInChildren<Character>().healthRegen)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                UpdateResources(3, 1);
            }

            int energyRegenRoll = Random.Range(1, 101);
            if (energyRegenRoll <= gameObject.GetComponentInChildren<Character>().energyRegen)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                UpdateResources(0, 1);
            }

            //in case of poisoned
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 23, 7) > 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 23, 7) <= 5)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 1);

                //lose 1 energy
                UpdateResources(3, -1);
            }

            //in case of heavily poisoned
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 23, 7) > 5)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 2);

                //lose 2 energy
                UpdateResources(3, -2);
            }
        }

        Invoke("ContinueStartTurn", 0.2f);
    }

    void ContinueStartTurn()
    {
        //lets do its own method for this
        if (CheckIfHeroDead() == true)
        {
            return;
        }

        //enable ui buttons here, just in case
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        //lets reset this here, just in case (should actually put all of these resets in same place..)
        //GameManager.ins.questCanvas.GetComponent<QuestingDialog>().numberInDeck = 0;

        //standingOn.Arrive(gameObject);
        standingOn.MoveBetweenNodes(gameObject);

        //allow overmap cards
        if (isExploring == true)
        {
            CardHandler.ins.SetUsables(2);
        }
        else
        {
            CardHandler.ins.SetUsables(1);
        }

        //artifact reset here?
        //make sure this is not called at start of game (cause its alrdy called at avatarsetup once)
        if (Clock.clock.turnNumber == 0 && Clock.clock.isNight == false && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
            Clock.clock.totalTurnsPlayed > 10)
        {
            //GameManager.ins.ArtifactReset();
            //ArtifactReset();
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            Debug.Log("straigh action becomes allowed");

            //allow straight action
            straightActionAllowed = true;

            //start timer (again to make sure)
            countTurnTimer = true;

            //reset the flag variables, just in case
            afterEffect = 0;
            wantToReturn = 0;
            cardFunction = 0;
            specialEffect = 0;

            //lets start Ai logic here, if this character is an AI
            //dont do anything if character is sleeping (should call the endturn method instead from issleeping method)
            if (isAi == true && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 21, 7) == 0 && ItsYourTurn())
            {
                /*dont start AI turn yet, if new day just started (lets test without)
                if(Clock.clock.turnNumber == 11 && Clock.clock.isNight == true)
                {
                    return;
                }
                */
                //need delay, so that the start turn arrive method has time to finish
                Invoke("StartAiTurn", 2.0f);
                //gameObject.GetComponentInChildren<AiHandler>().StartAiTurn();
            }
        }
    }

    //update action points
    public void CountActionPoints()
    {
        if(isAi == true)
        {
            return;
        }
        //GameManager.ins.references.actionText.text = "";

        //GameManager.ins.references.actionText.text = actionPoints + " <sprite index=32>";

        //new for v91
        GameManager.ins.references.GetComponent<SliderController>().SetBarValues(turnNumber);

        /*
        for (int i = 0; i < actionPoints; i++)
        {
            GameManager.ins.references.actionText.text += "<sprite index=32>";
        }
        */
    }

    

    //not used atm, but might need later
    public void StartAiTurnWithDelay()
    {
        Invoke("StartAiTurn", 2.0f);
    }

    void StartAiTurn()
    {
        //gameObject.GetComponentInChildren<AiHandler>().StartAiTurn();
    }


    //this is called at end of Arrive method at Node class
    public void SleepTest()
    {
        //only do this if eventDisplay isnt active
        if (Clock.clock.eventDisplayActive == false)
        {
            //in case of sleeping
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 21, 7) > 0 && GameManager.ins.turnNumber == turnNumber)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 21, 7, 1);
                
                // to make sure only you call this
                if (PV.IsMine)
                {
                    //ends players turn, if he was sleeping
                    EndTurn();
                }
            }
        }

        //leaves the node, if event display is active
        if (Clock.clock.eventDisplayActive == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().Leave();
        }
    }

    //returns true, if movementBonus ended turn
    public bool MovePointReduction(int cost)
    {
        // to make sure only you call this
        if (!PV.IsMine)
        {
            return false;
        }
        //special case for when pathways is activated (unused)
        if (hasPathways == true)
        {
            //play sfx
            //GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayTeleport();
            PV.RPC("RPC_PlayTeleport", RpcTarget.AllBufferedViaServer);
            return false;
        }

        actionPoints -= cost;


        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //new regen system
            int hpRegenRoll = Random.Range(1, 101);
            if (hpRegenRoll <= GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().healthRegen)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(3, 1);
                UpdateResources(3, 1);
            }

            int energyRegenRoll = Random.Range(1, 101);
            if (energyRegenRoll <= GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energyRegen)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(0, 1);
                UpdateResources(0, 1);
            }

            //in case of poisoned
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 23, 7) > 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 23, 7) <= 5)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 1);

                //lose 1 energy
                UpdateResources(3, -1);
            }

            //in case of heavily poisoned
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 23, 7) > 5)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 2);

                //lose 2 energy
                UpdateResources(3, -2);
            }
        }

        //lets do its own method for this
        if (CheckIfHeroDead() == true)
        {
            return true;
        }

        //set hero to sleep for negative action points
        if (actionPoints < 0)
        {
            int sleepturns = -actionPoints;

            //GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, sleepturns);
            //lets use the faster method
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 7, sleepturns);

            actionPoints = 0;

            //lets end turn on the movementBonus method instead
            //EndTurn();
            return true;
            //return;
        }

        if (actionPoints == 0)
        {
            //EndTurn();
            return true;
        }

        CountActionPoints();

        //Invoke("ContinueMovePointReduction", 0.05f);
        return false;
    }

    //can be used various other action also (despite the name
    public void ContinueMovePointReduction()
    {
        //lets do its own method for this
        if (CheckIfHeroDead() == true)
        {
            return;
        }

        //set hero to sleep for negative action points
        if (actionPoints < 0)
        {
            int sleepturns = -actionPoints;

            //GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, sleepturns);
            //lets use the faster method
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 7, sleepturns);

            actionPoints = 0;

            EndTurn();
            //return;
        }

        if (actionPoints == 0)
        {
            EndTurn();
        }

        //allow overmap cards
        //need this for some special occasions
        if (isExploring == true)
        {
            CardHandler.ins.SetUsables(2);
        }
        else
        {
            CardHandler.ins.SetUsables(1);
        }

        CountActionPoints();

        //CountFeet();
    }


    //for playing teleport sound for all players
    [PunRPC]
    void RPC_PlayTeleport()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayTeleport();
    }

    //unused?
    public void OpenActionWindow()
    {
        //makes sure only the player whos turn is it can open action window. probably not very simple way of doing this
        //if (gameObject.GetPhotonView().ViewID == GameManager.ins.avatars[GameManager.ins.turnNumber].GetPhotonView().ViewID)

        // to make sure only you call this
        if (!PV.IsMine)
        {
            return;
        }

        standingOn.RightClick();

    }

    //makes sure only you update your own stats
    public void UpdateStatsTexts()
    {
        if (!PV.IsMine)
        {
            return;
        }
        gameObject.GetComponentInChildren<Character>().UpdateStatTexts();
    }

    //makes sure only you update your own hero displays
    public void HeroDisplay()
    {
        //dont show this info for ai either
        if (!PV.IsMine || isAi == true)
        {
            return;
        }
        gameObject.GetComponentInChildren<Character>().HeroDisplay();
    }

    //opens first turn indicator, if its the first turn and if youre the first player
    public void FirstTurnIndicator()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if(GameManager.ins.avatars[0].GetComponent<CharController>().turnNumber == turnNumber)
        {
            //activates the turn indicator banner
            //update turn indicator
            GameManager.ins.turnIndicatorText.text = "<sprite index=1>";
        }
    }

    /*old interaction
    public void InteractWithLocation()
    {
        if (!PV.IsMine)
        {
            return;
        }

        //play sfx for all
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayInteractionForOthers();

        //dont do this if motivate is in effect
        if (specialEffect != 2)
        {
            PV.RPC("RPC_InteractWithLocation", RpcTarget.AllBufferedViaServer);

            //tests if player is relentless
            if (HasPassiveTest(10) == true && standingOn.interactCost > 0)
            {
                int relentlessCost = standingOn.interactCost - 1;

                //update energy
                UpdateResources(0, -relentlessCost);
            }

            else
            {
                //update energy
                UpdateResources(0, -standingOn.interactCost);
            }
            gameObject.GetComponentInChildren<Character>().UpdateResourceTexts();
        }
    }

    [PunRPC]
    void RPC_InteractWithLocation()
    {
        standingOn.ChangePlate();
    }
    */

    //updates resources
    public void UpdateResources(int resourceType, int resourceQty)
    {
        if (!PV.IsMine)
        {
            return;
        }
        PV.RPC("RPC_UpdateResources", RpcTarget.AllBufferedViaServer, resourceType, resourceQty);

    }

    [PunRPC]
    void RPC_UpdateResources(int resourceType, int resourceQty)
    {
        gameObject.GetComponentInChildren<Character>().UpdateResources(resourceType, resourceQty);
    }

    
    /* old card methods
     * uses turnNumber variable of this class
    public void DrawHandCards(int resourceType, int resourceQty)
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (resourceType == 7)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeQuestCardNumber();
                GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawHandCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, resourceType);
            }
        }
        if (resourceType == 8)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeIntelligenceCardNumber();
                GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawHandCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, resourceType);
            }
        }
    }

    //uses turnNumber variable of this class
    //draws one card only
    public void DrawSpecificCard(int resourceType, int cardNumber)
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (resourceType == 10)
        {
            PV.RPC("RPC_DrawHandCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, resourceType);
        }
    }

    //note that this uses the turnnumber variable of this class
    //but only of the instance which activates this method
    //used now also to draw specific artifacts, which arent exactly hand cards
    [PunRPC]
    void RPC_DrawHandCards(int sentTurnNumber, int cardNumber, int resourceType)
    {
        if (resourceType == 7)
            GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
        if (resourceType == 8)
            GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
        if (resourceType == 10)
            GameManager.ins.artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

        GameManager.ins.DrawHandCards2(sentTurnNumber, cardNumber, resourceType);
    }

    //method for drawing card for current player only (dont need to hassle with this classes turnnumber variable in this case)
    public void DrawHandCardsCurrentOnly(int resourceType, int resourceQty)
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (resourceType == 7)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeQuestCardNumber();
                GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawHandCardsCurrentOnly", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }
        if (resourceType == 8)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeIntelligenceCardNumber();
                GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawHandCardsCurrentOnly", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }
    }

    [PunRPC]
    void RPC_DrawHandCardsCurrentOnly(int cardNumber, int resourceType)
    {
        if (resourceType == 7)
            GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
        if (resourceType == 8)
            GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

        GameManager.ins.DrawHandCards2(GameManager.ins.turnNumber, cardNumber, resourceType);
    }

    //used for continuation quest draws (places the card on board also)
    [PunRPC]
    void RPC_DrawHandCardsCurrentOnly2(int cardNumber, int resourceType)
    {
        if (resourceType == 7)
            GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

        GameManager.ins.DrawHandCards3(GameManager.ins.turnNumber, cardNumber, resourceType);
    }

    //for card options dialog
    public void DrawCardOptions(int resourceType, int resourceQty)
    {
        if (resourceType == 7)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeQuestCardNumber2();
                GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }
        if (resourceType == 8)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeIntelligenceCardNumber();
                GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }
        //note that this wont need randomzing, since its only used to show the 4 cards on the pyramid offer list
        if (resourceType == 9)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = i;

                PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }
        //note that this wont need randomzing, since its only used to show the 4 cards on the sonyas offer list
        if (resourceType == 10)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = i;

                PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }

        //note that this wont need randomzing, since its only used to show the 3 cards on the incoming events list
        if (resourceType == 11)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = i;

                PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }

        //note that this uses special kind of randomizing
        //if you do the rpc call directly, you dont need to randomize cardnumber (used by rally the guildsmen quest)
        if (resourceType == 12)
        {
            for (int i = 0; i < resourceQty; i++)
            {
                int cardNumber = GameManager.ins.TakeWishCardNumber();
                GameManager.ins.eventDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, cardNumber, resourceType);
            }
        }

    }

    [PunRPC]
    void RPC_DrawCardOptions(int cardNumber, int resourceType)
    {
        if (resourceType == 7)
            GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
        if (resourceType == 8)
            GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
        //actually shouldnt use this
        //if (resourceType == 9)
        //    GameManager.ins.artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

        GameManager.ins.DrawCardOptions(cardNumber, resourceType);
    }

    //for drawing event cards
    //not that this takes random number, you need to call the rpc method directly if you want specific card
    //this method isnt actually mostly needed anymore, but lets save it just in case
    public void DrawEvent()
    {
        if (!PV.IsMine)
        {
            return;
        }

        //leave node for the duration of drawing event
        //standingOn.Leave();

        int cardNumber = GameManager.ins.TakeEventCardNumber();

        PV.RPC("RPC_DrawEvent", RpcTarget.AllBufferedViaServer, cardNumber, 1);
    }

    //drawtype 1 shows event display, drawtype 2 doesnt (only used for drawtype2 atm?)
    [PunRPC]
    void RPC_DrawEvent(int cardNumber, int drawType)
    {
        GameManager.ins.DrawEvent(cardNumber, drawType);
    }

    //new draw event method doesnt need to draw cardnumber or drawtype anymore
    public void DrawEvent2()
    {
        if (!PV.IsMine)
        {
            return;
        }

        //int cardNumber = GameManager.ins.TakeEventCardNumber();

        PV.RPC("RPC_DrawEvent2", RpcTarget.AllBufferedViaServer);
    }

    //draws next event from the list and puts new event to the back of the list
    [PunRPC]
    void RPC_DrawEvent2()
    {
        GameManager.ins.DrawEventFromList();
    }
    */

    //tests if its your turn or not
    public bool ItsYourTurn()
    {
        /*hmm.. seems we need this for now, although doesnt seem right
        if(PhotonNetwork.OfflineMode == true)
        {
            if(turnNumber == GameManager.ins.turnNumber)
            {
                return true;
            }
            return false;
        }
        */
        if (!PV.IsMine)
        {
            return false;
        }
        return true;
    }

    //tests if its your character or not, not used atm, but might work
    public bool ItsYourCharacter(int player)
    {
        /*
        //unless its your character
        for (int y = 0; y < GameManager.ins.avatars.Count; y++)
        {
        */
        if (ReferenceEquals(gameObject, GameManager.ins.avatars[player].gameObject))
        {
            return true;
        }
        //}

        return false;
    }

    /* more old cards methods
    public void ReturnCard(int card, int numberInDeck, int cardType, int deckType)
    {
        if (!PV.IsMine)
        {
            return;
        }
        PV.RPC("RPC_ReturnCard", RpcTarget.AllBufferedViaServer, card, numberInDeck, cardType, deckType);
        wantToReturn = 0;

    }

    [PunRPC]
    void RPC_ReturnCard(int card, int numberInDeck, int cardType, int deckType)
    {
        if (cardType == 7)
        {
            GameManager.ins.ReturnQuestCard(card, numberInDeck, deckType);

            Debug.Log("quest card returned");
        }

        if (cardType == 8)
        {
            GameManager.ins.ReturnIntelligenceCard(card, numberInDeck, deckType);
        }

        if (cardType == 9)
        {
            GameManager.ins.ReturnArtifactCard(card, numberInDeck, deckType);
        }

        if (cardType == 10)
        {
            GameManager.ins.ReturnEventCard(card, numberInDeck, deckType);
        }

        //special case for 2 quest card reward
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect == 1)
        {
            //GameObject.Find("Quest Discard Prompt").GetComponent<Action>().AfterEffect();
            AfterEffect(7, 2);
        }

        //special case for 2 intelligence card reward
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect == 2)
        {             
            //GameObject.Find("Intelligence Discard Prompt").GetComponent<Action>().AfterEffect();
            AfterEffect(8, 2);
        }

        //lets reset the skillcheck variable here, in case attack card is used
        GameObject.Find("Skill Check Handler").GetComponent<SkillCheck>().skillCheckSuccess = false;

    }

    //for testing int cards to see which decktype is used in gamemanager
    //decktype 5= skillcheck cards, 6= map cards, 7= skillcheck/defense cards, 8= defense cards only
    //13= for skill cards used in encounters, 14= both skill and defense cards (not used atm, nor fully implemented)
    public void TestIntCard(int i, int numberInDeck, int cardType)
    {
        //just in case
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType == null)
        {
            return;
        }

        for (int y = 0; y < GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType.Length; y++)
        {
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType[y] == 1 && cardFunction == 5)
            {
                //ReturnCard(i, numberInDeck, cardType, 5);
            }
            //added special case for straight action here
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType[y] == 2 && (cardFunction == 6 || straightActionAllowed == true))
            {
                //ReturnCard(i, numberInDeck, cardType, 6);
            }
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType[y] == 3 && cardFunction == 7)
            {
                //ReturnCard(i, numberInDeck, cardType, 7);
            }
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType[y] == 4 && cardFunction == 8)
            {
                //ReturnCard(i, numberInDeck, cardType, 8);
            }
            //special cases for int cards used in encounters
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType[y] == 1 && cardFunction == 13)
            {
                //ReturnCard(i, numberInDeck, cardType, 13);
            }
            //this is unused atm
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().activationType[y] == 3 && cardFunction == 14)
            {
                //ReturnCard(i, numberInDeck, cardType, 14);
            }
        }
        //else nothing else happens
    }

    //changed on v85
    public void ReclaimIntelligence(int numberInDeck)
    {
        //put this check here just in case i missed itsyourturn -check somewhere
        if (ItsYourTurn() == false)
        {
            return;
        }
        PV.RPC("RPC_ReclaimIntelligence", RpcTarget.AllBufferedViaServer, numberInDeck);
    }

    [PunRPC]
    void RPC_ReclaimIntelligence(int numberInDeck)
    {
        GameManager.ins.ReclaimIntelligence(numberInDeck);
    }

    public void ReclaimIntelligence2(int numberInDeck, int targetTurnNumber)
    {
        PV.RPC("RPC_ReclaimIntelligence2", RpcTarget.AllBufferedViaServer, numberInDeck, targetTurnNumber);

    }

    [PunRPC]
    void RPC_ReclaimIntelligence2(int numberInDeck, int targetTurnNumber)
    {
        GameManager.ins.ReclaimIntelligence2(numberInDeck, targetTurnNumber);
    }
    */

    //updates the character stats across network
    public void UpdateStats(int stat, int qty)
    {
        if (!PV.IsMine)
        {
            return;
        }
        PV.RPC("RPC_UpdateStats", RpcTarget.AllBufferedViaServer, stat, qty);
    }

    [PunRPC]
    void RPC_UpdateStats(int stat, int qty)
    {
        //update stats
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStats(stat, qty);
        gameObject.GetComponentInChildren<Character>().UpdateStats(stat, qty);
    }

    //for cancelling actions, simpler to have it here
    //isnt rly used for anything functional atm?
    public void Cancel()
    {
        //enable ui buttons, just in case
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;
        GameObject sendNode = standingOn.GetComponent<Node>().gameObject;

        //adds 1 "movementBonus", to put things as they were
        movement += 1;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        Mover(sendNode);

        //just in case, actually cardfunction reset here might be bad idea, since this method is called sometimes in mid turn (such as time warp)
        //no, we actually do need to reset cardfunction, time warp uses different variable
        cardFunction = 0;
        wantToReturn = 0;

        //allow straight actions again
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;

        //start counting time again
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().countTurnTimer = true;
    }

    //same as above, but uses MoveBetweenNodes method instead
    //used for encounter check returns
    public void Cancel2()
    {
        //enable ui buttons, just in case
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

        //adds 1 "movementBonus", to put things as they were
        movement += 1;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        //Mover(nodeViewID);

        //just in case, actually cardfunction reset here might be bad idea, since this method is called sometimes in mid turn (such as time warp)
        cardFunction = 0;
        wantToReturn = 0;

        //updates the players standingOn -node across all the players
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn = PhotonView.Find(nodeViewID).GetComponent<Node>();

        MovePointReduction(1);

        //then moves avatar to it
        //GameManager.ins.nextNode.Arrive(gameObject);
        standingOn.MoveBetweenNodes(gameObject);

        //allow straight actions again
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;

        //start counting time again
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().countTurnTimer = true;
    }

    /* old agent methods
    //for placing agents
    public void PlaceAgent(int placement)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_PlaceAgent", RpcTarget.AllBufferedViaServer, placement);
        }
    }

    //for agents
    [PunRPC]
    void RPC_PlaceAgent(int placement)
    {
        //instantiate the agent, and set gamemanager as parent for now
        GameObject myAgent = Instantiate(GameManager.ins.agent, transform.position, transform.rotation, GameManager.ins.transform);

        myAgent.SetActive(true);

        //reduce size 90%
        myAgent.transform.localScale += new Vector3(-0.5f, -0.5f, -0.5f);

        //places the agent on the next node
        myAgent.GetComponent<Agent>().standingOn = PhotonView.Find(placement).GetComponent<Node>();

        //add agent to the players agent list
        agents.Add(myAgent);

        //increases the number of agents variable on the node
        myAgent.GetComponent<Agent>().standingOn.numberOfAgents += 1;

        //then moves avatar to it
        myAgent.GetComponent<Agent>().standingOn.NpcArrive(myAgent);

        //give the agent a number (needs to be 1 less than the count?)
        myAgent.GetComponent<Agent>().agentNumber = agents.Count -1;

        //change position slightly, might need to do this on all agent / npc position changes
        //myAgent.transform.position += new Vector3(0, 0, 0.52f);

        //lets do this on canvas controller
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

        //special case for edgar twoface quest
        if (GameManager.ins.questDeck[GameObject.Find("Quest Canvas").GetComponent<QuestingDialog>().numberInDeck].GetComponent<QuestCard>().specialEffect2 == 3)
        {
            GameObject.Find("Quest Canvas").GetComponent<QuestingDialog>().QuestEnd();
            return;
        }

        //since this is in rpc call, other players can get here, but theres current player check after
        else
        {
            //ends turn
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        }
    }

    //for summons agent moving
    public void SummonsEffect(int agentNumber)
    {
        //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
        if (PV.IsMine)
        {
            PV.RPC("RPC_SummonsEffect", RpcTarget.AllBufferedViaServer, agentNumber);
        }
    }

    //for agents
    [PunRPC]
    void RPC_SummonsEffect(int agentNumber)
    {
        //leaves internal node
        standingOn.NpcLeave(agents[agentNumber]);

        //moves agent to your location
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.NpcArrive(gameObject);
        standingOn.NpcArrive(agents[agentNumber]);

        //decreases the number of agents variable on the node
        agents[agentNumber].GetComponent<Agent>().standingOn.numberOfAgents -= 1;

        agents[agentNumber].GetComponent<Agent>().standingOn = standingOn;

        //increases the number of agents variable on the node
        agents[agentNumber].GetComponent<Agent>().standingOn.numberOfAgents += 1;
    }

    //for changing skill values when character enters / leaves th location of an agent
    public void AgentBonus(int bonus)
    {
        //for entering location with agent, should proc only once this way
        if (agentBonus == false && bonus == 1)
        {
            agentBonus = true;

            //give +1 to all stats
            UpdateStats(1, 1);
            UpdateStats(2, 1);
            UpdateStats(3, 1);
            UpdateStats(4, 1);
            UpdateStats(5, 1);
            UpdateStats(6, 1);
        }

        //for leaving location with agent, should proc only once this way
        if (agentBonus == true && bonus == -1)
        {
            agentBonus = false;

            //give -1 to all stats
            UpdateStats(1, -1);
            UpdateStats(2, -1);
            UpdateStats(3, -1);
            UpdateStats(4, -1);
            UpdateStats(5, -1);
            UpdateStats(6, -1);
        }
    }
    */

    //for single target attack cards
    //actually other players can and will activate this method, so make sure to use the rpc call accordingly
    public void Attack()
    {
        Debug.Log("Attack Hit this player!");

        //the attacking player will activate this, so the attack card number is still stored
        int numberInDeck = GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().temporaryNumber2;
        PV.RPC("RPC_Attack", RpcTarget.AllBufferedViaServer, numberInDeck);
    }

    //attacking
    [PunRPC]
    void RPC_Attack(int numberInDeck)
    {
        Debug.Log("Attack Hit via rpc call!");

        //transfer this variable to the attacked player as well
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().temporaryNumber2 = numberInDeck;

        //stop timer for the duration of an attack
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().countTurnTimer = false;

        //opens the attack dialog for this scripts owner (not to the one initiating the script)
        if (ItsYourTurn() == true)//(PV.IsMine)
        {
            Debug.Log("targets turn number: " + turnNumber);

            //gives turnnumber of attack target to all, should do this first probably
            //bit complicated way of doing this, hopefully it will work
            PV.RPC("RPC_TransferTurnNumber", RpcTarget.AllBufferedViaServer, turnNumber);

            //check if the player is warded (only needed against multitarget attack cards, since the single target cards are alrdy tested)
            //check for imprisonment also, since imprisoned heroes are also warded
            //actually this might not be the best spot to do this..
            //also does the spyglasstest now
            if (AreYouProtected() == true)
            {
                //go test next player instead (this one cannot be targetted)
                //test order should shift for mass attack cards only probably?
                if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType == 7 && GameManager.ins.questDeck[numberInDeck].GetComponent<QuestCard>().specialEffect >= 14 &&
                    GameManager.ins.questDeck[numberInDeck].GetComponent<QuestCard>().specialEffect <= 17)
                {
                    GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().TestNextPlayer();
                    return;
                }
            }

            //in case of word of tera quest and player has over 3 energy
            if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType == 7 && GameManager.ins.questDeck[numberInDeck].GetComponent<QuestCard>().specialEffect == 15 &&
                gameObject.GetComponentInChildren<Character>().energy > 3)
            {
                //go test next player instead (this one cannot be targetted)
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().TestNextPlayer();
                return;
            }

            //AI doesnt need to see attack canvas
            //it always submits to attack cards now, after a delay
            if(isAi == true)
            {
                Invoke("NoobAiResponse", 1.0f);
                return;
            }

            GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().attackCanvas.SetActive(true);

            //GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber = turnNumber;

            //sets the appropriate buttons
            //this is for when skill & defense cards are allowed
            //make more conditions for quest cards as well
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect >= 31 && GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect <= 40 &&
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType != 7)
            {
                //swaps button visibility
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton1.SetActive(true);
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton2.SetActive(false);
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().skillCheckButton.SetActive(true);
            }

            //this is for defense cards only
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect > 40 &&
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType != 7)
            {
                //swaps button visibility
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton1.SetActive(false);
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton2.SetActive(true);
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().skillCheckButton.SetActive(false);
            }

            //this is for defense cards only, on quest attack cards
            if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType == 7)
            {
                //swaps button visibility
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton1.SetActive(false);
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton2.SetActive(true);
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().skillCheckButton.SetActive(false);
            }

            //if player is sleeping, intelligence and skillcheck options disabled
            if(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 21, 7) > 0)
            {
                //swaps button availabliity
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton1.GetComponent<Button>().interactable = false;
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton2.GetComponent<Button>().interactable = false;
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().skillCheckButton.GetComponent<Button>().interactable = false;
            }

            //otherwise these options are enabled (might still be invisible though)
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 21, 7) == 0)
            {
                //swaps button availabliity
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton1.GetComponent<Button>().interactable = true;
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().intelligenceButton2.GetComponent<Button>().interactable = true;
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().skillCheckButton.GetComponent<Button>().interactable = true;
            }

            //spawns the attack card for the attacked player to see (dunno if we need check here, but game seems to spawn this card several time for some reason..)
            if (ItsYourTurn())
            {
                //destroy the previously displayed card, just in case it still lingers for no reason
                if(GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardDisplay.transform.childCount > 0)
                {
                    Destroy(GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardDisplay.transform.GetChild(0).gameObject);
                }
                if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType != 7)
                {
                    GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<CardDisplay>().cardImg, new Vector3(0, 0, 0), Quaternion.identity);
                    playerCard.transform.SetParent(GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardDisplay.transform, false);

                    //disables shadow
                    playerCard.GetComponent<Shadow>().enabled = false;
                }
                if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardType == 7)
                {
                    GameObject playerCard = Instantiate(GameManager.ins.questDeck[numberInDeck].GetComponent<CardDisplay>().cardImg, new Vector3(0, 0, 0), Quaternion.identity);
                    playerCard.transform.SetParent(GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().attackCardDisplay.transform, false);

                    //disables shadow
                    playerCard.GetComponent<Shadow>().enabled = false;
                }

                //opens waiting window for other players
                PV.RPC("RPC_OpenInterruptWindow", RpcTarget.AllBufferedViaServer, turnNumber);
            }
        }
    }

    void NoobAiResponse()
    {
        GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().doNothingButton.onClick.Invoke();
    }

    //opens interruption windows for other players
    [PunRPC]
    void RPC_OpenInterruptWindow(int tTurnNumber)
    {
        if(GameManager.ins.avatars[tTurnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().interruptWindow.SetActive(true);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().interruptText.text = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " is attacking against " +
                GameManager.ins.avatars[tTurnNumber].GetComponentInChildren<Character>().heroName + "!";
        }
    }

    //tests if you have spyglass, and your agent is at your location
    public bool SpyGlassTest()
    {
        bool agentIsWithYou = false;

        //tests if theres your agent on your location
        if (agents != null)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (ReferenceEquals(standingOn, agents[i].GetComponent<Agent>().standingOn))
                {
                    agentIsWithYou = true;
                }
            }
        }

        //tests if player has spyglass
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 36 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    if(agentIsWithYou == true)
                    {
                        return true;
                    }
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have talisman of balance
    public bool TalismanTest()
    {
        //tests if player has spyglass
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 37 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //actually lets do a method which tests all the protection, to make things less complicated
    public bool AreYouProtected()
    {
        //returns true if youre protected
        //if (isWarded > 0 || isImprisoned > 0 || SpyGlassTest() == true || TalismanTest() == true)
        {
            return true;
        }

        //return false;
    }

    //tests whether you have wyrms blood artifact
    public bool WyrmsBloodTest()
    {
        //tests if player has spyglass
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++) //GameObject.Find("Artifact Canvas").GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 38 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    /* more pointless artifact etc tests
    //tests whether you have stone of alroman artifact
    public bool StoneOfAlromanTest()
    {
        //tests if player has stone of alroman
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 39 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have loremaster perk
    public bool LoremasterTest()
    {
        //tests if player has stone of alroman
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 64 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have side projects perk artifact
    public bool SideProjectsTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 51 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have victory rush perk
    public bool VictoryRushTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 54 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have stealth perk
    public bool StealthTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 55 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have stealth perk
    public bool RelentlessTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 56 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have heavy armor perk
    public bool HeavyArmorTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 57 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have swiftness perk
    public bool SwiftnessTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 3 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have contemplation perk
    public bool ContemplationTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 58 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have sentinel perk
    public bool SentinelTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 59 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have priestess perk
    public bool PriestessTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 60 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have priestess perk
    public bool BardSkillsTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 61 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have priestess perk
    public bool BookwormTest()
    {
        //tests if player has side projects
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 63 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have priestess perk
    public bool ForesightTest()
    {
        //tests if player has soothsayer perk
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 52 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //tests whether you have guitar of gereon
    public bool GuitarTest()
    {
        //tests if player has spyglass
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 40 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether you have guitar of gereon
    public bool LibraryKeyTest()
    {
        //tests if player has spyglass
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 51 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests if you have any artifacts
    public bool HasArtifact()
    {
        //tests if player has any artifact
        for (int i = 0; i < GameObject.Find("Artifact Canvas").GetComponent<Transform>().childCount; i++)
        {
            if (GameObject.Find("Artifact Canvas").transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameObject.Find("Artifact Canvas").transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }
    */

    public void SynchronizeTurnNumber()
    {
        if (PV.IsMine)
        {
            //gives turnnumber of the player for all
            PV.RPC("RPC_SynchronizeTurnNumber", RpcTarget.AllBufferedViaServer, turnNumber);
        }
    }

    //synchronizes turn number between all players
    [PunRPC]
    void RPC_SynchronizeTurnNumber(int transferredNumber)
    {
        //GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber = transferredNumber;
        GameManager.ins.avatars[transferredNumber].GetComponent<CharController>().turnNumber = transferredNumber;
    }

    //attacking
    [PunRPC]
    void RPC_TransferTurnNumber(int transferredNumber)
    {
        GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber = transferredNumber;
    }

    /* old int card checks
    //removes marching order from artifact canvas
    public void RemoveMarching(int turnNumber)
    {
        //tests if player is marching
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<IntelligenceCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<IntelligenceCard>().effect == 19 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    //"returns" card to deck
                    GameManager.ins.intelligenceDeck[GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck].GetComponent<CardDisplay>().isTaken = false;

                    Destroy(GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject);

                    //remove the movementBonus bonus
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().movementBonus -= 1;

                    //removes the card displayed (just in case)
                    if (GameObject.Find("Card Canvas").transform.childCount > 0)
                    {
                        Destroy(GameObject.Find("Card Canvas").transform.GetChild(0).gameObject);
                    }
                }
            }
        }
    }

    //removes the lethos ward from artifact canvas
    public void RemoveWarding(int turnNumber)
    {
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<IntelligenceCard>() != null)
            {
                //check that its a ward card, and belongs to the right player
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<IntelligenceCard>().effect == 20 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    //"returns" card to deck
                    GameManager.ins.intelligenceDeck[GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck].GetComponent<CardDisplay>().isTaken = false;

                    Destroy(GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject);

                    //removes the card displayed (just in case)
                    if (GameObject.Find("Card Canvas").transform.childCount > 0)
                    {
                        Destroy(GameObject.Find("Card Canvas").transform.GetChild(0).gameObject);
                    }
                }
            }
        }
    }

    public void RemoveBookkeeping(int turnNumber)
    {
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<IntelligenceCard>() != null)
            {
                //check that its a ward card, and belongs to the right player
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<IntelligenceCard>().effect == 21 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    //"returns" card to deck
                    GameManager.ins.intelligenceDeck[GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck].GetComponent<CardDisplay>().isTaken = false;

                    Destroy(GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject);

                    //removes the card displayed (just in case)
                    if (GameObject.Find("Card Canvas").transform.childCount > 0)
                    {
                        Destroy(GameObject.Find("Card Canvas").transform.GetChild(0).gameObject);
                    }
                }
            }
        }
    }

    //removes sleep from artifact canvas
    //technically not needed for ai player
    public void RemoveSleeping(int turnNumber)
    {
        //tests if player is sleeping
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == 21 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                {

                    Destroy(GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject);

                    //remove sleep overlay
                    GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().sleepOverlay.SetActive(false);
                }
            }
        }
    }

    //bookkeeping
    public void CheckBookkeeping()
    {
        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            //dont check your agents
            if (GameManager.ins.turnNumber != i)
            {
                for (int y = 0; y < GameManager.ins.avatars[i].GetComponent<CharController>().agents.Count; y++)
                {
                    //check all other players agents
                    if (ReferenceEquals(GameManager.ins.avatars[i].GetComponent<CharController>().agents[y].GetComponent<Agent>().standingOn, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn))
                    {
                        Debug.Log("finds agent on same location");
                        //check if that player has bookkeeping active
                        if (GameManager.ins.avatars[i].GetComponent<CharController>().isBookkeeping > 0)
                        {
                            Debug.Log("finds bookkeeping, should give coins");
                            //that player gains 3 coins (need to test)
                            //GameManager.ins.avatars[i].GetComponent<CharController>().UpdateResources(4, 3);
                            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().UpdateAnyPlayersResources(i, 4, 3);
                        }
                    }
                }
            }
        }
    }
    */

    //for imprisoned actions, boolean determines whether youre paying bail or not
    public void ReduceJailTime(bool bail)
    {
        PV.RPC("RPC_ReduceJailTime", RpcTarget.AllBufferedViaServer, bail);
    }

    //for imprisoned actions
    [PunRPC]
    void RPC_ReduceJailTime(bool bail)
    {
        //in case of imprisoned
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned > 0)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned -= 1;
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned == 0)
            {
                RemoveImprisonment(GameManager.ins.turnNumber);
            }
            // to make sure only you call this
            if (PV.IsMine && bail == false)
            {
                //ends players turn, if he was waited
                EndTurn();
            }
            // to make sure only you call this
            if (PV.IsMine && bail == true)
            {
                //continue turn, if bail was paid
                //StartTurn();

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned == 0)
                {
                    GameManager.ins.currentNode.SetReachableNodes(true);

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
                }
            }
        }
    }

    //removes imprisonment from artifact canvas
    public void RemoveImprisonment(int turnNumber)
    {
        //tests if player is imprisoned
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<EventCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<EventCard>().effect == 2 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {

                    Destroy(GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject);

                    //removes the card displayed (just in case)
                    if (GameObject.Find("Card Canvas").transform.childCount > 0)
                    {
                        Destroy(GameObject.Find("Card Canvas").transform.GetChild(0).gameObject);
                    }
                }
            }
        }

        //add imprisonment overlay
        GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().imprisonmentOverlay.SetActive(false);
    }

    //easier to put this in here
    //for drawing quest and int cards only
    public void AfterEffect(int resourceType, int resourceQty)
    {
        //lets try new method for this
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCardsCurrentOnly(resourceType, resourceQty);

        //special case for upgraded temple, additional arcanist
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 6 &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 1);
        }

        //special case for upgraded inn, additional energy
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 2 &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 1);
        }

        //update character energy & interaction token
        //better do this at aftereffect
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

        //this shuts down dialogs
        if (resourceType == 7)
            GameObject.Find("Quest Discard Prompt").gameObject.SetActive(false);

        if (resourceType == 8)
            GameObject.Find("Intelligence Discard Prompt").gameObject.SetActive(false);
        
        //gameObject.SetActive(false);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
    }

    /* not used atm
    //removes all cards from pyramid offer, and sets new cards
    public void ArtifactReset()
    {
        if (GameManager.ins.pyramidOffer.Count != 0)
        {
            if (GameManager.ins.pyramidOffer[2] != null)
            {
                int cardNumber = GameManager.ins.pyramidOffer[2].GetComponent<CardDisplay>().numberInDeck;
                GameManager.ins.artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = false;
                GameManager.ins.pyramidOffer.RemoveAt(2);
            }
            if (GameManager.ins.pyramidOffer[1] != null)
            {
                int cardNumber = GameManager.ins.pyramidOffer[1].GetComponent<CardDisplay>().numberInDeck;
                GameManager.ins.artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = false;
                GameManager.ins.pyramidOffer.RemoveAt(1);
            }
            if (GameManager.ins.pyramidOffer[0] != null)
            {
                int cardNumber = GameManager.ins.pyramidOffer[0].GetComponent<CardDisplay>().numberInDeck;
                GameManager.ins.artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = false;
                GameManager.ins.pyramidOffer.RemoveAt(0);
            }
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //draws 3 cards
            for (int i = 0; i < 3; i++)
            {
                //GameManager.ins.pyramidOffer.Add(TakeArtifactCard());

                int cardNumber = GameManager.ins.TakeArtifactCardNumber();

                PV.RPC("RPC_PyramidOffer", RpcTarget.AllBufferedViaServer, cardNumber);
            }
        }
    }

    //for draws cards for the pyramid offer
    [PunRPC]
    void RPC_PyramidOffer(int cardNumber)
    {
        GameManager.ins.artifactDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

        GameManager.ins.pyramidOffer.Add(GameManager.ins.artifactDeck[cardNumber]);
    }
    */

    //gives player number over network to character class
    //number1 is turnNumber, number2 is hero icon (hero type) number
    public void GetPlayerNumber(int number1, int number2)
    {
        //Debug.Log("number1 is: " + number1 + ", number2 is: " + number2);
        //dont need to do this in sp?
        if (ItsYourTurn() && PhotonRoom.room.isPractice == false)
        {
            PV.RPC("RPC_GetPlayerNumber", RpcTarget.AllBufferedViaServer, number1, number2);
            //gameObject.GetComponent<PhotonView>().RPC("RPC_GetPlayerNumber", RpcTarget.AllBufferedViaServer, number1, number2);
        }
    }

    //gives player number over network to character class
    [PunRPC]
    void RPC_GetPlayerNumber(int number1, int number2)
    {
        GameManager.ins.avatars[number1].GetComponent<CharController>().GetComponentInChildren<Character>().heroNumber = number2;
    }

    //refreshes the forces encounter check flags
    public void RefreshForcedEncounterChecks()
    {
        /*
        for (int i = 0; i < GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters.Count; i++)
        {
            GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters[i].GetComponent<Npc>().isChecked = false;
        }
        */
    }

    //gives player number over network to character class
    [PunRPC]
    void RPC_SetTotalScore(int turnNb, int score)
    {
        GameManager.ins.avatars[turnNb].GetComponent<CharController>().totalScore = score;
    }

    //gives player number over network to character class
    [PunRPC]
    void RPC_IsSelfMaintenance(int turnNb, bool value)
    {
        GameManager.ins.avatars[turnNb].GetComponentInChildren<Character>().isSelfMaintenance = value;
    }

    //for reputation displays
    public void ShowReputationDisplay()
    {
        if (PV.IsMine)
        {
            string userName = "";

            if (isAi == false)
            {
                userName = GameObject.Find("ChatManager").GetComponent<PhotonChatManager>().username;
            }
            else
            {
                userName = "Bot";
            }

            //gives turnnumber of the player for all
            PV.RPC("RPC_ShowReputationDisplay", RpcTarget.AllBufferedViaServer, turnNumber, GetComponentInChildren<Character>().heroNumber, GetComponentInChildren<Character>().heroName, userName);
        }
    }

    [PunRPC]
    void RPC_ShowReputationDisplay(int turnNumber, int heroNumber, string heroName, string userName)
    {
        //show your characters hero and score display at bottom left corner
        GameManager.ins.scoreCanvasController.GetComponent<ScoreCanvasController>().ShowCharacterInfo(GetComponentInParent<CharController>().turnNumber, heroNumber, heroName, userName);

        //update the display
        gameObject.GetComponentInChildren<Character>().UpdateVisibleReputation();

        //update resource texts
        gameObject.GetComponentInChildren<Character>().UpdateResourceTexts();
    }

    //tests whether you have card of certain effect on passive tray
    public bool HasPassiveTest(int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    public bool CheckIfHeroDead()
    {
        //end game if hero is dead (and youre the only player, change later for mp purposes)
        if (GameManager.ins.numberOfPlayers == 1 && isDead == true)
        {
            //GameManager.ins.finalWindows.GetComponent<FinishHandler>().defeatPrompt.SetActive(true);

            //for v92
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenDefeatWindow();

            GameManager.ins.uiButtonHandler.GetComponent<UiButtonHandler>().CloseAllDisplays();

            GameManager.ins.uiButtonHandler.GetComponent<UiButtonHandler>().DisableAllButtons();

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

            //stop musics (dont rly need to now?)
            //GameManager.ins.references.soundManager.mainMusicHolder.Stop();
            //GameManager.ins.references.soundManager.encounterMusicHolder.Stop();

            //starts defeat music
            //actually lets use score music for all of these events
            //GameManager.ins.sfxPlayer.GetComponent<SoundManager>().PlayDefeatMusic();
            GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(4);
            return true;
        }
        return false;
    }

    //for calling stat update of the Character class
    public void StatUpdate()
    {
        if (ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_StatUpdate", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_StatUpdate()
    {
        gameObject.GetComponentInChildren<Character>().StatUpdate();
    }
}
