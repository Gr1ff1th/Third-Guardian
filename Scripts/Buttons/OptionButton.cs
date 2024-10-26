using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionButton : MonoBehaviour
{
    public bool isPauseToggleButton;

    // Start is called before the first frame update
    void Start()
    {
        if (isPauseToggleButton == true)
        {
            //checks theres setting for this set alrdy
            if (PlayerPrefs.HasKey("PauseOn"))
            {
                if (PlayerPrefs.GetInt("PauseOn") == 0)
                {
                    gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "PAUSE ON PHASE CHANGE: <sprite=\"yes & no\" index=1>";
                }
                if (PlayerPrefs.GetInt("PauseOn") == 1)
                {
                    gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "PAUSE ON PHASE CHANGE: <sprite=\"yes & no\" index=3>";
                }
            }
            else
            {
                //1 is on, 0 is off
                PlayerPrefs.SetInt("PauseOn", 1);

                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "PAUSE ON PHASE CHANGE: <sprite=\"yes & no\" index=3>";
            }
        }
    }

    //toggles cmobat pause feature on / off
    public void TogglePauseFeature()
    {
        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene == 3)
        {
            GameManager.ins.toolTipBackground.SetActive(false);
        }

        //0 goes to 1, 1 goes to 0 
        //0 is off, 1 is on
        if (PlayerPrefs.GetInt("PauseOn") == 0)
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "PAUSE ON PHASE CHANGE: <sprite=\"yes & no\" index=3>";
            PlayerPrefs.SetInt("PauseOn", 1);
            return;
        }
        if (PlayerPrefs.GetInt("PauseOn") == 1)
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "PAUSE ON PHASE CHANGE: <sprite=\"yes & no\" index=1>";
            PlayerPrefs.SetInt("PauseOn", 0);
            return;
        }
    }
}
