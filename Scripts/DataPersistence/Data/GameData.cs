using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores all the gamedata
[System.Serializable]
public class GameData
{
    //data for main map
    public int startingNodeNumber;

    //data when hero was on minimap when game was saved
    public int sceneToGoTo;
    public int nodeToGoTo;

    //records current gamestage
    public int gameStage;
    public int startingDifficulty;
    public int scoreModifier;

    //need to record maptype & game start location (1=inn, 2=temple)
    public int mapType;
    public int startingLocation;

    //for v1.0.0.
    public int nabamaxLessonCost;

    //for v1.0.0. (is compared to explorehandlers exhaustableencounters)
    public bool[] uniqueTaken = new bool[100];

    //clock variables
    public int turnNumber;
    public int totalTurnsPlayed;
    public bool day2Started;
    public bool day3Started;
    public bool citadelEntered;
    public bool isNight;

    //special variable
    public bool zaarin3MessageGiven;

    //need variable to tell whether game can be continued or not
    public bool continueAvailable;

    //character resources
    //public int coins;

    //for v99 (randomizes certain transits at new gamemode)
    //index 0=hidden grove/hexwood, 
    public int[] randomTransits = new int[50];

    //for overmap encounter saving (v95)
    public int[] overmapEncounters = new int[100];
    public int[] overmapEncounterNumbers = new int[100];


    //first array stores and tells the minimap number
    //second array tell number of minimap nodes, and stores and tells the number of remaining encounters for that node
    //remember that each array starts at 0, so second node is actually 1 on the array etc
    //public int[,] minimapNodeEncounters = new int[2, 15];
    //public int[,] minimapNodeEncounters = new int[,] { { 0, 0 }, { 0, 2 } };

