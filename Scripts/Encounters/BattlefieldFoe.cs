using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlefieldFoe : MonoBehaviour
{
    //could have reference here for the original strategic encounter
    public StrategicEncounter thisStrategicEncounter;

    //this can be set manually for each of these
    public int foeNumber;

    //the sliders
    public Slider healthSlider;
    public Slider chargingSlider;

    //icon of the ability being charged
    public Image chargingAbility;

    //holder for the foe active effects & abilities
    public GameObject activeCardsArea;

    //need reference to this for turning it of when foe defeated etc
    //also for repositioning?
    public GameObject FoeDisplays;

    public Image FoeImage;

    public Vector2 foeImageOriginalSize;
    public Vector3 foeImageOriginalPosition;
    public Vector3 foeDisplayOriginalPosition;
    public Vector3 foeGameObjectOriginalPosition;

    //needed for remembering which spot foe is on
    public int spot;

    //maybe keep separate class for this? (could handle "bumps" & attacks)
    public EnemyResizing enemyResizing;

    //card which is being charged (mostly attack cards?)
    //default attacks could be handled slightly differently
    public GameObject chargingCard;

    //for foe attack dicerolls
    int rollCap;
    int rollMin;

    public bool foeDefeated;

    public TextMeshProUGUI damageText;
    public float textTimer;

    public CustomTooltips cooldownBarTooltips;
    public CustomTooltips healthBarTooltips;

    void Start()
    {
        //foeImageOriginalSize = new Vector2(222, 228);
        //foeGameObjectOriginalPosition = gameObject.transform.localPosition;
        damageText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //could handle damage text counter here as well
        if(textTimer > 0)
        {
            textTimer -= Time.deltaTime;
        }
        else
        {
            damageText.text = "";
        }

        //added time warp check here too in v0.7.0.
        if (CardHandler.ins.phaseNumber == 3 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused == false && 
            thisStrategicEncounter.isStunned == false && foeDefeated == false && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated == false)
        {
            if (chargingSlider.value < chargingSlider.maxValue)
            {
                chargingSlider.value += Time.deltaTime * GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().currentBattleSpeed;
            }
            else
            {
                //trigger card effect here
                //note that non-damage abilities should use different method
                if (chargingCard.GetComponent<Card>().attackCard == true)
                {
                    FoeAttack();
                }
                else if (chargingCard.GetComponent<Card>().foeInstantSkillcheck == true)
                {
                    GameManager.ins.references.GetComponent<CombatActions>().TriggerFoeCardSkillCheck(chargingCard, foeNumber);
                }

                SetChargingAbility();
            }
        }
    }


    public void SetHealthBarValues()
    {
        healthSlider.maxValue = thisStrategicEncounter.maxEnergy;//GameManager.ins.references.currentEncounter.maxEnergy;
        healthSlider.value = thisStrategicEncounter.currentEnergy;

        healthBarTooltips.customText = "<b>Health</b><br>Foes current health. If health goes below one, it will be defeated.<br><color=#FFDEA3ff>Current health: <b>" +
                thisStrategicEncounter.currentEnergy + "/" + thisStrategicEncounter.maxEnergy + "</b><sprite=\"sprites v92\" index=3>";
    }

    //for the initial spawn
    public void SetBattlefieldFoe(int position)
    {
        //should prolly reset this here also
        damageText.text = "";

        foeDefeated = false;

        FoeDisplays.SetActive(true);

        SetHealthBarValues();

        //could be ok to do this here? (unless some cards are drawn instantly when foe is initiated)
        ClearCardHolder();

        FoeImage.sprite = thisStrategicEncounter.encounter.icon;

        FoeImage.rectTransform.sizeDelta = foeImageOriginalSize * thisStrategicEncounter.encounter.battlefieldFoeSize;

        FoeImage.transform.localPosition = new Vector3(foeImageOriginalPosition.x, foeImageOriginalPosition.y + thisStrategicEncounter.encounter.floatHeight, foeImageOriginalPosition.z);

        //currentFoeLargeIcon.rectTransform.sizeDelta = foeIconOriginalSize * encounterOption.displaySize;

        FoeDisplays.transform.localPosition = new Vector3(foeDisplayOriginalPosition.x, foeDisplayOriginalPosition.y + thisStrategicEncounter.encounter.foeDisplayHeight, foeDisplayOriginalPosition.z);

        //do this last, so we can reset this class' position too
        enemyResizing.SetSize();

        SetChargingAbility();

        //need to relocate gameobjects in multi-combat
        if(position == 3)
        {
            gameObject.transform.localPosition = foeGameObjectOriginalPosition;
            spot = 3;
        }
        if (position == 2)
        {
            gameObject.transform.localPosition = new Vector3(foeGameObjectOriginalPosition.x -90, foeGameObjectOriginalPosition.y, foeGameObjectOriginalPosition.z);
            spot = 2;
        }
        if (position == 1)
        {
            gameObject.transform.localPosition = new Vector3(foeGameObjectOriginalPosition.x - 180, foeGameObjectOriginalPosition.y, foeGameObjectOriginalPosition.z);
            spot = 1;
        }
    }

    public void SetSecondPhaseFoe(int position)
    {
        foeDefeated = false;

        FoeDisplays.SetActive(true);

        SetHealthBarValues();

        //could be ok to do this here? (unless some cards are drawn instantly when foe is initiated)
        ClearCardHolder();

        //mostly needed for quiron atm, although the health bar update wont work if the second form has different hp than the first
        Encounter2 secondPhaseEncounter = thisStrategicEncounter.encounter.combatButton.secondPhase.GetComponent<Encounter2>();

        FoeImage.sprite = secondPhaseEncounter.icon;

        FoeImage.rectTransform.sizeDelta = foeImageOriginalSize * secondPhaseEncounter.battlefieldFoeSize;

        FoeImage.transform.localPosition = new Vector3(foeImageOriginalPosition.x, foeImageOriginalPosition.y + secondPhaseEncounter.floatHeight, foeImageOriginalPosition.z);

        //currentFoeLargeIcon.rectTransform.sizeDelta = foeIconOriginalSize * encounterOption.displaySize;

        FoeDisplays.transform.localPosition = new Vector3(foeDisplayOriginalPosition.x, foeDisplayOriginalPosition.y + secondPhaseEncounter.foeDisplayHeight, foeDisplayOriginalPosition.z);

        //do this last, so we can reset this class' position too
        enemyResizing.SetSize();

        SetChargingAbility();

        //need to relocate gameobjects in multi-combat
        if (position == 3)
        {
            gameObject.transform.localPosition = foeGameObjectOriginalPosition;
            spot = 3;
        }
        if (position == 2)
        {
            gameObject.transform.localPosition = new Vector3(foeGameObjectOriginalPosition.x - 90, foeGameObjectOriginalPosition.y, foeGameObjectOriginalPosition.z);
            spot = 2;
        }
        if (position == 1)
        {
            gameObject.transform.localPosition = new Vector3(foeGameObjectOriginalPosition.x - 180, foeGameObjectOriginalPosition.y, foeGameObjectOriginalPosition.z);
            spot = 1;
        }
    }

    //is it ok to clear this here?
    public void ClearCardHolder()
    {
        int numberOfCards = activeCardsArea.transform.childCount;

        for (int i = numberOfCards; i > 0; i--)
        {
            Destroy(activeCardsArea.transform.GetChild(i - 1).gameObject);
        }
    }

    //tests whether foe has certain card in the active effects holder
    public bool CheckFoeActiveAbility(int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < activeCardsArea.transform.childCount; i++)
        {
            if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //need to add active check here for v91
                if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    return true;
                }
            }
        }

        //if not, returns false
        return false;
    }

    //need to make checks here for special attacks etc
    public void SetChargingAbility()
    {
        chargingCard = null;
        int randomChance = Random.Range(1, 4);

        //2/3 chance to use special attack card
        if (randomChance > 1)
        {
            chargingCard = GetAvailableChargingCard();

            if (chargingCard != null)
            {
                chargingCard.GetComponent<CardDisplay2>().realTimeCooldown = chargingCard.GetComponent<CardDisplay2>().realTimeMaxCooldown;

                //play special sfx
                CardHandler.ins.extraSfxHolder.clip = GameManager.ins.references.sfxPlayer.FoeSpecialTrigger;
                CardHandler.ins.extraSfxHolder.Play();

                chargingAbility.sprite = chargingCard.GetComponent<Image>().sprite;

                cooldownBarTooltips.customText = chargingCard.GetComponent<CardDisplay2>().tooltipText;
            }
        }

        if (chargingCard == null)
        {
            //default physical attack
            if (thisStrategicEncounter.maxAttack > 0)
            {
                chargingCard = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultFoeCombatButtons[0];
                chargingAbility.sprite = chargingCard.GetComponent<Image>().sprite;

                cooldownBarTooltips.customText = "<b>Physical Attack</b><br>Foe is charging an attack with its default <sprite index=4> values";
            }

            //default arcane attack
            if (thisStrategicEncounter.maxArcanePower > 0)
            {
                chargingCard = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultFoeCombatButtons[1];
                chargingAbility.sprite = chargingCard.GetComponent<Image>().sprite;

                cooldownBarTooltips.customText = "<b>Arcane Attack</b><br>Foe is charging an attack with its default <sprite index=9> values";
            }
        }

        float randomTimer = Random.Range(-1f, 1f);
        randomTimer += chargingCard.GetComponent<CardDisplay2>().chargingTime;

        chargingSlider.maxValue = randomTimer;
        chargingSlider.value = 0f;
    }

    public GameObject GetAvailableChargingCard()
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount; i++)
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //need several checks here before choosing the card
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == foeNumber &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().foeChargingCard == true &&
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown <= 0)
                {
                    return GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject;
                }
            }
        }

        //if not, returns false
        return null;
    }

    //used against default attacks of the foe (both physical & magical)
    public void FoeAttack()
    {
        //kinda wanna close these
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().enemyStatsDisplay.SetActive(false);

        //play sfx
        if (chargingCard.GetComponent<CardDisplay2>().sfx != null)
        {
            CardHandler.ins.extraSfxHolder.clip = chargingCard.GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;
        int targettingBonus = 1;

        //parry
        GameObject card1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 7);
        if(card1 != null)
        {
            if (card1.GetComponent<Card>().cardLevel > 1)
            {
                targettingBonus = 2;
            }
        }
        //supershield
        GameObject card2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 7);
        if (card2 != null)
        {
            if (card2.GetComponent<Card>().cardLevel > 1)
            {
                targettingBonus = 2;
            }
        }
        //ward
        GameObject card3 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 7);
        if (card3 != null)
        {
            if (card3.GetComponent<Card>().cardLevel > 1)
            {
                targettingBonus = 2;
            }
        }


        rollCap = 5 + targettingBonus;
        rollMin = targettingBonus;

        //phantom attacks calculations
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 45) == true)
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
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 2))
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
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 35))
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

        AttackDiceroll();
    }


    void AttackDiceroll()
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

        //parry
        GameObject card1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 7);
        if (card1 != null)
        {
            if (card1.GetComponent<Card>().cardLevel > 2)
            {
                numberOfDice += 1;
            }
        }
        //supershield
        GameObject card2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 7);
        if (card2 != null)
        {
            if (card2.GetComponent<Card>().cardLevel > 2)
            {
                numberOfDice += 1;
            }
        }
        //ward
        GameObject card3 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 7);
        if (card3 != null)
        {
            if (card3.GetComponent<Card>().cardLevel > 2)
            {
                numberOfDice += 1;
            }
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

        ContinueAttackDiceroll(bestRoll);
    }

    void ContinueAttackDiceroll(int bestRoll)
    {
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        //actually we can keep these as baseline?
        int attack = thisStrategicEncounter.attack;
        int arcanePower = thisStrategicEncounter.arcanePower;

        //lets make changes to the values, if its not base attack
        if (chargingCard.GetComponent<Card>().effect != 246 && chargingCard.GetComponent<Card>().effect != 247)
        {
            if (chargingCard.GetComponent<CardDisplay2>().usesFoeDefaultAttackValue == true)
            {
                attack = thisStrategicEncounter.attack;
                arcanePower = thisStrategicEncounter.arcanePower;

                //special case for precise strike
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 71) == true)
                {
                    attack += 2;
                }
            }

            else
            {
                //in case foe uses special physical attack
                //need to use the long version, cause of the reroll function
                if (chargingCard.GetComponent<CardDisplay2>().foeCardRequirementType == 1)
                {
                    attack = chargingCard.GetComponent<CardDisplay2>().foeCardRequirementQty;
                    //need to set this to 0, in case foes regular attack in arcane
                    arcanePower = 0;
                }

                //in case foe uses special arcane attack
                if (chargingCard.GetComponent<CardDisplay2>().foeCardRequirementType == 3)
                {
                    arcanePower = chargingCard.GetComponent<CardDisplay2>().foeCardRequirementQty;
                    attack = 0;
                }
            }
        }


        int hits = 0;

        if (attack != 0)
        {
            hits = attack - (bestRoll + GameManager.ins.references.GetComponent<CombatActions>().GetSkillPoints(2));
        }
        else if (arcanePower != 0)
        {
            hits = arcanePower - (bestRoll + GameManager.ins.references.GetComponent<CombatActions>().GetSkillPoints(4));
        }

        //check dodge here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 108, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated == true)
        {
            hits -= 2;
        }

        //then check smoke bomb
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 232, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated == true)
        {
            hits -= 1;
        }

        //then check ward
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 110, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)// && attack == 0)
        {
            hits -= 1;
        }

        if (hits < 1)
        {
            hits = 0;
        }

        //special cases (always take hit on fumble)
        if (bestRoll == 1)
        {
            if (hits < 2)
            {
                //actually lets make this 2, so protections wont always negate the whole damage
                hits = 2;
            }
        }

        //no hits taken on crit
        if (bestRoll == 6)
        {
            hits = 0;
        }

        //special cases for heavy hitters
        //dismiss this if special phase, and foe doesnt have precise strike
        if (chargingCard.GetComponent<Card>().effect == 246 || chargingCard.GetComponent<Card>().effect == 247 || chargingCard.GetComponent<Card>().effect == 71)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == false || (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(71)))
        {
            //change this for v94
            if (hits > 0)
            {
                float damageModifier = 100f;

                if (attack != 0)
                {
                    damageModifier = thisStrategicEncounter.encounter.attackMod;
                }
                if (arcanePower != 0)
                {
                    damageModifier = thisStrategicEncounter.encounter.arcanePowerMod;
                }

                //round up the damage reduction?
                float newDamage = Mathf.Ceil(hits * (damageModifier / 100));
                hits = (int)newDamage;
            }
        }

        //special phase damage modifiers
        //also handle the effects here
        //1.5x damage (shadow bolt, bombardment, heavy strike, fireball, fire breath, frost breath, frost bolts)
        if (chargingCard.GetComponent<Card>().effect == 15 || chargingCard.GetComponent<Card>().effect == 17 ||
            chargingCard.GetComponent<Card>().effect == 28 || chargingCard.GetComponent<Card>().effect == 47 ||
            chargingCard.GetComponent<Card>().effect == 60 || chargingCard.GetComponent<Card>().effect == 62 ||
            chargingCard.GetComponent<Card>().effect == 67 || chargingCard.GetComponent<Card>().effect == 68)
        {
            //round hitsdone / 2 upwards
            float hitsFloat = Mathf.Ceil(hits / 2f);
            hits += (int)hitsFloat;
        }

        //2x damage (greater fireball, shadow strike, powerful charge)
        if (chargingCard.GetComponent<Card>().effect == 36 || chargingCard.GetComponent<Card>().effect == 44 ||
            chargingCard.GetComponent<Card>().effect == 59)
        {
            //round hitsdone / 2 upwards
            //float hitsFloat = Mathf.Ceil(hits / 2f);
            hits = hits * 2;
        }
        //shadow bolt (also greater shadow bolt)
        if (chargingCard.GetComponent<Card>().effect == 15)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(18, 4);
        }
        //bombard  & toxic vial
        if (chargingCard.GetComponent<Card>().effect == 17 ||
            chargingCard.GetComponent<Card>().effect == 30)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(9, 4);
        }
        //power strike, devastating strike, precise strike
        if (chargingCard.GetComponent<Card>().effect == 28 || chargingCard.GetComponent<Card>().effect == 59 ||
            chargingCard.GetComponent<Card>().effect == 71)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(38, 4);
        }
        //fireball
        if (chargingCard.GetComponent<Card>().effect == 47)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(0, 4);
        }
        //greater fireball, fire breath
        if (chargingCard.GetComponent<Card>().effect == 36 || chargingCard.GetComponent<Card>().effect == 60)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(2, 4);
        }
        //shadow strike
        if (chargingCard.GetComponent<Card>().effect == 44)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(18, 4);
        }
        //frost breath
        if (chargingCard.GetComponent<Card>().effect == 62)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(5, 4);
        }
        //frost bolts
        if (chargingCard.GetComponent<Card>().effect == 67 ||
            chargingCard.GetComponent<Card>().effect == 68)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(3, 4);
        }


        //effects for non-specials
        //physical attack
        if (chargingCard.GetComponent<Card>().effect == 246)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(28, 4);
        }
        //arcane attack
        if (chargingCard.GetComponent<Card>().effect == 247)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(17, 4);
        }

        //check heros damage reductions last
        //actually dont need to make alterations for these at v92?
        //smoke bombs 2 is also checked here now
        if (hits != 0 && arcanePower == 0) // && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == false)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) == 0)
        {
            //physical damage type
            hits = GameManager.ins.references.GetComponent<CombatActions>().CheckProtections(1, hits);
        }
        if (hits != 0 && attack == 0) // || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) > 0)
        {
            //magic damage type
            hits = GameManager.ins.references.GetComponent<CombatActions>().CheckProtections(3, hits);
        }

        //check block here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 109, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().blockActivated == true)
        {
            hits = hits / 2;
        }

        //check ward again here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 110, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)// && attack == 0)
        {
            hits = hits * 3 / 4;
        }

        //check shield of isolore here
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 111, 7) > 0)//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().shieldOfIsoloreActivated == true)
        {
            hits = 0;
        }

        /*
        if (hits >= 1)
        {
            isSuccess = false;
        }
        else
        {
            isSuccess = true;
        }
        */

        enemyResizing.ActivateFoeAttack(1);

        //show different animation depending on outcome
        //hit animation
        //this should be done before the basic defend method, since that method reduces hp
        //or we could put these animations to that method instead?
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

        //show hero damage display
        GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().SetDamageDisplay(hits, bestRoll, true);

        //handles skillcheck display 
        //also we should handle foe skill checks in this method (so they wont trigger if hero dies)
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().BasicDefend(hits, chargingCard, foeNumber);

    }

    public void DrawActiveCards(GameObject activeCard, int quantity)
    {
        //dont instantiate new card, if you alrdy have it, instead add to quantity
        if (IfHaveCard(activeCard, quantity) == false)
        {
            GameObject newCard = Instantiate(activeCard, new Vector3(0, 0, 0), Quaternion.identity);

            //places it in card area
            newCard.transform.SetParent(activeCardsArea.transform, false);

            //turns the card inactive
            newCard.SetActive(true);

            //set the "owner" variable to the card (dunno if necessary tho)
            newCard.GetComponent<Card>().belongsTo = foeNumber;
            newCard.GetComponent<CardDisplay2>().quantity = quantity;

            newCard.GetComponent<CardDisplay2>().quantityText.text = "";

            //show quantity in certain conditions
            if (newCard.GetComponent<CardDisplay2>().quantity > 1 ||
                newCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
            {
                newCard.GetComponent<CardDisplay2>().quantityText.text =
                    newCard.gameObject.GetComponent<CardDisplay2>().quantity.ToString();

                newCard.GetComponent<CardDisplay2>().quantityText.fontSize = 8;
            }

            newCard.GetComponent<CardDisplay2>().effectTime = newCard.GetComponent<CardDisplay2>().effectTimeMax;

            //need bit of a hack here? (since we using reference of the card from different size holder)
            newCard.GetComponent<CardDisplay2>().overlay.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 12);
        }
    }

    public bool IfHaveCard(GameObject activeCard, int quantity)
    {
        int effectNumber = activeCard.GetComponent<Card>().effect;

        //tests if player has passive of that effect number
        for (int i = 0; i < activeCardsArea.GetComponent<Transform>().childCount; i++)
        {
            if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity += quantity;

                    //show quantity in certain conditions
                    if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                        activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                    {
                        activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                            activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();

                        activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.fontSize = 8;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    //checks quantity of specific card on the active cards area
    public int CheckQuantity(int effectNumber)
    {
        //tests if player has passive of that number
        for (int i = 0; i < activeCardsArea.GetComponent<Transform>().childCount; i++)
        {
            if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    return activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity;
                }
            }
        }
        return 0;
    }

    //when reducing number of cards in the active cards holder
    //might need in some cases
    public void ReduceQuantity(int effectNumber, int quantity)
    {
        for (int i = 0; i < activeCardsArea.GetComponent<Transform>().childCount; i++)
        {
            if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                //note that we use effect here, instead of number in deck
                if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity -= quantity;

                    //show quantity in certain conditions
                    if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity > 1 ||
                        activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().showQuantityAlways == true)
                    {
                        activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text =
                            activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity.ToString();
                    }
                    else
                    {
                        activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
                    }

                    //remove card if quantity goes to 0
                    if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().quantity <= 0)
                    {
                        Destroy(activeCardsArea.transform.GetChild(i).gameObject);
                        return;
                    }
                }
            }
        }
    }

    //kinda need something like this
    public GameObject CopyActiveCard(int effectNumber)
    {
        //tests if player has passive of that effect number
        for (int i = 0; i < activeCardsArea.GetComponent<Transform>().childCount; i++)
        {
            if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>() != null)
            {
                if (activeCardsArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == effectNumber)
                {
                    return activeCardsArea.transform.GetChild(i).gameObject;
                }
            }
        }
        return null;
    }

    //lets put daze check here (for easier handling)
    public void DazeCheck()
    {
        //need reference to original card, for timer reset purposes (for stun specifically)
        //dont need != null check yet?
        GameObject dazeCard = CopyActiveCard(241);

        //could do initial daze calculations here (since you can add daze stacks to foes only by attack?)
        //daze check (special case for large)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 55) == true)
        {
            //Debug.Log("foe is large, number of dazestacks: " + CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4));

            if (CheckQuantity(241) >= 7 && thisStrategicEncounter.isDazed == false)//CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4) >= 7 && GameManager.ins.references.currentStrategicEncounter.isDazed == false)
            {
                if (thisStrategicEncounter.attack != 0)
                {
                    thisStrategicEncounter.attack -= 1;
                }
                if (thisStrategicEncounter.arcanePower != 0)
                {
                    thisStrategicEncounter.arcanePower -= 1;
                }

                thisStrategicEncounter.defense -= 1;
                thisStrategicEncounter.resistance -= 1;

                thisStrategicEncounter.isDazed = true;
            }

            if (CheckQuantity(241) < 7 && thisStrategicEncounter.isDazed == true)
            {
                if (thisStrategicEncounter.attack != 0)
                {
                    thisStrategicEncounter.attack += 1;
                }
                if (thisStrategicEncounter.arcanePower != 0)
                {
                    thisStrategicEncounter.arcanePower += 1;
                }

                thisStrategicEncounter.defense += 1;
                thisStrategicEncounter.resistance += 1;

                thisStrategicEncounter.isDazed = false;
            }

            // could add stun check here
            if (CheckQuantity(241) < 15)
            {
                thisStrategicEncounter.isStunned = false;
            }

            if (CheckQuantity(241) >= 15)
            {
                //the flag should be resetted only after 5s timer has been finished
                //should remove 1 less than 7, because 1 gets reduced elsewhere
                ReduceQuantity(241, 6);

                //reset timer
                dazeCard.GetComponent<CardDisplay2>().effectTime = dazeCard.GetComponent<CardDisplay2>().effectTimeMax;

                thisStrategicEncounter.isStunned = true;
                return;
            }
        }

        //daze check (if not large)
        else
        {
            if (CheckQuantity(241) >= 5 && thisStrategicEncounter.isDazed == false)
            {
                if (thisStrategicEncounter.attack != 0)
                {
                    thisStrategicEncounter.attack -= 1;
                }
                if (thisStrategicEncounter.arcanePower != 0)
                {
                    thisStrategicEncounter.arcanePower -= 1;
                }

                thisStrategicEncounter.defense -= 1;
                thisStrategicEncounter.resistance -= 1;

                thisStrategicEncounter.isDazed = true;
            }

            if (CheckQuantity(241) < 5 && thisStrategicEncounter.isDazed == true)
            {
                if (thisStrategicEncounter.attack != 0)
                {
                    thisStrategicEncounter.attack += 1;
                }
                if (thisStrategicEncounter.arcanePower != 0)
                {
                    thisStrategicEncounter.arcanePower += 1;
                }

                thisStrategicEncounter.defense += 1;
                thisStrategicEncounter.resistance += 1;

                thisStrategicEncounter.isDazed = false;
            }

            // could add stun check here
            if (CheckQuantity(241) < 10)
            {
                thisStrategicEncounter.isStunned = false;

                Debug.Log("has less than 10 stacks of daze");
            }

            if (CheckQuantity(241) >= 10)
            {
                //the flag should be resetted only after 5s timer has been finished
                ReduceQuantity(241, 4);

                //reset timer
                dazeCard.GetComponent<CardDisplay2>().effectTime = dazeCard.GetComponent<CardDisplay2>().effectTimeMax;

                thisStrategicEncounter.isStunned = true;
                return;
            }
        }
    }

    public void RemoveFloat()
    {
        if (thisStrategicEncounter.encounter.floatHeight > 0)
        {
            FoeImage.transform.localPosition = new Vector3(foeImageOriginalPosition.x, foeImageOriginalPosition.y, foeImageOriginalPosition.z);
        }

        if (thisStrategicEncounter.encounter.combatButton.secondPhase != null)
        {
            if (thisStrategicEncounter.encounter.combatButton.secondPhase.GetComponent<Encounter2>().floatHeight > 0)
            {
                FoeImage.transform.localPosition = new Vector3(foeImageOriginalPosition.x, foeImageOriginalPosition.y, foeImageOriginalPosition.z);
            }
        }
    }

    public void SetDamageDisplay(int hits, int bestRoll)
    {
        textTimer = 2f;

        damageText.text = GameManager.ins.references.GetComponent<CombatActions>().GetDice(bestRoll) + " -" + hits;
    }
}
