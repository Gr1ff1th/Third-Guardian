using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class CombatHandler : MonoBehaviour
{
    public PhotonView PV;

    //stores used attack type
    //1= default attack, 2=arcane attack
    public int attackTypeUsed;

    //stores opponent attack type
    //1= default attack (physical or magical)
    //rest are special attacks
    public int enemyAttackTypeUsed;

    public int hitsDone;
    public int hitsTaken;

    //true if opponent runs out of energy
    public bool opponentDefeated;

    //true if hero is knocked out
    public bool heroKnockedOut;

    //special case for when we wanna remove foe from battle, but keep him on map
    //only used when theres second step after the initial remove button
    public bool removeFoeFromBattleOnly;

    //true if special phases are active
    public bool isSpecialFoeAttackPhase;
    public bool isSpecialFoeDefensePhase;

    //flag for when foe special was just triggered (for timer purposes)
    public bool foeSpecialUsed;

    //used for timer purposes (on reroll phase, if timer runs out, use continue button instead)
    public bool isRerollPhase;

    //list containing default combat buttons
    public List<GameObject> defaultCombatButtons;

    //for v0.5.7.
    public List<GameObject> defaultFoeCombatButtons;

    //flag variables for hero specials
    public bool powerAttackActivated;
    public bool preciseStrikeActivated;
    public bool throwBombsActivated;
    public bool throwManaBombsActivated;
    public bool arcaneBarrageActivated;
    public bool arcaneOrbActivated;
    public bool boltOfLightActivated;
    public bool spearOfLightActivated;
    public bool dodgeActivated;
    public bool blockActivated;
    public bool wardActivated;
    public bool shieldOfIsoloreActivated;
    public bool extraStrikeActivated;
    public bool timeWarpActivated;
    public bool detonationActivated;
    public bool arcaneDetonationActivated;

    //need these too, so the effect wont get reduced twice (if remove card is called twice at the same time?)
    public bool invulnerabilityActivated;
    public bool wraithsGiftActivated;
    public bool berserkActivated;
    public bool smokeBombActivated;
    public bool avatarOfFireActivated;
    public bool potionOfPowerActivated;

    //need to bring this variable from cardDisplay2 for when triggering cards
    public int heroCardEffect;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public void SwitchBattlefieldDisplay(bool turnOn)
    {
        if(turnOn == true)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().battlefieldDisplay.SetActive(true);

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.SetActive(false);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOptions.SetActive(false);
            GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.SetActive(false);
        }

        if (turnOn == false)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().battlefieldDisplay.SetActive(false);

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.SetActive(true);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOptions.SetActive(true);
            GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characterBackground.SetActive(true);
        }
    }

    //changed for v0.5.7.
    //used when first starting battle?
    public void GoToRealTimeCombat(bool hasFirstStrike)
    {
        gameObject.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        if (PlayerPrefs.GetInt("PauseOn") == 1)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused = true;
        }

        SwitchBattlefieldDisplay(true);

        //need to put delay cause of storebutton rpc call
        //Invoke(nameof(DelayedGoToRealTimeCombat), 0.4f);

        //since this class is only called once per battle, we should activate timer here for now
        //need to make check here later? (once theres toggleable pause button)
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = false;

        //dunno if we really need delay?
        RealTimeCombat(hasFirstStrike);
    }

    //kinda duplicate to the combatButton on encounterbutton class
    void RealTimeCombat(bool hasFirstStrike)
    {
        //combat begins (opponent not swift)
        //string combatText = "<size=18>Attack Phase</size>\n<size=8>\n</size><color=#FFD370>You are attacking. You can optionally use one usable item or ability.";

        string combatText = "";

        //GameManager.ins.references.enemyResizing.ActivateFoeBump(1);
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].enemyResizing.ActivateFoeBump(1);

        //PV.RPC(nameof(RPC_AttackPhase), RpcTarget.AllBufferedViaServer, combatText);

        //allow combat cards
        //dont rly need phase 4 at all anymore?
        CardHandler.ins.SetUsables(3);

        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text = combatText;

        //show idle wide animation
        gameObject.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().ShowCharacter(GameManager.ins.turnNumber, 10);

        //Invoke(nameof(SpawnAttackButtons2), 0.3f);

        //set this here alrdy
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(true);

        //dont spawn cards, if there are any alrdy?
        if (GameManager.ins.combatCardArea.transform.childCount == 0)
        {
            if (hasFirstStrike == true)
            {
                SpawnAttackButtons2(0f);
            }
            else
            {
                SpawnAttackButtons2(5f);
            }
        }
    }

    //unnecessary since v0.5.7.
    //need to refactor the special effect checks from here later tho
    //starts attack phase
    [PunRPC]
    void RPC_AttackPhase(string combatText)
    {
        //why isnt reroll cost resetted here?
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;
        hitsTaken = 0;
        hitsDone = 0;

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //dunno how this worked before fixing it
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

            //allow combat cards
            CardHandler.ins.SetUsables(3);
        }

        //lets skip attack phase, if hero petrified
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 182, 7) > 0)
        {
            //should call the ContinueFromBasicAttackCheckButton method here
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.ContinueFromBasicAttackCheckButton();
            return;
        }

        //lets skip attack phase, if hero has more than 5 stacks of frostbite
        //reduce 5 stacks
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 185, 7) >= 5)
        {
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 185, 7, 5);

            //should call the ContinueFromBasicAttackCheckButton method here
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.ContinueFromBasicAttackCheckButton();
            return;
        }

        //lets put these here
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true && (CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4))
        {
            GameManager.ins.references.targettingHandler.targettingBorders.SetActive(true);
            GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(true);
            GameManager.ins.references.GetComponent<SliderController>().StartCombatTimer();
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().holedBackground;
            //might not need take this here, since its alrdy in reroll button and carddisplay combat effect?
            //GameManager.ins.references.targettingHandler.TakeBallScore();
            GameManager.ins.references.targettingHandler.ResetBalls();
            GameManager.ins.references.targettingHandler.TakeTargettingBackground();
            //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().StartCombatTimer();

            //GameManager.ins.references.targettingHandler.crosshair.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.crosshair.GetComponent<BallMove>().attackPhaseIcon;
            //GameManager.ins.references.targettingHandler.target.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.target.GetComponent<BallMove>().attackPhaseIcon;

        }
        else
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
            GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
            GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
            GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
        }

        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text = combatText;

        //dont check foe specials, if it was alrdy used this turn
        if (foeSpecialUsed == false)
        {
            //returns true, if cooldown ability is triggered
            if (CheckFoeDefenseCooldownAbilities() == true)
            {
                //case of magic shell
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(14))
                {
                    gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text += "\nThe foe is protected by magic shell.";
                }

                //case of dodge
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(16))
                {
                    gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text += "\nThe foe is dodging your attack.";
                }

                //case of block
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(20))
                {
                    gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text += "\nThe foe is blocking your attack.";
                }
            }
        }

        //show idle wide animation
        gameObject.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().ShowCharacter(GameManager.ins.turnNumber, 10);

        Invoke(nameof(SpawnAttackButtons2), 0.3f);
    }

    void SpawnAttackButtons()
    {
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().SpawnAttackButtons();
    }

    public void SpawnAttackButtonsWithDelay()
    {
        Invoke(nameof(SpawnAttackButtons2), 0.3f);
    }

    //new combat system for v91
    void SpawnAttackButtons2(float additionalStartCooldown)
    {
        //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().SpawnAttackButtons();

        //set purchase phase
        CardHandler.ins.SetUsables(3);

        /* lets do this elsewhere
         * special case for extra strike & time warp
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == false && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated == false) //CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 151, 7) == 0)
        {
            CardHandler.ins.ReduceHeroCombatCardTimers(GameManager.ins.turnNumber, 1);
        }
        */

        //instantiates default attack button
        GameObject playerCard = Instantiate(defaultCombatButtons[0], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);
        //note that this should be checked for most other abilities
        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;

        //for v0.5.7.
        playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        

        //instantiate arcane attack button for arcanists (or if wielding staff)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == true || CardHandler.ins.StaffCheck() == true)
        {
            playerCard = Instantiate(defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= 1 || 
                (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == true && CardHandler.ins.StaffCheck() == true))
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            //special case for arcanist with staff
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == true && CardHandler.ins.StaffCheck() == true)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Attack</b><br><color=#FFD370>Ranged Attack Ability</color><br>Attack the enemy using your default <sprite index=9> values.";
            }
            //special case for slow opponents
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Attack</b><br><color=#FFD370>Ranged Attack Ability</color> <sprite index=11><br>Attack the enemy using your default <sprite index=9> values.";
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate throw bomb button if you have bomb
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
        {
            playerCard = Instantiate(defaultCombatButtons[4], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            if (defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate throw manabomb button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(40) == true &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
        {
            playerCard = Instantiate(defaultCombatButtons[5], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            if (defaultCombatButtons[5].gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate quick reload button
        //need to also have bombs for this to show
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 199, 2) > 0 &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 199, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            //also bombs needs to have cooldown for this ability to be useful
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy &&
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject.GetComponent<CardDisplay2>().cooldown != 0)
            { 
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate detonation button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(79) == true &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 2)
        {
            playerCard = Instantiate(defaultCombatButtons[6], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //need reference to the host card
            GameObject detonationMaster = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 154, 2);

            if (detonationMaster.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate arcane detonation button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(79) == true && 
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(40) == true &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 2)
        {
            playerCard = Instantiate(defaultCombatButtons[7], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //need reference to the host card
            GameObject detonationMaster = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 154, 2);

            if (detonationMaster.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate concussion bomb button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 238, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 238, 2), new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check bomb and detonation cooldowns.
            GameObject cardToCheck1 = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject;
            GameObject cardToCheck2 = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject;
            GameObject cardToCheck3 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 239, 7);

            //check additionally that the player has energy to use it
            //also avatar of fire should not be active
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins >= playerCard.gameObject.GetComponent<Card>().requiresCoins)
            {
                //check that some bomb attack isnt on cooldown, also need to have appropriate number of bombs
                if (cardToCheck1.GetComponent<CardDisplay2>().cooldown == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
                if (cardToCheck2.GetComponent<CardDisplay2>().cooldown == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 2)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
                //need to make sure no bomb infusion is active
                if (cardToCheck3 != null)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate firebomb button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 239, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 239, 2), new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check bomb and detonation cooldowns.
            GameObject cardToCheck1 = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[4].gameObject;
            GameObject cardToCheck2 = GameManager.ins.exploreHandler.GetComponent<CombatHandler>().defaultCombatButtons[6].gameObject;
            GameObject cardToCheck3 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 238, 7);

            //check additionally that the player has energy to use it
            //also avatar of fire should not be active
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins >= playerCard.gameObject.GetComponent<Card>().requiresCoins)
            {
                //check that some bomb attack isnt on cooldown
                if (cardToCheck1.GetComponent<CardDisplay2>().cooldown == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
                if (cardToCheck2.GetComponent<CardDisplay2>().cooldown == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 2)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
                //need to make sure no bomb infusion is active
                if(cardToCheck3 != null)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate power attack button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(37) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate precise attack button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(38) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate invigorate button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 234, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 234, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check precise & power strikes
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, 2);
            GameObject cardToCheck2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, 2);

            //check additionally that the player has energy to use it
            //also precise or power strikes needs to be on cooldown for this ability to be useful
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                if (cardToCheck1 != null)
                {
                    if (cardToCheck1.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
                if (cardToCheck2 != null)
                {
                    if (cardToCheck2.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate extra strike button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(75) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 148, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate berserk button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 235, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 235, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate arcane barrage button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(43) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 102, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate arcane orb button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(44) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 103, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate arcane recharge button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 233, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 233, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check if barrage and orb
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 102, 2);
            GameObject cardToCheck2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 103, 2);

            //check additionally that the player has energy to use it
            //also barrage or orb needs to be on cooldown for this ability to be useful
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                if (cardToCheck1 != null)
                {
                    if (cardToCheck1.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
                if (cardToCheck2 != null)
                {
                    if (cardToCheck2.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate earth magic button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 153, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 153, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate wraithform button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 112, 2), new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check avatar of fire
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 237, 7);

            //check additionally that the player has energy to use it
            //also avatar of fire should not be active
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                //actually if avatar of fire exists in the effect holder, this card should be inactive
                if (cardToCheck1 != null)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                else
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate avatar of fire button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 237, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 237, 2), new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check wraithform
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 112, 7);

            //check additionally that the player has energy to use it
            //also wraithform should not be active
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                //actually if wraithform exists in the effect holder, this card should be inactive
                if (cardToCheck1 != null)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                else
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate time warp button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 151, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 151, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate bolt of light button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(45) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 104, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate spear of light button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(46) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 105, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate wrath of guliman button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 236, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 236, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check bolt of light, spear of light
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 104, 2);
            GameObject cardToCheck2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 105, 2);

            //check additionally that the player has energy to use it
            //also barrage or orb needs to be on cooldown for this ability to be useful
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                if (cardToCheck1 != null)
                {
                    if (cardToCheck1.GetComponent<CardDisplay2>().cooldown == 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
                if (cardToCheck2 != null)
                {
                    if (cardToCheck2.GetComponent<CardDisplay2>().cooldown == 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }


        //instantiate blessed blade button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(54) == true)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 113, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate blessed oil button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 114, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 114, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //dunno if we need cooldown
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate overdrive button
        //need different kind of check for this
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 180, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 180, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has health to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health >= 2)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate potion of power button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 298, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 298, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //check additionally that you dont alrdy have the effect on
            //dont need cooldown then necessarily
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 298, 7) == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }

            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        /*
         * COPIED FROM PREVIOUS DEFENSE BUTTONS
        */

        //instantiate rejuvenation pot button
        //ned different kind of check for this
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 19, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 19, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;

            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //quench flames button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 19, 1) > 0)
        {
            //check if you have any immolation
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 173, 7) > 0)
            {
                playerCard = Instantiate(defaultCombatButtons[10], new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

                GameObject cardToTest = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 19, 1);

                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                if (cardToTest.GetComponent<CardDisplay2>().cooldown == 0)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }

                //dont show quantity
                playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate heal button
        //ned different kind of check for this
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 24, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 24, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor
                && playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate bless button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 29, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 29, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //check if you have curses, disable card if not
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 28, 7) == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }


        //instantiate dodge button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 108, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate block button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 109, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate ward button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 110, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate shield of isolore button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 111, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate smoke bomb button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 2) > 0 &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 152, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate potion of invulnerability button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 168, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 168, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //check additionally that you dont alrdy have the effect on
            //dont need cooldown then necessarily
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 168, 7) == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        //instantiate lightstone blessing button
        //note the this has different cardnumber than the blessing itself
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 295, 5) > 0)
        {
            playerCard = Instantiate(defaultCombatButtons[13], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        }

        /* remove these for now?
        //instantiate aim button
        playerCard = Instantiate(defaultCombatButtons[8], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);
        
        playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Aim</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost +
            "<sprite index=11>.<br>Temporarily increase the minimum and maximum score of your next roll by 1.";

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=12>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=10>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=13>";
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
        }
        else
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
        }

        playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;

        //instantiate blessed aim button
        playerCard = Instantiate(defaultCombatButtons[11], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

        playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Blessed Aim</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost +
           "<sprite index=12>.<br>Roll one additional die on your next attack roll.";

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 1)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=17>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 2)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
        }
        else
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
        }

        playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;
        */

        //instantiates flee button
        //not when ensnared tho
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 171, 7) == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 176, 7) == 0 &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 182, 7) == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 185, 7) == 0)
        {
            playerCard = Instantiate(defaultCombatButtons[3], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.combatCardArea.transform, false);
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;

            //special case if theres any opponent with swiftness
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(2) &&
                    CardHandler.ins.mountSlot.transform.childCount == 0)//(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 2, 4) > 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Flee</b><br><color=#FFD370>Active Ability</color> <sprite index=11><sprite index=11> <sprite index=32><br>Flee from combat. Costs <sprite=\"sprites v92\" index=3> when <sprite index=11> not available.";
            }
            //special case if all foes are slow
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckIfAllFoesHaveSpecificAbility(35))//(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 35, 4) > 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Flee</b><br><color=#FFD370>Active Ability</color> <sprite index=32><br>Flee from combat.";
            }
            //special case for smoke bomb
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 7) > 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Flee</b><br><color=#FFD370>Active Ability</color> <sprite index=32><br>Flee from combat.";
            }
        }

        playerCard.gameObject.GetComponent<CardDisplay2>().realTimeCooldown = playerCard.gameObject.GetComponent<CardDisplay2>().realTimeStartingCooldown + additionalStartCooldown;

        //another special case for extra strike & time warp
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated = false;

        //might need to allow scrolling of the holder in some rare cases
        GameManager.ins.combatCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForCombatCards();
    }


    //used by default attack & basic magic attack
    //[PunRPC]
    public void BasicAttack(bool sfxToPlay, int attackType, int hits, int bestRoll)
    {
        //we still need this for certain checks? (such as gorms hammer etc)
        attackTypeUsed = attackType;

        //we should use the battlefield foe reference in v0.5.7.
        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1];
        battlefieldFoe.foeDefeated = false;
        //opponentDefeated = false;
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = false;

        //we could set the damage display here?
        battlefieldFoe.SetDamageDisplay(hits, bestRoll);

        //kinda need this too for now
        hitsDone = hits;

        if (hits >= battlefieldFoe.thisStrategicEncounter.currentEnergy)
        {
            //opponentDefeated = true;
            battlefieldFoe.foeDefeated = true;
            gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = true;
        }

        //note that variable only counts for sfx now
        /*true for hit sfx
        if (sfxToPlay == true)
        {
            GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlaySkillCheckSuccess();
            //gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = true;
        }
        if (sfxToPlay == false)
        {
            GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlaySkillCheckFail();
            //gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = false;
        }

        //this isnt really needed tho?, there shouldnt be any encounter buttons visible at this point anyway
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            gameObject.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
        }
        */

        //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text = skillCheckText;

        //isRerollPhase = true;

        //Invoke(nameof(SpawnButtonsAfterBasicAttack), 0.5f);

        if (battlefieldFoe.foeDefeated == true)
        {
            //need to change card timers
            ChangeRealTimeHeroCooldowns(1);

            battlefieldFoe.thisStrategicEncounter.currentEnergy -= hitsDone;
            battlefieldFoe.SetHealthBarValues();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = true;
            GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated(battlefieldFoe.foeNumber);
        }
        else
        {
            ContinueFromBasicAttackCheckButton();
        }
    }

    //refactored from encounter button class in v0.5.7.
    //conbined with the old defend phase functions
    public void ContinueFromBasicAttackCheckButton()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1];

        if (hitsDone > 0)
        {
            battlefieldFoe.thisStrategicEncounter.currentEnergy -= hitsDone;
            battlefieldFoe.SetHealthBarValues();

            /*GameManager.ins.references.currentStrategicEncounter.currentEnergy -= hitsDone;
            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget == 1)
            {
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic1.currentEnergy -= hitsDone;
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[0].SetHealthBarValues();
            }
            */
        }


        //update the display
        //might do this differently for now
        //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

        //put these just in case
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        //this also now includes stuff like regeneration
        //also hero energy drain
        //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckHitTakenAbilities(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone);

        //dunno if either of these next two are necessary in v0.5.7.
        //these effects should trigger directly from card cooldowns?
        //or actually we should prolly check here what foe / hero cards are active still, just need to change the methods
        //we dont really want those flag variables anymore, just need to check which cards are active

        //checks what additional damage effects you have active
        //modified for v0.5.7. (can no longer directly kill hero)
        AdditionalHeroDamageEffects();
        /*
        if (OncePerRoundHeroEffects() == true)
        {
            return;
        }
        */

        FoeTakesHitsCheckAbilities(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);

        /* probably wont need these checks now?
         * checks foe hit taken and hit done abilities, hero takes damage per hitstaken
        //if true, hero or foe was defeated
        if (ChecksWhenGoingDefensePhase() == true)
        {
            return;
        }
        */
        //this should come after the previous methods?
        ResetHeroAttackFlags();

        //need to change card timers
        ChangeRealTimeHeroCooldowns(1);
    }

    //when changing hero card cooldowns
    //should make this check resources also
    //usually when triggering cards? (this is for hero cards, foe cards might better have separate method)
    //card types:
    //0 = card type doesnt matter
    //1 = hero attack card
    //2 = defense card (uncertain if we need this)
    //3 = consumable card
    public void ChangeRealTimeHeroCooldowns(int cardTypeTriggered)
    {
        for (int i = 0 ; i < GameManager.ins.combatCardArea.transform.childCount; i++)
        {
            //special case for triggered card itself (or shared cooldown card)
            if(GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect == heroCardEffect)
            {
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().useMaxCooldownOnlyAtStart == false)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown =
                        GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeMaxCooldown;
                }
            }

            //attack, defense and consumable cards trigger 5s cooldown of same card type
            else if(cardTypeTriggered == 1 && GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().attackCard == true)
            {
                //special cases for extra strike (doesnt affect or get affected by other attack cards)
                if(GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown < 5f &&
                        heroCardEffect != 75 && GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().effect != 75)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown = 5f;
                }
            }
            else if (cardTypeTriggered == 2 && GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().defenseCard == true)
            {
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown < 5f)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown = 5f;
                }
            }
            else if (cardTypeTriggered == 3 && GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().combatConsumableCard == true)
            {
                if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown < 5f)
                {
                    GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown = 5f;
                }
            }

            //need resource checks too
            GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = 
                CheckCardResourceRequirements(GameManager.ins.combatCardArea.transform.GetChild(i).gameObject);
            
            //cards that are dependent on each other, share the cooldown
            //this should be done after the previous checks?
            if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards.Count > 0)
            {
                for (int j = 0; j < GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards.Count; j++)
                {
                    //check existing cards (for when calling this method via different cards)
                    if (TestSharedCooldownCards(GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards[j]) == true) 
                    {
                        if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().unusableIfSharedCardActive == true)
                        {
                            GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                        }
                    }

                    //special case for certain cards which go to effects holder
                    //need to reactivate these cards later
                    if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards[j] == heroCardEffect)
                    {
                        if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().unusableIfSharedCardActive == true)
                        {
                            GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                        }

                        //this can be used for the refresh abilities
                        //need resource check here too though?
                        else if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().enabledIfSharedCardActivated == true &&
                            CheckCardResourceRequirements(GameManager.ins.combatCardArea.transform.GetChild(i).gameObject) == true)
                        {
                            GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                        }
                        else
                        {
                            if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().useMaxCooldownOnlyAtStart == false)
                            {
                                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeCooldown =
                                GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().realTimeMaxCooldown;
                            }
                        }
                    }
                }
            }
        }
    }

    //returns true, if shared cooldown card found
    public bool TestSharedCooldownCards(int effectNumber)
    {
        //Card card = CardHandler.ins.CopyCard()
        //int cardNumber = card.GetComponent<Card>().numberInDeck;

        //its unfortunate we check effect number instead of number in deck
        if(CardHandler.ins.CheckQuantityViaEffectNumber(GameManager.ins.turnNumber, effectNumber, 7) > 0)
        {
            return true;
        }

        return false;
    }

    public bool CheckCardResourceRequirements(GameObject card)
    {
        Character character = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>();

        if (character.energy < card.gameObject.GetComponent<Card>().requiresEnergy)
        {
            return false;
        }
        if (character.favor < card.gameObject.GetComponent<Card>().requiresFavor)
        {
            return false;
        }
        if (card.gameObject.GetComponent<Card>().requiresBombs > 0)
        {
            //still check bomb quantity?
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) < card.gameObject.GetComponent<Card>().requiresBombs)
            {
                return false;
            }
        }
        if (character.health < card.gameObject.GetComponent<Card>().requiresHealth)
        {
            return false;
        }
        if (card.gameObject.GetComponent<CardDisplay2>().quantity <= 0)
        {
            return false;
        }

        return true;
    }

    public void ReactivateDependentCards(int effectNumber)
    {
        for (int i = 0; i < GameManager.ins.combatCardArea.transform.childCount; i++)
        {
            if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards.Count > 0)
            {
                for (int j = 0; j < GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards.Count; j++)
                {
                    if (GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().sharedCooldownCards[j] == effectNumber)
                    {
                        GameManager.ins.combatCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
        }
    }

    //unused?
    void SpawnButtonsAfterBasicAttack()
    {
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().SpawnButtonsAfterBasicAttack();

        //isRerollPhase = true;
    }

    //unused?
    //need to call this here because the button which calls this gets deleted
    public void RerollAttackButton()
    {
        gameObject.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);

        Invoke("RerollAttack", 0.5f);
    }

    //unused?
    //add more attack types here soon
    void RerollAttack()
    {
        //encounterHandler1.buttonChosen2.SkillCheck();

        //reroll default attack
        if (attackTypeUsed == 1)
        {
            //special cases for sfx
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().powerAttackActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[98].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().preciseStrikeActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[99].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[148].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }

            //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().buttonChosen2.DefaultAttackButton();
            //GameManager.ins.references.GetComponent<CombatActions>().DefaultAttackButton();
        }
        //reroll arcane attack
        if (attackTypeUsed == 2)
        {
            //special cases for sfx
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[102].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[103].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[104].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[105].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }

            //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().buttonChosen2.ArcaneAttackButton();
            //GameManager.ins.references.GetComponent<CombatActions>().ArcaneAttackButton();
        }

        //reroll bomb attack
        if (attackTypeUsed == 3)
        {
            //special cases for sfx
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = defaultCombatButtons[6].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = defaultCombatButtons[7].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }

            //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().buttonChosen2.ArcaneAttackButton();
            //GameManager.ins.references.GetComponent<CombatActions>().ThrowBombButton();
        }
    }

    //unused?
    public void GoToDefensePhase()
    {
        gameObject.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        if (PlayerPrefs.GetInt("PauseOn") == 1)
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused = true;
        }

        //need to put delay cause of storebutton rpc call
        Invoke(nameof(DelayedGoToDefensePhase), 0.4f);
    }

    //unused?
    void DelayedGoToDefensePhase()
    {
        //combat begins
        string combatText = "";
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            combatText = "<br><br><br><br><br><br><br><br><size=18>Defense Phase</size>\n<size=8>\n</size><color=#FFD370>Opponent is attacking. You can improve your defense score by timing your block:";
        }
        else
        {
            combatText = "<br><br><br><br><br><br><br><br><size=18>Defense Phase</size>\n<size=8>\n</size><color=#FFD370>Opponent is attacking. Your defense score is determined by diceroll.";
        }

        GameManager.ins.references.enemyResizing.ActivateFoeBump(1);

        PV.RPC(nameof(RPC_DefensePhase), RpcTarget.AllBufferedViaServer, combatText);
    }

    //unused?
    //starts defense phase
    [PunRPC]
    void RPC_DefensePhase(string combatText)
    {
        //reset foe special defense phase flag
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeDefensePhase = false;

        isRerollPhase = false;

        if (hitsDone > 0)
        {
            GameManager.ins.references.currentStrategicEncounter.currentEnergy -= hitsDone;
        }
        /*
        //special case for extra strike & time warp
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == true || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 151, 7) > 0)
        {
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 151, 7, 1);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToAttackPhase();
            return;
        }
        */
        //resets flags for hero special attacks
        ResetHeroAttackFlags();

        //this also now includes stuff like regeneration
        //also hero energy drain
        //CheckHitTakenAbilities(hitsDone);

        //lets try change foe turn here
        /*this shouldnt be checked twice on same foe?
        if (foeSpecialUsed == false)
        {
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;

            //change foe turn here (to 1 in this case)
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().SetFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);
        }
        */

        //update the display
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

        //reset reroll cost
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        hitsDone = 0;
        hitsTaken = 0;

        //allow defense cards
        CardHandler.ins.SetUsables(4);

        Debug.Log("phasenumber: " + CardHandler.ins.phaseNumber);

        //lets put these here
        //not optimal spot tho, but a curse of rpc call
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true && (CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4))
        {
            GameManager.ins.references.targettingHandler.targettingBorders.SetActive(true);
            GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(true);
            GameManager.ins.references.GetComponent<SliderController>().StartCombatTimer();
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().holedBackground;
            //might not need take this here, since its alrdy in reroll button and carddisply combat effect?
            GameManager.ins.references.targettingHandler.ResetBalls();
            GameManager.ins.references.targettingHandler.TakeTargettingBackground();

            //GameManager.ins.references.targettingHandler.crosshair.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.crosshair.GetComponent<BallMove>().defensePhaseIcon;
            //GameManager.ins.references.targettingHandler.target.GetComponent<SpriteRenderer>().sprite = GameManager.ins.references.targettingHandler.target.GetComponent<BallMove>().defensePhaseIcon;
        }
        else
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
            GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
            GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
            GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
        }

        //foe spcial check put to foesturnskipcheck -method

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
        }

        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().encounter1Text.text = combatText;

        //show idle wide animation
        gameObject.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().ShowCharacter(GameManager.ins.turnNumber, 10);


        //special case for checking if foes turn is skipped
        //spawns defense buttons, if foe wasnt skipped
        //FoesTurnSkipCheck();
        Invoke(nameof(FoesTurnSkipCheck), 0.5f);

    }


    //unused?
    public void FoesTurnSkipCheck()
    {
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(55))
        {
            //lets put 1 less, since 1 was reduced alrdy?
            if (CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4) >= 14)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4, 6);
                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.SkipFoesTurn();
                DelayedFoesTurnSkip();

                //add some sfx
                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[241].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();

                return;
            }
        }
        else
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4) >= 9)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4, 4);
                DelayedFoesTurnSkip();

                //add some sfx
                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[241].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
                return;
            }
        }

        //replace this here, so that daze delays foe specialities too
        //dont check foe specials, if it was alrdy used this turn
        if (foeSpecialUsed == false)
        {
            //returns true, if cooldown ability is triggered
            if (CheckFoeAttackCooldownAbilities() == true)
            {
                //show idle wide animation anyway
                gameObject.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().ShowCharacter(GameManager.ins.turnNumber, 10);

                return;
            }
        }

        //reset this here
        foeSpecialUsed = false;

        Invoke(nameof(SpawnDefenseButtons2), 0.2f);
    }

    void SpawnDefenseButtons()
    {
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().SpawnDefenseButtons();
    }

    public void SpawnDefenseButtonsWithDelay()
    {
        Invoke(nameof(SpawnDefenseButtons2), 0.2f);
    }

    //shouldnt be used in v0.5.7.
    //new combat system for v91
    void SpawnDefenseButtons2()
    {
        //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().SpawnAttackButtons();

        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(true);

        //set defense phase
        CardHandler.ins.SetUsables(4);

        //lets do this in onceperroundheroeffects method
        //CardHandler.ins.ReduceHeroCombatCardTimers(GameManager.ins.turnNumber, 2);

        //instantiates default defend button
        GameObject playerCard = Instantiate(defaultCombatButtons[2], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);
        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
        //playerCard.GetComponent<CardDisplay2>().UpdateTooltip();

        //instantiate rejuvenation pot button
        //ned different kind of check for this
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 19, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 19, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;

            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
        }

        //quench flames button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 19, 1) > 0)
        {
            //check if you have any immolation
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 173, 7) > 0)
            {
                playerCard = Instantiate(defaultCombatButtons[10], new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

                GameObject cardToTest = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 19, 1);

                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                if (cardToTest.GetComponent<CardDisplay2>().cooldown == 0)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }

                //dont show quantity
                playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";
            }
        }

        //instantiate heal button
        //ned different kind of check for this
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 24, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 24, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor
                && playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate bless button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 29, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 29, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //check if you have curses, disable card if not
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 28, 7) == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
        }

        //instantiate greater heal button
        //unused?
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 145, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 145, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate dodge button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 108, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate block button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 109, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate invigorate button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 234, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 234, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check parry & supershield
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 2);
            GameObject cardToCheck2 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 2);

            //check additionally that the player has energy to use it
            //also parry or supershield needs to be on cooldown for this ability to be useful
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                if(cardToCheck1 != null)
                {
                    if (cardToCheck1.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
                if (cardToCheck2 != null)
                {
                    if (cardToCheck2.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate ward button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 110, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate arcane recharge button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 233, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 233, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check ward strikes
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 2);

            //check additionally that the player has energy to use it
            //also ward needs to be on cooldown for this ability to be useful
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                if (cardToCheck1 != null)
                {
                    if (cardToCheck1.GetComponent<CardDisplay2>().cooldown != 0)
                    {
                        playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                    }
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate shield of isolore button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 111, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor >= playerCard.gameObject.GetComponent<Card>().requiresFavor)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate wraithform button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 112, 2), new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check avatar of fire
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 237, 7);

            //check additionally that the player has energy to use it
            //also avatar of fire should not be active
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                //actually if avatar of fire exists in the effect holder, this card should be inactive
                if (cardToCheck1 != null)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                else
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate avatar of fire button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 237, 2) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 237, 2), new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check wraithform
            GameObject cardToCheck1 = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 112, 7);

            //check additionally that the player has energy to use it
            //also wraithform should not be active
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0 &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy >= playerCard.gameObject.GetComponent<Card>().requiresEnergy)
            {
                //actually if wraithform exists in the effect holder, this card should be inactive
                if (cardToCheck1 != null)
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
                }
                else
                {
                    playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
                }
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate smoke bomb button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 2) > 0 &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 169, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 152, 2), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //check additionally that the player has energy to use it
            if (playerCard.gameObject.GetComponent<CardDisplay2>().cooldown == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate blessed oil button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 114, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 114, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //check if you have curses, disable card if not
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 28, 7) == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
        }

        //instantiate potion of invulnerability button
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 168, 1) > 0)
        {
            playerCard = Instantiate(CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 168, 1), new Vector3(0, 0, 0), Quaternion.identity);   //defaultCombatButtons[1], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

            //dont show quantity
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "";

            //check additionally that you dont alrdy have the effect on
            //dont need cooldown then necessarily
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 168, 7) == 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
            }
            else
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //instantiate shield focus button
        playerCard = Instantiate(defaultCombatButtons[9], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

        /*
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Shield Focus</b><br><color=#FFD370>Active Ability</color><br>Temporarily increase the size of your shield for the cost of " +
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost + "<sprite index=11>.";
        }
        */
        //else
        playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Shield Focus</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost +
            "<sprite index=11>.<br>Temporarily increase the minimum and maximum score of your next roll by 1.";
        
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=12>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=10>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=13>";
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
        }
        else
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
        }

        //instantiate blessed shield button
        playerCard = Instantiate(defaultCombatButtons[12], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);

        playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Blessed Shield</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost +
           "<sprite index=12>.<br>Roll one additional die on your next defense roll.";

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 1)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=17>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 2)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
        }
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3)
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
        }

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)

        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = false;
        }
        else
        {
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;
        }


        //instantiates flee button
        //not when ensnared tho
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 171, 7) == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 176, 7) == 0 && 
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 182, 7) == 0 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 185, 7) == 0)
        {
            playerCard = Instantiate(defaultCombatButtons[3], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(StoreHandler.ins.storeCardArea.transform, false);
            playerCard.gameObject.GetComponent<CardDisplay2>().isEnabled = true;

            //special case if theres any opponent with swiftness, and hero doesnt have mount
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(2) &&
                    CardHandler.ins.mountSlot.transform.childCount == 0)//(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 2, 4) > 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Flee</b><br><color=#FFD370>Active Ability</color> <sprite index=11><sprite index=11> <sprite index=32><br>Flee from combat. Costs <sprite=\"sprites v92\" index=3> when <sprite index=11> not available.";
            }
            //special case if all foes are slow
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckIfAllFoesHaveSpecificAbility(35))//(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 35, 4) > 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Flee</b><br><color=#FFD370>Active Ability</color> <sprite index=32><br>Flee from combat.";
            }
            //special case for smoke bomb
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 7) > 0)
            {
                playerCard.gameObject.GetComponent<CardDisplay2>().tooltipText = "<b>Flee</b><br><color=#FFD370>Active Ability</color> <sprite index=32><br>Flee from combat.";
            }
        }

    }

    //used by enemy attacks in v0.5.7.
    //[PunRPC]
    public void BasicDefend(int hits, GameObject chargingCard, int foeNumber)
    {
        //dunno whats the point of this? (for rerolling purposes only i guess)
        //enemyAttackTypeUsed = enemyAttackType;

        //do we need this?
        heroKnockedOut = false;

        //use this variable for combat checks too
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = true;

        //hitsTaken = hits;

        //note that going to 0 energy doesnt yet knock down hero
        if (hits > GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health)
        {
            heroKnockedOut = true;
            gameObject.GetComponent<ExploreHandler>().encounterHandler1.skillCheckSuccess = false;
        }

        /*
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            gameObject.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);
        }
        */

        if (heroKnockedOut == true)
        {
            //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeStrategic1.currentEnergy -= hitsDone;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -hits);
            //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[0].SetHealthBarValues();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused = true;
            //GameManager.ins.references.GetComponent<CombatActions>().FinishBattleButton();
            GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithHeroDefeated();
        }
        else
        {
            ContinueFromDefenseCheckButton(hits, chargingCard, foeNumber);
        }
    }

    //repurposed from encounter button class
    public void ContinueFromDefenseCheckButton(int hits, GameObject chargingCard, int foeNumber)
    {
        //put these just in case (actually we shouldnt need these soon)
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost = 1;
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost = 1;

        //take damage
        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -hits);

        //lets do the special effects on another method (was on explore handler previously)
        //this method checks special effects of the charging card
        FoeEffects.ins.FoeDamageSkillEffect(chargingCard, hits);

        //lets check inflict abilities here too?
        //does it matter in which order we do these methods?
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckSpecialFoeHitAbilities(hits, foeNumber);

        //need to still do inflict checks etc, lets repurpose the old method
        //actually we should do this earlier, so we still know foenumber
        //CheckSpecialFoeHitAbilities(hits);

        /*
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == true)
        {
            GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_ContinueFromSpecialSkillCheck", RpcTarget.AllBufferedViaServer);
            return;
        }
        */

        //reset single shot defense abilities
        //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetSingleShotDefense();

        //only apply certain effects once per combat round for foes (such as regeneration, decays)
        //returns true, if foe was defeated
        /*these need to be done another way
         * 
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().OncePerRoundFoeEffects() == true)
        {
            return;
        }
        *


        / these are irrelevant in real time combat
         * 
         * give turn to player, if all foes have acted
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

            //lets do the foe turn change inside the defense phase method
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn += 1;
            GameManager.ins.exploreHandler.GetComponent<MultiCombat>().RotateFoeTurn(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn);

            //need to do this elsewhere, since this button gets deleted
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToDefensePhase();
        }
        */
    }

    void SpawnButtonsAfterBasicDefense()
    {
        gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().SpawnButtonsAfterBasicDefense();

        //isRerollPhase = true;
    }

    //need to call this here because the button which calls this gets deleted
    public void RerollDefenseButton()
    {
        gameObject.GetComponent<ExploreHandler>().PV.RPC("RPC_DeleteButtons", RpcTarget.AllBufferedViaServer);

        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().PV.RPC("RPC_CancelExplore6", RpcTarget.AllBufferedViaServer);


        Invoke("RerollDefense", 0.5f);
    }

    //add more attack types here soon
    void RerollDefense()
    {
        //encounterHandler1.buttonChosen2.SkillCheck();

        //reroll opponent default attack (either physical or magical)
        if (enemyAttackTypeUsed == 1)
        {
            //special cases for sfx
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[108].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().blockActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[109].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[110].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }
            else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().shieldOfIsoloreActivated == true)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = CardHandler.ins.generalDeck[111].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.intelligenceSfxHolder.Play();
            }

            //gameObject.GetComponent<ExploreHandler>().encounterHandler1.GetComponent<EncounterHandler>().buttonChosen2.DefaultDefendButton();
            GameManager.ins.references.GetComponent<CombatActions>().DefaultDefendButton();
        }
    }

    //maybe keep this here for now, although could be simpler to put it at BattlefieldFoe class
    //special check for inflict poison, lifesteal, inflict curse? etc
    public void CheckSpecialFoeHitAbilities(int hitsTaken, int foeNumber)
    {
        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1];

        //see if foe has inflict poison
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 1) == true && hitsTaken > 0)
        {
            //check if hero is sentinel, and therefore immune
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                //poisons equal to hitsTaken
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 23, 7, hitsTaken);
            }
        }

        //see if foe has life steal
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 3) == true && hitsTaken > 0)
        {
            //check if hero is sentinel, and therefore cant be life stolen
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                //round hitsdone / 2 upwards
                float hitsFloat = Mathf.Ceil(hitsTaken / 2f);
                int hitsInt = (int)hitsFloat;

                battlefieldFoe.thisStrategicEncounter.currentEnergy += hitsInt;

                if (battlefieldFoe.thisStrategicEncounter.currentEnergy > battlefieldFoe.thisStrategicEncounter.maxEnergy)//battlefieldFoe.thisStrategicEncounter.encounter.maxEnergy)
                {
                    battlefieldFoe.thisStrategicEncounter.currentEnergy = battlefieldFoe.thisStrategicEncounter.maxEnergy;
                }
                /* 
                GameManager.ins.references.currentStrategicEncounter.currentEnergy += hitsInt;

                if (GameManager.ins.references.currentStrategicEncounter.currentEnergy > GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxEnergy)
                {
                    GameManager.ins.references.currentStrategicEncounter.currentEnergy = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxEnergy;
                }
                */
            }
        }

        //see if foe has inflict immolation
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 46) == true && hitsTaken > 0)
        {
            //immolated equal to hitsTaken
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 173, 7, hitsTaken);
        }

        //see if foe has inflict curse
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 64) == true && hitsTaken > 0)
        {
            //curses
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 28, 7, 1);

            //could check if theres skill penalty
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();
        }

        //see if foe has inflict frostbite
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 66) == true && hitsTaken > 0)
        {
            int finalStacks = hitsTaken / 2;

            //frostbite equal to hitsTaken / 2
            if (finalStacks > 0)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 185, 7, finalStacks);
            }

            FoeEffects.ins.FrozenCheckWithDelay();
        }
        /*
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(55))
        {
            //lets put 1 less, since 1 was reduced alrdy?
            if (CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4) >= 14)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4, 6);
                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.SkipFoesTurn();
                DelayedFoesTurnSkip();
                return true;
            }
        }
        else
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4) >= 9)
            {
                CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4, 4);
                DelayedFoesTurnSkip();
                return true;
            }
        }
        */

        //change foe turn here (to 1 in this case)
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().SetFoeTurn(1);
    }

    public void DelayedFoesTurnSkip()
    {
        Invoke("SkipFoesTurn", 0.2f);
    }

    public void SkipFoesTurn()
    {
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.ContinueFromBasicDefenseCheckButton();
        //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.SkipFoesTurn();
    }

    //called when foe takes hits
    //might not need foenumber, but maybe use it just in case
    public void FoeTakesHitsCheckAbilities(int foeNumber)//(int hitsDone)
    {
        BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[foeNumber - 1];

        //see if foe has strength in numbers (5 interval)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 51) == true)
        {
            int counter = 0;

            for (int i = battlefieldFoe.thisStrategicEncounter.maxEnergy - 5; i >= (battlefieldFoe.thisStrategicEncounter.currentEnergy - hitsDone); i -= 5)
            {
                counter += 1;
                battlefieldFoe.thisStrategicEncounter.attack = battlefieldFoe.thisStrategicEncounter.maxAttack - counter;

                //is not optimal to have to do this twice tho..
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.arcanePower = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxArcanePower - counter;
            }
        }

        //see if foe has strength in numbers (4 interval)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 5) == true)
        {
            int counter = 0;

            for (int i = battlefieldFoe.thisStrategicEncounter.maxEnergy - 4; i >= (battlefieldFoe.thisStrategicEncounter.currentEnergy - hitsDone); i -= 4)
            {
                counter += 1;
                battlefieldFoe.thisStrategicEncounter.attack = battlefieldFoe.thisStrategicEncounter.maxAttack - counter;
                //is not optimal to have to do this twice tho..
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.attack = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxAttack - counter;
            }
        }

        //see if foe has strength in numbers (2 interval)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 6) == true)
        {
            int counter = 0;

            //updated for v92 (need to check hitsdone too, because damage is added after this method is called)
            for (int i = battlefieldFoe.thisStrategicEncounter.maxEnergy - 2; i >= (battlefieldFoe.thisStrategicEncounter.currentEnergy - hitsDone); i -= 2)
            {
                counter += 1;
                battlefieldFoe.thisStrategicEncounter.attack = battlefieldFoe.thisStrategicEncounter.maxAttack - counter;
                //is not optimal to have to do this twice tho..
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.attack = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxAttack - counter;
            }
        }

        //see if foe has strength in numbers (3 interval)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 9) == true)
        {
            int counter = 0;

            for (int i = battlefieldFoe.thisStrategicEncounter.maxEnergy - 3; i >= (battlefieldFoe.thisStrategicEncounter.currentEnergy - hitsDone); i -= 3)
            {
                counter += 1;
                battlefieldFoe.thisStrategicEncounter.attack = battlefieldFoe.thisStrategicEncounter.maxAttack - counter;

                //is not optimal to have to do this twice tho..
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.attack = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxAttack - counter;
            }
        }

        //see if foe has strength in numbers (3 interval, arcane)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 12) == true)
        {
            int counter = 0;

            for (int i = battlefieldFoe.thisStrategicEncounter.maxEnergy - 3 - 3; i >= (battlefieldFoe.thisStrategicEncounter.currentEnergy - hitsDone); i -= 3)
            {
                counter += 1;
                battlefieldFoe.thisStrategicEncounter.arcanePower = battlefieldFoe.thisStrategicEncounter.maxArcanePower - counter;

                //is not optimal to have to do this twice tho..
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.arcanePower = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxArcanePower - counter;
            }
        }

        //see if foe has enrage
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 19) == true)
        {
            if (hitsDone > 0 && battlefieldFoe.CheckQuantity(19) < 3)//CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 19, 4) < 3)
            {
                //add attack if damage done to foe, and foe has less than 3 stacks on enrage
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxAttack += 1;
                GameManager.ins.references.currentStrategicEncounter.attack += 1;
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.attack += 1;

                //add engrage stack
                //actually we want to draw it to the battlefield foe holder
                //CardHandler.ins.DrawCards(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 19, 4, 1);
                GameObject cardToDraw = CardHandler.ins.CopyFoeCard(foeNumber, 19);
                battlefieldFoe.DrawActiveCards(cardToDraw, 1);
            }
        }

        //see if foe has enrage (arcane)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 53) == true)
        {
            if (hitsDone > 0 && battlefieldFoe.CheckQuantity(53) < 3)
            {
                //add attack if damage done to foe, and foe has less than 3 stacks on enrage
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.maxAttack += 1;
                GameManager.ins.references.currentStrategicEncounter.arcanePower += 1;
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.attack += 1;

                //add engrage stack
                GameObject cardToDraw = CardHandler.ins.CopyFoeCard(foeNumber, 53);
                battlefieldFoe.DrawActiveCards(cardToDraw, 1);
            }
        }

        //see if foe has flame aura (1 stack of immolation), and theres hits done?
        //only works against melee attacks
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 33) == true 
            && hitsDone > 0 && attackTypeUsed == 1)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 173, 7, 1);

            //play arcane orb sfx?
            //that way dont need to make whole new method :-)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[103].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //see if foe has flame aura (2 stacks of immolation), and theres hits done?
        //only works against melee attacks
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 34) == true 
            && hitsDone > 0 && attackTypeUsed == 1)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 173, 7, 2);

            //play arcane orb sfx?
            //that way dont need to make whole new method :-)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[103].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //see if foe has frost aura (2 stacks of frostbite), and theres hits done?
        //only works against melee attacks
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 61) == true 
            && hitsDone > 0 && attackTypeUsed == 1)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 185, 7, 2);

            FoeEffects.ins.FrozenCheckWithDelay();

            //play arcane orb sfx?
            //that way dont need to make whole new method :-)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[103].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //see if foe has frost aura (1 stacks of frostbite), and theres hits done?
        //only works against melee attacks
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(foeNumber, 63) == true 
            && hitsDone > 0 && attackTypeUsed == 1)
        {
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 185, 7, 1);

            FoeEffects.ins.FrozenCheckWithDelay();

            //play arcane orb sfx?
            //that way dont need to make whole new method :-)
            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[103].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //daze is handled elsewhere now

        //return false;
        //need this here too? (for strength in number, rage etc stat updates)
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();
    }

    public int CheckFoeProtectionAbilities(int hitsDone, int attackTypeDone)
    {
        //do specials first?
        //see if foe has activated magic shell
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].CheckFoeActiveAbility(14) == true)
        {
            //check blessed blade or oil, or holy sword
            if (attackTypeDone == 1 && (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 113, 7) > 0 || (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 114, 7) > 0) ||
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 4, 33) == true))
            {
                hitsDone = hitsDone / 2;
            }
            else if ((GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated) && attackTypeDone == 2)
            {
                hitsDone = hitsDone / 2;
            }
            else
            {
                hitsDone = 0;
            }
        }

        /* actually lets do the dodge check earlier, so it effects more than just scaled damage
        //see if foe has activated dodge
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(16) &&
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeDefensePhase == true)
        {
            //hitsDone = hitsDone / 2;
            hitsDone = hitsDone - 2;

            if (hitsDone < 0)
            {
                hitsDone = 0;
            }
        }
        */

        //see if foe has activated block
        if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].CheckFoeActiveAbility(20) == true)
        {
            hitsDone = hitsDone / 2;
        }

        //new system for v94
        //melee attacks
        //modified for v0.7.1.
        if (attackTypeDone == 1)
        {
            float defenseModifier = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].thisStrategicEncounter.encounter.defenseMod;//GameManager.ins.references.currentEncounter.defenseMod;

            if (GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].thisStrategicEncounter.secondWindActivated == true)
            {
                //longest line ever :-)
                defenseModifier = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].thisStrategicEncounter.encounter.combatButton.secondPhase.GetComponent<Encounter2>().defenseMod;
            }

            //blessed blade / oil / holy avenger
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 113, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 114, 7) > 0 ||
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 4, 33) == true)
            {
                defenseModifier -= 50f;
            }

            if(defenseModifier < 0f)
            {
                defenseModifier = 0f;
            }

            //round up the damage reduction?
            float damageReduction = Mathf.Ceil(hitsDone * (defenseModifier / 100));
            int damageReductionRoundedUp = (int)damageReduction;

            hitsDone = hitsDone - damageReductionRoundedUp;
        }

        //arcane attacks
        if (attackTypeDone == 2)
        {
            float resistanceModifier = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].thisStrategicEncounter.encounter.resistanceMod;

            if(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].thisStrategicEncounter.secondWindActivated == true)
            {
                //longest line ever :-)
                resistanceModifier = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].thisStrategicEncounter.encounter.combatButton.secondPhase.GetComponent<Encounter2>().resistanceMod;
            }

            //bolt of light, spear of light
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated || GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated)
            {
                resistanceModifier -= 50f;
            }

            if (resistanceModifier < 0f)
            {
                resistanceModifier = 0f;
            }

            //round up the damage reduction?
            float damageReduction = Mathf.Ceil(hitsDone * (resistanceModifier / 100));
            int damageReductionRoundedUp = (int)damageReduction;

            hitsDone = hitsDone - damageReductionRoundedUp;
        }

        return hitsDone;
    }

    //unused in v0.5.7.
    //used for foe special attacks
    //returns true, if cooldwon ability triggers
    bool CheckFoeAttackCooldownAbilities()
    {
        //see if foe has throw curse
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(13))
        {
            if(CardHandler.ins.CheckFoeCardTimers(13) == true)
            {
                return true;
            }
        }

        //see if foe has shadow bolt
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(15))
        {
            if (CardHandler.ins.CheckFoeCardTimers(15) == true)
            {
                return true;
            }
        }

        //see if foe has bombardment
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(17))
        {
            if (CardHandler.ins.CheckFoeCardTimers(17) == true)
            {
                return true;
            }
        }

        //confusion
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(18))
        {
            if (CardHandler.ins.CheckFoeCardTimers(18) == true)
            {
                return true;
            }
        }

        //power strike
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(28))
        {
            if (CardHandler.ins.CheckFoeCardTimers(28) == true)
            {
                return true;
            }
        }

        //toxic vial
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(30))
        {
            if (CardHandler.ins.CheckFoeCardTimers(30) == true)
            {
                return true;
            }
        }

        //greater fireball
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(36))
        {
            if (CardHandler.ins.CheckFoeCardTimers(36) == true)
            {
                return true;
            }
        }

        //see if foe has entangle (1 or 2)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(37))
        {
            if (CardHandler.ins.CheckFoeCardTimers(37) == true)
            {
                return true;
            }
        }

        //see if foe has shadow strike
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(44))
        {
            if (CardHandler.ins.CheckFoeCardTimers(44) == true)
            {
                return true;
            }
        }

        //fireball
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(47))
        {
            if (CardHandler.ins.CheckFoeCardTimers(47) == true)
            {
                return true;
            }
        }

        //see if foe has webbing (1 or 2)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(49))
        {
            if (CardHandler.ins.CheckFoeCardTimers(49) == true)
            {
                return true;
            }
        }

        //see if foe has doom gaze (1 - 3)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(56))
        {
            if (CardHandler.ins.CheckFoeCardTimers(56) == true)
            {
                return true;
            }
        }

        //see if foe has devastating power strike (powerful charge etc)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(59))
        {
            if (CardHandler.ins.CheckFoeCardTimers(59) == true)
            {
                return true;
            }
        }

        //see if foe has fire breath 1
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(60))
        {
            if (CardHandler.ins.CheckFoeCardTimers(60) == true)
            {
                return true;
            }
        }

        //see if foe has frost breath 2
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(62))
        {
            if (CardHandler.ins.CheckFoeCardTimers(62) == true)
            {
                return true;
            }
        }

        //see if foe has dark gaze
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(65))
        {
            if (CardHandler.ins.CheckFoeCardTimers(65) == true)
            {
                return true;
            }
        }

        //see if foe has frost bolts 1
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(67))
        {
            if (CardHandler.ins.CheckFoeCardTimers(67) == true)
            {
                return true;
            }
        }

        //see if foe has frost bolts 2
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(68))
        {
            if (CardHandler.ins.CheckFoeCardTimers(68) == true)
            {
                return true;
            }
        }

        //precise strike
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(71))
        {
            if (CardHandler.ins.CheckFoeCardTimers(71) == true)
            {
                return true;
            }
        }

        return false;
    }

    //used for foe special defenses
    //returns true, if cooldwon ability triggers
    bool CheckFoeDefenseCooldownAbilities()
    {
        //see if foe has magic shell
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(14))
        {
            if (CardHandler.ins.CheckFoeCardTimers(14) == true)
            {
                return true;
            }
        }

        //see if foe has dodging
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(16))
        {
            if (CardHandler.ins.CheckFoeCardTimers(16) == true)
            {
                return true;
            }
        }

        //see if foe has block
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(20))
        {
            if (CardHandler.ins.CheckFoeCardTimers(20) == true)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetHeroAttackFlags()
    {
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().powerAttackActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().preciseStrikeActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated = false;
        //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated = false;

        //reduce quantity of blessed blade, oil & earth magic
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 113, 7, 1);
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 114, 7, 1);
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 153, 7, 1);
    }

    public void ResetHeroDefenseFlags()
    {
        //dont rly need these anymore, but might as well
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().blockActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().shieldOfIsoloreActivated = false;

        //reduce quantity of spirit form & smoke bomb
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 112, 7, 1);
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 152, 7, 1);

        //reduce quantity of potion of invulnerability
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 168, 7, 1);

        //reduce quantity of ensnare
        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 171, 7, 1);

        //lets check immolation at CheckSpecialFoeHitAbilities
    }

    //resetted after single defense turn
    public void ResetSingleShotDefense()
    {
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().blockActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated = false;
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().shieldOfIsoloreActivated = false;
    }

    //shouldnt be used like this in v0.5.7.
    //returns true, if foe or hero is defeated
    public bool ChecksWhenGoingDefensePhase()
    {
        //this also now includes stuff like regeneration
        //also daze
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().FoeTakesHitsCheckAbilities(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone);

        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -hitsTaken);

            Debug.Log("checks defense phase hp decrease");
        }

        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetHeroDefenseFlags();

        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase = false;

        /* removed 
        //special effect if foe has certain abilities
        //returns true, if foe or hero died at the end of his turn
        //immolation is handled here too
        //also put foe turn change here, so the foe specials trigger on correct foe
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckSpecialFoeHitAbilities(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken) == true)
        {
            return true;
        }
        */
        return false;
    }

    //this is technically unused now?
    //returns true, if foe or hero is defeated
    public bool ChecksWhenGoingAttackPhase()
    {
        //this also now includes stuff like regeneration
        //also hero energy drain
        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().FoeTakesHitsCheckAbilities(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget);//(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone);
        /*
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().FoeTakesHitsCheckAbilities(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsDone) == true)
        {
            return true;
        }
        */

        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
        {
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy -= hitsDone;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -hitsTaken);

            Debug.Log("checks attack phase hp decrease");
        }

        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ResetHeroDefenseFlags();

        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isRerollPhase = false;

        /*special effect if foe has certain abilities
        //returns true, if foe or hero died at the end of his turn
        //immolation is handled here too
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().CheckSpecialFoeHitAbilities(GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken) == true)
        {
            return true;
        }
        */
        return false;
    }

    //this handles effects which should only be checked once per round, per foe
    //checks after defense check
    public bool OncePerRoundFoeEffects()
    {
        //reduce 1 immolated & 1 daze
        CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 240, 4, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4, 1);

        //see if foe has regeneration (1 energy)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(24))
        {
            if (GameManager.ins.references.currentStrategicEncounter.currentEnergy < GameManager.ins.references.currentStrategicEncounter.maxEnergy)
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy += 1;
            }
        }

        //see if foe has regeneration (2 energy)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(25))
        {
            if (GameManager.ins.references.currentStrategicEncounter.currentEnergy < GameManager.ins.references.currentStrategicEncounter.maxEnergy)
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy += 2;
            }

            if (GameManager.ins.references.currentStrategicEncounter.currentEnergy > GameManager.ins.references.currentStrategicEncounter.maxEnergy)
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy = GameManager.ins.references.currentStrategicEncounter.maxEnergy;
            }
        }


        //special case for decaying foes
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(7) == true)
        {
            if (GameManager.ins.references.currentStrategicEncounter.currentEnergy <= 1)
            {
                //need to set this
                opponentDefeated = true;

                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;

                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= 1;

                //should call this, in case enemy stats have changed?
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

                //GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated();
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();

                //lets reduce hero hp here for now, since the following method wont be called, if foe is defeated on its own turn
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
                }

                return true;
            }
            else
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= 1;
                //should call this, in case enemy stats have changed
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();
            }
        }

        //special case for decaying foes (2)
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(11) == true)
        {
            if (GameManager.ins.references.currentStrategicEncounter.currentEnergy <= 2)
            {
                //need to set this
                opponentDefeated = true;

                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;

                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= 2;

                //should call this, in case enemy stats have changed
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

                //GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated();
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();

                //lets reduce hero hp here for now, since the following method wont be called, if foe is defeated on its own turn
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
                }

                return true;
            }
            else
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= 2;
                //should call this, in case enemy stats have changed
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();
            }
        }

        //special case for immolated foes
        if (CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 240, 4) >= 3)
        {
            int damageTaken = CardHandler.ins.CheckQuantity(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 240, 4) / 3;

            if (GameManager.ins.references.currentStrategicEncounter.currentEnergy <= damageTaken)
            {
                //need to set this
                opponentDefeated = true;

                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().numberOfFoes -= 1;

                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= damageTaken;

                //should call this, in case enemy stats have changed?
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

                //GameManager.ins.references.GetComponent<CombatActions>().FinishBattleWithFoeDefeated();
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();

                //lets reduce hero hp here for now, since the following method wont be called, if foe is defeated on its own turn
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken > 0)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(3, -GameManager.ins.exploreHandler.GetComponent<CombatHandler>().hitsTaken);
                }

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[240].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();

                return true;
            }
            else
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= damageTaken;

                CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[240].GetComponent<CardDisplay2>().sfx;
                CardHandler.ins.extraSfxHolder.Play();
                //should call this, in case enemy stats have changed
                //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();
            }
        }

        return false;
    }

    //returns true, if hero was defeated, or extra strike / time warp was used
    //checked after hero attack phase, or when foe or hero is defeated
    //called after hero attack phase (or when foe defeated)
    public void AdditionalHeroDamageEffects()
    {
        //should use this for simplicitys sake
        //BattlefieldFoe battlefieldFoe = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1];

        //shouldnt calculate these, if theres no hits done?
        if (hitsDone > 0)
        {
            //gorms hammer check
            if (attackTypeUsed == 1 &&
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 4, 245) == true)
            {
                //add daze stacks to foe
                //unless foe is daze immune
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 245) == false)
                {
                    //CardHandler.ins.DrawCards(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().foeTurn, 241, 4, hitsDone);
                    GameObject cardToDraw = CardHandler.ins.generalDeck[241];
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].DrawActiveCards(cardToDraw, hitsDone);
                }
            }

            //see if hero has wraithform, and you used arcane attack (was used for energy drain ability)
            //need undead & machine checks here later
            //actually better to use the flag variable for easier tracking?
            if (attackTypeUsed == 2 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) > 0) //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wraithsGiftActivated == true)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) > 0)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 165, 2) > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, hitsDone / 3);
            }

            //see if hero has avatar of fire, and you used arcane attack
            if (attackTypeUsed == 2 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 237, 7) > 0)//CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 165, 2) > 0)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, hitsDone / 3);

                //add immolated stacks to foe
                //note that drawing new foe cards need same card & effect number, because of reasons
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 244) == false)
                {
                    GameObject cardToDraw = CardHandler.ins.generalDeck[240];
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].DrawActiveCards(cardToDraw, hitsDone);
                }
            }
        }

        //firebomb infusion
        //seems the flags are alrdy resetted before this, but attack type is not?
        if ((throwBombsActivated == true || throwManaBombsActivated == true || detonationActivated == true || arcaneDetonationActivated == true) && 
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 239, 7) > 0)
        {
            //actually we should check hits done separately here (because we wanna reduce the charge even with misses)
            //add immolated stacks to foe
            //note that drawing new foe cards need same card & effect number, because of reasons
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 244) == false &&
                hitsDone > 0)
            {
                GameObject cardToDraw = CardHandler.ins.generalDeck[240];
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].DrawActiveCards(cardToDraw, hitsDone);
            }

            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 239, 7, 1);
        }

        //concussion bomb infusion
        if ((throwBombsActivated == true || throwManaBombsActivated == true || detonationActivated == true || arcaneDetonationActivated == true) &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 238, 7) > 0)
        {
            //add daze stacks to foe
            //note that drawing new foe cards need same card & effect number, because of reasons
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 245) == false &&
                hitsDone > 0)
            {
                GameObject cardToDraw = CardHandler.ins.generalDeck[241];
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].DrawActiveCards(cardToDraw, hitsDone);
            }

            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 238, 7, 1);
        }

        //wrath of guliman
        //actually could make this affect all arcane attacks?
        if (attackTypeUsed == 2 && //(boltOfLightActivated == true || spearOfLightActivated == true) &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 236, 7) > 0)
        {
            //add daze stacks to foe
            //note that drawing new foe cards need same card & effect number, because of reasons
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromSpecificFoe(GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget, 245) == false
                && hitsDone > 0)
            {
                GameObject cardToDraw = CardHandler.ins.generalDeck[241];
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].DrawActiveCards(cardToDraw, hitsDone);
            }

            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 236, 7, 1);
        }

        //reduce earth magic stack, if using arcane attack
        if (attackTypeUsed == 2 && //(boltOfLightActivated == true || spearOfLightActivated == true) &&
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 153, 7) > 0)
        {
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 153, 7, 1);
        }

        //might need delay for daze check?
        DazeCheckWithDelay();

        /* dont need this anymore?
         * 
         * special case for extra strike & time warp
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == true || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 151, 7) > 0)
        {
            //reset foe special defense phase flag
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeDefensePhase = false;

            isRerollPhase = false;

            if (hitsDone > 0)
            {
                GameManager.ins.references.currentStrategicEncounter.currentEnergy -= hitsDone;
            }
            GameManager.ins.references.currentEncounter.ShowEnemyStats();

            //resets flags for hero special attacks
            ResetHeroAttackFlags();

            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 151, 7, 1);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().GoToRealTimeCombat(true);

            return true;
        }
        */

        /* dont need these anymore either
         * 
         * reduce quantity of blessed blade, oil & earth magic
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 113, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 114, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 153, 7, 1);

        //reduce quantity of wraiths gift & smoke bomb 1/2 & berserk & avatar of fire
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 112, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 152, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 232, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 235, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 237, 7, 1);

        //reduce wrath of guliman, concussion bomb infusion, firebomb infusion
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 236, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 238, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 239, 7, 1);

        //reduce quantity of potion of invulnerability & power
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 168, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 298, 7, 1);

        //reduce quantity of ensnares
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 171, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 176, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 182, 7, 1);
        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 185, 7, 1);
        */

        //this is done in reducequantity method alrdy
        //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().StatUpdate();

        /* shouldnt need any of this either?
         * 
         * the combat card cooldowns could be handled here as well
        //special case for extra strike & time warp
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated == false && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated == false) //CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 151, 7) == 0)
        {
            CardHandler.ins.ReduceHeroCombatCardTimers(GameManager.ins.turnNumber, 1);
        }

        CardHandler.ins.ReduceHeroCombatCardTimers(GameManager.ins.turnNumber, 2);


        //immolation calculations (1 damage per 3 stacks of immolation)
        if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 173, 7) > 0)
        {
            int damageTaken = CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 173, 7) / 3;

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health < damageTaken)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -damageTaken);

                //need to set this
                heroKnockedOut = true;
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = false;

                //should call this, in case enemy stats have changed
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption.ShowEnemyStats();

                //GameManager.ins.references.GetComponent<CombatActions>().FinishBattleButton();

                cant use finishbattlebutton here, rather do combat end like this?
                GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
                GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
                GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
                GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
                

                //changed in v95
                //kinda dont need the health check tho
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isDead == true)
                {
                    //need to set this?
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut = true;
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().skillCheckSuccess = false;
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);

                    return true;
                }

                //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().ContinueFromSkillCheck();

                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().usedButton1.CloseEnemyStatDisplay();

                //change foe turn here (to 1 in this case)
                //GameManager.ins.exploreHandler.GetComponent<MultiCombat>().SetFoeTurn(1);

                //return true;
            }

            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -damageTaken);

            //remove 1 stack
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 173, 7, 1);
        }

        return false;
        */
    }

    public void DazeCheckWithDelay()
    {
        Invoke(nameof(DazeCheck), 0.1f);
    }

    //need this with delay for some reason?
    public void DazeCheck()
    {
        //made separate method for daze handling (since it may need to be more often than just here)
        GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].DazeCheck();

    }

    public void GoToCombatWithCheck()
    {
        //enemies strike first, if any of them has alert
        //and if hero doesnt have swift mount
        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckAbilityFromAnyFoe(27) == true &&
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 64) == false &&
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 67) == false &&
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 68) == false &&
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 69) == false)
        {
            //combat begins (opponents have alert)
            GoToRealTimeCombat(false);
        }
        else
        {
            //combat begins (opponents not alert)
            GoToRealTimeCombat(true);
        }
    }
}