    //cornville
    public int[] minimap1Encounters = new int[15];
    //c farm east
    public int[] minimap2Encounters = new int[15];
    //c farm west
    public int[] minimap3Encounters = new int[15];
    //c farm south
    public int[] minimap4Encounters = new int[15];
    //badgerwood north
    public int[] minimap5Encounters = new int[15];
    //forest fort
    public int[] minimap6Encounters = new int[15];
    //badgerwood SE
    public int[] minimap9Encounters = new int[15];
    //badgerwood SW
    public int[] minimap10Encounters = new int[15];
    //guildhouse
    public int[] minimap11Encounters = new int[15];
    //moltenrock lvl1
    public int[] minimap12Encounters = new int[15];
    //moltenrock lvl2 (sub-area)
    public int[] minimap13Encounters = new int[15];
    //unused
    public int[] minimap14Encounters = new int[15];
    //coven
    public int[] minimap16Encounters = new int[15];
    //ancient graveyard
    public int[] minimap17Encounters = new int[15];
    //faewood mid
    public int[] minimap18Encounters = new int[15];
    //faewood west
    public int[] minimap19Encounters = new int[15];
    //faewood clearing
    public int[] minimap20Encounters = new int[15];
    //grimhold
    public int[] minimap21Encounters = new int[15];
    //faewood north
    public int[] minimap22Encounters = new int[15];
    //firstborn fortress lvl 1
    public int[] minimap23Encounters = new int[15];
    //firstborn fortress lvl 2
    public int[] minimap24Encounters = new int[15];
    //firstborn fortress lvl 0
    public int[] minimap25Encounters = new int[15];
    //crypts lvl 1
    public int[] minimap26Encounters = new int[15];
    //hidden grove
    public int[] minimap27Encounters = new int[15];
    //gorms smithy
    public int[] minimap28Encounters = new int[15];
    //unused
    public int[] minimap29Encounters = new int[15];
    //north harbor
    public int[] minimap30Encounters = new int[20];
    //second shot inn
    public int[] minimap31Encounters = new int[15];
    //unused
    public int[] minimap32Encounters = new int[15];
    //unused
    public int[] minimap33Encounters = new int[15];
    //oldmines
    public int[] minimap34Encounters = new int[15];
    //mt vaaran west
    public int[] minimap35Encounters = new int[15];
    //wilforge outskirts
    public int[] minimap36Encounters = new int[15];
    //wilforge
    public int[] minimap37Encounters = new int[15];
    //unused
    public int[] minimap38Encounters = new int[15];
    //mt vaaran MNW
    public int[] minimap39Encounters = new int[15];
    //
    public int[] minimap40Encounters = new int[15];
    //
    public int[] minimap41Encounters = new int[15];
    //
    public int[] minimap42Encounters = new int[15];
    //
    public int[] minimap43Encounters = new int[15];
    //
    public int[] minimap44Encounters = new int[15];
    //
    public int[] minimap45Encounters = new int[15];
    //
    public int[] minimap46Encounters = new int[15];
    //
    public int[] minimap47Encounters = new int[15];
    //
    public int[] minimap48Encounters = new int[15];
    //
    public int[] minimap49Encounters = new int[15];
    //
    public int[] minimap50Encounters = new int[15];
    //
    public int[] minimap51Encounters = new int[15];
    //
    public int[] minimap52Encounters = new int[15];
    //
    public int[] minimap53Encounters = new int[15];
    //underworld middle
    public int[] minimap54Encounters = new int[20];
    //
    public int[] minimap55Encounters = new int[15];
    //
    public int[] minimap56Encounters = new int[15];
    //
    public int[] minimap57Encounters = new int[15];
    //
    public int[] minimap58Encounters = new int[15];
    //
    public int[] minimap59Encounters = new int[15];
    //
    public int[] minimap60Encounters = new int[15];
    //
    public int[] minimap61Encounters = new int[15];
    //
    public int[] minimap62Encounters = new int[15];
    //
    public int[] minimap63Encounters = new int[15];
    //
    public int[] minimap64Encounters = new int[15];
    //
    public int[] minimap65Encounters = new int[15];
    //
    public int[] minimap66Encounters = new int[15];
    //
    public int[] minimap67Encounters = new int[15];
    //
    public int[] minimap68Encounters = new int[15];
    //
    public int[] minimap69Encounters = new int[15];
    //
    public int[] minimap70Encounters = new int[15];
    //
    public int[] minimap71Encounters = new int[15];
    //
    public int[] minimap72Encounters = new int[15];
    //
    public int[] minimap73Encounters = new int[15];
    //
    public int[] minimap74Encounters = new int[15];
    //
    public int[] minimap75Encounters = new int[15];
    //
    public int[] minimap76Encounters = new int[15];
    //
    public int[] minimap77Encounters = new int[15];
    //
    public int[] minimap78Encounters = new int[15];
    //
    public int[] minimap79Encounters = new int[15];


