using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreCanvasController : MonoBehaviour
{
    public List<GameObject> player1CharacterList;
    public List<GameObject> player2CharacterList;
    public List<GameObject> player3CharacterList;
    public List<GameObject> player4CharacterList;

    public GameObject player1Score;
    public GameObject player2Score;
    public GameObject player3Score;
    public GameObject player4Score;

    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI player3ScoreText;
    public TextMeshProUGUI player4ScoreText;

    //not used in v91
    //needs turnNumber and heronumber variables
    public void ShowCharacter (int turnNumber, int heroNumber, string heroName, string userName)
    {
        if(turnNumber == 0)
        {
            for (int i = 0; i < player1CharacterList.Count; i++)
            {
                if(i == heroNumber)
                {
                    player1CharacterList[i].gameObject.SetActive(true);
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText = "<b>" + heroName + " (" + userName + ")</b>";
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br><color=#FFD370>Level " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().characterLevel;
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br>Fame to levelup: " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().fameForNextLevel;
                }
                else
                {
                    player1CharacterList[i].gameObject.SetActive(false);
                }
            }
            player1Score.SetActive(true);

            GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().honorVisibleText = player1ScoreText;
        }
        if (turnNumber == 1)
        {
            for (int i = 0; i < player2CharacterList.Count; i++)
            {
                if (i == heroNumber)
                {
                    player2CharacterList[i].gameObject.SetActive(true);
                    player2CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText = "<b>" + heroName + " (" + userName + ")</b>";
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br><color=#FFD370>Level " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().characterLevel;
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br>Fame to levelup: " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().fameForNextLevel;
                }
                else
                {
                    player2CharacterList[i].gameObject.SetActive(false);
                }
            }
            player2Score.SetActive(true);

            GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().honorVisibleText = player2ScoreText;
        }
        if (turnNumber == 2)
        {
            for (int i = 0; i < player3CharacterList.Count; i++)
            {
                if (i == heroNumber)
                {
                    player3CharacterList[i].gameObject.SetActive(true);
                    player3CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText = "<b>" + heroName + " (" + userName + ")</b>";
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br><color=#FFD370>Level " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().characterLevel;
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br>Fame to levelup: " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().fameForNextLevel;

                }
                else
                {
                    player3CharacterList[i].gameObject.SetActive(false);
                }
            }
            player3Score.SetActive(true);

            GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().honorVisibleText = player3ScoreText;
        }
        if (turnNumber == 3)
        {
            for (int i = 0; i < player4CharacterList.Count; i++)
            {
                if (i == heroNumber)
                {
                    player4CharacterList[i].gameObject.SetActive(true);
                    player4CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText = "<b>" + heroName + " (" + userName + ")</b>";
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br><color=#FFD370>Level " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().characterLevel;
                    player1CharacterList[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br>Fame to levelup: " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().fameForNextLevel;

                }
                else
                {
                    player4CharacterList[i].gameObject.SetActive(false);
                }
            }
            player4Score.SetActive(true);

            GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().honorVisibleText = player4ScoreText;
        }
    }

    //new system for v91
    public void ShowCharacterInfo(int turnNumber, int heroNumber, string heroName, string userName)
    {
        //only update for you
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ItsYourTurn())
        {
            for (int i = 0; i < GameManager.ins.characterIcons.Count; i++)
            {
                if (i == heroNumber)
                {
                    GameManager.ins.characterIcons[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText = "<b>" + heroName + " (" + userName + ")</b>";
                    GameManager.ins.characterIcons[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br><color=#FFD370>Level " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().characterLevel;
                    GameManager.ins.characterIcons[i].gameObject.GetComponent<CharacterReputationDisplays>().playerInfoText += "<br>Fame to levelup: " + GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().fameForNextLevel;
                }
            }

            GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().honorVisibleText = GameManager.ins.references.fameText;
        }
    }
}
