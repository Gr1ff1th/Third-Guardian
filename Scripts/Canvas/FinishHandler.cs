using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class FinishHandler : MonoBehaviour
{
    public GameObject endScorePanel;
    public GameObject scorePrompt;
    public GameObject positionsPrompt;

    public GameObject defeatPrompt;

    public TextMeshProUGUI scoreDisclaimerText;
    public TextMeshProUGUI positionsDisclaimerText;

    //score variables
    public int followerScore;
    public int agentScore;
    public int coinScore;
    public int artifactScore;
    public int fameScore;
    public int totalScore;
    public int winnersTurnNumber;
    public int yourTurnNumber;

    public Button positionsOkButton;

    private void Start()
    {
        scoreDisclaimerText.text = "";
        followerScore = 0;
        agentScore = 0;
        coinScore = 0;
        artifactScore = 0;
        fameScore = 0;
        totalScore = 0;
        winnersTurnNumber = 0;
        yourTurnNumber = 0;
    }

    public void CalculateScore()
    {
        //opens the score display
        endScorePanel.SetActive(true);
        scorePrompt.SetActive(true);
        positionsPrompt.SetActive(false);

        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            if (GameManager.ins.avatars[i].GetComponent<CharController>().ItsYourTurn() == true)
            {
                int totalFollowers = GameManager.ins.avatars[i].GetComponentInChildren<Character>().warriors + GameManager.ins.avatars[i].GetComponentInChildren<Character>().artisans + GameManager.ins.avatars[i].GetComponentInChildren<Character>().arcanists;
                followerScore = totalFollowers / 2;

                //agentScore = GameManager.ins.avatars[i].GetComponentInChildren<Character>().agents;

                coinScore = GameManager.ins.avatars[i].GetComponentInChildren<Character>().coins / 10;

                CountArtifactScore(i);

                fameScore = GameManager.ins.avatars[i].GetComponentInChildren<Character>().fame;

                totalScore = followerScore + agentScore + coinScore + artifactScore + fameScore;

                //set total score via rpc call
                GameManager.ins.avatars[i].GetComponent<CharController>().PV.RPC("RPC_SetTotalScore", RpcTarget.AllBufferedViaServer, i, totalScore);

                //dont print the info for ai players
                if (GameManager.ins.avatars[i].GetComponent<CharController>().isAi == false)
                {
                    //the wall of text
                    scoreDisclaimerText.text = "<size=22><color=yellow><b>Final Score:</b></color></size>\n<size=20></size>\n" +
                        totalFollowers + "<sprite index=20>  =  " + followerScore + "<sprite index=12>" + "\n<size=8>\n</size>" + //GameManager.ins.avatars[i].GetComponentInChildren<Character>().agents + " <sprite index=22>  =  " + agentScore + " <sprite index=14>" + "\n<size=8>\n</size>" +
                        GameManager.ins.avatars[i].GetComponentInChildren<Character>().coins + "<sprite index=13>  =  " + coinScore + "<sprite index=12>" + "\n<size=8>\n</size>" +
                        "<sprite index=47>  =  " + artifactScore + "<sprite index=12>" + "\n<size=8>\n</size>" +
                        fameScore + " <sprite index=12>" + "\n<size=12>\n_____________________\n<color=yellow></size>\n" +
                        "<b>Total = " + totalScore + "</b><sprite index=12></color>";
                }
            }
        }

        //calculate the final positions in advance
        Invoke("FinalPositions", 0.6f);
    }

    //calculates final positioning

    void FinalPositions()
    {
        //calculate winner
        int highScore = 0;
        winnersTurnNumber = 0;

        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            if (GameManager.ins.avatars[i].GetComponent<CharController>().totalScore > highScore)
            {
                highScore = GameManager.ins.avatars[i].GetComponent<CharController>().totalScore;
                winnersTurnNumber = i;
            }
        }

        //lets calculate the final "election" table now too (in case some players disconnect quickly)
        positionsDisclaimerText.text = "<size=22><color=yellow><b>Election:</b></color></size>\n<size=20></size>\n";

        for (int i = 0; i < GameManager.ins.avatars.Count; i++)
        {
            if (GameManager.ins.avatars[i].GetComponent<CharController>().ItsYourTurn() == true && GameManager.ins.avatars[i].GetComponent<CharController>().isAi == false)
            {
                positionsDisclaimerText.text += "<color=yellow>";

                if (winnersTurnNumber == i)
                {
                    positionsDisclaimerText.text += "<sprite index=3>Septimus " + GameManager.ins.avatars[i].GetComponentInChildren<Character>().heroName + 
                        "  =  " + GameManager.ins.avatars[i].GetComponent<CharController>().totalScore + " <sprite index=12>";
                }

                if (winnersTurnNumber != i)
                {
                    positionsDisclaimerText.text += "Councillor " + GameManager.ins.avatars[i].GetComponentInChildren<Character>().heroName +
                        "  =  " + GameManager.ins.avatars[i].GetComponent<CharController>().totalScore + " <sprite index=12>";
                }
                positionsDisclaimerText.text += "</color><size=8>\n</size>";

                yourTurnNumber = i;
            }

            if (GameManager.ins.avatars[i].GetComponent<CharController>().ItsYourTurn() == false || GameManager.ins.avatars[i].GetComponent<CharController>().isAi == true)
            {
                if (winnersTurnNumber == i)
                {
                    positionsDisclaimerText.text += "<sprite index=3> Septimus " + GameManager.ins.avatars[i].GetComponentInChildren<Character>().heroName +
                        "  =  " + GameManager.ins.avatars[i].GetComponent<CharController>().totalScore + " <sprite index=12>";
                }

                if (winnersTurnNumber != i)
                {
                    positionsDisclaimerText.text += "Councillor " + GameManager.ins.avatars[i].GetComponentInChildren<Character>().heroName +
                        "  =  " + GameManager.ins.avatars[i].GetComponent<CharController>().totalScore + " <sprite index=12>";
                }

                positionsDisclaimerText.text += "<size=8>\n</size>";

                //dont need turnnumber variable here
            }
        }
    }

    public void ScoreOkButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //opens the election display
        scorePrompt.SetActive(false);
        positionsPrompt.SetActive(true);

        Invoke("PlayEndSfx", 0.3f);

        positionsOkButton.interactable = false;

        Invoke("ReEnablePosOk", 2.0f);
    }

    void ReEnablePosOk()
    {
        positionsOkButton.interactable = true;
    }

    void PlayEndSfx()
    {
        //actually need to do this another way
        if(winnersTurnNumber == yourTurnNumber)
        {
            //play sfx
            Invoke("WinSound1", 0.0f);
            Invoke("WinSound2", 1.0f);
            Invoke("WinSound1", 2.0f);
        }

        if (winnersTurnNumber != yourTurnNumber)
        {
            //play sfx
            GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionFail();
        }
    }

    void WinSound1()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess1();
    }
    void WinSound2()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayElectionSuccess2();
    }

    //returns to main menu
    public void PositionsOkButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        PhotonRoom.room.CloseGame();
    }

    public void CountArtifactScore(int turnNumber)
    {
        //sum of artifact values belonging to you
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().value;
                }
            }
        }

        //Purse of isolore check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 43 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().coins / 10;
                }
            }
        }

        //mask of quiron check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 44 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().fame / 2;
                }
            }
        }

        //champions ruby check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 45 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().warriors / 3;
                }
            }
        }

        //masters topaz check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 46 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().artisans / 3;
                }
            }
        }

        //mystics amethyst check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 47 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().arcanists / 3;
                }
            }
        }

        //thieves hood check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 48 &&
                GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    artifactScore += GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().agents;
                }
            }
        }

        //artificers tome check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 49 && 
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    //double loop here to check all the artifacts belonging to the player with the tome
                    for (int y = 0; y < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; y++)
                    {
                        if (GameManager.ins.artifactCardArea.transform.GetChild(y).gameObject.GetComponent<ArtifactCard>() != null)
                        {
                            if (GameManager.ins.artifactCardArea.transform.GetChild(y).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                            {
                                artifactScore += 1;
                            }
                        }
                    }
                }
            }
        }

        //guildmasters tome check
        for (int i = 0; i < GameManager.ins.artifactCardArea.GetComponent<Transform>().childCount; i++)
        {
            if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>() != null)
            {
                if (GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<ArtifactCard>().effect == 50 &&
                    GameManager.ins.artifactCardArea.transform.GetChild(i).gameObject.GetComponent<CardDisplay>().belongsTo == turnNumber)
                {
                    int totalFollowers = GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().warriors + 
                        GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().artisans + 
                        GameManager.ins.avatars[turnNumber].GetComponentInChildren<Character>().arcanists;
                    artifactScore += totalFollowers / 2;
                }
            }
        }
    }

}
