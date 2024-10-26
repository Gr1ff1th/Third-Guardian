using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

//collection of references (or some additional ones)
public class References : MonoBehaviour
{
    public TextMeshProUGUI movementText;
    public TextMeshProUGUI actionText;

    //extra button references (not needed as of v88)
    public GameObject exploreButton;

    //new interaction orbs (as of v92)
    public GameObject OvermapExploreButton;
    public GameObject RestButton;
    public GameObject SleepButton;
    public GameObject ContemplateButton;
    public GameObject SelfMaintenanceButton;
    public GameObject QuickMaintenanceButton;
    public GameObject WaitButton;

    public GameObject RestButton2;
    public GameObject SleepButton2;
    public GameObject WaitButton2;

    //for stat texts
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI arcanePowerText;
    public TextMeshProUGUI resistanceText;

    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI mechanicsText;
    public TextMeshProUGUI diggingText;
    public TextMeshProUGUI loreText;
    public TextMeshProUGUI observeText;

    //for resource texts
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI dustText;
    public TextMeshProUGUI skillPointText;
    public TextMeshProUGUI favorText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI fameText;
    public TextMeshProUGUI bombText;

    //combat modifiers
    public TextMeshProUGUI strengthModifierText;
    public TextMeshProUGUI arcanePowerModifierText;
    public TextMeshProUGUI bombModifierText;
    public TextMeshProUGUI defenseModifierText;
    public TextMeshProUGUI resistanceModifierText;

    //more modifiers for v93
    public TextMeshProUGUI bombAttackText;
    public TextMeshProUGUI holyAttackText;
    public TextMeshProUGUI holyModifierText;
    public TextMeshProUGUI hpRegenText;
    public TextMeshProUGUI energyRegenText;
    public TextMeshProUGUI movementBonusText;

    //final score modifier
    public TextMeshProUGUI scoreModifierText;

    //character level text
    public TextMeshProUGUI levelText;

    //other text references
    public TextMeshProUGUI locationText;

    //could take references for these
    public SFXPlayer sfxPlayer;
    public SoundManager soundManager;

    //nodes should have references too
    public List<Node> nodes;

    //needed for sub-areas
    public MinimapController currentMinimap;

    //needed for sub-areas
    public StrategicEncounter currentStrategicEncounter;

    public Encounter2 currentEncounter;

    //keeps track of which starting node to start on the next minimap
    public int startingNodeNumber;

    //take reference for this
    public TargettingHandler targettingHandler;

    //for interruptions
    public GameObject interruptionDisplay;
    public TextMeshProUGUI interruptionText;

    //for portal system
    //reworked for v99
    //waypoint 0=inn, 1=faewood, 2=blue citadel, 3=temple, 4=underworld
    public int[] foundWaypoints = new int[5] { 0, 0, 0, 0, 0};

    public EnemyResizing enemyResizing;

    //used for returning discs
    //in order: claudia, dweller, irino, lavinia, mack, nabamax, zaarin
    public bool[] discsReturned = { false, false, false, false, false, false, false };

    //for various story images
    public List<Sprite> epilogueSprites;
    public List<Sprite> eventSprites;

    //need to disable these icons in the corner occasionally
    public GameObject characterIconDisplay;

    //variable for multiplying (reducing) all combat modifiers
    public float combatModifierMultiplyer;

    //in v94, use these to display negative skillpoints on stats sheet
    public List<GameObject> skillOrbs;
    public List<Sprite> orbStateIcons;

    //for temporarily displaying systemn messages
    public TextMeshProUGUI systemText;

    //used as variable for game load spawn node (for minimaps)
    public int useSpecificNodeForSpawn;

    //relocate these for v95
    public List<GameObject> statDisplayCharacterIcons;
    public List<GameObject> equipmentDisplayCharacterIcons;

    //need backup for this
    public Sprite heroBackupImage;

    //for save purposes
    public int chosenCharacter;

    //need progressive cost for nabamax lessons
    public int nabamaxLessonCost;

    // Start is called before the first frame update
    void Start()
    {
        //foundWaypoints = new int[6] { 0, 0, 0, 0, 0, 0 };

        //dunno why i need to do it like this.. 
        foundWaypoints = DataPersistenceManager.instance.gameData.foundWaypoints;
        useSpecificNodeForSpawn = DataPersistenceManager.instance.gameData.nodeToGoTo;
        

    }

    public void ClearLocationText()
    {
        locationText.text = "";
    }

    public void ShowLevelInsufficientInterruption()
    {
        interruptionDisplay.SetActive(true);
        interruptionText.text = "Insufficient Character Level";
        Invoke("RemoveInterruptionDisplay", 3f);
    }

    public void ShowCoinsInsufficientInterruption()
    {
        interruptionDisplay.SetActive(true);
        interruptionText.text = "Insufficient Coins";
        Invoke("RemoveInterruptionDisplay", 3f);
    }

