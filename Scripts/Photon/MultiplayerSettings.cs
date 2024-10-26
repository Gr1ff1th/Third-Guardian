using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings multiplayerSettings;

    //if true, its delayed start game
    //is not set to true anywhere atm
    public bool delayStart;
    
    public int maxPlayers;
    public int menuScene;
    public int multiplayerScene;

    private void Awake()
    {
        //delayStart = true;

        if (MultiplayerSettings.multiplayerSettings == null)
        {
            MultiplayerSettings.multiplayerSettings = this;
        }
        else
        {
            if(MultiplayerSettings.multiplayerSettings != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
