using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CharacterReputationDisplays : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //gives info on character name & username
    public string playerInfoText;

    public GameObject toolTipBackground;
    public TextMeshProUGUI toolTipText;

    // Start is called before the first frame update
    void Start()
    {
        //pretty silly way of getting the references
        toolTipBackground = GameManager.ins.toolTipBackground;
        toolTipText = GameManager.ins.toolTipText;

        //playerInfoText = "Beren (GG)";
    }

    //
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        toolTipBackground.SetActive(true);
        toolTipText.text = playerInfoText.ToString();
    }

    //
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        toolTipText.text = "";
        toolTipBackground.SetActive(false);
    }
}