    public void ShowLightstoneInterruption()
    {
        interruptionDisplay.SetActive(true);
        interruptionText.text = "Lightstone required to enter";
        Invoke("RemoveInterruptionDisplay", 3f);
    }

    public void RemoveInterruptionDisplay()
    {
        interruptionDisplay.SetActive(false);
    }


    //for portal system in v93
    public void MakeWaypointChecks(int nodeNumber)
    {
        if (nodeNumber == 1)
        {
            if (GameManager.ins.references.foundWaypoints[0] == nodeNumber)
            {
                return;
            }
            else
            {
                GameManager.ins.references.foundWaypoints[0] = 1;
                //give message
                string msgs = "You opened Inn waypoint!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);

            }
        }
        if (nodeNumber == 2)
        {
            if (GameManager.ins.references.foundWaypoints[1] == nodeNumber)
            {
                return;
            }
            else
            {
                GameManager.ins.references.foundWaypoints[1] = 2;
                //give message
                string msgs = "You opened Faewood waypoint!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);

            }
        }
        if (nodeNumber == 3)
        {
            if (GameManager.ins.references.foundWaypoints[2] == nodeNumber)
            {
                return;
            }
            else
            {
                GameManager.ins.references.foundWaypoints[2] = 3;
                //give message
                string msgs = "You opened Blue Citadel waypoint!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);

            }
        }
        /*
        if (nodeNumber == 47)
        {
            if (GameManager.ins.references.foundWaypoints[3] == nodeNumber)
            {
                return;
            }
            else
            {
                GameManager.ins.references.foundWaypoints[3] = 47;
                //give message
                string msgs = "You opened new waypoint!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }
        }
        */

        //temple waypoint
        if (nodeNumber == 4)
        {
            if (GameManager.ins.references.foundWaypoints[3] == nodeNumber)
            {
                return;
            }
            else
            {
                GameManager.ins.references.foundWaypoints[3] = 4;
                //give message
                string msgs = "You opened Temple waypoint!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }
        }

        //special case for underworld waypoint
        if (nodeNumber == 5)
        {
            if (GameManager.ins.references.foundWaypoints[4] == nodeNumber)
            {
                return;
            }
            else
            {
                GameManager.ins.references.foundWaypoints[4] = 5;
                //give message
                string msgs = "You opened Underworld waypoint!";
                //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);
            }
        }
    }


    public void SetSkillOrbs()
    {
        if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength >= 0)
        {
            skillOrbs[0].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength == -1)
        {
            skillOrbs[0].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength == -2)
        {
            skillOrbs[0].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength <= -3)
        {
            skillOrbs[0].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense >= 0)
        {
            skillOrbs[1].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense == -1)
        {
            skillOrbs[1].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense == -2)
        {
            skillOrbs[1].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense <= -3)
        {
            skillOrbs[1].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower >= 0)
        {
            skillOrbs[2].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower == -1)
        {
            skillOrbs[2].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower == -2)
        {
            skillOrbs[2].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower <= -3)
        {
            skillOrbs[2].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance >= 0)
        {
            skillOrbs[3].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance == -1)
        {
            skillOrbs[3].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance == -2)
        {
            skillOrbs[3].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance <= -3)
        {
            skillOrbs[3].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence >= 0)
        {
            skillOrbs[4].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence == -1)
        {
            skillOrbs[4].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence == -2)
        {
            skillOrbs[4].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence <= -3)
        {
            skillOrbs[4].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics >= 0)
        {
            skillOrbs[5].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics == -1)
        {
            skillOrbs[5].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics == -2)
        {
            skillOrbs[5].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics <= -3)
        {
            skillOrbs[5].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging >= 0)
        {
            skillOrbs[6].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging == -1)
        {
            skillOrbs[6].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging == -2)
        {
            skillOrbs[6].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging <= -3)
        {
            skillOrbs[6].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore >= 0)
        {
            skillOrbs[7].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore == -1)
        {
            skillOrbs[7].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore == -2)
        {
            skillOrbs[7].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore <= -3)
        {
            skillOrbs[7].GetComponent<Image>().sprite = orbStateIcons[3];
        }

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe >= 0)
        {
            skillOrbs[8].GetComponent<Image>().sprite = orbStateIcons[0];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == -1)
        {
            skillOrbs[8].GetComponent<Image>().sprite = orbStateIcons[1];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == -2)
        {
            skillOrbs[8].GetComponent<Image>().sprite = orbStateIcons[2];
        }
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe <= -3)
        {
            skillOrbs[8].GetComponent<Image>().sprite = orbStateIcons[3];
        }
    }
}
