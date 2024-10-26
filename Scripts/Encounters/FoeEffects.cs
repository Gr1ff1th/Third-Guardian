using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoeEffects : MonoBehaviour
{
    public static FoeEffects ins;

    // Start is called before the first frame update
    void Start()
    {
        ins = this;
    }

    public void InstantFoeSkillCheckEffect(bool isSuccess, GameObject foeCard, int foeNumber)
    {
        if(foeCard.GetComponent<CardDisplay2>().sfx != null)
        {
            CardHandler.ins.extraSfxHolder.clip = foeCard.GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //explosive foe
        if (foeCard.GetComponent<Card>().effect == 242)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(9, foeNumber);

            //4 damage
            if (isSuccess == false)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -4);
            }
        }

        //explosive foe (poison)
        if (foeCard.GetComponent<Card>().effect == 243)
        {
            GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackOnce(9, foeNumber);

            //4 poison
            if (isSuccess == false)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, 4);
            }
        }

        //lets start adding non-damage foe skill effects here
        //throw curse
        if (foeCard.GetComponent<Card>().effect == 13)
        {
            if(isSuccess == false)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
            }
        }


        //ensnaring roots
        if (foeCard.GetComponent<Card>().effect == 37)
        {
            //ensnared on skillcheck failure
            if (isSuccess == false)
            {
                //2 stacks of ensnaring roots
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 171, 7, 2);
            }
        }


        //2 round web
        if (foeCard.GetComponent<Card>().effect == 49)
        {
            //ensnared on skillcheck failure
            if (isSuccess == false)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 176, 7, 2);
            }
        }

        //3 round web
        if (foeCard.GetComponent<Card>().effect == 50)
        {
            //ensnared on skillcheck failure
            if (isSuccess == false)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 176, 7, 3);
            }
        }

        //doom gaze
        //1 round petrified, 5 damage
        //might need different effect number for different versions?
        if (foeCard.GetComponent<Card>().effect == 56)
        {
            //ensnared on skillcheck failure
            if (isSuccess == false)
            {
                //2 stacks of stone curse
                //but not if hero has smoke screen or berserk
                if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 232, 7) > 0 ||
                    CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 235, 7) > 0)                    //(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().berserkActivated == true || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated == true) //||                    )
                {
                    //do nothing
                }
                else
                {
                    //dont need stat update, since its done in the drawcards method
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 182, 7, 1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -5);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isPetrified = true;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
                    {
                        //need to set this?
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = false;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
                        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);

                        //need to test this soon
                        return;
                    }
                }
            }
        }

        //dark gaze
        //1 round petrified, 1 stack of curse, 5 damage
        if (foeCard.GetComponent<Card>().effect == 65)
        {
            //ensnared on skillcheck failure
            if (isSuccess == false)
            {
                //2 stacks of stone curse, 1 stack of curse
                //but not if hero has smoke screen or berserk
                if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 232, 7) > 0 ||
                    CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 235, 7) > 0)
                {
                    //do nothing
                }
                else
                {
                    //dont need stat update, since its done in the drawcards method
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 182, 7, 1);
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -5);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isPetrified = true;

                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
                    {
                        //need to set this?
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
                        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = false;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
                        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);

                        return;
                    }
                }
            }
        }
    }


    //repurposed from ExploreHandler
    public void FoeDamageSkillEffect(GameObject foeCard, int hits)
    {
        //shadow bolt & shadow strike
        if (foeCard.GetComponent<Card>().effect == 15 || foeCard.GetComponent<Card>().effect == 44)
        {
            //curses if any hits taken
            if (hits > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
            }
        }

        //toxic vial
        if (foeCard.GetComponent<Card>().effect == 30)
        {
            //inflicts poison
            if (hits > 0)
            {
                //special case for sentinel
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
                {
                    CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, hits);
                }
            }
        }


        //immolating alternate attacks
        //greater fireball, fireball etc
        if (foeCard.GetComponent<Card>().effect == 36 || foeCard.GetComponent<Card>().effect == 47 || 
            foeCard.GetComponent<Card>().effect == 60 || foeCard.GetComponent<Card>().effect == 74)
        {
            //if hits taken
            if (hits > 0)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 173, 7, hits);
            }
        }

        //frostbite
        //frostbreath etc
        if (foeCard.GetComponent<Card>().effect == 62 || foeCard.GetComponent<Card>().effect == 67 ||
            foeCard.GetComponent<Card>().effect == 68)
        {
            //if hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 185, 7, GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);

                FrozenCheckWithDelay();
            }
        }

        //greater shadow bolt
        if (foeCard.GetComponent<Card>().effect == 76)
        {
            //curses if any hits taken
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);
            }
        }

        /* dont need these anymore?
         * 
         * only apply certain effects once per combat round for foes (such as regeneration, decays)
        //returns true, if foe was defeated
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().OncePerRoundFoeEffects() == true)
        {
            return;
        }

        // give turn to player, if all foes have acted
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

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn = 1;

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //need to do this elsewhere, since this button gets deleted
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToRealTimeCombat(true);
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

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;

            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //need to do this elsewhere, since this button gets deleted
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
        }
        */
    }

    //handles foe defense cards & passive cooldown effects
    //repurposed from combathandler
    public void FoeHiddenEffect(GameObject foeCard)
    {
        int foeNumber = foeCard.GetComponent<Card>().belongsTo;
        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1];

        //play sfx
        if (foeCard.GetComponent<CardDisplay2>().sfx != null)
        {
            CardHandler.ins.extraSfxHolder.clip = foeCard.GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //see if foe has regeneration (1 energy)
        if (foeCard.GetComponent<Card>().effect == 24)
        {
            if (battlefieldFoe.thisStrategicEncounter.currentEnergy < battlefieldFoe.thisStrategicEncounter.maxEnergy)
            {
                battlefieldFoe.thisStrategicEncounter.currentEnergy += 1;
            }
            battlefieldFoe.SetHealthBarValues();
        }

        //see if foe has regeneration (2 energy)
        if (foeCard.GetComponent<Card>().effect == 25)
        {
            if (battlefieldFoe.thisStrategicEncounter.currentEnergy < battlefieldFoe.thisStrategicEncounter.maxEnergy)
            {
                battlefieldFoe.thisStrategicEncounter.currentEnergy += 2;
            }

            if (battlefieldFoe.thisStrategicEncounter.currentEnergy > battlefieldFoe.thisStrategicEncounter.maxEnergy)
            {
                battlefieldFoe.thisStrategicEncounter.currentEnergy = battlefieldFoe.thisStrategicEncounter.maxEnergy;
            }
            battlefieldFoe.SetHealthBarValues();
        }


        //special case for decaying foes
        if (foeCard.GetComponent<Card>().effect == 7)
        {
            Debug.Log("decay 1 triggers, foeNumber: " + foeNumber);

            if (battlefieldFoe.thisStrategicEncounter.currentEnergy <= 1)
            {
                //need to set this?
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = true;
                battlefieldFoe.foeDefeated = true;

                battlefieldFoe.thisStrategicEncounter.currentEnergy -= 1;

                //should call this, in case enemy stats have changed?
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

                GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated(battlefieldFoe.foeNumber);
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
            }
            else
            {
                battlefieldFoe.thisStrategicEncounter.currentEnergy -= 1;
            }

            battlefieldFoe.SetHealthBarValues();
        }

        //special case for decaying foes (2)
        if (foeCard.GetComponent<Card>().effect == 11)
        {
            if (battlefieldFoe.thisStrategicEncounter.currentEnergy <= 2)
            {
                //need to set this?
                //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated = true;
                battlefieldFoe.foeDefeated = true;

                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;

                battlefieldFoe.thisStrategicEncounter.currentEnergy -= 2;

                GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated(battlefieldFoe.foeNumber);
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
            }

            else
            {
                battlefieldFoe.thisStrategicEncounter.currentEnergy -= 2;
            }

            battlefieldFoe.SetHealthBarValues();
        }

        // THESE GETS ADDED TO FOE CARDHOLDER

        //magic shell
        if (foeCard.GetComponent<Card>().effect == 14)
        {
            battlefieldFoe.DrawActiveCards(foeCard, 1);

            /* lets do this another way
             * 
            //instantiates random quest card from the deck
            GameObject newCard = Instantiate(foeCard, new Vector3(0, 0, 0), Quaternion.identity);

            //places it in hand card area
            newCard.transform.SetParent(battlefieldFoe.activeCardsArea.transform, false);

            //turns the card inactive
            newCard.SetActive(true);

            //set the "owner" variable to the card
            newCard.GetComponent<Card>().belongsTo = foeNumber;

            newCard.GetComponent<CardDisplay2>().quantity = 1;

            newCard.GetComponent<CardDisplay2>().quantityText.text = "";

            newCard.GetComponent<CardDisplay2>().effectTime = newCard.GetComponent<CardDisplay2>().effectTimeMax;

            //need bit of a hack here? (since we using reference of the card from different size holder)
            newCard.GetComponent<CardDisplay2>().overlay.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 12);
            */
        }

        //dodge
        if (foeCard.GetComponent<Card>().effect == 16)
        {
            battlefieldFoe.DrawActiveCards(foeCard, 1);
        }

        //block
        if (foeCard.GetComponent<Card>().effect == 20)
        {
            battlefieldFoe.DrawActiveCards(foeCard, 1);
        }

        //when immolation stack "triggers"
        if (foeCard.GetComponent<Card>().effect == 240)
        {
            //battlefieldFoe.DrawActiveCards(foeCard, 1);
            int damageDone = foeCard.GetComponent<CardDisplay2>().quantity / 3;
            battlefieldFoe.thisStrategicEncounter.currentEnergy -= damageDone;
            battlefieldFoe.SetHealthBarValues();

            if (battlefieldFoe.thisStrategicEncounter.currentEnergy < 1)
            {
                //opponentDefeated = true;
                battlefieldFoe.foeDefeated = true;
                gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = true;

                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = true;
                GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated(battlefieldFoe.foeNumber);
            }
        }

        //when daze stack "triggers"
        if (foeCard.GetComponent<Card>().effect == 241)
        {
            //battlefieldFoe.DazeCheck();
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().DazeCheckWithDelay();
        }
    }

    //dunno if this is the most appropriate place for this, but might as well
    //called when reducing stack from hero effect holder
    public void HeroEffectTrigger(GameObject card)
    {
        //play sfx
        if (card.GetComponent<CardDisplay2>().sfx != null)
        {
            CardHandler.ins.extraSfxHolder.clip = card.GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //when immolation stack "triggers"
        if (card.GetComponent<Card>().effect == 102)
        {
            int damageDone = card.GetComponent<CardDisplay2>().quantity / 3;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -damageDone);
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResourceTexts();
        }

        //when petrify stack "triggers"
        if (card.GetComponent<Card>().effect == 109)
        {
            //remove stun, if its the last stack
            if(card.GetComponent<CardDisplay2>().quantity < 2)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isPetrified = false;
            }
        }

        //when frostbite stack triggers
        if (card.GetComponent<Card>().effect == 110)
        {
            //remove stun, if less than 6 stacks
            if (card.GetComponent<CardDisplay2>().quantity < 6)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen = false;
            }
            else
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen = true;
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 185, 7, 4);
            }
        }
    }

    public void FrozenCheckWithDelay()
    {
        Invoke(nameof(FrozenCheck), 0.1f);
    }

    //need this with delay for some reason?
    public void FrozenCheck()
    {
        GameObject frostbittenCard = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 185, 7);

        if (frostbittenCard != null)
        {
            if (frostbittenCard.GetComponent<CardDisplay2>().quantity <= 5)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen = false;
            }
            if (frostbittenCard.GetComponent<CardDisplay2>().quantity > 5)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen = true;
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 185, 7, 5);
            }
        }
    }
}
