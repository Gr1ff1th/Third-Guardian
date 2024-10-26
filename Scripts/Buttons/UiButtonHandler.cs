using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class UiButtonHandler : MonoBehaviour
{
    public Button questOfferButton;
    public Button handCardsButton;
    public Button consoleButton;
    public Button equipmentButton;
    public Button artifactOfferButton;
    public Button statButton;

    public Button skillRerollButton;
    public List<Button> upgradeRerollButtons;

    public GameObject skillUpgradeButton1;
    public GameObject unequippedItemsButton;

    public GameObject skillUpgradeButtonInStatsScreen;
    public GameObject inventoryButtonInStatsScreen;

    public GameObject questOfferDisplay;
    //this isnt used in v91 anymore
    public GameObject handCardDisplay;
    public GameObject consoleFrame;
    //public GameObject passiveCardsDisplay;
    public GameObject equipmentDisplay;
    //public GameObject artifactOfferDisplay;

    public GameObject tutorialHolder;
    public GameObject consoleNamePlate;

    //for handling stat display
    public GameObject statDisplay;
    public GameObject defaultCharacterIcons;
    public GameObject defaultCharacterNameDisplay;
    //public List<GameObject> statDisplayCharacterIcons;
    public TextMeshProUGUI statDisplayHeroName;

    //for new upgrade system for v93
    public GameObject upgradeDisplay;
    //public GameObject levelupCardsDisplay;

    //for handling equipment display
    public List<GameObject> equipmentDisplayCharacterIcons;
    public TextMeshProUGUI equipmentDisplayHeroName;

    public bool statsPanelActivated;
    public bool equipmentPanelActivated;
    public bool upgradePanelActivated;

    // Start is called before the first frame update
    void Start()
    {
        statsPanelActivated = false;
        equipmentPanelActivated = false;
        upgradePanelActivated = false;
    }

    public void QuestOfferButtonPressed()
    {
        //play sfx
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        if (questOfferDisplay.activeSelf)
        {
            questOfferDisplay.SetActive(false);
            //ReturnToNodeIfYourTurn();
        }
        else
        {
            CloseAllDisplays();
            questOfferDisplay.SetActive(true);
            //LeaveNodeIfYourTurn();
        }
    }

    public void HandCardsButtonPressed()
    {
        //play sfx
        //GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        if (handCardDisplay.activeSelf)
        {
            handCardDisplay.SetActive(false);
            //ReturnToNodeIfYourTurn();
        }
        else
        {
            CloseAllDisplays();
            handCardDisplay.SetActive(true);
            //LeaveNodeIfYourTurn();
        }
    }

    public void ConsoleButtonPressed()
    {
        //play sfx
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //could return this in any case?
        GameManager.ins.references.characterIconDisplay.SetActive(true);

        if (consoleFrame.activeSelf)
        {
            consoleFrame.SetActive(false);

            if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().isTutorial == true)
            {
                tutorialHolder.gameObject.SetActive(false);
            }
            else
            {
                //close the console itself separately
                GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.gameObject.SetActive(false);
                //ReturnToNodeIfYourTurn();

            }
        }
        else
        {
            CloseAllDisplays();
            consoleFrame.SetActive(true);

            if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().isTutorial == true)
            {
                tutorialHolder.gameObject.SetActive(true);
            }
            else
            {
                //open the console itself separately
                GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.gameObject.SetActive(true);
                //LeaveNodeIfYourTurn();
            }
        }
    }

    //deprecated
    public void PassiveCardsButtonPressed()
    {
        // play sfx
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        /* disable this for now
        if (passiveCardsDisplay.activeSelf)
        {
            passiveCardsDisplay.SetActive(false);
            //ReturnToNodeIfYourTurn();
        }
        else
        {
            CloseAllDisplays();
            passiveCardsDisplay.SetActive(true);
            //LeaveNodeIfYourTurn();
        }
        */
    }

    public void EquipmentButtonPressed()
    {
        //play sfx
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        if (equipmentDisplay.activeSelf)
        {
            equipmentDisplay.SetActive(false);

            equipmentPanelActivated = false;
            statsPanelActivated = false;
            upgradePanelActivated = false;

            upgradeDisplay.SetActive(false);
            statDisplay.SetActive(false);

            GameManager.ins.references.characterIconDisplay.SetActive(true);
        }
        else
        {
            CloseAllDisplays();
            equipmentDisplay.SetActive(true);

            equipmentPanelActivated = true;

            upgradeDisplay.SetActive(false);
            upgradePanelActivated = true;

            statDisplay.SetActive(true);

            inventoryButtonInStatsScreen.SetActive(false);
            skillUpgradeButtonInStatsScreen.SetActive(true);

            GameManager.ins.references.characterIconDisplay.SetActive(false);

            UpdateEquipmentTooltips();
        }
    }
    /*
    public void ArtifactOfferButtonPressed()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        if (artifactOfferDisplay.activeSelf)
        {
            artifactOfferDisplay.SetActive(false);
            //ReturnToNodeIfYourTurn();
        }
        else
        {
            CloseAllDisplays();
            artifactOfferDisplay.SetActive(true);
            //LeaveNodeIfYourTurn();
        }
    }
    */

    //lets just use this for closing all the displays for now
    public void StatsButtonPressed()
    {
        //play sfx
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        /* dont need these anymore for v93
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().UpdateUpgradeOfferTooltips();
       
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor == 0)
        {
            //skillRerollButton.interactable = false;
            ActivateRerollButtons(false);
        }
        else
        {
            //skillRerollButton.interactable = true;
            ActivateRerollButtons(true);
        }
        */
        if (statDisplay.activeSelf)
        {
            statDisplay.SetActive(false);
            equipmentDisplay.SetActive(false);
            upgradeDisplay.SetActive(false);

            equipmentPanelActivated = false;
            upgradePanelActivated = false;
            statsPanelActivated = false;

            GameManager.ins.references.characterIconDisplay.SetActive(true);
            //ReturnToNodeIfYourTurn();
            //defaultCharacterIcons.SetActive(true);
            //defaultCharacterNameDisplay.SetActive(true);

            //enables movementBonus & AP displays
            //GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().EnablePlates();
        }
        else
        {
            CloseAllDisplays();
            statDisplay.SetActive(true);

            statsPanelActivated = true;

            GameManager.ins.references.characterIconDisplay.SetActive(false);

            //defaultCharacterIcons.SetActive(false);
            //defaultCharacterNameDisplay.SetActive(false);
            //LeaveNodeIfYourTurn();

            //GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().DisablePlates();
        }
    }

    public void UpgradeButtonPressed()
    {
        //play sfx
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        GameManager.ins.toolTipBackground.SetActive(false);

        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().UpdateUpgradeOfferTooltips();

        if (upgradeDisplay.activeSelf)
        {
            upgradeDisplay.SetActive(false);
            statDisplay.SetActive(false);
            equipmentPanelActivated = false;
            statsPanelActivated = false;
            upgradePanelActivated = false;

            GameManager.ins.references.characterIconDisplay.SetActive(true);
        }
        else
        {
            CloseAllDisplays();
            upgradeDisplay.SetActive(true);
            upgradePanelActivated = true;

            equipmentDisplay.SetActive(false);
            statDisplay.SetActive(true);

            inventoryButtonInStatsScreen.SetActive(true);
            skillUpgradeButtonInStatsScreen.SetActive(false);

            GameManager.ins.references.characterIconDisplay.SetActive(false);
        }
    }

    //levelup button on bottom tray
    public void LelevupButtonPressed1()
    {
        UpgradeButtonPressed();

        //levelupCardsDisplay.SetActive(true);
    }

    /*levelup button on stats display (unused)
    public void LelevupButtonPressed2()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        if (levelupCardsDisplay.activeSelf)
        {
            levelupCardsDisplay.SetActive(false);
        }
        else
        {
            levelupCardsDisplay.SetActive(true);
        }
    }
    */

    //for rerolling skills (any)
    public void skillRerollButtonPressed()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();
        
        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(6, -1);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor == 0)
        {
            //skillRerollButton.interactable = false;
            ActivateRerollButtons(false);
        }
        else
        {
            //skillRerollButton.interactable = true;
            ActivateRerollButtons(true);
        }

        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().ClearUpgradeOffer();
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();
    }

    //for v92
    //focus on skillpoint upgrades
    public void FocusSkillpointRerollButtonPressed()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(6, -1);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor == 0)
        {
            //skillRerollButton.interactable = false;
            ActivateRerollButtons(false);
        }
        else
        {
            //skillRerollButton.interactable = true;
            ActivateRerollButtons(true);
        }

        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().ClearUpgradeOffer();
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();
    }

    //focus on general upgrades
    public void FocusGeneralRerollButtonPressed()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(6, -1);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor == 0)
        {
            ActivateRerollButtons(false);
        }
        else
        {
            ActivateRerollButtons(true);
        }

        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().ClearUpgradeOffer();
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();
    }

    //focus on class upgrades
    public void FocusClassRerollButtonPressed()
    {
        GameManager.ins.sfxPlayer.GetComponent<SFXPlayer>().PlayButton1();

        //other way of doing sfx
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.clip = GameManager.ins.references.sfxPlayer.Contemplate.clip;
        GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().sfxHolder.Play();

        GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().UpdateResources(6, -1);

        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().favor == 0)
        {
            ActivateRerollButtons(false);
        }
        else
        {
            ActivateRerollButtons(true);
        }

        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().ClearUpgradeOffer();
        CardHandler.ins.gameObject.GetComponent<SkillUpgradeHandler>().DelayedUpgradeOffer();
    }

    public void CloseAllDisplays()
    {
        //enables movementBonus & AP displays
        //GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().EnablePlates();

        questOfferDisplay.SetActive(false);
        //handCardDisplay.SetActive(false);
        //passiveCardsDisplay.SetActive(false);
        equipmentDisplay.SetActive(false);
        //artifactOfferDisplay.SetActive(false);
        consoleFrame.SetActive(false);
        statDisplay.SetActive(false);
        upgradeDisplay.SetActive(false);

        statsPanelActivated = false;
        equipmentPanelActivated = false;
        upgradePanelActivated = false;

        //activate these in all other cases except when opening stat screen
        //defaultCharacterIcons.SetActive(true);
        //defaultCharacterNameDisplay.SetActive(true);

        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().isTutorial == true)
        {
            tutorialHolder.gameObject.SetActive(false);
        }

        //close the console itself separately
        GameObject.Find("AMMRoomController").GetComponentInChildren<PhotonChatManager>().chatPanel.gameObject.SetActive(false);
    }

    public void LeaveNodeIfYourTurn()
    {
        if(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.Leave();
        }
    }

    public void ReturnToNodeIfYourTurn()
    {
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == true && GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isAi == false)
        {
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel2();
        }
    }

    public void DisableAllButtons()
    {
        questOfferButton.interactable = false;
        //handCardsButton.interactable = false;
        consoleButton.interactable = false;
        equipmentButton.interactable = false;
        artifactOfferButton.interactable = false;
        statButton.interactable = false;
    }

    public void EnableAllButtons()
    {
        questOfferButton.interactable = true;
        //handCardsButton.interactable = true;
        consoleButton.interactable = true;
        equipmentButton.interactable = true;
        artifactOfferButton.interactable = true;
        statButton.interactable = true;
    }

    public void UpdateEquipmentTooltips()
    {
        //goes throught the equipment display
        for (int i = 0; i < GameManager.ins.equipmentCardArea.GetComponent<Transform>().childCount; i++)
        {
            if(GameManager.ins.turnNumber == GameManager.ins.equipmentCardArea.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                GameManager.ins.equipmentCardArea.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        //need to go through all the gear slots too?
        for (int i = 0; i < CardHandler.ins.helmSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.helmSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.helmSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.armorSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.armorSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.armorSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.ringSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.ringSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.ringSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.weaponSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.weaponSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.weaponSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.miscSlot1.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.miscSlot1.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.miscSlot1.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.mountSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.mountSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.mountSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        /* not needed on v94+
        for (int i = 0; i < CardHandler.ins.miscSlot2.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.miscSlot2.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.miscSlot2.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.miscSlot3.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.miscSlot3.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.miscSlot3.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        */
        for (int i = 0; i < CardHandler.ins.gogglesSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.gogglesSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.gogglesSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.maskSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.maskSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.maskSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.amuletSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.amuletSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.amuletSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.tomeSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.tomeSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.tomeSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.toolboxSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.toolboxSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.toolboxSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        for (int i = 0; i < CardHandler.ins.shovelSlot.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.turnNumber == CardHandler.ins.shovelSlot.GetComponent<Transform>().GetChild(i).GetComponent<Card>().belongsTo)
            {
                CardHandler.ins.shovelSlot.GetComponent<Transform>().GetChild(i).GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
    }

    //for v92
    public void ActivateRerollButtons(bool activate)
    {
        if(activate == true)
        {
            for (int i = 0; i < upgradeRerollButtons.Count; i++)
            {
                upgradeRerollButtons[i].interactable = true;
            }
        }
        else if (activate == false)
        {
            for (int i = 0; i < upgradeRerollButtons.Count; i++)
            {
                upgradeRerollButtons[i].interactable = false;
            }
        }
    }
}
