using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Encounter2 : MonoBehaviour
{
    //start icon (not used anymore?)
    //public int iconNumber;

    //played when the encounter if drawn, plays default sfx if left null
    public AudioClip discoverySfx;

    //initiated by destination encounter, lasts untill exploration ends
    //could also be used for special music in some cases (actually maybe combat music needs its own holder?)
    //should be different from discovery sfx, since they use different holders
    public AudioClip ambientSfxDay;
    public AudioClip ambientSfxNight;

    //sfx & music volume settings
    public float musicVolume;
    public float ambientSfxDayVolume;
    public float ambientSfxNightVolume;

    public Sprite icon;
    public Sprite nightIcon;

    //unsure if this can be used for unique encounters (or what it even does? :-))
    //actually lets use this specifically for unique encounter in v1.0.0. (didnt seem to do anything worthwhile before)
    public bool isTaken;

    //
    public bool respawns;

    //remember to set these at 0 on continuation encounters
    public int stage1Rarity;
    public int stage2Rarity;
    public int stage3Rarity;
    public int stage4Rarity;
    public int stage5Rarity;
    public int stage6Rarity;
    public int stage7Rarity;

    //keeps track of whether the encounter is forced or not, or if its unique
    //dont need to show icons anymore, unique is just for sfx
    public bool isForcedEncounter;
    public bool isUniqueEncounter;

    //might be better to check this instead of maxenergy
    public bool isCombatEncounter;

    //0 for no time requirement, 1 for day, 2 for night
    public int requirementTime;

    //dunno if needed?
    public int numberInList;

    //description for when encounter is first found
    [TextArea(5, 20)]
    public string startText;

    //buttons
    public List<GameObject> buttons;

    //enemy info
    //max energy stat now also used to determine if the encounter is actually an enemy
    public int maxEnergy;
    public int currentEnergy;
    public int maxAttack;
    public int attack;
    public int maxArcanePower;
    public int arcanePower;
    public int defense;
    public int resistance;

    //for v94
    public int attackMod;
    public int arcanePowerMod;
    public int defenseMod;
    public int resistanceMod;

    //need enemy name for stat display
    public string enemyName;

    //description for when encounter is first found
    [TextArea(5, 20)]
    public string enemyDescription;

    public string enemyTierText;

    //maybe use these to determine which attacks the opponent uses and in which order
    //0 = none, 1 = physical, 2 = magical
    //attack types above these are special attacks?
    //public int firstAttackType;
    //public int secondAttackType;

    //if true doesnt register as continue encounter
    //public bool isFirstEncounter;
    //public bool isContinueEncounter;

    //if false, explore more button is disabled
    //doesnt really do anything atm
    public bool allowExploreMore;

    //enemy traits
    public List<GameObject> enemyTraits;

    //for storing list number of destination encounters (this is instead stored at encounter buttons now)
    //public int destinationNumber;

    //need this for combats from continue encounters
    public Sprite overrideCombatIcon;

    //need this for new combat system (when ending combat)
    public EncounterButton combatButton;

    //if true, automatically selects one option, and disables buttons (if theres any automated buttons?)
    public bool useAutomation;

    public GameObject automatedButton;

    //for foe image resizing
    public float displaySize;
    public float battlefieldFoeSize;
    public float floatHeight;
    public float foeDisplayHeight;

    //for v99 (replaces location node checks for stores)
    //public int storeNumber;

    //this could be used to force musics on certain multi-combats for example
    //public bool isBoss;

    // Start is called before the first frame update
    void Start()
    {

    }

    //initial button spawn
    public void SpawnButtons()
    {
        //shows walking animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 12);

        //only update this on found buttons
        int buttonNumber = 0;

        bool hasSkillCheck = false;
        //bool hasCombat = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null)
            {
                //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                //reset this always when new encounter is drawn
                buttons[i].GetComponent<EncounterButton>().isTakenTemporarily = false;

                //lets try this more understandable way
                //buttons[i].GetComponent<EncounterButton>().originalButtonNumber = buttonNumber;
                buttons[i].GetComponent<EncounterButton>().originalButtonNumber = i;

                if (buttons[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                {
                    GameObject buttonObject = Instantiate(buttons[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                    //places it in hand card area
                    buttonObject.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonHolder.transform, false);

                    buttonObject.GetComponent<Button>().interactable = false;

                    //added new method here since v90
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        buttonObject.GetComponent<EncounterButton>().ButtonEnable() == true)//buttons[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                    {
                        buttonObject.GetComponent<Button>().interactable = true;

                        //focus system for v92
                        if (buttonObject.GetComponent<EncounterButton>().isSkillCheck.Length > 0)
                        {
                            if (buttonObject.GetComponent<EncounterButton>().isSkillCheck[0] == true)
                            {
                                hasSkillCheck = true;
                            }
                        }
                        //automatically chooses one option for certain encounters
                        //note that this now only works for initial option
                        if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().useAutomation == true)
                        {
                            if(GameManager.ins.references.currentEncounter.useAutomation == true)
                            {
                                buttonObject.GetComponent<Button>().interactable = false;

                                if (buttonObject.GetComponent<EncounterButton>().isChosenAutomatically == true)
                                {
                                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().firstCheckSolved = false;

                                    automatedButton = buttonObject.gameObject;
                                    UseAutomatedButtonWithDelay();
                                }
                            }
                        }

                        /*combat mode toggle for combats
                        if (buttons[i].GetComponent<EncounterButton>().isFightButton == true)
                        {
                            hasCombat = true;
                        }
                        */
                    }

                    //update button tooltip (if theres info to update)
                    buttonObject.GetComponent<EncounterButton>().UpdateTooltip();

                    //update button text(if its set on the element, otherwise use the default one)
                    buttonObject.GetComponent<EncounterButton>().UpdateButtonText();

                    //need to keep track of this?
                    buttonObject.GetComponent<EncounterButton>().buttonNumber = buttonNumber;

                    //note that original button number needs to be set on the original list
                    //buttons[i].GetComponent<EncounterButton>().originalButtonNumber = buttonNumber;
                    //or lets try it more understandable way
                    //buttonObject.GetComponent<EncounterButton>().originalButtonNumber = buttonNumber;

                    buttonNumber += 1;
                }
            }
        }

        if(hasSkillCheck == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnFocusButton();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnPrayerButton();
        }
        /*new option for v93
        if(hasCombat == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnCombatModeButton();
        }
        */
    }

    public void UseAutomatedButtonWithDelay()
    {
        Invoke(nameof(UseAutomatedButton), 1.0f);
    }

    public void UseAutomatedButton()
    {
        //put gravestone here 
        // !!! need testing to make sure this doesnt cause bugs
        // probably shouldnt, as long as theres no other automated checks after foe defeated, other than the corpse explosion
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true &&
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(242) == false &&
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(243) == false)
        {
            GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
            GameManager.ins.references.enemyResizing.ActivateFoeBump(1);

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].enemyResizing.foeImageObject.GetComponent<Image>().sprite =
                    GameManager.ins.references.enemyResizing.foeGravestone;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].enemyResizing.ActivateFoeBump(1);
        }

        automatedButton.GetComponent<Button>().onClick.Invoke();
    }

    //continuation button spawn
    //might need to change button number counting here too
    public void SpawnContinuationButtons(bool isSuccess, bool showDefaultAnimation)
    {
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == false && showDefaultAnimation == true)
        {
            //shows walking animation for all
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 12);
        }
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == true && showDefaultAnimation == false)
        {
            //shows KO animation for all, if died when fleeing
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 15);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
        }

        //EncounterButton usedButton = buttons[GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonChosen].GetComponent<EncounterButton>();
        EncounterButton usedButton = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonChosen2;

        //special case for combat encounters (take first usedbutton on list)
        //actually need to use specific button, depending on foeTurn
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference == true)
        {
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                usedButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2)
            {
                usedButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton2;
            }
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3)
            {
                usedButton = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton3;
            }
        }

        //only update this on found buttons
        int buttonNumber = 0;

        bool hasSkillCheck = false;
        //bool hasCombat = false;

        if (isSuccess == true)
        {
            for (int i = 0; i < usedButton.buttonsAfterSuccess.Count; i++)
            {
                if (usedButton.buttonsAfterSuccess[i] != null)
                {
                    //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                    if (usedButton.buttonsAfterSuccess[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                    {
                        GameObject buttonObject = Instantiate(usedButton.buttonsAfterSuccess[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                        //places it options area
                        buttonObject.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonHolder.transform, false);

                        buttonObject.GetComponent<Button>().interactable = false;

                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        usedButton.buttonsAfterSuccess[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                        {
                            buttonObject.GetComponent<Button>().interactable = true;

                            //focus system for v92
                            if (usedButton.buttonsAfterSuccess[i].GetComponent<EncounterButton>().isSkillCheck.Length > 0)
                            {
                                if (usedButton.buttonsAfterSuccess[i].GetComponent<EncounterButton>().isSkillCheck[0] == true)
                                {
                                    hasSkillCheck = true;
                                }
                            }

                            //special case for explosive foes
                            //this calls explosion check automatically
                            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true &&
                                buttonObject.GetComponent<EncounterButton>().isExplosiveDeathButton == true)
                               //(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(242) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(243) == true))
                            {
                                buttonObject.GetComponent<Button>().interactable = false;

                                if (buttonObject.GetComponent<EncounterButton>().isChosenAutomatically == true)
                                {
                                    automatedButton = buttonObject;
                                    UseAutomatedButtonWithDelay();
                                }
                            }

                            /*combat mode toggle for combats
                            if (usedButton.buttonsAfterSuccess[i].GetComponent<EncounterButton>().isFightButton == true)
                            {
                                hasCombat = true;
                            }
                            */
                        }

                        //update button tooltip (if theres info to update)
                        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();

                        //update button text(if its set on the element, otherwise use the default one)
                        buttonObject.GetComponent<EncounterButton>().UpdateButtonText();

                        //might need to change button number counting here too
                        //buttonObject.GetComponent<EncounterButton>().buttonNumber = i;

                        //need to keep track of this
                        buttonObject.GetComponent<EncounterButton>().buttonNumber = buttonNumber;
                        buttonNumber += 1;
                    }
                }
            }
        }
        if (isSuccess == false)
        {
            for (int i = 0; i < usedButton.buttonsAfterFailure.Count; i++)
            {
                if (usedButton.buttonsAfterFailure[i] != null)
                {
                    //EncounterButton encounterButton = Button1.GetComponent<EncounterButton>();

                    if (usedButton.buttonsAfterFailure[i].GetComponent<EncounterButton>().ButtonAvailability() == true)
                    {
                        GameObject buttonObject = Instantiate(usedButton.buttonsAfterFailure[i].gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                        //places it options area
                        buttonObject.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().buttonHolder.transform, false);

                        buttonObject.GetComponent<Button>().interactable = false;

                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true &&
                        usedButton.buttonsAfterFailure[i].GetComponent<EncounterButton>().ButtonEnable() == true)
                        {
                            buttonObject.GetComponent<Button>().interactable = true;

                            //focus system for v92
                            if (usedButton.buttonsAfterFailure[i].GetComponent<EncounterButton>().isSkillCheck.Length > 0)
                            {
                                if (usedButton.buttonsAfterFailure[i].GetComponent<EncounterButton>().isSkillCheck[0] == true)
                                {
                                    hasSkillCheck = true;
                                }
                            }

                            /*combat mode toggle for combats
                            if (usedButton.buttonsAfterFailure[i].GetComponent<EncounterButton>().isFightButton == true)
                            {
                                hasCombat = true;
                            }
                            */
                        }

                        //update button tooltip (if theres info to update)
                        buttonObject.GetComponent<EncounterButton>().UpdateTooltip();

                        //update button text(if its set on the element, otherwise use the default one)
                        buttonObject.GetComponent<EncounterButton>().UpdateButtonText();

                        //buttonObject.GetComponent<EncounterButton>().buttonNumber = i;
                        //need to keep track of this
                        buttonObject.GetComponent<EncounterButton>().buttonNumber = buttonNumber;
                        buttonNumber += 1;
                    }
                }
            }
        }

        if (hasSkillCheck == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnFocusButton();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnPrayerButton();
        }

        /*new option for v93
        if (hasCombat == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SpawnCombatModeButton();
        }
        */
    }

    //update these methods later
    public void ShowEnemyStats()
    {
        //reset the fields
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackText.text = "";
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyDefenseText.text = "";

        //dont show these, unless opponent has max energy stat?
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyInfoButton.SetActive(false);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyDisplay.SetActive(false);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyTierDisplay.SetActive(false);
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyText.GetComponentInParent<Image>().gameObject.SetActive(false);

        //do the rest of the method, if encounter has any max energy value
        if (maxEnergy == 0)
        {
            return;
        }

        //first show energy display & info button
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyInfoButton.SetActive(true);
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyDisplay.SetActive(true);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyTierDisplay.SetActive(true);


        //attack stats
        //only show 1 of these at a time
        if (GameManager.ins.references.currentStrategicEncounter.attack != 0)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackDisplay.SetActive(true);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAPDisplay.SetActive(false);

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackModDisplay.SetActive(true);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAPModDisplay.SetActive(false);

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackText.text = GameManager.ins.references.currentStrategicEncounter.attack.ToString();
        }
        if (GameManager.ins.references.currentStrategicEncounter.arcanePower != 0)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackDisplay.SetActive(false);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAPDisplay.SetActive(true);

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackModDisplay.SetActive(false);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAPModDisplay.SetActive(true);

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAPText.text = GameManager.ins.references.currentStrategicEncounter.arcanePower.ToString();
        }

        //defense stats
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyDefenseText.text = GameManager.ins.references.currentStrategicEncounter.defense.ToString();

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyResistanceText.text = GameManager.ins.references.currentStrategicEncounter.resistance.ToString();

        //modifiers
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackModText.text = GameManager.ins.references.currentEncounter.attackMod.ToString() + "%";
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAPModText.text = GameManager.ins.references.currentEncounter.arcanePowerMod.ToString() + "%";
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyDefenseModText.text = GameManager.ins.references.currentEncounter.defenseMod.ToString() + "%";
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyResistanceModText.text = GameManager.ins.references.currentEncounter.resistanceMod.ToString() + "%";

        //return the energy text for v0.5.7. (the old bar will be unused, instead we use the bars on battlefield display)
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyText.text = GameManager.ins.references.currentStrategicEncounter.currentEnergy.ToString();
        //for foe health bars
        //GameManager.ins.references.GetComponent<SliderController>().SetBarValues(0);


        //update enemy name on the display
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyNameText.text = enemyName;

        //update enemy tier text
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyTierText.text = enemyTierText;

        //update enemy description on the display
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyDescriptionText.text = enemyDescription;

    }

    public void HideEnemyStats()
    {
        //reset the fields
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyAttackText.text = "";
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyDefenseText.text = "";

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyInfoButton.SetActive(false);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyDisplay.SetActive(false);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyTierDisplay.SetActive(false);
    }

    //empties the foe ability list, and draws new ones
    //this updates starting cooldown now too
    //deprecated since v91
    public void NewFoeAbilities()
    {
        int numberOfFoeCards = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount;

        if (numberOfFoeCards > 0)
        {
            for (int i = numberOfFoeCards-1; i >= 0; i--)
            {
                Destroy(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject);
            }
        }

        for(int y = 0; y < enemyTraits.Count; y++)
        {
            //instantiates random quest card from the deck
            GameObject playerCard = Instantiate(enemyTraits[y], new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            playerCard.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform, false);

            //turns the card inactive
            playerCard.SetActive(true);

            //update cooldown, if there is starting cooldown (foe cooldown abilities should have this always?)
            if (playerCard.GetComponent<CardDisplay2>().startingCooldown > 0)
            {
                playerCard.GetComponent<CardDisplay2>().cooldown = playerCard.GetComponent<CardDisplay2>().startingCooldown;

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
    }
}
