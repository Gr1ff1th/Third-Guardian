using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class Character : MonoBehaviour, IDataPersistence
{
    private PhotonView PV;

    //hero name
    public string heroName;

    //hero number (beren = 0, suliman = 1, dazzle = 2, maximus = 3, melissya = 4, targas = 5, naomi = 6, ariel = 7, enigma = 8, rimlic = 9)
    public int heroNumber;

    //hero classes: 1= warrior, 2= artisan, 3= arcanist, 4= general
    //dont think this is used in v94
    public List<int> heroClasses;

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
    //could use array for these actually
    public int[] equipmentStats = {0, 0, 0, 0, 0, 0, 0, 0, 0};

    //combat modifiers
    public int strengthModifier;
    public int arcanePowerModifier;
    public int bombModifier;
    public int defenseModifier;
    public int resistanceModifier;

    //modified version for v94
    public int finalStrengthModifier;
    public int finalArcanePowerModifier;
    public int finalBombModifier;
    public int finalHolyModifier;
    public int finalDefenseModifier;
    public int finalResistanceModifier;

    //more mofidiers for v93
    public int bombAttack;
    public int holyAttack;
    public int holyModifier;
    public int healthRegen;
    public int energyRegen;

    //movementBonus stat
    public int movementBonus;
    //this isnt used anymore?
    //public int overmapMoveCost = 4;

    //character level variables
    public int characterLevel;
    public int fameForNextLevel;

    //this increases each time you gain level
    public int fameForNextLevelTreshold;

    //Resources
    //0=energy, 2=skillpoints, 3=health, 4=coins, 5=fame, 6=favor, 16=bombs
    public int energy;
    public int maxEnergy;
    public int warriors;
    public int artisans;
    public int arcanists;
    public int agents;
    public int coins;
    public int fame;
    public int honorTotal;
    public int favor;
    public int maxFavor;
    public int dust;
    public int skillPoints;
    public int health;
    public int maxHealth;

    //this is not longer used this way
    //public int bombs;

    //might need this for future perks?
    public int maxActionPoints;

    //save reference from reputation displays
    public TextMeshProUGUI honorVisibleText;

    //biography
    public string biographyText;

    //energy orbs
    public List<GameObject> energyOrbs;

    //heroname Text
    public TextMeshProUGUI heroNameText;
    /*
    public int movementBonus = 2;
    public int extraMovement = 1;
    public bool canMove = true;
    */

    // something to click on
    [HideInInspector]
    public Collider col;

    //for popup texts
    public GameObject toolTipBackground;
    public TextMeshProUGUI toolTipText;

    //for when collider is enabled for the purposes of attack cards
    public string targettedText;

    //for determining what kind of purpose the colliders are used for
    public int targettingAction;

    //special abilities
    public Image abilityIcon;
    public string abilityDesc;

    //head directions
    public Sprite facingRight;
    public Sprite facingLeft;

    //for keeping track of hand cards
    public int handCards;
    public Text hccText;

    //for keeping track of "artifact" cards
    public int artifactCards;

    public GameObject sleepOverlay;
    public GameObject imprisonmentOverlay;

    //flag variable for illustrious engineer (does nothing now)
    public bool hasPowder;

    //flag variable for self-maintenance
    public bool isSelfMaintenance;

    //for the purposes of character selection scene
    public bool isSelected;

    //variable that gets checked after hero is first loaded
    public bool firstLoaded;

    //needed to adjust the damage display position in v0.7.0.
    public float damageDisplayHeight;

    private void Start()
    {
        if (PhotonRoom.room.currentScene == 3)
        {
            //for v0.7.0.
            //sets the position of the damage display
            Vector3 damageDisplayPosition = GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().damageText.gameObject.transform.localPosition;
            GameObject damageTextObject = GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().damageText.gameObject;
            damageTextObject.transform.localPosition = 
                new Vector3(damageDisplayPosition.x, damageDisplayPosition.y + damageDisplayHeight, damageDisplayPosition.z);

            //dunno if this is rly needed
            GameManager.ins.references.characterIconDisplay.SetActive(true);

            PV = GetComponent<PhotonView>();

            col = GetComponent<Collider>();

            //remove collider from character
            col.enabled = false;

            //gets objects for tooltips, from node this case (dunno where else to find them..)
            toolTipBackground = GameManager.ins.toolTipBackground;
            toolTipText = GameManager.ins.toolTipText;


            /* not needed anymore
             * 
             * gets text objects
            strengthText = GameObject.Find("StrengthText").GetComponent<TextMeshProUGUI>();
            influenceText = GameObject.Find("InfluenceText").GetComponent<TextMeshProUGUI>();
            mechanicsText = GameObject.Find("MechanicsText").GetComponent<TextMeshProUGUI>();
            diggingText = GameObject.Find("DiggingText").GetComponent<TextMeshProUGUI>();
            loreText = GameObject.Find("LoreText").GetComponent<TextMeshProUGUI>();
            observeText = GameObject.Find("ObserveText").GetComponent<TextMeshProUGUI>();
            */
            hccText = GameManager.ins.hccText;

            //needs to be done this way or else game gives info to wrong player for some reason
            //gameObject.GetComponentInParent<CharController>().UpdateStatsTexts();

            //UpdateStatTexts();
            GetEnergyOrbs();

            //gets more text objects
            //energyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
            //agentsText = GameObject.Find("AgentsText").GetComponent<TextMeshProUGUI>();
            //warriorsText = GameObject.Find("WarriorText").GetComponent<TextMeshProUGUI>();
            //artisansText = GameObject.Find("ArtisanText").GetComponent<TextMeshProUGUI>();
            //arcanistsText = GameObject.Find("ArcanistText").GetComponent<TextMeshProUGUI>();
            //coinsText = GameObject.Find("CoinsText").GetComponent<TextMeshProUGUI>();
            //honorText = GameObject.Find("HonorText").GetComponent<TextMeshProUGUI>();
            //honorTotalText = GameObject.Find("HonorTotalText").GetComponent<TextMeshProUGUI>();

            //for Hero name display
            heroNameText = GameObject.Find("HeroNameText").GetComponent<TextMeshProUGUI>();

            //UpdateResourceTexts();

            //lets try something weird
            //gameObject.GetComponentInParent<CharController>().PV = gameObject.GetComponentInParent<CharController>().GetComponent<PhotonView>();

            //Invoke("GetPlayerNumber", 1.0f);
            GetPlayerNumber();

            //needs to be done this way or else game gives info to wrong player for some reason
            gameObject.GetComponentInParent<CharController>().HeroDisplay();

            //needs to be done this way or else game gives info to wrong player for some reason
            gameObject.GetComponentInParent<CharController>().FirstTurnIndicator();

            //synchronizes turn number variable between all players
            gameObject.GetComponentInParent<CharController>().SynchronizeTurnNumber();

            hasPowder = false;

            Invoke("ShowReputationDisplay", 1.0f);

            //lets put this here for now
            UpdateStatTexts();

            firstLoaded = false;

            SetStartResources();

            MakeImageBackup();
        }
    }

    void MakeImageBackup()
    {
        GameManager.ins.references.heroBackupImage = GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[heroNumber].GetComponent<Image>().sprite;
    }

    //for reputation displays
    void ShowReputationDisplay()
    {
        GetComponentInParent<CharController>().ShowReputationDisplay();
    }


    //synchronizes heronumber between clients
    public void GetPlayerNumber()
    {
        int number1 = GetComponentInParent<CharController>().turnNumber;
        int number2 = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>().mySelectedCharacter;
        
        GetComponentInParent<CharController>().GetPlayerNumber(number1, number2);
    }

    //shows hero icon & name
    public void HeroDisplay()
    {
        heroNameText.text = heroName.ToString();
        GameManager.ins.uiButtonHandler.statDisplayHeroName.text = heroName.ToString();
        GameManager.ins.uiButtonHandler.equipmentDisplayHeroName.text = heroName.ToString();
        GameManager.ins.DisplayHero(GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>().mySelectedCharacter);
    }

    public void UpdateStatTexts()
    {
        //show character skill values
        //strengthText.text = strength.ToString();
        GameManager.ins.references.strengthText.text = "";
        GameManager.ins.references.defenseText.text = "";
        GameManager.ins.references.arcanePowerText.text = "";
        GameManager.ins.references.resistanceText.text = "";

        GameManager.ins.references.influenceText.text = "";
        GameManager.ins.references.mechanicsText.text = "";
        GameManager.ins.references.diggingText.text = "";
        GameManager.ins.references.loreText.text = "";
        GameManager.ins.references.observeText.text = "";

        GameManager.ins.references.strengthModifierText.text = "";
        GameManager.ins.references.arcanePowerModifierText.text = "";
        GameManager.ins.references.bombModifierText.text = "";
        GameManager.ins.references.defenseModifierText.text = "";
        GameManager.ins.references.resistanceModifierText.text = "";

        GameManager.ins.references.bombAttackText.text = "";
        GameManager.ins.references.holyAttackText.text = "";
        GameManager.ins.references.holyModifierText.text = "";
        GameManager.ins.references.hpRegenText.text = "";
        GameManager.ins.references.energyRegenText.text = "";
        GameManager.ins.references.movementBonusText.text = "";

        /*
        if (strength > 6)
        {
            strengthText.text = "<voffset=4><size=24>" + strength.ToString() + "</size></voffset>" + "<sprite index=6>";
        }
        if (strength <= 6)
        {
            for (int i = 0; i < strength; i++)
            {
                strengthText.text += "<sprite index=6>";
            }
        }
        
        //save this just in case
        if (strength < 0)
        {
            for (int i = 0; i > strength && i > -6; i--)
            {
                GameManager.ins.references.strengthText.text += "<sprite=\"sprites v94\" index=0>";
            }
        }
        */
        //strength
        //ill leave color tag here for later reference (recolors to green, last two hexes are for alpha)
        //cap the max displayed skillpoints at 6

        for (int i = 0; i < strength && i < 6; i++)
        {
            //GameManager.ins.references.strengthText.text += "<sprite=\"resources & skills\" index=20 color=#00FF19>";
            if (i < strengthUpgrades)
            {
                GameManager.ins.references.strengthText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i < strengthUpgrades + equipmentStats[0])
            {
                GameManager.ins.references.strengthText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.strengthText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //defense

        for (int i = 0; i < defense && i < 6; i++)
        {
            if (i < defenseUpgrades)
            {
                GameManager.ins.references.defenseText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i < defenseUpgrades + equipmentStats[1])
            {
                GameManager.ins.references.defenseText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.defenseText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //AP

        for (int i = 0; i < arcanePower && i < 6; i++)
        {
            if (i < arcanePowerUpgrades)
            {
                GameManager.ins.references.arcanePowerText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i < arcanePowerUpgrades + equipmentStats[2])
            {
                GameManager.ins.references.arcanePowerText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.arcanePowerText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //resistance

        for (int i = 0; i < resistance && i < 6; i++)
        {
            if (i < resistanceUpgrades)
            {
                GameManager.ins.references.resistanceText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i < resistanceUpgrades + equipmentStats[3])
            {
                GameManager.ins.references.resistanceText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.resistanceText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //influence

        for (int i = 0; i < influence && i < 6; i++)
        {
            //need to do this differently for the right side
            if (i >= influence - influenceUpgrades)
            {
                GameManager.ins.references.influenceText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i >= influence - (influenceUpgrades + equipmentStats[4]))
            {
                GameManager.ins.references.influenceText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.influenceText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //mechanics

        for (int i = 0; i < mechanics && i < 6; i++)
        {
            if (i >= mechanics - mechanicsUpgrades)
            {
                GameManager.ins.references.mechanicsText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i >= mechanics - (mechanicsUpgrades + equipmentStats[5]))
            {
                GameManager.ins.references.mechanicsText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.mechanicsText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //digging

        for (int i = 0; i < digging && i < 6; i++)
        {
            if (i >= digging - diggingUpgrades)
            {
                GameManager.ins.references.diggingText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i >= digging - (diggingUpgrades + equipmentStats[6]))
            {
                GameManager.ins.references.diggingText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.diggingText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //lore

        for (int i = 0; i < lore && i < 6; i++)
        {
            if (i >= lore - loreUpgrades)
            {
                GameManager.ins.references.loreText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i >= lore - (loreUpgrades + equipmentStats[7]))
            {
                GameManager.ins.references.loreText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.loreText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }


        //discovery

        for (int i = 0; i < observe && i < 6; i++)
        {
            if (i >= observe - observeUpgrades)
            {
                GameManager.ins.references.observeText.text += "<sprite=\"blue bar\" index=0>";
            }
            else if (i >= observe - (observeUpgrades + equipmentStats[8]))
            {
                GameManager.ins.references.observeText.text += "<sprite=\"resources & skills\" index=21 color=#FF996EFF>";
            }
            else
            {
                GameManager.ins.references.observeText.text += "<sprite=\"resources & skills\" index=21>";
            }
        }

        //new feature for v94 (shows cursed orbs for negative skill values (up to -3)
        GameManager.ins.references.SetSkillOrbs();

        //could do the modifier conversions here?
        finalStrengthModifier = (int)(strengthModifier * GameManager.ins.references.combatModifierMultiplyer);
        finalArcanePowerModifier = (int)(arcanePowerModifier * GameManager.ins.references.combatModifierMultiplyer);
        finalBombModifier = (int)(bombModifier * GameManager.ins.references.combatModifierMultiplyer);
        finalHolyModifier = (int)(holyModifier * GameManager.ins.references.combatModifierMultiplyer);

        //will see if these work
        finalDefenseModifier = (int)((defenseModifier + 100) * GameManager.ins.references.combatModifierMultiplyer) - 100;
        finalResistanceModifier = (int)((resistanceModifier + 100) * GameManager.ins.references.combatModifierMultiplyer) - 100;

        //combat modifiers
        GameManager.ins.references.strengthModifierText.text = finalStrengthModifier + "%";
        GameManager.ins.references.arcanePowerModifierText.text = finalArcanePowerModifier + "%";
        GameManager.ins.references.bombModifierText.text = finalBombModifier + "%";
        GameManager.ins.references.defenseModifierText.text = finalDefenseModifier + "%";
        GameManager.ins.references.resistanceModifierText.text = finalResistanceModifier + "%";

        //special case for bomb & holy attacks
        float mechFloat = Mathf.Ceil(mechanics / 2f);
        int halvedMech = (int)mechFloat;
        int finalBombAttack = bombAttack + halvedMech;
        GameManager.ins.references.bombAttackText.text = "+" + finalBombAttack;

        float loreFloat = Mathf.Ceil(lore / 2f);
        int halvedLore = (int)loreFloat;
        int finalHolyAttack = holyAttack + halvedLore;
        GameManager.ins.references.holyAttackText.text = "+" + finalHolyAttack;

        GameManager.ins.references.holyModifierText.text = finalHolyModifier + "%";
        GameManager.ins.references.hpRegenText.text = healthRegen + "%";
        GameManager.ins.references.energyRegenText.text = energyRegen + "%";
        GameManager.ins.references.movementBonusText.text = movementBonus.ToString();

        GameManager.ins.references.scoreModifierText.text = GameManager.ins.scoreModifier.ToString() + "%";//PhotonRoom.room.scoreModifier.ToString() + "%";

        //might as well put this here
        GameManager.ins.references.levelText.text = "Level " + characterLevel;
    }

    public void UpdateStats(int stat, int qty)
    {
        if (stat == 1)
        {
            strength += qty;
        }
        if (stat == 2)
        {
            influence += qty;
        }
        if (stat == 3)
        {
            mechanics += qty;
        }
        if (stat == 4)
        {
            digging += qty;
        }
        if (stat == 5)
        {
            lore += qty;
        }
        if (stat == 6)
        {
            observe += qty;
        }

        //for movementBonus
        if (stat == 7)
        {
            movementBonus += qty;
        }
        /* not used anymore
         * 
         * update stat texts
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStats();
        if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
        {
            UpdateStatTexts();
        }
        */
    }

    //dunno if this should be RPC function?
    public void UpdateResourceTexts()
    {
        //do this only on your turn?
        if (GetComponentInParent<CharController>().ItsYourTurn()) // (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //energyText.text = energy.ToString();
            /*update energy orbs
            for(int i = 0; i < 8; i++)
            {
                energyOrbs[i].gameObject.SetActive(false);

                if(energy == i)
                {
                    energyOrbs[i].gameObject.SetActive(true);
                }
            }
            */

            //lets just put this here, so that ai players wont change resource display
            if(gameObject.GetComponentInParent<CharController>().isAi == true)
            {
                return;
            }


            GameManager.ins.references.coinsText.text = "";
            //GameManager.ins.references.dustText.text = "";
            GameManager.ins.references.skillPointText.text = "";
            //GameManager.ins.references.favorText.text = "";
            //GameManager.ins.references.energyText.text = "";
            //GameManager.ins.references.bombText.text = "";

            /*
            if (coins > 6)
            {
                coinsText.text = "<voffset=4><size=24>" + coins.ToString() + "</size></voffset>" + "<sprite index=3>";
            }
            if (coins <= 6)
            {
                for (int i = 0; i < coins; i++)
                {
                    coinsText.text += "<sprite index=3>";
                }
            }
            */
            GameManager.ins.references.coinsText.text = coins.ToString();

            //GameManager.ins.references.dustText.text = dust.ToString();

            GameManager.ins.references.skillPointText.text = skillPoints.ToString();

            //GameManager.ins.references.bombText.text = bombs.ToString();

            //GameManager.ins.references.favorText.text = favor.ToString();

            //GameManager.ins.references.energyText.text = energy.ToString() + "/" + maxEnergy.ToString();

            //update the bars
            GameManager.ins.references.GetComponent<SliderController>().SetBarValues(GetComponentInParent<CharController>().turnNumber);
        }
    }
    /*for more basic resources
    public void UpdateResources(int type, int qty)
    {
        PV.RPC("RPC_UpdateResources", RpcTarget.AllBufferedViaServer, type, qty);
    }
    */

    //for more basic resources
    public void UpdateResources(int type, int qty)
    {
        //energy
        if (type == 0)
        {
            /* lets do this the other way
            if (isSelfMaintenance == true)
            {
                energy = energy + qty;
                isSelfMaintenance = false;
            }
            */
            //if player is sentinel, he can only gain energy from self maintenance (can lose energy otherwise though)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == true)
            {
                if (qty < 0)
                {
                    energy = energy + qty;
                }
                else if (isSelfMaintenance == true)
                {
                    energy = energy + qty;
                }
            }

            //sentinel only gainst energy, if selfmaintenance flag is set true (is set true also for rejuvenation potions)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                energy = energy + qty;
            }

            //energy caps at max energy
            if (energy > maxEnergy)
            {
                energy = maxEnergy;
            }

            //sets energy to 0, if energy goes below 0
            if (energy < 0)
            {
                /*
                 * set sleep flag variable, in case its a free action, and current player is set to sleep
                if (ReferenceEquals(gameObject.GetComponentInParent<CharController>(), GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>()))
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().sleptWhileMoving = true;
                }
                
                if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
                {
                    
                    int negativeEnergy = energy * (-1);

                    //send player to sleep
                    GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().PV.RPC("RPC_Sleep", RpcTarget.AllBufferedViaServer, GetComponentInParent<CharController>().turnNumber, negativeEnergy);
                    

                    gameObject.GetComponentInParent<CharController>().isDead = true;
                }
                */
                //this shouldnt be under itsyourturn -check
                energy = 0;
            }

            //movementBonus checks
            /*special case for ironclad (dont rly need it at v90 tho?)
            if (gameObject.GetComponentInParent<CharController>().HasPassiveTest(11) == true && energy >= 2)
            {
                gameObject.GetComponentInParent<CharController>().canMove = true;
            }
            else if (gameObject.GetComponentInParent<CharController>().extraMovement == 0)
            {
                gameObject.GetComponentInParent<CharController>().canMove = false;
            }
            else if(gameObject.GetComponentInParent<CharController>().HasPassiveTest(11) == false && energy >= 1)
            {
                gameObject.GetComponentInParent<CharController>().canMove = true;
            }
            */
        }

        #region old resources
        /*
        //warriors
        else if (type == 1)
        {
            //tests if player has demons horn
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 29 && qty > 0 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                    {
                        UpdateResources(0, 1);
                    }
                }
            }

            warriors = warriors + qty;
            if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
            {
                //warriorsText.text = warriors.ToString();
            }
        }
        //artisans
        else if (type == 2)
        {
            //tests if player has illustrious engineer perk
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<PerkCard>().effect == 53 && qty > 0 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                    {
                        //should be true if we wanna limit the synergy
                        //hasPowder = true;
                        hasPowder = false;
                        UpdateResources(4, 3);
                    }
                }
            }

            artisans = artisans + qty;
            if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
            {
                //artisansText.text = artisans.ToString();
            }
        }
        //arcanists
        else if (type == 3)
        {
            //tests if player has staff of nabamax
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 31 && qty > 0 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                    {
                        qty += 1;
                    }
                }
            }

            arcanists = arcanists + qty;
            if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
            {
                //arcanistsText.text = arcanists.ToString();
            }
        }
        */
        #endregion

        //arcane dust
        else if (type == 1)
        {
            dust = dust + qty;
        }

        //skill points
        else if (type == 2)
        {
            skillPoints = skillPoints + qty;

            if (skillPoints > 0)
            {
                GameManager.ins.uiButtonHandler.skillUpgradeButton1.SetActive(true);
            }

            if (skillPoints == 0)
            {
                GameManager.ins.uiButtonHandler.skillUpgradeButton1.SetActive(false);
            }
        }

        //health
        //since v92
        else if (type == 3)
        {
            //health = health + qty;

            //if player is sentinel, he can only gain energy from self maintenance (can lose energy otherwise though)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == true)
            {
                if (qty < 0)
                {
                    health = health + qty;
                }
                else if (isSelfMaintenance == true)
                {
                    health = health + qty;
                }
            }

            //sentinel only gainst energy, if selfmaintenance flag is set true (is set true also for rejuvenation potions)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                health = health + qty;
            }

            //energy caps at max energy
            if (health > maxHealth)
            {
                health = maxHealth;
            }

            //sets player to dead, if energy goes below 0
            if (health < 0)
            {
                if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
                {
                    gameObject.GetComponentInParent<CharController>().isDead = true;
                }

                //this shouldnt be under itsyourturn -check
                health = 0;
            }
        }


        //coins
        else if (type == 4)
        {
            //tests if player has golden goblet
            if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 6, 246))
            {
                if (qty > 0)
                {
                    qty += qty / 4;

                    CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[246].GetComponent<CardDisplay2>().sfx;
                    CardHandler.ins.extraSfxHolder.Play();
                }
            }

            //tests if player has goldfingers
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(291) == true)
            {
                if (qty > 0)
                {
                    qty += qty / 4;

                    CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[246].GetComponent<CardDisplay2>().sfx;
                    CardHandler.ins.extraSfxHolder.Play();
                }
            }

            if ((qty + coins) < 0)
            {
                qty = -coins;
            }

            coins = coins + qty;
            if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
            {
                //coinsText.text = coins.ToString();
            }
        }

        //fame (was honor)
        else if (type == 5)
        {
            /* remove these for now
             * tests if player has skull ring, and getting negative favor
            for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
                {
                    if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 33 && qty < 0 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == GameManager.ins.turnNumber)
                    {
                        UpdateResources(0, 2);
                    }
                }
            }

            // priestess perk causes additional negative energy, if any was gained
            if (GetComponentInParent<CharController>().PriestessTest() == true && qty < 0)
            {
                UpdateResources(0, -1);

                if (GetComponentInParent<CharController>().ItsYourTurn() == true && GetComponentInParent<CharController>().isAi == false)
                {
                    //give message
                    string msgs = "You lost additional energy because you are a priestess.";
                    GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatDisplay.text += "\n<color=#00fcffff> System: " + msgs + "</color>";
                }
            }
            */

            //special case for poet
            if (GetComponentInParent<CharController>().HasPassiveTest(16) == true && qty >= 4)
            {
                int poetBonus = qty / 4;

                qty += poetBonus;

                //use poet sfx
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[15].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }

            //eagle feater talisman test
            if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 10, 86) == true && qty >= 4)
            {
                int talismanBonus = qty / 6;

                qty += talismanBonus;

                //use poet sfx
                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[86].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }


            fame = fame + qty;

            /* remove this for now
             * imprisons player, if favor goes below 0
            if (gameObject.GetComponentInParent<CharController>().ItsYourTurn() && fame < 0)
            {
                int negativeFavor = fame * (-1);

                //imprisons player
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().PV.RPC("RPC_Imprisonment", RpcTarget.AllBufferedViaServer, GetComponentInParent<CharController>().turnNumber, negativeFavor);

                //send to grimhold
                GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().PV.RPC("RPC_AcceptTransport", RpcTarget.AllBufferedViaServer, GetComponentInParent<CharController>().turnNumber, 7);

                fame = 0;
            }
            */

            //should reset self maintenance after energy & health checks?
            //why is it in fame check tho?
            isSelfMaintenance = false;

            //updates the visible (to all) reputation display
            UpdateVisibleReputation();

            LevelCalculator(qty);

            //remember to change VP's separately
        }

        //favor (was honor total)
        else if (type == 6)
        {
            favor = favor + qty;

            if (favor < 0)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, -favor);
                favor = 0;
            }

            if (favor > maxFavor)
            {
                favor = maxFavor;
            }

        }

        //agents (unused)
        else if (type == 15)
        {
            agents = agents + qty;
            if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
            {
                //agentsText.text = agents.ToString();
            }
        }

        // bombs
        // this works differently than other cases
        else if (type == 16)
        {
            //bombs = bombs + qty;
            //need at add additional "-" here, to make it triple negative, and get right result :-)
            if (qty < 0)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 169, 1, -qty);
            }

            if (qty > 0)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 169, 1, qty);
            }
        }

        //action points
        else if (type == 17)
        {
            //add sleep here?
            if (qty < 0)
            {
                //4 turns sleep
                //need to do on reverse?
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 21, 7, -qty);

                //add sleep overlay
                sleepOverlay.SetActive(true);
                //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 169, 1, -qty);
            }

            //consider if we need this later
            //not tested
            if (qty > 0)
            {
                GetComponentInParent<CharController>().actionPoints += qty;
                //CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 169, 1, qty);
            }
        }

        //should reset self maintenance after energy & health checks?
        isSelfMaintenance = false;

        if (gameObject.GetComponentInParent<CharController>().ItsYourTurn())
        {
            UpdateResourceTexts();
        }
    }

    public void UpdateVisibleReputation()
    {
        honorVisibleText.text = fame.ToString();
    }

    public void EnableCollider()
    {
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    //works for colliders of this class only
    void OnMouseEnter()
    {
        //for attack card targetting
        if (targettingAction == 1 || targettingAction == 2 || targettingAction == 3)
        {
            toolTipBackground.SetActive(true);
            toolTipText.text = targettedText.ToString();
        }
    }

    //works for colliders of this class only
    void OnMouseExit()
    {
        //for attack card targetting
        if (targettingAction == 1 || targettingAction == 2 || targettingAction == 3)
        {
            toolTipBackground.SetActive(false);
        }
    }
    //works for colliders of this class only
    void OnMouseDown()
    {
        //for attack
        if (targettingAction == 1)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomTextCanvas.SetActive(false);

            //store turnnumber of attack target
            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber = GetComponentInParent<CharController>().turnNumber;

            //
            //int turnNumber = gameObject.GetComponentInParent<CharController>().turnNumber;

            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Attack(turnNumber);
            GetComponentInParent<CharController>().Attack();

            //close tooltip
            toolTipBackground.SetActive(false);

            //reset flag variable & collider
            //actually this should be done for all heroes.. 
            //col.enabled = false;
            //targettingAction = 0;
            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CloseAllHeroColliders();
        }

        //for spying
        if (targettingAction == 2)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomTextCanvas.SetActive(false);

            //store turnnumber of attack target
            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber = gameObject.GetComponentInParent<CharController>().turnNumber;

            //actually might as well give the turn number directly
            GameObject.Find("Spying").GetComponent<SpyCanvas>().Spy(GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber, 1);

            //close tooltip
            toolTipBackground.SetActive(false);

            //reset flag variable & collider
            //actually this should be done for all heroes.. 
            //col.enabled = false;
            //targettingAction = 0;
            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CloseAllHeroColliders();

            //closes all encounter colliders also (because of v82 investigate)
            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CloseEncounterColliders();
        }

        //for spying with eye of isolore
        if (targettingAction == 3)
        {
            GameObject.Find("Dialog Canvas").GetComponent<CanvasController>().bottomTextCanvas.SetActive(false);

            //store turnnumber of attack target
            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber = gameObject.GetComponentInParent<CharController>().turnNumber;

            //actually might as well give the turn number directly
            GameObject.Find("Spying").GetComponent<SpyCanvas>().Spy(GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().targetTurnNumber, 5);

            //close tooltip
            toolTipBackground.SetActive(false);

            GameObject.Find("Dialog Canvas").GetComponent<AttackResolve>().CloseAllHeroColliders();
        }
    }
    /*
    public void Attack(int turnNumber)
    {
        Debug.Log("Attack Hit!");
        PV.RPC("RPC_Attack", RpcTarget.AllBufferedViaServer);
    }

    //just to test
    [PunRPC]
    void RPC_Attack()
    {
        Debug.Log("Attack Hit!");
    }
    */

    public void GetEnergyOrbs()
    {
        energyOrbs[0] = GameObject.Find("0 Energy");
        energyOrbs[1] = GameObject.Find("1 Energy");
        energyOrbs[2] = GameObject.Find("2 Energy");
        energyOrbs[3] = GameObject.Find("3 Energy");
        energyOrbs[4] = GameObject.Find("4 Energy");
        energyOrbs[5] = GameObject.Find("5 Energy");
        energyOrbs[6] = GameObject.Find("6 Energy");
        energyOrbs[7] = GameObject.Find("7 Energy");
    }

    //checks current stats, after applying all the effects
    public void StatUpdate()
    {
        //curse reduces 1 stat point per 2 curses
        int cursePenalty = CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 28, 7) / 2;

        //ensnsares reduce skills depending on how many stacks there are
        //petrified, entangle and web work slightly different
        int ensnarePenalty = 0;
        if(CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 171, 7) > 0 && CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 171, 7) < 3)
        {
            ensnarePenalty += 1;
        }
        if (CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 171, 7) > 2)
        {
            ensnarePenalty += 2;
        }
        if (CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 176, 7) > 0 && CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 176, 7) < 5)
        {
            ensnarePenalty += 1;
        }
        if (CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 176, 7) > 4)
        {
            ensnarePenalty += 2;
        }
        if (CardHandler.ins.CheckQuantity(gameObject.GetComponentInParent<CharController>().turnNumber, 182, 7) > 0)
        {
            ensnarePenalty += 1;
        }

        strength = maxStrength - cursePenalty - ensnarePenalty;
        defense = maxDefense - cursePenalty - ensnarePenalty;
        arcanePower = maxArcanePower - cursePenalty - ensnarePenalty;
        resistance = maxResistance - cursePenalty - ensnarePenalty;

        influence = maxInfluence - cursePenalty - ensnarePenalty;
        mechanics = maxMechanics - cursePenalty - ensnarePenalty;
        digging = maxDigging - cursePenalty - ensnarePenalty;
        lore = maxLore - cursePenalty - ensnarePenalty;
        observe = maxObserve - cursePenalty - ensnarePenalty;

        UpdateStatTexts();

    }

    void LevelCalculator(int fameChange)
    {
        fameForNextLevel -= fameChange;

        //update reputation display
        GetComponentInParent<CharController>().ShowReputationDisplay();

        if (fameForNextLevel <= 0)
        {
            do
            {
                characterLevel += 1;
                GameManager.ins.references.levelText.text = "Level " + characterLevel;

                //levelup upgrades
                maxEnergy += 1;
                energy += 1;
                skillPoints += 5;
                maxFavor += 1;
                maxHealth += 1;
                health += 1;

                healthRegen += 1;
                energyRegen += 1;
                //favor += 1;

                //special case for fast learner
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(249))
                {
                    skillPoints += 1;
                }

                GameManager.ins.uiButtonHandler.skillUpgradeButton1.SetActive(true);

                //this also updates the bars
                UpdateResourceTexts();

                //need to update stats in v93 too
                UpdateStatTexts();

                //next level requires 30% more fame?
                fameForNextLevelTreshold = (int)(fameForNextLevelTreshold * 1.3f);
                fameForNextLevel += fameForNextLevelTreshold;

                //draw possible new upgrade cards
                CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DrawUpgradeCards3();

                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
                {
                    //give message
                    string msgs = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroName + " gained a level!";
                    //GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().PV.RPC("RPC_SystemMessage", RpcTarget.AllBufferedViaServer, msgs);
                    GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().SystemMessage(msgs);

                    //update reputation display
                    GetComponentInParent<CharController>().ShowReputationDisplay();
                }


                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().SpawnEffectOnce(40, 1);
                GameManager.ins.references.sfxPlayer.PlayRum();
            }
            while (fameForNextLevel <= 0);
        }
    }

    //used by certain backgrounds
    //need to change the level treshold manually
    public void InstaLevelup()
    {
        characterLevel += 1;
        GameManager.ins.references.levelText.text = "Level " + characterLevel;

        //levelup upgrades
        maxEnergy += 1;
        energy += 1;
        skillPoints += 5;
        maxFavor += 1;
        maxHealth += 1;
        health += 1;

        healthRegen += 1;
        energyRegen += 1;
        //favor += 1;

        //special case for fast learner
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(249))
        {
            skillPoints += 1;
        }

        GameManager.ins.uiButtonHandler.skillUpgradeButton1.SetActive(true);

        //this also updates the bars
        UpdateResourceTexts();

        //need to update stats in v93 too
        UpdateStatTexts();

        //draw possible new upgrade cards
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DrawUpgradeCards3();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ShowReputationDisplay();
    }

    public void SetStartResources()
    {
        //dont need this in v94
        //int savedCoins = DataPersistenceManager.instance.gameData.coins;

        if (PhotonRoom.room.currentScene == 3)
        {
            if (firstLoaded == false && PhotonRoom.room.spContinue == true)
            {
                //coins = savedCoins;
                UpdateResourceTexts();
                firstLoaded = true;

                //try call loaddata here?
                LoadData(DataPersistenceManager.instance.gameData);
            }

            //only do this first time hero is loaded in main scene
            else if (firstLoaded == false && PhotonRoom.room.spContinue == false)
            {
                //dont need this anymore
                //UpdateResources(4, PhotonRoom.room.startCoins);

                firstLoaded = true;
            }
        }
    }

    //theres a risk this will be called twice now?
    public void LoadData(GameData data)
    {
        //only need to load these for continue games
        if (PhotonRoom.room.spContinue == true)
        {
            //hero stats
            strength = data.strength;
            defense = data.defense;
            arcanePower = data.arcanePower;
            resistance = data.resistance;

            influence = data.influence;
            mechanics = data.mechanics;
            digging = data.digging;
            lore = data.lore;
            observe = data.observe;

            // max stats
            // needed for curse effects
            maxStrength = data.maxStrength;
            maxDefense = data.maxDefense;
            maxArcanePower = data.maxArcanePower;
            maxResistance = data.maxResistance;

            maxInfluence = data.maxInfluence;
            maxMechanics = data.maxMechanics;
            maxDigging = data.maxDigging;
            maxLore = data.maxLore;
            maxObserve = data.maxObserve;

            //skillpoint upgrades
            strengthUpgrades = data.strengthUpgrades;
            defenseUpgrades = data.defenseUpgrades;
            arcanePowerUpgrades = data.arcanePowerUpgrades;
            resistanceUpgrades = data.resistanceUpgrades;

            influenceUpgrades = data.influenceUpgrades;
            mechanicsUpgrades = data.mechanicsUpgrades;
            diggingUpgrades = data.diggingUpgrades;
            loreUpgrades = data.loreUpgrades;
            observeUpgrades = data.observeUpgrades;

            //stat bonuses from equipment
            equipmentStats = data.equipmentStats;

            //combat modifiers
            strengthModifier = data.strengthModifier;
            arcanePowerModifier = data.arcanePowerModifier;
            bombModifier = data.bombModifier;
            defenseModifier = data.defenseModifier;
            resistanceModifier = data.resistanceModifier;

            bombAttack = data.bombAttack;
            holyAttack = data.holyAttack;
            holyModifier = data.holyModifier;
            healthRegen = data.healthRegen;
            energyRegen = data.energyRegen;

            //movementBonus stat
            movementBonus = data.movementBonus;

            //character level variables
            characterLevel = data.characterLevel;
            fameForNextLevel = data.fameForNextLevel;

            //this increases each time you gain level
            fameForNextLevelTreshold = data.fameForNextLevelTreshold;

            //Resources
            //0=energy, 2=skillpoints, 3=health, 4=coins, 5=fame, 6=favor, 16=bombs
            energy = data.energy;
            maxEnergy = data.maxEnergy;
            coins = data.coins;
            fame = data.fame;
            favor = data.favor;
            maxFavor = data.maxFavor;
            skillPoints = data.skillPoints;
            health = data.health;
            maxHealth = data.maxHealth;
            maxActionPoints = data.maxActionPoints;

            //actions points need to be updated on parent class
            GetComponentInParent<CharController>().actionPoints = data.actionPoints;

            //lets update these
            UpdateResourceTexts();
            UpdateStatTexts();
            //UpdateVisibleReputation();
        }
    }

    public void SaveData(ref GameData data)
    {
        //hero stats
        data.strength = strength;
        data.defense = defense;
        data.arcanePower = arcanePower;
        data.resistance = resistance;

        data.influence = influence;
        data.mechanics = mechanics;
        data.digging = digging;
        data.lore = lore;
        data.observe = observe;

        // max stats
        // needed for curse effects
        data.maxStrength = maxStrength;
        data.maxDefense = maxDefense;
        data.maxArcanePower = maxArcanePower;
        data.maxResistance = maxResistance;

        data.maxInfluence = maxInfluence;
        data.maxMechanics = maxMechanics;
        data.maxDigging = maxDigging;
        data.maxLore = maxLore;
        data.maxObserve = maxObserve;

        //skillpoint upgrades
        data.strengthUpgrades = strengthUpgrades;
        data.defenseUpgrades = defenseUpgrades;
        data.arcanePowerUpgrades = arcanePowerUpgrades;
        data.resistanceUpgrades = resistanceUpgrades;

        data.influenceUpgrades = influenceUpgrades;
        data.mechanicsUpgrades = mechanicsUpgrades;
        data.diggingUpgrades = diggingUpgrades;
        data.loreUpgrades = loreUpgrades;
        data.observeUpgrades = observeUpgrades;

        //stat bonuses from equipment
        data.equipmentStats = equipmentStats;

        //combat modifiers
        data.strengthModifier = strengthModifier;
        data.arcanePowerModifier = arcanePowerModifier;
        data.bombModifier = bombModifier;
        data.defenseModifier = defenseModifier;
        data.resistanceModifier = resistanceModifier;

        data.bombAttack = bombAttack;
        data.holyAttack = holyAttack;
        data.holyModifier = holyModifier;
        data.healthRegen = healthRegen;
        data.energyRegen = energyRegen;

        //movementBonus stat
        data.movementBonus = movementBonus;

        //character level variables
        data.characterLevel = characterLevel;
        data.fameForNextLevel = fameForNextLevel;

        //this increases each time you gain level
        data.fameForNextLevelTreshold = fameForNextLevelTreshold;

        //Resources
        //0=energy, 2=skillpoints, 3=health, 4=coins, 5=fame, 6=favor, 16=bombs
        data.energy = energy;
        data.maxEnergy = maxEnergy;
        data.coins = coins;
        data.fame = fame;
        data.favor = favor;
        data.maxFavor = maxFavor;
        data.skillPoints = skillPoints;
        data.health = health;
        data.maxHealth = maxHealth;
        data.maxActionPoints = maxActionPoints;

        //actions points need to be updated on parent class
        //data.actionPoints = GetComponentInParent<CharController>().actionPoints;
    }
}
