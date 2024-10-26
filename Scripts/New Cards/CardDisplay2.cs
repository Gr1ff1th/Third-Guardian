using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

//handles new "card" popup functions
public class CardDisplay2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject roomController;

    //cooldowns
    public bool isCooldownPassive;
    public bool isCombatCooldownCard;
    public int maxCooldown;
    public int cooldown;
    public int startingCooldown;

    //combat cards in v0.5.7. need float values
    public bool useMaxCooldownOnlyAtStart;
    public float realTimeMaxCooldown;
    public float realTimeCooldown;
    public float realTimeStartingCooldown;
    public float effectTimeMax;
    public float effectTime;
    //for foe cards
    public float chargingTime;
    //public int timer;

    //counts how many of this same card you got
    //actually this is also "timer" in many cases
    public int quantity;

    public TextMeshProUGUI quantityText;

    //if true, shows quantity even at 1 quantity
    public bool showQuantityAlways;

    public GameObject overlay;

    //used for current characters blinking
    public float blinkSpeed;
    public float blinkTimer;

    //tooltip text
    [TextArea(5, 20)]
    public string tooltipText;

    //cooldown text
    //actually used by equipment cards now also for different purpose..
    [TextArea(5, 20)]
    public string coolDownText;

    //if true, the card blinks and can be activated
    public bool isEnabled;

    //play this sfx when card is activated (normally)
    public AudioClip sfx;

    //show special icon on some cases
    public Sprite specialIcon;

    //icons used for special attacks
    public Sprite specialCrosshairIcon;
    public Sprite specialShieldIcon;

    //text when activating the special ability
    [TextArea(5, 20)]
    public string specialText;

    //used for foe special abilities
    public List<GameObject> specialButtons;
    //used after first choice is given in the first special buttons dialog
    public List<GameObject> specialButtons2;

    //set true, if check allows special buttons + additional buttons
    public bool defaultSkillCheckOptions;

    //set true, if only buttons on specialButtons are available
    public bool specialSkillCheckOptions;

    public Sprite goldSprite;
    public bool goldImageSwapped;

    //set true when cannot be used when paused
    public bool cannotBeUsedWhenPaused;

    //for v95 (only works for combat cards)
    public bool playSfxWithDelay;

    //for foe triggered skillchecks
    //used by various special attacks (and maybe normal attacks too)
    //note that in foe attacks type 1 is foes attack value and type 3 is arcane attack value
    public int foeCardRequirementType;
    public int foeCardRequirementQty;
    public bool usesFoeDefaultAttackValue;

    // Start is called before the first frame update
    void Start()
    {
        //isEnabled = false;

        //this should be available at all scenes I hope?
        roomController = GameObject.Find("AMMRoomController");

        //overlay.GetComponent<RectTransform>().rect.width = gameObject.GetComponent<RectTransform>().rect.width;
        
        /*need to check if the card is at effect holder (need different overlay size, dunno other way of doing this)
        if (overlay != null && gameObject.GetComponentInParent<ContentSizeFitter>() != null)
        {
            if (ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.effectCardArea))
            {
                //float cellSize = gameObject.GetComponent<RectTransform>().rect.width;
                float cellSize = 26;
                //Debug.Log("cellsize is: " + cellSize);
                overlay.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);
            }
        }
        */
    }

    //used for overlay blinking
    private void Update()
    {
        /* lets remove the blinking for now
        if (gameObject.activeSelf)
        {
            //only do the rest if the card is enabled (and its your turn)
            if (isEnabled == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
            {
                blinkTimer += Time.deltaTime * blinkSpeed;

                //lets try this for blinking 
                if (blinkTimer > 100 && overlay != null)
                {
                    overlay.SetActive(true);
                    blinkTimer = 0;
                    Invoke("RemoveOverlay", 0.15f);
                }
            }
        }
        */
        //special case for char selection screen background cards
        if(roomController.GetComponent<PhotonRoom>().currentScene == 2 && gameObject.GetComponent<Card>().cardType == 14)
        {
            //add special case for scene 1 here
            if (isEnabled == false)
            {
                Image image = gameObject.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = 0.5f;
                image.color = tempColor;
            }

            if (isEnabled == true)
            {
                Image image = gameObject.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = 1f;
                image.color = tempColor;
            }
        }

        //need some sort of safety check(s)
        if (roomController.GetComponent<PhotonRoom>().currentScene < 3)
        {
            return;
        }
        if (roomController.GetComponent<PhotonRoom>().isGameLoaded == false)
        {
            return;
        }

        //special case for when not enough favor
        if (isEnabled == true)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < gameObject.GetComponent<Card>().requiresFavor)
            {
                isEnabled = false;
            }
        }

        //special case for when not enough energy
        if (isEnabled == true)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < gameObject.GetComponent<Card>().requiresEnergy)
            {
                isEnabled = false;
            }
        }

        //special case for when not enough coins
        if (isEnabled == true)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins < gameObject.GetComponent<Card>().requiresCoins)
            {
                isEnabled = false;
            }
        }

        //special case for blood rage
        if (isEnabled == true && gameObject.GetComponent<Card>().effect == 107)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().health < 2)
            {
                isEnabled = false;
            }
        }

        //special case for nourishing meal & sentinel
        if (isEnabled == true && gameObject.GetComponent<Card>().effect == 21)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == true)
            {
                isEnabled = false;
            }
        }

        //special case for portal scrolls & spells & blue citadel
        if (isEnabled == true && (gameObject.GetComponent<Card>().effect == 114 || gameObject.GetComponent<Card>().effect == 286))
        {
            if (GameManager.ins.references.currentMinimap != null)
            {
                if (GameManager.ins.references.currentMinimap.minimapNumber == 73 ||
                GameManager.ins.references.currentMinimap.minimapNumber == 74)
                {
                    isEnabled = false;
                }
            }
        }

        //this might cause lag?
        if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
        {
            if (ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.levelupCardArea) ||
                ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.levelupCardArea2) ||
                ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.levelupCardArea3))
            {
                /*
                Image image = gameObject.GetComponent<Image>();
                var tempColor = image.color;

                tempColor.a = 0.75f;   //GameManager.ins.colorChangeValue;

                image.color = tempColor;
                */

                if(goldImageSwapped == false && goldSprite != null)
                {
                    gameObject.GetComponent<Image>().sprite = goldSprite;
                    goldImageSwapped = true;
                }
                return;
            }
        }

        //this might cause lag?
        if (gameObject.GetComponentInParent<ContentSizeFitter>() != null && gameObject.activeSelf)
        {
            if (ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.artifactCardArea))
            {
                return;
            }
            //dont gray out store items
            if ((CardHandler.ins.phaseNumber == 5 || CardHandler.ins.phaseNumber == 6) && ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, StoreHandler.ins.storeCardArea))
            {
                Image image = gameObject.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = 1f;
                image.color = tempColor;
                return;
            }

            //new case for combat cooldown cards
            //calculates cooldown time here
            if ((CardHandler.ins.phaseNumber == 3) && 
                ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.combatCardArea))//StoreHandler.ins.storeCardArea))
            {
                //make stun check here too
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused == false &&
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isPetrified == false &&
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen == false)
                {
                    if(realTimeCooldown > 0)
                    {
                        ApplyCooldown();
                    }
                    else
                    {
                        overlay.GetComponent<Image>().fillAmount = 0f;
                    }
                }
            }
            //calculates effect cooldown time here
            if ((CardHandler.ins.phaseNumber == 3) &&
                ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.effectCardArea))
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused == false)
                {
                    if (effectTime > 0)
                    {
                        ApplyEffectTime();
                    }
                    else
                    {
                        overlay.GetComponent<Image>().fillAmount = 0f;
                    }
                }
            }

            //calculates effect time for foe cards
            if ((CardHandler.ins.phaseNumber == 3) && GetComponentInParent<ScrollRectCenter>() != null)
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused == false && GetComponentInParent<ScrollRectCenter>().isFoeCardHolder == true)
                {
                    if (effectTime > 0)
                    {
                        ApplyFoeEffectTime();
                    }
                    else
                    {
                        overlay.GetComponent<Image>().fillAmount = 0f;
                    }
                }
            }
        }

        /* actually this doesnt work
         * we dont want active check here (since the cards arent active)
        //foe card handling (in foe card area, need separate handling for mini-icons)
        if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
        {
            if ((CardHandler.ins.phaseNumber == 3) &&
            ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea))
            {
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused == false)
                {
                    if (realTimeCooldown > 0)
                    {
                        ApplyCooldown();
                    }
                    else
                    {
                        overlay.GetComponent<Image>().fillAmount = 0f;
                    }
                }
            }
        }
        */

        //lets try make un-enabled cards transparent?
        if (gameObject.GetComponent<Card>() != null && gameObject.activeSelf)
        {
            //add special case for scene 1 here
            if (isEnabled == false && gameObject.GetComponent<Card>().isUsable == true)
            {
                Image image = gameObject.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = 0.5f;
                image.color = tempColor;
            }

            if (isEnabled == true && gameObject.GetComponent<Card>().isUsable == true)
            {
                Image image = gameObject.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = 1f;
                image.color = tempColor;
            }
        }
    }

    //need to do something like this to update the "hidden" foe cards
    public void UpdateHiddenCard()
    {
        //Debug.Log("call update hidden card");

        //if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
        //{
        if (CardHandler.ins.phaseNumber == 3) //&&            ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea))
        {
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().realTimeCombatPaused == false && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated == false)
            {
                //special case for defense cards when stunned (wont count cooldown)
                if ((gameObject.GetComponent<Card>().defenseCard == true &&
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[gameObject.GetComponent<Card>().belongsTo - 1].thisStrategicEncounter.isStunned == true) ||
                    GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[gameObject.GetComponent<Card>().belongsTo - 1].foeDefeated == true)
                {
                    //nothing happens
                }
                else
                {
                    if (realTimeCooldown > 0)
                    {
                        ApplyCooldown();
                    }
                    else
                    {
                        overlay.GetComponent<Image>().fillAmount = 0f;

                        //this handles defense abilities & hidden passive effects
                        if (gameObject.GetComponent<Card>().foePassiveCooldownCard == true)
                        {
                            FoeEffects.ins.FoeHiddenEffect(gameObject);

                            //lets give this bit of randomness
                            float randomTimer = Random.Range(-1f, 1f);
                            randomTimer += realTimeMaxCooldown;
                            realTimeCooldown = randomTimer;
                        }
                    }
                }
            }
        }
        //}
    }

    void ApplyCooldown()
    {
        realTimeCooldown -= Time.deltaTime * GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().currentBattleSpeed;
        
        if(realTimeCooldown > 0)
        {
            if (realTimeCooldown > realTimeMaxCooldown)
            {
                overlay.GetComponent<Image>().fillAmount = 1f;
            }
            else
            {
                overlay.GetComponent<Image>().fillAmount = realTimeCooldown / realTimeMaxCooldown;
            }
        }
    }

    void ApplyEffectTime()
    {
        effectTime -= Time.deltaTime * GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().currentBattleSpeed;

        if (effectTime > 0)
        {
            if (effectTime > effectTimeMax)
            {
                overlay.GetComponent<Image>().fillAmount = 1f;
            }
            else
            {
                overlay.GetComponent<Image>().fillAmount = effectTime / effectTimeMax;
            }
        }
        else
        {
            overlay.GetComponent<Image>().fillAmount = 0f;

            //need special case for immolation  when it triggers?
            //actually we need to check petrified & frostbitten too, but in a special way
            //web & ensnare are calculated automatically at stack reduce
            if (gameObject.GetComponent<Card>().effect == 102 || gameObject.GetComponent<Card>().effect == 109 || gameObject.GetComponent<Card>().effect == 110)
            {
                //name of this method is kinda misleading..
                FoeEffects.ins.HeroEffectTrigger(gameObject);
            }

            //need to add effect remove functions here (and check quantity etc)
            if (quantity == 1)
            {
                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ReactivateDependentCards(gameObject.GetComponent<Card>().effect);
                //Destroy(gameObject);
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, 7, 1);
            }
            else
            {
                //quantity -= 1;
                //quantityText.text = quantity.ToString();
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, 7, 1);
                effectTime = effectTimeMax;
            }
        }
    }

    //for foe active card holder
    void ApplyFoeEffectTime()
    {
        effectTime -= Time.deltaTime * GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().currentBattleSpeed;

        if (effectTime > 0)
        {
            if (effectTime > effectTimeMax)
            {
                overlay.GetComponent<Image>().fillAmount = 1f;
            }
            else
            {
                overlay.GetComponent<Image>().fillAmount = effectTime / effectTimeMax;
            }
        }
        else
        {
            overlay.GetComponent<Image>().fillAmount = 0f;

            //need special case for immolation & daze when they "trigger"?
            if (gameObject.GetComponent<Card>().effect == 240 || gameObject.GetComponent<Card>().effect == 241)
            {
                //name of this method is kinda misleading..
                FoeEffects.ins.FoeHiddenEffect(gameObject);
            }

            //need to add effect remove functions here (and check quantity etc)
            if (quantity == 1)
            {
                Destroy(gameObject);
                return;
                //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, 7, 1);
            }

            else
            {
                quantity -= 1;
                quantityText.text = quantity.ToString();
                //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, 7, 1);
                effectTime = effectTimeMax;
            }
        }
    }

    void RemoveOverlay()
    {
        if (gameObject != null)
        {
            overlay.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (roomController.GetComponent<PhotonRoom>().currentScene > 2)
        {
            GameManager.ins.toolTipBackground.SetActive(true);

            //special check for toxicology
            if (gameObject.GetComponent<Card>() != null)
            {
                if (gameObject.GetComponent<Card>().effect == 20 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 155, 2) > 0 && gameObject.GetComponent<Card>().cardType == 1)
                {
                    tooltipText = "<b>Rejuvenation Potion</b><br><color=#FFD370>Usable Item</color><br>Potion with rejuvenating properties. Use to regain 4<sprite=\"sprites v92\" index=3><sprite index=11>, removes 4 stacks of poison and immolation.";
                }
            }

            //special check for culinarist
            if (gameObject.GetComponent<Card>() != null)
            {
                if (gameObject.GetComponent<Card>().effect == 21 && CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 156, 2) > 0 && gameObject.GetComponent<Card>().cardType == 1)
                {
                    tooltipText = "<b>Nourishing Meal</b><br><color=#FFD370>Usable Item</color> <sprite index=32><br>Meal with high nutritional value. Use to regain 3<sprite=\"sprites v92\" index=3> 5<sprite index=11>.";
                }
            }

            GameManager.ins.toolTipText.text = tooltipText.ToString() + coolDownText.ToString();
        }
        if (roomController.GetComponent<PhotonRoom>().currentScene == 2)
        {
            SelectorScript2.ins.toolTipBackground.SetActive(true);
            SelectorScript2.ins.toolTipText.text = tooltipText.ToString() + coolDownText.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (roomController.GetComponent<PhotonRoom>().currentScene > 2)
        {
            GameManager.ins.toolTipText.text = "";
            GameManager.ins.toolTipBackground.SetActive(false);
        }
        if (roomController.GetComponent<PhotonRoom>().currentScene == 2)
        {
            SelectorScript2.ins.toolTipText.text = "";
            SelectorScript2.ins.toolTipBackground.SetActive(false);
        }
    }

    public void UpdateTooltip()
    {
        if (isCooldownPassive == true)
        {
            coolDownText = "\n<size=4> </size>\n <color=#00fffc>Cooldown:</color> " + cooldown + "<sprite index=35>.";
            //tooltipText = tooltipText + coolDownText;
        }
        /*
        else if (isCombatCooldownCard == true)
        {
            coolDownText = "\n<size=4> </size>\n <color=#00fffc>Cooldown:</color> " + cooldown + "<sprite index=35>.";
            //tooltipText = tooltipText + coolDownText;
        }

        else if (maxCooldown > 0)
        {
            coolDownText = "\n<size=4> </size>\n <color=#00fffc>Cooldown:</color> " + cooldown + "<sprite index=35>.";
            //tooltipText = tooltipText + coolDownText;
        }
        */

        //equipment
        //note that were using cooldown text for different purpose..
        if (gameObject.GetComponent<Card>().cardType == 5)
        {
            if ((CardHandler.ins.phaseNumber == 1 || CardHandler.ins.phaseNumber == 2) && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
            {
                //dont show these tooltips for special items
                if (gameObject.GetComponent<EquipmentCard>().equipmentType != 7)
                {
                    if (gameObject.GetComponentInParent<ScrollRectCenter>() != null)
                    {
                        coolDownText = "\n<size=4> </size>\n<color=#00fffc>Left click to equip.";
                    }

                    if (gameObject.GetComponentInParent<ScrollRectCenter>() == null)
                    {
                        coolDownText = "\n<size=4> </size>\n<color=#00fffc>Left click to un-equip.";
                    }
                }
                if (gameObject.GetComponent<EquipmentCard>().equipmentType == 7)
                {
                    coolDownText = "";
                }
            }
            else
            {
                coolDownText = "";
            }
        }

        //this should only be called for cards on store display
        //purchase phase
        if (CardHandler.ins.phaseNumber == 6 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //special case for unequippable equipment cards
            if (gameObject.GetComponent<EquipmentCard>() != null)
            {
                coolDownText = "\n<size=4> </size>\n<color=#00fffc>Costs: " + gameObject.GetComponent<Card>().value + "<sprite index=13>";
                /*do we need this?
                if (gameObject.GetComponent<EquipmentCard>().equipmentType != 7)
                {
                    coolDownText = "\n<size=4> </size>\n<color=#00fffc>Costs: " + gameObject.GetComponent<Card>().value + "<sprite index=13>";
                }
                */
            }
            else
            {
                coolDownText = "\n<size=4> </size>\n<color=#00fffc>Costs: " + gameObject.GetComponent<Card>().value + "<sprite index=13>";
            }
        }

        //this should only be called for cards on store display
        //sell phase
        if (CardHandler.ins.phaseNumber == 5 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            //get 25% of the original value?
            coolDownText = "\n<size=4> </size>\n<color=#00fffc>Sells for: " + gameObject.GetComponent<Card>().value / 4 + "<sprite index=13>";

            //special case for traders
            if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(108) == true)
            {
                float tradeBonus = 30 + (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence * 2f);
                float modifiedFloat = (float)gameObject.GetComponent<Card>().value * (tradeBonus / 100f);
                int modifiedValue = (int)modifiedFloat;

                coolDownText = "\n<size=4> </size>\n<color=#00fffc>Sells for: " + modifiedValue + "<sprite index=13>";
            }
        }
    }

    //advanced tooltip updates, since the previous didnt seem to work for all cases
    public void UpdateTooltip2(int function)
    {
        //lets do different kind of check for skill upgrades
        if (function == 1)
        {
            coolDownText = "\n<size=4> </size>\n<color=#00fffc>Skillpoint cost: " + gameObject.GetComponent<Card>().skillPointCost + "<sprite=\"sprites v88\" index=22>\n" +
                "Level requirement: " + gameObject.GetComponent<Card>().levelRequirement + "\n(Left click to acquire.)";
        }

        //special case for empty card
        if (gameObject.GetComponent<Card>().cardType == 14 && gameObject.GetComponent<Card>().numberInDeck == 260)
        {
            coolDownText = "\n<size=4> </size>\n<color=#00fffc>Left click to select another background.";
        }

        //function 2 unused?

        //lets use this for background display
        if (function == 3)
        {
            //special case for empty card
            if (gameObject.GetComponent<Card>().cardType == 14)
            {
                coolDownText = "\n<size=4> </size>\n<color=#00fffc>Left click to select this background.";
            }
        }

        //this is for background slots (not empty cards)
        if (function == 4 && gameObject.GetComponent<Card>().numberInDeck != 260)
        {
            //special case for empty card
            if (gameObject.GetComponent<Card>().cardType == 14)
            {
                coolDownText = "\n<size=4> </size>\n<color=#00fffc>Left click to empty this background.";
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //use this for background cards
        if (roomController.GetComponent<PhotonRoom>().currentScene == 2 && gameObject.GetComponent<Card>().cardType == 14)
        {
            if (gameObject.GetComponent<Card>().numberInDeck == 260)
            {
                if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlot1))
                {
                    SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlotChosen = 1;
                }

                if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlot2))
                {
                    SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlotChosen = 2;
                }

                if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlot3))
                {
                    SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlotChosen = 3;
                }

                SelectorScript2.ins.sfxPlayer.PlayButton1();
                SelectorScript2.ins.toolTipBackground.SetActive(false);

                SelectorScript2.ins.GetComponent<BackgroundHandler>().OpenBackgroundOptions();
            }

            else if (gameObject.GetComponent<Card>().numberInDeck != 260)
            {
                //drawing card from offer
                if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundCardArea))
                {
                    if (isEnabled == false)
                    {
                        SelectorScript2.ins.sfxPlayer.PlayUlriman();
                        return;
                    }
                    else
                    {
                        SelectorScript2.ins.sfxPlayer.PlayButton1();

                        SelectorScript2.ins.GetComponent<BackgroundHandler>().cardChosen = gameObject.GetComponent<Card>().numberInDeck;
                        SelectorScript2.ins.GetComponent<BackgroundHandler>().DrawBackGroundCard();
                        SelectorScript2.ins.toolTipBackground.SetActive(false);
                        return;
                    }
                }

                //clearing slot
                else if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlot1))
                {
                    SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlotChosen = 1;
                }

                else if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlot2))
                {
                    SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlotChosen = 2;
                }

                else if (ReferenceEquals(GetComponentInParent<GridLayoutGroup>().gameObject, SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlot3))
                {
                    SelectorScript2.ins.GetComponent<BackgroundHandler>().backgroundSlotChosen = 3;
                }

                SelectorScript2.ins.sfxPlayer.PlayButton1();
                SelectorScript2.ins.toolTipBackground.SetActive(false);

                SelectorScript2.ins.GetComponent<BackgroundHandler>().ResetBackgroundCard();
            }
        }

        //check that the scene is correct
        //could make turn check here also
        //consider if we need cardfunction check on these, similar to earlier carddisplay class
        if (roomController.GetComponent<PhotonRoom>().currentScene > 2  && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true)
        {
            //error sfx when card not enabled, and not on store phases
            if (!(CardHandler.ins.phaseNumber == 5 || CardHandler.ins.phaseNumber == 6) && isEnabled == false)
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();

                //allows purchasing more after delay
                StartCoroutine(CardHandler.ins.AllowEquipping(0.5f));
            }

            //added special case for petrify & frozen here (might be better to set the inabled tho.. but then we need to re-enable them)
            if(CardHandler.ins.phaseNumber == 3 && (realTimeCooldown > 0 ||
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isPetrified == true ||
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isFrozen == true))
            {
                CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                CardHandler.ins.intelligenceSfxHolder.Play();
                return;
            }

            //maybe make upgrade purchase check first?
            if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
            {
                if (ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.levelupCardArea) ||
                    ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.levelupCardArea2) ||
                    ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.levelupCardArea3))
                {
                    //make skillpoint & level check
                    if (CardHandler.ins.cardChangeInProgress == false &&
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().skillPoints >= gameObject.GetComponent<Card>().skillPointCost &&
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel >= gameObject.GetComponent<Card>().levelRequirement)
                    {
                        CardHandler.ins.cardChangeInProgress = true;

                        CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Influence.clip;
                        CardHandler.ins.intelligenceSfxHolder.Play();

                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(2, -gameObject.GetComponent<Card>().skillPointCost);

                        //dont draw new card for skillpoints or ability upgrades, instead add the effect on instant passive effects
                        if (gameObject.GetComponent<Card>().isSkillPoint == false && gameObject.GetComponent<Card>().isCardLevelUpgrade == false)
                        {
                            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, gameObject.GetComponent<Card>().cardType, 1);
                        }

                        //special case for card level upgrades
                        if(gameObject.GetComponent<Card>().isCardLevelUpgrade == true)
                        {
                            CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().CardLevelUpgrade(gameObject.GetComponent<Card>().numberInDeck);
                        }

                        CardHandler.ins.InstantPassiveEffect(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck);

                        //draw custom number of cards
                        //CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DrawUpgradeOffer2(1);
                        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();

                        //StartCoroutine(CardHandler.ins.AllowEquipping(0.5f));
                        //seems this need to be started elsewhere
                        CardHandler.ins.AllowEquippingAfterDelay();

                        CardHandler.ins.ResetPhaseAfterDelay();

                        GameManager.ins.toolTipBackground.SetActive(false);

                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                        CardHandler.ins.intelligenceSfxHolder.Play();
                    }
                }
            }

            //default usable cards
            //only allow clicking if card is enabled and its your turn
            if (isEnabled == true && gameObject.GetComponent<Card>().cardType != 5)
            {
                //play the audioclip of the effect chosen, if there is any (not when selling tho)
                //changed in v95
                if (sfx != null && CardHandler.ins.phaseNumber != 5 && playSfxWithDelay == false)
                {
                    CardHandler.ins.intelligenceSfxHolder.clip = sfx;
                    CardHandler.ins.intelligenceSfxHolder.Play();
                }

                //add delay for combat effects in v95
                //remove the sfx handling from end of the method)
                else if (sfx != null && (CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4) && playSfxWithDelay == true)
                {
                    //CardHandler.ins.intelligenceSfxHolder.clip = sfx;
                    //CardHandler.ins.intelligenceSfxHolder.Play();
                    CardHandler.ins.PlaySfxWithDelay(sfx);
                    //Invoke(nameof(PlaySfx), 2.0f);
                }

                //play selling sfx if phase is 5
                if (CardHandler.ins.phaseNumber == 5)
                {
                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().SonyaPurchase.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();
                }

                //rejuvenation potion
                if (gameObject.GetComponent<Card>().numberInDeck == 19)
                {
                    //sets the flag variable for sentinel (so he can drink also)
                    //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().PV.RPC("RPC_IsSelfMaintenance", RpcTarget.AllBufferedViaServer, GameManager.ins.turnNumber, true);

                    //special case for toxicology
                    if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 155, 2) > 0)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 4);
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 4);
                        //removes 4 stacks of poison
                        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 4);
                    }
                    else
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 3);
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
                    }

                    //removes 4 stacks of immolation
                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 173, 7, 4);

                    //disable cards, if on combat
                    if (CardHandler.ins.phaseNumber == 3)
                    {
                        //isEnabled = false;
                        //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 20);

                        //CardHandler.ins.RefreshAllCurrentCombatCardsWithDelay();

                        //need to exhaust quench fire too
                        //lets disable the card here too, since the disable method wont work on abilities that the hero dont technically own
                        //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 287);

                        //need special case for this card? (bit of a hack tho)
                        //point is this should be done before changing cooldowns?
                        if (ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.combatCardArea))
                        {
                            quantity -= 1;
                        }

                        //CardHandler.ins.DisableCombatCard(287, 1);
                        //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 19, 7, 1);
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroCardEffect = gameObject.GetComponent<Card>().effect;
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(3);
                    }

                    //allow movementBonus again, if on overmap
                    if (CardHandler.ins.phaseNumber == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
                    }
                }

                //nourishing meal
                if (gameObject.GetComponent<Card>().numberInDeck == 20)
                {
                    //special case for culinarist
                    if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 156, 2) > 0)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 3);
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 5);
                    }
                    else
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 2);
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 4);
                    }
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                }

                //healing (updated for v91)
                if (gameObject.GetComponent<Card>().numberInDeck == 24)
                {
                    if (gameObject.GetComponent<Card>().cardLevel == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 3);
                    }
                    if (gameObject.GetComponent<Card>().cardLevel == 2)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 4);
                        //removes 4 stacks of poison
                        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 4);
                    }
                    if (gameObject.GetComponent<Card>().cardLevel == 3)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, 5);
                        //removes 5 stacks of poison
                        CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 5);

                        //why would we do this?
                        //CardHandler.ins.RefreshAllCurrentCombatCardsWithDelay();
                    }
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -1);

                    //disable cards, if on combat
                    if (CardHandler.ins.phaseNumber == 3)
                    {
                        //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 25);
                        //isEnabled = false;
                        //CardHandler.ins.RefreshAllCurrentCombatCardsWithDelay();
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroCardEffect = gameObject.GetComponent<Card>().effect;
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
                    }

                    /*allow movementBonus again, if on overmap
                    if (CardHandler.ins.phaseNumber == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
                    }
                    */

                    //allow movementBonus again, if on overmap or location
                    //why do the other methods work without this?
                    if (CardHandler.ins.phaseNumber == 1 || CardHandler.ins.phaseNumber == 2)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                    }
                }

                //bless
                if (gameObject.GetComponent<Card>().numberInDeck == 29 && CardHandler.ins.phaseNumber != 3 && CardHandler.ins.phaseNumber != 4)
                {
                    //removes 1 stack of curse
                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 28, 7, 1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StatUpdate();

                    //allow movementBonus again, if on overmap
                    if (CardHandler.ins.phaseNumber == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
                    }
                }

                //blessed oil
                if (gameObject.GetComponent<Card>().numberInDeck == 114 && CardHandler.ins.phaseNumber != 3 && CardHandler.ins.phaseNumber != 4)
                {
                    //removes 1 stack of curse
                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 28, 7, 1);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StatUpdate();

                    //allow movementBonus again, if on overmap
                    if (CardHandler.ins.phaseNumber == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
                    }
                }

                //bombmaker
                if (gameObject.GetComponent<Card>().numberInDeck == 106)
                {
                    //craft bomb for coin cost
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -5);

                    //need cardlevel check here
                    if (gameObject.GetComponent<Card>().cardLevel == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, 1);
                    }
                    if (gameObject.GetComponent<Card>().cardLevel == 2)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, 2);
                    }

                    Invoke("PlayMechanicsSfx", 0.1f);
                    Invoke("PlayMechanicsSfx", 0.8f);

                    isEnabled = false;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                }

                //offering
                if (gameObject.GetComponent<Card>().numberInDeck == 107)
                {
                    //gain favor for coin cost
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -7);

                    //need cardlevel check here
                    if (gameObject.GetComponent<Card>().cardLevel == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 1);
                    }
                    if (gameObject.GetComponent<Card>().cardLevel == 2)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, 2);
                    }

                    //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 20, 1, 1);

                    isEnabled = false;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                }

                //clairvoyance (ability)
                if (gameObject.GetComponent<Card>().numberInDeck == 5)
                {
                    //costs 1 energy
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -1);

                    //show all encounters on curent minimap
                    GameManager.ins.references.currentMinimap.ShowAllEncounters();

                    isEnabled = false;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                }

                //distilling
                if (gameObject.GetComponent<Card>().numberInDeck == 166)
                {
                    //remove meal
                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 20, 1, 1);

                    //need cardlevel check here
                    if (gameObject.GetComponent<Card>().cardLevel == 1)
                    {
                        //add potion
                        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 19, 1, 1);
                    }

                    if (gameObject.GetComponent<Card>().cardLevel == 2)
                    {
                        //add potions
                        CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 19, 1, 2);
                    }

                    isEnabled = false;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                }

                //clairvoyance (potion)
                if (gameObject.GetComponent<Card>().numberInDeck == 167)
                {
                    //additional sfx
                    GameManager.ins.references.sfxPlayer.PlayDrinking();

                    //show all encounters on curent minimap
                    GameManager.ins.references.currentMinimap.ShowAllEncounters();

                    isEnabled = false;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                }

                //overcharge
                if (gameObject.GetComponent<Card>().numberInDeck == 180)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(3, -2);

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().isSelfMaintenance = true;
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 3);
                    
                    //disable card, if on combat
                    if (CardHandler.ins.phaseNumber == 3)
                    {
                        //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 107);
                        //isEnabled = false;
                        //CardHandler.ins.RefreshAllCurrentCombatCardsWithDelay();
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroCardEffect = gameObject.GetComponent<Card>().effect;
                        GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
                    }

                    //allow movementBonus again, if on overmap or location
                    //why do the other methods work without this?
                    if (CardHandler.ins.phaseNumber == 1 || CardHandler.ins.phaseNumber == 2)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().EndTurn();
                    }
                }

                //portal scroll
                if (gameObject.GetComponent<Card>().numberInDeck == 189)
                {
                    //set encounter phase to cards
                    CardHandler.ins.SetUsables(7);

                    //need this too?
                    GameManager.ins.references.currentEncounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().portalEncounter;

                    //swap encounteroption back to the original one
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().portalEncounter;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

                    //lets try use this for now
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetStrategicEncounter(true);
                }

                //portal scroll
                if (gameObject.GetComponent<Card>().numberInDeck == 286)
                {
                    //set encounter phase to cards
                    CardHandler.ins.SetUsables(7);

                    //lose energy
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -3);

                    //need this too?
                    GameManager.ins.references.currentEncounter = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().portalEncounter2;

                    //swap encounteroption back to the original one
                    GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterOption = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().portalEncounter2;

                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();

                    //lets try use this for now
                    GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().SetStrategicEncounter(true);
                }

                //remove card if its exhaustable
                //this should probably be last thing to call, cause card might get destroyed
                if (gameObject.GetComponent<Card>().isExhaustable)
                {
                    int holderType = 0;
                    if (gameObject.GetComponent<Card>().isUsable)
                    {
                        holderType = 1;
                    }
                    //this might be needed for selling purposes
                    if (gameObject.GetComponent<Card>().isPassive)
                    {
                        holderType = 2;
                    }

                    //reduce quantity in combatholder too
                    //this is alrdy done for rej. potion
                    //needs to be done before changing cooldowns, otherwise the card stays active at the combat holder
                    if (gameObject.GetComponent<Card>().numberInDeck != 19 && ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.combatCardArea))
                    {
                        quantity -= 1;
                    }

                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, holderType, 1);

                    //close tooltip in this case as well
                    GameManager.ins.toolTipBackground.SetActive(false);
                }

                //greater healing (unused)
                if (gameObject.GetComponent<Card>().numberInDeck == 145)
                {
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, 7);
                    GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -2);

                    //removes 5 stacks of poison
                    CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 23, 7, 5);

                    //disable cards, if on defense phase
                    if (CardHandler.ins.phaseNumber == 4)
                    {
                        CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 74);

                        isEnabled = false;
                    }

                    //allow movementBonus again, if on overmap
                    if (CardHandler.ins.phaseNumber == 1 && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().extraMovement == 1)
                    {
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove = true;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
                    }
                }
            }

            //equipment cards
            //can be only equipped/unequipped on overmap or when exploring
            if (gameObject.GetComponent<Card>().cardType == 5 && (CardHandler.ins.phaseNumber == 1 || CardHandler.ins.phaseNumber == 2) && CardHandler.ins.cardChangeInProgress == false)
            {
                //first turn the flag to true, then false again after interval
                //need to turn the flag false from another class it seems
                CardHandler.ins.cardChangeInProgress = true;
                //Invoke("EnableEquipping", 0.5f);

                //play the audioclip of the effect chosen, if there is any (not when selling tho)
                if (sfx != null && CardHandler.ins.phaseNumber != 5)
                {
                    CardHandler.ins.intelligenceSfxHolder.clip = sfx;
                    CardHandler.ins.intelligenceSfxHolder.Play();
                }

                //lets do a weird check to see if were on equipment canvas
                if (gameObject.GetComponentInParent<ScrollRectCenter>() != null)
                {
                    if (ReferenceEquals(gameObject.GetComponentInParent<ScrollRectCenter>().gameObject, GameManager.ins.equipmentCardArea))
                    {
                        //error sfx when trying to equip non-equippable items
                        if (gameObject.GetComponent<EquipmentCard>().equipmentType == 7)
                        {
                            CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                            CardHandler.ins.intelligenceSfxHolder.Play();

                            //allows purchasing more after delay
                            StartCoroutine(CardHandler.ins.AllowEquipping(0.5f));
                            return;
                        }

                        else
                        {
                            //this now also changes the cardChangeInProgress flag after delay
                            CardHandler.ins.EquipCards(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck);
                        }
                    }
                }
                //lets do a weird check to see if were not on equipment canvas
                if (gameObject.GetComponentInParent<ScrollRectCenter>() == null)
                {
                    //this now also changes the cardChangeInProgress flag after delay
                    CardHandler.ins.UnEquipCards(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck);
                }

                GameManager.ins.toolTipBackground.SetActive(false);

            }

            //buying
            //lets make weird check to see if were on store card area
            if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
            {
                if (CardHandler.ins.phaseNumber == 6 && ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, StoreHandler.ins.storeCardArea) && CardHandler.ins.cardChangeInProgress == false)
                {
                    CardHandler.ins.cardChangeInProgress = true;

                    //purchase item if you have the coins
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().coins >= gameObject.GetComponent<Card>().value)
                    {
                        CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().SonyaPurchase.clip;
                        CardHandler.ins.intelligenceSfxHolder.Play();

                        StoreHandler.ins.BuyCards(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, gameObject.GetComponent<Card>().cardType);

                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -gameObject.GetComponent<Card>().value);
                    }

                    //otherwise play different sfx
                    else
                    {
                        CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                        CardHandler.ins.intelligenceSfxHolder.Play();

                        //allows purchasing more after delay
                        StartCoroutine(CardHandler.ins.AllowEquipping(0.5f));
                    }

                    GameManager.ins.toolTipBackground.SetActive(false);
                }
            }

            //selling
            //lets make weird check to see if were on store card area
            if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
            {
                if (CardHandler.ins.phaseNumber == 5 && ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, StoreHandler.ins.storeCardArea) && CardHandler.ins.cardChangeInProgress == false)
                {
                    CardHandler.ins.cardChangeInProgress = true;

                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().SonyaPurchase.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();

                    //do the sell method
                    StoreHandler.ins.SellCards(GameManager.ins.turnNumber, gameObject.GetComponent<Card>().numberInDeck, gameObject.GetComponent<Card>().cardType);

                    //special case for traders
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(108) == true)
                    {
                        float tradeBonus = 30 + (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influence * 2f);
                        float modifiedFloat = (float)gameObject.GetComponent<Card>().value * (tradeBonus / 100f);
                        int modifiedValue = (int)modifiedFloat;

                        Debug.Log("modified float is: " + modifiedFloat);

                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, modifiedValue);
                    }
                    else
                    {
                        //give 25% of the value (rounded down
                        int reducedValue = gameObject.GetComponent<Card>().value / 4;
                        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, reducedValue);
                    }

                    GameManager.ins.toolTipBackground.SetActive(false);
                }
            }

            //attack & defense cards
            //lets make weird check to see if were on store card area
            if (gameObject.GetComponentInParent<ContentSizeFitter>() != null)
            {
                //might need to restart this here, so that healings also reset the timer
                if (CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4)
                {
                    //GameManager.ins.references.GetComponent<SliderController>().StartCombatTimer();
                }

                //special cases for rejuvenation pots & healings & blood rage, dunno if need?
                if (gameObject.GetComponent<Card>().numberInDeck == 19 || gameObject.GetComponent<Card>().numberInDeck == 24 || gameObject.GetComponent<Card>().numberInDeck == 180)
                {
                    return;
                }

                /* this can be done at start of method
                if ((CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4) && ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, StoreHandler.ins.storeCardArea) && isEnabled == false)
                {
                    CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Error.clip;
                    CardHandler.ins.intelligenceSfxHolder.Play();
                }
                */
                if ((CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4) && ReferenceEquals(GetComponentInParent<ContentSizeFitter>().gameObject, GameManager.ins.combatCardArea) && isEnabled == true)// && CardHandler.ins.cardChangeInProgress == false)
                {
                    //CardHandler.ins.cardChangeInProgress = true;

                    /* remove sfx call from here, since theyre alrdy handled at start of method
                     * 
                    if (sfx != null && playSfxWithDelay == false)
                    {
                        CardHandler.ins.intelligenceSfxHolder.clip = sfx;
                        CardHandler.ins.intelligenceSfxHolder.Play();
                    }
                    if (sfx != null && playSfxWithDelay == true)
                    {
                        //CardHandler.ins.intelligenceSfxHolder.clip = sfx;
                        //CardHandler.ins.intelligenceSfxHolder.Play();
                        CardHandler.ins.PlaySfxWithDelay(sfx);
                    }
                    */

                    //need to bring this to combat handler
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroCardEffect = gameObject.GetComponent<Card>().effect;

                    //do the cards method according to its effect
                    CombatEffect(gameObject.GetComponent<Card>().effect);

                    GameManager.ins.toolTipBackground.SetActive(false);
                }
            }

        }
    }

    void CombatEffect(int cardEffect)
    {
        //this should be done before resetting balls
        //aim
        if (cardEffect == 99)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost += 1;

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=12>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=10>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=13>";
            }

            tooltipText = "<b>Aim</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost +
                "<sprite index=11>.<br>Temporarily increase the minimum and maximum score of your next roll by 1.";

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
            {
                isEnabled = false;
            }
        }

        //blessed aim
        if (cardEffect == 296)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 1)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=17>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 2)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
            }

            tooltipText = "<b>Blessed Aim</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost +
           "<sprite index=12>.<br>Roll one additional die on your next attack roll.";

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
            {
                isEnabled = false;
            }
        }

        //shield focus
        if (cardEffect == 100)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost += 1;

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 1)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=12>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 2)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=10>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=13>";
            }

            tooltipText = "<b>Shield Focus</b><br><color=#FFD370>Active Ability</color> "+ GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost + 
                "<sprite index=11>.<br>Temporarily increase the minimum and maximum score of your next roll by 1.";

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().energy < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost)
            {
                isEnabled = false;
            }
        }

        //blessed shield
        if (cardEffect == 297)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost);
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 1)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=17>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 2)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
            }
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3)
            {
                quantityText.text = "<sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17><sprite=\"sprites v88\" index=17>";
            }

            tooltipText = "<b>Blessed Shield</b><br><color=#FFD370>Active Ability</color> " + GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost +
                "<sprite index=12>.<br>Roll one additional die on your next defense roll.";

            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost == 3 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
            {
                isEnabled = false;
            }
        }

        if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            GameManager.ins.references.targettingHandler.TakeBallScore();
        }

        //certain cards cant be used when paused
        //instead could unpause game tho?
        //changed to account for new combat mode for v93
        if(cannotBeUsedWhenPaused == true && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused == true 
            && GameManager.ins.references.targettingHandler.targettingEnabled == true)
        {
            CardHandler.ins.intelligenceSfxHolder.clip = GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().Button1.clip;
            CardHandler.ins.intelligenceSfxHolder.Play();

            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CombatPauseButton();
            return;
        }

        //need to end method here for these cards
        if (cardEffect == 99 || cardEffect == 100 || cardEffect == 296 || cardEffect == 297)
        {
            return;
        }

        //default attack
        if (cardEffect == 33)
        {
            GameManager.ins.references.GetComponent<CombatActions>().DefaultAttackButton(gameObject);

            //actually lets do these in the default attack button?
            //GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().AttackThreeTimesHalfSecInterval(28);
        }

        //arcane attack
        if (cardEffect == 34)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3) == true && CardHandler.ins.StaffCheck() == true)
            {
                //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);
            }
            else
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);
            }
            GameManager.ins.references.GetComponent<CombatActions>().ArcaneAttackButton(gameObject);
        }

        //defend
        if (cardEffect == 35)
        {
            GameManager.ins.references.GetComponent<CombatActions>().DefaultDefendButton();
        }

        //flee
        if (cardEffect == 36)
        {
            GameManager.ins.references.GetComponent<CombatActions>().FleeButton();
        }

        //power attack
        if (cardEffect == 37)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, 2);

            if(cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 37);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().powerAttackActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().DefaultAttackButton(gameObject);
        }

        //precise strike
        if (cardEffect == 38)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 38);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().preciseStrikeActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().DefaultAttackButton(gameObject);
        }

        //extra strike
        if (cardEffect == 75)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 148, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 75);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().extraStrikeActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().DefaultAttackButton(gameObject);
        }

        //throw bomb
        if (cardEffect == 39)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, -1);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 39);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ThrowBombButton(gameObject);
        }

        //throw manabomb
        if (cardEffect == 41)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, -1);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 41);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ThrowBombButton(gameObject);
        }

        //arcane barrage
        if (cardEffect == 43)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -1);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 43);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ArcaneAttackButton(gameObject);
        }

        //arcane orb
        if (cardEffect == 44)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -2);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 44);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ArcaneAttackButton(gameObject);
        }

        //bolt of light
        if (cardEffect == 45)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -1);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 45);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().boltOfLightActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ArcaneAttackButton(gameObject);
        }

        //spear of light
        if (cardEffect == 46)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -2);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 46);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().spearOfLightActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ArcaneAttackButton(gameObject);
        }

        //dodge
        if (cardEffect == 49)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //these defense cards are handled differently on the draw cards method
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 108, 7, 1);
            //GameObject playerCard = Instantiate(cardToCheck, new Vector3(0, 0, 0), Quaternion.identity);
            //playerCard.transform.SetParent(GameManager.ins.effectCardArea.transform, false);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 49);
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().dodgeActivated = true;
            //GameManager.ins.references.GetComponent<CombatActions>().DefaultDefendButton();
        }

        //block
        if (cardEffect == 50)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //these defense cards are handled differently on the draw cards method
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 109, 7, 1);
            //GameObject playerCard = Instantiate(cardToCheck, new Vector3(0, 0, 0), Quaternion.identity);
            //playerCard.transform.SetParent(GameManager.ins.effectCardArea.transform, false);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
        }

        //ward
        if (cardEffect == 51)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //these defense cards are handled differently on the draw cards method
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 110, 7, 1);
            //GameObject playerCard = Instantiate(cardToCheck, new Vector3(0, 0, 0), Quaternion.identity);
            //playerCard.transform.SetParent(GameManager.ins.effectCardArea.transform, false);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
        }

        //shield of isolore
        if (cardEffect == 52)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, 2);

            if (cardToCheck.GetComponent<Card>().requiresFavor > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -cardToCheck.GetComponent<Card>().requiresFavor);
            }
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 111, 7, 1);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
        }

        //wraithform
        if (cardEffect == 53)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 112, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 53);

            //disable avatar of fire
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 158);
            //CardHandler.ins.DisableCombatCard(237, 2);

            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 112, 7, 1);
            //give 20% def modifier, if first stack
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 112, 7) == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier += 20;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wraithsGiftActivated = true;

            //isEnabled = false;
            return;
        }

        //avatar of fire
        if (cardEffect == 158)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 237, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 158);

            //disable wraithform button
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 53);
            //CardHandler.ins.DisableCombatCard(112, 2);

            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 237, 7, 1);

            //give 20% def modifier
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 237, 7) == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier += 20;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().avatarOfFireActivated = true;

            //isEnabled = false;
            return;
        }

        //blessed blade
        if (cardEffect == 54)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 113, 2);

            if (cardToCheck.GetComponent<Card>().requiresFavor > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -cardToCheck.GetComponent<Card>().requiresFavor);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 54);
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated = true;
            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 113, 7, 1);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
            //isEnabled = false;
            return;
            //GameManager.ins.references.GetComponent<CombatActions>().DefaultDefendButton();
        }

        //blessed oil
        if (cardEffect == 55)
        {
            /*attack phase
            if (CardHandler.ins.phaseNumber == 3)
            {
                CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 55);
                //draw 3 "cards" into the effect holder
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 114, 7, 3);
                //reduce 1 potion
                //CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 114, 1, 1);

                isEnabled = false;
            }
            */
            //new attack phase
            if (CardHandler.ins.phaseNumber == 3)
            {
                //removes 1 stack of curse
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 28, 7, 1);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StatUpdate();

                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(3);

                //isEnabled = false;
            }
            return;
        }

        //bless ability
        if (cardEffect == 30)
        {
            //defense phase
            if (CardHandler.ins.phaseNumber == 3)
            {
                //removes 1 stack of curse
                CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 28, 7, 1);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6,-1);
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().StatUpdate();

                GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
                //isEnabled = false;
            }
            return;
        }

        //time warp
        if (cardEffect == 76)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 151, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 76);
            //this is the only "defense" ability which needs flag?
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated = true;

            //draw 1 "card" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 151, 7, 1);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
            //isEnabled = false;
            return;
        }

        //throw smokebomb
        if (cardEffect == 77)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, -1);
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 77);
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().timeWarpActivated = true;

            //draw 3 "card" into the effect holder
            //draw different card, depending on level
            if (gameObject.GetComponent<Card>().cardLevel == 1)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 152, 7, 1);
            }
            if (gameObject.GetComponent<Card>().cardLevel == 2)
            {
                CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 232, 7, 1);
            }

            /*reduce 10% def & res modifiers
             * actually lets not
            if (gameObject.GetComponent<Card>().cardLevel == 2)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier += 10;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier += 10;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }
            */
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().smokeBombActivated = true;

            //isEnabled = false;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
            return;
        }

        //earth magic
        if (cardEffect == 78)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 153, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 78);
            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().wardActivated = true;
            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 153, 7, 1);
            //isEnabled = false;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);
            return;
        }

        //detonation
        if (cardEffect == 80)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, -3);

            //actually lets exhaus the detonation master in this case
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 79);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ThrowBombButton(gameObject);
        }

        //arcane detonation
        if (cardEffect == 81)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(16, -3);

            //actually lets exhaust the detonation master in this case
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 79);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated = true;
            GameManager.ins.references.GetComponent<CombatActions>().ThrowBombButton(gameObject);
        }

        //potion of invulnerability
        if (cardEffect == 95)
        {
            Debug.Log("activates invulnerability potion");

            //dont rly need to exhaust card, ince the enabled check is done differently
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 95);

            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 168, 7, 1);
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(3);

            //give 40% def & res modifiers
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 168, 7) == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier += 40;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier += 40;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().invulnerabilityActivated = true;
            //isEnabled = false;
            return;
        }

        //quick reload
        if (cardEffect == 124)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 199, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //refresh bomb cards
            CardHandler.ins.RefreshBombs();
            //CardHandler.ins.RefreshCombatCard(102, 2, 5f);

            Invoke(nameof(PlayMechanicsSfx), 0.1f);
            Invoke(nameof(PlayMechanicsSfx), 0.8f);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            //isEnabled = false;
            return;
        }

        //arcane recharge in attack phase
        if (cardEffect == 154 && CardHandler.ins.phaseNumber == 3)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 233, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //refresh arcane cards
            CardHandler.ins.RefreshCombatCard(102, 2, 5f);
            CardHandler.ins.RefreshCombatCard(103, 2, 5f);
            CardHandler.ins.RefreshCombatCard(110, 2, 5f);

            //CardHandler.ins.RefreshAllCurrentCombatCardsWithDelay();
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[233].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();

            //isEnabled = false;
            return;
        }

        //invigorate in attack phase
        if (cardEffect == 155 && CardHandler.ins.phaseNumber == 3)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 234, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }

            //refresh warrior attack cards
            CardHandler.ins.RefreshCombatCard(98, 2, 5f);
            CardHandler.ins.RefreshCombatCard(99, 2, 5f);

            CardHandler.ins.RefreshCombatCard(108, 2, 5f);
            CardHandler.ins.RefreshCombatCard(109, 2, 5f);

            //CardHandler.ins.RefreshAllCurrentCombatCardsWithDelay();
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[234].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();

            //isEnabled = false;
            return;
        }


        //berserk
        if (cardEffect == 156)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 235, 2);

            if (cardToCheck.GetComponent<Card>().requiresEnergy > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(0, -cardToCheck.GetComponent<Card>().requiresEnergy);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 156);

            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 235, 7, 1);

            //reduce 20% def & res modifiers
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 235, 7) == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseModifier -= 20;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceModifier -= 20;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().berserkActivated = true;
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            CardHandler.ins.extraSfxHolder.clip = CardHandler.ins.generalDeck[235].GetComponent<CardDisplay2>().sfx;
            CardHandler.ins.extraSfxHolder.Play();

            //isEnabled = false;
            return;
        }

        //wrath of guliman
        if (cardEffect == 157)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 236, 2);

            if (cardToCheck.GetComponent<Card>().requiresFavor > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -cardToCheck.GetComponent<Card>().requiresFavor);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 157);

            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 236, 7, 1);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            //isEnabled = false;
            return;
        }

        //concussion bomb infusion
        if (cardEffect == 159)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 238, 2);

            if (cardToCheck.GetComponent<Card>().requiresCoins > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -cardToCheck.GetComponent<Card>().requiresCoins);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 159);

            //exhaust firebomb also
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 160);
            //CardHandler.ins.DisableCombatCard(239, 2);

            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 238, 7, 1);
            //need this to disable the dependent card in another way
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            //isEnabled = false;
            return;
        }

        //firebomb infusion
        if (cardEffect == 160)
        {
            //possible energy cost
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 239, 2);

            if (cardToCheck.GetComponent<Card>().requiresCoins > 0)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(4, -cardToCheck.GetComponent<Card>().requiresCoins);
            }
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 160);

            //exhaust concussion bomb also
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 159);
            //CardHandler.ins.DisableCombatCard(238, 2);

            //draw 1 "cards" into the effect holder
            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 239, 7, 1);
            //need this to disable the dependent card in another way
            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            //isEnabled = false;
            return;
        }

        //quench fire
        if (cardEffect == 287)
        {
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 287);

            //exhaust rejuvenation potion also
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 20);
            //CardHandler.ins.DisableCombatCard(19, 1);

            //remove potion
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 19, 1, 1);
            //reduce immolation
            CardHandler.ins.ReduceQuantity(GameManager.ins.turnNumber, 173, 7, 6);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(3);

            //isEnabled = false;
            return;
        }

        //potion of power
        if (cardEffect == 298)
        {
            //dont rly need to exhaust card, ince the enabled check is done differently
            //CardHandler.ins.ExhaustHeroCombatCard(GameManager.ins.turnNumber, 95);

            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 298, 7, 1);

            //give 40% all attack modifiers
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 298, 7) == 1)
            {
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthModifier += 40;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerModifier += 40;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().bombModifier += 40;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().holyModifier += 40;
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateStatTexts();
            }

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(3);

            //GameManager.ins.exploreHandler.GetComponent<CombatHandler>().potionOfPowerActivated = true;
            //isEnabled = false;
            return;
        }

        //lightstone blessing
        if (cardEffect == 301)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().UpdateResources(6, -gameObject.GetComponent<Card>().requiresFavor);
            //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost += 1;

            CardHandler.ins.DrawCards(GameManager.ins.turnNumber, 301, 7, 1);

            GameManager.ins.exploreHandler.GetComponent<CombatHandler>().ChangeRealTimeHeroCooldowns(0);

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor < GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().rerollCost)
            {
                isEnabled = false;
            }
        }

        //since this is also used for trading backtrack, lets add these here
        //trading holder shouldnt be deactivated in v0.5.7.
        //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingHolder.SetActive(false);
        GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().tradingBackButton.SetActive(false);

        //should destroy the trading canvas objects
        //GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().RemoveTradingCards();

        //actually we should change the cooldowns instead in v0.5.7.

        //set encounter phase to cards
        //CardHandler.ins.SetUsables(7);
    }

    //this only get called every second time for some reason
    //need to do this elsewhere
    void EnableEquipping()
    {
        Debug.Log("enables equipping");
        CardHandler.ins.cardChangeInProgress = false;
    }

    void PlayMechanicsSfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayMechanics();
    }

    /*actually this wont work, since the card gets deleted
    void PlaySfx()
    {
        //play sfx
        CardHandler.ins.intelligenceSfxHolder.clip = sfx;
        CardHandler.ins.intelligenceSfxHolder.Play();
    }
    */
}
