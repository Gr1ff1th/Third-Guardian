using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    public GameObject backgroundSlot1;
    public GameObject backgroundSlot2;
    public GameObject backgroundSlot3;

    public GameObject settingsDisplay;
    public GameObject backgroundDisplay;

    public GameObject backgroundCardArea;

    public int backgroundSlotChosen;
    public int cardChosen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OpenBackgroundOptions()
    {
        ShowBackgroundOptions(true);

        ClearBackGroundOffer();

        Invoke("DrawBackgroundOffer", 0.15f);

        //DrawBackgroundOffer();
    }

    //deletes the background offer
    public void ClearBackGroundOffer()
    {
        int upgradeCardCount = backgroundCardArea.transform.childCount;

        for (int i = upgradeCardCount; i > 0; i--)
        {
            Destroy(backgroundCardArea.transform.GetChild(i - 1).gameObject);
        }
    }

    public void DrawBackgroundOffer()
    {
        for (int i = 0; i < SelectorScript2.ins.cardSaveHandler.generalDeck.Count; i++)
        {
            if(SelectorScript2.ins.cardSaveHandler.generalDeck[i].GetComponent<Card>().cardType == 14)
            {
                //dont draw empty card here
                if (SelectorScript2.ins.cardSaveHandler.generalDeck[i].GetComponent<Card>().numberInDeck != 260)
                {
                    PlaceBackGroundCardOnOffer(i);
                }
            }
        }
    }

    //here could be various checks to see if the card is enabled or not
    public void PlaceBackGroundCardOnOffer(int cardNumber)
    {
        GameObject playerCard = Instantiate(SelectorScript2.ins.cardSaveHandler.generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.transform.SetParent(backgroundCardArea.transform, false);
        playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(3);

        playerCard.GetComponent<CardDisplay2>().isEnabled = true;

        //advancement cards
        if(cardNumber == 261 || cardNumber == 262 || cardNumber == 263 || cardNumber == 264)
        {
            if(CheckBackgroundSlotsForCard(261) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBackgroundSlotsForCard(262) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBackgroundSlotsForCard(263) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBackgroundSlotsForCard(264) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //wealth cards
        if (cardNumber == 265 || cardNumber == 266)
        {
            if (CheckBackgroundSlotsForCard(265) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBackgroundSlotsForCard(266) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //warrior
        //need to check base cards too
        if (cardNumber == 267)
        {
            if (CheckBackgroundSlotsForCard(267) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBaseCards(0) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }
        //artisan
        if (cardNumber == 268)
        {
            if (CheckBackgroundSlotsForCard(268) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBaseCards(1) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }
        //arcanist
        if (cardNumber == 269)
        {
            if (CheckBackgroundSlotsForCard(269) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBaseCards(2) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }
        //cleric
        if (cardNumber == 270)
        {
            if (CheckBackgroundSlotsForCard(270) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
            if (CheckBaseCards(101) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }

        //rest of the cards
        if (cardNumber > 270 && cardNumber < 286)
        {
            if (CheckBackgroundSlotsForCard(cardNumber) == true)
            {
                playerCard.GetComponent<CardDisplay2>().isEnabled = false;
            }
        }
    }

    public bool CheckBackgroundSlotsForCard(int cardNumber)
    {
        if(backgroundSlot1.transform.GetChild(0).GetComponent<Card>().numberInDeck == cardNumber)
        {
            return true;
        }
        if (backgroundSlot2.transform.GetChild(0).GetComponent<Card>().numberInDeck == cardNumber)
        {
            return true;
        }
        if (backgroundSlot3.transform.GetChild(0).GetComponent<Card>().numberInDeck == cardNumber)
        {
            return true;
        }

        return false;
    }

    //checks for specific card in the base card tray
    public bool CheckBaseCards(int cardNumber)
    {
        for (int i = 0; i < SelectorScript2.ins.perkCardArea.transform.childCount; i++)
        {
            if (SelectorScript2.ins.perkCardArea.transform.GetChild(i).GetComponent<Card>().numberInDeck == cardNumber)
            {
                return true;
            }
        }
        return false;
    }

    public void ShowBackgroundOptions(bool showBackgrounds)
    {
        if(showBackgrounds == true)
        {
            settingsDisplay.SetActive(false);
            backgroundDisplay.SetActive(true);
        }

        if (showBackgrounds == false)
        {
            settingsDisplay.SetActive(true);
            backgroundDisplay.SetActive(false);
        }
    }

    public void ResetBackGrounds()
    {
        DeleteBackground(1);
        DeleteBackground(2);
        DeleteBackground(3);

        Invoke("DrawEmptyBackgrounds", 0.1f);
    }

    public void DrawEmptyBackgrounds()
    {
        DrawBackgroundCardToSlot(1, 260);
        DrawBackgroundCardToSlot(2, 260);
        DrawBackgroundCardToSlot(3, 260);
    }

    public void DrawBackgroundCardToSlot(int slot, int cardNumber)
    {
        //instantiates random quest card from the deck
        GameObject playerCard = Instantiate(gameObject.GetComponent<SelectorScript2>().cardSaveHandler.generalDeck[cardNumber], new Vector3(0, 0, 0), Quaternion.identity);

        //places it in specific slot
        if (slot == 1)
        {
            playerCard.transform.SetParent(backgroundSlot1.transform, false);
        }
        if (slot == 2)
        {
            playerCard.transform.SetParent(backgroundSlot2.transform, false);
        }
        if (slot == 3)
        {
            playerCard.transform.SetParent(backgroundSlot3.transform, false);
        }

        //turns the card inactive
        playerCard.SetActive(true);

        playerCard.GetComponent<CardDisplay2>().isEnabled = true;

        playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(2);

        //different tooltip for non-empty cards
        if(cardNumber != 260)
        {
            playerCard.GetComponent<CardDisplay2>().UpdateTooltip2(4);
        }

        Invoke("SetScoreWithDelay", 0.1f);
    }

    public void SetScoreWithDelay()
    {
        SelectorScript2.ins.SetScore();
    }

    public void DeleteBackground(int slot)
    {
        if(slot == 1)
        {
            if(backgroundSlot1.transform.childCount > 1)
            {
                Destroy(backgroundSlot1.transform.GetChild(1).gameObject);
            }
            if (backgroundSlot1.transform.childCount > 0)
            {
                Destroy(backgroundSlot1.transform.GetChild(0).gameObject);
            }
        }

        if (slot == 2)
        {
            if (backgroundSlot2.transform.childCount > 1)
            {
                Destroy(backgroundSlot2.transform.GetChild(1).gameObject);
            }
            if (backgroundSlot2.transform.childCount > 0)
            {
                Destroy(backgroundSlot2.transform.GetChild(0).gameObject);
            }
        }

        if (slot == 3)
        {
            if (backgroundSlot3.transform.childCount > 1)
            {
                Destroy(backgroundSlot3.transform.GetChild(1).gameObject);
            }
            if (backgroundSlot3.transform.childCount > 0)
            {
                Destroy(backgroundSlot3.transform.GetChild(0).gameObject);
            }
        }
    }

    //draws card to selected slot
    public void DrawBackGroundCard()
    {
        ShowBackgroundOptions(false);

        DeleteBackground(backgroundSlotChosen);

        Invoke("DrawSelectedCardToSlotWithDelay", 0.1f);
    }

    public void DrawSelectedCardToSlotWithDelay()
    {
        DrawBackgroundCardToSlot(backgroundSlotChosen, cardChosen);
    }

    public void ResetBackgroundCard()
    {
        DeleteBackground(backgroundSlotChosen);

        Invoke("DrawEmptyCardToSlotWithDelay", 0.1f);
    }

    public void DrawEmptyCardToSlotWithDelay()
    {
        DrawBackgroundCardToSlot(backgroundSlotChosen, 260);
    }
}
