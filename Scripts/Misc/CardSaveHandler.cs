using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSaveHandler : MonoBehaviour
{
    //perk deck
    //note that this needs to contain identical list to the one at main scene GameManager
    public List<GameObject> generalDeck = new List<GameObject>();

    //arrays of each characters starting cards
    public int[] berenCards;
    public int[] sulimanCards;
    public int[] dazzleCards;
    public int[] maximusCards;
    public int[] melissyaCards;
    public int[] targasCards;
    public int[] naomiCards;
    public int[] arielCards;
    public int[] enigmaCards;
    public int[] rimlicCards;

    // Start is called before the first frame update
    void Start()
    {
        CardReset();
    }

    public void CardReset()
    {
        for (int i = 0; i < generalDeck.Count; i++)
        {
            generalDeck[i].GetComponent<Card>().isTaken = false;
            generalDeck[i].GetComponent<Card>().numberInDeck = i;
        }
    }
}
