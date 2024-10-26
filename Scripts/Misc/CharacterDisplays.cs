using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterDisplays : MonoBehaviour
{
    public List<GameObject> toDisable;

    public List<GameObject> characters;

    public GameObject characterList;
    public GameObject characterBackground;

    public GameObject darkPanel;
    public GameObject skillsDisplay;

    //for stat texts
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI arcanePowerText;
    public TextMeshProUGUI resistanceText;

    public TextMeshProUGUI influenceText;
    public TextMeshProUGUI mechanicsText;
    public TextMeshProUGUI diggingText;
    public TextMeshProUGUI loreText;
    public TextMeshProUGUI observeText;

    public bool showSkillPoints;

    //for v0.7.0.
    public TextMeshProUGUI damageText;
    public float textTimer;

    //public Quaternion defaultRotation;

    private void Start()
    {
        //take the rotation of one character as reference
        //defaultRotation = characters[9].gameObject.transform.rotation;

        /* removed in v95
        for (int i = 0; i < characters.Count; i++)
        {
            //seems this finally allows you to reset the rotation
            Animator myAnimator = characters[i].GetComponent<Animator>();
            myAnimator.keepAnimatorControllerStateOnDisable = true;
        }
        */

        damageText.text = "";
    }

    void Update()
    {
        //could handle damage text counter here
        if (textTimer > 0)
        {
            textTimer -= Time.deltaTime;
        }
        else
        {
            damageText.text = "";
        }
    }

    public void DisablePlates()
    {
        for (int i = 0; i < toDisable.Count; i++)
        {
            toDisable[i].gameObject.SetActive(false);
        }
    }

    public void EnablePlates()
    {
        for (int i = 0; i < toDisable.Count; i++)
        {
            toDisable[i].gameObject.SetActive(true);
        }
    }

    //modified for v95
    public void ShowCharacter(int turnNumber, int animationNumber)
    {
        //dont need this anymore?
        //DisablePlates();

        characterList.gameObject.SetActive(true);
        //characterBackground.gameObject.SetActive(true);
        darkPanel.gameObject.SetActive(true);
        skillsDisplay.gameObject.SetActive(true);

        //lets put takeballscore to combat handler methods instead
        if (GameManager.ins.references.targettingHandler.targettingEnabled == true && (CardHandler.ins.phaseNumber == 3 || CardHandler.ins.phaseNumber == 4)) 
        {
            GameManager.ins.references.targettingHandler.targettingBorders.SetActive(true);
            GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(true);
            GameManager.ins.references.GetComponent<SliderController>().StartCombatTimer();
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().holedBackground;
            //GameManager.ins.references.targettingHandler.TakeBallScore();
        }
        else
        {
            GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.GetComponent<Image>().sprite = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().normalBackground;
            GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
            GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
            GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
        }
        

        if (showSkillPoints == true)
        {
            skillsDisplay.SetActive(true);
            ShowSkillPoints(turnNumber);
        }
        else
        {
            skillsDisplay.SetActive(false);
        }

        int heroNumber = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().heroNumber;

        for (int i = 0; i < characters.Count; i++)
        {
            if (i == heroNumber)
            {
                characters[i].gameObject.SetActive(true);
            }

            if (i != heroNumber)
            {
                characters[i].gameObject.SetActive(false);
            }
        }

        //reset the animations
        /*probably not needed
        characters[heroNumber].GetComponent<Animator>().SetBool("Strength", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Talk 1", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Crouch Interact", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Attack Twohanded 1", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Talk 2", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Idle Relaxed", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Idle Wide", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Run", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Walk", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Talk 3", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Hit", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Die", false);

        //couldnt find another way to reset the rotation for now
        //characters[heroNumber].transform.rotation = defaultRotation;

        //test this to reset animation
        //doesnt reset rotation, but restarts the animation
        Animator anim = characters[heroNumber].GetComponent<Animator>();
        anim.Rebind();
        anim.Update(0f);
        */

        //attack anim
        if (animationNumber == 1 || animationNumber == 2)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Strength", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroAttack(1);
            Invoke(nameof(PlayStrengthSfx), 0.1f);
            Invoke(nameof(PlayStrengthSfx), 0.8f);
            Invoke(nameof(PlayStrengthSfx), 1.5f);
        }

        //defense animation
        if (animationNumber == 2)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Strength", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(2);
            Invoke(nameof(PlayStrengthSfx), 0.1f);
            Invoke(nameof(PlayStrengthSfx), 0.8f);
            Invoke(nameof(PlayStrengthSfx), 1.5f);
        }

        // influence animation
        if (animationNumber == 5)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Talk 1", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
            Invoke("PlayInfluenceSfx", 0.2f);
        }

        // mechanics animation
        if (animationNumber == 6)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Crouch Interact", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(2);
            Invoke("PlayMechanicsSfx", 0.5f);
            Invoke("PlayMechanicsSfx", 1.5f);
            Invoke("PlayMechanicsSfx", 2.5f);
        }

        // digging animation
        if (animationNumber == 7)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Attack Twohanded 1", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(2);
            Invoke("PlayDiggingSfx", 1.0f);
            Invoke("PlayDiggingSfx", 2.5f);
            Invoke("PlayDiggingSfx", 4.0f);
        }

        //lore animation
        if (animationNumber == 8)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Talk 3", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
            Invoke("PlayInfluenceSfx", 0.2f);
        }

        // arcane power
        if (animationNumber == 3)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Cast 1", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroAttack(1);
            Invoke("PlayLoreSfx", 0.2f);
        }
        // resistance animation
        if (animationNumber == 4)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Cast 1", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
            Invoke("PlayLoreSfx", 0.2f);
        }

        // discovery animation
        if (animationNumber == 9)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Idle Relaxed", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
            Invoke("PlayDiscoverySfx", 0.2f);
        }

        // victory animation (or not moving animation)
        if (animationNumber == 10)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Idle Wide", true);
            //characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
        }

        // running (failure) animation
        if (animationNumber == 11)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Run", true);
            //characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
        }

        // walking animation
        if (animationNumber == 12)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Walk", true);
            //characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
        }

        // old lore animation
        if (animationNumber == 13)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Talk 2", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);
        }

        // hit animation
        if (animationNumber == 14)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Hit", true);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(2);

            Invoke("PlayStrengthSfx", 0.1f);
            Invoke("PlayStrengthSfx", 0.7f);
            Invoke("PlayStrengthSfx", 1.3f);
        }

        // die animation
        if (animationNumber == 15)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Die", true);
            //we might want tombstone here
            characters[heroNumber].GetComponent<Image>().sprite = GameManager.ins.references.enemyResizing.foeGravestone;
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(1);

            Invoke("PlayStrengthSfx", 0.1f);
        }

        // throw bomb animation
        if (animationNumber == 16)
        {
            //characters[heroNumber].GetComponent<Animator>().SetBool("Offhand 1", true);
            //characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroBump(2);
            characters[heroNumber].GetComponent<HeroResizing>().ActivateHeroAttack(1);

            //also returns idle animation here
            Invoke("PlayExplosionSfx", 0.1f);
        }

        //Invoke("TestReset", 5.0f);
    }

    //when skills button is pressed
    public void SkillsButton()
    {
        if (skillsDisplay.activeSelf)
        {
            skillsDisplay.SetActive(false);
            showSkillPoints = false;
        }

        else
        {
            skillsDisplay.SetActive(true);
            ShowSkillPoints(GameManager.ins.turnNumber);
            showSkillPoints = true;
        }
    }

    //remove negative cap for v94
    public void ShowSkillPoints(int turnNumber)
    {
        int skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().strength;
        if(skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        strengthText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().defense;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        defenseText.text = skillpoints.ToString();


        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().arcanePower;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        arcanePowerText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().resistance;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        resistanceText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().influence;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        influenceText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().mechanics;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        mechanicsText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().digging;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        diggingText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().lore;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        loreText.text = skillpoints.ToString();

        skillpoints = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().observe;
        if (skillpoints > 6)
        {
            skillpoints = 6;
        }
        if (skillpoints < -3)
        {
            skillpoints = -3;
        }
        observeText.text = skillpoints.ToString();
    }

    //not used
    void TestReset()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            //this is needed to reset the rotation after certain animations (works for some reason, dont ask why :-))
            characters[i].GetComponent<Animator>().WriteDefaultValues();

            /*test this to reset animation
            Animator anim = characters[i].GetComponent<Animator>();
            anim.Rebind();
            anim.Update(0f);
            */
        }
    }

    //modify for v95
    public void HideCharacter(int turnNumber)
    {
        //dont need this anymore?
        //EnablePlates();

        int heroNumber = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().heroNumber;

        //this is needed to reset the rotation after certain animations (actually dunno if it does anything atm)
        //characters[heroNumber].GetComponent<Animator>().WriteDefaultValues();

        //reset the animations
        /*probably not needed
        characters[heroNumber].GetComponent<Animator>().SetBool("Strength", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Talk 1", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Crouch Interact", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Attack Twohanded 1", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Talk 2", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Idle Relaxed", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Idle Wide", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Run", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Walk", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Talk 3", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Hit", false);
        characters[heroNumber].GetComponent<Animator>().SetBool("Die", false);
        */
        //couldnt find another way to reset the rotation for now
        //characters[heroNumber].transform.rotation = defaultRotation;

        /*
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].gameObject.SetActive(false);
        }
        */

        //Animator anim = characters[heroNumber].GetComponent<Animator>();
        //anim.Rebind();
        //anim.Update(0f);

        //Invoke("DelayedHideCharacter", 2.5f);

        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].gameObject.SetActive(false);
        }

        characterList.gameObject.SetActive(false);
        characterBackground.gameObject.SetActive(false);
        darkPanel.gameObject.SetActive(false);
        skillsDisplay.gameObject.SetActive(false);

        //lets put these here temporarily
        GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
        GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
        GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();

        //characters[heroNumber].GetComponent<Animator>().SetBool("Cast 1", true);
    }

    //not used
    void DelayedHideCharacter()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].gameObject.SetActive(false);
        }

        characterList.gameObject.SetActive(false);
        characterBackground.gameObject.SetActive(false);
        darkPanel.gameObject.SetActive(false);
        skillsDisplay.gameObject.SetActive(false);

        //lets put these here temporarily
        GameManager.ins.references.targettingHandler.targettingBorders.SetActive(false);
        GameManager.ins.references.targettingHandler.targettingDisplay.SetActive(false);
        GameManager.ins.references.GetComponent<SliderController>().RemoveCombatTimer();
    }

    void PlayStrengthSfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayStrength();
    }

    void PlayInfluenceSfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayInfluence();
    }

    void PlayMechanicsSfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayMechanics();
    }

    void PlayDiggingSfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayDigging();
    }
    void PlayLoreSfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayLore();
    }

    void PlayDiscoverySfx()
    {
        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayDiscovery();
    }

    void PlayExplosionSfx()
    {
        int heroNumber = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber;

        //play sfx
        GameManager.ins.references.sfxPlayer.GetComponent<SFXPlayer>().PlayEruption();

        //characters[heroNumber].GetComponent<Animator>().SetBool("Offhand 1", false);
    }

    public void SetDamageDisplay(int hits, int bestRoll, bool showHits)
    {
        textTimer = 2f;

        if(showHits == true)
        {
            damageText.text = GameManager.ins.references.GetComponent<CombatActions>().GetDice(bestRoll) + " -" + hits;
        }

        else
        {
            damageText.text = GameManager.ins.references.GetComponent<CombatActions>().GetDice(bestRoll);
        }
    }
}
