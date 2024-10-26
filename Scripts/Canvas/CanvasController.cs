using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using TMPro;

//class for managing the various dialogs, and applying intelligence effect
public class CanvasController : MonoBehaviour
{
    //test
    private PhotonView PV;

    public GameObject actionCancelledCanvas;
    public GameObject handCardDiscard;
    public GameObject cardOptionsCanvas;
    public GameObject artifactOptionsCanvas;

    public GameObject questCanvas;
    public GameObject intelligenceUse;

    //this shouldnt be needed anymore, since recharging cards are no more
    public GameObject reclaimDialog;

    public GameObject boomstickDialog;
    public GameObject donationDialog;
    public GameObject bottomTextCanvas;
    public GameObject encounterSelectDialog;
    public GameObject claimQuestPrompt;
    public GameObject questingDialog;
    public GameObject questSelectionPrompt;
    public GameObject ProloguePrompt;
    public GameObject StoryPrompt;
    public GameObject TutorialStartPrompt;
    public GameObject TutorialEndPrompt;
    public GameObject tutorialHandler;
    //public GameObject exploreOption;

    public Button PrologueOk;
    public Button EpilogueNextButton;
    public Button EpilogueFinishButton;
    public Button DefeatNext;
    public Button StoryCloseButton;
    public Button RetryButton;

    public Button TutorialStartOk;
    public Button TutorialEndOk;
    //public Button exploreOptionOk;

    //crafting canvases & buttons
    public GameObject smithingCanvas;
    public GameObject scribingCanvas;

    //for options menu
    public GameObject optionsPanel;
    public bool optionsPanelActivated;
    public GameObject optionsDialog;
    public GameObject confirmationDialog;

    public Text actionCancelledText;
    public Text handDiscardText;
    public Text intelligenceUseText;
    public Text bottomText;

    //for storing the stored card info(s)
    public List<int> storeNumberInDeck;

    //another variable needed to store number in deck
    public int temporaryNumber;

    //another variable needed to store number in deck (for attack card use atm)
    public int temporaryNumber2;

    //for storing event cards number in deck
    public int eventCardNumber;

    //store combat magic boosts
    public List<int> combatMagicBoost;

    public bool inspirationUsed;

    //note that this flag variable is only used by divination, and is reset immediately after its checked
    public bool usingOvermapDivination;

    //for imprisonment bail test, when true game wont open nodes for movementBonus
    public bool bailTest;

    //for storing the temporary effects
    public List<int> storeTemporaryEffect;

    //various buttons for abcanvas usage (might as well put these here)
    public GameObject contemplateButton;
    public GameObject sleepButton;
    public GameObject selfMaintenanceButton;
    public GameObject foresightButton;
    public GameObject seekQuestButton;
    public GameObject researchButton;
    public GameObject attemptQuestButton;
    public GameObject useIntelligenceButton;
    public GameObject restButton;
    public GameObject investigateButton;
    public GameObject smithingButton;
    public GameObject scribingButton;
    public GameObject eventDisplay;
    public Button eventOkButton;

    //for interrupt window (used when defending against attacks atm)
    public GameObject interruptWindow;
    public Text interruptText;

    //scrolling text elements for v92
    //actually these are unused in v0.7.0.
    //public TextMeshProUGUI prologueText;
    //public TextMeshProUGUI epilogueTextLoss;
    //public TextMeshProUGUI epilogueTextWin;
    //public TextMeshProUGUI defeatText;

    //this is still used
    public TextMeshProUGUI epilogueTextScore;

    //for v95
    public TextMeshProUGUI eventText;
    public Vector3 eventTextStartPosition;
    //public Vector3 defeatTextStartPosition;

    //special panels for defeat prompt (kinda unoptimal way of doing this)
    public GameObject storyPanel;
    public GameObject defeatPanel;
    public GameObject darkStoryPanel;
    public Image storyImage;
    public Image prologueImage;

    //could transfer this to be saved in gamemanager
    public int finalScore;

    public float screenHeight;
    public float scrollSpeed;

    public int finalImagesNumber;

    // Start is called before the first frame update
    void Start()
    {
        eventTextStartPosition = eventText.gameObject.transform.position;
        //defeatTextStartPosition = defeatText.gameObject.transform.position;

        screenHeight = Screen.height;
        finalImagesNumber = 0;

        PV = GetComponent<PhotonView>();

        bailTest = false;
        optionsPanelActivated = false;
        usingOvermapDivination = false;

        if (GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isTutorial == true)
        {
            TutorialStartPrompt.SetActive(true);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);
            TutorialStartOk.interactable = false;
        }

        else
        {
            SetPrologue();
        }

