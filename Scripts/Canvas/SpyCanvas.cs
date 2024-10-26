using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SpyCanvas : MonoBehaviour
{
    //the canvases
    public GameObject spyCanvas;
    public GameObject spyPrompt;

    //spy canvas text fields
    public Text strengthText2;
    public Text influenceText2;
    public Text mechanicsText2;
    public Text diggingText2;
    public Text loreText2;
    public Text observeText2;

    public Text energyText2;
    public Text warriorsText2;
    public Text artisansText2;
    public Text arcanistsText2;
    public Text agentsText2;
    public Text coinsText2;
    public Text honorText2;
    public Text honorTotalText2;

    //could set this just 0+0
    public Text movement2;

    //for changing the text on the button
    public Text heroNameText2;

    //for changing the text on the prompt
    public Text spyPromptText;

    //for displaying the "unknown" hand cards text
    public Text unknownText;

    //for changing the text on the button
    public Text spyPromptButton;

    //list of hero icons
    public List<GameObject> characterIcons2 = new List<GameObject>();

    //card areas
    public GameObject handCardArea;
    public GameObject artifactCardArea;
    public GameObject handCardArea2;
    public GameObject artifactCardArea2;

    //for cancelling spying / stealing. Actually might not be needed, unless we wanna return the used int card.
    //public int cancelType;


    //tNumber = turnNumber
    //spyType 1 = regular spy, spyType 2 = claim quest, spytype 3 = memory steal, 4 = artifact "steal", 5 = eye of isolore, etc
    public void Spy(int tNumber, int spyType)
    {
        //activate the canvas
        spyCanvas.SetActive(true);

        /* not needed anymore
         * 
         * show the assets & skills of target player
        //lets use the same turn variable as in attack cards, for simplicitys sake
        strengthText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().strength.ToString();
        influenceText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().influence.ToString();
        mechanicsText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().mechanics.ToString();
        diggingText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().digging.ToString();
        loreText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().lore.ToString();
        observeText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().observe.ToString();
        */

        energyText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().energy.ToString();
        warriorsText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().warriors.ToString();
        artisansText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().artisans.ToString();
        arcanistsText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().arcanists.ToString();
        agentsText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().agents.ToString();
        coinsText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().coins.ToString();
        honorText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().fame.ToString();
        honorTotalText2.text = GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().honorTotal.ToString();
        movement2.text = "0+0";

        //spyed hero name
        heroNameText2.text = GameManager.ins.avatars[GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber].GetComponentInChildren<Character>().heroName.ToString();

        //displays hero icon (notice this uses heronumber instead of turnnumber)
        //DisplayHero2(GameManager.ins.avatars[GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber].GetComponentInChildren<Character>().heroNumber);
        DisplayHero2(GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().heroNumber);

        //displays heros artifact cards
        //DisplayArtifacts2(GameManager.ins.avatars[GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber].GetComponentInChildren<Character>().heroNumber);
        DisplayArtifacts2(tNumber);

        unknownText.gameObject.SetActive(true);

        //check for pretenders orb
        //could be used for "mirror" quest later too?
        if (spyType == 1)
        {
            spyPromptText.text = "Spying on opponent..";
            spyPromptButton.text = "OK";

            PretendersOrb(tNumber);
        }

        //show quest cards (claim quest int card)
        if (spyType == 2)
        {
            //disables the unknown text
            unknownText.gameObject.SetActive(false);

            spyPromptText.text = "Choose 1 Quest card to steal:";
            spyPromptButton.text = "Cancel";

            DisplayHandCards(tNumber, 1);

            //what type of card to steal
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 7;

            //special cardfunction
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 10;
        }

        //show intelligence cards (memory steal int card)
        if (spyType == 3)
        {
            //disables the unknown text
            unknownText.gameObject.SetActive(false);

            spyPromptText.text = "Choose 1 hand card to steal:";
            spyPromptButton.text = "Cancel";

            DisplayHandCards(tNumber, 3);

            //what type of card to steal
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 8;

            //special cardfunction
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 10;
        }

        //scroll of acquisition
        if (spyType == 4)
        {
            //disables the unknown text
            //unknownText.gameObject.SetActive(false);

            spyPromptText.text = "Choose 1 Artifact card to buy:";
            spyPromptButton.text = "Cancel";

            //lets check for pretenders orb here too
            PretendersOrb(tNumber);

            //what type of card to steal
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().wantToReturn = 9;

            //special cardfunction
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 11;
        }

        //eye of isolore quest
        if (spyType == 5)
        {
            //disables the unknown text
            unknownText.gameObject.SetActive(false);

            spyPromptText.text = "Spying on opponent..";
            spyPromptButton.text = "OK";

            DisplayHandCards(tNumber, 3);
        }
    }

    public void DisplayHero2(int hNumber)
    {
        for (int i = 0; i < characterIcons2.Count; i++)
        {
            if (i == hNumber)
            {
                characterIcons2[i].gameObject.SetActive(true);
            }
            else
            {
                characterIcons2[i].gameObject.SetActive(false);
            }
        }
    }

    public void DisplayArtifacts2(int tNumber)
    {
        for (int i = 0; i < artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            //duplicates all artifact cards of the spied player to the spy window
            //dont need artifact check for this now, since other card types should be shown too
            //if (artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null) 
            //{
            if (artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == tNumber)
            {
                if (artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardType == 8)
                {
                    GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);

                    playerCard.transform.SetParent(artifactCardArea2.transform, false);

                    //turns the card active
                    playerCard.SetActive(true);
                }
                if (artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardType == 9)
                {
                    GameObject playerCard = Instantiate(GameManager.ins.artifactDeck[artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);

                    playerCard.transform.SetParent(artifactCardArea2.transform, false);

                    //turns the card active
                    playerCard.SetActive(true);
                }
                if (artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardType == 10)
                {
                    GameObject playerCard = Instantiate(GameManager.ins.eventDeck[artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);

                    playerCard.transform.SetParent(artifactCardArea2.transform, false);

                    //turns the card active
                    playerCard.SetActive(true);
                }
                //perk cards
                if (artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardType == 11)
                {
                    GameObject playerCard = Instantiate(GameManager.ins.perkDeck[artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);

                    playerCard.transform.SetParent(artifactCardArea2.transform, false);

                    //turns the card active
                    playerCard.SetActive(true);
                }
            }
            //}
        }
        //checks if artifacts go above 8
        if (GameManager.ins.avatars[tNumber].GetComponentInChildren<Character>().artifactCards > 8) // && GameManager.ins.avatars[tNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //changes the content size fitter horizontal fit mode
            //for that player only hopefully
            artifactCardArea2.GetComponent<ScrollRectCenter>().ChangeSizeFitter();

            Debug.Log("The spytarget has over 8 artifacts!");
        }
    }

    /*
    public void SpyCancel()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        DestroyDisplayedCards();

        //activate the canvas
        spyCanvas.SetActive(false);


        //takes ViewID of the node the avatar is moving to
        int nodeViewID = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().gameObject.GetPhotonView().ViewID;

        //adds 1 "movementBonus", to put things as they were
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints += 1;

        //sends the nodeviewid to charcontrollers method
        //returns players "turn"
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

        //just in case
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().cardFunction = 0;

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
    }
    */

    public void DestroyDisplayedCards()
    {
        int artifactCount = artifactCardArea2.GetComponent<Transform>().childCount -1;
        
        for (int i = artifactCount; i >= 0; i--)
        {
            Destroy(artifactCardArea2.transform.GetChild(i).gameObject);
        }

        int handCardCount = handCardArea2.GetComponent<Transform>().childCount - 1;

        for (int i = handCardCount; i >= 0; i--)
        {
            Destroy(handCardArea2.transform.GetChild(i).gameObject);
        }
    }

    //shows opponents hand cards, if player has pretenders orb
    public void PretendersOrb(int tNumber)
    {
        //tests if player has pretenders orb
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 35 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                {
                    //disables the unknown text
                    unknownText.gameObject.SetActive(false);

                    DisplayHandCards(tNumber, 3);
                }
            }
        }
    }

    //showType 1= show quests, showType 2= show intelligence, showType 3 = show both
    public void DisplayHandCards(int tNumber, int showType)
    {
        for (int i = 0; i < handCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == tNumber)
            {
                if (handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardType == 7 && (showType == 1 || showType == 3))
                {
                    GameObject playerCard = Instantiate(GameManager.ins.questDeck[handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);

                    playerCard.transform.SetParent(handCardArea2.transform, false);

                    //turns the card active
                    playerCard.SetActive(true);
                }

                if (handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().cardType == 8 && (showType == 2 || showType == 3))
                {
                    GameObject playerCard = Instantiate(GameManager.ins.intelligenceDeck[handCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);

                    playerCard.transform.SetParent(handCardArea2.transform, false);

                    //turns the card active
                    playerCard.SetActive(true);
                }
            }
        }
    }
}
