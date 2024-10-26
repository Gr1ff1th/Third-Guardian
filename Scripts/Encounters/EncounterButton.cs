using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class EncounterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //text displayed on top of button
    [TextArea(5, 20)]
    public string buttonText;

    //tooltip text
    [TextArea(5, 20)]
    public string tooltipText;

    //added after tooltiptext via script
    public string additionText;

    //requirement types: 
    //0= energy, 1= attack, 2= defense, 3= arcane power, 4= resistance, 5= influence, 6= mechanics, 7= digging, 8= lore, 9= discovery, 
    //10= coins, 11= arcane dust, 12= upgrade point, 13= fame, 14= favor, 16= bombs, 17= action points
    //isSkillCheck tells what type of check it is (false if not skillcheck, true if it is skillscheck)
    //note that these are also used for foe special attacks now
    public int[] requirementType;
    public int[] requirementQty;
    public bool[] isSkillCheck;
    public bool[] isSkillCheckWithoutRoll;

    //if true, overrides the previous info
    public bool isCombat;

    //special requirement determine special requirement for certain encounter options
    public int specialRequirement;

    //used for getting appropriate info for combat button tooltips
    //0=not combat button, 1=default attack, 2=arcane attack, 3=defend button
    public int combatButtonType;

    //effect per chosen option:
    //0= energy, 1= arcane dust, 2= skill points, 4= coins, 5= fame, 6= favor, 16= bombs
    //separate list for when encounter fails
    //continue encounter tells which encounters the option can lead to (list is empty if it leads to none)
    public int[] effectType;
    public int[] effectQty;

    public int[] failEffectType;
    public int[] failEffectQty;

    //use these in case theres several possible outcomes
    public List<EncounterEffect> successEffectList;
    public List<EncounterEffect> successEffectList2;
    public List<EncounterEffect> failEffectList;

    public List<GameObject> continueEncounters;

    //shows specific button after specific result
    public List<GameObject> buttonsAfterSuccess;
    public List<GameObject> buttonsAfterFailure;
    //could be used to disable specific buttons, when returning to encounter options
    //also need variable to tell whether this button is exhausted
    //public int[] buttonsToExhaust;
    //public bool isExhausted;
    //public bool exhaustedAtStart;

    //used for undying, and maybe second phase on some boss battle?
    public GameObject secondPhase;

    //descriptions after the choice has been taken (or after check is finished, if there is an actual combat or skillcheck)
    //use endtext2 if check / combat fails
    [TextArea(5, 20)]
    public string endText1;
    [TextArea(5, 20)]
    public string endText2;

    //end icons (in case there is special icon, 0 if default)
    public Sprite successIcon;
    public Sprite successIconNight;
    public Sprite failIcon;
    public Sprite failIconNight;

    //played after the effect takes place (if its not random effect, and there is effect given here)
    public AudioClip successSfx;
    public AudioClip failSfx;

    //this only gets used if the button is a destination button
    public int destinationNumber;

    //keeps track of which button this is for current encounter
    //used by the store button method
    //the number is increased only if a button passes availability check (so unavailable buttons dont count)
    public int buttonNumber;

    //this is only refreshed by the initial encounter draw, used for exhausting certain buttons mid-encounter
    //note that only buttons tied to original encounter should be exhaustible this way
    public int originalButtonNumber;

    //set this to true for first continue button on destination
    public bool isDestinationButton;

    //changes difficulty of the whole destination, if not 0
    public int destinationDifficultyModifier;

    //if true, allows explore more on next encounter (if there is one)
    //public bool allowExploreMoreOnNext;

    //makes current encounter "taken" on successful check (if there is any)
    //also checks this on all continue encounter buttons 
    //dont think we need this anymore in v.1.0.0. (previously was used in explorehandler lines 2701 and 5186)
    //public bool exhaustCurrentEncounterOnSuccess;

    //if true, exhausts interdule encounter on success
    //also checks this on all continue encounter buttons 
    //public bool exhaustInterludeOnSuccess;

    //opens certain destination option for first explore dialog if the value is other than 0
    //note that we shouldnt change the location button list
    //public int openDestinationOnSuccess;

    //special effects are similar to those as previous quests?
    //used by progressive skillcheck to determine progressive reward type
    public int specialEffectOnSuccess;
    public int specialEffectOnFail;

    //need to do this this way, since id rather not change the special effects to array at this point
    //overrides forced encounter checks depending on outcome of the skillcheck
    public bool removeForcedOnSuccess;
    public bool removeForcedOnFailure;

    //if true, subsequent encounters wont allow explore more (untill continue exploring is used, or new explore action is used on overmap)
    //note that this is now solely called from the continue encounter button (so buttons without continue encounter method wont be taken into account)
    public bool dontAllowExploreMore;

    //if true, the button doesnt show at start (used for buttons at first location dialog)
    public bool isExhaustedAtStart;

    //keeps track of whether this button is exhausted (same as taken in encounter2 class, but for buttons)
    public bool isTaken;

    //tells whether this buttons gets taken after first chosen (for the duration of the encounter)
    //note that this only works for buttons directly in the original encounters list
    public bool getsTakenTemporarily;

    //for cases when button is alrdy chosen in the encounter once
    public bool isTakenTemporarily;

    //tells whether this buttons gets taken after first chosen (for the duration the minimap is loaded)
    //note that this only works for buttons directly in the original encounters list
    //also, once this is triggered, all buttons in this encounter with this flag will be exhausted while player stays on the same minimap instance
    public bool getsTakenPermanently;

    //for cases when button is taken for the duration you stay on the location (dont need anymore)
    //public bool isTakenPermanently;

    //tells whether this goes to purchase display
    public bool isPurchaseButton;

    //tells whether this goes to sell display
    public bool isSellButton;

    //new flag variables for v90
    public bool removeEncounterOnSuccess;
    public bool removeEncounterOnFailure;

    //use this for fleeing foes (on pre-battle, such as stealing foes etc)
    public bool foeDefeatedOnCheckFailure;

    //set true if foe special attack uses foes default attack values
    public bool useDefaultAttackValuesFS;

    //set true, if its detect trap check
    public bool isDetectTrapCheck;

    //set true, if its detect trap check
    //this also shows gravestone
    public bool isTurnUnholyCheck;

    //show gravestone when check succeeds (when skillcheck kills foe)
    public bool showGraveOnSuccess;

    //set true, if its fightbutton
    //needed for spawning cmobat mode button in v93
    public bool isFightButton;

    public int teleportLocation;

    //if this quest have been taken or solved before, this button wont show
    public int questAvailable;

    //needs certain quest to be solved to show this button
    public int questSolvedRequirement;

    //needs certain quest to not be solved to show this button
    //used for certain quest item returns
    public int questNotSolvedRequirement;

    //needs certain quest to be taken to show this button
    public int questTakenRequirement;

    //add number here, if doing this action flags the quest objective done
    //needed only for quests which doesnt require item return
    public int questObjectiveDone;

    //use this for returning non-item requirement quests
    public int questObjectiveDoneRequirement;

    //use this to hide quest give options, when the objective is alrdy done
    public int questObjectiveNotDoneRequirement;

    //needs certain equipment to be owned to show this button (1 is enough)
    public int requiredEquipment;

    //needs specific consumable (could actually replace many of the old special requirements with this)
    public int requiredConsumable;

    //if true, this button is always chosen, if automation is on, all buttons are also disabled
    public bool isChosenAutomatically;

    //show large icon in smoe cases
    public bool showLargeIcon;

    //automates foes explosive death (also dont show gravestone untill its calculated)
    //actually maybe better to do this with foe special ability
    //we can repurpose this for explosive effect for sabotage etc in v0.7.0. ?
    public bool isExplosiveDeathButton;

    //for new type of skillchecks in v0.6.0.
    public bool isHitDependantSkillcheck;

    //roll is done instantly (without dicerolling phase) when this is selected
    //public bool isInstantSkillCheck;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameManager.ins.toolTipBackground.SetActive(true);
        GameManager.ins.toolTipText.text = tooltipText.ToString(); 

        if(additionText != "")
        {
            GameManager.ins.toolTipText.text += additionText.ToString();
        }
    }

    //
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GameManager.ins.toolTipText.text = "";
        GameManager.ins.toolTipBackground.SetActive(false);
    }

    //add requirement tests here
    //returns true, if button will be shown
    public bool ButtonAvailability()
    {
        //dont show exhausted buttons (removed due to previous action)
        if (isTaken == true)
        {
            return false;
        }

        //dont show exhausted buttons (removed due to previous action)
        if (isTakenTemporarily == true)
        {
            return false;
        }

        //dont show exhausted buttons (removed due to previous action)
        if (getsTakenPermanently == true && GameManager.ins.references.currentStrategicEncounter.removeExhaustableButtons[originalButtonNumber] == true)
        {
            return false;
        }

        //show only this button when getting defeated by initial foe ability
        if (specialRequirement == 40)
        {
            //returns false if hero not KO'd
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
            {
                return true;
            }
            return false;
        }

        //explore more button
        if (specialRequirement == 1)
        {
            //dont show this, if hero is dead
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead)
            {
                return false;
            }

            //cannot explore more if this flag is checked
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().dontAllowExploreMore == true)
            {
                return false;
            }

            //returns false if explore cost is more than 2
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().exploreCost > 2)
            {
                return false;
            }
            //returns false if player has less energy than explore cost
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().exploreCost)
            {
                return false;
            }
            //stealth case
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().HasPassiveTest(9) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
            {
                return true;
            }
            //foresight case
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().HasPassiveTest(6) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter == true)
            {
                return true;
            }
            //return false if its forced encounter (need to add scouting & premonition check here later)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().forcedEncounterCancelled == false)
            {
                return false;
            }
        }

        //reroll button
        if (specialRequirement == 2)
        {
            Debug.Log("reroll cost is: " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);

            //returns false if explore cost is more than 2
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost > 2)
            {
                return false;
            }
            //special case for priestess (guidance)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
                {
                    return false;
                }
            }

            //returns false if player has less favor than explore cost
            //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) == false)
            else
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
                {
                    return false;
                }
            }
        }
        //continue exploring button
        if (specialRequirement == 3)
        {
            //dont show this, if hero is dead
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead)
            {
                return false;
            }

            //returns false if player has less than 2 action points left (the action points get reduced after the encounter is done)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().actionPoints < 2)
            {
                return false;
            }
            //return false if explore more button is available
            //note that continue explore button should be after explore more button (if both are on the list)
            if (CheckButtonHolder(1) == true)
            {
                return false;
            }

            /*returns false if player has less energy than interaction cost of the location
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.interactCost)
            {
                return false;
            }
            */
            //stealth case
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().HasPassiveTest(9) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
            {
                return true;
            }
            //foresight case
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().HasPassiveTest(6) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter == true)
            {
                return true;
            }
            //return false if its forced encounter (need to add scouting & premonition check here later)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().forcedEncounterCancelled == false)
            {
                return false;
            }
        }

        //fleebutton (dont show if hero is dead)
        if (specialRequirement == 5)
        {
            /* dont need this check for v90?
             * only check at encounter phase (not combat or defense phase)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter == true && CardHandler.ins.phaseNumber == 2)
            {
                //stealth
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(9) == true &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
                {
                    return false;
                }
                //foresight
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(6) == true &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter == true)
                {
                    return false;
                }
            }
            */

            //returns false if hero is dead, or ensnared
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 171, 7) > 0 ||
                CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 176, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 182, 7) > 0 ||
                CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 185, 7) > 0)
            {
                return false;
            }
        }

        //finish after foe defeated button
        if (specialRequirement == 6)
        {
            //returns false if opponent is not defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == false)
            {
                return false;
            }
        }

        //continue after basic attack button
        if (specialRequirement == 7)
        {
            //returns false if opponent is not defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true)
            {
                return false;
            }
        }

        //finish after getting defeated in battle button
        if (specialRequirement == 8)
        {
            //returns false if hero not KO'd
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == false)
            {
                return false;
            }
        }

        //continue after defense button
        if (specialRequirement == 9)
        {
            //returns false if hero KO'd
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == true)
            {
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
                return false;
            }
        }
        //arcane attack button
        if (specialRequirement == 10)
        {
            //cant use arcane attack with 1 energy
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < 1 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == false)
            {
                return false;
            }
        }
        //arcanist training check
        if (specialRequirement == 11)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == false)
            {
                return false;
            }
        }
        //leave from forced encounter (traps, combat etc)
        if (specialRequirement == 12)
        {
            /* dont think we need this for v90
            //stealth
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(9) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
            {
                return true;
            }
            //foresight
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(6) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isForcedEncounter == true)
            {
                return true;
            }
            */
            //else return false
            return false;

        }

        //skinning test
        if (specialRequirement == 13)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(26) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skinningDone == false)
            {
                return true;
            }
            return false;
        }

        //trophy hunter test
        if (specialRequirement == 14)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(27) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().trophyHuntingDone == false)
            {
                return true;
            }
            return false;
        }

        //charm animal test
        if (specialRequirement == 15)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(28) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().charmAnimalDone == false)
            {
                return true;
            }
            return false;
        }

        //charm test
        if (specialRequirement == 16)
        {
            //returns false if hero is dead
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
            {
                return false;
            }
            //check for charm ability or ring of charming
            if ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(19) == true || CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 3, 60)) &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().charmDone == false)
            {
                return true;
            }
            return false;
        }

        //disenchant test
        if (specialRequirement == 17)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().disenchantDone == false)
            {
                return true;
            }
            return false;
        }

        //chosen one test
        if (specialRequirement == 18)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(4) == false)
            {
                return false;
            }
        }

        //warrior test
        if (specialRequirement == 19)
        {
            //dont show this, if hero is dead
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead)
            {
                return false;
            }

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(1) == false)
            {
                return false;
            }
        }

        //artisan test
        if (specialRequirement == 20)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(2) == false)
            {
                return false;
            }
        }

        //lore upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 21)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades > 0)
            {
                return false;
            }
        }

        //lore upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 22)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades == 2)
            {
                return false;
            }
        }

        //strength upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 23)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades > 0)
            {
                return false;
            }
        }

        //strength upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 24)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades == 2)
            {
                return false;
            }
        }

        //defense upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 25)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades > 0)
            {
                return false;
            }
        }

        //defense upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 26)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades == 2)
            {
                return false;
            }
        }
        //mechanics upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 27)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades > 0)
            {
                return false;
            }
        }

        //mechanics upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 28)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades == 2)
            {
                return false;
            }
        }

        //AP upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 29)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades > 0)
            {
                return false;
            }
        }

        //AP upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 30)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades == 2)
            {
                return false;
            }
        }

        //resistance upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 31)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades > 0)
            {
                return false;
            }
        }

        //resistance upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 32)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades == 2)
            {
                return false;
            }
        }

        //influence upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 33)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades > 0)
            {
                return false;
            }
        }

        //influence upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 34)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades == 2)
            {
                return false;
            }
        }

        //digging upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 35)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades > 0)
            {
                return false;
            }
        }

        //digging upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 36)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades == 2)
            {
                return false;
            }
        }

        //discovery upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (specialRequirement == 37)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades > 0)
            {
                return false;
            }
        }

        //discovery upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (specialRequirement == 38)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades == 2)
            {
                return false;
            }
        }

        //sneak past button
        if (specialRequirement == 39)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
            {
                //returns false, if any foe is alert
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(27))
                {
                    return false;
                }
                //stealth
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(9) == true)
                {
                    return true;
                }
                //quirons armor
                if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 2, 220) == true)
                {
                    return true;
                }
                return false;
            }
        }


        //could use this for certain fight buttons (if hero can die from initial check at least)
        if (specialRequirement == 41)
        {
            //returns false if hero is dead
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
            {
                return false;
            }
        }

        //mounted check
        if (specialRequirement == 42)
        {
            if (CardHandler.ins.mountSlot.transform.childCount > 0)
            {
                return true;
            }
            return false;
        }

        //bless check (skill)
        //actually these shouldnt have true returns mostly, messes with the logic
        if (specialRequirement == 43)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 29, 1) == 0)
            {
                return false;
            }
        }

        //heal check (skill)
        if (specialRequirement == 44)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 24, 1) > 0)
            {
                return true;
            }
            return false;
        }

        //cleric
        if (specialRequirement == 45)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(42) == false)
            {
                return false;
            }
        }

        //toxicology check
        if (specialRequirement == 46)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 155, 2) > 0)
            {
                return true;
            }
            return false;
        }

        //clairvoyance check
        if (specialRequirement == 47)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 5, 1) == 0)
            {
                return false;
            }
        }

        //arcane power allowed check (see if player is either arcanist, or is wielding staff)
        if (specialRequirement == 48)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == false &&
                CardHandler.ins.StaffCheck() == false)
            {
                return false;
            }
        }

        //checks whether youre cursed or not
        if (specialRequirement == 49)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 28, 7) == 0)
            {
                return false;
            }
        }

        //meal check
        if (specialRequirement == 50)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 20, 1) == 0)
            {
                return false;
            }
        }

        /* lets do this with required equipment check
         * 
         * brooch of guliman check
        if (specialRequirement == 53)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 192, 5) > 0)
            {
                return true;
            }

            //can use this here?
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 6, 192) == true) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        */

        //cornville portal check
        //inn in v99
        if (specialRequirement == 54)
        {
            if (GameManager.ins.references.foundWaypoints[0] == 0)
            {
                return false;
            }
        }

        //coven portal check
        //faewood in v99
        if (specialRequirement == 55)
        {
            if (GameManager.ins.references.foundWaypoints[1] == 0)
            {
                return false;
            }
        }

        //wilforge portal check
        //blue citadel in v99
        if (specialRequirement == 56)
        {
            if (GameManager.ins.references.foundWaypoints[2] == 0)
            {
                return false;
            }
        }
        /*
        //temple portal check
        if (specialRequirement == 57)
        {
            if (GameManager.ins.references.foundWaypoints[3] == 0)
            {
                return false;
            }
        }
        */

        //citadel portal check
        //temple in v99
        if (specialRequirement == 58)
        {
            if (GameManager.ins.references.foundWaypoints[3] == 0)
            {
                return false;
            }
        }

        //underworld portal check
        //stays same in v99
        if (specialRequirement == 60)
        {
            if (GameManager.ins.references.foundWaypoints[4] == 0)
            {
                return false;
            }
        }

        //shield of isolore check
        if (specialRequirement == 61)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 111, 2) == 0)
            {
                return false;
            }
        }

        //don’t show if have clairvoyance
        if (specialRequirement == 62)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 5, 1) > 0)
            {
                return false;
            }
        }

        //hexwood req.
        if (specialRequirement == 64)
        {
            if (GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().gameData.randomTransits[0] != 1)
            {
                return false;
            }
        }

        //hidden grove req.
        if (specialRequirement == 65)
        {
            if (GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().gameData.randomTransits[0] != 0)
            {
                return false;
            }
        }
        //forest fort req.
        if (specialRequirement == 66)
        {
            if (GameObject.Find("DataPersistance").GetComponent<DataPersistenceManager>().gameData.randomTransits[7] != 0)
            {
                return false;
            }
        }

        //special checks for quests in progress
        //returns false if quest taken, or solved?
        if (questAvailable != 0)
        {
            if (GameManager.ins.specialVariables.CheckIfQuestTaken(questAvailable) == true)
            {
                return false;
            }
        }

        //special checks for quests in progress
        //returns false, if requirement not met
        if (questSolvedRequirement != 0)
        {
            if (GameManager.ins.specialVariables.CheckIfQuestCompleted(questSolvedRequirement) == false)
            {
                return false;
            }
        }

        //for quests that can only be done once
        //returns false, if quest alrdy done once
        if (questNotSolvedRequirement != 0)
        {
            if (GameManager.ins.specialVariables.CheckIfQuestCompleted(questNotSolvedRequirement) == true)
            {
                return false;
            }
        }

        //special checks for quests in progress
        //returns false if quest taken, or solved?
        if (questTakenRequirement != 0)
        {
            if (GameManager.ins.specialVariables.CheckIfQuestTaken(questTakenRequirement) == false)
            {
                return false;
            }
        }

        //special checks for quest objectives done (this one is used to hide quest give button for cases where the objective is done alrdy)
        //these quest returns dont need item to complete, only the objective
        if (questObjectiveNotDoneRequirement != 0)
        {
            if (GameManager.ins.specialVariables.CheckIfQuestObjectiveCompleted(questObjectiveNotDoneRequirement) == true)
            {
                return false;
            }
        }

        //special checks for quest objectives done
        //these quest returns dont need item to complete, only the objective
        if (questObjectiveDoneRequirement != 0)
        {
            if (GameManager.ins.specialVariables.CheckIfQuestObjectiveCompleted(questObjectiveDoneRequirement) == false)
            {
                return false;
            }
        }

        //required equipment check (returns true, if you have the required item)
        //checks both equipment holder & all item slots
        //this should be last, because of the true statements?
        if (requiredEquipment != 0)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, requiredEquipment, 5) > 0)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 2, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 3, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 4, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 6, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 8, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 9, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 10, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 11, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 12, requiredEquipment) == true)
            {
                return true;
            }
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 13, requiredEquipment) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //for singular consumable checks since v94
        if (requiredConsumable != 0)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, requiredConsumable, 1) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    //tests if you have the resources to allow enabling button
    public bool ButtonEnable()
    {
        //3 keystone check
        if (specialRequirement == 59)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 186, 5) < 3)
            {
                return false;
            }
        }

        //nabamax lesson (modify tooltip & button values)
        if (specialRequirement == 67)
        {
            requirementQty[0] = GameManager.ins.references.nabamaxLessonCost;

            tooltipText = "Ask for Lesson.<br>Costs " + GameManager.ins.references.nabamaxLessonCost + "<sprite index=13>.<br>Gain 3<sprite=\"sprites v88\" index=22>.";
            
            effectQty[1] = -GameManager.ins.references.nabamaxLessonCost;

            /* actually this test will be done anyway
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < GameManager.ins.references.nabamaxLessonCost)
            {
                return false;
            }
            */
        }

        //other resource test (dont need special requirement?)
        for (int i = 0; i < requirementType.Length; i++)
        {
            //special case for skillchecks without rolls
            if (requirementType[i] > 0 && requirementType[i] < 10)
            {
                if (isSkillCheckWithoutRoll.Length > 0)
                {
                    if (isSkillCheckWithoutRoll[i] == true)
                    {
                        if (requirementType[i] == 1 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength)
                        {
                            return false;
                        }
                        if (requirementType[i] == 2 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense)
                        {
                            return false;
                        }
                        if (requirementType[i] == 3 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower)
                        {
                            return false;
                        }
                        if (requirementType[i] == 4 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance)
                        {
                            return false;
                        }
                        if (requirementType[i] == 5 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence)
                        {
                            return false;
                        }
                        if (requirementType[i] == 6 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics)
                        {
                            return false;
                        }
                        if (requirementType[i] == 7 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging)
                        {
                            return false;
                        }
                        if (requirementType[i] == 8 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore)
                        {
                            return false;
                        }
                        if (requirementType[i] == 9 && requirementQty[i] > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe)
                        {
                            return false;
                        }
                    }
                }
            }
            //energy test
            if (requirementType[i] == 0)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < requirementQty[i])
                {
                    return false;
                }
            }

            //coin test
            if (requirementType[i] == 10)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < requirementQty[i])
                {
                    return false;
                }
            }

            //arcane dust test
            if (requirementType[i] == 11)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().dust < requirementQty[i])
                {
                    return false;
                }
            }

            //upgrade point test
            if (requirementType[i] == 12)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().skillPoints < requirementQty[i])
                {
                    return false;
                }
            }

            //favor test (added in v0.5.6.)
            //note that we dont need to add favor requirement for all instances that cost favor
            //we want it for prayer checks tho
            if (requirementType[i] == 14)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < requirementQty[i])
                {
                    return false;
                }
            }

            // bomb test
            // its weird, but we could leave this here for now?
            if (requirementType[i] == 16)
            {
                if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) < requirementQty[i])
                {
                    return false;
                }
            }
            //dont need to test fame or favor?
        }

        return true;
    }

    //update this according to skillcheck chance
    //only need updating when theres actual skillcheck
    public void UpdateTooltip()
    {
        //explore more button
        if (specialRequirement == 1)
        {
            tooltipText = "Draws new encounter for the cost of " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().exploreCost + "<sprite index=11>";
        }

        //reroll button (for skillchecks)
        if (specialRequirement == 2)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
            {
                tooltipText = "Rerolls the skillcheck for " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost + "<sprite index=11>";
            }
            else
            {
                tooltipText = "Rerolls the skillcheck for " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost + "<sprite index=12>";
            }
        }

        //continue exploring button
        if (specialRequirement == 3)
        {
            tooltipText = "Move some distance to find new things. (Costs <sprite index=32>)";//the cost of " + GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.interactCost + "<sprite index=11> and <sprite index=32>";
        }

        //destination encounter button (shows number of unexplored encounters on that location)
        //unused on v91?
        if (specialRequirement == 4)
        {
            tooltipText += "\n<size=4> </size>\n<color=#00fffc>(" + CountAvailableEncounters() + " available encounters)</color>";
        }

        //flee button (update later when we got swift opponents)
        if (specialRequirement == 5)
        {
            //if player has stealth, and theres no alert foes
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(8) == true &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(27) == false)
            {
                tooltipText = "Flee from battle for no cost.";
            }
            else
            {
                //special case if theres any opponent with swiftness, and hero isnt mounted
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(2) &&
                    CardHandler.ins.mountSlot.transform.childCount == 0)
                {
                    tooltipText = "Flee from battle for the cost of <sprite index=32> and 2<sprite index=11>. Costs <sprite=\"sprites v92\" index=3> when <sprite index=11> not available.";
                }
                //special case if all foes are slow
                else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckIfAllFoesHaveSpecificAbility(35))
                {
                    tooltipText = "Flee from battle for the cost of <sprite index=32> and 0<sprite index=11>";
                }
                else
                {
                    tooltipText = "Flee from battle for the cost of <sprite index=32> and 1<sprite index=11>. Costs <sprite=\"sprites v92\" index=3> when <sprite index=11> not available.";
                }
            }
        }

        //new focus system for v92
        if (specialRequirement == 51)
        {
            tooltipText = "Increase the minimum score of your next skillcheck. Costs " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost + "<sprite index=11>.";

            if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Focus <sprite=\"sprites v88\" index=12>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Focus <sprite=\"sprites v88\" index=10>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Focus <sprite=\"sprites v88\" index=13>";
                tooltipText = "Max focus.";

                gameObject.GetComponent<Button>().interactable = false;
            }
            //maybe not use the encounter button values for this (is less of a hassle)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
            {
                gameObject.GetComponent<Button>().interactable = false;
            }
        }

        //new "reroll" system for v95
        if (specialRequirement == 63)
        {
            tooltipText = "Increase the amount of dice rolled in your next skillcheck.<br>" +
                "Costs " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost + " <sprite index=12>.";

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 1)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Prayer <sprite=\"sprites v88\" index=17>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 2)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Prayer <sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Prayer <sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
                tooltipText = "Max prayer.";

                gameObject.GetComponent<Button>().interactable = false;
            }
            //maybe not use the encounter button values for this (is less of a hassle)
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
            {
                gameObject.GetComponent<Button>().interactable = false;
            }
        }

        //combat mode for v93
        if (specialRequirement == 52)
        {
            if (GameManager.ins.references.targettingHandler.targettingEnabled == false)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Classic";
                //tooltipText = "Increase the minimum score of your next skillcheck. Costs " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost + "<sprite index=11>.";
            }
            if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
            {
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Timed";
            }
        }

        if (requirementType.Length != 0)
        {
            if (requirementType[0] > 0 && requirementType[0] < 10)
            {
                //need new text holder for adding changing text field?
                additionText = "\n<size=4> </size>\n<color=#00fffc>(";
                additionText += SkillCheckChance(requirementType[0], requirementQty[0]) + ")";
            }
        }

        if (combatButtonType != 0)
        {
            tooltipText += "\n<size=4> </size>\n<color=#00fffc>(";
            tooltipText += CombatButtonTooltip() + ")";
        }
    }

    public void UpdateButtonText()
    {
        if(buttonText != "")
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        }
    }

    //counts number of available encounters for that destination
    public int CountAvailableEncounters()
    {
        //int totalNumberOfEncounters = continueEncounters[0].GetComponent<Encounter2>().buttons[0].GetComponent<EncounterButton>().continueEncounters.Count;

        //take reference to the appropriate encounter
        EncounterButton encounterButtonToCheck = continueEncounters[0].GetComponent<Encounter2>().buttons[0].GetComponent<EncounterButton>();

        int totalNumberOfEncounters = encounterButtonToCheck.continueEncounters.Count;

        //Debug.Log("number of encounters found: " + totalNumberOfEncounters);

        int encountersFound = 0;

        int modifiedGameStage = continueEncounters[0].GetComponent<Encounter2>().buttons[0].GetComponent<EncounterButton>().destinationDifficultyModifier + GameManager.ins.gameStage;

        for (int i = 0; i < totalNumberOfEncounters; i++)
        {
            //check for each of these encounters if they are available for current phase, and its not exhausted
            if (modifiedGameStage == 1 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage1Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
            if (modifiedGameStage == 2 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage2Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
            if (modifiedGameStage == 3 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage3Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
            if (modifiedGameStage == 4 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage4Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
            if (modifiedGameStage == 5 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage5Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
            if (modifiedGameStage == 6 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage6Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
            if (modifiedGameStage == 7 && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().stage7Rarity > 0
                && encounterButtonToCheck.continueEncounters[i].GetComponent<Encounter2>().isTaken == false)
            {
                encountersFound += 1;
            }
        }

        //reduce 1 for the sake of nothing encounter
        encountersFound -= 1;

        if (encountersFound > 0)
        {
            return encountersFound;
        }
        else
        {
            return 0;
        }
    }

    //new for v92
    public void FocusButton()
    {
        CardHandler.ins.extraSfxHolder.clip = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[8].GetComponent<CardDisplay2>().sfx;
        CardHandler.ins.extraSfxHolder.Play();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost += 1;

        //making button un-interactable also happens here
        UpdateTooltip();

        //also update the tooltips for skillcheck buttons
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().UpdateSkillCheckTooltips();
    }

    //new for v95
    public void PrayerButton()
    {
        CardHandler.ins.extraSfxHolder.clip = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[8].GetComponent<CardDisplay2>().sfx;
        CardHandler.ins.extraSfxHolder.Play();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

        //making button un-interactable also happens here
        UpdateTooltip();

        //also update the tooltips for skillcheck buttons
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().UpdateSkillCheckTooltips();
    }

    //new for v93
    public void CombatModeButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            GameManager.ins.references.targettingHandler.targettingEnabled = false;
        }
        else if (GameManager.ins.references.targettingHandler.targettingEnabled == false)
        {
            GameManager.ins.references.targettingHandler.targettingEnabled = true;
        }

        //making button un-interactable also happens here
        UpdateTooltip();
    }

    //leave button actually (for explore encounters)
    public void DoneButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().allFoesDefeated == true)
        {
            if(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn < GameManager.ins.exploreHandler.GetComponent<MultiCombat>().originalNumberOfFoes)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = true;
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference = true;

                //need to reset these too
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skinningDone = false;
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().trophyHuntingDone = false;

                GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
            }
            else
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().allFoesDefeated = false;
                GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
            }
        }
        else
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
        }
    }

    //leave button actually (for normal interactions or whn cancelling explore on the first dialog)
    //No AP cost, no music change
    public void DoneButton2()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs ap no AP
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore7", RpcTarget.AllBufferedViaServer, false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
    }

    //leave button actually (for normal interactions or whn cancelling explore on the first dialog)
    //costs AP, no music change
    public void DoneButton3()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs ap
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore7", RpcTarget.AllBufferedViaServer, true);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
    }

    //cancel button actually (regain portal scroll)
    public void DoneButtonWithPortalScroll()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 189, 1, 1);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
    }

    //leave button actually (for explore encounters)
    public void WhirlpoolButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //check divers mask or sentinel
        if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 1, 193) == false &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
        {
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -999);
        }
        else
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().WhirlpoolContinueWithDelay();
            
        }

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
    }

    //new return button for v90 (returns to previous node)
    public void ReturnButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelAndReturn", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
    }

    //sneak past button for v90 (stay on node)
    public void SneakButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
    }


    //leave button actually
    public void FleeButton()
    {
        GameManager.ins.references.GetComponent<CombatActions>().FleeButton();

        /*
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //only reduce energy for non-swift characters
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(8) == false)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(2))
            {
                //cost 2 vs swift opponents
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -2);
            }
            else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(35))
            {
                //cost 0 vs slow opponents
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -2);
            }
            else
            {
                //costs 1 energy by default
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);
            }
        }

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_FleeFromCombat", RpcTarget.AllBufferedViaServer);

        //Invoke("FleeWithDelay", 0.4f);
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
        */
    }

    //not used
    void FleeWithDelay()
    {
    }

    //this should be unused?
    public void ExploreMoreButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs energy
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().exploreCost);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().exploreCost += 1;

        //need to do this elsewhere, since this button gets deleted
        //dont think we need this at all anymore (was at line 2495 in explorehandler)
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ExploreMoreButton();
    }

    //used for continue encounters
    public void ContinueEncounterButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueEncounterButton();
    }

    //used for continue encounters
    public void SpawnNewEncounterButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SpawnNewEncounterButton();
    }

    //used for continue encounters
    public void SpawnNewEncounterPermanently()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SpawnNewEncounterPermanently(specialEffectOnSuccess);
    }

    //used for second explore action inside the explore dialog
    //havent needed this in a while either
    public void ContinueExploringButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetDestinationEncounter();

        //"ends" turn, practically just removes 1 AP
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();

        //update character energy & interaction token
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

        //use a button from list, to get to destination encounter (be sure to keep location buttons and destination buttons at the same index), doesnt work this way
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().exploreLocationChoiceButtons[destinationNumber].GetComponent<Button>().onClick.Invoke();
    }

    public void RerollButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs energy for priestess, favor for others
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }
        else
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RerollButton();
    }

    public void ProgressiveRerollButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs energy for priestess, favor for others
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }
        else
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ProgressiveRerollButton();
    }

    public void ContinueFromSkillCheckButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
    }

    public void ContinueFromProgressiveSkillCheckButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromProgressiveSkillCheck();
    }

    //need to store the button number for skillchecks
    public void StoreButton()
    {
        //clears storage
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ClearStorage", RpcTarget.AllBufferedViaServer);

        //store the chosen button number
        //Invoke("StoreButtonAfterDelay", 0.1f);

        //this stores the button reference
        //additionally checks whether the button gets temporarily taken
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_StoreButtonNumber", RpcTarget.AllBufferedViaServer, buttonNumber, originalButtonNumber);
    }

    public void HitDependentSkillCheck()
    {
        //do 10 "dicerolls" before moving onward
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        CloseEnemyStatDisplay();

        GameManager.ins.toolTipBackground.SetActive(false);
        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        Diceroll(true);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().DisableEncounterButtons();
    }

    public void SkillCheck()
    {
        //do 10 "dicerolls" before moving onward
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        CloseEnemyStatDisplay();

        GameManager.ins.toolTipBackground.SetActive(false);
        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        Diceroll(false);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().DisableEncounterButtons();
    }

    //lets remove the "diceroll" phase for v0.5.7.
    void Diceroll(bool isHitDependent)
    {
        string skillCheckText = "";

        
        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            skillCheckText += "<br><br><br><br><br><br><br><br>";
        }

        skillCheckText += "<size=18>Skillcheck</size>\n<size=8>\n</size><color=#FFD370>";

        //if (GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        //{
        skillCheckText += "Requirement\n";

        for (int i = 0; i < requirementQty[0]; i++)
        {
           skillCheckText += GetSkillIcon(requirementType[0]);
        }

        skillCheckText += "\n<size=8>\n</size>";
        //}

        skillCheckText += "Rolling dice...";

        //int roll = Random.Range(1, 7);
        int roll = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);
        int roll2 = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);
        int roll3 = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);

        //skillCheckText += "Roll\n";
        //skillCheckText += GetDice(roll);

        //for v95
        int numberOfDice = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost;
        int bestRoll = roll;

        if (numberOfDice > 1)
        {
            if (roll2 > bestRoll)
            {
                bestRoll = roll2;
            }
        }
        if (numberOfDice > 2)
        {
            if (roll3 > bestRoll)
            {
                bestRoll = roll3;
            }
        }

        /*
        skillCheckText += "Roll\n";

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount != 11)
        {
            if (bestRoll == roll && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
            {
                skillCheckText += "<size=18>" + GetDice(roll) + "</size>";
            }
            else
            {
                skillCheckText += GetDice(roll);
            }
        }

        if (numberOfDice > 1 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount != 11)
        {
            if (bestRoll == roll2 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
            {
                skillCheckText += " <size=18>" + GetDice(roll2) + "</size>";
            }
            else
            {
                skillCheckText += " " + GetDice(roll2);
            }
        }

        if (numberOfDice > 2 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount != 11)
        {
            if (bestRoll == roll3 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
            {
                skillCheckText += " <size=18>" + GetDice(roll3) + "</size>";
            }
            else
            {
                skillCheckText += " " + GetDice(roll3);
            }
        }

        //show big dice for the best roll
        if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            skillCheckText += "<size=18>" + GetDice(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll) + "</size>";
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text = skillCheckText;

        if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount < 10)
        {
            Invoke(nameof(Diceroll), 0.12f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
        }
        else if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
        {
            Invoke(nameof(Diceroll), 0.8f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll = bestRoll;
            //ContinueSkillCheck(skillCheckText, bestRoll);
        }
        //this is kinda drunken way of doing this, but lets see if it works
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            ContinueSkillCheck(skillCheckText, GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll);
        }
        */
        ContinueSkillCheck(skillCheckText, bestRoll, isHitDependent);
    }

    void ContinueSkillCheck(string skillCheckText, int bestRoll, bool isHitDependent)
    {
        //for normal skillchecks
        int score = bestRoll + GetSkillPoints(requirementType[0]);
        int scoreBonus = 0;

        //skillCheckText += "\n<size=8>\n</size>Score\n";

        //special case for trap sense
        if (isDetectTrapCheck == true && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 170, 2) > 0)
        {
            scoreBonus += 2;
        }

        //special case for turn undead
        //checks holy power
        if (isTurnUnholyCheck == true && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 160, 2) > 0)
        {
            scoreBonus += 1;
        }

        if (isHitDependent == false)
        {
            score += scoreBonus;
        }

        if (isHitDependent == true)
        {
            int maxBonus = GetSkillPoints(requirementType[0]) + scoreBonus;

            if (bestRoll <= maxBonus)
            {
                maxBonus = bestRoll;
            }
            score = bestRoll + maxBonus;
        }

        bool isSuccess = false;

        //special cases
        if (bestRoll == 1)
        {
            //skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;
        }
        if (bestRoll == 6)
        {
            //skillCheckText += "<color=green>Critical</color>";

            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = true;

            //might as well give cosmic conection bonus here? (even if rerolled for some reason, you still shouldnt get profit)
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 164, 2) > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 1);

                //lets use the third sfx holder for this (the first two might get taken)
                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[164].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
            }
        }

        //other cases
        if (bestRoll > 1 && bestRoll < 6)
        {
            for (int i = 0; i < score; i++)
            {
                //skillCheckText += GetSkillIcon(requirementType[0]);
            }

            if (score >= requirementQty[0])
            {
                if (GameManager.ins.references.currentEncounter.isCombatEncounter == false)
                {
                    //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                }
                else
                {
                    //skillCheckText += "   <sprite=\"yes & no\" index=3>";
                }

                isSuccess = true;
            }
            else
            {
                if (GameManager.ins.references.currentEncounter.isCombatEncounter == false)
                {
                    //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
                }
                else
                {
                    //skillCheckText += "   <sprite=\"yes & no\" index=1>";
                }
                isSuccess = false;
            }
        }

        /*put this here, so that game wont bug when defeating explosive foes with skillcheck
        if (isSuccess == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true)
        {
            CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 242, 4, 1);
            CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 243, 4, 1);
        }
        */

        //handles skillcheck display 
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_SkillCheck", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SkillCheck(skillCheckText, isSuccess);

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, requirementType[0]);


        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text
    }

    public void ProgressiveSkillCheck()
    {

        //do 10 "dicerolls" before moving onward
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        CloseEnemyStatDisplay();

        GameManager.ins.toolTipBackground.SetActive(false);
        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        ProgressiveDiceroll();

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().DisableEncounterButtons();
    }

    void ProgressiveDiceroll()
    {
        string skillCheckText = "";

        if (GameManager.ins.references.currentEncounter.isCombatEncounter == true)
        {
            skillCheckText += "<br><br><br><br><br><br><br><br>";
        }

        skillCheckText += "<size=18>Progressive skillcheck</size>\n<size=8>\n</size><color=#FFD370>";

        //if (GameManager.ins.references.currentEncounter.isCombatEncounter == false)
        //{
        skillCheckText += "Minimum Requirement\n";

        for (int i = 0; i < requirementQty[0]; i++)
        {
            skillCheckText += GetSkillIcon(requirementType[0]);
        }

        skillCheckText += "\n<size=8>\n</size>";
        //}

        skillCheckText += "Rolling dice...";

        //int roll = Random.Range(1, 7);
        int roll = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);
        int roll2 = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);
        int roll3 = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);

        //skillCheckText += "Roll\n";
        //skillCheckText += GetDice(roll);

        //for v95
        int numberOfDice = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost;
        int bestRoll = roll;

        if (numberOfDice > 1)
        {
            if (roll2 > bestRoll)
            {
                bestRoll = roll2;
            }
        }
        if (numberOfDice > 2)
        {
            if (roll3 > bestRoll)
            {
                bestRoll = roll3;
            }
        }
        /*
        skillCheckText += "Roll\n";

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount != 11)
        {
            if (bestRoll == roll && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
            {
                skillCheckText += "<size=18>" + GetDice(roll) + "</size>";
            }
            else
            {
                skillCheckText += GetDice(roll);
            }
        }

        if (numberOfDice > 1 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount != 11)
        {
            if (bestRoll == roll2 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
            {
                skillCheckText += " <size=18>" + GetDice(roll2) + "</size>";
            }
            else
            {
                skillCheckText += " " + GetDice(roll2);
            }
        }

        if (numberOfDice > 2 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount != 11)
        {
            if (bestRoll == roll3 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
            {
                skillCheckText += " <size=18>" + GetDice(roll3) + "</size>";
            }
            else
            {
                skillCheckText += " " + GetDice(roll3);
            }
        }

        //show big dice for the best roll
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            skillCheckText += "<size=18>" + GetDice(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll) + "</size>";
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text = skillCheckText;

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount < 10)
        {
            Invoke(nameof(ProgressiveDiceroll), 0.14f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
        }
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
        {
            Invoke(nameof(ProgressiveDiceroll), 0.8f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll = bestRoll;
        }
        //this is kinda drunken way of doing this, but lets see if it works
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            ContinueProgressiveSkillCheck(skillCheckText, GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll);
        }
        */
        ContinueProgressiveSkillCheck(skillCheckText, bestRoll);
    }

    void ContinueProgressiveSkillCheck(string skillCheckText, int bestRoll)
    {

        int hits = bestRoll + GetSkillPoints(requirementType[0]) - requirementQty[0] + 1;

        if (hits < 1)
        {
            hits = 0;
        }

        //skillCheckText += "\n<size=8>\n</size>Hits\n";

        bool isSuccess = false;

        //special cases
        if (bestRoll == 1)
        {
            //skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;
        }

        //might as well give cosmic connection bonus here? (even if rerolled for some reason, you still shouldnt get profit)
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 164, 2) > 0 && bestRoll == 6)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 1);

            //lets use the third sfx holder for this (the first two might get taken)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[164].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //other cases
        if (bestRoll > 1)
        {
            for (int i = 0; i < hits; i++)
            {
                //skillCheckText += GetSkillIcon(requirementType[0]);
            }
            if (hits >= 1)
            {
                //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                isSuccess = true;
            }
            else
            {
                //skillCheckText += "<color=red>No hits</color>";
                isSuccess = false;
            }

        }

        //handles skillcheck display 
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ProgressiveSkillCheck", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess, hits);
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ProgressiveSkillCheck(skillCheckText, isSuccess, hits);

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, requirementType[0]);

    }

    public void NoSkillCheck()
    {
        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromNoSkillCheck();

        //might as well close stat display always when doing actions
        CloseEnemyStatDisplay();

        //Invoke("NoSkillCheckWithDelay", 0.4f);
    }

    //dunno if needed
    void NoSkillCheckWithDelay()
    {
        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromNoSkillCheck();
    }

    public string GetSkillIcon(int skillType)
    {
        if (skillType == 1)
        {
            return "<sprite index=4>";
        }
        if (skillType == 2)
        {
            return "<sprite=\"sprites v88\" index=18>";
        }
        if (skillType == 3)
        {
            return "<sprite index=9>";
        }
        if (skillType == 4)
        {
            return "<sprite=\"sprites v88\" index=19>";
        }
        if (skillType == 5)
        {
            return "<sprite=\"sprites v92\" index=0>";
        }
        if (skillType == 6)
        {
            return "<sprite index=1>";
        }
        if (skillType == 7)
        {
            return "<sprite index=7>";
        }
        if (skillType == 8)
        {
            return "<sprite index=5>";
        }
        if (skillType == 9)
        {
            return "<sprite=\"eye v88\" index=0>";
        }
        return "";
    }

    public string GetDice(int roll)
    {
        if(roll == 1)
        {
            return "<sprite=\"sprites v88\" index=12>";
        }
        if (roll == 2)
        {
            return "<sprite=\"sprites v88\" index=10>";
        }
        if (roll == 3)
        {
            return "<sprite=\"sprites v88\" index=13>";
        }
        if (roll == 4)
        {
            return "<sprite=\"sprites v88\" index=15>";
        }
        if (roll == 5)
        {
            return "<sprite=\"sprites v88\" index=14>";
        }
        if (roll == 6)
        {
            return "<sprite=\"sprites v88\" index=11>";
        }
        return "";
    }

    public int GetSkillPoints(int skillType)
    {
        int skillpoints = 0;

        if(skillType == 1)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength;
        }
        if (skillType == 2)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense;
        }
        if (skillType == 3)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower;
        }
        if (skillType == 4)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance;
        }
        if (skillType == 5)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence;
        }
        if (skillType == 6)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics;
        }
        if (skillType == 7)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging;
        }
        if (skillType == 8)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore;
        }
        if (skillType == 9)
        {
            skillpoints = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe;
        }

        //special cases
        if(skillpoints > 6)
        {
            return 6;
        }
        if (skillpoints < -3)
        {
            //remove this for v94
            return -3;
        }
        return skillpoints;
    }

    //gets the additional tooltip info for combat buttons
    //unused?
    public string CombatButtonTooltip()
    {
        string chance = "17% chance";

        //basic attack
        if (combatButtonType == 1)
        {
            chance = SkillCheckChance(1, GameManager.ins.references.currentStrategicEncounter.defense + 1);
            chance += " to hit";
        }

        //arcane attack
        if (combatButtonType == 2)
        {
            chance = SkillCheckChance(3, GameManager.ins.references.currentStrategicEncounter.resistance + 1); //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.
            chance += " to hit";
        }

        //defense button
        if (combatButtonType == 3)
        {
            Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();

            int attack = GameManager.ins.references.currentStrategicEncounter.attack;
            int arcanePower = GameManager.ins.references.currentStrategicEncounter.arcanePower;

            //lets use this for foe special damage attacks too
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().useDefaultAttackValuesFS == true)
                {
                    attack = GameManager.ins.references.currentStrategicEncounter.attack;
                    arcanePower = GameManager.ins.references.currentStrategicEncounter.arcanePower;

                }

                else
                {
                    //in case foe uses special physical attack
                    //need to use the long version, cause of the reroll function
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementType[0] == 1)
                    {
                        attack = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementQty[0];
                        //need to set this to 0, in case foes regular attack in arcane
                        arcanePower = 0;
                    }

                    //in case foe uses special arcane attack
                    if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementType[0] == 3)
                    {
                        arcanePower = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementQty[0];
                        attack = 0;
                    }
                }
            }

            if (arcanePower == 0)
            {
                chance = SkillCheckChance(2, attack);
                chance += " to dodge";
            }

            else if(attack == 0)
            {
                chance = SkillCheckChance(4, arcanePower);
                chance += " to dodge";
            }
        }

        return chance;
    }

    public string SkillCheckChance(int reqType, int reqQty)
    {
        float statValue = 0;

        if (reqType == 1)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strength;
        }
        if (reqType == 2)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defense;
        }
        if (reqType == 3)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePower;
        }
        if (reqType == 4)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistance;
        }
        if (reqType == 5)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence;
        }
        if (reqType == 6)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanics;
        }
        if (reqType == 7)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().digging;
        }
        if (reqType == 8)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().lore;

            //special case for turn undead
            //checks holy power
            if (isTurnUnholyCheck == true && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 160, 2) > 0)
            {
                statValue += 1;
            }
        }
        if (reqType == 9)
        {
            statValue = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe;

            //special case for trap sense
            if (isDetectTrapCheck == true && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 170, 2) > 0)
            {
                statValue += 2;
            }
        }

        float hitChance = 1f;

        if (isHitDependantSkillcheck == false)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
            {
                hitChance = statValue + 7 - reqQty;
                hitChance = hitChance / 6;

                if (hitChance > 0.83f)
                {
                    hitChance = 0.83f;
                }
                if (hitChance < 0.17f)
                {
                    hitChance = 0.17f;
                }
            }

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
            {
                hitChance = statValue + 7 - reqQty;
                hitChance = hitChance / 5;

                if (hitChance > 1)
                {
                    hitChance = 1;
                }
                if (hitChance < 0.2f)
                {
                    hitChance = 0.2f;
                }
            }

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
            {
                hitChance = statValue + 7 - reqQty;
                hitChance = hitChance / 4;

                if (hitChance > 1)
                {
                    hitChance = 1;
                }
                if (hitChance < 0.25f)
                {
                    hitChance = 0.25f;
                }
            }
        }

        //new skillchecks for v0.6.0.
        if(isHitDependantSkillcheck == true)
        {
            //Debug.Log("is hit dependant skillcheck");
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
            {
                //roll required
                float valueMissing = reqQty /2;
                if (reqQty > statValue * 2)
                {
                    valueMissing = reqQty - statValue;
                }

                if(valueMissing >= 6)
                {
                    valueMissing = 6;
                }

                float chanceToMiss = (valueMissing -1) / 6;
                hitChance -= chanceToMiss;

                if (hitChance < 0.17f)
                {
                    hitChance = 0.17f;
                }
            }

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
            {
                //roll required
                float valueMissing = reqQty / 2;
                if (reqQty > statValue * 2)
                {
                    valueMissing = reqQty - statValue;
                }
                if (valueMissing >= 6)
                {
                    valueMissing = 6;
                }

                float chanceToMiss = (valueMissing - 2) / 5;
                hitChance -= chanceToMiss;

                if (hitChance < 0.2f)
                {
                    hitChance = 0.2f;
                }
            }

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
            {
                //roll required
                float valueMissing = reqQty / 2;
                if (reqQty > statValue * 2)
                {
                    valueMissing = reqQty - statValue;
                }
                if (valueMissing >= 6)
                {
                    valueMissing = 6;
                }

                float chanceToMiss = (valueMissing - 3) / 4;
                hitChance -= chanceToMiss;

                if (hitChance < 0.25f)
                {
                    hitChance = 0.25f;
                }
            }
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 2)
        {
            float hitChanceNegation = 1-hitChance;
            hitChance = 1 - (hitChanceNegation * hitChanceNegation);
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3)
        {
            float hitChanceNegation = 1 - hitChance;
            hitChance = 1 - (hitChanceNegation * hitChanceNegation * hitChanceNegation);
        }

        //bring it to percentages
        hitChance = (int)(hitChance * 100);

        /* old (noob) system
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
        {
            if (statValue + 2 >= reqQty)
            {
                return "83% chance";
            }
            if (statValue + 3 >= reqQty)
            {
                return "67% chance";
            }
            if (statValue + 4 >= reqQty)
            {
                return "50% chance";
            }
            if (statValue + 5 >= reqQty)
            {
                return "33% chance";
            }
            return "17% chance";
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
        {
            if (statValue + 2 >= reqQty)
            {
                return "100% chance";
            }
            if (statValue + 3 >= reqQty)
            {
                return "80% chance";
            }
            if (statValue + 4 >= reqQty)
            {
                return "60% chance";
            }
            if (statValue + 5 >= reqQty)
            {
                return "40% chance";
            }
            return "20% chance";
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
        {
            if (statValue + 3 >= reqQty)
            {
                return "100% chance";
            }
            if (statValue + 4 >= reqQty)
            {
                return "75% chance";
            }
            if (statValue + 5 >= reqQty)
            {
                return "50% chance";
            }
            return "25% chance";
        }
        */

        //shouldnt go here at all now?
        //return "17% chance";
        return hitChance + "% chance";
    }

    //used when combat is first initiated
    public void CombatButton()
    {
        //kinda wanna close these
        CloseEnemyStatDisplay();

        GameManager.ins.toolTipBackground.SetActive(false);

        //store the button number separately

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1 = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.combatButton;
        }
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2 = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.combatButton;
        }
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3 = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.combatButton;
        }

        //go to combat if theres no more foes to check
        //could add initialphase check here, in case of second phase encounters?
        if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 2) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 3) ||
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().initialPhaseDone == true)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn = 1;

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().initialPhaseDone = true;

            //lets try swapping these for now
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1;

            //kinda need need second reference too to the actual instantiated strategic encounter unfortunately?
            GameManager.ins.references.currentStrategicEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic1;
            GameManager.ins.references.currentEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1;

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateIcons(1);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

            //display encounter icon
            //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().ShowIcon3(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.icon);

            //set foe icon here also
            //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SetFoeIcon(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.icon);

            //need to swap active foe abilities & stats too
            //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ResetFoeStats(1);
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ActivateFoeCards(1);

            //need to set this just in case (otherwise pause plate sometimes doesnt show at start)
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase = false;

            /* lets do this with a method instead
             * 
             * enemies strike first, if any of them has alert
            //and if hero doesnt have swift mount
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(27) == true &&
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 64) == false &&
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 67) == false &&
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 68) == false &&
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 69) == false)
            {
                //combat begins (opponents have alert)
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToRealTimeCombat(false);
            }
            else
            {
                //combat begins (opponents not alert)
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToRealTimeCombat(true);
            }
            */

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
        }

        //open options for secondary foe(s), if there are any
        else if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 2))
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;

            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                //lets try swapping these for now
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter2;

                //kinda need need second reference too to the actual instantiated strategic encounter unfortunately?
                GameManager.ins.references.currentStrategicEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic2;
                GameManager.ins.references.currentEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter2;

                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateIcons(2);
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ActivateFoeCards(2);
            }

            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                //lets try swapping these for now
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter3;

                //kinda need need second reference too to the actual instantiated strategic encounter unfortunately?
                GameManager.ins.references.currentStrategicEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic3;
                GameManager.ins.references.currentEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter3;

                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateIcons(3);
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ActivateFoeCards(3);
            }

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetStrategicEncounter(false);
        }
    }

    //almost same as second phase continue at explorehandler
    public void SecondWindContinue()
    {
        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().resurgingFoeNumber - 1];

        int spot = battlefieldFoe.spot;
        //battlefieldFoe.SetBattlefieldFoe(spot);
        battlefieldFoe.SetSecondPhaseFoe(spot);
        battlefieldFoe.SetHealthBarValues();
    }

    /*unused attack button?
    public void DefaultAttackButton()
    {
        //allow no cards
        CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //store the button number separately

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        string skillCheckText = "<size=18>Default Attack</size>\n<size=8>\n</size><color=#FFD370>";

        //skillCheckText += "Foe Defense\n";

        Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        for (int i = 0; i < encounter.defense; i++)
        {
            //get defense icons
            //skillCheckText += GetSkillIcon(2);
        }

        //skillCheckText += "\n<size=8>\n</size>Your Attack\n";

        for (int i = 0; i < character.strength && i < 6; i++)
        {
            //get defense icons
            //skillCheckText += GetSkillIcon(1);
        }

        int roll = Random.Range(1, 7);

        //skillCheckText += "\n<size=8>\n</size>Roll\n";
        skillCheckText += "Roll\n";
        skillCheckText += GetDice(roll);

        int hits = roll + GetSkillPoints(1) - encounter.defense;

        if (hits < 1)
        {
            hits = 0;
        }

        skillCheckText += "\n<size=8>\n</size>Damage Done\n";

        //this is only used for sfx here?
        bool isSuccess = false;

        //need to determine attack type for reroll button
        //type 1 is default attack
        int attackTypeUsed = 1;

        //special cases
        if (roll == 1)
        {
            skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;

            hits = 0;
        }
        
        //always give at least 1 hit on crit
        if (roll == 6 && hits == 0)
        {
            //skillCheckText += "<color=green>";

            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = true;

            hits = 1;
        }

        //check melee damage modifier (rounded up)
        float strModifiedFloat = Mathf.Ceil(hits * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier / 100f);
        hits = (int)strModifiedFloat;

        //checks special foe protection abilities
        //also cooldown abilities
        hits = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckFoeProtectionAbilities(hits, attackTypeUsed);

        //other cases
        if (roll > 1)
        {
            for (int i = 0; i < hits; i++)
            {
                //use energy icons hits
                skillCheckText += "<sprite index=11>";
            }

            if (hits >= 1)
            {
                //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                isSuccess = true;
            }
            else
            {
                skillCheckText += "<color=red>No damage</color>";
                isSuccess = false;
            }
        }

        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().PV.RPC("RPC_BasicAttack", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess, attackTypeUsed, hits);

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 1);

    }
    */

    /*unused arcane attack button?
    public void ArcaneAttackButton()
    {
        //allow no cards
        CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //store the button number separately

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        string skillCheckText = "<size=18>Arcane Attack</size>\n<size=8>\n</size><color=#FFD370>";

        //skillCheckText += "Foe Resistance\n";

        Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        for (int i = 0; i < encounter.resistance; i++)
        {
            //get defense icons
            //skillCheckText += GetSkillIcon(4);
        }

        //skillCheckText += "\n<size=8>\n</size>Your Arcane Power\n";

        for (int i = 0; i < character.arcanePower && i < 6; i++)
        {
            //get defense icons
            //skillCheckText += GetSkillIcon(3);
        }

        int roll = Random.Range(1, 7);

        skillCheckText += "Roll\n";
        skillCheckText += GetDice(roll);

        int hits = roll + GetSkillPoints(3) - encounter.resistance;

        if (hits < 1)
        {
            hits = 0;
        }

        skillCheckText += "\n<size=8>\n</size>Damage Done\n";

        //this is only used for sfx here?
        bool isSuccess = false;

        //need to determine attack type for reroll button
        //type 1 is default attack, type 2 is arcane attack
        int attackTypeUsed = 2;

        //special cases
        if (roll == 1)
        {
            skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;

            hits = 0;
        }

        //always give at least 1 hit on crit
        if (roll == 6 && hits == 0)
        {
            //skillCheckText += "<color=green>";

            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = true;

            hits = 1;
        }

        //check melee damage modifier (rounded up)
        float apModifiedFloat = Mathf.Ceil(hits * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier / 100f);
        hits = (int)apModifiedFloat;

        hits = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckFoeProtectionAbilities(hits, attackTypeUsed);

        //other cases
        if (roll > 1)
        {
            for (int i = 0; i < hits; i++)
            {
                //use energy icons hits
                skillCheckText += "<sprite index=11>";
            }

            if (hits >= 1)
            {
                //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                isSuccess = true;
            }
            else
            {
                skillCheckText += "<color=red>No damage</color>";
                isSuccess = false;
            }
        }

        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().PV.RPC("RPC_BasicAttack", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess, attackTypeUsed, hits);

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 3);

    }
    */

    //used by arcaneattack button at least (when initially activated)
    public void LoseEnergy()
    {
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);
    }

    public void RerollAttackButton()
    {
        //certain cards cant be used when paused
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused == true && GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Button1.clip;
            CardHandler.ins.intelligenceSfxHolder.Play();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CombatPauseButton();
            return;
        }

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs energy for priestess, favor for others
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }
        else
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().RerollAttackButton();

        //add new targetting system here
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            GameManager.ins.references.targettingHandler.TakeBallScore();
        }

        gameObject.SetActive(false);
    }

    public void ContinueFromBasicAttackCheckButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //put these just in case
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        //this also now includes stuff like regeneration
        //also hero energy drain
        //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckHitTakenAbilities(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone);

        //reduce special ability etc cooldowns
        //reduces active effect timers
        //returns true, if hero was defeated (by immolation or such), or when extra strike / time warp was used
        /*
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().OncePerRoundHeroEffects() == true)
        {
            return;
        }
        */

        //checks foe hit taken and hit done abilities, hero takes damage per hitstaken
        //if true, hero or foe was defeated
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingDefensePhase() == true)
        {
            return;
        }

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
    }

    //used against default attacks of the foe (both physical & magical)
    /*is defend button used here? (remove for now)
    public void DefaultDefendButton()
    {
        //allow no cards
        CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //store the button number separately

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        string skillCheckText = "<size=18>Defend</size>\n<size=8>\n</size><color=#FFD370>";

        //skillCheckText += "Foe Attack\n";

        Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        int attack = encounter.attack;
        int arcanePower = encounter.arcanePower;

        //lets use this for foe special damage attacks too
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().useDefaultAttackValuesFS == true)
            {
                attack = encounter.attack;
                arcanePower = encounter.arcanePower;
            }

            else
            {
                //in case foe uses special physical attack
                //need to use the long version, cause of the reroll function
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementType[0] == 1)
                {
                    attack = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementQty[0];
                    //need to set this to 0, in case foes regular attack in arcane
                    arcanePower = 0;
                }

                //in case foe uses special arcane attack
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementType[0] == 3)
                {
                    arcanePower = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().requirementQty[0];
                    attack = 0;
                }
            }
        }

        //determine which attack type is used
        //note that enemies should only have 1 default attack type then
        if (attack != 0)   //(encounter.attack != 0)
        {
            for (int i = 0; i < attack; i++)
            {
                //get defense icons
                //skillCheckText += GetSkillIcon(1);
            }
        }
        else if (arcanePower != 0)   //(encounter.arcanePower != 0)
        {
            for (int i = 0; i < arcanePower; i++)
            {
                //get defense icons
                //skillCheckText += GetSkillIcon(3);
            }
        }

        if (attack != 0)
        {
            //skillCheckText += "\n<size=8>\n</size>Your Defense\n";

            for (int i = 0; i < character.defense && i < 6; i++)
            {
                //get defense icons
                //skillCheckText += GetSkillIcon(2);
            }
        }

        else if (arcanePower != 0)
        {
            //skillCheckText += "\n<size=8>\n</size>Your Resistance\n";

            for (int i = 0; i < character.resistance && i < 6; i++)
            {
                //get defense icons
                //skillCheckText += GetSkillIcon(4);
            }
        }

        int roll = Random.Range(1, 7);

        skillCheckText += "Roll\n";
        skillCheckText += GetDice(roll);

        int hits = 0;

        if (attack != 0)
        {
            hits = attack - (roll + GetSkillPoints(2));
        }
        else if (arcanePower != 0)
        {
            hits = arcanePower - (roll + GetSkillPoints(4));
        }

        if (hits < 1)
        {
            hits = 0;
        }

        skillCheckText += "\n<size=8>\n</size>Damage Taken\n";

        //this is only used for sfx here?
        bool isSuccess = false;

        //need to determine attack type for reroll button
        //type 1 is default attack (either physical or magical in this case)
        int enemyAttackTypeUsed = 1;

        //need to determine attack type for reroll button
        //type 1 is default attack
        //int attackTypeUsed = 1;

        //special cases (always take hit on fumble)
        if (roll == 1)
        {
            //skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            //isSuccess = false;

            if (hits == 0)
            {
                hits = 1;
            }
        }

        //no hits taken on crit
        if (roll == 6)
        {
            //skillCheckText += "<color=green>";
            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            //isSuccess = true;

            hits = 0;
        }

        //special cases for heavy hitters
        //dismiss this if special phase
        if (hits > 0 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(4) == true &&
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == false)
        {
            //round hitsdone / 2 upwards
            float hitsFloat = Mathf.Ceil(hits / 2f);
            hits += (int)hitsFloat;
        }

        //special phase damage modifiers
        if(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            //1.5x damage (shadow bolt, bombardment, heavy strike specials)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(15) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(17) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(28) == true)
            {
                //round hitsdone / 2 upwards
                float hitsFloat = Mathf.Ceil(hits / 2f);
                hits += (int)hitsFloat;
            }

            //2x damage (greater fireball, shadow strike)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(36) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(44) == true)
            {
                //hits are doubled
                hits = hits * 2;
            }
        }

        //check heros damage reductions last
        if (hits != 0 && arcanePower == 0)
        {
            //physical damage type
            hits = CheckProtections(1, hits);
        }
        if (hits != 0 && attack == 0)
        {
            //magic damage type
            hits = CheckProtections(3, hits);
        }

        //dont need roll check?
        for (int i = 0; i < hits; i++)
        {
            skillCheckText += "<sprite index=11>";
        }

        if (hits >= 1)
        {
            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = false;
        }
        else
        {
            skillCheckText += "<color=green>No damage</color>";
            isSuccess = true;
        }


        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().PV.RPC("RPC_BasicDefend", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess, enemyAttackTypeUsed, hits);

        //show different animation depending on outcome
        //hit animation
        if (hits > 0 && hits <= character.energy)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 14);
        }
        //KO animation
        else if (hits > 0 && hits > character.energy)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 15);
        }
        //no hits animation
        else if (hits == 0)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ShowHeroAnimation", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, 10);
        }

    }
    */

    public int CheckProtections(int damageType, int hits)
    {
        if (damageType == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalDefenseModifier != 0)
        {
            //check defense damage modifier (rounded down)
            //float defModifiedFloat = Mathf.Ceil(hits - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier / 100f);
            float defModifiedFloat = hits - (hits * (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalDefenseModifier / 100f));
            int newHits = (int)defModifiedFloat;

            Debug.Log("newHits is: " + newHits);

            return newHits;
        }

        if (damageType == 3 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalResistanceModifier != 0)
        {
            //check defense damage modifier (rounded down)
            //float defModifiedFloat = Mathf.Ceil(hits - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier / 100f);
            float resModifiedFloat = hits - (hits * (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalResistanceModifier / 100f));
            int newHits = (int)resModifiedFloat;

            Debug.Log("newHits is: " + newHits);

            return newHits;
        }

        return hits;
    }

    public void RerollDefenseButton()
    {
        //certain cards cant be used when paused
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused == true && GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Button1.clip;
            CardHandler.ins.intelligenceSfxHolder.Play();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CombatPauseButton();
            return;
        }

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.toolTipBackground.SetActive(false);

        //costs energy for priestess, favor for others
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(15) &&
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }
        else
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().RerollDefenseButton();

        //add new targetting system here
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            GameManager.ins.references.targettingHandler.TakeBallScore();
        }

        gameObject.SetActive(false);
    }

    //shouldnt be used in v0.5.7.
    public void ContinueFromBasicDefenseCheckButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //put these just in case
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ContinueFromSpecialSkillCheck", RpcTarget.AllBufferedViaServer);
            return;
        }

        //reset single shot defense abilities
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetSingleShotDefense();

        //only apply certain effects once per combat round for foes (such as regeneration, decays)
        //returns true, if foe was defeated
        if(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().OncePerRoundFoeEffects() == true)
        {
            return;
        }

        //give turn to player, if all foes have acted
        if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 2) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 3))
        {
            //checks foe hit taken and hit done abilities, hero takes damage per hitstaken
            //if true, hero or foe was defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingAttackPhase() == true)
            {
                return;
            }

            //lets do the foe turn change inside the attack phase method
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn = 1;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //need to do this elsewhere, since this button gets deleted
            //enemies strike first, if any of them has alert
            //and if hero doesnt have swift mount
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
        }

        //otherwise give turn to next foe
        else if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 2))
        {
            //checks foe hit taken and hit done abilities, hero takes damage per hitstaken
            //if true, hero or foe was defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingDefensePhase() == true)
            {
                return;
            }

            //lets do the foe turn change inside the defense phase method
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //need to do this elsewhere, since this button gets deleted
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
        }
    }

    //are we using this method at all anymore?
    //used when hero or foe is defeated
    public void FinishBattleButton()
    {
        GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();

        /* lets move this to combat actions
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //remove combat effects from hero
        CardHandler.ins.RemoveCombatEffectsFromHero();

        //remove targetting window
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
        GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
        GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
        GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();

        //undying check here (could as well make secondPhase != null check, but whatever)
        //could check attack type used for now
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(8) && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true &&
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().attackTypeUsed != 2)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_SetSecondPhaseEncounter", RpcTarget.AllBufferedViaServer);
            return;
        }

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();

        //special case for victory rush
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(7))
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
        }

        //see if hero has energy drain, and you used arcane attack
        //need undead & machine checks here later
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().attackTypeUsed == 2 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 165, 2) > 0)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone / 4);
        }
        */
    }

    //closes the enemy stat display
    public void CloseEnemyStatDisplay()
    {
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);

        //close enemy health bar display also, should be reopened by default on each new phase if theres multi-combat?
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyDisplay.SetActive(false);
    }

    //returns true, if buttonholder has the special requirement were looking for
    public bool CheckButtonHolder(int special)
    {
        for(int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonHolder.transform.childCount; i++)
        {
            if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonHolder.transform.GetChild(i).GetComponent<EncounterButton>().specialRequirement == special)
            {
                return true;
            }
        }

        return false;
    }

    //used for normal interactions
    //returns to normal interaction main options
    public void BackTrackButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //need to do this elsewhere, since this button gets deleted
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().BacktrackButton();
    }

    //used by main location explore actions
    //repurposed from action class for new system
    public void ExploreButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //"leaves" node
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.DeactivateButtons();

        //if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed == false)
        //CloseCanvases();

        GameManager.ins.toolTipBackground.SetActive(false);

        //note that this now uses the nodenumber, not the number set on action button
        int exploreListNumber = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.nodeNumber;

        //dont think weve used this in a while (was at line 159 on explorehandler)
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().NewInteraction(exploreListNumber, 1, "");

        //update character energy & interaction token
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().InteractWithLocation();

        //open usables display
        //GameManager.ins.uiButtonHandler.CloseAllDisplays();
        //GameManager.ins.uiButtonHandler.HandCardsButtonPressed();

        //allow encounter cards
        //CardHandler.ins.SetUsables(2);
    }

    //for old teleport scrolls
    public void Teleport()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore5", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToMapButton(teleportLocation);
    }

    //for v99 teleport
    public void TeleportToSubArea()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore5", RpcTarget.AllBufferedViaServer);

        //inn
        if (teleportLocation == 1)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(5, 1, false);
        }
        //faewood
        if (teleportLocation == 2)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(17, 3, false);
        }
        //blue citadel
        if (teleportLocation == 3)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(37, 5, false);
        }
        //temple of isolore
        if (teleportLocation == 4)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(58, 2, false);
        }
        //underworld mid
        if (teleportLocation == 5)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(23, 3, false);
        }
        //add more here soon
    }

    public void TeleportToUnderworld()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore5", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(37, 4, false);
    }

    //for first area teleporter
    public void FirstTeleport()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //this might bug out tho, if theres several instances of the game running..
        int locationNumber = PhotonRoom.room.startLocation;

        //dont need map type for now (maybe later)
        if (locationNumber == 1)//(PhotonRoom.room.mapType == 1)
        {
            //GameManager.ins.references.useSpecificNodeForSpawn = 2;
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(5, 1, false);
        }
        if (locationNumber == 2)//(PhotonRoom.room.mapType == 1)
        {
            //GameManager.ins.references.useSpecificNodeForSpawn = 2;
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToSubArea(58, 2, false);
        }

        /* unused in v0.5.0.+
         * fixed
        if (PhotonRoom.room.mapType == 2)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().TeleportToMapButton(teleportLocation);
        }
        */

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore5", RpcTarget.AllBufferedViaServer);
    }

    public void OperateNemonoxButton()
    {
        //special bonus
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(5, 100);

        //lets use the epiloguewindow now
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().InitiateTrueVictory();

        //starts score music
        GameObject.Find("SFX Player").GetComponent<SoundManager>().PlayScoreMusic(2);
        return;
    }

    public void ZaarinsMessage1()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().Zaarin1();
    }
}