        //play sfx
        //GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess1();

    }


    // Update is called once per frame
    //could be used for various text scrolls
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionsPanel.GetComponent<Canvas>().enabled = !optionsPanel.GetComponent<Canvas>().enabled;
        }
        */

        float finalScrollSpeed = screenHeight * scrollSpeed * Time.deltaTime;

        /*
        if (ProloguePrompt.activeSelf)
        {
            prologueText.gameObject.transform.position += new Vector3 (0, finalScrollSpeed, 0);
        }
        */

        //lets put all the event prompts in single holder in v0.7.0.
        //too stupid otherwise
        if (StoryPrompt.activeSelf)
        {
            /*
            if (epilogueTextLoss.gameObject.activeSelf)
            {
                epilogueTextLoss.gameObject.transform.position += new Vector3(0, finalScrollSpeed, 0);
            }
            if (epilogueTextWin.gameObject.activeSelf)
            {
                epilogueTextWin.gameObject.transform.position += new Vector3(0, finalScrollSpeed, 0);
            }
            if (defeatText.gameObject.activeSelf)
            {
                defeatText.gameObject.transform.position += new Vector3(0, finalScrollSpeed, 0);
            }
            */
            if (eventText.gameObject.activeSelf)
            {
                eventText.gameObject.transform.position += new Vector3(0, finalScrollSpeed, 0);
            }
        }
    }


    void SetPrologue()
    {
        eventText.gameObject.transform.position = eventTextStartPosition;

        if (PhotonRoom.room.spContinue == false)
        {
            /*
            ProloguePrompt.SetActive(true);
            PrologueOk.interactable = false;
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);
            */
            eventText.text = "<size=10>\n</size>You awake in a damp catacomb. You were on a kingseeker trial, to fight undead and gain a name for yourself before the upcoming election.<br><br>" +
                "You see bodies fallen adventurers around, and remember there was an earthquake. It's a miracle youre still alive, judging from the damage around, it seems you really shouldnt be.<br><br>" +
                "Either way, you really should get moving. Unfortunately the entrance from which you came seem blocked by unpassable debris..";

            storyImage.sprite = GameManager.ins.references.eventSprites[9];

            StoryPrompt.SetActive(true);
            PrologueOk.interactable = false;
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);

            EnableStoryButtons(1);

            Invoke(nameof(PrologueSFX), 0.5f);
        }
        else
        {
            eventText.text = "Your journey continues.";
            storyImage.sprite = GameManager.ins.references.eventSprites[8];

            StoryPrompt.SetActive(true);
            PrologueOk.interactable = false;

            EnableStoryButtons(1);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);
        }
    }

    public void PrologueSFX()
    {
        CardHandler.ins.extraSfxHolder.clip = GameManager.ins.references.sfxPlayer.EarthShatter;
        CardHandler.ins.extraSfxHolder.Play();
    }

    //type 1 = prologue, 2 regular event, 3 end by death, 4 end by timer (epilogue next), 5 end by victory (same as timer end?), 6 epilogue finish
    public void EnableStoryButtons(int type)
    {
        if (type == 1)
        {
            PrologueOk.gameObject.SetActive(true);
            EpilogueNextButton.gameObject.SetActive(false);
            EpilogueFinishButton.gameObject.SetActive(false);
            DefeatNext.gameObject.SetActive(false);
            StoryCloseButton.gameObject.SetActive(false);
            RetryButton.gameObject.SetActive(false);
        }
        if (type == 2)
        {
            PrologueOk.gameObject.SetActive(false);
            EpilogueNextButton.gameObject.SetActive(false);
            EpilogueFinishButton.gameObject.SetActive(false);
            DefeatNext.gameObject.SetActive(false);
            StoryCloseButton.gameObject.SetActive(true);
            RetryButton.gameObject.SetActive(false);
        }
        if (type == 3)
        {
            PrologueOk.gameObject.SetActive(false);
            EpilogueNextButton.gameObject.SetActive(false);
            EpilogueFinishButton.gameObject.SetActive(false);
            DefeatNext.gameObject.SetActive(true);
            StoryCloseButton.gameObject.SetActive(false);
            RetryButton.gameObject.SetActive(true);
        }
        //timer end
        if (type == 4)
        {
            PrologueOk.gameObject.SetActive(false);
            EpilogueNextButton.gameObject.SetActive(true);
            EpilogueFinishButton.gameObject.SetActive(false);
            DefeatNext.gameObject.SetActive(false);
            StoryCloseButton.gameObject.SetActive(false);
            RetryButton.gameObject.SetActive(false);
        }
        //end by victory
        //same as 4?
        if (type == 5)
        {
            PrologueOk.gameObject.SetActive(false);
            EpilogueNextButton.gameObject.SetActive(true);
            EpilogueFinishButton.gameObject.SetActive(false);
            DefeatNext.gameObject.SetActive(false);
            StoryCloseButton.gameObject.SetActive(false);
            RetryButton.gameObject.SetActive(false);
        }
        //enable epilogue finish
        if (type == 6)
        {
            PrologueOk.gameObject.SetActive(false);
            EpilogueNextButton.gameObject.SetActive(false);
            EpilogueFinishButton.gameObject.SetActive(true);
            DefeatNext.gameObject.SetActive(false);
            StoryCloseButton.gameObject.SetActive(false);
            RetryButton.gameObject.SetActive(false);
        }
    }

    /* old stuff
    //used for interactions and intelligence cards mostly, for some quest cards also
    public void ActionCancelled(int action)
    {
        //need these now, in case of straight action
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

        actionCancelledCanvas.gameObject.SetActive(true);

        //lets disable this by default, so dont need to do this in card display
        questSelectionPrompt.gameObject.SetActive(false);

        //this action is for artifact canvas
        if (action == 1)
        {
            //destroys the artifact offer cards
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                GameManager.ins.GM.RPC("RPC_DestroyArtifactOffer", RpcTarget.AllBufferedViaServer);
            }

            actionCancelledText.text = "You didnt have enough resources to get the artifact.";

            //this shuts down the previous dialog 
            GameManager.ins.artifactOptionsCanvas.SetActive(false);
        }

        //this action is for deficit in interaction resources
        if (action == 2)
        {
            actionCancelledText.text = "You didnt have enough resources to do the interaction.";

            //use flag variable in case this is being called from imprisonment bail dialog
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().isImprisoned > 0)
                bailTest = true;
        }

        //this action is for deficit in interaction resources
        if (action == 3)
        {
            //inspirationUsed = false;
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You didnt have enough resources to use the intelligence card.";
        }

        //this action is for standing on wrong location
        if (action == 4)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You cant use this intelligence card at this location.";
        }

        //this action is for not having valid targets
        if (action == 5)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "There was no valid targets for the card.";
        }

        //this action is for when using night-time card at day-time
        if (action == 6)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You cannot use this card during daytime.";
        }

        //when theres alrdy 2 agents in a location
        if (action == 7)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "There cant be more than 2 agents on a location.";
        }

        //when you dont have any agents to summon
        if (action == 8)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You dont have any agents.";
        }

        //you alrdy the same card in effect
        if (action == 9)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You already have that card in effect.";
        }

        //you alrdy have maximum number of agents
        if (action == 10)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You already have maximum number of agents.";
        }
        //this action is for not having valid targets
        if (action == 11)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "There was no valid targets to spy on.";
        }

        //this action is for not having enough coins to buy an artifact (for scroll of acquisition int card)
        if (action == 12)
        {
            //destroys the artifact offer cards
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //destroys displayed cards
                GameObject.Find("Spying").GetComponent<SpyCanvas>().DestroyDisplayedCards();

                //closes spy dialog and ends turn
                //activate the canvas
                GameObject.Find("Spying").GetComponent<SpyCanvas>().spyCanvas.SetActive(false);

                actionCancelledText.text = "You didnt have enough coins to get the artifact.";

                //this shuts down the previous dialog 
                //GameManager.ins.artifactOptionsCanvas.SetActive(false);
            }
        }
        //this action is for not having special encounters on the location
        if (action == 13)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "There is no encounters on this location.";
        }

        //message for not having enough resources for the action (could refer to several things
        if (action == 14)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You didnt have enough resources to do the action.";
        }
        //this action is for when using night-time card at day-time
        if (action == 15)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You cannot use this card during night-time.";
        }

        //when talisman prevents you from using intelligence / quest card
        if (action == 16)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "Talisman of Balance prevents you from playing attack cards!";
        }

        //when you dont have lividium powder for smithing
        if (action == 17)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You dont have lividium powder.";
        }

        //when you dont have blank tome for scribing
        if (action == 18)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You dont have blank tome.";
        }

        //when you dont have blank tome for scribing
        if (action == 19)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You're not a soothsayer.";
        }

        //when theres no targets to investigate
        if (action == 20)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "There's no targets to investigate.";
        }

        //message about using attack cards in the first 12 hours
        if (action == 21)
        {
            SetInspiration(false);
            intelligenceUse.gameObject.SetActive(false);
            actionCancelledText.text = "You cannot use attack cards in the first 12 hours of game.";
        }
    }
    */

    /* old stuff
    //confirm button from ac
    //this also now handles encounter action cancels
    public void ActionCancelConfirmed()
    {
        //enable ui buttons
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //this closes a dialog 
        //actually lets close encounterselection canvas as well..
        actionCancelledCanvas.SetActive(false);
        encounterSelectDialog.SetActive(false);

        //closes all encounter colliders
        GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CloseEncounterColliders();
        GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CloseAllHeroColliders();

        //in case of failed imprisonment bailout
        if (bailTest == true)
        {
            //enable interaction again
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.inter.enabled = true;

            //reset flag
            bailTest = false;

            return;
        }

        //for int cards (or cancelling questing due to nighttime)
        //also various other non turn ending cancels should proc this
        //the code above is bit messily made though
        else
        {
            //takes ViewID of the node the avatar is moving to
            int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

            //adds 1 "movementBonus", to put things as they were
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints += 1;

            //sends the nodeviewid to charcontrollers method
            //returns players "turn"
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

            //just in case
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;

            //in case of motivate
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect == 2)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect = 0;
            }
        }

        //lets allow straight actions again
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;

        //make sure this text is set back to original
        //intelligenceUseText.text = "Choose Intelligence card to use >";
    }

    //for valley interaction reward
    public void ValleyReward()
    {
        Debug.Log("valley reward");

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;

        //update character energy & interaction token
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

        //special case for upgraded valley, 1 favor, 3 energy
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 11 &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == true)
        {
            //updates info on character class
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
        }
        else
        {
            //1 favor, 2 energy
            //updates info on character class
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 2);
        }
        //ends turn
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();

        //closes the dialog
        handCardDiscard.SetActive(false);
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
    }

    //for regret card reward
    public void RegretReward()
    {
        temporaryNumber = 0;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;

        //2 favor
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 2);
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 2);

        //ends turn
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        //actually regret shouldnt end turn
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();

        //just in case
        intelligenceUse.SetActive(false);

        //closes the dialog
        handCardDiscard.SetActive(false);
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
    }

    //for intelligence cards used in quests
    public void UseIntelligenceForQuest()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //closes tooltip
        if (GameObject.Find("ToolTipBackground").gameObject.activeSelf)
        {
            GameObject.Find("ToolTipBackground").SetActive(false);
        }

        questCanvas.SetActive(false);
        intelligenceUse.SetActive(true);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 8;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 5;

    }

    public void UseIntelligenceAsAction()
    {
        //reset this here
        intelligenceUseText.text = "Choose Intelligence card to use >";

        intelligenceUse.SetActive(true);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 8;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 6;

        //reset the attack resolve list here, cause it seems to work funky
        gameObject.GetComponent<AttackResolve>().storeTemporaryEffect.Clear();

        //close all displays except hand card display, disable buttons
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.handCardDisplay.SetActive(true);
        GameManager.ins.uiButtonHandler.DisableAllButtons();
    }

    //lets set this variable as rpc call (although the delay might be a problem..
    public void SetInspiration(bool isInspired)
    {
        PV.RPC("RPC_SetInspiration", RpcTarget.AllBufferedViaServer, isInspired);
    }

    [PunRPC]
    void RPC_SetInspiration(bool isInspired)
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().inspirationUsed = isInspired;
    }
    */

    /* old 
    //this method works scarily, because it comes through rpc call (hence many itsyourturn -checks)
    //should do something about this soon, if more intelligence cards bug out
    public void IntelligenceUsedAsAction(int numberInDeck)
    {
        //in v85, lets allow only current player to get here
        if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        //special case for inspiration
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 9)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //set this here, in case of straight action
                intelligenceUse.SetActive(true);
                intelligenceUseText.GetComponent<Text>().text = "Choose Intelligence card to use for free >";
            }

            SetInspiration(true);

            inspirationUsed = true;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 8;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 6;

            GameManager.ins.uiButtonHandler.CloseAllDisplays();
            GameManager.ins.uiButtonHandler.handCardDisplay.SetActive(true);
            GameManager.ins.uiButtonHandler.DisableAllButtons();
            return;
        }

        //else enable buttons again
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        //check if you have talisman of balance, and therefore cant use attack cards
        //changed this on v85
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().isAttack == true)
        {
            intelligenceUse.SetActive(false);

            //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().TalismanTest() == true)
            {
                //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                if (inspirationUsed == false && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                   // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(numberInDeck);
                }

                //makes sure it your turn
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    ActionCancelled(16);
                }
                return;
            }
        }

        //special case when using attack cards before turn 12
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().isAttack == true && Clock.clock.totalTurnsPlayed < 6)
        {
            intelligenceUse.SetActive(false);

            //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
            if (inspirationUsed == false && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(numberInDeck);
            }

            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                ActionCancelled(21);
            }
            return;

        }

        //special case for sentinel schematic, check if player has less than 10 coins
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 10 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < 10)
        {
            //returns card here, and message about insufficient resources, flag is false so you cant gain duplicate cards
            if (inspirationUsed == false)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(numberInDeck);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                ActionCancelled(3);
            }
            return;
        }

        //special case for pious donation, check if player has less than 10 coins, or is on wrong spot
        if ((GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 12 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < 10) ||
            (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 12 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber != 8))
        {
            //returns card here, and message about insufficient resources, flag is false so you cant gain duplicate cards
            if (inspirationUsed == false && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(numberInDeck);
            }
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 12 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber != 8)
            {
                ActionCancelled(4);
                return;
            }
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 12 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < 10)
            {
                ActionCancelled(3);
                return;
            }
            return;
        }

        if (inspirationUsed == false)
        {
            intelligenceUse.SetActive(false);
        }

        //store cards number
        temporaryNumber2 = numberInDeck;

        //apply the int card effect
        ApplyIntelligenceEffect(GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect);
    }
    */

    /* shouldnt need this
    public void IntelligenceUseCancel()
    {
        //enable ui buttons again, disable displays
        GameManager.ins.uiButtonHandler.EnableAllButtons();
        GameManager.ins.uiButtonHandler.CloseAllDisplays();

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        SetInspiration(false);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction == 5)
        {
            questCanvas.SetActive(true);
            intelligenceUse.SetActive(false);
        }

        //for overmap intelligence cards used on your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction == 6)
        {
            intelligenceUse.SetActive(false);

            //takes ViewID of the node the avatar is moving to
            int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

            //adds 1 "movementBonus", to put things as they were
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints += 1;

            //sends the nodeviewid to charcontrollers method
            //returns players "turn"
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

            //just in case
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 0;

            //allow straight actions again
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
        }
    }
    */

    /*should be unused method, since you no longer use intelligence for quests?
    public void IntelligenceUsedFQ(int numberInDeck)
    {
        //special case for inspiration
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 9)
        {
            intelligenceUseText.text = "Choose Intelligence card to use for free >";
            inspirationUsed = true;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 8;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 5;
            return;
        }

        if (inspirationUsed == false)
        {
            intelligenceUse.SetActive(false);
        }

        //special case for boomsticks
        if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect == 7)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                boomstickDialog.SetActive(true);
            }
            temporaryNumber = numberInDeck;
            return;
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
        {
            if (GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().rechargeable == true && inspirationUsed == false)
            {
                reclaimDialog.SetActive(true);
                temporaryNumber = numberInDeck;
            }
            else
            {
                questCanvas.SetActive(true);
            }
        }

        //apply the int card effect
        ApplyIntelligenceEffect(GameManager.ins.intelligenceDeck[numberInDeck].GetComponent<IntelligenceCard>().effect);
    }
    */

    /*for boomstick effects
    public void BoomstickDigging()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        ApplyIntelligenceEffect(4);

        questCanvas.SetActive(true);
        boomstickDialog.SetActive(false);

    }

    public void BoomstickStrength()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        ApplyIntelligenceEffect(7);

        //dunno why this wasnt here previously
        questCanvas.SetActive(true);

        boomstickDialog.SetActive(false);
    }
    */

    /* old int effects
    //this method is as messy as the one which calls it
    //shouldnt probably allow non-current player to get here at all, although inspiration flag wont be resetted then?
    public void ApplyIntelligenceEffect(int effect)
    {
        //this is for temporary +3 strength
        if (effect == 1)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(1, 3);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthText.color = Color.yellow;
            }

            //stores the effect to the list
            storeTemporaryEffect.Add(1);
        }
        //this is for temporary +3 influence
        if (effect == 2)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(2, 3);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceText.color = Color.yellow;
            }
            storeTemporaryEffect.Add(2);
        }
        //this is for temporary +3 mechanics
        if (effect == 3)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(3, 3);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsText.color = Color.yellow;
            }
            storeTemporaryEffect.Add(3);
        }

        //this is for temporary +3 digging
        if (effect == 4)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(4, 3);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingText.color = Color.yellow;
            }
            storeTemporaryEffect.Add(4);
        }
        //this is for temporary +3 lore
        if (effect == 5)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(5, 3);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreText.color = Color.yellow;
            }
            storeTemporaryEffect.Add(5);
        }
        //this is for temporary +3 discovery
        if (effect == 6)
        {
            if (usingOvermapDivination == false)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(6, 3);
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
                {
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeText.color = Color.yellow;
                }
                storeTemporaryEffect.Add(6);
            }
            if (usingOvermapDivination == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //do event card peek
                ShowThreeEvents();
            }
        }
        //could reset this flag variable instantly, since only divination card uses this atm
        usingOvermapDivination = false;

        //lets try use boomsticks effect number for its str +2 bonus
        if (effect == 7)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(1, 2);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthText.color = Color.yellow;
            }

            //stores the effect to the list
            storeTemporaryEffect.Add(7);
        }

        //this is for temporarily adding lore to str
        if (effect == 8)
        {
            //store custom bonus
            combatMagicBoost.Add(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(1, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore);
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthText.color = Color.yellow;
            }
            storeTemporaryEffect.Add(8);
        }

        //this is for sentinel schematic
        if (effect == 10)
        {
            string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " created a sentinel.";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -10);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(1, 2);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 2);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
        }

        //time warp
        if (effect == 11)
        {
            string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " warped time.";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            intelligenceUse.SetActive(false);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect = 1;
            // maybe add special icon for this somewhere
        }

        //pious donation
        if (effect == 12)
        {
            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                donationDialog.SetActive(true);
            }
            donationDialog.GetComponent<CoinDialog>().dialogText.text = "Choose the amount of coins to donate:";
        }

        //nourishing meal
        if (effect == 13)
        {
            string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " is well fed.";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
        }

        //regret
        if (effect == 14)
        {
            temporaryNumber = 14;

            string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " has deep regrets.";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                GameManager.ins.uiButtonHandler.DisableAllButtons();
                GameManager.ins.uiButtonHandler.CloseAllDisplays();
                GameManager.ins.uiButtonHandler.handCardDisplay.SetActive(true);

                handCardDiscard.SetActive(true);

                //update text
                handDiscardText.text = "You can discard 1 card:";
            }

            //special aftereffect
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 4;
        }

        //sabotage
        if (effect == 15)
        {
            //temporaryNumber = 14;
            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //open the text dialog
                bottomTextCanvas.SetActive(true);
                bottomText.text = "Choose a location as target for the sabotage:";

                //opens the hero colliders on your location, or the location of your agents
                //sends number in deck value of the first card stored
                gameObject.GetComponent<AttackResolve>().EnableLocationCollidersForAction(2);
            }
        }

        //insider (was motivate)
        if (effect == 16)
        {
            //temporaryNumber = 14;
            intelligenceUse.SetActive(false);

            string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " sent secret agent.";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
            }
        }


        //pathways (not attack card atm, was displacement)
        if (effect == 18)
        {
            string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " is using pathways.";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //dont need this anymore
                //StartDisplacement();
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().hasPathways = true;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
            }
        }

        //marching order
        if (effect == 19)
        {
            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //cant march, if youre alrdy marching
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isMarching > 0)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(9);
                    return;
                }
                //MarchingOrder();
            }
        }

        //ward of letho
        if (effect == 20)
        {
            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if player is alrdy warded
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isWarded > 0)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(9);
                    return;
                }
                //WardOfLetho();
            }
        }

        //bookkeeping
        if (effect == 21)
        {
            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //cant summon agent, if theres alrdy 2 on your location
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isBookkeeping > 0)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(9);
                    return;
                }
                //Bookkeeping();
            }
        }

        //wish
        if (effect == 22)
        {
            intelligenceUse.SetActive(false);

            //opens new dialog if its your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                SelectWish();
            }
        }


        //beatdown
        if (effect == 31)
        {
            intelligenceUse.SetActive(false);

            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if theres valid targets for attack card
                //lets do this check here, instead of the intial method, should work hopefully
                if (gameObject.GetComponent<AttackResolve>().CheckForValidTargets() == false)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(5);
                    return;
                }

                //opens new dialog if its your turn
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //open the text dialog
                    bottomTextCanvas.SetActive(true);
                    bottomText.text = "Choose an opponent hero as target for the attack card:";

                    //opens the hero colliders on your location, or the location of your agents
                    //sends number in deck value of the first card stored
                    //gameObject.GetComponent<AttackResolve>().EnableHeroCollidersForAttack();
                }
            }
        }

        //scold
        if (effect == 32)
        {
            intelligenceUse.SetActive(false);

            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if theres valid targets for attack card
                //lets do this check here, instead of the intial method, should work hopefully
                if (gameObject.GetComponent<AttackResolve>().CheckForValidTargets() == false)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(5);
                    return;
                }

                //opens new dialog if its your turn
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //open the text dialog
                    bottomTextCanvas.SetActive(true);
                    bottomText.text = "Choose an opponent hero as target for the attack card:";

                    //opens the hero colliders on your location, or the location of your agents
                    //sends number in deck value of the first card stored
                    //gameObject.GetComponent<AttackResolve>().EnableHeroCollidersForAttack();
                }
            }
        }



        //minefield
        //note that this is an unique type of card (placed on board), and not really even an attack card anymore
        if (effect == 34)
        {
            intelligenceUse.SetActive(false);

            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //opens new dialog if its your turn, although this check is alrdy done
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //message
                    string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " created a minefield.";
                    GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

                    //closes colliders and resets special actions
                    //GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

                    //close tooltip
                    //toolTipBackground.SetActive(false);

                    //get this locations ID
                    int placement = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.gameObject.GetPhotonView().ViewID;

                    //get card number of the intelligence card used
                    int intCardNumber = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().temporaryNumber2;
                    //int eventCardNumber = GameManager.ins.intelligenceDeck[intCardNumber].GetComponent<EventCard>().numberInEventDeck;

                    //Debug.Log("placement, intcardnumber are: " + placement + " " + intCardNumber);

                    //spawns the encounter for all to see
                    //GameManager.ins.eventCanvas.GetComponent<EventHandler>().PV.RPC("RPC_SpawnMinefield", RpcTarget.AllBufferedViaServer, intCardNumber, placement);

                    //re-enables straight actions
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
                }
            }
        }

        //quickfingers 
        //as you can only use defense cards against this, the effect number starts at 41
        //put other attack cards before this (just for clarity sake)
        if (effect == 41)
        {
            intelligenceUse.SetActive(false);
            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if theres valid targets for attack card
                //lets do this check here, instead of the intial method, should work hopefully
                if (gameObject.GetComponent<AttackResolve>().CheckForValidTargets() == false)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(5);
                    return;
                }
                //check if its nighttime
                if (Clock.clock.isNight == false && Clock.clock.ulrimanInPlay == false)
                {
                    //returns card here, and message about wrong timing, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                      //  GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(6);
                    return;
                }

                //opens new dialog if its your turn, although this check is alrdy done
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //open the text dialog
                    bottomTextCanvas.SetActive(true);
                    bottomText.text = "Choose an opponent hero as target for the attack card:";

                    //opens the hero colliders on your location, or the location of your agents
                    //sends number in deck value of the first card stored
                    //gameObject.GetComponent<AttackResolve>().EnableHeroCollidersForAttack();
                }
            }
        }

        //claim quest
        if (effect == 42)
        {
            intelligenceUse.SetActive(false);
            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {

                //opens new dialog if its your turn, although this check is alrdy done
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //cant remember what this does anymore, but its needed :-)
                    temporaryNumber = 42;

                    //open the prompt
                    claimQuestPrompt.SetActive(true);

                    //what type of card to return
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 7;

                    //indicates youre doing a claim quest action
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 12;

                    GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().EnableQuestPlates();

                    //show greenarrows:
                    GameObject.Find("Quest Arrows").GetComponent<QuestArrows>().EnableArrowsForClaimQuest();
                }
            }
        }

        //memory steal
        if (effect == 43)
        {
            intelligenceUse.SetActive(false);
            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if theres valid targets for attack card
                //lets do this check here, instead of the intial method, should work hopefully
                if (gameObject.GetComponent<AttackResolve>().CheckForValidTargets() == false)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(5);
                    return;
                }

                //opens new dialog if its your turn, although this check is alrdy done
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //open the text dialog
                    bottomTextCanvas.SetActive(true);
                    bottomText.text = "Choose an opponent hero as target for the attack card:";

                    //opens the hero colliders on your location, or the location of your agents
                    //sends number in deck value of the first card stored
                    //gameObject.GetComponent<AttackResolve>().EnableHeroCollidersForAttack();
                }
            }
        }

        //scroll of acquisition
        if (effect == 44)
        {
            intelligenceUse.SetActive(false);
            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if theres valid targets for attack card
                //lets do this check here, instead of the intial method, should work hopefully
                if (gameObject.GetComponent<AttackResolve>().CheckForValidTargets() == false)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(5);
                    return;
                }

                //opens new dialog if its your turn, although this check is alrdy done
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //open the text dialog
                    bottomTextCanvas.SetActive(true);
                    bottomText.text = "Choose an opponent hero as target for the attack card:";

                    //opens the hero colliders on your location, or the location of your agents
                    //sends number in deck value of the first card stored
                    //gameObject.GetComponent<AttackResolve>().EnableHeroCollidersForAttack();
                }
            }
        }

        //this cards effect nr. was swapped at v82
        //convert
        if (effect == 45)
        {
            intelligenceUse.SetActive(false);

            //makes sure it your turn
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //check if theres valid targets for attack card
                //lets do this check here, instead of the intial method, should work hopefully
                if (gameObject.GetComponent<AttackResolve>().CheckForValidTargets() == false)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(5);
                    return;
                }

                //check if player has 6 coins
                if (GameManager.ins.intelligenceDeck[temporaryNumber2].GetComponent<IntelligenceCard>().effect == 45 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < 7)
                {
                    //returns card here, and message about no valid targets, flag is false so you cant gain duplicate cards
                    if (inspirationUsed == false)
                    {
                       // GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
                    }
                    ActionCancelled(3);
                    return;
                }

                //opens new dialog if its your turn, although this check is alrdy done
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
                {
                    //open the text dialog
                    bottomTextCanvas.SetActive(true);
                    bottomText.text = "Choose an opponent hero as target for the attack card:";

                    //opens the hero colliders on your location, or the location of your agents
                    //sends number in deck value of the first card stored
                    //gameObject.GetComponent<AttackResolve>().EnableHeroCollidersForAttack();
                }
            }
        }

        if (effect == 61)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().ItsYourTurn())
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().isBlessed = true;
            }

            storeTemporaryEffect.Add(61);
        }

        GameManager.ins.uiButtonHandler.CloseAllDisplays();

        //in case inspiration was used, make sure not to change cardfunction before this
        if (inspirationUsed == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction == 5)
        {
            //note that this is only used to "reset" the text field (dunno if needed)
            intelligenceUseText.text = "Choose Intelligence card to use >";

            intelligenceUse.SetActive(false);
            questCanvas.SetActive(true);
            inspirationUsed = false;
            SetInspiration(false);
            Debug.Log("inspiration used = false");
            return;
        }

        if (inspirationUsed == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction == 6)
        {
            intelligenceUseText.text = "Choose Intelligence card to use >";
            intelligenceUse.SetActive(false);
            inspirationUsed = false;
            SetInspiration(false);
            Debug.Log("inspiration used = false");
            return;
        }

        //special case for regret
        if (inspirationUsed == true && temporaryNumber == 14)
        {
            intelligenceUseText.text = "Choose Intelligence card to use >";
            intelligenceUse.SetActive(false);
            inspirationUsed = false;
            SetInspiration(false);
            Debug.Log("inspiration used = false");
            return;
        }

        //special case for claim quest
        if (inspirationUsed == true && temporaryNumber == 42)
        {
            //intelligenceUseText.text = "Choose Intelligence card to use >";
            intelligenceUse.SetActive(false);
            inspirationUsed = false;
            SetInspiration(false);
            Debug.Log("inspiration used = false");
            return;
        }

        //ermm lets try this here
        //inspirationUsed = false;
        //SetInspiration(false);

        intelligenceUseText.text = "Choose Intelligence card to use >";
        intelligenceUse.SetActive(false);
        inspirationUsed = false;
        SetInspiration(false);
        Debug.Log("inspiration used = false");
    }
    */

    /*old
    public void RemoveIntelligenceEffects()
    {
        if (storeTemporaryEffect == null)
        {
            return;
        }

        //while (storeTemporaryEffect != null)
        //could as well make this go from beginning to end, but meh
        //for (int i = storeTemporaryEffect.Count -1; i > -1; i--)
        for (int i = 0; i < storeTemporaryEffect.Count; i++)
        {
            //this removes the temporary +3 strength
            if (storeTemporaryEffect[i] == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(1, -3);
                
            }
            //this removes the temporary +3 influence
            if (storeTemporaryEffect[i] == 2)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(2, -3);

            }
            //this removes the temporary +3 mechanics
            if (storeTemporaryEffect[i] == 3)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(3, -3);
            }

            //this removes the temporary +3 digging
            if (storeTemporaryEffect[i] == 4)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(4, -3);
            }
            //this removes the temporary +3 lore
            if (storeTemporaryEffect[i] == 5)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(5, -3);
            }
            //this removes the temporary +3 discovery
            if (storeTemporaryEffect[i] == 6)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(6, -3);
            }
            //this removes the temporary +2 strength
            //same effect number as boomsticks itself though!
            if (storeTemporaryEffect[i] == 7)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(1, -2);
            }

            //this removes the temporary lore to str
            if (storeTemporaryEffect[i] == 8)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateStats(1, -combatMagicBoost[0]);
                combatMagicBoost.RemoveAt(0);
            }

            //this removes the blessing
            if (storeTemporaryEffect[i] == 61)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInParent<CharController>().isBlessed = false;
            }
        }
        //clear the list
        storeTemporaryEffect.Clear();
    }
    */

    /*old
    //dont reclaim button pressed
    public void DontReclaimIntelligence()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //closes tooltip
        if (GameObject.Find("ToolTipBackground").gameObject.activeSelf)
        {
            GameObject.Find("ToolTipBackground").SetActive(false);
        }

        reclaimDialog.SetActive(false);

        //in case of summons
        if (GameManager.ins.intelligenceDeck[temporaryNumber2].GetComponent<IntelligenceCard>().effect == 17)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().Cancel();
            EmptyStorage();
            temporaryNumber2 = 0;
        }
        else
        {
            questCanvas.SetActive(true);
        }
    }

    //reclaim intelligence card button pressed
    public void ReclaimIntelligence()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //closes tooltip
        if (GameObject.Find("ToolTipBackground").gameObject.activeSelf)
        {
            GameObject.Find("ToolTipBackground").SetActive(false);
        }

        //should do a check here for deficit.. or put to sleep if energy goes below 0
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(0, -1);

        storeNumberInDeck.Add(temporaryNumber);
        reclaimDialog.SetActive(false);

        //in case of summons
        if (GameManager.ins.intelligenceDeck[temporaryNumber2].GetComponent<IntelligenceCard>().effect == 17)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(temporaryNumber2);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().Cancel();
            EmptyStorage();
            temporaryNumber2 = 0;
        }
        else
        {
            questCanvas.SetActive(true);
        }
    }
    */

    public void SpawnCard()
    {
        if (storeNumberInDeck == null)
        {
            return;
        }

        //for (int i = storeNumberInDeck.Count -1; i > -1; i--)
        for (int i = 0; i < storeNumberInDeck.Count; i++)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().ReclaimIntelligence(storeNumberInDeck[i]);
            //not needed?
            //storeNumberInDeck.RemoveAt(i);
        }

        storeNumberInDeck.Clear();
    }

    //empty storage, not used atm
    public void EmptyStorage()
    {
        storeNumberInDeck.Clear();
    }

    /*for activating agent placement
    public void SetAgent()
    {
        //open the text dialog
        bottomTextCanvas.SetActive(true);
        bottomText.text = "Choose the location for your agent. \n (2 agents max per location)";

        //enable all location colliders, unles theres alrdy 2 agents there
        if (GameObject.Find("Red Pyramid").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Red Pyramid").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Oldmines").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Oldmines").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Grimhold").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Grimhold").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Coven of Karnau").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Coven of Karnau").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Valley of Wonders").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Valley of Wonders").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Grand Bazaar").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Grand Bazaar").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Cornville").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Cornville").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Bronzium Bank").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Bronzium Bank").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Pale Spire").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Pale Spire").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Eledion").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Eledion").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Hall of Champions").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Hall of Champions").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Wilforge").GetComponent<Node>().numberOfAgents < 2)
            GameObject.Find("Wilforge").GetComponent<Node>().col.enabled = true;

        //set the target flag
        GameObject.Find("Red Pyramid").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Oldmines").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Grimhold").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Cornville").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Pale Spire").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Eledion").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Hall of Champions").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().specialAction = 1;
        GameObject.Find("Wilforge").GetComponent<Node>().specialAction = 1;
    }
    */

    /*when activating insider int card
    public void Insider()
    {
        //open the text dialog
        bottomTextCanvas.SetActive(true);
        bottomText.text = "Choose the location to interact with. \n (You cannot interact with disabled locations)";

        //enable all location colliders, unles theres alrdy 2 agents there
        if (GameObject.Find("Red Pyramid").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Red Pyramid").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Oldmines").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Oldmines").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Grimhold").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Grimhold").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Coven of Karnau").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Coven of Karnau").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Valley of Wonders").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Valley of Wonders").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Grand Bazaar").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Grand Bazaar").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Cornville").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Cornville").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Bronzium Bank").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Bronzium Bank").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Pale Spire").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Pale Spire").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Eledion").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Eledion").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Hall of Champions").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Hall of Champions").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().col.enabled = true;

        if (GameObject.Find("Wilforge").GetComponent<Node>().interactCost < 4)
            GameObject.Find("Wilforge").GetComponent<Node>().col.enabled = true;

        //set the target flag
        GameObject.Find("Red Pyramid").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Oldmines").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Grimhold").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Cornville").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Pale Spire").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Eledion").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Hall of Champions").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().specialAction = 3;
        GameObject.Find("Wilforge").GetComponent<Node>().specialAction = 3;
    }
    */
    //when activating portals quest
    public void Portals()
    {
        //open the text dialog
        //bottomTextCanvas.SetActive(true);
        //bottomText.text = "Choose the location for your agent. \n (2 agents max per location)";

        //enable all location colliders
        GameObject.Find("Red Pyramid").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Oldmines").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Grimhold").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Cornville").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Pale Spire").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Eledion").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Hall of Champions").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Wilforge").GetComponent<Node>().col.enabled = true;

        //set the target flag
        GameObject.Find("Red Pyramid").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Oldmines").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Grimhold").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Cornville").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Pale Spire").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Eledion").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Hall of Champions").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().specialAction = 5;
        GameObject.Find("Wilforge").GetComponent<Node>().specialAction = 5;
    }

    //for activating displacement
    public void StartDisplacement()
    {
        //open the text dialog
        bottomTextCanvas.SetActive(true);
        bottomText.text = "Choose the location to transport yourself to:";

        //enable all location colliders
        GameObject.Find("Red Pyramid").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Oldmines").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Grimhold").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Cornville").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Pale Spire").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Eledion").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Hall of Champions").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().col.enabled = true;
        GameObject.Find("Wilforge").GetComponent<Node>().col.enabled = true;

        //set the target flag
        GameObject.Find("Red Pyramid").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Oldmines").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Grimhold").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Cornville").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Pale Spire").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Eledion").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Hall of Champions").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().specialAction = 4;
        GameObject.Find("Wilforge").GetComponent<Node>().specialAction = 4;
    }

    //for resetting nodes
    public void ResetNodes()
    {
        //close the text dialog
        bottomTextCanvas.SetActive(false);

        //enable all location colliders
        GameObject.Find("Red Pyramid").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Oldmines").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Grimhold").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Cornville").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Pale Spire").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Eledion").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Hall of Champions").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().col.enabled = false;
        GameObject.Find("Wilforge").GetComponent<Node>().col.enabled = false;

        //set the target flag
        GameObject.Find("Red Pyramid").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Oldmines").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Grimhold").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Coven of Karnau").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Valley of Wonders").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Grand Bazaar").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Cornville").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Bronzium Bank").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Pale Spire").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Eledion").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Hall of Champions").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Gorms Hammer Inn").GetComponent<Node>().specialAction = 0;
        GameObject.Find("Wilforge").GetComponent<Node>().specialAction = 0;
    }

    /*old int effects
    //for lethos ward card effect
    public void MarchingOrder()
    {
        PV.RPC("RPC_MarchingOrder", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_MarchingOrder()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().movementBonus += 1;

        //takes ViewID of the node the avatar is moving to
        int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

        //adds 2 "movementBonus", to add 1 movementBonus
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().movementBonus += 2;

        //set this flag variable, just in case
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove = true;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[temporaryNumber2], new Vector3(0, 0, 0), Quaternion.identity);

        //sets the flag variable
        GameManager.ins.intelligenceDeck[temporaryNumber2].GetComponent<CardDisplay>().isTaken = true;

        //places it in hand card area
        playerCard.transform.SetParent(GameManager.ins.artifactCardArea.transform, false);

        //set the "owner" variable to the card
        playerCard.GetComponent<CardDisplay>().belongsTo = GameManager.ins.turnNumber;

        //turns the card inactive
        playerCard.SetActive(false);

        //unless its your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            playerCard.SetActive(true);
        }

        // set the card timer
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isMarching = 6;
    }

    //for lethos ward card effect
    public void WardOfLetho()
    {
        PV.RPC("RPC_WardOfLetho", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_WardOfLetho()
    {
        //takes ViewID of the node the avatar is moving to
        int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

        //adds 2 "movementBonus", to add 1 movementBonus
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().movementBonus += 1;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[temporaryNumber2], new Vector3(0, 0, 0), Quaternion.identity);

        //sets the flag variable
        GameManager.ins.intelligenceDeck[temporaryNumber2].GetComponent<CardDisplay>().isTaken = true;

        //places it in hand card area
        playerCard.transform.SetParent(GameManager.ins.artifactCardArea.transform, false);

        //set the "owner" variable to the card
        playerCard.GetComponent<CardDisplay>().belongsTo = GameManager.ins.turnNumber;

        //turns the card inactive
        playerCard.SetActive(false);

        //unless its your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            playerCard.SetActive(true);
        }

        //set the card timer
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isWarded = 24;
    }

    //for lethos ward card effect
    public void Bookkeeping()
    {
        PV.RPC("RPC_Bookkeeping", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_Bookkeeping()
    {
        //takes ViewID of the node the avatar is moving to
        int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

        //adds 1 "movementBonus"
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().movementBonus += 1;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

        //instantiates card
        GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[temporaryNumber2], new Vector3(0, 0, 0), Quaternion.identity);

        //sets the flag variable
        GameManager.ins.intelligenceDeck[temporaryNumber2].GetComponent<CardDisplay>().isTaken = true;

        //places it in hand card area
        playerCard.transform.SetParent(GameManager.ins.artifactCardArea.transform, false);

        //set the "owner" variable to the card
        playerCard.GetComponent<CardDisplay>().belongsTo = GameManager.ins.turnNumber;

        //turns the card inactive
        playerCard.SetActive(false);

        //unless its your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            playerCard.SetActive(true);
        }

        //set the card timer
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isBookkeeping = 24;
    }
    */

    //lets add the options functionality here, it makes some sense
    public void OnOptionsButtonClicked()
    {
        optionsPanel.SetActive(true);
        optionsPanelActivated = true;
        optionsDialog.SetActive(true);
        confirmationDialog.SetActive(false);

        //checks the sfx level player prefs
        if (PlayerPrefs.HasKey("SfxLevel"))
        {
            GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxLevel = PlayerPrefs.GetFloat("SfxLevel");
            GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxVol = PlayerPrefs.GetFloat("SfxVol");
            GameObject.Find("SFX Volume Slider").GetComponent<Slider>().value = GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxLevel;
        }
        else
        {
            GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxLevel = 0.5f;
            GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxVol = -6.0f;
            PlayerPrefs.SetFloat("SfxLevel", GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxLevel);
            PlayerPrefs.SetFloat("SfxVol", GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxVol);
            GameObject.Find("SFX Volume Slider").GetComponent<Slider>().value = GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxLevel;
        }
        //checks the music level player prefs
        if (PlayerPrefs.HasKey("MusicLevel"))
        {
            GameObject.Find("SFX Player").GetComponent<SoundManager>().musicLevel = PlayerPrefs.GetFloat("MusicLevel");
            GameObject.Find("SFX Player").GetComponent<SoundManager>().musicVol = PlayerPrefs.GetFloat("MusicVol");
            GameObject.Find("Music Volume Slider").GetComponent<Slider>().value = GameObject.Find("SFX Player").GetComponent<SoundManager>().musicLevel;
        }
        else
        {
            GameObject.Find("SFX Player").GetComponent<SoundManager>().musicLevel = 0.5f;
            GameObject.Find("SFX Player").GetComponent<SoundManager>().musicVol = -6.0f;
            PlayerPrefs.SetFloat("MusicLevel", GameObject.Find("SFX Player").GetComponent<SoundManager>().musicLevel);
            PlayerPrefs.SetFloat("MusicVol", GameObject.Find("SFX Player").GetComponent<SoundManager>().musicVol);
            GameObject.Find("Music Volume Slider").GetComponent<Slider>().value = GameObject.Find("SFX Player").GetComponent<SoundManager>().musicLevel;
        }
    }

    public void OnBackButtonClicked()
    {
        optionsPanel.SetActive(false);
        optionsDialog.SetActive(true);
        confirmationDialog.SetActive(false);
        optionsPanelActivated = false;
    }

    public void OpenConfirmationDialog()
    {
        optionsDialog.SetActive(false);
        confirmationDialog.SetActive(true);
    }

    public void Surrender()
    {
        optionsPanel.SetActive(false);
        optionsDialog.SetActive(true);
        confirmationDialog.SetActive(false);
        optionsPanelActivated = false;

        //lets put this here, so gamemanager knows the hero is defeated
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;

        OpenDefeatWindow();

        GameManager.ins.uiButtonHandler.GetComponent<UiButtonHandler>().CloseAllDisplays();

        GameManager.ins.uiButtonHandler.GetComponent<UiButtonHandler>().DisableAllButtons();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

        //stop musics
        GameManager.ins.references.soundManager.mainMusicHolder.Stop();
        GameManager.ins.references.soundManager.encounterMusicHolder.Stop();

        //starts defeat music
        //GameManager.ins.sfxPlayer.GetComponent<SoundManager>().PlayDefeatMusic();
        GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(4);
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    //for leave game button at scene 1
    //same as in selectorscript2?
    public void OnReturnToMainMenu()
    {
        DataPersistenceManager.instance.SaveGame();

        Invoke(nameof(CloseGameWithDelay), 0.25f);
    }

    void CloseGameWithDelay()
    {
        GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().PlayTitleMusicWithDelay();
        PhotonRoom.room.CloseGame();
    }

    public void ShowThreeEvents()
    {
        //opens the dialog 
        GameManager.ins.artifactOptionsCanvas.gameObject.SetActive(true);

        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(11, 3);

        //this card function should be unused? so you shouldnt be able to target anything
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 1;

        GameManager.ins.artifactOptionsCanvas.GetComponent<Action>().cardOptionType = 11;

        GameManager.ins.artifactOptionsText.text = "Incoming Events:";

        GameManager.ins.artifactOptionsCanvas.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<Text>().text = "OK";

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();
    }

    public void SelectWish()
    {
        //opens the dialog 
        GameManager.ins.cardOptionsCanvas.gameObject.SetActive(true);

        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(12, 4);

        //make new cardfunction for this purpose
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 16;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 10;

        //GameManager.ins.cardOptionsCanvas.GetComponent<Action>().cardOptionType = 11;

        GameManager.ins.cardOptionsText.text = "Make a Wish:";

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();
    }

    //called from the followerdialog (lets just keep it here, since its so similar to selectwish)
    public void SelectPayday()
    {
        //opens the dialog 
        GameManager.ins.cardOptionsCanvas.gameObject.SetActive(true);

        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(13, 3);

        //draw 3 specific cards to offer
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, 19, 12);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, 20, 12);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_DrawCardOptions", RpcTarget.AllBufferedViaServer, 21, 12);

        //make new cardfunction for this purpose
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 17;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 10;

        //GameManager.ins.cardOptionsCanvas.GetComponent<Action>().cardOptionType = 11;

        GameManager.ins.cardOptionsText.text = "Select Payday card to activate:";

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();
    }

    public void TutorialPrologueOkPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //disable chat panel
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.SetActive(false);

        TutorialStartPrompt.SetActive(false);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(false);
        //tutorialHandler.GetComponent<TutorialHandler>().background.SetActive(true);

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();

        tutorialHandler.GetComponent<TutorialHandler>().ActivateTutorialPlates();
    }

    public void TutorialEpilogueOkPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        PhotonRoom.room.CloseGame();
    }

    public void PrologueOkPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //PV.RPC("RPC_ClosePrologueWindow", RpcTarget.AllBufferedViaServer);
        ClosePrologueWindow();

        //could add starting upgrade card draw here for now
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();
    }

    //[PunRPC]
    void ClosePrologueWindow()
    {
        //ProloguePrompt.SetActive(false);
        StoryPrompt.SetActive(false);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(false);

        //clear location text
        //GameManager.ins.references.ClearLocationText();

        //lets try this fix for v94 (otherwise citadel start seems not to work)
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;

        if (PhotonRoom.room.spContinue == false)
        {
            //easy / tutorial
            if (PhotonRoom.room.startDifficulty == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints += 6;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber);
            }

            //normal
            if (PhotonRoom.room.startDifficulty == 2)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints += 6;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber);
            }
            //advanced
            if (PhotonRoom.room.startDifficulty == 3)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints += 6;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber);
            }
            //lets give additional expert penalty here (might not work at GameManager)
            //actually all difficulties have same action points in v0.7.0. so..
            if (PhotonRoom.room.startDifficulty == 4)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints += 6;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints;
                //removed this in v0.7.1.
                //ExpertStatPenalty();
            }
        }

        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_StartTurn", RpcTarget.AllBufferedViaServer);

        //need to do something like this, so the load "teleport" is called after the start function
        //Invoke("MinimapStart", 1f);

    }

    //start teleport, unused atm
    public void MinimapStart()
    {
        //load starting location, if theres 
        if (DataPersistenceManager.instance.gameData.sceneToGoTo != 999)
        {
            //only use this once
            GameManager.ins.references.useSpecificNodeForSpawn = DataPersistenceManager.instance.gameData.nodeToGoTo;

            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(DataPersistenceManager.instance.gameData.sceneToGoTo, DataPersistenceManager.instance.gameData.nodeToGoTo, true);
        }
    }

    public void ExpertStatPenalty()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxStrength -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxArcanePower -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe -= 1;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve -= 1;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
    }

    //for initial resource boosts
    //not used atm
    public void GiveStartingPassiveEffects()
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                Card card = GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>();

                CardHandler.ins.InstantPassiveEffect(card.belongsTo, card.numberInDeck);
            }
        }
    }

    //when operating nemonox
    public void InitiateTrueVictory()
    {
        StoryPrompt.SetActive(true);
        storyPanel.SetActive(true);
        darkStoryPanel.SetActive(true);

        EnableStoryButtons(5);
        EpilogueNextButton.interactable = false;
        /*
        defeatPanel.SetActive(false);
        EpilogueNextButton.gameObject.SetActive(true);
        DefeatNext.gameObject.SetActive(false);
        RetryButton.gameObject.SetActive(false);
        EpilogueNextButton.interactable = false;
        */

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess1();

        GenerateTrueVictoryTexts();
        GenerateTrueVictoryImages();

        /*
        epilogueTextLoss.gameObject.SetActive(false);
        epilogueTextWin.gameObject.SetActive(true);
        eventText.gameObject.SetActive(false);
        */

        eventText.gameObject.transform.position = eventTextStartPosition;
        eventText.gameObject.SetActive(true);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            Invoke(nameof(EnableEpilogueNext), 5.0f);
        }
    }

    public void GenerateTrueVictoryTexts()
    {
        eventText.text = "You approach the Nemonox with reverence. It is an artifact originating before time itself, with powers greater than even the Founders can command.<br><br>" +
            "You feel connection to the device you cannot fully explain, as if some invisible transaction is taking place. Slowly the sounds from machinery within start to change, and premonitions start to emerge in your minds eye.<br><br>" +
            "You can but sit and watch..<br><br><br><br><br>";

        //claudias disc
        if (GameManager.ins.references.discsReturned[0] == true)
        {
            eventText.text += "You see Wilforge palace transformed into parliamentary building, where elected representatives of the people vote for the matters of the realm.<br><br>" +
                "The elected monarcy is gone, and the power is with the people. But it will take time and trials for them to realize what to do with it..<br><br><br><br><br>";
        }

        //dwellers disc
        if (GameManager.ins.references.discsReturned[1] == true)
        {
            eventText.text += "You see grulls and arduuns mixing with the surface people. Doing trade and mingling with humans and brevir as equals.<br><br>" +
                "The old enmities are gone, the Dweller has calmed down, and so has his children. People of different races work together for the common good, but not without their occasional disagreements..<br><br><br><br><br>";
        }

        //irinos disc
        if (GameManager.ins.references.discsReturned[2] == true)
        {
            eventText.text += "You see technological revolution take place. Great machines transport people to any corner of the isle within minutes. Various helpful appliances are made available for everyone, for an affordable price.<br><br>" +
                "Irino and Esmeralda are overseeing this all together with their many children, feeling satisfied their lifes work has come to fruition.<br><br><br><br><br>";
        }

        //lavinias disc
        if (GameManager.ins.references.discsReturned[3] == true)
        {
            eventText.text += "You see the great pathway to Lexus open. Every 10 years, travel between the realms is again possible, after many a millenia.<br><br>" +
                "The witches of Karnau sees for its proper conservation and administration, but only grant the honor of planeswalking to the most accomplished heroes and diplomats.<br><br><br><br><br>";
        }

        //macks disc
        if (GameManager.ins.references.discsReturned[4] == true)
        {
            eventText.text += "You see great fleets of ships travelling between the mainland and the former wandering isle, which has become stationary and renamed as Emerald Isle.<br><br>" +
                "Old bonds are renewed, trade of ideas and goods flourish, and people of both lands are richer for it.<br><br><br><br><br>";
        }

        //nabamax' disc
        if (GameManager.ins.references.discsReturned[5] == true)
        {
            eventText.text += "You see new star light up in the sky, almost as bright as the night star. Nabamax has been granted his wish, and has ascended to heavens as a new Founder.<br><br>" +
                "Being the new Founder of Arts and Culture, these disciplines gain more momentum among the people, and peoples lives are enriched by it.<br><br><br><br><br>";
        }

        //zaarins disc
        if (GameManager.ins.references.discsReturned[6] == true)
        {
            eventText.text += "You see the spirits of the dead leaving their coils, never to return to Nemia, to a place unknown to mortal men, or even the Founders themselves.<br><br>" +
                "The curse of undeath is broken, and Zaarin has been granted his final wish.<br><br><br><br><br>";
        }

        //zaarins final message
        eventText.text += "Finally the shrouded man reveals himself. He is Zaarin the First Guardian.<br><br><color=#e0cc9f>\"You have done the impossible, and did what I could not. I was bound to my duty even beyond death, due to allowing this evil to prevail. Now i can finally rest." +
            "<br><br>This world is but one of many, but it is worth saving. Which I'm sure you already know. See you on the other side!\"</color><br><br>The First Guardian then departs with a mysterious smile on his face.<br><br><br><br><br>";

        //third guardian
        eventText.text += "After some time, the visions pass, and you meditate in silence. Your task is not yet done, but all will be well, as long as the Nemonox isnt tampered with further.<br><br>" +
                "You find it is your desire to see the best wishes of the ones who came before to come to fruition, as the Third Guardian!<br><br><br>";
    }

    public void GenerateTrueVictoryImages()
    {
        storyPanel.GetComponent<Image>().sprite = GameManager.ins.references.epilogueSprites[0];

        //float nextImageIn = 10f;

        Invoke(nameof(ProceedToNextImage), 16.5f);

    }

    void ProceedToNextImage()
    {
        finalImagesNumber += 1;

        if (GameManager.ins.references.discsReturned[0] == true && finalImagesNumber == 1)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (GameManager.ins.references.discsReturned[1] == true && finalImagesNumber == 2)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (GameManager.ins.references.discsReturned[2] == true && finalImagesNumber == 3)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (GameManager.ins.references.discsReturned[3] == true && finalImagesNumber == 4)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (GameManager.ins.references.discsReturned[4] == true && finalImagesNumber == 5)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (GameManager.ins.references.discsReturned[5] == true && finalImagesNumber == 6)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        if (GameManager.ins.references.discsReturned[6] == true && finalImagesNumber == 7)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }

        //zaarins final message
        if (finalImagesNumber == 8)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }

        if (finalImagesNumber == 9 || finalImagesNumber == 10)
        {
            StartCoroutine(FadeEpilogueImageOut(true));
            Invoke(nameof(SwapNextImage), 4f);
            return;
        }
        //this shouldnt be called tho?
        if (finalImagesNumber > 10)
        {
            return;
        }
        else
        {
            ProceedToNextImage();
            return;
        }
    }

    void SwapNextImage()
    {
        if (finalImagesNumber > 8)
        {
            //female image
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber == 2 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber == 4 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber == 6 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber == 7)
            {
                storyPanel.GetComponent<Image>().sprite = GameManager.ins.references.epilogueSprites[9];
            }
            //male image
            else
            {
                storyPanel.GetComponent<Image>().sprite = GameManager.ins.references.epilogueSprites[10];
            }

            StartCoroutine(FadeEpilogueImageIn(true));
            return;
        }

        storyPanel.GetComponent<Image>().sprite = GameManager.ins.references.epilogueSprites[finalImagesNumber];

        StartCoroutine(FadeEpilogueImageIn(true));

        //could have different delays for different images (depending on text lenghts?)
        if (finalImagesNumber == 1)
        {
            Invoke("ProceedToNextImage", 11f);
        }

        if (finalImagesNumber == 2)
        {
            Invoke("ProceedToNextImage", 12f);
        }

        if (finalImagesNumber == 3)
        {
            Invoke("ProceedToNextImage", 12.5f);
        }

        if (finalImagesNumber == 4)
        {
            Invoke("ProceedToNextImage", 11.5f);
        }

        if (finalImagesNumber == 5)
        {
            Invoke("ProceedToNextImage", 11.5f);
        }

        if (finalImagesNumber == 6)
        {
            Invoke("ProceedToNextImage", 11.5f);
        }

        if (finalImagesNumber == 7)
        {
            Invoke("ProceedToNextImage", 10.5f);
        }

        if (finalImagesNumber > 7)
        {
            Invoke("ProceedToNextImage", 19.3f);
        }
    }

    public void OpenEpiloqueWindow()
    {
        StoryPrompt.SetActive(true);
        storyPanel.SetActive(true);
        //defeatPanel.SetActive(false);

        //whats the point of this?
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);

        /*
        EpilogueNextButton.gameObject.SetActive(true);
        DefeatNext.gameObject.SetActive(false);
        RetryButton.gameObject.SetActive(false);
        */
        EnableStoryButtons(4);
        EpilogueNextButton.interactable = false;

        //add this as custom image in v95
        //actually image 6 and 10 are duplicates
        storyImage.sprite = GameManager.ins.references.eventSprites[10];

        //play sfx
        //GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess1();
        GameManager.ins.references.sfxPlayer.PlaySkillCheckFail();

        eventText.gameObject.transform.position = eventTextStartPosition;
        eventText.text = "Suddenly the skies darken, and a strange green portal appears high in the sky.<br><br>" +
            "Toxic mist fills the land, and violent lightning storm ensues. Some strange creatures pour from the portal upon the suffocating land.<br><br>" +
            "You will never know what the purpose of the destruction was, but at least the end was mercifully swift." +
            "<br><br><color=#2cd4f2>Consider upgrading to the full version of the game, to delay the enemys plans for three times longer.</color>";

        eventText.gameObject.SetActive(true);

        /* changed in v95
         * choose different text according to your score
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame >= 1000)
        {
            epilogueTextLoss.gameObject.SetActive(false);
            epilogueTextWin.gameObject.SetActive(true);
        }
        else
        {
            epilogueTextLoss.gameObject.SetActive(true);
            epilogueTextWin.gameObject.SetActive(false);
        }
        */

        //epilogueTextLoss.gameObject.SetActive(true);
        //epilogueTextWin.gameObject.SetActive(false);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            Invoke(nameof(EnableEpilogueNext), 5.0f);
        }
    }

    public void EnableEpilogueNext()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().EpilogueNextButton.interactable = true;
    }

    public void OpenStoryWindow()
    {
        StoryPrompt.SetActive(true);
        storyPanel.SetActive(true);
        //defeatPanel.SetActive(false);

        //whats the point of this?
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);
        /*
        EpilogueNextButton.gameObject.SetActive(false);
        DefeatNext.gameObject.SetActive(false);
        RetryButton.gameObject.SetActive(false);
        EpilogueFinishButton.gameObject.SetActive(false);
        StoryCloseButton.gameObject.SetActive(true);
        StoryCloseButton.interactable = false;
        */
        StoryCloseButton.interactable = false;
        EnableStoryButtons(2);

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess1();

        //restart event text
        eventText.gameObject.transform.position = eventTextStartPosition;

        //epilogueTextLoss.gameObject.SetActive(false);
        //epilogueTextWin.gameObject.SetActive(false);
        eventText.gameObject.SetActive(true);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            Invoke(nameof(EnableStoryClose), 5.0f);
        }
    }

    void EnableStoryClose()
    {
        StoryCloseButton.interactable = true;
    }

    public void CloseStoryPrompt()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        if(Clock.clock.isZaarinsMessage == true)
        {
            Clock.clock.isZaarinsMessage = false;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();
        }
        //this might be useful for other event ends later
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Arrive(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        StoryCloseButton.gameObject.SetActive(false);
        StoryPrompt.SetActive(false);

        //need to do this a more complicated way
        GameManager.ins.references.soundManager.scoreMusic.GetComponent<AudioSource>().Stop();

        //need to compare if the score music is the same as location music in this case
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.clip = GameManager.ins.references.soundManager.scoreMusic.clip;
        
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();
    }

    //for v92
    public void EpilogueNextPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        /*
        EpilogueNextButton.gameObject.SetActive(false);

        EpilogueFinishButton.interactable = false;
        EpilogueFinishButton.gameObject.SetActive(true);

        epilogueTextLoss.gameObject.SetActive(false);
        epilogueTextWin.gameObject.SetActive(false);
        */

        //we want to disable this here?
        eventText.gameObject.SetActive(false);

        EnableStoryButtons(6);
        EpilogueFinishButton.interactable = false;

        //lets do the calculations here? (before the show method)
        int score = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame;

        /*
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame >= 500)
        {
            score += 100;
        }
        */

        //calculate the score properly
        float fameFloat = Mathf.Ceil(score / 100f * GameManager.ins.scoreModifier);
        finalScore = (int)fameFloat;

        ShowEpilogueEndScore();
        epilogueTextScore.gameObject.SetActive(true);

        PlayEndSfx();

        //need to save the possible high score on gamemanager
        //also kinda need the score info for possible metagame tokens later
        DataPersistenceManager.instance.SaveGame();

        Invoke(nameof(EnableEpilogueFinish), 3.0f);
    }

    void PlayEndSfx()
    {
        //play sfx
        Invoke("WinSound1", 0.0f);
        Invoke("WinSound2", 1.0f);
        Invoke("WinSound1", 2.0f);
    }

    void WinSound1()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess1();
    }
    void WinSound2()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess2();
    }

    public void EnableEpilogueFinish()
    {
        EpilogueFinishButton.interactable = true;
    }

    public void ShowEpilogueEndScore()
    {
        epilogueTextScore.text = "<color=white>Score:  </color>" + finalScore + " <sprite=3>";
    }

    //returns to main menu, resets game variables
    //might need flag variable to disallow continue game button somewhere
    public void FinishGameButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        DataPersistenceManager.instance.gameData.ResetInGameData();

        GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().PlayTitleMusicWithDelay();

        //Invoke(nameof(CloseGame), 0.15f);

        PhotonRoom.room.CloseGame();
    }


    //for v92 (no longer in finishhandler)
    public void OpenDefeatWindow()
    {
        StoryPrompt.SetActive(true);
        //storyPanel.SetActive(false);
        //defeatPanel.SetActive(true);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);

        //lets test this (although not optimal to use 2 different text start positions)
        //defeatText.gameObject.transform.position = defeatTextStartPosition;
        eventText.gameObject.transform.position = eventTextStartPosition;

        /*
        EpilogueNextButton.gameObject.SetActive(false);
        DefeatNext.gameObject.SetActive(true);
        DefeatNext.interactable = false;
        RetryButton.gameObject.SetActive(true);
        RetryButton.interactable = false;

        epilogueTextLoss.gameObject.SetActive(false);
        epilogueTextWin.gameObject.SetActive(false);
        defeatText.gameObject.SetActive(true);
        */

        eventText.text = "Your journey has come to a sudden end. The great peril to the realm remains unanswered.";
        storyImage.sprite = GameManager.ins.references.eventSprites[11];

        EnableStoryButtons(3);
        DefeatNext.interactable = false;
        RetryButton.interactable = false;

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            Invoke(nameof(EnableDefeatNext), 5.0f);
        }
    }

    public void EnableDefeatNext()
    {
        DefeatNext.interactable = true;
        RetryButton.interactable = true;
    }

    //for v92
    public void DefeatNextPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        DefeatNext.gameObject.SetActive(false);
        RetryButton.gameObject.SetActive(false);

        EpilogueFinishButton.interactable = false;
        EpilogueFinishButton.gameObject.SetActive(true);

        //defeatText.gameObject.SetActive(false);

        //lets do the calculations here? (before the show method)
        int score = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame;

        //calculate the score properly
        float fameFloat = Mathf.Ceil(score / 100f * GameManager.ins.scoreModifier);
        finalScore = (int)fameFloat;

        ShowEpilogueEndScore();
        epilogueTextScore.gameObject.SetActive(true);

        PlayEndSfx();

        //need to save the possible high score on gamemanager
        //also kinda need the score info for possible metagame tokens later
        DataPersistenceManager.instance.SaveGame();

        Invoke(nameof(EnableEpilogueFinish), 3.0f);
    }

    public void RetryButtonPressed() 
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();
        GameManager.ins.references.sfxPlayer.RPC_PlayContemplate();

        //reset flags
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = false;

        //give all? energy & hp back
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 99);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 99);

        //reduce score modifier
        GameManager.ins.scoreModifier -= 10;
        if(GameManager.ins.scoreModifier < 0)
        {
            GameManager.ins.scoreModifier = 0;
        }

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();

        //hide the defeat panel
        StoryPrompt.SetActive(false);
        //storyPanel.SetActive(false);
        //defeatPanel.SetActive(false);
        //defeatText.gameObject.SetActive(false);

        //need to do these?
        GameManager.ins.references.soundManager.scoreMusic.Stop();
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        //and this too, although this should be made simpler somehow
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<Image>().sprite =
            GameManager.ins.references.heroBackupImage;

        GameManager.ins.toolTipBackground.SetActive(false);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelAndReturn", RpcTarget.AllBufferedViaServer);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
    }

    //not used now
    public void EpilogueOkPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        PV.RPC("RPC_CloseEpilogueWindow", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_CloseEpilogueWindow()
    {
        StoryPrompt.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(false);

        GameManager.ins.finalWindows.GetComponent<FinishHandler>().CalculateScore();
    }

    //converted from eventhandler (which is mostly deprecated in v90)
    public void EventOkButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        PV.RPC("RPC_RemoveEventCanvas", RpcTarget.AllBufferedViaServer);

        eventDisplay.SetActive(false);

        //rather better "start" the players turn here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartAiTurnWithDelay();
        }
    }

    [PunRPC]
    void RPC_RemoveEventCanvas()
    {
        //destroys the shown card
        //Destroy(eventDisplay.transform.GetChild(0).gameObject);

        //remove the day change information display
        Clock.clock.dayChangeInformation.SetActive(false);

        //eventDisplay.SetActive(false);
        //eventDisclaimer.SetActive(false);

        //reset the flag variable
        Clock.clock.eventDisplayActive = false;
    }

    public void EnterCitadelWithDelay()
    {
        //need clear objective holder
        CardHandler.ins.ClearCardHolder(11);

        Invoke("EnterCitadel", 0.15f);
    }

    public void EnterCitadel()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.LeaveNode(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        //draw quest
        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 290, 11, 1);

        //starts score music
        //GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(3);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventText.text = "With the power of the keystones, you dispell the wards created by the guardian.<br><br>You are a step closer in the path the mysterious figure appointed to you in your visions. Now to see your destiny fulfilled..";
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().storyImage.sprite = GameManager.ins.references.eventSprites[2];
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenStoryWindow();
    }

    //first appearance of Zaarin
    public void Zaarin1()
    {
        //repurposed from earlier version
        Clock.clock.isZaarinsMessage = true;

        Invoke("ContinueZaarin1", 0.25f);
    }

    public void ContinueZaarin1()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.LeaveNode(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        //draw portal ability
        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 286, 1, 1);

        //need to draw new quest too actually
        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 302, 11, 1);

        //starts story music
        GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(5);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventText.text = "As you enter the space between spaces, a strange figure appears to you, and speaks:<br><br><color=#e0cc9f>\"Fear not, friend, for I mean you no harm. I appear to you now, as I foresee great evil befall this land, unless a hero appears to change its course.<br><br>I no longer reside in your realm, and cannot take direct action. But you have the potential to save your world from destruction, which is why I have brought you back now.<br><br>" +
            "Forget your previous commitments and become the hero youre ment to be. Take this gift. I will empower you more once youve proven yourself worthy. But beware, time is of the essence.\"</color><br><br>You feel a surge of power inside you, as the power of portals is granted to you. The figure then vanishes, and you find yourself transported back to the surface.";
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().storyImage.sprite = GameManager.ins.references.eventSprites[3];
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenStoryWindow();
    }

    public void Zaarin2()
    {
        //repurposed from earlier version
        Clock.clock.isZaarinsMessage = true;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.LeaveNode(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        Invoke(nameof(ContinueZaarin2), 0.25f);
    }

    public void ContinueZaarin2()
    {
        //remove quest, add new one
        //give lightstone
        //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 293, 11, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 302, 11, 1);
        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 211, 11, 1);
        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 295, 5, 1);

        //give blessed blade ability
        //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 113, 2, 1);

        //starts story music
        GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(5);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventText.text = "Suddenly you feel dizzy, and lose consciousness. The shrouded man appears to you again, and speaks:<br><br><color=#e0cc9f>\"You have done well. Take this stone, it will lend you aid of the Founders in the hour of need.<br><br>Next you must find keystones, which were gifted to powerful beings. These are necessary to breach into the Guardians Citadel." + //The keystones will allow passage to his citadel.<br><br>Quiron must be defeated at any cost, he is close opening rift to a dimension of unspeakable horrors. Make haste!
            "\"</color><br><br>You gain Lightstone Blessing combat option. The figure then vanishes, and you regain consciousness.";
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().storyImage.sprite = GameManager.ins.references.eventSprites[4];
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenStoryWindow();
    }

    public void Zaarin3()
    {
        //repurposed from earlier version
        Clock.clock.isZaarinsMessage = true;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.LeaveNode(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        Invoke(nameof(ContinueZaarin3), 0.25f);
    }

    public void ContinueZaarin3()
    {
        Clock.clock.Zaarin3MessageGiven = true;

        //starts story music
        GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(5);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventText.text = "As you collect the third keystone, the shrouded man appears again:<br><br><color=#e0cc9f>\"You have collected the keys necessary to open the warded citadel doors. " +
            "There Quiron is preparing to make his final move to seal the fate of this world.<br><br>Make haste, and defeat him, before all is lost.\"</color><br><br>The vision then passes, and you can see clearly again.";
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().storyImage.sprite = GameManager.ins.references.eventSprites[5];
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenStoryWindow();
    }

    IEnumerator FadeEpilogueImageIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            storyImage.color = new Color(1, 1, 1, 0);

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime * 0.35f)
            {
                // set color with i as alpha
                storyImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeEpilogueImageOut(bool fadeAway)
    {
        Debug.Log("image should fade");
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 0.35f)
            {
                // set color with i as alpha
                storyImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }
}
