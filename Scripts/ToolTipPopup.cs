using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//not used atm
public class ToolTipPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public ToolTipText tooltipText;
    /*
    public string theText;

    void Awake()
    {
        Text txtTransfer = theText;
    }
    */

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipText.DisplayWindow();
        Debug.Log("Pointer is Down");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipText.HideWindow();
        Debug.Log("Pointer is up");
    }
}
