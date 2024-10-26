using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class EncounterHandler : MonoBehaviour
{
    public ExploreHandler exploreHandler;

    //encounter in play in this handler (lets use this also for v88 encounter system)
    public Encounter2 encounterOption;

    public List<GameObject> encounter1Icons;
    public List<GameObject> encounter2Icons;

    public GameObject encounterDisplay;
    public GameObject encounterOptions;

    //for v0.5.7.
    public GameObject battlefieldDisplay;

    //new trading thingys
    public GameObject tradingHolder;
    public GameObject tradingBackButton;

    public TextMeshProUGUI encounter1Text;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyDescriptionText;

    //stores foe ability cards
    public GameObject foeCardArea;

    //for handling the enemy statdisplays
    public TextMeshProUGUI enemyAttackText;
    public TextMeshProUGUI enemyDefenseText;
    public TextMeshProUGUI enemyAPText;
    public TextMeshProUGUI enemyResistanceText;
    public TextMeshProUGUI enemyEnergyText;
    public TextMeshProUGUI enemyTierText;

    //for v94
    public TextMeshProUGUI enemyAttackModText;
    public TextMeshProUGUI enemyDefenseModText;
    public TextMeshProUGUI enemyAPModText;
    public TextMeshProUGUI enemyResistanceModText;

    public GameObject enemyAttackDisplay;
    public GameObject enemyAPDisplay;
    public GameObject enemyEnergyDisplay;
    public GameObject enemyTierDisplay;

    public GameObject enemyAttackModDisplay;
    public GameObject enemyAPModDisplay;

    //when showing foe stats & abilities
    public GameObject enemyStatsDisplay;
    public GameObject enemyInfoButton;

    //public Button doneButton1;
    //public GameObject skulls1;
    //public GameObject unique1;
    //public Button exploreMoreButton;

    //counters for bards skills & priestess perks
    //int bardCounter;
    //int bardReduction;
    //int healCounter;
    //int healReduction;

    //for storing the costs of the encounter
    //cost type 0 = energy, 1= warriors, 2= artisans, 3= arcanists, 4= coins, 5= Favor
    //note that index 3 cost type is reserved for favor and index 4 for energy
    //public int[] costType;
    //public int[] costQty;

    //keep track of possible continue encounter & special effect
    //int continueEncounter;
    //int specialEffect;

    //put an overlay on encounter if its not available
    //public GameObject overlayPanel;

    //holder for spawned buttons
    public GameObject buttonHolder;

    //special effects info
    public int exploreCost;
    public int rerollCost;
    public int focusCost;

    //keep track of difficulty modifier
    public int destinationDifficultyModifier;

    //keep track of progress on encounter
    public int buttonChosen;
    public bool skillCheckSuccess;
    public int progressiveSkillcheckHits;

    //flag variables for various other actions
    public bool skinningDone;
    public bool trophyHuntingDone;
    public bool disenchantDone;
    public bool charmDone;
    public bool charmAnimalDone;
    public bool forcedEncounterCancelled;
    public bool dontAllowExploreMore;

    //for simplifying simple encounters (one option encounters)
    public bool useAutomation;
    public bool firstCheckSolved;

    //for v90
    //used for making sure multi-stage encounters cant reduce encounter counters several times
    public bool encounterBeenResolved;

    public EncounterButton buttonChosen2;

    //skillcheck buttons
    public List<GameObject> skillCheckButtons;
    public List<GameObject> progressiveSkillCheckButtons;

    //combat buttons
    public List<GameObject> attackButtons;
    public List<GameObject> buttonsAfterBasicAttack;

    public List<GameObject> defenseButtons;
    public List<GameObject> buttonsAfterDefend;

    public List<GameObject> fleeButtons;

    public GameObject focusButton;
    public GameObject combatModeButton;
    public GameObject prayerButton;

    //special check buttons 
    //could be used for any special ability (hero or foe?)
    public List<GameObject> specialButtons;
    //special buttons spawned after the initial choice
    public List<GameObject> specialButtons2;

    //lets put this on combathandler instead
    //public int attackTypeUsed;
    //public bool isDestinationEncounter;

    public Image encounterIcon;
    public Image foeEncounterIcon;
    //used by foe specials
    public Image specialIcon;

    //for v94
    public GameObject encounterIconBackground;
    public GameObject encounterBorder;
    public Image currentFoeLargeIcon;

    public Sprite normalBackground;
    public Sprite holedBackground;

    public GameObject pauseButton;
    public GameObject pauseOverlay;

    //for v0.5.7. pause handling
    public GameObject battlefieldPauseButton;
    public TextMeshProUGUI battleSpeedtext;
    public Button reduceBattleSpeedButton;
    public Button increaseBattleSpeedButton;
    public float currentBattleSpeed;

    //keeps track whether combat is paused or not
    //need to refactor this for the new system soon
    //this variable is unused in v0.5.7.?
    public bool combatPaused;
    //actually better use another variable :-)
    public bool realTimeCombatPaused;

    //special encounter for portals in v93
    //portal 1 is scroll, portal 2 is spell
    public Encounter2 portalEncounter;
    public Encounter2 portalEncounter2;

    public GameObject largeIconHolder;
    public Image largeIcon;

    //for foe resizing
    public Vector2 foeIconOriginalSize;

    //used by dicerolling in v95
    public int diceRollCount;
    public int bestRoll;

    // Start is called before the first frame update
    void Start()
    {
        //reset all the taken flags on the encounters
        //only non-respawning encounters will have their flag changed on discovery
        //EncounterReset();

        useAutomation = true;

        //foeIconOriginalSize = new Vector2(currentFoeLargeIcon.sprite.rect.width, currentFoeLargeIcon.sprite.rect.height);

        //bit worried this might get messed with different resolutions?
        foeIconOriginalSize = new Vector2(160, 160);

        currentBattleSpeed = 1f;
    }

    //could delete this soon as well
    #region old get encounter
    /* dont need this anymore
     * return an encounter number of the wanted list
    public int GetEncounter(int listType)
    {
        List<GameObject> listReference = exploreHandler.plainEncounters;

        //plains
        if (listType == 1)
        {
            listReference = exploreHandler.plainEncounters;
        }
        //forest
        if (listType == 2)
        {
            listReference = exploreHandler.forestEncounters;
        }
        //mountains
        if (listType == 3)
        {
            listReference = exploreHandler.mountainEncounters;
        }
        //shore
        if (listType == 4)
        {
            listReference = exploreHandler.shoreEncounters;
        }
        //farmlands
        if (listType == 5)
        {
            listReference = exploreHandler.farmlandEncounters;
        }
        //habitat
        if (listType == 10)
        {
            listReference = exploreHandler.habitatEncounters;
        }
        //mystics
        if (listType == 11)
        {
            listReference = exploreHandler.mysticsEncounters;
        }
        //underground
        if (listType == 12)
        {
            listReference = exploreHandler.undergroundEncounters;
        }
        //store
        if (listType == 13)
        {
            listReference = exploreHandler.storesEncounters;
        }
        //academy
        if (listType == 14)
        {
            listReference = exploreHandler.academyEncounters;
        }
        //fortress
        if (listType == 15)
        {
            listReference = exploreHandler.fortressEncounters;
        }
        //smithy
        if (listType == 20)
        {
            listReference = exploreHandler.smithyEncounters;
        }
        //inn
        if (listType == 21)
        {
            listReference = exploreHandler.innEncounters;
        }
        //wilorge
        if (listType == 22)
        {
            listReference = exploreHandler.wilforgeEncounters;
        }
        //oldmines
        if (listType == 23)
        {
            listReference = exploreHandler.oldminesEncounters;
        }
        //factory
        if (listType == 24)
        {
            listReference = exploreHandler.factoryEncounters;
        }
        //temple
        if (listType == 25)
        {
            listReference = exploreHandler.templeEncounters;
        }
        //grimhold
        if (listType == 26)
        {
            listReference = exploreHandler.grimholdEncounters;
        }
        //brevir fort
        if (listType == 27)
        {
            listReference = exploreHandler.brevirFortEncounters;
        }
        //blue citadel
        if (listType == 28)
        {
            listReference = exploreHandler.blueCitadelEncounters;
        }
        //forest vault
        if (listType == 29)
        {
            listReference = exploreHandler.forestVaultEncounters;
        }
        //coven
        if (listType == 30)
        {
            listReference = exploreHandler.covenEncounters;
        }
        //graveyard
        if (listType == 31)
        {
            listReference = exploreHandler.graveyardEncounters;
        }
        //valley
        if (listType == 32)
        {
            listReference = exploreHandler.valleyEncounters;
        }
        //guildhouse
        if (listType == 33)
        {
            listReference = exploreHandler.guildhouseEncounters;
        }
        //cornville
        if (listType == 34)
        {
            listReference = exploreHandler.cornvilleEncounters;
        }
        //continue
        if (listType == 40)
        {
            listReference = exploreHandler.continueEncounters;
        }

        int i = 0;
        do
        {
            int encounterNumber = Random.Range(0, listReference.Count);
            if (listReference[encounterNumber].GetComponent<Encounter2>().isTaken == false)
            {
                int rarityCheck = Random.Range(1, 11);
                Encounter2 encounter = listReference[encounterNumber].GetComponent<Encounter2>();

                //check time requirements (dont allow invalid encounters through)
                if ((encounter.requirementTime == 1 && Clock.clock.isNight == false) || (encounter.requirementTime == 2 && Clock.clock.isNight == true) || encounter.requirementTime == 0)
                {
                    if (GameManager.ins.gameStage == 1 && rarityCheck <= encounter.stage1Rarity)
                    {
                        return encounterNumber;
                    }
                    if (GameManager.ins.gameStage == 2 && rarityCheck <= encounter.stage2Rarity)
                    {
                        return encounterNumber;
                    }
                    if (GameManager.ins.gameStage == 3 && rarityCheck <= encounter.stage3Rarity)
                    {
                        return encounterNumber;
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        //plains
        if (listType == 1)
        {
            int i = 0;
            do
            {
                int encounterNumber = Random.Range(0, exploreHandler.plainEncounters.Count);
                if (exploreHandler.plainEncounters[encounterNumber].GetComponent<Encounter>().isTaken == false)
                {
                    int rarityCheck = Random.Range(1, 11);
                    Encounter encounter = exploreHandler.plainEncounters[encounterNumber].GetComponent<Encounter>();

                    //check time requirements (dont allow invalid encounters through)
                    if ((encounter.requirementTime == 1 && Clock.clock.isNight == false) || (encounter.requirementTime == 2 && Clock.clock.isNight == true) || encounter.requirementTime == 0)
                    {
                        //raritychecks for both early and lategame
                        if (GameManager.ins.lateGame == false && rarityCheck <= encounter.stage1Rarity)
                        {
                            return encounterNumber;
                        }
                        if (GameManager.ins.lateGame == true && rarityCheck <= encounter.stage3Rarity)
                        {
                            return encounterNumber;
                        }
                    }
                }
                i++;
            }
            //not the best idea, but dunno what condition to set here
            while (i < 1000);
        }

        return 0;
    }
    */
    #endregion

    /* old showicon
    public void ShowIcon1(Encounter2 encounterOption1)
    {
        for (int i = 0; i < encounter1Icons.Count; i++)
        {
            encounter1Icons[i].gameObject.SetActive(false);

            if(i == encounterOption1.iconNumber)
            {
                encounter1Icons[i].gameObject.SetActive(true);
            }
        }
    }

    //similar as above, but gives int value instead of encounter
    public void ShowIcon2(int iconNumber)
    {
        for (int i = 0; i < encounter1Icons.Count; i++)
        {
            encounter1Icons[i].gameObject.SetActive(false);

            if (i == iconNumber)
            {
                encounter1Icons[i].gameObject.SetActive(true);
            }
        }
    }
    */

    //similar as above, but needs image instead of iconnumber
    public void ShowIcon3(Sprite icon)
    {
        encounterIcon.gameObject.SetActive(true);

        encounterBorder.SetActive(true);
        encounterIconBackground.SetActive(true);

        encounterIcon.sprite = icon;

        currentFoeLargeIcon.gameObject.SetActive(false);

        //special case for large icons
        if (buttonChosen2 != null)
        {
            if (buttonChosen2.showLargeIcon == true)
            {
                largeIconHolder.SetActive(true);
                largeIcon.sprite = icon;
            }
            else
            {
                largeIconHolder.SetActive(false);
            }
        }
    }

    public void SetFoeIcon(Sprite icon)
    {
        foeEncounterIcon.gameObject.SetActive(true);

        foeEncounterIcon.sprite = icon;

        currentFoeLargeIcon.gameObject.SetActive(true);
        currentFoeLargeIcon.sprite = icon;

        encounterIcon.gameObject.SetActive(false);
        encounterBorder.SetActive(false);
        encounterIconBackground.SetActive(false);

        //make foe icons size change here
        currentFoeLargeIcon.rectTransform.sizeDelta = foeIconOriginalSize * encounterOption.displaySize;
        //need to update the enemy resizing class too
        //GameManager.ins.references.enemyResizing.original = foeIconOriginalSize * encounterOption.displaySize;
    }

    public void ShowSpecialIconBriefly(Sprite icon)
    {
        specialIcon.gameObject.SetActive(true);

        specialIcon.sprite = icon;

        //set full alpha, then make it transparent slowly
        specialIcon.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        StartCoroutine(FadeSpecialIconOut(true));
    }

    IEnumerator FadeSpecialIconOut(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 0.35f)
            {
                // set color with i as alpha
                specialIcon.GetComponent<Image>().color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    //maybe lets store these for later reference

    #region old donebutton
    /*ends exploration phase
    public void DoneButton()
    {
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //goblet of glory reward / check
        if (specialEffect == 2)
        {
            //if card is taken alrdy, give favor instead
            if (GameManager.ins.artifactDeck[13].GetComponent<CardDisplay>().isTaken == true)
            {
                //for testing if the artifact is on offer, and takes it from offer if so
                if (GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(13) == true)
                {
                    //note that this uses childnumber of the artifact offer canvas
                    GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_RemoveArtifactFromOffer", RpcTarget.AllBufferedViaServer, GameManager.ins.questCanvas.GetComponent<QuestingDialog>().artifactToRemove);

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawSpecificCard(10, 13);
                }
                else
                {
                    exploreHandler.cardTakenDialog.SetActive(true);
                    exploreHandler.cardTakenText.text = "Artifact is already taken, you gain 3 Reputation instead.";
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 3);

                    //close also the current window
                    exploreHandler.PV.RPC("RPC_CancelExplore5", RpcTarget.AllBufferedViaServer);
                    return;
                }
            }
            //otherwise give the artifact, VP alrdy given earlier
            else
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawSpecificCard(10, 13);
            }
        }

        //lividium powder reward / check
        if (specialEffect == 6)
        {
            //if card is taken alrdy, give favor instead
            if (GameManager.ins.artifactDeck[9].GetComponent<CardDisplay>().isTaken == true)
            {
                //for testing if the artifact is on offer, and takes it from offer if so
                if (GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(9) == true)
                {
                    //note that this uses childnumber of the artifact offer canvas
                    GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_RemoveArtifactFromOffer", RpcTarget.AllBufferedViaServer, GameManager.ins.questCanvas.GetComponent<QuestingDialog>().artifactToRemove);

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawSpecificCard(10, 9);
                }

                //special case for ai
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 3);
                }
                else
                {
                    exploreHandler.cardTakenDialog.SetActive(true);
                    exploreHandler.cardTakenText.text = "Artifact is already taken, you gain 3 Reputation instead.";
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 3);

                    //close also the current window
                    exploreHandler.PV.RPC("RPC_CancelExplore5", RpcTarget.AllBufferedViaServer);
                    return;
                }
            }
            //otherwise give the artifact, VP alrdy given earlier
            else
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawSpecificCard(10, 9);
            }
        }

        if (continueEncounter == 0)
        {
            exploreHandler.PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        }

        //second explore
        if (continueEncounter == 999)
        {
            exploreHandler.PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

            //open hero animation
            GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 12);

            exploreHandler.encounterHandlerToUse = 1;
            //update this
            //exploreHandler.PV.RPC("RPC_HandlerToUse", RpcTarget.AllBufferedViaServer, 1);

            if (exploreHandler.locationOptionChosen == 1)
            {
                exploreHandler.ExploreChosen1();
            }
            if (exploreHandler.locationOptionChosen == 2)
            {
                exploreHandler.ExploreChosen2();
            }
            if (exploreHandler.locationOptionChosen == 3)
            {
                exploreHandler.ExploreChosen3();
            }
        }

        else if (continueEncounter != 0)
        {
            //sentinel cant become poisoned (so no need for continue encounter)
            if (specialEffect == 4)
            {
                //special check for sentinel
                if (character.GetComponentInParent<CharController>().SentinelTest() == true)
                {
                    //give poisoner a message too	
                    string msgs = "You are immune to the poison.";
                    GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_PrivateMessage", RpcTarget.AllBufferedViaServer, msgs, GameManager.ins.turnNumber);

                    //end encounter
                    exploreHandler.PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
                    return;
                }
            }

            //show walking animation
            GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 12);

            //closes the option windows
            exploreHandler.PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

            //open continue encounter on window1
            exploreHandler.PV.RPC("RPC_SetEncounter", RpcTarget.AllBufferedViaServer, 1, 40, continueEncounter);
        }
    }
    */
    #endregion

    #region old optionbuttons
    /*
     * when clicking the first option button
    public void Option1Button1()
    {
        exploreHandler.exploreMoreButton.interactable = false;

        Encounter2 encounter = encounterOption.GetComponent<Encounter2>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //disable second display, and show encounter icon on the first display
        //so continuation effects also show at first display
        exploreHandler.PV.RPC("RPC_DisableSecondDisplay", RpcTarget.AllBufferedViaServer, encounter.iconNumber);

        //counters for bards skills & priestess perks
        bardCounter = 0;
        bardReduction = 0;
        healCounter = 0;
        healReduction = 0;

        //special effect 1 leaves the explore phase and ends turn
        //also interrupts method
        if (encounterOption.specialEffectButton1 == 1)
        {
            exploreHandler.PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
            return;
        }

        //set sleep
        if (encounterOption.specialEffectButton1 == 5)
        {
            GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().PV.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, character.GetComponentInParent<CharController>().turnNumber, 1);
        }

        //lose up to 3 random followers
        //note that you can have other costs with this
        if (encounterOption.specialEffectButton1 == 7)
        {
            LoseRandomFollowers(3);
        }

        //gain random quest
        if (encounterOption.specialEffectButton1 == 8)
        {
            int questCard = GameManager.ins.TakeQuestCardNumber();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_DrawHandCards", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, questCard, 7);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<AvatarSetup>().PV.RPC("RPC_DrawQuestCardOnBoard2", RpcTarget.AllBufferedViaServer, questCard, GameManager.ins.turnNumber);
        }

        //goes through all the requirements
        for (int i = 0; i < encounter.requirementTypeButton1.Length; i++)
        {
            //energy cost
            if (encounter.requirementTypeButton1[i] == 0)
            {
                int difference = encounter.requirementQtyButton1[i];

                //special check for swiftness
                if (character.GetComponentInParent<CharController>().SwiftnessTest() == true && encounter.specialEffectButton1 == 3)
                {
                    difference = 0;
                }

                //actually lets allow going into negatives
                costType[4] = 0;
                costQty[4] += difference;
            }

            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton1[i] == 1 || encounter.requirementTypeButton1[i] == 2)
            {
                int difference = encounter.requirementQtyButton1[i] - character.strength;

                //special check for heavy armor
                if (encounter.isSkillCheckButton1[i] == 2 && character.GetComponentInParent<CharController>().HeavyArmorTest() == true)
                {
                    difference -= 1;
                }
                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton1[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton1[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 1;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton1[i] == 3 || encounter.requirementTypeButton1[i] == 4)
            {
                int difference = encounter.requirementQtyButton1[i] - character.mechanics;

                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton1[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton1[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 2;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton1[i] == 5 || encounter.requirementTypeButton1[i] == 6)
            {
                int difference = encounter.requirementQtyButton1[i] - character.lore;

                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton1[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton1[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 3;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //coins
            if (encounter.requirementTypeButton1[i] == 7)
            {
                int difference = encounter.requirementQtyButton1[i];

                costType[i] = 4;
                costQty[i] = difference;
            }
            //reputation
            if (encounter.requirementTypeButton1[i] == 8)
            {
                int difference = encounter.requirementQtyButton1[i];

                //actually lets allow going into negatives
                costType[i] = 5;
                costQty[i] = difference;
            }
        }

        ImplementCost();

        //better implement reward per button, instead of its own method
        //reward types: 0 = energy, 1 = warriors, 2 = artisans, 3 = arcanists, 4 = coins, 5 = fame (6 = fame total?)
        //furthermore: 7 = quest cards, 8 = intelligence cards, 9 = artifact cards, 10 = event cards?
        for (int i = 0; i < encounter.rewardTypeButton1.Length; i++)
        {
            if (encounter.rewardTypeButton1[i] >= 0 && encounter.rewardTypeButton1[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounter.rewardTypeButton1[i], encounter.rewardQtyButton1[i]);
            }
            else if (encounter.rewardTypeButton1[i] == 7 || encounter.rewardTypeButton1[i] == 8)
            {
                //special case for AI
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(5, 1);
                }
                else
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCardsCurrentOnly(encounter.rewardTypeButton1[i], encounter.rewardQtyButton1[i]);
                }
            }
        }

        character.UpdateResourceTexts();

        //update info for the done button
        //probably dont need to be rpc call, since the done button has it?
        exploreHandler.encounterHandler1.continueEncounter = encounter.continueEncounterButton1;
        exploreHandler.encounterHandler1.specialEffect = encounter.specialEffectButton1;

        /*
         * Show endtext, and cost + reward
         

        string resourceChanges = "";

        if (encounter.requirementQtyButton1.Length > 0 || costQty[0] != 0 || costQty[1] != 0 || costQty[2] != 0)
        {
            resourceChanges = "\n<size=8>\n</size>Cost: ";

            for (int i = 0; i < costType.Length; i++)
            {
                if (costQty[i] > 0 && costQty[i] < 10)
                {
                    for (int y = 0; y < costQty[i]; y++)
                    {
                        resourceChanges += GameManager.ins.questCanvas.GetComponent<QuestingDialog>().CostName(costType[i]);
                    }
                    resourceChanges += " ";
                }
                if (costQty[i] >= 10)
                {
                    resourceChanges += costQty[i] + GameManager.ins.questCanvas.GetComponent<QuestingDialog>().CostName(costType[i]) + " ";
                }
            }
        }
        if (encounter.rewardQtyButton1.Length > 0)
        {
            resourceChanges += "\n<size=8>\n</size>Reward: ";

            for (int i = 0; i < encounter.rewardTypeButton1.Length; i++)
            {
                if (encounter.rewardQtyButton1[i] > 0 && encounter.rewardQtyButton1[i] < 10)
                {
                    for (int y = 0; y < encounter.rewardQtyButton1[i]; y++)
                    {
                        resourceChanges += GameManager.ins.questCanvas.GetComponent<QuestingDialog>().RewardName(encounter.rewardTypeButton1[i]);
                    }
                    resourceChanges += " ";
                }
                if (encounter.rewardQtyButton1[i] >= 10)
                {
                    resourceChanges += encounter.rewardQtyButton3[i] + GameManager.ins.questCanvas.GetComponent<QuestingDialog>().RewardName(encounter.rewardTypeButton1[i]) + " ";
                }
            }
        }
        //encounter1Text.text = encounter.endText2 + resourceChanges;

        //updates text & buttons for all
        exploreHandler.PV.RPC("RPC_UpdateEncounter1", RpcTarget.AllBufferedViaServer, encounter.endText1 + resourceChanges, exploreHandler.encounterHandler1.continueEncounter);

        //shows first skillcheck animation for all (if there was a skillcheck)
        if (encounter.requirementTypeButton1.Length > 0)
        {
            if (encounter.requirementTypeButton1[0] > 0 && encounter.requirementTypeButton1[0] < 7)
            {
                GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, encounter.requirementTypeButton1[0]);
            }
        }

        //actually this is kinda complicated way of doing this, lets do the other checks more simple
        StoneOfAlromanCheck(1, encounter);

        //victory rush check
        for (int i = 0; i < encounter.requirementTypeButton1.Length; i++)
        {
            if ((encounter.requirementTypeButton1[i] == 1 && encounter.isSkillCheckButton1[i] == 2) ||
                (encounter.requirementTypeButton1[i] == 2 && encounter.isSkillCheckButton1[i] == 2))
            {
                //check if you got stone of alroman, and didnt fail on quest
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().VictoryRushTest() == true)
                {
                    //gives 3 energy
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
                }
            }
        }

        //tests if player has the folk hero perk
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 1 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                {
                    for (int y = 0; y < encounter.rewardTypeButton1.Length; y++)
                    {
                        if (encounter.rewardTypeButton1[y] == 5 && encounter.rewardQtyButton1[y] > 3)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);

                            //dont show message to AI players
                            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
                            {
                                //give message
                                string msgs = "You gained additional Reputation.";
                                GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                            }
                        }
                    }
                }
            }
        }
    }

    //when clicking the second option button
    public void Option1Button2()
    {
        exploreHandler.exploreMoreButton.interactable = false;

        Encounter encounter = encounterOption.GetComponent<Encounter>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //disable second display, and show encounter icon on the first display
        //so continuation effects also show at first display
        exploreHandler.PV.RPC("RPC_DisableSecondDisplay", RpcTarget.AllBufferedViaServer, encounter.iconNumber);

        //counters for bards skills & priestess perks
        bardCounter = 0;
        bardReduction = 0;
        healCounter = 0;
        healReduction = 0;

        //special effect 1 leaves the explore phase and ends turn
        //also interrupts method
        if (encounterOption.specialEffectButton2 == 1)
        {
            exploreHandler.PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
            return;
        }

        //set sleep
        if (encounterOption.specialEffectButton2 == 5)
        {
            GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().PV.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, character.GetComponentInParent<CharController>().turnNumber, 1);
        }

        //lose up to 3 random followers
        //note that you can have other costs with this
        if (encounterOption.specialEffectButton2 == 7)
        {
            LoseRandomFollowers(3);
        }

        //gain random quest
        if (encounterOption.specialEffectButton2 == 8)
        {
            int questCard = GameManager.ins.TakeQuestCardNumber();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_DrawHandCards", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, questCard, 7);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<AvatarSetup>().PV.RPC("RPC_DrawQuestCardOnBoard2", RpcTarget.AllBufferedViaServer, questCard, GameManager.ins.turnNumber);
        }

        //goes through all the requirements
        for (int i = 0; i < encounter.requirementTypeButton2.Length; i++)
        {
            //energy cost
            if (encounter.requirementTypeButton2[i] == 0)
            {
                int difference = encounter.requirementQtyButton2[i];

                //special check for swiftness
                if (character.GetComponentInParent<CharController>().SwiftnessTest() == true && encounter.specialEffectButton2 == 3)
                {
                    difference = 0;
                }

                //actually lets allow going into negatives
                costType[4] = 0;
                costQty[4] += difference;
            }

            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton2[i] == 1 || encounter.requirementTypeButton2[i] == 2)
            {
                int difference = encounter.requirementQtyButton2[i] - character.strength;

                //special check for heavy armor
                if (encounter.isSkillCheckButton2[i] == 2 && character.GetComponentInParent<CharController>().HeavyArmorTest() == true)
                {
                    difference -= 1;
                }
                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton2[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton2[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 1;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton2[i] == 3 || encounter.requirementTypeButton2[i] == 4)
            {
                int difference = encounter.requirementQtyButton2[i] - character.mechanics;

                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton2[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton2[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 2;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton2[i] == 5 || encounter.requirementTypeButton2[i] == 6)
            {
                int difference = encounter.requirementQtyButton2[i] - character.lore;

                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton2[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton2[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 3;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //coins
            if (encounter.requirementTypeButton2[i] == 7)
            {
                int difference = encounter.requirementQtyButton2[i];

                costType[i] = 4;
                costQty[i] = difference;
            }
            //reputation
            if (encounter.requirementTypeButton2[i] == 8)
            {
                int difference = encounter.requirementQtyButton2[i];

                //actually lets allow going into negatives
                costType[i] = 5;
                costQty[i] = difference;
            }
        }

        ImplementCost();

        //better implement reward per button, instead of its own method
        //reward types: 0 = energy, 1 = warriors, 2 = artisans, 3 = arcanists, 4 = coins, 5 = fame (6 = fame total?)
        //furthermore: 7 = quest cards, 8 = intelligence cards, 9 = artifact cards, 10 = event cards?
        for (int i = 0; i < encounter.rewardTypeButton2.Length; i++)
        {
            if (encounter.rewardTypeButton2[i] >= 0 && encounter.rewardTypeButton2[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounter.rewardTypeButton2[i], encounter.rewardQtyButton2[i]);
            }
            else if (encounter.rewardTypeButton2[i] == 7 || encounter.rewardTypeButton2[i] == 8)
            {
                //special case for AI
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(5, 1);
                }
                else
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCardsCurrentOnly(encounter.rewardTypeButton2[i], encounter.rewardQtyButton2[i]);
                }
            }
        }

        character.UpdateResourceTexts();

        //update info for the done button
        exploreHandler.encounterHandler1.continueEncounter = encounter.continueEncounterButton2;
        exploreHandler.encounterHandler1.specialEffect = encounter.specialEffectButton2;

        /*
         * Show endtext, and cost + reward
         

        string resourceChanges = "";

        if (encounter.requirementQtyButton2.Length > 0 || costQty[0] != 0 || costQty[1] != 0 || costQty[2] != 0)
        {
            resourceChanges = "\n<size=8>\n</size>Cost: ";

            for (int i = 0; i < costType.Length; i++)
            {
                if (costQty[i] > 0 && costQty[i] < 10)
                {
                    for (int y = 0; y < costQty[i]; y++)
                    {
                        resourceChanges += GameManager.ins.questCanvas.GetComponent<QuestingDialog>().CostName(costType[i]);
                    }
                    resourceChanges += " ";
                }
                if (costQty[i] >= 10)
                {
                    resourceChanges += costQty[i] + GameManager.ins.questCanvas.GetComponent<QuestingDialog>().CostName(costType[i]) + " ";
                }
            }
        }
        if (encounter.rewardQtyButton2.Length > 0)
        {
            resourceChanges += "\n<size=8>\n</size>Reward: ";

            for (int i = 0; i < encounter.rewardTypeButton2.Length; i++)
            {
                if (encounter.rewardQtyButton2[i] > 0 && encounter.rewardQtyButton2[i] < 10)
                {
                    for (int y = 0; y < encounter.rewardQtyButton2[i]; y++)
                    {
                        resourceChanges += GameManager.ins.questCanvas.GetComponent<QuestingDialog>().RewardName(encounter.rewardTypeButton2[i]);
                    }
                    resourceChanges += " ";
                }
                if (encounter.rewardQtyButton2[i] >= 10)
                {
                    resourceChanges += encounter.rewardQtyButton2[i] + GameManager.ins.questCanvas.GetComponent<QuestingDialog>().RewardName(encounter.rewardTypeButton2[i]) + " ";
                }
            }
        }
        //encounter1Text.text = encounter.endText2 + resourceChanges;

        //updates text & buttons for all
        exploreHandler.PV.RPC("RPC_UpdateEncounter1", RpcTarget.AllBufferedViaServer, encounter.endText2 + resourceChanges, exploreHandler.encounterHandler1.continueEncounter);

        //shows first skillcheck animation for all (if there was a skillcheck)
        if (encounter.requirementTypeButton2.Length > 0)
        {
            if (encounter.requirementTypeButton2[0] > 0 && encounter.requirementTypeButton2[0] < 7)
            {
                GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, encounter.requirementTypeButton2[0]);
            }
        }

        //actually this is kinda complicated way of doing this, lets do the other checks more simple
        StoneOfAlromanCheck(2, encounter);

        //victory rush check
        for (int i = 0; i < encounter.requirementTypeButton2.Length; i++)
        {
            if ((encounter.requirementTypeButton2[i] == 1 && encounter.isSkillCheckButton2[i] == 2) ||
                (encounter.requirementTypeButton2[i] == 2 && encounter.isSkillCheckButton2[i] == 2))
            {
                //check if you got stone of alroman, and didnt fail on quest
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().VictoryRushTest() == true)
                {
                    //gives 3 energy
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
                }
            }
        }

        //tests if player has the folk hero perk
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 1 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                {
                    for (int y = 0; y < encounter.rewardTypeButton2.Length; y++)
                    {
                        if (encounter.rewardTypeButton2[y] == 5 && encounter.rewardQtyButton2[y] > 3)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);

                            //dont show message to AI players
                            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
                            {
                                //give message
                                string msgs = "You gained additional Reputation.";
                                GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                            }
                        }
                    }
                }
            }
        }
    }

    //when clicking the third option button
    public void Option1Button3()
    {
        exploreHandler.exploreMoreButton.interactable = false;

        Encounter encounter = encounterOption.GetComponent<Encounter>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //disable second display, and show encounter icon on the first display
        //so continuation effects also show at first display
        exploreHandler.PV.RPC("RPC_DisableSecondDisplay", RpcTarget.AllBufferedViaServer, encounter.iconNumber);

        //counters for bards skills & priestess perks
        bardCounter = 0;
        bardReduction = 0;
        healCounter = 0;
        healReduction = 0;

        //special effect 1 leaves the explore phase and ends turn
        //also interrupts method
        if (encounterOption.specialEffectButton3 == 1)
        {
            exploreHandler.PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
            return;
        }
        //set sleep
        if (encounterOption.specialEffectButton3 == 5)
        {
            //send player to sleep
            GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().PV.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, character.GetComponentInParent<CharController>().turnNumber, 1);
        }

        //lose up to 3 random followers
        //note that you can have other costs with this
        if (encounterOption.specialEffectButton3 == 7)
        {
            LoseRandomFollowers(3);
        }

        //gain random quest
        if (encounterOption.specialEffectButton3 == 8)
        {
            int questCard = GameManager.ins.TakeQuestCardNumber();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_DrawHandCards", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, questCard, 7);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<AvatarSetup>().PV.RPC("RPC_DrawQuestCardOnBoard2", RpcTarget.AllBufferedViaServer, questCard, GameManager.ins.turnNumber);
        }

        //goes through all the requirements
        for (int i = 0; i < encounter.requirementTypeButton3.Length; i++)
        {
            //energy cost
            if (encounter.requirementTypeButton3[i] == 0)
            {
                int difference = encounter.requirementQtyButton3[i];

                //special check for swiftness
                if (character.GetComponentInParent<CharController>().SwiftnessTest() == true && encounter.specialEffectButton3 == 3)
                {
                    difference = 0;
                }

                //actually lets allow going into negatives
                costType[4] = 0;
                costQty[4] += difference;
            }

            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton3[i] == 1 || encounter.requirementTypeButton3[i] == 2)
            {
                int difference = encounter.requirementQtyButton3[i] - character.strength;

                //special check for heavy armor
                if (encounter.isSkillCheckButton3[i] == 2 && character.GetComponentInParent<CharController>().HeavyArmorTest() == true)
                {
                    difference -= 1;
                }
                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton3[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton3[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 1;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton3[i] == 3 || encounter.requirementTypeButton3[i] == 4)
            {
                int difference = encounter.requirementQtyButton3[i] - character.mechanics;

                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton3[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton3[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 2;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //can group the same "type" skills now, since theyre the same thing practically
            if (encounter.requirementTypeButton3[i] == 5 || encounter.requirementTypeButton3[i] == 6)
            {
                int difference = encounter.requirementQtyButton3[i] - character.lore;

                if (difference < 0)
                {
                    difference = 0;
                }

                //special checks for bards skills & priestess
                if (encounter.isSkillCheckButton3[i] == 1 && character.GetComponentInParent<CharController>().BardSkillsTest() == true)
                {
                    bardReduction = BardCheck(difference);
                }
                if (encounter.isSkillCheckButton3[i] == 2 && character.GetComponentInParent<CharController>().PriestessTest() == true)
                {
                    healReduction = HealCheck(difference);
                }

                difference = difference - bardReduction - healReduction;

                costType[i] = 3;
                costQty[i] = difference;

                bardReduction = 0;
                healReduction = 0;
            }
            //coins
            if (encounter.requirementTypeButton3[i] == 7)
            {
                int difference = encounter.requirementQtyButton3[i];

                costType[i] = 4;
                costQty[i] = difference;
            }
            //reputation
            if (encounter.requirementTypeButton3[i] == 8)
            {
                int difference = encounter.requirementQtyButton3[i];

                //actually lets allow going into negatives
                costType[i] = 5;
                costQty[i] = difference;
            }
        }

        ImplementCost();

        //better implement reward per button, instead of its own method
        //reward types: 0 = energy, 1 = warriors, 2 = artisans, 3 = arcanists, 4 = coins, 5 = fame (6 = fame total?)
        //furthermore: 7 = quest cards, 8 = intelligence cards, 9 = artifact cards, 10 = event cards?
        for (int i = 0; i < encounter.rewardTypeButton3.Length; i++)
        {
            if (encounter.rewardTypeButton3[i] >= 0 && encounter.rewardTypeButton3[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounter.rewardTypeButton3[i], encounter.rewardQtyButton3[i]);
            }
            else if (encounter.rewardTypeButton3[i] == 7 || encounter.rewardTypeButton3[i] == 8)
            {
                //special case for AI
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(5, 1);
                }
                else
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCardsCurrentOnly(encounter.rewardTypeButton3[i], encounter.rewardQtyButton3[i]);
                }
            }
        }

        character.UpdateResourceTexts();

        //update info for the done button
        //probably dont need to be rpc call, since the done button has it?
        exploreHandler.encounterHandler1.continueEncounter = encounter.continueEncounterButton3;
        exploreHandler.encounterHandler1.specialEffect = encounter.specialEffectButton3;

        /*
         * Show endtext, and cost + reward
         

        string resourceChanges = "";

        if (encounter.requirementQtyButton3.Length > 0 || costQty[0] != 0 || costQty[1] != 0 || costQty[2] != 0)
        {
            resourceChanges = "\n<size=8>\n</size>Cost: ";

            for (int i = 0; i < costType.Length; i++)
            {
                if (costQty[i] > 0 && costQty[i] < 10)
                {
                    for (int y = 0; y < costQty[i]; y++)
                    {
                        resourceChanges += GameManager.ins.questCanvas.GetComponent<QuestingDialog>().CostName(costType[i]);
                    }
                    resourceChanges += " ";
                }
                if (costQty[i] >= 10)
                {
                    resourceChanges += costQty[i] + GameManager.ins.questCanvas.GetComponent<QuestingDialog>().CostName(costType[i]) + " ";
                }
            }
        }
        if (encounter.rewardQtyButton3.Length > 0)
        {
            resourceChanges += "\n<size=8>\n</size>Reward: ";

            for (int i = 0; i < encounter.rewardTypeButton3.Length; i++)
            {
                if (encounter.rewardQtyButton3[i] > 0 && encounter.rewardQtyButton3[i] < 10)
                {
                    for (int y = 0; y < encounter.rewardQtyButton3[i]; y++)
                    {
                        resourceChanges += GameManager.ins.questCanvas.GetComponent<QuestingDialog>().RewardName(encounter.rewardTypeButton3[i]);
                    }
                    resourceChanges += " ";
                }
                if (encounter.rewardQtyButton3[i] >= 10)
                {
                    resourceChanges += encounter.rewardQtyButton3[i] + GameManager.ins.questCanvas.GetComponent<QuestingDialog>().RewardName(encounter.rewardTypeButton3[i]) + " ";
                }
            }
        }
        //encounter1Text.text = encounter.endText2 + resourceChanges;

        //updates text & buttons for all
        exploreHandler.PV.RPC("RPC_UpdateEncounter1", RpcTarget.AllBufferedViaServer, encounter.endText3 + resourceChanges, exploreHandler.encounterHandler1.continueEncounter);

        //shows first skillcheck animation for all (if there was a skillcheck)
        if (encounter.requirementTypeButton3.Length > 0)
        {
            if (encounter.requirementTypeButton3[0] > 0 && encounter.requirementTypeButton3[0] < 7)
            {
                GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, encounter.requirementTypeButton3[0]);
            }
        }

        //actually this is kinda complicated way of doing this, lets do the other checks more simple
        StoneOfAlromanCheck(3, encounter);

        //victory rush check
        for (int i = 0; i < encounter.requirementTypeButton3.Length; i++)
        {
            if ((encounter.requirementTypeButton3[i] == 1 && encounter.isSkillCheckButton3[i] == 2) ||
                (encounter.requirementTypeButton3[i] == 2 && encounter.isSkillCheckButton3[i] == 2))
            {
                //check if you got stone of alroman, and didnt fail on quest
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().VictoryRushTest() == true)
                {
                    //gives 3 energy
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
                }
            }
        }

        //tests if player has the folk hero perk
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 1 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                {
                    for (int y = 0; y < encounter.rewardTypeButton3.Length; y++)
                    {
                        if (encounter.rewardTypeButton3[y] == 5 && encounter.rewardQtyButton3[y] > 3)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);

                            //dont show message to AI players
                            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
                            {
                                //give message
                                string msgs = "You gained additional Reputation.";
                                GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                            }
                        }
                    }
                }
            }
        }
    }
    */
    #endregion

    /* old bards skills, healing, follower loss etc
     * 
     * makes bards skills calculations
    public int BardCheck(int difference)
    {
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        costType[4] = 0;

        for (int y = 0; y < difference; y++)
        {
            bardCounter += 1;

            if (bardCounter == 2 && character.energy > bardReduction)
            {
                bardCounter = 0;
                costQty[4] += 1;
                bardReduction += 1;
            }
        }

        return bardReduction;
    }

    //makes bards skills calculations
    public int HealCheck(int difference)
    {
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        costType[4] = 0;

        for (int y = 0; y < difference; y++)
        {
            healCounter += 1;

            if (healCounter == 2 && character.energy > healReduction)
            {
                healCounter = 0;
                costQty[4] += 1;
                healReduction += 1;
            }
        }

        return healReduction;
    }

    public void ImplementCost()
    {
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //goes through all the requirements
        for (int i = 0; i < costType.Length; i++)
        {
            //should work for all the resources: 0 = energy, 1 = warriors, 2 = artisans, 3 = arcanists, 4 = coins, 5 = reputation
            //energy and VP now rly used atm for costs
            if (costType[i] >= 0 && costType[i] < 7)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(costType[i], -costQty[i]);
            }
        }
    }

    //this is actually suliman perk check now
    //the other loremaster instances not relevant or ai, since ai wont use those
    public void StoneOfAlromanCheck(int buttonNumber, Encounter encounter)
    {
        if (buttonNumber == 1)
        {
            for (int i = 0; i < encounter.requirementTypeButton1.Length; i++)
            {
                if (encounter.requirementTypeButton1[i] == 5 || encounter.requirementTypeButton1[i] == 6)
                {
                    //check if you got stone of alroman, and didnt fail on quest
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().LoremasterTest() == true)
                    {
                        //special case for ai
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
                        }
                        else
                        {
                            //gives 1 int card
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(8, 1);
                        }
                    }
                }
            }
        }
        if (buttonNumber == 2)
        {
            for (int i = 0; i < encounter.requirementTypeButton2.Length; i++)
            {
                if (encounter.requirementTypeButton2[i] == 5 || encounter.requirementTypeButton2[i] == 6)
                {
                    //check if you got stone of alroman, and didnt fail on quest
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().LoremasterTest() == true)
                    {
                        //special case for ai
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
                        }
                        else
                        {
                            //gives 1 int card
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(8, 1);
                        }
                    }
                }
            }
        }
        if (buttonNumber == 3)
        {
            for (int i = 0; i < encounter.requirementTypeButton3.Length; i++)
            {
                if (encounter.requirementTypeButton3[i] == 5 || encounter.requirementTypeButton3[i] == 6)
                {
                    //check if you got stone of alroman, and didnt fail on quest
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().LoremasterTest() == true)
                    {
                        //special case for ai
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
                        }
                        else
                        {
                            //gives 1 int card
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(8, 1);
                        }
                    }
                }
            }
        }
    }

    //loses set number of random followers
    //note that you shouldnt have other cost with this special effect
    public void LoseRandomFollowers(int followers)
    {
        costType[0] = 1;
        costType[1] = 2;
        costType[2] = 3;

        costQty[0] = 0;
        costQty[1] = 0;
        costQty[2] = 0;

        int foundFollowers = 0;

        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        int totalFollowers = character.warriors + character.artisans + character.arcanists;

        //if less than 3 followers, only lose what you got
        if(totalFollowers < followers)
        {
            followers = totalFollowers;
        }

        do
        {
            int randomFollower = Random.Range(1, 4);

            if (randomFollower == 1 && character.warriors > costQty[0])
            {
                costQty[0] += 1;
                foundFollowers += 1;
            }
            if (randomFollower == 2 && character.artisans > costQty[1])
            {
                costQty[1] += 1;
                foundFollowers += 1;
            }
            if (randomFollower == 3 && character.arcanists > costQty[2])
            {
                costQty[2] += 1;
                foundFollowers += 1;
            }

        }
        while (foundFollowers < followers);
    }
    */

    //could make special check here for special foe skillchecks
    public void SpawnSkillCheckButtons()
    {
        if(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            for (int i = 0; i < specialButtons2.Count; i++)
            {
                if (specialButtons2[i] != null)
                {
                    //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                    if (specialButtons2[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                    {
                        GameObject buttonObject = Instantiate(specialButtons2[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                        //places it in button holder area
                        buttonObject.transform.SetParent(buttonHolder.transform, false);

                        buttonObject.GetComponent<Button>().interactable = false;

                        //lets change this for v95
                        if (specialButtons2[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                        {
                            GameManager.ins.references.currentEncounter.automatedButton = specialButtons2[i].gameObject;
                            GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                        }

                        /*
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        specialButtons2[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                        {
                            buttonObject.GetComponent<Button>().interactable = true;
                        }
                        */

                        //update button tooltip (if theres info to update)
                        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                    }
                }
            }
            return;
        }

        for (int i = 0; i < skillCheckButtons.Count; i++)
        {
            if (skillCheckButtons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (skillCheckButtons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    Debug.Log("should call skillcheck buttons");

                    GameObject buttonObject = Instantiate(skillCheckButtons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    /*
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        skillCheckButtons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }
                    */

                    //lets change this for v95
                    if (skillCheckButtons[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                    {
                        GameManager.ins.references.currentEncounter.automatedButton = skillCheckButtons[i].gameObject;
                        GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                    }

                    //automatically chooses one option for certain encounters
                    /*note that this now only works for initial option
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().useAutomation == true)
                    {
                        if (GameManager.ins.references.currentEncounter.useAutomation == true && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().firstCheckSolved == false)
                        {
                            buttonObject.GetComponent<Button>().interactable = false;

                            if (skillCheckButtons[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                            {
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().firstCheckSolved = false;

                                GameManager.ins.references.currentEncounter.automatedButton = skillCheckButtons[i].gameObject;
                                GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                            }
                        }
                    }
                    */

                    //special case for explosive foes
                    if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true &&
                        (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(242) == true ||
                         GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(243) == true))
                    {
                        //buttonObject.GetComponent<Button>().interactable = false;

                        if (buttonObject.GetComponent<EncounterButton>().isChosenAutomatically == true)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().firstCheckSolved = false;

                            CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 242, 4, 1);
                            CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 243, 4, 1);

                            //GameManager.ins.references.currentEncounter.automatedButton = buttonObject;
                            //GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                        }
                    }
                    

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    public void SpawnProgressiveSkillCheckButtons()
    {
        for (int i = 0; i < progressiveSkillCheckButtons.Count; i++)
        {
            if (progressiveSkillCheckButtons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (progressiveSkillCheckButtons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    Debug.Log("should call skillcheck buttons");

                    GameObject buttonObject = Instantiate(progressiveSkillCheckButtons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    //lets change this for v95
                    if (progressiveSkillCheckButtons[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                    {
                        GameManager.ins.references.currentEncounter.automatedButton = progressiveSkillCheckButtons[i].gameObject;
                        GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                    }

                    /*
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        progressiveSkillCheckButtons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }

                    //automatically chooses one option for certain encounters
                    //note that this now only works for initial option
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().useAutomation == true)
                    {
                        if (GameManager.ins.references.currentEncounter.useAutomation == true && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().firstCheckSolved == false)
                        {
                            buttonObject.GetComponent<Button>().interactable = false;

                            if (progressiveSkillCheckButtons[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                            {
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().firstCheckSolved = false;

                                GameManager.ins.references.currentEncounter.automatedButton = progressiveSkillCheckButtons[i].gameObject;
                                GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                            }
                        }
                    }
                    */

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    public void SpawnAttackButtons()
    {
        for (int i = 0; i < attackButtons.Count; i++)
        {
            if (attackButtons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (attackButtons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    Debug.Log("should call skillcheck buttons");

                    GameObject buttonObject = Instantiate(attackButtons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        attackButtons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    public void SpawnButtonsAfterBasicAttack()
    {
        for (int i = 0; i < buttonsAfterBasicAttack.Count; i++)
        {
            if (buttonsAfterBasicAttack[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (buttonsAfterBasicAttack[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    Debug.Log("should call skillcheck buttons");

                    GameObject buttonObject = Instantiate(buttonsAfterBasicAttack[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    /*
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        buttonsAfterBasicAttack[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }
                    */

                    //lets change this for v95
                    if (buttonsAfterBasicAttack[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                    {
                        GameManager.ins.references.currentEncounter.automatedButton = buttonsAfterBasicAttack[i].gameObject;
                        GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    //this is unused ?
    public void SpawnDefenseButtons()
    {
        for (int i = 0; i < defenseButtons.Count; i++)
        {
            if (defenseButtons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (defenseButtons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    Debug.Log("should call skillcheck buttons");

                    GameObject buttonObject = Instantiate(defenseButtons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        defenseButtons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    public void SpawnButtonsAfterBasicDefense()
    {
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            for (int i = 0; i < specialButtons2.Count; i++)
            {
                if (specialButtons2[i] != null)
                {
                    //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                    if (specialButtons2[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                    {
                        GameObject buttonObject = Instantiate(specialButtons2[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                        //places it in button holder area
                        buttonObject.transform.SetParent(buttonHolder.transform, false);

                        buttonObject.GetComponent<Button>().interactable = false;

                        /*
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        specialButtons2[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                        {
                            buttonObject.GetComponent<Button>().interactable = true;
                        }
                        */

                        //lets change this for v95
                        if (specialButtons2[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                        {
                            GameManager.ins.references.currentEncounter.automatedButton = specialButtons2[i].gameObject;
                            GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                        }

                        //update button tooltip (if theres info to update)
                        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                    }
                }
            }
            return;
        }

        for (int i = 0; i < buttonsAfterDefend.Count; i++)
        {
            if (buttonsAfterDefend[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (buttonsAfterDefend[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    GameObject buttonObject = Instantiate(buttonsAfterDefend[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    /*
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        buttonsAfterDefend[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }
                    */

                    //lets change this for v95
                    if (buttonsAfterDefend[i].GetComponent<EncounterButton>().isChosenAutomatically == true)
                    {
                        GameManager.ins.references.currentEncounter.automatedButton = buttonsAfterDefend[i].gameObject;
                        GameManager.ins.references.currentEncounter.UseAutomatedButtonWithDelay();
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    public void SpawnSpecialButtons()
    {
        bool hasSkillCheck = false;

        for (int i = 0; i < specialButtons.Count; i++)
        {
            if (specialButtons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (specialButtons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    GameObject buttonObject = Instantiate(specialButtons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        specialButtons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;

                        //focus system for v92
                        if (specialButtons[i].GetComponent<EncounterButton>().isSkillCheck.Length > 0)
                        {
                            if (specialButtons[i].GetComponent<EncounterButton>().isSkillCheck[0] == true)
                            {
                                hasSkillCheck = true;
                            }
                        }
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }

        if (hasSkillCheck == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnFocusButton();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnPrayerButton();
        }
    }

    //dunno if this is rly necessary, but too drunk to think otherwise
    public void SpawnFleeButtons()
    {
        for (int i = 0; i < fleeButtons.Count; i++)
        {
            if (fleeButtons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                if (fleeButtons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    GameObject buttonObject = Instantiate(fleeButtons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in button holder area
                    buttonObject.transform.SetParent(buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        fleeButtons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
                }
            }
        }
    }

    //dunno if this is rly necessary, but too drunk to think otherwise
    public void SpawnFocusButton()
    {
        GameObject buttonObject = Instantiate(focusButton.gameObject, new Vector3(0, 0, 0), Quaternion.identity);

        //places it in button holder area
        buttonObject.transform.SetParent(buttonHolder.transform, false);

        buttonObject.GetComponent<Button>().interactable = false;

        //maybe not use the encounter button values for this (is less of a hassle)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
        {
            buttonObject.GetComponent<Button>().interactable = true;

            //automatically chooses one option for certain encounters
            //note that this now only works for initial option
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().useAutomation == true)
            {
                if (GameManager.ins.references.currentEncounter.useAutomation == true && firstCheckSolved == false)
                {
                    buttonObject.GetComponent<Button>().interactable = false;
                }
            }

            //spcial case for exploding foes
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true &&
               (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(242) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(243) == true))
            {
                buttonObject.GetComponent<Button>().interactable = false;
            }
        }

        //update button tooltip (if theres info to update)
        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
    }

    //dunno if this is rly necessary, but too drunk to think otherwise
    public void SpawnPrayerButton()
    {
        GameObject buttonObject = Instantiate(prayerButton.gameObject, new Vector3(0, 0, 0), Quaternion.identity);

        //places it in button holder area
        buttonObject.transform.SetParent(buttonHolder.transform, false);

        buttonObject.GetComponent<Button>().interactable = false;

        //maybe not use the encounter button values for this (is less of a hassle)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            // && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
        {
            buttonObject.GetComponent<Button>().interactable = true;

            //automatically chooses one option for certain encounters
            //note that this now only works for initial option
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().useAutomation == true)
            {
                if (GameManager.ins.references.currentEncounter.useAutomation == true && firstCheckSolved == false)
                {
                    buttonObject.GetComponent<Button>().interactable = false;
                }
            }

            //spcial case for exploding foes
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true &&
               (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(242) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(243) == true))
            {
                buttonObject.GetComponent<Button>().interactable = false;
            }
        }

        //update button tooltip (if theres info to update)
        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
    }

    //dunno if this is rly necessary, but too drunk to think otherwise
    public void SpawnCombatModeButton()
    {
        GameObject buttonObject = Instantiate(combatModeButton.gameObject, new Vector3(0, 0, 0), Quaternion.identity);

        //places it in button holder area
        buttonObject.transform.SetParent(buttonHolder.transform, false);

        //buttonObject.GetComponent<Button>().interactable = false;

        //maybe not use the encounter button values for this (is less of a hassle)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            buttonObject.GetComponent<Button>().interactable = true;
        }

        //update button tooltip (if theres info to update)
        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();
    }

    //for new focus system
    public void UpdateSkillCheckTooltips()
    {
        for (int i = 0; i < buttonHolder.transform.childCount; i++)
        {
            if (buttonHolder.transform.GetChild(i).GetComponent<EncounterButton>().isSkillCheck.Length > 0)
            {
                buttonHolder.transform.GetChild(i).GetComponent<EncounterButton>().UpdateTooltip();
            }
        }
    }

    public void EnemyInfoButton()
    {
        if (enemyStatsDisplay.activeSelf)
        {
            enemyStatsDisplay.SetActive(false);

            //combatPaused = false;
            //pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=1> Unpaused";
            //pauseOverlay.SetActive(false);
        }
        else
        {
            enemyStatsDisplay.SetActive(true);
            //run the foe display script
            foeCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForEnemyCards();

            //combatPaused = true;
            //pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=0> Paused";
            //pauseOverlay.SetActive(true);
        }
    }

    //tests whether foe has certain card
    public bool CheckFoeAbility(int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < foeCardArea.transform.childCount; i++)
        {
            if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //need to add active check here for v91
                if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber && foeCardArea.transform.GetChild(i).gameObject.activeSelf == true)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether foe has certain card
    public bool CheckAbilityFromAnyFoe(int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < foeCardArea.transform.childCount; i++)
        {
            if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //need to add active check here for v91
                if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether certain foe has certain card
    public bool CheckAbilityFromSpecificFoe(int foeNumber, int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < foeCardArea.transform.childCount; i++)
        {
            if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //need to add active check here for v91
                if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber && 
                    foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == foeNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //tests whether all foe have certain card
    //used for slow check atm
    public bool CheckIfAllFoesHaveSpecificAbility(int effectNumber)
    {
        int numberOfFoes = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes;
        int cardsFound = 0;

        //tests if player has passive of that effect number
        for (int i = 0; i < foeCardArea.transform.childCount; i++)
        {
            if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //need to add active check here for v91
                if (foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    cardsFound += 1;
                }
            }
        }

        if(cardsFound == numberOfFoes)
        {
            return true;
        }

        //if not, returns false
        return false;
    }

    public void CombatPauseButton()
    {
        //dont do anything if enemy stats display is active
        //also pausebutton must be active
        if (enemyStatsDisplay.activeSelf == false && pauseButton.activeSelf)
        {
            /* this shouldnt do anything now? (in v92)
            if (combatPaused == false)
            {
                combatPaused = false;
                pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=0> Paused";
                pauseOverlay.SetActive(true);
            }
            */
            if (combatPaused == true)
            {
                combatPaused = false;
                pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=1> Unpaused";
                pauseOverlay.SetActive(false);
                pauseButton.SetActive(false);

                GameManager.ins.references.GetComponent<SliderController>().timerOn = true;

                GameManager.ins.references.targettingHandler.RestartAfterDelayContinued();
            }
        }
    }

    public void BattlefieldCombatPauseButton()
    {
        //also pausebutton must be active
        if (realTimeCombatPaused == false)
        {
            realTimeCombatPaused = true;
            battlefieldPauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=0> Paused";
            return;
            //battlefieldPauseButton.SetActive(true);
        }
        if (realTimeCombatPaused == true)
        {
            realTimeCombatPaused = false;
            battlefieldPauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=1> Unpaused";
            return;

            //pauseOverlay.SetActive(false);
            //pauseButton.SetActive(false);
            //GameManager.ins.references.GetComponent<SliderController>().timerOn = true;
            //GameManager.ins.references.targettingHandler.RestartAfterDelayContinued();
        }
    }

    public void ReduceBattleSpeed()
    {
        currentBattleSpeed -= 0.5f;

        if (currentBattleSpeed == 0.5f)
        {
            reduceBattleSpeedButton.enabled = false;
            increaseBattleSpeedButton.enabled = true;
        }
        if (currentBattleSpeed == 1f)
        {
            reduceBattleSpeedButton.enabled = true;
            increaseBattleSpeedButton.enabled = true;
        }
        if (currentBattleSpeed == 1.5f)
        {
            reduceBattleSpeedButton.enabled = true;
            increaseBattleSpeedButton.enabled = false;
        }

        int percentage = (int)(currentBattleSpeed * 100);
        battleSpeedtext.text = percentage.ToString() + "%";
    }

    public void IncreaseBattleSpeed()
    {
        currentBattleSpeed += 0.5f;

        if (currentBattleSpeed == 0.5f)
        {
            reduceBattleSpeedButton.enabled = false;
            increaseBattleSpeedButton.enabled = true;
        }
        if (currentBattleSpeed == 1f)
        {
            reduceBattleSpeedButton.enabled = true;
            increaseBattleSpeedButton.enabled = true;
        }
        if (currentBattleSpeed == 1.5f)
        {
            reduceBattleSpeedButton.enabled = true;
            increaseBattleSpeedButton.enabled = false;
        }

        int percentage = (int)(currentBattleSpeed * 100);
        battleSpeedtext.text = percentage.ToString() + "%";
    }


    //these are called from the slider controller
    public void StartCombatTimer()
    {
        if (PlayerPrefs.GetInt("PauseOn") == 1)
        {
            pauseButton.SetActive(true);
            combatPaused = true;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "<sprite=\"stop n play\" index=0> Paused";

            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == true)
            {
                pauseOverlay.SetActive(false);
            }
            else
            {
                pauseOverlay.SetActive(true);
            }
        }
        else
        {
            pauseButton.SetActive(false);
            combatPaused = false;
            pauseOverlay.SetActive(false);
        }
    }

    public void RemoveCombatTimer()
    {
        pauseButton.SetActive(false);
        pauseOverlay.SetActive(false);
    }

    //for v95
    //not certain if this is the best option
    public void DisableEncounterButtons()
    {
        int buttonCount = buttonHolder.transform.childCount;

        for (int i = 0; i < buttonCount; i++)
        {
            buttonHolder.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
    }
}
