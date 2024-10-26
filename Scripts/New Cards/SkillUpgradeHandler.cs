using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradeHandler : MonoBehaviour
{
    List<GameObject> generalDeck;

    //used for focusing upgrade rerolls
    //1= any, 2= skillpoints, 3= general, 4= class
    public int focusType;

    // Start is called before the first frame update
    void Start()
    {
        generalDeck = GameObject.Find("AMMRoomController").GetComponent<CardSaveHandler>().generalDeck;

        //DrawUpgradeOffer();
        
    }

    public void DelayedUpgradeOffer()
    {
        //this isnt actually needed anymore
        //focusType = focus;

        Invoke("DrawUpgradeCards3", 0.5f);
    }

    //new system for v93
    public void DrawUpgradeCards3()
    {
        for (int i = 0; i < gameObject.GetComponent<CardHandler>().generalDeck.Count; i++)
        {
            //also check whether you have the card alrdy
            // could make level check here also
            if (generalDeck[i].GetComponent<Card>().isPossibleUpgrade == true && CheckIfHasCard(i) == false &&
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel >= generalDeck[i].GetComponent<Card>().levelRequirement)
            {
                //additional checks for special cards (skillpoints, higher card levels)
                //returns true for valid cards
                if (TestUpgradeCard(i) == true)
                {
                    //warrior test
                    if (generalDeck[i].GetComponent<Card>().classRequirement == 1)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(1))
                        {
                            InstantiateUpgradeCard(i, 3);
                        }
                    }

                    //artisan test
                    else if (generalDeck[i].GetComponent<Card>().classRequirement == 2)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(2))
                        {
                            InstantiateUpgradeCard(i, 3);
                        }
                    }

                    //arcanist test
                    else if (generalDeck[i].GetComponent<Card>().classRequirement == 3)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3))
                        {
                            InstantiateUpgradeCard(i, 3);
                        }
                    }
                    //cleric test
                    else if (generalDeck[i].GetComponent<Card>().classRequirement == 4)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(42))
                        {
                            InstantiateUpgradeCard(i, 3);
                        }
                    }

                    //no class req
                    else if (generalDeck[i].GetComponent<Card>().classRequirement == 0)
                    {
                        //new focus system
                        //skillpoint check
                        if (generalDeck[i].GetComponent<Card>().isSkillPoint == true)
                        {
                            InstantiateUpgradeCard(i, 1);
                        }

                        //general card check
                        else if (generalDeck[i].GetComponent<Card>().isSkillPoint == false)
                        {
                            InstantiateUpgradeCard(i, 2);
                        }
                    }
                }
            }
        }
        //could put this here too, incase card was removed from carddisplay method, but no new cards were drawn
        GameManager.ins.levelupCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForUpgradeCards();
    }


    //new system for v93
    //might be kinda heavy?
    public void InstantiateUpgradeCard(int numberInDeck, int holderType)
    {
        if(holderType == 1)
        {
            GameObject playerCard = Instantiate(gameObject.GetComponent<CardHandler>().generalDeck[numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.levelupCardArea.transform, false);
            playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(1);
        }
        if (holderType == 2)
        {
            GameObject playerCard = Instantiate(gameObject.GetComponent<CardHandler>().generalDeck[numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.levelupCardArea2.transform, false);
            playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(1);
        }
        if (holderType == 3)
        {
            GameObject playerCard = Instantiate(gameObject.GetComponent<CardHandler>().generalDeck[numberInDeck], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.levelupCardArea3.transform, false);
            playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(1);
        }

        //actually not sure if this is needed, since its alrdy called in drawupgradecards3 method
        //GameManager.ins.levelupCardArea.GetComponent<ScrollRectCenter>().ChangeSizeFitterForUpgradeCards();
    }

    /* old methods
    public void DrawUpgradeOffer()
    {
        int upgradeCardCount = GameManager.ins.levelupCardArea.transform.childCount;

        //maybe 6 cards on offer is enough?
        int cardsToAcquire = 6 - upgradeCardCount;

        for (int i = 0; i < cardsToAcquire; i++)
        {
            GameObject playerCard = Instantiate(gameObject.GetComponent<CardHandler>().generalDeck[DrawUpgradeCard()], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.levelupCardArea.transform, false);
            playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(1);
        }
    }

    //draws custom number of cards to the offer
    public void DrawUpgradeOffer2(int numberOfCards)
    {
        //this should always draw any type of card?
        focusType = 1;

        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject playerCard = Instantiate(gameObject.GetComponent<CardHandler>().generalDeck[DrawUpgradeCard()], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(GameManager.ins.levelupCardArea.transform, false);
            playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(1);
        }
    }
    */


    /* maybe save old system just in case
    //
    public int DrawUpgradeCard()
    {
        int i = 0;

        //draws any type of card, once counter runs to 3
        //otherwise draws only desired card
        int focusCounter = 3;

        do
        {
            int numberInDeck = Random.Range(0, generalDeck.Count);

            int rarityCheck = Random.Range(0, 101);

            //also check whether you have the card alrdy
            // could make level check here also
            if (generalDeck[numberInDeck].GetComponent<Card>().isPossibleUpgrade == true && CheckIfHasCard(numberInDeck) == false && 
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().characterLevel >= generalDeck[numberInDeck].GetComponent<Card>().levelRequirement)
            {
                //additional checks for special cards (skillpoints, higher card levels)
                //returns true for valid cards
                if (TestUpgradeCard(numberInDeck) == true)
                {
                    //warrior test
                    if (generalDeck[numberInDeck].GetComponent<Card>().classRequirement == 1 && generalDeck[numberInDeck].GetComponent<Card>().rarity >= rarityCheck)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(1))
                        {
                            //new focus system
                            if (focusCounter <= 0 || focusType == 1 || focusType == 4)
                            {
                                return numberInDeck;
                            }
                            focusCounter -= 1;
                        }
                    }

                    //artisan test
                    else if (generalDeck[numberInDeck].GetComponent<Card>().classRequirement == 2 && generalDeck[numberInDeck].GetComponent<Card>().rarity >= rarityCheck)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(2))
                        {
                            //new focus system
                            if (focusCounter <= 0 || focusType == 1 || focusType == 4)
                            {
                                return numberInDeck;
                            }
                            focusCounter -= 1;
                        }
                    }

                    //arcanist test
                    else if (generalDeck[numberInDeck].GetComponent<Card>().classRequirement == 3 && generalDeck[numberInDeck].GetComponent<Card>().rarity >= rarityCheck)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(3))
                        {
                            //new focus system
                            if (focusCounter <= 0 || focusType == 1 || focusType == 4)
                            {
                                return numberInDeck;
                            }
                            focusCounter -= 1;
                        }
                    }
                    //cleric test
                    else if (generalDeck[numberInDeck].GetComponent<Card>().classRequirement == 4 && generalDeck[numberInDeck].GetComponent<Card>().rarity >= rarityCheck)
                    {
                        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(42))
                        {
                            //new focus system
                            if (focusCounter <= 0 || focusType == 1 || focusType == 4)
                            {
                                return numberInDeck;
                            }
                            focusCounter -= 1;
                        }
                    }

                    //no class req
                    else if (generalDeck[numberInDeck].GetComponent<Card>().classRequirement == 0 && generalDeck[numberInDeck].GetComponent<Card>().rarity >= rarityCheck)
                    {
                        //new focus system
                        //skillpoint check
                        if (generalDeck[numberInDeck].GetComponent<Card>().isSkillPoint == true)
                        {
                            if (focusCounter <= 0 || focusType == 1 || focusType == 2)
                            {
                                return numberInDeck;
                            }
                            focusCounter -= 1;
                        }

                        //general card check
                        else if (generalDeck[numberInDeck].GetComponent<Card>().isSkillPoint == false)
                        {
                            if (focusCounter <= 0 || focusType == 1 || focusType == 3)
                            {
                                return numberInDeck;
                            }
                            focusCounter -= 1;
                        }
}
                }
            }

            i++;
        }
        //not the best idea, but dunno what condition to set here
        while (i < 1000);

        return 0;
    }
    */


    //returns true, if upgradecard is valid
    public bool TestUpgradeCard(int numberInDeck)
    {
        //strength upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 115)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades > 0)
            {
                return false;
            }
        }

        //strength upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 116)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().strengthUpgrades == 2)
            {
                return false;
            }
        }

        //defense upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 117)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades > 0)
            {
                return false;
            }
        }

        //defense upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 118)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().defenseUpgrades == 2)
            {
                return false;
            }
        }

        //ap upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 119)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades > 0)
            {
                return false;
            }
        }

        //ap upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 120)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().arcanePowerUpgrades == 2)
            {
                return false;
            }
        }

        //resistance upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 121)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades > 0)
            {
                return false;
            }
        }

        //resistance upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 122)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().resistanceUpgrades == 2)
            {
                return false;
            }
        }

        //influence upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 123)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades > 0)
            {
                return false;
            }
        }

        //influence upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 124)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().influenceUpgrades == 2)
            {
                return false;
            }
        }

        //mechanics upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 125)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades > 0)
            {
                return false;
            }
        }

        //mechanics upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 126)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().mechanicsUpgrades == 2)
            {
                return false;
            }
        }

        //digging upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 127)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades > 0)
            {
                return false;
            }
        }

        //digging upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 128)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().diggingUpgrades == 2)
            {
                return false;
            }
        }

        //lore upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 129)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades > 0)
            {
                return false;
            }
        }

        //lore upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 130)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().loreUpgrades == 2)
            {
                return false;
            }
        }

        //observe upgrade test 1 (cannot get upgrade 1, if you alrdy have it)
        if (numberInDeck == 131)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades > 0)
            {
                return false;
            }
        }

        //observe upgrade test 2 (cannot get upgrade 2, if you alrdy have it, or dont have upgrade 1)
        if (numberInDeck == 132)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades == 0 ||
                GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().observeUpgrades == 2)
            {
                return false;
            }
        }

        //power attack 2 test (cannot get upgrade 2, if you dont have regular power attack, or have power attack 3)
        if (numberInDeck == 133)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 98, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //power attack 3 test (cannot get upgrade 3, if you dont have regular power attack, or have power attack 3)
        if (numberInDeck == 134)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 98, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //precise strike 2 test 
        if (numberInDeck == 135)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 99, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //precise strike 3 test
        if (numberInDeck == 136)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 99, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //arcane barrage 2 test
        if (numberInDeck == 137)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 102, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 102, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //arcane barrage 3 test
        if (numberInDeck == 231)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 102, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 102, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //arcane orb 2 test
        if (numberInDeck == 138)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 103, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 103, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //arcane orb 3 test
        if (numberInDeck == 303)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 103, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 103, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //bombmaking 2 test
        if (numberInDeck == 139)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 106, 1) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 106, 1);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        //dodge 2 test
        if (numberInDeck == 140)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 108, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //dodge 3 test
        if (numberInDeck == 141)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 108, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //block 2 test
        if (numberInDeck == 142)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 109, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //block 3 test
        if (numberInDeck == 143)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 109, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //arcane ward 2 test
        if (numberInDeck == 144)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 110, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //arcane ward 3 test
        if (numberInDeck == 304)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 110, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //greater healing test (need basic healing, unused)
        if (numberInDeck == 145)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 24, 1) == 0)
            {
                return false;
            }
        }

        //healing 2 test
        if (numberInDeck == 146)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 24, 1) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 24, 1);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //healing 3 test
        if (numberInDeck == 147)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 24, 1) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 24, 1);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //extra strike 2 test
        if (numberInDeck == 149)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 148, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 148, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //extra strike 3 test
        if (numberInDeck == 150)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 148, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 148, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //offering 2 test
        if (numberInDeck == 172)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 107, 1) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 107, 1);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        //distilling 2 test
        if (numberInDeck == 174)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 166, 1) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 166, 1);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        //special case for culinarist & meditation (wont appear for sentinel)
        if (numberInDeck == 156 || numberInDeck == 175)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 11, 2) > 0)
            {
                return false;
            }
        }

        //regrowth 2 test
        if (numberInDeck == 183)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 178, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 178, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        //inner energy 2 test
        if (numberInDeck == 184)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 179, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 179, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        //shield of isolore 2 test
        if (numberInDeck == 229)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 111, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //shield of isolore 3 test
        if (numberInDeck == 230)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 111, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 1 || cardToCheck.GetComponent<Card>().cardLevel == 3)
                {
                    return false;
                }
            }
        }

        //smoke bomb 2 test
        if (numberInDeck == 232)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 152, 2) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 152, 2);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        //overcharge 2 test
        if (numberInDeck == 305)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, 180, 1) == 0)
            {
                return false;
            }
            else
            {
                //check card upgrade level
                GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 180, 1);

                if (cardToCheck.GetComponent<Card>().cardLevel == 2)
                {
                    return false;
                }
            }
        }

        return true;
    }

    //returns false, if you dont have the card
    public bool CheckIfHasCard(int numberInDeck)
    {
        if (generalDeck[numberInDeck].GetComponent<Card>().cardType == 1)
        {
            if(CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 1) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 8) > 0 ||
                CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 9) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 10) > 0)
            {
                return true;
            }
        }

        else if (generalDeck[numberInDeck].GetComponent<Card>().cardType == 2)
        {
            if (CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 2) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 8) > 0 ||
                CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 9) > 0 || CardHandler.ins.CheckQuantity(GameManager.ins.turnNumber, numberInDeck, 10) > 0)
            {
                return true;
            }
        }

        return false;
    }

    //clears whole upgrade offer
    public void ClearUpgradeOffer()
    {
        int upgradeCardCount = GameManager.ins.levelupCardArea.transform.childCount;

        for (int i = upgradeCardCount; i > 0; i--)
        {
            Destroy(GameManager.ins.levelupCardArea.transform.GetChild(i-1).gameObject);
        }
    }

    public void UpdateUpgradeOfferTooltips()
    {
        int upgradeCardCount = GameManager.ins.levelupCardArea.transform.childCount;

        for (int i = 0; i < upgradeCardCount; i++)
        {
            GameManager.ins.levelupCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay2>().UpdateTooltip2(1);
        }
    }

    public void CardLevelUpgrade(int numberInDeck)
    {
        //power attack 2 upgrade
        if(numberInDeck == 133)
        {
            int holderType = generalDeck[98].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10f;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Power Strike 2</b><br><color=#FFD370>Warrior Attack Ability</color> <sprite index=11><br>Attack the enemy using your <sprite index=4> values with 50% increased damage modifier. Increased focus. Reduced Cooldown.";
        }

        //power attack 3 upgrade
        if (numberInDeck == 134)
        {
            int holderType = generalDeck[98].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 98, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5f;
            cardToCheck.GetComponent<Card>().requiresEnergy = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Power Strike 3</b><br><color=#FFD370>Warrior Attack Ability</color> <sprite index=11><sprite index=11><br>Attack the enemy using your <sprite index=4> values with 50% increased damage modifier. Increased focus, dice count & energy cost. Fast Cooldown.";
        }

        //precise attack 2 upgrade
        if (numberInDeck == 135)
        {
            int holderType = generalDeck[98].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10f;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Precise Strike 2</b><br><color=#FFD370>Warrior Attack Ability</color> <sprite index=11><br>Attack the enemy using your <sprite index=4> values with +2<sprite index=4> modifier. Increased focus. Reduced Cooldown.";
        }

        //precise attack 3 upgrade
        if (numberInDeck == 136)
        {
            int holderType = generalDeck[98].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 99, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5;
            cardToCheck.GetComponent<Card>().requiresEnergy = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Precise Strike 3</b><br><color=#FFD370>Warrior Attack Ability</color> <sprite index=11><sprite index=11><br>Attack the enemy using your <sprite index=4> values with +2<sprite index=4> modifier. Increased focus, dice count & energy cost. Fast Cooldown.";
        }

        //arcane barrage 2 upgrade
        if (numberInDeck == 137)
        {
            int holderType = generalDeck[102].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 102, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Barrage 2</b><br><color=#FFD370>Arcanist Ranged Attack Ability</color> <sprite index=11><br>Attack the enemy using your <sprite index=9> values with +2<sprite index=9> modifier. Increased focus. Reduced cooldown.";
        }

        //arcane barrage 3 upgrade
        if (numberInDeck == 231)
        {
            int holderType = generalDeck[102].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 102, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5;
            cardToCheck.GetComponent<Card>().requiresEnergy = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Barrage 3</b><br><color=#FFD370>Arcanist Ranged Attack Ability</color> <sprite index=11><sprite index=11><br>Attack the enemy using your <sprite index=9> values with +2<sprite index=9> modifier. Increased focus, dice count & energy cost. Fast cooldown.";
        }

        //arcane orb 2 upgrade
        if (numberInDeck == 138)
        {
            int holderType = generalDeck[103].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 103, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Orb 2</b><br><color=#FFD370>Arcanist Ranged Attack Ability</color> <sprite index=11><sprite index=11><br>Attack the enemy using your <sprite index=9> values with 100% increased damage modifier. Increased focus. Reduced cooldown.";
        }

        //arcane orb 3 upgrade
        if (numberInDeck == 303)
        {
            int holderType = generalDeck[103].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 103, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5;
            cardToCheck.GetComponent<Card>().requiresEnergy = 3;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Orb 3</b><br><color=#FFD370>Arcanist Ranged Attack Ability</color> <sprite index=11><sprite index=11><sprite index=11><br>Attack the enemy using your <sprite index=9> values with 100% increased damage modifier. Increased focus & dice count. Fast cooldown.";
        }

        //bombmaking 2 upgrade
        if (numberInDeck == 139)
        {
            int holderType = generalDeck[106].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 106, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Bombmaking 2</b><br><color=#FFD370>Artisan Usable Ability</color> <sprite index=32> 5<sprite index=13><br>You know how to craft bombs out of base materials. Use to gain 2<sprite=\"bombs\" index=0>.";
        }

        //dodge 2 upgrade
        if (numberInDeck == 140)
        {
            int holderType = generalDeck[108].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Dodge 2</b><br><color=#FFD370>General Defense Ability</color> <sprite index=11><br>Defend against the attack using your <sprite=\"sprites v88\" index=18> or <sprite=\"sprites v88\" index=19> values and +2 modifier. Increased focus. Reduced cooldown.";
        }

        //dodge 3 upgrade
        if (numberInDeck == 141)
        {
            int holderType = generalDeck[108].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 108, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5;
            cardToCheck.GetComponent<Card>().requiresEnergy = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Dodge 3</b><br><color=#FFD370>General Defense Ability</color> <sprite index=11><sprite index=11><br>Defend against the attack using your <sprite=\"sprites v88\" index=18> or <sprite=\"sprites v88\" index=19> values and +2 modifier. Increased focus, dice count & energy cost. Fast cooldown.";
        }

        //block 2 upgrade
        if (numberInDeck == 142)
        {
            int holderType = generalDeck[109].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, holderType);
            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            //cardToCheck.GetComponent<Card>().requiresEnergy = 1;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Supershield 2</b><br><color=#FFD370>Warrior Defense Ability</color> <sprite index=11><sprite index=11><br>Defend against the attack using your <sprite=\"sprites v88\" index=18> or <sprite=\"sprites v88\" index=19> values and additional 50% damage reduction. Increased focus. Reduced cooldown.";
        }

        //block 3 upgrade
        if (numberInDeck == 143)
        {
            int holderType = generalDeck[109].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 109, holderType);
            //cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5;

            //cardToCheck.GetComponent<Card>().requiresEnergy = 1;

            //cardToCheck.GetComponent<CardDisplay2>().maxCooldown = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Supershield 3</b><br><color=#FFD370>Warrior Defense Ability</color> <sprite index=11><sprite index=11><br>Defend against the attack using your <sprite=\"sprites v88\" index=18> or <sprite=\"sprites v88\" index=19> values and additional 50% damage reduction. Increased focus & dice count. Reduced cooldown.";
        }

        //arcane ward 2 upgrade
        if (numberInDeck == 144)
        {
            int holderType = generalDeck[110].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Ward 2</b><br><color=#FFD370>Arcanist Defense Ability</color> <sprite index=11><br>Defend against attack with +1<sprite=\"sprites v88\" index=19> +1<sprite=\"sprites v88\" index=18> and 25% damage reduction. Increased focus. Reduced cooldown.";
        }

        //arcane ward 3 upgrade
        if (numberInDeck == 304)
        {
            int holderType = generalDeck[110].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 110, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5;

            cardToCheck.GetComponent<Card>().requiresEnergy = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Arcane Ward 3</b><br><color=#FFD370>Arcanist Defense Ability</color> <sprite index=11><sprite index=11><br>Defend against attack with +1<sprite=\"sprites v88\" index=19> +1<sprite=\"sprites v88\" index=18> and 25% damage reduction. Increased focus & dice count. Fast cooldown.";
        }

        //healing 2 upgrade
        if (numberInDeck == 146)
        {
            int holderType = generalDeck[24].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 24, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Healing 2</b><br><color=#FFD370>Cleric Usable Ability</color> <sprite index=12><br>You have the gift of healing. Use to regain 4<sprite=\"sprites v92\" index=3>. Also removes 4 stacks of poison. Can be used as additional action in combat.";
        }

        //healing 3 upgrade
        if (numberInDeck == 147)
        {
            int holderType = generalDeck[24].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 24, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Healing 3</b><br><color=#FFD370>Cleric Usable Ability</color> <sprite index=12><br>You have the gift of healing. Use to regain 5<sprite=\"sprites v92\" index=3>. Also removes 5 stacks of poison. Can be used as additional action in combat.";
        }

        //extra strike 2 upgrade
        if (numberInDeck == 149)
        {
            int holderType = generalDeck[148].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 148, holderType);
            //cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            cardToCheck.GetComponent<Card>().requiresEnergy = 1;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Extra Strike 2</b><br><color=#FFD370>Warrior Attack Ability</color> <sprite index=11><br>Attack the enemy using your <sprite index=4> values as an additional attack action.";
        }

        //extra strike 3 upgrade
        if (numberInDeck == 150)
        {
            int holderType = generalDeck[148].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 148, holderType);
            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10;

            //cardToCheck.GetComponent<Card>().requiresEnergy = 1;
            //cardToCheck.GetComponent<CardDisplay2>().maxCooldown = 2;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Extra Strike 3</b><br><color=#FFD370>Warrior Attack Ability</color> <sprite index=11><br>Attack the enemy using your <sprite index=4> values as an additional attack action. Reduced cooldown.";
        }

        //offering 2 upgrade
        if (numberInDeck == 172)
        {
            int holderType = generalDeck[107].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 107, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Offering 2</b><br><color=#FFD370>Cleric Usable Ability</color> <sprite index=32> 7<sprite index=13><br>You know the ritual of sacrifice. Use to gain 2<sprite index=12>.";
        }

        //distilling 2 upgrade
        if (numberInDeck == 174)
        {
            int holderType = generalDeck[166].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 166, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Distilling 2</b><br><color=#FFD370>Artisan Usable Ability</color> <sprite index=32><br>You know how to make potent drinks from biological material. Use to turn 1 nourishing meal into 2 rejuvenation potions.";
        }

        //regrowth 2 upgrade
        if (numberInDeck == 183)
        {
            int holderType = generalDeck[178].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 178, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Regrowth 2</b><br><color=#FFD370>General Passive Ability</color><br>You have trollish regenerative powers. You gain 30% <sprite=\"sprites v93\" index=1>.";
        }

        //inner energy 2 upgrade
        if (numberInDeck == 184)
        {
            int holderType = generalDeck[179].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 179, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Inner Energy 2</b><br><color=#FFD370>General Passive Ability</color><br>You are a living dynamo. You gain 30% <sprite=\"sprites v93\" index=0>.";
        }

        //shield of isolore 2 upgrade
        if (numberInDeck == 229)
        {
            int holderType = generalDeck[111].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 10f;

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Shield of Isolore 2</b><br><color=#FFD370>Cleric Defense Ability</color> <sprite index=12><br>Use to entirely negate next incoming attack. Reduced Cooldown.";
        }

        //shield of isolore 3 upgrade
        if (numberInDeck == 230)
        {
            int holderType = generalDeck[111].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 111, holderType);

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5f;

            cardToCheck.GetComponent<Card>().cardLevel = 3;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Shield of Isolore 3</b><br><color=#FFD370>Cleric Defense Ability</color> <sprite index=12><br>Use to entirely negate next incoming attack. Fast Cooldown.";
        }

        //smoke bomb 2 upgrade
        if (numberInDeck == 232)
        {
            int holderType = generalDeck[152].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 152, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Smoke Bomb 2</b><br><color=#FFD370>Artisan Defense Ability</color> <sprite=\"bombs\" index=0><br>Veil yourself in thick smoke. Gain +1<sprite=\"sprites v88\" index=18>, +1<sprite=\"sprites v88\" index=19>, 10%<sprite=\"sprites v88\" index=18> modifier and 10%<sprite=\"sprites v88\" index=19> modifier for 3<sprite index=35>. You can also flee combat for no <sprite index=11> cost, and are immune to Gaze attacks. Can be used as additional action in combat.";
        }

        //overcharge 2 upgrade
        if (numberInDeck == 305)
        {
            int holderType = generalDeck[180].GetComponent<Card>().cardType;

            //check card upgrade level
            GameObject cardToCheck = CardHandler.ins.CopyCard(GameManager.ins.turnNumber, 180, holderType);

            cardToCheck.GetComponent<Card>().cardLevel = 2;

            cardToCheck.GetComponent<CardDisplay2>().realTimeMaxCooldown = 5f;

            cardToCheck.GetComponent<CardDisplay2>().tooltipText = "<b>Overcharge 2</b><br><color=#FFD370>General Usable Ability</color> <sprite=\"sprites v92\" index=3><sprite=\"sprites v92\" index=3><br>You exhaust yourself beyond normal limits, for a cost. Use to regain <sprite index=11><sprite index=11><sprite index=11>. Reduced cooldown in combat.";
        }
    }
}
