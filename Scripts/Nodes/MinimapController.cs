using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour, IDataPersistence
{
    public List<Node> minimapNodes;

    //public Node startingNode;

    //lets update this for multiple possible start nodes
    public List<Node> startingNodes;

    //only used on certain locations
    //when previous node has specific node number, startingnode is set as indexnumber of this list
    public int[] customStartWhenPreviousNodeIs;

    public PhotonView PV;

    //singleton
    //public static MinimapController ins;

    public GameObject myCharacter;

    //keep track of which minimap we dealing with
    //same as list index number in references.nodes
    public int minimapNumber;

    //need the build index too for loading purposes (could be useful elsewhere too)
    public int sceneBuildIndex;

    //use the minimap info instead of location node info on sub-areas, when true
    public bool isSubArea;

    //set true if difficulty progresses per gamestage (only used for sub-areas)
    public bool hasProgressiveDifficulty;

    //difficulty modifier (only used for sub-areas)
    //if difficulty is progressive, this alters the GameManager.ins.gameStage modifier instead
    public int locationDifficultyModifier;

    //only used for sub-areas
    public int availableEncounters;

    //location name
    public string locationName;

    // the image you want to fade, assign in inspector
    public Image nightImage;

    //night map canvas, for toggling on/off
    public GameObject nightCanvas;

    // the image you want to fade, assign in inspector
    public Image overlayNightImage;

    //night map canvas, for toggling on/off
    public GameObject overlayDayCanvas;
    public GameObject overlayNightCanvas;

    //locations should all have specific icon for new timed combat (v91)
    //set these only for sub areas, other areas can be put into node itself
    public Sprite targetBackgroundImage;
    public Sprite targetBackgroundImageNight;

    //initiated when going into location
    //could also be used for special music in some cases (actually maybe combat music needs its own holder?)
    //kinda preferable to have these here instead on nodes
    public AudioClip ambientSfxDay;
    public AudioClip ambientSfxNight;

    //sfx & music volume settings
    public float musicVolume;
    public float ambientSfxDayVolume;
    public float ambientSfxNightVolume;

    //for triggered overlays
    //numbers check savefile, whether you have that overlay triggered
    public List<int> triggeredOverlayNumbers;
    public List<GameObject> triggeredOverlays;

    void Awake()
    {
        GameManager.ins.references.currentMinimap = gameObject.GetComponent<MinimapController>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //ins = this;

        PV = GetComponent<PhotonView>();

        Invoke(nameof(FirstArrive), 0.5f);
        //FirstArrive();

        //this doesnt need to be called very early?
        //actually should not need this now?
        //RemovePermanentlyTaken();

        SetBackGround();

        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("currently active scene is number: " + scene.buildIndex);

        //lets do this here?
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PlayLocationMusic();

        //special case for citadel in v94
        if(minimapNumber == 73 || minimapNumber == 74)
        {
            Clock.clock.citadelEntered = true;
        }
    }

    //sets certain background at start
    public void SetBackGround()
    {
        if (nightCanvas != null)
        {
            if (Clock.clock.isNight == true)
            {
                nightCanvas.SetActive(true);

                if (nightImage != null)
                {
                    nightImage.color = new Color(1, 1, 1, 1);
                }
                if (overlayNightImage != null)
                {
                    overlayDayCanvas.SetActive(false);
                    overlayNightCanvas.SetActive(true);
                    overlayNightImage.color = new Color(1, 1, 1, 0.7f);
                }
            }
            else if (nightImage != null)
            {
                nightImage.color = new Color(1, 1, 1, 0);

                if (overlayNightImage != null)
                {
                    overlayDayCanvas.SetActive(true);
                    overlayNightImage.color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    /*
    public void RemovePermanentlyTaken()
    {
        for(int i = 0; i < GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().permanentlyTakenButtons.Count; i++)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().permanentlyTakenButtons[i].isTakenPermanently = false;
        }
    }
    */

    public void FirstArrive()
    {
        //GameManager.ins.nextNode = startingNode;

        //disable main scene camera
        GameManager.ins.camera.enabled = false;

        //takes ViewID of the node the avatar is moving to
        //int nodeViewID = startingNode.gameObject.GetPhotonView().ViewID;
        //this is kinda dumb way of doing it tho, couldve simply made any node a possible starting node (wouldve worked better with save / load system at least)
        //int nodeViewID = startingNodes[GameManager.ins.references.startingNodeNumber].gameObject.GetPhotonView().ViewID;
        GameObject sendNode = startingNodes[GameManager.ins.references.startingNodeNumber].gameObject;


        /* old
         * check if previous node is location node
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode != null)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode.isLocationNode == true && customStartWhenPreviousNodeIs != null)
            {
                for (int i = 0; i < customStartWhenPreviousNodeIs.Length; i++)
                {
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousNode.nodeNumber == customStartWhenPreviousNodeIs[i])
                    {
                        nodeViewID = startingNodes[i].gameObject.GetPhotonView().ViewID;
                    }
                }
            }
        }
        */

        //custom starting node on certain locations, depending on the location youre coming from
        //check previouslocationnode in v94
        //should save this variable always when moving from location to another?
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousLocationNode != null && customStartWhenPreviousNodeIs != null)
        {
            for (int i = 0; i < customStartWhenPreviousNodeIs.Length; i++)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().previousLocationNode.nodeNumber == customStartWhenPreviousNodeIs[i])
                {
                    //nodeViewID = startingNodes[i].gameObject.GetPhotonView().ViewID;
                    sendNode = startingNodes[i].gameObject;
                }
            }
        }

        //only used when loading from save atm
        if (GameManager.ins.references.useSpecificNodeForSpawn != 999)
        {
            //nodeViewID = minimapNodes[GameManager.ins.references.useSpecificNodeForSpawn].gameObject.GetPhotonView().ViewID;
            sendNode = minimapNodes[GameManager.ins.references.useSpecificNodeForSpawn].gameObject;

            //dont play sound for reloads
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, 1, true, false);

        }
        else
        {
            //sends the nodeviewid to charcontrollers method
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().MoveCommanded(sendNode, 1, true, true);
        }
        //startingNode.MoveBetweenNodes(GameManager.ins.avatars[GameManager.ins.turnNumber].gameObject);

        GameManager.ins.references.useSpecificNodeForSpawn = 999;

        DisplayLocationName();

        //reset this
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Canvas>().enabled = true;
        GameManager.ins.keepHeroInvisible = false;
    }

    public void DisplayLocationName()
    {
        int difficulty = GetDifficulty();

        string difficultyIcons = "";

        for(int i = 0; i < difficulty; i++)
        {
            difficultyIcons += "<sprite index=42>";
        }

        //removed difficulty icons in v99
        GameManager.ins.references.locationText.text = locationName;// + "   " + difficultyIcons;
    }

    public int GetDifficulty()
    {
        //set this as something
        int difficultyModifier = 2;

        //might need to change this for overmap random encounters later, if there are to be such
        if (GameManager.ins.references.currentMinimap != null)
        {
            //special case for sub-areas
            if (GameManager.ins.references.currentMinimap.isSubArea == true)
            {
                if (GameManager.ins.references.currentMinimap.hasProgressiveDifficulty == true)
                {
                    difficultyModifier = GameManager.ins.gameStage + GameManager.ins.references.currentMinimap.locationDifficultyModifier;
                }

                if (GameManager.ins.references.currentMinimap.hasProgressiveDifficulty == false)
                {
                    difficultyModifier = GameManager.ins.references.currentMinimap.locationDifficultyModifier;
                }
            }

            //for other (most) areas
            else
            {
                //add gamestage to difficulty modifier if its progressive
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.hasProgressiveDifficulty == true)
                {
                    difficultyModifier = GameManager.ins.gameStage + GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.locationDifficultyModifier;
                }
                //otherwise use fixed difficulty
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.hasProgressiveDifficulty == false)
                {
                    difficultyModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.locationDifficultyModifier;
                }
            }
        }

        return difficultyModifier;
    }

    //called by clairvoyance, additionally identifies all encounters
    public void ShowAllEncounters()
    {
        for(int i = 0; i < minimapNodes.Count; i++)
        {
            if (minimapNodes[i].GetComponent<NodeEncounterHandler>() != null)
            {
                minimapNodes[i].GetComponent<NodeEncounterHandler>().ShowEncounters(true);
            }
        }
    }

    public void LoadData(GameData data)
    {
        //lets try additional check here, so theres no additional loads..
        //might not be optimal fix
        if (data.hasVisitedMinimap[minimapNumber] == true && minimapNumber == GameManager.ins.references.currentMinimap.minimapNumber)
        {
            //need a way of handling the triggered overlays in v0.7.0.
            if(triggeredOverlayNumbers.Count > 0)
            {
                for(int i= 0; i < triggeredOverlayNumbers.Count; i++)
                {
                    if(data.triggeredOverlays[triggeredOverlayNumbers[i]] == true)
                    {
                        triggeredOverlays[i].SetActive(true);
                    }
                    else
                    {
                        triggeredOverlays[i].SetActive(false);
                    }
                }
            }

            //seems to be an issue here..
            Debug.Log("should load minimap data, minimap "+ minimapNumber + " is flagged visited");

            int encounterToCheck = 0;

            //first keep track of how many encounters are available for each node
            //the encounters arent random anymore, despite the variable name
            for (int i = 0; i < minimapNodes.Count; i++)
            {
                if (minimapNumber == 1)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap1Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap1EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }

                if (minimapNumber == 2)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap2Encounters[i];

                    if(minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++) 
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap2EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 3)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap3Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap3EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 4)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap4Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap4EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 5)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap5Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap5EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 6)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap6Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap6EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 9)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap9Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap9EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 10)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap10Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap10EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                //guildhouse
                if (minimapNumber == 11)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap11Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap11EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 12)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap12Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap12EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 13)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap13Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[13];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap13EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                //unused?
                if (minimapNumber == 14)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap14Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap14EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 16)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap16Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap16EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 17)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap17Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap17EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 18)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap18Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap18EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 19)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap19Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap19EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 20)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap20Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap20EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 21)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap21Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap21EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 22)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap22Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap22EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 23)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap23Encounters[i];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap23EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                //firstborn fortress lvl2
                if (minimapNumber == 24)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap24Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[24];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap24EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 25)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap25Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[25];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap25EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 26)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap26Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[26];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap26EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 27)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap27Encounters[i];
                    //might not need this for non-sub areas
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[27];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap27EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 28)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap28Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[28];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap28EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 30)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap30Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[30];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap30EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 31)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap31Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[31];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap31EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 34)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap34Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[34];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap34EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 35)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap35Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[35];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap35EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 36)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap36Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[36];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap36EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 37)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap37Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[37];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap37EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 39)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap39Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap39EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 40)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap40Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap40EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 41)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap41Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap41EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 42)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap42Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap42EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 43)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap43Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap43EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 44)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap44Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap44EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 45)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap45Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap45EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 46)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap46Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap46EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 47)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap47Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap47EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 48)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap48Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap48EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 49)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap49Encounters[i];
                    //GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[39];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap49EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                //all are sub areas from here onwards?
                if (minimapNumber == 50)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap50Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[50];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap50EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 51)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap51Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[51];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap51EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 52)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap52Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[52];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap52EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 53)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap53Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[53];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap53EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 54)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap54Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[54];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap54EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 55)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap55Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[55];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap55EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 56)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap56Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[56];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap56EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 57)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap57Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[57];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap57EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 58)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap58Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[58];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap58EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 59)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap59Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[59];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap59EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 60)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap60Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[60];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap60EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 61)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap61Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[61];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap61EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 62)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap62Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[62];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap62EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 63)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap63Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[63];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap63EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 64)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap64Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[64];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap64EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 65)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap65Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[65];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap65EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 66)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap66Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[66];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap66EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 67)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap67Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[67];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap67EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 68)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap68Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[68];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap68EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 69)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap69Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[69];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap69EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 70)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap70Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[70];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap70EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 71)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap71Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[71];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap71EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 72)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap72Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[72];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap72EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 73)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap73Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[73];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap73EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 74)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap74Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[74];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap74EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 75)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap75Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[75];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap75EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 76)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap76Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[76];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap76EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 77)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap77Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[77];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap77EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 78)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap78Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[78];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap78EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 79)
                {
                    minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount = data.minimap79Encounters[i];
                    GameManager.ins.references.currentMinimap.availableEncounters = data.totalEncounters[79];

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber.Add(data.minimap79EncounterNumbers[encounterToCheck]);
                            encounterToCheck += 1;
                        }
                    }
                }
                //add here check for each minimapnumber
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        if (gameObject != null)
        {
            Debug.Log("should save minimap data");

            //need a way of handling the triggered overlays in v0.7.0.
            if (triggeredOverlayNumbers.Count > 0)
            {
                for (int i = 0; i < triggeredOverlayNumbers.Count; i++)
                {
                    if (triggeredOverlays[i].activeSelf == true)
                    {
                        data.triggeredOverlays[triggeredOverlayNumbers[i]] = true;
                    }
                    else
                    {
                        data.triggeredOverlays[triggeredOverlayNumbers[i]] = false;
                    }
                }
            }

            int encounterToCheck = 0;

            for (int i = 0; i < minimapNodes.Count; i++)
            {
                if (minimapNumber == 1)
                {
                    data.minimap1Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap1EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }

                if (minimapNumber == 2)
                {
                    data.minimap2Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap2EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }

                if (minimapNumber == 3)
                {
                    data.minimap3Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap3EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 4)
                {
                    data.minimap4Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap4EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 5)
                {
                    data.minimap5Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap5EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 6)
                {
                    data.minimap6Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap6EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 9)
                {
                    data.minimap9Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap9EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 10)
                {
                    data.minimap10Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap10EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                //guildhouse
                if (minimapNumber == 11)
                {
                    data.minimap11Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap11EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 12)
                {
                    data.minimap12Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap12EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                //moltenrock lvl 2
                if (minimapNumber == 13)
                {
                    data.minimap13Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[13] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap13EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                //unused?
                if (minimapNumber == 14)
                {
                    data.minimap14Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap14EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 16)
                {
                    data.minimap16Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap16EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 17)
                {
                    data.minimap17Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap17EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                //faewood mid
                if (minimapNumber == 18)
                {
                    data.minimap18Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap18EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 19)
                {
                    data.minimap19Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap19EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 20)
                {
                    data.minimap20Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap20EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 21)
                {
                    data.minimap21Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap21EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 22)
                {
                    data.minimap22Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap22EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 23)
                {
                    data.minimap23Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap23EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 24)
                {
                    data.minimap24Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[24] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap24EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 25)
                {
                    data.minimap25Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[25] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap25EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 26)
                {
                    data.minimap26Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[26] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap26EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 27)
                {
                    data.minimap27Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[27] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap27EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 28)
                {
                    data.minimap28Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[28] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap28EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 29)
                {
                    data.minimap29Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[29] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap29EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 30)
                {
                    data.minimap30Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[30] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap30EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 31)
                {
                    data.minimap31Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[31] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap31EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 34)
                {
                    data.minimap34Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[34] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap34EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 35)
                {
                    data.minimap35Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[35] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap35EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 36)
                {
                    data.minimap36Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[36] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap36EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 37)
                {
                    data.minimap37Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[37] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap37EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 39)
                {
                    data.minimap39Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[39] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap39EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 40)
                {
                    data.minimap40Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[40] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap40EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 41)
                {
                    data.minimap41Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[41] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap41EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 42)
                {
                    data.minimap42Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[42] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap42EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 43)
                {
                    data.minimap43Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[43] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap43EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 44)
                {
                    data.minimap44Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[44] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap44EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 45)
                {
                    data.minimap45Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[45] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap45EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 46)
                {
                    data.minimap46Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[46] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap46EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 47)
                {
                    data.minimap47Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[47] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap47EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 48)
                {
                    data.minimap48Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[48] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap48EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 49)
                {
                    data.minimap49Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    //data.totalEncounters[49] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap49EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 50)
                {
                    data.minimap50Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[50] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap50EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 51)
                {
                    data.minimap51Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[51] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap51EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 52)
                {
                    data.minimap52Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[52] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap52EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 53)
                {
                    data.minimap53Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[53] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap53EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 54)
                {
                    data.minimap54Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[54] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap54EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 55)
                {
                    data.minimap55Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[55] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap55EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 56)
                {
                    data.minimap56Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[56] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap56EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 57)
                {
                    data.minimap57Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[57] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap57EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 58)
                {
                    data.minimap58Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[58] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap58EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 59)
                {
                    data.minimap59Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[59] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap59EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 60)
                {
                    data.minimap60Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[60] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap60EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 61)
                {
                    data.minimap61Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[61] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap61EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 62)
                {
                    data.minimap62Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[62] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap62EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 63)
                {
                    data.minimap63Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[63] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap63EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 64)
                {
                    data.minimap64Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[64] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap64EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 65)
                {
                    data.minimap65Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[65] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap65EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 66)
                {
                    data.minimap66Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[66] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap66EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 67)
                {
                    data.minimap67Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[67] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap67EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 68)
                {
                    data.minimap68Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[68] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap68EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 69)
                {
                    data.minimap69Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[69] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap69EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 70)
                {
                    data.minimap70Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[70] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap70EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 71)
                {
                    data.minimap71Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[71] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap71EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 72)
                {
                    data.minimap72Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[72] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap72EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 73)
                {
                    data.minimap73Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[73] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap73EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 74)
                {
                    data.minimap74Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[74] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap74EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 75)
                {
                    data.minimap75Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[75] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap75EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 76)
                {
                    data.minimap76Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[76] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap76EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 77)
                {
                    data.minimap77Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[77] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap77EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 78)
                {
                    data.minimap78Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[78] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap78EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }
                if (minimapNumber == 79)
                {
                    data.minimap79Encounters[i] = minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount;
                    data.totalEncounters[79] = GameManager.ins.references.currentMinimap.availableEncounters;

                    if (minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount > 0)
                    {
                        for (int j = 0; j < minimapNodes[i].GetComponent<NodeEncounterHandler>().randomEncounterCount; j++)
                        {
                            data.minimap79EncounterNumbers[encounterToCheck] = minimapNodes[i].GetComponent<NodeEncounterHandler>().remainingEncountersByNumber[j];
                            encounterToCheck += 1;
                        }
                    }
                }

                //add here check for each minimapnumber
            }

            data.hasVisitedMinimap[minimapNumber] = true;
        }
    }

    public void NightStart()
    {
        if (nightCanvas != null)
        {
            if (GameManager.ins.references.currentMinimap.minimapNumber == 30)
            {
                StartCoroutine(FadeWaterIn(true));
            }

            // fades the image in when started
            else
            {
                StartCoroutine(FadeImageIn(true));

                if (overlayNightCanvas != null)
                {
                    StartCoroutine(FadeOverlayIn(true));
                }
            }
        }
    }

    public void DayStart()
    {
        // fades the image out when started
        if (nightCanvas != null)
        {
            if (GameManager.ins.references.currentMinimap.minimapNumber == 30)
            {
                StartCoroutine(FadeWaterOut(true));
            }

            // fades the image in when started
            else
            {
                StartCoroutine(FadeImageOut(true));

                if(overlayNightCanvas != null)
                {
                    StartCoroutine(FadeOverlayOut(true));
                }
            }
        }
    }

    IEnumerator FadeImageIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            nightCanvas.SetActive(true);

            nightImage.color = new Color(0, 0, 0);

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                nightImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeImageOut(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                nightImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeOverlayIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            overlayDayCanvas.SetActive(false);
            overlayNightCanvas.SetActive(true);

            overlayNightImage.color = new Color(0, 0, 0);

            // loop over 1 second backwards
            for (float i = 0; i <= 0.7f; i += Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                overlayNightImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeOverlayOut(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            overlayDayCanvas.SetActive(true);

            // loop over 1 second backwards
            for (float i = 0.7f; i >= 0; i -= Time.deltaTime * 0.2f)
            {
                // set color with i as alpha
                overlayNightImage.color = new Color(1, 1, 1, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeWaterIn(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            nightCanvas.SetActive(true);

            //for colored water
            Color oldColor = nightCanvas.GetComponent<Renderer>().material.color;

            // loop over 1 second backwards
            for (float i = 0; i <= 1; i += Time.deltaTime * 0.2f)
            {
                //fade night water in
                nightCanvas.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, i);

                yield return null;
            }
        }
    }

    IEnumerator FadeWaterOut(bool fadeAway)
    {
        //fade from opaque to transparent
        if (fadeAway)
        {
            //for colored water
            Color oldColor = nightCanvas.GetComponent<Renderer>().material.color;

            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime * 0.2f)
            {
                //fade night water out
                nightCanvas.GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, i);

                yield return null;
            }
        }
    }

    //for finding number of node where hero is standing
    public int FindCurrentNodeNumber()
    {
        for(int i = 0; i < minimapNodes.Count; i++)
        {
            if(ReferenceEquals(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn, minimapNodes[i]))
            {
                return i;
            }
        }
        return 0;
    }
}
