using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//stores shop offers
public class StoreHandler : MonoBehaviour
{
    //singleton
    public static StoreHandler ins;

    public PhotonView PV;

    //used for buying & selling
    public GameObject storeCardArea;

    //new randomized system for v93
    //offer says which cards can spawn, probabilities gives chance, quantity tells total amount of cards drawn
    public List<GameObject> wilforgeStage1Offer;
    public int[] wilforgeStage1Probabilities;
    public int wilforgeStage1DrawQuantity;
    public List<GameObject> wilforgeStage2Offer;
    public int[] wilforgeStage2Probabilities;
    public int wilforgeStage2DrawQuantity;
    public List<GameObject> wilforgeStage3Offer;
    public int[] wilforgeStage3Probabilities;
    public int wilforgeStage3DrawQuantity;

    public List<GameObject> smithyStage1Offer;
    public int[] smithyStage1Probabilities;
    public int smithyStage1DrawQuantity;
    public List<GameObject> smithyStage2Offer;
    public int[] smithyStage2Probabilities;
    public int smithyStage2DrawQuantity;
    public List<GameObject> smithyStage3Offer;
    public int[] smithyStage3Probabilities;
    public int smithyStage3DrawQuantity;

    public List<GameObject> innStage1Offer;
    public int[] innStage1Probabilities;
    public int innStage1DrawQuantity;
    public List<GameObject> innStage2Offer;
    public int[] innStage2Probabilities;
    public int innStage2DrawQuantity;
    public List<GameObject> innStage3Offer;
    public int[] innStage3Probabilities;
    public int innStage3DrawQuantity;

    public List<GameObject> factoryStage1Offer;
    public int[] factoryStage1Probabilities;
    public int factoryStage1DrawQuantity;
    public List<GameObject> factoryStage2Offer;
    public int[] factoryStage2Probabilities;
    public int factoryStage2DrawQuantity;
    public List<GameObject> factoryStage3Offer;
    public int[] factoryStage3Probabilities;
    public int factoryStage3DrawQuantity;

    public List<GameObject> templeStage1Offer;
    public int[] templeStage1Probabilities;
    public int templeStage1DrawQuantity;
    public List<GameObject> templeStage2Offer;
    public int[] templeStage2Probabilities;
    public int templeStage2DrawQuantity;
    public List<GameObject> templeStage3Offer;
    public int[] templeStage3Probabilities;
    public int templeStage3DrawQuantity;

    public List<GameObject> covenStage1Offer;
    public int[] covenStage1Probabilities;
    public int covenStage1DrawQuantity;
    public List<GameObject> covenStage2Offer;
    public int[] covenStage2Probabilities;
    public int covenStage2DrawQuantity;
    public List<GameObject> covenStage3Offer;
    public int[] covenStage3Probabilities;
    public int covenStage3DrawQuantity;

    public List<GameObject> guildhouseStage1Offer;
    public int[] guildhouseStage1Probabilities;
    public int guildhouseStage1DrawQuantity;
    public List<GameObject> guildhouseStage2Offer;
    public int[] guildhouseStage2Probabilities;
    public int guildhouseStage2DrawQuantity;
    public List<GameObject> guildhouseStage3Offer;
    public int[] guildhouseStage3Probabilities;
    public int guildhouseStage3DrawQuantity;

    public List<GameObject> cornvilleStage1Offer;
    public int[] cornvilleStage1Probabilities;
    public int cornvilleStage1DrawQuantity;
    public List<GameObject> cornvilleStage2Offer;
    public int[] cornvilleStage2Probabilities;
    public int cornvilleStage2DrawQuantity;
    public List<GameObject> cornvilleStage3Offer;
    public int[] cornvilleStage3Probabilities;
    public int cornvilleStage3DrawQuantity;

