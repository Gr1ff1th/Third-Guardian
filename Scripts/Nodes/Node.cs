using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Node : MonoBehaviour, IDataPersistence
{
    //the object to move on the node
    //public GameObject piece;
    //public GameObject piece = PhotonPlayer.myAvatar;

    public PhotonView PV;

    // node-list
    public List<Node> reachableNodes = new List<Node>();

    // route node-list (unused)
    //public List<RouteNode> routeNodes = new List<RouteNode>();

    // internal positions
    public List<InternalNode> internalNodes = new List<InternalNode>();

    //extra node (not used in v90)
    //public Node extraNode;

    //extra node list (list is better in case theres several extra nodes)
    public List<Node> extraNodes = new List<Node>();
    //public RouteNode extraRoute;

    //interaction plates
    //public GameObject interactionPlate0;
    //public GameObject interactionPlate1;
    //public GameObject interactionPlate2;

    //public GameObject interactionPlate0Upg;
    //public GameObject interactionPlate1Upg;
    //public GameObject interactionPlate2Upg;

    /*new interaction orbs (unused)
    public GameObject interactionOrb0;
    public GameObject interactionOrb1;
    public GameObject interactionOrb2;
    public GameObject interactionOrb3;
    public GameObject interactionOrbX;
    
    //new interaction orbs (unused as of v88)
    public Button OrbButton0;
    public Button OrbButton1;
    public Button OrbButton2;
    public Button OrbButtonX;
    */
    //public GameObject interactionPlateDisabled;

    //movementBonus orbs
    public GameObject goldMovement;
    public GameObject redMovement;

    //new gameobjects to disable when not current or adjacent
    public GameObject namePlate;
    public GameObject encountersPlate;
    public GameObject locationOrb;
    public GameObject encountersPlateImage;

    //action button canvas linked to this location
    public ABCanvas abCanvas;

    //for popup texts
    public GameObject toolTipBackground;
    //public Text toolTipText;
    //public string agentPlacementText;
    public string targettingText;

    // the image you want to fade, assign in inspector
    public Image goldImage;

    //gold location canvas, for toggling on/off
    public GameObject goldCanvas;

    //variable to tell if location is upgraded or not
    public bool isUpgraded;

    //variable to tell if location is target for something other than movementBonus
    public int specialAction;

    // something to click on
    [HideInInspector]
    public Collider col;

    //[HideInInspector]
    //public Node sentNode;

    public Interactable inter;

    //public int interactCost = 0;

    //counter for disabled interaction
    public int turnsDisabled;

    //number of the location
    //used for starting node loading also
    //note that the nodenumber needs to point to minimap number on transit nodes
    public int nodeNumber;

    //number of agents on the location
    //public int numberOfAgents;

    //stores reference to player Arriving to this node temporarily
    //public GameObject arrivingPlayer;

    //displays interaction energy cost (v88)
    //public TextMeshProUGUI energyCostText;

    //displays available encounters (v90)
    public TextMeshProUGUI availableEncountersText;

    public int movecost;

    //set true if this is location node, and difficulty progresses per gamestage
    public bool hasProgressiveDifficulty;

    //difficulty modifier (dont need it on encounter buttons anymore?)
    //if difficulty is progressive, this alters the GameManager.ins.gameStage modifier instead
    public int locationDifficultyModifier;

    //for keeping track which internalnode we dealing with (or which encounter)
    public int chosenInternalNode;

    //initiated when going into location
    //could also be used for special music in some cases (actually maybe combat music needs its own holder?)
    public AudioClip ambientSfxDay;
    public AudioClip ambientSfxNight;

    //sfx & music volume settings
    public float musicVolume;
    public float ambientSfxDayVolume;
    public float ambientSfxNightVolume;

    //set this to true on all overmap nodes
    public bool isLocationNode;

    //for calculating movecosts
    public bool isOvermapNode;

    //for marking sub-area transits
    public bool isSubAreaTransit;

    //for special bonfires on some areas
    public bool isBonfire;

    //keeps count of encounters on location nodes
    public int availableEncounters;

    //set this to true, if you want to open routes to the nodes on the extra nodes list, when this node is entered (bothways)
    public bool openAdjacentNodes;

    //locations should all have specific icon for new timed combat (v91)
    public Sprite targetBackgroundImage;
    public Sprite targetBackgroundImageNight;

    public bool isWayPoint;

    //only use these for randomized transits
    public List<GameObject> randomTransitButtons;
    //which random transit index to read
    public int transitIndex;
    //could use this to spawn special transit encounters (based on random transit roll and index)
    public List<GameObject> transitEncounters;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();

        col = GetComponent<Collider>();

        //remove colliders from nodes
        col.enabled = false;

        inter = GetComponent<Interactable>();

        //location isnt upgraded at start
        isUpgraded = false;

        //numberOfAgents = 0;

        turnsDisabled = 0;

        goldMovement.gameObject.SetActive(false);
        redMovement.gameObject.SetActive(false);

        //this actually resets energy cost texts now (as of v88)
        ResetButtons();

        if (namePlate != null)
        {
            namePlate.SetActive(false);
        }
        if (encountersPlate != null)
        {
            encountersPlate.SetActive(false);
        }
        if (locationOrb != null)
        {
            locationOrb.SetActive(false);
        }
    }

    void Start()
    {
        UpdateEncounterPlate();
        SetRandomizedTransits();
    }

    //could later update this to spawn encounters too
    public void SetRandomizedTransits()
    {
        //for v99 (activates certain button)
        if (randomTransitButtons != null)
        {
            for (int i = 0; i < randomTransitButtons.Count; i++)
            {
                if (GameManager.ins.mapRandomizeHandler.randomTransits[transitIndex] == i)
                {
                    abCanvas.button1 = randomTransitButtons[i];

                }
                //show specific transit encounter in some cases
                if (transitEncounters.Count > 0)
                {
                    if (GameManager.ins.mapRandomizeHandler.randomTransits[transitIndex] == i)
                    {
                        transitEncounters[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        transitEncounters[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void ResetButtons()
    {
        //OrbButton0.gameObject.SetActive(true);
        //OrbButton1.gameObject.SetActive(false);
        //OrbButton2.gameObject.SetActive(false);
        //OrbButtonX.gameObject.SetActive(false);

        //OrbButton0.interactable = false;
        //OrbButton1.interactable = false;
        //OrbButton2.interactable = false;
        //OrbButtonX.interactable = false;

        //energyCostText.text = "0<sprite index=11>";
}

    public void ActivateButtons()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            //OrbButton0.interactable = true;
            //OrbButton1.interactable = true;
            //OrbButton2.interactable = true;
            //OrbButtonX.interactable = true;

            /*
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy > interactCost && abCanvas.GetComponent<ABCanvas>().interactionDisabled == false)
            {
                OvermapExploreButton.SetActive(true);
            }
            */
            abCanvas.ActivateActionOrbs();
        }
    }

    public void DeactivateButtons()
    {
        //OrbButton0.interactable = false;
        //OrbButton1.interactable = false;
        //OrbButton2.interactable = false;
        //OrbButtonX.interactable = false;

        //OvermapExploreButton.SetActive(false);

        abCanvas.DeactivateActionOrbs();
    }

    //works for colliders of this class only
    void OnMouseDown()
    {
        //make sure options panel is not activated
        //bit hard way of doing this, consider changing this and gamemanager method later?
        //probably should only use 1 panel for these at least?
        if (GameManager.ins.dialogCanvas.GetComponent<CanvasController>().optionsPanelActivated == false &&
            GameManager.ins.uiButtonHandler.statsPanelActivated == false &&
            GameManager.ins.uiButtonHandler.equipmentPanelActivated == false &&
            GameManager.ins.uiButtonHandler.upgradePanelActivated == false)
        {
            /* better not use on v90, since encounters might block move commands otherwise?
             * for blocking physics raycasts going through ui elements
            //is kinda buggy tho, sometimes theres some invisible element in front of the node collider
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked on UI");
                return;
            }
            */
            
            //for movement
            if (specialAction == 0)
            {
                GameManager.ins.nextNode = this;

                //takes ViewID of the node the avatar is moving to
                //int nodeViewID = this.gameObject.GetPhotonView().ViewID;

                //new movecost system for v90
                int modifiedMoveCost;

                if (isOvermapNode == true)
                {
                    modifiedMoveCost = movecost - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().movementBonus; //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().overmapMoveCost;
                }
                else
                {
                    modifiedMoveCost = movecost;
                }

                if(modifiedMoveCost < 1)
                {
                    modifiedMoveCost = 1;
                }

                //sends the nodeviewid to charcontrollers method
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(gameObject, modifiedMoveCost, false, true);

                //actually maybe this dont need to be called here at all, can be put part of mover method
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MovePointReduction();
            }

            /*for agent placement
            if (specialAction == 1)
            {
                //takes ViewID of the node the agent is placed to
                int nodeViewID = this.gameObject.GetPhotonView().ViewID;

                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PlaceAgent(nodeViewID);

                //close tooltip
                toolTipBackground.SetActive(false);
            }

            //for sabotage
            if (specialAction == 2)
            {
                DisableInteraction();

                //close tooltip
                toolTipBackground.SetActive(false);

                //closes colliders and resets special actions
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

                //continues turn
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
            }

            //for motivate
            //unused
            if (specialAction == 3)
            {
                //close tooltip
                toolTipBackground.SetActive(false);

                //closes colliders and resets special actions
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

                //MotivateInteraction();

                //continues turn
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
            }

            //for displacement
            if (specialAction == 4)
            {
                //play sfx
                GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayTeleport();

                //close tooltip
                toolTipBackground.SetActive(false);

                //closes colliders and resets special actions
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

                //takes ViewID of the node the avatar is moving to
                int nodeViewID = this.gameObject.GetPhotonView().ViewID;

                //adds 1 "movementBonus", to put things as they were
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints += 1;

                //sends the nodeviewid to charcontrollers method
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Mover(nodeViewID);

                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
            }

            //for portals quest
            if (specialAction == 5)
            {
                //closes colliders and resets special actions
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

                //close tooltip
                toolTipBackground.SetActive(false);

                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().PV.RPC("RPC_TeleportPlayer", RpcTarget.AllBufferedViaServer, nodeNumber);
            }

            //for placing minefield
            if (specialAction == 6)
            {
                //message
                string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " created a minefield.";
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);

                //closes colliders and resets special actions
                GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().ResetNodes();

                //close tooltip
                toolTipBackground.SetActive(false);

                //get this locations ID
                int placement = gameObject.GetPhotonView().ViewID;

                //get card number of the intelligence card used
                int intCardNumber = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().temporaryNumber2;
                //int eventCardNumber = GameManager.ins.intelligenceDeck[intCardNumber].GetComponent<EventCard>().numberInEventDeck;

                //Debug.Log("placement, intcardnumber are: " + placement + " " + intCardNumber);

                //spawns the encounter for all to see
                GameManager.ins.eventCanvas.GetComponent<EventHandler>().PV.RPC("RPC_SpawnMinefield", RpcTarget.AllBufferedViaServer, intCardNumber, placement);

                //re-enables straight actions
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
            }
            */
        }
    }

    //works for colliders of this class only?
    void OnMouseOver()
    {
        //for movementBonus
        if (specialAction == 0)
        {
            // Check if the mouse was over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Pointer is on the UI");
                return;
            }

            GameManager.ins.toolTipBackground.SetActive(true);

            if (isOvermapNode == true)
            {
                int modifiedMoveCost = movecost - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().movementBonus;//GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().overmapMoveCost;

                if (modifiedMoveCost < 1)
                {
                    modifiedMoveCost = 1;
                }

                GameManager.ins.toolTipText.text = "Move cost: " + modifiedMoveCost + " <sprite index=32>";
            }
            else
            {
                GameManager.ins.toolTipText.text = "Move cost: " + movecost + " <sprite index=32>";
            }
        }

        //these are deprecated
        /*for agent placement phase
        if (specialAction == 1)
        {
            toolTipBackground.SetActive(true);
            GameManager.ins.toolTipText.text = agentPlacementText.ToString();
        }
        */
        //for sabotage & motivate & displacement targetting
        if (specialAction == 2 || specialAction == 3 || specialAction == 4 || specialAction == 5 || specialAction == 6)
        {
            toolTipBackground.SetActive(true);
            GameManager.ins.toolTipText.text = targettingText.ToString();
        }
    }

    //works for colliders of this class only?
    void OnMouseExit()
    {
        // Check if the mouse was over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Pointer is on the UI");
            return;
        }

        GameManager.ins.toolTipBackground.SetActive(false);

        //for agent placement phase
        if (specialAction == 1)
        {
            toolTipBackground.SetActive(false);
            //toolTipText.text = agentPlacementText.ToString();
        }
        //for sabotage & motivate & displacement targetting
        if (specialAction == 2 || specialAction == 3 || specialAction == 4 || specialAction == 5 || specialAction == 6)
        {
            toolTipBackground.SetActive(false);
            //toolTipText.text = agentPlacementText.ToString();
        }
    }

    //for first arrival only
    public void FirstArrive(GameObject avatar)
    {
        //make sure currentNode isnt null (for start of game), then
        /*leave existing currentNode
        if (GameManager.ins.currentNode != null)
        {
            GameManager.ins.currentNode.Leave();
        }
        */

        //set currentNode
        GameManager.ins.currentNode = this;

        //move the piece to first available internal node
        avatar.transform.position = GetSlot(avatar).transform.position;

        /* shouldnt need these now?
        //turn off own collider
        if (col != null)
        {
            col.enabled = false;
        }

        foreach (Node node in reachableNodes)
        {
            if (node.col != null)
            {
                node.col.enabled = true;

                //node.SetMovementOrb(true);
                                
            }
        }

        //make this object interactable
        if (inter != null)
        {
            //keeps nodes collider!?
            inter.enabled = true;

            //dunno if this enables non-current players too? (need to test)
            ActivateButtons();
        }
        */

    }

    //dont rly need this, since game should call movement before game start anyway
    public void OpenRoutes()
    {
        //opens adjacent nodes if this flag is true
        if (openAdjacentNodes == true)
        {
            for (int i = 0; i < extraNodes.Count; i++)
            {
                AddRoute(i);
            }
        }
    }

    //for other arrivals, except for primary movementBonus
    public void InstantArrive(GameObject avatar)
    {
        //Debug.Log("instant arrive is called");

        //make sure currentNode isnt null (for start of game), then
        //leave existing currentNode
        if (GameManager.ins.currentNode != null)
        {
            GameManager.ins.currentNode.LeaveNode(avatar);
        }

        //lets test this here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().CheckIfHeroDead() == true)
        {
            return;
        }

        avatar.GetComponentInChildren<Image>().sprite = avatar.GetComponentInChildren<Character>().facingRight;

        //set currentNode
        GameManager.ins.currentNode = this;

        //move the piece to first available internal node
        avatar.transform.position = GetSlot(avatar).transform.position;

        //turn off own collider
        if (col != null)
        {
            col.enabled = false;
        }

        //gine the basic overlays for current location
        EnableLocationOverlays(true);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
        {
            //opens adjacent nodes if this flag is true
            if (openAdjacentNodes == true)
            {
                for (int i = 0; i < extraNodes.Count; i++)
                {
                    AddRoute(i);
                }
            }

            //should show current nodes encounters too?
            if (gameObject.GetComponent<NodeEncounterHandler>() != null)
            {
                gameObject.GetComponent<NodeEncounterHandler>().ShowEncounters(false);
            }

            foreach (Node node in reachableNodes)
            {
                //should make check here if player can still move
                //we shouldnt rly need extra movementBonus check anymore?
                //node.col != null
                if (node.col != null && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove == true)// && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                {
                    node.col.enabled = true;

                    node.SetMovementOrb(true);
                    //Debug.Log("collider enabled");

                    //makes current node encounters "untaken"
                    node.RefreshEncounters();

                    //makes encounters visible (either id on unid)
                    if (node.GetComponent<NodeEncounterHandler>() != null)
                    {
                        node.GetComponent<NodeEncounterHandler>().ShowEncounters(false);
                    }
                }
            }

            //make this object interactable
            if (inter != null)
            {
                //keeps nodes collider!?
                //SetReachableNodes(true);
                inter.enabled = true;

                ActivateButtons();
            }
        }


        //put sleeptest here
        //its bit dangerous, since now sleep timer is reduced each time arrive method is called (except when event is drawn)
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().SleepTest();
    }

    //for other arrivals, except for primary movementBonus
    public void Arrive(GameObject avatar)
    {
        //Debug.Log("arrive is called");

        //make sure currentNode isnt null (for start of game), then
        //leave existing currentNode
        if (GameManager.ins.currentNode != null)
        {
            GameManager.ins.currentNode.LeaveNode(avatar);
        }

        //switches the avatar rotation depending on which direction avatar is moving
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].gameObject.transform.position.x < GameManager.ins.nextNode.gameObject.transform.position.x)
        {
            //Debug.Log("moving right");
            //bool test1 = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Animator>().GetBool("Mirror");

            //avatar.transform.rotation = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<AvatarSetup>().headRight;

            avatar.GetComponentInChildren<Image>().sprite = avatar.GetComponentInChildren<Character>().facingRight;
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].gameObject.transform.position.x > GameManager.ins.nextNode.gameObject.transform.position.x)
        {
            //Debug.Log("moving left ");
            //avatar.transform.rotation = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<AvatarSetup>().headLeft;

            avatar.GetComponentInChildren<Image>().sprite = avatar.GetComponentInChildren<Character>().facingLeft;
        }

        //set currentNode
        GameManager.ins.currentNode = this;

        //changes the lerping destination to first available internal node
        GameManager.ins.lerpPos2 = GameManager.ins.currentNode.GetSlot(avatar).transform.position;

        //lets use different variables for this, to make sure movementBonus can finish
        //GameManager.ins.movementNumber = GameManager.ins.turnNumber;

        //changes the flag variable
        GameManager.ins.startMoving2 = true;

    }

    //continues arrive method, called from gamemanager update method
    //note that this uses current players turnnumber now always (is it a problem?)
    public void ArriveContinued()
    {
        //GameObject avatar = GameManager.ins.avatars[GameManager.ins.turnNumber];

        /* dont have agents anymore
         * check if theres your agent on your currentNode, and gives skill bonus if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents != null)
        {
            for (int i = 0; i < GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents.Count; i++)
            {
                if (ReferenceEquals(GameManager.ins.currentNode, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents[i].GetComponent<Agent>().standingOn))
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().AgentBonus(1);
                }
            }
        }
        */

        //lets test this here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().CheckIfHeroDead() == true)
        {
            return;
        }

        //turn off own collider
        if (col != null)
        {
            col.enabled = false;
        }

        //gine the basic overlays for current location
        EnableLocationOverlays(true);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
        {
            foreach (Node node in reachableNodes)
            {
                //should make check here if player can still move
                if (node.col != null && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove == true)// && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                {
                    node.col.enabled = true;

                    node.SetMovementOrb(true);
                    //Debug.Log("collider enabled");

                    //makes current node encounters "untaken"
                    node.RefreshEncounters();

                    //makes encounters visible (either id on unid)
                    if(node.GetComponent<NodeEncounterHandler>() != null)
                    {
                        node.GetComponent<NodeEncounterHandler>().ShowEncounters(false);
                    }
                }
            }

            //make this object interactable
            if (inter != null)
            {
                //keeps nodes collider!?
                //SetReachableNodes(true);
                inter.enabled = true;

                ActivateButtons();
            }
        }

        //lets try this here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned > 0)
        {
            GameManager.ins.currentNode.SetReachableNodes(false);

            //straight actions should be disabled when imprisoned
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;
        }

        //put sleeptest here
        //its bit dangerous, since now sleep timer is reduced each time arrive method is called (except when event is drawn)
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().SleepTest();
    }


    //for npc arrivals, except for primary movementBonus
    public void NpcArrive(GameObject npc)
    {
        //set currentNode
        //GameManager.ins.currentNode = this;

        //move the piece to first available internal node
        npc.transform.position = GetSlot2(npc).transform.position;

        //change position slightly, might need to do this on all agent / npc position changes
        //actually this should be unnecessary soon
        npc.transform.position += new Vector3(0, 0, 0); //0.52f);
    }

    //for character arrivals, not on his turn
    public void NonTurnArrive(GameObject avatar, int turnNumber)
    {
        /*check if theres your agent on the node you moving to, and gives skill bonus if so
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().agents != null)
        {
            for (int i = 0; i < avatar.GetComponent<CharController>().agents.Count; i++)
            {
                if (ReferenceEquals(avatar.GetComponent<CharController>().standingOn, avatar.GetComponent<CharController>().agents[i].GetComponent<Agent>().standingOn))
                {
                    avatar.GetComponent<CharController>().AgentBonus(1);
                }
            }
        }
        */
        //move the piece to first available internal node
        avatar.transform.position = GetSlot2(avatar).transform.position;

    }

    //new method for primary movementBonus only, takes route nodes into account
    //actually route not needed, this method is still only used for current player
    public void MoveBetweenNodes(GameObject avatar)
    {
        //switches the avatar rotation depending on which direction avatar is moving
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].gameObject.transform.position.x < GameManager.ins.nextNode.gameObject.transform.position.x)
        {
            avatar.GetComponentInChildren<Image>().sprite = avatar.GetComponentInChildren<Character>().facingRight;

        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].gameObject.transform.position.x > GameManager.ins.nextNode.gameObject.transform.position.x)
        {
            //Debug.Log("moving left ");
            //avatar.transform.rotation = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<AvatarSetup>().headLeft;

            avatar.GetComponentInChildren<Image>().sprite = avatar.GetComponentInChildren<Character>().facingLeft;
        }

        //make sure currentNode isnt null (for start of game), then
        //leave existing currentNode
        if (GameManager.ins.currentNode != null)
        {
            GameManager.ins.currentNode.LeaveNode(avatar);
        }
        
        //set currentNode
        GameManager.ins.currentNode = this;

        //changes the lerping destination to first available internal node
        GameManager.ins.lerpPos = GameManager.ins.currentNode.GetSlot(avatar).transform.position;

        //lets use different variable for this, to make sure movementBonus can finish
        GameManager.ins.movementNumber = GameManager.ins.turnNumber;

        //changes the flag variable
        GameManager.ins.startMoving = true;

        //lets try if this bugs out
        MoveBetweenNodesContinued();
    }

    public void MoveBetweenNodesContinued()
    {
        CharController character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>();

        if (isWayPoint == true)
        {
            GameManager.ins.references.MakeWaypointChecks(nodeNumber);
        }

        //turn off own collider
        if (col != null)
        {
            col.enabled = false;
        }

        if(character.turnEnded == true)
        {
            character.EndTurn();
            return;
        }

        //check if player fell asleep while moving
        //end turn here if so
        //otherwise opens movementBonus and interaction options
        //actually is the delay here even needed anymore?
        Invoke(nameof(DelayedMovementFinish), 0.5f);
    }

    //continuation for movebetweennodes method
    //is needed to sync the isSpleeping variable, which is set by rpc call
    void DelayedMovementFinish()
    {
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 21, 7) > 0) //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isSleeping > 0) (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().sleptWhileMoving == true)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isSleeping -= 1;
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 21, 7, 1);

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
            return;
        }

        //lets do its own method for this
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().CheckIfHeroDead() == true)
        {
            return;
        }

        //make encounter check here
        //interrupt the arrive method here, is such encounter is found
        if (CheckStrategicEncounters() == true)
        {
            return;
        }

        EnableLocationOverlays(true);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
        {
            //opens adjacent nodes if this flag is true
            if (openAdjacentNodes == true)
            {
                for (int i = 0; i < extraNodes.Count; i++)
                {
                    AddRoute(i);
                }
            }

            //check this again, just to make sure
            //actually dont rly need this check on v90?
            //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
            //{
            foreach (Node node in reachableNodes)
            {
                if (node.col != null)//(node.col != null)
                {
                    node.col.enabled = true;

                    //Debug.Log("node col should be enabled");

                    node.SetMovementOrb(true);

                    //makes current node encounters "untaken"
                    node.RefreshEncounters();

                    //makes encounters visible (either id on unid)
                    if (node.GetComponent<NodeEncounterHandler>() != null)
                    {
                        node.GetComponent<NodeEncounterHandler>().ShowEncounters(false);
                    }
                }
            }
            //

            //make this object interactable
            if (inter != null)
            {
                //keeps nodes collider!?
                //SetReachableNodes(true);
                inter.enabled = true;

                ActivateButtons();
            }
        }
    }
    

    //for arrivals of current player
    public InternalNode GetSlot(GameObject avatar)
    {
        for (int i = 0; i <= 8; i++)
        {
            //use index number to look at empty slots in the list
            InternalNode iNode = GameManager.ins.currentNode.internalNodes[i];
            if (iNode.isTaken == false)
            {
                iNode.isTaken = true;

                //give internal node number to player
                if(avatar.GetComponent<CharController>() != null)
                {
                    avatar.GetComponent<CharController>().internalNode = iNode;
                }

                return iNode;
            }
        }
        //should probably change this
        return GameManager.ins.currentNode.internalNodes[0];
    }

    //for slots inside this node only
    //for arrivals of any gameobject (not necessarily character)
    public InternalNode GetSlot2(GameObject being)
    {
        for (int i = 0; i <= 8; i++)
        {
            //use index number to look at empty slots in the list
            InternalNode iNode = internalNodes[i];
            if (iNode.isTaken == false)
            {
                //give internal node number to player
                if (being.GetComponent<CharController>() != null)
                {
                    being.GetComponent<CharController>().internalNode = iNode;
                }

                //give internal node number to npc
                if (being.GetComponent<StrategicEncounter>() != null)
                {
                    being.GetComponent<StrategicEncounter>().internalNode = iNode;
                }

                iNode.isTaken = true;
                return iNode;
            }
        }
        //should probably change this
        return internalNodes[0];
    }

    //tells first free internal node
    public InternalNode TellFreeSlot()
    {
        for (int i = 0; i <= 8; i++)
        {
            //use index number to look at empty slots in the list
            InternalNode iNode = internalNodes[i];
            if (iNode.isTaken == false)
            {
                return iNode;
            }
        }
        //should probably change this
        return internalNodes[0];
    }

    //right click for interactions, initiated from GameManager
    public void RightClick()
    {
        if (inter != null && inter.enabled)
        {
            inter.Interact();
            return;
        }

        //dunno why this is here
        //Arrive(GameManager.ins.avatars[0]);
    }

    public void Leave()
    {
        /*check if theres your agent on the node your leaving from, and removes skill bonus if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents != null)
        {
            for (int i = 0; i < GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents.Count; i++)
            {
                if (ReferenceEquals(GameManager.ins.currentNode, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents[i].GetComponent<Agent>().standingOn))
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().AgentBonus(-1);
                }
            }
        }
        */
        //this should be here?
        DeactivateButtons();

        //turn off all reachable colliders
        SetReachableNodes(false);

        if (inter != null)
        {
            inter.enabled = false;
        }
    }

    //leaves node permanently, releasing internal node
    public void LeaveNode(GameObject avatar)
    {
        /*
        //check if theres your agent on the node your leaving from, and removes skill bonus if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents != null)
        {
            for (int i = 0; i < GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents.Count; i++)
            {
                if (ReferenceEquals(GameManager.ins.currentNode, GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().agents[i].GetComponent<Agent>().standingOn))
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().AgentBonus(-1);
                }
            }
        }
        */
        //release internal node
        if (avatar.GetComponent<CharController>() != null)
        {
            avatar.GetComponent<CharController>().internalNode.isTaken = false;
        }

        EnableLocationOverlays(false);

        //turn off all reachable colliders
        SetReachableNodes(false);

        if (inter != null)
        {
            inter.enabled = false;

            DeactivateButtons();
        }
    }

    //leaving node on not your turn
    //leaves internal node as well
    public void NonTurnLeave(int turnNumber)
    {
        /*
        //check if theres your agent on the node your leaving from, and removes skill bonus if so
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().agents != null)
        {
            for (int i = 0; i < GameManager.ins.avatars[turnNumber].GetComponent<CharController>().agents.Count; i++)
            {
                //made this more logical
                if (ReferenceEquals(GameManager.ins.avatars[turnNumber].GetComponent<CharController>().standingOn, GameManager.ins.avatars[turnNumber].GetComponent<CharController>().agents[i].GetComponent<Agent>().standingOn))
                {
                    GameManager.ins.avatars[turnNumber].GetComponent<CharController>().AgentBonus(-1);
                }
            }
        }
        */

        //release internal node
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>() != null)
        {
            GameManager.ins.avatars[turnNumber].GetComponent<CharController>().internalNode.isTaken = false;
        }
    }


    //this leave function doesnt remove the agent bonus
    public void LeaveForQuest()
    {
        //turn off all reachable colliders
        SetReachableNodes(false);

        if (inter != null)
        {
            inter.enabled = false;
        }
    }

    public void SetReachableNodes(bool set)
    {
        //set reachable colliders
        foreach (Node node in reachableNodes)
        {
            if (node.col != null)
            {
                node.col.enabled = set;

                node.SetMovementOrb(set);
            }
        }
    }

    /*when changing interaction cost
    //actually doesnt need to change "plate" anymore, just the orb
    public void ChangePlate()
    {
        //
        interactCost += 1;

        if (interactCost > 3)
        {
            interactCost = 3;
        }

        if (interactCost == 1 && turnsDisabled == 0)
        {
            //OrbButton0.gameObject.SetActive(false);
            //OrbButton1.gameObject.SetActive(true);
            //OrbButton2.gameObject.SetActive(false);
            //OrbButtonX.gameObject.SetActive(false);

            energyCostText.text = "1<sprite index=11>";
        }
        if (interactCost == 2 && turnsDisabled == 0)
        {
            energyCostText.text = "2<sprite index=11>";
        }

        if (interactCost == 3 && turnsDisabled == 0)
        {
            energyCostText.text = "3<sprite index=11>";

        }

        //not rly needed now
        if (interactCost > 3 && turnsDisabled == 0)
        {
            energyCostText.text = "<sprite index=23>";

            //disables interaction buttons
            abCanvas.GetComponent<ABCanvas>().interactionDisabled = true;
        }
        
}
*/

    //for the initial upgrade, starts the goldening effect also
    public void UpgradePlate()
    {
        isUpgraded = true;

        MakeGolden();
    }

    //for v90
    public void ReduceInteractionCost()
    {
        /*
        if (interactCost > 0)
        {
            interactCost -= 1;
        }
        energyCostText.text = interactCost + "<sprite index=11>";

        abCanvas.GetComponent<ABCanvas>().interactionDisabled = false;
        */
    }

    /* old resetplate
        public void ResetPlate()
        {

            interactCost = 0;

            if (turnsDisabled == 0)
            {
                /*
                interactionOrb0.SetActive(true);
                interactionOrb1.SetActive(false);
                interactionOrb2.SetActive(false);
                interactionOrb3.SetActive(false);
                interactionOrbX.SetActive(false);

                OrbButton0.gameObject.SetActive(true);
                OrbButton1.gameObject.SetActive(false);
                OrbButton2.gameObject.SetActive(false);
                OrbButtonX.gameObject.SetActive(false);


                energyCostText.text = "0<sprite index=11>";

                abCanvas.GetComponent<ABCanvas>().interactionDisabled = false;
            }

            if (isUpgraded == true)
            {
                //interactionPlate0Upg.SetActive(true);
                //interactionPlate1Upg.SetActive(false);
                //interactionPlate2Upg.SetActive(false);
            }

            if(turnsDisabled > 0)
            {

                OrbButton0.gameObject.SetActive(false);
                OrbButton1.gameObject.SetActive(false);
                OrbButton2.gameObject.SetActive(false);
                OrbButtonX.gameObject.SetActive(true);


                energyCostText.text = "<sprite index=23>";
            }

        }
    */

    //for the initial upgrade, upgrades action buttons
    public void UpgradeInteractions()
    {

        abCanvas.UpgradeInteractions();

        //isUpgraded = true;

        /* old way
        sets the upgraded interaction(s)
        if (abCanvas.interaction1Upg != null)
        {
            abCanvas.interaction1Start.SetActive(false);
            abCanvas.a = abCanvas.interaction1Upg;
            abCanvas.a.SetActive(true);
        }

        if (abCanvas.interaction2Upg != null)
        {
            abCanvas.interaction2Start.SetActive(false);
            abCanvas.b = abCanvas.interaction2Upg;
            abCanvas.b.SetActive(true);
        }
        */
    }

    /* old motivate
    public void MotivateInteraction()
    {
        //give the player "extra" turn
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().specialEffect = 2;

        //Debug.Log("gameobject is: " + gameObject.GetComponentInChildren<ABCanvas>().gameObject);

        abCanvas.GetComponent<ABCanvas>().gameObject.SetActive(true);

        //invoke the button effect
        if (abCanvas.GetComponent<ABCanvas>().isUpgraded == false)
        {
            abCanvas.GetComponent<ABCanvas>().interaction1Start.GetComponent<Action>().isMotivateInteraction = true;
            abCanvas.GetComponent<ABCanvas>().interaction1Start.GetComponent<Button>().onClick.Invoke();
            abCanvas.GetComponent<ABCanvas>().interaction1Start.GetComponent<Action>().isMotivateInteraction = false;
        }
        if (abCanvas.GetComponent<ABCanvas>().isUpgraded == true)
        {
            abCanvas.GetComponent<ABCanvas>().interaction1Upg.GetComponent<Action>().isMotivateInteraction = true;
            abCanvas.GetComponent<ABCanvas>().interaction1Upg.GetComponent<Button>().onClick.Invoke();
            abCanvas.GetComponent<ABCanvas>().interaction1Upg.GetComponent<Action>().isMotivateInteraction = false;
        }

        //changes the plate
        //this should be an rpc call
        //ChangePlate();
        PV.RPC("RPC_ChangePlate", RpcTarget.AllBufferedViaServer);

        //update energy
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -interactCost);
    }

    //increases interaction cost
    [PunRPC]
    void RPC_ChangePlate()
    {
        ChangePlate();
    }
    */

    //disables the location interaction
    public void DisableInteraction()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_DisableInteraction", RpcTarget.AllBufferedViaServer);
        }
    }

    //disables the location interaction
    [PunRPC]
    void RPC_DisableInteraction()
    {
        turnsDisabled = 24;

        //interactionPlate0.SetActive(false);
        //interactionPlate1.SetActive(false);
        //interactionPlate2.SetActive(false);

        //interactionPlate0Upg.SetActive(false);
        //interactionPlate1Upg.SetActive(false);
        //interactionPlate2Upg.SetActive(false);
        /*
        interactionOrb0.SetActive(false);
        interactionOrb1.SetActive(false);
        interactionOrb2.SetActive(false);
        interactionOrb3.SetActive(false);

        //set the "disabled" orb visible
        interactionOrbX.SetActive(true);
        
        OrbButton0.gameObject.SetActive(false);
        OrbButton1.gameObject.SetActive(false);
        OrbButton2.gameObject.SetActive(false);
        OrbButtonX.gameObject.SetActive(true);
        */

        //energyCostText.text = "<sprite index=23>";

        //disables interaction buttons
        abCanvas.GetComponent<ABCanvas>().interactionDisabled = true;
    }

    /*disables the location interaction
    public void ReduceDisabledTimer()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_ReduceDisabledTimer", RpcTarget.AllBufferedViaServer);
        }
    }

    //disables the location interaction
    [PunRPC]
    void RPC_ReduceDisabledTimer()
    {
        turnsDisabled -= 1;

        if (turnsDisabled == 0)
        {
            //enables the buttons again
            abCanvas.GetComponent<ABCanvas>().interactionDisabled = false;

            //set the "disabled" plate invisible
            interactionPlateDisabled.SetActive(false);

            abCanvas.GetComponent<ABCanvas>().interactionDisabled = false;

            //resets the interaction plate
            ResetPlate();
        }
    }
    */
    public void MakeGolden()
    {
        // fades the image in when you click
        StartCoroutine(FadeImageIn(true));
    }

    IEnumerator FadeImageIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            goldCanvas.SetActive(true);

            goldImage.color = new Color(0, 0, 0);

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                goldImage.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }

    //can be used on any extra nodes in v90
    public void AddRoute(int extraNodeNumber)
    {
        int sameRoutesFound = 0;

        for (int i = 0; i < reachableNodes.Count; i++)
        {
            if (ReferenceEquals(reachableNodes[i], extraNodes[extraNodeNumber]))
            {
                sameRoutesFound += 1;
            }
        }
        //only add new route, if it doesnt alrdy exists
        if (sameRoutesFound == 0)
        {
            reachableNodes.Add(extraNodes[extraNodeNumber]);

            //give message
            string msgs = "New route opened!";
            GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
        }

        //also add current location node to as reachable node the extra node, if it doesnt have that alrdy
        sameRoutesFound = 0;

        for (int i = 0; i < extraNodes[extraNodeNumber].reachableNodes.Count; i++)
        {
            //need different check for location and overmap nodes?
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == false)
            {
                if (ReferenceEquals(extraNodes[extraNodeNumber].reachableNodes[i], GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode))
                {
                    sameRoutesFound += 1;
                }
            }
            else
            {
                if (ReferenceEquals(extraNodes[extraNodeNumber].reachableNodes[i], gameObject.GetComponent<Node>()))
                {
                    sameRoutesFound += 1;
                }
            }
        }
        //only add new route, if it doesnt alrdy exists
        if (sameRoutesFound == 0)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == false)
            {
                extraNodes[extraNodeNumber].reachableNodes.Add(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode);
            }
            else
            {
                extraNodes[extraNodeNumber].reachableNodes.Add(gameObject.GetComponent<Node>());
            }

        }
    }

    public void SetMovementOrb(bool set)
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        if (set == false)
        {
            goldMovement.SetActive(false);
            redMovement.SetActive(false);

            if (namePlate != null)
            {
                namePlate.SetActive(false);
            }
            if (encountersPlate != null)
            {
                encountersPlate.SetActive(false);
            }
            if (locationOrb != null)
            {
                locationOrb.SetActive(false);
            }
            return;
        }
        //actually in v91 this should always be called, if it gets here?
        //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints > 0)
        //{
            goldMovement.SetActive(true);
            redMovement.SetActive(false);

            if (namePlate != null)
            {
                namePlate.SetActive(true);
            }
            if(encountersPlate != null)
            {
                encountersPlate.SetActive(true);
            }
            if(locationOrb != null)
            {
                locationOrb.SetActive(true);
            }
        //}

        /*this should never be called on v90 tho?
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints == 0)
        {
            goldMovement.SetActive(false);
            redMovement.SetActive(true);
        }
        */
        /* dont need this since v85
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().movementBonus == 0 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 0)
        {
            goldMovement.SetActive(false);
            redMovement.SetActive(false);
        }
        */
    }

    //checks if there are unhandled encounters on your node
    //returns true, if such encounter is found
    public bool CheckStrategicEncounters()
    {
        /*return false if player has stealth perk
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StealthTest() == true)
        {
            return false;
        }
        // need to remake this later
        for(int i = 0; i < GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters.Count; i++)
        {
            if(ReferenceEquals(GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters[i].GetComponent<Npc>().standingOn, gameObject.GetComponent<Node>()))
            {
                if (GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters[i].GetComponent<Npc>().isForced == true &&
                    GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters[i].GetComponent<Npc>().isChecked == false)
                {
                    //only do the interaction if player doesnt have stealth
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                    {
                        GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters[i].GetComponent<Npc>().EncounterInteraction();
                    }

                    //GameManager.ins.eventCanvas.GetComponent<EventHandler>().encounters[i].GetComponent<Npc>().isChecked = true;
                    return true;
                }
            }
        }
        */
        for (int i = 0; i < internalNodes.Count; i++)
        {
            if (internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>() != null)
            {
                //modified for v94
                if(internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().isChecked == false && 
                    internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().NonInteractable == false &&
                    (internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().encounter.isForcedEncounter == true ||
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().interactWithAnyEncounter == true))
                {
                    //perhaps grab the encounter2 info here as well, and send to encounterhandler.encounteroption as RPC call? 
                    //actually might not need rpc call, since it alrdy does rpc call to get here?
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().encounter;

                    //kinda need need second reference too to the actual instantiated strategic encounter unfortunately
                    GameManager.ins.references.currentStrategicEncounter = internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>();
                    GameManager.ins.references.currentEncounter = internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().encounter;

                    //remove stealth
                    GameManager.ins.references.currentStrategicEncounter.hasStealth = false;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                    {
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetStrategicEncounter(true);
                    }

                    chosenInternalNode = i;

                    internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().isChecked = true;

                    //for the purpose of multi-stage encounters
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved = false;

                    return true;
                }
            }
        }

        return false;
    }

    //updates the available encounters counter
    public void UpdateAvailableEncountersCounter(int counter)
    {
        availableEncountersText.text = counter + "<sprite=\"torch\" index=1>";
    }

    //for first arrival only
    public void FirstArriveOnMinimap(GameObject avatar)
    {
        //set currentNode
        GameManager.ins.currentNode = this;

        //move the piece to first available internal node
        avatar.transform.position = GetSlot(avatar).transform.position;

    }

    //enables basic info for current location
    public void EnableLocationOverlays(bool activate)
    {
        if (activate == true)
        {
            if (namePlate != null)
            {
                namePlate.SetActive(true);
            }
            if (encountersPlate != null)
            {
                encountersPlate.SetActive(true);
            }
            if (locationOrb != null)
            {
                locationOrb.SetActive(true);
            }
        }
        if (activate == false)
        {
            if (namePlate != null)
            {
                namePlate.SetActive(false);
            }
            if (encountersPlate != null)
            {
                encountersPlate.SetActive(false);
            }
            if (locationOrb != null)
            {
                locationOrb.SetActive(false);
            }
        }
    }

    //
    public void RefreshEncounters()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().interactWithAnyEncounter = false;

        for (int i = 0; i < internalNodes.Count; i++)
        {
            //use index number to look at empty slots in the list
            InternalNode iNode = internalNodes[i]; // GameManager.ins.currentNode.internalNodes[i];

            if (iNode.transform.childCount > 0)
            {
                if(iNode.transform.GetChild(0).gameObject.GetComponent<StrategicEncounter>() != null)
                {
                    iNode.transform.GetChild(0).gameObject.GetComponent<StrategicEncounter>().isChecked = false;
                }
            }
        }
    }

    //opposite of refresh
    public void ExhaustEncounters()
    {
        for (int i = 0; i < internalNodes.Count; i++)
        {
            //use index number to look at empty slots in the list
            InternalNode iNode = internalNodes[i]; // GameManager.ins.currentNode.internalNodes[i];

            if (iNode.transform.childCount > 0)
            {
                if (iNode.transform.GetChild(0).gameObject.GetComponent<StrategicEncounter>() != null)
                {
                    iNode.transform.GetChild(0).gameObject.GetComponent<StrategicEncounter>().isChecked = true;
                }
            }
        }
    }

    //used for interaction button
    public bool CheckEncounters()
    {
        for (int i = 0; i < internalNodes.Count; i++)
        {
            //use index number to look at empty slots in the list
            InternalNode iNode = GameManager.ins.currentNode.internalNodes[i];

            if (iNode.transform.childCount > 0)
            {
                if (iNode.transform.GetChild(0).gameObject.GetComponent<StrategicEncounter>() != null)
                {
                    if (iNode.transform.GetChild(0).gameObject.GetComponent<StrategicEncounter>().NonInteractable == false)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //handles encounter plate info
    //including encounter count, encounter tooltip and encounter plate recoloring
    public void UpdateEncounterPlate()
    {
        if (availableEncountersText != null)
        {
            availableEncountersText.text = availableEncounters + "<sprite=\"torch\" index=1>";
        }

        int modifiedGameStage = 1;

        //get the difficulty level of the location
        if (hasProgressiveDifficulty) 
        {
            modifiedGameStage = GameManager.ins.gameStage + locationDifficultyModifier;
        }
        if (!hasProgressiveDifficulty)
        {
            modifiedGameStage = locationDifficultyModifier;
        }
        string difficultyText = "";

        //use icons for showing difficulty
        for(int i = 0; i < modifiedGameStage; i++)
        {
            difficultyText += "<sprite index=42>";
        }

        if (encountersPlateImage != null)
        {
            encountersPlateImage.GetComponent<CustomTooltips>().customText = "<b>Available Encounters</b><br>Total available encounters on this location: <b>" + availableEncounters +
                "</b><br><color=#00FFFC>Threat level: <b>" + encountersPlateImage.GetComponent<CustomTooltips>().threatLevel + "</b><br><color=#00FFFC>Difficulty level: " + difficultyText;

            if(hasProgressiveDifficulty == true)
            {
                encountersPlateImage.GetComponent<CustomTooltips>().customText += " (progressive difficulty).";
            }

            if (hasProgressiveDifficulty == false)
            {
                encountersPlateImage.GetComponent<CustomTooltips>().customText += " (fixed difficulty).";
            }

            if (encountersPlateImage.GetComponent<CustomTooltips>().levelRequirement != 0)
            {
                encountersPlateImage.GetComponent<CustomTooltips>().customText += "</b><br><color=#00FFFC>Level requirement: <b>" + encountersPlateImage.GetComponent<CustomTooltips>().levelRequirement + "</b>.";
            }

            if (encountersPlateImage.GetComponent<CustomTooltips>().lightstoneRequirement == true)
            {
                encountersPlateImage.GetComponent<CustomTooltips>().customText += "</b><br><color=#00FFFC>Lightstone required.";
            }

            /* actually lets not do this
             * addtionally change the encounterplateimage color to match difficulty
            if (modifiedGameStage == 1)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(66, 181, 199, 255);
            }
            if (modifiedGameStage == 2)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(85, 121, 43, 255);
            }
            if (modifiedGameStage == 3)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(236, 227, 36, 255);
            }
            if (modifiedGameStage == 4)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(199, 138, 66, 255);
            }
            if (modifiedGameStage == 5)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(241, 73, 52, 255);
            }
            if (modifiedGameStage == 6)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(231, 78, 171, 255);
            }
            if (modifiedGameStage == 7)
            {
                encountersPlateImage.GetComponent<Image>().color = new Color32(172, 64, 255, 255);
            }
            */
        }
    }

    //load / save encounter count inside location
    public void LoadData(GameData data)
    {
        if (isLocationNode == true && data.hasVisitedMinimap[nodeNumber] == true)
        {
            availableEncounters = data.totalEncounters[nodeNumber];
        }
        else if (isSubAreaTransit == true && data.hasVisitedMinimap[nodeNumber] == true)
        {
            //hmm
            availableEncounters = data.totalEncounters[nodeNumber];
        }
    }

    public void SaveData(ref GameData data)
    {
        if (isLocationNode == true)
        {
            data.totalEncounters[nodeNumber] = availableEncounters;
            //lets save the available encounters for sub areas on the minimap controller
        }
    }

}
