using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider energySlider;
    public Slider actionPointSlider;
    public Slider favorSlider;
    public Slider combatTimerSlider;
    public Slider healthSlider;
    public Slider enemyHealthSlider;

    public CustomTooltips energyBarTooltips;
    public CustomTooltips actionPointBarTooltips;
    public CustomTooltips favorBarTooltips;
    public CustomTooltips combatTimerBarTooltips;
    public CustomTooltips healthBarTooltips;
    public CustomTooltips enemyHealthBarTooltips;

    public GameObject combatTimer;

    //set this on when combat timer is activated
    public bool timerOn;

    private void Update()
    {
        /*actually all of this is unused now?
        if (timerOn == true)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused == false)
            {
                combatTimerSlider.value -= Time.deltaTime;
            }

            if(combatTimerSlider.value == 0)
            {
                //uses basic attack
                if(CardHandler.ins.phaseNumber == 3 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == false)
                {
                    GameManager.ins.references.targettingHandler.TakeBallScore();
                    //GameManager.ins.references.GetComponent<CombatActions>().DefaultAttackButton();

                    //since this is also used for trading backtrack, lets add these here
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingBackButton.SetActive(false);

                    //should destroy the trading canvas objects
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveCombatCards();
                }
                //uses continue button (if foe not defeated)
                else if (CardHandler.ins.phaseNumber == 3 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == false)
                {
                    //should call the ContinueFromBasicAttackCheckButton method here
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.ContinueFromBasicAttackCheckButton();

                    //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
                }
                //uses finish battle button (if foe defeated)
                else if (CardHandler.ins.phaseNumber == 3 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true)
                {
                    //GameManager.ins.references.GetComponent<CombatActions>().FinishBattleButton();
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                }
                //uses basic defend
                else if (CardHandler.ins.phaseNumber == 4 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == false)
                {
                    GameManager.ins.references.targettingHandler.TakeBallScore();
                    GameManager.ins.references.GetComponent<CombatActions>().DefaultDefendButton();

                    //since this is also used for trading backtrack, lets add these here
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingBackButton.SetActive(false);

                    //should destroy the trading canvas objects
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveCombatCards();
                }
                //uses continue button (if hero not defeated)
                else if (CardHandler.ins.phaseNumber == 4 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == false)
                {
                    //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
                    //should call the ContinueFromBasicDefenseCheckButton method here
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.ContinueFromBasicDefenseCheckButton();
                }
                //finished battle (if hero defeated)
                else if (CardHandler.ins.phaseNumber == 4 && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == true)
                {
                    //cant use finish battle method, need to be more creative for now
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
                    GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
                    GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
                    GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();

                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                }

                combatTimerSlider.value = combatTimerSlider.maxValue;
            }
        }
        */
    }

    //can be used for any bar value
    //type 1: energy, 2:ap, 3: favor, 4: health
    public void SetBarValues(int turnNumber)
    {
        //could make sure not to update other players bars
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        if (energyBarTooltips != null)
        {
            energySlider.maxValue = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxEnergy;
            energySlider.value = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().energy;

            energyBarTooltips.customText = "<b>Energy</b><br>Energy is required to use most special actions and abilities. If energy goes to zero, these options cant be used.<br><color=#FFDEA3ff>Current energy: <b>" +
                GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().energy + "/" + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxEnergy + "</b><sprite index=11>";
        }

        if (actionPointBarTooltips != null)
        {
            actionPointSlider.maxValue = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxActionPoints;
            actionPointSlider.value = GameManager.ins.avatars[turnNumber].GetComponent<CharController>().actionPoints;

            actionPointBarTooltips.customText = "<b>Action Points</b><br>Used for movementBonus and various actions. When you run out of action points, your turn ends.<br><color=#FFDEA3ff>Current action points: <b>" +
                GameManager.ins.avatars[turnNumber].GetComponent<CharController>().actionPoints + "/" + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxActionPoints + "</b><sprite index=32>";
        }

        if (favorBarTooltips != null)
        {
            favorSlider.maxValue = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxFavor;
            favorSlider.value = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().favor;

            favorBarTooltips.customText = "<b>Favor</b><br>This indicates whether the Founders owe you any favors. Can be used for divine interventions.<br><color=#FFDEA3ff>Current favor: <b>" +
                GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().favor + "/" + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxFavor + "</b><sprite index=12>";
        }

        if (healthBarTooltips != null)
        {
            healthSlider.maxValue = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxHealth;
            healthSlider.value = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().health;

            healthBarTooltips.customText = "<b>Health</b><br>This indicates your characters health. If health goes below zero, your hero will be defeated.<br><color=#FFDEA3ff>Current health: <b>" +
                GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().health + "/" + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().maxHealth + "</b><sprite=\"sprites v92\" index=3>";
        }

        if (GameManager.ins.references.currentStrategicEncounter != null)//(GameManager.ins.references.currentEncounter != null)
        {
            if (enemyHealthBarTooltips != null)
            {
                enemyHealthSlider.maxValue = GameManager.ins.references.currentStrategicEncounter.maxEnergy;
                enemyHealthSlider.value = GameManager.ins.references.currentStrategicEncounter.currentEnergy;

                enemyHealthBarTooltips.customText = "<b>Health</b><br>This indicates your foes health. If its health goes to zero, it will will be defeated.<br><color=#FFDEA3ff>Current health: <b>" +
                    GameManager.ins.references.currentStrategicEncounter.currentEnergy + "/" + GameManager.ins.references.currentStrategicEncounter.maxEnergy + "</b><sprite=\"sprites v92\" index=3>";
            }
        }
    }

    public void StartCombatTimer()
    {
        if (PlayerPrefs.GetInt("PauseOn") == 1)
        {
            combatTimer.SetActive(true);

            timerOn = false;

            combatTimerSlider.value = combatTimerSlider.maxValue;

            combatTimerBarTooltips.customText = "<b>Combat Timer</b><br>If the timer runs out, the round will be resolved with the current targetting positions and default action.";
        }
        else
        {
            combatTimer.SetActive(true);

            timerOn = true;

            combatTimerSlider.value = combatTimerSlider.maxValue;

            combatTimerBarTooltips.customText = "<b>Combat Timer</b><br>If the timer runs out, the round will be resolved with the current targetting positions and default action.";
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().StartCombatTimer();
    }

    public void RemoveCombatTimer()
    {
        combatTimer.SetActive(false);

        timerOn = false;

        combatTimerSlider.value = combatTimerSlider.maxValue;

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().RemoveCombatTimer();
    }
}