    //these record the number of each encounter, based on the possible encounters list in NodeEncounterHandler (999 is ignored)
    //cornville
    public int[] minimap1EncounterNumbers = new int[20];
    //c farm east
    public int[] minimap2EncounterNumbers = new int[20];
    //c farm west
    public int[] minimap3EncounterNumbers = new int[20];
    //c farm south
    public int[] minimap4EncounterNumbers = new int[20];
    //badgerwood north
    public int[] minimap5EncounterNumbers = new int[20];
    //forest fort
    public int[] minimap6EncounterNumbers = new int[20];
    //badgerwood SE
    public int[] minimap9EncounterNumbers = new int[20];
    //badgerwood SW
    public int[] minimap10EncounterNumbers = new int[20];
    //guildhouse
    public int[] minimap11EncounterNumbers = new int[20];
    //moltenrock lvl1
    public int[] minimap12EncounterNumbers = new int[20];
    //moltenrock lvl2 (sub-area)
    public int[] minimap13EncounterNumbers = new int[20];
    //unused
    public int[] minimap14EncounterNumbers = new int[20];
    //coven
    public int[] minimap16EncounterNumbers = new int[20];
    //ancient graveyard
    public int[] minimap17EncounterNumbers = new int[20];
    //faewood mid
    public int[] minimap18EncounterNumbers = new int[20];
    //faewood west
    public int[] minimap19EncounterNumbers = new int[20];
    //faewood clearing
    public int[] minimap20EncounterNumbers = new int[20];
    //grimhold
    public int[] minimap21EncounterNumbers = new int[20];
    //faewood north
    public int[] minimap22EncounterNumbers = new int[20];
    //firstborn fortress lvl 1
    public int[] minimap23EncounterNumbers = new int[20];
    //firstborn fortress lvl 2
    public int[] minimap24EncounterNumbers = new int[20];
    //firstborn fortress lvl 0
    public int[] minimap25EncounterNumbers = new int[20];
    //crypts lvl 1
    public int[] minimap26EncounterNumbers = new int[20];
    //
    public int[] minimap27EncounterNumbers = new int[20];
    //
    public int[] minimap28EncounterNumbers = new int[20];
    //
    public int[] minimap29EncounterNumbers = new int[20];
    //
    public int[] minimap30EncounterNumbers = new int[20];
    //
    public int[] minimap31EncounterNumbers = new int[20];
    //
    public int[] minimap32EncounterNumbers = new int[20];
    //
    public int[] minimap33EncounterNumbers = new int[20];
    //
    public int[] minimap34EncounterNumbers = new int[20];
    //
    public int[] minimap35EncounterNumbers = new int[20];
    //
    public int[] minimap36EncounterNumbers = new int[20];
    //
    public int[] minimap37EncounterNumbers = new int[20];
    //
    public int[] minimap38EncounterNumbers = new int[20];
    //
    public int[] minimap39EncounterNumbers = new int[20];
    //
    public int[] minimap40EncounterNumbers = new int[20];
    //
    public int[] minimap41EncounterNumbers = new int[20];
    //
    public int[] minimap42EncounterNumbers = new int[20];
    //
    public int[] minimap43EncounterNumbers = new int[20];
    //
    public int[] minimap44EncounterNumbers = new int[20];
    //
    public int[] minimap45EncounterNumbers = new int[20];
    //
    public int[] minimap46EncounterNumbers = new int[20];
    //
    public int[] minimap47EncounterNumbers = new int[20];
    //
    public int[] minimap48EncounterNumbers = new int[20];
    //
    public int[] minimap49EncounterNumbers = new int[20];
    //
    public int[] minimap50EncounterNumbers = new int[20];
    //
    public int[] minimap51EncounterNumbers = new int[20];
    //
    public int[] minimap52EncounterNumbers = new int[20];
    //
    public int[] minimap53EncounterNumbers = new int[20];
    //
    public int[] minimap54EncounterNumbers = new int[20];
    //
    public int[] minimap55EncounterNumbers = new int[20];
    //
    public int[] minimap56EncounterNumbers = new int[20];
    //
    public int[] minimap57EncounterNumbers = new int[20];
    //
    public int[] minimap58EncounterNumbers = new int[20];
    //
    public int[] minimap59EncounterNumbers = new int[20];
    //
    public int[] minimap60EncounterNumbers = new int[20];
    //
    public int[] minimap61EncounterNumbers = new int[20];
    //
    public int[] minimap62EncounterNumbers = new int[20];
    //
    public int[] minimap63EncounterNumbers = new int[20];
    //
    public int[] minimap64EncounterNumbers = new int[20];
    //
    public int[] minimap65EncounterNumbers = new int[20];
    //
    public int[] minimap66EncounterNumbers = new int[20];
    //
    public int[] minimap67EncounterNumbers = new int[20];
    //
    public int[] minimap68EncounterNumbers = new int[20];
    //
    public int[] minimap69EncounterNumbers = new int[20];
    //
    public int[] minimap70EncounterNumbers = new int[20];
    //
    public int[] minimap71EncounterNumbers = new int[20];
    //
    public int[] minimap72EncounterNumbers = new int[20];
    //
    public int[] minimap73EncounterNumbers = new int[20];
    //
    public int[] minimap74EncounterNumbers = new int[20];
    //
    public int[] minimap75EncounterNumbers = new int[20];
    //
    public int[] minimap76EncounterNumbers = new int[20];
    //
    public int[] minimap77EncounterNumbers = new int[20];
    //
    public int[] minimap78EncounterNumbers = new int[20];
    //
    public int[] minimap79EncounterNumbers = new int[20];


