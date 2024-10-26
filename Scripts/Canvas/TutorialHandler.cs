using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    public List<GameObject> tutorialPages;

    public int currentPage;
    public Text pageNumbers;

    public GameObject backButton;
    public GameObject nextButton;
    public GameObject finishButton;

    public GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateTutorialPlates()
    {
        currentPage = 1;

        //GameManager.ins.uiButtonHandler.CloseAllDisplays();
        GameManager.ins.uiButtonHandler.consoleFrame.SetActive(true);
        GameManager.ins.uiButtonHandler.consoleNamePlate.SetActive(false);
        GameManager.ins.uiButtonHandler.tutorialHolder.SetActive(true);

        for (int i = 0; i < tutorialPages.Count; i++)
        {
            if (currentPage - 1 == i)
            {
                tutorialPages[i].gameObject.SetActive(true);
            }
            else
            {
                tutorialPages[i].gameObject.SetActive(false);
            }
        }

        pageNumbers.gameObject.SetActive(true);
        pageNumbers.text = currentPage + "/" + tutorialPages.Count;

        nextButton.SetActive(true);
        backButton.SetActive(true);

        backButton.GetComponent<Button>().interactable = false;

    }

    public void NextPage()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        currentPage += 1;
        pageNumbers.text = currentPage + "/" + tutorialPages.Count;

        for(int i = 0; i < tutorialPages.Count; i++)
        {
            if(currentPage - 1 == i)
            {
                tutorialPages[i].gameObject.SetActive(true);
            }
            else
            {
                tutorialPages[i].gameObject.SetActive(false);
            }
        }

        if (currentPage > 1)
        {
            backButton.GetComponent<Button>().interactable = true;
        }

        if (currentPage == tutorialPages.Count)
        {
            backButton.GetComponent<Button>().interactable = true;
            nextButton.SetActive(false);
            finishButton.SetActive(true);
        }
    }

    public void PreviousPage()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        currentPage -= 1;
        pageNumbers.text = currentPage + "/" + tutorialPages.Count;

        for (int i = 0; i < tutorialPages.Count; i++)
        {
            if (currentPage - 1 == i)
            {
                tutorialPages[i].gameObject.SetActive(true);
            }
            else
            {
                tutorialPages[i].gameObject.SetActive(false);
            }
        }

        if (currentPage == 1)
        {
            backButton.GetComponent<Button>().interactable = false;
        }

        if (currentPage < tutorialPages.Count)
        {
            nextButton.SetActive(true);
            finishButton.SetActive(false);
        }
    }

    public void FinishButton()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        PhotonRoom.room.CloseGame();
    }
}
