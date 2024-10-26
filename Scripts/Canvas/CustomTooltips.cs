using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CustomTooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //gives info on character name & username
    public string customText;

    public GameObject toolTipBackground;
    public TextMeshProUGUI toolTipText;

    public string threatLevel;
    public int levelRequirement;
    public bool lightstoneRequirement;

    public bool isHighScoreTooltip;
    public int highScoreHeroNumber;
    public TextMeshProUGUI highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        //pretty silly way of getting the references
        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene > 2)
        {
            toolTipBackground = GameManager.ins.toolTipBackground;
            toolTipText = GameManager.ins.toolTipText;
        }
        else
        {
            toolTipBackground = GameObject.Find("TooltipCanvas").GetComponent<ToolTipCanvas>().tooltipBackground;
            toolTipText = toolTipBackground.GetComponentInChildren<TextMeshProUGUI>();

            if(isHighScoreTooltip == true)
            {
                //could set the tooltip and actual display separately?
                SetHighScore();
                SetHighScoreTooltip();
            }
        }
    }

    //
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        toolTipBackground.SetActive(true);
        toolTipText.text = customText.ToString();
    }

    //
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        toolTipText.text = "";
        toolTipBackground.SetActive(false);
    }

    public void SetHighScoreTooltip()
    {
        //beren
        if(highScoreHeroNumber == 0)
        {
            customText = "<b>Beren</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[0] + "<sprite index=3></b> ";
            
            if (DataPersistenceManager.instance.gameData.highScores[0] > 0)
            {
                customText += GetVersionNumber(0);
            }

            if (DataPersistenceManager.instance.gameData.highScores[10] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[10] + "<sprite index=3></b> " + GetVersionNumber(10);
            }
            if (DataPersistenceManager.instance.gameData.highScores[20] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[20] + "<sprite index=3></b> " + GetVersionNumber(20);
            }
        }
        //suliman
        if (highScoreHeroNumber == 1)
        {
            customText = "<b>Suliman</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[1] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[1] > 0)
            {
                customText += GetVersionNumber(1);
            }

            if (DataPersistenceManager.instance.gameData.highScores[11] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[11] + "<sprite index=3></b> " + GetVersionNumber(11);
            }
            if (DataPersistenceManager.instance.gameData.highScores[21] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[21] + "<sprite index=3></b> " + GetVersionNumber(21);
            }
        }
        //dazzle
        if (highScoreHeroNumber == 2)
        {
            customText = "<b>Dazzle</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[2] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[2] > 0)
            {
                customText += GetVersionNumber(2);
            }

            if (DataPersistenceManager.instance.gameData.highScores[12] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[12] + "<sprite index=3></b> " + GetVersionNumber(12);
            }
            if (DataPersistenceManager.instance.gameData.highScores[22] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[22] + "<sprite index=3></b> " + GetVersionNumber(22);
            }
        }
        //magnus
        if (highScoreHeroNumber == 3)
        {
            customText = "<b>Magnus</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[3] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[3] > 0)
            {
                customText += GetVersionNumber(3);
            }

            if (DataPersistenceManager.instance.gameData.highScores[13] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[13] + "<sprite index=3></b> " + GetVersionNumber(13);
            }
            if (DataPersistenceManager.instance.gameData.highScores[23] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[23] + "<sprite index=3></b> " + GetVersionNumber(23);
            }
        }
        //melissya
        if (highScoreHeroNumber == 4)
        {
            customText = "<b>Melissya</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[4] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[4] > 0)
            {
                customText += GetVersionNumber(4);
            }

            if (DataPersistenceManager.instance.gameData.highScores[14] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[14] + "<sprite index=3></b> " + GetVersionNumber(14);
            }
            if (DataPersistenceManager.instance.gameData.highScores[24] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[24] + "<sprite index=3></b> " + GetVersionNumber(24);
            }
        }
        //targas
        if (highScoreHeroNumber == 5)
        {
            customText = "<b>Targas</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[5] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[5] > 0)
            {
                customText += GetVersionNumber(5);
            }

            if (DataPersistenceManager.instance.gameData.highScores[15] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[15] + "<sprite index=3></b> " + GetVersionNumber(15);
            }
            if (DataPersistenceManager.instance.gameData.highScores[25] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[25] + "<sprite index=3></b> " + GetVersionNumber(25);
            }
        }
        //naomi
        if (highScoreHeroNumber == 6)
        {
            customText = "<b>Naomi</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[6] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[6] > 0)
            {
                customText += GetVersionNumber(6);
            }
            if (DataPersistenceManager.instance.gameData.highScores[16] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[16] + "<sprite index=3></b> " + GetVersionNumber(16);
            }
            if (DataPersistenceManager.instance.gameData.highScores[26] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[26] + "<sprite index=3></b> " + GetVersionNumber(26);
            }
        }
        //ariel
        if (highScoreHeroNumber == 7)
        {
            customText = "<b>Ariel</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[7] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[7] > 0)
            {
                customText += GetVersionNumber(7);
            }
            if (DataPersistenceManager.instance.gameData.highScores[17] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[17] + "<sprite index=3></b> " + GetVersionNumber(17);
            }
            if (DataPersistenceManager.instance.gameData.highScores[27] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[27] + "<sprite index=3></b> " + GetVersionNumber(27);
            }
        }
        //Leopold
        if (highScoreHeroNumber == 8)
        {
            customText = "<b>Leopold</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[8] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[8] > 0)
            {
                customText += GetVersionNumber(8);
            }
            if (DataPersistenceManager.instance.gameData.highScores[18] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[18] + "<sprite index=3></b> " + GetVersionNumber(18);
            }
            if (DataPersistenceManager.instance.gameData.highScores[28] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[28] + "<sprite index=3></b> " + GetVersionNumber(28);
            }
        }
        //rimlic
        if (highScoreHeroNumber == 9)
        {
            customText = "<b>Rimlic</b><br>Best score:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[9] + "<sprite index=3></b> ";

            if (DataPersistenceManager.instance.gameData.highScores[9] > 0)
            {
                customText += GetVersionNumber(9);
            }

            if (DataPersistenceManager.instance.gameData.highScores[19] > 0)
            {
                customText += "<br>2nd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[13] + "<sprite index=3></b> " + GetVersionNumber(19);
            }
            if (DataPersistenceManager.instance.gameData.highScores[29] > 0)
            {
                customText += "<br>3rd best:<br>  <b>" + DataPersistenceManager.instance.gameData.highScores[23] + "<sprite index=3></b> " + GetVersionNumber(29);
            }
        }
    }

    public string GetVersionNumber(int index)
    {
        if(DataPersistenceManager.instance.gameData.highScoreForVersion[index] == 0)
        {
            return "(pre v1.0.0.)";
        }

        if (DataPersistenceManager.instance.gameData.highScoreForVersion[index] == 1)
        {
            return "(v1.0.0.)";
        }

        return "(pre v1.0.0.)";
    }

    public void SetHighScore()
    {
        /*
        if (highScoreHeroNumber == 0)
        {
            highScoreText.text = DataPersistenceManager.instance.gameData.highScores[0].ToString();
        }
        */
        highScoreText.text = DataPersistenceManager.instance.gameData.highScores[highScoreHeroNumber].ToString();
    }
}
