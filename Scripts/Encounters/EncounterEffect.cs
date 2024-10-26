using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for randomized encounter effects
//for treasures etc
public class EncounterEffect : MonoBehaviour
{
    //played when the effect takes place
    public AudioClip sfx;

    //used in case theres special icon for the effect
    public Sprite icon;

    //chance of drawing this effect (in percentages)
    public int probability;

    //effect per chosen option:
    //0= energy, 1= arcane dust, 2= upgrade points, 4= coins, 5= fame, 6= favor
    public int[] effectType;
    public int[] effectQty;

    //used for skill rewards, curses? etc
    public int[] specialEffect;

    //used for item rewards, abilities (stuff from general deck, uses the card number exactly)
    public int[] rewardType;
    public int[] rewardQty;

    //check this to use fail continue buttons on NoSkillChecks
    public bool useFailContinueButtons;

    //for disallowing secondary rewards
    public bool dontAllowSecondaryEffect;

    //text displayed when the effect is chosen
    //starts after the main encounter button effect text
    [TextArea(5, 20)]
    public string effectText;
}