    //keeps track whether the location is visited or not
    public bool[] hasVisitedMinimap = new bool[80];

    //keeps track of unresolved encounters for each location
    //node number 13 reserved for moltenrock lvl 2
    //only needed for sub areas?
    public int[] totalEncounters = new int[80];

    //for storing portal destinations
    public int[] foundWaypoints = new int[5] { 0, 0, 0, 0, 0};

    //need to save this too
    public float combatModifierMultiplyer;

    //could save all triggered overlays in single array, i think?
    public bool[] triggeredOverlays = new bool[100];

    /*
     * CHARACTER SAVE
     */

    //this variable has some special applications
    public int chosenCharacter;

    //hero stats
    public int strength;
    public int defense;
    public int arcanePower;
    public int resistance;

    public int influence;
    public int mechanics;
    public int digging;
    public int lore;
    public int observe;

    // max stats
    // needed for curse effects
    public int maxStrength;
    public int maxDefense;
    public int maxArcanePower;
    public int maxResistance;

    public int maxInfluence;
    public int maxMechanics;
    public int maxDigging;
    public int maxLore;
    public int maxObserve;

    //skillpoint upgrades
    public int strengthUpgrades;
    public int defenseUpgrades;
    public int arcanePowerUpgrades;
    public int resistanceUpgrades;

    public int influenceUpgrades;
    public int mechanicsUpgrades;
    public int diggingUpgrades;
    public int loreUpgrades;
    public int observeUpgrades;

    //stat bonuses from equipment
    public int[] equipmentStats = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    //combat modifiers
    public int strengthModifier;
    public int arcanePowerModifier;
    public int bombModifier;
    public int defenseModifier;
    public int resistanceModifier;

    public int bombAttack;
    public int holyAttack;
    public int holyModifier;
    public int healthRegen;
    public int energyRegen;

    //movementBonus stat
    public int movementBonus;

    //character level variables
    public int characterLevel;
    public int fameForNextLevel;

    //this increases each time you gain level
    public int fameForNextLevelTreshold;

    //Resources
    //0=energy, 2=skillpoints, 3=health, 4=coins, 5=fame, 6=favor, 16=bombs
    public int energy;
    public int maxEnergy;
    public int coins;
    public int fame;
    public int favor;
    public int maxFavor;
    public int skillPoints;
    public int health;
    public int maxHealth;
    public int maxActionPoints;

    //this is set to charcontroller class
    public int actionPoints;

    /*
     * Card saving
     * 
    */
    public int[] usableCards = new int[10];
    public int[] skillCards = new int[10];
    public int[] equipmentCards = new int[10];
    public int[] effectCards = new int[10];

    public int[] usableQty = new int[10];
    public int[] usableLvl = new int[10];
    public int[] skillLvl = new int[10];
    public int[] equipmentQty = new int[10];
    public int[] effectQty = new int[10];

    public int[] objectiveCards = new int[10];

    //one number for each equipment slot
    //helm 0, armor 1, ring 2, weapon 3, mount 4, misc 5, goggles 6, mask 7, amulet 8, tome 9, toolbox 10, shovel 11
    public int[] equippedItems = new int[12];

    //store cards need to be saved too
    public int[] wilforgeShop;
    public int[] smithyShop;
    public int[] innShop;
    public int[] factoryShop;
    public int[] templeShop;
    public int[] covenShop;
    public int[] guildhouseShop;
    public int[] cornvilleShop;

    public int[] wilforgeShopQty;
    public int[] smithyShopQty;
    public int[] innShopQty;
    public int[] factoryShopQty;
    public int[] templeShopQty;
    public int[] covenShopQty;
    public int[] guildhouseShopQty;
    public int[] cornvilleShopQty;

