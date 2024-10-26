using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dunno if needed, selectorscript2 is doing this atm
public class MenuController : MonoBehaviour
{

    public void OnClickCharacterPick(int whichCharacter)
    {
        if(PlayerInfo.PI != null)
        {
            PlayerInfo.PI.mySelectedCharacter = whichCharacter;
            PlayerPrefs.SetInt("MyCharacter", whichCharacter);
        }
    }
}
