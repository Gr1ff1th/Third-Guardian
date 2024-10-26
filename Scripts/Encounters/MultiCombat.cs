using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


//handles various variables and methods for multi-unit combat purposes
public class MultiCombat : MonoBehaviour
{
    public Image extraFoeIcon1;
    public Image extraFoeIcon2;

    public GameObject extraFoeBorder1;
    public GameObject extraFoeBorder2;

    public Encounter2 foeEncounter1;
    public Encounter2 foeEncounter2;
    public Encounter2 foeEncounter3;

    public StrategicEncounter foeStrategic1;
    public StrategicEncounter foeStrategic2;
    public StrategicEncounter foeStrategic3;

    //needed for storing initial fight buttons for each foe
    public EncounterButton usedButton1;
    public EncounterButton usedButton2;
    public EncounterButton usedButton3;

    //keeps track of which foes turn it is
    public int foeTurn = 1;

    //this number gets reduced whenever you defeat a foe
    public int numberOfFoes = 0;

    //this tracks the original amount of foes
    public int originalNumberOfFoes = 0;

    //maybe were using too many flag variables
    public bool allFoesDefeated;

    //set this to true, only while finishing battle against a foe
    public bool useCombatButtonReference;

    //set this to true, if initial check is done for all foes
    public bool initialPhaseDone;

    //need something like this for v0.5.7.
    //determines to which foe the hero attacks are targetted to
    public int currentTarget = 1;

    //
    public List<BattlefieldFoe> BattlefieldFoes;

    // Start is called before the first frame update
    void Start()
    {
        BattlefieldFoes[0].foeImageOriginalPosition = BattlefieldFoes[0].FoeImage.transform.localPosition;
        BattlefieldFoes[0].foeDisplayOriginalPosition = BattlefieldFoes[0].FoeDisplays.transform.localPosition;
        BattlefieldFoes[0].foeGameObjectOriginalPosition = BattlefieldFoes[0].gameObject.transform.localPosition;

        BattlefieldFoes[1].foeImageOriginalPosition = BattlefieldFoes[1].FoeImage.transform.localPosition;
        BattlefieldFoes[1].foeDisplayOriginalPosition = BattlefieldFoes[1].FoeDisplays.transform.localPosition;
        BattlefieldFoes[1].foeGameObjectOriginalPosition = BattlefieldFoes[1].gameObject.transform.localPosition;

        BattlefieldFoes[2].foeImageOriginalPosition = BattlefieldFoes[2].FoeImage.transform.localPosition;
        BattlefieldFoes[2].foeDisplayOriginalPosition = BattlefieldFoes[2].FoeDisplays.transform.localPosition;
        BattlefieldFoes[2].foeGameObjectOriginalPosition = BattlefieldFoes[2].gameObject.transform.localPosition;
    }

    //for some tricky behavior
    private void Update()
    {
        //this might be dangerous
        UpdateFoeCards();
    }

