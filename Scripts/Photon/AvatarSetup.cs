using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSetup : MonoBehaviour
{
    public PhotonView PV;
    public int characterValue;
    public GameObject myCharacter;

    //save the rotations, made the hard way :-)
    public Quaternion headRight;
    public Quaternion headLeft;

    //keeps track of turns (unused?)
    public bool myTurn;

    public CardSaveHandler cardSaveHandler;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            if (PhotonRoom.room.isPractice == false)
            {
                //first command specifies the method, AllBuffered makes clients joining later still get the info, overload comes last
                // change to AllBufferedViaServer for better ordering?

                //changed this for offline mode
                if (PhotonNetwork.OfflineMode == true)
                {
                    Invoke(nameof(AddCharacter), 0.5f);
                }
                else
                {
                    PV.RPC("RPC_AddCharacter", RpcTarget.AllBufferedViaServer, PlayerInfo.PI.mySelectedCharacter);
                }

                /*old tests
                GameManager.ins.startingNode.Arrive(myCharacter);
                Debug.Log("Arrived at: " + GameManager.ins.startingNode);


                GameManager.ins.AddPlayer(myCharacter);
                //test
                //GameManager.ins.GM.RPC("AddPlayer", RpcTarget.AllBuffered, myCharacter);
                //PV.RPC("GameManager.ins.AddPlayer", RpcTarget.AllBuffered, myCharacter);

                //PV.RPC("RPC_PlaceCharacter", RpcTarget.AllBuffered, myCharacter);
                */

                //get reference to card save handler
                cardSaveHandler = GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>();
            }

            //for main player in practice mode
            if (PhotonRoom.room.isPractice == true && GameManager.ins.avatars.Count == 0)
            {
                //first command specifies the method, AllBuffered makes clients joining later still get the info, overload comes last
                // change to AllBufferedViaServer for better ordering?
                PV.RPC("RPC_AddCharacter", RpcTarget.AllBufferedViaServer, PlayerInfo.PI.mySelectedCharacter);

                //get reference to card save handler
                cardSaveHandler = GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>();
            }

            //for bots in sp mode
            if (PhotonRoom.room.isPractice == true && GameManager.ins.avatars.Count > 0)
            {
                //make sure each player has unique character
                int randomCharacter = 0;
                bool isValidHero = false;
                do
                {
                    randomCharacter = Random.Range(0, 9);
                    bool conflictFound = false;
                    for (int i = 0; i < GameManager.ins.avatars.Count; i++)
                    {
                        if(GameManager.ins.avatars[i].GetComponentInChildren<Character>().heroNumber == randomCharacter)
                        {
                            conflictFound = true;
                        }
                    }
                    if(conflictFound == false)
                    {
                        isValidHero = true;
                    }
                }
                while (isValidHero == false);

                //first command specifies the method, AllBuffered makes clients joining later still get the info, overload comes last
                // change to AllBufferedViaServer for better ordering?
                PV.RPC("RPC_AddCharacter", RpcTarget.AllBufferedViaServer, randomCharacter);

                //get reference to card save handler
                cardSaveHandler = GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>();
            }

            //disable UI buttons to begin with
            GameManager.ins.uiButtonHandler.DisableAllButtons();
        }
    }

    void AddCharacter()
    {
        PV.RPC("RPC_AddCharacter", RpcTarget.AllBufferedViaServer, PlayerInfo.PI.mySelectedCharacter);
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        //take the character number from savefile in v0.7.1.
        //overwrites the incoming value in this case
        //might be problematic way of doing this, since we need to reset many values..
        if(PhotonRoom.room.spContinue == true)
        {
            whichCharacter = GameManager.ins.references.chosenCharacter;
            PlayerPrefs.SetInt("MyCharacter", whichCharacter);
            GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>().mySelectedCharacter = whichCharacter;
        }

        characterValue = whichCharacter;
        GameManager.ins.references.chosenCharacter = whichCharacter;

        //last transform is for setting parent
        myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
        //GameObject.Find("MainBoard").GetComponent<Transform>().

        myCharacter.SetActive(true);

        //set bot flag here
        if (PhotonRoom.room.isPractice == true && GameManager.ins.avatars.Count > 0)
        {
            gameObject.GetComponent<CharController>().isAi = true;
            //gameObject.GetComponentInChildren<Character>().heroNumber = whichCharacter;
        }

        //set starting movementBonus point texts
        if (PV.IsMine)
        {
            //gameObject.GetComponent<CharController>().CountMovementPoints();

            //lets do it like this for now
            gameObject.GetComponent<CharController>().actionPoints = myCharacter.GetComponent<Character>().maxActionPoints;//16;
            //gameObject.GetComponent<CharController>().CountActionPoints();
            gameObject.GetComponent<CharController>().isExploring = false;
            //GameObject.Find("MovementText").GetComponent<Text>().text = gameObject.GetComponent<CharController>().movementBonus.ToString() + "+" + gameObject.GetComponent<CharController>().extraMovement.ToString();

            //just in case, test if this works here: seems not
            //gameObject.GetComponentInChildren<Character>().UpdateResourceTexts();
        }

        //gameObject.transform.localxscale = -1;


        /*the hard way to make rotation & scale :-)
        gameObject.transform.eulerAngles = new Vector3(
            gameObject.transform.eulerAngles.x + 90,
            gameObject.transform.eulerAngles.y + 180,
            gameObject.transform.eulerAngles.z
            );
        */

        //reduce size 70%
        myCharacter.gameObject.transform.localScale += new Vector3(2.4f, 2.4f, 2.4f);

        //change position slightly
        myCharacter.gameObject.transform.position += new Vector3(0, 0, 0); // 0.52f);

        GameManager.ins.startingNode.FirstArrive(gameObject);

        //update current node for charcontroller script also
        gameObject.GetComponent<CharController>().standingOn = GameManager.ins.startingNode;
        gameObject.GetComponent<CharController>().locationNode = GameManager.ins.startingNode;

        GameManager.ins.AddPlayer(gameObject);

        //update number of players variable
        GameManager.ins.numberOfPlayers += 1;

        //get turnnumber
        for (int y = 0; y < GameManager.ins.avatars.Count; y++)
        {
            if (ReferenceEquals(gameObject, GameManager.ins.avatars[y].gameObject)) // && PV.IsMine)
            {
                gameObject.GetComponent<CharController>().turnNumber = y;

                //reset death variable
                gameObject.GetComponent<CharController>().isDead = false;

                //show your characters hero and score display at bottom left corner
                //GameManager.ins.scoreCanvasController.GetComponent<ScoreCanvasController>().ShowCharacter(y, whichCharacter);

                //if turnNumber is 0, then show starting movementBonus options
                //actially lets use this for initiating starting card selection sequence
                if (y == 0 && PV.IsMine)
                {
                    //lets try this
                    gameObject.GetComponent<CharController>().PV = gameObject.GetComponent<CharController>().GetComponent<PhotonView>();

                    //leave the node just in case
                    gameObject.GetComponent<CharController>().standingOn.Leave();
                    gameObject.GetComponent<CharController>().straightActionAllowed = false;

                    gameObject.GetComponent<CharController>().cardFunction = 4;

                    //show hand cards at the beginning (actually this could be called later)
                    //GameManager.ins.uiButtonHandler.HandCardsButtonPressed();

                    //GameManager.ins.InitialCardSelection();

                    StartCoroutine(AllInGame(true));

                    //need to add starting artifact & quest offer tho
                    //GameManager.ins.ArtifactReset();

                    //generate cards in shops, but only if its not continue game
                    if (PhotonRoom.room.spContinue == false)
                    {
                        StoreHandler.ins.DrawStage1Cards();
                    }

                    //GameManager.ins.QuestOfferReset();
                }
            }
        }

        if (gameObject.GetComponent<CharController>().isAi == false)
        {
            //gives 8 starting cards
            //make sure only this player calls this
            //StartingCards();
            if (PV.IsMine)
            {
                //lets remove starting quest for now.. (not necessary and doesnt seem to work properly atm)
                //GameObject.Find("Quest Plate Handler").GetComponent<QuestPlates>().DrawQuestOnBoard();

                //special case for tutorial
                if (GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonRoom>().isTutorial == true)
                {
                    /*take gereons game
                    int cardNumber = 0;
                    GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;
                    PV.RPC("RPC_StartingCards", RpcTarget.AllBufferedViaServer, cardNumber, 7);
                    */
                   
                    CheckSaves();

                }

                //if its not tutorial, allow card selection
                else
                {
                    /*
                    //maybe take 4 quest handcards at start
                    for (int i = 0; i < 4; i++)
                    {
                        int cardNumber = GameManager.ins.TakeQuestCardNumber();

                        GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                        PV.RPC("RPC_StartingCards", RpcTarget.AllBufferedViaServer, cardNumber, 7);
                    }


                    //Debug.Log("players turnnumber is: " + gameObject.GetComponent<CharController>().turnNumber);
                    //spawn intelligence starting cards, note that this has special statement
                    for (int i = 0; i < 0; i++)
                    {
                        int cardNumber = GameManager.ins.TakeIntelligenceCardNumber2(gameObject.GetComponent<CharController>().turnNumber);

                        GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                        PV.RPC("RPC_StartingCards", RpcTarget.AllBufferedViaServer, cardNumber, 8);
                    }
                    */

                    CheckSaves();

                }
            }
        }
        /* no ai in play atm
        else
        {
            //maybe take 2 quest handcards at start
            for (int i = 0; i < 2; i++)
            {
                int cardNumber = GameManager.ins.TakeQuestCardNumber();

                GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

                PV.RPC("RPC_StartingCards", RpcTarget.AllBufferedViaServer, cardNumber, 7);
            }
            //fix this soon
            CheckSaves();
        }
        */
    }

    //tests that all players are in main game scene
    IEnumerator AllInGame(bool isTesting)
    {
        if (isTesting)
        {
            bool allInGame = false;

            int playersInRoom = GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().playersInRoom;

            while (allInGame == false)
            {
                if (GameManager.ins.avatars.Count == playersInRoom)//(GameManager.ins.numberOfPlayers == playersInRoom)
                {
                    //GameManager.ins.InitialCardSelection();
                    //allInGame = true;

                    if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().isTutorial == true)
                    {
                        Invoke(nameof(EnableTutorialOk), 5.0f);
                    }
                    else
                    {
                        Invoke(nameof(EnablePrologueOk), 5.0f);
                    }

                    //this actually works, but dunno if timing is right
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    //could use this if the lobbies become visible at some point
                    //PhotonNetwork.CurrentRoom.IsVisible = false;
                    yield break;
                }
                yield return null;
            }
        }
    }

    public void EnableTutorialOk()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().TutorialStartOk.interactable = true;
    }

    public void EnablePrologueOk()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().PrologueOk.interactable = true;
    }

    /* old start cards
    [PunRPC]
    void RPC_StartingCards(int cardNumber, int cardType)
    {
        if (cardType == 7)
        {
            //set the flag
            GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

            //instantiates random quest card from the deck
            GameObject playerCard = Instantiate(GameManager.ins.questDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(GameManager.ins.handCardArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //set the "owner" variable to the card
            //playerCard.GetComponent<CardDisplay>().belongsTo = gameObject.GetComponent<CharController>().turnNumber;

            //unless its your character and your PV
            for (int y = 0; y < GameManager.ins.avatars.Count; y++)
            {
                if (ReferenceEquals(gameObject, GameManager.ins.avatars[y].gameObject) && PV.IsMine)// && gameObject.GetComponent<CharController>().isAi == false)
                {
                    if (gameObject.GetComponent<CharController>().isAi == false)
                    {
                        playerCard.SetActive(true);
                    }

                    //places quest on board also
                    //note that ai players now place hidden cards on board
                    PV.RPC("RPC_DrawQuestCardOnBoard2", RpcTarget.AllBufferedViaServer, cardNumber, y);
                }

                if (ReferenceEquals(gameObject, GameManager.ins.avatars[y].gameObject))
                {
                    //set the "owner" variable to the card
                    playerCard.GetComponent<CardDisplay>().belongsTo = GameManager.ins.avatars[y].gameObject.GetComponent<CharController>().turnNumber;
                }
            }
        }

        if (cardType == 8)
        {
            GameManager.ins.intelligenceDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

            //instantiates random quest card from the deck
            //GameObject playerCard = Instantiate(GameManager.ins.TakeIntelligenceCard(gameObject.GetComponent<CharController>().turnNumber), new Vector3(0, 0, 0), Quaternion.identity);
            GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(GameManager.ins.handCardArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(false);

            //set the "owner" variable to the card
            //playerCard.GetComponent<CardDisplay>().belongsTo = gameObject.GetComponent<CharController>().turnNumber;

            //unless its your character and your PV
            for (int y = 0; y < GameManager.ins.avatars.Count; y++)
            {
                if (ReferenceEquals(gameObject, GameManager.ins.avatars[y].gameObject) && PV.IsMine)
                {
                    playerCard.SetActive(true);
                }
                if (ReferenceEquals(gameObject, GameManager.ins.avatars[y].gameObject))
                {
                    //set the "owner" variable to the card
                    playerCard.GetComponent<CardDisplay>().belongsTo = GameManager.ins.avatars[y].gameObject.GetComponent<CharController>().turnNumber;
                }
            }

        }
    }
    */

    // checks the saved character info
    // similar function is done at selectorscript2 for the character selection scene
    // wont need to read the playerprefs a second time though, since its alrdy updated in the cardsavehandler
    public void CheckSaves()
    {
        Invoke(nameof(CheckSaves2), 4.5f);

        Invoke(nameof(CheckBackgrounds), 6.0f);
    }

    public void CheckBackgrounds()
    {
        //dont do these for continue games
        if(PhotonRoom.room.spContinue == false)
        {
            /*add these temporarily for testing
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 82, 5, 1); 
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 83, 5, 1);
            */
            CardHandler.ins.ApplyBackground(GameManager.ins.turnNumber, PhotonRoom.room.background1);
            CardHandler.ins.ApplyBackground(GameManager.ins.turnNumber, PhotonRoom.room.background2);
            CardHandler.ins.ApplyBackground(GameManager.ins.turnNumber, PhotonRoom.room.background3);
        }
    }

    public void CheckSaves2()
    {
        Debug.Log("Checking saved perk cards.. Heronumber is: " + GetComponentInChildren<Character>().heroNumber);

        if (GetComponent<CharController>().isAi == false)
        {
            //checks if beren is selected
            if (GetComponentInChildren<Character>().heroNumber == 0)
            {

                for (int i = 0; i < cardSaveHandler.berenCards.Length; i++)
                {
                    //GameObject playerCard = Instantiate(cardSaveHandler.perkDeck[cardSaveHandler.berenCards[i]], new Vector3(0, 0, 0), Quaternion.identity);
                    int cardNumber = cardSaveHandler.berenCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);

                    //playerCard.transform.SetParent(perkCardArea.transform, false);
                }
            }

            //checks if suliman is selected
            if (GetComponentInChildren<Character>().heroNumber == 1)
            {
                for (int i = 0; i < cardSaveHandler.sulimanCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.sulimanCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if dazzle is selected
            if (GetComponentInChildren<Character>().heroNumber == 2)
            {
                for (int i = 0; i < cardSaveHandler.dazzleCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.dazzleCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if maximus is selected
            if (GetComponentInChildren<Character>().heroNumber == 3)
            {
                for (int i = 0; i < cardSaveHandler.maximusCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.maximusCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if melissya is selected
            if (GetComponentInChildren<Character>().heroNumber == 4)
            {
                for (int i = 0; i < cardSaveHandler.melissyaCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.melissyaCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if targas is selected
            if (GetComponentInChildren<Character>().heroNumber == 5)
            {
                for (int i = 0; i < cardSaveHandler.targasCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.targasCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if naomi is selected
            if (GetComponentInChildren<Character>().heroNumber == 6)
            {
                for (int i = 0; i < cardSaveHandler.naomiCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.naomiCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if ariel is selected
            if (GetComponentInChildren<Character>().heroNumber == 7)
            {
                for (int i = 0; i < cardSaveHandler.arielCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.arielCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if enigma is selected
            if (GetComponentInChildren<Character>().heroNumber == 8)
            {
                for (int i = 0; i < cardSaveHandler.enigmaCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.enigmaCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //checks if rimlic is selected
            if (GetComponentInChildren<Character>().heroNumber == 9)
            {
                for (int i = 0; i < cardSaveHandler.rimlicCards.Length; i++)
                {
                    int cardNumber = cardSaveHandler.rimlicCards[i];

                    PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, cardNumber);
                }
            }

            //draw extra portal scroll, if its not continue game
            //also septimus quest
            // dont need either in v95
            if (PhotonRoom.room.spContinue == false)
            {
                //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 189, 1, 1);
                //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 190, 11, 1);
            }
        }

        //separate case for ai players, so that they are always vanilla characters
        else
        {
            //checks if beren is selected
            if (GetComponentInChildren<Character>().heroNumber == 0)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 0);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 3);
            }

            //checks if suliman is selected
            if (GetComponentInChildren<Character>().heroNumber == 1)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 2);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 17);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 18);
            }

            //checks if dazzle is selected
            if (GetComponentInChildren<Character>().heroNumber == 2)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 1);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 4);
            }

            //checks if maximus is selected
            if (GetComponentInChildren<Character>().heroNumber == 3)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 0);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 6);
            }

            //checks if melissya is selected
            if (GetComponentInChildren<Character>().heroNumber == 4)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 2);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 13);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 14);
            }

            //checks if targas is selected
            if (GetComponentInChildren<Character>().heroNumber == 5)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 1);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 9);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 10);
            }

            //checks if naomi is selected
            if (GetComponentInChildren<Character>().heroNumber == 6)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 0);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 7);
            }

            //checks if ariel is selected
            if (GetComponentInChildren<Character>().heroNumber == 7)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 2);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 15);
            }

            //checks if enigma is selected
            if (GetComponentInChildren<Character>().heroNumber == 8)
            {
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 1);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 11);
                PV.RPC("RPC_PlacePerkCard", RpcTarget.AllBufferedViaServer, 12);
            }
        }
    }

    //gives player number over network to character class
    [PunRPC]
    void RPC_PlacePerkCard(int cardNumber)
    {
        Debug.Log("Placing perk cards..");

        //GameObject playerCard = Instantiate(GameManager.ins.perkDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);
        //lets do this differently for now
        GameObject playerCard = Instantiate(GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>().generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

        //note that we can only set one of these at a time now
        if (playerCard.GetComponent<Card>().isPassive == true)
        {
            playerCard.transform.SetParent(GameManager.ins.artifactCardArea.transform, false);
        }
        if(playerCard.GetComponent<Card>().isUsable == true && playerCard.GetComponent<Card>().isPassive == false)
        {
            playerCard.transform.SetParent(GameManager.ins.handCardArea.transform, false);
        }

        //turns the card inactive
        playerCard.SetActive(false);

        //set the "owner" variable to the card
        playerCard.GetComponent<Card>().belongsTo = GetComponent<CharController>().turnNumber;

        //unless its your card
        if (PV.IsMine == true && gameObject.GetComponent<CharController>().isAi == false)
        {
            playerCard.SetActive(true);

            GameManager.ins.handCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForUsableCards();

            GameManager.ins.artifactCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForAbilityCards();
        }
        //cant seem to make this work properly
        CardHandler.ins.InstantPassiveEffect(GetComponent<CharController>().turnNumber, playerCard.GetComponent<Card>().numberInDeck);
    }

    [PunRPC]
    void RPC_DrawQuestCardOnBoard2(int cardNumber, int turnNumber)
    {
        GameManager.ins.questDeck[cardNumber].GetComponent<CardDisplay>().isTaken = true;

        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(GameManager.ins.questDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in quest plates area
        playerCard.transform.SetParent(GameManager.ins.questPlateHandler.GetComponent<QuestPlates>().questPlates[GameManager.ins.questDeck[cardNumber].GetComponent<QuestCard>().qDestination - 1].transform, false);

        //set the "owner" variable to the card
        playerCard.GetComponent<CardDisplay>().belongsTo = turnNumber;

        //turns the card inactive
        playerCard.SetActive(false);

        //unless its your character and your PV
        for (int y = 0; y < GameManager.ins.avatars.Count; y++)
        {
            if (ReferenceEquals(gameObject, GameManager.ins.avatars[y].gameObject) && PV.IsMine && GameManager.ins.avatars[y].GetComponent<CharController>().isAi == false)
            {
                playerCard.SetActive(true);
            }
        }
    }
}