    /* old system
    //for spawning Wilforge offers
    public int[] wilforgeStage1Card;
    public int[] wilforgeStage1Qty;
    public int[] wilforgeStage2Card;
    public int[] wilforgeStage2Qty;
    public int[] wilforgeStage3Card;
    public int[] wilforgeStage3Qty;

    //for spawning smithy offers
    public int[] smithyStage1Card;
    public int[] smithyStage1Qty;
    public int[] smithyStage2Card;
    public int[] smithyStage2Qty;
    public int[] smithyStage3Card;
    public int[] smithyStage3Qty;

    //for spawning inn offers
    public int[] innStage1Card;
    public int[] innStage1Qty;
    public int[] innStage2Card;
    public int[] innStage2Qty;
    public int[] innStage3Card;
    public int[] innStage3Qty;

    //for spawning factory offers
    public int[] factoryStage1Card;
    public int[] factoryStage1Qty;
    public int[] factoryStage2Card;
    public int[] factoryStage2Qty;
    public int[] factoryStage3Card;
    public int[] factoryStage3Qty;

    //for spawning temple offers
    public int[] templeStage1Card;
    public int[] templeStage1Qty;
    public int[] templeStage2Card;
    public int[] templeStage2Qty;
    public int[] templeStage3Card;
    public int[] templeStage3Qty;

    //for spawning coven offers
    public int[] covenStage1Card;
    public int[] covenStage1Qty;
    public int[] covenStage2Card;
    public int[] covenStage2Qty;
    public int[] covenStage3Card;
    public int[] covenStage3Qty;

    //for spawning guildhouse offers
    public int[] guildhouseStage1Card;
    public int[] guildhouseStage1Qty;
    public int[] guildhouseStage2Card;
    public int[] guildhouseStage2Qty;
    public int[] guildhouseStage3Card;
    public int[] guildhouseStage3Qty;
    */

    /*
    public List<GameObject> willforgePhase1;
    public List<GameObject> willforgePhase2;
    public List<GameObject> willforgePhase3;
    */
    public List<GameObject> wilforgeShop;
    public List<GameObject> smithyShop;
    public List<GameObject> innShop;
    public List<GameObject> factoryShop;
    public List<GameObject> templeShop;
    public List<GameObject> covenShop;
    public List<GameObject> guildhouseShop;
    public List<GameObject> cornvilleShop;


    // Start is called before the first frame update
    void Start()
    {
        // very bad singleton
        ins = this;

        //get the reference
        PV = GetComponent<PhotonView>();
    }

