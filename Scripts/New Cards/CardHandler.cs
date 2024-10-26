using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using TMPro;
using Photon.Pun.UtilityScripts;
using UnityEngine.EventSystems;

//for handling card draws etc
public class CardHandler : MonoBehaviour
{
    public PhotonView PV;

    //singleton
    public static CardHandler ins;

    //need copy of this?
    public List<GameObject> generalDeck = new List<GameObject>();

    //keep track of which gamestage were on (for card activation purposes)
    //phaseEntered: 0=disable all, 1=overmap, 2=in location, 3=attack, 4=defense, 5=selling, 6=buying, 7=in encounter
    public int phaseNumber;

    //use this as reference to an audiosource to replace
    public AudioSource intelligenceSfxHolder;

    //lets add another, if the first two isnt enough
    public AudioSource extraSfxHolder;

    //equipment slot canvases
    public GameObject helmSlot;
    public GameObject armorSlot;
    public GameObject ringSlot;
    public GameObject weaponSlot;
    public GameObject mountSlot;
    public GameObject miscSlot1;
    //public GameObject miscSlot2;
    //public GameObject miscSlot3;
    public GameObject gogglesSlot;
    public GameObject maskSlot;
    public GameObject amuletSlot;
    public GameObject tomeSlot;
    public GameObject toolboxSlot;
    public GameObject shovelSlot;

    //actually better put specials on the same deck too?
    //public List<GameObject> specialDeck = new List<GameObject>();

    //flag variable for keeping track whether equipping is done (and whether you can do it again)
    public bool cardChangeInProgress;

    //used by combat card effects
    public AudioClip tempSfx;

    // Start is called before the first frame update
    void Start()
    {
        //get the reference
        PV = GetComponent<PhotonView>();

        // very bad singleton
        ins = this;

        //copy this
        generalDeck = GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>().generalDeck;
    }

