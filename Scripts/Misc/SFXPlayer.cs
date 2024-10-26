using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SFXPlayer : MonoBehaviour
{
    public PhotonView PV;

    //for movementBonus
    public AudioSource Step1;
    public AudioSource Step2;
    public AudioSource Step3;
    public AudioSource Step4;
    public AudioSource Step5;

    //for horsesounds (need lesser volume
    public AudioSource HorseStepHolder;

    //questing
    public AudioSource SkillCheckFail;
    public AudioSource SkillCheckSuccess;
    public AudioSource QuestFail;
    public AudioSource QuestSuccess;

    //UI
    public AudioSource Button1;
    public AudioSource OpenActionMenu;
    public AudioSource Message;
    public AudioSource PrivateMessage;
    public AudioSource ElectionSuccess1;
    public AudioSource ElectionSuccess2;
    public AudioSource ElectionFail;
    public AudioSource SystemMessage;
    public AudioSource PrivateSystemMessage;
    public AudioSource Alert;

    //actions
    public AudioSource Interaction;
    public AudioSource Rest;
    public AudioSource Sleep;
    public AudioSource Contemplate;
    public AudioSource SelfMaintenance;

    //turn start
    public AudioSource TurnStart;

    //cards
    public AudioSource CardDisplayed;
    public AudioSource CardDrawn;

    //crafting
    public AudioSource Smithing;
    public AudioSource Writing;

    //encounters (these dont have method yet)
    //actually these are used for various purposes now..
    public AudioSource Error;
    public AudioSource Sonya;
    public AudioSource SonyaPurchase;
    public AudioSource Eruption;
    public AudioSource Ghost;
    public AudioSource Refugees;
    public AudioSource Pirates;
    public AudioSource Steele;
    public AudioSource Plague;
    public AudioSource Storm;
    public AudioSource Teleport;
    public AudioSource Rum;
    public AudioSource RumDrank;
    public AudioSource Minefield;
    public AudioSource Coinpurse;
    public AudioSource Paydays;
    public AudioSource Turmoil;

    //skill sounds
    public AudioSource Strength;
    public AudioSource Influence;
    public AudioSource Mechanics;
    public AudioSource Digging;
    public AudioSource Lore;
    public AudioSource Discovery;

    //misc
    public AudioSource Encounter;
    public AudioSource Drinking;

    //lets just use mostly clips since v94+
    public AudioClip Sweep;
    public AudioClip HorseGallop1;
    public AudioClip HorseGallop2;
    public AudioClip HorseGallop3;
    public AudioClip HorseGallop4;
    public AudioClip HorseGallop5;
    public AudioClip HorseGallop6;
    public AudioClip EarthShatter;
    public AudioClip Diceroll;

    //v0.5.7.
    public AudioClip FoeSpecialTrigger;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    //makes sounds depending on movetype
    public void MovementSound()
    {
        //special case for water
        if (GameManager.ins.references.currentMinimap != null)
        {
            if (GameManager.ins.references.currentMinimap.minimapNumber == 30 ||
                GameManager.ins.references.currentMinimap.minimapNumber == 72)
            {
                CardHandler.ins.extraSfxHolder.clip = Sweep;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //gryphon
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 68) == true)
            {
                CardHandler.ins.extraSfxHolder.clip = Sweep;
                CardHandler.ins.extraSfxHolder.Play();
            }

            //check if any horse equipped
            else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 63) == true ||
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 64) == true ||
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 65) == true ||
                CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 69) == true)
            {
                HorseGallopSound();
            }

            else
            {
                WalkSound();
            }
        }
        
        //gryphon
        else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 68) == true)
        {
            CardHandler.ins.extraSfxHolder.clip = Sweep;
            CardHandler.ins.extraSfxHolder.Play();
        }

        //check if any horse equipped
        else if (CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 63) == true ||
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 64) == true ||
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 65) == true ||
            CardHandler.ins.CheckItemInSlot(GameManager.ins.turnNumber, 5, 69) == true)
        {
            HorseGallopSound();
        }
        else
        {
            WalkSound();
        }
    }

    public void HorseGallopSound()
    {
        //CardHandler.ins.extraSfxHolder.clip = HorseGallop1;
        //CardHandler.ins.extraSfxHolder.Play();

        //CardHandler.ins.extraSfxHolder.volume = 0.5f;

        HorseGallopSingle();

        Invoke("HorseGallopSingle", 0.15f);
        Invoke("HorseGallopSingle", 0.45f);
        Invoke("HorseGallopSingle", 0.6f);

        //Invoke(nameof(ReturnVolume), 1f);
    }

    public void ReturnVolume()
    {
        CardHandler.ins.extraSfxHolder.volume = 1f;
    }

    public void HorseGallopSingle()
    {
        int sound1 = Random.Range(1, 7);

        if (sound1 == 1)
        {
            PlayHorseGallop1();
        }
        if (sound1 == 2)
        {
            PlayHorseGallop2();
        }
        if (sound1 == 3)
        {
            PlayHorseGallop3();
        }
        if (sound1 == 4)
        {
            PlayHorseGallop4();
        }
        if (sound1 == 5)
        {
            PlayHorseGallop5();
        }
        if (sound1 == 6)
        {
            PlayHorseGallop6();
        }
    }

    public void WalkSound()
    {
        int sound1 = Random.Range(1, 6);

        if (sound1 == 1)
        {
            PlayStep1();
        }
        if (sound1 == 2)
        {
            PlayStep2();
        }
        if (sound1 == 3)
        {
            PlayStep3();
        }
        if (sound1 == 4)
        {
            PlayStep4();
        }
        if (sound1 == 5)
        {
            PlayStep5();
        }

        int sound2 = Random.Range(1, 6);

        if (sound2 == 1)
        {
            Invoke("PlayStep1", 0.3f);
        }
        if (sound2 == 2)
        {
            Invoke("PlayStep2", 0.3f);
        }
        if (sound2 == 3)
        {
            Invoke("PlayStep3", 0.3f);
        }
        if (sound2 == 4)
        {
            Invoke("PlayStep4", 0.3f);
        }
        if (sound2 == 5)
        {
            Invoke("PlayStep5", 0.3f);
        }

        int sound3 = Random.Range(1, 6);

        if (sound3 == 1)
        {
            Invoke("PlayStep1", 0.6f);
        }
        if (sound3 == 2)
        {
            Invoke("PlayStep2", 0.6f);
        }
        if (sound3 == 3)
        {
            Invoke("PlayStep3", 0.6f);
        }
        if (sound3 == 4)
        {
            Invoke("PlayStep4", 0.6f);
        }
        if (sound3 == 5)
        {
            Invoke("PlayStep5", 0.6f);
        }
    }

    //tweaked for v0.7.0.
    void PlayHorseGallop1()
    {
        HorseStepHolder.clip = HorseGallop1;
        HorseStepHolder.Play();
        //AudioSource.PlayClipAtPoint(HorseGallop1, CardHandler.ins.extraSfxHolder.transform.position, 0.5f * GameManager.ins.references.soundManager.sfxVol);
    }

    void PlayHorseGallop2()
    {
        HorseStepHolder.clip = HorseGallop2;
        HorseStepHolder.Play();
    }

    void PlayHorseGallop3()
    {
        HorseStepHolder.clip = HorseGallop3;
        HorseStepHolder.Play();
    }

    void PlayHorseGallop4()
    {
        HorseStepHolder.clip = HorseGallop4;
        HorseStepHolder.Play();
    }

    void PlayHorseGallop5()
    {
        HorseStepHolder.clip = HorseGallop5;
        HorseStepHolder.Play();
    }

    void PlayHorseGallop6()
    {
        HorseStepHolder.clip = HorseGallop6;
        HorseStepHolder.Play();
    }

    public void PlayDiceroll()
    {
        CardHandler.ins.extraSfxHolder.clip = Diceroll;
        CardHandler.ins.extraSfxHolder.Play();
    }

    void PlayStep1()
    {
        Step1.Play();
    }

    void PlayStep2()
    {
        Step2.Play();
    }

    void PlayStep3()
    {
        Step3.Play();
    }

    void PlayStep4()
    {
        Step4.Play();
    }
    void PlayStep5()
    {
        Step5.Play();
    }

    public void PlaySkillCheckFail()
    {
        SkillCheckFail.Play();
    }

    public void PlaySkillCheckSuccess()
    {
        SkillCheckSuccess.Play();
    }

    public void PlayQuestFail()
    {
        QuestFail.Play();
    }

    public void PlayQuestSuccess()
    {
        QuestSuccess.Play();
    }

    public void PlayElectionSuccess1()
    {
        ElectionSuccess1.Play();
    }

    public void PlayElectionSuccess2()
    {
        ElectionSuccess2.Play();
    }

    public void PlayElectionFail()
    {
        ElectionFail.Play();
    }

    public void PlayButton1()
    {
        Button1.Play();
    }

    public void PlayOpenActionMenu()
    {
        OpenActionMenu.Play();
    }

    public void PlayTurnStart()
    {
        TurnStart.Play();
    }

    public void PlayCardDisplayed()
    {
        CardDisplayed.Play();
    }

    public void PlayCardDrawn()
    {
        CardDrawn.Play();
    }

    public void PlaySmithing()
    {
        Smithing.Play();
    }

    public void PlayWriting()
    {
        Writing.Play();
    }

    //these could be used when receiving messages in the console
    public void PlayMessage()
    {
        Message.Play();
    }

    public void PlayPrivateMessage()
    {
        PrivateMessage.Play();
    }

    public void PlaySystemMessage()
    {
        SystemMessage.Play();
    }

    public void PlayPrivateSystemMessage()
    {
        PrivateSystemMessage.Play();
    }

    public void PlayAlert()
    {
        Alert.Play();
    }

    //ACTIONS

    public void PlayInteractionForOthers()
    {
        //Interaction.Play();
        //playes sfx for all
        PV.RPC("RPC_PlayInteractionForOthers", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPC_PlayInteractionForOthers()
    {
        //dont play for current player
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            return;
        }
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().Interaction.Play();
    }

    [PunRPC]
    void RPC_PlayRest()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().Rest.Play();
    }

    [PunRPC]
    void RPC_PlaySleep()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().Sleep.Play();
    }

    [PunRPC]
    public void RPC_PlayContemplate()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().Contemplate.Play();
    }

    [PunRPC]
    void RPC_PlaySelfMaintenance()
    {
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().SelfMaintenance.Play();
    }

    /*
     * Encounter SFX's
     * 
     */

    public void PlayUlriman()
    {
        Error.Play();
    }

    public void PlaySonya()
    {
        Sonya.Play();
    }

    public void PlaySonyaPurchase()
    {
        SonyaPurchase.Play();
    }

    public void PlayEruption()
    {
        Eruption.Play();
    }

    public void PlayGhost()
    {
        Ghost.Play();
    }

    public void PlayRefugees()
    {
        Refugees.Play();
    }

    public void PlayPirates()
    {
        Pirates.Play();
    }

    public void PlaySteele()
    {
        Steele.Play();
    }

    public void PlayPlague()
    {
        Plague.Play();
    }

    public void PlayStorm()
    {
        Storm.Play();
    }

    public void PlayTeleport()
    {
        Teleport.Play();
    }

    public void PlayRum()
    {
        Rum.Play();
    }

    public void PlayRumDrank()
    {
        RumDrank.Play();
    }

    public void PlayMinefield()
    {
        Minefield.Play();
    }

    public void PlayCoinpurse()
    {
        Coinpurse.Play();
    }

    public void PlayPaydays()
    {
        Paydays.Play();
    }

    public void PlayTurmoil()
    {
        Turmoil.Play();
    }

    //skill sfx
    public void PlayStrength()
    {
        Strength.Play();
    }
    public void PlayInfluence()
    {
        Influence.Play();
    }
    public void PlayMechanics()
    {
        Mechanics.Play();
    }
    public void PlayDigging()
    {
        Digging.Play();
    }
    public void PlayLore()
    {
        Lore.Play();
    }
    public void PlayDiscovery()
    {
        Discovery.Play();
    }

    //misc
    public void PlayEncounter()
    {
        Encounter.Play();
    }

    public void PlayDrinking()
    {
        Drinking.Play();
    }
}
