using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargettingHandler : MonoBehaviour
{
    public GameObject targettingDisplay;
    public GameObject targettingBorders;

    public GameObject crosshair;
    public GameObject target;

    public GameObject crosshair2;
    public GameObject target2;

    public Vector3 crosshairStartPosition;
    public Vector3 targetStartPosition;

    public Vector2 targetStartSize;

    //migh as well put this as false at start in v93?
    public bool targettingEnabled = false;

    //stores score of the targetting result (1-6)
    public int score;

    public string timingText;

    public SpriteRenderer targettingBackground;
    public GameObject humanoidShadow;

    public float timer;
    public float blinkSpeed;

    //for handling special icons
    public Sprite defaultFoeAttackIcon;
    public Sprite defaultFoeArcanePowerIcon;
    public Sprite foeCurrentSpecialAbilityIcon;

    public Sprite defaultHeroDefendIcon;
    public Sprite defaultHeroResistIcon;
    public Sprite heroCurrentDefendTypeIcon;


    // Start is called before the first frame update
    void Start()
    {
        crosshairStartPosition = crosshair.transform.position;
        targetStartPosition = target.transform.position;

        targetStartSize = target.transform.localScale;

        targettingEnabled = false;
    }


    private void Update()
    {
        timer += Time.deltaTime * blinkSpeed;

        //lets try this for target decoy recolor 
        if (timer > 100 && target2.activeSelf)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe < 2)
            {
                target2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 2 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 3)
            {
                target2.GetComponent<Image>().color = new Color(0.8f, 0.8f, 1, 1);
                Invoke("ReEnableDecoyColors", 0.25f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 4 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 5)
            {
                target2.GetComponent<Image>().color = new Color(0.7f, 0.7f, 1, 1);
                Invoke("ReEnableDecoyColors", 0.45f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 6)
            {
                target2.GetComponent<Image>().color = new Color(0.6f, 0.6f, 1, 1);
            }

            timer = 0;
        }

        //lets try this for crosshair decoy recolor 
        if (timer > 100 && crosshair2.activeSelf)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe < 2)
            {
                crosshair2.GetComponent<BallMove>().heroImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 2 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 3)
            {
                crosshair2.GetComponent<BallMove>().heroImage.GetComponent<Image>().color = new Color(0.55f, 1, 1, 1);
                Invoke("ReEnableDecoyColors", 0.25f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 4 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 5)
            {
                crosshair2.GetComponent<BallMove>().heroImage.GetComponent<Image>().color = new Color(0.4f, 1, 1, 1);
                Invoke("ReEnableDecoyColors", 0.45f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 6)
            {
                crosshair2.GetComponent<BallMove>().heroImage.GetComponent<Image>().color = new Color(0.3f, 1, 1, 1);
            }

            timer = 0;
        }

        //lets try this for stealth transparency 
        if (timer > 90 && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(26) && CardHandler.ins.phaseNumber == 3)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe < 2)
            {
                target.GetComponent<Image>().color = new Color(1, 1, 1, 0f);
                Invoke("ReEnableMainTargetAlpha", 0.75f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 2 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 3)
            {
                target.GetComponent<Image>().color = new Color(1, 1, 1, 0f);
                Invoke("ReEnableMainTargetAlpha", 0.55f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 4 || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 5)
            {
                target.GetComponent<Image>().color = new Color(1, 1, 1, 0f);
                Invoke("ReEnableMainTargetAlpha", 0.35f);
            }
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observe == 6)
            {
                target.GetComponent<Image>().color = new Color(0.6f, 0.6f, 1, 1);
            }

            timer = 0;
        }
    }

    void ReEnableDecoyColors()
    {
        target2.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        crosshair2.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        crosshair2.GetComponent<BallMove>().heroImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    void ReEnableMainTargetAlpha()
    {
        target.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    //this takes the score of balls, also resets the ball locations and trajectories
    public void TakeBallScore()
    {
        StoreTargettingResult();

        /*
        crosshair.GetComponent<BallMove>().ResetIgnoreCollision();
        target.GetComponent<BallMove>().ResetIgnoreCollision();

        crosshair.GetComponent<BallMove>().Halt();
        target.GetComponent<BallMove>().Halt();
        */

        ResetBalls();

    }

    //could use this separately in some cases
    public void ResetBalls()
    {
        crosshair.GetComponent<BallMove>().ResetIgnoreCollision();
        target.GetComponent<BallMove>().ResetIgnoreCollision();

        crosshair.GetComponent<BallMove>().Halt();
        target.GetComponent<BallMove>().Halt();

        //should do these too
        crosshair2.GetComponent<BallMove>().ResetIgnoreCollision();
        target2.GetComponent<BallMove>().ResetIgnoreCollision();

        crosshair2.GetComponent<BallMove>().Halt();
        target2.GetComponent<BallMove>().Halt();

        ChangeTargetSettings();

        RestartAfterDelay();
    }

    public void ChangeTargetSettings()
    {
        //default crosshair values
        //may be changed on certain conditions
        crosshair.GetComponent<BallMove>().speed = 12000f;
        crosshair2.GetComponent<BallMove>().speed = 12000f;

        //targetting difficulty calculations (ranges from 0-4, 0 being the hardest to hit)
        //lets use focus cost as base
        int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;

        if (CardHandler.ins.phaseNumber == 3)
        {
            //small foe
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(54))
            {
                targettingBonus -= 1;
            }
            //large foe
            else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(55))
            {
                targettingBonus += 1;
            }
        }

        //attack phase
        if (CardHandler.ins.phaseNumber == 3)
        {
            crosshair2.SetActive(false);

            crosshair.GetComponent<Image>().sprite = crosshair.GetComponent<BallMove>().attackPhaseIcon;

            //used for setting hero faces in defense phase
            //SetHeroImage(false);

            //change foe icon
            if (GameManager.ins.references.currentEncounter.overrideCombatIcon == null)
            {
                target.GetComponent<Image>().sprite = GameManager.ins.references.currentStrategicEncounter.identifiedImage.GetComponent<Image>().sprite;
            }
            else
            {
                target.GetComponent<Image>().sprite = GameManager.ins.references.currentEncounter.overrideCombatIcon;
            }

            //check mirror image here
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(43) == false)
            {
                target2.SetActive(false);
            }
            else
            {
                target2.SetActive(true);

                if (GameManager.ins.references.currentEncounter.overrideCombatIcon == null)
                {
                    target2.GetComponent<Image>().sprite = GameManager.ins.references.currentStrategicEncounter.identifiedImage.GetComponent<Image>().sprite;
                }
                else
                {
                    target2.GetComponent<Image>().sprite = GameManager.ins.references.currentEncounter.overrideCombatIcon;
                }
            }

            //set speed
            //swift foe
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(2))
            {
                target.GetComponent<BallMove>().speed = 8000f;
                target2.GetComponent<BallMove>().speed = 8000f;
            }
            //slow foe
            else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(35))
            {
                target.GetComponent<BallMove>().speed = 4000f;
                target2.GetComponent<BallMove>().speed = 4000f;
            }
            //normal speed
            else
            {
                target.GetComponent<BallMove>().speed = 6000f;
                target2.GetComponent<BallMove>().speed = 6000f;
            }

        }

        //defense phase
        if (CardHandler.ins.phaseNumber == 4)
        {
            target2.SetActive(false);

            //smaller shield for ensnared hero
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 171, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 176, 7) > 0 ||
            CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 182, 7) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 185, 7) > 0)
            {
                targettingBonus -= 1;

                //crosshair.GetComponent<BallMove>().speed = 2f;
                //crosshair2.GetComponent<BallMove>().speed = 2f;
            }

            //used for setting hero faces in defense phase
            //SetHeroImage(true);

            //target2.GetComponent<SpriteRenderer>().sprite = target2.GetComponent<BallMove>().defensePhaseIcon;

            //only change icons when special attack not activated
            if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().isSpecialFoeAttackPhase == false)
            {

                //actually these two no longer needed?
                //target.GetComponent<SpriteRenderer>().sprite = target.GetComponent<BallMove>().defensePhaseIcon;
                //crosshair.GetComponent<SpriteRenderer>().sprite = crosshair.GetComponent<BallMove>().defensePhaseIcon;
                //crosshair2.GetComponent<SpriteRenderer>().sprite = crosshair2.GetComponent<BallMove>().defensePhaseIcon;

                //set custom crosshair icon here
                if (GameManager.ins.references.currentStrategicEncounter.attack != 0)
                {
                    crosshair.GetComponent<Image>().sprite = defaultFoeAttackIcon;
                    crosshair2.GetComponent<Image>().sprite = defaultFoeAttackIcon;
                    target.GetComponent<Image>().sprite = defaultHeroDefendIcon;
                }
                if (GameManager.ins.references.currentStrategicEncounter.arcanePower != 0)
                {
                    crosshair.GetComponent<Image>().sprite = defaultFoeArcanePowerIcon;
                    crosshair2.GetComponent<Image>().sprite = defaultFoeArcanePowerIcon;
                    target.GetComponent<Image>().sprite = defaultHeroResistIcon;
                }
            }

            //check phantom attacks here
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(45) == false)
            {
                crosshair2.SetActive(false);
            }
            else
            {
                crosshair2.SetActive(true);
            }

            //set speed
            target.GetComponent<BallMove>().speed = 6000f;
            target2.GetComponent<BallMove>().speed = 6000f;
        }

        //might not need phasenumber check now?
        //hardest difficulty
        if (targettingBonus == 0)
        {
            //target.GetComponent<BallMove>().speed = 8000f;

            Vector2 scaleUp = new Vector2(targetStartSize.x - 0.4f, targetStartSize.y - 0.4f);

            target.GetComponent<Transform>().localScale = scaleUp;

            if (target2.activeSelf)
            {
                //target2.GetComponent<BallMove>().speed = 8000f;
                target2.GetComponent<Transform>().localScale = scaleUp;
            }
        }
        //default values
        else if (targettingBonus == 1)// && (CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4))
        {
            //target.GetComponent<BallMove>().speed = 6000f;
            target.GetComponent<Transform>().localScale = targetStartSize;

            if (target2.activeSelf)
            {
                //target2.GetComponent<BallMove>().speed = 6000f;
                target2.GetComponent<Transform>().localScale = targetStartSize;
            }

            //reset these also
            target.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            target2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            crosshair2.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        //improved chance
        else if (targettingBonus == 2)
        {
            //target.GetComponent<BallMove>().speed = 4000f;

            Vector2 scaleUp = new Vector2(targetStartSize.x + 0.4f, targetStartSize.y + 0.4f);

            target.GetComponent<Transform>().localScale = scaleUp;

            if (target2.activeSelf)
            {
                //target2.GetComponent<BallMove>().speed = 4000f;
                target2.GetComponent<Transform>().localScale = scaleUp;
            }
        }
        //improved chance
        else if (targettingBonus == 3)
        {
            //target.GetComponent<BallMove>().speed = 3000f;

            Vector2 scaleUp = new Vector2(targetStartSize.x + 0.8f, targetStartSize.y + 0.8f);

            target.GetComponent<Transform>().localScale = scaleUp;

            if (target2.activeSelf)
            {
                //target2.GetComponent<BallMove>().speed = 3000f;
                target2.GetComponent<Transform>().localScale = scaleUp;
            }
        }
        //improved chance ((this should only happen in attack phase?)
        else if (targettingBonus == 4)
        {
            //target.GetComponent<BallMove>().speed = 2000f;

            Vector2 scaleUp = new Vector2(targetStartSize.x + 0.8f, targetStartSize.y + 0.8f);

            target.GetComponent<Transform>().localScale = scaleUp;

            if (target2.activeSelf)
            {
                //target2.GetComponent<BallMove>().speed = 2000f;
                target2.GetComponent<Transform>().localScale = scaleUp;
            }

            if (CardHandler.ins.phaseNumber == 3)
            {
                //set speed
                //swift foe
                if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(2))
                {
                    target.GetComponent<BallMove>().speed = 6000f;
                    target2.GetComponent<BallMove>().speed = 6000f;
                }
                //slow foe
                else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(35))
                {
                    target.GetComponent<BallMove>().speed = 2000f;
                    target2.GetComponent<BallMove>().speed = 2000f;
                }
                //normal speed
                else
                {
                    target.GetComponent<BallMove>().speed = 4000f;
                    target2.GetComponent<BallMove>().speed = 4000f;
                }
            }
        }

        //lets override some of the changes here for v92
        //might want to remove that whole defense mode crosshair change, if we keep this
        if (CardHandler.ins.phaseNumber == 3)
        {
            SetHeroImage(false);
        }
        else if (CardHandler.ins.phaseNumber == 4)
        {
            SetHeroImage(true);
        }
    }

    public void SetSpecialFoeAttackPhaseIcons()
    {
        crosshair.GetComponent<Image>().sprite = foeCurrentSpecialAbilityIcon;
        crosshair2.GetComponent<Image>().sprite = foeCurrentSpecialAbilityIcon;
        target.GetComponent<Image>().sprite = heroCurrentDefendTypeIcon;
    }


    void StoreTargettingResult()
    {
        //targetting difficulty calculations (ranges from 0-4, 0 being the hardest to hit)
        //lets use focus cost as base
        int targettingBonus = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().focusCost;

        if (CardHandler.ins.phaseNumber == 3)
        {
            //small foe
            if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(54))
            {
                targettingBonus -= 1;
            }
            //large foe
            else if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().CheckFoeAbility(55))
            {
                targettingBonus += 1;
            }
        }

        //take distance before reset
        float distance = Vector3.Distance(crosshair.transform.position, target.transform.position);
        //Debug.Log("distance between targetters is: " + distance);

        if (targettingBonus == 0)
        {
            if (distance > 0.6f)
            {
                //special cases
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
                {
                    score = Random.Range(1, 4);
                }
                else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
                {
                    score = Random.Range(1, 3);
                }
                else
                {
                    score = 1;
                }
                timingText = "poor timing";
            }
            if (distance <= 0.6f && distance > 0.45f)
            {
                //Debug.Log("Hit edge of target!");
                score = Random.Range(2, 5);
                timingText = "decent timing";
            }
            if (distance <= 0.45f && distance > 0.3f)
            {
                //Debug.Log("Hit mid-ring of target!");
                score = Random.Range(3, 6);
                timingText = "good timing";
            }
            if (distance <= 0.3f)
            {
                //Debug.Log("Hit bullseye!");
                score = Random.Range(4, 7);
                timingText = "excellent timing";
            }
        }

        //default values
        else if (targettingBonus == 1)
        {
            if (distance > 0.8f)
            {
                //special cases
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
                {
                    score = Random.Range(1, 4);
                }
                else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
                {
                    score = Random.Range(1, 3);
                }
                else
                {
                    score = 1;
                }
                timingText = "poor timing";
            }
            if (distance <= 0.8f && distance > 0.6f)
            {
                //Debug.Log("Hit edge of target!");
                score = Random.Range(2, 5);
                timingText = "decent timing";
            }
            if (distance <= 0.6f && distance > 0.4f)
            {
                //Debug.Log("Hit mid-ring of target!");
                score = Random.Range(3, 6);
                timingText = "good timing";
            }
            if (distance <= 0.4f)
            {
                //Debug.Log("Hit bullseye!");
                score = Random.Range(4, 7);
                timingText = "excellent timing";
            }
        }

        //easier targetting
        else if (targettingBonus == 2)
        {
            if (distance > 1f)
            {
                //special cases
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
                {
                    score = Random.Range(1, 4);
                }
                else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
                {
                    score = Random.Range(1, 3);
                }
                else
                {
                    score = 1;
                }
                timingText = "poor timing";
            }
            if (distance <= 1f && distance > 0.75f)
            {
                //Debug.Log("Hit edge of target!");
                score = Random.Range(2, 5);
                timingText = "decent timing";
            }
            if (distance <= 0.75f && distance > 0.5f)
            {
                //Debug.Log("Hit mid-ring of target!");
                score = Random.Range(3, 6);
                timingText = "good timing";
            }
            if (distance <= 0.5f)
            {
                //Debug.Log("Hit bullseye!");
                score = Random.Range(4, 7);
                timingText = "excellent timing";
            }
        }

        //easier targetting
        else if (targettingBonus == 3 || targettingBonus == 4)
        {
            if (distance > 1.2f)
            {
                //special cases
                if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneBarrageActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().detonationActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneDetonationActivated == true)
                {
                    score = Random.Range(1, 4);
                }
                else if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().throwManaBombsActivated == true ||
                    GameManager.ins.exploreHandler.GetComponent<CombatHandler>().arcaneOrbActivated == true)
                {
                    score = Random.Range(1, 3);
                }
                else
                {
                    score = 1;
                }
                timingText = "poor timing";
            }
            if (distance <= 1.20f && distance > 0.9f)
            {
                //Debug.Log("Hit edge of target!");
                score = Random.Range(2, 5);
                timingText = "decent timing";
            }
            if (distance <= 0.9f && distance > 0.6f)
            {
                //Debug.Log("Hit mid-ring of target!");
                score = Random.Range(3, 6);
                timingText = "good timing";
            }
            if (distance <= 0.6f)
            {
                //Debug.Log("Hit bullseye!");
                score = Random.Range(4, 7);
                timingText = "excellent timing";
            }
        }
    }

    public void RestartAfterDelay()
    {
        Invoke("RestartAfterDelayContinued", 0.5f);
    }

    public void RestartAfterDelayContinued()
    {
        /*lets try this, because sometimes this method can be called multiple times at the same time atm
        if (crosshair.GetComponent<BallMove>().rb.velocity == Vector2.zero)
        {
            crosshair.transform.position = crosshairStartPosition;
            crosshair.GetComponent<BallMove>().Restart();
        }

        if (target.GetComponent<BallMove>().rb.velocity == Vector2.zero)
        {
            target.transform.position = targetStartPosition;
            target.GetComponent<BallMove>().Restart();

        }
        */

        if (GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused == false)
        {
            Debug.Log("ball posions restart");

            //this is done in the the restart method itself now
            //crosshair.transform.position = crosshairStartPosition;
            crosshair.GetComponent<BallMove>().Restart();

            //target.transform.position = targetStartPosition;
            target.GetComponent<BallMove>().Restart();

            crosshair2.GetComponent<BallMove>().Restart();
            target2.GetComponent<BallMove>().Restart();
        }
    }

    //this now also handles the shadow
    public void TakeTargettingBackground()
    {
        if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == true && GameManager.ins.references.currentMinimap != null)
        {
            //could use the image set in location, if theres no image specified for the minimap
            if(GameManager.ins.references.currentMinimap.targetBackgroundImage != null)
            {
                if (Clock.clock.isNight == false || GameManager.ins.references.currentMinimap.targetBackgroundImageNight == null)
                {
                    targettingBackground.sprite = GameManager.ins.references.currentMinimap.targetBackgroundImage;
                }
                else
                {
                    targettingBackground.sprite = GameManager.ins.references.currentMinimap.targetBackgroundImageNight;
                }
            }
            else if (GameManager.ins.references.currentMinimap.targetBackgroundImage == null)
            {
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImage != null)
                {
                    if (Clock.clock.isNight == false || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImageNight == null)
                    {
                        targettingBackground.sprite = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImage;
                    }
                    else
                    {
                        targettingBackground.sprite = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImageNight;
                    }
                }
            }
        }

        //lets make separate check for overmap backgrounds
        else if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode != null && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == false)
        {
            if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImage != null)
            {
                if (Clock.clock.isNight == false || GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImageNight == null)
                {
                    targettingBackground.sprite = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImage;
                }
                else
                {
                    targettingBackground.sprite = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.targetBackgroundImageNight;
                }
            }
        }
    }

    //just for menu1 testing purposes
    public void TestResetBalls()
    {
        crosshair.GetComponent<BallMove>().ResetIgnoreCollision();
        target.GetComponent<BallMove>().ResetIgnoreCollision();

        crosshair.GetComponent<BallMove>().Halt();
        target.GetComponent<BallMove>().Halt();

        Invoke("RestartAfterDelay", 0.5f);
    }

    public void SetHeroImage(bool isEnabled)
    {
        if(isEnabled == true)
        {
            crosshair.GetComponent<BallMove>().heroImage.GetComponent<Image>().sprite = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().facingRight;
            crosshair2.GetComponent<BallMove>().heroImage.GetComponent<Image>().sprite = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().facingRight;

            crosshair.GetComponent<BallMove>().heroImage.SetActive(true);
            crosshair2.GetComponent<BallMove>().heroImage.SetActive(true);

            crosshair.GetComponent<Image>().enabled = false;
            crosshair2.GetComponent<Image>().enabled = false;
        }

        else if (isEnabled == false)
        {
            crosshair.GetComponent<BallMove>().heroImage.SetActive(false);
            crosshair2.GetComponent<BallMove>().heroImage.SetActive(false);

            crosshair.GetComponent<Image>().enabled = true;
            crosshair2.GetComponent<Image>().enabled = true;
        }
    }
}
