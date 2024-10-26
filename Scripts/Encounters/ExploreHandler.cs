using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ExploreHandler : MonoBehaviour
{
    public PhotonView PV;

    //make note that this is changed (has nothing to do with nodenumber anymore)
    //instead stores index number of the location encounter list
    public int locationNumber;
    //this stores index number of the continue "encounter" chosen from the location "encounter"
    //used for explore more button purposes
    public int destinationNumber;

    //stores the interlude encounter number
    //used for exhausting the interlude encounter after the encounter "line" ends
    //needs to be updated after explore more or when destination button is used
    //dont need to update on continue exploring button, since it will give destination button anyway?
    public int interludeNumber;

    //keep track of which encounter display to use
    //not really needed anymore
    public int encounterHandlerToUse;

    //store encounters (for both displays)
    //public Encounter encounterOption1;
    //public Encounter encounterOption2;

    //the dialog for choosing whether to explore or not
    //public GameObject exploreOption;

    //references to the 2 different encounter option handlers
    public EncounterHandler encounterHandler1;
    //public EncounterHandler encounterHandler2;

    public GameObject exploreLocation1;
    //public GameObject exploreLocation2;
    //public GameObject exploreLocation3;

    //public Button Explore1;
    //public Button Explore2;
    //public Button Explore3;

    //public List<GameObject> exploreLocationIcons1;
    //public List<GameObject> exploreLocationIcons2;
    //public List<GameObject> exploreLocationIcons3;

    //public TextMeshProUGUI exploreLocationText1;
    //public TextMeshProUGUI exploreLocationText2;
    //public TextMeshProUGUI exploreLocationText3;

    //main explore location encounters
    //public List<GameObject> exploreLocationEncounters;

    //list of encounter options chosen from explore location encounter (first exploration dialog)
    //public List<GameObject> exploreLocationChoiceButtons;

    //list of encounter options chosen from destination encounter (second encounter dialog, there is only 1 button to choose from in this dialog, besides the leave button)
    //public List<GameObject> destinationButtons;

    //need to unexhaust these at start of game
    public List<GameObject> exhaustableEncounters;

    //need to unexhaust these at start of location
    public List<EncounterButton> permanentlyTakenButtons;

    //references for the undying encounters
    public List<GameObject> undyingEncounters;

    //list of normal locations (similar to explore locations, except you have different options)
    //public List<GameObject> normalLocationEncounters;

    public GameObject cardTakenDialog;
    public Text cardTakenText;

    //used for animations
    public GameObject characterDisplays;

    //for storing info of deleted buttons (for a while)
    public GameObject buttonBackup;

    //use this as reference to an audiosource to replace
    public AudioSource sfxHolder;

    //use this as reference to an ambient audiosource to replace
    public AudioSource encounterMusicHolder;

    public GameObject darkOverlay;

    //need this for undying / second wind checks for now
    public int resurgingFoeNumber;

    //public Button exploreMoreButton;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        //might need to do different kind of encounter reset
        EncounterReset();
    }

    /*no longer used?
    public void ExploreNo()
    {
        exploreOption.SetActive(false);

        PV.RPC("RPC_CancelExplore", RpcTarget.AllBufferedViaServer);
    }
    */

    //no longer used?
    [PunRPC]
    void RPC_CancelExplore()
    {
        /*
        if (GameManager.ins.turnNumber < GameManager.ins.numberOfPlayers)
        {
            GameManager.ins.turnNumber += 1;
        }

        if (GameManager.ins.turnNumber == GameManager.ins.numberOfPlayers)
        {
            GameManager.ins.turnNumber = 0;
        }
        Clock.clock.MoveHand();
        */
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();

        //maybe also reset pathways variable here
        //hasPathways = false;

        //could enable all character canvases here, in case something was left invisible
        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            GameManager.ins.avatars[i].GetComponentInChildren<Canvas>().enabled = true;
        }
    }

    /*old system
    public void ExploreYes()
    {
        exploreOption.SetActive(false);

        //reduce 1 energy
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);

        PV.RPC("RPC_ExploreOptions", RpcTarget.AllBufferedViaServer);
    }
    */

    //do we need this?
    //for first explore dialog
    public void FirstExploreChosen()
    {
        //exploreOption.SetActive(false);

        //PV.RPC("RPC_CancelExplore3", RpcTarget.AllBufferedViaServer);

        //keep track of this
        //locationOptionChosen = 1;

        //open hero animation
        //GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 12);

        //simply use the location number to open appropriate encounter options
        //SetLocationEncounter();

    }

    //might want to delete this soon
    #region old explore buttons
    /*for explore location option 1 chosen on the explore dialog
    public void ExploreChosen1()
    {
        //exploreOption.SetActive(false);

        PV.RPC("RPC_CancelExplore3", RpcTarget.AllBufferedViaServer);

        //keep track of this
        locationOptionChosen = 1;

        //open hero animation
        //GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 12);

        //smithy first option (smithy itself)
        if (locationNumber == 1)
        {
            //60% chance from habitat list, 40% chance from smithy list
            int listChoice = Random.Range(1, 11);

            if (listChoice <= 6)
            {
                //can get the encounter number from either handler
                int encounterNumber = encounterHandler1.GetEncounter(10);
                SetEncounter(encounterHandlerToUse, 10, encounterNumber);
            }

            if (listChoice > 6)
            {
                int encounterNumber = encounterHandler1.GetEncounter(20);
                SetEncounter(encounterHandlerToUse, 20, encounterNumber);
            }
        }

        //wilforge (only habitat for now)
        if (locationNumber == 3)
        {
            int encounterNumber = encounterHandler1.GetEncounter(10);
            SetEncounter(encounterHandlerToUse, 10, encounterNumber);
        }

        //oldmines (add oldmines later)
        if (locationNumber == 4)
        {
            int encounterNumber = encounterHandler1.GetEncounter(12);
            SetEncounter(encounterHandlerToUse, 12, encounterNumber);
        }

        //blue citadel
        if (locationNumber == 8)
        {
            int encounterNumber = encounterHandler1.GetEncounter(28);
            SetEncounter(encounterHandlerToUse, 28, encounterNumber);
        }
    }

    //for explore location option 2 chosen on the explore dialog
    public void ExploreChosen2()
    {
        //exploreOption.SetActive(false);

        PV.RPC("RPC_CancelExplore3", RpcTarget.AllBufferedViaServer);

        //keep track of this
        locationOptionChosen = 2;

        //open hero animation
        //GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 12);

        //smithy shore (add shore later)
        if (locationNumber == 1)
        {
            int encounterNumber = encounterHandler1.GetEncounter(1);
            SetEncounter(encounterHandlerToUse, 1, encounterNumber);
        }

        //inn shore (add shore later)
        if (locationNumber == 2)
        {
            int encounterNumber = encounterHandler1.GetEncounter(1);
            SetEncounter(encounterHandlerToUse, 1, encounterNumber);
        }

        //wilforge plains (add habitat later)
        if (locationNumber == 3)
        {
            int encounterNumber = encounterHandler1.GetEncounter(1);
            SetEncounter(encounterHandlerToUse, 1, encounterNumber);
        }

        //oldmine mountains
        if (locationNumber == 4)
        {
            int encounterNumber = encounterHandler1.GetEncounter(3);
            SetEncounter(encounterHandlerToUse, 3, encounterNumber);
        }

        //factory underground
        if (locationNumber == 5)
        {
            int encounterNumber = encounterHandler1.GetEncounter(12);
            SetEncounter(encounterHandlerToUse, 12, encounterNumber);
        }

        //temple underground
        if (locationNumber == 6)
        {
            int encounterNumber = encounterHandler1.GetEncounter(12);
            SetEncounter(encounterHandlerToUse, 12, encounterNumber);
        }

        //brevir fort (add the rest later)
        if (locationNumber == 7)
        {
            int encounterNumber = encounterHandler1.GetEncounter(12);
            SetEncounter(encounterHandlerToUse, 12, encounterNumber);
        }

        //citadel mountains
        if (locationNumber == 8)
        {
            int encounterNumber = encounterHandler1.GetEncounter(3);
            SetEncounter(encounterHandlerToUse, 3, encounterNumber);
        }

        //vault forest
        if (locationNumber == 9)
        {
            int encounterNumber = encounterHandler1.GetEncounter(2);
            SetEncounter(encounterHandlerToUse, 2, encounterNumber);
        }

        //coven graveyard
        if (locationNumber == 10)
        {
            //50% chance from forest list, 50% chance from graveyard list
            int listChoice = Random.Range(1, 11);

            if (listChoice <= 5)
            {
                int encounterNumber = encounterHandler1.GetEncounter(2);
                SetEncounter(encounterHandlerToUse, 2, encounterNumber);
            }
            if (listChoice > 5)
            {
                int encounterNumber = encounterHandler1.GetEncounter(31);
                SetEncounter(encounterHandlerToUse, 31, encounterNumber);
            }
        }

        //valley mountains
        if (locationNumber == 11)
        {
            int encounterNumber = encounterHandler1.GetEncounter(3);
            SetEncounter(encounterHandlerToUse, 3, encounterNumber);
        }

        //guildhouse underground
        if (locationNumber == 12)
        {
            int encounterNumber = encounterHandler1.GetEncounter(12);
            SetEncounter(encounterHandlerToUse, 12, encounterNumber);
        }

        //cornville farmlands (add farmlands later)
        if (locationNumber == 13)
        {
            int encounterNumber = encounterHandler1.GetEncounter(1);
            SetEncounter(encounterHandlerToUse, 1, encounterNumber);
        }
    }


    //for explore location option 3 chosen on the explore dialog
    public void ExploreChosen3()
    {
        //exploreOption.SetActive(false);

        PV.RPC("RPC_CancelExplore3", RpcTarget.AllBufferedViaServer);

        //keep track of this
        locationOptionChosen = 3;

        //open hero animation
        //GameManager.ins.questCanvas.GetComponent<QuestingDialog>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 12);

        //factory mountains
        if (locationNumber == 5)
        {
            int encounterNumber = encounterHandler1.GetEncounter(3);
            SetEncounter(encounterHandlerToUse, 3, encounterNumber);
        }

        //temple plains
        if (locationNumber == 6)
        {
            int encounterNumber = encounterHandler1.GetEncounter(1);
            SetEncounter(encounterHandlerToUse, 1, encounterNumber);
        }
        //grimhold forest
        if (locationNumber == 7)
        {
            int encounterNumber = encounterHandler1.GetEncounter(2);
            SetEncounter(encounterHandlerToUse, 2, encounterNumber);
        }
        //coven forest
        if (locationNumber == 10)
        {
            int encounterNumber = encounterHandler1.GetEncounter(2);
            SetEncounter(encounterHandlerToUse, 2, encounterNumber);
        }
        //guildhouse forest
        if (locationNumber == 12)
        {
            int encounterNumber = encounterHandler1.GetEncounter(2);
            SetEncounter(encounterHandlerToUse, 2, encounterNumber);
        }
    }
    */
    #endregion

    /*
    [PunRPC]
    void RPC_CancelExplore3()
    {
        exploreLocation1.SetActive(false);
        exploreLocation2.SetActive(false);
        exploreLocation3.SetActive(false);
    }
    */

    //for v0.5.7.
    public void RemoveOneFoe(int foeNumber)
    {
        ResetEncounterVariables(false);

        //need to add functionality to remove also secondary foes from battle
        //actually need to rework this method for v0.5.7.
        RemoveEncounter(foeNumber, true);
    }

    //closes option canvases and ends turn
    //also closes the hero animation
    //used always when exploring ends one way or another?
    //lets remove the rpc call from here
    //[PunRPC]
    public void CancelExplore4()
    {
        Debug.Log("cancel4, numberOfFoes: " + GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes);

        //special case for multi-combat (continue combat)
        //also takes into account if foe was "defeated" due to previous skillcheck (special case for rogues)
        //added special case for foe-fleebutton here too (effect 35)
        //actually this now only handles opponents defeated by skillcheck
        if ((GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1 &&
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true) ||//GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true) ||
            (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1 &&
            encounterHandler1.buttonChosen2.specialEffectOnSuccess == 35))
        {
            Debug.Log("should remove foe");

            ResetEncounterVariables(false);

            //need to add functionality to remove also secondary foes from battle
            RemoveEncounter(0, false);

            //this method name is misleading
            //RemoveFoeFromBattleOnly();

            Debug.Log("should remove foenumber: " + GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);
            return;
        }

        //special case for putting foe out of battle, but keeping them in map (if theres more than 1 foe)
        //effect 36 is for when button directly goes here, effect 37 is for cases where theres second step for leaving encounter (sets removeFoeFromBattleOnly)
        if ((GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1 &&
            encounterHandler1.buttonChosen2.specialEffectOnSuccess == 36) ||
            (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1 &&
            gameObject.GetComponent<CombatHandler>().removeFoeFromBattleOnly == true))
        {
            ResetEncounterVariables(false);

            RemoveFoeFromBattleOnly();
            return;
        }

        //do these for final foes, or non-combat encounters, or when hero is knocked out
        if ((GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes < 2) ||
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == false || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == true)
        {
            //for v0.5.7.
            //need to remove old combat buttons
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveCombatCards();

            encounterHandler1.encounterDisplay.SetActive(false);
            encounterHandler1.encounterOptions.SetActive(false);
            GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(false);
            //encounterHandler2.encounterDisplay.SetActive(false);
            //exploreLocation2.SetActive(false);

            darkOverlay.SetActive(false);

            //stop ambient sfx
            //encounterMusicHolder.Stop();

            //return music volume back to normal, after some delay
            //Invoke("ChangeDayMusicVolAfterDelay", 4f);
            //this method is at line 4370?
            //Invoke("PlayLocationMusic", 0f);

            PlayLocationMusic();

            //should remove small icons
            gameObject.GetComponent<MultiCombat>().RemoveSmalIcons();

            //GameManager.ins.references.soundManager.ChangeMusicVolume(15f, 1f);

            //close plus button too
            //exploreMoreButton.gameObject.SetActive(false);

            //identify this nodes encounters
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>() != null)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().IdentifyEncounters();
            }

            //closes animation (need to call only once)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                PV.RPC("RPC_HideHeroAnimation", RpcTarget.AllBufferedViaServer);

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == true)
                {
                    CardHandler.ins.SetUsables(2);
                }
                else
                {
                    CardHandler.ins.SetUsables(1);
                }

                //might as well close all displays for now
                GameManager.ins.uiButtonHandler.CloseAllDisplays();
            }

            //counts total available encounters for current heros location (not used for v90)
            //CountTotalAvailableEncounters(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber);

            //cancel this flag variable before turn end, for leave button only
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = false;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();


            //could enable all character canvases here, in case something was left invisible
            for (int i = 0; i < GameManager.ins.avatars.Count; i++)
            {
                GameManager.ins.avatars[i].GetComponentInChildren<Canvas>().enabled = true;
            }

            //removes encounter if one of these are true
            //this should make the prvious methods in additional effect unnecessary?
            /*shouldnt remove encounter if hero is defeated, but all of the above should work still?
            if (encounterHandler1.buttonChosen2.removeEncounterOnSuccess && encounterHandler1.skillCheckSuccess == true && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == false)
            {
                RemoveEncounter();
            }

            else if (encounterHandler1.buttonChosen2.removeEncounterOnFailure && encounterHandler1.skillCheckSuccess == false && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == false)
            {
                RemoveEncounter();
            }
            */

            //remove defeated singular foes
            //removes fleeing singular foes also
            if ((GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true) ||
                encounterHandler1.buttonChosen2.specialEffectOnSuccess == 35)
            {
                RemoveEncounter(0, false);
            }

            //put this here, just in case
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ClearAllFoeAbilities();
        }
    }

    //when going back to overmap
    public void ChangeToMapMusic()
    {
        encounterMusicHolder.clip = null;

        Invoke("ChangeDayMusicVolAfterDelay", 1f);
    }

    void ChangeDayMusicVolAfterDelay()
    {

        //only bring the music back if hero is not dead
        if (encounterMusicHolder.isPlaying == false && GameManager.ins.references.soundManager.scoreMusic.isPlaying == false)// || encounterMusicHolder.volume < 0.2)
        {
            GameManager.ins.references.soundManager.mainMusicHolder.volume = 0;
            GameManager.ins.references.soundManager.mainMusicHolder.Play();
            GameManager.ins.references.soundManager.ChangeMusicVolume(3f, 1f);
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
        {
            GameManager.ins.references.soundManager.mainMusicHolder.Stop();
            GameManager.ins.references.soundManager.encounterMusicHolder.Stop();
        }
    }

    //closes option canvases but doesnt end turn
    //also closes the hero animation
    [PunRPC]
    void RPC_CancelExplore5()
    {
        encounterHandler1.encounterDisplay.SetActive(false);
        encounterHandler1.encounterOptions.SetActive(false);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(false);
        //encounterHandler2.option1.SetActive(false);

        //close plus button too
        //exploreMoreButton.gameObject.SetActive(false);

        //closes animation (need to call only once)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_HideHeroAnimation", RpcTarget.AllBufferedViaServer);
        }
    }

    //closes option canvases but doesnt end turn
    //doesnt close hero animation
    [PunRPC]
    void RPC_CancelExplore6()
    {
        encounterHandler1.encounterDisplay.SetActive(false);
        encounterHandler1.encounterOptions.SetActive(false);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(false);
        //encounterHandler2.option1.SetActive(false);

        //close plus button too
        //exploreMoreButton.gameObject.SetActive(false);
    }

    //closes option canvases and ends turn
    //also closes the hero animation
    //used when ending explore or normal interaction
    //if boolean set to true, costs action point
    //no music change
    [PunRPC]
    void RPC_CancelExplore7(bool costsAP)
    {
        encounterHandler1.encounterDisplay.SetActive(false);
        encounterHandler1.encounterOptions.SetActive(false);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(false);
        //encounterHandler2.encounterDisplay.SetActive(false);
        //exploreLocation2.SetActive(false);

        darkOverlay.SetActive(false);

        //stop ambient sfx
        //encounterMusicHolder.Stop();

        //return music volume back to normal, after some delay
        //Invoke("ChangeDayMusicVolAfterDelay", 4f);
        //GameManager.ins.references.soundManager.ChangeMusicVolume(15f, 1f);

        //close plus button too
        //exploreMoreButton.gameObject.SetActive(false);

        //closes animation (need to call only once)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_HideHeroAnimation", RpcTarget.AllBufferedViaServer);

            //allow overmap buttons
            if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == true)
            {
                CardHandler.ins.SetUsables(2);
            }
            else
            {
                CardHandler.ins.SetUsables(1);
            }

            //might as well close all displays for now
            GameManager.ins.uiButtonHandler.CloseAllDisplays();

            //need to "return" to node?
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
        }

        //cancel this flag variable before turn end, for leave button only
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = false;

        if (costsAP == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        }

        //maybe also reset pathways variable here
        //hasPathways = false;

        //could enable all character canvases here, in case something was left invisible
        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            GameManager.ins.avatars[i].GetComponentInChildren<Canvas>().enabled = true;
        }
    }

    //closes option canvases and ends turn
    //also closes the hero animation, and return to previous node
    //used when fleeing from foe or failed on removing obstacles
    [PunRPC]
    void RPC_CancelAndReturn()
    {
        encounterHandler1.encounterDisplay.SetActive(false);
        encounterHandler1.encounterOptions.SetActive(false);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(false);
        //encounterHandler2.encounterDisplay.SetActive(false);
        //exploreLocation2.SetActive(false);

        darkOverlay.SetActive(false);

        //stop ambient sfx
        //encounterMusicHolder.Stop();

        //return music volume back to normal, after some delay
        //Invoke("ChangeDayMusicVolAfterDelay", 4f);
        //this method is at line 4370?
        //Invoke("PlayLocationMusic", 1f);
        PlayLocationMusic();

        //GameManager.ins.references.soundManager.ChangeMusicVolume(15f, 1f);

        //close plus button too
        //exploreMoreButton.gameObject.SetActive(false);

        //closes animation (need to call only once)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_HideHeroAnimation", RpcTarget.AllBufferedViaServer);

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == true)
            {
                CardHandler.ins.SetUsables(2);
            }
            else
            {
                CardHandler.ins.SetUsables(1);
            }

            //might as well close all displays for now
            GameManager.ins.uiButtonHandler.CloseAllDisplays();
        }

        //counts total available encounters for current heros location
        //CountTotalAvailableEncounters(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber);

        //cancel this flag variable before turn end, for leave button only
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = false;
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode.MoveBetweenNodes(GameManager.ins.avatars[GameManager.ins.turnNumber]);

        //identify this nodes encounters
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>() != null)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().IdentifyEncounters();
        }

        //refresh current node, then exhaust previous node encounters
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.RefreshEncounters();
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode.ExhaustEncounters();

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            //takes ViewID of the node the avatar is moving to
            //int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode.gameObject.GetPhotonView().ViewID;
            GameObject sendNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode.gameObject;

            //sends the nodeviewid to charcontrollers method
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, 1, false, true);
        }

        //maybe also reset pathways variable here
        //hasPathways = false;

        //could enable all character canvases here, in case something was left invisible
        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            GameManager.ins.avatars[i].GetComponentInChildren<Canvas>().enabled = true;
        }

        //put this here, just in case
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ClearAllFoeAbilities();
    }

    /* old system
     * shows location icon & text & destination buttons
    public void SetLocationEncounter()
    {
        PV.RPC("RPC_SetLocationEncounter", RpcTarget.AllBufferedViaServer, locationNumber);
    }



    [PunRPC]
    void RPC_SetLocationEncounter(int locationNumberChosen)
    {
        EncounterHandler encounterHandler = encounterHandler1;
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //encounterHandler.encounterDisplay.SetActive(true);
        encounterHandler.encounterOptions.SetActive(true);
        //encounterHandler.doneButton1.gameObject.SetActive(false);

        encounterHandler.encounterOption = exploreLocationEncounters[locationNumberChosen].GetComponent<Encounter2>();

        //set isContinueEncounter flag
        //encounterHandler.isDestinationEncounter = true;
        //encounterHandler.encounterOption.isContinueEncounter = false;

        //play the audioclip of the effect chosen, if there is any
        //otherwise play one of the default sfx
        if (encounterHandler.encounterOption.discoverySfx != null)
        {
            sfxHolder.clip = encounterHandler.encounterOption.discoverySfx;
            sfxHolder.Play();
        }
        else
        {
            //Invoke("PlayEncounterSfx", 0f);
        }
        //handles music changes
        ChangeMusic();

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();

    }
    */

    //for new encounter system for v90
    //set true for first encounter (false for secondary encounters, like extra enemies)
    public void SetStrategicEncounter(bool firstEncounter)
    {
        PV.RPC("RPC_SetStrategicEncounter", RpcTarget.AllBufferedViaServer, firstEncounter);
    }

    [PunRPC]
    void RPC_SetStrategicEncounter(bool firstEncounter)
    {
        EncounterHandler encounterHandler = encounterHandler1;
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        encounterHandler.encounterDisplay.SetActive(true);
        encounterHandler.encounterOptions.SetActive(true);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(true);

        //set encounter phase
        CardHandler.ins.SetUsables(7);

        //could make general method to reset general encounter variables
        ResetEncounterVariables(true);

        /* dont need this anymore
         * show usables
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            GameManager.ins.uiButtonHandler.CloseAllDisplays();
            GameManager.ins.uiButtonHandler.HandCardsButtonPressed();
        }
        */

        /* better do this instantly after first unique encoutner is drawn
         * lets check this here for now
        if (encounterHandler.encounterOption.respawns == false)
        {
            encounterHandler.encounterOption.isTaken = true;
        }
        */

        //display encounter icon
        //encounterHandler.ShowIcon1(encounterHandler.encounterOption);
        encounterHandler.ShowIcon3(encounterHandler.encounterOption.icon);


        //display enemy stats, if there are any
        //enemy stat reset (these are changed for v91)
        //ResetFoeStats();

        //might need to keep this, since it removes the displays, if theres no enemy
        encounterHandler.encounterOption.ShowEnemyStats();

        //this updates starting cooldown now too
        //encounterHandler.encounterOption.NewFoeAbilities();

        //do these only for first encounter drawn (in possible multi-combat encounters)
        if (firstEncounter == true)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().initialPhaseDone = false;

            //should remove small icons by default
            gameObject.GetComponent<MultiCombat>().RemoveSmalIcons();

            //should actually make these fields null in any case
            gameObject.GetComponent<MultiCombat>().ClearFoes();

            //new system for checking and handling multiple units (also works for singular foes)
            if (encounterHandler.encounterOption.isCombatEncounter == true)
            {
                gameObject.GetComponent<MultiCombat>().CheckFoes();
            }
        }

        //lets put the text after the foe reveal (so that enemy swap can take effect)
        encounterHandler.encounter1Text.text = "";

        //set foe icon here also
        if (encounterHandler.encounterOption.isCombatEncounter == true)
        {
            encounterHandler.SetFoeIcon(encounterHandler.encounterOption.icon);

            encounterHandler.encounter1Text.text += "<br><br><br><br><br><br><br><br>";
        }

        //display night icon at night, if there is any
        if (Clock.clock.isNight == true && encounterHandler.encounterOption.nightIcon != null)
        {
            encounterHandler.ShowIcon3(encounterHandler.encounterOption.nightIcon);

            if (encounterHandler.encounterOption.isCombatEncounter == true)
            {
                encounterHandler.SetFoeIcon(encounterHandler.encounterOption.icon);
            }
        }

        //display start text
        encounterHandler.encounter1Text.text += encounterHandler.encounterOption.startText;

        //play the audioclip of the effect chosen, if there is any
        //otherwise play one of the default sfx
        if (encounterHandler.encounterOption.discoverySfx != null)
        {
            sfxHolder.clip = encounterHandler.encounterOption.discoverySfx;
            sfxHolder.Play();
        }
        else if (encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayUniqueSfx", 0.2f);
        }

        else if (encounterHandler.encounterOption.isForcedEncounter)
        {
            Invoke("PlayForcedSfx", 0.2f);
        }

        else if (!encounterHandler.encounterOption.isForcedEncounter && !encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayEncounterSfx", 0f);
        }

        //reset these here
        encounterHandler1.rerollCost = 1;
        encounterHandler1.focusCost = 1;
        encounterHandler1.dontAllowExploreMore = false;

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();

        //modified for v94
        if (firstEncounter == true)// || encounterHandler.encounterOption.isBoss == true)
        {
            //special case for some bosses etc
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.ambientSfxDay != null)
            {
                PlayNodeMusic();
            }

            else
            {
                //bring combat music for combat encounters
                ChangeMusic();
            }
        }
    }

    //store this for now
    #region old set encounter methods
    /*
    public void SetEncounter(int optionWindow, int listType, int encounterNumber)
    {
        PV.RPC("RPC_SetEncounter", RpcTarget.AllBufferedViaServer, optionWindow, listType, encounterNumber);
    }

    [PunRPC]
    void RPC_SetEncounter(int optionWindow, int listType, int encounterNumber)
    {
        EncounterHandler encounterHandler = encounterHandler1;
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        if (optionWindow == 1)
        {
            encounterHandler = encounterHandler1;
        }
        if (optionWindow == 2)
        {
            encounterHandler = encounterHandler2;
        }

        encounterHandler.encounterDisplay.SetActive(true);
        encounterHandler.encounterOptions.SetActive(true);
        encounterHandler.doneButton1.gameObject.SetActive(false);

        //plains
        if (listType == 1)
        {
            //encounterOption1 = plainEncounters[encounterNumber].GetComponent<Encounter>();
            encounterHandler.encounterOption = plainEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //forest
        if (listType == 2)
        {
            encounterHandler.encounterOption = forestEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //mountains
        if (listType == 3)
        {
            encounterHandler.encounterOption = mountainEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //shore
        if (listType == 4)
        {
            encounterHandler.encounterOption = shoreEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //shore
        if (listType == 5)
        {
            encounterHandler.encounterOption = farmlandEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //habitat
        if (listType == 10)
        {
            encounterHandler.encounterOption = habitatEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //mystics
        if (listType == 11)
        {
            encounterHandler.encounterOption = mysticsEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //underground
        if (listType == 12)
        {
            encounterHandler.encounterOption = undergroundEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //stores
        if (listType == 13)
        {
            encounterHandler.encounterOption = storesEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //academy
        if (listType == 14)
        {
            encounterHandler.encounterOption = academyEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //fortress
        if (listType == 15)
        {
            encounterHandler.encounterOption = fortressEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //smithy
        if (listType == 20)
        {
            encounterHandler.encounterOption = smithyEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //inn
        if (listType == 21)
        {
            encounterHandler.encounterOption = innEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //wilforge
        if (listType == 22)
        {
            encounterHandler.encounterOption = wilforgeEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //oldmines
        if (listType == 23)
        {
            encounterHandler.encounterOption = oldminesEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //factory
        if (listType == 24)
        {
            encounterHandler.encounterOption = factoryEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //temple
        if (listType == 25)
        {
            encounterHandler.encounterOption = templeEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //grimhold
        if (listType == 26)
        {
            encounterHandler.encounterOption = grimholdEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //brevir fortress
        if (listType == 27)
        {
            encounterHandler.encounterOption = brevirFortEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //blue citadel
        if (listType == 28)
        {
            encounterHandler.encounterOption = blueCitadelEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //forest vault
        if (listType == 29)
        {
            encounterHandler.encounterOption = forestVaultEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //coven
        if (listType == 30)
        {
            encounterHandler.encounterOption = covenEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //graveyard
        if (listType == 31)
        {
            encounterHandler.encounterOption = graveyardEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //valley
        if (listType == 32)
        {
            encounterHandler.encounterOption = valleyEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //guildhouse
        if (listType == 33)
        {
            encounterHandler.encounterOption = guildhouseEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //cornville
        if (listType == 34)
        {
            encounterHandler.encounterOption = cornvilleEncounters[encounterNumber].GetComponent<Encounter2>();
        }
        //continue encounters
        if (listType == 40)
        {
            encounterHandler.encounterOption = continueEncounters[encounterNumber].GetComponent<Encounter2>();
        }

        //lets check this here for now
        if (encounterHandler.encounterOption.respawns == false)
        {
            encounterHandler.encounterOption.isTaken = true;
        }
    //display encounter icon
    encounterHandler.ShowIcon1(encounterHandler.encounterOption);

        //display start text
        encounterHandler.encounter1Text.text = encounterHandler.encounterOption.startText;

        //set isContinueEncounter flag
        encounterHandler.encounterOption.isContinueEncounter = false;

        //disable buttons for other players, and disable them for starters
        //encounterHandler.option1Button1.interactable = false;
        //encounterHandler.option1Button2.interactable = false;
        //encounterHandler.option1Button3.interactable = false;

        //lets save this here for later reference
        /*show buttons & their text, if the string isnt empty
        if (encounterHandler.encounterOption.optionButtonText1 != "")
        {
            //if not priestess, dont show this option
            if (encounterHandler.encounterOption.specialRequirementButton1 == 1 && character.GetComponentInParent<CharController>().PriestessTest() == false)
            {
                //nothing happens
            }
            else
            {
                encounterHandler.option1Button1.gameObject.SetActive(true);

                encounterHandler.option1Button1.GetComponentInChildren<TextMeshProUGUI>().text = encounterHandler.encounterOption.optionButtonText1;

                //these should actually only be interactable if you can complete the requirement
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //add special requirement check here also, if the option requires specific perk or such
                    if (PassRequirements(1, encounterHandler.encounterOption) == true)
                    {
                        encounterHandler.option1Button1.interactable = true;
                    }
                }
            }
        }
        if (encounterHandler.encounterOption.optionButtonText2 != "")
        {
            //if not priestess, dont show this option
            if (encounterHandler.encounterOption.specialRequirementButton2 == 1 && character.GetComponentInParent<CharController>().PriestessTest() == false)
            {
                //nothing happens
            }
            else
            {
                encounterHandler.option1Button2.gameObject.SetActive(true);

                encounterHandler.option1Button2.GetComponentInChildren<TextMeshProUGUI>().text = encounterHandler.encounterOption.optionButtonText2;

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    if (PassRequirements(2, encounterHandler.encounterOption) == true)
                    {
                        encounterHandler.option1Button2.interactable = true;
                    }
                }
            }
        }
        if (encounterHandler.encounterOption.optionButtonText3 != "")
        {
            //if not priestess, dont show this option
            if (encounterHandler.encounterOption.specialRequirementButton3 == 1 && character.GetComponentInParent<CharController>().PriestessTest() == false)
            {
                //nothing happens
            }
            else
            {
                encounterHandler.option1Button3.gameObject.SetActive(true);

                encounterHandler.option1Button3.GetComponentInChildren<TextMeshProUGUI>().text = encounterHandler.encounterOption.optionButtonText3;

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    if (PassRequirements(3, encounterHandler.encounterOption) == true)
                    {
                        encounterHandler.option1Button3.interactable = true;
                    }
                }
            }
        }

        //show skulls on forced encounters, if its not unique
        //unique icons overwrite the skulls
        //encounterHandler.skulls1.SetActive(false);
        //encounterHandler.unique1.SetActive(false);

        if (encounterHandler.encounterOption.isUniqueEncounter)
        {
            //encounterHandler.skulls1.SetActive(false);
            //encounterHandler.unique1.SetActive(true);

            Invoke("PlayUniqueSfx", 0.2f);
        }

        else if (encounterHandler.encounterOption.isForcedEncounter)
        {
            //encounterHandler.skulls1.SetActive(true);

            Invoke("PlayForcedSfx", 0.2f);
        }

        else if (!encounterHandler.encounterOption.isForcedEncounter && !encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayEncounterSfx", 0f);
        }

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();


        /* dont need these anymore
         * 
        //enable second exploring option, if location was not forced, or if you are immune to forced encounters
        //exploreMoreButton.gameObject.SetActive(true);
        //exploreMoreButton.interactable = false;

        //lets do this for now, although its alrdy done once in most cases
        //encounterHandler1.overlayPanel.SetActive(false);
        //encounterHandler2.overlayPanel.SetActive(false);

        //might need to update these later
        if (!encounterHandler.encounterOption.isForcedEncounter && encounterHandlerToUse != 2)
        {
            exploreMoreButton.interactable = true;
        }
        //put overlaypanel on option 1 if option 2 is forced
        if (encounterHandler.encounterOption.isForcedEncounter && encounterHandlerToUse == 2)
        {
            //exploreMoreButton.interactable = true;
            encounterHandler1.overlayPanel.SetActive(true);
        }
        
    }
    */
    #endregion



    #region old requirement test
    //returns true if you have the requirements for the option
    /*lets not test energy & reputation for now
    public bool PassRequirements(int button, Encounter option)
    {
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        if (button == 1)
        {
            //goes through all the requirements
            for (int y = 0; y < option.requirementTypeButton1.Length; y++)
            {
                /*energy
                if (option.requirementTypeButton1[y] == 0)
                {
                    if (option.requirementQtyButton1[y] > character.energy)
                    {
                        return false;
                    }
                }
                
                //can group the same "type" skills now, since theyre the same thing practically (different animations tho)
                if (option.requirementTypeButton1[y] == 1 || option.requirementTypeButton1[y] == 2)
                {
                    if (option.requirementQtyButton1[y] > (character.strength + character.warriors))
                    {
                        return false;
                    }
                }
                if (option.requirementTypeButton1[y] == 3 || option.requirementTypeButton1[y] == 4)
                {
                    if (option.requirementQtyButton1[y] > (character.mechanics + character.artisans))
                    {
                        return false;
                    }
                }
                if (option.requirementTypeButton1[y] == 5 || option.requirementTypeButton1[y] == 6)
                {
                    if (option.requirementQtyButton1[y] > (character.lore + character.arcanists))
                    {
                        return false;
                    }
                }
                //coins
                if (option.requirementTypeButton1[y] == 7)
                {
                    if (option.requirementQtyButton1[y] > character.coins)
                    {
                        return false;
                    }
                }
            }
        }
        if (button == 2)
        {
            //goes through all the requirements
            for (int y = 0; y < option.requirementTypeButton2.Length; y++)
            {
                //can group the same "type" skills now, since theyre the same thing practically (different animations tho)
                if (option.requirementTypeButton2[y] == 1 || option.requirementTypeButton2[y] == 2)
                {
                    if (option.requirementQtyButton2[y] > (character.strength + character.warriors))
                    {
                        return false;
                    }
                }
                if (option.requirementTypeButton2[y] == 3 || option.requirementTypeButton2[y] == 4)
                {
                    if (option.requirementQtyButton2[y] > (character.mechanics + character.artisans))
                    {
                        return false;
                    }
                }
                if (option.requirementTypeButton2[y] == 5 || option.requirementTypeButton2[y] == 6)
                {
                    if (option.requirementQtyButton2[y] > (character.lore + character.arcanists))
                    {
                        return false;
                    }
                }
                //coins
                if (option.requirementTypeButton2[y] == 7)
                {
                    if (option.requirementQtyButton2[y] > character.coins)
                    {
                        return false;
                    }
                }
            }
        }
        if (button == 3)
        {
            //goes through all the requirements
            for (int y = 0; y < option.requirementTypeButton3.Length; y++)
            {
                //can group the same "type" skills now, since theyre the same thing practically (different animations tho)
                if (option.requirementTypeButton3[y] == 1 || option.requirementTypeButton3[y] == 2)
                {
                    if (option.requirementQtyButton3[y] > (character.strength + character.warriors))
                    {
                        return false;
                    }
                }
                if (option.requirementTypeButton3[y] == 3 || option.requirementTypeButton3[y] == 4)
                {
                    if (option.requirementQtyButton3[y] > (character.mechanics + character.artisans))
                    {
                        return false;
                    }
                }
                if (option.requirementTypeButton3[y] == 5 || option.requirementTypeButton3[y] == 6)
                {
                    if (option.requirementQtyButton3[y] > (character.lore + character.arcanists))
                    {
                        return false;
                    }
                }
                //coins
                if (option.requirementTypeButton3[y] == 7)
                {
                    if (option.requirementQtyButton3[y] > character.coins)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    */
    #endregion

    /* old updateEncounter
    [PunRPC]
    void RPC_UpdateEncounter1(string description, int continueEncounter)
    {
        encounterHandler1.encounter1Text.text = description;

        encounterHandler1.doneButton1.gameObject.SetActive(true);
        encounterHandler1.doneButton1.interactable = false;

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            encounterHandler1.doneButton1.interactable = true;
        }

        if (continueEncounter == 0)
        {
            encounterHandler1.doneButton1.GetComponentInChildren<TextMeshProUGUI>().text = "<size=16>Done";
        }
        if (continueEncounter != 0)
        {
            encounterHandler1.doneButton1.GetComponentInChildren<TextMeshProUGUI>().text = "<size=16>Continue";
        }
        if (continueEncounter == 999)
        {
            encounterHandler1.doneButton1.GetComponentInChildren<TextMeshProUGUI>().text = "<size=16>Explore More";
        }

        //reset the cost arrays
        for (int i = 0; i < encounterHandler1.costType.Length; i++)
        {
            encounterHandler1.costType[i] = 0;
            encounterHandler1.costQty[i] = 0;
        }

        //reset these cost arrays also?
        for (int i = 0; i < encounterHandler2.costType.Length; i++)
        {
            encounterHandler2.costType[i] = 0;
            encounterHandler2.costQty[i] = 0;
        }
    }
    */

    //reset encounters 
    public void EncounterReset()
    {
        /* dont need any of these anymore
        //this is done to check the buttons on the first dialog of the explore option
        for (int i = 0; i < exploreLocationChoiceButtons.Count; i++)
        {
            if (exploreLocationChoiceButtons[i].GetComponent<EncounterButton>().isExhaustedAtStart == true)
            {
                exploreLocationChoiceButtons[i].GetComponent<EncounterButton>().isTaken = true;
            }
        }

        //reset available encounters for each node
        //this sould done after exhausting the "hidden" destinations
        for (int i = 0; i < exploreLocationEncounters.Count; i++)
        {
            if (exploreLocationEncounters[i] != null)
            {
                //not used for v90
                //CountTotalAvailableEncounters(i);
            }
            else if (i != 0)
            {
                //set the nodes encounter display counter to 0 
                GameManager.ins.references.nodes[i].UpdateAvailableEncountersCounter(0);
            }
        }

        //these are button of the second dialog of the explore option
        for (int i = 0; i < destinationButtons.Count; i++)
        {
            destinationButtons[i].GetComponent<EncounterButton>().destinationNumber = i;

            int interludeEncounters = destinationButtons[i].GetComponent<EncounterButton>().continueEncounters.Count;

            //then reset taken flags on all encounters on destinationEncounter continue encounter lists, and give each of those a number as well
            for (int y = 0; y < interludeEncounters; y++)
            {
                destinationButtons[i].GetComponent<EncounterButton>().continueEncounters[y].GetComponent<Encounter2>().isTaken = false;
            }
        }
        */

        //reset the exhaustable encounters, if new game
        if (PhotonRoom.room.spContinue == false)
        {
            for (int i = 0; i < exhaustableEncounters.Count; i++)
            {
                if (exhaustableEncounters[i].GetComponent<Encounter2>() != null)
                {
                    exhaustableEncounters[i].GetComponent<Encounter2>().isTaken = false;
                }
            }
        }
        else 
        {
            for (int i = 0; i < exhaustableEncounters.Count; i++)
            {
                if (exhaustableEncounters[i].GetComponent<Encounter2>() != null)
                {
                    exhaustableEncounters[i].GetComponent<Encounter2>().isTaken = DataPersistenceManager.instance.gameData.uniqueTaken[i];
                }
            }
        }
    }

    //confirm button from card taken dialog
    public void CardTakenConfirmed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        cardTakenDialog.SetActive(false);

        //PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        CancelExplore4();
    }

    void PlayEncounterSfx()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayEncounter();
    }

    void PlayForcedSfx()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlaySkillCheckFail();
    }

    void PlayUniqueSfx()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlaySkillCheckSuccess();
    }

    /* plus button
    public void ExploreMore()
    {
        //exploreMoreButton.interactable = false;

        //encounterHandler2.gameObject.SetActive(true);

        //update this
        encounterHandlerToUse = 2;
        //PV.RPC("RPC_HandlerToUse", RpcTarget.AllBufferedViaServer, 2);

        if (locationOptionChosen == 1)
        {
            ExploreChosen1();
        }
        if (locationOptionChosen == 2)
        {
            ExploreChosen2();
        }
        if (locationOptionChosen == 3)
        {
            ExploreChosen3();
        }
    }
    */

    //updates the handler number to all players
    //actually dunno if we need this
    [PunRPC]
    void RPC_HandlerToUse(int handlerNumber)
    {
        encounterHandlerToUse = handlerNumber;
    }

    /*disables second display and shows encounter info and options on first display
    [PunRPC]
    void RPC_DisableSecondDisplay(int iconNumber)
    {
        encounterHandler2.encounterDisplay.SetActive(false);

        //display encounter icon
        encounterHandler1.ShowIcon2(iconNumber);

        //encounterHandler1.overlayPanel.SetActive(false);
    }
    */

    //Deletes all action option buttons
    [PunRPC]
    void RPC_DeleteButtons()
    {
        int numberOfButtons = encounterHandler1.buttonHolder.gameObject.transform.childCount;

        for (int i = numberOfButtons; i > 0; i--)
        {
            Destroy(encounterHandler1.buttonHolder.transform.GetChild(i - 1).gameObject);
        }
    }

    //lets test without rpc call
    public void DeleteButtons()
    {
        int numberOfButtons = encounterHandler1.buttonHolder.gameObject.transform.childCount;

        for (int i = numberOfButtons; i > 0; i--)
        {
            Destroy(encounterHandler1.buttonHolder.transform.GetChild(i - 1).gameObject);
        }
    }

    /* old stuff
    //need to call this here because the button which calls this gets deleted
    //this needs changing asap
    public void ExploreMoreButton()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke("ExploreMoreContinued", 0.3f);
    }

    void ExploreMoreContinued()
    {
        int newEncounter = RollExploreMore();

        PV.RPC("RPC_SetExploreMoreEncounter", RpcTarget.AllBufferedViaServer, newEncounter);
    }


    public int RollExploreMore()
    {
        List<GameObject> listReference = destinationButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters;

        int i = 0;
        do
        {
            int encounterNumber = Random.Range(0, listReference.Count);
            if (listReference[encounterNumber].GetComponent<Encounter2>().isTaken == false)
            {
                int rarityCheck = Random.Range(1, 11);
                Encounter2 encounter = listReference[encounterNumber].GetComponent<Encounter2>();

                int modifiedGameStage = GameManager.ins.gameStage + encounterHandler1.destinationDifficultyModifier;

                //check time requirements (dont allow invalid encounters through)
                if ((encounter.requirementTime == 1 && Clock.clock.isNight == false) || (encounter.requirementTime == 2 && Clock.clock.isNight == true) || encounter.requirementTime == 0)
                {
                    if (modifiedGameStage == 1 && rarityCheck <= encounter.stage1Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 2 && rarityCheck <= encounter.stage2Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 3 && rarityCheck <= encounter.stage3Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 4 && rarityCheck <= encounter.stage4Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 5 && rarityCheck <= encounter.stage5Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 6 && rarityCheck <= encounter.stage6Rarity)
                    {
                        return encounterNumber;
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //draws new encounter from the destination encounter list
    //similar to setContinueEncounter method
    [PunRPC]
    void RPC_SetExploreMoreEncounter(int newEncounter)
    {
        EncounterHandler encounterHandler = encounterHandler1;
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        encounterHandler.encounterDisplay.SetActive(true);
        encounterHandler.encounterOptions.SetActive(true);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(true);

        //close this in case it was open
        exploreLocation2.SetActive(false);

        //bring back destination music when going out from battle (with combat evading ability)
        //need to do this before new encounter option is drawn
        if (encounterHandler.encounterOption.isCombatEncounter == true && gameObject.GetComponent<CombatHandler>().opponentDefeated == false)
        {
            if (exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay != null && Clock.clock.isNight == false)
            {
                encounterMusicHolder.volume = 0;
                encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay;
                GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDayVolume);
                encounterMusicHolder.Play();

            }
            if (exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNight != null && Clock.clock.isNight == true)
            {
                encounterMusicHolder.volume = 0;
                encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNight;
                GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNightVolume);
                encounterMusicHolder.Play();
            }
        }

        //take encounter from the stored button continue encounter list (interlude encounter in other words)
        encounterHandler.encounterOption = destinationButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[newEncounter].GetComponent<Encounter2>();

        //need to update interlude number here
        interludeNumber = newEncounter;

        //could make general method to reset general encounter variables
        ResetEncounterVariables(false);

        //lets check this here for now
        if (encounterHandler.encounterOption.respawns == false)
        {
            encounterHandler.encounterOption.isTaken = true;
        }

        //display encounter icon
        //encounterHandler.ShowIcon1(encounterHandler.encounterOption);
        encounterHandler.ShowIcon3(encounterHandler.encounterOption.icon);

        encounterHandler.encounter1Text.text = "";

        //set foe icon here also
        if (encounterHandler.encounterOption.isCombatEncounter == true)
        {
            encounterHandler.SetFoeIcon(encounterHandler.encounterOption.icon);

            encounterHandler.encounter1Text.text += "<br><br><br><br><br><br><br><br>";
        }

        //display night icon at night, if there is any
        //this assumes all foes have same icon day & night
        if (Clock.clock.isNight == true && encounterHandler.encounterOption.nightIcon != null && encounterHandler.encounterOption.isCombatEncounter == false)
        {
            encounterHandler.ShowIcon3(encounterHandler.encounterOption.nightIcon);
        }

        //display start text
        encounterHandler.encounter1Text.text += encounterHandler.encounterOption.startText;

        //display enemy stats, if there are any
        //enemy stat reset
        ResetFoeStats();
        encounterHandler.encounterOption.ShowEnemyStats();
        //this updates starting cooldown now too
        encounterHandler.encounterOption.NewFoeAbilities();

        //should be set to false?
        //encounterHandler.encounterOption.isContinueEncounter = false;
        //encounterHandler.encounterOption.dontAllowExploreMore = true;

        //play the audioclip of the effect chosen, if there is any
        //otherwise play one of the default sfx
        if (encounterHandler.encounterOption.discoverySfx != null)
        {
            sfxHolder.clip = encounterHandler.encounterOption.discoverySfx;
            sfxHolder.Play();
        }
        else if (encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayUniqueSfx", 0.2f);
        }

        else if (encounterHandler.encounterOption.isForcedEncounter)
        {
            Invoke("PlayForcedSfx", 0.2f);
        }

        else if (!encounterHandler.encounterOption.isForcedEncounter && !encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayEncounterSfx", 0f);
        }

        //handles music changes
        ChangeMusic();

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();
    }
    */

    //need to call this here because the button which calls this gets deleted
    public void ContinueEncounterButton()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke(nameof(DrawContinueEncounter), 0.4f);
    }

    void DrawContinueEncounter()
    {
        int continueEncounter = RollContinueEncounter();

        PV.RPC(nameof(RPC_SetContinueEncounter), RpcTarget.AllBufferedViaServer, continueEncounter);
    }

    [PunRPC]
    void RPC_SetContinueEncounter(int encounterNumber)
    {
        /* dont think we need these anymore
        //exhaust the current encounter if this flag is set to true on the encounter button
        //do this before new encounteroption is set
        if (encounterHandler1.buttonChosen2.exhaustCurrentEncounterOnSuccess == true)
        {
            //encounterHandler1.encounterOption.isTaken = true;
        }
        //exhaust the interlude encounter if this flag is set to true on the encounter button
        if (encounterHandler1.buttonChosen2.exhaustInterludeOnSuccess == true)
        {
            //use the stored destination & interlude -numbers
            destinationButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[interludeNumber].GetComponent<Encounter2>().isTaken = true;
        }

        //certain encounter options disallow exploring more
        if (encounterHandler1.buttonChosen2.dontAllowExploreMore == true)
        {
            encounterHandler1.dontAllowExploreMore = true;
        }
        */

        //special case for purchase button
        if (encounterHandler1.buttonChosen2.isPurchaseButton == true)
        {
            encounterHandler1.tradingHolder.SetActive(true);
            encounterHandler1.tradingBackButton.SetActive(true);

            //set purchase phase
            CardHandler.ins.SetUsables(6);

            StoreHandler.ins.ShowStoreCards();
        }

        //special case for sell button
        if (encounterHandler1.buttonChosen2.isSellButton == true)
        {
            encounterHandler1.tradingHolder.SetActive(true);
            encounterHandler1.tradingBackButton.SetActive(true);

            //set sell phase
            CardHandler.ins.SetUsables(5);

            StoreHandler.ins.ShowSellableCards();
        }

        EncounterHandler encounterHandler = encounterHandler1;
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        encounterHandler.encounterDisplay.SetActive(true);
        encounterHandler.encounterOptions.SetActive(true);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(true);

        //could make general method to reset general encounter variables
        ResetEncounterVariables(false);

        //close this in case it was open
        //exploreLocation2.SetActive(false);

        //take encounter from the stored button continue encounter list
        encounterHandler.encounterOption = encounterHandler.buttonChosen2.continueEncounters[encounterNumber].GetComponent<Encounter2>();
        //pretty dumb to do this twice, or is it?
        GameManager.ins.references.currentEncounter = encounterHandler.buttonChosen2.continueEncounters[encounterNumber].GetComponent<Encounter2>();

        /*lets check this here for now
        if (encounterHandler.encounterOption.respawns == false)
        {
            encounterHandler.encounterOption.isTaken = true;
        }
        */

        //if (encounterHandler.buttonChosen2.isFirstEncounter == true)

        //display encounter icon
        //encounterHandler.ShowIcon1(encounterHandler.encounterOption);
        encounterHandler.ShowIcon3(encounterHandler.encounterOption.icon);

        //display enemy stats, if there are any
        //enemy stat reset
        ResetFoeStats();
        /*
        encounterHandler.encounterOption.currentEnergy = encounterHandler.encounterOption.maxEnergy;
        encounterHandler.encounterOption.attack = encounterHandler.encounterOption.maxAttack;
        encounterHandler.encounterOption.arcanePower = encounterHandler.encounterOption.maxArcanePower;
        */
        encounterHandler.encounterOption.ShowEnemyStats();

        //draw the abilities if there are any
        encounterHandler.encounterOption.NewFoeAbilities();

        //do these always for continue encounters?
        //if (firstEncounter == true)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().initialPhaseDone = false;

            //should remove small icons by default
            gameObject.GetComponent<MultiCombat>().RemoveSmalIcons();

            //should actually make these fields null in any case
            gameObject.GetComponent<MultiCombat>().ClearFoes();

            //new system for checking and handling multiple units (also works for singular foes)
            if (encounterHandler.encounterOption.isCombatEncounter == true)
            {
                gameObject.GetComponent<MultiCombat>().CheckFoes();
            }
        }

        encounterHandler.encounter1Text.text = "";

        //set foe icon here also
        if (encounterHandler.encounterOption.isCombatEncounter == true)
        {
            encounterHandler.SetFoeIcon(encounterHandler.encounterOption.icon);

            encounterHandler.encounter1Text.text += "<br><br><br><br><br><br><br><br>";
        }

        //display night icon at night, if there is any
        if (Clock.clock.isNight == true && encounterHandler.encounterOption.nightIcon != null && encounterHandler.encounterOption.isCombatEncounter == false)
        {
            encounterHandler.ShowIcon3(encounterHandler.encounterOption.nightIcon);
        }

        //display start text
        encounterHandler.encounter1Text.text += encounterHandler.encounterOption.startText;

        //special case for sell button text (need to come after the initial text is set)
        if (encounterHandler1.buttonChosen2.isSellButton == true)
        {
            //special case for traders (change text here)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(108) == true)
            {
                int tradeBonus = 30 + (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence * 2);

                encounterHandler1.encounter1Text.text += tradeBonus + "% of their value.";
            }
            else
            {
                encounterHandler1.encounter1Text.text += "25% of their value.";
            }
        }

        /*this is done only on the second encounter (so basically destination)
        if (encounterHandler.isDestinationEncounter == true)
        {
            encounterHandler.isDestinationEncounter = false;
            //destinationNumber = encounterHandler.encounterOption.destinationNumber;
        }
        */
        //this should be done though?
        //otherwise the game wont know which interlude we using after destination choice
        if (encounterHandler.buttonChosen2.isDestinationButton == true)
        {
            //encounterHandler.isDestinationEncounter = false;
            //encounterHandler.encounterOption.dontAllowExploreMore = true;

            //update interlude number here
            interludeNumber = encounterNumber;
        }
        else
        {
            //encounterHandler.encounterOption.isContinueEncounter = true;
            //encounterHandler.encounterOption.dontAllowExploreMore = false;
        }

        //play the audioclip of the effect chosen, if there is any
        //otherwise play one of the default sfx
        if (encounterHandler.encounterOption.discoverySfx != null)
        {
            sfxHolder.clip = encounterHandler.encounterOption.discoverySfx;
            sfxHolder.Play();
        }
        else if (encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayUniqueSfx", 0.2f);
        }

        else if (encounterHandler.encounterOption.isForcedEncounter)
        {
            Invoke("PlayForcedSfx", 0.2f);
        }

        else if (!encounterHandler.encounterOption.isForcedEncounter && !encounterHandler.encounterOption.isUniqueEncounter)
        {
            Invoke("PlayEncounterSfx", 0f);
        }
        /* 
        if (encounterHandler.encounterOption.ambientSfxDay != null && Clock.clock.isNight == false)
        {
            encounterMusicHolder.clip = encounterHandler.encounterOption.ambientSfxDay;
            encounterMusicHolder.volume = encounterHandler.encounterOption.ambientSfxDayVolume;
            GameManager.ins.references.soundManager.ChangeMusicVolume(3f, encounterHandler.encounterOption.musicVolume);
            encounterMusicHolder.Play();
        }
        if (encounterHandler.encounterOption.ambientSfxNight != null && Clock.clock.isNight == true)
        {
            encounterMusicHolder.clip = encounterHandler.encounterOption.ambientSfxNight;
            encounterMusicHolder.volume = encounterHandler.encounterOption.ambientSfxNightVolume;
            GameManager.ins.references.soundManager.ChangeMusicVolume(3f, encounterHandler.encounterOption.musicVolume);
            encounterMusicHolder.Play();
        }
        */
        //handles music changes
        ChangeMusic();

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();

    }

    /* old stuff
    //for redrawing destination encounter, by continue exploring button
    //need to call this here because the button which calls this gets deleted
    public void SetDestinationEncounter()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke("DelayedSetDestinationEncounter", 0.4f);
    }

    void DelayedSetDestinationEncounter()
    {
        PV.RPC("RPC_SetDestinationEncounter", RpcTarget.AllBufferedViaServer);
    }

    //for redrawing destination encounter, by continue exploring button
    [PunRPC]
    public void RPC_SetDestinationEncounter()
    {
        EncounterHandler encounterHandler = encounterHandler1;
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        encounterHandler.encounterDisplay.SetActive(true);
        encounterHandler.encounterOptions.SetActive(true);
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.gameObject.SetActive(true);

        //close this in case it was open
        exploreLocation2.SetActive(false);

        //set this flag
        encounterHandler.dontAllowExploreMore = false;

        //bring back destination music when going out from battle (with combat evading ability)
        //need to do this before new encounter option is drawn
        //perhaps change music only if foe wasnt defeated? (so the music doesnt change twice)
        if (encounterHandler.encounterOption.isCombatEncounter == true && gameObject.GetComponent<CombatHandler>().opponentDefeated == false)
        {
            if (exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay != null && Clock.clock.isNight == false)
            {
                encounterMusicHolder.volume = 0;
                encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay;
                GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDayVolume);
                encounterMusicHolder.Play();

            }
            if (exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNight != null && Clock.clock.isNight == true)
            {
                encounterMusicHolder.volume = 0;
                encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNight;
                GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNightVolume);
                encounterMusicHolder.Play();
            }
        }

        //encounterHandler.encounterOption = encounterHandler.buttonChosen2.[encounterNumber].GetComponent<Encounter2>();

        //take encounter from location button continue enounter list 
        //kinda complicated way of doing this, but rather not make new list if it can be avoided
        encounterHandler.encounterOption = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>();


        //if (encounterHandler.buttonChosen2.isFirstEncounter == true)

        //display encounter icon
        //encounterHandler.ShowIcon1(encounterHandler.encounterOption);
        encounterHandler.ShowIcon3(encounterHandler.encounterOption.icon);
        
        encounterHandler.encounter1Text.text = "";

        //set foe icon here also
        if (encounterHandler.encounterOption.isCombatEncounter == true)
        {
            encounterHandler.SetFoeIcon(encounterHandler.encounterOption.icon);

            encounterHandler.encounter1Text.text += "<br><br><br><br><br><br><br><br>";
        }

        //display night icon at night, if there is any
        if (Clock.clock.isNight == true && encounterHandler.encounterOption.nightIcon != null && encounterHandler.encounterOption.isCombatEncounter == false)
        {
            encounterHandler.ShowIcon3(encounterHandler.encounterOption.nightIcon);
        }

        //display start text
        encounterHandler.encounter1Text.text += encounterHandler.encounterOption.startText;

        //display enemy stats, if there are any
        //enemy stat reset
        ResetFoeStats();
        encounterHandler.encounterOption.ShowEnemyStats();

        //draw the abilities if there are any
        encounterHandler.encounterOption.NewFoeAbilities();

        if (encounterHandler.buttonChosen2.isDestinationButton == true)
        {
            //encounterHandler.isDestinationEncounter = false;
            //encounterHandler.encounterOption.allowExploreMore = true;

        }
        else
        {
            //encounterHandler.encounterOption.isContinueEncounter = true;
            //encounterHandler.encounterOption.allowExploreMore = false;
        }

        // dont need to change sfx

        //reset reroll & explore more costs
        //special case for foresight
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().HasPassiveTest(6))
        {
            encounterHandler1.exploreCost = 0;
        }
        else
        {
            encounterHandler1.exploreCost = 1;
        }
        encounterHandler.rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();

    }
    */

    public int RollContinueEncounter()
    {
        List<GameObject> listReference = encounterHandler1.buttonChosen2.continueEncounters;

        int i = 0;
        do
        {
            int encounterNumber = Random.Range(0, listReference.Count);
            if (listReference[encounterNumber].GetComponent<Encounter2>().isTaken == false)
            {
                int rarityCheck = Random.Range(1, 11);
                Encounter2 encounter = listReference[encounterNumber].GetComponent<Encounter2>();

                int modifiedGameStage = GameManager.ins.gameStage + encounterHandler1.destinationDifficultyModifier;

                //check time requirements (dont allow invalid encounters through)
                if ((encounter.requirementTime == 1 && Clock.clock.isNight == false) || (encounter.requirementTime == 2 && Clock.clock.isNight == true) || encounter.requirementTime == 0)
                {
                    if (modifiedGameStage == 1 && rarityCheck <= encounter.stage1Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 2 && rarityCheck <= encounter.stage2Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 3 && rarityCheck <= encounter.stage3Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 4 && rarityCheck <= encounter.stage4Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 5 && rarityCheck <= encounter.stage5Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 6 && rarityCheck <= encounter.stage6Rarity)
                    {
                        return encounterNumber;
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //for drawing new strategic encounter to map
    public int RollContinueStrategicEncounter()
    {
        List<GameObject> listReference = encounterHandler1.buttonChosen2.continueEncounters;

        int i = 0;
        do
        {
            int encounterNumber = Random.Range(0, listReference.Count);
            if (listReference[encounterNumber].GetComponent<StrategicEncounter>().encounter.isTaken == false)
            {
                int rarityCheck = Random.Range(1, 11);
                Encounter2 encounter = listReference[encounterNumber].GetComponent<StrategicEncounter>().encounter;

                int modifiedGameStage = GameManager.ins.gameStage + encounterHandler1.destinationDifficultyModifier;

                //check time requirements (dont allow invalid encounters through)
                if ((encounter.requirementTime == 1 && Clock.clock.isNight == false) || (encounter.requirementTime == 2 && Clock.clock.isNight == true) || encounter.requirementTime == 0)
                {
                    if (modifiedGameStage == 1 && rarityCheck <= encounter.stage1Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 2 && rarityCheck <= encounter.stage2Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 3 && rarityCheck <= encounter.stage3Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 4 && rarityCheck <= encounter.stage4Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 5 && rarityCheck <= encounter.stage5Rarity)
                    {
                        return encounterNumber;
                    }
                    if (modifiedGameStage == 6 && rarityCheck <= encounter.stage6Rarity)
                    {
                        return encounterNumber;
                    }
                }
            }
            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }

    //need to call this here because the button which calls this gets deleted
    public void RerollButton()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke("RerollSkillCheck", 0.5f);
    }

    void RerollSkillCheck()
    {
        //note that this now always uses first button on the list 
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            encounterHandler1.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().SkillCheck();
            return;
        }

        //encounterHandler1.encounterOption.buttons[encounterHandler1.buttonChosen].GetComponent<EncounterButton>().SkillCheck();
        encounterHandler1.buttonChosen2.SkillCheck();
    }

    //need to call this here because the button which calls this gets deleted
    public void ProgressiveRerollButton()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke("RerollProgressiveSkillCheck", 0.5f);
    }

    void RerollProgressiveSkillCheck()
    {
        //encounterHandler1.encounterOption.buttons[encounterHandler1.buttonChosen].GetComponent<EncounterButton>().SkillCheck();
        encounterHandler1.buttonChosen2.ProgressiveSkillCheck();
    }

    //Stores the chosen encounter button number
    [PunRPC]
    void RPC_ClearStorage()
    {
        for (int i = buttonBackup.transform.childCount; i > 0; i--)
        {
            Destroy(buttonBackup.transform.GetChild(i - 1).gameObject);
        }
    }

    //Stores the chosen encounter button number
    [PunRPC]
    void RPC_StoreButtonNumber(int chosenButton, int originalNumber)
    {
        //encounterHandler1.buttonChosen = chosenButton;

        for (int i = buttonBackup.transform.childCount; i > 0; i--)
        {
            Destroy(buttonBackup.transform.GetChild(i - 1).gameObject);
        }

        /*set the button as taken, if it has the getstaken flag
        //uses separate number taken from the original encounter instantiate
        for (int i = 0; i < encounterHandler1.encounterOption.buttons.Count; i++)
        {
            // these are relocated to skillcheck additional effects
            if (originalNumber == i && encounterHandler1.encounterOption.buttons[i].GetComponent<EncounterButton>().getsTakenTemporarily == true)
            {
                encounterHandler1.encounterOption.buttons[i].GetComponent<EncounterButton>().isTakenTemporarily = true;
                //encounterHandler1.encounterOption.buttons[i].GetComponent<EncounterButton>().originalButtonNumber = originalNumber;
            }
            

            //this uses different buttonnumber than the one above, seems to work for now at least :-)
            //but could consider removing the button number check entirely, to remove all buttons with the flag at once (in case of multi-option doors and such)
            if (chosenButton == i && encounterHandler1.encounterOption.buttons[i].GetComponent<EncounterButton>().getsTakenPermanently == true)
            {
                //note that this button is taken on all instances in the same location, so shouldnt use the same button twice on same location
                //also note that each of these buttons should be added to the permanentlyTakenButtons list of this class
                //encounterHandler1.encounterOption.buttons[i].GetComponent<EncounterButton>().isTakenPermanently = true;

                //lets actually change the value directly at the instantiated strategic encounter (so we have separate values for each door etc)
                GameManager.ins.references.currentStrategicEncounter.removeExhaustableButtons = true;
            }
        }
        */

        for (int i = 0; i < encounterHandler1.buttonHolder.transform.childCount; i++)
        {
            if (i == chosenButton)
            {

                //encounterHandler1.buttonChosen2 = encounterHandler1.buttonHolder.transform.GetChild(i).GetComponent<EncounterButton>();
                GameObject buttonObject = Instantiate(encounterHandler1.buttonHolder.transform.GetChild(i).gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                //if (encounterHandler1.isDestinationEncounter == true)
                if (buttonObject.GetComponent<EncounterButton>().isDestinationButton == true)
                {
                    destinationNumber = buttonObject.GetComponent<EncounterButton>().destinationNumber;

                    encounterHandler1.destinationDifficultyModifier = buttonObject.GetComponent<EncounterButton>().destinationDifficultyModifier;
                }

                //places it options area
                buttonObject.transform.SetParent(buttonBackup.transform, false);

                encounterHandler1.buttonChosen2 = buttonObject.GetComponent<EncounterButton>();
                //buttonObject.GetComponent<Button>().interactable = false;

                /*lets try make this specific button taken permanently, if the flag is true
                if (buttonObject.GetComponent<EncounterButton>().getsTakenPermanently == true)
                {
                    //note that this button is taken on all instances in the same location, so shouldnt use the same button twice on same location
                    //also note that each of these buttons should be added to the permanentlyTakenButtons list of this class
                    buttonObject.GetComponent<EncounterButton>().isTakenPermanently = true;
                }
                */
            }
        }
    }

    //Stores the chosen encounter button number
    //[PunRPC]
    public void SkillCheck(string skillCheckText, bool isSuccess)
    {
        if (isSuccess == true)
        {
            GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlaySkillCheckSuccess();
            encounterHandler1.skillCheckSuccess = true;
        }
        if (isSuccess == false)
        {
            GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlaySkillCheckFail();
            encounterHandler1.skillCheckSuccess = false;
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
        }

        encounterHandler1.encounter1Text.text = "";

        //we shouldnt need this?
        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            //encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br>";
        }

        encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text += skillCheckText;

        Invoke(nameof(SpawnSkillCheckButtons), 0.2f);
    }

    void SpawnSkillCheckButtons()
    {
        encounterHandler1.GetComponent<EncounterHandler>().SpawnSkillCheckButtons();
    }

    //for v0.5.7.
    public void InstantSkillCheck(bool isSuccess)
    {

    }

    //remove rpc call
    //Stores the chosen encounter button number
    //[PunRPC]
    public void ProgressiveSkillCheck(string skillCheckText, bool isSuccess, int hits)
    {
        Debug.Log("number of hits is: " + hits);

        if (isSuccess == true)
        {
            GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlaySkillCheckSuccess();
            encounterHandler1.skillCheckSuccess = true;
            encounterHandler1.progressiveSkillcheckHits = hits;

        }
        if (isSuccess == false)
        {
            GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlaySkillCheckFail();
            encounterHandler1.skillCheckSuccess = false;
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
        }

        encounterHandler1.encounter1Text.text = "";

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            //encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
        }

        encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text += skillCheckText;

        Invoke(nameof(SpawnProgressiveSkillCheckButtons), 0.2f);
    }

    void SpawnProgressiveSkillCheckButtons()
    {
        encounterHandler1.GetComponent<EncounterHandler>().SpawnProgressiveSkillCheckButtons();
    }

    public void ContinueFromNoSkillCheck()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //lets set this for now, so that these encopunters can actually be removed?
        encounterHandler1.skillCheckSuccess = true;

        //need to delay this, because the new button gets stored by rpc call which is slow
        Invoke("DelayedContinueFromNoSkillCheck", 0.4f);
    }

    void DelayedContinueFromNoSkillCheck()
    {
        if (encounterHandler1.buttonChosen2.successEffectList.Count > 0)
        {
            bool rewardFound = false;
            int numberOfEffects = encounterHandler1.buttonChosen2.successEffectList.Count;

            do
            {
                int probabilityCheck = Random.Range(1, 101);
                int chosenEffect = Random.Range(0, numberOfEffects);

                if (encounterHandler1.buttonChosen2.successEffectList[chosenEffect].probability >= probabilityCheck)
                {
                    PV.RPC("RPC_ContinueFromNoSkillCheckRandomReward", RpcTarget.AllBufferedViaServer, chosenEffect);
                    rewardFound = true;
                }
            }
            while (rewardFound == false);
        }
        else
        {
            PV.RPC("RPC_ContinueFromNoSkillCheck", RpcTarget.AllBufferedViaServer);
        }
    }

    //for default rewards
    [PunRPC]
    void RPC_ContinueFromNoSkillCheck()
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        encounterHandler1.encounter1Text.text = "";

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
        }

        encounterHandler1.encounter1Text.text += encounterHandler1.buttonChosen2.endText1;
        encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";

        //opponent becomes "defeated" on charm success (also works whenever we want to remove foe with successful skillcheck)
        //kinda unused atm? 
        if (encounterButton.specialEffectOnSuccess == 10 && encounterHandler1.skillCheckSuccess == true)
        {
            gameObject.GetComponent<CombatHandler>().opponentDefeated = true;
        }

        //opponent is removed from battle, but stays on map
        if (encounterButton.specialEffectOnSuccess == 37 && encounterHandler1.skillCheckSuccess == true)
        {
            gameObject.GetComponent<CombatHandler>().removeFoeFromBattleOnly = true;
        }

        SkillCheckSuccessEffect();

        if (Clock.clock.isNight == true && encounterButton.successIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            encounterHandler1.ShowIcon3(encounterButton.successIconNight);
        }
        else if (encounterHandler1.buttonChosen2.successIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            encounterHandler1.ShowIcon3(encounterButton.successIcon);
        }
        //play the audioclip of the effect chosen, if there is any
        if (encounterHandler1.buttonChosen2.successSfx != null)
        {
            sfxHolder.clip = encounterButton.successSfx;
            sfxHolder.Play();
        }

        //do additional effects, if there are any set on the button
        //this should always happen before spanwing buttons (otherwise the flag variables will go wrong)
        SuccessfulSkillCheckAdditionalEffects();

        //spawn the encounter buttons again (now modified version)
        encounterHandler1.encounterOption.SpawnContinuationButtons(true, true);
    }

    //for random rewards
    [PunRPC]
    void RPC_ContinueFromNoSkillCheckRandomReward(int chosenEffect)
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        encounterHandler1.encounter1Text.text = "";

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
        }

        encounterHandler1.encounter1Text.text += encounterButton.endText1;

        //add the new text at end of old one
        encounterHandler1.encounter1Text.text += encounterButton.successEffectList[chosenEffect].effectText;

        int chosenEffect2 = 999;

        //new feature for v94
        //can give secondary reward in some cases
        if (encounterButton.successEffectList2.Count > 0 && encounterButton.successEffectList[chosenEffect].dontAllowSecondaryEffect == false)
        {
            bool rewardFound = false;
            int numberOfEffects = encounterButton.successEffectList2.Count;

            do
            {
                int probabilityCheck = Random.Range(1, 101);
                chosenEffect2 = Random.Range(0, numberOfEffects);

                if (encounterButton.successEffectList2[chosenEffect2].probability >= probabilityCheck)
                {
                    encounterHandler1.encounter1Text.text += " " + encounterButton.successEffectList2[chosenEffect2].effectText;
                    rewardFound = true;
                }
            }
            while (rewardFound == false);
        }

        encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";

        SkillCheckSuccessRandomEffect(chosenEffect, chosenEffect2);

        //lets try doing the default effect too
        SkillCheckSuccessEffect();

        //show new icon if there is one given
        if (Clock.clock.isNight == true && encounterButton.successIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            encounterHandler1.ShowIcon3(encounterButton.successIconNight);
        }
        else if (encounterHandler1.buttonChosen2.successIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            encounterHandler1.ShowIcon3(encounterButton.successIcon);
        }
        //play the audioclip of the effect chosen, if there is any
        if (encounterHandler1.buttonChosen2.successSfx != null)
        {
            sfxHolder.clip = encounterButton.successSfx;
            sfxHolder.Play();
        }

        if (encounterButton.successEffectList[chosenEffect].icon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            encounterHandler1.ShowIcon3(encounterButton.successEffectList[chosenEffect].icon);
        }
        //play the audioclip of the effect chosen, if there is any
        if (encounterButton.successEffectList[chosenEffect].sfx != null)
        {
            sfxHolder.clip = encounterButton.successEffectList[chosenEffect].sfx;
            sfxHolder.Play();
        }

        //do additional effects, if there are any set on the button
        //this should always happen before spanwing buttons (otherwise the flag variables will go wrong)
        SuccessfulSkillCheckAdditionalEffects();

        //spawn failed continue buttons in some rare cases (such as with gambler)
        if (encounterButton.successEffectList[chosenEffect].useFailContinueButtons == true)
        {
            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(false, true);
        }
        else
        {
            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(true, true);
        }
    }

    //this handles both skillcheck & combat -finish buttons
    public void ContinueFromSkillCheck()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        encounterHandler1.firstCheckSolved = true;

        //in case we coming from foe special skillcheck
        //dont continue loop, if hero is defeated
        /*this should be removed soon
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true &&
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == false)
        {
            Invoke(nameof(DelayedContinueFromSpecialSkillCheck), 0.4f);
            return;
        }
        */

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            //allow encounter cards
            CardHandler.ins.SetUsables(7);
        }
        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            //allow encounter cards
            CardHandler.ins.SetUsables(0);
        }

        //need to put delay cause of storebutton rpc call
        Invoke(nameof(DelayedContinueFromSkillCheck), 0.4f);

    }

    //unused
    //used for foe specials
    //changed in v95
    void DelayedContinueFromSpecialSkillCheck()
    {
        //PV.RPC("RPC_ContinueFromSpecialSkillCheck", RpcTarget.AllBufferedViaServer);
        //RPC_ContinueFromSpecialSkillCheck();
    }

    void DelayedContinueFromSkillCheck()
    {
        //Debug.Log("should come at delay, successlist count is: " + encounterHandler1.buttonChosen2.successEffectList.Count);

        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        // we might stil need this for pre-combat effects? (such as assasin strike?)
        // lets add this here, to make sure hero can actually be defeated properly in v91
        if (gameObject.GetComponent<CombatHandler>().heroKnockedOut == true)
        {
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
            PV.RPC(nameof(RPC_ContinueFromSkillCheck), RpcTarget.AllBufferedViaServer);
            return;
        }
        

        //this isnt optimal for combat reward handling tho..
        if (encounterHandler1.skillCheckSuccess == true && encounterButton.successEffectList.Count > 0)
        {
            bool rewardFound = false;
            int numberOfEffects = encounterButton.successEffectList.Count;

            do
            {
                int probabilityCheck = Random.Range(1, 101);
                int chosenEffect = Random.Range(0, numberOfEffects);

                if (encounterButton.successEffectList[chosenEffect].probability >= probabilityCheck)
                {
                    PV.RPC(nameof(RPC_ContinueFromSkillCheckRandomReward), RpcTarget.AllBufferedViaServer, chosenEffect);
                    rewardFound = true;
                }
            }
            while (rewardFound == false);
            return;
        }

        if (encounterHandler1.skillCheckSuccess == false && encounterButton.failEffectList.Count > 0)
        {
            bool rewardFound = false;
            int numberOfEffects = encounterButton.failEffectList.Count;

            do
            {
                int probabilityCheck = Random.Range(1, 101);
                int chosenEffect = Random.Range(0, numberOfEffects);

                if (encounterButton.failEffectList[chosenEffect].probability >= probabilityCheck)
                {
                    PV.RPC(nameof(RPC_ContinueFromSkillCheckRandomReward), RpcTarget.AllBufferedViaServer, chosenEffect);
                    rewardFound = true;
                }
            }
            while (rewardFound == false);
            return;
        }

        else
        {
            PV.RPC(nameof(RPC_ContinueFromSkillCheck), RpcTarget.AllBufferedViaServer);
        }

        //can put this back to original value here
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference = false;
    }

    //used for combat encounters too
    [PunRPC]
    void RPC_ContinueFromSkillCheck()
    {
        //reset reroll cost
        encounterHandler1.rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        //opponent becomes "defeated" on charm success (also works whenever we want to remove foe with successful skillcheck)
        if (encounterButton.specialEffectOnSuccess == 10 && encounterHandler1.skillCheckSuccess == true)
        {
            gameObject.GetComponent<CombatHandler>().opponentDefeated = true;

            if(encounterButton.isTurnUnholyCheck == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(40, 3);
                GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
            }
            else if (encounterButton.showGraveOnSuccess == true)
            {
                GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);

                //repurposed variable here for v0.7.0.
                if(encounterButton.isExplosiveDeathButton == true)
                {
                    GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(9, 3);
                }
            }
        }

        //opponent is removed from battle, but stays on map
        if (encounterButton.specialEffectOnSuccess == 37 && encounterHandler1.skillCheckSuccess == true)
        {
            gameObject.GetComponent<CombatHandler>().removeFoeFromBattleOnly = true;
        }

        //opponent becomes "defeated" when fleeing etc (in pre-battle, used for rogues atm)
        if (encounterHandler1.skillCheckSuccess == false && encounterButton.removeEncounterOnFailure == true && encounterButton.foeDefeatedOnCheckFailure == true)
        {
            gameObject.GetComponent<CombatHandler>().opponentDefeated = true;
        }

        //special cases for combat results
        if (gameObject.GetComponent<CombatHandler>().opponentDefeated == true)
        {
            //encounterHandler1.encounterOption.currentEnergy = 0;
            encounterHandler1.GetComponent<EncounterHandler>().encounterOption.HideEnemyStats();

            //test this to "grave" exploding foes
            /*dont grave untill explosive death perk has been removed?
             * replaced to spawn continuation buttons method
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(242) == false &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(243) == false)
                
            Debug.Log("checks opponent defeated.");

            if (CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 242, 4) == 0 &&
                CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 243, 4) == 0)
            {
                GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
            }
            */
        }

        if (gameObject.GetComponent<CombatHandler>().heroKnockedOut == true)
        {
            //encounterHandler1.encounterOption.currentEnergy = 0;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -gameObject.GetComponent<CombatHandler>().hitsTaken);
            encounterHandler1.GetComponent<EncounterHandler>().encounterOption.HideEnemyStats();
        }

        //bring back destination music when battle ends
        if (gameObject.GetComponent<CombatHandler>().opponentDefeated == true || gameObject.GetComponent<CombatHandler>().heroKnockedOut == true)
        {
            //Invoke("PlayLocationMusic", 1f);
            //lets remove this for now
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();

            /* dont need this for v90
            if (exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay != null && Clock.clock.isNight == false)
            {
                encounterMusicHolder.volume = 0;
                encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay;
                GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDayVolume);
                encounterMusicHolder.Play();
                //encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxDay;
                //encounterMusicHolder.Play();

            }
            if (exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNight != null && Clock.clock.isNight == true)
            {
                encounterMusicHolder.volume = 0;
                encounterMusicHolder.clip = exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNight;
                GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, exploreLocationChoiceButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[0].GetComponent<Encounter2>().ambientSfxNightVolume);
                encounterMusicHolder.Play();
            }
            */
        }

        //show appropriate text & implement effects
        if (encounterHandler1.skillCheckSuccess == true)
        {
            encounterHandler1.encounter1Text.text = "";

            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
            {
                encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
            }

            encounterHandler1.encounter1Text.text += encounterButton.endText1;
            encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";
            //note that this also could cancel the forced encounter flag
            SkillCheckSuccessEffect();

            //show new icon if there is one given
            if (Clock.clock.isNight == true && encounterButton.successIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.successIconNight);
            }
            else if (encounterHandler1.buttonChosen2.successIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.successIcon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterButton.successSfx != null)
            {
                sfxHolder.clip = encounterButton.successSfx;
                sfxHolder.Play();
            }
            //this should always be done before button spawn
            SuccessfulSkillCheckAdditionalEffects();

            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(true, true);
        }


        if (encounterHandler1.skillCheckSuccess == false)
        {
            encounterHandler1.encounter1Text.text = "";

            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
            {
                encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
            }

            encounterHandler1.encounter1Text.text += encounterButton.endText2;
            encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";

            //note that this also could cancel the forced encounter flag
            SkillCheckFailureEffect();

            //show new icon if there is one given
            if (Clock.clock.isNight == true && encounterButton.failIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.failIconNight);
            }
            else if (encounterButton.failIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.failIcon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterButton.failSfx != null)
            {
                sfxHolder.clip = encounterButton.failSfx;
                sfxHolder.Play();
            }

            //this should always be done before button spawn
            FailedSkillCheckAdditionalEffects();

            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(false, true);
        }

        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference = false;
    }

    public void ContinueFromProgressiveSkillCheck()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //need to put delay cause of storebutton rpc call
        Invoke("DelayedContinueFromProgressiveSkillCheck", 0.4f);
    }

    void DelayedContinueFromProgressiveSkillCheck()
    {
        encounterHandler1.firstCheckSolved = true;

        PV.RPC("RPC_ContinueFromProgressiveSkillCheck", RpcTarget.AllBufferedViaServer);
    }

    //Stores the chosen encounter button number
    [PunRPC]
    void RPC_ContinueFromProgressiveSkillCheck()
    {
        //reset reroll cost
        encounterHandler1.rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        //show appropriate text & implement effects
        if (encounterHandler1.skillCheckSuccess == true)
        {
            encounterHandler1.encounter1Text.text = "";

            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
            {
                encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
            }

            encounterHandler1.encounter1Text.text += encounterHandler1.buttonChosen2.endText1;
            encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";
            ProgressiveSkillCheckSuccessEffect();

            //show new icon if there is one given
            if (Clock.clock.isNight == true && encounterHandler1.buttonChosen2.successIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterHandler1.buttonChosen2.successIconNight);
            }
            else if (encounterHandler1.buttonChosen2.successIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterHandler1.buttonChosen2.successIcon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterHandler1.buttonChosen2.successSfx != null)
            {
                sfxHolder.clip = encounterHandler1.buttonChosen2.successSfx;
                sfxHolder.Play();
            }
            //this should be done before button spawn
            SuccessfulSkillCheckAdditionalEffects();

            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(true, true);
        }


        if (encounterHandler1.skillCheckSuccess == false)
        {
            encounterHandler1.encounter1Text.text = "";

            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
            {
                encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
            }

            encounterHandler1.encounter1Text.text += encounterHandler1.buttonChosen2.endText2;
            encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";

            //maybe leave the regular skillcheck fail effect here for now, incase needed later
            SkillCheckFailureEffect();

            //show new icon if there is one given
            if (Clock.clock.isNight == true && encounterHandler1.buttonChosen2.failIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterHandler1.buttonChosen2.failIconNight);
            }
            else if (encounterHandler1.buttonChosen2.failIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterHandler1.buttonChosen2.failIcon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterHandler1.buttonChosen2.failSfx != null)
            {
                sfxHolder.clip = encounterHandler1.buttonChosen2.failSfx;
                sfxHolder.Play();
            }
            //this should be done before button spawn
            FailedSkillCheckAdditionalEffects();

            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(false, true);
        }
    }



    //Stores the chosen encounter button number
    [PunRPC]
    void RPC_ContinueFromSkillCheckRandomReward(int chosenEffect)
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        //show appropriate text & implement effects
        if (encounterHandler1.skillCheckSuccess == true)
        {
            encounterHandler1.encounter1Text.text = "";

            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
            {
                encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
            }

            encounterHandler1.encounter1Text.text += encounterButton.endText1;
            encounterHandler1.encounter1Text.text += encounterButton.successEffectList[chosenEffect].effectText;

            int chosenEffect2 = 999;

            //new feature for v94
            //can give secondary reward in some cases
            if (encounterButton.successEffectList2.Count > 0 && encounterButton.successEffectList[chosenEffect].dontAllowSecondaryEffect == false)
            {
                bool rewardFound = false;
                int numberOfEffects = encounterButton.successEffectList2.Count;

                do
                {
                    int probabilityCheck = Random.Range(1, 101);
                    chosenEffect2 = Random.Range(0, numberOfEffects);

                    if (encounterButton.successEffectList2[chosenEffect2].probability >= probabilityCheck)
                    {
                        encounterHandler1.encounter1Text.text += " " + encounterButton.successEffectList2[chosenEffect2].effectText;
                        rewardFound = true;
                    }
                }
                while (rewardFound == false);
            }

            encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";
            SkillCheckSuccessRandomEffect(chosenEffect, chosenEffect2);

            //lets try doing the default effect too
            SkillCheckSuccessEffect();

            //show new icon if there is one given
            if (Clock.clock.isNight == true && encounterButton.successIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.successIconNight);
            }
            else if (encounterHandler1.buttonChosen2.successIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.successIcon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterButton.successSfx != null)
            {
                sfxHolder.clip = encounterButton.successSfx;
                sfxHolder.Play();
            }

            //alternatively, show new icon if there is one given
            if (encounterButton.successEffectList[chosenEffect].icon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.successEffectList[chosenEffect].icon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterButton.successEffectList[chosenEffect].sfx != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                sfxHolder.clip = encounterButton.successEffectList[chosenEffect].sfx;
                sfxHolder.Play();
            }
            //this should be done before button spawn
            SuccessfulSkillCheckAdditionalEffects();

            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(true, true);
        }

        if (encounterHandler1.skillCheckSuccess == false)
        {
            encounterHandler1.encounter1Text.text = "";

            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
            {
                encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br><size=12>";
            }

            encounterHandler1.encounter1Text.text += encounterButton.endText2;
            encounterHandler1.encounter1Text.text += encounterButton.failEffectList[chosenEffect].effectText;
            encounterHandler1.encounter1Text.text += "\n<size=8>\n</size>";
            SkillCheckFailureRandomEffect(chosenEffect);

            //lets try doing the default effect too
            SkillCheckFailureEffect();

            //show new icon if there is one given
            if (Clock.clock.isNight == true && encounterButton.failIconNight != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.failIconNight);
            }
            else if (encounterHandler1.buttonChosen2.failIcon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.failIcon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterButton.failSfx != null)
            {
                sfxHolder.clip = encounterButton.failSfx;
                sfxHolder.Play();
            }

            //alternatively, show new icon if there is one given
            if (encounterButton.failEffectList[chosenEffect].icon != null && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
            {
                encounterHandler1.ShowIcon3(encounterButton.failEffectList[chosenEffect].icon);
            }
            //play the audioclip of the effect chosen, if there is any
            if (encounterButton.failEffectList[chosenEffect].sfx != null)
            {
                sfxHolder.clip = encounterButton.failEffectList[chosenEffect].sfx;
                sfxHolder.Play();
            }

            //this should be done before button spawn
            FailedSkillCheckAdditionalEffects();

            //spawn the encounter buttons again (now modified version)
            encounterHandler1.encounterOption.SpawnContinuationButtons(false, true);
        }

        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference = false;
    }

    void SkillCheckSuccessEffect()
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        if (encounterButton.effectQty.Length > 0)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            for (int i = 0; i < encounterButton.effectQty.Length; i++)
            {
                encounterHandler1.encounter1Text.text += encounterButton.effectQty[i] + EffectName(encounterButton.effectType[i]) + " ";

                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounterButton.effectType[i], encounterButton.effectQty[i]);
                //this should remove the delay?
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(encounterButton.effectType[i], encounterButton.effectQty[i]);
            }
        }

        //special skillcheck bonuses
        if (encounterButton.requirementType.Length > 0)
        {
            //special case for side projects
            if (encounterButton.requirementType[0] == 6 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(5))
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 5);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[4].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for mask of eloquence
            if (encounterButton.requirementType[0] == 5 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 9, 226) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 3);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[226].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for paladins helm
            if (encounterButton.requirementType[0] == 8 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 53) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 1);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[53].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for helm of zaarin
            if (encounterButton.requirementType[0] == 8 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 213) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 2);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[213].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for insightful
            if (encounterButton.requirementType[0] == 8 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(250))
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 1);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[250].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for goggles of inspiration
            if (encounterButton.requirementType[0] == 9 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 8, 46) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 2);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[46].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }
        }

        /* this is done in the finishbattlebutton now
         * 
         * special case victory rush
        if (encounterHandler1.encounterOption.isCombatEncounter == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(7))
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
        }
        */
    }

    void ProgressiveSkillCheckSuccessEffect()
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special skillcheck bonuses
        if (encounterButton.requirementType.Length > 0)
        {
            //special case for side projects
            if (encounterButton.requirementType[0] == 6 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(5))
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 5);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[4].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for mask of eloquence
            if (encounterButton.requirementType[0] == 5 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 9, 226) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 3);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[226].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for paladins helm
            if (encounterButton.requirementType[0] == 8 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 53) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 1);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[53].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for helm of zaarin
            if (encounterButton.requirementType[0] == 8 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 213) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 2);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[213].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for goggles of inspiration
            if (encounterButton.requirementType[0] == 9 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 8, 46) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 2);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[46].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }
        }

        //bronzium deposit 1 / build fence
        if (encounterButton.specialEffectOnSuccess == 1)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain fame for each 3 hits
            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 3f);
            int fameReward = (int)fameFloat;
            //gain 2 coins per hit
            int coinReward = encounterHandler1.progressiveSkillcheckHits * 2;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += coinReward + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        //bronzium deposit 2
        if (encounterButton.specialEffectOnSuccess == 2)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain fame for each 2 hits
            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            int fameReward = (int)fameFloat;
            //gain 3 coins per hit
            int coinReward = encounterHandler1.progressiveSkillcheckHits * 3;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += coinReward + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        //gold deposit 1
        if (encounterButton.specialEffectOnSuccess == 3)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain fame for each 2 hits
            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            int fameReward = (int)fameFloat;
            //gain 3 coins per hit
            int coinReward = encounterHandler1.progressiveSkillcheckHits * 3;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += coinReward + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        //gold deposit 2
        if (encounterButton.specialEffectOnSuccess == 4)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain fame for each 2 hits
            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            int fameReward = (int)fameFloat;
            //gain 5 coins per hit
            int coinReward = encounterHandler1.progressiveSkillcheckHits * 5;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += coinReward + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        //lividium deposit 1 (8 coins per hit on v91)
        if (encounterButton.specialEffectOnSuccess == 5)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain fame for each 2 hits
            //float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            //int fameReward = (int)fameFloat;
            //gain 1 dust per 2 hits (need rounding function)
            //float dustFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            //int dustReward = (int)dustFloat;

            int coinReward = encounterHandler1.progressiveSkillcheckHits * 8;

            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += coinReward + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, encounterHandler1.progressiveSkillcheckHits);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        //azarian root
        if (encounterButton.specialEffectOnSuccess == 6)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain fame for each 2 hits
            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            int fameReward = (int)fameFloat;
            //gain 1 potion per 2 hits (need rounding function)
            float potionFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits / 2f);
            int potionReward = (int)potionFloat;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += "\n" + potionReward + " rejuvenation potions";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            //draw set number of potions for current player
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 19, 1, potionReward);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(1, dustReward);
        }

        //1 coins, 1 fame per hit
        if (encounterButton.specialEffectOnSuccess == 28)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, encounterHandler1.progressiveSkillcheckHits);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, encounterHandler1.progressiveSkillcheckHits);
        }

        //1 coins, 1.5 fame per hit
        if (encounterButton.specialEffectOnSuccess == 29)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain 1.5 fame fame for each hit
            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits * 1.5f);
            int fameReward = (int)fameFloat;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(4) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, encounterHandler1.progressiveSkillcheckHits);
        }

        // 0.5 bomb, 1 fame per hit
        if (encounterButton.specialEffectOnSuccess == 32)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            //gain 1.5 fame fame for each hit
            float bombFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits * 0.5f);
            int bombReward = (int)bombFloat;

            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += bombReward + EffectName(16) + " ";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, encounterHandler1.progressiveSkillcheckHits);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 169, 1, bombReward);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        //eldertree (8 coins per hit on v91), inflicts curse
        if (encounterButton.specialEffectOnSuccess == 34)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            int coinReward = encounterHandler1.progressiveSkillcheckHits * 8;

            //encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(5) + " ";
            encounterHandler1.encounter1Text.text += coinReward + EffectName(4) + " ";

            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, encounterHandler1.progressiveSkillcheckHits);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);

            //gives curse
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
        }

        // 0.5 food, 1 fame per hit
        if (encounterButton.specialEffectOnSuccess == 40)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            float foodFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits * 0.5f);
            int foodReward = (int)foodFloat;

            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + EffectName(5) + "<br>";
            encounterHandler1.encounter1Text.text += foodReward + " nourishing meals";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, encounterHandler1.progressiveSkillcheckHits);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 20, 1, foodReward);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

        // 1 food, 0.5 fame per hit
        if (encounterButton.specialEffectOnSuccess == 41)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            float fameFloat = Mathf.Ceil(encounterHandler1.progressiveSkillcheckHits * 0.5f);
            int fameReward = (int)fameFloat;

            encounterHandler1.encounter1Text.text += fameReward + EffectName(5) + "<br>";
            encounterHandler1.encounter1Text.text += encounterHandler1.progressiveSkillcheckHits + " nourishing meals";

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, fameReward);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 20, 1, encounterHandler1.progressiveSkillcheckHits);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, coinReward);
        }

    }

    //need to add special effect checks here soon
    void SkillCheckSuccessRandomEffect(int chosenEffect, int chosenEffect2)
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2.GetComponent<EncounterButton>();

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        EncounterEffect encounterEffect = encounterButton.successEffectList[chosenEffect];
        EncounterEffect encounterEffect2 = null;
        
        if (chosenEffect2 != 999)
        {
            encounterEffect2 = encounterButton.successEffectList2[chosenEffect2];
        }

        if (encounterEffect.effectQty.Length > 0)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            for (int i = 0; i < encounterEffect.effectQty.Length; i++)
            {
                encounterHandler1.encounter1Text.text += encounterEffect.effectQty[i] + EffectName(encounterEffect.effectType[i]) + " ";

                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounterEffect.effectType[i], encounterEffect.effectQty[i]);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(encounterEffect.effectType[i], encounterEffect.effectQty[i]);
            }
        }

        //equipment rewards
        if (encounterEffect.rewardType.Length > 0)
        {
            for (int i = 0; i < encounterEffect.rewardType.Length; i++)
            {
                //make sure this works for consumables too
                int holderType = CardHandler.ins.generalDeck[encounterEffect.rewardType[i]].GetComponent<Card>().cardType;

                //lets allow reducing quantity with this as well?
                //need double negative in this case?
                if (encounterEffect.rewardQty[i] < 0)
                {
                    //special case for removing equipped items (need to be single card obviously)
                    if(CardHandler.ins.generalDeck[encounterEffect.rewardType[i]].GetComponent<EquipmentCard>() != null)
                    {
                        CardHandler.ins.RemoveItemFromSlot(GameManager.ins.turnNumber, encounterEffect.rewardType[i], true);
                    }

                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, encounterEffect.rewardType[i], holderType, -encounterEffect.rewardQty[i]);

                    //CardHandler.ins.CheckIfUnusedItems();
                }

                else
                {
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, encounterEffect.rewardType[i], holderType, encounterEffect.rewardQty[i]);
                }
            }
        }

        //for v94 (secondary effects)
        if (encounterEffect2 != null)
        {
            if (encounterEffect2.effectQty.Length > 0)
            {
                encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

                for (int i = 0; i < encounterEffect2.effectQty.Length; i++)
                {
                    encounterHandler1.encounter1Text.text += encounterEffect2.effectQty[i] + EffectName(encounterEffect2.effectType[i]) + " ";

                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounterEffect.effectType[i], encounterEffect.effectQty[i]);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(encounterEffect2.effectType[i], encounterEffect2.effectQty[i]);
                }
            }

            //equipment rewards
            if (encounterEffect2.rewardType.Length > 0)
            {
                for (int i = 0; i < encounterEffect2.rewardType.Length; i++)
                {
                    //make sure this works for consumables too
                    int holderType = CardHandler.ins.generalDeck[encounterEffect2.rewardType[i]].GetComponent<Card>().cardType;

                    //lets allow reducing quantity with this as well?
                    //need double negative in this case?
                    if (encounterEffect2.rewardQty[i] < 0)
                    {
                        //special case for removing equipped items (need to be single card obviously)
                        if (CardHandler.ins.generalDeck[encounterEffect2.rewardType[i]].GetComponent<EquipmentCard>() != null)
                        {
                            CardHandler.ins.RemoveItemFromSlot(GameManager.ins.turnNumber, encounterEffect2.rewardType[i], true);
                        }

                        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, encounterEffect2.rewardType[i], holderType, -encounterEffect2.rewardQty[i]);

                        //CardHandler.ins.CheckIfUnusedItems();
                    }

                    else
                    {
                        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, encounterEffect2.rewardType[i], holderType, encounterEffect2.rewardQty[i]);
                    }
                }
            }
        }

        //special skillcheck bonuses
        if (encounterButton.requirementType.Length > 0)
        {
            //special case for side projects
            if (encounterButton.requirementType[0] == 6 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(5))
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 5);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[4].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for mask of eloquence
            if (encounterButton.requirementType[0] == 5 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 9, 226) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 3);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[226].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for paladins helm
            if (encounterButton.requirementType[0] == 8 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 53) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 1);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[53].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for helm of zaarin
            if (encounterButton.requirementType[0] == 8 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 213) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 2);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[213].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //special case for goggles of inspiration
            if (encounterButton.requirementType[0] == 9 && CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 8, 46) == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 2);

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[46].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }
        }

    }

    //need to add special effect checks here soon
    void SkillCheckFailureEffect()
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        if (encounterButton.failEffectQty.Length > 0)
        {
            encounterHandler1.encounter1Text.text += "";// "\n<size=8>\n</size>";

            for (int i = 0; i < encounterButton.failEffectQty.Length; i++)
            {
                encounterHandler1.encounter1Text.text += encounterButton.failEffectQty[i] + EffectName(encounterButton.failEffectType[i]) + " ";

                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounterButton.failEffectType[i], encounterButton.failEffectQty[i]);
                //lets try doing this without rpc call (so we dont need to add extra delays)
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(encounterButton.failEffectType[i], encounterButton.failEffectQty[i]);
            }
        }
    }

    void SkillCheckFailureRandomEffect(int chosenEffect)
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2.GetComponent<EncounterButton>();

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        EncounterEffect encounterEffect = encounterButton.failEffectList[chosenEffect];

        if (encounterEffect.effectQty.Length > 0)
        {
            encounterHandler1.encounter1Text.text += ""; // "\n<size=8>\n</size>";

            for (int i = 0; i < encounterEffect.effectQty.Length; i++)
            {
                encounterHandler1.encounter1Text.text += encounterEffect.effectQty[i] + EffectName(encounterEffect.effectType[i]) + " ";

                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(encounterEffect.effectType[i], encounterEffect.effectQty[i]);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(encounterEffect.effectType[i], encounterEffect.effectQty[i]);
            }
        }

        //equipment rewards
        if (encounterEffect.rewardType.Length > 0)
        {
            for (int i = 0; i < encounterEffect.rewardType.Length; i++)
            {
                //make sure this works for consumables too
                int holderType = CardHandler.ins.generalDeck[encounterEffect.rewardType[i]].GetComponent<Card>().cardType;

                //lets allow reducing quantity with this as well?
                if (encounterEffect.rewardQty[i] < 0)
                {
                    //special case for removing equipped items (need to be single card obviously)
                    if (CardHandler.ins.generalDeck[encounterEffect.rewardType[i]].GetComponent<EquipmentCard>() != null)
                    {
                        CardHandler.ins.RemoveItemFromSlot(GameManager.ins.turnNumber, encounterEffect.rewardType[i], true);
                    }

                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, encounterEffect.rewardType[i], holderType, -encounterEffect.rewardQty[i]);

                    //this is technically called alrdy, but doesnt seem to work
                    //CardHandler.ins.CheckIfUnusedItems();
                }

                else
                {
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, encounterEffect.rewardType[i], holderType, encounterEffect.rewardQty[i]);
                }
            }
        }
    }

    //revised for v92
    public string EffectName(int type)
    {
        //reward types: 
        //0= energy, 1= arcane dust, 2= upgrade points, 4= coins, 5= fame, 6= favor
        if (type == 0)
        {
            return "<sprite index=11>";
        }
        if (type == 1)
        {
            return "<sprite=\"sprites v88\" index=16>";
        }
        if (type == 2)
        {
            return "<sprite=\"sprites v88\" index=22>";
        }
        if (type == 3)
        {
            return "<sprite=\"sprites v92\" index=3>";
        }
        if (type == 4)
        {
            return "<sprite index=13>";
        }
        if (type == 5)
        {
            return "<sprite index=3>";
        }
        if (type == 6)
        {
            return "<sprite index=12>";
        }
        if (type == 16)
        {
            return "<sprite=\"bombs\" index=0>";
        }
        //ap cost for v94, actually gives sleep atm
        if (type == 17)
        {
            return "<sprite index=32>";
        }
        else
        {
            return null;
        }
    }

    //shows animation for all
    //[PunRPC]
    public void ShowHeroAnimation(int turnNumber, int animationNumber)
    {
        characterDisplays.GetComponent<CharacterDisplays>().ShowCharacter(turnNumber, animationNumber);
    }

    //shows animation for all
    [PunRPC]
    void RPC_HideHeroAnimation()
    {
        characterDisplays.GetComponent<CharacterDisplays>().HideCharacter(GameManager.ins.turnNumber);
    }

    //used for noskillchecks and successful skillcheck
    //exhausts encounters, opens new destinations etc (when the button has appropriate settings)
    public void SuccessfulSkillCheckAdditionalEffects()
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        //lets put the button exhaust here now
        if (encounterButton.getsTakenTemporarily == true)
        {
            //GameManager.ins.references.currentStrategicEncounter.encounter.buttons[encounterButton.originalButtonNumber].GetComponent<EncounterButton>().isTakenTemporarily = true;

            //theres something wrong with this, lets make temporary patch on it for now
            if (GameManager.ins.references.currentEncounter.buttons.Count >= encounterButton.originalButtonNumber)
            {
                GameManager.ins.references.currentEncounter.buttons[encounterButton.originalButtonNumber].GetComponent<EncounterButton>().isTakenTemporarily = true;
            }
        }

        if (encounterButton.getsTakenPermanently == true)
        {
            //lets put it here too, although unsure if necessary
            if (GameManager.ins.references.currentStrategicEncounter.removeExhaustableButtons.Length >= encounterButton.originalButtonNumber)
            {
                GameManager.ins.references.currentStrategicEncounter.removeExhaustableButtons[encounterButton.originalButtonNumber] = true;
            }
        }

        //removes encounter if this is checked (combat removes are handled elsewhere)
        if (encounterButton.removeEncounterOnSuccess && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            RemoveEncounter(0, false);

            //could add the triggered overlay check here
            //should be good enough, if the triggered overlays are only used for doorways.. (but if not, need different flag variable)
            if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().triggerOverlay > 0)
            {
                TriggerOverlay(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().triggerOverlay);
            }
        }

        //cancels forced encounter if this is checked
        if (encounterButton.removeForcedOnSuccess)
        {
            encounterHandler1.forcedEncounterCancelled = true;
        }

        //2 rations (not sure if we should use another variable for this?)
        if (encounterButton.specialEffectOnSuccess == 7)
        {
            encounterHandler1.encounter1Text.text += "\n2 nourishing meals"; //"\n<size=8> \n</size>2 nourishing meals";

            //give 2 rations
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 20, 1, 2);
        }

        //4 turn poison, 1 turn sleep
        if (encounterButton.specialEffectOnSuccess == 8)
        {
            //special case for sentinel
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                encounterHandler1.encounter1Text.text += "\n4 stacks of poison";

                //4 poisons
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, 4);
            }
            //1 sleep
            //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 2, 1);

            encounterHandler1.encounter1Text.text += "\nsleep for 1 turn";

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 1);
            }
        }
        //1 turn sleep
        if (encounterButton.specialEffectOnSuccess == 9)
        {
            encounterHandler1.encounter1Text.text += "\nsleep for 1 turn";

            //1 sleep
            //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 2, 1);

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 1);
            }
        }

        //opponent becomes "defeated" on charm success
        if (encounterButton.specialEffectOnSuccess == 10 && encounterHandler1.skillCheckSuccess == true)
        {
            gameObject.GetComponent<CombatHandler>().opponentDefeated = true;

            if (encounterButton.isTurnUnholyCheck == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(40, 3);
                GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
            }
            else if (encounterButton.showGraveOnSuccess == true)
            {
                GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);

                //repurposed variable here for v0.7.0.
                if (encounterButton.isExplosiveDeathButton == true)
                {
                    GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(9, 3);
                }
            }
        }

        //skinning done
        if (encounterButton.specialEffectOnSuccess == 11)
        {
            encounterHandler1.skinningDone = true;
        }

        //trophy hunting done
        if (encounterButton.specialEffectOnSuccess == 12)
        {
            encounterHandler1.trophyHuntingDone = true;
        }

        //disenchant done
        if (encounterButton.specialEffectOnSuccess == 13)
        {
            encounterHandler1.disenchantDone = true;

            //remove magic shell, if there is any
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(14))
            {
                //note that we use effect number here, instead of number in deck
                //turnnumber is irrelevant
                //holder 4 is for foe cards
                CardHandler.ins.ReduceQuantity(0, 14, 4, 1);
            }
        }

        //charm done
        if (encounterButton.specialEffectOnSuccess == 14)
        {
            encounterHandler1.charmDone = true;
        }

        //charm animal done
        if (encounterButton.specialEffectOnSuccess == 15)
        {
            encounterHandler1.charmAnimalDone = true;
        }

        //4 turn poison
        if (encounterButton.specialEffectOnSuccess == 16)
        {
            //special case for sentinel
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                encounterHandler1.encounter1Text.text += "\n4 stacks of poison";

                //4 poisons
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, 4);
            }
        }

        //lore upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 17)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(129);
        }

        //strength upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 18)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxStrength += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(115);
        }

        //defense upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 19)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(117);
        }

        //mechanics upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 20)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(125);
        }

        //arcanePower upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 21)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxArcanePower += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(119);
        }

        //resistance upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 22)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(121);
        }

        //influence upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 23)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(123);
        }

        //digging upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 24)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(127);
        }

        //discovery upgrade (works for both level 1 and 2)
        if (encounterButton.specialEffectOnSuccess == 25)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(131);
        }

        //4 rations
        if (encounterButton.specialEffectOnSuccess == 26)
        {
            encounterHandler1.encounter1Text.text += "\n4 nourishing meals";

            //give 2 rations
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 20, 1, 4);
        }

        //sumpter horse
        if (encounterButton.specialEffectOnSuccess == 27)
        {
            encounterHandler1.encounter1Text.text += "\nyou get sumpter horse";

            //give sumpter horse
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 63, 5, 1);
        }

        //sumpter horse
        if (encounterButton.specialEffectOnSuccess == 30)
        {
            encounterHandler1.encounter1Text.text += "\nyou get courser";

            //give sumpter horse
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 64, 5, 1);
        }

        //shovel
        if (encounterButton.specialEffectOnSuccess == 31)
        {
            encounterHandler1.encounter1Text.text += "\nyou get bronzium shovel";

            //give sumpter horse
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 79, 5, 1);
        }

        /*
        //exhaust the current encounter if this flag is set to true on the encounter button
        if (encounterHandler1.buttonChosen2.exhaustCurrentEncounterOnSuccess == true)
        {
            encounterHandler1.encounterOption.isTaken = true;
        }

        //exhaust the interlude encounter if this flag is set to true on the encounter button
        if (encounterHandler1.buttonChosen2.exhaustInterludeOnSuccess == true)
        {
            //use the stored destination & interlude -numbers
            destinationButtons[destinationNumber].GetComponent<EncounterButton>().continueEncounters[interludeNumber].GetComponent<Encounter2>().isTaken = true;
        }
        */
        /*
        if (encounterHandler1.buttonChosen2.openDestinationOnSuccess != 0)
        {
            exploreLocationChoiceButtons[encounterHandler1.buttonChosen2.openDestinationOnSuccess].GetComponent<EncounterButton>().isTaken = false;

            //give message
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " opened a new exploration option!";
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
            }
        }
        */

        //6 turn poison, 2 turn sleep
        if (encounterButton.specialEffectOnSuccess == 33)
        {
            //special case for sentinel
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                encounterHandler1.encounter1Text.text += "\n6 stacks of poison";

                //4 poisons
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, 6);
            }
            //1 sleep
            //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 2, 1);

            encounterHandler1.encounter1Text.text += "\nsleep for 2 turn";

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 2);
            }
        }

        //2 curses removed
        if (encounterButton.specialEffectOnSuccess == 38)
        {
            encounterHandler1.encounter1Text.text += "\n2 curses removed";

            //give 2 rations
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 28, 7, 2);
        }

        //1 curse removed
        if (encounterButton.specialEffectOnSuccess == 43)
        {
            encounterHandler1.encounter1Text.text += "\n1 curse removed"; //\n<size=8> \n</size>

            //give 2 rations
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 28, 7, 1);
        }

        //2 turn sleep
        if (encounterButton.specialEffectOnSuccess == 39)
        {
            encounterHandler1.encounter1Text.text += "\nsleep for 2 turns";

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 2);
            }
        }

        //6 turn poison
        if (encounterButton.specialEffectOnSuccess == 44)
        {
            //special case for sentinel
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                encounterHandler1.encounter1Text.text += "\n6 stacks of poison";

                //6 poisons
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, 6);
            }
        }

        //return disc of claudia
        if (encounterButton.specialEffectOnSuccess == 46)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[0] = true;
        }
        //return disc of dweller
        if (encounterButton.specialEffectOnSuccess == 47)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[1] = true;
        }
        //return disc of irino
        if (encounterButton.specialEffectOnSuccess == 48)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[2] = true;
        }
        //return disc of lavinia
        if (encounterButton.specialEffectOnSuccess == 49)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[3] = true;
        }
        //return disc of mack
        if (encounterButton.specialEffectOnSuccess == 50)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[4] = true;
        }
        //return disc of nabamax
        if (encounterButton.specialEffectOnSuccess == 51)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[5] = true;
        }
        //return disc of zaarin
        if (encounterButton.specialEffectOnSuccess == 52)
        {
            //set the flag for later use
            GameManager.ins.references.discsReturned[6] = true;
        }

        //nabamax lesson (progressive cost)
        if (encounterButton.specialEffectOnSuccess == 53)
        {
            //set the flag for later use
            GameManager.ins.references.nabamaxLessonCost = (int)(GameManager.ins.references.nabamaxLessonCost * 1.2f);
        }

        //special case for doing certain objectives 
        //objectives with no quest item
        if (encounterButton.questObjectiveDone != 0)
        {
            GameManager.ins.specialVariables.haveCompletedQuestObjective.Add(encounterButton.questObjectiveDone);
        }
    }

    public void FailedSkillCheckAdditionalEffects()
    {
        EncounterButton encounterButton = encounterHandler1.buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                encounterButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        //lets put the button exhaust here now
        if (encounterButton.getsTakenTemporarily == true)
        {
            //GameManager.ins.references.currentStrategicEncounter.encounter.buttons[encounterButton.originalButtonNumber].GetComponent<EncounterButton>().isTakenTemporarily = true;
            GameManager.ins.references.currentEncounter.buttons[encounterButton.originalButtonNumber].GetComponent<EncounterButton>().isTakenTemporarily = true;
        }

        if (encounterButton.getsTakenPermanently == true)
        {
            GameManager.ins.references.currentStrategicEncounter.removeExhaustableButtons[encounterButton.originalButtonNumber] = true;
        }

        //removes encounter if this is checked
        if (encounterButton.removeEncounterOnFailure && GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        {
            RemoveEncounter(0, false);
        }

        //cancels forced encounter if this is checked
        if (encounterButton.removeForcedOnFailure)
        {
            encounterHandler1.forcedEncounterCancelled = true;
        }

        //4 poisons
        if (encounterButton.specialEffectOnFail == 1)
        {
            //check for sentinel
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                encounterHandler1.encounter1Text.text += "\nyou gain 4 stacks of poison";

                //4 poisons
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, 4);
            }
        }

        //disenchant done
        if (encounterButton.specialEffectOnFail == 3)
        {
            encounterHandler1.disenchantDone = true;
        }

        //charm done
        if (encounterButton.specialEffectOnFail == 4)
        {
            encounterHandler1.charmDone = true;
        }

        //charm animal done
        if (encounterButton.specialEffectOnFail == 5)
        {
            encounterHandler1.charmAnimalDone = true;
        }

        //1 curse
        if (encounterButton.specialEffectOnFail == 7)
        {
            encounterHandler1.encounter1Text.text += "\nyou gain 1 stack of curse";

            //1 curse
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
        }

        //1 turn sleep
        if (encounterButton.specialEffectOnFail == 10)
        {
            encounterHandler1.encounter1Text.text += "\nsleep for 1 turn";

            //1 sleep
            //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 2, 1);

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 1);
            }
        }
    }

    public void ResetEncounterVariables(bool resetHeroTimers)
    {
        // why ?
        // reset hero cooldown abilities too
        if (resetHeroTimers == true)
        {
            CardHandler.ins.ResetHeroCombatCards(GameManager.ins.turnNumber);
        }
        

        //could reset combat info here
        gameObject.GetComponent<CombatHandler>().hitsDone = 0;
        gameObject.GetComponent<CombatHandler>().hitsTaken = 0;
        gameObject.GetComponent<CombatHandler>().heroKnockedOut = false;
        gameObject.GetComponent<CombatHandler>().opponentDefeated = false;
        gameObject.GetComponent<CombatHandler>().removeFoeFromBattleOnly = false;

        //reset these too?
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeDefensePhase = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = false;

        //reset these variables as well
        encounterHandler1.skinningDone = false;
        encounterHandler1.trophyHuntingDone = false;
        encounterHandler1.disenchantDone = false;
        encounterHandler1.charmDone = false;
        encounterHandler1.charmAnimalDone = false;
        encounterHandler1.forcedEncounterCancelled = false;

        //should reset these also
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetHeroAttackFlags();
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetHeroDefenseFlags();

    }

    //[PunRPC]
    public void SetSecondPhaseEncounter(int foeNumber, bool isSecondWind)
    {
        EncounterHandler encounterHandler = encounterHandler1;
        resurgingFoeNumber = foeNumber;

        //encounterHandler.encounterOptions.SetActive(true);
        EncounterButton usedButton = null;

        //kinda dumb way of doing this
        if(foeNumber == 1)
        {
            usedButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
        }
        if (foeNumber == 2)
        {
            usedButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
        }
        if (foeNumber == 3)
        {
            usedButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
        }

        encounterHandler.encounterOption = usedButton.secondPhase.GetComponent<Encounter2>();//exploreLocationEncounters[locationNumberChosen].GetComponent<Encounter2>();

        if (foeNumber == 1)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1 = encounterHandler.encounterOption;
        }
        if (foeNumber == 2)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter2 = encounterHandler.encounterOption;
        }
        if (foeNumber == 3)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter3 = encounterHandler.encounterOption;
        }

        //take info of first foe as well
        //why do we want to change foe encounter?
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1 = encounterHandler.encounterOption;
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ResetFoeStats(foeNumber);

        //initially lets only show stats of first combat encounter
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1.ShowEnemyStats();
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1].thisStrategicEncounter.encounter.ShowEnemyStats();
        encounterHandler.encounterOption.ShowEnemyStats();

        //need to reset daze, so it wont carry over for second phase (would give bonus to second form otherwise?
        //GameManager.ins.references.currentStrategicEncounter.isDazed = false;
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1].thisStrategicEncounter.isDazed = false;

        //also clear all previous foe cards
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ClearSpecificFoeAbilities(foeNumber);

        //then Draw new ones
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().DrawFoeCards(foeNumber);

        //could make general method to reset general encounter variables
        //is this needed here?
        ResetEncounterVariables(false);

        //set the phase to 0 temporarily
        CardHandler.ins.phaseNumber = 0;

        encounterHandler1.encounter1Text.text = "";

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            encounterHandler1.encounter1Text.text += "<br><br><br><br><br><br><br><br>";
        }

        //display start text
        encounterHandler.encounter1Text.text += encounterHandler.encounterOption.startText;

        //display enemy stats, if there are any
        //enemy hp reset
        //encounterHandler.encounterOption.currentEnergy = encounterHandler.encounterOption.maxEnergy;
        //encounterHandler.encounterOption.attack = encounterHandler.encounterOption.maxAttack;
        //encounterHandler.encounterOption.arcanePower = encounterHandler.encounterOption.maxArcanePower;
        //encounterHandler.encounterOption.ShowEnemyStats();

        //draw the abilities if there are any
        //encounterHandler.encounterOption.NewFoeAbilities();

        //play the audioclip of the effect chosen, if there is any
        //otherwise play one of the default sfx
        if (encounterHandler.encounterOption.discoverySfx != null)
        {
            sfxHolder.clip = encounterHandler.encounterOption.discoverySfx;
            sfxHolder.Play();
        }
        else
        {
            //Invoke("PlayEncounterSfx", 0f);
        }

        //start new button spawn system here
        encounterHandler.encounterOption.SpawnButtons();

        //need to set new icons too (why wasnt this here in the first place tho?)
        encounterHandler.SetFoeIcon(encounterHandler.encounterOption.icon);

        //actually in v0.5.7. we wanna trigger the continue button automatically
        //encounterHandler.encounterOption.buttons[0].GetComponent<Button>().onClick.Invoke();

        //dont autocontinue with second wind
        if (isSecondWind == false)
        {
            Invoke(nameof(SecondPhaseContinue), 1.5f);
        }

        //lets test this for v0.7.1. to show correct stats
        //currently only quiron should call this?
        //bit of a hack tho, should fix the showenemystats -method instead
        else
        {
            GameManager.ins.references.currentEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber-1].thisStrategicEncounter.encounter.combatButton.secondPhase.GetComponent<Encounter2>();
            GameManager.ins.references.currentEncounter.ShowEnemyStats();
        }
    }

    //actually this is now used only for respawns
    public void SecondPhaseContinue()
    {
        //encounterHandler1.encounterOption.buttons[0].GetComponent<Button>().onClick.Invoke();
        encounterHandler1.buttonHolder.transform.GetChild(0).GetComponent<Button>().onClick.Invoke();

        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[resurgingFoeNumber - 1];

        int spot = battlefieldFoe.spot;
        //battlefieldFoe.SetBattlefieldFoe(spot);
        battlefieldFoe.SetSecondPhaseFoe(spot);
        battlefieldFoe.SetHealthBarValues();

        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget -1].SetBattlefieldFoe();
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].SetHealthBarValues();
    }

    //might add more stats here later
    public void ResetFoeStats()
    {
        GameManager.ins.references.currentStrategicEncounter.maxEnergy = encounterHandler1.encounterOption.maxEnergy;
        GameManager.ins.references.currentStrategicEncounter.maxAttack = encounterHandler1.encounterOption.maxAttack;
        GameManager.ins.references.currentStrategicEncounter.maxArcanePower = encounterHandler1.encounterOption.maxArcanePower;

        //advanced
        if (GameManager.ins.startingDifficulty == 3)
        {
            GameManager.ins.references.currentStrategicEncounter.maxEnergy = (int)(encounterHandler1.encounterOption.maxEnergy * 1.25f);
        }
        //expert
        if (GameManager.ins.startingDifficulty == 4)
        {
            GameManager.ins.references.currentStrategicEncounter.maxEnergy = (int)(encounterHandler1.encounterOption.maxEnergy * 1.50f);

            if(encounterHandler1.encounterOption.maxAttack > 0)
            {
                GameManager.ins.references.currentStrategicEncounter.maxAttack = encounterHandler1.encounterOption.maxAttack + 1;
            }
            if (encounterHandler1.encounterOption.maxArcanePower > 0)
            {
                GameManager.ins.references.currentStrategicEncounter.maxArcanePower = encounterHandler1.encounterOption.maxArcanePower + 1;
            }
        }

        GameManager.ins.references.currentStrategicEncounter.currentEnergy = GameManager.ins.references.currentStrategicEncounter.maxEnergy;
        GameManager.ins.references.currentStrategicEncounter.attack = GameManager.ins.references.currentStrategicEncounter.maxAttack;
        GameManager.ins.references.currentStrategicEncounter.arcanePower = GameManager.ins.references.currentStrategicEncounter.maxArcanePower;
    }

    //deprecated since v0.5.7.
    //used for foe specials
    //rly shouldnt be an rpc call
    //[PunRPC]
    public void ContinueFromSpecialSkillCheck()
    {
        //lets do resource cost check here also (need for shield of isolore special at least)
        if(encounterHandler1.specialButtons.Count > 1)
        {
            //note that for this purpose the favor cost button must be at slot 2
            //also this assumes same effect for both success and failure
            if (encounterHandler1.specialButtons[1].GetComponent<EncounterButton>().effectType.Length > 0)
            {
                if (encounterHandler1.specialButtons[1].GetComponent<EncounterButton>().effectType[0] == 6)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, encounterHandler1.specialButtons[1].GetComponent<EncounterButton>().effectQty[0]);
                }
            }
        }

        //throw curse
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 2)
        {
            //cursed on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //1 curse
                //theres a stat update in this method as well (in case of curse lowering stats)
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
            }

            /*return default icon
            encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = false;
            //set this to true, so foe specials wont reduce duration a second time
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().foeSpecialUsed = true;
            */

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                return;
            }
        }

        //shadow bolt & shadow strike
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 6)
        {
            //return default icon
            //encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

            //curses if any hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();

                //actually this is not additional attack anymore
                //the go to attack phase check could be put at the end of method
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
            }
        }

        //normal alternate attacks
        //bombardment & power strike & precise strike etc
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 8)
        {
            //return default icon
            //encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

            //if hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 2, 1);
            }
            //what if we dont reset hits "taken" on this case?
            //we want the inflict abilities checked anyway in the following CheckSpecialFoeHitAbilities() method
            //ResetFoeSpecials();

            //should call this, in case enemy stats have changed
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

            //reset reroll cost
            GetComponentInChildren<EncounterHandler>().rerollCost = 1;
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken = 0;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone = 0;

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = false;
            //set this to true, so foe specials wont reduce duration a second time
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().foeSpecialUsed = true;

        }

        //confusion
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 9)
        {
            //confused on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //4 turns sleep
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 7, 4);

                //add sleep overlay
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().sleepOverlay.SetActive(true);

                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //finish battle?
                    //encounterHandler1.buttonChosen2.FinishBattleButton();
                    //or rather flee from foe for now
                    encounterHandler1.buttonChosen2.FleeButton();

                    ResetFoeSpecials();
                    return;
                }
            }

            ResetFoeSpecials();

            //confused on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == true)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //return to foe attack phase
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                    return;
                }
            }
        }

        //toxic vial
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 11)
        {
            //return default icon
            //encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

            //inflicts poison
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                //special case for sentinel
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
                {
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
                }
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //actually this is not additional attack anymore
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
            }
        }

        //ensnaring roots
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 12)
        {
            //ensnared on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //2 stacks of ensnaring roots
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 171, 7, 2);
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                return;
            }
        }

        //immolating alternate attacks
        //greater fireball, fireball etc
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 13)
        {
            //return default icon
            //encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

            //if hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 173, 7, GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();

                //actually this is not additional attack anymore
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
            }
        }

        //2 round web
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 14)
        {
            //ensnared on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //2 stacks of ensnaring roots
                //dont need stat update, since its done in the drawcards method
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 176, 7, 2);
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();

                return;
            }
        }

        //3 round web
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 15)
        {
            //ensnared on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //2 stacks of ensnaring roots
                //dont need stat update, since its done in the drawcards method
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 176, 7, 3);
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                return;
            }
        }

        //1 round petrified, 5 damage
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 16)
        {
            //ensnared on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //2 stacks of stone curse
                //but not if hero has smoke screen or berserk
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().berserkActivated == true ||
                   GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated == true) //||                    )
                {
                    //do nothing
                }
                else
                {
                    //dont need stat update, since its done in the drawcards method
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 182, 7, 1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -5);

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
                    {
                        //need to set this?
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = false;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
                        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);

                        return;
                    }
                }
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();

                Debug.Log("petrified activated, not additional attack tho");
            }
        }

        //frostbite
        //frostbreath etc
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 17)
        {
            //return default icon
            //encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

            //if hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 185, 7, GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
            }

            ResetFoeSpecials();
        }

        //1 round petrified, 1 stack of curse, 5 damage
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 18)
        {
            //ensnared on skillcheck failure
            if (encounterHandler1.skillCheckSuccess == false)
            {
                //2 stacks of stone curse, 1 stack of curse
                //but not if hero has smoke screen or berserk
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().berserkActivated == true ||
                   GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated == true) //||                    )
                {
                    //do nothing
                }
                else
                {
                    //dont need stat update, since its done in the drawcards method
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 182, 7, 1);
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -5);

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
                    {
                        //need to set this?
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = false;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
                        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);

                        return;
                    }
                }
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();

                Debug.Log("petrified activated, not additional attack tho");
            }
        }

        //greater shadow bolt
        if (encounterHandler1.specialButtons[0].GetComponent<EncounterButton>().specialEffectOnFail == 19)
        {
            //curses if any hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
            }

            ResetFoeSpecials();

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                //return to foe attack phase
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();

                //actually this is not additional attack anymore
                //the go to attack phase check could be put at the end of method
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();

                return;
            }
        }

        //only apply certain effects once per combat round for foes (such as regeneration, decays)
        //returns true, if foe was defeated
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().OncePerRoundFoeEffects() == true)
        {
            return;
        }

        //give turn to player, if all foes have acted
        if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 2) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 3))
        {
            //checks foe hit taken and hit done abilities, hero takes damage per hitstaken
            //if true, hero or foe was defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingAttackPhase() == true)
            {
                return;
            }

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn = 1;

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //enemies strike first, if any of them has alert
            //and if hero doesnt have swift mount
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
        }

        //otherwise give turn to next foe
        else if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 2))
        {
            //checks foe hit taken and hit done abilities, hero takes damage per hitstaken
            //if true, hero or foe was defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingDefensePhase() == true)
            {
                return;
            }

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //need to do this elsewhere, since this button gets deleted
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
        }
    }

    //dunno if used in v0.7.1.?
    void ResetFoeSpecials()
    {
        //return default icon
        //encounterHandler1.ShowIcon3(encounterHandler1.encounterOption.icon);

        //should call this, in case enemy stats have changed
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

        //reset reroll cost
        GetComponentInChildren<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken = 0;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone = 0;

        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = false;
        //set this to true, so foe specials wont reduce duration a second time
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().foeSpecialUsed = true;
    }

    //returns to normal interaction main options
    public void BacktrackButton()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke("BacktrackContinued", 0.3f);
    }

    void BacktrackContinued()
    {
        PV.RPC("RPC_BacktrackContinued", RpcTarget.AllBufferedViaServer);
    }

    //used for foe specials
    [PunRPC]
    void RPC_BacktrackContinued()
    {
        //since this is also used for trading backtrack, lets add these here
        encounterHandler1.tradingHolder.SetActive(false);
        encounterHandler1.tradingBackButton.SetActive(false);

        //should destroy the trading canvas objects
        RemoveTradingCards();

        //set encounter phase to cards
        CardHandler.ins.SetUsables(7);

        //should keep the old desciption this way? (at least for now..)
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().NewInteraction(locationNumber, 2, "");

        //swap encounteroption back to the original one
        encounterHandler1.encounterOption = GameManager.ins.references.currentStrategicEncounter.encounter;

        //lets try use this for now
        SetStrategicEncounter(false);
    }

    public void RemoveTradingCards()
    {
        int numberOfCards = StoreHandler.ins.storeCardArea.gameObject.transform.childCount;

        for (int i = numberOfCards; i > 0; i--)
        {
            Destroy(StoreHandler.ins.storeCardArea.transform.GetChild(i - 1).gameObject);
        }
    }

    public void RemoveCombatCards()
    {
        int numberOfCards = GameManager.ins.combatCardArea.gameObject.transform.childCount;

        for (int i = numberOfCards; i > 0; i--)
        {
            Destroy(GameManager.ins.combatCardArea.transform.GetChild(i - 1).gameObject);
        }
    }

    //counts total available encounters for a location (all available destinations)
    //this needs remaking for v90
    public void CountTotalAvailableEncounters(int nodeNumber)
    {
        /*
        int totalEncounters = 0;

        //need this check for now
        if (exploreLocationEncounters[nodeNumber] != null)
        {
            for (int i = 0; i < exploreLocationEncounters[nodeNumber].GetComponent<Encounter2>().buttons.Count; i++)
            {
                EncounterButton encounterButton = exploreLocationEncounters[nodeNumber].GetComponent<Encounter2>().buttons[i].GetComponent<EncounterButton>();

                if (encounterButton.continueEncounters.Count != 0 && encounterButton.isTaken == false)
                {
                    totalEncounters += encounterButton.CountAvailableEncounters();
                }
            }
        }
        //update the available encounters display for the desired node
        //note that this should show 0, if the exploreLocationEncounters slot is null
        GameManager.ins.references.nodes[nodeNumber].UpdateAvailableEncountersCounter(totalEncounters);
        */
    }

    //used for foe specials
    [PunRPC]
    void RPC_FleeFromCombat()
    {
        PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //need to delay this, because the new button gets stored by rpc call which is slow
        Invoke("DelayedFlee", 0.4f);


        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
    }

    void DelayedFlee()
    {
        encounterHandler1.encounter1Text.text = "<br><br><br><br><br><br><br><br><size=16>Flee</size>\n<size=8>\n</size><color=#FFD370>You flee from the foe.";

        characterDisplays.GetComponent<CharacterDisplays>().ShowCharacter(GameManager.ins.turnNumber, 11);

        //remove targetting window
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
        GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
        GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
        GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();

        //encounterHandler1.buttonChosen2.CloseEnemyStatDisplay();
        encounterHandler1.enemyInfoButton.SetActive(false);
        encounterHandler1.forcedEncounterCancelled = true;

        //need to do this here?
        CardHandler.ins.RemoveCombatEffectsFromHero();

        //should remove small icons
        gameObject.GetComponent<MultiCombat>().RemoveSmalIcons();

        //spawn the encounter buttons again (now modified version)
        encounterHandler1.SpawnFleeButtons();
    }

    //removes current encounter
    //leave foenumber 0, if  its not a foe
    //if isMidcombat == true, foe cards are not rotated and no encoutner buttons called etc
    public void RemoveEncounter(int foeNumber, bool isMidCombat)
    {
        //int chosenINodeNb = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.chosenInternalNode;
        //InternalNode currentINode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[chosenINodeNb];

        //lets try this
        InternalNode currentINode = GameManager.ins.references.currentStrategicEncounter.internalNode;

        //actually we need this somewhere, doesnt matter if foe number goes negative on non-combat encounters?
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;

        if (foeNumber != 0)
        {
            currentINode = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1].thisStrategicEncounter.internalNode;
        }

        //update random encounter count
        //dunno why were still keeping this (although might help with the save handling)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>() != null &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved == false)
        {
            //lets use different system for overmap and minimap nodes
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isOvermapNode == false)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().randomEncounterCount > 0 && GameManager.ins.references.currentStrategicEncounter.dontReduceEncounterCount == false)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().randomEncounterCount -= 1;

                    //remove the originally generated number from the list
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Remove(currentINode.transform.GetChild(0).GetComponent<StrategicEncounter>().originalListNumber);

                    //also update the location node encounter count (or sub-area transit count)
                    //perhaps need to do this here, so it doesnt get too many times?
                    if (GameManager.ins.references.currentMinimap != null)
                    {
                        if (GameManager.ins.references.currentMinimap.isSubArea == true && GameManager.ins.references.currentMinimap.availableEncounters > 0)
                        {
                            //dont need to update encounter plate, since its updated if/when the previous area is loaded
                            GameManager.ins.references.currentMinimap.availableEncounters -= 1;
                        }
                        else if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.availableEncounters > 0)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.availableEncounters -= 1;
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.UpdateEncounterPlate();
                        }
                    }
                }
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isOvermapNode == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().strategicEncounterCount -= 1;

                //remove the originally generated number from the list
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Remove(currentINode.transform.GetChild(0).GetComponent<StrategicEncounter>().originalListNumber);

            }

            //use this flag so that the counters cant be reduced several times, on multi-stage encounters
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved = true;
        }

        //make combat adjustments here
        //note that code only goes here, if theres still more foes left
        //in midcombat, we dont swap card ownerships, or draw new buttons etc
        if (isMidCombat == true)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;

            //these should be done here too?
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved = false;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = false;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().removeFoeFromBattleOnly = false;
        }
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().originalNumberOfFoes -= 1;

            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RemoveFirstFoe();
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RemoveSecondFoe();
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RemoveThirdFoe();
            }
        }
        

        if (currentINode.transform.childCount > 0)
        {
            currentINode.isTaken = false;

            if (currentINode.transform.childCount > 1)
            {
                Destroy(currentINode.transform.GetChild(1).gameObject);
            }
            if (currentINode.transform.childCount > 0)
            {
                Destroy(currentINode.transform.GetChild(0).gameObject);
            }
        }
    }

    //removes current foe from battle only
    public void RemoveFoeFromBattleOnly()
    {
        //make combat adjustments here
        //note that code only goes here, if theres still more foes left
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().originalNumberOfFoes -= 1;

            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RemoveFirstFoe();
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RemoveSecondFoe();
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RemoveThirdFoe();
            }
        }
    }

    public void ChangeMusic()
    {
        //handles music changes
        if (encounterHandler1.encounterOption.ambientSfxDay != null && Clock.clock.isNight == false)
        {
            encounterMusicHolder.volume = 0;
            encounterMusicHolder.clip = encounterHandler1.encounterOption.ambientSfxDay;
            //encounterMusicHolder.volume = encounterHandler.encounterOption.ambientSfxDayVolume;
            //GameManager.ins.references.soundManager.mainMusicHolder.volume = 0;
            GameManager.ins.references.soundManager.mainMusicHolder.Stop();
            //GameManager.ins.references.soundManager.ChangeMusicVolume(3f, encounterHandler.encounterOption.musicVolume);
            GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, encounterHandler1.encounterOption.ambientSfxDayVolume);
            encounterMusicHolder.Play();
        }
        if (encounterHandler1.encounterOption.ambientSfxNight != null && Clock.clock.isNight == true)
        {
            encounterMusicHolder.volume = 0;
            encounterMusicHolder.clip = encounterHandler1.encounterOption.ambientSfxNight;
            GameManager.ins.references.soundManager.mainMusicHolder.Stop();
            GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, encounterHandler1.encounterOption.ambientSfxNightVolume);
            encounterMusicHolder.Play();
        }
    }

    //special method for some fixed encounters (such as some multi-bosses)
    public void PlayNodeMusic()
    {
        //handles music changes
        //lets only use "day" music for now
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.ambientSfxDay != null)
        {
            encounterMusicHolder.volume = 0;
            encounterMusicHolder.clip = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.ambientSfxDay;
            GameManager.ins.references.soundManager.mainMusicHolder.Stop();
            GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.ambientSfxDayVolume);
            encounterMusicHolder.Play();
        }
    }

    public void PlayLocationMusic()
    {
        Node node = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode;

        //actually bring overmaps music, if youre not exploring
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == false)
        {
            encounterMusicHolder.Stop();
            ChangeToMapMusic();
        }

        else
        {
            //handles music changes
            if (Clock.clock.isNight == false)
            {
                AudioClip audio = null;

                if (GameManager.ins.references.currentMinimap.ambientSfxDay != null)
                {
                    audio = GameManager.ins.references.currentMinimap.ambientSfxDay;

                    //if (audio != encounterMusicHolder.clip)
                    if (!ReferenceEquals(audio, encounterMusicHolder.clip) || encounterMusicHolder.isPlaying == false)
                    {
                        Debug.Log("is day, and is supposed to change to location minimap music");

                        encounterMusicHolder.volume = 0;
                        encounterMusicHolder.clip = audio;
                        GameManager.ins.references.soundManager.mainMusicHolder.Stop();
                        GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, GameManager.ins.references.currentMinimap.ambientSfxDayVolume);
                        encounterMusicHolder.Play();
                    }
                }

                else if (node.ambientSfxDay != null)
                {
                    audio = node.ambientSfxDay;

                    Debug.Log("is day, and is supposed to change to location node music");

                    if (!ReferenceEquals(audio, encounterMusicHolder.clip) || encounterMusicHolder.isPlaying == false)
                    {
                        encounterMusicHolder.volume = 0;
                        encounterMusicHolder.clip = audio;
                        GameManager.ins.references.soundManager.mainMusicHolder.Stop();
                        GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, node.ambientSfxDayVolume);
                        encounterMusicHolder.Play();
                    }
                }
            }

            else if (Clock.clock.isNight == true)
            {
                AudioClip audio = null;

                if (GameManager.ins.references.currentMinimap.ambientSfxNight != null)
                {
                    audio = GameManager.ins.references.currentMinimap.ambientSfxNight;

                    if (!ReferenceEquals(audio, encounterMusicHolder.clip) || encounterMusicHolder.isPlaying == false)
                    {
                        encounterMusicHolder.volume = 0;
                        encounterMusicHolder.clip = audio;
                        GameManager.ins.references.soundManager.mainMusicHolder.Stop();
                        GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, GameManager.ins.references.currentMinimap.ambientSfxNightVolume);
                        encounterMusicHolder.Play();
                    }
                }

                else if (node.ambientSfxNight != null)
                {
                    audio = node.ambientSfxNight;

                    if (!ReferenceEquals(audio, encounterMusicHolder.clip) || encounterMusicHolder.isPlaying == false)
                    {
                        encounterMusicHolder.volume = 0;
                        encounterMusicHolder.clip = audio;
                        GameManager.ins.references.soundManager.mainMusicHolder.Stop();
                        GameManager.ins.references.soundManager.ChangeEncounterMusicVolume(3f, node.ambientSfxNightVolume);
                        encounterMusicHolder.Play();
                    }
                }
            }
        }
    }

    //need to call this here because the button which calls this gets deleted
    //spanws new strategic encounter, and ends old one (old one should be removed from board alrdy)
    public void SpawnNewEncounterButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        int continueEncounter = RollContinueStrategicEncounter();

        SpawnContinueStrategicEncounter(continueEncounter);

        //finally ends the original encounter
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
    }

    //need to call this here because the button which calls this gets deleted
    //spawns new strategic encounter, and ends old one (old one should be removed from board alrdy)
    //also adds it to the nodeencounterhandler permanently
    public void SpawnNewEncounterPermanently(int specialEffect)
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        int continueEncounter = 0;

        //special case for spawning first encounter in nodeencounterhandler and adding it
        if (specialEffect == 42)
        {
            continueEncounter = 0;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(0);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<NodeEncounterHandler>().randomEncounterCount += 1;

            if (GameManager.ins.references.currentMinimap.isSubArea == true)
            {
                GameManager.ins.references.currentMinimap.availableEncounters += 1;
            }
            else
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.availableEncounters += 1;
            }
        }

        SpawnContinueStrategicEncounter(continueEncounter);

        //finally ends the original encounter
        //dont rly need this right now, since return to previous is called also atm
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
    }

    public void SpawnContinueStrategicEncounter(int encounterNumber)
    {
        PV.RPC("RPC_SpawnStrategicEncounter", RpcTarget.AllBufferedViaServer, encounterNumber);
    }


    [PunRPC]
    void RPC_SpawnStrategicEncounter(int encounterNumber)
    {
        InternalNode iNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.TellFreeSlot();

        //instantiate strawberries, and set gamemanager as parent for now
        GameObject newEncounter = Instantiate(encounterHandler1.buttonChosen2.continueEncounters[encounterNumber], transform.position, transform.rotation, iNode.gameObject.transform);
        //GameObject newNpc = Instantiate(strawberries, transform.position, transform.rotation, GameManager.ins.transform);

        //newEncounter.SetActive(true);

        //reduce size 90%
        //newNpc.transform.localScale += new Vector3(-0.9f, -0.9f, -0.9f);

        //places the agent on the next node
        newEncounter.GetComponent<StrategicEncounter>().standingOn = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;

        //then moves npc to it
        newEncounter.GetComponent<StrategicEncounter>().standingOn.NpcArrive(newEncounter);

        newEncounter.GetComponent<StrategicEncounter>().identifiedImage.SetActive(false);
        newEncounter.GetComponent<StrategicEncounter>().unIdentifiedImage.SetActive(false);

        newEncounter.GetComponent<StrategicEncounter>().dontReduceEncounterCount = true;

        //add the original number as reference
        newEncounter.GetComponent<StrategicEncounter>().originalListNumber = encounterNumber;
    }

    //for v93 portal scrolls
    public void TeleportToMapButton(int nodeNumber)
    {
        //lets try this here, instead of on unload scene
        DataPersistenceManager.instance.SaveGame();

        //play sfx
        GameManager.ins.references.sfxPlayer.PlayTeleport();

        //take reference to previous location node
        //actually lets put it as standingOn node, it will get turned into previous node in MoveCommanded method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn = GameManager.ins.references.nodes[nodeNumber];
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode = GameManager.ins.references.nodes[nodeNumber];

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = false;

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.extraNodes[extraNodeNumber].gameObject.GetPhotonView().ViewID;
        //int nodeViewID = GameManager.ins.references.nodes[nodeNumber].gameObject.GetPhotonView().ViewID;
        GameObject sendNode = GameManager.ins.references.nodes[nodeNumber].gameObject;

        //sends the nodeviewid to charcontrollers method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, 0, true, true);

        Invoke("ContinueTeleportToMapButton", 0.4f);
    }

    public void ContinueTeleportToMapButton()
    {
        GameManager.ins.camera.enabled = true;

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();
        //stop ambient sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.Stop();

        if (GameManager.ins.specialVariables.currentSceneIndex != 3)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ChangeToMapMusic();

            //unloads minimap
            SceneManager.UnloadSceneAsync(GameManager.ins.specialVariables.currentSceneIndex);
        }

        GameManager.ins.specialVariables.currentSceneIndex = 3;

        //clear location text
        GameManager.ins.references.ClearLocationText();

        //CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(1);

        GameManager.ins.DisplayMapInfo();
    }

    //used by portals and load spawns
    public void TeleportToSubArea(int sceneToGoTo, int setStartNode, bool isLoadSpawn)
    {
        if (isLoadSpawn == false)
        {
            //lets try this here, instead of on unload scene
            DataPersistenceManager.instance.SaveGame();

            //play sfx
            GameManager.ins.references.sfxPlayer.PlayTeleport();

            //set the starting node to setStartNode
            GameManager.ins.references.startingNodeNumber = setStartNode;

            //play sfx
            GameManager.ins.references.sfxPlayer.PlayButton1();
        }

        //need to do these as RPC calls later
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = true;

        int previousSceneIndex = 3;

        //only need to unload if youre at minimap
        if (GameManager.ins.specialVariables.currentSceneIndex != 3)
        {
            previousSceneIndex = GameManager.ins.specialVariables.currentSceneIndex;
        }

        //load the sub area
        SceneManager.LoadScene(sceneToGoTo, LoadSceneMode.Additive);

        GameManager.ins.specialVariables.currentSceneIndex = sceneToGoTo;


        //unloads minimap
        if (previousSceneIndex != 3)
        {
            SceneManager.UnloadSceneAsync(previousSceneIndex);
        }


        //this needs a delay?
        //actually this shouldnt be called here, since the buttons gets deleted
        //Invoke("StartLocationMusic", 0.3f);
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();

        //CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(2);
        //Invoke("ContinueTeleportToSubArea", 0.4f);
    }

    public void ContinueTeleportToSubArea()
    {
        GameManager.ins.camera.enabled = true;

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();
        //stop ambient sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.Stop();

        if (GameManager.ins.specialVariables.currentSceneIndex != 3)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ChangeToMapMusic();

            //unloads minimap
            SceneManager.UnloadSceneAsync(GameManager.ins.specialVariables.currentSceneIndex);
        }

        GameManager.ins.specialVariables.currentSceneIndex = 3;

        //clear location text
        GameManager.ins.references.ClearLocationText();

        //CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(1);

        GameManager.ins.DisplayMapInfo();
    }

    public void WhirlpoolContinueWithDelay()
    {
        Invoke("WhirlpoolContinue", 0.2f);
    }

    void WhirlpoolContinue()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponentInChildren<ABCanvas>().button1.GetComponent<Action>().TransitToSubAreaButton();
    }

    //for v0.7.0.
    public void TriggerOverlay(int overlayTriggered)
    {
        if (GameManager.ins.references.currentMinimap.triggeredOverlayNumbers.Count > 0) 
        {
            for (int i = 0; i < GameManager.ins.references.currentMinimap.triggeredOverlayNumbers.Count; i++)
            {
                if (GameManager.ins.references.currentMinimap.triggeredOverlayNumbers[i] == overlayTriggered)
                {
                    GameManager.ins.references.currentMinimap.triggeredOverlays[i].SetActive(true);
                }
            }
        }
    }
}