    //draws all stage 1 shop cards
    public void DrawStage1Cards()
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_DrawStage1Cards", RpcTarget.AllBufferedViaServer);
    }


    //draws cards to the shops
    //shoptype 1= wilforge
    [PunRPC]
    void RPC_DrawStage1Cards()
    {
        //wilforge
        for(int i = 0; i < wilforgeStage1DrawQuantity; i++)
        {
            RollShopCards(37, 1);
        }

        //smithy
        for (int i = 0; i < smithyStage1DrawQuantity; i++)
        {
            RollShopCards(28, 1);
        }

        //inn
        for (int i = 0; i < innStage1DrawQuantity; i++)
        {
            RollShopCards(31, 1);
        }

        //special case for sextant
        DrawShopCards(216, 31, 1);
        //also draw the mike stuff for now, except bridge pass
        //axe, staff, chainmail, shovel, toolbox, horse, ensiocs tome
        DrawShopCards(87, 31, 1);
        DrawShopCards(34, 31, 1);
        DrawShopCards(37, 31, 1);
        DrawShopCards(79, 31, 1);
        DrawShopCards(76, 31, 1);
        DrawShopCards(63, 31, 1);
        DrawShopCards(72, 31, 1);

        //factory
        for (int i = 0; i < factoryStage1DrawQuantity; i++)
        {
            RollShopCards(44, 1);
        }

        //special case for divers mask
        DrawShopCards(193, 44, 1);

        //temple
        for (int i = 0; i < templeStage1DrawQuantity; i++)
        {
            RollShopCards(47, 1);
        }

        //draw some blessed oils & rej. pots by default
        DrawShopCards(114, 47, 5);
        DrawShopCards(19, 47, 3);

        //sword, staff of striking, chainmail, robes of res, ensiocs tome
        DrawShopCards(30, 47, 1);
        DrawShopCards(91, 47, 1);
        DrawShopCards(37, 47, 1);
        DrawShopCards(42, 47, 1);
        DrawShopCards(72, 47, 1);

        //coven
        for (int i = 0; i < covenStage1DrawQuantity; i++)
        {
            RollShopCards(16, 1);
        }

        //potion of power
        DrawShopCards(298, 16, 1);

        //guildhouse
        for (int i = 0; i < guildhouseStage1DrawQuantity; i++)
        {
            RollShopCards(11, 1);
        }

        //special case for shard
        //DrawShopCards(294, 11, 1);

        //cornville
        for (int i = 0; i < cornvilleStage1DrawQuantity; i++)
        {
            RollShopCards(1, 1);
        }

        //draw some basic equipment for cornville
        //sword, staff, chainmail, shovel, toolbox, horse, ensiocs tome, bridge pass
        DrawShopCards(30, 1, 1);
        DrawShopCards(34, 1, 1);
        DrawShopCards(37, 1, 1);
        DrawShopCards(79, 1, 1);
        DrawShopCards(76, 1, 1);
        DrawShopCards(63, 1, 1);
        DrawShopCards(72, 1, 1);
        DrawShopCards(292, 1, 1);
    }

    //draws all stage 2 shop cards
    public void DrawStage2Cards()
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_DrawStage2Cards", RpcTarget.AllBufferedViaServer);
    }


    //draws cards to the shops
    //shoptype 1= wilforge
    [PunRPC]
    void RPC_DrawStage2Cards()
    {
        //wilforge
        for (int i = 0; i < wilforgeStage2DrawQuantity; i++)
        {
            RollShopCards(37, 2);
        }

        //smithy
        for (int i = 0; i < smithyStage2DrawQuantity; i++)
        {
            RollShopCards(28, 2);
        }

        //inn
        for (int i = 0; i < innStage2DrawQuantity; i++)
        {
            RollShopCards(31, 2);
        }

        //factory
        for (int i = 0; i < factoryStage2DrawQuantity; i++)
        {
            RollShopCards(44, 2);
        }

        //temple
        for (int i = 0; i < templeStage2DrawQuantity; i++)
        {
            RollShopCards(47, 2);
        }

        //coven
        for (int i = 0; i < covenStage2DrawQuantity; i++)
        {
            RollShopCards(16, 2);
        }

        //guildhouse
        for (int i = 0; i < guildhouseStage2DrawQuantity; i++)
        {
            RollShopCards(11, 2);
        }

        //cornville
        for (int i = 0; i < cornvilleStage2DrawQuantity; i++)
        {
            RollShopCards(1, 2);
        }
    }

    //draws all stage 3 shop cards
    public void DrawStage3Cards()
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_DrawStage3Cards", RpcTarget.AllBufferedViaServer);
    }


    //draws cards to the shops
    //shoptype 1= wilforge
    [PunRPC]
    void RPC_DrawStage3Cards()
    {
        //wilforge
        for (int i = 0; i < wilforgeStage3DrawQuantity; i++)
        {
            RollShopCards(37, 3);
        }

        //special case for kingsbreed horse & potion of power
        DrawShopCards(69, 37, 1);
        DrawShopCards(298, 37, 1);

        //smithy
        for (int i = 0; i < smithyStage3DrawQuantity; i++)
        {
            RollShopCards(28, 3);
        }

        //special case for gorms hammer & paladins plate
        DrawShopCards(245, 28, 1);
        DrawShopCards(41, 28, 1);

        //inn
        for (int i = 0; i < innStage3DrawQuantity; i++)
        {
            RollShopCards(31, 3);
        }

        //factory
        for (int i = 0; i < factoryStage3DrawQuantity; i++)
        {
            RollShopCards(44, 3);
        }

        //special case for irinos toolbox
        DrawShopCards(78, 44, 1);

        //temple
        for (int i = 0; i < templeStage3DrawQuantity; i++)
        {
            RollShopCards(47, 3);
        }

        //special case for codex of isolore & holy avenger
        DrawShopCards(198, 47, 1);
        DrawShopCards(33, 47, 1);

        //coven
        for (int i = 0; i < covenStage3DrawQuantity; i++)
        {
            RollShopCards(16, 3);
        }

        //special case for staff of mastery & potion of power
        DrawShopCards(96, 16, 1);
        DrawShopCards(298, 16, 1);

        //guildhouse
        for (int i = 0; i < guildhouseStage3DrawQuantity; i++)
        {
            RollShopCards(11, 3);
        }

        //special case for teras codex
        DrawShopCards(75, 11, 1);

        //cornville
        for (int i = 0; i < cornvilleStage3DrawQuantity; i++)
        {
            RollShopCards(1, 3);
        }
    }

    //lets use the node numbers for numbering the stores
    public void DrawShopCards(int cardNumber, int storeType, int quantity)
    {
        //could use same code for all stores?
        //need to check if theres instances of this card yet
        //add quantity to the carddisplay if so
        if (IfHaveCard(cardNumber, storeType, quantity) == false)
        {
            //instantiates random quest card from the deck
            GameObject playerCard = Instantiate(CardHandler.ins.generalDeck[cardNumber]);

            //store it under the storehandler for now?
            playerCard.transform.SetParent(ins.transform, false);

            playerCard.GetComponent<CardDisplay2>().quantity = quantity;

            if (storeType == 37)
            {
                //add card to the wilforge shop
                wilforgeShop.Add(playerCard);
            }
            if (storeType == 28)
            {
                //smithy
                smithyShop.Add(playerCard);
            }
            if (storeType == 31)
            {
                //inn
                innShop.Add(playerCard);
            }
            if (storeType == 44)
            {
                //factory
                factoryShop.Add(playerCard);
            }
            if (storeType == 47)
            {
                //temple
                templeShop.Add(playerCard);
            }
            if (storeType == 16)
            {
                //coven
                covenShop.Add(playerCard);
            }
            if (storeType == 11)
            {
                //guildhouse
                guildhouseShop.Add(playerCard);
            }
            if (storeType == 1)
            {
                //guildhouse
                cornvilleShop.Add(playerCard);
            }

            //show quantity in certain conditions
            if (playerCard.GetComponent<CardDisplay2>().quantity > 1 ||
                playerCard.GetComponent<CardDisplay2>().showQuantityAlways == true)
            {
                playerCard.GetComponent<CardDisplay2>().quantityText.text =
                    playerCard.gameObject.GetComponent<CardDisplay2>().quantity.ToString();
            }
        }
    }

    //new system for v93
    //lets use the node numbers for numbering the stores, for now..
    public void RollShopCards(int storeType, int gameStage)
    {
        int i = 0;

        //wilforge
        if(storeType == 37)
        {
            //lets use given variable for now
            if(gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, wilforgeStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= wilforgeStage1Probabilities[randomCard])
                    {
                        DrawShopCards(wilforgeStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 37, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, wilforgeStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= wilforgeStage2Probabilities[randomCard])
                    {
                        DrawShopCards(wilforgeStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 37, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, wilforgeStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= wilforgeStage3Probabilities[randomCard])
                    {
                        DrawShopCards(wilforgeStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 37, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //smithy
        if (storeType == 28)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, smithyStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= smithyStage1Probabilities[randomCard])
                    {
                        DrawShopCards(smithyStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 28, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, smithyStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= smithyStage2Probabilities[randomCard])
                    {
                        DrawShopCards(smithyStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 28, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, smithyStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= smithyStage3Probabilities[randomCard])
                    {
                        DrawShopCards(smithyStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 28, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //inn
        if (storeType == 31)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, innStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= innStage1Probabilities[randomCard])
                    {
                        DrawShopCards(innStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 31, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, innStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= innStage2Probabilities[randomCard])
                    {
                        DrawShopCards(innStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 31, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, innStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= innStage3Probabilities[randomCard])
                    {
                        DrawShopCards(innStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 31, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //factory
        if (storeType == 44)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, factoryStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= factoryStage1Probabilities[randomCard])
                    {
                        DrawShopCards(factoryStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 44, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, factoryStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= factoryStage2Probabilities[randomCard])
                    {
                        DrawShopCards(factoryStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 44, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, factoryStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= factoryStage3Probabilities[randomCard])
                    {
                        DrawShopCards(factoryStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 44, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //temple
        if (storeType == 47)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, templeStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= templeStage1Probabilities[randomCard])
                    {
                        DrawShopCards(templeStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 47, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, templeStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= templeStage2Probabilities[randomCard])
                    {
                        DrawShopCards(templeStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 47, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, templeStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= templeStage3Probabilities[randomCard])
                    {
                        DrawShopCards(templeStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 47, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //coven
        if (storeType == 16)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, covenStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= covenStage1Probabilities[randomCard])
                    {
                        DrawShopCards(covenStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 16, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, covenStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= covenStage2Probabilities[randomCard])
                    {
                        DrawShopCards(covenStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 16, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, covenStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= covenStage3Probabilities[randomCard])
                    {
                        DrawShopCards(covenStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 16, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //guildhouse
        if (storeType == 11)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, guildhouseStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= guildhouseStage1Probabilities[randomCard])
                    {
                        DrawShopCards(guildhouseStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 11, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, guildhouseStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= guildhouseStage2Probabilities[randomCard])
                    {
                        DrawShopCards(guildhouseStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 11, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, guildhouseStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= guildhouseStage3Probabilities[randomCard])
                    {
                        DrawShopCards(guildhouseStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 11, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }

        //cornville
        if (storeType == 1)
        {
            //could use the gamestage variable for this (gamestage 2 is actually day 1)
            if (gameStage == 1)
            {
                do
                {
                    int randomCard = Random.Range(0, cornvilleStage1Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= cornvilleStage1Probabilities[randomCard])
                    {
                        DrawShopCards(cornvilleStage1Offer[randomCard].GetComponent<Card>().numberInDeck, 1, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 2)
            {
                do
                {
                    int randomCard = Random.Range(0, cornvilleStage2Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= cornvilleStage2Probabilities[randomCard])
                    {
                        DrawShopCards(cornvilleStage2Offer[randomCard].GetComponent<Card>().numberInDeck, 1, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }

            if (gameStage == 3)
            {
                do
                {
                    int randomCard = Random.Range(0, cornvilleStage3Offer.Count);
                    int randomRoll = Random.Range(1, 101);

                    //roll needs to be less or equal of the probability for it to take effect
                    if (randomRoll <= cornvilleStage3Probabilities[randomCard])
                    {
                        DrawShopCards(cornvilleStage3Offer[randomCard].GetComponent<Card>().numberInDeck, 1, 1);
                        //this doesnt draw anything if no card is found in 1000 rolls tho? should be good enough
                        return;
                    }

                    i++;
                }
                while (i < 1000);
            }
        }
    }

    //adds quantity to the card, if it exists alrdy
    public bool IfHaveCard(int cardNumber, int storeType, int quantity)
    {
        //wilforge
        if (storeType == 37)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < wilforgeShop.Count; i++)
            {
                if (wilforgeShop[i].GetComponent<Card>() != null)
                {
                    if (wilforgeShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        wilforgeShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (wilforgeShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            wilforgeShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            wilforgeShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                wilforgeShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //smithy
        if (storeType == 28)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < smithyShop.Count; i++)
            {
                if (smithyShop[i].GetComponent<Card>() != null)
                {
                    if (smithyShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        smithyShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (smithyShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            smithyShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            smithyShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                smithyShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //inn
        if (storeType == 31)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < innShop.Count; i++)
            {
                if (innShop[i].GetComponent<Card>() != null)
                {
                    if (innShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        innShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (innShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            innShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            innShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                innShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //factory
        if (storeType == 44)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < factoryShop.Count; i++)
            {
                if (factoryShop[i].GetComponent<Card>() != null)
                {
                    if (factoryShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        factoryShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (factoryShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            factoryShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            factoryShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                factoryShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //temple
        if (storeType == 47)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < templeShop.Count; i++)
            {
                if (templeShop[i].GetComponent<Card>() != null)
                {
                    if (templeShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        templeShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (templeShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            templeShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            templeShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                templeShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //coven
        if (storeType == 16)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < covenShop.Count; i++)
            {
                if (covenShop[i].GetComponent<Card>() != null)
                {
                    if (covenShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        covenShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (covenShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            covenShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            covenShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                covenShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        //guildhouse
        if (storeType == 11)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < guildhouseShop.Count; i++)
            {
                if (guildhouseShop[i].GetComponent<Card>() != null)
                {
                    if (guildhouseShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        guildhouseShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (guildhouseShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            guildhouseShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            guildhouseShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                guildhouseShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }
        //cornville
        if (storeType == 1)
        {
            //tests if player has passive of that effect number
            for (int i = 0; i < cornvilleShop.Count; i++)
            {
                if (cornvilleShop[i].GetComponent<Card>() != null)
                {
                    if (cornvilleShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                    {
                        cornvilleShop[i].GetComponent<CardDisplay2>().quantity += quantity;

                        //show quantity in certain conditions
                        if (cornvilleShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            cornvilleShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            cornvilleShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                cornvilleShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //when spawning store cards for all to see
    //v99 change shop checks from nodes to storenumbers
    public void ShowStoreCards()
    {
        //wilforge
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 37)//(GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().locationNode.GetComponent<Node>().nodeNumber == 37)
        {
            SetStoreOffer(37);
        }

        //smithy
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 28)
        {
            SetStoreOffer(28);
        }

        //inn
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 31)
        {
            SetStoreOffer(31);
        }

        //factory
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 44)
        {
            SetStoreOffer(44);
        }

        //temple
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 47)
        {
            SetStoreOffer(47);
        }

        //coven
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 16)
        {
            SetStoreOffer(16);
        }

        //guildhouse (updated for v91)
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 11)
        {
            //note that these numbers need to be changed for all new shops (to use location node number instead of standingOn etc)
            SetStoreOffer(11);
        }

        //cornville
        if (GameManager.ins.references.currentStrategicEncounter.storeNumber == 1)
        {
            SetStoreOffer(1);
        }
    }

    public void SetStoreOffer(int storeType)
    {
        //wilforge
        if(storeType == 37)
        {
            for(int i = 0; i < wilforgeShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(wilforgeShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //smithy
        if (storeType == 28)
        {
            for (int i = 0; i < smithyShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(smithyShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //inn
        if (storeType == 31)
        {
            for (int i = 0; i < innShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(innShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //factory
        if (storeType == 44)
        {
            for (int i = 0; i < factoryShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(factoryShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //temple
        if (storeType == 47)
        {
            for (int i = 0; i < templeShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(templeShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //coven
        if (storeType == 16)
        {
            for (int i = 0; i < covenShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(covenShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //guildhouse
        if (storeType == 11)
        {
            for (int i = 0; i < guildhouseShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(guildhouseShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
        //cornville
        if (storeType == 1)
        {
            for (int i = 0; i < cornvilleShop.Count; i++)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(cornvilleShop[i], new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
    }

    //when selling cards
    public void ShowSellableCards()
    {
        for (int i = 0; i < GameManager.ins.handCardArea.transform.childCount; i++)
        {
            //only show consumables from handcard area
            //or rather things that can be sold (since cardtype only tells which holder to use)
            if(GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == GameManager.ins.turnNumber &&
                GameManager.ins.handCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeSold == true)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(GameManager.ins.handCardArea.transform.GetChild(i).gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }

        //show sellable equipment also
        for (int i = 0; i < GameManager.ins.equipmentCardArea.transform.childCount; i++)
        {
            //only show consumables from handcard area
            if (GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().belongsTo == GameManager.ins.turnNumber &&
                GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject.GetComponent<Card>().canBeSold == true)
            {
                //instantiates random quest card from the list
                GameObject playerCard = Instantiate(GameManager.ins.equipmentCardArea.transform.GetChild(i).gameObject, new Vector3(0, 0, 0), Quaternion.identity);

                //place it in store card area
                playerCard.transform.SetParent(ins.storeCardArea.transform, false);

                playerCard.GetComponent<CardDisplay2>().UpdateTooltip();
            }
        }
    }

    //when buying card from offer
    public void BuyCards(int turnNumber, int cardNumber, int cardType)
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_BuyCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, cardType);
    }

    //or equipping gear
    [PunRPC]
    void RPC_BuyCards(int turnNumber, int cardNumber, int cardType)
    {
        //remove the original card from equipment display (or reduce the quantity)
        CardHandler.ins.ReduceQuantity(turnNumber, cardNumber, 6, 1);

        //reduces the quantity in deck
        //this needs to match the v91 nodenumber
        //changed for v99
        ReduceQuantityFromStoreList(GameManager.ins.references.currentStrategicEncounter.storeNumber, cardNumber);//(GameManager.ins.avatars[turnNumber].GetComponent<CharController>().locationNode.nodeNumber, cardNumber);

        //AddItemToSlot(turnNumber, cardNumber);
        //StartCoroutine(CardHandler.ins.AddItemDelay(turnNumber, cardNumber, 0.3f));
        //check new stats (do this on coroutine instead?)
        //GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().StatUpdate();

        //draw purchased card to player
        CardHandler.ins.DrawCards(turnNumber, cardNumber, cardType, 1);

        //GameManager.ins.uiButtonHandler.UpdateEquipmentTooltips();

        //allows purchasing more after delay
        StartCoroutine(CardHandler.ins.AllowEquipping(0.5f));

        //CardHandler.ins.cardChangeInProgress = false;
    }

    //when buying card from offer
    public void SellCards(int turnNumber, int cardNumber, int cardType)
    {
        //perhaps make turnnumber check here
        if (GameManager.ins.avatars[turnNumber].GetComponent<CharController>().ItsYourTurn() == false)
        {
            return;
        }

        PV.RPC("RPC_SellCards", RpcTarget.AllBufferedViaServer, turnNumber, cardNumber, cardType);
    }

    //or equipping gear
    [PunRPC]
    void RPC_SellCards(int turnNumber, int cardNumber, int cardType)
    {
        //remove the original card from equipment display (or reduce the quantity)
        CardHandler.ins.ReduceQuantity(turnNumber, cardNumber, 6, 1);

        //reduce quantity from player as well
        CardHandler.ins.ReduceQuantity(turnNumber, cardNumber, cardType, 1);

        //add card to the current shop
        ins.DrawShopCards(cardNumber, GameManager.ins.references.currentStrategicEncounter.storeNumber, 1);

        //allows purchasing more after delay
        StartCoroutine(CardHandler.ins.AllowEquipping(0.5f));
    }

    public void ReduceQuantityFromStoreList(int location, int cardNumber)
    {
        //wilforge
        if(location == 37)
        {
            for(int i = 0; i < wilforgeShop.Count; i++)
            {
                if (wilforgeShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (wilforgeShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        wilforgeShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (wilforgeShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            wilforgeShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            wilforgeShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                wilforgeShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        wilforgeShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //smithy
        if (location == 28)
        {
            for (int i = 0; i < smithyShop.Count; i++)
            {
                if (smithyShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (smithyShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        smithyShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (smithyShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            smithyShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            smithyShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                smithyShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        smithyShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //inn
        if (location == 31)
        {
            for (int i = 0; i < innShop.Count; i++)
            {
                if (innShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (innShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        innShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (innShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            innShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            innShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                innShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        innShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //factory
        if (location == 44)
        {
            for (int i = 0; i < factoryShop.Count; i++)
            {
                if (factoryShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (factoryShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        factoryShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (factoryShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            factoryShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            factoryShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                factoryShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        factoryShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //temple
        if (location == 47)
        {
            for (int i = 0; i < templeShop.Count; i++)
            {
                if (templeShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (templeShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        templeShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (templeShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            templeShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            templeShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                templeShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        templeShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //coven
        if (location == 16)
        {
            for (int i = 0; i < covenShop.Count; i++)
            {
                if (covenShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (covenShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        covenShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (covenShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            covenShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            covenShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                covenShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        covenShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //guildhouse
        if (location == 11)
        {
            for (int i = 0; i < guildhouseShop.Count; i++)
            {
                if (guildhouseShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (guildhouseShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        guildhouseShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (guildhouseShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            guildhouseShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            guildhouseShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                guildhouseShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        guildhouseShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        //cornville
        if (location == 1)
        {
            for (int i = 0; i < cornvilleShop.Count; i++)
            {
                if (cornvilleShop[i].GetComponent<Card>().numberInDeck == cardNumber)
                {
                    if (cornvilleShop[i].GetComponent<CardDisplay2>().quantity > 1)
                    {
                        cornvilleShop[i].GetComponent<CardDisplay2>().quantity -= 1;

                        if (cornvilleShop[i].GetComponent<CardDisplay2>().quantity > 1 ||
                            cornvilleShop[i].GetComponent<CardDisplay2>().showQuantityAlways == true)
                        {
                            cornvilleShop[i].GetComponent<CardDisplay2>().quantityText.text =
                                cornvilleShop[i].GetComponent<CardDisplay2>().quantity.ToString();
                        }
                        return;
                    }
                    else
                    {
                        cornvilleShop.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}
