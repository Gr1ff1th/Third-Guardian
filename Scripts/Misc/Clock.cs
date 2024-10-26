using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Clock : MonoBehaviour
{
    public PhotonView PV;

    //singleton
    public static Clock clock;

    public int turnNumber = 0;

    public int totalTurnsPlayed = 0;

    public string agentCollections;

    public GameObject clockHand;
    public GameObject dayToken;
    public GameObject nightToken;

    public GameObject dayChangeInformation;
    public TextMeshProUGUI dayChangeInformationText;

    //various flag variables
    public bool isNight;
    public bool steeleInPlay;
    public bool ulrimanInPlay;
    public bool isHarshSentencing;
    public bool eventDisplayActive;

    //this shouldnt be needed anymore?
    //public bool turmoilJustEnded;

    //timers for events
    public int isStorming;
    public int isDayOfTurmoil;
    public int isErupting;
    public int isGateEngineMalfunction;

    // internal positions
    public List<GameObject> handSlots = new List<GameObject>();

    // the image you want to fade, assign in inspector
    public Image nightImage;

    //water planes
    public GameObject dayWater;
    public GameObject nightWater;

    //night board canvas, for toggling onn/off
    public GameObject nightCanvas;

    public GameObject questPlateHandler;

    public string eruptionMessage;

    //canvases to close via turn timer ending
    public List<GameObject> canvasList = new List<GameObject>();
    public List<GameObject> abCanvasList = new List<GameObject>();

    public bool day2Started;
    public bool day3Started;

    public bool citadelEntered;

    public bool isZaarinsMessage;

    public bool Zaarin3MessageGiven;

    // Start is called before the first frame update
    void Awake()
    {
        //set singleton
        clock = this;
    }

    // when started
    void Start()
    {
        PV = GetComponent<PhotonView>();

        //isNight = false;
        steeleInPlay = false;
        ulrimanInPlay = false;
        isStorming = 0;
        isDayOfTurmoil = 0;
        isErupting = 0;
        isGateEngineMalfunction = 0;
        isHarshSentencing = false;
        eruptionMessage = "";
        eventDisplayActive = false;

        citadelEntered = false;
        //turmoilJustEnded = false;

    }

    public void MoveHand()
    {
        turnNumber += 1;
        totalTurnsPlayed += 1;

        //dont do these on blue citadel
        //if (GameManager.ins.references.currentMinimap.minimapNumber != 73 && GameManager.ins.references.currentMinimap.minimapNumber != 74)
        if (citadelEntered == false)
        {
            //updates the gamestage variable (when 1/3 is played)
            //looks pretty complicated tho
            if (totalTurnsPlayed * 3 >= GameManager.ins.endTime && day2Started == false)//!(GameManager.ins.gameStage == 3 && GameManager.ins.startingDifficulty == 1) && !(GameManager.ins.gameStage == 4 && GameManager.ins.startingDifficulty == 2)) //GameManager.ins.lateGame == false)
            {
                GameManager.ins.gameStage += 1;
                Debug.Log("Game is at mid stage");

                StoreHandler.ins.DrawStage2Cards();

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //give message
                    string msgs = "First day is over. Encounters become more challenging!";
                    //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                    GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
                }

                //show map info, if not on minimap
                if (GameManager.ins.camera.enabled == true)
                {
                    GameManager.ins.DisplayMapInfo();
                }

                //draws new encounter for certain chokepoints
                DrawStage2OvermapEncounters();

                day2Started = true;

                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(40, 2);

                GameManager.ins.references.sfxPlayer.PlayElectionSuccess1();

                //special case for zaarins message
                //dont need in v95
                //ZaarinsMessage();
            }

            //updates the gamestage variable (when 2/3 is played)
            if (totalTurnsPlayed * 3 >= GameManager.ins.endTime * 2 && day3Started == false)//!(GameManager.ins.gameStage == 4 && GameManager.ins.startingDifficulty == 1) && !(GameManager.ins.gameStage == 5 && GameManager.ins.startingDifficulty == 2))
            {
                GameManager.ins.gameStage += 1;
                Debug.Log("Game is at late stage");

                StoreHandler.ins.DrawStage3Cards();

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //give message
                    string msgs = "The second day is over. Encounters become more challenging!";
                    //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                    GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
                }

                //show map info, if not on minimap
                if (GameManager.ins.camera.enabled == true)
                {
                    GameManager.ins.DisplayMapInfo();
                }

                //draws new encounter for certain chokepoints
                DrawStage3OvermapEncounters();

                day3Started = true;

                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(40, 2);
                GameManager.ins.references.sfxPlayer.PlayElectionSuccess1();

            }

            //ends game if total turns played equal endtime setting
            if (totalTurnsPlayed >= GameManager.ins.endTime)
            {
                //GameManager.ins.finalWindows.GetComponent<FinishHandler>().CalculateScore();

                if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().isTutorial == true)
                {
                    //starts score music
                    GameObject.Find("SFX Player").GetComponent<SoundManager>().PlayScoreMusic(1);

                    GameManager.ins.dialogCanvas.GetComponent<CanvasController>().TutorialEndPrompt.SetActive(true);
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().darkOverlay.SetActive(true);
                    return;
                }

                else
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

                    //lets use the epiloguewindow now
                    GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenEpiloqueWindow();

                    //starts defeat music
                    GameObject.Find("SFX Player").GetComponent<SoundManager>().PlayScoreMusic(6);
                    return;
                }
            }

            //update the turns left counter
            //GameManager.ins.turnsLeftText.text = "GAME ENDS\n" + (GameManager.ins.endTime - totalTurnsPlayed) + " <sprite index=25>";
            GameManager.ins.turnsLeftText.text = totalTurnsPlayed + "/" + GameManager.ins.endTime + " <sprite index=25>";
        }

        else if (citadelEntered == true)
        {
            GameManager.ins.turnsLeftText.text = "--/-- <sprite index=25>";
        }
        

        //starts night
        if (turnNumber > 5 && isNight == false)
        {
            dayToken.SetActive(false);
            nightToken.SetActive(true);

            isNight = true;

            turnNumber = 0;

            if (ulrimanInPlay == false)
            {
                NightStart();

                if (GameManager.ins.references.currentMinimap != null)
                {
                    if (GameManager.ins.references.currentMinimap.gameObject.activeSelf)
                    {
                        GameManager.ins.references.currentMinimap.NightStart();
                    }
                }
            }

            //for v90
            //ReduceAllInteractionCosts();

            //starts night music
            //GameObject.Find("SFX Player").GetComponent<SoundManager>().PlayNightMusic2();
            GameManager.ins.references.soundManager.PlayNightMusic2();
        }

        //starts day
        else if (turnNumber > 5 && isNight == true && totalTurnsPlayed < GameManager.ins.endTime)
        {
            dayToken.SetActive(true);
            nightToken.SetActive(false);

            isNight = false;

            turnNumber = 0;

            //only do these if ulriman is not on play
            if (ulrimanInPlay == false)
            {
                //starts day music
                //GameObject.Find("SFX Player").GetComponent<SoundManager>().PlayDayMusic2();
                GameManager.ins.references.soundManager.PlayDayMusic2();

                //fades the map in
                DayStart();

                if (GameManager.ins.references.currentMinimap != null)
                {
                    if (GameManager.ins.references.currentMinimap.gameObject.activeSelf)
                    {
                        GameManager.ins.references.currentMinimap.DayStart();
                    }
                }
            }

            //need to update plates somewhere
            UpdateEncounterPlates();

            //for v90
            //ReduceAllInteractionCosts();

            //additional information plate
            //dayChangeInformation.SetActive(true);
            //dayChangeInformationText.text = "<size=16><color=#ffffff>New Day</color></size>\n<size=8>\n</size>" + GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + "s turn is skipped.\nArtifact offer refreshes.\n"; // + agentCollections;

            //advance turnnumber by additional 1, so that one player doesnt get to be first indefinitely
            //this allows turnnumber go over the player number however
            if (GameManager.ins.turnNumber < GameManager.ins.numberOfPlayers)
            {
                GameManager.ins.turnNumber += 1;

                if (GameManager.ins.turnNumber == GameManager.ins.numberOfPlayers)
                {
                    GameManager.ins.turnNumber = 0;
                }
            }

            /* dont rly need any of this in v91?
             * 
             * actually lets do these here
            //move clock hand
            MoveHand2(turnNumber);

            //should always go here, so check kinda unnecessary
            if (turnNumber == 0 && isNight == false)
            {
                //leaves the node
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

                //hide the ok button for others except current player
                //GameManager.ins.eventCanvas.GetComponent<EventHandler>().eventOkDisplay.SetActive(false);
                GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventOkButton.interactable = false;

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //GameManager.ins.eventCanvas.GetComponent<EventHandler>().eventOkDisplay.SetActive(true);
                    GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventOkButton.interactable = true;
                }

                //actualy we need to synchronize action points now
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints;//16;

                //this is kinda pointless now
                ReduceTimers();
            }

            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();

            //return;
            */
        }

        //move clock hand
        //clockHand.transform.position = handSlots[turnNumber].transform.position;
        MoveHand2(turnNumber);

        //this method is used to countdown various other timers
        //note that this is now on 3 separate places, tactically positioned :-)
        //this is because the sleeptest might end turn prematurely otherwise
        //not used atm, but might need later if we add some global timed effects?
        ReduceTimers();

        //actualy we need to synchronize action points now
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxActionPoints;

        //doesnt need PV check here, since theres one at start of called method
        //this is located here now, instead of updatetimer method in charcontroller
        //need special case for certain event here
        if (isZaarinsMessage == false)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StartTurn();
        }
    }


    //reduces all interaction costs, if theyre higher than 0
    //not used anymore after v90
    public void ReduceAllInteractionCosts()
    {
        for(int i = 0; i < GameManager.ins.references.nodes.Count; i++)
        {
            if (i != 0)
            {
                GameManager.ins.references.nodes[i].ReduceInteractionCost();
            }
        }
    }

    //used for day changes
    public void UpdateEncounterPlates()
    {
        for (int i = 0; i < GameManager.ins.references.nodes.Count; i++)
        {
            if (GameManager.ins.references.nodes[i] != null)
            {
                GameManager.ins.references.nodes[i].UpdateEncounterPlate();
            }
        }
    }


    //new way of moving the clock hand
    public void MoveHand2(int turn)
    {
        for (int i = 0; i < 6; i++)
        {
            if (i == turn)
            {
                handSlots[i].gameObject.SetActive(true);
            }
            else
            {
                handSlots[i].gameObject.SetActive(false);
            }
        }

        if (citadelEntered == false)
        {
            GameManager.ins.turnsLeftText.text = totalTurnsPlayed + "/" + GameManager.ins.endTime + " <sprite index=25>";
        }
        else if(citadelEntered == true)
        {
            GameManager.ins.turnsLeftText.text = "--/-- <sprite index=25>";
        }
    }

    //for moving hand only, doesnt switch players etc
    //used when player triggers second action
    public void MoveHandOnly()
    {
        turnNumber += 1;
        totalTurnsPlayed += 1;

        MoveHand2(turnNumber);
    }

    public void ReduceQuestTimers()
    {
        for (int i = 0; i < questPlateHandler.GetComponent<QuestPlates>().questPlates.Count; i++)
        {
            for (int y = 0; y < questPlateHandler.GetComponent<QuestPlates>().questPlates[i].transform.childCount; y++)
            {
                questPlateHandler.GetComponent<QuestPlates>().questPlates[i].transform.GetChild(y).GetComponent<QuestCard>().ReduceTimer();
            }
        }
    }

    public void ReduceTimers()
    {
        //reducing ward timers
        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            if (GameManager.ins.avatars[i].GetComponent<CharController>().isWarded > 0)
            {
                GameManager.ins.avatars[i].GetComponent<CharController>().isWarded -= 1;
                if (GameManager.ins.avatars[i].GetComponent<CharController>().isWarded == 0)
                {
                    //GameManager.ins.avatars[i].GetComponent<CharController>().RemoveWarding(i);
                }
            }
        }

        //reducing bookkeeping timers
        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            if (GameManager.ins.avatars[i].GetComponent<CharController>().isBookkeeping > 0)
            {
                GameManager.ins.avatars[i].GetComponent<CharController>().isBookkeeping -= 1;
                if (GameManager.ins.avatars[i].GetComponent<CharController>().isBookkeeping == 0)
                {
                    //GameManager.ins.avatars[i].GetComponent<CharController>().RemoveBookkeeping(i);
                }
            }
        }

        //reducing minefield timers
        for (int i = 0; i < GameManager.ins.artifactCardArea.transform.childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).transform.GetComponent<IntelligenceCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).transform.GetComponent<IntelligenceCard>().effect == 34)
                {
                    //reduce timer
                    GameManager.ins.artifactCardArea.transform.GetChild(i).transform.GetComponent<IntelligenceCard>().timer -= 1;

                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).transform.GetComponent<IntelligenceCard>().timer == 0)
                    {
                        int cardNumber = GameManager.ins.artifactCardArea.transform.GetChild(i).transform.GetComponent<CardDisplay>().numberInDeck;
                        GameManager.ins.eventCanvas.GetComponent<EventHandler>().PV.RPC("RPC_RemoveMinefield", RpcTarget.AllBufferedViaServer, cardNumber);
                    }
                }
            }
        }

    }

    public void NightStart()
    {
        // fades the image in when started
        StartCoroutine(FadeImageIn(true));
    }

    public void DayStart()
    {
        // fades the image out when started
        StartCoroutine(FadeImageOut(true));
        //nightCanvas.SetActive(false);
    }

    IEnumerator FadeImageIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            nightCanvas.SetActive(true);

            nightWater.SetActive(true);


            nightImage.color = new Color(0, 0, 0);

            //for colored water
            Color oldColor = nightWater.GetComponent<Renderer>().material.color;

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                nightImage.color = new Color(1, 1, 1, i);

                //Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
                //mat.SetColor("_Color", newColor);

                //fade night water in
                nightWater.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeImageOut(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            //for colored water
            Color oldColor = nightWater.GetComponent<Renderer>().material.color;

            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                nightImage.color = new Color(1, 1, 1, i);

                //fade night water out
                nightWater.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, i);


                yield return null;
            }
        }
    }


    //called from gamemangers update method
    //closes all dialogs and resets all flag variables etc
    public void TurnTimeEnded()
    {
        for(int i = 0; i < canvasList.Count; i++)
        {
            canvasList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < abCanvasList.Count; i++)
        {
            abCanvasList[i].gameObject.SetActive(false);
        }

        //close the extra abcanvas buttons also, in case abcanvas was open
        CloseExtraButtons();

        //force close quest displays, incase they were open
        //GameManager.ins.dialogCanvas.GetComponent<CanvasController>().questCanvas.GetComponent<QuestingDialog>().ForceCloseQuestDisplays();

        //reset the flag variables, just in case
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = 0;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 0;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect = 0;

        //close also all hero & node colliders
        GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().CloseAllHeroColliders();
        GameManager.ins.dialogCanvas.GetComponent<AttackResolve>().CloseEncounterColliders();
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().ResetNodes();
    }

    public void CloseExtraButtons()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().contemplateButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().sleepButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().selfMaintenanceButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().foresightButton.SetActive(false);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().seekQuestButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().researchButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().attemptQuestButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().useIntelligenceButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().investigateButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().smithingButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().scribingButton.SetActive(false);
    }

    //draws stage 2 encounters for certain chokepoitns
    public void DrawStage2OvermapEncounters()
    {
        //bridge south
        GameManager.ins.references.nodes[7].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
        //bridge east
        GameManager.ins.references.nodes[8].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
        //mountain pass south
        GameManager.ins.references.nodes[15].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
        //crossroads NE
        GameManager.ins.references.nodes[29].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
        //mountain pass NE
        GameManager.ins.references.nodes[32].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
        //bridge NE
        GameManager.ins.references.nodes[33].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
        //mountain pass NW
        GameManager.ins.references.nodes[38].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(2);
    }

    //draws stage 3 encounters for certain chokepoitns
    public void DrawStage3OvermapEncounters()
    {
        //bridge south
        GameManager.ins.references.nodes[7].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
        //bridge east
        GameManager.ins.references.nodes[8].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
        //mountain pass south
        GameManager.ins.references.nodes[15].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
        //crossroads NE
        GameManager.ins.references.nodes[29].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
        //mountain pass NE
        GameManager.ins.references.nodes[32].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
        //bridge NE
        GameManager.ins.references.nodes[33].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
        //mountain pass NW
        GameManager.ins.references.nodes[38].gameObject.GetComponent<NodeEncounterHandler>().SpawnNewOvermapEncounters(3);
    }

    //for v95
    public void ZaarinsMessage()
    {
        isZaarinsMessage = true;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.LeaveNode(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        //draw quest
        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 211, 11, 1);

        //starts score music
        GameManager.ins.references.soundManager.GetComponent<SoundManager>().PlayScoreMusic(3);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().eventText.text = "As you rest for a moment from the days events, you have a strange dream..<br><br>Some shrouded, mysterious figure is surrounded by starlight. He speaks in strange accent:<br><br><color=#e0cc9f>\"Strive not for the crown, but for the truth which awaits you at the blue citadel. You can make the difference.\"</color><br><br>You soon wake from your slumber, and have a feeling it was no ordinary dream.";
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().storyImage.sprite = GameManager.ins.references.eventSprites[1];
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().OpenStoryWindow();
    }
}
