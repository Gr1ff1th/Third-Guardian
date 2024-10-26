using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


//stores all info of items & abilities
public class Card : MonoBehaviour
{
    //could use this for some unique cards?
    public bool isTaken;

    //class requirement: 1= warrior, 2= artisan, 3= arcanist, 4= general
    //public int classReq;

    //this has to match the slot in deck!!
    public int numberInDeck;

    // 1 for consumable, 2 for skill, 3 for special, 4 for foe, 5 for equipment, 7 for effect etc
    // is 3 actually used anywhere?
    //cardtype 12: location cards, 13: difficulty, 14: backgrounds
    public int cardType;

    //activation type: 1= any skill check, 2= map activation, 3 = defense card or skillcheck (shield of isolore, writ of quiron. used against attack cards where skillcheck is also possible), 4 = defense card only (inspiration can also be used)
    //certain cards can have multiple or all types, cardfunction at charcontroller determines which function is used.
    //activation times are done by case by case basis, dont need variable for it
    //these variables only needed for usable cards
    public int[] activationType;

    //if true, the card is usable
    public bool isUsable;

    //if true, the card is passive
    public bool isPassive;

    //if true, card is exhausted on use (removed from display, not taken necessarily)
    public bool isExhaustable;

    //use when skill requires favor
    public int requiresFavor;

    //use when skill requires favor
    public int requiresEnergy;

    //use when skill requires coins
    public int requiresCoins;

    //for v94, only used to check refreshs atm
    public int requiresBombs;
    public int requiresHealth;

    //special flag for abilities, which cannot be refreshed with abilities
    //for cases when energy etc is refreshed
    public bool cannotBeRefreshed;

    //need another special case
    public bool isFocusButton;
    public bool isPrayerButton;

    //variables to determine usable availabilities
    public bool canBeUsedOnOvermap;
    public bool canBeUsedWhenExploring;

    //note that this marks direct damage abilities for foes (dont flag for abilities which do conventional skillcheck etc)
    public bool attackCard;
    public bool defenseCard;

    //this should be flagged for each foe charging ability (direct damage as well as support abilities etc)
    public bool foeChargingCard;

    //should be flagged when foe forces hero to do instant skillcheck (not direct damage ability)
    public bool foeInstantSkillcheck;

    //for defense abilities & cooldown passives (defense abilities should have defense card also marked)
    public bool foePassiveCooldownCard;

    public bool combatConsumableCard;
    public bool canBeSold;
    public bool canBeUsedOnEncounter;

    //for v0.5.7. list of cards which share the same cooldown
    //the boolean keeps track when conflicting card is active (in the effects holder)
    public bool unusableIfSharedCardActive;
    public bool enabledIfSharedCardActivated;
    public List<int> sharedCooldownCards;

    //keeps track of which player the card belongs to
    //actually could use this for all cards, cause of the stealing mechanics
    public int belongsTo;

    //the effect of the card
    public int effect;

    //value of the card in coins (only needed for items?)
    public int value;

    //skill upgrade variables
    public int skillPointCost;
    public int levelRequirement;

    //the lower the rarer (0-100)
    public int rarity;

    //1: warrior, 2: artisan, 3: arcanist, 4: cleric
    public int classRequirement;

    //could make variable to tell if this is a gainable upgrade
    public bool isPossibleUpgrade;

    //could make variable for skillpoints
    public bool isSkillPoint;

    //used for multi-level cards
    public int cardLevel;
    public bool isCardLevelUpgrade;

    //used when loading cards
    //index 0 is level 2 cardnumber, index 1 level 3 cardnumber (add more if needed)
    //this only needs to be set on the level 1 card, which can be upgraded
    //would be much simpler to simply replace the cards in the first place tho :-)
    public List<int> levelUpgradeCardNumbers;

    private void Start()
    {
        
    }
}