    //quest info saves
    //stored by number in deck
    public int[] haveTakenQuest;
    public int[] haveCompletedQuestObjective;
    public int[] haveCompletedQuest;


    /* persistent variables
     * 
     * DO NOT RESET !
     * 
     */

    public int[] highScores = new int[30];

    //v0 = pre v1.0.0., v1 = v1.0.0.
    public int[] highScoreForVersion = new int[30];


    //the initial values when starting new game
    //or actually this is only called at the first creation of the savefile in v92?
    public GameData()
    {
        //this.numberOfPlayers = 1;

        //there should be no avatars loaded at new game, those are selected from previous scene
        //this.avatars = null;

        /*
        if (this.avatars.Count > 0)
        {
            //Invoke("WaitForAvatarsToSpawn", 2.0f);
        }
        */

        //startingNode = GameManager.ins.startingNode;

        //default start spot is at 1 (cornville)
        //modified for v92 in gameManager

        /* do we need these at all anymore actually?
         * 
        startingNodeNumber = 1;

        gameStage = 2;
        startingDifficulty = 1;
        scoreModifier = 100;

        coins = 0;
        */
        //minimapNodeEncounters = new int[,] {{ 0, 0 }, { 0, 2 }} ;
        //hasVisitedMinimap = { false, false}

        //totalEncounters = new int[10] {12, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    }

    public void WaitForAvatarsToSpawn()
    {

    }

    //for resetting non-persistent data
    //metadata shouldnt be resetted (highscores etc)
    public void ResetInGameData()
    {
        //Debug.Log("should reset data");

        //data for main map
        startingNodeNumber = 1;

        //needed for telling if continue game is available or not
        continueAvailable = false;

        //used by custom start nodes
        sceneToGoTo = 999;
        nodeToGoTo = 999;

        //records current gamestage
        gameStage = 2;
        startingDifficulty = 1;
        scoreModifier = 100;

        //character resources
        coins = 0;

        //reset these at new game
        randomTransits = new int[50];

        //for overmap encounter saving
        overmapEncounters = new int[100];
        overmapEncounterNumbers = new int[100];

        //added in v0.7.1.
        chosenCharacter = 0;

        //for v1.0.0.
        nabamaxLessonCost = 50;

        //for v1.0.0.
        //list of taken unique encounters
        uniqueTaken = new bool[100];

        //cornville
        minimap1Encounters = new int[15];
        //c farm east
        minimap2Encounters = new int[15];
        //c farm west
        minimap3Encounters = new int[15];
        //c farm south
        minimap4Encounters = new int[15];
        //badgerwood north
        minimap5Encounters = new int[15];
        //forest fort
        minimap6Encounters = new int[15];
        //badgerwood SE
        minimap9Encounters = new int[15];
        //badgerwood SW
        minimap10Encounters = new int[15];
        //guildhouse
        minimap11Encounters = new int[15];
        //moltenrock lvl1
        minimap12Encounters = new int[15];
        //moltenrock lvl2 (sub-area)
        minimap13Encounters = new int[15];
        //unused
        minimap14Encounters = new int[15];
        //coven
        minimap16Encounters = new int[15];
        //ancient graveyard
        minimap17Encounters = new int[15];
        //faewood mid
        minimap18Encounters = new int[15];
        //faewood west
        minimap19Encounters = new int[15];
        //faewood clearing
        minimap20Encounters = new int[15];
        //grimhold
        minimap21Encounters = new int[15];
        //faewood north
        minimap22Encounters = new int[15];
        //firstborn fortress lvl 1
        minimap23Encounters = new int[15];
        //firstborn fortress lvl 2
        minimap24Encounters = new int[15];
        //firstborn fortress lvl 0
        minimap25Encounters = new int[15];
        //crypts lvl 1
        minimap26Encounters = new int[15];
        //hidden grove
        minimap27Encounters = new int[15];
        //gorms smithy
        minimap28Encounters = new int[15];
        //unused
        minimap29Encounters = new int[15];
        //north harbor
        minimap30Encounters = new int[20];
        //second shot inn
        minimap31Encounters = new int[15];
        //unused
        minimap32Encounters = new int[15];
        //unused
        minimap33Encounters = new int[15];
        //oldmines
        minimap34Encounters = new int[15];
        //mt vaaran west
        minimap35Encounters = new int[15];
        //wilforge outskirts
        minimap36Encounters = new int[15];
        //wilforge
        minimap37Encounters = new int[15];
        //unused
        minimap38Encounters = new int[15];
        //mt vaaran MNW
        minimap39Encounters = new int[15];
        //
        minimap40Encounters = new int[15];
        //
        minimap41Encounters = new int[15];
        //
        minimap42Encounters = new int[15];
        //
        minimap43Encounters = new int[15];
        //
        minimap44Encounters = new int[15];
        //
        minimap45Encounters = new int[15];
        //
        minimap46Encounters = new int[15];
        //
        minimap47Encounters = new int[15];
        //
        minimap48Encounters = new int[15];
        //
        minimap49Encounters = new int[15];
        //
        minimap50Encounters = new int[15];
        //
        minimap51Encounters = new int[15];
        //
        minimap52Encounters = new int[15];
        //
        minimap53Encounters = new int[15];
        //underworld middle
        minimap54Encounters = new int[20];
        //
        minimap55Encounters = new int[15];
        //
        minimap56Encounters = new int[15];
        //
        minimap57Encounters = new int[15];
        //
        minimap58Encounters = new int[15];
        //
        minimap59Encounters = new int[15];
        //
        minimap60Encounters = new int[15];
        //
        minimap61Encounters = new int[15];
        //
        minimap62Encounters = new int[15];
        //
        minimap63Encounters = new int[15];
        //
        minimap64Encounters = new int[15];
        //
        minimap65Encounters = new int[15];
        //
        minimap66Encounters = new int[15];
        //
        minimap67Encounters = new int[15];
        //
        minimap68Encounters = new int[15];
        //
        minimap69Encounters = new int[15];
        //
        minimap70Encounters = new int[15];
        //
        minimap71Encounters = new int[15];
        //
        minimap72Encounters = new int[15];
        //
        minimap73Encounters = new int[15];
        //
        minimap74Encounters = new int[15];
        //
        minimap75Encounters = new int[15];
        //
        minimap76Encounters = new int[15];
        //
        minimap77Encounters = new int[15];
        //
        minimap78Encounters = new int[15];
        //
        minimap79Encounters = new int[15];


        //these record the number of each encounter, based on the possible encounters list in NodeEncounterHandler (999 is ignored)
        //cornville
        minimap1EncounterNumbers = new int[20];
        //c farm east
        minimap2EncounterNumbers = new int[20];
        //c farm west
        minimap3EncounterNumbers = new int[20];
        //c farm south
        minimap4EncounterNumbers = new int[20];
        //badgerwood north
        minimap5EncounterNumbers = new int[20];
        //forest fort
        minimap6EncounterNumbers = new int[20];
        //badgerwood SE
        minimap9EncounterNumbers = new int[20];
        //badgerwood SW
        minimap10EncounterNumbers = new int[20];
        //guildhouse
        minimap11EncounterNumbers = new int[20];
        //moltenrock lvl1
        minimap12EncounterNumbers = new int[20];
        //moltenrock lvl2 (sub-area)
        minimap13EncounterNumbers = new int[20];
        //unused
        minimap14EncounterNumbers = new int[20];
        //coven
        minimap16EncounterNumbers = new int[20];
        //ancient graveyard
        minimap17EncounterNumbers = new int[20];
        //faewood mid
        minimap18EncounterNumbers = new int[20];
        //faewood west
        minimap19EncounterNumbers = new int[20];
        //faewood clearing
        minimap20EncounterNumbers = new int[20];
        //grimhold
        minimap21EncounterNumbers = new int[20];
        //faewood north
        minimap22EncounterNumbers = new int[20];
        //firstborn fortress lvl 1
        minimap23EncounterNumbers = new int[20];
        //firstborn fortress lvl 2
        minimap24EncounterNumbers = new int[20];
        //firstborn fortress lvl 0
        minimap25EncounterNumbers = new int[20];
        //crypts lvl 1
        minimap26EncounterNumbers = new int[20];
        //
        minimap27EncounterNumbers = new int[20];
        //
        minimap28EncounterNumbers = new int[20];
        //
        minimap29EncounterNumbers = new int[20];
        //
        minimap30EncounterNumbers = new int[20];
        //
        minimap31EncounterNumbers = new int[20];
        //
        minimap32EncounterNumbers = new int[20];
        //
        minimap33EncounterNumbers = new int[20];
        //
        minimap34EncounterNumbers = new int[20];
        //
        minimap35EncounterNumbers = new int[20];
        //
        minimap36EncounterNumbers = new int[20];
        //
        minimap37EncounterNumbers = new int[20];
        //
        minimap38EncounterNumbers = new int[20];
        //
        minimap39EncounterNumbers = new int[20];
        //
        minimap40EncounterNumbers = new int[20];
        //
        minimap41EncounterNumbers = new int[20];
        //
        minimap42EncounterNumbers = new int[20];
        //
        minimap43EncounterNumbers = new int[20];
        //
        minimap44EncounterNumbers = new int[20];
        //
        minimap45EncounterNumbers = new int[20];
        //
        minimap46EncounterNumbers = new int[20];
        //
        minimap47EncounterNumbers = new int[20];
        //
        minimap48EncounterNumbers = new int[20];
        //
        minimap49EncounterNumbers = new int[20];
        //
        minimap50EncounterNumbers = new int[20];
        //
        minimap51EncounterNumbers = new int[20];
        //
        minimap52EncounterNumbers = new int[20];
        //
        minimap53EncounterNumbers = new int[20];
        //
        minimap54EncounterNumbers = new int[20];
        //
        minimap55EncounterNumbers = new int[20];
        //
        minimap56EncounterNumbers = new int[20];
        //
        minimap57EncounterNumbers = new int[20];
        //
        minimap58EncounterNumbers = new int[20];
        //
        minimap59EncounterNumbers = new int[20];
        //
        minimap60EncounterNumbers = new int[20];
        //
        minimap61EncounterNumbers = new int[20];
        //
        minimap62EncounterNumbers = new int[20];
        //
        minimap63EncounterNumbers = new int[20];
        //
        minimap64EncounterNumbers = new int[20];
        //
        minimap65EncounterNumbers = new int[20];
        //
        minimap66EncounterNumbers = new int[20];
        //
        minimap67EncounterNumbers = new int[20];
        //
        minimap68EncounterNumbers = new int[20];
        //
        minimap69EncounterNumbers = new int[20];
        //
        minimap70EncounterNumbers = new int[20];
        //
        minimap71EncounterNumbers = new int[20];
        //
        minimap72EncounterNumbers = new int[20];
        //
        minimap73EncounterNumbers = new int[20];
        //
        minimap74EncounterNumbers = new int[20];
        //
        minimap75EncounterNumbers = new int[20];
        //
        minimap76EncounterNumbers = new int[20];
        //
        minimap77EncounterNumbers = new int[20];
        //
        minimap78EncounterNumbers = new int[20];
        //
        minimap79EncounterNumbers = new int[20];


        //keeps track whether the location is visited or not
        hasVisitedMinimap = new bool[80];

        //keeps track of unresolved encounters for each location
        //node number 13 reserved for moltenrock lvl 2
        totalEncounters = new int[80];

        //portal destinations
        foundWaypoints = new int[5] { 0, 0, 0, 0, 0 };

        triggeredOverlays = new bool[100];
    }
}
