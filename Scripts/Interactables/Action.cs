using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Action : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public List<Resource> resources = new List<Resource>();

    //resources types: 0 = energy, 1 = warriors, 2 = artisans, 3 = arcanists, 4 = coins, 5 = fame (6 = fame total?)
    //furthermore: 7 = quest cards, 8 = intelligence cards, 9 = artifact cards, 10 = event cards?
    public int[] resourceType;
    public int[] resourceQty;

    public int customFollowers;

    public bool isInteraction;

    public bool endsTurn;

    public GameObject objectOpened;

    //variable, to tell if you're going to return card from hand back to deck, and which type of card
    public int wantToReturn;

    //number of dicarded cards
    //public int discardNumber;

    //variable to tell what happens after certain methods are ran
    public int afterEffect;

    //variable used by card options, to see which cancel function is used
    public int cardOptionType;

    //flag variable for motivate (insider) interactions
    //also used by bot actions now
    public bool isMotivateInteraction;

    //flag variable for relayed interactions (used in wilforge)
    public bool isRelayedInteraction;

    //variables to tell how many times to run certain function(s)
    //public int counter;
    //public int counterMax;

    //for keeping track which encounter list to open initially when starting exploration
    //should be same as nodenumbe now?
    //not used?
    //public int exploreListNumber;

    //description for when interaction is used (explore or location action)
    [TextArea(5, 20)]
    public string interactionDescription;

    //for explore area buttons (used for both loading and unloading now)
    public int sceneToLoad;

    //this is only used when going into sub-areas (otherwise use sceneToLoad for unloading too)
    public int sceneToUnload;

    //action point cost for certain actions (such as returning to map etc)
    public int actionPointCost;

    //used by transit buttons
    public int extraNodeNumber;

    //the starting node for area transits (or default explore)
    public int setStartNode;

    //for v95 (moved to customtooltips, even though bit illogical)
    //public bool requiresLightstone;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (interactionDescription != "")
        {
            GameManager.ins.toolTipBackground.SetActive(true);
            GameManager.ins.toolTipText.text = interactionDescription.ToString();
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameManager.ins.toolTipText.text = "";
        GameManager.ins.toolTipBackground.SetActive(false);
    }
    
    //might need to keep this
    public void ResourceChange()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //check for possible deficits in coins or favor
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] == 4)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                {
                    //"leaves" node
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                    //GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return;
                }
            }
            if (resourceType[i] == 5)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame + resourceQty[i]) < 0)
                {
                    //"leaves" node
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                    //GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return;
                }
            }
        }

        //for other cases
        //do the resource change
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] >= 0 && resourceType[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
            }
        }

        if(actionPointCost > 0)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().actionPoints -= actionPointCost;

            int hpRegenRoll = Random.Range(1, 101);
            if (hpRegenRoll <= GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().healthRegen)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(3, actionPointCost);

                //CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[164].GetComponent<CardDisplay2>().sfx;
                //CardHandler.ins.extraSfxHolder.Play();
            }

            int energyRegenRoll = Random.Range(1, 101);
            if (energyRegenRoll <= GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energyRegen)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(0, actionPointCost);
            }

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ContinueMovePointReduction();
        }

        /*
        if (endsTurn)
        {
            //changes turn
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        }
        */
    }

    /* old junk
    public void CustomFollowers()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        {
            CloseCanvases();
        }

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

        //check for possible deficits in coins or favor
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] == 4)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                {
                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return;
                }
            }
            if (resourceType[i] == 5)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame + resourceQty[i]) < 0)
                {
                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return;
                }
            }
        }

        //add this here also just in case needed, could make own method of this later
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] >= 0 && resourceType[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
            }
            else if (resourceType[i] == 7 || resourceType[i] == 8)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(resourceType[i], resourceQty[i]);
            }
        }

        //update character energy & interaction token
        if (isInteraction)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
        }

        objectOpened.SetActive(true);

        //opens the dialog window
        objectOpened.GetComponent<FollowerDialog>().StartDialog(customFollowers, endsTurn);
    }

    //only changes resources, and does nothing else
    public void CustomResourceChange()
    {
        //add this here also just in case needed, could make own method of this later
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] >= 0 && resourceType[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
            }
            else if (resourceType[i] == 7 || resourceType[i] == 8)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(resourceType[i], resourceQty[i]);
            }
        }
    }
    */

    /* old junk
     * better do this somewhere else, for simplicity, so not needed anymore
     * //mostly for special uses only, has to be set on canvas
    public void AfterEffect()
    {
        Debug.Log("turnnumber is: " + GameManager.ins.turnNumber);

        //add this here also just in case needed, could make own method of this later
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] >= 0 && resourceType[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
            }
            else if (resourceType[i] == 7 || resourceType[i] == 8)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(resourceType[i], resourceQty[i]);
            }
        }

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
        if (isInteraction)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
        }
        //this shuts down the dialog 
        gameObject.SetActive(false);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
    }
    

    public void SecondQuestDiscard()
    {
        //update character energy & interaction token
        //better do this at aftereffect
        if (isInteraction)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
        }

        objectOpened.SetActive(true);
        objectOpened.GetComponent<Action>().OpenCardOptions();

        Debug.Log("Object opened is: " + objectOpened);

        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;

        //this shuts down this dialog 
        //GameObject.Find("Quest Discard Prompt2").SetActive(false);
    }
    */

    /* old junk
     * 
     * for choosing cards from the options canvas
    public void EndDialog()
    {
        Debug.Log("test45");
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = 0;
        //ends the players turn
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();

        //counter = 0;

        //this shuts down the dialog 
        gameObject.SetActive(false);
    }

    public void SecondIntelligenceDiscard()
    {
        //update character energy & interaction token
        //better do this at aftereffect
        if (isInteraction)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
        }

        objectOpened.SetActive(true);
        objectOpened.GetComponent<Action>().OpenCardOptions2();

        Debug.Log("Object opened is: " + objectOpened);

        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;

        //this shuts down this dialog 
        //GameObject.Find("Quest Discard Prompt2").SetActive(false);
    }

    //old
    //second aftereffect for choosing card after discarding one

    public void OpenCardOptions()
    {
        GameObject.Find("Quest Discard Prompt2").SetActive(false);

        //opens the dialog 
        objectOpened.gameObject.SetActive(true);

        resourceType[0] = 7;
        resourceQty[0] = 3;
        afterEffect = 4;

        //draws cards to the card choice canvas
        for (int i = 0; i < resourceType.Length; i++)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(resourceType[i], resourceQty[i]);
        }

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 2;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
    }

    public void OpenQuestOptions()
    {
        //disable ui buttons & displays
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.DisableAllButtons();

        if (isRelayedInteraction == false)
        {
            //play sfx
            GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

            CloseCanvases();
        }

        //opens the dialog 
        objectOpened.gameObject.SetActive(true);

        objectOpened.GetComponent<Action>().resourceType[0] = 7;
        objectOpened.GetComponent<Action>().resourceQty[0] = 4;
        objectOpened.GetComponent<Action>().afterEffect = 4;

        GameManager.ins.GM.RPC("RPC_DrawQuestOptions", RpcTarget.AllBufferedViaServer);

        /*draws cards to the card choice canvas
        for (int i = 0; i < objectOpened.GetComponent<Action>().resourceType.Length; i++)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(objectOpened.GetComponent<Action>().resourceType[i], objectOpened.GetComponent<Action>().resourceQty[i]);
        }

        // special case for upgraded cornville, 1 artisan.
           not used anymore
         * 
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 13 &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 1);
        }

         * lets do this at gamemanager instead, so the reward order stays coherent
         * actually i removed this there too for v82
         * 
         * special case for un-upgraded inn, 1 energy
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 2 &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == false)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 1);
        }
        //special case for upgraded inn, 1 energy and 1 warrior
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 2 &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 1);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(1, 1);
        }
        
        //reset the flag variable (only needed for wilforge atm)
        isRelayedInteraction = false;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 2;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = objectOpened.GetComponent<Action>().afterEffect;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.inter.enabled = false;
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
    }

    //second aftereffect for choosing card after discarding one
    //this might be unnecessary now
    public void OpenCardOptions2()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        GameObject.Find("Intelligence Discard Prompt2").SetActive(false);

        //opens the dialog 
        objectOpened.gameObject.SetActive(true);

        resourceType[0] = 8;
        resourceQty[0] = 3;
        afterEffect = 4;

        //draws cards to the card choice canvas
        for (int i = 0; i < resourceType.Length; i++)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(resourceType[i], resourceQty[i]);
        }

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 2;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
    }

    //for choosing 1 int card out of 3
    public void OpenIntelligenceOptions()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //opens the dialog 
        objectOpened.gameObject.SetActive(true);

        objectOpened.GetComponent<Action>().resourceType[0] = 8;
        objectOpened.GetComponent<Action>().resourceQty[0] = 4;
        objectOpened.GetComponent<Action>().afterEffect = 4;

        //draws cards to the card choice canvas
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(objectOpened.GetComponent<Action>().resourceType[0], objectOpened.GetComponent<Action>().resourceQty[0]);

        /* not used anymore
         * 
         * special case for upgraded coven & temple
        if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 6 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber == 10) &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isUpgraded == true)
        {
            //objectOpened.GetComponent<Action>().resourceType[1] = 3;
            //objectOpened.GetComponent<Action>().resourceQty[1] = 1;
            //gain additional arcanist
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 1);
        }
        

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 2;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = objectOpened.GetComponent<Action>().afterEffect;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.inter.enabled = false;
    }

    //opens blue citadel artifact offer
    public void OpenArtifactOffer()
    {
        //disable ui buttons & displays
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.DisableAllButtons();

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //GameObject.Find("Intelligence Discard Prompt2").SetActive(false);

        //check if location has bookkeepers (do this elsewhere actually)
        //CheckBookkeeping();

        //opens the dialog 
        objectOpened.gameObject.SetActive(true);

        //draws cards to the card choice canvas
        for (int i = 0; i < resourceType.Length; i++)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawCardOptions(resourceType[i], resourceQty[i]);
        }

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 2;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();

        //dont do this for straight interaction
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        {
            CloseCanvases();
        }

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();
    }

    //this has aftereffect, which can be customized
    public void OpenDiscardDialog()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

        objectOpened.SetActive(true);
        //objectOpened.GetComponentInChildren<Text>().gameObject.SetActive(true);

        //what type of card to return
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = wantToReturn;

        //special aftereffect
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;
    }

    //this opens handcard discard dialog
    public void DiscardHandCard()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        {
            CloseCanvases();
        }

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

        objectOpened.SetActive(true);

        //update text
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().handDiscardText.text = "You can discard 1 card:";

        //special cardfunction
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 4;

        //close all displays, open hand card display
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.handCardDisplay.SetActive(true);
        GameManager.ins.uiButtonHandler.DisableAllButtons();
    }

    public void OpenQuestingDialog()
    {
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.DisableAllButtons();
        //GameManager.ins.uiButtonHandler.handCardDisplay.SetActive(true);
        
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //"leaves" node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.LeaveForQuest();

        objectOpened.SetActive(true);
        //objectOpened.GetComponentInChildren<Text>().gameObject.SetActive(true);

        //what type of card to return
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = wantToReturn;

        //indicates youre doing a Quest
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 3;

        GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().EnableQuestPlates();

        //reset reroll cost
        GameObject.Find("Skill Check Handler").GetComponent<SkillCheck>().rerollCost = 1;

        //reset quest failed variable
        GameObject.Find("Quest Canvas").GetComponent<QuestingDialog>().questFailed = false;

        //show greenarrows:
        GameObject.Find("Quest Arrows").GetComponent<QuestArrows>().EnableArrowsForQuest();

        //disallow straight actions
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;

        //special aftereffect
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = afterEffect;
    }

    public void Cancel()
    {
        //enable buttons again, close displays
        GameManager.ins.uiButtonHandler.EnableAllButtons();
        GameManager.ins.uiButtonHandler.CloseAllDisplays();

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //disables Green Arrows
        GameObject.Find("Quest Arrows").GetComponent<QuestArrows>().DisableArrows();

        //in case you doing the offering quest
        if (GameManager.ins.questDeck[GameManager.ins.questCanvas.GetComponent<QuestingDialog>().numberInDeck].GetComponent<QuestCard>().specialEffect == 8)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().handCardDiscard.SetActive(false);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
            GameObject.Find("Quest Canvas").GetComponent<QuestingDialog>().QuestEnd();
            GameObject.Find("Quest Canvas").GetComponent<QuestingDialog>().numberInDeck = 0;
        }
        //for other cases
        else
        {
            //this shuts down the dialog 
            gameObject.SetActive(false);

            //takes ViewID of the node the avatar is moving to
            int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

            //adds 1 "movementBonus", to put things as they were
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().movementBonus += 1;

            //sends the nodeviewid to charcontrollers method
            //returns players "turn"
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

            //just in case
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = 0;

            //should enable straight action again?
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
        }
    }

    public void CancelArtifactOffer()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //this shuts down the dialog 
        gameObject.SetActive(false);

        //enable ui buttons & close displays
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.EnableAllButtons();

        if (cardOptionType != 11 && cardOptionType != 7)
        {
            //destroys the artifact offer cards
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                GameManager.ins.GM.RPC("RPC_DestroyArtifactOffer", RpcTarget.AllBufferedViaServer);
            }
        }

        //when using divination
        if (cardOptionType == 11)
        {
            //destroys the artifact offer cards
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                GameManager.ins.GM.RPC("RPC_DestroyArtifactOffer2", RpcTarget.AllBufferedViaServer);
            }
        }

        //when doing water of visions quest
        if (cardOptionType == 7)
        {
            //destroys the artifact offer cards
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                GameManager.ins.GM.RPC("RPC_DestroyArtifactOffer2", RpcTarget.AllBufferedViaServer);

                GameManager.ins.dialogCanvas.GetComponent<CanvasController>().questCanvas.GetComponent<QuestingDialog>().QuestEnd();
                return;
            }
        }

        //takes ViewID of the node the avatar is moving to
        int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

        //adds 1 "movementBonus", to put things as they were
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().movementBonus += 1;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

        //just in case
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().afterEffect = 0;

        //in case of motivate
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect == 2)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect = 0;
        }

        //allow straight actions again
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
    }

    public void UseIntelligenceAsAction()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().UseIntelligenceAsAction();
    }

    public void WilforgeInteraction()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //check if player alrdy has 5 agents
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().agents >= 5)
        {
            //"leaves" node
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(10);
            return;
        }

        //make sure end turn isnt enabled
        //set necessary resource changes
        if (CustomResourceChangeWithCheck() == true)
        {
            //interacts with location
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

            //lets do this on canvas controller
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().SetAgent();

            //updates agents variable on character class
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(15, 1);
        }
        else
        {
            return;
        }
        //else nothing else happens
    }

    //checks for deficits, and changes resources if enough resources
    public bool CustomResourceChangeWithCheck()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //check for possible deficits in coins or favor
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] == 4)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                {
                    //"leaves" node
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return false;
                }
            }
            if (resourceType[i] == 5)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame + resourceQty[i]) < 0)
                {
                    //"leaves" node
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return false;
                }
            }
        }
        //add this here also just in case needed, could make own method of this later
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] >= 0 && resourceType[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
            }
            else if (resourceType[i] == 7 || resourceType[i] == 8)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(resourceType[i], resourceQty[i]);
            }
        }
        return true;
    }

    //for bribing captain clavius
    public void Bail()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < 5)
        {
            //"leaves" node
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
            return;
        }
        else
        {
            //updates info on character class
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(4, -5);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ReduceJailTime(true);
        }
    }

    //for serving your sentence
    public void Wait()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //reduces jailtime, but ends turn
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ReduceJailTime(false);

    }

    public void Spy()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //makes sure it your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //check if theres valid targets for attack card
            //lets do this check here, instead of the intial method, should work hopefully
            if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CheckForValidTargets4() == false)
            {
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(11);
                return;
            }

            //opens new dialog if its your turn, although this check is alrdy done
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //open the text dialog
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomTextCanvas.SetActive(true);
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomText.text = "Choose an opponent hero to spy on:";

                //opens the hero colliders on your location, or the location of your agents
                //sends number in deck value of the first card stored
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().EnableHeroCollidersForSpying();
            }
        }
    }

    //spy & encounter interaction at once
    //encounter interaction not rly needed tho
    public void Investigate()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //makes sure it your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //check if theres valid targets for spying / encounter interaction (can be used to spy from any location now)
            if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CheckForValidTargets5() == false && GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CheckForEncounters() == false)
            {
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(20);
                return;
            }

            //opens new dialog if its your turn, although this check is alrdy done
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //open the text dialog
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomTextCanvas.SetActive(true);
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomText.text = "Choose a target to investigate:";

                //opens the hero colliders on your location, or the location of your agents
                //sends number in deck value of the first card stored
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().EnableHeroCollidersForSpying2();

                //leaves the node
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

                //opens the encounter colliders on your location
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().EnableEncounterColliders();
            }
        }
    }

    //for encounter action
    public void SpecialEncounter()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //makes sure it your turn
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //check if theres valid special encounters
            //lets do this check here, instead of the intial method, should work hopefully
            if (GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CheckForEncounters() == false)
            {
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(13);
                return;
            }

            //opens new dialog if its your turn, although this check is alrdy done
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //open the text dialog
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().encounterSelectDialog.SetActive(true);
                //GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomText.text = "Choose an encounter to interact with:";

                //leaves the node
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

                //opens the encounter colliders on your location, or the location of your agents
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().EnableEncounterColliders();
            }
        }
    }

    public void InnInteraction()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //check for possible deficits in coins or favor
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] == 4)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                {
                    //"leaves" node
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return;
                }
            }
            if (resourceType[i] == 5)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame + resourceQty[i]) < 0)
                {
                    //"leaves" node
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                    return;
                }
            }
        }

        //do the resource change
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] >= 0 && resourceType[i] <= 6)
            {
                //updates info on character class
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
            }
            else if (resourceType[i] == 7 || resourceType[i] == 8)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawHandCards(resourceType[i], resourceQty[i]);
            }
        }

        /* why was this even here?
         * 
        //draw quest on board
        GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().DrawQuestOnBoard();
        

        //update character energy & interaction token
        if (isInteraction)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
        }

        if (endsTurn)
        {
            //changes turn
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        }
    }
    */

    //this doesnt really work, but might need later?
    //additional test for wyrms blood after any action
    //tests if opponent has wyrms blood, and if he or his agents are at your location
    //initially also tests whether youre sentinel or not (sentinel isnt affected)
    public void WyrmsBloodTest()
    {
        /* dont rly need this, unless something similar is added later after v90?
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().SentinelTest() == false)
        {
            for (int i = 0; i < GameManager.ins.avatars.Count; i++)
            {
                //tests if a hero has wyrms blood, which isnt you
                if (GameManager.ins.avatars[i].GetComponent<CharController>().WyrmsBloodTest() == true && GameManager.ins.turnNumber != i)
                {
                    //test if that hero is standing on the same location as you
                    if (ReferenceEquals(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn, GameManager.ins.avatars[i].GetComponent<CharController>().standingOn))
                    {
                        //drains one energy from current player
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -2);

                        //poisoned
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                        {
                            //dont show messages for AI
                            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
                            {
                                //give message
                                string msgs = "You have been poisoned!";
                                GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                            }

                            if (GameManager.ins.avatars[i].GetComponent<CharController>().isAi == false)
                            {
                                //give poisoner a message too
                                string msgs2 = "You have poisoned " + GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + "!";
                                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_PrivateMessage", RpcTarget.AllBufferedViaServer, msgs2, i);
                            }
                        }

                    }

                    //check if theres that heros agent in your location (not rly needed anymore tho)
                    for (int y = 0; y < GameManager.ins.avatars[i].GetComponent<CharController>().agents.Count; y++)
                    {
                        if (ReferenceEquals(GameManager.ins.avatars[i].GetComponent<CharController>().agents[y].GetComponent<Agent>().standingOn, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn))
                        {
                            //drains one energy from current player
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -2);
                        }
                    }
                }
            }
        }
        */
    }

    /* old junk
    //gives int card at temple, if player has library key
    public void LibraryKeyTest()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().LibraryKeyTest() == true)
        {
            //AI player gains reputation instead
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
            }

            //human players gain int card
            else
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().DrawHandCards(8, 1);
            }
        }
    }

    public void Smithing()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //tests if player has lividium powder artifact
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 30 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                {
                    //opens the smithing dialog
                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.SetActive(true);
                    CheckSmithyItems();
                    return;
                }
            }
        }

        //else give message about not having the artifact
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(17);
    }

    //checks if the smithy items have been taken
    //uses numberindeck index number, so be careful not to change the deck !
    public void CheckSmithyItems()
    {
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.GetComponent<Crafting>().swordButton.interactable = true;
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.GetComponent<Crafting>().pickaxeButton.interactable = true;
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.GetComponent<Crafting>().shovelButton.interactable = true;

        //now uses methods from questingdialog class
        if (GameManager.ins.artifactDeck[13].GetComponent<CardDisplay>().isTaken == true && GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(13) == false)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.GetComponent<Crafting>().swordButton.interactable = false;
        }
        if (GameManager.ins.artifactDeck[14].GetComponent<CardDisplay>().isTaken == true && GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(14) == false)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.GetComponent<Crafting>().pickaxeButton.interactable = false;
        }
        if (GameManager.ins.artifactDeck[15].GetComponent<CardDisplay>().isTaken == true && GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(15) == false)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().smithingCanvas.GetComponent<Crafting>().shovelButton.interactable = false;
        }
    }

    public void Scribing()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //tests if player has lividium powder artifact
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 42 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                {
                    //opens the smithing dialog
                    GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.SetActive(true);
                    CheckTomes();
                    return;
                }
            }
        }

        //else give message about not having the artifact
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(18);
    }

    //checks if the smithy items have been taken
    //uses numberindeck index number, so be careful not to change the deck !
    public void CheckTomes()
    {
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.GetComponent<Crafting>().warriorButton.interactable = true;
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.GetComponent<Crafting>().artisanButton.interactable = true;
        GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.GetComponent<Crafting>().arcanistButton.interactable = true;

        if (GameManager.ins.artifactDeck[18].GetComponent<CardDisplay>().isTaken == true && GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(18) == false)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.GetComponent<Crafting>().warriorButton.interactable = false;
        }
        if (GameManager.ins.artifactDeck[19].GetComponent<CardDisplay>().isTaken == true && GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(19) == false)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.GetComponent<Crafting>().artisanButton.interactable = false;
        }
        if (GameManager.ins.artifactDeck[20].GetComponent<CardDisplay>().isTaken == true && GameManager.ins.questCanvas.GetComponent<QuestingDialog>().IsArtifactOnOffer(20) == false)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().scribingCanvas.GetComponent<Crafting>().arcanistButton.interactable = false;
        }
    }

    //test not needed here anymore?
    public void Foresight()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        //pay 1 energy
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(0, -1);

        //this should use cardOptionType 11 by default, which should work?
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().ShowThreeEvents();

    }
    */

    //disabled tooltip, extra buttons and ABcanvas
    public void CloseCanvases()
    {
        //closes tooltip
        GameManager.ins.toolTipBackground.SetActive(false);

    }

    public void SleepButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 7);

        GameManager.ins.GM.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 1);
        
        //changes turn
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        
    }

    //update this later for quick maintenance
    public void SelfMaintenanceButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        CloseCanvases();

        int totalGain = 5;
        int missingHealth = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxHealth - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health;
        int missingEnergy = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxEnergy - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy;
        //int coinLoss = 5;

        if ((missingHealth < 5) && (missingEnergy < 5))
        {
            if(missingHealth >= missingEnergy)
            {
                totalGain = missingHealth;
            }
            if (missingEnergy >= missingHealth)
            {
                totalGain = missingEnergy;
            }
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < totalGain)
        {
            totalGain = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins;
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins == 0)
        {
            totalGain = 1;
        }

        //sets the flag variable
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_IsSelfMaintenance", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, true);
        
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, totalGain);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, totalGain);

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -totalGain);


        if (endsTurn)
        {
            //changes turn
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        }
    }

    /* old
     * 
     * for rerolling quest offer and selecting card from there
    public void WilforgeInteraction2()
    {
        //disable ui buttons & displays
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.DisableAllButtons();

        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        {
            CloseCanvases();
        }

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

        //do this as rpc call
        GameManager.ins.GM.RPC("RPC_ClearQuestOffer", RpcTarget.AllBufferedViaServer);

        //draws 4 cards
        for (int i = 0; i < 4; i++)
        {
            //GameManager.ins.pyramidOffer.Add(TakeArtifactCard());

            int cardNumber = GameManager.ins.TakeQuestCardNumber();

            GameManager.ins.GM.RPC("RPC_QuestOffer", RpcTarget.AllBufferedViaServer, cardNumber);
        }

        isRelayedInteraction = true;

        Invoke("OpenQuestOptions", 0.3f);

        //special case for upgraded wilforge
        if (GameObject.Find("Wilforge").GetComponent<Node>().isUpgraded == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 3);
        }

        //update character energy & interaction token
        //actually this should probably be changed cause of motivate change?
        if (isInteraction)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
        }
        
        if (endsTurn)
        {
            //changes turn
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
        }
        
    }
    */

    //used when straight actions should be disabled (like when opening certain dialogs)
    public void DisableStraightAction()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;

        /*also disable the buttons
         * 
        gameObject.GetComponentInParent<Node>().DeactivateButtons();
        */
        
    }

    //for straight interaction buttons
    public void StraightResourceChange()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == true)
        {

            //play sfx
            GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

            //check for possible deficits in coins or favor
            for (int i = 0; i < resourceType.Length; i++)
            {
                if (resourceType[i] == 4)
                {
                    if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                    {
                        //"leaves" node
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                        //GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                        return;
                    }
                }
                if (resourceType[i] == 5)
                {
                    if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fame + resourceQty[i]) < 0)
                    {
                        //"leaves" node
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.Leave();

                        //GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ActionCancelled(2);
                        return;
                    }
                }
            }

            //update character energy & interaction token
            if (isInteraction)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();
            }

            //special case for upgraded oldmines
            if (gameObject.GetComponentInParent<Node>().nodeNumber == 4 && //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 7);
            }

            //special case for upgraded coven
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 10 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 2);
            }

            //special case for upgraded cornville
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 13 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 2);
            }

            //special case for upgraded forest vault
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 9 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 12);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(5, -1);
            }

            //special case for upgraded temple
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 6 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 1);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 1);
            }

            //special case for upgraded factory
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 5 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 1);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(1, 1);
            }

            //special case for upgraded smithy
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 1 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(1, 2);
            }

            //special case for upgraded inn
            else if (gameObject.GetComponentInParent<Node>().nodeNumber == 2 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(1, 1);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 1);
            }


            //for other cases
            else
            {
                //otherwise do the resource change
                for (int i = 0; i < resourceType.Length; i++)
                {
                    if (resourceType[i] >= 0 && resourceType[i] <= 6)
                    {
                        //updates info on character class
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(resourceType[i], resourceQty[i]);
                    }
                    else if (resourceType[i] == 7 || resourceType[i] == 8)
                    {
                        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().DrawHandCards(resourceType[i], resourceQty[i]);
                    }
                }
            }

            if (endsTurn)
            {
                //changes turn
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
            }
        }
    }

    /* old
     * for straight interaction buttons
    public void StraightArtifactOffer()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == true)
        {
            OpenArtifactOffer();
        }
    }
    */

    /*old junk
     * for straight interaction buttons
    public void StraightCustomFollowers()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == true)
        {
            //special case for upgraded grimhold
            if (gameObject.GetComponentInParent<Node>().nodeNumber == 7 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 1);
                customFollowers = 3;
            }

            //special case for upgraded guildhouse
            if (gameObject.GetComponentInParent<Node>().nodeNumber == 12 &&
                gameObject.GetComponentInParent<Node>().isUpgraded == true)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 1);
                resourceQty[0] = -8;
            }

            //CustomFollowers();
        }
    }

    
     * for straight interaction buttons
    public void StraightDiscardHandCard()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == true)
        {
            DiscardHandCard();
        }
    }

    //for straight interaction buttons
    public void StraightWilforgeInteraction()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == true)
        {
            WilforgeInteraction2();
        }
    }
    */

    //give sfx for all players
    public void RestSfx()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PV.RPC("RPC_PlayRest", RpcTarget.AllBufferedViaServer);
    }

    //give sfx for all players
    public void SleepSfx()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PV.RPC("RPC_PlaySleep", RpcTarget.AllBufferedViaServer);
    }

    //give sfx for all players
    public void ContemplateSfx()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PV.RPC("RPC_PlayContemplate", RpcTarget.AllBufferedViaServer);
    }

    //give sfx for all players
    public void SelfMaintenanceSfx()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PV.RPC("RPC_PlaySelfMaintenance", RpcTarget.AllBufferedViaServer);
    }

    /*old  explore action & action menu
    public void Explore()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //"leaves" node
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.DeactivateButtons();

        //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        //CloseCanvases();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().NewInteraction(exploreListNumber, 1, interactionDescription);

        //update character energy & interaction token
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

        //open usables display
        GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.HandCardsButtonPressed();

        //allow encounter cards
        CardHandler.ins.SetUsables(2);
    }
    
    //replaces all location buttons
    public void OpenActionMenu()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //"leaves" node
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

        //gameObject.GetComponentInParent<Node>().abCanvas.Activate();
    }
    */

    /*used by location default actions
    //dont rly need anymore?
    public void LocationAction()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //"leaves" node
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.DeactivateButtons();

        //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        //CloseCanvases();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().NewInteraction(exploreListNumber, 2, interactionDescription);

        //update character energy & interaction token
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

        //open usables display
        //GameManager.ins.uiButtonHandler.CloseAllDisplays();
        //GameManager.ins.uiButtonHandler.HandCardsButtonPressed();

        //allow encounter cards
        CardHandler.ins.SetUsables(2);
    }
    */

    //for v90 location explore actions
    //opens new additive scene
    public void ExploreAreaButton()
    {
        //make level check here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage != null)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage.GetComponent<CustomTooltips>().levelRequirement >
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();

                GameManager.ins.references.ShowLevelInsufficientInterruption();
                CloseCanvases();
                return;
            }
        }

        //make resource check here
        //check for possible deficits in coins
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] == 4)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                {
                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();

                    GameManager.ins.references.ShowCoinsInsufficientInterruption();
                    CloseCanvases();
                    return;
                }
                else
                {
                    //updates info on character class
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(resourceType[i], resourceQty[i]);

                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().SonyaPurchase.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();
                }
            }
        }

        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();

        //set the starting node to setStartNode
        GameManager.ins.references.startingNodeNumber = setStartNode;

        //need to do these as RPC calls later
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;

        //seems we need to save hero variables when entering location from overmap (otherwise changes made on overmap wont take effect)
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().SaveData(ref DataPersistenceManager.instance.gameData);
        DataPersistenceManager.instance.SaveGame();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = true;

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        //this works here, since the overmap buttons are not deleted
        //dunno if it needed tho, since the minimap initiates music anyway?
        //Invoke("StartLocationMusic", 0.3f);
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();

        GameManager.ins.specialVariables.currentSceneIndex = sceneToLoad;

        CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(2);

        //need to disable the button, so it cant be double clicked?
        gameObject.SetActive(false);
    }

    //for v90 location explore actions
    //returns to overmap
    public void ReturnToMapButton()
    {
        //lets try this here, instead of on unload scene
        DataPersistenceManager.instance.SaveGame();

        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = false;

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.gameObject.GetPhotonView().ViewID;

        //actually lets do it like this, so that it returns to the correct node
        //just need to make sure not to add return buttons to sub areas then
        //int nodeViewID = GameManager.ins.references.nodes[GameManager.ins.references.currentMinimap.minimapNumber].gameObject.GetPhotonView().ViewID;
        GameObject sendNode = GameManager.ins.references.nodes[GameManager.ins.references.currentMinimap.minimapNumber].gameObject;

        //sends the nodeviewid to charcontrollers method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, actionPointCost, true, true);

        //need to disable the button, so it cant be double clicked?
        gameObject.SetActive(false);

        Invoke("ContinueReturnToMapButton", 0.4f);
    }

    void ContinueReturnToMapButton()
    {
        GameManager.ins.camera.enabled = true;

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();
        //stop ambient sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.Stop();
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ChangeToMapMusic();

        //unloads minimap
        SceneManager.UnloadSceneAsync(sceneToLoad);

        GameManager.ins.specialVariables.currentSceneIndex = 3;

        //clear location text
        GameManager.ins.references.ClearLocationText();

        CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(1);

        GameManager.ins.DisplayMapInfo();
    }

    //for v90 location explore actions
    //moves to an extra node
    public void TransitToMapButton()
    {
        //make level check here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage != null)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage.GetComponent<CustomTooltips>().levelRequirement >
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();

                GameManager.ins.references.ShowLevelInsufficientInterruption();
                CloseCanvases();
                return;
            }
        }

        //lets try this here, instead of on unload scene
        DataPersistenceManager.instance.SaveGame();

        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();

        //need to save previous location node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousLocationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode;

        //take reference to previous location node
        //actually lets put it as standingOn node, it will get turned into previous node in MoveCommanded method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = false;

        //open new route
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.AddRoute(extraNodeNumber);

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.extraNodes[extraNodeNumber].gameObject.GetPhotonView().ViewID;
        GameObject sendNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.extraNodes[extraNodeNumber].gameObject;


        //sends the nodeviewid to charcontrollers method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, actionPointCost, true, true);

        //need to disable the button, so it cant be double clicked?
        gameObject.SetActive(false);

        Invoke("ContinueTransitToMapButton", 0.4f);
    }

    void ContinueTransitToMapButton()
    {
        GameManager.ins.camera.enabled = true;

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();
        //stop ambient sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.Stop();
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ChangeToMapMusic();

        //unloads minimap
        SceneManager.UnloadSceneAsync(sceneToLoad);

        GameManager.ins.specialVariables.currentSceneIndex = 3;

        //clear location text
        GameManager.ins.references.ClearLocationText();

        CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(1);

        GameManager.ins.DisplayMapInfo();
    }

    //for v94 traversing from one location to another (when both are linked to overmap)
    //not needed in all cases?
    //opens new additive scene
    public void TransitToAnotherLocationButton()
    {
        //make level check here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage != null)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage.GetComponent<CustomTooltips>().levelRequirement >
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();

                GameManager.ins.references.ShowLevelInsufficientInterruption();

                CloseCanvases();
                return;
            }
        }

        //lets try this here, instead of on unload scene
        DataPersistenceManager.instance.SaveGame();

        //set the starting node to setStartNode
        GameManager.ins.references.startingNodeNumber = setStartNode;

        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();

        //need to save previous location node
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousLocationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode;

        //take reference to previous location node
        //actually lets put it as standingOn node, it will get turned into previous node in MoveCommanded method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode;

        //open new route
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.AddRoute(extraNodeNumber);

        //takes ViewID of the location node the avatar is moving to
        //int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.extraNodes[extraNodeNumber].gameObject.GetPhotonView().ViewID;
        GameObject sendNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.extraNodes[extraNodeNumber].gameObject;

        //sends the nodeviewid to charcontrollers method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, actionPointCost, true, false);
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = true;

        //there should be delay here, so theres not 2 moves at the same time
        Invoke("ContinueTransitToAnotherLocation", 0.3f);

        //need to disable the button, so it cant be double clicked?
        gameObject.SetActive(false);

    }

    void ContinueTransitToAnotherLocation()
    {
        //load the sub area
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        GameManager.ins.specialVariables.currentSceneIndex = sceneToLoad;

        //unloads minimap
        SceneManager.UnloadSceneAsync(sceneToUnload);

        CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(2);
    }

    //for v90 traversing to sub area action
    //opens new additive scene
    public void TransitToSubAreaButton()
    {
        //make level check here
        //actually this doesnt work in v0.5.7.
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage != null)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage.GetComponent<CustomTooltips>().levelRequirement >
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();

                GameManager.ins.references.ShowLevelInsufficientInterruption();

                CloseCanvases();
                return;
            }

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.encountersPlateImage.GetComponent<CustomTooltips>().lightstoneRequirement == true &&
                CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 295, 5) == 0)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();

                GameManager.ins.references.ShowLightstoneInterruption();

                CloseCanvases();
                return;
            }
        }

        //make resource check here
        //check for possible deficits in coins
        for (int i = 0; i < resourceType.Length; i++)
        {
            if (resourceType[i] == 4)
            {
                if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins + resourceQty[i]) < 0)
                {
                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();

                    GameManager.ins.references.ShowCoinsInsufficientInterruption();
                    CloseCanvases();
                    return;
                }
                else
                {
                    //updates info on character class
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().UpdateResources(resourceType[i], resourceQty[i]);

                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().SonyaPurchase.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();
                }
            }
        }

        //lets try this here, instead of on unload scene
        DataPersistenceManager.instance.SaveGame();

        //set the starting node to setStartNode
        GameManager.ins.references.startingNodeNumber = setStartNode;

        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();

        //need to do these as RPC calls later
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring = true;

        //load the sub area
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

        //for keeping hero invisible during transit
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Canvas>().enabled = false;
        GameManager.ins.keepHeroInvisible = true;

        //lets set this to null, so that we dont use wrong start node when returning to the original area
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousLocationNode = null;

        GameManager.ins.specialVariables.currentSceneIndex = sceneToLoad;

        //unloads minimap
        SceneManager.UnloadSceneAsync(sceneToUnload);

        //this needs a delay?
        //actually this shouldnt be called here, since the buttons gets deleted
        //Invoke("StartLocationMusic", 0.3f);
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();

        CloseCanvases();

        //allow encounter cards
        CardHandler.ins.SetUsables(2);

        //need to disable the button, so it cant be double clicked?
        gameObject.SetActive(false);
    }


    //when redoing interaction on a node
    public void InteractButton()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.PlayButton1();

        CloseCanvases();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.RefreshEncounters();

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.gameObject.GetPhotonView().ViewID;
        GameObject sendNode = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.gameObject;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().interactWithAnyEncounter = true;

        //sends the nodeviewid to charcontrollers method
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, 0, false, false);

        //need to disable the button, so it cant be double clicked?
        gameObject.SetActive(false);
    }

    //bad idea to use this, since button gets deleted
    public void StartLocationMusic()
    {
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();
    }

    public void EnterCitadel()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().EnterCitadelWithDelay();
    }
}