    public void UpdateFoeCards()
    {
        for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount; i++)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateHiddenCard();
            }
        }
    }

    //checked when first combat encounter is drawn in node
    public void CheckFoes()
    {
        //clear foe holders (encounter2 and strategicEncounter)
        ClearFoes();

        foeTurn = 1;
        numberOfFoes = 1;
        originalNumberOfFoes = 1;
        useCombatButtonReference = false;
        allFoesDefeated = false;

        //take info of first foe as well
        foeEncounter1 = GameManager.ins.references.currentEncounter;
        foeStrategic1 = GameManager.ins.references.currentStrategicEncounter;
        foeStrategic1.isChecked = true;
        //reset this in case of second wind foes
        foeStrategic1.secondWindActivated = false;
        ResetFoeStats(1);

        //initially lets only show stats of first combat encounter
        foeEncounter1.ShowEnemyStats();

        //also clear all previous foe cards
        ClearAllFoeAbilities();

        //then Draw new ones
        //this also sets starting cooldowns
        DrawFoeCards(1);

        //need this for battlefield display in v0.5.7.
        BattlefieldFoes[0].thisStrategicEncounter = foeStrategic1;


        for (int i = 0; i < GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes.Count; i++)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>() != null)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().isChecked == false && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().encounter.isCombatEncounter == true)
                {
                    //take this info
                    if (foeEncounter2 == null)
                    {
                        foeEncounter2 = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().encounter;
                        foeStrategic2 = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>();
                        //reset this in case of second wind foes
                        foeStrategic2.secondWindActivated = false;
                        ResetFoeStats(2);
                        DrawFoeCards(2);

                        numberOfFoes += 1;
                        originalNumberOfFoes += 1;
                        ShowSmallIcon(foeEncounter2.icon, 1);

                        //need this for battlefield display in v0.5.7.
                        BattlefieldFoes[1].thisStrategicEncounter = foeStrategic2;

                    }

                    else if (foeEncounter3 == null)
                    {
                        foeEncounter3 = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().encounter;
                        foeStrategic3 = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>();
                        //reset this in case of second wind foes
                        foeStrategic3.secondWindActivated = false;
                        ResetFoeStats(3);
                        DrawFoeCards(3);

                        numberOfFoes += 1;
                        originalNumberOfFoes += 1;
                        ShowSmallIcon(foeEncounter3.icon, 2);

                        //need this for battlefield display in v0.5.7.
                        BattlefieldFoes[2].thisStrategicEncounter = foeStrategic3;

                    }


                    //remove stealth
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().hasStealth = false;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.internalNodes[i].gameObject.GetComponentInChildren<StrategicEncounter>().isChecked = true;

                }
            }
        }

        //these need to come after for loop?
        //empower check
        EmpowerCheck();

        //defender check
        DefenderCheck();

        //need delay or defender check might bug
        Invoke(nameof(CheckFoesContinue), 0.3f);
    }

    //need to be done with delay or defender check might bug out
    public void CheckFoesContinue()
    {
        //dont rly need to do this several times
        //set the battlefield foes here
        //better do this after defender check, since the foe positions might change
        if (numberOfFoes == 3)
        {
            BattlefieldFoes[2].gameObject.SetActive(true);
            BattlefieldFoes[1].gameObject.SetActive(true);
            BattlefieldFoes[0].gameObject.SetActive(true);

            //this also sets foe position etc
            //position 1 is leftmost spot, 3 rightmost
            BattlefieldFoes[0].SetBattlefieldFoe(1);
            BattlefieldFoes[1].SetBattlefieldFoe(2);
            BattlefieldFoes[2].SetBattlefieldFoe(3);
        }
        if (numberOfFoes == 2)
        {
            BattlefieldFoes[2].gameObject.SetActive(false);
            BattlefieldFoes[1].gameObject.SetActive(true);
            BattlefieldFoes[0].gameObject.SetActive(true);

            BattlefieldFoes[0].SetBattlefieldFoe(2);
            BattlefieldFoes[1].SetBattlefieldFoe(3);
        }
        if (numberOfFoes == 1)
        {
            BattlefieldFoes[2].gameObject.SetActive(false);
            BattlefieldFoes[1].gameObject.SetActive(false);
            BattlefieldFoes[0].gameObject.SetActive(true);

            BattlefieldFoes[0].SetBattlefieldFoe(3);
        }

        currentTarget = 1;

        //reset these in case player flees and returns to this node?
        if (foeStrategic1 != null)
        {
            foeStrategic1.isChecked = false;
        }
        if (foeStrategic2 != null)
        {
            foeStrategic2.isChecked = false;
        }
        if (foeStrategic3 != null)
        {
            foeStrategic3.isChecked = false;
        }
    }

    public void ClearFoes()
    {
        foeEncounter1 = null;
        foeStrategic1 = null;
        usedButton1 = null;

        foeEncounter2 = null;
        foeStrategic2 = null;
        usedButton2 = null;

        foeEncounter3 = null;
        foeStrategic3 = null;
        usedButton3 = null;
    }

    //might add more stats here later
    public void ResetFoeStats(int foeNumber)
    {
        if(foeNumber == 1)
        {
            //foeStrategic1.currentEnergy = foeEncounter1.maxEnergy;
            //foeStrategic1.attack = foeEncounter1.maxAttack;
            //foeStrategic1.arcanePower = foeEncounter1.maxArcanePower;

            foeStrategic1.maxEnergy = foeEncounter1.maxEnergy;
            foeStrategic1.maxAttack = foeEncounter1.maxAttack;
            foeStrategic1.maxArcanePower = foeEncounter1.maxArcanePower;

            //advanced
            if (GameManager.ins.startingDifficulty == 3)
            {
                foeStrategic1.maxEnergy = (int)(foeEncounter1.maxEnergy * 1.25f);
            }
            //expert
            if (GameManager.ins.startingDifficulty == 4)
            {
                foeStrategic1.maxEnergy = (int)(foeEncounter1.maxEnergy * 1.50f);

                if (foeEncounter1.maxAttack > 0)
                {
                    foeStrategic1.maxAttack = foeEncounter1.maxAttack + 1;
                }
                if (foeEncounter1.maxArcanePower > 0)
                {
                    foeStrategic1.maxArcanePower = foeEncounter1.maxArcanePower + 1;
                }
            }

            foeStrategic1.currentEnergy = foeStrategic1.maxEnergy;
            foeStrategic1.attack = foeStrategic1.maxAttack;
            foeStrategic1.arcanePower = foeStrategic1.maxArcanePower;

            foeStrategic1.defense = foeEncounter1.defense;
            foeStrategic1.resistance = foeEncounter1.resistance;
        }
        if (foeNumber == 2)
        {
            foeStrategic2.maxEnergy = foeEncounter2.maxEnergy;
            foeStrategic2.maxAttack = foeEncounter2.maxAttack;
            foeStrategic2.maxArcanePower = foeEncounter2.maxArcanePower;

            //advanced
            if (GameManager.ins.startingDifficulty == 3)
            {
                foeStrategic2.maxEnergy = (int)(foeEncounter2.maxEnergy * 1.25f);
            }
            //expert
            if (GameManager.ins.startingDifficulty == 4)
            {
                foeStrategic2.maxEnergy = (int)(foeEncounter2.maxEnergy * 1.50f);

                if (foeEncounter2.maxAttack > 0)
                {
                    foeStrategic2.maxAttack = foeEncounter2.maxAttack + 1;
                }
                if (foeEncounter2.maxArcanePower > 0)
                {
                    foeStrategic2.maxArcanePower = foeEncounter2.maxArcanePower + 1;
                }
            }

            foeStrategic2.currentEnergy = foeStrategic2.maxEnergy;
            foeStrategic2.attack = foeStrategic2.maxAttack;
            foeStrategic2.arcanePower = foeStrategic2.maxArcanePower;

            foeStrategic2.defense = foeEncounter2.defense;
            foeStrategic2.resistance = foeEncounter2.resistance;
        }
        if (foeNumber == 3)
        {
            foeStrategic3.maxEnergy = foeEncounter3.maxEnergy;
            foeStrategic3.maxAttack = foeEncounter3.maxAttack;
            foeStrategic3.maxArcanePower = foeEncounter3.maxArcanePower;

            //advanced
            if (GameManager.ins.startingDifficulty == 3)
            {
                foeStrategic3.maxEnergy = (int)(foeEncounter3.maxEnergy * 1.25f);
            }
            //expert
            if (GameManager.ins.startingDifficulty == 4)
            {
                foeStrategic3.maxEnergy = (int)(foeEncounter3.maxEnergy * 1.50f);

                if (foeEncounter3.maxAttack > 0)
                {
                    foeStrategic3.maxAttack = foeEncounter3.maxAttack + 1;
                }
                if (foeEncounter3.maxArcanePower > 0)
                {
                    foeStrategic3.maxArcanePower = foeEncounter3.maxArcanePower + 1;
                }
            }

            foeStrategic3.currentEnergy = foeStrategic3.maxEnergy;
            foeStrategic3.attack = foeStrategic3.maxAttack;
            foeStrategic3.arcanePower = foeStrategic3.maxArcanePower;

            foeStrategic3.defense = foeEncounter3.defense;
            foeStrategic3.resistance = foeEncounter3.resistance;
        }
    }

    //is called after all foe cards have been drawn
    public void EmpowerCheck()
    {
        //empower check (from any foe)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(72) == true)
        {
            if(foeStrategic1 != null)
            {
                if(foeStrategic1.attack != 0)
                {
                    foeStrategic1.attack += 1;
                }
                if (foeStrategic1.arcanePower != 0)
                {
                    foeStrategic1.arcanePower += 1;
                }
            }
            if (foeStrategic2 != null)
            {
                if (foeStrategic2.attack != 0)
                {
                    foeStrategic2.attack += 1;
                }
                if (foeStrategic2.arcanePower != 0)
                {
                    foeStrategic2.arcanePower += 1;
                }
            }
            if (foeStrategic3 != null)
            {
                if (foeStrategic3.attack != 0)
                {
                    foeStrategic3.attack += 1;
                }
                if (foeStrategic3.arcanePower != 0)
                {
                    foeStrategic3.arcanePower += 1;
                }
            }
        }
    }

    public void DefenderCheck()
    {
        StrategicEncounter strategicHolder;
        Encounter2 encounterHolder;

        Debug.Log("number of foes is: " + numberOfFoes);

        if (numberOfFoes == 3)
        {
            //check if foe 3 is defender
            if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(3, 73) == true)
            {
                //dont swap units if foe 1 is also defender
                if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(1, 73) == false)
                {
                    strategicHolder = foeStrategic1;
                    foeStrategic1 = foeStrategic3;

                    encounterHolder = foeEncounter1;
                    foeEncounter1 = foeEncounter3;

                    foeStrategic3 = strategicHolder;
                    foeEncounter3 = encounterHolder;

                    //replace battlefield encounters too
                    BattlefieldFoes[0].thisStrategicEncounter = foeStrategic1;
                    BattlefieldFoes[2].thisStrategicEncounter = foeStrategic3;

                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = foeEncounter1;
                    GameManager.ins.references.currentEncounter = foeEncounter1;
                    //ClearSpecificFoeAbilities(1);
                    //ChangeFoeCardOwnership(1);

                    //DrawFoeCards(3);
                    ShowSmallIcon(foeEncounter3.icon, 2);
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SetFoeIcon(foeEncounter1.icon);

                    //foeEncounter1.ShowEnemyStats();

                    ClearAllFoeAbilities();

                    //need to redraw these, to avoid bugs
                    //this also shows current enemy stats now
                    Invoke(nameof(RedrawFoeCards), 0.2f);
                    return;
                }
            }

            //check if foe 2 is defender
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(2, 73) == true)
            {
                //dont swap units if foe 1 is also defender
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(1, 73) == false)
                {
                    strategicHolder = foeStrategic1;
                    foeStrategic1 = foeStrategic2;

                    encounterHolder = foeEncounter1;
                    foeEncounter1 = foeEncounter2;

                    foeStrategic2 = foeStrategic3;
                    foeEncounter2 = foeEncounter3;

                    foeStrategic3 = strategicHolder;
                    foeEncounter3 = encounterHolder;

                    //replace battlefield encounters too
                    BattlefieldFoes[0].thisStrategicEncounter = foeStrategic1;
                    BattlefieldFoes[1].thisStrategicEncounter = foeStrategic2;
                    BattlefieldFoes[2].thisStrategicEncounter = foeStrategic3;

                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = foeEncounter1;
                    GameManager.ins.references.currentEncounter = foeEncounter1;
                    //ClearSpecificFoeAbilities(1);
                    //ChangeFoeCardOwnership(1);

                    //DrawFoeCards(3);
                    ShowSmallIcon(foeEncounter2.icon, 1);
                    ShowSmallIcon(foeEncounter3.icon, 2);
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SetFoeIcon(foeEncounter1.icon);

                    //foeEncounter1.ShowEnemyStats();

                    ClearAllFoeAbilities();

                    //need to redraw these, to avoid bugs
                    //this also shows current enemy stats now
                    Invoke(nameof(RedrawFoeCards), 0.2f);
                    return;
                }
            }
        }
        if (numberOfFoes == 2)
        {
            Debug.Log("foe 1 is defender: " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(1, 73));

            //check if foe 2 is defender
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(2, 73) == true)
            {
                //dont swap units if foe 1 is also defender
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(1, 73) == false)
                {
                    strategicHolder = foeStrategic1;
                    foeStrategic1 = foeStrategic2;

                    encounterHolder = foeEncounter1;
                    foeEncounter1 = foeEncounter2;

                    foeStrategic2 = strategicHolder;
                    foeEncounter2 = encounterHolder;

                    //replace battlefield encounters too
                    BattlefieldFoes[0].thisStrategicEncounter = foeStrategic1;
                    BattlefieldFoes[1].thisStrategicEncounter = foeStrategic2;

                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = foeEncounter1;
                    GameManager.ins.references.currentEncounter = foeEncounter1;
                    //ClearSpecificFoeAbilities(1);
                    //ChangeFoeCardOwnership(1);

                    //DrawFoeCards(2);
                    ShowSmallIcon(foeEncounter2.icon, 1);
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SetFoeIcon(foeEncounter1.icon);

                    //foeEncounter1.ShowEnemyStats();

                    ClearAllFoeAbilities();

                    //need to redraw these, to avoid bugs
                    //this also shows current enemy stats now
                    Invoke(nameof(RedrawFoeCards), 0.2f);
                    return;
                }
            }
        }

    }

    //redraws foe cards
    //probably bet used with delay (at least if theres delete function before)
    //seems its needed to get it working properly
    public void RedrawFoeCards()
    {
        if(numberOfFoes == 3)
        {
            DrawFoeCards(1);
            DrawFoeCards(2);
            DrawFoeCards(3);
        }
        if (numberOfFoes == 2)
        {
            DrawFoeCards(1);
            DrawFoeCards(2);
        }
        if (numberOfFoes == 1)
        {
            DrawFoeCards(1);
        }

        //need this?
        GameManager.ins.references.currentStrategicEncounter = foeStrategic1;
        foeEncounter1.ShowEnemyStats();
    }

    //might add more stats here later
    public void ShowFoeStats(int foeNumber)
    {
        if (foeNumber == 1)
        {
            foeStrategic1.currentEnergy = foeStrategic1.maxEnergy;
            foeStrategic1.attack = foeStrategic1.maxAttack;
            foeStrategic1.arcanePower = foeStrategic1.maxArcanePower;
        }
        if (foeNumber == 2)
        {
            foeStrategic2.currentEnergy = foeStrategic1.maxEnergy;
            foeStrategic2.attack = foeStrategic1.maxAttack;
            foeStrategic2.arcanePower = foeStrategic1.maxArcanePower;
        }
        if (foeNumber == 3)
        {
            foeStrategic3.currentEnergy = foeStrategic1.maxEnergy;
            foeStrategic3.attack = foeStrategic1.maxAttack;
            foeStrategic3.arcanePower = foeStrategic1.maxArcanePower;
        }
    }

    //clears all foe abilities
    public void ClearAllFoeAbilities()
    {
        int numberOfFoeCards = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount;

        if (numberOfFoeCards > 0)
        {
            for (int i = numberOfFoeCards - 1; i >= 0; i--)
            {
                Destroy(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject);
            }
        }
    }

    //only when removing cards of the certain
    public void ClearSpecificFoeAbilities(int foeNumber)
    {
        int numberOfFoeCards = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount;

        if (numberOfFoeCards > 0)
        {
            for (int i = numberOfFoeCards - 1; i >= 0; i--)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).GetComponent<Card>().belongsTo == foeNumber)
                {
                    Destroy(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    //new system for multicombat
    public void DrawFoeCards(int foeNumber)
    {
        if (foeNumber == 1)
        {
            for (int y = 0; y < foeEncounter1.enemyTraits.Count; y++)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(foeEncounter1.enemyTraits[y], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(true);

                playerCard.GetComponent<Card>().belongsTo = 1;

                /* dont need this in v0.5.7.
                 * update cooldown, if there is starting cooldown (foe cooldown abilities should have this always?)
                if (playerCard.GetComponent<CardDisplay2>().startingCooldown > 0)
                {
                    playerCard.GetComponent<CardDisplay2>().cooldown = playerCard.GetComponent<CardDisplay2>().startingCooldown;

                    playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
                }
                */
                if (playerCard.GetComponent<CardDisplay2>().realTimeStartingCooldown > 0)
                {
                    playerCard.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.GetComponent<CardDisplay2>().realTimeStartingCooldown;

                    //playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
                }

            }
        }

        //dont initially show these
        if (foeNumber == 2)
        {
            for (int y = 0; y < foeEncounter2.enemyTraits.Count; y++)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(foeEncounter2.enemyTraits[y], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                playerCard.GetComponent<Card>().belongsTo = 2;

                //update cooldown, if there is starting cooldown (foe cooldown abilities should have this always?)
                if (playerCard.GetComponent<CardDisplay2>().realTimeStartingCooldown > 0)
                {
                    playerCard.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.GetComponent<CardDisplay2>().realTimeStartingCooldown;
                }
            }
        }

        //dont initially show these
        if (foeNumber == 3)
        {
            for (int y = 0; y < foeEncounter3.enemyTraits.Count; y++)
            {
                //instantiates random quest card from the deck
                GameObject playerCard = Instantiate(foeEncounter3.enemyTraits[y], new Vector3(0, 0, 0), Quaternion.identity);

                //places it in hand card area
                playerCard.transform.SetParent(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform, false);

                //turns the card inactive
                playerCard.SetActive(false);

                playerCard.GetComponent<Card>().belongsTo = 3;

                if (playerCard.GetComponent<CardDisplay2>().realTimeStartingCooldown > 0)
                {
                    playerCard.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.GetComponent<CardDisplay2>().realTimeStartingCooldown;
                }
            }
        }
    }

    //activates cards of certain foes
    public void ActivateFoeCards (int foeNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //note that we use effect here, instead of number in deck
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == foeNumber)
                {
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    public void RemoveSmalIcons()
    {
        extraFoeIcon1.gameObject.SetActive(false);
        extraFoeBorder1.SetActive(false);
        extraFoeIcon2.gameObject.SetActive(false);
        extraFoeBorder2.SetActive(false);
    }

    //handles small icon changes
    public void ShowSmallIcon(Sprite icon, int iconNumber)
    {
        if (iconNumber == 1)
        {
            extraFoeIcon1.gameObject.SetActive(true);

            extraFoeIcon1.sprite = icon;

            extraFoeBorder1.SetActive(false);
        }

        if (iconNumber == 2)
        {
            extraFoeIcon2.gameObject.SetActive(true);

            extraFoeIcon2.sprite = icon;

            extraFoeBorder2.SetActive(false);
        }
    }

    //rotates the icons
    //give number of the encounter (in order of first appearance)
    //stupidly complicated :-) (should remake if there are issues)
    //dunno if the gravestone checks actually do anything either, since we dont really call this method untill all foes are defeated anyway? (except when fleeing)
    public void RotateIcons(int encounterNumber)
    {
        /* these are alrdy done beforehand
        int numberOfFoes = 0;

        if (foeEncounter1 != null)
        {
            numberOfFoes += 1;
        }
        if (foeEncounter2 != null)
        {
            numberOfFoes += 1;
        }
        if (foeEncounter3 != null)
        {
            numberOfFoes += 1;
        }
        */

        if (numberOfFoes == 3)
        {
            if (encounterNumber == 1)
            {
                extraFoeIcon1.gameObject.SetActive(true);
                extraFoeIcon1.sprite = foeEncounter2.icon;
                //extraFoeBorder1.SetActive(true);

                extraFoeIcon2.gameObject.SetActive(true);
                extraFoeIcon2.sprite = foeEncounter3.icon;
                //extraFoeBorder2.SetActive(true);

                //gravestone checks
                if(BattlefieldFoes[1].foeDefeated == true)
                {
                    extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                }
                if (BattlefieldFoes[2].foeDefeated == true)
                {
                    extraFoeIcon2.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                }
            }

            if (encounterNumber == 2)
            {
                extraFoeIcon2.gameObject.SetActive(true);
                extraFoeIcon2.sprite = foeEncounter1.icon;
                //extraFoeBorder2.SetActive(true);

                extraFoeIcon1.gameObject.SetActive(true);
                extraFoeIcon1.sprite = foeEncounter3.icon;
                //extraFoeBorder1.SetActive(true);

                if (BattlefieldFoes[0].foeDefeated == true)
                {
                    extraFoeIcon2.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                }
                if (BattlefieldFoes[2].foeDefeated == true)
                {
                    extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                }
            }

            if (encounterNumber == 3)
            {
                extraFoeIcon1.gameObject.SetActive(true);
                extraFoeIcon1.sprite = foeEncounter1.icon;
                //extraFoeBorder1.SetActive(true);

                extraFoeIcon2.gameObject.SetActive(true);
                extraFoeIcon2.sprite = foeEncounter2.icon;
                //extraFoeBorder2.SetActive(true);

                if (BattlefieldFoes[0].foeDefeated == true)
                {
                    extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                }
                if (BattlefieldFoes[1].foeDefeated == true)
                {
                    extraFoeIcon2.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                }
            }
        }

        if (numberOfFoes == 2)
        {
            if (encounterNumber == 1)
            {
                if (foeEncounter2 != null)
                {
                    extraFoeIcon1.gameObject.SetActive(true);
                    extraFoeIcon1.sprite = foeEncounter2.icon;
                    //extraFoeBorder1.SetActive(true);

                    if (BattlefieldFoes[1].foeDefeated == true)
                    {
                        extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    }
                }
                if (foeEncounter3 != null)
                {
                    extraFoeIcon1.gameObject.SetActive(true);
                    extraFoeIcon1.sprite = foeEncounter3.icon;
                    //extraFoeBorder1.SetActive(true);

                    if (BattlefieldFoes[2].foeDefeated == true)
                    {
                        extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    }
                }
            }
            if (encounterNumber == 2)
            {
                if (foeEncounter1 != null)
                {
                    extraFoeIcon1.gameObject.SetActive(true);
                    extraFoeIcon1.sprite = foeEncounter1.icon;
                    //extraFoeBorder1.SetActive(true);

                    if (BattlefieldFoes[0].foeDefeated == true)
                    {
                        extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    }
                }
                if (foeEncounter3 != null)
                {
                    extraFoeIcon1.gameObject.SetActive(true);
                    extraFoeIcon1.sprite = foeEncounter3.icon;

                    if (BattlefieldFoes[2].foeDefeated == true)
                    {
                        extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    }
                    //extraFoeBorder1.SetActive(true);
                }
            }

            if (encounterNumber == 3)
            {
                if (foeEncounter1 != null)
                {
                    extraFoeIcon1.gameObject.SetActive(true);
                    extraFoeIcon1.sprite = foeEncounter1.icon;

                    if (BattlefieldFoes[0].foeDefeated == true)
                    {
                        extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    }
                    //extraFoeBorder1.SetActive(true);
                }
                if (foeEncounter2 != null)
                {
                    extraFoeIcon1.gameObject.SetActive(true);
                    extraFoeIcon1.sprite = foeEncounter2.icon;

                    if (BattlefieldFoes[1].foeDefeated == true)
                    {
                        extraFoeIcon1.sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    }
                    //extraFoeBorder1.SetActive(true);
                }
            }

            extraFoeIcon2.gameObject.SetActive(false);
            extraFoeBorder2.SetActive(false);
        }

        if (numberOfFoes == 1)
        {
            extraFoeIcon1.gameObject.SetActive(false);
            extraFoeBorder1.SetActive(false);

            extraFoeIcon2.gameObject.SetActive(false);
            extraFoeBorder2.SetActive(false);
        }

        //could set the main icon here as well, instead of the other methods
        //display encounter icon
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().ShowIcon3(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.icon);

        //set foe icon here also
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.isCombatEncounter == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SetFoeIcon(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.icon);
        }
    }

    //set the turnnumber of the foe you want to activate
    public void RotateFoeTurn(int turnToGoTo)
    {
        //this isnt technically necessary to make this complex, if we just switch places of the foes, once they get defeated
        Encounter2 foeEncounter = GameManager.ins.references.currentEncounter;
        StrategicEncounter foeStrategic = GameManager.ins.references.currentStrategicEncounter;
        int foeNumber = 1;

        if (turnToGoTo == 1)
        {
            foeEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1;
            foeStrategic = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic1;
            foeNumber = 1;
        }
        else if (turnToGoTo == 2)
        {
            foeEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter2;
            foeStrategic = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic2;
            foeNumber = 2;
        }
        else if (turnToGoTo == 3)
        {
            foeEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter3;
            foeStrategic = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic3;
            foeNumber = 3;
        }

        //lets try swapping these for now
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = foeEncounter;

        //kinda need need second reference too to the actual instantiated strategic encounter unfortunately?
        GameManager.ins.references.currentStrategicEncounter = foeStrategic;
        GameManager.ins.references.currentEncounter = foeEncounter;

        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateIcons(foeNumber);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

        //display encounter icon
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().ShowIcon3(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.icon);

        //set foe icon here also
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().SetFoeIcon(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.icon);

        //need to swap active foe abilities & stats too
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ResetFoeStats(foeNumber);
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ActivateFoeCards(foeNumber);
    }

    //these methods are horrendous
    //when first foe is defeated, swap foe slots and card owners
    //called at the end of RemoveEncounter method (only if theres more than 1 foe)
    public void RemoveFirstFoe()
    {
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().removeFoeFromBattleOnly = false;
        //BattlefieldFoes[0].thisStrategicEncounter.isTemporarilyRemoved = true;

        if (numberOfFoes == 1)
        {
            foeEncounter1 = foeEncounter2;
            foeStrategic1 = foeStrategic2;
            usedButton1 = usedButton2;
            
            foeEncounter2 = null;
            foeStrategic2 = null;
            usedButton2 = null;

            BattlefieldFoes[0].thisStrategicEncounter = BattlefieldFoes[1].thisStrategicEncounter;
            BattlefieldFoes[0].SetBattlefieldFoe(3);
            BattlefieldFoes[1].gameObject.SetActive(false);

        }

        if (numberOfFoes == 2)
        {
            foeEncounter1 = foeEncounter2;
            foeStrategic1 = foeStrategic2;
            usedButton1 = usedButton2;

            foeEncounter2 = foeEncounter3;
            foeStrategic2 = foeStrategic3;
            usedButton2 = usedButton3;

            foeEncounter3 = null;
            foeStrategic3 = null;
            usedButton3 = null;

            BattlefieldFoes[0].thisStrategicEncounter = BattlefieldFoes[1].thisStrategicEncounter;
            BattlefieldFoes[1].thisStrategicEncounter = BattlefieldFoes[2].thisStrategicEncounter;
            BattlefieldFoes[0].SetBattlefieldFoe(2);
            BattlefieldFoes[1].SetBattlefieldFoe(3);
            BattlefieldFoes[2].gameObject.SetActive(false);
        }

        //clears cards belonging to previous first foe
        ClearSpecificFoeAbilities(1);

        ChangeFoeCardOwnership(1);

        //RotateFoeTurn(1);

        //RotateIcons(1);

        foeTurn = 0;
        GoToNextFoe();

        //should reset stats here, in case there are foes of the same type? (since currently they use the same Encounter2 reference)
        //not good in any case, changes were made
        //ResetFoeStats(1);

        //initially lets only show stats of first combat encounter
        foeEncounter1.ShowEnemyStats();
    }

    public void RemoveSecondFoe()
    {
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().removeFoeFromBattleOnly = false;

        if (numberOfFoes == 1)
        {
            //BattlefieldFoes[0].thisStrategicEncounter = BattlefieldFoes[1].thisStrategicEncounter;
            BattlefieldFoes[0].SetBattlefieldFoe(3);
            BattlefieldFoes[1].gameObject.SetActive(false);

            foeEncounter2 = null;
            foeStrategic2 = null;
            usedButton2 = null;
        }

        if (numberOfFoes == 2)
        {
            foeEncounter2 = foeEncounter3;
            foeStrategic2 = foeStrategic3;
            usedButton2 = usedButton3;

            foeEncounter3 = null;
            foeStrategic3 = null;
            usedButton3 = null;

            BattlefieldFoes[0].SetBattlefieldFoe(2);
            BattlefieldFoes[1].thisStrategicEncounter = BattlefieldFoes[2].thisStrategicEncounter;
            BattlefieldFoes[1].SetBattlefieldFoe(3);
            BattlefieldFoes[2].gameObject.SetActive(false);
        }

        //clears cards belonging to specific foe
        ClearSpecificFoeAbilities(2);

        //changes card ownership depending on foe number
        ChangeFoeCardOwnership(2);


        //RotateFoeTurn(2);

        //RotateIcons(2);

        foeTurn = 1;
        GoToNextFoe();
    }

    public void RemoveThirdFoe()
    {
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterBeenResolved = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().removeFoeFromBattleOnly = false;

        foeEncounter3 = null;
        foeStrategic3 = null;
        usedButton3 = null;

        BattlefieldFoes[0].SetBattlefieldFoe(2);
        BattlefieldFoes[1].SetBattlefieldFoe(3);
        //BattlefieldFoes[1].thisStrategicEncounter = BattlefieldFoes[2].thisStrategicEncounter;
        BattlefieldFoes[2].gameObject.SetActive(false);

        //clears cards belonging to specific foe
        ClearSpecificFoeAbilities(3);

        //changes card ownership depending on foe number
        //dont need this for third foe
        //ChangeFoeCardOwnership(3);

        //RotateFoeTurn(1);

        //RotateIcons(1);

        foeTurn = 2;
        GoToNextFoe();
    }

    //similar as combat button function (bit altered actually)
    public void GoToNextFoe()
    {
        // go to combat if theres no more foes to check
        if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 2) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 3 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes == 3))
        {
            /*checks foe hit taken and hit done abilities, hero takes damage per hitstaken
            //if true, hero or foe was defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingDefensePhase() == true)
            {
                return;
            }
            */

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

            /*foe hits first, if it has swiftness
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(2) == true)
            {
                //combat begins (opponent swift)
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
            }
            else
            {
                //combat begins (opponent not swift)
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
            }
            */
            //maybe always go to defense phase actually, after foe is defeated
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToRealTimeCombat(false);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
        }

        //open options for secondary foe(s), if there are any
        else if ((GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 0 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 0) || 
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1) ||
            (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 2 && GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 2))
        {
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().DeleteButtons();

            /*checks foe hit taken and hit done abilities, hero takes damage per hitstaken
            //if true, hero or foe was defeated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChecksWhenGoingDefensePhase() == true)
            {
                return;
            }
            */

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;

            Debug.Log("should open secondary foe options, foeturn is: " + GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn == 1)
            {
                //lets try swapping these for now
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1;

                //kinda need need second reference too to the actual instantiated strategic encounter unfortunately?
                GameManager.ins.references.currentStrategicEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic1;
                GameManager.ins.references.currentEncounter = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeEncounter1;

                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateIcons(1);
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().ActivateFoeCards(1);
            }

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

            //shouldnt go here now?
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

            if(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().initialPhaseDone == true)
            {
                /*foe hits first, if it has swiftness
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(2) == true)
                {
                    //combat begins (opponent swift)
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                }
                else
                {
                    //combat begins (opponent not swift)
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
                }
                */

                //maybe always go to defense phase actually, after foe is defeated
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToCombatWithCheck();
            }

            else
            {
                Debug.Log("should set strategic encounter");
                GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetStrategicEncounter(false);
            }
        }
    }

    //reduces ownership of all foe cards by 1
    public void ChangeFoeCardOwnership(int foeToRemove)
    {
        int numberOfFoeCards = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount;

        if (numberOfFoeCards > 0)
        {
            for (int i = numberOfFoeCards - 1; i >= 0; i--)
            {
                if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).GetComponent<Card>().belongsTo == 2 && foeToRemove == 1)
                {
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).GetComponent<Card>().belongsTo = 1;
                }
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).GetComponent<Card>().belongsTo == 3 && (foeToRemove == 1 || foeToRemove == 2))
                {
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).GetComponent<Card>().belongsTo = 2;
                }
            }
        }
    }

    public void SetFoeTurn(int turnNumber)
    {
        foeTurn = turnNumber;

        RotateFoeTurn(turnNumber);
    }

    /* actually we do this another way
    public int GetModifiedHealth(Encounter2 encounter)
    {
        //advanced
        if (GameManager.ins.startingDifficulty == 2)
        {
            int maxEnergy = (int)(encounter.maxEnergy * 1.25f); 
            return maxEnergy;
        }
        //expert
        if (GameManager.ins.startingDifficulty == 3)
        {
            int maxEnergy = (int)(encounter.maxEnergy * 1.50f);
            return maxEnergy;
        }

        //easy / normal
        return encounter.maxEnergy;
    }
    */
}
