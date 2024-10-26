using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;


//for handling some of the combat logic
//so everything isnt in encounter buttons & explorehandler & combatHandler
public class CombatActions : MonoBehaviour
{
    //used by attack & defense rolls
    //need to save these separately, since they cant be added to invoke methods
    //too lazy to use coroutine
    int rollCap;
    int rollMin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //modified for v0.5.7.
    public void DefaultAttackButton(GameObject card)
    {
        //for v94
        //GameManager.ins.references.enemyResizing.ActivateFoeBump(2);

        //for v0.5.7.
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].enemyResizing.ActivateFoeBump(2);

        //allow no cards
        //CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //do 10 "dicerolls" before moving onward
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);

        //store the button number separately

        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();
        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        //lets try set phasenumer here in advance
        CardHandler.ins.phaseNumber = 3;

        int targettingBonus = 1;
        //int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;
        if (card.GetComponent<Card>().cardLevel > 1)
        {
            targettingBonus += 1;
        }
        rollCap = 5 + targettingBonus;
        rollMin = targettingBonus;

        //mirror image calculations
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 43) == true)
        {
            int capPenalty = Random.Range(0, 4);
            capPenalty -= (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe - 1) / 2;

            if (capPenalty < 0)
            {
                capPenalty = 0;
            }
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        //small stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 54) == true)
        {
            int capPenalty = Random.Range(0, 2);
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }
        //large stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 55) == true)
        {
            int capBonus = Random.Range(0, 2);
            rollCap = rollCap + capBonus;
            rollMin = rollMin + capBonus;
        }
        //flying check
        //gryphon negates the penalty
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 69) == true &&
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 68) == false)
        {
            int capPenalty = Random.Range(0, 3);
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        if (rollCap > 6)
        {
            rollCap = 6;
        }
        if (rollMin < 1)
        {
            rollMin = 1;
        }

        DefaultAttackDiceroll(card);

    }

    void DefaultAttackDiceroll(GameObject card)
    {
        //string skillCheckText = "";
        /*
        //could do all effects here too?
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().powerAttackActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Power Attack</size>\n<size=8>\n</size><color=#FFD370>";
            //GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(38);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().preciseStrikeActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Precise Strike</size>\n<size=8>\n</size><color=#FFD370>";
            //GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(26);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Extra Strike</size>\n<size=8>\n</size><color=#FFD370>";
            //GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(37);
        }
        else
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Basic Attack</size>\n<size=8>\n</size><color=#FFD370>";
            //GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(28);
        }
        */
        //skillCheckText += "Foe Defense\n";

        //Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        /*
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
        */

        //int roll = 0;
        /*
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            roll = GameManager.ins.references.targettingHandler.score;
            skillCheckText += GameManager.ins.references.targettingHandler.timingText + "\n";
            skillCheckText += GetDice(roll);
        }
        */
        //else

        //lets add new method here to do the dice cap calculations
        //int roll = GetAttackRoll();

        int roll = Random.Range(rollMin, (rollCap + 1));
        int roll2 = Random.Range(rollMin, (rollCap + 1));
        int roll3 = Random.Range(rollMin, (rollCap + 1));

        //for v0.5.7.
        //int numberOfDice = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost;
        int numberOfDice = 1;

        //lightstone blessing check
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 301, 7) > 0)
        {
            numberOfDice += 1;
        }

        if (card.GetComponent<Card>().cardLevel > 2)
        {
            numberOfDice += 1;
        }

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

        //skillCheckText += "Roll\n";
        //skillCheckText += "Roll (" + rollMin + "-" + rollCap + ")\n";

        /* dont need "diceroll" or text in v 0.5.7.
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

        //skillCheckText += "Roll (" + rollMin + "-" + rollCap + ")\n";
        //skillCheckText += GetDice(roll);

        //show big dice for the best roll
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            skillCheckText += "<size=18>" + GetDice(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll) + "</size>";
        }

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounter1Text.text = skillCheckText;

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount < 10)
        {
            Invoke(nameof(DefaultAttackDiceroll), 0.12f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
        }
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
        {
            Invoke(nameof(DefaultAttackDiceroll), 0.8f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll = bestRoll;
            //ContinueSkillCheck(skillCheckText, bestRoll);
        }
        
        //this is kinda drunken way of doing this, but lets see if it works
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            ContinueDefaultAttack(skillCheckText, GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll);
        }
        */

        //should we send card reference here, instead of using those flag variables?
        ContinueDefaultAttack(bestRoll);
    }

    void ContinueDefaultAttack(int bestRoll)
    {
        //could do all effects here too?
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().powerAttackActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Power Attack</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(38, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().preciseStrikeActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Precise Strike</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(26, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Extra Strike</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(37, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Basic Attack</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(28, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }

        //caps skillpoints at 6 here
        int hits = bestRoll + GetSkillPoints(1) - GameManager.ins.references.currentStrategicEncounter.defense;

        //add precise strike modifier here, if its activated
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().preciseStrikeActivated == true)
        {
            hits += 2;
        }

        //see if foe has activated dodge (need to do this before other checks?)
        //modified for v0.5.7.
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget -1].CheckFoeActiveAbility(16) == true)//GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(16) && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeDefensePhase == true)
        {
            hits = hits - 2;
        }

        /* these are alrdy checked?
         * small stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 54) == true)
        {
            hits = hits - 1;
        }
        //large stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 55) == true)
        {
            hits = hits + 1;
        }
        //flying check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 69) == true)
        {
            hits = hits - 1;
        }
        */

        if (hits < 1)
        {
            hits = 0;
        }

        //skillCheckText += "\n<size=8>\n</size>Damage Done\n";

        //this is only used for sfx here?
        bool isSuccess = false;

        //need to determine attack type for reroll button
        //type 1 is default attack
        int attackTypeUsed = 1;

        //special cases
        if (bestRoll == 1)
        {
            //skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;

            hits = 0;
        }

        //always give at least 1 hit on crit
        if (bestRoll == 6 && hits == 0)
        {
            //skillCheckText += "<color=green>";

            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = true;

            hits = 1;
        }

        //get default damage modifier
        //float damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier;
        float damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalStrengthModifier;

        //combine strengthmodifier with possible power attack
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().powerAttackActivated == true)
        {
            damageModifier += 50f * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalStrengthModifier / 100f;
        }

        //add bullseye check here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 163, 2) > 0 && bestRoll == 6)
        {
            hits += 1;
            damageModifier += 25f;

            //lets use the third sfx holder for this (the first two might get taken)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[163].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //add berserker check here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().berserkActivated == true)
        {
            hits += 1;
            damageModifier += 25f;
        }

        //check melee damage modifier (rounded up)
        float strModifiedFloat = Mathf.Ceil(hits * damageModifier / 100f);
        hits = (int)strModifiedFloat;

        //checks special foe protection abilities
        //also cooldown abilities, except dodge
        hits = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckFoeProtectionAbilities(hits, attackTypeUsed);

        //other cases
        if (bestRoll > 1)
        {
            for (int i = 0; i < hits; i++)
            {
                //use energy icons hits
                //skillCheckText += "<sprite=\"sprites v92\" index=3>";
            }

            if (hits >= 1)
            {
                //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                isSuccess = true;
            }
            else
            {
                //skillCheckText += "<color=red>No damage</color>";
                isSuccess = false;
            }
        }

        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().BasicAttack(isSuccess, attackTypeUsed, hits, bestRoll);

        //lets try set phasenumer here in advance
        CardHandler.ins.phaseNumber = 3;

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 1);

    }

    public void ArcaneAttackButton(GameObject card)
    {
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].enemyResizing.ActivateFoeBump(2);

        //allow no cards
        //CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //string skillCheckText = "";

        //do 10 "dicerolls" before moving onward
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);

        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();
        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        //lets try set phasenumer here in advance
        //CardHandler.ins.phaseNumber = 3;

        //skillCheckText += "Foe Resistance\n";

        //Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        /*
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

        //int roll = 0;
        
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            roll = GameManager.ins.references.targettingHandler.score;
            skillCheckText += GameManager.ins.references.targettingHandler.timingText + "\n";
            skillCheckText += GetDice(roll);
        }
        */
        //else
        //int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;

        int targettingBonus = 1;
        if (card.GetComponent<Card>().cardLevel > 1)
        {
            targettingBonus += 1;
        }

        rollCap = 5 + targettingBonus;
        rollMin = targettingBonus;

        //mirror image calculations
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 43) == true)
        {
            int capPenalty = Random.Range(0, 4);
            capPenalty -= (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe - 1) / 2;

            if (capPenalty < 0)
            {
                capPenalty = 0;
            }
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        //small stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 54) == true)
        {
            int capPenalty = Random.Range(0, 2);
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }
        //large stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 55) == true)
        {
            int capBonus = Random.Range(0, 2);
            rollCap = rollCap + capBonus;
            rollMin = rollMin + capBonus;
        }
        //melee tactician check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 70) == true)
        {
            int capPenalty = Random.Range(0, 3);
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        if (rollCap > 6)
        {
            rollCap = 6;
        }
        if (rollMin < 1)
        {
            rollMin = 1;
        }

        ArcaneAttackDiceroll(card);

        //roll = Random.Range(rollMin, (rollCap + 1));
        //skillCheckText += "Roll (" + rollMin + "-" + rollCap + ")\n";
        //skillCheckText += GetDice(roll);

    }

    void ArcaneAttackDiceroll(GameObject card)
    {
        int roll = Random.Range(rollMin, (rollCap + 1));
        int roll2 = Random.Range(rollMin, (rollCap + 1));
        int roll3 = Random.Range(rollMin, (rollCap + 1));

        //for v95
        //int numberOfDice = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost;
        int numberOfDice = 1;

        //lightstone blessing check
        if(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 301, 7) > 0)
        {
            numberOfDice += 1;
        }

        if (card.GetComponent<Card>().cardLevel > 2)
        {
            numberOfDice += 1;
        }

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

        ContinueArcaneAttack(bestRoll);
    }

    void ContinueArcaneAttack(int bestRoll)
    {

        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Arcane Barrage</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(18, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Arcane Orb</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(39, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
            //Debug.Log("should call arcane orb effect");
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Bolt of light</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(12, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Spear of light</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(40, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Arcane Attack</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(17, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }

        int hits = 0;

        //special cases
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated)
        {
            float loreFloat = Mathf.Ceil(GetSkillPoints(8) / 2f);
            int halvedLore = (int)loreFloat;
            int finalHolyAttack = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyAttack + halvedLore;
            hits = bestRoll + finalHolyAttack - GameManager.ins.references.currentStrategicEncounter.resistance;
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated)
        {
            float loreFloat = Mathf.Ceil(GetSkillPoints(8) / 2f);
            int halvedLore = (int)loreFloat;
            int finalHolyAttack = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyAttack + halvedLore;
            hits = bestRoll + finalHolyAttack - GameManager.ins.references.currentStrategicEncounter.resistance + 2;
        }
        //earth magic check
        else if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 153, 7) > 0)
        {
            hits = bestRoll + GetSkillPoints(3) - GameManager.ins.references.currentStrategicEncounter.defense;
        }
        else
        {
            hits = bestRoll + GetSkillPoints(3) - GameManager.ins.references.currentStrategicEncounter.resistance;
        }

        //add precise strike modifier here, if its activated
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true)
        {
            hits += 2;
        }

        //see if foe has activated dodge (need to do this before other checks?)
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].CheckFoeActiveAbility(16) == true)
        {
            hits = hits - 2;
        }

        if (hits < 1)
        {
            hits = 0;
        }

        //skillCheckText += "\n<size=8>\n</size>Damage Done\n";

        //this is only used for sfx here?
        bool isSuccess = false;

        //need to determine attack type for reroll button
        //type 1 is default attack, type 2 is arcane attack
        //need separate variable to check enemy protections because of possible earth magic
        int attackTypeUsed = 2;
        int channelToCheck = 2;

        //special cases
        if (bestRoll == 1)
        {
            //skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;

            hits = 0;
        }

        //always give at least 1 hit on crit
        if (bestRoll == 6 && hits == 0)
        {
            //skillCheckText += "<color=green>";

            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = true;

            hits = 1;
        }


        float damageModifier = 100f;

        //divine attack cases
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated)
        {
            damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalHolyModifier;

            //undead check
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(48) == true)
            {
                damageModifier += 50f * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalHolyModifier / 100f;
            }
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated)
        {
            damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalHolyModifier;
            damageModifier += 50f * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalHolyModifier / 100f;

            //undead check
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(48) == true)
            {
                damageModifier += 50f * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalHolyModifier / 100f;
            }
        }

        //arcane attack cases
        else
        {
            //earth magic check
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 153, 7) > 0)
            {
                channelToCheck = 1;
            }

            //get default damage modifier
            //damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier;
            damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalArcanePowerModifier;

            //combine strengthmodifier with possible power attack
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
            {
                damageModifier += 100f * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalArcanePowerModifier / 100f;
            }
        }

        //add bullseye check here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 163, 2) > 0 && bestRoll == 6)
        {
            hits += 1;
            damageModifier += 25f;

            //lets use the third sfx holder for this (the first two might get taken)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[163].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }


        //check melee damage modifier (rounded up)
        float strModifiedFloat = Mathf.Ceil(hits * damageModifier / 100f);
        hits = (int)strModifiedFloat;

        hits = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckFoeProtectionAbilities(hits, channelToCheck);

        //other cases
        if (bestRoll > 1)
        {
            for (int i = 0; i < hits; i++)
            {
                //use energy icons hits
                //skillCheckText += "<sprite=\"sprites v92\" index=3>";
            }

            if (hits >= 1)
            {
                //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                isSuccess = true;
            }
            else
            {
                //skillCheckText += "<color=red>No damage</color>";
                isSuccess = false;
            }
        }

        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().BasicAttack(isSuccess, attackTypeUsed, hits, bestRoll);

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 3);

    }

    public void ThrowBombButton(GameObject card)
    {
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].enemyResizing.ActivateFoeBump(2);

        //allow no cards
        //CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //string skillCheckText = "";

        //do 10 "dicerolls" before moving onward
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);

        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();
        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        //lets try set phasenumer here in advance
        //CardHandler.ins.phaseNumber = 3;

        //skillCheckText += "Foe Defense\n";

        //Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        //Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //int roll = 0;
        //int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;

        int targettingBonus = 1;
        if (card.GetComponent<Card>().cardLevel > 1)
        {
            targettingBonus += 1;
        }
        rollCap = 5 + targettingBonus;
        rollMin = targettingBonus;

        //mirror image calculations
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 43) == true)
        {
            int capPenalty = Random.Range(0, 4);
            capPenalty -= (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe - 1) / 2;

            if (capPenalty < 0)
            {
                capPenalty = 0;
            }
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        //small stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 54) == true)
        {
            int capPenalty = Random.Range(0, 2);
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }
        //large stature check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 55) == true)
        {
            int capBonus = Random.Range(0, 2);
            rollCap = rollCap + capBonus;
            rollMin = rollMin + capBonus;
        }
        //melee tactician check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 70) == true)
        {
            int capPenalty = Random.Range(0, 3);
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        if (rollCap > 6)
        {
            rollCap = 6;
        }
        if (rollMin < 1)
        {
            rollMin = 1;
        }

        ThrowBombDiceroll(card);

    }

    void ThrowBombDiceroll(GameObject card)
    {
        int roll = Random.Range(rollMin, (rollCap + 1));
        int roll2 = Random.Range(rollMin, (rollCap + 1));
        int roll3 = Random.Range(rollMin, (rollCap + 1));

        //for v95
        //int numberOfDice = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost;
        int numberOfDice = 1;

        //lightstone blessing check
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 301, 7) > 0)
        {
            numberOfDice += 1;
        }

        if (card.GetComponent<Card>().cardLevel > 2)
        {
            numberOfDice += 1;
        }

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

        ContinueThrowBomb(bestRoll);
    }

    void ContinueThrowBomb(int bestRoll)
    {

        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Throw Bomb</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(9, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Throw Manabomb</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(3, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Detonation</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(11, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Arcane Detonation</size>\n<size=8>\n</size><color=#FFD370>";
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(5, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
        }
        else
        {
            //skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Bomb</size>\n<size=8>\n</size><color=#FFD370>";
        }

        int hits = 0;

        //need to determine attack type for reroll button
        //type 3 is throw bombs
        int attackTypeUsed = 3;

        //1 is physical bombs, 2 is magical bombs
        int bombTypeUsed = 1;

        //could make separate check for each bomb type here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated == true)
        {
            //make it like this, so the odd numbers are rounded up
            float mechFloat = Mathf.Ceil(GetSkillPoints(6) / 2f);
            int halvedMech = (int)mechFloat;
            int finalBombAttack = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack + halvedMech;
            hits = bestRoll + finalBombAttack - GameManager.ins.references.currentStrategicEncounter.defense;

            //caps skillpoints at 6 here
            //hits = roll + (GetSkillPoints(6) / 2) + 1 - encounter.defense;

            bombTypeUsed = 1;
        }
        //could make separate check for each bomb type here
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true)
        {
            float mechFloat = Mathf.Ceil(GetSkillPoints(6) / 2f);
            int halvedMech = (int)mechFloat;
            int finalBombAttack = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack + halvedMech;
            hits = bestRoll + finalBombAttack - GameManager.ins.references.currentStrategicEncounter.resistance;

            bombTypeUsed = 2;
        }
        //could make separate check for each bomb type here
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true)
        {
            float mechFloat = Mathf.Ceil(GetSkillPoints(6) / 2f);
            int halvedMech = (int)mechFloat;
            int finalBombAttack = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack + halvedMech;
            hits = bestRoll + finalBombAttack + 2 - GameManager.ins.references.currentStrategicEncounter.defense;

            bombTypeUsed = 1;
        }
        //could make separate check for each bomb type here
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
        {
            float mechFloat = Mathf.Ceil(GetSkillPoints(6) / 2f);
            int halvedMech = (int)mechFloat;
            int finalBombAttack = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombAttack + halvedMech;
            hits = bestRoll + finalBombAttack + 2 - GameManager.ins.references.currentStrategicEncounter.resistance;

            bombTypeUsed = 2;
        }

        /* not needed in v93
         * special case for grenadier
        if(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 157, 2) > 0)
        {
            hits += 1;
        }
        */

        //see if foe has activated dodge (need to do this before other checks?)
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].CheckFoeActiveAbility(16) == true)
        {
            hits = hits - 2;
        }

        if (hits < 1)
        {
            hits = 0;
        }

        //skillCheckText += "\n<size=8>\n</size>Damage Done\n";

        //this is only used for sfx here?
        bool isSuccess = false;


        //special cases
        if (bestRoll == 1)
        {
            //skillCheckText += "<color=red>Fumble</color>";

            //skillCheckText += "\n<size=8>\n</size><color=red>Failure";
            isSuccess = false;

            hits = 0;
        }

        //always give at least 1 hit on crit
        if (bestRoll == 6 && hits == 0)
        {
            //skillCheckText += "<color=green>";

            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            isSuccess = true;

            hits = 1;
        }

        //get default damage modifier
        //float damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombModifier;
        float damageModifier = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalBombModifier;

        //additional modifier for detonations
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
        {
            damageModifier += 50f * GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalBombModifier / 100f;
        }

        //add bullseye check here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 163, 2) > 0 && bestRoll == 6)
        {
            hits += 1;
            damageModifier += 25f;

            //lets use the third sfx holder for this (the first two might get taken)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[163].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //translate into hits
        float strModifiedFloat = Mathf.Ceil(hits * damageModifier / 100f);
        hits = (int)strModifiedFloat;

        //checks special foe protection abilities
        //also cooldown abilities
        //actually need check for different kinds of attack types
        hits = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckFoeProtectionAbilities(hits, bombTypeUsed);

        //other cases
        if (bestRoll > 1)
        {
            for (int i = 0; i < hits; i++)
            {
                //use energy icons hits
                //skillCheckText += "<sprite=\"sprites v92\" index=3>";
            }

            if (hits >= 1)
            {
                //skillCheckText += "\n<size=8>\n</size><color=green>Success";
                isSuccess = true;
            }
            else
            {
                //skillCheckText += "<color=red>No damage</color>";
                isSuccess = false;
            }
        }

        //handles skillcheck display 
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().BasicAttack(isSuccess, attackTypeUsed, hits, bestRoll);

        //lets try set phasenumer here in advance
        //CardHandler.ins.phaseNumber = 3;

        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 16);

    }

    //remove this whole chain in v0.5.7. (repurposed in BattlefieldFoe class)
    //used against default attacks of the foe (both physical & magical)
    public void DefaultDefendButton()
    {
        //for v94
        GameManager.ins.references.enemyResizing.ActivateFoeBump(2);

        //allow no cards
        CardHandler.ins.SetUsables(0);

        GameManager.ins.toolTipBackground.SetActive(false);

        //string skillCheckText = "";

        //do 10 "dicerolls" before moving onward
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount = 0;

        //kinda wanna close these
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);

        //GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayDiceroll();

        //lets try set phasenumer here in advance
        CardHandler.ins.phaseNumber = 4;

        //skillCheckText += "Foe Attack\n";

        //int roll = 0;

        int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;
        rollCap = 5 + targettingBonus;
        rollMin = targettingBonus;

        //phantom attacks calculations
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(45) == true)
        {
            int capPenalty = Random.Range(0, 4);
            capPenalty -= (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe - 1) / 2;

            if (capPenalty < 0)
            {
                capPenalty = 0;
            }
            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        //swift foe
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(2))
        {
            int penalty = 1;

            //bigger penalty if hero isnt mounted
            if (CardHandler.ins.mountSlot.transform.childCount == 0)
            {
                penalty = 2;
            }
            int capPenalty = Random.Range(0, penalty + 1);

            rollCap = rollCap - capPenalty;
            rollMin = rollMin - capPenalty;
        }

        //slow foe
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(35))
        {
            int bonus = 1;

            //bigger bonus if hero is mounted
            if (CardHandler.ins.mountSlot.transform.childCount > 0)
            {
                bonus = 2;
            }
            int capBonus = Random.Range(0, bonus + 1);

            rollCap = rollCap + capBonus;
            rollMin = rollMin + capBonus;
        }

        if (rollCap > 6)
        {
            rollCap = 6;
        }
        if (rollMin < 1)
        {
            rollMin = 1;
        }

        //roll = Random.Range(rollMin, (rollCap + 1));
        //skillCheckText += "Roll (" + rollMin + "-" + rollCap + ")\n";
        //skillCheckText += GetDice(roll);

        DefendDiceroll();
    }

    void DefendDiceroll()
    {
        string skillCheckText = "";

        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Defend with Parry</size>\n<size=8>\n</size><color=#FFD370>";
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().blockActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Defend with Supershield</size>\n<size=8>\n</size><color=#FFD370>";
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Defend with Ward</size>\n<size=8>\n</size><color=#FFD370>";
        }
        else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().shieldOfIsoloreActivated == true)
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Shield of Isolore</size>\n<size=8>\n</size><color=#FFD370>";
        }
        else
        {
            skillCheckText = "<br><br><br><br><br><br><br><br><size=18>Defend</size>\n<size=8>\n</size><color=#FFD370>";
        }


        Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        int roll = Random.Range(rollMin, (rollCap + 1));
        int roll2 = Random.Range(rollMin, (rollCap + 1));
        int roll3 = Random.Range(rollMin, (rollCap + 1));

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

        //skillCheckText += "Roll\n";
        skillCheckText += "Roll (" + rollMin + "-" + rollCap + ")\n";

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
            Invoke(nameof(DefendDiceroll), 0.12f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
        }
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 10)
        {
            Invoke(nameof(DefendDiceroll), 0.8f);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount += 1;
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll = bestRoll;
            //ContinueSkillCheck(skillCheckText, bestRoll);
        }
        //this is kinda drunken way of doing this, but lets see if it works
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().diceRollCount == 11)
        {
            ContinueDefendDiceroll(skillCheckText, GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().bestRoll);
        }
    }

    void ContinueDefendDiceroll(string skillCheckText, int bestRoll)
    {

        Encounter2 encounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.GetComponent<Encounter2>();
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        int attack = GameManager.ins.references.currentStrategicEncounter.attack;
        int arcanePower = GameManager.ins.references.currentStrategicEncounter.arcanePower;

        //lets use this for foe special damage attacks too
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().specialButtons[0].GetComponent<EncounterButton>().useDefaultAttackValuesFS == true)
            {
                attack = GameManager.ins.references.currentStrategicEncounter.attack;
                arcanePower = GameManager.ins.references.currentStrategicEncounter.arcanePower;

                //special case for precise strike
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(71) == true)
                {
                    attack += 2;
                }
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


        int hits = 0;

        if (attack != 0)
        {
            /* actually lets remove this in v95
                check if hero has ward activated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) > 0)
            {
                //lets switch attack and arcane power scores in this case
                //actually lets just use resistance value if ward is activated on v92
                //arcanePower = attack;
                //attack = 0;
                hits = attack - (bestRoll + GetSkillPoints(4));
            }
            */
            //else

            hits = attack - (bestRoll + GetSkillPoints(2));
            
        }
        else if (arcanePower != 0)
        {
            hits = arcanePower - (bestRoll + GetSkillPoints(4));
        }

        //check dodge here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated == true)
        {
            hits -= 2;
        }

        //then check smoke bomb
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated == true)
        {
            hits -= 1;
        }

        //then check ward
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)// && attack == 0)
        {
            hits -= 1;
        }

        if (hits < 1)
        {
            hits = 0;
        }


        skillCheckText += "\n<size=8>\n</size>Damage Taken\n";

        //this is only used for sfx here?
        //bool isSuccess = false;

        //need to determine attack type for reroll button
        //type 1 is default attack (either physical or magical in this case)
        //int enemyAttackTypeUsed = 1;

        //need to determine attack type for reroll button
        //type 1 is default attack
        //int attackTypeUsed = 1;

        //special cases (always take hit on fumble)
        if (bestRoll == 1)
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
        if (bestRoll == 6)
        {
            //skillCheckText += "<color=green>";
            //skillCheckText += "\n<size=8>\n</size><color=green>Success";
            //isSuccess = true;

            hits = 0;
        }

        //special cases for heavy hitters
        //dismiss this if special phase, and foe doesnt have precise strike
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == false ||
            (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(71)))
        {
            //change this for v94
            if (hits > 0)
            {
                float damageModifier = 100f;

                if (attack != 0)
                {
                    damageModifier = GameManager.ins.references.currentEncounter.attackMod;
                }
                if (arcanePower != 0)
                {
                    damageModifier = GameManager.ins.references.currentEncounter.arcanePowerMod;
                }

                //round up the damage reduction?
                float newDamage = Mathf.Ceil(hits * (damageModifier / 100));
                hits = (int)newDamage;
            }

            /* old stuff
             * 
             * 50%
            if (hits > 0 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(4) == true)
            {
                //round hitsdone / 2 upwards
                float hitsFloat = Mathf.Ceil(hits / 2f);
                hits += (int)hitsFloat;
            }
            //25%
            if (hits > 0 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(52) == true)
            {
                //round hitsdone / 2 upwards
                float hitsFloat = Mathf.Ceil(hits / 4f);
                hits += (int)hitsFloat;
            }
            */
        }

        //special phase damage modifiers
        //also handle the effects here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            //1.5x damage (shadow bolt, bombardment, heavy strike, fireball, fire breath, frost breath, frost bolts)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(15) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(17) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(28) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(47) ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(60) || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(62) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(67) || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(68))
            {
                //round hitsdone / 2 upwards
                float hitsFloat = Mathf.Ceil(hits / 2f);
                hits += (int)hitsFloat;
            }

            //2x damage (greater fireball, shadow strike, powerful charge)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(36) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(44) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(59) == true)
            {
                //round hitsdone / 2 upwards
                //float hitsFloat = Mathf.Ceil(hits / 2f);
                hits = hits * 2;
            }
            //shadow bolt (also greater shadow bolt)
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(15) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(18, 4);
            }
            //bombard  & toxic vial
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(17) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(30) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(9, 4);
            }
            //power strike, devastating strike, precise strike
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(28) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(59) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(71) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(38, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
            }
            //fireball
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(47) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(0, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
            }
            //greater fireball, fire breath
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(36) == true || GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(60))
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(2, 4);
            }
            //shadow strike
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(44) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(18, 4);
            }
            //frost breath
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(62) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(5, 4);
            }
            //frost bolts
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(67) == true ||
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(68) == true)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(3, 4);
            }
        }

        //effects for non-specials
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == false)
        {
            //physical attack
            if (arcanePower == 0)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(28, 4);
            }
            //arcane attack
            if (attack == 0)
            {
                GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(17, 4);
            }
        }

        //check heros damage reductions last
        //actually dont need to make alterations for these at v92?
        //smoke bombs 2 is also checked here now
        if (hits != 0 && arcanePower == 0) // && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == false)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) == 0)
        {
            //physical damage type
            hits = CheckProtections(1, hits);
        }
        if (hits != 0 && attack == 0) // || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) > 0)
        {
            //magic damage type
            hits = CheckProtections(3, hits);
        }

        //check block here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().blockActivated == true)
        {
            hits = hits / 2;
        }

        //check ward again here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)// && attack == 0)
        {
            hits = hits * 3 / 4;
        }

        //check shield of isolore here
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().shieldOfIsoloreActivated == true)
        {
            hits = 0;
        }

        //dont need roll check?
        for (int i = 0; i < hits; i++)
        {
            skillCheckText += "<sprite=\"sprites v92\" index=3>";
        }

        /*
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
        */


        //handles skillcheck display (unused now)
        //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().PV.RPC("RPC_BasicDefend", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess, enemyAttackTypeUsed, hits);

        //lets try set phasenumer here in advance
        //CardHandler.ins.phaseNumber = 4;

        //show different animation depending on outcome
        //hit animation
        if (hits > 0 && hits <= character.health)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 14);
        }
        //KO animation
        else if (hits > 0 && hits > character.health)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 15);
        }
        //no hits animation
        else if (hits == 0)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, 10);
        }

    }

    //flee from combat
    public void FleeButton()
    {
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);

        //for v0.5.7.
        //need to remove old combat buttons
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveCombatCards();
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        int fleeCost = 1;

        //special case if theres any opponent with swiftness, and hero isnt mounted
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(2) &&
            CardHandler.ins.mountSlot.transform.childCount == 0)
        {
            fleeCost = 2;
        }
        //special case if all foes are slow
        else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckIfAllFoesHaveSpecificAbility(35))
        {
            fleeCost = 0;
        }
        else
        {
            fleeCost = 1;
        }

        //take health if hero doesnt have enough energy
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= fleeCost)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -fleeCost);
        }
        else if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < fleeCost &&
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy > 0)
        {
            int formerEnergy = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -formerEnergy);
            fleeCost -= formerEnergy;

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -fleeCost);
        }
        else
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -fleeCost);
        }


        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_FleeFromCombat", RpcTarget.AllBufferedViaServer);

        //Invoke("FleeWithDelay", 0.4f);
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
    }

    public string GetDice(int roll)
    {
        if (roll == 1)
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

    //this also caps skills
    public int GetSkillPoints(int skillType)
    {
        int skillpoints = 0;

        if (skillType == 1)
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
        if (skillpoints > 6)
        {
            return 6;
        }
        if (skillpoints < -3)
        {
            //removed for v94
            return -3;
        }
        return skillpoints;
    }

    public int CheckProtections(int damageType, int hits)
    {
        float smokeBombModifier = 0f;
        //better do this here, so theres no potential for bugs (otherwise if you upgrade the skill while its active, you could lose modifiers permanently)
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 232, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated == true)
        {
            /*need cardlevel check too though
             * althoguh this should alrdy be done, if the smokebomb has that cardnumber now?
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 152, 2);

            if (cardToCheck.gameObject.GetComponent<Card>().cardLevel == 2)
            {
                smokeBombModifier = 10f;
            }
            */
            smokeBombModifier = 10f;
        }

        if (damageType == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalDefenseModifier != 0)
        {
            //check defense damage modifier (rounded down)
            float defModifiedFloat = hits - (hits * ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalDefenseModifier + smokeBombModifier) / 100f));
            int newHits = (int)defModifiedFloat;

            Debug.Log("newHits is: " + newHits);

            return newHits;
        }

        if (damageType == 3 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalResistanceModifier != 0)
        {
            //check defense damage modifier (rounded down)
            //float defModifiedFloat = Mathf.Ceil(hits - GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier / 100f);
            float resModifiedFloat = hits - (hits * ((GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().finalResistanceModifier + smokeBombModifier) / 100f));
            int newHits = (int)resModifiedFloat;

            Debug.Log("newHits is: " + newHits);

            return newHits;
        }

        return hits;
    }

    public void FinishBattleWithHeroDefeated()
    {
        GameManager.ins.toolTipBackground.SetActive(false);

        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = true;

        //need to remove effects too
        CardHandler.ins.RemoveCombatEffectsFromHero();

        Invoke(nameof(LeaveBattlefield), 1.5f);

        //set this to true when using finishbattlebutton
        //need to manually set this each time when checking fight button
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference = true;

        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
    }

    //copied from encounterbutton class
    //used when hero or foe is defeated
    //could dedicate this for foe defeats in v0.5.7.
    //we should give this foenumber variable? (and remove the opponent defeated check?)
    public void FinishBattleWithFoeDefeated(int foeNumber)
    {
        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1];

        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        //put these just in case
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        //pause battle in v0.5.7.
        //need to unpause somewhere after 1.5s if theres still foes living
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = true;

        //show the foe gravestone always when foe defeated in battlefield?
        if (battlefieldFoe.foeDefeated == true)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == true)
        {
            battlefieldFoe.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
            battlefieldFoe.enemyResizing.ActivateFoeBump(1);

            //remove possible float here too
            battlefieldFoe.RemoveFloat();
        }

        //close enemy hp bar display here?
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyEnergyDisplay.SetActive(false);

        //lets make check here whether we using attack which negates undying
        //need to be done before resetting the flags
        bool undyingNegate = false;

        //various ways to negate undying
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().attackTypeUsed == 2 || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true ||
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 113, 7) != 0 ||
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 114, 7) != 0 || CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 4, 33) == true)
        {
            undyingNegate = true;
        }

        //resets flags for hero special attacks (dont remove time warp in v0.5.7.)
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetHeroAttackFlags();
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 151, 7, 1);

        //reduce special ability etc timers
        //returns true, if hero was defeated (by immolation or such), or when extra strike / time warp was used
        //need to check that hero isnt knocked out yet, otherwise it can result in loop
        /* remove this for now
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().OncePerRoundHeroEffects() == true)
        {
            return;
        }
        */

        /* this shouldnt be active anyway
         * remove targetting window
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
        GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
        GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
        GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
        */

        //Debug.Log("should remove targetting window");

        //undying check here (could as well make secondPhase != null check, but whatever)
        //could check attack type, manabomb & blessed weapons, to possibly nullify undying
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(battlefieldFoe.foeNumber, 8) && battlefieldFoe.foeDefeated == true && undyingNegate == false)
        {
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_SetSecondPhaseEncounter", RpcTarget.AllBufferedViaServer);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetSecondPhaseEncounter(battlefieldFoe.foeNumber, false);
            return;
        }

        //second wind check
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(battlefieldFoe.foeNumber, 75) && battlefieldFoe.foeDefeated == true)
        {
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_SetSecondPhaseEncounter", RpcTarget.AllBufferedViaServer);
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetSecondPhaseEncounter(battlefieldFoe.foeNumber, true);
            battlefieldFoe.thisStrategicEncounter.secondWindActivated = true;
            Invoke(nameof(LeaveBattlefield), 1.5f);
            return;
        }

        //remove combat effects from hero (but only if theres no more enemies)
        //why is this set to 2?
        //we could use the number of foes variable to tell how many foes are alive? but we might need different variable to tell original number of foes?
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes < 2)
        {
            CardHandler.ins.RemoveCombatEffectsFromHero();
            //LeaveBattlefield();
            Invoke(nameof(LeaveBattlefield), 1.5f);
        }

        //set this to true when using finishbattlebutton
        //need to manually set this each time when checking fight button
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().useCombatButtonReference = true;

        //special case for victory rush
        if (battlefieldFoe.foeDefeated == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(7))
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 2);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 2);
        }

        //see if hero has life drain, and you used arcane attack
        //need machine check here later
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().attackTypeUsed == 2 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 165, 2) > 0)
        {
            //undead check
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(battlefieldFoe.foeNumber, 48) == false)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone / 4);
            }
        }

        

        //lets swap the icon here
        if (battlefieldFoe.foeDefeated == true)
        {
            //this shouldnt be called here tho, since we dont want to leave battlefield display untill all foes are defeated in v0.5.7.
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);

            //explosive death check
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 242) == true)
            {
                //dont swap the icon yet for explosive death foes
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);

                GameObject foeCard = CardHandler.ins.CopyFoeCard(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 242);
                TriggerFoeCardSkillCheck(foeCard, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
            }
            //poison explosive death
            else if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 243) == true)
            {
                //dont swap the icon yet for explosive death foes
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
                GameObject foeCard = CardHandler.ins.CopyFoeCard(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 243);
                TriggerFoeCardSkillCheck(foeCard, GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);
            }
            /*
            else
            {
                //we need a reference to the other foe images too to put as gravestones when needed
                GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
            }
            */
            //can put gravestones to explosive foes now as well
            GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
            GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
        }

        //for v0.5.7.
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes > 1)
        {
            battlefieldFoe.foeDefeated = true;
            battlefieldFoe.FoeDisplays.SetActive(false);

            if (foeNumber == GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget += 1;
            }
            //this variable shouldnt really be used?
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = false;
            //we should rly remove rpc call from this
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore4", RpcTarget.AllBufferedViaServer);
            //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().CancelExplore4();
            //this should probably have foenumber variable
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveOneFoe(battlefieldFoe.foeNumber);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = false;

            SetGravestone(battlefieldFoe.foeNumber);
            //need to return here, so the next method wont be called
            return;
        }

        //when all foes defeated
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes < 2)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveOneFoe(battlefieldFoe.foeNumber);

            SetGravestone(battlefieldFoe.foeNumber);

            //start foe counter from 1 when handling the finish encounter buttons
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn = 1;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().allFoesDefeated = true;

            //its bit unfortunate if we need to use this tho
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = true;

            //lets remove foe info button here
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyInfoButton.SetActive(false);

            //should we still use this method? (need to clean it at least)
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
        }
    }

    public void SetGravestone(int foeNumber)
    {
        if(foeNumber == 1)
        {
            GameManager.ins.references.enemyResizing.foeImageObject.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
            GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
        }
        if (foeNumber == 2)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().extraFoeIcon1.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
        }
        if (foeNumber == 3)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().extraFoeIcon2.GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
        }
    }

    //actually we should trigger all the foe final buttons here (in order)
    public void LeaveBattlefield()
    {
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().SwitchBattlefieldDisplay(false);
    }


    //for v0.5.7.
    //could be used for handling foe abilities & skills (instantly)
    public void TriggerFoeCardSkillCheck(GameObject foeCard, int foeNumber)
    {
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().DisableEncounterButtons();

        int roll = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);
        int roll2 = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);
        int roll3 = Random.Range(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost, 7);

        int numberOfDice = 1; // GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost;

        //lightstone blessing check
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 301, 7) > 0)
        {
            numberOfDice += 1;
        }

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

        int score = bestRoll + GetSkillPoints(foeCard.GetComponent<CardDisplay2>().foeCardRequirementType);

        bool isSuccess = false;

        //special cases
        if (bestRoll == 1)
        {
            isSuccess = false;
        }
        if (bestRoll == 6)
        {
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
            if (score >= foeCard.GetComponent<CardDisplay2>().foeCardRequirementQty)
            {
                isSuccess = true;
            }

            else
            {
                isSuccess = false;
            }
        }

        //show hero roll here (dont show hits)
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().SetDamageDisplay(0, bestRoll, false);

        //handles skillcheck display 
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().InstantSkillCheck(isSuccess);//PV.RPC("RPC_SkillCheck", RpcTarget.AllBufferedViaServer, skillCheckText, isSuccess);
        FoeEffects.ins.InstantFoeSkillCheckEffect(isSuccess, foeCard, foeNumber);


        //shows first skillcheck animation for all
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ShowHeroAnimation(GameManager.ins.turnNumber, foeCard.GetComponent<CardDisplay2>().foeCardRequirementType);

    }
}