    //handles all card placements fomr general deck on usable & passive displays
    //holdertype 1= usables, 2= passive abilities, 4= foe cards, 5= equipment, 7= effects, 11= objectives
    //lets just use this as regular method, untill multiplayer is added, and worry about it later
    public void DrawCards(int turnNumber, int cardNumber, int holderType, int quantity)
    {
        /*perhaps make turnnumber check here
         * Actually no, cause of foe cards
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }
        */

        //make exception for poison here (dont work on sentinel)
        if (cardNumber == 23)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == true)
            {
                return;
            }
        }

        //make exception for curses here (dont work if hero is wearing ring of moon goddess)
        if (cardNumber == 28)
        {
            if (ins.CheckItemInSlot(GameManager.ins.turnNumber, 3, 57) == true)
            {
                return;
            }
        }

        //make exception for web, entangle, frostbitten here (dont work if hero is in wraithform)
        if (cardNumber == 171 || cardNumber == 176 || cardNumber == 185)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) > 0)
            {
                return;
            }
        }

        //make exception for immolated here (dont work if hero is avatar of fire)
        if (cardNumber == 173)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 237, 7) > 0)
            {
                return;
            }
        }

        //make exception for keystone fragment (merges into keystone if you have 1 alrdy)
        if (cardNumber == 288)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 288, 5) > 0)
            {
                ins.ReduceQuantity(GameManager.ins.turnNumber, 288, 5, 1);
                ins.DrawCards(GameManager.ins.turnNumber, 186, 5, 1);

                //give message
                string msgs = "The fragments merge into a keystone!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);

                return;
            }
        }

        //make exception for lightstone shards (merges into lightstone if you have 2 alrdy)
        if (cardNumber == 294)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 294, 5) == 2)
            {
                ins.ReduceQuantity(GameManager.ins.turnNumber, 294, 5, 2);
                ins.DrawCards(GameManager.ins.turnNumber, 295, 5, 1);

                //give message
                string msgs = "The shards merge into a lightstone!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);

                return;
            }
        }

        if (holderType == 1)
        {
            //need to check if theres instances of this card yet
            //add quantity to the carddisplay if so
            if (IfHaveCard(turnNumber, cardNumber, holderType, quantity) == false)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.handCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                //set the "owner" variable to the card
                playerCard.GetComponent<Card>().belongsTo = turnNumber;

                playerCard.GetComponent<CardDisplay2>().quantity = quantity;

                //show quantity in certain conditions
                if (playerCard.GetComponent<CardDisplay2>().quantity > 1 ||
                    playerCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
                {
                    playerCard.GetComponent<CardDisplay2>().quantityText.text =
                        playerCard.gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                }

                //unless its the wanted hero, method name misleading
                if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    playerCard.SetActive(true);
                    GameManager.ins.handCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForUsableCards();
                }
            }
        }
        if (holderType == 2)
        {
            //need to check if theres instances of this card yet
            //add quantity to the carddisplay if so
            if (IfHaveCard(turnNumber, cardNumber, holderType, quantity) == false)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.artifactCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                //set the "owner" variable to the card
                playerCard.GetComponent<Card>().belongsTo = turnNumber;

                playerCard.GetComponent<CardDisplay2>().quantity = quantity;

                //show quantity in certain conditions
                if (playerCard.GetComponent<CardDisplay2>().quantity > 1 ||
                    playerCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
                {
                    playerCard.GetComponent<CardDisplay2>().quantityText.text =
                        playerCard.GetComponent<CardDisplay2>().quantity.ToString();
                }

                //unless its the wanted hero, method name misleading
                if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    playerCard.SetActive(true);
                    GameManager.ins.artifactCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForAbilityCards();
                }

                //special case for meditation (update ab buttons)
                if (cardNumber == 175)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Arrive(GameManager.ins.avatars[GameManager.ins.turnNumber]);
                }
            }
        }

        //foe cards
        //turnnumber and cardnumber work differently here
        if (holderType == 4)
        {
            if (IfHaveCard(turnNumber, cardNumber, holderType, quantity) == false)
            {
                //instantiates random quest card from the deck
                GameObject foeCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                foeCard.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform, false);

                //turns the card unactive
                foeCard.SetActive(false);

                //set the "owner" variable to the card
                foeCard.GetComponent<Card>().belongsTo = turnNumber;

                foeCard.GetComponent<CardDisplay2>().quantity = quantity;

                //show quantity in certain conditions
                if (foeCard.GetComponent<CardDisplay2>().quantity > 1 ||
                    foeCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
                {
                    foeCard.GetComponent<CardDisplay2>().quantityText.text =
                        foeCard.GetComponent<CardDisplay2>().quantity.ToString();
                }

                //unless its the wanted foe, method name misleading
                if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == turnNumber)
                {
                    foeCard.SetActive(true);
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForEnemyCards();
                }
            }

            /*tests if foe has passive of that effect number
            for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    //note that we use effect here, instead of number in deck
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == cardNumber &&
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        //
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }
                    }
                }
            }
                */
        }

        if (holderType == 5)
        {
            //need to check if theres instances of this card yet
            //add quantity to the carddisplay if so
            if (IfHaveCard(turnNumber, cardNumber, holderType, quantity) == false)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.equipmentCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                //set the "owner" variable to the card
                playerCard.GetComponent<Card>().belongsTo = turnNumber;

                playerCard.GetComponent<CardDisplay2>().quantity = quantity;

                //show quantity in certain conditions
                if (playerCard.GetComponent<CardDisplay2>().quantity > 1 ||
                    playerCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
                {
                    playerCard.GetComponent<CardDisplay2>().quantityText.text =
                        playerCard.gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                }

                //unless its the wanted hero, method name misleading
                if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    playerCard.SetActive(true);

                    GameManager.ins.equipmentCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForEquipmentCards();
                }

                //these should be done last
                GameManager.ins.uiButtonHandler.UpdateEquipmentTooltips();
                //ins.cardChangeInProgress = false;

                //lets handle the unused equipment button here
                CheckIfUnusedItems();
                //CheckIfUnusedItemsWithDelay();
            }
        }

        if (holderType == 7)
        {
            //need to check if theres instances of this card yet
            //add quantity to the carddisplay if so
            if (IfHaveCard(turnNumber, cardNumber, holderType, quantity) == false)
            {
                GameObject playerCard = null;

                //instantiates card from the deck
                //actually lets make exception for dodge, supershield & ward cards here, so the card levels get transferred
                if (cardNumber == 108 || cardNumber == 109 || cardNumber == 110)
                {
                    playerCard = Instantiate(ins.CopyCard(GameManager.ins.turnNumber, cardNumber, 2), new Vector3(0, 0, 0), Quaternion.identity);
                }
                else
                {
                    playerCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);
                }

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.effectCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                //set the "owner" variable to the card
                playerCard.GetComponent<Card>().belongsTo = turnNumber;

                playerCard.GetComponent<CardDisplay2>().quantity = quantity;

                //show quantity in certain conditions
                if (playerCard.GetComponent<CardDisplay2>().quantity > 1 ||
                    playerCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
                {
                    playerCard.GetComponent<CardDisplay2>().quantityText.text =
                        playerCard.GetComponent<CardDisplay2>().quantity.ToString();
                }

                //unless its the wanted hero, method name misleading
                if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    playerCard.SetActive(true);
                }

                GameManager.ins.effectCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForEffectCards();

                //why would we need quantity check here, since its a fresh card here always?
                if(playerCard.GetComponent<CardDisplay2>().effectTimeMax > 0f) //playerCard.GetComponent<CardDisplay2>().quantity == 1 && 
                {
                    playerCard.GetComponent<CardDisplay2>().effectTime = playerCard.GetComponent<CardDisplay2>().effectTimeMax;
                }
            }
        }

        //objectives
        if (holderType == 11)
        {
            //need to check if theres instances of this card yet
            //add quantity to the carddisplay if so
            if (IfHaveCard(turnNumber, cardNumber, holderType, quantity) == false)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.objectiveCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                //set the "owner" variable to the card
                playerCard.GetComponent<Card>().belongsTo = turnNumber;

                playerCard.GetComponent<CardDisplay2>().quantity = quantity;

                //show quantity in certain conditions
                if (playerCard.GetComponent<CardDisplay2>().quantity > 1 ||
                    playerCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
                {
                    playerCard.GetComponent<CardDisplay2>().quantityText.text =
                        playerCard.GetComponent<CardDisplay2>().quantity.ToString();
                }

                //unless its the wanted hero, method name misleading
                if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    playerCard.SetActive(true);
                }

                GameManager.ins.objectiveCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForObjectiveCards();

                //add card to the quest "taken" list
                GameManager.ins.specialVariables.haveTakenQuest.Add(cardNumber);
            }
        }

        if (holderType != 4 && holderType != 11)
        {
            //check new stats
            GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().StatUpdate();
        }

        //PV.RPC("RPC_DrawCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, holderType, quantity);
    }

    //doesnt work for special cards atm
    //holdertype 1= usables, 2= passive abilities, 4= foe cards, 5= equipment, 7= effects
    //could remove the rpc call untill multiplayer is added
    [PunRPC]
    void RPC_DrawCards(int turnNumber, int cardNumber, int holderType, int quantity)
    {

    }

    //adds quantity to the card, if it exists alrdy
    //holdertype 1= usables, 2= passive abilities, 4= foe cards, 5= equipment
    public bool IfHaveCard(int turnNumber, int cardNumber, int holderType, int quantity)
    {
        if (holderType == 1)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        if (holderType == 2)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //need to use this for foe cards too in v94 
        //better use the same card & effect number from now on
        if (holderType == 4)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == cardNumber &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        if (holderType == 5)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.equipmentCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        //this should be updated as well
                        //ins.cardChangeInProgress = false;
                        return true;
                    }
                }
            }
        }

        if (holderType == 7)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.effectCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //objectives
        if (holderType == 11)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.objectiveCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }

        }

        return false;
    }

    //should make this an rpc function?
    //holdertype 5 is equipment canvas now, 6 is store canvas, 7 is effect card canvas, 11 is objective canvas
    public void ReduceQuantity(int turnNumber, int cardNumber, int holderType, int quantity)
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_ReduceQuantity", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, holderType, quantity);
    }

    [PunRPC]
    void RPC_ReduceQuantity(int turnNumber, int cardNumber, int holderType, int quantity)
    {
        if (holderType == 1)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }

                        //remove card if quantity goes to 0 (or below)
                        if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            Destroy(GameManager.ins.handCardArea.transform.GetChild(i).gameObject);

                            GameManager.ins.handCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForUsableCards();
                            return;
                        }
                    }
                }
            }
        }
        if (holderType == 2)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }
                        //special case for sleep
                        if (cardNumber == 21 && GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity == 0)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().sleepOverlay.SetActive(false);
                        }

                        //remove card if quantity goes to 0
                        if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            Destroy(GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject);
                            GameManager.ins.artifactCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForAbilityCards();
                            return;
                        }
                    }
                }
            }
        }
        //foe cards
        if (holderType == 4)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    //note that we use effect here, instead of number in deck
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == cardNumber)
                    {
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }

                        //remove card if quantity goes to 0
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            Destroy(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject);
                            return;
                        }
                    }
                }
            }
        }

        if (holderType == 5)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.equipmentCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }

                        //remove card if quantity goes to 0
                        if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            Destroy(GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject);
                            GameManager.ins.equipmentCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForEquipmentCards();

                            //lets handle the unused equipment button here
                            //CheckIfUnusedItems();
                            CheckIfUnusedItemsWithDelay();
                            return;
                        }
                    }
                }
            }
        }

        //store display
        if (holderType == 6)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < StoreHandler.ins.storeCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }

                        //remove card if quantity goes to 0
                        if (StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            Destroy(StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject);
                            return;
                        }
                    }
                }
            }
        }

        if (holderType == 7)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.effectCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }
                        //special case for sleep
                        if (cardNumber == 21 && GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity == 0)
                        {
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().sleepOverlay.SetActive(false);
                        }

                        //remove card if quantity goes to 0 (or below)
                        if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            //special case for potion of invulnerability
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 95)
                            {
                                //reduce 40% def & res modifiers
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier -= 40;
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier -= 40;

                                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().invulnerabilityActivated = false;
                            }

                            //special case for potion of power
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 298)
                            {
                                //reduce 40% all damage modifiers
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier -= 40;
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier -= 40;
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombModifier -= 40;
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyModifier -= 40;

                                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().potionOfPowerActivated = false;
                            }

                            //special case for wraiths gift
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 53)
                            {
                                //reduce 40% def & res modifiers
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier -= 20;

                                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wraithsGiftActivated = false;
                            }

                            //special case for avatar of fire gift
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 158)
                            {
                                //reduce 40% def & res modifiers
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier -= 20;

                                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().avatarOfFireActivated = false;
                            }

                            //special case for berserk
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 156)
                            {
                                //increase def & res modifiers
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier += 20;
                                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier += 20;

                                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().berserkActivated = false;
                            }

                            //special case for time warp on v0.7.0.
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 76)
                            {
                                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated = false;
                            }

                            //special case for smoke bomb
                            if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 77)
                            {
                                /* actually better keep this at the protections check methods, to avoid bug
                                if(GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().cardLevel == 2)
                                {
                                    //reduce def & res modifiers
                                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier -= 10;
                                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier -= 10;

                                }
                                */
                                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated = false;
                            }
                            //lets try this here
                            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ReactivateDependentCards(GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect);

                            //need to update stat texts here for ensnaring roots, pot of invulnerability etc..
                            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

                            Destroy(GameManager.ins.effectCardArea.transform.GetChild(i).gameObject);
                            GameManager.ins.effectCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForEffectCards();
                            return;
                        }
                    }
                }
            }
        }

        //objective cards
        if (holderType == 11)
        {
            //add card to the quest "taken" list
            //a special case where quest gets "removed", even if you didnt have it previously
            GameManager.ins.specialVariables.haveTakenQuest.Add(cardNumber);
            //also quests would get "completed" when returning it?
            GameManager.ins.specialVariables.haveCompletedQuest.Add(cardNumber);

            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.objectiveCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                        //show quantity in certain conditions
                        if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                            GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                                GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        else
                        {
                            GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                        }

                        //remove card if quantity goes to 0
                        if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                        {
                            Destroy(GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject);
                            GameManager.ins.objectiveCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForObjectiveCards();
                            return;
                        }
                    }
                }
            }
        }
    }

    //returns the quantity of the chosen card
    //holdertype 1= usables, 2= passive abilities, 4= foe cards, 5= equipment, 7=effect, 8= levelup holder 1 9= levelup holder 2, 10= levelup holder 3, 11= objective holder
    //note that holdertype 4 checks effect number
    //is pretty dumb to have both effect number and cardnumber separately tho
    public int CheckQuantity(int turnNumber, int cardNumber, int holderType)
    {
        if (holderType == 1)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }
        if (holderType == 2)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        //checks effect number, unlike the other cases
        //added belongs to check here in v94
        if (holderType == 4)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    //note that we use effect here, instead of number in deck
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == cardNumber &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        if (holderType == 5)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.equipmentCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        if (holderType == 7)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.effectCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        if (holderType == 8)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.levelupCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        if (holderType == 9)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.levelupCardArea2.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.levelupCardArea2.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.levelupCardArea2.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.levelupCardArea2.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.levelupCardArea2.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        if (holderType == 10)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.levelupCardArea3.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.levelupCardArea3.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.levelupCardArea3.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.levelupCardArea3.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.levelupCardArea3.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        if (holderType == 11)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.objectiveCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.objectiveCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }

        return 0;
    }

    //need to check separately via effect number due to my stupidity
    public int CheckQuantityViaEffectNumber(int turnNumber, int effectNumber, int holderType)
    {
        if (holderType == 7)
        {
            //tests if player has passive of that number
            for (int i = 0; i < GameManager.ins.effectCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber &&
                    GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;

                    }
                }
            }
        }
        return 0;
    }

    //copies card from certain holder
    //holdertype 1 is usables canvas now, 2 is ability canvas, 7 is effects
    //can also be used to check certain card values?
    public GameObject CopyCard(int turnNumber, int cardNumber, int holderType)
    {
        //usable holder
        if (holderType == 1)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.handCardArea.transform.GetChild(i).gameObject;
                    }
                }
            }
        }
        //ability holder 
        if (holderType == 2)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject;
                    }
                }
            }
        }


        //effect holder 
        if (holderType == 7)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < GameManager.ins.effectCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
                {
                    if (GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                    GameManager.ins.effectCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == turnNumber)
                    {
                        return GameManager.ins.effectCardArea.transform.GetChild(i).gameObject;
                    }
                }
            }
        }

        return null;
    }

    public GameObject CopyFoeCard(int foeNumber, int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == foeNumber)
                {
                    return GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject;
                }
            }
        }
        return null;
    }

    //sets usable card as isEnabled = true, if its available to use, otherwise false
    //phaseEntered: 0=disable all, 1=overmap, 2=in location, 3=attack, 4=defense, 5=selling, 6=buying, 7=in encounter
    public void SetUsables(int phaseEntered)
    {
        phaseNumber = phaseEntered;

        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == GameManager.ins.turnNumber)
                {
                    //if phase is 0, all is disabled
                    if (phaseEntered == 0)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    //these activate or deactivate determined by phase
                    if (phaseEntered == 1 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeUsedOnOvermap == true)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                    if (phaseEntered == 1 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeUsedOnOvermap == false)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    if (phaseEntered == 2 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeUsedWhenExploring == true)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                    if (phaseEntered == 2 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeUsedWhenExploring == false)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    /*
                    if (phaseEntered == 3 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().attackCard == true)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                    if (phaseEntered == 3 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().attackCard == false)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    if (phaseEntered == 4 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().defenseCard == true)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                    if (phaseEntered == 4 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().defenseCard == false)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }
                    
                    //might as well put the targetting changes here?
                    if (phaseEntered == 3)
                    {
                        GameManager.ins.references.targettingHandler.crosshair.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.crosshair.GetComponent<BallMove>().attackPhaseIcon;
                        GameManager.ins.references.targettingHandler.target.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.target.GetComponent<BallMove>().attackPhaseIcon;
                    }
                    //might as well put the targetting changes here?
                    if (phaseEntered == 4)
                    {
                        GameManager.ins.references.targettingHandler.crosshair.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.crosshair.GetComponent<BallMove>().defensePhaseIcon;
                        GameManager.ins.references.targettingHandler.target.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.target.GetComponent<BallMove>().defensePhaseIcon;
                    }
                    */

                    //shouldnt allow any cards to be used from usable holder in combat in v91?
                    if (phaseEntered == 3 || phaseEntered == 4)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    //if (phaseEntered == 5 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeSold == true)
                    //disable all cards when buying or selling
                    if (phaseEntered == 5)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }
                    if (phaseEntered == 6)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    //on encounters
                    if (phaseEntered == 7 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeUsedOnEncounter == true)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                    if (phaseEntered == 7 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeUsedOnEncounter == false)
                    {
                        GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }

                    //special case for distilling
                    if ((phaseEntered == 1 || phaseEntered == 2) && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 93)
                    {
                        //check if player has meals
                        if (CheckQuantity(GameManager.ins.turnNumber, 20, 1) > 0)
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                        }
                        else
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                        }
                    }

                    /*special case for offering
                    if ((phaseEntered == 1 || phaseEntered == 2) && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 48)
                    {
                        //check if player has meals
                        if (CheckQuantity(GameManager.ins.turnNumber, 20, 1) > 0)
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                        }
                        else
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                        }
                    }

                    //special case for when card requires favor
                    if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresFavor > 0)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresFavor)
                        {
                            GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                        }
                    }
                    */
                }
            }
        }
    }

    //resets phase, when card is drawn in mid-phase for example
    public void ResetPhaseAfterDelay()
    {
        Invoke("ResetPhase", 0.5f);
    }

    public void ResetPhase()
    {
        SetUsables(phaseNumber);
    }

    //handles instant passive effects of cards
    public void InstantPassiveEffect(int turnNumber, int cardNumber)
    {
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            //chosen one
            if (cardNumber == 3)
            {
                //3 favor
                GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(6, 3);
            }

            //investor
            if (cardNumber == 12)
            {
                //15 coins
                GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(4, 15);
            }

            //poet
            if (cardNumber == 15)
            {
                //4 fame
                GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(5, 4);
            }

            //cleric
            if (cardNumber == 101)
            {
                //3 favor
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxFavor += 3;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor += 3;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber); ;
            }

            //arcanist
            if (cardNumber == 2)
            {
                //3 favor
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxEnergy += 3;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy += 3;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber); ;
            }

            //warrior
            if (cardNumber == 0)
            {
                //3 favor
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxHealth += 3;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health += 3;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber); ;
            }

            //artisan
            if (cardNumber == 1)
            {
                DrawCards(GameManager.ins.turnNumber, 169, 1, 3);
            }

            //relentless
            if (cardNumber == 9)
            {
                //2 max AP
                GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxActionPoints += 3;
                //GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(6, 3);
            }

            //give 3 bombs for bombmaker
            if (cardNumber == 106)
            {
                DrawCards(GameManager.ins.turnNumber, 169, 1, 3);
            }

            //strength upgrade (works for both level 1 and 2)
            if (cardNumber == 115 || cardNumber == 116)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxStrength += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //defense upgrade (works for both level 1 and 2)
            if (cardNumber == 117 || cardNumber == 118)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //arcanePower upgrade (works for both level 1 and 2)
            if (cardNumber == 119 || cardNumber == 120)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxArcanePower += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //resistance upgrade (works for both level 1 and 2)
            if (cardNumber == 121 || cardNumber == 122)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //influence upgrade (works for both level 1 and 2)
            if (cardNumber == 123 || cardNumber == 124)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //mechanics upgrade (works for both level 1 and 2)
            if (cardNumber == 125 || cardNumber == 126)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //digging upgrade (works for both level 1 and 2)
            if (cardNumber == 127 || cardNumber == 128)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //lore upgrade (works for both level 1 and 2)
            if (cardNumber == 129 || cardNumber == 130)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //discovery upgrade (works for both level 1 and 2)
            if (cardNumber == 131 || cardNumber == 132)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
            }

            //grenadier
            if (cardNumber == 157)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack += 1;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //heavy hitter upgrade
            if (cardNumber == 158)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier += 25;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //combat mage upgrade
            if (cardNumber == 159)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier += 25;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //holy power
            if (cardNumber == 160)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyModifier += 25;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //ironskin upgrade
            if (cardNumber == 161)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier += 20;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //combat mage upgrade
            if (cardNumber == 162)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier += 20;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //fortitude
            if (cardNumber == 17)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxHealth += 3;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health += 3;
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber);
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //give 3 r. pots for distiller & toxicology
            if (cardNumber == 166 || cardNumber == 155)
            {
                DrawCards(GameManager.ins.turnNumber, 19, 1, 3);
            }

            //regrowth 1
            if (cardNumber == 178)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().healthRegen += 15;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }
            //inner energy 1
            if (cardNumber == 179)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energyRegen += 15;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }
            //regrowth 2
            if (cardNumber == 183)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().healthRegen += 15;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }
            //inner energy 2
            if (cardNumber == 184)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energyRegen += 15;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //frailty
            if (cardNumber == 16)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxHealth -= 2;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health -= 2;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier -= 10;

                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
                GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GameManager.ins.turnNumber);
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }
        }
    }

    //for timed passive cards
    public void CheckCardTimers(int turnNumber)
    {
        //need rpc call here?
        PV.RPC("RPC_CheckCardTimers", RpcTarget.AllBufferedViaServer, turnNumber);
    }

    [PunRPC]
    void RPC_CheckCardTimers(int turnNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCooldownPassive == true)
            {
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                //triggers effect
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown == 0)
                {
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown =
                        GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().maxCooldown;

                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                    ActivatePassiveEffect(turnNumber, GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck);

                    //play the audioclip of the effect chosen, if there is any
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().sfx != null)
                    {
                        intelligenceSfxHolder.clip = GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().sfx;
                        intelligenceSfxHolder.Play();
                    }
                }
            }

        }
    }

    public void ActivatePassiveEffect(int turnNumber, int numberInDeck)
    {
        //chosen one
        if (numberInDeck == 3)
        {
            GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(6, 2);
        }
        //investor
        if (numberInDeck == 12)
        {
            GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(4, 10);
        }
        /* remove this for now
         * poet
        if (numberInDeck == 15)
        {
            GameManager.ins.avatars[turnNumber].GetComponent<CharController>().UpdateResources(5, 1);
        }
        */
    }

    //for timed foe cooldown cards
    //checks for specific effect
    //returns true, if specific effect triggers
    //unused since v0.5.7. ?
    public bool CheckFoeCardTimers(int effect)
    {
        //tests if foe has cooldown ability
        for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
        {
            //added special case for extra strike & tme warp
            //also check that the card is active for multi-unit battles
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effect &&
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == false && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated == false &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.activeSelf == true)
            {
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                //returns true, if the cooldown reaches 0
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown == 0)
                {
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown =
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().maxCooldown;

                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                    //ActivatePassiveEffect(turnNumber, GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck);

                    //play the audioclip of the effect chosen, if there is any
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().sfx != null)
                    {
                        intelligenceSfxHolder.clip = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().sfx;
                        intelligenceSfxHolder.Play();
                    }

                    //could display custom icon also
                    //dont change foe icon on foe display tho
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialIcon != null)
                    {
                        //remove the icon for now
                        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().ShowSpecialIconBriefly(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialIcon);
                    }

                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialCrosshairIcon != null)
                    {
                        GameManager.ins.characterDisplays.GetComponent<TargettingHandler>().foeCurrentSpecialAbilityIcon = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialCrosshairIcon;
                        GameManager.ins.characterDisplays.GetComponent<TargettingHandler>().heroCurrentDefendTypeIcon = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialShieldIcon;
                        GameManager.ins.characterDisplays.GetComponent<TargettingHandler>().SetSpecialFoeAttackPhaseIcons();
                        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().ShowSpecialIconBriefly(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialIcon);
                    }

                    //spawn special buttons only for foe defense skills
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().defenseCard == true)
                    {
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeDefensePhase = true;
                    }

                    //spawn special buttons only for foe attack skills
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().attackCard == true)
                    {
                        /* old system
                         * spawn special buttons, if theres any on the list
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons.Count > 0)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons2 =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons2;

                            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = true;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialText;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnSpecialButtons();
                            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SpawnDefenseButtonsWithDelay();

                            return true;
                        }
                        */

                        //uses values stored in the foe attack card, but spawns default options (with icons etc)
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().defaultSkillCheckOptions)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons2 =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons2;

                            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = true;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text = "";

                            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
                            {
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text += "<br><br><br><br><br><br><br><br>";
                            }

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text +=
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialText;

                            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SpawnDefenseButtonsWithDelay();
                        }

                        //this is the old button system for now, spawns special options (with text buttons)
                        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialSkillCheckOptions)
                        {
                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons2 =
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialButtons2;

                            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase = true;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text = "";

                            if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
                            {
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text += "<br><br><br><br><br><br><br><br>";
                            }

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text +=
                                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().specialText;

                            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnSpecialButtons();

                            //set this to 0 temporarily, so that ShowCharacter method wont show targetting display
                            phaseNumber = 0;
                            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SpawnDefenseButtonsWithDelay();
                        }
                    }

                    return true;
                }
            }

        }
        return false;
    }

    //for timed hero combat cooldown cards
    //returns true, if specific effect triggers
    //cardType 1: attack, 2: defense, 3: consumable?
    public void ReduceHeroCombatCardTimers(int turnNumber, int cardType)
    {
        //special case for normal bombs
        if (cardType == 1 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown > 0)
        {
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
        }

        for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCombatCooldownCard == true)
            {
                if (cardType == 1 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().attackCard == true &&
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown > 0)
                {
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                }

                if (cardType == 2 && GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().defenseCard == true &&
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown > 0)
                {
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                    GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();


                    /*special case for quench fire (not sure if needed actually)
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 20)
                    {
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[10].gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[10].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                    }
                    */
                }
            }
        }

        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCombatCooldownCard == true)
            {
                if (cardType == 1 && GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().attackCard == true &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown > 0)
                {
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                    //special case for detonations
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 79)
                    {
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                    }
                }

                if (cardType == 2 && GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().defenseCard == true &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown > 0)
                {
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown -= 1;
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                }
            }
        }
    }

    //not needed in v0.7.0. ?
    //exhausts specific combat card
    public void ExhaustHeroCombatCard(int turnNumber, int effectNumber)
    {
        //special case for bombs
        if (effectNumber == 39 || effectNumber == 41)
        {
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().maxCooldown;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().maxCooldown;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
            return;
        }

        //special case for quench fire
        //can do disable here, just this once, since the other method cant handle it it seems
        if (effectNumber == 287)
        {
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[10].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[10].gameObject.GetComponent<CardDisplay2>().maxCooldown;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[10].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[10].GetComponent<CardDisplay2>().isEnabled = false;
            return;
        }

        for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCombatCooldownCard == true &&
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
            {
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().maxCooldown;
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

            }
        }

        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCombatCooldownCard == true &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
            {
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().maxCooldown;
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                //special case for detonations
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 79)
                {
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().maxCooldown;
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().maxCooldown;
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                }
            }
        }
    }

    //reset all combat cards
    //not used in v0.7.0. ?
    public void ResetHeroCombatCards(int turnNumber)
    {
        //default bombs
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().startingCooldown;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().startingCooldown;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

        for (int i = 0; i < GameManager.ins.handCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCombatCooldownCard == true)
            {
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().startingCooldown;
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null &&
                turnNumber == GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isCombatCooldownCard == true)
            {
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().startingCooldown;
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                //special case for detonations
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 79)
                {
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().startingCooldown;
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().startingCooldown;
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[7].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                }
            }
        }
    }


    //when equipping an item from equipment display
    public void EquipCards(int turnNumber, int cardNumber)
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_EquipCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber);
    }

    //or equipping gear
    [PunRPC]
    void RPC_EquipCards(int turnNumber, int cardNumber)
    {
        //remove the original card from equipment display (or reduce the quantity)
        ReduceQuantity(turnNumber, cardNumber, 5, 1);

        RemoveItemFromSlot(turnNumber, cardNumber, false);

        //AddItemToSlot(turnNumber, cardNumber);

        StartCoroutine(AddItemDelay(turnNumber, cardNumber, 0.3f));

        //check new stats (do this on coroutine instead?)
        //GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().StatUpdate();

        GameManager.ins.uiButtonHandler.UpdateEquipmentTooltips();

        //seems these need to be started here, or it wont work
        //could check if there are free slots for your items here too
        StartCoroutine(AllowEquipping(0.5f));

        //CardHandler.ins.cardChangeInProgress = false;
    }

    //when unequipping an item from item slot
    public void UnEquipCards(int turnNumber, int cardNumber)
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_UnEquipCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber);
    }

    //or unequipping gear
    [PunRPC]
    void RPC_UnEquipCards(int turnNumber, int cardNumber)
    {
        //technically this method alrdy updates the tooltips, so dunno if we need to do it a second time
        RemoveItemFromSlot(turnNumber, cardNumber, false);

        //check new stats
        GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().StatUpdate();

        //seems these need to be started here, or it wont work
        StartCoroutine(AllowEquipping(0.5f));

        //GameManager.ins.uiButtonHandler.UpdateEquipmentTooltips();
        //CardHandler.ins.cardChangeInProgress = false;
    }

    //re-enables equipping left clicks after delay
    public IEnumerator AllowEquipping(float time)
    {
        yield return new WaitForSeconds(time);

        Debug.Log("enables equipping");

        //lets handle the unused equipment button here
        CheckIfUnusedItems();

        ins.cardChangeInProgress = false;
        yield break;

    }

    //when called from destructible objects
    public void AllowEquippingAfterDelay()
    {
        //seems these need to be started here, or it wont work
        StartCoroutine(AllowEquipping(0.5f));
    }

    //adds item to slot after delay (to make sure the slot is actually empty)
    public IEnumerator AddItemDelay(int turnNumber, int cardNumber, float time)
    {
        yield return new WaitForSeconds(time);

        Debug.Log("adds card");
        AddItemToSlot(turnNumber, cardNumber, true);

        GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().StatUpdate();
        yield break;

    }

    //removes equipped card from slot
    //used by equipping and unequipping method
    public void RemoveItemFromSlot(int turnNumber, int cardNumber, bool removeOnlySpecific)
    {
        int itemType = generalDeck[cardNumber].GetComponent<EquipmentCard>().equipmentType;

        //remove the previous item (if any), and remove its bonus
        if (itemType == 1)
        {
            for (int i = 0; i < helmSlot.transform.childCount; i++)
            {
                if (helmSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && helmSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, helmSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, helmSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(helmSlot.transform.GetChild(i).gameObject);

                    //special case for removing divers mask underwater, and not sentinel
                    //check divers mask or sentinel
                    if (GameManager.ins.references.currentMinimap.minimapNumber == 72 &&
                        cardNumber == 193 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
                    {
                        //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -999);
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                    }
                    return;
                }
            }
        }

        //remove the previous item (if any), and remove its bonus
        if (itemType == 2)
        {
            for (int i = 0; i < armorSlot.transform.childCount; i++)
            {
                if (armorSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && armorSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, armorSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, armorSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(armorSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //remove the previous item (if any), and remove its bonus
        if (itemType == 3)
        {
            for (int i = 0; i < ringSlot.transform.childCount; i++)
            {
                if (ringSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && ringSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, ringSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, ringSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(ringSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //remove the previous item (if any), and remove its bonus
        if (itemType == 4)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                if (weaponSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && weaponSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, weaponSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, weaponSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(weaponSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //remove the previous item (if any), and remove its bonus
        if (itemType == 5)
        {
            for (int i = 0; i < mountSlot.transform.childCount; i++)
            {
                if (mountSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && mountSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, mountSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, mountSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(mountSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //remove the previous item (if any), and remove its bonus
        if (itemType == 6)
        {
            for (int i = 0; i < miscSlot1.transform.childCount; i++)
            {
                if (miscSlot1.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(miscSlot1.transform.GetChild(i).gameObject);
                    return;
                }
            }

            /* unused in v94+
             * 
            //removes card of that type from one misc slot (if any)
            int miscType = generalDeck[cardNumber].GetComponent<EquipmentCard>().miscType;

            for (int i = 0; i < miscSlot1.transform.childCount; i++)
            {
                if (miscSlot1.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber && miscSlot1.transform.GetChild(i).GetComponent<EquipmentCard>().miscType == miscType)
                {
                    DrawCards(turnNumber, miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(miscSlot1.transform.GetChild(i).gameObject);
                    return;
                }
            }
            for (int i = 0; i < miscSlot2.transform.childCount; i++)
            {
                if (miscSlot2.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber && miscSlot2.transform.GetChild(i).GetComponent<EquipmentCard>().miscType == miscType)
                {
                    DrawCards(turnNumber, miscSlot2.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, miscSlot2.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(miscSlot2.transform.GetChild(i).gameObject);
                    return;
                }
            }
            for (int i = 0; i < miscSlot3.transform.childCount; i++)
            {
                if (miscSlot3.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber && miscSlot3.transform.GetChild(i).GetComponent<EquipmentCard>().miscType == miscType)
                {
                    DrawCards(turnNumber, miscSlot3.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, miscSlot3.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(miscSlot3.transform.GetChild(i).gameObject);
                    return;
                }
            }

            //should check if all slots are full of different misc types, and clear slot 1 if so?
            if(IsMiscSlotEmpty(1, turnNumber) == false && IsMiscSlotEmpty(2, turnNumber) == false && IsMiscSlotEmpty(3, turnNumber) == false)
            {
                for (int i = 0; i < miscSlot1.transform.childCount; i++)
                {
                    if (miscSlot1.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                    {
                        DrawCards(turnNumber, miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                        StatChangeByEquipment(turnNumber, miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                        Destroy(miscSlot1.transform.GetChild(i).gameObject);
                        return;
                    }
                }
            }
            */
        }

        //remove the previous item (if any), and remove its bonus
        //goggles
        if (itemType == 8)
        {
            for (int i = 0; i < gogglesSlot.transform.childCount; i++)
            {
                if (gogglesSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && gogglesSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, gogglesSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, gogglesSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(gogglesSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //remove the previous item (if any), and remove its bonus
        //mask
        if (itemType == 9)
        {
            for (int i = 0; i < maskSlot.transform.childCount; i++)
            {
                if (maskSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && maskSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, maskSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, maskSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(maskSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //amulet
        if (itemType == 10)
        {
            for (int i = 0; i < amuletSlot.transform.childCount; i++)
            {
                if (amuletSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && amuletSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, amuletSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, amuletSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(amuletSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //tome
        if (itemType == 11)
        {
            for (int i = 0; i < tomeSlot.transform.childCount; i++)
            {
                if (tomeSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && tomeSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, tomeSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, tomeSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(tomeSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //toolbox
        if (itemType == 12)
        {
            for (int i = 0; i < toolboxSlot.transform.childCount; i++)
            {
                if (toolboxSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && toolboxSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, toolboxSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, toolboxSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(toolboxSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        //shovel
        if (itemType == 13)
        {
            for (int i = 0; i < shovelSlot.transform.childCount; i++)
            {
                if (shovelSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (removeOnlySpecific == true && shovelSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck != cardNumber)
                    {
                        return;
                    }

                    DrawCards(turnNumber, shovelSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, 5, 1);

                    StatChangeByEquipment(turnNumber, shovelSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck, false);

                    Destroy(shovelSlot.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }
    }

    public void AddItemToSlot(int turnNumber, int cardNumber, bool allowStatChange)
    {
        int itemType = generalDeck[cardNumber].GetComponent<EquipmentCard>().equipmentType;

        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

        if (itemType == 1)
        {
            //places it in hand card area
            playerCard.transform.SetParent(helmSlot.transform, false);
        }

        if (itemType == 2)
        {
            //places it in hand card area
            playerCard.transform.SetParent(armorSlot.transform, false);
        }

        if (itemType == 3)
        {
            //places it in hand card area
            playerCard.transform.SetParent(ringSlot.transform, false);
        }

        if (itemType == 4)
        {
            //places it in hand card area
            playerCard.transform.SetParent(weaponSlot.transform, false);
        }

        if (itemType == 5)
        {
            //places it in hand card area
            playerCard.transform.SetParent(mountSlot.transform, false);
        }

        if (itemType == 6)
        {
            playerCard.transform.SetParent(miscSlot1.transform, false);

            /*
            if (IsMiscSlotEmpty(1, turnNumber) == true)
            {
                //places it in hand card area
                playerCard.transform.SetParent(miscSlot1.transform, false);
            }
            else if (IsMiscSlotEmpty(2, turnNumber) == true)
            {
                //places it in hand card area
                playerCard.transform.SetParent(miscSlot2.transform, false);
            }
            else if (IsMiscSlotEmpty(3, turnNumber) == true)
            {
                //places it in hand card area
                playerCard.transform.SetParent(miscSlot3.transform, false);
            }
            */
        }

        if (itemType == 8)
        {
            //places it in hand card area
            playerCard.transform.SetParent(gogglesSlot.transform, false);
        }

        if (itemType == 9)
        {
            //places it in hand card area
            playerCard.transform.SetParent(maskSlot.transform, false);
        }

        if (itemType == 10)
        {
            //places it in hand card area
            playerCard.transform.SetParent(amuletSlot.transform, false);
        }

        if (itemType == 11)
        {
            //places it in hand card area
            playerCard.transform.SetParent(tomeSlot.transform, false);
        }

        if (itemType == 12)
        {
            //places it in hand card area
            playerCard.transform.SetParent(toolboxSlot.transform, false);
        }

        if (itemType == 13)
        {
            //places it in hand card area
            playerCard.transform.SetParent(shovelSlot.transform, false);
        }

        //turns the card inactive
        playerCard.SetActive(false);

        //set the "owner" variable to the card
        playerCard.GetComponent<Card>().belongsTo = turnNumber;

        //unless its the wanted hero, method name misleading
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            playerCard.SetActive(true);
        }

        //lets make small change for this for v94
        if (allowStatChange == true)
        {
            StatChangeByEquipment(turnNumber, cardNumber, true);
        }

        //lets handle the unused equipment button here
        CheckIfUnusedItems();
    }

    public void CheckIfUnusedItemsWithDelay()
    {
        Invoke("CheckIfUnusedItems", 0.3f);
    }

    //checks if there are available itemslots for items in your inventory
    public void CheckIfUnusedItems()
    {
        //the button could be inactive by default
        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(false);

        for (int i = 0; i < GameManager.ins.equipmentCardArea.transform.childCount; i++)
        {
            if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<Card>().belongsTo == GameManager.ins.turnNumber)
            {
                //helm
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 1)
                {
                    if (helmSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }
                //armor
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 2)
                {
                    if (armorSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }
                //ring
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 3)
                {
                    if (ringSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }
                //weapon
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 4)
                {
                    if (weaponSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }
                //mount
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 5)
                {
                    if (mountSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }

                //misc
                //check if all slots are empty
                //else check if that misc type is alrdy equipped
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 6)
                {
                    if (miscSlot1.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }

                    /*
                    if (miscSlot1.transform.childCount == 0 && miscSlot2.transform.childCount == 0 && miscSlot3.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }

                    //lets use flag variable for this?
                    bool hasThatMiscType = false;

                    if (miscSlot1.transform.childCount > 0)
                    {
                        if(GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().miscType == miscSlot1.transform.GetChild(0).GetComponent<EquipmentCard>().miscType)
                        {
                            hasThatMiscType = true;
                        }
                    }
                    if (miscSlot2.transform.childCount > 0)
                    {
                        if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().miscType == miscSlot2.transform.GetChild(0).GetComponent<EquipmentCard>().miscType)
                        {
                            hasThatMiscType = true;
                        }
                    }
                    if (miscSlot3.transform.childCount > 0)
                    {
                        if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().miscType == miscSlot3.transform.GetChild(0).GetComponent<EquipmentCard>().miscType)
                        {
                            hasThatMiscType = true;
                        }
                    }

                    //show button if theres no items of that type equipped
                    if(hasThatMiscType == false)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                    */
                }

                //goggles
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 8)
                {
                    if (gogglesSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }

                //mask
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 9)
                {
                    if (maskSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }

                //amulet
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 10)
                {
                    if (amuletSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }

                //tome
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 11)
                {
                    if (tomeSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }

                //toolbox
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 12)
                {
                    if (toolboxSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }

                //shovel
                if (GameManager.ins.equipmentCardArea.transform.GetChild(i).GetComponent<EquipmentCard>().equipmentType == 13)
                {
                    if (shovelSlot.transform.childCount == 0)
                    {
                        GameManager.ins.uiButtonHandler.unequippedItemsButton.SetActive(true);
                    }
                }
            }
        }
    }

    //checks if theres a card on misc slot belonging to that player
    //unused in v94+ hopefully
    public bool IsMiscSlotEmpty(int slot, int turnNumber)
    {
        if (slot == 1)
        {
            for (int i = 0; i < miscSlot1.transform.childCount; i++)
            {
                if (miscSlot1.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    return false;
                }
            }
        }
        /*
        if (slot == 2)
        {
            for (int i = 0; i < miscSlot2.transform.childCount; i++)
            {
                if (miscSlot2.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    return false;
                }
            }
        }
        if (slot == 3)
        {
            for (int i = 0; i < miscSlot3.transform.childCount; i++)
            {
                if (miscSlot3.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    return false;
                }
            }
        }
        */

        return true;
    }

    //if isPositive == true, adds stats
    //note that this should be called through rpc methods
    public void StatChangeByEquipment(int turnNumber, int cardNumber, bool isPositive)
    {
        if (isPositive == true)
        {
            for (int i = 0; i < generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType.Length; i++)
            {
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 0)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxEnergy += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 1)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxStrength += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[0] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 2)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[1] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 3)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxArcanePower += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[2] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 4)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[3] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 5)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[4] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 6)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[5] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 7)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[6] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 8)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[7] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 9)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[8] += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                //movementBonus bonus changed for v90
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 15)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().movementBonus += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                //for v92
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 17)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxHealth += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                //for v94
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 18)
                {
                    //max AP
                    GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxActionPoints += generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }
            }

            //combat modifiers
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().attMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier += generalDeck[cardNumber].GetComponent<EquipmentCard>().attMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().apMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier += generalDeck[cardNumber].GetComponent<EquipmentCard>().apMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().bombMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombModifier += generalDeck[cardNumber].GetComponent<EquipmentCard>().bombMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().defMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier += generalDeck[cardNumber].GetComponent<EquipmentCard>().defMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().resMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier += generalDeck[cardNumber].GetComponent<EquipmentCard>().resMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().healthRegen != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().healthRegen += generalDeck[cardNumber].GetComponent<EquipmentCard>().healthRegen;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().energyRegen != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energyRegen += generalDeck[cardNumber].GetComponent<EquipmentCard>().energyRegen;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().bombAttack != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack += generalDeck[cardNumber].GetComponent<EquipmentCard>().bombAttack;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().holyAttack != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyAttack += generalDeck[cardNumber].GetComponent<EquipmentCard>().holyAttack;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().holyMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyModifier += generalDeck[cardNumber].GetComponent<EquipmentCard>().holyMultiplier;
            }
        }

        if (isPositive == false)
        {
            for (int i = 0; i < generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType.Length; i++)
            {
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 0)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxEnergy -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 1)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxStrength -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[0] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 2)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[1] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 3)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxArcanePower -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[2] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 4)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[3] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 5)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[4] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 6)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[5] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 7)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[6] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 8)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[7] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 9)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().equipmentStats[8] -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 15)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().movementBonus -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                //for v92
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 17)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxHealth -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

                //for v94
                if (generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusType[i] == 18)
                {
                    //max AP
                    GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxActionPoints -= generalDeck[cardNumber].GetComponent<EquipmentCard>().statBonusQty[i];
                }

            }
            //combat modifiers
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().attMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier -= generalDeck[cardNumber].GetComponent<EquipmentCard>().attMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().apMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier -= generalDeck[cardNumber].GetComponent<EquipmentCard>().apMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().bombMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombModifier -= generalDeck[cardNumber].GetComponent<EquipmentCard>().bombMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().defMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier -= generalDeck[cardNumber].GetComponent<EquipmentCard>().defMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().resMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier -= generalDeck[cardNumber].GetComponent<EquipmentCard>().resMultiplier;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().healthRegen != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().healthRegen -= generalDeck[cardNumber].GetComponent<EquipmentCard>().healthRegen;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().energyRegen != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energyRegen -= generalDeck[cardNumber].GetComponent<EquipmentCard>().energyRegen;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().bombAttack != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack -= generalDeck[cardNumber].GetComponent<EquipmentCard>().bombAttack;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().holyAttack != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyAttack -= generalDeck[cardNumber].GetComponent<EquipmentCard>().holyAttack;
            }
            if (generalDeck[cardNumber].GetComponent<EquipmentCard>().holyMultiplier != 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyModifier -= generalDeck[cardNumber].GetComponent<EquipmentCard>().holyMultiplier;
            }

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResourceTexts();
        }
    }

    //called at end of combat
    public void RemoveCombatEffectsFromHero()
    {
        //reduce quantity of wraiths gift & avatar of fire
        ReduceQuantity(GameManager.ins.turnNumber, 112, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 237, 7, 99);

        //reduce quantity of blessed blade and oil
        ReduceQuantity(GameManager.ins.turnNumber, 113, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 114, 7, 99);

        //remove time warp
        ReduceQuantity(GameManager.ins.turnNumber, 151, 7, 99);

        //remove smoke bomb effect 1 & 2
        ReduceQuantity(GameManager.ins.turnNumber, 152, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 232, 7, 99);

        //remove earth magic effect
        ReduceQuantity(GameManager.ins.turnNumber, 153, 7, 99);

        //remove potion of invulnerability & power effect
        ReduceQuantity(GameManager.ins.turnNumber, 168, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 298, 7, 99);

        //ensnaring roots & web
        ReduceQuantity(GameManager.ins.turnNumber, 171, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 176, 7, 99);

        //frostbite & stone curse
        ReduceQuantity(GameManager.ins.turnNumber, 185, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 182, 7, 99);

        //remove berserk
        ReduceQuantity(GameManager.ins.turnNumber, 235, 7, 99);

        //remove wrath of guliman, concussion bomb infusion, firebomb infusion
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 236, 7, 99);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 238, 7, 99);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 239, 7, 99);

        //remove other defense abilities (isolore, parry, block, ward, lightstone blessing)
        ReduceQuantity(GameManager.ins.turnNumber, 111, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 108, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 109, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 110, 7, 99);
        ReduceQuantity(GameManager.ins.turnNumber, 301, 7, 99);

        //immolation
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 173, 7) > 0)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 173, 7) / 3));
            ReduceQuantity(GameManager.ins.turnNumber, 173, 7, 99);
        }

        //remove stuns
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isPetrified = false;
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen = false;

        //this is done in reducequantity method alrdy
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
    }

    //checks if you have specific item equipped
    //itemtype 1= helm, 2= armor, 3= ring, 4= weapon, 5= mount, 6= misc
    public bool CheckItemInSlot(int turnNumber, int itemType, int numberInDeck)
    {
        if (itemType == 1)
        {
            for (int i = 0; i < helmSlot.transform.childCount; i++)
            {
                if (helmSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (helmSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }
        if (itemType == 2)
        {
            for (int i = 0; i < armorSlot.transform.childCount; i++)
            {
                if (armorSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (armorSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }
        if (itemType == 3)
        {
            for (int i = 0; i < ringSlot.transform.childCount; i++)
            {
                if (ringSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (ringSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }
        if (itemType == 4)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                if (weaponSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (weaponSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }
        if (itemType == 5)
        {
            for (int i = 0; i < mountSlot.transform.childCount; i++)
            {
                if (mountSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (mountSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }
        if (itemType == 6)
        {
            for (int i = 0; i < miscSlot1.transform.childCount; i++)
            {
                if (miscSlot1.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (miscSlot1.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
            /*
            for (int i = 0; i < miscSlot2.transform.childCount; i++)
            {
                if (miscSlot2.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (miscSlot2.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
            for (int i = 0; i < miscSlot3.transform.childCount; i++)
            {
                if (miscSlot3.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (miscSlot3.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            */
        }

        if (itemType == 8)
        {
            for (int i = 0; i < gogglesSlot.transform.childCount; i++)
            {
                if (gogglesSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (gogglesSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }

        if (itemType == 9)
        {
            for (int i = 0; i < maskSlot.transform.childCount; i++)
            {
                if (maskSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (maskSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }

        if (itemType == 10)
        {
            for (int i = 0; i < amuletSlot.transform.childCount; i++)
            {
                if (amuletSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (amuletSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }

        if (itemType == 11)
        {
            for (int i = 0; i < tomeSlot.transform.childCount; i++)
            {
                if (tomeSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (tomeSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }

        if (itemType == 12)
        {
            for (int i = 0; i < toolboxSlot.transform.childCount; i++)
            {
                if (toolboxSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (toolboxSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }

        if (itemType == 13)
        {
            for (int i = 0; i < shovelSlot.transform.childCount; i++)
            {
                if (shovelSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == turnNumber)
                {
                    if (shovelSlot.transform.GetChild(i).GetComponent<Card>().numberInDeck == numberInDeck)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //checks if the player has magic staff 
    public bool StaffCheck()
    {
        for (int i = 0; i < weaponSlot.transform.childCount; i++)
        {
            if (weaponSlot.transform.GetChild(i).GetComponent<Card>().belongsTo == GameManager.ins.turnNumber)
            {
                if (weaponSlot.transform.GetChild(i).GetComponent<EquipmentCard>().isStaff == true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //removes skillcard of specific effect from offer, if there is one, then draw new one
    //note that this only handles skillpoint changes for now
    public void RemoveSkillCardFromOffer(int cardNumber)
    {
        //tests if player has that card on offer alrdy
        for (int i = 0; i < GameManager.ins.levelupCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber &&
                GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == GameManager.ins.turnNumber)
                {
                    Destroy(GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject);

                    //draw custom number of cards
                    //CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DrawUpgradeOffer2(1);
                    CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DrawUpgradeCards3();
                    //CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();
                }
            }
        }
    }

    public void RefreshBombs()
    {
        /*
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().startingCooldown;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().startingCooldown;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
        */
        int numberOfCards = GameManager.ins.combatCardArea.gameObject.transform.childCount;

        //check concussion & firebombs
        //GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 238, 7);
        //GameObject cardToCheck2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 239, 7);

        for (int i = 0; i < numberOfCards; i++)
        {
            //bomb attacks
            if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 39 ||
                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 41)
            {
                /*still check bomb quantity?
                 *this check should be done each time a bomb is activated tho
                if (ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
                {
                    //GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
                */

                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown -= 5f;

                /*
                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown = GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().startingCooldown;
                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();
                */
            }

            /*check that concussion & firebombs arent active
            //why though?
            if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 159 ||
                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == 160)
            {
                if (cardToCheck1 == null & cardToCheck2 == null)
                {
                    if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresEnergy <=
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy)
                    {
                        GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
            */
        }
    }

    //refreshes particular card
    //updated for v0.5.7.
    //holdertype doesnt really do anything here tho?
    public void RefreshCombatCard(int cardNumber, int holderType, float timeToReduce)
    {
        //need to make sure the card exists
        if (ins.CheckQuantity(GameManager.ins.turnNumber, cardNumber, holderType) > 0)
        {
            GameObject cardToCheck = ins.CopyCard(GameManager.ins.turnNumber, cardNumber, holderType);

            //cardToCheck.gameObject.GetComponent<CardDisplay2>().cooldown = cardToCheck.gameObject.GetComponent<CardDisplay2>().startingCooldown;
            //cardToCheck.gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

            int numberOfCards = GameManager.ins.combatCardArea.gameObject.transform.childCount;

            for (int i = 0; i < numberOfCards; i++)
            {
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber)
                {
                    //StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown = StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().startingCooldown;
                    //StoreHandler.ins.storeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip();

                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown -= timeToReduce;
                }
            }
        }
    }

    //disables particular card
    //dont affect cooldown
    public void DisableCombatCard(int cardNumber, int holderType)
    {
        //need to make sure the card exists
        if (ins.CheckQuantity(GameManager.ins.turnNumber, cardNumber, holderType) > 0)
        {
            //GameObject cardToCheck = ins.CopyCard(GameManager.ins.turnNumber, cardNumber, holderType);

            int numberOfCards = GameManager.ins.combatCardArea.gameObject.transform.childCount;

            for (int i = 0; i < numberOfCards; i++)
            {
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().numberInDeck == cardNumber)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
            }
        }
    }

    public void RefreshAllCurrentCombatCardsWithDelay()
    {
        //Invoke("RefreshAllCurrentCombatCards", 0.15f);
        Invoke("DeleteAndRespawnCombatCards", 0.1f);
    }

    //in v95, lets try delete whole card offer, and draw new cards
    public void DeleteAndRespawnCombatCards()
    {
        //should destroy the trading canvas objects
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveCombatCards();

        //Invoke("RespawnCombatCards", 0.1f);
        if (ins.phaseNumber == 3)
        {
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SpawnAttackButtonsWithDelay();
        }

        if (ins.phaseNumber == 4)
        {
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SpawnDefenseButtonsWithDelay();
        }
    }


    //for v94
    //actually better do it another way
    public void RefreshAllCurrentCombatCards()
    {
        int numberOfCards = GameManager.ins.combatCardArea.gameObject.transform.childCount;

        for (int i = 0; i < numberOfCards; i++)
        {
            //lets try it like this
            //test every cooldown & requirement in the combat card area
            //firstly dont refresh card that have been used this turn and flagged as this
            if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().cannotBeRefreshed == true)
            {
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled == false)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
            }

            //technically this is bit of an overkill tho, since mostly only energy and cooldown should be checked in v94
            else
            {
                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;

                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().cooldown > 0)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresEnergy)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresFavor)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresBombs > 0)
                {
                    //still check bomb quantity?
                    if (ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) < GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresBombs)
                    {
                        GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }
                }
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health < GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().requiresHealth)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                //need special case for focus buttons & prayers (since their energy cost dont refresh in the same way)
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().isFocusButton == true)
                {
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3 ||
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
                    {
                        GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }
                }
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().isPrayerButton == true)
                {
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3 ||
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
                    {
                        GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                    }
                }
            }
        }
    }

    //only used when starting new game
    public void ApplyBackground(int turnNumber, int cardNumber)
    {
        //260 card does nothing

        //advancement backgrounds
        if (cardNumber == 261)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
            Invoke("CallInstaLevelup", 0.25f);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fameForNextLevel = 25;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fameForNextLevelTreshold = 25;
        }
        if (cardNumber == 262)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
            Invoke("CallInstaLevelup", 0.25f);
            Invoke("CallInstaLevelup", 0.5f);
            Invoke("CallInstaLevelup", 0.75f);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fameForNextLevel = 43;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().fameForNextLevelTreshold = 43;
        }
        if (cardNumber == 263)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 5);
        }
        if (cardNumber == 264)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, 10);
        }


        //wealth bacgrounds
        if (cardNumber == 265)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 50);
        }
        if (cardNumber == 266)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, 100);
        }

        //class backgrounds
        if (cardNumber == 267)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 0, 2, 1);
        }
        if (cardNumber == 268)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 1, 2, 1);
        }
        if (cardNumber == 269)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 2, 2, 1);
        }
        if (cardNumber == 270)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 101, 2, 1);
        }

        //draw possible new upgrade cards
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DrawUpgradeCards3();

        //special backgrounds
        if (cardNumber == 271)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxStrength += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance -= 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(115);
            CardHandler.ins.RemoveSkillCardFromOffer(117);
        }

        if (cardNumber == 272)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxArcanePower += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxResistance += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence -= 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(119);
            CardHandler.ins.RemoveSkillCardFromOffer(121);
            CardHandler.ins.RemoveSkillCardFromOffer(131);
        }

        if (cardNumber == 273)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore -= 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(125);
            CardHandler.ins.RemoveSkillCardFromOffer(127);
        }

        if (cardNumber == 274)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxInfluence += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades += 1;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxLore += 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDigging -= 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

            //this actually only works for level 1
            CardHandler.ins.RemoveSkillCardFromOffer(123);
            CardHandler.ins.RemoveSkillCardFromOffer(129);
        }

        if (cardNumber == 275)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxObserve -= 1;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier += 20;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier += 20;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombModifier += 20;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyModifier += 20;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
        }

        if (cardNumber == 276)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxMechanics -= 1;

            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 161, 2, 1);
            CardHandler.ins.InstantPassiveEffect(GameManager.ins.turnNumber, 161);

            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 162, 2, 1);
            CardHandler.ins.InstantPassiveEffect(GameManager.ins.turnNumber, 162);

            //these cards require level 7 so dont need to remove anything?
            //there doesnt seem to be method for removing cards from offer specifically anyway? (other than skillpoint lvl 1 cards)

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
        }

        if (cardNumber == 277)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().maxDefense -= 1;

            //need special case here?
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 178, 2) > 0)
            {
                CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(183);
                CardHandler.ins.InstantPassiveEffect(GameManager.ins.turnNumber, 183);
            }
            else
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 178, 2, 1);
                CardHandler.ins.InstantPassiveEffect(GameManager.ins.turnNumber, 178);
            }
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 179, 2, 1);
            CardHandler.ins.InstantPassiveEffect(GameManager.ins.turnNumber, 179);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
        }


        //equipment backgrounds
        if (cardNumber == 278)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 30, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 37, 5, 1);
        }

        if (cardNumber == 279)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 34, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 42, 5, 1);
        }

        if (cardNumber == 280)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 40, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 196, 5, 1);
        }

        if (cardNumber == 281)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 37, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 45, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 194, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 169, 1, 5);
        }

        if (cardNumber == 282)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 79, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 76, 5, 1);
        }

        if (cardNumber == 283)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 49, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 59, 5, 1);
        }

        if (cardNumber == 284)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 63, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 72, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 20, 1, 5);
        }

        if (cardNumber == 285)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 51, 5, 1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 55, 5, 1);
        }
    }

    void CallInstaLevelup()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().InstaLevelup();
    }

    //only objective holder needs clearing in v95
    //11: objectives
    public void ClearCardHolder(int holder)
    {
        if (holder == 11)
        {
            int numberOfObjectives = GameManager.ins.objectiveCardArea.transform.childCount;

            for (int i = numberOfObjectives; i > 0; i--)
            {
                Destroy(GameManager.ins.objectiveCardArea.transform.GetChild(i - 1).gameObject);
            }
        }
    }

    public void PlaySfxWithDelay(AudioClip sfx)
    {
        tempSfx = sfx;

        Invoke(nameof(PlaySfx), 2.0f);
    }

    public void PlaySfx()
    {
        CardHandler.ins.intelligenceSfxHolder.clip = tempSfx;
        CardHandler.ins.intelligenceSfxHolder.Play();
    }
}
