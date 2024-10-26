using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NodeEncounterHandler : MonoBehaviour
{
    public PhotonView PV;

    //tells the number of random encounters on this node on start
    public int randomEncounterCount;

    //for v95 overmap encounter saving
    public int strategicEncounterCount;

    //list of encounters which are possible to spawn here
    public List<GameObject> possibleEncounters;

    //keeps track of which encounters are still available (only used for secondary visits on the location)
    public List<int> remainingEncountersByNumber;

    //for setting specific encounter to always appear, as first or last
    //note that the fixed encounter shouldnt be on slot 0
    public int fixedFirstEncounterNumber;
    public int fixedLastEncounterNumber;

    //fixed first encounter for start location 2 (wilforge start in v0.7.1.)
    //for customizing start locations, although dont seem optimal way of doing this
    public int FFEForStartLocation2;

    //encounters on these slots cant be rolled (can only be fixed encounters)
    public int[] unrollableEncounters;

    //uses this list to draw encounters to overmap chokepoints etc
    public int[] strategicMapEncounters;

    //could be used for some situations (such as multi-combat)
    //just need to make sure the modifier cant go over 7..
    public int difficultyChange;

    //this overlay will be saved as active to the savefile, if certain encounter is triggered here (doorways mostly for v0.7.0.)
    public int triggerOverlay;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();

        if (GameManager.ins.references.currentMinimap != null)
        {
            int currentMinimapNumber = GameManager.ins.references.currentMinimap.minimapNumber;

            //only roll random encounters, if minimap hasnt been visited
            if (DataPersistenceManager.instance.gameData.hasVisitedMinimap[currentMinimapNumber] == false)
            {
                for (int i = 0; i < randomEncounterCount; i++)
                {
                    //special case for start location 2 (wilforge)
                    //but we shouldnt really be using playerprefs for this..
                    if (FFEForStartLocation2 != 0 && i == 0 && PhotonRoom.room.startLocation == 2)
                    {
                        remainingEncountersByNumber.Add(FFEForStartLocation2);

                        SpawnStrategicEncounter(FFEForStartLocation2);
                    }
                    else if (fixedFirstEncounterNumber != 0 && i == 0)
                    {
                        remainingEncountersByNumber.Add(fixedFirstEncounterNumber);

                        SpawnStrategicEncounter(fixedFirstEncounterNumber);
                    }

                    else if (fixedLastEncounterNumber != 0 && i == randomEncounterCount - 1)
                    {
                        remainingEncountersByNumber.Add(fixedLastEncounterNumber);

                        SpawnStrategicEncounter(fixedLastEncounterNumber);
                    }

                    else
                    {
                        int randomEncounter = RollStrategicEncounter();

                        remainingEncountersByNumber.Add(randomEncounter);

                        SpawnStrategicEncounter(randomEncounter);
                    }
                }
            }

            //grab previous encounters, if minimap has been visited
            if (DataPersistenceManager.instance.gameData.hasVisitedMinimap[currentMinimapNumber] == true)
            {
                for (int i = 0; i < remainingEncountersByNumber.Count; i++)
                {
                    //int randomEncounter = RollStrategicEncounter();
                    //remainingEncountersByNumber.Add(randomEncounter);

                    SpawnStrategicEncounter(remainingEncountersByNumber[i]);
                }
            }
        }

        //spawns overmap encounters for v92
        if(gameObject.GetComponent<Node>().isOvermapNode == true)//strategicMapEncounters.Length > 0)
        {
            if (strategicMapEncounters.Length > 0)
            {
                if (PhotonRoom.room.spContinue == true)
                {
                    for (int i = 0; i < remainingEncountersByNumber.Count; i++)
                    {
                        SpawnOvermapEncounter(remainingEncountersByNumber[i]);
                    }
                }
                else if (strategicMapEncounters[0] != 0)
                {
                    int encounter = RollStrategicEncounter();
                    SpawnOvermapEncounter(encounter);
                    strategicEncounterCount += 1;
                    remainingEncountersByNumber.Add(encounter);
                }
            }
        }
    }

    //spawns additional overmap encounters for v92
    public void SpawnNewOvermapEncounters(int phaseNumber)
    {
        if (gameObject.GetComponent<Node>().isOvermapNode == true)//strategicMapEncounters.Length > 0)
        {
            if (strategicMapEncounters.Length > 0)
            {
                if (strategicMapEncounters[1] != 0)
                {
                    if (phaseNumber == 2)
                    {
                        int encounter = RollStrategicEncounter();
                        SpawnOvermapEncounter(encounter);
                        strategicEncounterCount += 1;
                        remainingEncountersByNumber.Add(encounter);
                    }
                }
                if (strategicMapEncounters[2] != 0)
                {
                    if (phaseNumber == 3)
                    {
                        int encounter = RollStrategicEncounter();
                        SpawnOvermapEncounter(encounter);
                        strategicEncounterCount += 1;
                        remainingEncountersByNumber.Add(encounter);
                    }
                }
            }
        }
    }

    public bool IsRollableEncounter(int encounterNumber)
    {
        if (unrollableEncounters != null)
        {
            for (int i = 0; i < unrollableEncounters.Length; i++)
            {
                if (encounterNumber == unrollableEncounters[i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int RollStrategicEncounter()
    {
        int i = 0;

        do
        {
            int encounterNumber = Random.Range(0, possibleEncounters.Count);

            if (possibleEncounters[encounterNumber].GetComponent<StrategicEncounter>().encounter.isTaken == false && IsRollableEncounter(encounterNumber) == true)
            {
                int rarityCheck = Random.Range(1, 11);
                Encounter2 encounter = possibleEncounters[encounterNumber].GetComponent<StrategicEncounter>().encounter;

                //set this as something
                int difficultyModifier = 2;

                //need new check for overmap spawns (only for chokepoints?)
                //althoguh need to be careful not to add strategic encounters on explore nodes then..
                if (gameObject.GetComponent<Node>().isOvermapNode == true)//strategicMapEncounters.Length > 0)
                {
                    if (strategicMapEncounters.Length > 0)
                    {
                        if (gameObject.GetComponent<Node>().hasProgressiveDifficulty == true)
                        {
                            difficultyModifier = GameManager.ins.gameStage + gameObject.GetComponent<Node>().locationDifficultyModifier + difficultyChange;
                        }
                        if (gameObject.GetComponent<Node>().hasProgressiveDifficulty == false)
                        {
                            difficultyModifier = gameObject.GetComponent<Node>().locationDifficultyModifier + difficultyChange;
                        }
                    }
                }

                //need different check for overmap encoutners
                else if (GameManager.ins.references.currentMinimap != null)
                {
                    //special case for sub-areas
                    if (GameManager.ins.references.currentMinimap.isSubArea == true)
                    {
                        if (GameManager.ins.references.currentMinimap.hasProgressiveDifficulty == true)
                        {
                            difficultyModifier = GameManager.ins.gameStage + GameManager.ins.references.currentMinimap.locationDifficultyModifier + difficultyChange;
                        }

                        if (GameManager.ins.references.currentMinimap.hasProgressiveDifficulty == false)
                        {
                            difficultyModifier = GameManager.ins.references.currentMinimap.locationDifficultyModifier + difficultyChange;
                        }
                    }

                    //for other (most) areas
                    else
                    {
                        //add gamestage to difficulty modifier if its progressive
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.hasProgressiveDifficulty == true)
                        {
                            difficultyModifier = GameManager.ins.gameStage + GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.locationDifficultyModifier + difficultyChange;
                        }
                        //otherwise use fixed difficulty
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.hasProgressiveDifficulty == false)
                        {
                            difficultyModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.locationDifficultyModifier + difficultyChange;
                        }
                    }
                }

                //check time requirements (dont allow invalid encounters through)
                if ((encounter.requirementTime == 1 && Clock.clock.isNight == false) || (encounter.requirementTime == 2 && Clock.clock.isNight == true) || encounter.requirementTime == 0)
                {
                    if (difficultyModifier == 1 && rarityCheck <= encounter.stage1Rarity)
                    {
                        return encounterNumber;
                    }
                    if (difficultyModifier == 2 && rarityCheck <= encounter.stage2Rarity)
                    {
                        return encounterNumber;
                    }
                    if (difficultyModifier == 3 && rarityCheck <= encounter.stage3Rarity)
                    {
                        return encounterNumber;
                    }
                    if (difficultyModifier == 4 && rarityCheck <= encounter.stage4Rarity)
                    {
                        return encounterNumber;
                    }
                    if (difficultyModifier == 5 && rarityCheck <= encounter.stage5Rarity)
                    {
                        return encounterNumber;
                    }
                    if (difficultyModifier == 6 && rarityCheck <= encounter.stage6Rarity)
                    {
                        return encounterNumber;
                    }
                    if (difficultyModifier == 7 && rarityCheck <= encounter.stage7Rarity)
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

    /* dont need rpc call in v99?
    public void SpawnStrategicEncounter(int encounterNumber)
    {
        PV.RPC("RPC_SpawnStrategicEncounter", RpcTarget.AllBufferedViaServer, encounterNumber);
    }
    */

    //[PunRPC]
    public void SpawnStrategicEncounter(int encounterNumber)
    {
        InternalNode iNode = gameObject.GetComponent<Node>().TellFreeSlot();

        //should make non-respawning encounter "taken" before instantiate? (since instantiate takes time?)
        //actually this could be the only place where this needs to be checked? (technically non-respawn and unique overlap each other tho..)
        //maybe best use uniqueEncounter flag for this purpose (which basically makes respawns-flag redundant?)
        if(possibleEncounters[encounterNumber].GetComponent<StrategicEncounter>().encounter.isUniqueEncounter == true)
        {
            possibleEncounters[encounterNumber].GetComponent<StrategicEncounter>().encounter.isTaken = true;
        }

        //instantiate strawberries, and set gamemanager as parent for now
        GameObject newEncounter = Instantiate(possibleEncounters[encounterNumber], transform.position, transform.rotation, iNode.gameObject.transform);
        //GameObject newNpc = Instantiate(strawberries, transform.position, transform.rotation, GameManager.ins.transform);

        //newEncounter.SetActive(true);

        //reduce size 90%
        //newNpc.transform.localScale += new Vector3(-0.9f, -0.9f, -0.9f);

        //places the agent on the next node
        newEncounter.GetComponent<StrategicEncounter>().standingOn = gameObject.GetComponent<Node>();

        //then moves npc to it
        newEncounter.GetComponent<StrategicEncounter>().standingOn.NpcArrive(newEncounter);

        newEncounter.GetComponent<StrategicEncounter>().identifiedImage.SetActive(false);
        newEncounter.GetComponent<StrategicEncounter>().unIdentifiedImage.SetActive(false);

        //add the original number as reference
        newEncounter.GetComponent<StrategicEncounter>().originalListNumber = encounterNumber;

        //should set these here?
        if (newEncounter.GetComponent<StrategicEncounter>().hasStealth == true)
        {
            newEncounter.GetComponent<StrategicEncounter>().isIdentified = false;
        }
        else
        {
            newEncounter.GetComponent<StrategicEncounter>().isIdentified = true;
        }
    }

    //for v92
    public void SpawnOvermapEncounter(int encounterNumber)
    {
        PV.RPC("RPC_SpawnOvermapEncounter", RpcTarget.AllBufferedViaServer, encounterNumber);
    }


    [PunRPC]
    void RPC_SpawnOvermapEncounter(int encounterNumber)
    {
        InternalNode iNode = gameObject.GetComponent<Node>().TellFreeSlot();

        //instantiate strawberries, and set gamemanager as parent for now
        GameObject newEncounter = Instantiate(possibleEncounters[encounterNumber], transform.position, transform.rotation, iNode.gameObject.transform);
        //GameObject newNpc = Instantiate(strawberries, transform.position, transform.rotation, GameManager.ins.transform);

        //newEncounter.SetActive(true);

        //reduce size 90%
        //newNpc.transform.localScale += new Vector3(-0.9f, -0.9f, -0.9f);

        //places the agent on the next node
        newEncounter.GetComponent<StrategicEncounter>().standingOn = gameObject.GetComponent<Node>();

        //then moves npc to it
        newEncounter.GetComponent<StrategicEncounter>().standingOn.NpcArrive(newEncounter);

        newEncounter.GetComponent<StrategicEncounter>().identifiedImage.SetActive(false);
        newEncounter.GetComponent<StrategicEncounter>().unIdentifiedImage.SetActive(false);

        //need this for overmap encounters for some reason
        newEncounter.GetComponent<Canvas>().overrideSorting = true;

        //add the original number as reference
        newEncounter.GetComponent<StrategicEncounter>().originalListNumber = encounterNumber;

        //should set these here?
        if (newEncounter.GetComponent<StrategicEncounter>().hasStealth == true)
        {
            newEncounter.GetComponent<StrategicEncounter>().isIdentified = false;
        }
        else
        {
            newEncounter.GetComponent<StrategicEncounter>().isIdentified = true;
        }
    }

    //shows encounters on this node (either identified or unidentified)
    //these should stay visible after first discovery
    //clairvoyance should identify always on v94
    public void ShowEncounters(bool identifyAlways)
    {
        for(int i = 0; i < gameObject.GetComponent<Node>().internalNodes.Count; i++)
        {
            if (gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>() != null)
            {
                /* lets remove scouting from game for now
                 * special case for scouting
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(32) == true)
                {
                    gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified = true;
                }
                */

                //set this when encounter is spawned instead
                //gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified = true;

                //special case for foe stealth
                if (gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().hasStealth == true)
                {
                    //special case for seers orb, x-ray goggles, clairvoyance, and if encounter has alrdy been identified
                    if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 6, 84) == true || CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 8, 227) == true || 
                        identifyAlways == true || gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified == true)
                    {
                        gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified = true;
                    }
                    else
                    {
                        gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified = false;
                    }
                }

                if (gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified == true)
                {
                    gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().identifiedImage.SetActive(true);
                    gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().unIdentifiedImage.SetActive(false);
                }
                if (gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified == false)
                {
                    gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().identifiedImage.SetActive(false);
                    gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().unIdentifiedImage.SetActive(true);
                }
            }
        }
    }

    public void IdentifyEncounters()
    {
        for (int i = 0; i < gameObject.GetComponent<Node>().internalNodes.Count; i++)
        {
            if (gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>() != null)
            {
                gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().isIdentified = true;

                gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().identifiedImage.SetActive(true);
                gameObject.GetComponent<Node>().internalNodes[i].transform.GetComponentInChildren<StrategicEncounter>().unIdentifiedImage.SetActive(false);
            }
        }
    }
}
